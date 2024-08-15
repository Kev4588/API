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
    public class ActividadesController : Controller
    {
        // Método que maneja la vista principal, permite la búsqueda y filtrado de productos según varios parámetros.
        public ActionResult Index(string nombre, string personas, string lugar, string proveedor, decimal? precio, decimal? precioMin, decimal? precioMax, string categoria)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de productos.
            List<ProductosTableViewModel> lstProductos = new List<ProductosTableViewModel>();

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
                                Precio = (decimal)p.Precio,
                                Categoria = p.Categoria,
                                Personas = p.Personas,
                                Comentario = p.Comentarios,
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Aplica filtros según los parámetros proporcionados.
                if (!string.IsNullOrEmpty(categoria))
                {
                    query = query.Where(p => p.Categoria.Contains(categoria));
                }

                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar.Contains(lugar));
                }

                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }

                // Convierte la consulta en una lista de productos.
                lstProductos = query.ToList();
            }

            // Devuelve la vista con la lista de productos filtrados.
            return View(lstProductos);
        }

        // Método para mostrar información filtrada de productos.
        public ActionResult MostrarInformacion(string nombre, string personas, string lugar, string proveedor, decimal? precio, decimal? precioMin, decimal? precioMax, string categoria)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de productos.
            List<ProductosTableViewModel> lstProductos;

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
                                Precio = (decimal)p.Precio,
                                Categoria = p.Categoria,
                                Personas = p.Personas,
                                CantidadEnStock = p.CantidadEnStock,
                                Comentario = p.Comentarios,
                                Calificacion = p.Calificacion ?? 0,
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Aplica filtros según los parámetros proporcionados.
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }

                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar.Contains(lugar));
                }

                // Convierte la consulta en una lista de productos.
                lstProductos = query.ToList();
            }

            // Devuelve la vista con la lista de productos filtrados.
            return View(lstProductos);
        }

        // -------------------------------- FUNCIONALIDAD DEL BOTÓN "Agregar Productos" -------------------------------- 
        [HttpGet]
        public ActionResult Add()
        {
            // Devuelve la vista para agregar un nuevo producto.
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ProductosTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para agregar el nuevo producto.
            using (var db = new TRAVEL2Entities())
            {
                // Procesa la primera imagen.
                byte[] imagenBytes = null;
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                    {
                        imagenBytes = binaryReader.ReadBytes(ImagenFile.ContentLength);
                    }
                }

                // Procesa la segunda imagen.
                byte[] imagenBytes2 = null;
                if (ImagenFile2 != null && ImagenFile2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile2.InputStream))
                    {
                        imagenBytes2 = binaryReader.ReadBytes(ImagenFile2.ContentLength);
                    }
                }

                // Procesa la tercera imagen.
                byte[] imagenBytes3 = null;
                if (ImagenFile3 != null && ImagenFile3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile3.InputStream))
                    {
                        imagenBytes3 = binaryReader.ReadBytes(ImagenFile3.ContentLength);
                    }
                }

                // Crea un nuevo objeto de producto con los datos proporcionados.
                var productoTO = new Productos
                {
                    Nombre = model.Nombre,
                    Categoria = model.Categoria,
                    Personas = model.Personas,
                    Precio = model.Precio,
                    Imagen = imagenBytes,
                    Imagen2 = imagenBytes2,
                    Imagen3 = imagenBytes3,
                };

                // Agrega el producto a la base de datos y guarda los cambios.
                db.Productos.Add(productoTO);
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Actividades/MostrarInformacion"));
            }
        }

        // -------------------------------------------- EDITAR PRODUCTO -----------------------------------------------

        [HttpGet]
        public ActionResult Edit(int ProductoID)
        {
            // Usa la base de datos para buscar el producto por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var producto = db.Productos.Find(ProductoID);

                // Guarda el nombre del producto en un ViewBag para usarlo en la vista de edición.
                ViewBag.Nombre = producto.Nombre;

                // Si el producto no se encuentra, redirige a la vista de mostrar información.
                if (producto == null)
                {
                    return RedirectToAction("MostrarInformacion");
                }

                // Crea un modelo de vista con los datos del producto.
                var model = new ProductosTableViewModel
                {
                    Nombre = producto.Nombre,
                    Categoria = producto.Categoria,
                    Personas = producto.Personas,
                    Precio = (decimal)producto.Precio,
                    Imagen = producto.Imagen,
                    Imagen2 = producto.Imagen2,
                    Imagen3 = producto.Imagen3,
                };

                // Devuelve la vista con el modelo de edición.
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductosTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para editar el producto.
            using (var db = new TRAVEL2Entities())
            {
                // Recupera el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Busca el producto por su ID en la base de datos.
                var productoTO = db.Productos.Find(model.ProductoID);

                // Actualiza las propiedades del producto con los valores del modelo.
                productoTO.Nombre = model.Nombre;
                productoTO.Categoria = model.Categoria;
                productoTO.Personas = model.Personas;
                productoTO.Precio = model.Precio;

                // Procesa y actualiza las imágenes según las acciones del usuario.

                // Para la primera imagen.
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        productoTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                // Para la segunda imagen.
                if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        productoTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                // Elimina la segunda imagen si el usuario lo indica.
                if (action == "EliminarImagen2" || model.EliminarImagen2)
                {
                    productoTO.Imagen2 = null;
                }

                // Para la tercera imagen.
                if (NuevaImagen3 != null && NuevaImagen3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen3.InputStream))
                    {
                        productoTO.Imagen3 = binaryReader.ReadBytes(NuevaImagen3.ContentLength);
                    }
                }

                // Elimina la tercera imagen si el usuario lo indica.
                if (action == "EliminarImagen3" || model.EliminarImagen3)
                {
                    productoTO.Imagen3 = null;
                }

                // Elimina la primera imagen si el usuario lo indica.
                if (action == "EliminarImagen" || model.EliminarImagen)
                {
                    productoTO.Imagen = null;
                }

                // Guarda los cambios en la base de datos.
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Actividades/MostrarInformacion"));
            }
        }

        // -------------------------------------------- ELIMINAR PRODUCTO -----------------------------------------------

        [HttpGet]
        public ActionResult Delete(int ProductoID)
        {
            // Usa la base de datos para eliminar un producto por su ID.
            using (var db = new TRAVEL2Entities())
            {
                // Recupera el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Busca el producto por su ID en la base de datos.
                var productoTO = db.Productos.Find(ProductoID);

                // Si el producto no se encuentra, devuelve un error 404.
                if (productoTO == null)
                {
                    return HttpNotFound();
                }

                // Elimina el producto de la base de datos y guarda los cambios.
                db.Productos.Remove(productoTO);
                db.SaveChanges();
            }

            // Redirige a la vista de mostrar información.
            return RedirectToAction("MostrarInformacion");
        }
    }
}
