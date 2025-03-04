using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;
using System.Data.Entity;
using Newtonsoft.Json;

namespace Ictshop.Areas.Admin.Controllers
{
    public class ThongkesController : Controller
    {
        private Qlbanhang db = new Qlbanhang();

        // GET: Admin/Thongkes
        public ActionResult Index()
        {
            // Thống kê Top 5 khách hàng
            var dataThongke = (from s in db.Donhangs
                               join x in db.Nguoidungs on s.MaNguoidung equals x.MaNguoiDung
                               group s by s.MaNguoidung into g
                               select new Thongke
                               {
                                   Tennguoidung = g.FirstOrDefault().Nguoidung.Hoten,
                                   Tongtien = g.Sum(x => x.Tongtien),
                                   Dienthoai = g.FirstOrDefault().Nguoidung.Dienthoai,
                                   Soluong = g.Count()
                               }).OrderByDescending(s => s.Tongtien).Take(5).ToList();

            ViewBag.TopKhachHang = dataThongke;

            // Doanh thu theo tháng
            var doanhThuTheoThang = db.Donhangs
                .Where(x => x.Ngaydat.HasValue)
                .GroupBy(x => new { x.Ngaydat.Value.Year, x.Ngaydat.Value.Month })
                .Select(g => new
                {
                    Thang = g.Key.Month,
                    Nam = g.Key.Year,
                    Tongtien = g.Sum(x => x.Tongtien)
                })
                .OrderBy(g => g.Nam).ThenBy(g => g.Thang).ToList();

            ViewBag.DoanhThuThang = JsonConvert.SerializeObject(doanhThuTheoThang);

            // Doanh thu theo tuần
            var doanhThuTheoTuan = db.Donhangs
                .Where(x => x.Ngaydat.HasValue)
                .AsEnumerable() // Fix lỗi LINQ
                .GroupBy(x => new
                {
                    x.Ngaydat.Value.Year,
                    Tuan = (x.Ngaydat.Value - new DateTime(x.Ngaydat.Value.Year, 1, 1)).Days / 7
                })
                .Select(g => new
                {
                    Nam = g.Key.Year,
                    Tuan = g.Key.Tuan,
                    Tongtien = g.Sum(x => x.Tongtien)
                })
                .OrderBy(g => g.Nam).ThenBy(g => g.Tuan).ToList();

            ViewBag.DoanhThuTuan = JsonConvert.SerializeObject(doanhThuTheoTuan);

            // Sản phẩm bán chạy (Top 5)
            var sanphamBanChay = db.Chitietdonhangs
                .GroupBy(x => x.Masp)
                .Select(g => new
                {
                    Sanpham = g.FirstOrDefault().Sanpham.Tensp,
                    Soluong = g.Sum(x => x.Soluong)
                })
                .OrderByDescending(g => g.Soluong)
                .Take(5).ToList();

            ViewBag.SanphamBanChay = JsonConvert.SerializeObject(sanphamBanChay);

            return View();
        }
      

    }
}
