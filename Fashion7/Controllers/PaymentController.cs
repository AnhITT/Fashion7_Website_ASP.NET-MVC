using Fashion7.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fashion7.Controllers.User
{
    public class PaymentController : Controller
    {
        DataFashion7DataContext data = new DataFashion7DataContext();
        // GET: Payment
        
        public ActionResult Index()
        {
            return View();
        }
        [Route("CheckOut/{id:string}")]
        public ActionResult CheckOut(string id)
        {
            var listThanhToan = data.ThanhToans.ToList();
            ThanhToan thanhToan = listThanhToan.Where(x => x.iDThanhToan.ToString() ==id.ToString()).FirstOrDefault();
           
            if (thanhToan == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var listOrders = data.CTThanhToans.Where(o => o.idThanhToan.ToString() == id).ToList();
            foreach(var item in listOrders)
            {
                var sanPham = data.SanPhams.Where(o => o.idSP == item.idSP).FirstOrDefault();
                ViewData[item.idSP] = sanPham.tenSP; 
            }
            ViewBag.listOrders = listOrders;

            var listproducts = data.CTThanhToans.Where(o => o.idThanhToan.ToString() == id).ToList();
            foreach (var item in listproducts)
            {
                var delSLSP = data.SanPhams.Where(o => o.idSP == item.idSP).FirstOrDefault();
                if (delSLSP != null)
                { 
                    if (delSLSP.soLuongSP < item.soLuongSP)
                    {
                        data.SanPhams.Where(o => o.idSP == delSLSP.idSP).FirstOrDefault().soLuongSP = delSLSP.soLuongSP = 0;
                    }
                    else
                        data.SanPhams.Where(o => o.idSP == delSLSP.idSP).FirstOrDefault().soLuongSP = delSLSP.soLuongSP = data.SanPhams.Where(o => o.idSP == delSLSP.idSP).FirstOrDefault().soLuongSP = delSLSP.soLuongSP - item.soLuongSP;

                    
                }
                
            }
            data.SubmitChanges();
            Session["Cart"] = null;
            return View(thanhToan);
        }
    }
}