using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Dapper;
using Team7MVC.Models;
using Team7MVC.ViewModels;

namespace Team7MVC.Repositories
{
    public class AdminRepository
    {
        private static string connString;
        private readonly SqlConnection conn;

        public AdminRepository()
        {
            if (string.IsNullOrEmpty(connString))
            {
                connString = ConfigurationManager.ConnectionStrings["WineDB"].ConnectionString;
            }

            conn = new SqlConnection(connString);
        }

        #region Product

        public List<Products> GetAllProducts()
        {
            List<Products> products;
            using (conn)
            {
                string sql = "select * from Products";
                products = conn.Query<Products>(sql).ToList();
            }

            return products;
        }

        public List<Products> GetAllProducts(string SortBy)
        {
            List<Products> products;
            using (conn)
            {
                string sql = $"select * from Products order by {SortBy}";
                products = conn.Query<Products>(sql).ToList();
            }

            return products;
        }

        public Products GetProduct(int id)
        {
            Products product;

            using (conn)
            {
                string sql = "select * from Products where ProductID = @ProductId";
                product = conn.QueryFirstOrDefault<Products>(sql, new { ProductId = id });
            }

            return product;
        }

        public void CreateProduct(Products product)
        {
            using (conn)
            {
                string sql = @"insert into Products (ProductName,Origin,Year,Capacity,UnitPrice,Stock,Grade,Variety,Area,Picture,Introduction,CategoryID)
                                values(@ProductName,@Origin,@Year,@Capacity,@UnitPrice,@Stock,@Grade,@Variety,@Area,@Picture,@Introduction,@CategoryID);";
                conn.Execute(sql, new { product.ProductName, product.Origin, product.Year, product.Capacity, product.UnitPrice, product.Stock, product.Grade, product.Variety, product.Area, product.Picture, product.Introduction, product.CategoryID });
            }
        }

        public void UpdateProduct(Products product)
        {
            using (conn)
            {
                string sql = @"update Products
                                set ProductName = @ProductName,
                                Origin = @Origin,
                                Year = @Year,
                                Capacity = @Capacity,
                                UnitPrice = @UnitPrice,
                                Stock = @Stock,
                                Grade = @Grade,
                                Variety = @Variety,
                                Area = @Area,
                                Picture = @Picture,
                                Introduction = @Introduction,
                                CategoryID = @CategoryID
                                where ProductID = @ProductID";
                conn.Execute(sql, new { product.ProductName, product.Origin, product.Year, product.Capacity, product.UnitPrice, product.Stock, product.Grade, product.Variety, product.Area, product.Picture, product.Introduction, product.CategoryID, product.ProductID });
            }
        }

        public void DeleteProduct(int Id)
        {
            using (conn)
            {
                string sql = @"delete from Products
                                where ProductID = @ProductId";
                conn.Execute(sql, new { ProductId = Id });
            }
        }
        #endregion

        #region Order
        public List<Orders> GetAllOrders()
        {
            List<Orders> orders;
            using (conn)
            {
                string sql = "select * from Orders";
                orders = conn.Query<Orders>(sql).ToList();
            }

            return orders;
        }

        public void CreateOrder(Orders order)
        {
            using (conn)
            {
                string sql = @"insert into Orders (OrderID,CustomerID,OrderDate,RequiredDate,ShippedDate,ShipperID,ShipName,ShipAddress,Freight,PayWay,PayDate)
                                values(@OrderID,@CustomerID,@OrderDate,@RequiredDate,@ShippedDate,@ShipperID,@ShipName,@ShipAddress,@Freight,@PayWay,@PayDate);";
                conn.Execute(sql, new { order.OrderID, order.CustomerID, order.OrderDate, order.RequiredDate, order.ShippedDate, order.ShipperID, order.ShipName, order.ShipAddress, order.Freight, order.PayWay, order.PayDate });
            }
        }

        public void UpdateOrder(Orders order)
        {
            using (conn)
            {
                string sql = @"update Orders
                                set OrderID=@OrderID,
                                CustomerID=@CustomerID,
                                OrderDate=@OrderDate,
                                RequiredDate=@RequiredDate,
                                ShippedDate=@ShippedDate,
                                ShipperID=@ShipperID,
                                ShipName=@ShipName,
                                ShipAddress=@ShipAddress,
                                Freight=@Freight,
                                PayWay=@PayWay,
                                PayDate=@PayDate";
                conn.Execute(sql, new { order.OrderID, order.CustomerID, order.OrderDate, order.RequiredDate, order.ShippedDate, order.ShipperID, order.ShipName, order.ShipAddress, order.Freight, order.PayWay, order.PayDate });
            }
        }

        public void DeleteOrder(int Id)
        {
            using (conn)
            {
                string sql = @"delete from Orders
                                where OrderID = @OrderId";
                conn.Execute(sql, new { OrderId = Id });
            }
        }

        #endregion

        #region Customer

        public List<AdminCustomersViewModel> GetAllCustomers()
        {
            List<AdminCustomersViewModel> customers;
            using (conn)
            {
                string sql = @"select c.CustomerID, c.Account, c.CustomerName, c.Gender, c.Email,
                                c.Address, c.Phone,
                                SUM(ISNULL(od.UnitPrice * od.Quantity,0)) as TotalCost
                                from Customers as c
                                LEFT OUTER JOIN Orders as o on o.CustomerID = c.CustomerID
                                LEFT OUTER JOIN [Order Details] as od on od.OrderID = o.OrderID
                                group by c.CustomerID, c.Account, c.CustomerName, c.Gender, c.Email,
                                c.Address, c.Phone";
                customers = conn.Query<AdminCustomersViewModel>(sql).ToList();
            }

            return customers;
        }

        #endregion

        #region Message
        public List<Messages> GetAllMessages()
        {
            List<Messages> messages;
            using (conn)
            {
                string sql = "select * from Messages";
                messages = conn.Query<Messages>(sql).ToList();
            }

            return messages;
        }

        #endregion
       
    }
}