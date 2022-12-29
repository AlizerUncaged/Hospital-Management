using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Hospital_Management.Models
{
    [Table("DentistsTable")]
    public class Dentist : IdentityUser
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public string Birthdate { get; set; }

        public string Gender { get; set; }

        public string CellphoneNumber { get; set; }

        public string LicenseNumber { get; set; }

        public List<AppointmentModel> Appointments { get; set; }
    }
}