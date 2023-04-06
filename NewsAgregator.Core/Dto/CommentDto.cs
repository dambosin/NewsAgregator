﻿using NewsAgregator.Data.Entities;

namespace NewsAgregator.Core.Dto
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTime Created { get; set; }
        public Guid? ParentCommentId { get; set; }
        public User User { get; set; }
    }
}
