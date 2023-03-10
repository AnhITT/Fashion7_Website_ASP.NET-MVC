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
using System.Collections.ObjectModel;
using OfficeOpenXml;

namespace Fashion7.Areas.Admin.Controllers
{
    public class QLSanPhamController : Controller
    {
        // GET: Admin/QLSanPham
        DataFashion7DataContext data = new DataFashion7DataContext();
        #region Sản phẩm
        [HttpGet]
        private List<SanPham> GetSanPhams()
        {
            return data.SanPhams.ToList();
        }
        public void ViewTrangThai(SanPham sp)
        {
            if (sp.status == true)
            {
                ViewBag.stt = "Đang bán";
            }
            else if (sp.status == false)
            {
                ViewBag.stt = "Tạm ngưng";
            }
            else
            {
                ViewBag.stt = "Hết hàng";
            }
        }

        public ActionResult QLSanPham(int? page)
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                
                int pageNumber = (page ?? 1);
                int pageSize = 15;
                ChangeTrangThai();
                ViewBag.dangBan = "Đang bán";
                ViewBag.tamNgung = "Tạm ngưng";
                ViewBag.chuaBan = "Hết hàng";
                ViewBag.Titlee = "Quản lý sản phẩm";

                return View(data.SanPhams.ToList().OrderBy(n => n.idSP).ToPagedList(pageNumber, pageSize));
            }
        }

        private void ChangeTrangThai()
        {
            var sp = GetSanPhams();
            foreach (var s in sp)
            {
                if (s.status == true && s.soLuongSP == 0)
                {
                    s.status = null;
                }
                data.SubmitChanges();
            }
        }
        [HttpGet]
        public ActionResult AddSanPham()
        {
            ViewBag.idDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.idDM), "idDM", "tenDM");
            ViewBag.idKM = new SelectList(data.KhuyenMais.ToList().OrderBy(n => n.idKM), "idKM", "tenKM");
            ViewBag.Titlee = "Thêm mới sản phẩm";

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AddSanPham(FormCollection collection, SanPham product)
        {
            var status = collection["status"];
            if (status == "Đang bán")
            {
                product.status = true;
            }
            else if (status == "Tạm ngưng")
            {
                product.status = false;
            }
            else
            {
                product.status = null;
            }
            var idDM = collection["idDM"];
            var idKM = collection["idKM"];
            product.sale = idKM;
            product.idDanhMuc = idDM;
            product.ngayCapNhat = DateTime.Now;
            data.SanPhams.InsertOnSubmit(product);
            data.SubmitChanges();
            return RedirectToAction("AddSanPham");
        }
        public ActionResult DetailSanPham(string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);

            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewTrangThai(product);
            ViewBag.Titlee = "Chi tiết sản phẩm";

            return View(product);
        }
        [HttpGet]
        public ActionResult DelSanPham(string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewTrangThai(product);
            ViewBag.Titlee = "Xoá sản phẩm";

            return View(product);
        }
        [HttpPost, ActionName("DelSanPham")]
        public ActionResult AcceptDelSanPham(string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idSP = product.idSP;
            data.SanPhams.DeleteOnSubmit(product);
            data.SubmitChanges();
            return RedirectToAction("QLSanPham");
        }
        [HttpGet]
        public ActionResult EditSanPham(string id)
        {
            ViewBag.idDM = new SelectList(data.DanhMucs.ToList().OrderBy(n => n.idDM), "idDM", "tenDM");
            ViewBag.idKM = new SelectList(data.KhuyenMais.ToList().OrderBy(n => n.idKM), "idKM", "tenKM");

            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Titlee = "Chỉnh sửa sản phẩm";

            return View(product);
        }
        [HttpPost, ActionName("EditSanPham")]
        [ValidateInput(false)]
        public ActionResult SaveSanPham(FormCollection collection, string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var idDM = collection["idDM"];
            var idKM = collection["idKM"];
            var status = collection["status1"];
            if (status == "Đang bán")
            {
                product.status = true;
            }
            else if (status == "Tạm ngưng")
            {
                product.status = false;
            }
            else
            {
                product.status = null;
            }
            product.idDanhMuc = idDM;
            product.sale = idKM;
            product.ngayCapNhat = DateTime.Now;
            ViewBag.idSP = product.idSP;
            UpdateModel(product);
            data.SubmitChanges();
            return RedirectToAction("QLSanPham");
        }
        public void ExportExcelSP()
        {

            var list = GetSanPhams();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "ID Sản phẩm";
            Sheet.Cells["B1"].Value = "Tên sản phẩm";
            Sheet.Cells["C1"].Value = "Giá sản phẩm";
            Sheet.Cells["D1"].Value = "Số lượng sản phẩm";
            Sheet.Cells["E1"].Value = "Hình ảnh sản phẩm";
            Sheet.Cells["F1"].Value = "Thông tin sản phẩm";
            Sheet.Cells["G1"].Value = "Ngày cập nhật";
            Sheet.Cells["H1"].Value = "Danh mục";
            Sheet.Cells["I1"].Value = "Giảm giá";
            Sheet.Cells["J1"].Value = "Trạng thái";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.idSP;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.tenSP;
                Sheet.Cells[string.Format("C{0}", row)].Value = item.giaSP;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.soLuongSP;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.hinhAnhSP;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.thongTinSP;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.ngayCapNhat.ToString();
                Sheet.Cells[string.Format("H{0}", row)].Value = item.idDanhMuc;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.sale;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.status;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
        #endregion

    }
}