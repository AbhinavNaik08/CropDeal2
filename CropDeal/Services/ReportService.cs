using System.Formats.Asn1;
using System.Text;
using CropDeal.Interfaces;


namespace CropDeal.Services
{
    public class ReportService : IReportService
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public ReportService(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<Stream> GetAllInvoice()
        {
            var res= await _invoiceRepository.GetAllInvoiceAsync();

            var stream=new MemoryStream();

            using (var writer = new StreamWriter(stream,Encoding.UTF8,leaveOpen:true))
            {
                writer.WriteLine("CropId,DealerId,Quantity,Amount");

                foreach(var item in res)
                {
                    writer.WriteLine($"{item.Transaction.CropId},{item.Transaction.DealerId},{item.Transaction.Quantity},{item.Transaction.Amount}");
                }
                writer.Flush();
            }

            stream.Position=0;
            return stream;
            
        }
    }
}