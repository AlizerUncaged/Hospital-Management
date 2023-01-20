using System.ComponentModel.DataAnnotations;

namespace Hospital_Management.Models
{
    public class AppointmentModel
    {
        [Key] public int? AppointmentID { get; set; }

        public DateTime? DateCreated { get; set; } = DateTime.Now;

        public string? Time { get; set; }
        
        public string? Date { get; set; }

        public Dentist? Dentist { get; set; }

        public Patient? Patient { get; set; }


        public double? TotalPrice { get; set; }

        /// <summary>
        /// Comma delimited services.
        /// </summary>
        public string? Services { get; set; }

        public string? Note { get; set; }

        public bool IsCancelled { get; set; } = false;
    }
}