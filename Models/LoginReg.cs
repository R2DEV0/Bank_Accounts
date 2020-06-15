using System.Collections.Generic;

namespace BankAccount.Models
{
    public class LoginReg
    {
        public List<User> AllUsers{get; set;}
        public List<LoginUser> LoginUsers{get; set;}
        public User UserForm{get; set;}
        public LoginUser LoggedUserForm{get; set;}
    }
}