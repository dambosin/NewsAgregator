﻿using System.ComponentModel.DataAnnotations;

namespace NewsAgregator.Data.Entities
{
    public class User : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Name { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Like> Likes { get; set; }
    }
}