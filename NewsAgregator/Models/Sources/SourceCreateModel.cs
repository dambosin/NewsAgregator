﻿using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Mvc.Models.Sources
{
    public class SourceCreateModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }
    }
}
