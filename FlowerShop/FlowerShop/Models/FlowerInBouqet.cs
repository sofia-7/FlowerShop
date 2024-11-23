using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcFlowers.Models
{
    public class FlowerInBouqet
    {
        [Key]
        public int FlowerId { get; set; }

        [ForeignKey("FlowerId")]
        public Flower Flower { get; set; }
        public int Count { get; set; }
        //public Bouqet Bouqet { get; set; }
    }
}
