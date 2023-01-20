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
        
        public string? Tag { get; set; }
        

        public List<Chat> Chats { get; set; } = new();

        public DateTime RegisterDate { get; set; } = DateTime.Now;
    }
}