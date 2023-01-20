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


    [HttpGet("/clearCookies")]
    public async Task<IActionResult> ClearCookies()
    {
        foreach (var cookie in HttpContext.Request.Cookies)
        {
            HttpContext.Response.Cookies.Delete(cookie.Key);
        }

        return Content("Cookies Cleared");
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