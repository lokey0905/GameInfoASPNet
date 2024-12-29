using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using prj_web.Models;

namespace prj_web.Controllers
{
    public class RssItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }
        public DateTime PubDate { get; set; }
    }

    public class HomeController : Controller
    {
        userEntities db = new userEntities();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        //Get: Home/Login
        public ActionResult Login()
        {
            return View();
        }
        //Post: Home/Login
        [HttpPost]
        public ActionResult Login(string fUserId, string fPwd)
        {
            // 依帳密取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == fUserId && m.fPwd == fPwd)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                ViewBag.Message = "帳密錯誤，登入失敗";
                return View();
            }
            //使用Session變數記錄歡迎詞
            if (member.fIsAdmin == true)
            {
                Session["WelCome"] = member.fName + "管理員，歡迎光臨";
            }
            else
            {
                Session["WelCome"] = member.fName + "歡迎光臨";
            }
            FormsAuthentication.RedirectFromLoginPage(fUserId, true);
            return RedirectToAction("Index", "Member");
        }
        //Get:Home/Register
        public ActionResult Register()
        {
            return View();
        }
        //Post:Home/Register
        [HttpPost]
        public ActionResult Register(tMember pMember)
        {
            //若模型沒有通過驗證則顯示目前的View
            if (ModelState.IsValid == false)
            {
                return View();
            }
            // 依帳號取得會員並指定給member
            var member = db.tMember
                .Where(m => m.fUserId == pMember.fUserId)
                .FirstOrDefault();
            //若member為null，表示會員未註冊
            if (member == null)
            {
                //預設不是管理員
                pMember.fIsAdmin = false;

                //將會員記錄新增到tMember資料表
                db.tMember.Add(pMember);
                db.SaveChanges();
                //執行Home控制器的Login動作方法
                return RedirectToAction("Login");
            }
            ViewBag.Message = "此帳號己有人使用，註冊失敗";
            return View();
        }


        public ActionResult LatestNews(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://www.4gamers.com.tw/rss/latest-news");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("media", "http://search.yahoo.com/mrss/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["description"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    // 縮圖
                    XmlNode mediaNode = node.SelectSingleNode("media:content",nsmgr);
                    if (mediaNode != null)
                    {
                        var urlAttr = mediaNode.Attributes["url"];
                        if (urlAttr != null)
                        {
                            item.ImageUrl = urlAttr.Value;
                        }
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }

        public ActionResult Steam(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://freesteam.games/category/steam/feed");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["content:encoded"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }
        public ActionResult Epic(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://freesteam.games/category/epic-games/feed");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["content:encoded"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }

        public ActionResult Ubisoft(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://freesteam.games/category/ubisoft/feed");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["content:encoded"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }
        public ActionResult EA(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://freesteam.games/category/electronic-arts/feed");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["content:encoded"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }

        public ActionResult GOG(bool isPartial = false)
        {
            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://freesteam.games/category/gog/feed");

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("content", "http://purl.org/rss/1.0/modules/content/");

            XmlNodeList itemNodes = xmlDoc.SelectNodes("//channel/item");
            if (itemNodes != null)
            {
                foreach (XmlNode node in itemNodes)
                {
                    RssItem item = new RssItem();

                    item.Title = node["title"]?.InnerText ?? "";            // 標題
                    item.Link = node["link"]?.InnerText ?? "";              // 文章連結
                    item.Description = node["content:encoded"]?.InnerText ?? "";// 內文

                    // 發布日期
                    var pubDateNode = node["pubDate"]?.InnerText ?? "";
                    DateTime pubDt;
                    if (DateTime.TryParse(pubDateNode, null, DateTimeStyles.AdjustToUniversal, out pubDt))
                    {
                        item.PubDate = pubDt;
                    }

                    rssModel.Add(item);
                }
            }

            ViewBag.isPartial = isPartial;
            return View(rssModel);
        }
    }
}