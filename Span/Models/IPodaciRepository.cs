using System.Collections.Generic;

namespace Span.Models
{
    public interface IPodaciRepository
    {
        IEnumerable<Podaci> GetAllPodaci { get; }

        string WriteAllPodaci(IEnumerable<Podaci> podaci);
    }
}
