using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcFlowers.Models
{
    public class Flower
    {
        [Key]
        public int FlowerId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string? Colour { get; set; }
        public List<Pack> Packs { get; set; }

        [NotMapped] 
        public int PriceAsInt
        {
            get => (int)Price; 
            set => Price = value; 
        }
        
        public string DisplayName => $"{Name} ({Colour})";
    
    // Метод для подсчета свежести цветка
    public int CalculateFreshness()
        {
            if (Packs.Last() != null) {
                // Получаем количество дней, прошедших с момента получения цветка
                int daysSinceReceived = (DateTime.Now - Packs.Last().RecievementDate).Days;
                return daysSinceReceived;
            }
            return -1;
        }

        // Метод для проверки свежести цветка
        public bool IsFresh()
        {
            var days = CalculateFreshness();
            return (days != -1) && days < 7; // Свежий, если меньше 7 дней
        }
    }
}
