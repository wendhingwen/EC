using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Team7MVC.Models;
using Team7MVC.ViewModels;

namespace Team7MVC.Repositories
{
    public class ProductRepository
    {
        private static string connString;
        private readonly SqlConnection conn;
        private SqlConnection _conn;

        public ProductRepository()
        {
            if (string.IsNullOrEmpty(connString))
            {
                connString = ConfigurationManager.ConnectionStrings["WineDB"].ConnectionString;
            }

            conn = new SqlConnection(connString);
            //_conn = new SqlConnection(connString);
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

        public Products GetProductById(int id)
        {
            Products product;

            using (conn)
            {
                string sql = "select * from Products where ProductID = @ProductId";
                product = conn.QueryFirstOrDefault<Products>(sql, new { ProductId = id });
            }

            return product;
        }

        public void CreateShoppingCartData( string Account, int ProductId, int buyQty)
        {
            int OrderQty = 0;
            using (conn)
            {
                string sql = @"SELECT s.Quantity 
                               FROM ShopLists s
                               INNER JOIN Customers c ON c.CustomerID = s.CustomerID
                               WHERE  c.Account = @Account AND s.ProductID = @ProductId";
                OrderQty = conn.QueryFirstOrDefault<int>(sql, new { Account, ProductId });

                if (OrderQty == 0)
                {
                    sql = @"insert into ShopLists (CustomerID, ProductID, Price, Quantity)
                            values((select CustomerID from Customers where Account = @Account), @ProductID, (select UnitPrice from Products where ProductID = @Product2ID),@Quantity)";
                    conn.Execute(sql, new { Account, ProductID = ProductId, Product2ID = ProductId, Quantity = buyQty });
                }
                else
                {
                    OrderQty = OrderQty + buyQty;

                    sql = @"UPDATE ShopLists 
                            SET Quantity = @OrderQty
                            FROM ShopLists s
                            INNER JOIN Customers c ON c.CustomerID = s.CustomerID
                            WHERE  c.Account = @Account AND ProductID = @ProductId";
                    conn.Execute(sql, new { OrderQty, Account, ProductId });
                }
            }
        }

        public List<ShopListsViewModel> ShopList(string CustomerID)
        {
            List<ShopListsViewModel> shopLists;

            using (conn)
            {
                string sql = @"select p.Picture, p.ProductName, p.Year, p.Origin, sh.Price,
                                sh.Quantity, (sh.Price * sh.Quantity) as TotalCost
                                from ShopLists as sh
                                INNER JOIN Products as p on p.ProductID = sh.ProductID
                                INNER JOIN Customers as c on c.CustomerID = sh.CustomerID
                                where c.Account = @CustomerID";
                shopLists = conn.Query<ShopListsViewModel>(sql, new { CustomerID }).ToList();
            }

            return shopLists;
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
                        from ShopLists as sh
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
    }
}