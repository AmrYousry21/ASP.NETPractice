using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using NathansCRUDWebsite.Models;
using System.Data;
using Dapper;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Configuration;

namespace NathansCRUDWebsite
{
    public class ProductRepo : IProductRepository
    {
        private readonly IConfiguration _configuration;
        private readonly MySqlConnection _conn;

        public ProductRepo(IConfiguration configuration, MySqlConnection conn)
        {
            _configuration = configuration;
            _conn = conn;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            
            using (_conn)
            {
                _conn.Open();
                MySqlCommand cmd = _conn.CreateCommand();
                cmd.CommandText = "SELECT ProductID, Name, Price, CategoryID, OnSale, StockLevel FROM products;";

                MySqlDataReader reader = cmd.ExecuteReader();
                List<Product> products = new List<Product>();
                while (reader.Read())
                {
                    Product row = new Product();
                    row.ProductID = reader.GetInt32("ProductID");
                    row.Name = reader.GetString("Name");
                    

                    if (reader.IsDBNull(reader.GetOrdinal("Price")))
                    {
                        row.Price = null;
                    }
                    else
                    {
                        row.Price = reader.GetDouble("Price");
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("CategoryID")))
                    {
                        row.CategoryID = null;
                    }
                    else
                    {
                        row.CategoryID = reader.GetInt32("CategoryID");
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("OnSale")))
                    {
                        row.OnSale = null;
                    }
                    else
                    {
                        row.OnSale = reader.GetInt32("OnSale");
                    }

                    if (reader.IsDBNull(reader.GetOrdinal("StockLevel")))
                    {
                        row.StockLevel = null;
                    }
                    else
                    {
                        row.StockLevel = reader.GetInt32("StockLevel");
                    }

                    products.Add(row);
                }
                return products;
            }
        }

        public Product GetProduct(int id) 
        {
            return _conn.QuerySingle<Product>("SELECT * FROM PRODUCTS WHERE PRODUCTID = @id",
                new { id = id });
        }


        public void CreateProducts(Product p)
        {
            
            using (_conn)
            {
                _conn.Open();

                MySqlCommand cmd = _conn.CreateCommand();
                cmd.CommandText = "INSERT INTO products(Name, Price, CategoryID, OnSale, StockLevel) VALUES(@name, @price, @catid, @sale, @stock);";
                cmd.Parameters.AddWithValue("name", p.Name);
                cmd.Parameters.AddWithValue("price", p.Price);
                cmd.Parameters.AddWithValue("catid", p.CategoryID);
                cmd.Parameters.AddWithValue("sale", p.OnSale);
                cmd.Parameters.AddWithValue("stock", p.StockLevel);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(Product product)
        {
            
            using (_conn)
            {
                _conn.Execute("UPDATE products SET Name = @name, Price = @price WHERE ProductID = @id",
                 new { name = product.Name, price = product.Price, id = product.ProductID });
            }
        }

        public void DeleteProduct(Product product)
        {
            _conn.Execute("DELETE FROM REVIEWS WHERE ProductID = @id;",
                                       new { id = product.ProductID });
            _conn.Execute("DELETE FROM Sales WHERE ProductID = @id;",
                                       new { id = product.ProductID });
            _conn.Execute("DELETE FROM Products WHERE ProductID = @id;",
                                       new { id = product.ProductID });
        }


        public void InsertProduct(Product productToInsert)
        {
            _conn.Execute("INSERT INTO products (NAME, PRICE, CATEGORYID) VALUES (@name, @price, @categoryID);",
                new { name = productToInsert.Name, price = productToInsert.Price, categoryID = productToInsert.CategoryID });
        }

        public IEnumerable<Category> GetCategories()
        {
            return _conn.Query<Category>("SELECT * FROM categories;");
        }

        public Product AssignCategory()
        {
            var categoryList = GetCategories();
            var product = new Product();
            product.Categories = categoryList;

            return product;
        }
    }
}
