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
                int maNguoiDung;
                if (Session["use"] == null || !int.TryParse(Session["use"].ToString(), out maNguoiDung))
                {
                    // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
                    return RedirectToAction("DangNhap", "User");
                }

                // Lấy danh sách sản phẩm yêu thích của người dùng
                var sanphamyeuthich = db.SanPhamYeuThichs
                                        .Where(s => s.MaNguoiDung == maNguoiDung)
                                        .Include(s => s.SanPham) // Bao gồm thông tin sản phẩm
                                        .ToList();

                // Trả về danh sách sản phẩm yêu thích vào view
                return View(sanphamyeuthich);
            }
            catch (Exception ex)
            {
                // Gửi thông báo lỗi tới view
                ViewBag.ErrorMessage = ex.Message;
                return View("Error"); // Hiển thị trang lỗi nếu xảy ra vấn đề
            }
        }

        // Thêm sản phẩm vào danh sách yêu thích
        public ActionResult ThemSanPhamYeuThich(int masp)
        {
            try
            {
                // Kiểm tra người dùng đã đăng nhập
                int maNguoiDung;
                if (Session["MaNguoiDung"] == null || !int.TryParse(Session["MaNguoiDung"].ToString(), out maNguoiDung))
                {
                    // Chuyển hướng về trang đăng nhập nếu chưa đăng nhập
                    return RedirectToAction("DangNhap", "User");
                }

                // Kiểm tra sản phẩm đã tồn tại trong danh sách yêu thích chưa
                bool daTonTai = db.SanPhamYeuThichs.Any(s => s.MaNguoiDung == maNguoiDung && s.Masp == masp);
                if (!daTonTai)
                {
                    // Thêm sản phẩm vào danh sách yêu thích
                    var sanPhamYeuThich = new SanPhamYeuThich
                    {
                        MaNguoiDung = maNguoiDung,
                        Masp = masp
                    };
                    db.SanPhamYeuThichs.Add(sanPhamYeuThich);
                    db.SaveChanges();
                }

                // Quay lại danh sách yêu thích
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Gửi thông báo lỗi tới view
                ViewBag.ErrorMessage = ex.Message;
                return View("Error"); // Hiển thị trang lỗi nếu xảy ra vấn đề
            }
        }

        // Dispose DbContext để giải phóng tài nguyên
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
