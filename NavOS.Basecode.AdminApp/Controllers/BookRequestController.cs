using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NavOS.Basecode.AdminApp.Mvc;
using NavOS.Basecode.Services.Interfaces;

namespace NavOS.Basecode.AdminApp.Controllers
{
    public class BookRequestsController : ControllerBase<BookRequestsController>
    {
        private readonly IBookRequestService _bookRequestService;

        public BookRequestsController(IBookService bookService,
                              IBookRequestService bookRequestService,
                              IHttpContextAccessor httpContextAccessor,
                              ILoggerFactory loggerFactory,
                              IConfiguration configuration,
                              IMapper mapper = null) : base(httpContextAccessor, loggerFactory, configuration, mapper)
        {
            _bookRequestService = bookRequestService;

        }
        [HttpGet]
        public IActionResult RecievedRequest()
        {
            var data = _bookRequestService.GetBooksRequest();
            return View(data);
        }
    }
}
