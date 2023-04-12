using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Articles
{
    public class ArticleCreateModel
    {
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? ShortDescription { get; set; }
        [Required]
        public string? Content { get; set; }
        [Range(0, 10)]
        public double? PositiveIndex { get; set; }
        [Required]
        public Guid SourceId { get; set; }

        public List<SelectListItem>? AvailiableSources { get; set; }
    }
}
