using Microsoft.EntityFrameworkCore;

namespace eager_lazy_explicit;

public class ApplicationDbContext : DbContext
{
	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<InvoiceLine> InvoiceLines { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseInMemoryDatabase("InMemoryDB");
	}
}