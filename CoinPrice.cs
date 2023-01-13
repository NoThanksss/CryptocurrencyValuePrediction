using Microsoft.ML.Data;

namespace CryptocurrencyValuePrediction
{
    public class CoinPrice
    {
        [LoadColumn(0)]
        public float Price { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}
