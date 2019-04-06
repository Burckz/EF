using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Product")]
    public class ProductsInRangeDto
    {
        public string name { get; set; }

        public decimal price { get; set; }

        public string buyer { get; set; }
    }
}
