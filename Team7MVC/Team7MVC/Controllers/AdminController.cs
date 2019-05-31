using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Team7MVC.Repositories;
using Team7MVC.Models;

namespace Team7MVC.Controllers
{
    public class AdminController : Controller
    {
        public readonly AdminRepository _repo;

        public AdminController()
        {
            _repo = new AdminRepository();
        }
        
        //Dashboard
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Product()
        {
            var products = _repo.GetAllProducts();
            

            return View(products);
        }

        [HttpPost]
        public ActionResult Product(string SortBy)
        {
            var products = _repo.GetAllProducts(SortBy);

            return View(products);
        }

        [HttpGet]
        public ActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ProductCreate(string ProductName, string Origin, int Year, int Capacity, decimal UnitPrice, int Stock, string Grade, string Variety, string Area, string Picture, string Introduction, int CategoryId)
        {
            Products product = new Products()
            {
                ProductName = ProductName,
                Origin = Origin,
                Year = Year,
                Capacity = Capacity,
                UnitPrice = UnitPrice,
                Stock = Stock,
                Grade = Grade,
                Variety = Variety,
                Area = Area,
                Picture = Picture,
                Introduction = Introduction,
                CategoryID = CategoryId
            };

            _repo.CreateProduct(product);

            return RedirectToAction("Product");
        }

        [HttpGet]
        public ActionResult ProductEdit(int Id)
        {
            var product = _repo.GetProduct(Id);
            return View(product);
        }

        [HttpPost]
        public ActionResult ProductEdit(int Id, string ProductName, string Origin, int Year, int Capacity, decimal UnitPrice, int Stock, string Grade, string Variety, string Area, string Picture, string Introduction, int CategoryId)
        {
            Products product = new Products()
            {
                ProductID = Id,
                ProductName = ProductName,
                Origin = Origin,
                Year = Year,
                Capacity = Capacity,
                UnitPrice = UnitPrice,
                Stock = Stock,
                Grade = Grade,
                Variety = Variety,
                Area = Area,
                Picture = Picture,
                Introduction = Introduction,
                CategoryID = CategoryId
            };

            _repo.UpdateProduct(product);

            return RedirectToAction("Product");
        }

        [HttpGet]
        public ActionResult ProductDelete(int Id)
        {
            _repo.DeleteProduct(Id);

            return RedirectToAction("Product");
        }

        public ActionResult Order()
        {
            var orders = _repo.GetAllOrders();

            return View(orders);
        }

        // GET: Customers
        public ActionResult Customer()
        {
            var customers = _repo.GetAllCustomers();

            return View(customers);
        }

        public ActionResult Messages()
        {
            var messages = _repo.GetAllMessages();

            return View(messages);
        }

        [HttpGet]
        public ActionResult OrderCreate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OrderCreate(int CustomerID, DateTime OrderDate, DateTime RequiredDate, DateTime ShippedDate, int ShipperID, string ShipName, string ShipAddress, decimal Freight, string PayWay, DateTime PayDate)
        {
            Orders order = new Orders()
            {
                CustomerID = CustomerID,
                OrderDate = OrderDate,
                RequiredDate = RequiredDate,
                ShippedDate = ShippedDate,
                ShipperID = ShipperID,
                ShipName = ShipName,
                ShipAddress = ShipAddress,
                Freight = Freight,
                PayWay = PayWay,
                PayDate = PayDate
            };

            _repo.CreateOrder(order);

            return RedirectToAction("Order");
        }
    }
}