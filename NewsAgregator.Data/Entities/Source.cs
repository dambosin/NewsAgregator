using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Source : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public string RssUrl { get; set; }
    }
}