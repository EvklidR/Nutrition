using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Configurations
{
    public class RoleInitializer
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public RoleInitializer(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task InitializeAsync()
        {
            string adminEmail = _configuration["AdminData:email"];
            string password = _configuration["AdminData:password"];

            string applicationUrl = _configuration["profiles:https:applicationUrl"]
                                     ?? _configuration["profiles:http:applicationUrl"]
                                     ?? "https://localhost:7075";

            if (await _roleManager.FindByNameAsync("admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("admin"));
            }
            if (await _roleManager.FindByNameAsync("user") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>("user"));
            }
            if (await _userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new User { Email = adminEmail, UserName = adminEmail };

                IdentityResult result = await _userManager.CreateAsync(admin, password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(admin);

                    var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                    var confirmationLink = $"{applicationUrl}/User/confirmEmail?userId={admin.Id}&code={code}";

                    await _emailService.SendConfirmationEmailAsync(adminEmail, $"<a href='{confirmationLink}'>Confirm</a>");
                    await _userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

    }
}
