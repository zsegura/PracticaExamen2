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

        // Load the Insert/Edit Estadio form
        public ActionResult RegistrarEstadio()
        {
            using (var db = new EstadiosDB("MyDatabase"))
            {
                ViewBag.Paises = new SelectList(db.SpRetornaPais().ToList(), "Id_Pais", "DatosPais");
                ViewBag.Ciudades = new SelectList(Enumerable.Empty<SelectListItem>(), "Id_Ciudad", "DatosCiudad");
            }
            return View();
        }


        // Insert or notify that updates are not supported
        [HttpPost]
        public JsonResult RegistrarEstadio(ModelEstadio estadio)
        {
            string resultado;
            try
            {
                if (estadio == null || string.IsNullOrEmpty(estadio.Nombre) || estadio.Capacidad <= 0 || estadio.IdCiudad <= 0)
                {
                    return Json("Datos incompletos o inválidos.", JsonRequestBehavior.AllowGet);
                }

                using (var db = new EstadiosDB("MyDatabase"))
                {
                    db.SpInsertaEstadioCiudad(estadio.IdCiudad, estadio.Nombre, estadio.Capacidad);
                    resultado = "El estadio ha sido registrado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                resultado = $"Error al registrar el estadio: {ex.Message}";
            }
            return Json(resultado, JsonRequestBehavior.AllowGet);
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
            string resultado;
            try
            {
                using (var db = new EstadiosDB("MyDatabase"))
                {
                    db.SpInsertaEstadioCiudad(estadio.IdCiudad, estadio.Nombre, estadio.Capacidad);
                    resultado = "El estadio se ha insertado correctamente.";
                }
            }
            catch (Exception ex)
            {
                resultado = $"Error al insertar el estadio: {ex.Message}";
            }
            return Json(resultado);
        }
    }
}
