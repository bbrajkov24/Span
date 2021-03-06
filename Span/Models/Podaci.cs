﻿using System.ComponentModel.DataAnnotations;

namespace Span.Models
{
    public class Podaci
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string PBr { get; set; }
        public string Grad { get; set; }
        public string Telefon { get; set; }
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }
}
