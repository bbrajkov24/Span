using System.Collections.Generic;
using System.Threading.Tasks;

namespace Span.Models
{
    public interface IPodaciRepository
    {
        public IEnumerable<Podaci> GetAllFromCSV();
        public Task<List<Podaci>> GetAllFromSQL();
        public Task<string> WriteAll();
    }
}
