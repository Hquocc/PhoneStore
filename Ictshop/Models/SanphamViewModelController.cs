using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ictshop.Models
{
    public class SanphamViewModelController : Controller
    {
        // GET: SanphamViewModel
        public class SanphamViewModel
        {
            public int Masp { get; set; }
            public string Tensp { get; set; }
            public decimal? Giatien { get; set; }
            public int? Soluong { get; set; }
            public string Mota { get; set; }
            public string Tenhang { get; set; } 
        }
    }
}