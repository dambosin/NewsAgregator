using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class UserRole : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        [Required]
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }
}
