using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Team7MVC.ViewModels;
using Dapper;
using Team7MVC.Models;

namespace Team7MVC.Repositories
{
    public class WineRepository
    {
        private static string connString;
        private readonly SqlConnection conn;

        int affectedRows = -1;

        public WineRepository()
        {
            if (string.IsNullOrEmpty(connString))
            {
                connString = ConfigurationManager.ConnectionStrings["WineDB"].ConnectionString;
            }

            conn = new SqlConnection(connString);
        }

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

        public List<ShopListsViewModel> ShopList(string CustomerID)////////////////////////////////////////////
        {
            List<ShopListsViewModel> shopLists;

            using (conn)
            {
                string sql = @"select p.Picture, p.ProductName, p.Year, p.Origin, sh.Price,
                                sh.Quantity, (sh.Price * sh.Quantity) as TotalCost
                                from ShopList as sh
                                INNER JOIN Products as p on p.ProductID = sh.ProductID
                                INNER JOIN Customers as c on c.CustomerID = sh.CustomerID
                                where c.Account = @CustomerID";
                shopLists = conn.Query<ShopListsViewModel>(sql, new { CustomerID }).ToList();
            }

            return shopLists;
        }

        public int GetCustomerID(string Account)
        {
            int CustomerId;

            using (conn)
            {
                string sql = @"select CustomerID 
                                from Customers
                                where Account = @Account";

                CustomerId = conn.QueryFirstOrDefault<int>(sql, new { Account });
            }

            return CustomerId;
        }

        public void Payment(Orders orders, string Account)
        {
            List<ShopLists> shopLists;
            int OrderID = 0;
            using (conn)
            {
                string sql = @"insert into Orders (CustomerID, OrderDate, RequiredDate, ShipName,
					            ShipperID, ShipAddress, Freight, PayWay, PayDate)
                                values((select CustomerID from Customers where Account = @Account),
                                @OrderDate, @RequiredDate, @ShipName,@ShipperID, @ShipAddress,
                                @Freight, @PayWay, @PayDate)";
                conn.Execute(sql, new { Account, orders.OrderDate, orders.RequiredDate, orders.ShipName, orders.ShipperID, orders.ShipAddress, orders.Freight, orders.PayWay, orders.PayDate });

                sql = @"select OrderID
                        from Orders as o
                        INNER JOIN Customers as c on c.CustomerID = o.CustomerID
                        where Account = 'Account01'
                        order by OrderID Desc";

                OrderID = conn.QueryFirstOrDefault<int>(sql);

                sql = @"select sh.CustomerID, ProductID, Price, Quantity 
                        from ShopList as sh
                        INNER JOIN Customers as c on c.CustomerID = sh.CustomerID
                        where Account = @Account";
                shopLists = conn.Query<ShopLists>(sql, new { Account }).ToList();

                sql = @"insert into [Order Details] (OrderID, ProductID, UnitPrice, Quantity,
                        Discount)
                        values(@OrderID, @ProductID, @Price, @Quantity, 1)";
                foreach (var item in shopLists)
                {
                    conn.Execute(sql, new { OrderID, item.ProductID, item.Price, item.Quantity });
                }
            }
        }

        public int CreateMessages(Messages mess)
        {
            //List<Questions> questions;
            using (conn)
            {
                string sql = @"INSERT INTO Messages(Name, Email, Phone, QuestionCategory, Comments, Datetime) VALUES (@name, @email, @phone, @questionCategory, @comments, @dateTime)";
                affectedRows = conn.Execute(sql, new { mess.Name, mess.Email, mess.Phone, mess.QuestionCategory, mess.Comments, mess.Datetime });
            }

            return affectedRows;
        }
    }
}