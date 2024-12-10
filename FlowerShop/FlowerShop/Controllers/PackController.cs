using MvcFlowers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Data;
using MvcFlowers.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcFlowers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacksController : ControllerBase
    {
        private readonly MvcFlowersContext _context;

        public PacksController(MvcFlowersContext context)
        {
            _context = context;
        }

        // GET: api/packs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PackDto>>> GetPacks()
        {
            var packs = await _context.Packs.Include(p => p.Flower).ToListAsync();
            var packDtos = packs.Select(p => new PackDto
            {
                Id = p.Id,
                FlowerId = p.FlowerId,
                RecievementDate = p.RecievementDate,
                Count = p.Count,
            });

            return Ok(packDtos);
        }

        // GET: api/packs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PackDto>> GetPack(int id)
        {
            var pack = await _context.Packs
                .Include(p => p.Flower)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pack == null)
            {
                return NotFound();
            }

            var packDto = new PackDto
            {
                Id = pack.Id,
                FlowerId = pack.FlowerId,
                RecievementDate = pack.RecievementDate,
                Count = pack.Count,
            };

            return packDto;
        }


        [HttpPost]
        public async Task<ActionResult<PackDto>> CreatePack(PackCreateDto packCreateDto)
        {
            Pack pack = null;

            try
            {
                // Проверяем, существует ли цветок с таким же именем и цветом
                var existingFlower = await _context.Flowers
                    .Include(f => f.Packs) // Включаем связанные пакеты
                    .FirstOrDefaultAsync(f => f.Name == packCreateDto.FlowerName && f.Colour == packCreateDto.Color);

                if (existingFlower != null)
                {
                    // Если цветок существует, создаем новую партию
                    pack = new Pack
                    {
                        FlowerId = existingFlower.FlowerId,
                        RecievementDate = packCreateDto.RecievementDate,
                        Count = packCreateDto.Count,
                        Color = existingFlower.Colour,
                        Price = packCreateDto.Price,
                        FlowerName = existingFlower.Name,
                    };

                    // Добавляем новую партию в коллекцию Packs цветка
                    existingFlower.Packs.Add(pack);
                }
                else
                {
                    // Если цветка нет, создаем новый цветок
                    var newFlower = new Flower
                    {
                        Name = packCreateDto.FlowerName,
                        Price = packCreateDto.Price,
                        Colour = packCreateDto.Color,
                        Packs = new List<Pack>() // Инициализируем коллекцию
                    };

                    _context.Flowers.Add(newFlower);
                    await _context.SaveChangesAsync(); // Сохраняем изменения

                    pack = new Pack
                    {
                        FlowerId = newFlower.FlowerId,
                        RecievementDate = packCreateDto.RecievementDate,
                        Count = packCreateDto.Count,
                        Color = newFlower.Colour,
                        Price = packCreateDto.Price,
                        FlowerName = newFlower.Name,
                    };

                    // Добавляем новую партию в коллекцию Packs нового цветка
                    newFlower.Packs.Add(pack);
                }

                // Добавляем партию в контекст
                _context.Packs.Add(pack);
                await _context.SaveChangesAsync();

                var packDto = new PackDto
                {
                    Id = pack.Id,
                    FlowerId = pack.FlowerId,
                    RecievementDate = pack.RecievementDate,
                    Count = pack.Count,
                };

                return CreatedAtAction(nameof(GetPack), new { id = pack.Id }, packDto);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании упаковки: {ex.Message}");
            }
        }





        // PUT: api/packs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPack(int id, PackDto packDto)
        {
            if (id != packDto.Id)
            {
                return BadRequest();
            }

            var pack = new Pack
            {
                Id = packDto.Id,
                FlowerId = packDto.FlowerId,
                RecievementDate = packDto.RecievementDate,
                Count = packDto.Count
            };

            _context.Entry(pack).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PackExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/packs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePack(int id)
        {
            var pack = await _context.Packs.FindAsync(id);
            if (pack == null)
            {
                return NotFound();
            }

            _context.Packs.Remove(pack);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PackExists(int id)
        {
            return _context.Packs.Any(e => e.Id == id);
        }
    }
}
