using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Roles
{
    public class RoleCreateModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}
