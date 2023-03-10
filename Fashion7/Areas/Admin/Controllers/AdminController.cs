using Fashion7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace Fashion7.Areas.Admin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Admin
        DataFashion7DataContext data = new DataFashion7DataContext();
        public void ViewGioiTinh(TaiKhoan tk)
        {
            if (tk.gioiTinh == true)
            {
                ViewBag.gt = "Nam";
            }
            else if (tk.gioiTinh == false)
            {
                ViewBag.gt = "Nữ";
            }
            else
            {
                ViewBag.gt = "Khác";
            }
        }
        [HttpGet]
        public ActionResult Index()
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                ViewBag.toTalHoaDon = data.ThanhToans.Count();
                ViewBag.toTalDanhMuc = data.DanhMucs.Count();
                ViewBag.toTalUsers = data.TaiKhoans.Where(n => n.maQuyen == "User").Count();
                ViewBag.toTalAdmins = data.TaiKhoans.Where(n => n.maQuyen == "Boss" || n.maQuyen == "Admin").Count();
                ViewBag.toTalOrders = data.CTThanhToans.Sum(x => x.soLuongSP);
                ViewBag.toTalOutStock = outStock();
                ViewBag.toTalProductsActivate = data.SanPhams.Count(a => a.status == true);
                ViewBag.toTalProductsDeAct = data.SanPhams.Count(a => a.status == false);
                ViewBag.toTalProfit = data.ThanhToans.Sum(x => x.tongTien);
                return View();
            }
        }
        [HttpPost]
        private List<SanPham> GetSanPhams()
        {
            return data.SanPhams.ToList();
        }
        private int outStock()
        {
            int outStock = 0;
            foreach (SanPham pham in GetSanPhams())
            {
                if (pham.soLuongSP <= 3)
                    outStock++;
            }
            return outStock;
        }
        public ActionResult InfoAdmin()
        {
            var id = Session["TKAdmin"];
            var id1 = Session["TKBoss"];
            ViewBag.Titlee = "Thông tin cá nhân";

            if (id == null && id1 == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else if (id != null)
            {
                TaiKhoan tkadmin = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id && n.maQuyen == "Admin");
                ViewGioiTinh(tkadmin);
                return View(tkadmin);
            }
            else
            {
                TaiKhoan tkboss = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id1 && n.maQuyen == "Boss");
                ViewGioiTinh(tkboss);
                return View(tkboss);
            }
        }
        [HttpGet]
        public ActionResult DoiMatKhau(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Titlee = "Đổi mật khẩu";
            return View(tk);
        }
        [HttpPost, ActionName("DoiMatKhau")]
        [ValidateInput(false)]
        public ActionResult SaveDoiMatKhau(string id, FormCollection collection)
        {
            var matkhaucu = collection["MatKhauCu"];
            var matkhaumoi1 = collection["MatKhauMoi1"];
            var matkhaumoi2 = collection["MatKhauMoi2"];
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else if (tk.matKhau != QLTaiKhoanController.MD5Hash(matkhaucu))
            {
                ViewData["Loi1"] = "Mật khẩu không chính xác!";
            }
            else if (String.IsNullOrEmpty(matkhaumoi1) || matkhaumoi1.Count() < 6)
            {
                ViewData["Loi2"] = "Vui lòng nhập mật khẩu dài hơn 6 ký tự!";
            }
            else if (matkhaumoi1 != matkhaumoi2)
            {
                ViewData["Loimk2"] = "Mật khẩu không khớp!";
            }
            else
            {
                tk.matKhau = QLTaiKhoanController.MD5Hash(matkhaumoi1);
                UpdateModel(tk);
                data.SubmitChanges();
                return RedirectToAction("InfoAdmin");
            }
            return InfoAdmin();
        }

        public ActionResult Dangxuat()
        {
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Clear();
            return RedirectToAction("../Login/DangNhap");
        }
    }
}