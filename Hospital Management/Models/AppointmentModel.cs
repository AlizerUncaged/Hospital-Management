using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class AppointmentModel
    {
        [Key] public int? AppointmentId { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;

        [DataType(DataType.Date)] public DateTime? Date { get; set; }

        public Dentist? Dentist { get; set; } = null;

        public Patient? Patient { get; set; }

        public string MessageToPatient { get; set; } = string.Empty;

        public double? TotalPrice { get; set; }

        /// <summary>
        /// Comma delimited services.
        /// </summary>
        public string Services { get; set; } = string.Empty;

        public string? Note { get; set; }

        public bool IsCancelled { get; set; } = false;
    }
}