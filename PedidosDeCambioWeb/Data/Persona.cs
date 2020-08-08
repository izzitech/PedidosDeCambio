using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosDeCambioWeb.Models
{
    public class Persona
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
