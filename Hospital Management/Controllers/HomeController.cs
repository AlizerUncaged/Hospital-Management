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

    public HomeController(ILogger<HomeController> logger, RoleCreation roleCreation, ApplicationDbContext dbContext,
        UserManager<IdentityUser> userManager)
    {
        _logger = logger;
        _roleCreation = roleCreation;
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpGet("/delete/{appointmentId}")]
    public async Task<IActionResult> DeleteAppointment(int appointmentId)
    {
        var appointment = await _dbContext.Appointments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppointmentID == appointmentId);

        _dbContext.Appointments.Remove(appointment);

        await _dbContext.SaveChangesAsync();

        return Redirect("/");
    }

    [HttpGet("/cancel/{appointmentId}/{reason}")]
    public async Task<IActionResult> CancelAppointment(int appointmentId, string reason)
    {
        var appointment = await _dbContext.Appointments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppointmentID == appointmentId);

        appointment.IsCancelled = true;

        _dbContext.Appointments.Update(appointment);

        await _dbContext.SaveChangesAsync();

        return Redirect("/dentistdashboard");
    }

    [HttpGet("/approve/{appointmentId}")]
    public async Task<IActionResult> AcceptAppointment(int appointmentId)
    {
        var appointment = await _dbContext.Appointments.AsNoTracking()
            .FirstOrDefaultAsync(x => x.AppointmentID == appointmentId);

        appointment.IsCancelled = false;

        var currentUser = await _userManager.GetUserAsync(User);

        appointment.Dentist = await _dbContext.Dentists.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        _dbContext.Appointments.Update(appointment);

        await _dbContext.SaveChangesAsync();

        return Redirect("/dentistdashboard");
    }

    [HttpGet("/register")]
    public async Task<IActionResult> RegisterDoctor()
    {
        return View();
    }
    

    [HttpGet("/aboutus")]
    public async Task<IActionResult> AboutUs()
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
    public async Task<IActionResult> RegisterAppointment()
    {
        return View();
    }

    [HttpGet("/dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        return View();
    }

    [HttpGet("/dentistdashboard")]
    public async Task<IActionResult> DentistDashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var currentDoctor =
            await _dbContext.Dentists.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        ViewData["Dentist"] = currentDoctor;
        return View();
    }

    [HttpGet("/patientdashboard")]
    public async Task<IActionResult> PatientDashboard()
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var currentPatient =
            await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        ViewData["Patient"] = currentPatient;
        return View();
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