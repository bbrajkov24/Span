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

        public async Task<ViewResult> List()
        {
            var errorMessage = await _podaciRepository.WriteAll();

            var podaci = await _podaciRepository.GetAllFromSQL();

            return View(podaci);
        }
    }
}
