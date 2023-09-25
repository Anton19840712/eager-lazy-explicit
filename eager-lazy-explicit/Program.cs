using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace eager_lazy_explicit;

class Program
{
	static void Main()
	{
		using (var context = new ApplicationDbContext())
		{
			var invoice1 = new Invoice { InvoiceNumber = "INV001", InvoiceDate = DateTime.Now, CustomerName = "Client1" };
			var invoice2 = new Invoice { InvoiceNumber = "INV002", InvoiceDate = DateTime.Now, CustomerName = "Client2" };

			var invoiceLine1 = new InvoiceLine { ProductName = "product1", UnitPrice = 10.0m, Quantity = 2, Invoice = invoice1 };
			var invoiceLine2 = new InvoiceLine { ProductName = "product2", UnitPrice = 5.0m, Quantity = 3, Invoice = invoice1 };
			var invoiceLine3 = new InvoiceLine { ProductName = "product3", UnitPrice = 8.0m, Quantity = 1, Invoice = invoice2 };

			context.Invoices.Add(invoice1);
			context.Invoices.Add(invoice2);
			context.InvoiceLines.AddRange(invoiceLine1, invoiceLine2, invoiceLine3);

			context.SaveChanges();
		}

		using (var context = new ApplicationDbContext())
		{
			// Eager Loading: Include, one query, observability.
			// INCLUDE
			// Load all the relevant data from included branches.
			// As a developer, I like how clearly this communicates what’s going on in code.
			var invoicesWithLines = context.Invoices
				.Include(i => i.InvoiceLines)
				//ThenInclude Product, etc.
				.ToList();

			Console.WriteLine("Eager Loading:");
			foreach (var invoice in invoicesWithLines)
			{
				Console.WriteLine($"Invoice №{invoice.InvoiceNumber} for client {invoice.CustomerName}");
				foreach (var line in invoice.InvoiceLines)
				{
					Console.WriteLine($"Product: {line.ProductName}, Price: {line.UnitPrice}, Quantity: {line.Quantity}");
				}
			}

			// Lazy Loading: N+1 issue, no include, pipeline of a separate queries, several queries.
			// With lazy loading, navigational properties are queried AUTOMATICALLY WHEN NEEDED
			// No include
			// Lazy loading does come at a cost: there’s a big risk that for each Invoice entity you process(N),
			// another query is run on the database to fetch the collection of InvoiceLine for that entity (+1).
			// As a result, the database may have to process N + 1 queries instead of just one query in the eager loading example we saw earlier.

			var invoice1Lazy = context.Invoices.Single(i => i.InvoiceNumber == "INV001");
			Console.WriteLine("\nLazy Loading:");
			foreach (var line in invoice1Lazy.InvoiceLines)
			{
				// ...the related invoice lines ARE ALSO queried WHEN ACCESSED
				Console.WriteLine($"Product: {line.ProductName}, Price: {line.UnitPrice}, Quantity: {line.Quantity}");
			}

			// Explicit Loading: differed, just ENTRY and LOAD words is used to sign this process explicitly for developer eyes.
			// Other words, it becomes very clear where navigational data is queried in code.
			// Related data is also queried, when it is needed. The mechanism of querying is the same as lazy, but here you write this explicitly.
			// ENTRY, LOAD
			var invoice2Explicit = context.Invoices.Single(i => i.InvoiceNumber == "INV002");
			context.Entry(invoice2Explicit).Collection(i => i.InvoiceLines).Load();
			Console.WriteLine("\nExplicit Loading:");
			foreach (var line in invoice2Explicit.InvoiceLines)
			{
				Console.WriteLine($"Product:  {line.ProductName} , Price:  {line.UnitPrice} , Quantity: {line.Quantity}");
			}
		}

		Console.ReadLine();
	}
}