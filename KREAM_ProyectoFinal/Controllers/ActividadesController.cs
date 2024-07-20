using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using KREAM_ProyectoFinal.Models;
using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models.ViewModel;

namespace KREAM_ProyectoFinal.Controllers
{
    [VerificarSesion]
    public class ActividadesController : Controller
    {

        public ActionResult Index(string nombre, string personas, string lugar, string proveedor, decimal? precio, decimal? precioMin, decimal? precioMax, string categoria)
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            List<ProductosTableViewModel> lstProductos = new List<ProductosTableViewModel>();

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
                                Comentario = p.Comentario,
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Apply filters
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

                lstProductos = query.ToList();
            }

            return View(lstProductos);
        }


        
        public ActionResult MostrarInformacion(string nombre, string personas, string lugar, string proveedor, decimal? precio, decimal? precioMin, decimal? precioMax, string categoria)
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            List<ProductosTableViewModel> lstProductos; //= new List<ProductosTableViewModel>();

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
                                Comentario = p.Comentario,
                                Calificacion = p.Calificacion ?? 0,
                                Proveedor = p.Proveedor,
                                Imagen = p.Imagen,
                                Imagen2 = p.Imagen2,
                                Imagen3 = p.Imagen3,
                            };

                // Apply filters
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(p => p.Nombre.Contains(nombre));
                }



                if (!string.IsNullOrEmpty(lugar))
                {
                    query = query.Where(p => p.Lugar.Contains(lugar));
                }


               



                lstProductos = query.ToList();
            }

            return View(lstProductos);
        }



        
        //---------------------------------------------- FUNCIONALIDAD DEL BOTON ADD TOURS / ACCION DE BOTON  -------------------------------------------------- 
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(ProductosTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
        {
            if (!ModelState.IsValid) return View(model);

            using (var db = new TRAVEL2Entities())
            {
                // Proceso para la primera imagen
                byte[] imagenBytes = null;
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                    {
                        imagenBytes = binaryReader.ReadBytes(ImagenFile.ContentLength);
                    }
                }

                // Proceso para la segunda imagen
                byte[] imagenBytes2 = null;
                if (ImagenFile2 != null && ImagenFile2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile2.InputStream))
                    {
                        imagenBytes2 = binaryReader.ReadBytes(ImagenFile2.ContentLength);
                    }
                }

                // Proceso para la tercera imagen
                byte[] imagenBytes3 = null;
                if (ImagenFile3 != null && ImagenFile3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile3.InputStream))
                    {
                        imagenBytes3 = binaryReader.ReadBytes(ImagenFile3.ContentLength);
                    }
                }

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

                db.Productos.Add(productoTO);
                db.SaveChanges();

                return Redirect(Url.Content("~/Actividades/MostrarInformacion")); // Corregir la redirección aquí
            }
        }


        // --------------------------------------------  EDIT  -----------------------------------------------

        [HttpGet]
        public ActionResult Edit(int ProductoID)
        {
            using (var db = new TRAVEL2Entities())
            {
                var producto = db.Productos.Find(ProductoID); // El ProductID viene como parametro para poder buscar el Producto

                ViewBag.Nombre = producto.Nombre; // SALVAMOS EL NOMBRE DEL PRODUCTO EN UN VIEWBAG PARA USARLO LUEGO EN LA VISTA DE EDIT

                if (producto == null)
                {
                    return RedirectToAction("MostrarInformacion");
                }

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

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductosTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action) //
        {
            if (!ModelState.IsValid) return View(model);


            using (var db = new TRAVEL2Entities())
            {

                // DECLARANDO LA SESION ACTIVA AL USUARIO ACTUAL PARA PASAR POR VIEWBAGS
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                var productoTO = db.Productos.Find(model.ProductoID);

                productoTO.Nombre = model.Nombre;
                productoTO.Categoria = model.Categoria;
                productoTO.Personas = model.Personas;
                productoTO.Precio = model.Precio;

                // IMAGENES  -- Esto tambien aplica para eliminarlas o agregar otra 

                // Actualiza las propiedades de las imágenes según la acción del usuario
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    // El usuario ha proporcionado una nueva imagen
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

                // Actualiza las propiedades de las imágenes según la acción del usuario para Imagen3
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
                    // El usuario ha proporcionado una nueva imagen
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        productoTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                db.SaveChanges();

                return Redirect(Url.Content("~/Actividades/MostrarInformacion"));
            }
        }




        // --------------------------------------------  DELETE  -----------------------------------------------
        [HttpGet]
        public ActionResult Delete(int ProductoID)
        {
            using (var db = new TRAVEL2Entities())
            {
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                var productoTO = db.Productos.Find(ProductoID);

                if (productoTO == null)
                {
                    return HttpNotFound();
                }

                db.Productos.Remove(productoTO);
                db.SaveChanges();
            }

            return RedirectToAction("MostrarInformacion");
        }

    }
}








