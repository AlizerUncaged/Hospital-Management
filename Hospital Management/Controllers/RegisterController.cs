using Hospital_Management.Data;
using Hospital_Management.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Controllers;

public class RegisterController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly SignInManager<IdentityUser> _signInManager;

    public RegisterController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager,
        ApplicationDbContext dbContext, SignInManager<IdentityUser> signInManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _signInManager = signInManager;
    }

    [HttpPost("/registerDoctor")]
    public async Task<IActionResult> Register([FromForm] string name, [FromForm] string address,
        [FromForm] string birthdate, [FromForm] string gender,
        [FromForm] string cellphoneNumber, [FromForm] string licenseNumber, [FromForm] string password)
    {
        var dentist = new Dentist()
        {
            Name = name, Address = address, Gender = gender, Birthdate = birthdate, CellphoneNumber = cellphoneNumber,
            LicenseNumber = licenseNumber, UserName = name
        };

        var newEntity = await _dbContext.Dentists.AddAsync(dentist);

        var registerResult = await _userManager.CreateAsync(dentist, password);

        if (!registerResult.Succeeded)
            return Content(registerResult.Errors.FirstOrDefault().Description);
        await _userManager.AddToRoleAsync(newEntity.Entity, "Dentist");

        await _dbContext.SaveChangesAsync();

        await _signInManager.SignInAsync(newEntity.Entity, true);

        return RedirectPermanent("/");
    }

    [HttpPost("/registerPatient")]
    public async Task<IActionResult> RegisterPatient([FromForm] string name, [FromForm] string address,
        [FromForm] string birthdate, [FromForm] string gender,
        [FromForm] string cellphoneNumber, [FromForm] string guardian, [FromForm] string password)
    {
        var dentist = new Patient()
        {
            Name = name, Address = address, Gender = gender, Birthdate = birthdate, CellphoneNumber = cellphoneNumber,
            Guardian = guardian, UserName = name
        };

        var newEntity = await _dbContext.Patients.AddAsync(dentist);

        var registerResult = await _userManager.CreateAsync(dentist, password);

        if (!registerResult.Succeeded)
            return Content(registerResult.Errors.FirstOrDefault().Description);

        await _userManager.AddToRoleAsync(newEntity.Entity, "Patient");

        await _dbContext.SaveChangesAsync();

        await _signInManager.SignInAsync(newEntity.Entity, true);

        return RedirectPermanent("/");
    }

    [HttpPost("/newAppointment")]
    public async Task<IActionResult> RegisterPatient([FromForm] string description)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var currentDoctor =
            await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        var dentist = new AppointmentModel()
        {
            Note = description, Patient = currentDoctor
        };

        var newEntity = await _dbContext.Appointments.AddAsync(dentist);

        await _dbContext.SaveChangesAsync();

        return RedirectPermanent("/");
    }
}