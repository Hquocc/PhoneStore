using Ictshop.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;

namespace Ictshop.Controllers
{
    public class SanPhamYeuThichController : Controller
    {
        // Khởi tạo DbContext
        private Qlbanhang db = new Qlbanhang();

        // GET: SanPhamYeuThich
        public ActionResult Index()
        {
            try
            {
                // Kiểm tra người dùng đã đăng nhập
                if (Session["MaNguoiDung"] == null)
                {
                    return RedirectToAction("Login", "Account"); // Điều hướng đến trang đăng nhập nếu chưa đăng nhập
                }

                int maNguoiDung = (int)Session["MaNguoiDung"];

                // Lấy danh sách sản phẩm yêu thích của người dùng
                var sanphamyeuthich = db.SanPhamYeuThichs
                                        .Where(s => s.MaNguoiDung == maNguoiDung)                                  
                                        .Include(s => s.SanPham)   // Bao gồm thông tin sản phẩm
                                        .ToList();

                // Trả về danh sách vào view
                return View(sanphamyeuthich);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error"); // Hiển thị trang lỗi nếu xảy ra vấn đề
            }
        }

     

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
