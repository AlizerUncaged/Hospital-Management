using System.Diagnostics;
using System.Security.Claims;
using Hospital_Management.Data;
using Microsoft.AspNetCore.Mvc;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RoleCreation _roleCreation;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IWebHostEnvironment _environment;

    public HomeController(ILogger<HomeController> logger, RoleCreation roleCreation, ApplicationDbContext dbContext,
        UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
    {
        _logger = logger;
        _roleCreation = roleCreation;
        _dbContext = dbContext;
        _userManager = userManager;
        _environment = environment;
    }


    [HttpGet("/clearCookies")]
    public async Task<IActionResult> ClearCookies()
    {
        foreach (var cookie in HttpContext.Request.Cookies)
        {
            HttpContext.Response.Cookies.Delete(cookie.Key);
        }

        return Content("Cookies Cleared");
    }


    [HttpGet("/doctor-appointment")]
    public async Task<IActionResult> DoctorAppointment()
    {
        return View();
    }

    [HttpGet("/patients/edit/{id}")]
    public async Task<IActionResult> PatientEditor(string id)
    {
        ViewData["Patient"] =
            await _dbContext.Patients.Include(x => x.Appointments).FirstOrDefaultAsync(x => x.Id == id);

        return View(ViewData["Patient"]);
    }

    [HttpPost("/patients/edit")]
    public async Task<IActionResult> PatientEditor(Patient patient, [FromForm] IFormFile? image,
        [FromForm] IFormFile? prescriptionImage)
    {
        var samePatient = await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == patient.Id);
        samePatient.Name = patient.Name;
        samePatient.Address = patient.Address;
        samePatient.Birthdate = patient.Birthdate;
        samePatient.CellphoneNumber = patient.CellphoneNumber;
        samePatient.Email = patient.Email;
        samePatient.Guardian = patient.Guardian;
        samePatient.PulseRate = patient.PulseRate;
        samePatient.BloodPressure = patient.BloodPressure;
        samePatient.Allergy = patient.Allergy;

        if (image is { })
        {
            samePatient.Image = $"/images/clients/{image.FileName}";

            {
                var path = Path.Combine(_environment.WebRootPath, "images", "clients");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, image.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                    stream.Close();
                }
            }
        }

        if (prescriptionImage is { })
        {
            samePatient.PrescriptionImage = $"/images/clients/{prescriptionImage.FileName}";

            {
                var path = Path.Combine(_environment.WebRootPath, "images", "clients");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path = Path.Combine(path, prescriptionImage.FileName);

                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    await prescriptionImage.CopyToAsync(stream);
                    stream.Close();
                }
            }
        }


        _dbContext.Patients.Update(samePatient);
        await _dbContext.SaveChangesAsync();

        return Redirect("/doctor-patients");
    }

    [HttpGet("/doctor-patients")]
    public async Task<IActionResult> DoctorPatientList()
    {
        return View();
    }

    [HttpGet("/register")]
    public async Task<IActionResult> RegisterDoctor()
    {
        return View();
    }

    [HttpGet("/smile-makeovers")]
    public async Task<IActionResult> SmileMakeovers()
    {
        return View();
    }

    [HttpGet("/invoice")]
    public async Task<IActionResult> Receipt([FromQuery] double paid)
    {
        var user = await _userManager.GetUserAsync(User);
        var firstName = user.UserName;
        var change = paid - 600;

        ViewData["FName"] = firstName;
        ViewData["Change"] = change;
        ViewData["Paid"] = paid;

        return View();
    }


    [HttpGet("/aboutus")]
    public async Task<IActionResult> AboutUs()
    {
        return View();
    }

    [HttpGet("/patients")]
    public async Task<IActionResult> PatientList()
    {
        return View();
    }

    [HttpGet("/dentists")]
    public async Task<IActionResult> DentistList()
    {
        return View();
    }

    [HttpGet("/messages")]
    public async Task<IActionResult> AdminMessaging()
    {
        return View();
    }

    [HttpGet("/services")]
    public async Task<IActionResult> Services()
    {
        return View();
    }

    [HttpGet("/registerPatient")]
    public async Task<IActionResult> RegisterPatient()
    {
        return View();
    }

    [HttpGet("/registerAppointment")]
    public async Task<IActionResult> Appointment()
    {
        return View();
    }

    [HttpGet("/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    [HttpGet("/dentistdashboard")]
    public async Task<IActionResult> Dentists()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var currentDoctor =
            await _dbContext.Dentists.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        ViewData["Dentist"] = currentDoctor;
        return View();
    }

    [HttpGet("/patientdashboard")]
    public async Task<IActionResult> Patient()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var currentPatient =
                await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

            ViewData["Patient"] = currentPatient;
            return View();
        }
        catch
        {
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            return Content("Refresh the page...");
        }
    }

    public async Task<bool> IsUserValid(ClaimsPrincipal user)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser is null)
            return false;

        // Get the user's email from their claims
        var emailClaim = user.FindFirst(ClaimTypes.Email);
        if (emailClaim == null)
        {
            return false;
        }

        // Look up the user in the database by their email
        var userInDb = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailClaim.Value);
        if (userInDb == null)
        {
            return false;
        }

        // The user was found in the database, so they are valid
        return true;
    }

    public async Task<IActionResult> Index()
    {
        await _roleCreation.CreateRolesAsync();

        if (!await IsUserValid(User))
        {
            await HttpContext.SignOutAsync();
        }

        // Send to dashboards.
        if (User.IsInRole("Dentist"))
        {
            return Redirect("/dentistdashboard");
        }

        if (User.IsInRole("Patient"))
        {
            return Redirect("/patientdashboard");
        }

        if (User.IsInRole("Admin"))
        {
            return Redirect("/dashboard");
        }

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}