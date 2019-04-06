using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CarDealer
{

    public class StartUp
    {
        private static string importSuppliersJson = File
            .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\CarDealer\\CarDealer\\Datasets\\suppliers.json")
            .ReadToEnd();

        private static string importPartsJson = File
            .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\CarDealer\\CarDealer\\Datasets\\parts.json")
            .ReadToEnd();

        private static string importCarsJson = File
            .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\CarDealer\\CarDealer\\Datasets\\cars.json")
            .ReadToEnd();

        private static string importCustomersJson = File
            .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\CarDealer\\CarDealer\\Datasets\\customers.json")
            .ReadToEnd();

        private static string importSalesJson = File
            .OpenText("D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\CarDealer\\CarDealer\\Datasets\\sales.json")
            .ReadToEnd();

        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();
            //Console.WriteLine(ImportSuppliers(context, importSuppliersJson));
            //Console.WriteLine(ImportParts(context, importPartsJson));
            //Console.WriteLine(ImportCars(context, importCarsJson));
            //Console.WriteLine(ImportCustomers(context, importCustomersJson));
            //Console.WriteLine(ImportSales(context, importSalesJson));

            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var top10Sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    customerName = s.Customer.Name,
                    Discount = $"{s.Discount:f2}",
                    price = $"{s.Car.PartCars.Sum(pc => pc.Part.Price):f2}",
                    priceWithDiscount = $"{(s.Car.PartCars.Sum(pc => pc.Part.Price)) * (1 - (s.Discount / 100)):f2}"
                })
                .ToList();

            return $"{JsonConvert.SerializeObject(top10Sales, Formatting.Indented)}";
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new
                {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToList();

            return $"{JsonConvert.SerializeObject(customers, Formatting.Indented)}";

            throw new NotImplementedException();
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                        
                    },
                    parts = c.PartCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = $"{pc.Part.Price:f2}"
                    })
                        .ToList()
                })
                .ToList();

            return $"{JsonConvert.SerializeObject(cars, Formatting.Indented)}";
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToList();

            return $"{JsonConvert.SerializeObject(suppliers, Formatting.Indented)}";

            throw new NotImplementedException();
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            return $"{JsonConvert.SerializeObject(cars, Formatting.Indented)}";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString(@"dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToList();

            return $"{JsonConvert.SerializeObject(customers, Formatting.Indented)}";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            JArray sales = JArray.Parse(inputJson);

            foreach (var saleJson in sales)
            {
                var sale = JsonConvert.DeserializeObject<Sale>(saleJson.ToString());

                context.Sales.Add(sale);
            }

            return $"Successfully imported {context.SaveChanges().ToString()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            JArray customers = JArray.Parse(inputJson);

            foreach (var customerJson in customers)
            {
                var customer = JsonConvert.DeserializeObject<Customer>(customerJson.ToString());

                context.Customers.Add(customer);
            }


            return $"Successfully imported {context.SaveChanges().ToString()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            JArray cars = JArray.Parse(inputJson);

            foreach (var carJson in cars)
            {
                var car = JsonConvert.DeserializeObject<Car>(carJson.ToString());
                context.Cars.Add(car);

                foreach (var partJson in carJson["partsId"].Distinct())
                {
                    PartCar partCar = new PartCar
                    {
                        PartId = int.Parse(partJson.ToString()),
                        CarId = car.Id
                    };

                    if (!context.PartCars.Contains(partCar))
                    {
                        car.PartCars.Add(partCar);
                    }

                }

            }
            context.SaveChanges();

            return $"Successfully imported {context.Cars.Local.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            JArray parts = JArray.Parse(inputJson);

            foreach (var partJson in parts)
            {
                var part = JsonConvert.DeserializeObject<Part>(partJson.ToString());

                part.Supplier = context.Suppliers.FirstOrDefault(s => s.Id.ToString() == partJson["supplierId"].ToString());

                if (part.Supplier == null)
                {
                    continue;
                }

                context.Parts.Add(part);
            }

            return $"Successfully imported {context.SaveChanges().ToString()}.";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            JArray suppliersJson = JArray.Parse(inputJson);

            foreach (var supplierJson in suppliersJson)
            {
                var supplier = JsonConvert.DeserializeObject<Supplier>(supplierJson.ToString());

                context.Suppliers.Add(supplier);
            }


            return $"Successfully imported {context.SaveChanges().ToString()}.";
        }
    }
}