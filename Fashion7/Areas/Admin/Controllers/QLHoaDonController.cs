using Fashion7.Models;
using OfficeOpenXml;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fashion7.Areas.Admin.Controllers
{
    public class QLHoaDonController : Controller
    {
        // GET: Admin/QLHoaDon
        DataFashion7DataContext data = new DataFashion7DataContext();
        public ActionResult QLHoaDon(int? page)
        {
            if (Session["TaiKhoanAdmin"] == null && Session["TaiKhoanBoss"] == null)
            {
                return RedirectToAction("../Login/DangNhap");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 20;
                ViewBag.Titlee = "Quản lý hoá đơn";
                return View(data.ThanhToans.ToList().OrderBy(n => n.iDThanhToan).ToPagedList(pageNumber, pageSize));
            }
        }
        public ActionResult DetailHoaDon(int id)
        {
            ThanhToan tt = data.ThanhToans.SingleOrDefault(n => n.iDThanhToan == id);

            if (tt == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.Titlee = "Thông tin hoá đơn";
            return View(tt);
        }
        private List<ThanhToan> GetHoaDons()
        {
            return data.ThanhToans.ToList();
        }
        public void ExportExcel()
        {

            var list = GetHoaDons();
            ExcelPackage ep = new ExcelPackage();
            ExcelWorksheet Sheet = ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Mã hoá đơn";
            Sheet.Cells["B1"].Value = "Ngày lập hoá đơn";
            Sheet.Cells["C1"].Value = "Tài khoản";
            Sheet.Cells["D1"].Value = "Tên chủ tài khoản";
            Sheet.Cells["E1"].Value = "SĐT";
            Sheet.Cells["F1"].Value = "Địa chỉ";
            Sheet.Cells["G1"].Value = "Email";
            Sheet.Cells["H1"].Value = "Ghi chú";
            Sheet.Cells["I1"].Value = "Giảm giá";
            Sheet.Cells["J1"].Value = "Tổng tiền";

            int row = 2;// dòng bắt đầu ghi dữ liệu
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.iDThanhToan;
                Sheet.Cells[string.Format("B{0}", row)].Value = item.ngayThanhToan.ToString();
                Sheet.Cells[string.Format("C{0}", row)].Value = item.taiKhoan;
                Sheet.Cells[string.Format("D{0}", row)].Value = item.hoTen;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.sdt;
                Sheet.Cells[string.Format("F{0}", row)].Value = item.diaChi;
                Sheet.Cells[string.Format("G{0}", row)].Value = item.email;
                Sheet.Cells[string.Format("H{0}", row)].Value = item.ghiChu;
                Sheet.Cells[string.Format("I{0}", row)].Value = item.giamGia;
                Sheet.Cells[string.Format("J{0}", row)].Value = item.tongTien;

                row++;
            }
            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=" + "Report.xlsx");
            Response.BinaryWrite(ep.GetAsByteArray());
            Response.End();

        }
    }
}