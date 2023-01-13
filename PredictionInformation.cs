using System.ComponentModel.DataAnnotations;

namespace CryptocurrencyValuePrediction
{
    public class PredictionInformation
    {

        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Predictions { get; set; }
        public float ActualValue { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
