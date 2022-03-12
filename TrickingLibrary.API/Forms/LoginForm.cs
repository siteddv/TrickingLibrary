using System.ComponentModel.DataAnnotations;

namespace TrickingLibrary.API.Forms
{
    public class LoginForm
    {
        [Required] 
        public string ReturnUrl { get; set; }
        
        [Required] 
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}