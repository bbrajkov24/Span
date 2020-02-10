using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Span.Models
{
    public class PodaciRepository : IPodaciRepository
    {
        private readonly IConfiguration _configuration;

        public PodaciRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Podaci> GetAllFromCSV()
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

        public async Task<List<Podaci>> GetAllFromSQL()
        {
            using (SqlConnection sql = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("span.sp_GetPodaci", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    var response = new List<Podaci>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            response.Add(MapToValue(reader));
                        }
                    }

                    return response;
                }
            }
        }

        public async Task<string> WriteAll()
        {
            var podaciCSV = GetAllFromCSV();

            List<Podaci> podaci = new List<Podaci>();

            DataTable csvData = new DataTable();
            csvData.Columns.Add("Ime", typeof(string));
            csvData.Columns.Add("Prezime", typeof(string));
            csvData.Columns.Add("PBr", typeof(string));
            csvData.Columns.Add("Grad", typeof(string));
            csvData.Columns.Add("Telefon", typeof(string));

            podaciCSV.ToList().ForEach(p => AddRow(csvData, p));

            using (SqlConnection sql = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("span.sp_InsertPodaci", sql))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter tvpeParam = cmd.Parameters.AddWithValue("@InsertPodaci", csvData);
                    tvpeParam.SqlDbType = SqlDbType.Structured;
                    tvpeParam.TypeName = "span.PodaciDTO";
                    cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar) { Direction = ParameterDirection.Output, Value = string.Empty, Size = 2000 });                    
                    await sql.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    string output = (string)cmd.Parameters["@ErrorMessage"].Value;
                    return output;
                }
            }
        }

        private void AddRow(DataTable dataTable, Podaci data)
        {
            var row = dataTable.NewRow();
            row["Ime"] = data.Ime;
            row["Prezime"] = data.Prezime;
            row["PBr"] = data.PBr;
            row["Grad"] = data.Grad;
            row["Telefon"] = data.Telefon;

            dataTable.Rows.Add(row);
        }

        private Podaci MapToValue(SqlDataReader reader)
        {
            return new Podaci()
            {
                Ime = reader["Ime"].ToString(),
                Prezime = reader["Prezime"].ToString(),
                PBr = reader["PBr"].ToString(),
                Grad = reader["Grad"].ToString(),
                Telefon = reader["Telefon"].ToString(),
            };
        }
    }
}
