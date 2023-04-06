using NewsAgregator.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewsAgregator.Core.Dto
{
    internal class ArticleWithSourceDto
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Content { get; set; }
        public double? PositiveIndex { get; set; }
        public List<Comment>? Comments { get; set; }
        public Guid SourceId { get; set; }
        public Source Source { get; set; }
        public DateTime? Created { get; set; }
        public int LikesCount { get; set; }
    }
}
