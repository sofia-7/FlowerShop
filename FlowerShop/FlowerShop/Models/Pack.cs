using System.ComponentModel.DataAnnotations;

namespace MvcFlowers.Models
{
    public class Pack
    {
        public int Id { get; set; }
        public Flower Flower { get; set; }

        [DataType(DataType.Date)]
        public DateTime RecievementDate { get; set; }
        public int Count { get; set; }
    }
}
