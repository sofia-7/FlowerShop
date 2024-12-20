﻿// DTO для метода GET
public class PackDto
{
    public int Id { get; set; }
    public int FlowerId { get; set; }
    public DateTime RecievementDate { get; set; }
    public int Count { get; set; }
}

// DTO для метода POST
public class PackCreateDto
{
    public int? FlowerId { get; set; }
    public DateTime RecievementDate { get; set; }
    public int Count { get; set; }
    public string FlowerName { get; set; }
    public string Color { get; set; }
    public decimal Price { get; set; }
}

public class FlowerDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Colour { get; set; }
    public decimal Price { get; set; }
    public List<PackDto> Packs { get; set; } // Добавляем свойство для партий
}