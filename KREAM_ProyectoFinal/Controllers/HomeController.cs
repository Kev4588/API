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
    [VerificarSesion]
    public class HomeController : Controller
    {


        public ActionResult Index(string nombre, string personas, string lugar, string proveedor, decimal? precioMin, decimal? precioMax, string categoria)
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;
            ViewBag.Id = usuarioActual.Id;

            List<ProductosTableViewModel> lstProductos = null;

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
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

                // Filtrar por nombre
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }

                // Filtrar por persona 
                if (!string.IsNullOrEmpty(personas))
                {
                    query = query.Where(p => p.Personas.Contains(personas));
                }

                // Obtener marcas distintas de la base de datos en caso que si este buscando alguna marca 
                var Personas = db.Productos.Select(p => p.Personas).Distinct().ToList();
                ViewBag.Personas = new SelectList(Personas);

                if (!string.IsNullOrEmpty(personas))
                {
                    query = query.Where(p => p.Personas == personas);
                }

                // CATEGORIA 
                // Esto es por si la persona quiera simplemente no buscar una categoria en particular igual se puede 
                if (!string.IsNullOrEmpty(categoria))
                {
                    query = query.Where(p => p.Categoria.Contains(categoria));
                }
                // Obtener marcas distintas de la base de datos en caso que si este buscando alguna marca 
                var categorias = db.Productos.Select(p => p.Categoria).Distinct().ToList();
                ViewBag.Categoria = new SelectList(categorias);

                if (!string.IsNullOrEmpty(categoria))
                {
                    query = query.Where(p => p.Categoria == categoria);
                }


                //************************************************************************************

                // Filtrar por LUGAR
                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar.Contains(lugar));  // ESTO CON EL .CONTAINS , LO QUE HACE SES QUE SI POR EJEMPLO UN PRODUCTO DIJERA AZUL/BLANCO Y NO SOLO BLANCO  igual lo tomaria proque el contains lo que hace es traier justo eso que contenga la palabra = True 
                }
                // Obtener marcas distintas de la base de datos en caso que si este buscando alguna marca 
                var lugares = db.Productos.Select(p => p.Lugar).Distinct().ToList();
                ViewBag.Lugar = new SelectList(lugares);

                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar == lugar);
                }


              


                // Filtrar por proveedor
                if (!string.IsNullOrEmpty(proveedor))
                {
                    query = query.Where(p => p.Proveedor.Contains(proveedor));
                }


                //************************************************************************************

                // Filtrar por rango de precio
                if (precioMin.HasValue)
                {
                    query = query.Where(p => p.Precio >= precioMin.Value);
                }

                if (precioMax.HasValue)
                {
                    query = query.Where(p => p.Precio <= precioMax.Value);
                }

                // Ejemplo de cómo obtener el precio máximo de la base de datos
                var precioMaximo = db.Productos.Max(p => p.Precio);  // Asumiendo que tienes una entidad Producto con un campo Precio




                // Pasar el precio máximo a la vista
                ViewBag.PrecioMaximo = (int)precioMaximo; // hay que convertirlo  a INT o cuando se usa en los filtros da error , esto 
                // porque los filtros de rango de precio no toman decimales sino numeros enteros y como precio si viene en decimales se va al demonio

                // Obtener precios distintos y ordenarlos
                var precios = db.Productos.Select(p => p.Precio).Distinct().OrderBy(p => p).ToList();
                ViewBag.Precios = precios;

                // Establecer el precio máximo seleccionado
                ViewBag.PrecioMaxSeleccionado = precioMax ?? precios.LastOrDefault() ?? 1;

                ViewBag.CategoriaSeleccionada = categoria;
                ViewBag.LugarSeleccionado = lugar;
                ViewBag.PersonasSeleccionada = personas;
                ViewBag.IntensidadSeleccionada = proveedor;
                ViewBag.PrecioMaxSeleccionado = precioMax ?? 1;
                ViewBag.NombreSeleccionado = nombre;
                lstProductos = query.ToList();



            }

            return View(lstProductos);
        }







        [HttpPost]
        public ActionResult AgregarAlCarrito(int productoId)
        {
            using (var db = new TRAVEL2Entities())
            {
                // Obtener el usuario actual desde la sesión
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Obtener el objeto User de la base de datos
                var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);

                // Verificar si el usuario existe en la base de datos
                if (usuarioEnBD != null)
                {
                    // Si ProductosEnCarrito es null, establecerlo en el primer productoId, de lo contrario, agregarlo a la lista existente
                    usuarioEnBD.ProductosEnCarrito = usuarioEnBD.ProductosEnCarrito == null
                        ? productoId.ToString()
                        : usuarioEnBD.ProductosEnCarrito + "," + productoId;

                    try
                    {
                        // Guardar los cambios en la base de datos
                        db.SaveChanges();
                    }
                    catch (DbUpdateException)
                    {
                        // Manejar excepciones de base de datos, si es necesario
                        // Por ejemplo, el productoId no existe
                    }

                    // Actualizar la propiedad ProductosEnCarrito en el ViewModel y guardar en la sesión
                    usuario.ProductosEnCarrito = usuarioEnBD.ProductosEnCarrito;
                    Session["UsuarioActual"] = usuario;


                }


                return RedirectToAction("Index");
            }
        }




    }
}


