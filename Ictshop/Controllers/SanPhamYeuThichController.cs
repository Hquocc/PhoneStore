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
                                        .Include(s => s.Nguoidung) // Bao gồm thông tin người dùng (nếu cần)
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

        [HttpPost]
        public JsonResult Add(int masp)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại từ Session
                if (Session["MaNguoiDung"] == null)
                {
                    return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
                }

                int maNguoiDung = (int)Session["MaNguoiDung"];

                // Kiểm tra sản phẩm đã tồn tại trong danh sách yêu thích chưa
                var existing = db.SanPhamYeuThichs.FirstOrDefault(s => s.MaNguoiDung == maNguoiDung && s.Masp == masp);
                if (existing == null)
                {
                    // Thêm sản phẩm vào danh sách yêu thích
                    var favorite = new SanPhamYeuThich
                    {
                        MaNguoiDung = maNguoiDung,
                        Masp = masp
                    };
                    db.SanPhamYeuThichs.Add(favorite);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Sản phẩm đã được thêm vào danh sách yêu thích." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Remove(int masp)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại từ Session
                if (Session["MaNguoiDung"] == null)
                {
                    return Json(new { success = false, message = "Người dùng chưa đăng nhập." });
                }

                int maNguoiDung = (int)Session["MaNguoiDung"];

                // Tìm sản phẩm trong danh sách yêu thích
                var favorite = db.SanPhamYeuThichs.FirstOrDefault(s => s.MaNguoiDung == maNguoiDung && s.Masp == masp);
                if (favorite != null)
                {
                    // Xóa sản phẩm khỏi danh sách yêu thích
                    db.SanPhamYeuThichs.Remove(favorite);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi danh sách yêu thích." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
