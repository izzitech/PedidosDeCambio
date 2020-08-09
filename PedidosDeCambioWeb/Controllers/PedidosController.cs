using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PedidosDeCambio;
using PedidosDeCambioWeb.Data;
using PedidosDeCambioWeb.Models;

namespace PedidosDeCambioWeb.Controllers
{
    public class PedidosController : Controller
    {
        private readonly PedidosContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;

        private readonly int DefaultPageSize = 10;

        public PedidosController(PedidosContext context, UserManager<IdentityUser> userManager, ILogger<PedidosController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

    /*
        // GET: Pedidos
        public async Task<IActionResult> Index()
        {
            var pedidosContext = 
                _context.Pedidos
                .Include(p => p.Accion)
                .Include(p => p.Causante)
                .OrderByDescending(p => p.Fecha);
            return View(await pedidosContext.ToListAsync());
        }
    */

        [HttpGet]
        public async Task<IActionResult> Index(int pagina, int? mostradosEnPagina){
            if(pagina < 1) pagina = 1;
            int _mostradosEnPagina = mostradosEnPagina??DefaultPageSize;
            int pedidosTotal =  _context.Pedidos.Count();

            ViewBag.PedidosTotal = pedidosTotal;
            ViewBag.PaginasTotal = decimal.Round((decimal)pedidosTotal / (decimal)_mostradosEnPagina, 0);
            ViewBag.PaginaActual = pagina;
            

            var pedidosContext = 
                _context.Pedidos
                .Include(p => p.Accion)
                .Include(p => p.Causante)
                .OrderByDescending(p => p.Fecha).Skip((pagina - 1) * _mostradosEnPagina).Take(_mostradosEnPagina);
            return View(await pedidosContext.ToListAsync());
        }


        // GET: Pedidos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Accion)
                .Include(p => p.Causante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // GET: Pedidos/Create
        [Authorize]
        public IActionResult Create()
        {
            LoadViewDataForPedidoCreate();
            return View();
        }

        private void LoadViewDataForPedidoCreate()
        {
            ViewData["Fecha"] = DateTime.Now;
            LoadViewDataForPedido();
        }

        private void LoadViewDataForPedidoEdit()
        {
            LoadViewDataForPedido();
        }

        private void LoadViewDataForPedido()
        {
            ViewData["Acciones"] = _context.Acciones.OrderBy(a => a.Name);
            ViewData["Causantes"] = _context.Personas.OrderBy(p => p.Nombre);
            ViewData["ResponsableNombre"] = User.Identity.Name;
        }

        [Authorize]
        public async Task<IActionResult> Importar()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarData([Bind("content")]string content)
        {
            Importar importar = new Importar(_context, _logger);
            importar.Import(_userManager.GetUserId(User), content);

            string stringApprovedStrings = "";
            foreach (var cadena in importar.approvedStrings)
            {
                stringApprovedStrings += $"{cadena}\n";
            }
            ViewBag.approvedStrings = stringApprovedStrings;
            
            ViewBag.NotApprovedStringsWithErrorMessages = importar.notApprovedStringsWithErrorMessages;
            string notApprovedWhatever = "";
            foreach(var cadena in importar.notApprovedStrings)
            {
                notApprovedWhatever += $"{cadena}\n";
            }
            
            ViewBag.NotApprovedStrings = notApprovedWhatever;
            

            return View(importar.pedidos);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ImportarGuardar([Bind("content")]string content)
        {
            Importar importar = new Importar(_context, _logger);
            importar.Import(_userManager.GetUserId(User), content);

            try
            {
                _context.Pedidos.AddRange(importar.pedidos);
                _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"No se guardó la importación porque: {ex.Message}");
            }

            return RedirectToAction(nameof(PedidosController), "Index");
        }

        // POST: Pedidos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Fecha,ResponsableId,CausanteId,AccionId,Protocolo,Motivo,Razon,Detalles")] Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                pedido.ResponsableId = new Guid(_userManager.GetUserId(User));
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            LoadViewDataForPedidoCreate();
            return View(pedido);
        }

        // GET: Pedidos/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            LoadViewDataForPedido();
            return View(pedido);
        }

        // POST: Pedidos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,ResponsableId,CausanteId,AccionId,Protocolo,Motivo,Razon,Detalles")] Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pedido.ResponsableId = new Guid(_userManager.GetUserId(User));
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            LoadViewDataForPedido();
            return View(pedido);
        }

        // GET: Pedidos/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedidos
                .Include(p => p.Accion)
                .Include(p => p.Causante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }

            return View(pedido);
        }

        // POST: Pedidos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedidos.Any(e => e.Id == id);
        }
    }
}
