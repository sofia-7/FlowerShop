using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcFlowers.Models
{
    public class Pack
    {
        [Key]
        public int Id { get; set; }
        public int FlowerId { get; set; }

        [ForeignKey("FlowerId")]
        public Flower Flower { get; set; }

        [DataType(DataType.Date)]
        public DateTime RecievementDate { get; set; }
        public int Count { get; set; }
        public string FlowerName { get; set; }
        public string Color { get; set; }
        public decimal Price { get; set; }
    }
}
