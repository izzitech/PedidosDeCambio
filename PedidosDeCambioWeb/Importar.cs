using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PedidosDeCambioWeb.Data;
using PedidosDeCambioWeb.Models;
using DuoVia.FuzzyStrings;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace PedidosDeCambio
{
    public class Importar
    {
        private readonly PedidosContext _context;
        private readonly ILogger _logger;
        public List<Pedido> pedidos;
        public List<string> approvedStrings;
        public List<string> notApprovedStrings;
        public List<string> notApprovedStringsWithErrorMessages;
        public Importar(PedidosContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Import(string userID, string data)
        {
            pedidos = new List<Pedido>();
            approvedStrings = new List<string>();
            notApprovedStrings = new List<string>();
            notApprovedStringsWithErrorMessages = new List<string>();

            foreach (var row in data.Split("\n"))
            {
                string[] field = row.Split(";");

                try
                {
                    DateTime date;
                    var dateSucceed = DateTime.TryParse(field[0], out date);
                    if (!dateSucceed) throw new NullReferenceException("La fecha no fue comprendida.");

                    Pedido pedido = new Pedido()
                    {
                        Fecha = date,
                        Causante = ObtenerPersona(field[1]),
                        Accion = ObtenerAccion(field[2]),
                        ResponsableId = new Guid(userID),
                        Protocolo = ObtenerProtocolo(field[3])
                    };

                    
                    pedidos.Add(pedido);
                    approvedStrings.Add(row);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error en cadena: \"{row}\", {ex.Message}");   
                    notApprovedStrings.Add(row);
                    notApprovedStringsWithErrorMessages.Add($"Error en cadena: \"{row}\", {ex.Message}");
                }

            }
        }

        private long ObtenerProtocolo(string input)
        {
            long protocolo = 0;
            if (long.TryParse(input, out protocolo))
            {
                return protocolo;
            }
            else
            {
                throw new NullReferenceException("No se pudo obtener el protocolo.");
            }
        }

        private string ObtenerMejorCadena(string[] array, string input)
        {
            // Usando Levenshtein
            var distance = 5;
            int tempDistance = 0;
            string bestString = "";
            foreach (var item in array)
            {
                tempDistance = item.LevenshteinDistance(input);
                if (tempDistance < 3 && tempDistance < distance)
                {
                    distance = tempDistance;
                    bestString = item;
                }
            }

            return bestString;
        }

        private Accion ObtenerAccion(string nombre)
        {
            // Implementar Levenshtein distance https://www.wikiwand.com/en/Levenshtein_distance
            // http://www.tsjensen.com/blog/post/2011/05/27/Four+Functions+For+Finding+Fuzzy+String+Matches+In+C+Extensions

            string[] accionesNombre = _context.Acciones.Select(x => x.Name).ToArray();
            Accion accion = null;

            var mejorCadena = ObtenerMejorCadena(accionesNombre, nombre);           
            if (!string.IsNullOrEmpty(mejorCadena))
            {
                accion = _context.Acciones.FirstOrDefault(a => a.Name == mejorCadena);
            }

            if (accion != null)
                return accion;
            else
                throw new NullReferenceException("La acción no fue encontrada.");
        }

        private Persona ObtenerPersona(string nombre)
        {
            int temp;
            if (int.TryParse(nombre, out temp))
            {
                try
                {
                    var persona = _context.Medicos.Include(m => m.Persona).FirstOrDefault(x => x.Matricula == temp).Persona;
                    return persona;
                }
                catch(Exception ex)
                {
                    throw new NullReferenceException($"No se encontró al médico con la matrícula {temp}.");
                }
            }
            else
            {
                var personasNombres = _context.Personas.Select(p => p.Nombre).ToList();
                var nombresSueltos = new List<string>();
                foreach(var estosNombres in personasNombres)
                {
                    var nombreSplit = estosNombres.Split(" ");
                    foreach(var esteNombre in nombreSplit)
                    {
                        nombresSueltos.Add(esteNombre);
                    }
                }

                Persona persona = null;

                var mejorCadena = ObtenerMejorCadena(nombresSueltos.ToArray(), nombre);
                if (!string.IsNullOrEmpty(mejorCadena))
                {
                    persona = _context.Personas.FirstOrDefault(p => p.Nombre.Contains(mejorCadena));
                }

                if (persona != null)
                    return persona;
                else
                {
                    throw new NullReferenceException("No se encontró el nombre del causante.");
                }
            }
        }
    }
}
