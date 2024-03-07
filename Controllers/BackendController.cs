using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LeakHunter.Models;

namespace LeakHunter.Controllers
{
    public class BackendController : Controller
    {
        private readonly HunterContext DB;

        public BackendController(HunterContext _DB)
        {
            DB = _DB;

        }
        public bool CheckSession()
        {
            if (HttpContext.Session.GetInt32("UserId") != null || HttpContext.Session.GetInt32("admin") != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<IActionResult> Index()
        {
            if (CheckSession())
            {
                var Announment = await DB.Announments.ToListAsync();
                var all = from m in DB.Announments select m;
                return View(all);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public async Task<IActionResult> Announment_Content(int id)
        {
            var content = await DB.Announments.Where(x => x.AnnoId == id).FirstAsync();
            return View(content);
        }

        public async Task<IActionResult> Announment_Manage()
        {
            var manage_content = await DB.Announments.ToListAsync();
            return View(manage_content);
        }

        public async Task<IActionResult> Announment_Edit(int id)
        {
            var message = await DB.Announments.Where(x => x.AnnoId == id).FirstOrDefaultAsync();
            return View(message);
        }

        public async Task<IActionResult> Announment_Edit_run(int id, [Bind("AnnoId,Title,Content,Date")] Announment announment)
        {

            if (announment.Date == null)
            {
                announment.Date = DateTime.Now;
            }
            try
            {
                DB.Update(announment);
                await DB.SaveChangesAsync();
                return RedirectToAction("Announment_Manage", "Backend");
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.EditError = "修改失敗";
                return RedirectToAction("Announment_Edit", "Backend", new { id = id });
            }

        }
        public async Task<IActionResult> Announment_del(int id)
        {
            var message = await DB.Announments.Where(x => x.AnnoId == id).FirstAsync();
            DB.Announments.Remove(message);
            DB.SaveChanges();
            return RedirectToAction("Announment_Manage", "Backend");
        }

        public IActionResult Announment_Create()
        {
            return View();
        }

        public async Task<IActionResult> Announment_Instert([Bind("Title,Content")] Announment announment)
        {
            announment.Date = DateTime.Now;
            DB.Add(announment);
            await DB.SaveChangesAsync();
            return RedirectToAction("Announment_Manage", "Backend");
        }

        public async Task<IActionResult> Message_Manage()
        {
            List<User_Message> User_Message = new List<User_Message>();
            var message = from User in DB.Users
                          join Message in DB.Messages
                          on User.UId equals Message.UId
                          select new { User, Message };
            var ms = await message.ToListAsync();
            foreach (var odj in ms)
            {
                User_Message.Add(new User_Message()
                {
                    UId = odj.User.UId,
                    UName = odj.User.UName,
                    MesId = odj.Message.MesId,
                    UEmail = odj.User.UEmail,
                    Content = odj.Message.Content,
                    Date = odj.Message.Date,
                    Title = odj.Message.Title,
                });
            }
            return View(User_Message);
        }
        public IActionResult Message_Create()
        {
            List<User> user = new List<User>();
            var u = DB.Users.ToList();
            foreach (var odj in u)
            {
                user.Add(new User()
                {
                    UId = odj.UId,
                    UEmail = odj.UEmail,
                    UName = odj.UName,
                });
            }
            return View(user);
        }
        public async Task<IActionResult> Message_Instert(string UEmail, [Bind("Title,Content")] Message UMessage)
        {
            var message = DB.Users.Where(u => u.UEmail == UEmail).First();
            if (message != null)
            {
                UMessage.UId = message.UId;
                UMessage.Date = DateTime.Now;
                DB.Add(UMessage);
                await DB.SaveChangesAsync();
                return RedirectToAction("Message_Manage", "Backend");
            }
            ViewBag.state = "沒有該使用者";
            return RedirectToAction("Message_Create", "Backend");
        }
        public async Task<IActionResult> Message_Edit(int id)
        {
            var message = await DB.Messages.Where(x => x.MesId == id).FirstOrDefaultAsync();
            var user = await DB.Users.Where(x => x.UId == message.UId).FirstOrDefaultAsync();
            User_Message User_Message = new User_Message();
            User_Message.UId = user.UId;
            User_Message.Title = message.Title;
            User_Message.UName = user.UName;
            User_Message.UEmail = user.UEmail;
            User_Message.MesId = message.MesId;
            User_Message.Date = message.Date;
            User_Message.Content = message.Content;
            return View(User_Message);
        }

        public async Task<IActionResult> Message_Edit_run(int id, [Bind("MesId,UId,Content,Date,Title")] Message message)
        {
            if (message.Date == null)
            {
                message.Date = DateTime.Now;
            }
            try
            {
                DB.Update(message);
                await DB.SaveChangesAsync();
                return RedirectToAction("Message_Manage", "Backend"); ;
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.EditError = "修改失敗";
                return RedirectToAction("Message_Edit", "Backend", new { id = id });
            }
        }
        public async Task<IActionResult> Message_del(int id)
        {
            var message = await DB.Messages.Where(x => x.MesId == id).FirstAsync();
            DB.Messages.Remove(message);
            DB.SaveChanges();
            return RedirectToAction("Message_Manage", "Backend");
        }
        public async Task<IActionResult> Message()
        {
            if (CheckSession())
            {
                if (HttpContext.Session.GetInt32("UserId") != null)
                {
                    var Uid = HttpContext.Session.GetInt32("UserId");
                    var message = await DB.Messages.Where(x => x.UId == Uid).ToListAsync();
                    return View(message);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public async Task<IActionResult> Message_Content(int id)
        {
            if (CheckSession())
            {
                var content = await DB.Messages.Where(x => x.MesId == id).FirstAsync();
                return View(content);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public async Task<IActionResult> Analysis_Manage()
        {
            List<Analysis_User> analysis = new List<Analysis_User>();
            var content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          select new { Users, Hunter, Analyses };
            var ma = await content.ToListAsync();

            var qq = 1;
            foreach (var odj in ma)
            {
                if (qq != odj.Hunter.CId)
                {
                    qq = odj.Hunter.CId;

                    analysis.Add(new Analysis_User()
                    {
                        UId = odj.Users.UId,
                        AId = odj.Analyses.AId,
                        UEmail = odj.Users.UEmail,
                        UPassword = odj.Users.UPassword,
                        UName = odj.Users.UName,
                        CId = odj.Hunter.CId,
                        TId = odj.Analyses.TId,
                        Content = odj.Analyses.Content,
                        Url = odj.Hunter.Url,
                        WebName = odj.Hunter.WebName
                    });
                }

            }
            return View(analysis);
        }
        [HttpPost]
        public async Task<IActionResult> Analysis_Manage(string UEmail, string WebName, string Url)
        {

            List<Analysis_User> analysis = new List<Analysis_User>();
            var content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          select new { Users, Hunter, Analyses };
            if (UEmail != null && WebName != null && Url != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Users.UEmail == UEmail && Hunter.WebName == WebName && Hunter.Url == Url
                          select new { Users, Hunter, Analyses };

            }
            else if (UEmail != null && WebName != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Users.UEmail == UEmail && Hunter.WebName == WebName
                          select new { Users, Hunter, Analyses };
            }
            else if (WebName != null && Url != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Hunter.WebName == WebName && Hunter.Url == Url
                          select new { Users, Hunter, Analyses };
            }
            else if (UEmail != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Users.UEmail == UEmail
                          select new { Users, Hunter, Analyses };
            }
            else if (WebName != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Hunter.WebName == WebName
                          select new { Users, Hunter, Analyses };
            }
            else if (Url != null)
            {
                content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Hunter.Url == Url
                          select new { Users, Hunter, Analyses };
            }
            var ma = await content.ToListAsync();
            foreach (var odj in ma)
            {
                analysis.Add(new Analysis_User()
                {
                    UId = odj.Users.UId,
                    AId = odj.Analyses.AId,
                    UEmail = odj.Users.UEmail,
                    UPassword = odj.Users.UPassword,
                    UName = odj.Users.UName,
                    CId = odj.Hunter.CId,
                    TId = odj.Analyses.TId,
                    Content = odj.Analyses.Content,
                    Url = odj.Hunter.Url,
                    WebName = odj.Hunter.WebName
                });
            }
            if (analysis.Count > 0)
            {
                return View(analysis);

            }
            else
            {
                return Redirect("Analysis_Manage");
            }
        }
        //public async Task<ActionResult> Analysis_Delete(int id)
        //{
        //    var a = DB.Analyses.Where(x => x.AId == id).First();
        //    return Redirect("Analysis_Manage");
        //}

        public async Task<IActionResult> Analysis_Content(int id)
        {
            Analysis_User a = new Analysis_User();
            var content = from Analyses in DB.Analyses
                          join Hunter in DB.Hunters
                          on Analyses.CId equals Hunter.CId
                          join Users in DB.Users
                          on Hunter.UId equals Users.UId
                          where Analyses.AId == id
                          select new { Users, Hunter, Analyses };
            var odj = await content.FirstOrDefaultAsync();
            if (odj != null)
            {
                a.UId = odj.Users.UId;
                a.AId = odj.Analyses.AId;
                a.UEmail = odj.Users.UEmail;
                a.UPassword = odj.Users.UPassword;
                a.UName = odj.Users.UName;
                a.CId = odj.Hunter.CId;
                a.TId = odj.Analyses.TId;
                a.Content = odj.Analyses.Content;
                a.WContent = odj.Hunter.Content;
                a.Url = odj.Hunter.Url;
                a.Time = odj.Hunter.Time;
                a.WebName = odj.Hunter.WebName;
                return View(a);
            }
            else
            {
                return Redirect("Analysis_Manage");
            }
        }
    }
}
