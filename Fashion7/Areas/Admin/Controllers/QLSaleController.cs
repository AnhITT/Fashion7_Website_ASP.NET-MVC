using Fashion7.Models;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Web;
using System;

namespace Fashion7.Areas.Admin.Controllers
{
    public class QLSaleController : Controller
    {
        // GET: Admin/QLSale
        DataFashion7DataContext data = new DataFashion7DataContext();

        public ActionResult QLSale(int? page, string id)
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 10;
                ViewBag.Titlee = "Quản lý khuyến mãi";

                return View(data.KhuyenMais.ToList().OrderBy(n => n.idKM).ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult ListSale(int? page, string id)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            ViewBag.dangBan = "Đang bán";
            ViewBag.tamNgung = "Tạm ngưng";
            ViewBag.chuaBan = "Chưa bán";
            ViewBag.Titlee = "Danh sách sản phẩm khuyến mãi";

            return View(data.SanPhams.ToList().OrderBy(n => n.idSP).Where(n => n.sale == id).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult AddSale()
        {
            ViewBag.Titlee = "Thêm khuyến mãi";
            return View();
        }
        [HttpPost]
        public ActionResult AddSale(KhuyenMai km)
        {
            data.KhuyenMais.InsertOnSubmit(km);
            data.SubmitChanges();

            return RedirectToAction("QLSale");
        }
        [HttpGet]
        public ActionResult DelSale(string id)
        {
            KhuyenMai km = data.KhuyenMais.SingleOrDefault(n => n.idKM == id);
            if (km == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Titlee = "Xoá khuyến mãi";

            return View(km);
        }
        [HttpPost, ActionName("DelSale")]
        public ActionResult AcceptDelSale(string id)
        {
            KhuyenMai km = data.KhuyenMais.SingleOrDefault(n => n.idKM == id);
            if (km == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idKM = km.idKM;
            data.KhuyenMais.DeleteOnSubmit(km);
            data.SubmitChanges();
            return RedirectToAction("QLSale");
        }
        [HttpGet]
        public ActionResult EditSale(string id)
        {
            KhuyenMai km = data.KhuyenMais.SingleOrDefault(n => n.idKM == id);
            if (km == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Titlee = "Chỉnh sửa khuyến mãi";

            return View(km);
        }
        [HttpPost, ActionName("EditSale")]
        [ValidateInput(false)]
        public ActionResult SaveSale(string id)
        {
            KhuyenMai km = data.KhuyenMais.SingleOrDefault(n => n.idKM == id);
            if (km == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idKM = km.idKM;
            UpdateModel(km);
            data.SubmitChanges();
            return RedirectToAction("QLSale");
        }
    }
}