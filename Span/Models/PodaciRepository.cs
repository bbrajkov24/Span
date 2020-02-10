using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Span.Models
{
    public class PodaciRepository : IPodaciRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string VALIDATION_LENGTH_LESS_THAN_50;
        private readonly string VALIDATION_LENGTH_LESS_THAN_20;
        private readonly string VALIDATION_FIELD_EMPTY;
        private readonly string VALIDATION_NUMBER;


        public PodaciRepository(IConfiguration configuration)
        {
            _configuration = configuration;

            VALIDATION_LENGTH_LESS_THAN_50 = "Length of {0} must be less than 50 characters. ";
            VALIDATION_LENGTH_LESS_THAN_20 = "Length of {0} must be less than 20 characters. ";
            VALIDATION_FIELD_EMPTY = "{0} can't be empty. ";
            VALIDATION_NUMBER = "{0} can only contain digits. ";
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

                    string message = ValidateRow(strRow);
                    
                    podaci.Add(MapRow(strRow, message));                  
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
            var podaciCSV = GetAllFromCSV().Where(p => p.IsValid);

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

        #region Private Methods

        private Podaci MapRow(string[] strRow, string message)
        {
            return new Podaci
            {
                Ime = strRow[0],
                Prezime = strRow[1],
                PBr = strRow[2],
                Grad = strRow[3],
                Telefon = strRow[4],
                IsValid = String.IsNullOrWhiteSpace(message),
                ValidationMessage = message
            };
        }

        private string ValidateRow(string[] strRow)
        {
            StringBuilder message = new StringBuilder();

            if (String.IsNullOrWhiteSpace(strRow[0]))
                message.Append(String.Format(VALIDATION_FIELD_EMPTY, "Ime"));
            if (strRow[0].Length > 50)
                message.Append(String.Format(VALIDATION_LENGTH_LESS_THAN_50, "Ime"));

            if (String.IsNullOrWhiteSpace(strRow[1]))
                message.Append(String.Format(VALIDATION_FIELD_EMPTY, "Prezime"));
            if (strRow[1].Length > 50)
                message.Append(String.Format(VALIDATION_LENGTH_LESS_THAN_50, "Prezime"));

            if (String.IsNullOrWhiteSpace(strRow[2]))
                message.Append(String.Format(VALIDATION_FIELD_EMPTY, "Poštanski Broj"));
            if (strRow[2].Length > 20)
                message.Append(String.Format(VALIDATION_LENGTH_LESS_THAN_20, "Poštanski Broj"));
            if (!IsConvertableToInt(strRow[2]))
                message.Append(String.Format(VALIDATION_NUMBER, "Poštanski Broj"));

            if (String.IsNullOrWhiteSpace(strRow[3]))
                message.Append(String.Format(VALIDATION_FIELD_EMPTY, "Grad"));
            if (strRow[3].Length > 50)
                message.Append(String.Format(VALIDATION_LENGTH_LESS_THAN_50, "Grad"));

            if (String.IsNullOrWhiteSpace(strRow[4]))
                message.Append(String.Format(VALIDATION_FIELD_EMPTY, "Telefon"));
            if (strRow[4].Length > 20)
                message.Append(String.Format(VALIDATION_LENGTH_LESS_THAN_20, "Telefon"));

            return message.ToString().TrimEnd();
        }

        private bool IsConvertableToInt(string data)
        {
            int num;
            return int.TryParse(data, out num);
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

        #endregion Private Methods
    }
}
