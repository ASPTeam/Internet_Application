//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projects_Management_System.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Responding_Post
    {
        [Key]
        public int ID { get; set; }
        public int Post_ID { get; set; }
        public int Admin_ID { get; set; }
        [Required]
        public bool post_stat { get; set; }
    
        public virtual User User { get; set; }
    }
}
