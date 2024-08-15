using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class VendedorController : Controller
    {
        // Acción GET para mostrar la lista de productos con la posibilidad de aplicar filtros.
        public ActionResult Index(string nombre, string personas, string lugar, string proveedor, decimal? precio, decimal? precioMin, decimal? precioMax, string categoria)
        {
            // Recupera el usuario actual desde la sesión y lo pasa a la vista mediante ViewBag.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de productos para la vista.
            List<ProductosTableViewModel> lstProductos = new List<ProductosTableViewModel>();

            // Usa la base de datos para consultar los productos.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
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
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Aplicar filtros según los parámetros recibidos.
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

                // Convertir la consulta en una lista de productos.
                lstProductos = query.ToList();
            }

            // Devuelve la vista con la lista de productos filtrados.
            return View(lstProductos);
        }

        // --------------------------------------------  ADD  -----------------------------------------------

        // Acción GET que muestra la vista para agregar un nuevo producto.
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        // Acción POST que maneja la lógica para agregar un nuevo producto.
        [HttpPost]
        [ValidateInput(false)] // Permite la entrada de HTML, necesaria para manejar las imágenes.
        public ActionResult Add(ProductosTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
        {
            // Verifica si el modelo es válido.
            if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para agregar el nuevo producto.
            using (var db = new TRAVEL2Entities())
            {
                // Recupera el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Variables para almacenar las imágenes en bytes.
                byte[] imagenBytes = null;
                byte[] imagenBytes2 = null;
                byte[] imagenBytes3 = null;

                // Procesa la primera imagen.
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                    {
                        imagenBytes = binaryReader.ReadBytes(ImagenFile.ContentLength);
                    }
                }

                // Procesa la segunda imagen.
                if (ImagenFile2 != null && ImagenFile2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile2.InputStream))
                    {
                        imagenBytes2 = binaryReader.ReadBytes(ImagenFile2.ContentLength);
                    }
                }

                // Procesa la tercera imagen.
                if (ImagenFile3 != null && ImagenFile3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile3.InputStream))
                    {
                        imagenBytes3 = binaryReader.ReadBytes(ImagenFile3.ContentLength);
                    }
                }

                // Crea una nueva instancia de la entidad Productos con los datos proporcionados.
                Productos productoTO = new Productos
                {
                    Nombre = model.Nombre,
                    Lugar = model.Lugar,
                    Descripcion = model.Descripcion,
                    Precio = model.Precio,
                    Categoria = model.Categoria,
                    Personas = model.Personas,
                    CantidadEnStock = model.CantidadEnStock,
                    Comentarios = model.Comentario,
                    Calificacion = model.Calificacion,
                    Proveedor = model.Proveedor,
                    Vendedor = usuario.Cedula, // Asigna la cédula del vendedor al producto.
                    Imagen = imagenBytes,
                    Imagen2 = imagenBytes2,
                    Imagen3 = imagenBytes3
                };

                // Agrega el producto a la base de datos y guarda los cambios.
                db.Productos.Add(productoTO);
                db.SaveChanges();

                // Redirige al usuario a la página de índice después de agregar el producto.
                return Redirect(Url.Content("~/Vendedor/Index"));
            }
        }

        // --------------------------------------------  EDIT  -----------------------------------------------

        // Acción GET que muestra la vista para editar un producto específico.
        [HttpGet]
        public ActionResult Edit(int ProductoID)
        {
            using (var db = new TRAVEL2Entities())
            {
                // Busca el producto por su ID.
                var producto = db.Productos.Find(ProductoID);

                // Guarda el nombre del producto en un ViewBag para usarlo en la vista de edición.
                ViewBag.ProductoNombre = producto.Nombre;
                ViewBag.ProductoGenero = producto.Lugar;

                // Si el producto no se encuentra, redirige al índice.
                if (producto == null)
                {
                    return RedirectToAction("Index");
                }

                // Mapea los datos del producto al modelo de la vista.
                var model = new ProductosTableViewModel
                {
                    Nombre = producto.Nombre,
                    Lugar = producto.Lugar,
                    Descripcion = producto.Descripcion,
                    Precio = (decimal)producto.Precio,
                    Categoria = producto.Categoria,
                    CantidadEnStock = producto.CantidadEnStock,
                    Comentario = producto.Comentarios,
                    Calificacion = producto.Calificacion != null ? (int)producto.Calificacion : 0,
                    Proveedor = producto.Proveedor
                };

                // Guarda el proveedor actual en un ViewBag para usarlo en la vista.
                ViewBag.GeneroSeleccionado = producto.Proveedor;

                // Devuelve la vista con el modelo de producto.
                return View(model);
            }
        }

        // Acción POST que maneja la lógica para actualizar un producto.
        [HttpPost]
        public ActionResult Edit(ProductosTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action)
        {
            // Verifica si el modelo es válido.
            if (!ModelState.IsValid) return View(model);

            using (var db = new TRAVEL2Entities())
            {
                // Busca el producto por su ID en la base de datos.
                var productoTO = db.Productos.Find(model.ProductoID);

                // Actualiza las propiedades del producto con los valores del modelo.
                productoTO.Nombre = model.Nombre;
                productoTO.Lugar = model.Lugar;
                productoTO.Descripcion = model.Descripcion;
                productoTO.Precio = model.Precio;
                productoTO.Categoria = model.Categoria;
                productoTO.CantidadEnStock = model.CantidadEnStock;
                productoTO.Comentarios = model.Comentario;
                productoTO.Calificacion = model.Calificacion;
                productoTO.Proveedor = model.Proveedor;

                // Actualiza las imágenes del producto según las acciones del usuario.
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        productoTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        productoTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                if (action == "EliminarImagen2" || model.EliminarImagen2)
                {
                    productoTO.Imagen2 = null;
                }

                else if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        productoTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                if (action == "EliminarImagen3" || model.EliminarImagen3)
                {
                    productoTO.Imagen3 = null;
                }
                else if (NuevaImagen3 != null && NuevaImagen3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen3.InputStream))
                    {
                        productoTO.Imagen3 = binaryReader.ReadBytes(NuevaImagen3.ContentLength);
                    }
                }

                if (action == "EliminarImagen" || model.EliminarImagen)
                {
                    productoTO.Imagen = null;
                }
                else if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        productoTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                // Guarda los cambios en la base de datos.
                db.SaveChanges();

                // Redirige al usuario a la página de índice después de actualizar el producto.
                return Redirect(Url.Content("~/Vendedor/Index"));
            }
        }

        // --------------------------------------------  DELETE  -----------------------------------------------

        // Acción GET que maneja la eliminación de un producto.
        [HttpGet]
        public ActionResult Delete(int ProductoID)
        {
            using (var db = new TRAVEL2Entities())
            {
                // Recupera el usuario actual desde la sesión.
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                // Busca el producto por su ID en la base de datos.
                var productoTO = db.Productos.Find(ProductoID);

                // Si el producto no se encuentra, retorna un error 404.
                if (productoTO == null)
                {
                    return HttpNotFound();
                }

                // Elimina el producto de la base de datos y guarda los cambios.
                db.Productos.Remove(productoTO);
                db.SaveChanges();
            }

            // Redirige al usuario a la página de índice después de eliminar el producto.
            return RedirectToAction("Index");
        }
    }
}
