using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using LeakHunter.Models;


namespace LeakHunter.Controllers
{
    public class MemberController : Controller
    {
        private readonly HunterContext DB;
        public MemberController(HunterContext _DB)
        {
            DB = _DB;
        }


        public IActionResult User_Self()
        {
            if (HttpContext.Session.GetInt32("UserId") != null)
            {
                var id = HttpContext.Session.GetInt32("UserId");
                return View(FById((int)id));
            }
            else
            {
                return View("Login");
            }
        }

        private object? FById(int? id)
        {
            throw new NotImplementedException();
        }

        public List<User> FById(int Id)
        {
            var N = DB.Users.Where(x => x.UId == Id).ToList();
            return N;
        }


        public IActionResult Search_User()
        {
            var all = from m in DB.Users select m;
            return View(all);
        }
        [HttpPost]//告訴這個funtion有傳值
        public async Task<IActionResult> Search_User(string? Name, string? Email, string? Per)
        {
            if (Name == null && Email == null && Per == "請選擇權限")
            {
                return RedirectToAction("Search_User", "Member");
            }
            else
            {
                var ans = DB.Users.Where(x => x.Permission == Per || x.UEmail == Email || x.UName == Name).ToList();
                return View(ans);
            }
        }

        public IActionResult User_Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //var categoryFromDb = _db.Categories.Find(id);
            var re = DB.Users.Find(id);
            //var categoryFromDbSingle = _db.Categories.SingleOrDefault(u => u.Id == id);

            if (re == null)
            {
                return NotFound();
            }

            return View(re);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> User_Edit(int id, [Bind("UId,UName,UPassword,UEmail,Permission,PhoneNumber")] User user)
        {
            try
            {
                DB.Update(user);
                await DB.SaveChangesAsync();
                TempData["success"] = "Userdata updated successfully";
                return RedirectToAction("User_Edit", "Member"); ;
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["unsuccess"] = "Userdata updated unsuccessfully";
                return RedirectToAction("User_Edit", "Member", new { id = id });
            }
        }
    }
}
