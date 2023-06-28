using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Users
{
    public class UserCreateModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Login { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [MaxLength(30)]
        public string Password { get; set; }
    }
}
