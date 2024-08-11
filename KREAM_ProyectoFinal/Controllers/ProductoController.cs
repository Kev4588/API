using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;
using PURIS_FLASH.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KREAM_ProyectoFinal.Models;


namespace PURIS_FLASH.Controllers
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
                model.Comentario = productTO.Comentarios;
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

                int v = db.Database.ExecuteSqlCommand(
                    "EXEC GuardarComentario @IdProducto, @Comentario",                    
                    new SqlParameter("@IdProducto", idProducto),
                    new SqlParameter("@Comentario", comentario)
                );

            }
            return RedirectToAction("MostrarInformacion", new { Id = idProducto });

        }








    }
}


