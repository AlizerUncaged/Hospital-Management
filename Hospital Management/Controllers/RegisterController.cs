﻿using Hospital_Management.Data;
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
    private readonly IWebHostEnvironment _environment;

    public RegisterController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager,
        ApplicationDbContext dbContext, SignInManager<IdentityUser> signInManager, IWebHostEnvironment environment)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _signInManager = signInManager;
        _environment = environment;
    }

    [HttpPost("/registerDoctor")]
    public async Task<IActionResult> Register([FromForm] string name, [FromForm] string address,
        [FromForm] string birthdate, [FromForm] string gender,
        [FromForm] string cellphoneNumber, [FromForm] string licenseNumber, [FromForm] string password,
        [FromForm] IFormFile? image)
    {
        var dentistImage = string.Empty;
        if (image is { })
        {
            var path = Path.Combine(_environment.WebRootPath, "images", "doctors");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path = Path.Combine(path, image.FileName);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(stream);
                stream.Close();
            }
            
            dentistImage = $"/images/doctors/{image.FileName}";
        }

        var dentist = new Dentist()
        {
            Name = name, Address = address, Gender = gender, Birthdate = birthdate, CellphoneNumber = cellphoneNumber,
            LicenseNumber = licenseNumber, UserName = name, DentistImage = dentistImage
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
        [FromForm] string birthdate, [FromForm] string gender, [FromForm] string email,
        [FromForm] string cellphoneNumber, [FromForm] string guardian, [FromForm] string password)
    {
        var dentist = new Patient()
        {
            Name = name, Address = address, Gender = gender, Birthdate = birthdate, CellphoneNumber = cellphoneNumber,
            Guardian = guardian, UserName = name, Email = email
        };

        var newEntity = await _dbContext.Patients.AddAsync(dentist);

        var registerResult = await _userManager.CreateAsync(dentist, password);

        if (!registerResult.Succeeded)
            return Content(registerResult.Errors.FirstOrDefault().Description);

        await _userManager.AddToRoleAsync(newEntity.Entity, "Patient");

        await _dbContext.SaveChangesAsync();

        await _signInManager.SignInAsync(newEntity.Entity, true);

        return Redirect("/");
    }

    [HttpPost("/newAppointment")]
    public async Task<IActionResult> RegisterPatient([FromForm] string description, [FromForm] double paid,
        [FromForm] DateTime? date, [FromForm] string[] concerns)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var currentPatient =
            await _dbContext.Patients.FirstOrDefaultAsync(x => x.Id == currentUser.Id);

        var dentist = new AppointmentModel()
        {
            Note = description, Patient = currentPatient,
            Services = string.Join(",", concerns),
            Date = date, TotalPrice = paid
        };

        var newEntity = await _dbContext.Appointments.AddAsync(dentist);

        await _dbContext.SaveChangesAsync();

        return Redirect($"/Invoice?appointmentId={newEntity.Entity.AppointmentId}");
    }
}