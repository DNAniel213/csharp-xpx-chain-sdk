using System.ComponentModel.DataAnnotations;

namespace Xarcade.Application.Authentication.Models
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6)]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is required.")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
