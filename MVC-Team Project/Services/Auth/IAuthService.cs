using Microsoft.AspNetCore.Identity;
using MVC_Team_Project.Models;
using MVC_Team_Project.View_Models;

namespace MVC_Team_Project.Services.Auth
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterPatientAsync(RegisterPatientViewModel model);
        Task<IdentityResult> RegisterDoctorAsync(RegisterDoctorViewModel model);
        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);

        Task LogoutAsync();
    }
}