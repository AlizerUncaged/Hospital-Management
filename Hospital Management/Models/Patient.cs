using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Hospital_Management.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class Patient : IdentityUser
    {
        public bool Blocked { get; set; }

        public virtual List<AppointmentModel> Appointments { get; set; } = new List<AppointmentModel>();

        public string Name { get; set; }

        public string FullName => UserName;

        public string Address { get; set; }

        public string Birthdate { get; set; }

        public string Gender { get; set; }

        public string Guardian { get; set; }

        public string CellphoneNumber { get; set; }
        

        public List<Chat> Chats { get; set; } = new();


        public bool? IsBraced { get; set; }
        public bool? IsPasta { get; set; }

        public string? FullInfo { get; set; }

        public int? ToothExtracted { get; set; }
        public bool? IsToothExtract { get; set; }

        public DateTime RegisterDate { get; set; } = DateTime.Now;
    }
}