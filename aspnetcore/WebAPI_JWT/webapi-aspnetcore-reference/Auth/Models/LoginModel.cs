using System.ComponentModel.DataAnnotations;

namespace webapi_aspnetcore_reference.Auth.Models
{
    /// <summary>
    /// Created to modeling Login entity.
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
