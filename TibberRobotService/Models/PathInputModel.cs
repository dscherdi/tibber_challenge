using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TibberRobotService.Models
{
    public class PathInputModel
    {
        [Required]
        [JsonPropertyName("start")]
        public Coordinate Start { get; set; }
        
        [Required]
        [MaxLength(10000)]
        [JsonPropertyName("commands")]
        public List<Command> Commands { get; set; }
    }

    public class Coordinate
    {
        [Range(-100000, 100000)]
        [JsonPropertyName("x")]
        public int X { get; set; }

        [Range(-100000, 100000)]
        [JsonPropertyName("y")]
        public int Y { get; set; }
    }

    public class Command
    {
        [Required]
        [RegularExpression("^(north|east|south|west)$", ErrorMessage = "Direction must be north, east, south, or west.")]
        [JsonPropertyName("direction")]
        public string Direction { get; set; }
        
        [Range(1, 99999)]
        [JsonPropertyName("steps")]
        public int Steps { get; set; }
    }
}
