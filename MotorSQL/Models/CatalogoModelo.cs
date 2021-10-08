using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MotorSQL.Models
{
    public class CatalogoModelo
    {
        public int id { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
    }
}
