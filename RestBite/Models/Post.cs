// Shenhav Meshulam - 313614273
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestBite.Models
{
    public class Post
    {
        public int ID { get; set; }

        [Required]
        [ForeignKey("Client")]
        [DisplayName("Posting Client")]
        public int ClientID { get; set; }

        [Required]
        [ForeignKey("Genre")]
        [DisplayName("Genre of the Post")]
        public int GenreID { get; set; }

        [MaxLength(20)]
        [Required]
        [DisplayName("Title")]
        public string Title { get; set; }

        [MaxLength(5000)]
        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created at")]
        public DateTime CreationDate { get; set; }

        public virtual Client Client { get; set; }

        public virtual Genre Genre { get; set; }

        public virtual List<Comment> Comments { get; set; }
    }

    public class PostCommentViewModel
    {
        public int ID { get; set; }

        [DisplayName("The Post")]
        public string Title { get; set; }

        [DisplayName("Number Of Comments")]
        public int NumberOfComment { get; set; }
    }
}