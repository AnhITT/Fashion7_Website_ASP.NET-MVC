using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fashion7.Models;

namespace Fashion7.Models
{
    public class CartItem
    {
        public SanPham _shopping_product { get; set; }
        public int _shopping_quantity { get; set; }

    }
    public class Cart
    {
        List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }
        public void Add(SanPham _pro, int _quantity = 1)
        {
            var item = items.FirstOrDefault(s => s._shopping_product.idSP == _pro.idSP);
            if(item == null)
            {
                items.Add(new CartItem
                {
                    _shopping_product = _pro,
                    _shopping_quantity = _quantity 
                }
                    );

            }
            else
            {
                item._shopping_quantity += _quantity;
            }

        }
        public void Update_Quantity_ShoppingPro(string id, int _quantity)
        {
            var item = items.Find(s => s._shopping_product.idSP == id);
                if(item != null)
            {
                item._shopping_quantity = _quantity;
            }    
        }
       public double Total_Money()
        {
            var total = items.Sum(s => s._shopping_product.giaSP * s._shopping_quantity);
            return (double)total;
        }
        public void Remove_CartItem(string id)
        {
            items.RemoveAll(s => s._shopping_product.idSP == id);
        }
        //Tong so luong SP trong gio hang
        public int Total_Quantity_in_Cart()
        {
            return items.Sum(s => s._shopping_quantity);

        }
        public void ClearCart()
        {
            items.Clear(); //Xoa gio hang de thuc hien order
        }
        
    }
   
    
     
    
}