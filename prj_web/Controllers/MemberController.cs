using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using Microsoft.Ajax.Utilities;
using prj_web.Models;

namespace prj_web.Controllers
{
    [Authorize]  //指定Member控制器所有的動作方法必須通過授權才能執行。
    public class MemberController : Controller
    {
        userEntities db = new userEntities();

        public ActionResult Index()
        {
            // 檢查會員是否滿18歲
            var userId = User.Identity.Name; // 或從 Session/Claims 中取得
  
            using (var db = new userEntities()) // 替換為你的資料庫上下文
            {
                // 從資料庫中取得會員資料
                var member = db.tMember.SingleOrDefault(m => m.fUserId == userId);
                if (member == null)
                {
                    return HttpNotFound(); // 如果找不到會員，返回 404
                }

                if (member.fBirth.Value.AddYears(18) > DateTime.Now)
                {
                    Session["IsAdult"] = false;
                }
                else
                {
                    Session["IsAdult"] = true;
                }

                db.SaveChanges();
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();   // 登出
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Edit()
        {
            // 假設使用已登入的會員 ID
            var userId = User.Identity.Name; // 或從 Session/Claims 中取得
            using (var db = new userEntities()) // 替換為你的資料庫上下文
            {
                // 從資料庫中取得會員資料
                var member = db.tMember.SingleOrDefault(m => m.fUserId == userId);
                if (member == null)
                {
                    return HttpNotFound(); // 如果找不到會員，返回 404
                }

                bool isAdmin = member.fIsAdmin;
                ViewBag.fIsAdmin = isAdmin;

                if (isAdmin)
                {
                    return View("EditAdmin", member); // Admin 使用特定的 View
                }
                else
                {
                    return View("Edit", member); // 一般會員使用另一個 View
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tMember model,bool IsAdmin = false)
        {
            if (ModelState.IsValid)
            {
                using (var db = new userEntities()) // 替換為你的資料庫上下文
                {
                    // 取得現有的會員資料
                    var member = db.tMember.SingleOrDefault(m => m.fId == model.fId);
                    if (member == null)
                    {
                        return HttpNotFound(); // 如果找不到會員，返回 404
                    }

                    // 更新資料
                    member.fPwd = model.fPwd;
                    member.fName = model.fName;
                    member.fEmail = model.fEmail;
                    member.fBirth = model.fBirth;

                    /*if (IsAdmin)
                    {
                        //member.fIsAdmin = model.fIsAdmin;
                        member.fIsAdult = model.fIsAdult;
                    }*/

                    // 儲存更改
                    db.SaveChanges();

                    // 返回列表或成功訊息頁面
                    return RedirectToAction("Index");
                }
            }

            if (IsAdmin)
            {
                return View("EditAdmin", model); // Admin 使用特定的 View
            }
            else
            {
                return View("Edit", model); // 一般會員使用另一個 View
            }
        }

        public ActionResult AdultOnly(bool isPartial = false)
        {
            var userId = User.Identity.Name; // 或從 Session/Claims 中取得
            using (var db = new userEntities()) // 替換為你的資料庫上下文
            {
                // 從資料庫中取得會員資料
                var member = db.tMember.SingleOrDefault(m => m.fUserId == userId);
                if (member == null)
                {
                    return HttpNotFound(); // 如果找不到會員，返回 404
                }

                if (member.fBirth.Value.AddYears(18) > DateTime.Now && member.fIsAdmin != true)
                {
                    Session["WelCome"] = member.fName + " 該帳號沒有讀取權限:(";
                    return RedirectToAction("Index");
                }
            }

            var rssModel = new List<RssItem>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("https://hgamefree.info/tag/illusion/feed");

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