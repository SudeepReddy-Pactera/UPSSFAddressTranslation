using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPS.DataObjects.WR_FLW;
using UPS.ServicesAsyncActions;
using UPS.ServicesDataRepository;
using UPS.ServicesDataRepository.DataContext;
using UPS.DataObjects.Reports;

namespace AtService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {
        private IHostingEnvironment _hostingEnvironment;

        private readonly ApplicationDbContext _context;
        private IAddressBookService _addressBookService;
        private IEntityValidationService _entityValidationService;
        public ReportsController(IHostingEnvironment hostingEnvironment, ApplicationDbContext applicationDbContext,
           IAddressBookService addressBookService,
           IEntityValidationService entityValidationService)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = applicationDbContext;
            _addressBookService = addressBookService;
            _entityValidationService = entityValidationService;
        }


        [HttpGet("[action]")]
        public List<SummaryData> getSummaryData(DateTime fromDate , DateTime toDate)
        {
            WorkflowService workflowService = new WorkflowService(_context, _addressBookService, _entityValidationService);
            return workflowService.getSummaryData(fromDate,toDate);
        }
    }
}