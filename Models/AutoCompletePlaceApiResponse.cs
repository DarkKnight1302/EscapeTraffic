namespace TrafficEscape.Models
{
    public class AutoCompletePlaceApiResponse
    {
        public IEnumerable<PlacePrediction> predictions { get; set; }

        public string status { get; set; }
    }
}
