using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KREAM_ProyectoFinal.Controllers
{
    public class ArticuloController : Controller
    {
        // GET: Articulo
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult View(int Id)
        {
            using (var db = new KreamDBEntities())
            {
                var producto = db.Productos.Find(Id);

                if (producto == null)
                {
                    return RedirectToAction("Index");
                }

                var model = new ProductosTableViewModel
                {

                    ProductoID = producto.ProductoID,
                    Nombre = producto.Nombre,
                    Color = producto.Color,
                    Descripcion = producto.Descripcion,
                    Precio = producto.Precio,
                    Categoria = producto.Categoria,
                    Marca = producto.Marca,
                    CantidadEnStock = producto.CantidadEnStock,
                    Comentario = producto.Comentario,
                    Calificacion = producto.Calificacion,
                };
                return View(model);
            }
        }




    }
}
