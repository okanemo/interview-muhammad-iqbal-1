using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Testing2.Models;

namespace Testing2.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseContext _context;

        public HomeController(DatabaseContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var menu = GetMenu();
            if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Name") == null && HttpContext.Session.GetString("RoleId") == null && HttpContext.Session.GetString("RoleName") == null)
                menu = null;
                
            var vm = new ViewModel() { Menu = menu };
            return View(vm);
        }

        public IActionResult Login()
        {
            Menu menu = null;
            var vm = new ViewModel() { Menu = menu };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Login(Models.Login request)
        {
            if(request.Password == null)
                return RedirectToAction("ErrorLogin");

            var checkUser = _context.Users.FirstOrDefault(x => x.UserName.Contains(request.UserName));
            /* Extract the bytes */
            byte[] hashBytes = Convert.FromBase64String(checkUser.Password);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(request.Password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return RedirectToAction("ErrorLogin");
            
            if(checkUser == null)
                return RedirectToAction("ErrorLogin");

            var getDataRole = _context.MasterRole.FirstOrDefault(x => x.Id == checkUser.RoleId);

            HttpContext.Session.SetString("UserName", checkUser.UserName);
            HttpContext.Session.SetString("Name", checkUser.Name);
            HttpContext.Session.SetString("RoleId", checkUser.RoleId.ToString());
            HttpContext.Session.SetString("RoleName", getDataRole.Name);

            var listMenu = GetMenu();
            var menu = listMenu.Items.FirstOrDefault();
            
            return RedirectToAction("DynamicMenu", "Home", new { linkmenu = $"{menu.LinkText}" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult DynamicMenu(string linkmenu)
        {
            if (HttpContext.Session.GetString("UserName") == null && HttpContext.Session.GetString("Name") == null && HttpContext.Session.GetString("RoleId") == null && HttpContext.Session.GetString("RoleName") == null)
                return RedirectToAction("ErrorAccess");

            var checkAccess = _context.AccessMenu.Where(x => x.RoleId == int.Parse(HttpContext.Session.GetString("RoleId"))).ToList();
            var getIdMenu = _context.MasterMenu.FirstOrDefault(x => x.Name.Contains(linkmenu));
            var getAccessMenu = checkAccess.FirstOrDefault(x => x.MenuId == getIdMenu.Id);

            if(getAccessMenu == null)
                return RedirectToAction("ErrorAccess");

            ViewData["Menu"] = linkmenu;
            var menu = GetMenu();
            var vm = new ViewModel() { Menu = menu };
            return View(vm);
        }

        private Menu GetMenu()
        {
            var menu = new Menu();
            var menuItems = new List<MenuItem>();

            var dataList = _context.MasterMenu.ToList();
            foreach(var data in dataList)
            {
                var setData = new MenuItem
                {
                    LinkText = data.Name,
                    ActionName = "DynamicMenu",
                    ControllerName = "Home"
                };

                menuItems.Add(setData);
            }
            menu.Items = menuItems;

            return menu;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ErrorLogin()
        {
            Menu menu = null;
            var vm = new ViewModel() { Menu = menu };
            return View(vm);
        }

        public IActionResult ErrorAccess()
        {
            Menu menu = null;
            if (HttpContext.Session.GetString("UserName") != null && HttpContext.Session.GetString("Name") != null && HttpContext.Session.GetString("RoleId") != null && HttpContext.Session.GetString("RoleName") != null)
                menu = GetMenu();

            var vm = new ViewModel() { Menu = menu };
            return View(vm);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("RoleId");
            HttpContext.Session.Remove("RoleName");
            return RedirectToAction("Login");
        }
    }
}
