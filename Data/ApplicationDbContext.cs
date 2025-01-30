using Microsoft.EntityFrameworkCore;
namespace InventorySystem.Data{
    public class ApplicationDbContext:DbContext{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options){}
        public DbSet<InventoryItem> InventoryItems {get; set;}
    }
}
