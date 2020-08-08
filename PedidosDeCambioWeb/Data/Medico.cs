using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosDeCambioWeb.Models
{
    public class Medico
    {
        [Key]
        [ForeignKey("Persona")]
        public Guid Id { get; set; }
        public virtual Persona Persona { get; set; }

        public int Matricula { get; set; }
    }
}
