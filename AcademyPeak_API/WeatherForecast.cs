﻿namespace AcademyPeak_API
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.55);
        public string? Summary { get; set; }
    }
}
