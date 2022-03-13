using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TrickingLibrary.API.Forms;

namespace TrickingLibrary.API.Pages.Account
{
    public class Register : PageModel
    {
        [BindProperty] public RegisterForm Form { get; set; }

        public void OnGet(string returnUrl)
        {
            Form = new RegisterForm {ReturnUrl = returnUrl};
        }

        public async Task<IActionResult> OnPostAsync(
            [FromServices] UserManager<IdentityUser> userManager,
            [FromServices] SignInManager<IdentityUser> signInManager)
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new IdentityUser(Form.Username) {Email = Form.Email};

            var createUserResult = await userManager.CreateAsync(user, Form.Password);

            if (!createUserResult.Succeeded) return Page();
            
            await signInManager.SignInAsync(user, true);
            return Redirect(Form.ReturnUrl);
        }
    }
}