using System.Collections.Generic;

namespace Span.Models
{
    public interface IPodaciRepository
    {
        public IEnumerable<Podaci> GetAllPodaci();
        public string WriteAllPodaci(IEnumerable<Podaci> podaci);
    }
}
