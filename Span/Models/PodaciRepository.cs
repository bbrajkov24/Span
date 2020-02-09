using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Span.Models
{
    public class PodaciRepository : IPodaciRepository
    {
        private IConfiguration _configuration;

        public PodaciRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Podaci> GetAllPodaci()
        {       
            List<Podaci> podaci = new List<Podaci>();

            string path = _configuration["CSVConfig:Path"];

            using (StreamReader sr = new StreamReader(@path))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] strRow = line.Split(';');

                    podaci.Add(new Podaci
                    {
                        Ime = strRow[0],
                        Prezime = strRow[1],
                        PBr = strRow[2],
                        Grad = strRow[3],
                        Telefon = strRow[4]
                    });
                }
            }
            return podaci;
        }

        public string WriteAllPodaci(IEnumerable<Podaci> podaci)
        {
            throw new NotImplementedException();
        }
    }
}
