namespace eager_lazy_explicit;

public class InvoiceLine
{
	public int InvoiceLineId { get; set; }
	public string ProductName { get; set; }
	public decimal UnitPrice { get; set; }
	public int Quantity { get; set; }
	
	public int InvoiceId { get; set; }
	
	public Invoice Invoice { get; set; }
}