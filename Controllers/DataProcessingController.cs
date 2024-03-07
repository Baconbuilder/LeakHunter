using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using LeakHunter.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Web;
using LeakHunter.Models.Statisics_PostAnalysis;

namespace LeakHunter.Controllers
{
    public class DataProcessingController : Controller
    {
        private readonly HunterContext DB;

        public DataProcessingController(HunterContext _DB)
        {
            DB = _DB;
            conn.ConnectionString = LeakHunter.Properties.Resources.ConnectionString;
        }
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        SqlConnection conn = new SqlConnection();
        List<User> userinfo = new List<User>();
        List<Hunter> Hunterinfo = new List<Hunter>();
        List<PostPreview> postinfo = new List<PostPreview>();
        List<PostAnalysis> PAnalysis = new List<PostAnalysis>();

        public IActionResult SetInterval()
        {
            return View("SetInterval");
        }

        public IActionResult ScanRecords()
        {
            fetchData();
            return View(userinfo);
        }

        public IActionResult PreAnalysis()
        {
            getHunterData();
            return View(Hunterinfo);

        }
        public IActionResult PostAnalysis()
        {
            getPostData();
            return View(postinfo);

        }

        public IActionResult Detect()
        {
            return View();
        }

        [HttpPost]
        public IActionResult detectInfo(string urlName, string Url)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"Detect.py";
            var id = (int)HttpContext.Session.GetInt32("UserId");
            //var typeID = 1; // typeId 1 = names
            //var end = "10";

            psi.Arguments = $"\"{script}\" \"{Url}\" \"{urlName}\" \"{id}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            var process = Process.Start(psi);
            process.WaitForExit();
            TempData["urlName"] = urlName;
            TempData["Url"] = Url;
            return View("Temp");

        }

        [HttpPost]
        public IActionResult show()
        {
            getHunterData();
            return RedirectToAction("PreAnalysis", "DataProcessing");
        }

        public IActionResult show2()
        {
            getPostData();
            return RedirectToAction("PostAnalysis", "DataProcessing");
        }

        private void fetchData()
        {
            if (userinfo.Count > 0)
            {
                userinfo.Clear();
            }
            try
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                conn.Open();
                com.Connection = conn;
                com.CommandText = $"SELECT TOP (1000) [U_Email],[U_Password],[U_Name],[Phone_Number]FROM[Hunter].[dbo].[User] WHERE [U_ID]={id}";
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    userinfo.Add(new User()
                    {
                        UEmail = dr["U_Email"].ToString()
                    ,
                        UPassword = dr["U_Password"].ToString()
                    ,
                        UName = dr["U_Name"].ToString()
                    ,
                        PhoneNumber = dr["Phone_Number"].ToString()
                    });
                }
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void getHunterData()
        {
            try
            {
                int id = (int)HttpContext.Session.GetInt32("UserId");
                conn.Open();
                com.Connection = conn;
                com.CommandText = $"SELECT TOP (1) [C_ID],[Time],[Content],[URL],[Web_name] FROM[Hunter].[dbo].[Hunter] WHERE [U_ID]={id} ORDER BY [Time] DESC";
                //  ORDER BY [Time] DESC
                dr = com.ExecuteReader();
                while (dr.Read())
                {
                    Hunterinfo.Add(new Hunter()
                    {
                        CId = (int)dr["C_ID"]
                    ,
                        Time = (DateTime?)dr["Time"]
                    ,
                        Content = dr["Content"].ToString()
                    ,
                        Url = dr["URL"].ToString()
                    ,
                        WebName = dr["Web_name"].ToString()

                    });
                }
                conn.Close();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void getPostData()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var message = from Hunter in DB.Hunters
                          where Hunter.UId == id
                          select new { Hunter};
            var ms = message.ToList();
            ms.Reverse();

            int cid = ms[0].Hunter.CId;
            var Hunter = DB.Hunters.Where(x => x.CId == cid).FirstOrDefault();
            var name = DB.Analyses.Where(x => x.CId == cid && x.TId == 1).FirstOrDefault();
            var phone = DB.Analyses.Where(x => x.CId == cid && x.TId == 2).FirstOrDefault();
            var email = DB.Analyses.Where(x => x.CId == cid && x.TId == 3).FirstOrDefault();
            var address = DB.Analyses.Where(x => x.CId == cid && x.TId == 4).FirstOrDefault();

            postinfo.Add(new PostPreview()
            {
                Time = Hunter.Time,
                Url = Hunter.Url,
                WebName = Hunter.WebName,
                Name = name.Content,
                Phone = phone.Content,
                Email = email.Content,
                Address = address.Content

            });



        }

        public async Task<IActionResult> UserRecords_Index()
        {
            if (PAnalysis.Count > 0)
            {
                PAnalysis.Clear();
            }
            int id = (int)HttpContext.Session.GetInt32("UserId");
            var message = from Hunter in DB.Hunters
                          join Analysis in DB.Analyses
                          on Hunter.CId equals Analysis.CId
                          where Hunter.UId == id
                          select new { Hunter, Analysis };
            var ms = await message.ToListAsync();
            ms.Reverse();
            var qq = 1;
            foreach (var odj in ms)
            {
                if (qq != odj.Hunter.CId)
                {
                    qq = odj.Hunter.CId;

                    PAnalysis.Add(new PostAnalysis()
                    {
                        AId = odj.Analysis.AId,
                        CId = odj.Hunter.CId,
                        TId = odj.Analysis.TId,
                        PreContent = odj.Hunter.Content,
                        PostContent = odj.Analysis.Content,
                        Time = odj.Hunter.Time,
                        Url = odj.Hunter.Url,
                        WebName = odj.Hunter.WebName,
                        Count = odj.Analysis.Count
                    });
                }

            }
            return View(PAnalysis);
        }

        public PostAnalysis PreContent_new(Analysis analysis,Hunter Hunter)
        {
            PostAnalysis PostAnalysis = new PostAnalysis();
            PostAnalysis.AId = analysis.AId;
            PostAnalysis.CId = Hunter.CId;
            PostAnalysis.TId = analysis.TId;
            PostAnalysis.PreContent = Hunter.Content;
            PostAnalysis.PostContent = analysis.Content;
            PostAnalysis.Time = Hunter.Time;
            PostAnalysis.Url = Hunter.Url;
            PostAnalysis.WebName = Hunter.WebName;
            PostAnalysis.Count = analysis.Count;
            return PostAnalysis;
        }
        public async Task<IActionResult> PreContent(int id)
        {
            var analysis = await DB.Analyses.Where(x => x.AId == id).FirstOrDefaultAsync();
            var Hunter = await DB.Hunters.Where(x => x.CId == analysis.CId).FirstOrDefaultAsync();
            PostAnalysis PostAnalysis = new PostAnalysis();
            PostAnalysis.AId = analysis.AId;
            PostAnalysis.CId = Hunter.CId;
            PostAnalysis.TId = analysis.TId;
            PostAnalysis.PreContent = Hunter.Content;
            PostAnalysis.PostContent = analysis.Content;
            PostAnalysis.Time = Hunter.Time;
            PostAnalysis.Url = Hunter.Url;
            PostAnalysis.WebName = Hunter.WebName;
            PostAnalysis.Count = analysis.Count;
            return View(PostAnalysis);
        }

        public async Task<IActionResult> PostContent(int id)
        {
            var analysis = await DB.Analyses.Where(x => x.AId == id).FirstOrDefaultAsync();
            var Hunter = await DB.Hunters.Where(x => x.CId == analysis.CId).FirstOrDefaultAsync();
            PostAnalysis PostAnalysis = new PostAnalysis();
            PostAnalysis.AId = analysis.AId;
            PostAnalysis.CId = Hunter.CId;
            PostAnalysis.TId = analysis.TId;
            PostAnalysis.PreContent = Hunter.Content;
            PostAnalysis.PostContent = analysis.Content;
            PostAnalysis.Time = Hunter.Time;
            PostAnalysis.Url = Hunter.Url;
            PostAnalysis.WebName = Hunter.WebName;
            PostAnalysis.Count = analysis.Count;
            return View(PostAnalysis);
        }
        public async Task<IActionResult> UserRecords_class(int id)
        {
            SPA spa = new SPA();
            var name = await DB.Analyses.Where(x => x.CId == id && x.TId == 1).FirstOrDefaultAsync();
            var phone = await DB.Analyses.Where(x => x.CId == id && x.TId == 2).FirstOrDefaultAsync();
            var email = await DB.Analyses.Where(x => x.CId == id && x.TId == 3).FirstOrDefaultAsync();
            var address = await DB.Analyses.Where(x => x.CId == id && x.TId == 4).FirstOrDefaultAsync();
            Statistics Statistics_data = new Statistics();
            if (name != null || phone != null || email != null || address != null)
            {
                Statistics_data.CId = id;
                Statistics_data.name = (int)name.Count;
                Statistics_data.phone = (int)phone.Count;
                Statistics_data.email = (int)email.Count;
                Statistics_data.address = (int)address.Count;

                TempData["name_count"] = name.Count;
                TempData["phone_count"] = phone.Count;
                TempData["email_count"] = email.Count;
                TempData["address_count"] = address.Count;

                TempData["name_id"] = (int)name.AId;
                TempData["phone_id"] = (int)phone.AId;
                TempData["email_id"] = (int)email.AId;
                TempData["address_id"] = (int)address.AId;
                
                spa.statistics = Statistics_data;
                int[] Count = { (int)name.Count, (int)phone.Count, (int)email.Count, (int)address.Count };
                int[] id2 = { (int)name.AId, (int)phone.AId, (int)email.AId, (int)address.AId };
                for (int i = 0; i < 4; i++)
                {
                    if(Count[i] != 0)
                    {
                        var analysis = await DB.Analyses.Where(x => x.AId == id2[i]).FirstOrDefaultAsync();
                        var Hunter = await DB.Hunters.Where(x => x.CId == analysis.CId).FirstOrDefaultAsync();

                        switch (i)
                        {
                            case 0:
                                spa.postAnalysis1 = PreContent_new(analysis, Hunter);
                                break;
                            case 1:
                                spa.postAnalysis2 = PreContent_new(analysis, Hunter);
                                break;
                            case 2:
                                spa.postAnalysis3 = PreContent_new(analysis, Hunter);
                                break;
                            case 3:
                                spa.postAnalysis4 = PreContent_new(analysis, Hunter);
                                break;
                        }
                        
                    }
                }
            }
            else
            {
                Statistics_data.name = id;
                Statistics_data.phone = id;
                Statistics_data.email = id;
                Statistics_data.address = id;
                spa.statistics = Statistics_data;
            }
            TempData["user_id"] = id;

            //-------------------------------
            
            

            return View(spa);
        }

        public IActionResult Excel()
        {
            return View();
        }

        public IActionResult export(int id)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"export.py";
            //var typeID = 1; // typeId 1 = names
            //var end = "10";

            psi.Arguments = $"\"{script}\" \"{id}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            var process = Process.Start(psi);
            process.WaitForExit();
            
            return RedirectToAction("UserRecords_class", "DataProcessing", new { id = id });

        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult uploadFile(string urlName)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"FileUpload.py";
            var id = (int)HttpContext.Session.GetInt32("UserId");
            //var typeID = 1; // typeId 1 = names
            //var end = "10";

            psi.Arguments = $"\"{script}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            
            var process = Process.Start(psi);
            process.WaitForExit();
            return RedirectToAction("User_Interval", "Interval");

        }

        public IActionResult current(int id)
        {

            //string link = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string baseUrl = string.Format("{0}://{1}/DataProcessing/UserRecords_class/{2}",
                        HttpContext.Request.Scheme, HttpContext.Request.Host, id);

            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Users\USER\AppData\Local\Programs\Python\Python310\python.exe";

            // 2) Provide script and arguments
            var script = @"pdfDownload.py";
            //var typeID = 1; // typeId 1 = names
            //var end = "10";

            psi.Arguments = $"\"{script}\" \"{baseUrl}\"";

            // 3) Process configuration
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;



            var process = Process.Start(psi);
            process.WaitForExit();

            return RedirectToAction("UserRecords_class", "DataProcessing", new { id = id });

        }
    }
}
