using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;

namespace PURIS_FLASH.Controllers
{
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class RepartidoresController : Controller
    {
        // Método que maneja la vista principal de repartidores, permitiendo la búsqueda y filtrado por nombre y tipo de transporte.
        public ActionResult Index(string nombre, string transporte)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de repartidores.
            List<RepartidoresTableViewModel> lstRepartidores = new List<RepartidoresTableViewModel>();

            // Usa la base de datos para realizar consultas sobre los repartidores.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Consulta básica para seleccionar repartidores de la base de datos y mapearlos a la vista.
                var query = from r in db.Repartidores
                            select new RepartidoresTableViewModel
                            {
                                IDRepartidor = r.IDRepartidor,
                                NombreRepartidor = r.NombreRepartidor,
                                TipoDeTransporte = r.TipoDeTransporte,
                                Descripcion = r.Descripcion,
                                Telefono = (int)r.Telefono,
                                Imagen = r.Imagen,
                                Imagen2 = r.Imagen2,
                                Imagen3 = r.Imagen3,
                            };

                // Filtrar por nombre del repartidor si se proporciona.
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(r => r.NombreRepartidor.Contains(nombre));
                }
                ViewBag.NombreRepartidorSeleccionado = nombre;

                // Filtrar por tipo de transporte si se proporciona.
                if (!string.IsNullOrEmpty(transporte))
                {
                    query = query.Where(r => r.TipoDeTransporte.Contains(transporte));
                }

                // Obtener la lista de tipos de transporte distintos de la base de datos.
                var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                ViewBag.Categoria = new SelectList(transportes);

                // Pasa el tipo de transporte seleccionado a la vista.
                ViewBag.TipoDeTransporteSeleccionado = transporte;

                // Convierte la consulta en una lista de repartidores.
                lstRepartidores = query.ToList();
            }

            // Devuelve la vista con la lista de repartidores filtrados.
            return View(lstRepartidores);
        }

        // ------------------------------------------- MENÚ PRINCIPAL DE ADMINISTRACIÓN DE REPARTIDORES --------------------------------

        // Método que maneja la visualización de la información de los repartidores, permitiendo al administrador eliminar, editar o crear nuevos repartidores.
        public ActionResult MostrarInformacion(string tipoDeTransporte, int? Telefono, string NombreRepartidor)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de repartidores.
            List<RepartidoresTableViewModel> lstRepartidores = new List<RepartidoresTableViewModel>();

            // Usa la base de datos para realizar consultas sobre los repartidores.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Consulta básica para seleccionar repartidores de la base de datos y mapearlos a la vista.
                var query = from r in db.Repartidores
                            select new RepartidoresTableViewModel
                            {
                                IDRepartidor = r.IDRepartidor,
                                NombreRepartidor = r.NombreRepartidor,
                                TipoDeTransporte = r.TipoDeTransporte,
                                Descripcion = r.Descripcion,
                                Telefono = (int)r.Telefono,
                                Imagen = r.Imagen,
                                Imagen2 = r.Imagen2,
                                Imagen3 = r.Imagen3,
                            };

                // Filtrar por nombre del repartidor si se proporciona.
                if (!string.IsNullOrEmpty(NombreRepartidor))
                {
                    query = query.Where(r => r.NombreRepartidor.Contains(NombreRepartidor));
                }

                // Obtener la lista de tipos de transporte distintos de la base de datos.
                var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                ViewBag.Categoria = new SelectList(transportes);

                // Pasa los parámetros seleccionados a la vista.
                ViewBag.TipoDeTransporteSeleccionado = tipoDeTransporte;
                ViewBag.NombreRepartidorSeleccionado = NombreRepartidor;

                // Convierte la consulta en una lista de repartidores.
                lstRepartidores = query.ToList();
            }

            // Devuelve la vista con la lista de repartidores filtrados.
            return View(lstRepartidores);
        }

        //---------------------------------------------- FUNCIONALIDAD DEL BOTÓN "ADD REPARTIDORES" -------------------------------- 

        // Método que muestra la vista para agregar un nuevo repartidor.
        [HttpGet]
        public ActionResult Add()
        {
            // Establecer el ViewBag.Categoria con tipos de transporte antes de cargar la vista.
            using (var db = new TRAVEL2Entities())
            {
                var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                ViewBag.Categoria = new SelectList(transportes);
            }

            return View();
        }

        // Método que maneja la lógica para agregar un nuevo repartidor.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(RepartidoresTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid)
            {
                // Reestablecer ViewBag.Categoria antes de devolver la vista.
                using (var db = new TRAVEL2Entities())
                {
                    var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                    ViewBag.Categoria = new SelectList(transportes);
                }

                return View(model);
            }

            // Usa la base de datos para agregar el nuevo repartidor.
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

                // Crea una nueva instancia de la entidad Repartidores con los datos proporcionados.
                var repartidorTO = new Repartidores
                {
                    NombreRepartidor = model.NombreRepartidor,
                    TipoDeTransporte = model.TipoDeTransporte,
                    Descripcion = model.Descripcion,
                    Telefono = model.Telefono,
                    Imagen = imagenBytes,
                    Imagen2 = imagenBytes2,
                    Imagen3 = imagenBytes3
                };

                // Agrega el repartidor a la base de datos y guarda los cambios.
                db.Repartidores.Add(repartidorTO);
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Repartidores/MostrarInformacion"));
            }
        }

        // -------------------------------------------- EDICIÓN DE REPARTIDORES -----------------------------------------------

        // Método que muestra la vista para editar un repartidor específico.
        [HttpGet]
        public ActionResult Edit(int IDRepartidor)
        {
            using (var db = new TRAVEL2Entities())
            {
                var repartidor = db.Repartidores.Find(IDRepartidor);

                // Guarda el nombre del repartidor en un ViewBag para usarlo en la vista de edición.
                ViewBag.RepartidorNombre = repartidor.NombreRepartidor;

                // Si el repartidor no se encuentra, redirige a la vista de mostrar información.
                if (repartidor == null)
                {
                    return RedirectToAction("MostrarInformacion");
                }

                // Obtener la lista de tipos de transporte distintos de la base de datos.
                var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                ViewBag.Categoria = new SelectList(transportes);

                // Crea un modelo de vista con los datos del repartidor.
                var model = new RepartidoresTableViewModel
                {
                    IDRepartidor = repartidor.IDRepartidor,
                    NombreRepartidor = repartidor.NombreRepartidor,
                    TipoDeTransporte = repartidor.TipoDeTransporte,
                    Descripcion = repartidor.Descripcion,
                    Telefono = (int)repartidor.Telefono,
                    Imagen = repartidor.Imagen,
                    Imagen2 = repartidor.Imagen2,
                    Imagen3 = repartidor.Imagen3
                };

                // Devuelve la vista con el modelo de edición.
                return View(model);
            }
        }

        // Método que maneja la lógica para editar un repartidor.
        [HttpPost]
        public ActionResult Edit(RepartidoresTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid)
            {
                using (var db = new TRAVEL2Entities())
                {
                    var transportes = db.Repartidores.Select(r => r.TipoDeTransporte).Distinct().ToList();
                    ViewBag.Categoria = new SelectList(transportes);
                }
                return View(model);
            }

            // Usa la base de datos para actualizar los datos del repartidor.
            using (var db = new TRAVEL2Entities())
            {
                var repartidorTO = db.Repartidores.Find(model.IDRepartidor);

                // Actualiza las propiedades del repartidor con los valores del modelo.
                repartidorTO.NombreRepartidor = model.NombreRepartidor;
                repartidorTO.TipoDeTransporte = model.TipoDeTransporte;
                repartidorTO.Descripcion = model.Descripcion;
                repartidorTO.Telefono = model.Telefono;

                // Procesa y actualiza las imágenes según las acciones del usuario.

                // Para la primera imagen.
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        repartidorTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                // Para la segunda imagen.
                if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        repartidorTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                // Elimina la segunda imagen si el usuario lo indica.
                if (action == "EliminarImagen2" || model.EliminarImagen2)
                {
                    repartidorTO.Imagen2 = null;
                }

                // Para la tercera imagen.
                if (NuevaImagen3 != null && NuevaImagen3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen3.InputStream))
                    {
                        repartidorTO.Imagen3 = binaryReader.ReadBytes(NuevaImagen3.ContentLength);
                    }
                }

                // Elimina la tercera imagen si el usuario lo indica.
                if (action == "EliminarImagen3" || model.EliminarImagen3)
                {
                    repartidorTO.Imagen3 = null;
                }

                // Elimina la primera imagen si el usuario lo indica.
                if (action == "EliminarImagen" || model.EliminarImagen)
                {
                    repartidorTO.Imagen = null;
                }

                // Guarda los cambios en la base de datos.
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Repartidores/MostrarInformacion"));
            }
        }

        // -------------------------------------------- ELIMINACIÓN DE REPARTIDORES -----------------------------------------------

        // Método que maneja la eliminación de un repartidor.
        [HttpGet]
        public ActionResult Delete(int IDRepartidor)
        {
            // Usa la base de datos para eliminar un repartidor por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var repartidorTO = db.Repartidores.Find(IDRepartidor);

                // Si el repartidor no se encuentra, devuelve un error 404.
                if (repartidorTO == null)
                {
                    return HttpNotFound();
                }

                // Elimina el repartidor de la base de datos y guarda los cambios.
                db.Repartidores.Remove(repartidorTO);
                db.SaveChanges();
            }

            // Redirige a la vista de mostrar información.
            return RedirectToAction("MostrarInformacion");
        }
    }
}
