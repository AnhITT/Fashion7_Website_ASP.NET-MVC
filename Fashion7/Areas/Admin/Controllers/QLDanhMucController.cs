using Fashion7.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fashion7.Areas.Admin.Controllers
{
    public class QLDanhMucController : Controller
    {
        // GET: Admin/QLDanhMuc
        DataFashion7DataContext data = new DataFashion7DataContext();
        #region Danh mục
        [HttpGet]
        public ActionResult AddDanhMuc()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddDanhMuc(DanhMuc danhMuc)
        {
            data.DanhMucs.InsertOnSubmit(danhMuc);
            data.SubmitChanges();
            return RedirectToAction("QLDanhMuc");
        }
        public ActionResult QLDanhMuc(int? page)
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                ViewBag.Titlee = "Quản lý danh mục";
                return View(data.DanhMucs.ToList().OrderBy(n => n.idDM).ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult ListDanhMuc(int? page, string id)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 5;
            ViewBag.dangBan = "Đang bán";
            ViewBag.tamNgung = "Tạm ngưng";
            ViewBag.chuaBan = "Chưa bán";
            ViewBag.Titlee = "Danh sách sản phẩm";
            return View(data.SanPhams.Where(n => n.idDanhMuc == id).ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult DelDanhMuc(string id)
        {
            if (Session["TaikhoanBoss"] == null)
            {
                ViewData["LoiQuyenDelDM"] = "Bạn không có quyền xoá danh mục!";
                return View("../Shared/Error");
            }
            else
            {
                DanhMuc dm = data.DanhMucs.SingleOrDefault(n => n.idDM == id);
                if (dm == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.Titlee = "Xoá danh mục";
                return View(dm);
            }
        }
        [HttpPost, ActionName("DelDanhMuc")]
        public ActionResult AcceptDelDanhMuc(string id)
        {
            DanhMuc dm = data.DanhMucs.SingleOrDefault(n => n.idDM == id);
            if (dm == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idDM = dm.idDM;
            data.DanhMucs.DeleteOnSubmit(dm);
            data.SubmitChanges();
            return RedirectToAction("QLDanhMuc");
        }
        [HttpGet]
        public ActionResult EditDanhMuc(string id)
        {
            if (Session["TaikhoanBoss"] == null)
            {
                ViewData["LoiQuyenEditDM"] = "Bạn không có quyền chỉnh sửa danh mục!";
                return View("Error");
            }
            else
            {
                DanhMuc dm = data.DanhMucs.SingleOrDefault(n => n.idDM == id);
                if (dm == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                ViewBag.Titlee = "Chỉnh sửa danh mục";
                return View(dm);
            }
        }
        [HttpPost, ActionName("EditDanhMuc")]
        [ValidateInput(false)]
        public ActionResult SaveDanhMuc(string id)
        {
            DanhMuc dm = data.DanhMucs.SingleOrDefault(n => n.idDM == id);
            if (dm == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idDM = dm.idDM;
            UpdateModel(dm);
            data.SubmitChanges();
            return RedirectToAction("QLDanhMuc");
        }
        #endregion

    }
}