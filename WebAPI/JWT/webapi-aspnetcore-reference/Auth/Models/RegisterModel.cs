using System.ComponentModel.DataAnnotations;

namespace webapi_aspnetcore_reference.Auth.Models
{
    /// <summary>
    /// Created to modeling a Register entity.
    /// </summary>
    public class RegisterModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string? Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}
