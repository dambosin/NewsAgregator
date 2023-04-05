﻿using NewsAgregator.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAgregator.Data.Entities
{
    public class Article : IBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
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