using System;
using System.ComponentModel.DataAnnotations;

namespace TibberRobotService.Models
{
    public class Execution
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int Commands { get; set; }
        public double Duration { get; set; }
        public int Result { get; set; }
    }
}