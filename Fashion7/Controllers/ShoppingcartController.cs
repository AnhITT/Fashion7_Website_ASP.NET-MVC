using System;
using System.Linq;
using System.Web.Mvc;
using Fashion7.Models;

namespace Fashion7.Controllers
{
    public class ShoppingCartController : Controller
    {
        DataFashion7DataContext data = new DataFashion7DataContext();
        // GET: ShoppingCartt
        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
                Session.Timeout = 500000;
            }
            return cart;
        }
        //Them SP vao GH
        public ActionResult AddtoCart(string id)
        {
            SanPham product = data.SanPhams.SingleOrDefault(s => s.idSP == id);
            if (product != null && product.soLuongSP > 0 && Session["TaiKhoan"] != null)
            {
                GetCart().Add(product);
                return Redirect("/Products/Detail/"+id);
            }
            else
            {
                return View("../ShoppingCart/cartNull");
            }
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
        //Trang Gio Hang    
        public ActionResult ShowToCart()
        {
            if (Session["Cart"] == null)
                return RedirectToAction("cartNull", "ShoppingCart");
            ViewBag.sale = 0;
            foreach(var item in GetCart().Items)
            {
                if (item._shopping_product.sale != null && checkExpried(item._shopping_product) == false)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item._shopping_product.sale).FirstOrDefault();
                    if (KM.donGia != null)
                    {

                        ViewBag.sale += (item._shopping_product.giaSP - KM.donGia)*item._shopping_quantity;
                    }
                    else
                    {

                        ViewBag.sale += (item._shopping_product.giaSP - ((item._shopping_product.giaSP * KM.chietKhau) / 100)) * item._shopping_quantity;
                    }
                }
            }
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        public ActionResult cartNull()
        {
            return  View();
        }


        [HttpPost]
        public ActionResult ShowToCart(FormCollection collection, ThanhToan thanhToan)
        {
            if (Session["Cart"] == null)
                return RedirectToAction("ShowToCart", "ShoppingCart");
            ViewBag.sale = 0;
            foreach (var item in GetCart().Items)
            {
                if (item._shopping_product.sale != null && checkExpried(item._shopping_product) == false)
                {
                    var KM = data.KhuyenMais.Where(x => x.idKM == item._shopping_product.sale).FirstOrDefault();
                    if (KM.donGia != null)
                    {

                        ViewBag.sale += (item._shopping_product.giaSP - KM.donGia) * item._shopping_quantity;
                    }
                    else
                    {

                        ViewBag.sale += (item._shopping_product.giaSP - ((item._shopping_product.giaSP * KM.chietKhau) / 100)) * item._shopping_quantity;
                    }
                }
            }
            var name = collection["ten"];
            var sdt = collection["sdt"];
            var email = collection["email"];
            var diachi = collection["diaChi"];
            var ghichu = collection["ghiChu"];
            if(data.ThanhToans.Count() > 0)
            {
                var idMax = data.ThanhToans.Max(o => o.iDThanhToan);
                thanhToan.iDThanhToan = idMax + 1;
            }
            else thanhToan.iDThanhToan = 1;
            thanhToan.ngayThanhToan = DateTime.Today;
            if (Session["TaiKhoan"] != null)
                thanhToan.taiKhoan = (Session["TaiKhoan"] as TaiKhoan).taiKhoan1;
            else thanhToan.taiKhoan = null;
            thanhToan.tongTien = (decimal?)(Session["Cart"] as Cart).Total_Money() - ViewBag.sale;
            thanhToan.giamGia = ViewBag.sale;
            thanhToan.ghiChu = collection["ghiChu"];
            thanhToan.status = true;
            thanhToan.hoTen = name;
            data.ThanhToans.InsertOnSubmit(thanhToan);
            data.SubmitChanges();
            Cart cart = Session["Cart"] as Cart;
            foreach (var item in GetCart().Items)
            {
                CTThanhToan cTThanhToan = new CTThanhToan();
                if (data.CTThanhToans.Count() > 0)
                {
                    cTThanhToan.stt = data.CTThanhToans.Max(o => o.stt) + 1;
                }
                else cTThanhToan.stt = 1;
                cTThanhToan.idThanhToan = thanhToan.iDThanhToan;
                cTThanhToan.soLuongSP = item._shopping_quantity;
                cTThanhToan.idSP = item._shopping_product.idSP;
                cTThanhToan.giaSP = item._shopping_product.giaSP;
                data.CTThanhToans.InsertOnSubmit(cTThanhToan);
            }
            data.SubmitChanges();
            return Redirect("/Payment/CheckOut/"+thanhToan.iDThanhToan);
        }
        public ActionResult Update_Quantity_Cart(FormCollection form)
        {
            Cart cart = Session["Cart"] as Cart;
            string id_pro =form["ID_Product"];
            int quantity = int.Parse(form["Quantity"]);
            cart.Update_Quantity_ShoppingPro(id_pro, quantity);
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }
        public ActionResult RemoveCart (string id)
        {
            Cart cart = Session["Cart"] as Cart;
            cart.Remove_CartItem(id);
            return RedirectToAction("ShowToCart", "ShoppingCart");

        }
        public PartialViewResult _BagCart()
        {
            int total_item = 0;
            Cart cart = Session["Cart"] as Cart;
            if(cart != null)
                total_item = cart.Total_Quantity_in_Cart();
                ViewBag.Total_Quantity_In_Cart = total_item;
            return PartialView("BagCart");
            
        }
    }
}