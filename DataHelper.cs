using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace CryptocurrencyValuePrediction
{
    public class DataHelper
    {
        public static List<CoinPrice> GetYahooData()
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, @"..\..\..\" + "BTC-USD5.csv");
            string sFilePath = Path.GetFullPath(sFile);

            using (var reader = new StreamReader(sFilePath))
            {
                //Load training data in list   
                List<CoinPrice> CryptoPrices = new List<CoinPrice>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cruptocurrecnyValue = line.Split(',')[1];
                    var date = line.Split(',')[0];
                    var coinPrice = new CoinPrice()
                    {
                        Price = float.Parse(cruptocurrecnyValue, CultureInfo.InvariantCulture),
                        Name = "BitCoin",
                        Date = DateTime.Parse(date)
                    };

                    CryptoPrices.Add(coinPrice);
                }
                return CryptoPrices;
            }
        }

        public static List<CoinPrice> GetBinanceData()
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, @"..\..\..\" + "BTCUSDT_Binance.csv");
            string sFilePath = Path.GetFullPath(sFile);

            using (var reader = new StreamReader(sFilePath))
            {
                //Load training data in list   
                List<CoinPrice> CryptoPrices = new List<CoinPrice>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var cruptocurrecnyValue = line.Split(',')[3];
                    var date = line.Split(',')[1];
                    var coinPrice = new CoinPrice()
                    {
                        Price = float.Parse(cruptocurrecnyValue, CultureInfo.InvariantCulture),
                        Name = "BitCoin",
                        Date = DateTime.Parse(date)
                    };

                    CryptoPrices.Add(coinPrice);
                }
                CryptoPrices.Reverse();

                return CryptoPrices;
            }
        }

        public static List<CoinPrice> MergeData(List<CoinPrice> data1, List<CoinPrice> data2) 
        {
            var result = new List<CoinPrice>();
            data1.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
            data2.Sort((x, y) => DateTime.Compare(x.Date, y.Date));

            var recordsToSkip = RecordsToSkip(data1, data2);
            result.AddRange(data1.Take(recordsToSkip));
            data1.RemoveRange(0, recordsToSkip);

            for (int i = 0; i < data1.Count(); i++)
            {
                result.Add(new CoinPrice() 
                {
                    Date = data1[i].Date,
                    Name= data1[i].Name,
                    Price= (data1[i].Price + data2[i].Price)/2
                });
            }

            return result;
            
        }

        private static int RecordsToSkip(List<CoinPrice> data1, List<CoinPrice> data2)
        {
            return data1.FindIndex(x => x.Date == data2[0].Date);
        }

        public static void WriteToDataFile(string path, string record)
        {
            string sCurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string sFile = Path.Combine(sCurrentDirectory, @"..\..\..\" + path);
            string sFilePath = Path.GetFullPath(sFile);

            var configPersons = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false
            };
            using (var stream = File.Open(sFilePath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, configPersons))
            {
                csv.WriteField(record, false);
                csv.NextRecord();
            }
        }
    }
}
