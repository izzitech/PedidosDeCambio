using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PedidosDeCambioWeb.Models;
using System;

namespace PedidosDeCambioWeb.Data
{
    public class PedidosContext : IdentityDbContext
    {
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Persona> Personas { get; set; }
        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Accion> Acciones { get; set; }

        public PedidosContext(DbContextOptions<PedidosContext> dbContextOptions) 
            : base(dbContextOptions)
        {

        }
    }
}
