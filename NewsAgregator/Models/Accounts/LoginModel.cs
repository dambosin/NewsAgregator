using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Accounts
{
    public class LoginModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Login is required")]
        [MinLength(4)]
        [MaxLength(30)]
        public string Login { get; set; }
        [Required(AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; }
    }
}
