using Binance.Net;
using Binance.Net.Objects;
using CryptocurrencyValuePrediction;
using CryptocurrencyValuePrediction.Repositories;
using CryptoExchange.Net;
using CryptoExchange.Net.Authentication;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.TimeSeries;
using Spectre.Console;
using System.Data;


var context = new ApplicationContext();
var predictionsRepository = new PredictionInformationRepository(context);
var modelErrorRepository = new PredictionModelErrorRepository(context);

var client = new BinanceClient(new BinanceClientOptions()
{
    ApiCredentials = new ApiCredentials("key", "secret")
});

string dataPath = "BTC-USD5.csv";
var mlContext = new MLContext(seed: 0);
var predictor = await GetPredictionEngine(mlContext);
float[] lastPredictions = new float[] { 0 };


while (true)
{
    var callResult = await client.Spot.Market.GetPriceAsync("BTCBUSD");

    DataHelper.WriteToDataFile(dataPath, $"{DateTimeOffset.UtcNow},{callResult.Data.Price.Normalize().ToString().Replace(',', '.')}");

    if (!callResult.Success)
    {
        Console.WriteLine("Error during getting price");
        break;
    }
    else
    {
        var prediction = predictor.Predict(new CoinPrice { Price = (float)callResult.Data.Price });
        await predictionsRepository.AddEntityAsync(new PredictionInformation() 
        {
            ID = Guid.NewGuid(),
            ActualValue = (float)callResult.Data.Price,
            Name = "BTCBUSD",
            Predictions = string.Join("; ", prediction.Score),
            Date = DateTimeOffset.UtcNow
        });

        var consoleColor = (Math.Abs((float)callResult.Data.Price - lastPredictions[0]) < (float)callResult.Data.Price / 100 ) ? "[green]" : "[red]";
        var row = $"{consoleColor + callResult.Data.Price + "[/]"} " + String.Join(' ', prediction.Score);

        predictor.CheckPoint(mlContext, "TrainedModel");

        lastPredictions = prediction.Score;

        AnsiConsole.MarkupLine(row);

        await Task.Delay(60000 * 60 * 24);
    }
}


async Task<TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction>> GetPredictionEngine(MLContext mlContext)
{


    if (File.Exists("TrainedModel"))
    {
        return GetExistingEngine(mlContext);
    }

    return await CreateEngineAsync(mlContext);
}

TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction> GetExistingEngine(MLContext mLContext) 
{
    // Load the forecast engine that has been previously saved in bin folder.
    ITransformer forecaster;
    using (var file = File.OpenRead("TrainedModel"))
    {
        forecaster = mLContext.Model.Load(file, out DataViewSchema schema);
    }

    // creating a new prediction engine from the persisted model.
    return forecaster.CreateTimeSeriesEngine<CoinPrice, CoinPricePrediction>(mLContext);
}

async Task<TimeSeriesPredictionEngine<CoinPrice, CoinPricePrediction>> CreateEngineAsync(MLContext mLContext)
{
    var YahooData = DataHelper.GetYahooData();
    var BinanceData = DataHelper.GetBinanceData();
    var resultData = DataHelper.MergeData(BinanceData, YahooData);

    IDataView baseTrainingDataView = mLContext.Data.LoadFromEnumerable<CoinPrice>(YahooData);

    var trainer = mLContext.Forecasting.ForecastBySsa(
        outputColumnName: nameof(CoinPricePrediction.Score),
        inputColumnName: nameof(CoinPrice.Price),
        windowSize: 2,
        seriesLength: YahooData.Count,
        trainSize: YahooData.Count,
        horizon: 5,
        confidenceLevel: 0.95f,
        confidenceLowerBoundColumn: "MinPred",
        confidenceUpperBoundColumn: "MaxPred");

    ITransformer trainedModel = trainer.Fit(baseTrainingDataView);

    await Eval(baseTrainingDataView, trainedModel, mLContext);

    return trainedModel.CreateTimeSeriesEngine<CoinPrice, CoinPricePrediction>(mLContext);

}



async Task Eval(IDataView testData, ITransformer model, MLContext mlCon)
{
    IDataView pred = model.Transform(testData);

    IEnumerable<float> actualVal = testData.GetColumn<float>("Price");  

    IEnumerable<float> forVal =
        mlCon.Data.CreateEnumerable<CoinPricePrediction>(pred, false)
            .Select(prediction => prediction.Score.Average());

 var diff = actualVal.Zip(forVal, (actualVal, forVal) => actualVal -
 forVal);

    var mae = diff.Average(error => Math.Abs(error));

    var rmse = Math.Sqrt(diff.Average(error => Math.Pow(error, 2)));

    await modelErrorRepository.AddEntityAsync(new PredictionModelError() 
    {
        ID = Guid.NewGuid(),
        Name = "BTCBUSD",
        AbsoluteError = mae,
        RootMeanSquaredError = rmse,
        Date = DateTimeOffset.UtcNow
    });

    Console.WriteLine($"Mean Absolute Error: {mae:F3}");
    Console.WriteLine($"Root Mean Squared Error: {rmse:F3}");
}
