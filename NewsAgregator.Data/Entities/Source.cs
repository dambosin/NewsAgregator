using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class Source : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? RssUrl { get; set; }
    }
}