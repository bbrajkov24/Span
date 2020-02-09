using System;
using System.Collections.Generic;

namespace Span.Models
{
    public class PodaciRepository : IPodaciRepository
    {
        public IEnumerable<Podaci> GetAllPodaci =>
            new List<Podaci>
            {
                new Podaci{ PodaciID=1, Ime = "Ime1", Prezime = "Prezime1", PBr = "10000", Grad = "Zagreb", Telefon = "12345678"},
                new Podaci{ PodaciID=2, Ime = "Ime2", Prezime = "Prezime2", PBr = "21000", Grad = "Split", Telefon = "56789"}
            };

        public string WriteAllPodaci(IEnumerable<Podaci> podaci)
        {
            throw new NotImplementedException();
        }
    }
}
