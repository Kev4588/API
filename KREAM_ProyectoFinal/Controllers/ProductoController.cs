using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models.ViewModel;
using KREAM_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KREAM_ProyectoFinal.Controllers
{
    [VerificarSesion]
    public class ProductoController : Controller
    {
        // GET: Producto
        public ActionResult Index()
        { 
            return View();
        }

        [HttpGet]
        public ActionResult MostrarInformacion(int Id)
        {
            ProductosViewModel model = new ProductosViewModel();
            using (var db = new TRAVEL2Entities())
            {
                var productTO = db.Productos.Find(Id);

                model.Nombre = productTO.Nombre; 

                model.ProductoID = productTO.ProductoID;


                model.Lugar = productTO.Lugar;
                model.Descripcion = productTO.Descripcion;
                model.Precio = (decimal)productTO.Precio;
                model.Categoria = productTO.Categoria;
                model.Personas = productTO.Personas;
                model.CantidadEnStock = productTO.CantidadEnStock;
                model.Comentario = productTO.Comentario;
                model.Calificacion = (int)(productTO.Calificacion ?? 0);
                model.Proveedor = productTO.Proveedor;
                model.Vendedor = productTO.Vendedor;
                model.Imagen = productTO.Imagen;
                model.Imagen2 = productTO.Imagen2;
                model.Imagen3 = productTO.Imagen3;
                ViewBag.Nombre = productTO.Nombre;

                
                var comments = db.Comentarios.Where(c => c.IdProducto == Id).ToList();
                model.Comentarios = comments;

            }
            return View(model);
        }

        [HttpPost]      
        public ActionResult MostrarInformacion(int idProducto, string comentario)
        {
            using (var db = new TRAVEL2Entities())
            {
                var usuario = Session["UsuarioActual"] as UsersTableViewModel;

                db.Database.ExecuteSqlCommand(
                    "EXEC GuardarComentarios @IdProducto, @Comentario",                    
                    new SqlParameter("@IdProducto", idProducto),
                    new SqlParameter("@Comentario", comentario)
                );

            }
            return RedirectToAction("MostrarInformacion", new { Id = idProducto });

        }





        
        public ActionResult MostrarInformacionHoteles(int Id)
        {
            HotelesViewModel model = new HotelesViewModel();
            using (var db = new TRAVEL2Entities())
            {
                var hotelTO = db.Hoteles.Find(Id);

                model.IDHotel = hotelTO.IDHotel;
                model.TipoDeHabitacion = hotelTO.TipoDeHabitacion;
                model.CantidadDePersonas = hotelTO.CantidadDePersonas;
                model.Descripcion = hotelTO.Descripcion;
                model.Precio = (decimal)hotelTO.Precio;
                model.NombreHotel = hotelTO.NombreHotel;
                model.Imagen = hotelTO.Imagen;
                model.Imagen2 = hotelTO.Imagen2;
                model.Imagen3 = hotelTO.Imagen3;
                model.web = hotelTO.web;
                ViewBag.Nombre = hotelTO.NombreHotel;

            }
            return View(model);
        }



    }
}


