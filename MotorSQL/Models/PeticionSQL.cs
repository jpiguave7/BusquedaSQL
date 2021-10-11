using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MotorSQL.Models
{
    public class PeticionSQL
    {
        /*
        [Required(ErrorMessage = "La base de datos es obligatoria.")]
        [StringLength(20,ErrorMessage ="El {0} debe ser almenos {2} y maximo {1} caracteres.", MinimumLength = 3)]
        [Display(Name ="Base de Datos")]
        public string BaseDatos  { get; set; }
        
        [Required(ErrorMessage = "El tipo de objeto es obligatorio.")]
        [StringLength(2, ErrorMessage = "El {0} debe ser almenos {2} y maximo {1} caracteres.", MinimumLength = 1)]
        [Display(Name = "Tipo de Objeto")]
        public string TipoObjeto { get; set; }

        [Required(ErrorMessage = "El objeto es obligatorio.")]
        [StringLength(50, ErrorMessage = "El {0} debe ser almenos {2} y maximo {1} caracteres.", MinimumLength = 3)]
        [Display(Name = "Objeto")]
        public string Objeto { get; set; }
        */
        [Required(ErrorMessage = "El script es obligatorio.")]
        [StringLength(2000, ErrorMessage = "El {0} debe ser almenos {2} y maximo {1} caracteres.", MinimumLength = 3)]
        [Display(Name = "Script")]
        public string Script { get; set; }

        [Required(ErrorMessage = "La fecha de ejecucón es obligatoria.")]
        [Display(Name = "Fecha de Ejecución")]
        public DateTime FechaEjecucion { get; set; }

        public string TipoReporte { get; set; }
        public MensajeRespuesta Respuesta { get; set; }

    }
    

    public class MensajeRespuesta
    {
        public int Codigo { get; set; }
        public string Mensaje { get; set; }
        public int CantidadTablas { get; set; }
        public int CantidadFilas { get; set; }
        public List<string> NombresColumnas { get; set; }
        public DataTable Dato { get; set; }
    }

    public class CatalogoSQL
    {

    }
}
