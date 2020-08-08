using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PedidosDeCambioWeb.Models
{
    public class Accion
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }

        public virtual ICollection<Pedido> Pedidos { get; set; }
    }
}
