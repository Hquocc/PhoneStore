using System;
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
            if (ModelState.IsValid)
            {
                try
                {
                    db.Nguoidungs.Add(nguoidung);
                    db.SaveChanges();
                    string verificationLink = Url.Action("VerifyEmail", "User", new { id = nguoidung.MaNguoiDung }, protocol: Request.Url.Scheme);
                    MailMessage mailMessage = new MailMessage
                    {
                        From = new MailAddress("vietviprp2003@gmail.com"),
                        Subject = "Xác minh email của bạn",
                        Body = $"Vui lòng nhấn vào liên kết sau để xác minh tài khoản của bạn: {verificationLink}",
                        IsBodyHtml = true
                    };
                    mailMessage.To.Add(nguoidung.Email);

                    SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                    {
                        Credentials = new NetworkCredential("vietviprp2003@gmail.com", "euexrfrwgjunivzp"),
                        EnableSsl = true
                    };
                    smtpClient.Send(mailMessage);

                    TempData["SuccessMessage"] = "Đăng ký thành công. Vui lòng kiểm tra email để xác minh tài khoản.";
                    return RedirectToAction("Dangnhap");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    TempData["ErrorMessage"] = "Đã có lỗi xảy ra. Vui lòng thử lại.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Đăng ký không thành công. Vui lòng kiểm tra lại!";
            }
            return View(nguoidung);
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
                TempData["Error"] = "Vui lòng kiểm tra lại email hoặc mật khẩu.";
                return View("Dangnhap");
            }
        }

        public ActionResult DangXuat()
        {
            Session["use"] = null;
            return RedirectToAction("Index", "Home");
        }

        // Quên mật khẩu
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
                return RedirectToAction("Dangnhap");
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

        // Reset mật khẩu
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
