using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Role : IBaseEntity
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}
