using Microsoft.EntityFrameworkCore;

namespace AspireSampleWeb.Crm;

public class CrmContext : DbContext
{
    public DbSet<Deal> Deals { get; set; }
    
    public CrmContext(DbContextOptions options) : base(options)
    {
    }
    
    
}

public class Deal
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public double Price { get; set; }
}