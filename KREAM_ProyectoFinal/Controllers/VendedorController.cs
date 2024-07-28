using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Antlr.Runtime.Misc;
using KREAM_ProyectoFinal.Models;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;

namespace PURIS_FLASH.Controllers
{
    [VerificarSesion]
    public class VendedorController : Controller
    {
        // GET: Productos
        
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
                                CantidadEnStock = p.CantidadEnStock,
                                Comentario = p.Comentarios,
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


        // --------------------------------------------  ADD  -----------------------------------------------

        [HttpGet]

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)] // TIENE QUE VER CON LAS IMAGENES 
        // Aqui estamos pasando los parametros del form en ADD donde estan los productos model y algo especial para las imagenes 1 2 y 3
        public ActionResult Add(ProductosTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)


        {
            if (!ModelState.IsValid) return View(model);

            using (var db = new TRAVEL2Entities())

            {
                // estamos tomando la sesion activa de usuario para instanciarla y manipularla aqui 
                var usuario = Session["UsuarioActual"] as UsersViewModel;


                // capturando la imagen que se subio como un null por ahora , ya luego cuando el user la suba habra una conversion a base 64
                byte[] imagenBytes = null;
                byte[] imagenBytes2 = null;
                byte[] imagenBytes3 = null;

                // Proceso para la primera imagen
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile.InputStream))
                    {
                        imagenBytes = binaryReader.ReadBytes(ImagenFile.ContentLength);
                    }
                }

                // Proceso para la segunda imagen
                if (ImagenFile2 != null && ImagenFile2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile2.InputStream))
                    {
                        imagenBytes2 = binaryReader.ReadBytes(ImagenFile2.ContentLength);
                    }
                }

                // Proceso para la tercera imagen
                if (ImagenFile3 != null && ImagenFile3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImagenFile3.InputStream))
                    {
                        imagenBytes3 = binaryReader.ReadBytes(ImagenFile3.ContentLength);
                    }
                }



                // fin de proceso de captura de imagenes 

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
                    Vendedor = usuario.Cedula,
                    // Agregar el campo de imagen
                    Imagen = imagenBytes,
                    Imagen2 = imagenBytes2,
                    Imagen3 = imagenBytes3

                };

                db.Productos.Add(productoTO);
                db.SaveChanges();

                return Redirect(Url.Content("~/Vendedor/Index"));
            }
        }



        // --------------------------------------------  EDIT  -----------------------------------------------

        [HttpGet]
        public ActionResult Edit(int ProductoID)
        {
            using (var db = new TRAVEL2Entities())
            {
                var producto = db.Productos.Find(ProductoID); // El ProductID viene como parametro para poder buscar el Producto

                ViewBag.ProductoNombre = producto.Nombre; // SALVAMOS EL NOMBRE DEL PRODUCTO EN UN VIEWBAG PARA USARLO LUEGO EN LA VISTA DE EDIT

                ViewBag.ProductoGenero = producto.Lugar;


                if (producto == null)
                {
                    return RedirectToAction("Index");
                }

                var model = new ProductosTableViewModel
                {
                   
                    Nombre = producto.Nombre,
                   Lugar = producto.Lugar,
                    Descripcion = producto.Descripcion,
                    Precio = (decimal)producto.Precio,
                    Categoria = producto.Categoria,
                   
                    CantidadEnStock = producto.CantidadEnStock,
                    Comentario = producto.Comentarios,
                    Calificacion = producto.Calificacion != null ? (int)producto.Calificacion : 0, // Convertir de forma segura
                    Proveedor = producto.Proveedor,


                    // Cargando las imagenes para que la persona vea las que tiene relacionadas a ese producto 
                    //Imagen = producto.Imagen,
                    //Imagen2 = producto.Imagen2,
                    //Imagen3 = producto.Imagen3
                };

                // Esta pequeña validacion de abajo lo que hace es que recuerde cual es el genero que ya tenia el producto y lo muestre si la persona queire cambiarlo puede hacerlo 
                ViewBag.GeneroSeleccionado = producto.Proveedor;

                return View(model);
            }
        }

        /*------------ ESTE METODO ES QUIEN ENVIA LO QUE SE HAYA INGRESADO EN EL EDIT ---------------------------   */

        [HttpPost]
        public ActionResult Edit(ProductosTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action) // INCLUIDOS LOS PARAMETROS PARA LAS IMAGENES  llaamdsod nuevaimagen# eso le dice cual debe controlar
        {
            if (!ModelState.IsValid) return View(model);


            using (var db = new TRAVEL2Entities())
            {

                // DECLARANDO LA SESION ACTIVA AL USUARIO ACTUAL PARA PASAR POR VIEWBAGS
                var usuario = Session["UsuarioActual"] as UsersViewModel;


                // DECLARANDO LOS ATRIBUTOS DE PRODUCTO COMO VIEWBAGS
                var productoTO = db.Productos.Find(model.ProductoID);



                productoTO.Nombre = model.Nombre; 

                productoTO.Lugar = model.Lugar;
                productoTO.Descripcion = model.Descripcion;
                productoTO.Precio = model.Precio;
                productoTO.Categoria = model.Categoria;
                //productoTO.Personas = model.Personas;
                productoTO.CantidadEnStock = model.CantidadEnStock;
                productoTO.Comentarios = model.Comentario;
                productoTO.Calificacion = model.Calificacion;
                productoTO.Proveedor = model.Proveedor;

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

                return Redirect(Url.Content("~/Vendedor/Index"));
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

            return RedirectToAction("Index");
        }
    }
}
