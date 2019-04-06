using System.Xml.Serialization;

namespace CarDealer.Models
{
    public class PartCar
    {
        [XmlElement("partId")]
        public int PartId { get; set; }
        [XmlIgnore]
        public Part Part { get; set; }

        [XmlElement()]
        public int CarId { get; set; }
        [XmlIgnore]
        public Car Car { get; set; }
    }
}
