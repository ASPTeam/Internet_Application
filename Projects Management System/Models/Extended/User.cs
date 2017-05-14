using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Projects_Management_System.Models
{
    [MetadataType(typeof(usermetadata))]
    public partial class User
    {
        public string confirmpassword { get; set; }
        public HttpPostedFileBase File { get; set; }

    }
    public class usermetadata
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "User name is required")]
        [Display(Name = "User Name")]
        public string User_Name { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [MinLength(6, ErrorMessage = "Password length must be at least 6 characters")]
        public string password { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Enter a valid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        public byte[] Photo { get; set; }


        [Required(ErrorMessage = "please upload your photo")]
        [Display(Name = "Upload your photo")]
        public HttpPostedFileBase File { get; set; }


        [Required(ErrorMessage = "Please enter the job Description")]
        [Display(Name = "Job Description")]
        public string Job_Description { get; set; }


        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string First_Name { get; set; }


        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string Last_Name { get; set; }


        [Required(ErrorMessage = "please enter a phone number ")]
        [DataType(DataType.PhoneNumber)]
        public string Mobile { get; set; }

        [Display(Name = "Role")]
        [Required(ErrorMessage = "please determine the new user privileges")]
        public string Type { get; set; }


        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("password", ErrorMessage = "passwords doesn't match ")]
        public string confirmpassword { get; set; }

      
    }
}