    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using Ictshop.Models;

    namespace Ictshop.Controllers
    {
        public class DonhangsController : Controller
        {
            private Qlbanhang db = new Qlbanhang();

            // GET: Donhangs
            // Hiển thị danh sách đơn hàng
            public ActionResult Index()
            {
                //Kiểm tra đang đăng nhập
                if (Session["use"] == null || Session["use"].ToString() == "")
                {
                    return RedirectToAction("Dangnhap", "User");
                }
                Nguoidung kh = (Nguoidung)Session["use"];
                int maND = kh.MaNguoiDung;
                var donhangs = db.Donhangs.Include(d => d.Nguoidung).Where(d=>d.MaNguoidung == maND);
                return View(donhangs.ToList());
            }

            // GET: Donhangs/Details/5
            //Hiển thị chi tiết đơn hàng
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Donhang donhang = db.Donhangs.Find(id);
                var chitiet = db.Chitietdonhangs.Include(d => d.Sanpham).Where(d=> d.Madon == id).ToList();
                if (donhang == null)
                {
                    return HttpNotFound();
                }
                return View(chitiet);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }
        // GET: Donhangs/Delete/5
        // Hiển thị trang xác nhận xóa đơn hàng
        // GET: Donhangs/Delete/5
        // GET: Donhangs/Delete/5
        // GET: Donhangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Donhang donhang = db.Donhangs.Find(id);
            if (donhang == null)
            {
                return HttpNotFound();
            }

            // Kiểm tra tình trạng đơn hàng
            if (donhang.Tinhtrang != null && donhang.Tinhtrang == 1)
            {
                // Trả về thông báo lỗi nếu đơn hàng đã được xác nhận
                TempData["ErrorMessage"] = "Đơn hàng đã được xác nhận, không thể hủy.";
                return RedirectToAction("Index");
            }

            return View(donhang); // Truyền một đối tượng Donhang duy nhất vào view
        }

        // POST: Donhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donhang donhang = db.Donhangs.Find(id);
            if (donhang != null)
            {
                // Xóa các chi tiết đơn hàng liên quan trước
                var chitietDonhangs = db.Chitietdonhangs.Where(ct => ct.Madon == id).ToList();
                foreach (var chitiet in chitietDonhangs)
                {
                    db.Chitietdonhangs.Remove(chitiet); 
                }

                // Xóa đơn hàng
                db.Donhangs.Remove(donhang);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đơn hàng đã được hủy thành công.";
            }

            return RedirectToAction("Index");
        }


    }
}
