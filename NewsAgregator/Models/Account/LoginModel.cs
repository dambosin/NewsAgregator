using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Account
{
    public class LoginModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
