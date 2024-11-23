using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcFlowers.Models
{
    public class Bouqet
    {
        [Key]
        public int BouqetId { get; set; }
        public List<FlowerInBouqet> Flowers { get; set; } = new List<FlowerInBouqet>();

        //[NotMapped]
        public decimal TotalPrice => Flowers.Sum(f => f.Flower.PriceAsInt * f.Count);
        [NotMapped]
        public int FlowersCount => Flowers.Sum(fl => fl.Count);
        public string SelectedFlowerIds { get; set; }

        public Bouqet()
        {
            Flowers = new List<FlowerInBouqet>();
        }

        // Метод для подсчета стоимости букета
        public decimal CalculateTotalPrice()
        {
            return Flowers.Sum(flower => flower.Flower.Price * flower.Count);
        }
        public void AddFlowers(Flower flower, int count)
        {
            // Проверяем, существует ли цветок в букете
            var existingFlowerInBouqet = Flowers.FirstOrDefault(f => f.FlowerId == flower.FlowerId);

            if (existingFlowerInBouqet != null)
            {
                // Если цветок уже есть в букете, увеличиваем его количество
                existingFlowerInBouqet.Count += count;
            }
            else
            {
                // Если цветка нет в букете, добавляем новый объект FlowerInBouqet
                Flowers.Add(new FlowerInBouqet
                {
                    FlowerId = flower.FlowerId,
                    Flower = flower,
                    Count = count
                });
            }
        }

        public void RemoveFlowers(Flower flower, int count)
        {
            // Находим цветок в букете
            var existingFlowerInBouqet = Flowers.FirstOrDefault(f => f.FlowerId == flower.FlowerId);

            if (existingFlowerInBouqet != null)
            {
                // Если количество цветков в букете больше чем количество, которое нужно удалить
                if (existingFlowerInBouqet.Count > count)
                {
                    existingFlowerInBouqet.Count -= count; // Уменьшаем количество
                }
                else
                {
                    // Если количество цветков в букете меньше или равно количеству, которое нужно удалить
                    Flowers.Remove(existingFlowerInBouqet); // Удаляем цветок из букета
                }
            }
        }


        /*
                // Метод для создания букета
                public static async Task<Bouqet> CreateBouquet(MvcFlowersContext context, string selectedFlowerIds)
                {
                    var bouquet = new Bouqet();

                    // Преобразуем строку идентификаторов в список целых чисел
                    var selectedFlowerIdList = selectedFlowerIds
                        .Split(',')
                        .Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null)
                        .Where(id => id.HasValue)
                        .Select(id => id.Value)
                        .ToList();

                    // Получаем все MonoFlowerId, которые уже существуют в других букете
                    var existingFlowerIds = await context.Bouqet
                        .SelectMany(b => b.Flowers.Select(f => f.FlowerId)) // Извлекаем MonoFlowerId
                        .Distinct() // Убираем дубликаты
                        .ToListAsync();

                    // Извлекаем доступные цветы, которые не существуют в других букете
                    bouquet.Flowers = await context.MonoFlowers
                        .Where(f => selectedFlowerIdList.Contains(f.FlowerId) && !existingFlowerIds.Contains(f.FlowerId))
                        .ToListAsync();

                    if (!bouquet.Flowers.Any())
                    {
                        throw new InvalidOperationException("Не найдено ни одного выбранного цветка, который можно добавить.");
                    }

                    return bouquet;
                }

                public static async Task<Bouqet> EditBouquet(MvcFlowersContext context, Bouqet bouqet, List<int> selectedFlowerIds)
                {
                    // Извлечение всех MonoFlowerId, которые уже существуют в других букете
                    var existingFlowerIds = await context.Bouqet
                        .Where(b => b.BouqetId != bouqet.BouqetId) // Исключаем текущий букет
                        .SelectMany(b => b.Flowers.Select(f => f.FlowerId)) // Извлекаем MonoFlowerId
                        .Distinct() // Убираем дубликаты
                        .ToListAsync();

                    // Извлечение цветов из базы данных
                    var selectedFlowers = await context.MonoFlowers
                        .Where(f => selectedFlowerIds.Contains(f.FlowerId))
                        .ToListAsync();

                    var existingBouquet = await context.Bouqet
                        .Include(b => b.Flowers)
                        .FirstOrDefaultAsync(b => b.BouqetId == bouqet.BouqetId);

                    if (existingBouquet == null)
                    {
                        throw new InvalidOperationException("Букет не найден.");
                    }

                    // Добавление только новых цветов, которые не существуют в других букете
                    foreach (var flower in selectedFlowers)
                    {
                        // Добавляем только если цветок не существует в других букете
                        if (!existingBouquet.Flowers.Any(f => f.FlowerId == flower.FlowerId) &&
                            !existingFlowerIds.Contains(flower.FlowerId))
                        {
                            existingBouquet.Flowers.Add(flower);
                        }
                    }

                    return existingBouquet;
                }

                public static async Task DeleteBouquet(MvcFlowersContext context, int bouqetId)
                {
                    var bouqet = await context.Bouqet
                        .Include(b => b.Flowers)
                        .FirstOrDefaultAsync(b => b.BouqetId == bouqetId);

                    if (bouqet == null)
                    {
                        throw new InvalidOperationException("Букет не найден.");
                    }

                    // Удаляем все цветы из букета
                    bouqet.Flowers.Clear();

                    // Удаляем букет из контекста
                    context.Bouqet.Remove(bouqet);
                }

                public static async Task DeleteFlower(MvcFlowersContext context, int bouqetId, int flowerId)
                {
                    var bouqet = await context.Bouqet
                        .Include(b => b.Flowers)
                        .FirstOrDefaultAsync(b => b.BouqetId == bouqetId);

                    if (bouqet == null)
                    {
                        throw new InvalidOperationException("Букет не найден.");
                    }

                    var flowerToRemove = bouqet.Flowers.FirstOrDefault(f => f.FlowerId == flowerId);
                    if (flowerToRemove == null)
                    {
                        throw new InvalidOperationException("Цветок не найден в букете.");
                    }

                    bouqet.Flowers.Remove(flowerToRemove);
                }

                public static async Task<List<int>> GetExistingMonoFlowerIds(MvcFlowersContext context, int currentBouqetId)
                {
                    // Получаем все MonoFlowerId, которые уже существуют в других букете
                    var existingMonoFlowerIds = await context.Bouqet
                        .Where(b => b.BouqetId != currentBouqetId) // Исключаем текущий букет
                        .SelectMany(b => b.Flowers.Select(f => f.FlowerId)) // Извлекаем MonoFlowerId
                        .Distinct() // Убираем дубликаты
                        .ToListAsync();

                    return existingMonoFlowerIds;
                }
        */
    }
}
