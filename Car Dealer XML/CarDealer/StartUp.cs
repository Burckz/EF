using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        private static string ImportSupplierString = File.OpenText(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Car Dealer XML\\CarDealer\\Datasets\\suppliers.xml").ReadToEnd();

        private static string ImportPartString = File.OpenText(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Car Dealer XML\\CarDealer\\Datasets\\parts.xml").ReadToEnd();

        private static string ImportCarsString = File.OpenText(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Car Dealer XML\\CarDealer\\Datasets\\cars.xml").ReadToEnd();

        private static string ImportSalesString = File.OpenText(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Car Dealer XML\\CarDealer\\Datasets\\sales.xml").ReadToEnd();

        private static string ImportCustomersString = File.OpenText(@"D:\\Studies\\SoftUni\\Database - Entity Framework\\SoftUni Project Homeworks\\Car Dealer XML\\CarDealer\\Datasets\\customers.xml").ReadToEnd();


        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //ImportSuppliers(context, ImportSupplierString);
            //ImportParts(context, ImportPartString);
            //ImportCars(context, ImportCarsString);
            //ImportSales(context, ImportSalesString));
            //ImportCustomers(context, ImportCustomersString);

            Mapper.Initialize(cfg => cfg.AddProfile<CarDealerProfile>());

            Console.WriteLine(GetLocalSuppliers(context));

        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleWithDiscountDto
                {
                    Car = Mapper.Map<CarExportDto>(s.Car),
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100))).ToString().TrimEnd('0')
                })
                .ToList();
            

            var serializer = new XmlSerializer(typeof(List<SaleWithDiscountDto>), new XmlRootAttribute("sales"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), sales, ns);

            return $"{result}";
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new CustomerDto
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Sum(s => s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CustomerDto>), new XmlRootAttribute("customers"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), customers, ns);

            return $"{result}";
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .OrderByDescending(c => c.TravelledDistance)
                .ThenBy(c => c.Model)
                .Select(c => new CarsWithPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars.Select(pc => new PartExportDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToList()
                })
                .Take(5)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CarsWithPartsDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), cars, ns);

            return $"{result}";
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new SupplierNotImporterDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count()
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<SupplierNotImporterDto>), new XmlRootAttribute("suppliers"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), suppliers, ns);

            return $"{result}".Trim();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var carsBmw = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new CarsFromBMWDto
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CarsFromBMWDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), carsBmw, ns);

            return $"{result}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Select(c => new CarsWithDistanceDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .Take(10)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<CarsWithDistanceDto>), new XmlRootAttribute("cars"));
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            StringBuilder result = new StringBuilder();

            serializer.Serialize(new StringWriter(result), cars, ns);

            return $"{result}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Customer[]), new XmlRootAttribute("Customers"));

            var customers = (Customer[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var customer in customers)
            {
                context.Customers.Add(customer);
            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Sale[]), new XmlRootAttribute("Sales"));

            var sales = (Sale[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var sale in sales)
            {
                if (context.Cars.Any(c => c.Id == sale.CarId))
                {

                    context.Sales.Add(sale);

                }
            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(CarDto[]), new XmlRootAttribute("Cars"));

            var cars = (CarDto[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var car in cars)
            {
                Car newCar = new Car
                {
                    Model = car.Model,
                    Make = car.Make,
                    TravelledDistance = car.TravelledDistance,
                    PartCars = new List<PartCar>()
                };

                context.Cars.Add(newCar);

                foreach (var part in car.Parts)
                {
                    if (context.Parts.Any(p => p.Id == part.PartId) &&
                        !newCar.PartCars.Any(cpc => cpc.PartId == part.PartId))
                    {
                        PartCar partCar = new PartCar
                        {
                            PartId = part.PartId,
                            CarId = newCar.Id
                        };

                        if (!context.PartCars.Contains(partCar))
                        {
                            newCar.PartCars.Add(partCar);
                        }

                    }
                }

                context.SaveChanges();
            }

            return $"Successfully imported {context.Cars.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Part[]), new XmlRootAttribute("Parts"));

            var parts = (Part[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var part in parts)
            {
                if (context.Suppliers.Any(s => s.Id == part.SupplierId))
                {
                    context.Parts.Add(part);

                }
            }

            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var serializer = new XmlSerializer(typeof(Supplier[]), new XmlRootAttribute("Suppliers"));

            var suppliers = (Supplier[])serializer.Deserialize(new StringReader(inputXml));

            foreach (var supplier in suppliers)
            {
                context.Suppliers.Add(supplier);
            }

            return $"Successfully imported {context.SaveChanges()}";
        }
    }
}