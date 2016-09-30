using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Capstone4.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterHomeownerViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote("doesEmailExist1", "Account", HttpMethod = "POST", ErrorMessage = "This email address is already taken.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [StringLength(15, MinimumLength = 6)]
        [Required]
        [Display(Name = "Homeowner Username")]
        [Remote("doesUserNameExist1", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string Screen_name { get; set; }
        [StringLength(20, MinimumLength = 1)]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(25, MinimumLength = 1)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [StringLength(40, MinimumLength = 1)]
        [Required]
        public string City { get; set; }
        [StringLength(2, MinimumLength = 2)]
        [Required]
        public string State { get; set; }
        [StringLength(5, MinimumLength = 5)]
        [Required]
        public string Zip { get; set; }
        public bool vacant { get; set; }
        public bool validated { get; set; }
        public bool Inactive { get; set; }
    }
    public class RegisterContractorViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote("doesEmailExist1", "Account", HttpMethod = "POST", ErrorMessage = "This email address is already taken.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [StringLength(15, MinimumLength = 6)]
        [Required]
        [Display(Name = "Contractor Username")]
        [Remote("doesUserNameExist1", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string Screen_name { get; set; }
        [StringLength(20, MinimumLength = 1)]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(25, MinimumLength = 1)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Street { get; set; }
        [StringLength(40, MinimumLength = 1)]
        [Required]
        public string City { get; set; }
        [StringLength(2, MinimumLength = 2)]
        [Required]
        public string State { get; set; }
        [StringLength(5, MinimumLength = 5)]
        [Required]
        public string Zip { get; set; }
        [Required]
        [Display(Name = "Miles willing to travel:")]
        public double travelDistance { get; set; }
        public bool vacant { get; set; }
        public bool validated { get; set; }
        public bool Inactive { get; set; }
    }

    public class RegisterAdminViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [System.Web.Mvc.Remote("adminPass", "Account", HttpMethod = "POST", ErrorMessage = "This is the incorrect admin creation password.")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [StringLength(15, MinimumLength = 6)]
        [Required]
        [Display(Name = "Admin Username")]
        [System.Web.Mvc.Remote("doesUserNameExist1", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
        public string Screen_name { get; set; }
        [StringLength(20, MinimumLength = 1)]
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [StringLength(25, MinimumLength = 1)]
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

    }


    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "User Type")]
        public int roles { get; set; }

    }

    public class ChooseRoleViewModel
    {

        [Display(Name = "User Type")]
        public int roles { get; set; }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
