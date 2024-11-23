using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Data;
using MvcFlowers.Models;

public class Manager
{
    // Метод для создания букета
    public static async Task<Bouqet> CreateBouquet(MvcFlowersContext context, List<FlowerSelection> selectedFlowers)
    {
        var bouquet = new Bouqet();

        // Получаем все FlowerId, которые уже существуют в других букете
        var existingFlowerIds = await context.Bouqet
            .SelectMany(b => b.Flowers.Select(f => f.FlowerId)) // Извлекаем FlowerId
            .Distinct() // Убираем дубликаты
            .ToListAsync();

        // Получаем все цветы, которые были выбраны пользователем и не существуют в других букете
        var flowerIds = selectedFlowers.Select(sf => sf.FlowerId).ToList();
        var flowers = await context.Flowers
            .Where(f => flowerIds.Contains(f.FlowerId) && !existingFlowerIds.Contains(f.FlowerId))
            .ToListAsync();

        if (!flowers.Any())
        {
            throw new InvalidOperationException("Не найдено ни одного выбранного цветка, который можно добавить.");
        }

        // Преобразуем цветы в объекты FlowerInBouqet и добавляем их в букет
        bouquet.Flowers = flowers.Select(f =>
        {
            var selectedFlower = selectedFlowers.First(sf => sf.FlowerId == f.FlowerId);
            return new FlowerInBouqet
            {
                FlowerId = f.FlowerId,
                Flower = f,  // Сохраняем ссылку на объект Flower
                Count = selectedFlower.Count // Устанавливаем количество из пользовательского ввода
            };
        }).ToList();

        return bouquet;
    }



    public static async Task<Bouqet> EditBouquet(MvcFlowersContext context, Bouqet bouqet, List<FlowerSelection> selectedFlowers)
    {
        // Извлечение всех FlowerId, которые уже существуют в других букете
        var existingFlowerIds = await context.Bouqet
            .Where(b => b.BouqetId != bouqet.BouqetId) // Исключаем текущий букет
            .SelectMany(b => b.Flowers.Select(f => f.FlowerId)) // Извлекаем FlowerId
            .Distinct() // Убираем дубликаты
            .ToListAsync();

        var existingBouquet = await context.Bouqet
            .Include(b => b.Flowers)
            .FirstOrDefaultAsync(b => b.BouqetId == bouqet.BouqetId);

        if (existingBouquet == null)
        {
            throw new InvalidOperationException("Букет не найден.");
        }

        // Добавление только новых цветов, которые не существуют в других букете
        foreach (var selectedFlower in selectedFlowers)
        {
            // Проверяем, существует ли цветок в букете или в других букете
            if (!existingBouquet.Flowers.Any(f => f.FlowerId == selectedFlower.FlowerId) &&
                !existingFlowerIds.Contains(selectedFlower.FlowerId))
            {
                var flower = await context.Flowers.FindAsync(selectedFlower.FlowerId);
                if (flower != null)
                {
                    // Создаем новый объект FlowerInBouqet и добавляем его в букет
                    var flowerInBouqet = new FlowerInBouqet
                    {
                        FlowerId = flower.FlowerId,
                        Flower = flower, // Сохраняем ссылку на объект Flower
                        Count = selectedFlower.Count // Устанавливаем количество из пользовательского ввода
                    };

                    existingBouquet.Flowers.Add(flowerInBouqet);
                }
            }
        }

        // Сохраняем изменения в контексте базы данных
        await context.SaveChangesAsync();

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
}
