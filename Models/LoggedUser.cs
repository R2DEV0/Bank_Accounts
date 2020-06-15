using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccount.Models
{
    public class LoginUser
    {
    [Required(ErrorMessage="No Email was entered")]
    public string Email {get; set;}

    [Required(ErrorMessage="No Password was entered")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    }
}