using System.ComponentModel.DataAnnotations;

namespace TrickingLibrary.API.Forms
{
    public class RegisterForm
    {
        [Required] public string ReturnUrl { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}