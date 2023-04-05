﻿using Microsoft.Identity.Client;
using NewsAgregator.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Eentities
{
    public class Source : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Url { get; set; }
    }
}