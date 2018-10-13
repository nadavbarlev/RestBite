// Shenhav Meshulam - 313614273
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestBite.Models
{
    public class Client
    {
        public int ID { get; set; }
  
        public Gender Gender { get; set; }

        [Required]
        [DisplayName("Logon Name")]
        public string ClientName { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName("Administrator")]
        public bool isAdmin { get; set; }

        public virtual List<Comment> Comments { get; set; }

        public virtual List<Post> Posts { get; set; }
    }

    public class userPostsViewModel
    {
        public int ID { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }


        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("The Post")]
        public string Title { get; set; }

        [DisplayName("Genre ID")]
        public int GenreID { get; set; }
    }
    public enum Gender
    {
        Male,
        Female
    }
}