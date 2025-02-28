using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ictshop.Models;

namespace Ictshop.Controllers
{
    public class SanphamController : Controller
    {
        Qlbanhang db = new Qlbanhang();

        // GET: Sanpham
        public ActionResult dtiphonepartial()
        {
            var ip = db.Sanphams.Where(n => n.Mahang == 2).Take(8).ToList();
            return PartialView(ip);
        }

        public ActionResult dtsamsungpartial()
        {
            var ss = db.Sanphams.Where(n => n.Mahang == 1).Take(12).ToList();
            return PartialView(ss);
        }
        public ActionResult dtxiaomipartial()
        {
            var mi = db.Sanphams.Where(n => n.Mahang == 3).Take(12).ToList();
            return PartialView(mi);
        }
        public ActionResult xemchitiet(int Masp = 0)
        {
            var chitiet = db.Sanphams.SingleOrDefault(n => n.Masp == Masp);
            if (chitiet == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitiet);
        }
        public ActionResult Timkiem(string keyword)
        {
            // Kiểm tra nếu từ khóa rỗng hoặc null
            if (string.IsNullOrEmpty(keyword))
            {
                return PartialView(new List<Sanpham>()); // Trả về danh sách rỗng nếu không có từ khóa
            }

            // Tìm kiếm sản phẩm theo từ khóa (so sánh với tên sản phẩm)
            var products = db.Sanphams.Where(sp => sp.Tensp.Contains(keyword)).Take(10).ToList();

            return PartialView(products);
        }

        public ActionResult SanphamTheoHang(int Mahang, decimal? minGia = null, decimal? maxGia = null, int? Ram = null, int? Bonhotrong = null)
        {
            ViewBag.Mahang = Mahang;
            var sanpham = db.Sanphams.Where(sp => sp.Mahang == Mahang);

            if (minGia.HasValue)
            {
                sanpham = sanpham.Where(sp => sp.Giatien >= minGia);
            }
            if (maxGia.HasValue)
            {
                sanpham = sanpham.Where(sp => sp.Giatien <= maxGia);
            }
            if (Ram.HasValue)
            {
                sanpham = sanpham.Where(sp => sp.Ram == Ram);
            }
            if (Bonhotrong.HasValue)
            {
                sanpham = sanpham.Where(sp => sp.Bonhotrong == Bonhotrong);
            }

            var sanphams = sanpham.ToList();

            if (sanphams.Count == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm nào!";
            }

            ViewBag.TenHang = db.Hangsanxuats.SingleOrDefault(h => h.Mahang == Mahang)?.Tenhang;

            return View(sanphams);
        }










    }

}