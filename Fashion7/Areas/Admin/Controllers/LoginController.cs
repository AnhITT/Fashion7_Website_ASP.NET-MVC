using Fashion7.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Fashion7.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        DataFashion7DataContext data = new DataFashion7DataContext();
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
            if(String.IsNullOrEmpty(tendn) && String.IsNullOrEmpty(matkhau))
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
                TaiKhoan tkadmin = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.matKhau == MD5Hash(matkhau) && n.maQuyen == "Admin");
                TaiKhoan tkboss = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.matKhau == MD5Hash(matkhau) && n.maQuyen == "Boss");
                TaiKhoan tkadminCheck = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.maQuyen == "Admin");
                TaiKhoan tkbossCheck = data.TaiKhoans.SingleOrDefault(n => n.taiKhoan1 == tendn && n.maQuyen == "Boss");
                if (tkadmin != null)
                {
                    Session["ViewNameAdmin"] = tkadmin.ten;
                    Session["TKAdmin"] = tkadmin.taiKhoan1;
                    Session["ChucVuAdmin"] = tkadmin.TaiKhoanAdmin.chucVu;
                    Session["TaikhoanAdmin"] = tkadmin;
                    Session.Timeout = 500000;
                    return RedirectToAction("../Admin/Index");//Admin
                }
                else if (tkboss != null)
                {
                    Session["ViewNameBoss"] = tkboss.ten;
                    Session["TKBoss"] = tkboss.taiKhoan1;
                    Session["ChucVuBoss"] = tkboss.TaiKhoanAdmin.chucVu;
                    Session["TaikhoanBoss"] = tkboss;
                    Session.Timeout = 500000;
                    return RedirectToAction("../Admin/Index");//Boss: thêm chức năng quản lí, CRUD tk hệ thống
                }
                else if (tkadminCheck == null && tkbossCheck == null)
                {
                    ViewData["nullTK"] = "Tài khoản chưa tồn tại!";
                }
                else
                    ViewData["ThongBao"] = "Mật khẩu không chính xác!";
            }
            return View();
        }

    }
}
