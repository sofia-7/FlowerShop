namespace MvcFlowers.Models
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public int BouqetId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
