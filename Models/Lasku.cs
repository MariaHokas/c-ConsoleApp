using System;
using System.Collections.Generic;

namespace ConsoleAppCSI.Models
{
    public partial class Lasku
    {
        public int IdLasku { get; set; }
        public int? LaskuNro { get; set; }
        public string Selite { get; set; }
        public decimal? Summa { get; set; }
        public int? AsiakasNro { get; set; }
    }
}
