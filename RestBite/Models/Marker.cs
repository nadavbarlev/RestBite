
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RestBite.Models
{
    public class Marker
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(40)]
        [DisplayName("Address")]
        public string Address { get; set; }

        
        [Required]
        [DisplayName("latitude")]
        public float lat { get; set; }

        [Required]
        [DisplayName("longitude")]
        public float lng { get; set; }

        [Required]
        [MaxLength(20)]
        [DisplayName("type")]
        public string type { get; set; }

    }
}