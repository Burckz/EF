using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;

namespace VaporStore.Datasets.ImportDtos
{
    [XmlType("Purchase")]
    public class PurchaseDto
    {
        [XmlElement("Type")]
        [Required]
        public string Type { get; set; }

        [XmlElement("Key")]
        [Required]
        [RegularExpression(@"[\dA-Z]{4}-[\dA-Z]{4}-[\dA-Z]{4}")]
        public string ProductKey { get; set; }

        [XmlElement("Date")]
        [Required]
        public string Date { get; set; }

        [XmlElement("Card")]
        [Required]
        public string Card { get; set; }

        [XmlAttribute("title")]
        [Required]
        public string Game { get; set; }
    }
}
