using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace BankAccount.Models
{
    public class User
    {
        [Key]
        public int UserId{get; set;}

        [Required(ErrorMessage="First Name is Required")]
        [MinLength(2, ErrorMessage="First name must be at least 2 characters")]
        public string FirstName{get; set;}

        [Required(ErrorMessage="Last Name is Required")]
        [MinLength(2, ErrorMessage="Last name must be at least 2 characters")]
        public string LastName{get; set;}


        [EmailAddress(ErrorMessage="Email address is not valid")]
        [Required(ErrorMessage="Email address is required")]
        public string Email{get; set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage="Password is required")]
        [MinLength(7, ErrorMessage="Password must be at least 7 characters")]
        public string Password {get; set;}


        [NotMapped]
        [Compare("Password", ErrorMessage="Passwords must both match")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}

        public List<Transaction> OwenerTransactions {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
    }
}