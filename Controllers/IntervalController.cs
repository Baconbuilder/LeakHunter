using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using LeakHunter.Models;

namespace LeakHunter.Controllers
{
    public class IntervalController : Controller
    {
        private readonly HunterContext DB;

        public IntervalController(HunterContext _DB)
        {
            DB = _DB;

        }

        public IActionResult SetInterval()
        {
            DateTime date = DateTime.Now;
            TempData["date"] = date;
            return View();
        }
        public async Task<IActionResult> Set_Interval(string WebName,string Url,int time)
        {
            var check = DB.Intervals.ToList();
            bool answer = false;
            foreach (var s in check)
            {
                if(s.WebName == WebName)
                {
                    answer = true;
                    break;
                }
            }
            if (answer != true)
            {
                Interval interval = new Interval();
                DateTime date_1 = new DateTime(2022, 07, 28, 22, 20, 00);
                DateTime date_3 = DateTime.Now;
                switch (time)
                {
                    case 1:
                        date_3 = date_3.AddDays(1);
                        break;
                    case 2:
                        date_3 = date_3.AddDays(7);
                        break;
                    case 3:
                        date_3 = date_3.AddMonths(1);
                        break;
                    case 4:
                        date_3 = date_3.AddYears(1);
                        break;
                }



                interval.UId = HttpContext.Session.GetInt32("UserId");
                interval.WebName = WebName;
                interval.Url = Url;
                interval.Next = date_3;
                interval.Day = time;
                DB.Add(interval);
                await DB.SaveChangesAsync();

                //Content(((date_1 - date_2).TotalMinutes).ToString())
                @ViewBag.interval = "設定成功";

                var psi = new ProcessStartInfo();
                var urlName = interval.WebName;
                psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

                // 2) Provide script and arguments
                var script = @"Detect.py";
                var id = interval.UId;
                var typeID = 1; // typeId 1 = names
                                //var end = "10";

                psi.Arguments = $"\"{script}\" \"{Url}\" \"{urlName}\" \"{id}\" \"{typeID}\"";

                // 3) Process configuration
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;



                var process = Process.Start(psi);
                process.WaitForExit();
                TempData["urlName"] = urlName;
                TempData["Url"] = Url;

                return RedirectToAction("User_Interval", "interval", interval);
            }
            else
            {
                ViewBag.check = "網頁標題重複";
                return View("SetInterval");
            }
            

        }


        public IActionResult Run_Interval()
        {
            Interval interval = new Interval();

            var a = DB.Intervals.OrderByDescending(x=>x.Next).FirstOrDefault();
            interval.Next = a.Next;
            DateTime date_1 = (DateTime)interval.Next;
            DateTime date_3 = DateTime.Now;
            var time = Math.Round((date_1 - date_3).TotalSeconds);
            if (time <= 0)
            {
                ViewBag.time = 0;
            }
            else if(time > 600)
            {
                ViewBag.time = 600;
            }
            else 
            {
                ViewBag.time = time;
            }
            return View(interval);
        }
        public IActionResult Run_DataProcessing()
        {
            Interval interval = new Interval();

            var a = DB.Intervals.OrderByDescending(x => x.Next).FirstOrDefault();
            interval.UId = a.UId;
            interval.Url = a.Url;
            interval.WebName = a.WebName;
            interval.IId = a.IId;
            interval.Next = a.Next;
            interval.Day = a.Day;
            DateTime date_1 = (DateTime)interval.Next;
            DateTime date_3 = DateTime.Now;
            
            if(date_1 >= date_3)
            {
                return RedirectToAction("Run_Interval", "interval");
            }
            return RedirectToAction("detectInfo","interval",interval);
        }

        

        public async Task<IActionResult> Data_Processing_run(int id)
        {
            Interval interval = new Interval();
            var a = DB.Intervals.Where(x => x.IId == id).FirstOrDefault();
            
            if (a == null)
            {
                return View("User_Interval");
            }
            else
            {
                interval.UId = a.UId;
                interval.Url = a.Url;
                interval.WebName = a.WebName;
                interval.IId = a.IId;
                interval.Next = a.Next;
                interval.Day = a.Day;

                var psi = new ProcessStartInfo();
                var Url = a.Url;
                var urlName = a.WebName;
                psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

                // 2) Provide script and arguments
                var script = @"Detect.py";
                var Uid = a.UId;
                var typeID = 1; // typeId 1 = names
                                //var end = "10";

                psi.Arguments = $"\"{script}\" \"{Url}\" \"{urlName}\" \"{Uid}\" \"{typeID}\"";

                // 3) Process configuration
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;



                var process = Process.Start(psi);
                process.WaitForExit();
                TempData["urlName"] = urlName;
                TempData["Url"] = Url;
                return RedirectToAction("User_Interval", "interval", interval);
            }
        }
        public async Task<IActionResult> detectInfo(Interval interval)
        {
            var psi = new ProcessStartInfo();
            var Url = interval.Url;
            var urlName = interval.WebName;
            psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"Detect.py";
            var id = interval.UId;
            var typeID = 1; // typeId 1 = names
            //var end = "10";

            psi.Arguments = $"\"{script}\" \"{Url}\" \"{urlName}\" \"{id}\" \"{typeID}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            var process = Process.Start(psi);
            process.WaitForExit();
            TempData["urlName"] = urlName;
            TempData["Url"] = Url;

            DateTime date = (DateTime)interval.Next;
            switch (interval.Day)
            {
                case 1:
                    date = date.AddDays(1);
                    break;
                case 2:
                    date = date.AddDays(7);
                    break;
                case 3:
                    date = date.AddMonths(1);
                    break;
                case 4:
                    date = date.AddYears(1);
                    break;
            }

            interval.Next = date;
            DB.Update(interval);
            await DB.SaveChangesAsync();

            return RedirectToAction("Run_Interval", "interval", interval);

        }
        public async Task<IActionResult> User_Interval()
        {
            TempData["check_count"] = "";
            var user = DB.Users.Where(x => x.UId == HttpContext.Session.GetInt32("UserId")).First();
            if (user.Permission == "B")
            {
                var count = 0;
                var check_count = DB.Intervals.Where(x => x.UId == HttpContext.Session.GetInt32("UserId"));
                //var webname_check="";
                foreach (var c in check_count)
                {
                    //if (webname_check == "")
                    //{
                    //    webname_check=c.WebName;
                    //}
                    //else if(webname_check != c.WebName)
                    //{
                    count++;
                    //}
                }
                if (count >= 3)
                {
                    TempData["check_count"] = "Can not use";
                }
            }

            var User = HttpContext.Session.GetInt32("UserId");
            var User_Interval = await DB.Intervals.Where(x => x.UId == User).ToListAsync();
            return View(User_Interval);
        }

        public async Task<IActionResult> User_Interval_Edit(int id)
        {
            Interval interval1 = new Interval();
            interval1 = await DB.Intervals.Where(x => x.IId == id).FirstAsync();
            return View(interval1);
        }

        public async Task<IActionResult> User_Interval_Edit_Run( [Bind("IId,WebName,Url,Day")] Interval interval)
        {
            try
            {
                switch (interval.Day)
                {
                    case 1:
                        interval.Next = DateTime.Now.AddDays(1);
                        break ;
                    case 2:
                        interval.Next = DateTime.Now.AddDays(7);
                        break;
                    case 3:
                        interval.Next = DateTime.Now.AddMonths(1);
                        break;
                    case 4:
                        interval.Next = DateTime.Now.AddYears(1);
                        break;
                }   
                interval.UId = HttpContext.Session.GetInt32("UserId");
                DB.Update(interval);
                await DB.SaveChangesAsync();
                return RedirectToAction("User_Interval", "interval"); ;
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.EditError = "修改失敗";
                return RedirectToAction("User_Interval", "interval");
            }
        }

        public async Task<IActionResult> User_Interval_del(int id)
        {
            var message = await DB.Intervals.Where(x => x.IId == id).FirstAsync();
            DB.Intervals.Remove(message);
            DB.SaveChanges();
            return RedirectToAction("User_Interval", "interval" );
        }
        public async Task<IActionResult> Admin_Search_User_interval(int id)
        {
            var User = id;
            var User_Interval = await DB.Intervals.Where(x => x.UId == User).ToListAsync();
            ViewBag.id = id;
            return View(User_Interval);
        }
        public IActionResult Admin_SetInterval(int id)
        {
            ViewBag.id= id;
            return View();
        }
        public async Task<IActionResult> Admin_Set_Interval(int id, string WebName, string Url, int time)
        {
            Interval interval = new Interval();
            DateTime date_1 = new DateTime(2022, 07, 28, 22, 20, 00);
            DateTime date_3 = DateTime.Now;
            switch (time)
            {
                case 1:
                    date_3 = date_3.AddDays(1);
                    break;
                case 2:
                    date_3 = date_3.AddDays(7);
                    break;
                case 3:
                    date_3 = date_3.AddMonths(1);
                    break;
                case 4:
                    date_3 = date_3.AddYears(1);
                    break;
            }
            interval.UId = id;
            interval.WebName = WebName;
            interval.Url = Url;
            interval.Next = date_3;
            interval.Day = time;
            DB.Add(interval);
            await DB.SaveChangesAsync();

            //Content(((date_1 - date_2).TotalMinutes).ToString())
            @ViewBag.interval = "設定成功";
            return RedirectToAction("Admin_Search_User_interval", "Interval", new { id = id });
        }
        public async Task<IActionResult> Admin_Interval_del(int id)
        {
            var message = await DB.Intervals.Where(x => x.IId == id).FirstAsync();
            var i = message.UId;
            DB.Intervals.Remove(message);
            DB.SaveChanges();
            return RedirectToAction("Admin_Search_User_interval", "Interval", new { id = i });
        }
        public async Task<IActionResult> Admin_Interval_Edit(int id)
        {
            Interval interval1 = new Interval();
            interval1 = await DB.Intervals.Where(x => x.IId == id).FirstAsync();
            return View(interval1);
        }

        public async Task<IActionResult> Admin_Interval_Edit_Run(int UId,[Bind("IId,WebName,Url,Day")] Interval interval)
        {
            try
            {
                switch (interval.Day)
                {
                    case 1:
                        interval.Next = DateTime.Now.AddDays(1);
                        break;
                    case 2:
                        interval.Next = DateTime.Now.AddDays(7);
                        break;
                    case 3:
                        interval.Next = DateTime.Now.AddMonths(1);
                        break;
                    case 4:
                        interval.Next = DateTime.Now.AddYears(1);
                        break;
                }
                interval.UId = UId;
                DB.Update(interval);
                await DB.SaveChangesAsync();
                return RedirectToAction("Admin_Search_User_interval", "Interval", new { id = UId });
            }
            catch (DbUpdateConcurrencyException)
            {
                ViewBag.EditError = "修改失敗";
                return RedirectToAction("Admin_Search_User_interval", "Interval", new { id = UId });
            }
        }
        public async Task<IActionResult> Interval_History(string id)
        {
            var Hunter = await DB.Hunters.Where(x=>x.WebName==id).ToListAsync();
            return View(Hunter);
        }
    }
}
