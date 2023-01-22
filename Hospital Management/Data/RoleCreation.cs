using Hospital_Management.Models;
using Microsoft.AspNetCore.Identity;

namespace Hospital_Management.Data;

public class RoleCreation
{
    private readonly IServiceProvider _serviceProvider;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _dbContext;

    public RoleCreation(IServiceProvider serviceProvider, RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _serviceProvider = serviceProvider;
        _roleManager = roleManager;
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task CreateRolesAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();


        //initializing custom roles 
        string[] roleNames = { "Admin", "Dentist", "Patient" };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                //create the roles and seed them to the database: Question 1
                roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Register sample patient.
        {
            var patient = new Patient()
            {
                Name = "patient", Address = "patient", Gender = "patient", Birthdate = DateTime.Now.ToString(),
                CellphoneNumber = "123",
                Guardian = "patient", UserName = "patient", Email = "patient@patient.com"
            };

            var newEntity = await _dbContext.Patients.AddAsync(patient);

            var registerResult = await _userManager.CreateAsync(patient, "pass123");

            await _userManager.AddToRoleAsync(newEntity.Entity, "Patient");

            await _dbContext.SaveChangesAsync();
        }

        // Register sample doctor.
        {
            var dentist = new Dentist()
            {
                Name = "dentist", Address = "dentist", Gender = "dentist", Birthdate = DateTime.Now.ToString(),
                CellphoneNumber = "123", Email = "dentist@dentist.com",
                LicenseNumber = "dentist", UserName = "dentist", DentistImage = $"/images/doctors/image.png"
            };

            var newEntity = await _dbContext.Dentists.AddAsync(dentist);

            var registerResult = await _userManager.CreateAsync(dentist, "pass123");

            await _userManager.AddToRoleAsync(newEntity.Entity, "Dentist");

            await _dbContext.SaveChangesAsync();
        }

        // Create admin if not exist yet.
        var powerhouse = new IdentityUser
        {
            UserName = "Admin",
            Email = "Admin@admin.com",
            EmailConfirmed = true,
            TwoFactorEnabled = false
        };

        const string userPwd = "password123";
        var user = await _userManager.FindByEmailAsync("Admin@admin.com");

        if (user == null)
        {
            var createPowerUser = await _userManager.CreateAsync(powerhouse, userPwd);
            if (createPowerUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(powerhouse, "Admin");
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}