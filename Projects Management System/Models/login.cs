using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Projects_Management_System.Models
{
    public class login
    {
        [Display(Name ="Email")]
        [Required(AllowEmptyStrings =false ,ErrorMessage ="Email Required")]
        public string Email { get; set; }

        [Display(Name ="Password")]
        [Required(AllowEmptyStrings =false , ErrorMessage ="Password Required")]
        [DataType(DataType.Password)]
        public string password { get; set; }


        [Display(Name ="Remeber ME")]
        public bool RememberMe  { get; set; }
    }
}