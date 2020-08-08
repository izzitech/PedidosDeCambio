using Microsoft.AspNetCore.Identity;
using PedidosDeCambioWeb.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosDeCambioWeb.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        
        [Display(Name="Responsable")]
        public Guid ResponsableId { get; set; }
        
        [ForeignKey("Causante")]
        public Guid CausanteId { get; set; }
        public virtual Persona Causante { get; set; }
        
        [ForeignKey("Accion")]
        public int AccionId { get; set; }
        public virtual Accion Accion { get; set; }

        public long Protocolo { get; set; }
        
        [StringLength(280)]
        public string Motivo { get; set; }

        [StringLength(280)]
        public string Razon { get; set; }

        [StringLength(5000)]
        public string Detalles { get; set; }
    }
}
