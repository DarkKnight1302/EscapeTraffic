﻿using System.Text.Json.Serialization;

namespace TrafficEscape.Models
{
    public class PlacePrediction
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }

        public string Description { get; set; }

        public string[] types { get; set; }
    }
}
