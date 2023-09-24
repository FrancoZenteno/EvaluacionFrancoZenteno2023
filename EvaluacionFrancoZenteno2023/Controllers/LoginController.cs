using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Claims;
using EvaluacionFrancoZenteno2023.Models;
using EvaluacionFrancoZenteno2023.Helpers;
using Newtonsoft.Json;

namespace MiEcommApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly AdventureWorksLt2019Context _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(AdventureWorksLt2019Context context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string cEmail, string cPassword)
        {
            var userInfo = await (from emp in _context.Customers
                                  where emp.EmailAddress == cEmail 
                                  select new
                                  {
                                      IDEmployee = emp.CustomerId,
                                      Nombre = emp.FirstName,
                                      Apellido = emp.LastName,
                                      Email = emp.EmailAddress,
                                      Password = emp.PasswordHash

                                  }).FirstOrDefaultAsync();

            if (userInfo != null)
            {
                if (userInfo != null) //&& Argon2PasswordHasher.VerifyHashedPassword(userInfo.Email, cPassword)
                {
                    var claims = new List<Claim>();
                    /*
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, userInfo.Usuario.Email),
                        new Claim(ClaimTypes.NameIdentifier, userInfo.Usuario.Email.ToString()),
                        new Claim(ClaimTypes.Role, userInfo.TipoUsuario.Descripcion.ToString())
                    };

                    var allPermisos = userInfo.PermisosTipoUsuario.Concat(userInfo.PermisosUsuario).Distinct();
                    foreach (var permiso in allPermisos)
                    {
                        claims.Add(new Claim("Permiso", permiso));
                    }
                    */
                    var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");

                    await HttpContext.SignInAsync(
                        "CookieAuth",
                        new ClaimsPrincipal(claimsIdentity),
                        new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30) // Tiempo de expiración de la cookie
                        });

                    _logger.LogInformation("User: {} successfully logged in", userInfo.Email);

                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuth");
            return RedirectToAction("Index", "Home");
        }


    }
}


