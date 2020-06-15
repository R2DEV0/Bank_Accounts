using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BankAccount.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace BankAccount.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        public HomeController(MyContext context)
        {
            dbContext = context;
        }

// ****************************************************  GET REQUEST ******************************************* // 
        // Register Page, create new User //
        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }

        // Display Users bank account page when logged in //
        [HttpGet("account")]
        public ViewResult Account()
        {
            int? LoggedUser = HttpContext.Session.GetInt32("UserId");
            if(LoggedUser == null)
            {
                return View("Index");
            }
            User ToView = dbContext.Users.Include(t => t.OwenerTransactions).FirstOrDefault(user => user.UserId == LoggedUser);
            return View("Account", ToView);
        }


// ****************************************************  POST REQUEST ******************************************* // 
        // Create a new User //
        [HttpPost("register")]
        public IActionResult Register(LoginReg FromForm)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == FromForm.UserForm.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use!");
                    return Index();
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                FromForm.UserForm.Password = Hasher.HashPassword(FromForm.UserForm, FromForm.UserForm.Password);
                dbContext.Users.Add(FromForm.UserForm);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", FromForm.UserForm.UserId);
                return RedirectToAction("Account");
            }
            return Index();
        }

        // login in existing User //
        [HttpPost("login")]
        public IActionResult Login(LoginReg FromForm)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == FromForm.LoggedUserForm.Email);
                if(userInDb == null)
                {
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return Index();
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(FromForm.LoggedUserForm, userInDb.Password, FromForm.LoggedUserForm.Password);
                if(result == 0)
                {
                    ModelState.AddModelError("Password", "Wrong Password");
                    return Index();
                }
                else
                {
                    HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                    return RedirectToAction("Account");
                }
            }
            else
            {
                return Index();
            }
        }

        // Deposit/Withdraw Money //
        [HttpPost("transfer")]
        public IActionResult Transfer()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            User user = dbContext.Users.Include(u => u.OwenerTransactions).FirstOrDefault(u => u.UserId == userId);
            decimal amount = Convert.ToDecimal(Request.Form["Amount"]);
            // checks to see if valid amount is submitted //
            if (Request.Form["Amount"] == "")
            {
                ViewBag.ErrorMessage = "Invalid. Please enter a valid amount.";
                return View("Account", user);
            }
            // checks to see if sufficient funds are in user account //
            decimal sum = 0;
                foreach(var trans in user.OwenerTransactions)
                {
                    sum += trans.Amount;
                }
                if(sum + amount < 0)
                {
                    ViewBag.ErrorMessage = "Insufficient funds, cannot make transaction";
                    return View("Account", user);
                }
                
            Transaction transaction = new Transaction() { Amount = amount, UserId = (int)userId };
            dbContext.Add(transaction);
            dbContext.SaveChanges();
            ViewBag.ErrorMessage = "";
            return RedirectToAction("Account");
        }


        // Logout User //
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
