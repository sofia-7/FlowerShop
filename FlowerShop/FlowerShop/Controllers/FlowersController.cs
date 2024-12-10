using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcFlowers.Data;
using MvcFlowers.Models;

namespace MvcFlowers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowersController : ControllerBase
    {
        private readonly MvcFlowersContext _context;

        public FlowersController(MvcFlowersContext context)
        {
            _context = context;
        }

        // GET: api/MonoFlowers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flower>>> GetMonoFlower()
        {
            return Ok(await _context.Flowers.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FlowerDto>> GetFlower(int id)
        {
            var flower = await _context.Flowers
                .Include(f => f.Packs) // Включаем связанные партии
                .FirstOrDefaultAsync(f => f.FlowerId == id);

            if (flower == null)
            {
                return NotFound();
            }

            var flowerDto = new FlowerDto
            {
                Id = flower.FlowerId,
                Name = flower.Name,
                Colour = flower.Colour,
                Price = flower.Price,
                Packs = flower.Packs.Select(p => new PackDto
                {
                    Id = p.Id,
                    RecievementDate = p.RecievementDate,
                    Count = p.Count
                }).ToList() // Преобразуем партии в DTO
            };

            return Ok(flowerDto);
        }


        // POST: api/MonoFlowers
        [HttpPost]
        public async Task<ActionResult<Flower>> CreateMonoFlower([FromBody] Flower monoFlowers)
        {
            if (ModelState.IsValid)
            {
                _context.Add(monoFlowers);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMonoFlower), new { id = monoFlowers.FlowerId }, monoFlowers);
            }

            return BadRequest(ModelState);
        }

        // PUT: api/MonoFlowers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMonoFlower(int id, [FromBody] Flower monoFlowers)
        {
            if (id != monoFlowers.FlowerId)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(monoFlowers);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MonoFlowersExists(monoFlowers.FlowerId))
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

            return BadRequest(ModelState);
        }

        // DELETE: api/MonoFlowers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonoFlower(int id)
        {
            var monoFlowers = await _context.Flowers.FindAsync(id);
            if (monoFlowers == null)
            {
                return NotFound();
            }

            _context.Flowers.Remove(monoFlowers);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MonoFlowersExists(int id)
        {
            return _context.Flowers.Any(e => e.FlowerId == id);
        }
    }
}
