namespace Weather.Abstractions
{
    public class WeatherResult
    {
        public string Location { get; set; }
        public float? TempCelsius { get; set; }
        public string Description { get; set; }
        public float? Pressure { get; set; }
        public float? Humidity { get; set; }
    }
}
