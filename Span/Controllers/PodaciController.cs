using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Span.Models;

namespace Span.Controllers
{
    public class PodaciController : Controller
    {
        private readonly IPodaciRepository _podaciRepository;

        public PodaciController(IPodaciRepository podaciRepository)
        {
            _podaciRepository = podaciRepository;
        }

        public ViewResult List()
        {
            return View(_podaciRepository.GetAllPodaci);
        }
    }
}
