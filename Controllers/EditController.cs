﻿using Microsoft.AspNetCore.Mvc;
using ThreeFriends.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;

namespace ThreeFriends.Controllers
{
    public class EditController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Appdbcontxt _dbContext;

        public EditController(IWebHostEnvironment webHostEnvironment, Appdbcontxt dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }


        public IActionResult Login()
        {
            return View(new User());
        }

        public IActionResult AccountSettingPage()
        {
            var CurUser = GetCurrentSessionUser();
            SharedValues.setHover("Account");
            return View(CurUser);
        }

        public IActionResult emailSetting()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult saveEmailSetting(User test)
        {

            var CurUser = GetCurrentSessionUser();
            if (IsValidEmail(test.Email))

            {
                CurUser.Email = test.Email;
                UpdateUser(CurUser);
                return RedirectToAction("AccountSettingPage");
            }
            else


            {
                ModelState.AddModelError("Email", "Please enter a valid email address.");
                return View("emailSetting", test);
            }

        }

        public IActionResult passwordSetting()
        {
            return View(new User());
        }


        [HttpPost]
        public IActionResult savePasswordSetting(User test, string confirmPass)
        {
            var CurUser = GetCurrentSessionUser();
            if (IsValidPassword(test.Password) && test.Password == confirmPass)
            {
                CurUser.Password = Hashing.HashPassword(test.Password);
                UpdateUser(CurUser);
                return RedirectToAction("AccountSettingPage");

            }
            else
            {

                ModelState.AddModelError("Password", "Please enter a valid password and ensure both passwords match.");
                return View("passwordSetting", test);

            }
        }


        public IActionResult nameSetting()
        {
            return View(new User());
        }


        [HttpPost]
        public IActionResult saveNameSetting(User test)
        {
            var CurUser = GetCurrentSessionUser();
            
            Regex r = new Regex(@"(^[a-zA-Z][a-zA-Z][a-zA-Z]*$)");
            bool testfn = !string.IsNullOrEmpty(test.First_Name) && r.Match(test.First_Name).Success;
            bool testln = !string.IsNullOrEmpty(test.Last_Name) && r.Match(test.Last_Name).Success;
            if (string.IsNullOrWhiteSpace(test.First_Name) && string.IsNullOrWhiteSpace(test.Last_Name))
            {
                ModelState.AddModelError("both_empty", "please fill either first name or last name");
                return View("nameSetting", test);
            }
            if (test.First_Name != null && string.IsNullOrWhiteSpace(test.Last_Name))
            {
                if (testfn == true)
                {
                    CurUser.First_Name = test.First_Name;
                    UpdateUser(CurUser);
                    return RedirectToAction("AccountSettingPage");
                }
                else
                {
                    ModelState.AddModelError("correctFirstName", "First name should alphabets only and 2 or more characters");
                    return View("nameSetting", test);
                }

            }
            if (test.Last_Name != null && string.IsNullOrWhiteSpace(test.First_Name))
            {
                if (testln == true)
                {

                    CurUser.Last_Name = test.Last_Name;
                    UpdateUser(CurUser);
                    return RedirectToAction("AccountSettingPage");
                }
                else
                {
                    ModelState.AddModelError("correctLastName", "Last name should alphabets only and 2 or more characters");
                    return View("nameSetting", test);
                }
            }
            if (test.Last_Name != null && test.First_Name != null)
            {
                if ((testfn == true) && (testln == true))
                {
                    CurUser.Last_Name = test.Last_Name;
                    CurUser.First_Name = test.First_Name;
                    UpdateUser(CurUser);
                    return RedirectToAction("AccountSettingPage");
                }
                else
                {
                    ModelState.AddModelError("correctBothName", "First and Last name should alphabets only and 2 or more characters");
                    return View("nameSetting", test);
                }

            }
            else
                return View("nameSetting", test);
        }


        [HttpPost]
        public IActionResult deleteAccount()
        {
            var CurUser = GetCurrentSessionUser();
            if (CurUser != null)
            {
                _dbContext.Users.Remove(CurUser);
                _dbContext.SaveChanges();
                HttpContext.Session.Clear();
            }
            return RedirectToAction("index", "LogOut");
        }

        public IActionResult imageSetting()
        {
            return View(new User());
        }

        [HttpPost]
        public IActionResult saveImageSetting(User test, IFormFile ImageFile)
        {
            var CurUser = GetCurrentSessionUser();
            if (ImageFile != null && ImageFile.Length > 0 && ImageFile.ContentType.StartsWith("image"))
            {


                string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ImageFile.CopyTo(fileStream);
                }


                string imagePath = "/images/" + uniqueFileName;
                CurUser.photoPath = imagePath;
                HttpContext.Session.SetString("PhotoPath", imagePath);
                UpdateUser(CurUser);
                return RedirectToAction("AccountSettingPage");
            }
            else
            {

                ModelState.AddModelError("ImageFile", "Please upload a valid image.");
                return View("imageSetting", test);

            }
        }

        private User GetCurrentSessionUser()
        {
            var username = HttpContext.Session.GetString("UserName");
            var password = SharedValues.CurUser.Password;
            return _dbContext.Users.FirstOrDefault(U => U.User_Name == username);
        }

        private bool IsValidEmail(string email)
        {
            Regex r = new Regex(@"(^[a-zA-z]+[0-9]*@[a-z]+\.[a-z]{3}$)");
            return !string.IsNullOrEmpty(email) && r.Match(email).Success;
        }

        private bool IsValidPassword(string password)
        {
            Regex r = new Regex(@"(^(?=.*[A-Z])(?=.*[\d])(?=.*[\W_]).{8,}$)");
            return !string.IsNullOrEmpty(password) && r.Match(password).Success;
        }

        private void UpdateUser(User user)
        {
            _dbContext.SaveChanges();
        }

       
    }
}
