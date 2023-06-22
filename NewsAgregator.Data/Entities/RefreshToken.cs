using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class RefreshToken : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set;}
        [Required]
        public Guid UserId { get; set;}
        public User User { get; set;}
    }
}
