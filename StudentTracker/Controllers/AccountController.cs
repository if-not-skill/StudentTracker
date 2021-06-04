using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using StudentTracker.Data;
using StudentTracker.Models;
using StudentTracker.ViewModels;

namespace StudentTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly StudentTrackerContext _context;

        public AccountController(StudentTrackerContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Students");
                }

                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User
                    {
                        LastName = model.LastName, FirstName = model.FirstName, Email = model.Email,
                        Password = model.Password
                    };

                    Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    // добавляем пользователя в бд
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user); // аутентификация

                    return RedirectToAction("Index", "Students");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }

            return View(model);
        }

        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name!)
            };

            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Details()
        {
            User user = _context.Users
                .Include(s => s.Role)
                .First(s => s.Email == User.Identity.Name);

            return  View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                User user = _context.Users
                    .Include(s => s.Role)
                    .First(s => s.Email == User.Identity.Name);

                ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

                return View(user);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,LastName,FirstName,Email")]
            User user)
        {
            if (_context.Users.First(u => u.Email == User.Identity.Name).UserId != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    User currentUser = _context.Users.First(u => u.UserId == user.UserId);
                    currentUser.FirstName = user.FirstName;
                    currentUser.LastName = user.LastName;
                    currentUser.Email = user.Email;

                    _context.Update(currentUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Details));
            }

            ViewData["Roles"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> ChangePassword(int? id)
        {
            User user;


            ChangePasswordModel changePasswordModel;
            if (id == null)
            {
                user = _context.Users.First(u => u.Email == User.Identity.Name);

                changePasswordModel = new ChangePasswordModel {Id = user.UserId, Email = user.Email};

                return View(changePasswordModel);
            }

            user = _context.Users.First(u => u.UserId == id);

            changePasswordModel = new ChangePasswordModel {Id = user.UserId, Email = user.Email};

            return View(changePasswordModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstAsync(u => u.UserId == model.Id);
                
                if (user != null)
                {
                    if (user.Password != model.OldPassword)
                    {
                        ModelState.AddModelError(String.Empty,  "Старый пароль указан не верно");
                    }
                    else
                    {

                        if (model.NewPassword != model.ConfirmNewPassword)
                        {
                            ModelState.AddModelError(String.Empty, "Новый пароль не совпадает");
                        }
                        else
                        {
                            try
                            {
                                user.Password = model.NewPassword;

                                _context.Update(user);
                                await _context.SaveChangesAsync();
                            }
                            catch (DbUpdateConcurrencyException)
                            {
                                if (!UserExists(user.UserId))
                                {
                                    return NotFound();
                                }
                                else
                                {
                                    throw;
                                }
                            }


                            return RedirectToAction(nameof(Details));
                        }
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (User.IsInRole("admin"))
            {
                if (id == null)
                {
                    return NotFound();
                }

                User user = await _context.Users.FirstAsync(u => u.UserId == id);

                return View(user);
            }
            else
            {
                User user = await _context.Users.FirstAsync(u => u.Email == User.Identity.Name);

                return View(user);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (User.IsInRole("admin"))
            {
                var user = await _context.Users.FindAsync(id);

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            if (_context.Users.First(u => u.Email == User.Identity.Name).UserId == id)
            {
                var user = await _context.Users.FindAsync(id);

                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Logout));
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> Index()
        {
            IIncludableQueryable<User, Role> users = _context.Users.Include(u => u.Role);

            return View(users);
        }
    }
}

