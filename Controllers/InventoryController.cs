using Microsoft.AspNetCore.Mvc;
using InventorySystem.Models;
using InventorySystem.Data;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public InventoryController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _context.InventoryItems.ToListAsync();
            return Ok(items);
        }

        // GET: api/inventory/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] InventoryItem item, IFormFile image)
        {
            if (image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                item.ImageUrl = $"/uploads/{uniqueFileName}";
            }

            _context.InventoryItems.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
        }

        // PUT: api/inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromForm] InventoryItem item, IFormFile image)
        {
            if (id != item.Id) return BadRequest();

            var existingItem = await _context.InventoryItems.FindAsync(id);
            if (existingItem == null) return NotFound();

            if (image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                existingItem.ImageUrl = $"/uploads/{uniqueFileName}";
            }

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Quantity = item.Quantity;
            existingItem.Price = item.Price;

            _context.Entry(existingItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null) return NotFound();

            if (!string.IsNullOrEmpty(item.ImageUrl))
            {
                var filePath = Path.Combine(_env.WebRootPath, item.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // SEARCH: api/inventory/search?name=xxx
        [HttpGet("search")]
        public async Task<IActionResult> Search(string name)
        {
            var items = await _context.InventoryItems
                .Where(i => i.Name.Contains(name))
                .ToListAsync();
            return Ok(items);
        }
    }
}
