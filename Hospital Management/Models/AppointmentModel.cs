using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class AppointmentModel
    {
        [Key] public int? AppointmentID { get; set; }
        
        public DateTime? DateCreated { get; set; } = DateTime.Now;

        [DataType(DataType.Time)] public DateTime? Time { get; set; } = DateTime.Now;

        public Dentist? Dentist { get; set; }

        public Patient? Patient { get; set; }

        public double? Income { get; set; }

        public bool? IsPasta { get; set; }

        public int? ExtractedTooth { get; set; }

        public string? Note { get; set; }

        public bool IsCancelled { get; set; } = false;
    }
}