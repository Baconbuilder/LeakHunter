using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using LeakHunter.Models;


namespace LeakHunter.Controllers
{
    public class StatusSettingController : Controller
    {
        private readonly HunterContext DB;
        public StatusSettingController(HunterContext _DB)
        {
            DB = _DB;
        }

        public IActionResult MaintenanceStatus_User()
        {
            return View();
        }

        public IActionResult StatusSetting_Manager()
        {
            var n = DB.Managers.Where(x => x.Account == HttpContext.Session.GetString("admin")).FirstOrDefault();
            if (n.State == 0)
            {
                TempData["state"] = "一般狀態";
            }
            else
            {
                TempData["state"] = "維修狀態";
            }
            return View(n);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StatusSetting_Manager(int id, [Bind("MId,Account,MPassword,MName,State")] Manager m)
        {
            try
            {
                DB.Update(m);
                await DB.SaveChangesAsync();
                TempData["success"] = "更新成功";
                return RedirectToAction("StatusSetting_Manager", "StatusSetting"); ;
            }
            catch (DbUpdateConcurrencyException)
            {
                TempData["unsuccess"] = "更新失敗";
                return RedirectToAction("StatusSetting_Manager", "StatusSetting", new { id = id });
            }
        }

    }
}
