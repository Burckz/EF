using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductShop.Data;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //ImportUsers(context, File
            //    .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\ProductShop\\ProductShop\\Datasets\\users.json")
            //    .ReadToEnd());

            //ImportProducts(context, File
            //    .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\ProductShop\\ProductShop\\Datasets\\products.json")
            //    .ReadToEnd());

            //ImportCategories(context, File
            //    .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\ProductShop\\ProductShop\\Datasets\\categories.json")
            //    .ReadToEnd());

            //ImportCategoryProducts(context, File
            //    .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\ProductShop\\ProductShop\\Datasets\\categories-products.json")
            //    .ReadToEnd());

            Console.WriteLine(GetUsersWithProducts(context));

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = new
            {
                usersCount = context.Users
                .Where(x => x.ProductsSold.Count > 0 && x.ProductsSold.Any(ps => ps.Buyer != null)).Count(),

                users = context.Users
                .Where(u => u.ProductsSold.Count > 0 && u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderByDescending(u => u.ProductsSold.Where(ps => ps.Buyer != null).Count())
                .Select(u =>

                new
                {

                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Count(ps => ps.Buyer != null),
                        products = u.ProductsSold
                        .Where(ps => ps.Buyer != null)
                        .Select(ps => new
                        {
                            name = ps.Name,
                            price = ps.Price
                        }).ToList()
                    }


                }).ToList()
            };

            return JsonConvert.SerializeObject(users, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });
            
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .OrderByDescending(c => c.CategoryProducts.Count)
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoryProducts.Count,
                    averagePrice = $"{(c.CategoryProducts.Average(cp => cp.Product.Price)):f2}",
                    totalRevenue = $"{(c.CategoryProducts.Sum(cp => cp.Product.Price)):f2}"
                }).ToList();


            return (JsonConvert.SerializeObject(categories));
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Include(u => u.ProductsSold)
                .Where(u => u.ProductsSold.Count > 0 && u.ProductsSold.Any(ps => ps.Buyer != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(ps => ps.Buyer != null)
                    .Select(ps => new
                    {
                        name = ps.Name,
                        price = ps.Price,
                        buyerFirstName = ps.Buyer.FirstName,
                        buyerLastName = ps.Buyer.LastName
                    })
                }).ToList();

            var result = JsonConvert.SerializeObject(users);

            return result;
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName + " " + p.Seller.LastName
                })
                .ToList();

            var result = JsonConvert.SerializeObject(products);

            return result;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            JArray categoriesProductsCollection = JArray.Parse(inputJson);

            foreach (var categoryProductsJson in categoriesProductsCollection)
            {
                var categoryProduct = JsonConvert.DeserializeObject<CategoryProduct>(categoryProductsJson.ToString());

                categoryProduct.Product = context.Products.FirstOrDefault(p => p.Id.ToString() == categoryProductsJson["ProductId"].ToString());


                context.CategoryProducts.Add(categoryProduct);


            }

            return $"Successfully imported {context.SaveChanges().ToString()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            JArray categoriesCollection = JArray.Parse(inputJson);

            foreach (var categoryJson in categoriesCollection)
            {
                var category = JsonConvert.DeserializeObject<Category>(categoryJson.ToString());


                if (category.Name != null)
                {
                    context.Categories.Add(category);

                }
            }

            return $"Successfully imported {context.SaveChanges().ToString()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            JArray productCollection = JArray.Parse(inputJson);

            foreach (var productJson in productCollection)
            {
                var product = JsonConvert.DeserializeObject<Product>(productJson.ToString());

                context.Products.Add(product);
            }

            return $"Successfully imported {context.SaveChanges().ToString()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            JArray userCollection = JArray.Parse(inputJson);

            foreach (var userJson in userCollection)
            {
                var user = JsonConvert.DeserializeObject<User>(userJson.ToString());

                context.Users.Add(user);
            }

            return $"Successfully imported {context.SaveChanges().ToString()}";
        }
    }
}