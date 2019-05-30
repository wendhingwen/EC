using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7MVC.Models;
using Team7MVC.Repositories;
using Team7MVC.ViewModels;

namespace Team7MVC.Controllers
{
    public class ProductController : Controller
    {
        public readonly ProductRepository _repo;

        public ProductController()
        {
            _repo = new ProductRepository();
        }
        // GET: Product
        public ActionResult Index()
        {
            List<Products> products;
            products = _repo.GetAllProducts();

            return View(products);
        }

        [HttpGet]
        public ActionResult ProductDetail(int Id)
        {
            var product = _repo.GetProductById(Id);
            return View(product);
        }

        [HttpPost]
        public ActionResult ProductDetail(int ProductId, int buyQty)
        {
            //var product = _repo.GetProductById(Id);

            //ShopListsViewModel shopLists = new ShopListsViewModel
            //{
            //    ProductId = product.ProductID,
            //    Price = product.UnitPrice,
            //    Quantity = buyQty
            //};

            _repo.CreateShoppingCartData( User.Identity.Name, ProductId, buyQty);

            return RedirectToAction("ShoppingCart");
        }

        [Authorize]
        public ActionResult ShoppingCart()
        {
            List<ShopListsViewModel> shopLists;
            shopLists = _repo.ShopList(User.Identity.Name);

            return View(shopLists);
        }

        [HttpPost]
        public ActionResult ShoppingCart(string nothing)
        {
            return RedirectToAction("Payment");
        }

        [Authorize]
        [HttpGet]
        public ActionResult Payment()
        {
            int CustomerId = _repo.GetCustomerID(User.Identity.Name);
            ViewData["CustomerId"] = CustomerId;
            return View();
        }

        [HttpPost]
        public ActionResult Payment(DateTime RequiredDate, string ShipName, string ShipAddress, decimal Freight, string PayWay)
        {
            Orders orders = new Orders()
            {
                OrderDate = DateTime.Now,
                RequiredDate = RequiredDate,
                ShipperID = 1,
                ShipName = ShipName,
                ShipAddress = ShipAddress,
                Freight = Freight,
                PayWay = PayWay,
                PayDate = DateTime.Now
            };

            _repo.Payment(orders, User.Identity.Name);

            return RedirectToAction("Index", "Wine");
        }
    }
}