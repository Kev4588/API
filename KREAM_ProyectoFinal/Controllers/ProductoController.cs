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
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class ProductoController : Controller
    {
        // Acción que maneja la vista principal del producto (actualmente solo devuelve la vista).
        public ActionResult Index()
        {
            return View();
        }

        // Acción GET que muestra la información detallada de un producto específico.
        [HttpGet]
        public ActionResult MostrarInformacion(int Id)
        {
            // Inicializa un modelo para pasar los datos del producto a la vista.
            ProductosViewModel model = new ProductosViewModel();

            // Usa la base de datos para buscar el producto por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var productTO = db.Productos.Find(Id);

                // Mapea los datos del producto al modelo.
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

                // Guarda el nombre del producto en un ViewBag para usarlo en la vista.
                ViewBag.Nombre = productTO.Nombre;
            }

            // Devuelve la vista con el modelo de producto.
            return View(model);
        }

        // Acción POST que maneja la lógica para guardar un comentario sobre un producto.
        [HttpPost]
        public ActionResult MostrarInformacion(int idProducto, string comentario)
        {
            // Usa la base de datos para guardar el comentario.
            using (var db = new TRAVEL2Entities())
            {
                // Recupera el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersTableViewModel;

                // Ejecuta un procedimiento almacenado para guardar el comentario del usuario sobre el producto.
                int v = db.Database.ExecuteSqlCommand(
                    "EXEC GuardarComentario @IdProducto, @Comentario",
                    new SqlParameter("@IdProducto", idProducto),
                    new SqlParameter("@Comentario", comentario)
                );
            }

            // Redirige a la vista de mostrar información del producto para actualizar la vista con el nuevo comentario.
            return RedirectToAction("MostrarInformacion", new { Id = idProducto });
        }
    }
}
