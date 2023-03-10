using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fashion7.Models;
using PagedList;
using PagedList.Mvc;
namespace Fashion7.Controllers
{
    public class ProductsController : Controller
    {
        DataFashion7DataContext data = new DataFashion7DataContext();

        private IList<SanPham> GetSanPhamsDanhMuc(int count,string idDanhMuc)
        {
            IList<SanPham> sanPhams = data.SanPhams.Where(n => n.idDanhMuc == idDanhMuc).Take(count).ToList();
            return sanPhams;
        }
        public ActionResult Index(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;

            foreach (var item in data.SanPhams.ToList())
            {
                if (item.sale != null)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                   
                    if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
                    {
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                    }
                    else ViewData["checkSale"+item.idSP] = "true";
                }
            }

            return View(data.SanPhams.ToList().OrderBy(n => n.ngayCapNhat).ToPagedList(pageNumber, pageSize));
            
        }

        public ActionResult Top(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;

            foreach (var item in data.SanPhams.Where(n => n.idDanhMuc == "swe-top" || n.idDanhMuc == "outerwear").ToList())
            {
                if (item.sale != null)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                    if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
                    {
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                    }
                    else ViewData["checkSale" + item.idSP] = "true";
                }
            }
            return View(data.SanPhams.Where(n => n.idDanhMuc == "swe-top" || n.idDanhMuc == "outerwear").ToPagedList(pageNumber, pageSize));
        }
        
        public ActionResult Bottom(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;
            foreach (var item in data.SanPhams.Where(n => n.idDanhMuc == "swe-bottom").ToList())
            {
                if (item.sale != null)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                    if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
                    {
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                    }
                    else ViewData["checkSale" + item.idSP] = "true";
                }
            }

            return View(data.SanPhams.Where(n => n.idDanhMuc == "swe-bottom").ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Outerwear(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;
            foreach (var item in data.SanPhams.Where(n => n.idDanhMuc == "outerwear").ToList())
            {
                if (item.sale != null)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                    if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
                    {
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                    }
                    else ViewData["checkSale" + item.idSP] = "true";
                }
            }

            return View(data.SanPhams.Where(n => n.idDanhMuc == "outerwear").ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Accessories(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;
            foreach (var item in data.SanPhams.Where(n => n.idDanhMuc == "a-c-sories").ToList())
            {
                if (item.sale != null)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                    if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
                    {
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                    }
                    else ViewData["checkSale" + item.idSP] = "true";
                }
            }


            return View(data.SanPhams.Where(n => n.idDanhMuc == "a-c-sories").ToPagedList(pageNumber, pageSize));
        }

        private bool checkExpried(SanPham sanPham)
        {
            var KM = data.KhuyenMais.Where(x => x.idKM == sanPham.sale).FirstOrDefault();
            if (DateTime.Now >= KM.dateStart && DateTime.Now <= KM.dateEnd && KM.soLuong > 0)
            {
                return false;
            }
            else return true;
        }
        public ActionResult Sale(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 20;
            var sale = data.SanPhams.Where(n => n.sale != null).ToList();
            List<SanPham> saleChecked = new List<SanPham>();
            foreach(var item in sale)
            {
                if(checkExpried(item) == false)
                {
                    saleChecked.Add(item);
                    var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                    if (KM.donGia != null)
                    {

                        ViewData[item.idSP] = item.giaSP - KM.donGia;
                    }
                    else
                    {

                        ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                    }
                }   
            }
            return View(saleChecked.ToPagedList(pageNumber, pageSize));
        }
        [Route("Detail/{id:string}")]
        public ActionResult Detail(string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(n => n.idSP == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.idSP = product.idSP;

                if (product.sale != null && checkExpried(product) == false)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == product.sale).FirstOrDefault();
                    if (KM.donGia != null)
                    {

                        ViewBag.giaSale = product.giaSP - KM.donGia;
                    }
                    else
                    {

                        ViewBag.giaSale = product.giaSP - ((product.giaSP * KM.chietKhau) / 100);
                    }
                }
                else ViewBag.checkExpired = true;
           

            IList<SanPham> sanPhamDanhMuc = GetSanPhamsDanhMuc(4, product.idDanhMuc);
            foreach(var item in sanPhamDanhMuc)
            {
                if (item.sale != null && checkExpried(item) == false)
                {
                        var KM = data.KhuyenMais.Where(x => x.idKM == item.sale).FirstOrDefault();
                        if (KM.donGia != null)
                        {

                            ViewData[item.idSP] = item.giaSP - KM.donGia;
                        }
                        else
                        {

                            ViewData[item.idSP] = item.giaSP - ((item.giaSP * KM.chietKhau) / 100);
                        }
                }
                else ViewData[item.idSP+"saleCheck"] = "true";

            }
            ViewBag.spDanhMuc = sanPhamDanhMuc;
            return View(product);
        }


    }
}