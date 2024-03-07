using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using LeakHunter.Models;
using System.Data.SqlClient;

namespace LeakHunter.Controllers
{
    public class HomeController : Controller
    {
        private readonly HunterContext DB;

        public HomeController(HunterContext _DB)
        {
            DB = _DB;

        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult about()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login_Check(string Email,string Password)
        {
            if(Email=="admin" && Password == "aaa")
            {
                HttpContext.Session.SetString("admin", "admin");
                return RedirectToAction("Index", "Backend");
            }
            if (Email == null || Password == null)
            {
                return View("Login");
            }
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                var Check = DB.Users.Where(x => x.UEmail == Email && x.UPassword == Password).FirstOrDefault();
                if(Check != null)
                {
                    HttpContext.Session.SetInt32("UserId", Check.UId);
                    HttpContext.Session.SetString("UserName", Check.UName);
                    HttpContext.Session.SetString("Permission", Check.Permission);
                    @ViewBag.complete = "登入成功";
                    var n = DB.Managers.Where(x => x.Account == "admin").FirstOrDefault();
                    if (n.State == 0)
                    {
                        HttpContext.Session.SetInt32("State", 0);
                        return RedirectToAction("Index", "Backend");
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("State", 1);
                        return RedirectToAction("MaintenanceStatus_User", "StatusSetting");
                    }
                }
            }
            @ViewBag.complete = "登入失敗";
            return View("Login");
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register_Join(string UEmail,[Bind("UName,UEmail,UPassword,PhoneNumber")]User user)
        {
            if (DB.Users.Where(x => x.UEmail == UEmail).FirstOrDefault() == null)
            {
                if (user.UEmail != null && user.UName != null && user.UPassword != null)
                {
                    user.Permission = "Z";
                    DB.Add(user);
                    await DB.SaveChangesAsync();
                    @ViewBag.complete = "註冊成功";
                    return View("Login");
                }
                else
                {
                    @ViewBag.complete = "註冊失敗";
                    return View("Login");
                }
            }
            else
            {
                @ViewBag.complete = "Email重複註冊";
                return View("Login");
            }
        }

        public IActionResult Login_Out()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Remove("admin");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("Permission");
            return RedirectToAction("Login");
        }

        public IActionResult test(string Url)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\Williamko\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"C:\Users\Williamko\Desktop\Core\C#\LeakHunter\Web.py";
            var start = Url;
            //var end = "10";

            psi.Arguments = $"\"{script}\" \"{start}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            var process = Process.Start(psi);


            return Content(Url);
        }
    }
}
