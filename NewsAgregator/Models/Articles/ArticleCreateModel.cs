using Microsoft.AspNetCore.Mvc.Rendering;
using NewsAgregator.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Articles
{
    public class ArticleCreateModel
    {
        public Guid Id { get; set; }
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
        public DateTime? Created { get; set; }
        [Range(0, int.MaxValue)]
        public int LikesCount { get; set; }

        public List<SelectListItem>? AvailiableSources { get; set; }
    }
}
