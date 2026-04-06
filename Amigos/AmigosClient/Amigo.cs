using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmigosClient
{
    public class Amigo
    {
        public int ID { get; set; }

        [Display(Name = "Nombre")]
        public string? name { get; set; }

        [Display(Name = "Longitud")]
        public string? longi { get; set; }

        [Display(Name = "Latitud")]
        public string? lati { get; set; }
    }
}
