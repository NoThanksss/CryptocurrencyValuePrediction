using Microsoft.ML.Data;

namespace CryptocurrencyValuePrediction
{
    public class CoinPricePrediction
    {
        [ColumnName("Score")]
        public float[] Score;
        public float[] MinPred { get; set; }
        public float[] MaxPred { get; set; }
    }
}
