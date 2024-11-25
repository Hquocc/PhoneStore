using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
namespace Ictshop.Controllers
{
    public class UserController : Controller
    {
        Qlbanhang db = new Qlbanhang();
        // ĐĂNG KÝ
        public ActionResult Dangky()
        {
            return View();
        }

        // ĐĂNG KÝ PHƯƠNG THỨC POST
        [HttpPost]
        public ActionResult Dangky(Nguoidung nguoidung)
        {
            try
            {
                // Thêm người dùng mới vào cơ sở dữ liệu
                db.Nguoidungs.Add(nguoidung);
                db.SaveChanges();

                if (ModelState.IsValid)
                {
                    // Gửi email xác minh
                    string verificationLink = Url.Action("VerifyEmail", "User", new { id = nguoidung.MaNguoiDung }, protocol: Request.Url.Scheme);

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("vietviprp2003@gmail.com");
                    mailMessage.To.Add(nguoidung.Email);
                    mailMessage.Subject = "Xác minh email của bạn";
                    mailMessage.Body = $"Vui lòng nhấn vào liên kết sau để xác minh tài khoản của bạn: {verificationLink}";
                    mailMessage.IsBodyHtml = true;
                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                    smtpClient.Credentials = new NetworkCredential("vietviprp2003@gmail.com", "euexrfrwgjunivzp");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(mailMessage);

                    return RedirectToAction("Dangnhap");
                }
                return View("Dangky");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult VerifyEmail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }

            nguoidung.IsVerified = true;
            db.SaveChanges();

            TempData["Success"] = "Email của bạn đã được xác minh thành công. Bạn có thể đăng nhập.";
            return RedirectToAction("Dangnhap");
        }

        public ActionResult Dangnhap()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userMail = User.Identity.Name;
                var nguoidung = db.Nguoidungs.SingleOrDefault(x => x.Email == userMail);

                if (nguoidung != null && !nguoidung.IsVerified)
                {
                    TempData["Error"] = "Bạn cần xác minh email của mình trước khi đăng nhập.";
                    return RedirectToAction("Dangnhap");
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult Dangnhap(FormCollection userlog)
        {

            string userMail = userlog["userMail"].ToString();
            string password = userlog["password"].ToString();
            var islogin = db.Nguoidungs.SingleOrDefault(x => x.Email.Equals(userMail) && x.Matkhau.Equals(password));

            if (islogin != null)
            {
                if (!islogin.IsVerified)
                {

                    TempData["Error"] = "Bạn cần xác minh email của mình trước khi đăng nhập.";
                    return RedirectToAction("Dangnhap");
                }
               
                if (userMail == "Admin@gmail.com")
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Admin/Home");
                }
                else
                {
                    Session["use"] = islogin;
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                TempData["Error1"] = "Vui lòng kiểm tra lại gmail hoặc mật khẩu.";
                return View("Dangnhap");
            }

        }
        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Profile(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoiDung = db.Nguoidungs.Find(id);
            if (nguoiDung == null)
            {
                return HttpNotFound();
            }
            return View(nguoiDung);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }

        // POST: Admin/Nguoidungs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNguoiDung,Hoten,Email,Dienthoai,Matkhau,IDQuyen, Anhdaidien,Diachi")] Nguoidung nguoidung)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nguoidung).State = EntityState.Modified;
                db.SaveChanges();
                //@ViewBag.show = "Chỉnh sửa hồ sơ thành công";
                //return View(nguoidung);
                return RedirectToAction("Profile", new { id = nguoidung.MaNguoiDung });

            }
            ViewBag.IDQuyen = new SelectList(db.PhanQuyens, "IDQuyen", "TenQuyen", nguoidung.IDQuyen);
            return View(nguoidung);
        }
        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [HttpPost]
        public ActionResult QuenMatKhau(string email)
        {
            var nguoidung = db.Nguoidungs.SingleOrDefault(x => x.Email == email);
            if (nguoidung == null)
            {
                TempData["Error"] = "Email không tồn tại.";
                return RedirectToAction("Dangnhap", "User"); 
            }

            string resetLink = Url.Action("ResetMatKhau", "User", new { id = nguoidung.MaNguoiDung }, protocol: Request.Url.Scheme);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("vietviprp2003@gmail.com");
            mailMessage.To.Add(email);
            mailMessage.Subject = "Yêu cầu khôi phục mật khẩu";
            mailMessage.Body = $"Vui lòng nhấn vào liên kết sau để khôi phục mật khẩu của bạn: {resetLink}";
            mailMessage.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
            smtpClient.Credentials = new NetworkCredential("vietviprp2003@gmail.com", "euexrfrwgjunivzp");
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);

            TempData["Success"] = "Một email khôi phục mật khẩu đã được gửi đến địa chỉ của bạn.";
            return RedirectToAction("Dangnhap");
        }
        public ActionResult ResetMatKhau(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }

            return View(nguoidung);
        }
        [HttpPost]
        public ActionResult ResetMatKhau(int id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới và mật khẩu xác nhận không khớp.";
                return View();
            }

            Nguoidung nguoidung = db.Nguoidungs.Find(id);
            if (nguoidung == null)
            {
                return HttpNotFound();
            }
            nguoidung.Matkhau = newPassword; 
            db.SaveChanges();

            TempData["Success"] = "Mật khẩu của bạn đã được khôi phục thành công.";
            return RedirectToAction("Dangnhap");
        }


    }
   
}