using System.ComponentModel.DataAnnotations;

namespace CryptocurrencyValuePrediction
{
    public class PredictionModelError
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public double AbsoluteError { get; set; }
        public double RootMeanSquaredError { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
