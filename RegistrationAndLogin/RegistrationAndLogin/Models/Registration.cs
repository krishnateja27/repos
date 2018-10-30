using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models
{
    public class Registration
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string firstName { get; set; }
        [DisplayName("Last Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string lastName { get; set; }
        [Range(0,100)]
        [Required(ErrorMessage = "This field is required.")]
        public int age { get; set; }
        [DisplayName("User Name")]
        [Required(ErrorMessage = "This field is required.")]
        public string userName { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required.")]
        public string password { get; set; }
    }
}