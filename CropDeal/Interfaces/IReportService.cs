namespace CropDeal.Interfaces
{
    public interface IReportService
    {
        Task<Stream> GetAllInvoice();
        
    }
}