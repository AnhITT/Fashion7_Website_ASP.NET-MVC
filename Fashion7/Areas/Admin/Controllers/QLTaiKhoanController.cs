using Fashion7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace Fashion7.Areas.Admin.Controllers
{
    public class QLTaiKhoanController : Controller
    {
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
        // GET: Admin/QLTaiKhoan
        #region Quản lí tài khoản KH
        public ActionResult QLTaiKhoanKH(int? page)
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                ViewBag.Titlee = "Quản lý khách hàng";

                return View(data.TaiKhoans.Where(n => n.maQuyen == "User").ToList().OrderBy(n => n.taiKhoan1).ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult DelTaiKhoanKH(string id)
        {
            TaiKhoan tk = data.TaiKhoans.Where(n => n.maQuyen == "User").SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.Titlee = "Xoá tài khoản";
            return View(tk);
        }
        [HttpPost, ActionName("DelTaiKhoanKH")]
        public ActionResult AcceptDelTaiKhoanKH(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.taiKhoan1 = tk.taiKhoan1;
            data.TaiKhoans.DeleteOnSubmit(tk);
            data.SubmitChanges();
            return RedirectToAction("QLTaiKhoanKH");
        }
        [HttpGet]
        public ActionResult DetailTaiKhoanKH(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);

            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.Titlee = "Thông tin tài khoản";
            return View(tk);
        }
        [HttpGet]
        public ActionResult EditTaiKhoanKH(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.Titlee = "Sửa thông tin tài khoản";
            return View(tk);
        }
        [HttpPost, ActionName("EditTaiKhoanKH")]
        [ValidateInput(false)]
        public ActionResult SaveTaiKhoanKH(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(tk);
            data.SubmitChanges();
            return RedirectToAction("QLTaiKhoanKH");
        }

        #endregion

        #region Quản lý tài khoản quản trị
        public ActionResult QLTaiKhoanAdmin(int? page)
        {
            if (Session["TaikhoanBoss"] == null)
            {
                ViewData["LoiQuyenTruyCapQLTKAD"] = "Bạn không có quyền truy cập tài khoản quản trị hệ thống!";
                return View("../Shared/Error");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                ViewBag.Titlee = "Quản lý tài khoản hệ thống";
                return View(data.TaiKhoans.Where(n => n.maQuyen == "Boss" || n.maQuyen == "Admin").ToList().OrderBy(n => n.taiKhoan1).ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult DetailTaiKhoanAdmin(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            TaiKhoanAdmin tkad = data.TaiKhoanAdmins.SingleOrDefault(n => n.taiKhoan == id);

            if (tk == null || tkad == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.Titlee = "Thông tin tài khoản";
            return View(tk);
        }

        public ActionResult DelTaiKhoanAdmin(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.taiKhoan1 = tk.taiKhoan1;
            ViewBag.Titlee = "Xoá tài khoản";
            return View(tk);
        }
        [HttpPost, ActionName("DelTaiKhoanAdmin")]
        public ActionResult AcceptDelTaiKhoanAdmin(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            TaiKhoanAdmin tkad = data.TaiKhoanAdmins.SingleOrDefault(n => n.taiKhoan == id);
            if (tk == null || tkad == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.taiKhoan1 = tk.taiKhoan1;
            ViewBag.taiKhoan = tkad.taiKhoan;
            data.TaiKhoans.DeleteOnSubmit(tk);
            data.TaiKhoanAdmins.DeleteOnSubmit(tkad);
            data.SubmitChanges();
            return RedirectToAction("QLTaiKhoanAdmin");
        }
        [HttpGet]

        public ActionResult EditTaiKhoanAdmin(string id)
        {
            ViewBag.maQuyen = new SelectList(data.PhanQuyens.ToList().OrderBy(n => n.maQuyen), "maQuyen", "tenQuyen");
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            if (tk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewGioiTinh(tk);
            ViewBag.maQuyen = new SelectList(data.PhanQuyens.ToList().OrderBy(n => n.maQuyen), "maQuyen", "tenQuyen");
            ViewBag.Titlee = "Sửa thông tin tài khoản";
            return View(tk);
        }
        [HttpPost, ActionName("EditTaiKhoanAdmin")]
        [ValidateInput(false)]
        public ActionResult SaveTaiKhoanAdmin(string id)
        {
            TaiKhoan tk = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == id);
            TaiKhoanAdmin tkad = data.TaiKhoanAdmins.SingleOrDefault(n => n.taiKhoan == id);
            if (tk == null || tkad == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            UpdateModel(tk);
            UpdateModel(tkad);
            data.SubmitChanges();
            return RedirectToAction("QLTaiKhoanAdmin");
        }
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
        public ActionResult AddTaiKhoan()
        {
            ViewBag.maQuyen = new SelectList(data.PhanQuyens.ToList().OrderBy(n => n.maQuyen), "maQuyen", "tenQuyen");
            ViewBag.Titlee = "Thêm tài khoản";
            return View();
        }
        [HttpPost]
        public ActionResult AddTaiKhoan(FormCollection collection, TaiKhoan tk, TaiKhoanAdmin tkad)
        {
            var hoten = collection["HoTen"];
            var tendn = collection["TenDN"];
            var matkhau = collection["MatKhau"];
            var matkhau2 = collection["MatKhau2"];
            var gioitinh = collection["GioiTinh"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var sdt = collection["SDT"];
            var luong = collection["Luong"];
            var chucVu = collection["ChucVu"];
            var maQuyen = collection["MaQuyen"];
            var ngaysinh = String.Format("{0:dd/mm/yyyy}", collection["NgaySinh"]);
            TaiKhoanAdmin id = data.TaiKhoanAdmins.SingleOrDefault(n => n.taiKhoan == tendn);
            if (String.IsNullOrEmpty(hoten))
            {
                ViewData["Loi1"] = "Họ tên không được để trống!";
            }
            else if (String.IsNullOrEmpty(tendn) || tendn.Count() < 6)
            {
                ViewData["Loi2"] = "Vui lòng nhập tài khoản dài hơn 6 ký tự!";
            }
            else if (id != null)
            {
                ViewData["LoiTrungTK"] = "Tài khoản đã được đăng ký!";

            }
            else if (String.IsNullOrEmpty(matkhau) || matkhau.Count() < 6)
            {
                ViewData["Loi3"] = "Vui lòng nhập mật khẩu dài hơn 6 ký tự!";
            }
            else if (String.IsNullOrEmpty(matkhau2))
            {
                ViewData["Loi4"] = "Vui lòng nhập lại mật khẩu!";
            }
            else if (matkhau != matkhau2)
            {
                ViewData["Loimk2"] = "Mật khẩu không khớp!";
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
            else if (String.IsNullOrEmpty(chucVu))
            {
                ViewData["Loi8"] = "Vui lòng nhập số chức vụ!";
            }
            else if (String.IsNullOrEmpty(luong))
            {
                ViewData["Loi9"] = "Vui lòng nhập lương!";
            }
            else if (gioitinh == null)
            {
                ViewData["Loi11"] = "Vui lòng chọn giới tính!";
            }
            else
            {
                tk.ten = hoten;
                tkad.taiKhoan = tk.taiKhoan1 = tendn;
                tk.matKhau = MD5Hash(matkhau);
                tk.diaChi = diachi;
                tk.email = email;
                tk.sdt = sdt;
                tkad.luong = float.Parse(luong);
                tkad.chucVu = chucVu;
                tk.ngaySinh = DateTime.Parse(ngaysinh);
                tk.maQuyen = maQuyen;
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
                data.TaiKhoanAdmins.InsertOnSubmit(tkad);
                data.SubmitChanges();
                return RedirectToAction("QLTaiKhoanAdmin");
            }
            return AddTaiKhoan();
        }
        #endregion
    }
}