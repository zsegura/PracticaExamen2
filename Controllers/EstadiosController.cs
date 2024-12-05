using DataModels;
using PracticaExamen2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static DataModels.EstadiosDBStoredProcedures;

namespace PracticaExamen2.Controllers
{
    public class EstadiosController : Controller
    {
        // Main view that lists all Estadios
        public ActionResult ListaEstadios()
        {
            return View();
        }

        // Return all Estadios as JSON
        public JsonResult Estadios()
        {
            var estadios = new List<SpRetornaEstadioCiudadPAISResult>();
            using (var db = new EstadiosDB("MyDatabase"))
            {
                estadios = db.SpRetornaEstadioCiudadPais().ToList();
            }
            return Json(new { data = estadios }, JsonRequestBehavior.AllowGet);
        }

        // Fetch all default cities for dropdown
        public JsonResult DefaultCiudades()
        {
            var ciudades = new List<DropDown>();
            using (var db = new EstadiosDB("MyDatabase"))
            {
                ciudades = db.SpRetornaCiudadPorPais(1).Select(c => new DropDown
                {
                    Id = c.Id_Ciudad,
                    Nombre = c.DatosCiudad
                }).ToList();
            }
            return Json(ciudades, JsonRequestBehavior.AllowGet);
        }

        // Fetch all countries for dropdown
        public JsonResult RetornaPaises()
        {
            var paises = new List<DropDown>();
            using (var db = new EstadiosDB("MyDatabase"))
            {
                paises = db.SpRetornaPais().Select(p => new DropDown
                {
                    Id = p.Id_Pais,
                    Nombre = p.DatosPais
                }).ToList();
            }
            return Json(paises, JsonRequestBehavior.AllowGet);
        }

        // Fetch cities based on the selected country
        public JsonResult Ciudades(int idPais)
        {
            try
            {
                using (var db = new EstadiosDB("MyDatabase"))
                {
                    var ciudades = db.SpRetornaCiudadPorPais(idPais)
                                     .Select(c => new DropDown
                                     {
                                         Id = c.Id_Ciudad,
                                         Nombre = c.DatosCiudad
                                     }).ToList();
                    return Json(ciudades, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Insert a new Estadio
        [HttpPost]
        public JsonResult InsertaEstadio(ModelEstadio estadio)
        {
            try
            {
                // Validate input
                if (estadio == null || string.IsNullOrEmpty(estadio.Nombre) || estadio.Capacidad <= 0 || estadio.IdCiudad <= 0)
                {
                    return Json("Datos incompletos o inválidos. Verifique e intente nuevamente.");
                }

                // Save data using the database context
                using (var db = new EstadiosDB("MyDatabase"))
                {
                    db.SpInsertaEstadioCiudad(estadio.IdCiudad, estadio.Nombre, estadio.Capacidad);
                }

                return Json("El estadio se ha registrado exitosamente.");
            }
            catch (Exception ex)
            {
                return Json($"Error al registrar el estadio: {ex.Message}");
            }
        }

    }
}
