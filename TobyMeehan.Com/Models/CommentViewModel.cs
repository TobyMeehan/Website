using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Models
{
    public class CommentViewModel
    {
        public string Id { get; set; }

        [MaxLength(321, ErrorMessage = "Comments should be less than 321 characters.")]
        [MinLength(1, ErrorMessage = "You need to actually write something.")]
        public string Content { get; set; }
    }
}
