using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ictshop.Models
{
    public class SanPhamYeuThich
    {
        [Key]
        public int MaSPYeuThich { get; set; }

        [ForeignKey("Nguoidung")]
        public int MaNguoiDung { get; set; }  // Mã khách hàng

        [ForeignKey("SanPham")]
        public int Masp { get; set; }  // Mã sản phẩm

        // Liên kết với bảng KhachHang
        public virtual Nguoidung Nguoidung { get; set; } 


        // Liên kết với bảng SanPham
        public virtual Sanpham SanPham { get; set; }
    }
}
