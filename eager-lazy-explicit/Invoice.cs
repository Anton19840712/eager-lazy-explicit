namespace eager_lazy_explicit;

public class Invoice
{
	public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string CustomerName { get; set; }
    
    public ICollection<InvoiceLine> InvoiceLines { get; set; }
}