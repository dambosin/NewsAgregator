using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Accounts
{
    public class RegisterModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}