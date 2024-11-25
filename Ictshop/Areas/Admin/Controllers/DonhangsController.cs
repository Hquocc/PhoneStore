using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using Ictshop.Models;


namespace Ictshop.Areas.Admin.Controllers
{
    public class DonhangsController : Controller
    {
        private Qlbanhang db = new Qlbanhang();

        // GET: Admin/Donhangs
        public ActionResult Index()
        {
            var donhangs = db.Donhangs.Include(d => d.Nguoidung);
            return View(donhangs.ToList());
        }

        // GET: Admin/Donhangs/Details/5
        public ActionResult Details(int? id)
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
            return View(donhang);
        }

        // GET: Admin/Donhangs/Create
        public ActionResult Create()
        {
            // Lấy danh sách người dùng từ cơ sở dữ liệu
            var nguoidungs = db.Nguoidungs.ToList();
            var donhang = new Donhang
            {
                Ngaydat = DateTime.Now.Date  
            };
            // Gán vào ViewBag với đúng tên thuộc tính "MaNguoidung" và "Hoten"
            ViewBag.Nguoidungs = new SelectList(nguoidungs, "MaNguoidung", "Hoten");

            return View();
        }



        //public ActionResult Xacnhan(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Donhang donhang = db.Donhangs.Find(id);
        //    donhang.Tinhtrang = 1;
        //    db.SaveChanges();
        //    if (donhang == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return RedirectToAction("Index");
        //}

        // POST: Admin/Donhangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Madon,Ngaydat,Tinhtrang,Thanhtoan,MaNguoidung,Diachinhanhang")] Donhang donhang)
        {
            if (ModelState.IsValid)
            {
                db.Donhangs.Add(donhang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Nguoidungs = new SelectList(db.Nguoidungs, "MaNguoidung", "Hoten", donhang.MaNguoidung);
            return View(donhang);
        }

        // GET: Admin/Donhangs/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien", donhang.MaNguoidung);
            return View(donhang);
        }

        // POST: Admin/Donhangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Madon,Ngaydat,Tinhtrang,Thanhtoan,MaNguoidung,Diachinhanhang")] Donhang donhang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donhang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaNguoidung = new SelectList(db.Nguoidungs, "MaNguoiDung", "Anhdaidien", donhang.MaNguoidung);
            return View(donhang);
        }

        // GET: Admin/Donhangs/Delete/5
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
            return View(donhang);
        }
        //public ActionResult Dagiao(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Donhang donhang = db.Donhangs.Find(id);
        //    if (donhang == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    donhang.Tinhtrang = 2; // Đã giao
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}
        //public ActionResult Hoanthanh(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Donhang donhang = db.Donhangs.Find(id);
        //    if (donhang == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    donhang.Tinhtrang = 3; // Hoàn thành
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        // Action cập nhật tình trạng đơn hàng
        [HttpPost]
        public ActionResult UpdateTinhTrang(int? id, int tinhTrang)
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

            // Cập nhật tình trạng
            donhang.Tinhtrang = tinhTrang;
            db.Entry(donhang).State = EntityState.Modified;
            db.SaveChanges();

            TempData["Message"] = "Cập nhật tình trạng đơn hàng thành công!";
            return RedirectToAction("Index");
        }

        // POST: Admin/Donhangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Donhang donhang = db.Donhangs.Find(id);
            db.Donhangs.Remove(donhang);
            db.SaveChanges();
            return RedirectToAction("Index");
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
