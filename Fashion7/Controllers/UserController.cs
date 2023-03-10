using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Fashion7.Areas.Admin.Controllers;
using Fashion7.Models;

namespace Fashion7.Controllers
{
    public class UserController : Controller
    {
        // GET: User

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
        public ActionResult Index()
        {
            return RedirectToAction("DangNhap");
        }
        [HttpGet]
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        public ActionResult DangKy()
        {
            return View();
        }
        [HttpPost]

        public ActionResult DangKy(FormCollection collection, TaiKhoan tk)
        {
            var hoten = collection["HoTenKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhau2 = collection["MatKhau2"];
            var gioitinh = collection["GioiTinh"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var sdt = collection["SDT"];
            var ngaysinh = String.Format("{0:dd/mm/yyyy}", collection["NgaySinh"]);
            TaiKhoan id = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn);

            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên khách hàng không được để trống!";
            }
            else if (String.IsNullOrEmpty(tendn) || tendn.Count() < 6)
            {
                ViewData["LoiCountTK"] = "Vui lòng nhập tài khoản dài hơn 6 ký tự!";
            }
            else if (id != null)
            {
                ViewData["LoiTrungTK"] = "Tài khoản đã được sử dụng!";
            }
            else if (String.IsNullOrEmpty(matkhau) || matkhau.Count() < 6)
            {
                ViewData["LoiCountMK"] = "Vui lòng nhập mật khẩu dài hơn 6 ký tự!";
            }
            else if (String.IsNullOrEmpty(matkhau2))
            {
                ViewData["Loi4"] = "Vui lòng nhập lại mật khẩu!";
            }
            else if (matkhau != matkhau2)
            {
                ViewData["LoiTrungMK"] = "Mật khẩu không khớp!";
            }
            else if (String.IsNullOrEmpty(diachi))
            {
                ViewData["Loi5"] = "Địa chỉ không được để trống!";
            }
            else if (String.IsNullOrEmpty(email))
            {
                ViewData["Loi6"] = "Vui lòng nhập email!";
            }
            else if (String.IsNullOrEmpty(sdt))
            {
                ViewData["Loi7"] = "Vui lòng nhập số điện thoại!";
            }
            else if (gioitinh == null)
            {
                ViewData["Loi11"] = "Vui lòng chọn giới tính!";
            }
            else
            {
                tk.ten = hoten;
                tk.taiKhoan1 = tendn;
                tk.matKhau = MD5Hash(matkhau);
                tk.diaChi = diachi;
                tk.email = email;
                tk.sdt = sdt;
                tk.ngaySinh = DateTime.Parse(ngaysinh);
                tk.maQuyen = "User";
                if (gioitinh == "Nam")
                {
                    tk.gioiTinh = true;
                }
                else if (gioitinh == "Nữ")
                {
                    tk.gioiTinh = false;
                }
                else
                {
                    tk.gioiTinh = null;
                }
                data.TaiKhoans.InsertOnSubmit(tk);
                data.SubmitChanges();
                return RedirectToAction("DangNhap");
            }
            return DangKy();
        }
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {

            var tendn = collection["TenDN"];
            var matkhau = collection["Matkhau"];
            System.Diagnostics.Debug.WriteLine(tendn);
            System.Diagnostics.Debug.WriteLine(matkhau);

            if (String.IsNullOrEmpty(tendn) && String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi"] = "Vui lòng nhập tài khoản và mật khẩu!";
            }
            else if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = "Vui lòng nhập tài khoản!";
            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Vui lòng nhập mật khẩu!";
            }
            else
            {
                TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.matKhau == MD5Hash(matkhau) && n.maQuyen == "User");
                TaiKhoan tkCheck = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.maQuyen == "User");
                if (tkCheck == null)
                {
                    ViewBag.checkTK = "Tài khoản chưa tồn tại!";
                }
                else if (tk != null)
                {
                    Session["Taikhoan"] = tk;
                    Session["TKUser"] = tendn;
                    Session.Timeout = 500000;
                    return RedirectToAction("Index", "Products");
                }
                else
                {
                    ViewData["ThongBao"] = "Mật khẩu không chính xác!";
                }
            }
            return View();
        }
        public ActionResult InfoUser()
        {
            if (Session["Taikhoan"] == null)
            {
                return RedirectToAction("DangNhap");
            }
            else
            {
                var id = Session["TKUser"];
                TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
                ViewGioiTinh(tk);
                return View(tk);
            }
        }
        [HttpGet]
        public ActionResult EditTaiKhoan(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
                if (tk == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewGioiTinh(tk);
                return View(tk);
            }
        }

        [HttpPost, ActionName("EditTaiKhoan")]
        [ValidateInput(false)]
        public ActionResult SaveTaiKhoan(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(tk);
            data.SubmitChanges();
            return RedirectToAction("InfoUser");
        }
        public ActionResult DoiMatKhauUser(string id)
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
        [HttpPost, ActionName("DoiMatKhauUser")]
        [ValidateInput(false)]
        public ActionResult SaveDoiMatKhauUser(string id, FormCollection collection)
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
                return RedirectToAction("InfoUser");
            }
            return InfoUser();
        }

        public ActionResult DangXuat()
        {
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Clear();
            return RedirectToAction("Index");
        }
    }
}