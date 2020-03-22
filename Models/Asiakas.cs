using System;
using System.Collections.Generic;

namespace ConsoleAppCSI.Models
{
    public partial class Asiakas
    {
        public int AsiakasNro { get; set; }
        public string Etunimi { get; set; }
        public string Sukunimi { get; set; }
        public string Osoite { get; set; }
        public int? Postinumero { get; set; }
        public string Postitoimipaikka { get; set; }
    }
}
