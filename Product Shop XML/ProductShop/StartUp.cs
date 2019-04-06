using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        private static string ImportUsersString = XDocument.Load(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Product Shop XML\\ProductShop\\Datasets\\users.xml").ToString();

        private static string ImportProductsString = XDocument.Load(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Product Shop XML\\ProductShop\\Datasets\\products.xml").ToString();

        private static string ImportCategoriesString = XDocument.Load(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Product Shop XML\\ProductShop\\Datasets\\categories.xml").ToString();

        private static string ImportCategoriesProductsString = XDocument.Load(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Product Shop XML\\ProductShop\\Datasets\\categories-products.xml").ToString();

        private static string ImportTestString = XDocument.Load(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Product Shop XML\\ProductShop\\Datasets\\test.xml").ToString();

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            Mapper.Initialize(cfg => cfg.AddProfile(new ProductShopProfile()));

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //ImportUsers(context, ImportUsersString);
            //ImportProducts(context, ImportProductsString);
            //ImportCategories(context, ImportCategoriesString);
            //ImportCategoryProducts(context, ImportCategoriesProductsString);

            Console.WriteLine(GetUsersWithProducts(context));
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => 
                new UsersWithProductsDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto
                    {
                        Count = u.ProductsSold.Count(),
                        ProductsSold = u.ProductsSold.Select(p => new Product
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            LastExportDto export = new LastExportDto()
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = users
            };

            var serializer = new XmlSerializer(typeof(LastExportDto), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), export, ns);

            return $"{result}";
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoryDto
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToList();

            var serializer = new XmlSerializer(categories.GetType(), new XmlRootAttribute("Categories"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), categories, ns);

            return $"{result}";
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UsersSoldProductDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    ProductsSold = u.ProductsSold.Select(ps => new Product
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    })
                    .ToArray()
                })
                .Take(5)
                .ToArray();

            var serializer = new XmlSerializer(typeof(UsersSoldProductDto[]), new XmlRootAttribute("Users"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");


            StringBuilder result = new StringBuilder();
            serializer.Serialize(new StringWriter(result), users, ns);

            return $"{result}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price <= 1000 && p.Price >= 500)
                .OrderBy(p => p.Price)
                .Select(p => new ProductsInRangeDto{
                    name = p.Name,
                    price = p.Price,
                    buyer = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .Take(10)
                .ToList();

            StringBuilder result = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ProductsInRangeDto>), new XmlRootAttribute("Products"));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            serializer.Serialize(new StringWriter(result), products, ns);

            return $"{result}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CategoryProduct[]), new XmlRootAttribute("CategoryProducts"));

            var categoryProducts = (CategoryProduct[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var categoryProduct in categoryProducts)
            {
                if (context.Products.Any(p => p.Id == categoryProduct.ProductId) &&
                    context.Categories.Any(c => c.Id == categoryProduct.CategoryId))
                {
                    context.CategoryProducts.Add(categoryProduct);
                }

            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Category[]), new XmlRootAttribute("Categories"));

            var categories = (Category[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var category in categories)
            {
                context.Categories.Add(category);
            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Product[]), new XmlRootAttribute("Products"));

            var products = (Product[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var product in products)
            {
                context.Products.Add(product);
            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(User[]), new XmlRootAttribute("Users"));

            var users = (User[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            return $"Successfully imported {context.SaveChanges()}";
        }
    }
}