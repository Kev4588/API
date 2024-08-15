using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using KREAM_ProyectoFinal.Models;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;

namespace PURIS_FLASH.Controllers
{
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class HomeController : Controller
    {
        // Método que maneja la lógica de la vista principal. Permite la búsqueda y filtrado de productos según varios parámetros.
        public ActionResult Index(string nombre, string personas, string lugar, string proveedor, decimal? precioMin, decimal? precioMax, string categoria)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;
            ViewBag.Id = usuarioActual.Id;

            // Inicializa una lista de productos.
            List<ProductosTableViewModel> lstProductos = null;

            // Usa la base de datos para realizar consultas sobre los productos.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Consulta básica para seleccionar productos de la base de datos y mapearlos a la vista.
                var query = from p in db.Productos
                            select new ProductosTableViewModel
                            {
                                ProductoID = p.ProductoID,
                                Nombre = p.Nombre,
                                Lugar = p.Lugar,
                                Descripcion = p.Descripcion,
                                Precio = (decimal)(p.Precio ?? 0),
                                Categoria = p.Categoria,
                                Personas = p.Personas,
                                CantidadEnStock = p.CantidadEnStock,
                                Comentario = p.Comentarios,
                                Calificacion = (int)(p.Calificacion ?? 0),
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Filtra por nombre del producto si se proporciona.
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }

                // Filtra por cantidad de personas si se proporciona.
                if (!string.IsNullOrEmpty(personas))
                {
                    query = query.Where(p => p.Personas.Contains(personas));
                }

                // Obtiene la lista de personas (número de personas) distintas de la base de datos.
                var Personas = db.Productos.Select(p => p.Personas).Distinct().ToList();
                ViewBag.Personas = new SelectList(Personas);

                // Filtra nuevamente por la cantidad de personas si se proporciona.
                if (!string.IsNullOrEmpty(personas))
                {
                    query = query.Where(p => p.Personas == personas);
                }

                // Filtra por categoría si se proporciona.
                if (!string.IsNullOrEmpty(categoria))
                {
                    query = query.Where(p => p.Categoria.Contains(categoria));
                }

                // Obtiene la lista de categorías distintas de la base de datos.
                var categorias = db.Productos.Select(p => p.Categoria).Distinct().ToList();
                ViewBag.Categoria = new SelectList(categorias);

                // Filtra nuevamente por categoría si se proporciona.
                if (!string.IsNullOrEmpty(categoria))
                {
                    query = query.Where(p => p.Categoria == categoria);
                }

                // Filtra por lugar si se proporciona.
                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar.Contains(lugar));
                }

                // Obtiene la lista de lugares distintos de la base de datos.
                var lugares = db.Productos.Select(p => p.Lugar).Distinct().ToList();
                ViewBag.Lugar = new SelectList(lugares);

                // Filtra nuevamente por lugar si se proporciona.
                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar == lugar);
                }

                // Filtra por proveedor si se proporciona.
                if (!string.IsNullOrEmpty(proveedor))
                {
                    query = query.Where(p => p.Proveedor.Contains(proveedor));
                }

                // Filtra por rango de precio mínimo.
                if (precioMin.HasValue)
                {
                    query = query.Where(p => p.Precio >= precioMin.Value);
                }

                // Filtra por rango de precio máximo.
                if (precioMax.HasValue)
                {
                    query = query.Where(p => p.Precio <= precioMax.Value);
                }

                // Obtiene el precio máximo de la base de datos.
                var precioMaximo = db.Productos.Max(p => p.Precio);

                // Pasa el precio máximo a la vista.
                ViewBag.PrecioMaximo = (int)precioMaximo; // Se convierte a entero porque los filtros no aceptan decimales.

                // Obtiene y ordena la lista de precios distintos.
                var precios = db.Productos.Select(p => p.Precio).Distinct().OrderBy(p => p).ToList();
                ViewBag.Precios = precios;

                // Establece el precio máximo seleccionado.
                ViewBag.PrecioMaxSeleccionado = precioMax ?? precios.LastOrDefault() ?? 1;

                // Pasa los parámetros seleccionados a la vista.
                ViewBag.CategoriaSeleccionada = categoria;
                ViewBag.LugarSeleccionado = lugar;
                ViewBag.PersonasSeleccionada = personas;
                ViewBag.IntensidadSeleccionada = proveedor;
                ViewBag.PrecioMaxSeleccionado = precioMax ?? 1;
                ViewBag.NombreSeleccionado = nombre;

                // Convierte la consulta en una lista de productos.
                lstProductos = query.ToList();
            }

            // Devuelve la vista con la lista de productos filtrados.
            return View(lstProductos);
        }

        // Método para agregar un producto al carrito.
        [HttpPost]
        public ActionResult AgregarAlCarrito(int productoId)
        {
            // Usa la base de datos para realizar operaciones.
            using (var db = new TRAVEL2Entities())
            {
                // Obtiene el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Busca el objeto User correspondiente en la base de datos.
                var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);

                // Verifica si el usuario existe en la base de datos.
                if (usuarioEnBD != null)
                {
                    // Si no hay productos en el carrito, establece el primer producto. De lo contrario, agrega el nuevo producto.
                    usuarioEnBD.ProductosEnCarrito = usuarioEnBD.ProductosEnCarrito == null
                        ? productoId.ToString()
                        : usuarioEnBD.ProductosEnCarrito + "," + productoId;

                    try
                    {
                        // Guarda los cambios en la base de datos.
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        // Maneja excepciones relacionadas con la base de datos si es necesario.
                        // Por ejemplo, si el productoId no existe.
                    }

                    // Actualiza la propiedad ProductosEnCarrito en el ViewModel y guarda en la sesión.
                    usuario.ProductosEnCarrito = usuarioEnBD.ProductosEnCarrito;
                    Session["UsuarioActual"] = usuario;
                }

                // Redirige a la acción Index para mostrar la lista de productos actualizada.
                return RedirectToAction("Index");
            }
        }
    }
}
