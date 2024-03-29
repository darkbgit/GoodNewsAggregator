﻿using System;

namespace GoodNewsAggregator.Core.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }

        public string Text { get; set; }
        public DateTime PublicationDate { get; set; }

        public Guid NewsId { get; set; }
        public Guid UserId { get; set; }
        //public string UserName { get; set; }
    }
}
