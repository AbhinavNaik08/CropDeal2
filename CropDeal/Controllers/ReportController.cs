using Microsoft.AspNetCore.Mvc;
using CropDeal.Interfaces;
using CropDeal.Models;
using CropDeal.DTOs.Subscription;
using Microsoft.AspNetCore.Authorization;
using System.Data.Common;

namespace CropDeal.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("DownloadInvoice")]
        [Authorize(Roles="Admin")]
        
        public async Task<IActionResult> DownloadInvoice()
        {
            var res= await _reportService.GetAllInvoice();

            return File(res, "text/csv", "Invoices.csv");
        }
    }
}