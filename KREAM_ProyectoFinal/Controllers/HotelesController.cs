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
    public class HotelesController : Controller
    {
        // Método que maneja la vista principal de hoteles, permitiendo la búsqueda y filtrado por nombre y tipo de habitación.
        public ActionResult Index(string nombre, string habitacion)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de hoteles.
            List<HotelesTableViewModel> lstHoteles = new List<HotelesTableViewModel>();

            // Usa la base de datos para realizar consultas sobre los hoteles.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Consulta básica para seleccionar hoteles de la base de datos y mapearlos a la vista.
                var query = from h in db.Hoteles
                            select new HotelesTableViewModel
                            {
                                IDHotel = h.IDHotel,
                                NombreHotel = h.NombreHotel,
                                web = h.web,
                                Descripcion = h.Descripcion,
                                TipoDeHabitacion = h.TipoDeHabitacion,
                                CantidadDePersonas = (int)h.CantidadDePersonas,
                                Telefono = (int)h.Telefono,
                                Imagen = h.Imagen,
                                Imagen2 = h.Imagen2,
                                Imagen3 = h.Imagen3,
                            };

                // Filtrar por nombre del hotel si se proporciona.
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(h => h.NombreHotel.Contains(nombre));
                }
                ViewBag.NombreHotelSeleccionado = nombre;

                // Filtrar por tipo de habitación si se proporciona.
                if (!string.IsNullOrEmpty(habitacion))
                {
                    query = query.Where(h => h.TipoDeHabitacion.Contains(habitacion));
                }

                // Obtener la lista de tipos de habitaciones distintas de la base de datos.
                var habitaciones = db.Hoteles.Select(h => h.TipoDeHabitacion).Distinct().ToList();
                ViewBag.Categoria = new SelectList(habitaciones);

                // Pasa el tipo de habitación seleccionado a la vista.
                ViewBag.tipoDeHabitacionSeleccionada = habitacion;

                // Convierte la consulta en una lista de hoteles.
                lstHoteles = query.ToList();
            }

            // Devuelve la vista con la lista de hoteles filtrados.
            return View(lstHoteles);
        }

        // ------------------------------------------- MENU PRINCIPAL DE ADMINISTRACIÓN DE HABITACIONES --------------------------------

        // Método que maneja la visualización de la información de las habitaciones, permitiendo al administrador eliminar, editar o crear nuevas habitaciones.
        public ActionResult MostrarInformacion(string tipoDeHabitacion, int? cantidadDePersonas, int? Telefono, int? calificacionHotel, string web, string NombreHotel)
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Pasa la información del usuario a la vista mediante ViewBag.
            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            // Inicializa una lista de hoteles.
            List<HotelesTableViewModel> lstHoteles = new List<HotelesTableViewModel>();

            // Usa la base de datos para realizar consultas sobre los hoteles.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Consulta básica para seleccionar hoteles de la base de datos y mapearlos a la vista.
                var query = from h in db.Hoteles
                            select new HotelesTableViewModel
                            {
                                IDHotel = h.IDHotel,
                                NombreHotel = h.NombreHotel,
                                web = h.web,
                                Descripcion = h.Descripcion,
                                TipoDeHabitacion = h.TipoDeHabitacion,
                                CantidadDePersonas = (int)h.CantidadDePersonas,
                                Telefono = (int)h.Telefono,
                                Imagen = h.Imagen,
                                Imagen2 = h.Imagen2,
                                Imagen3 = h.Imagen3,
                            };

                // Filtrar por nombre del hotel si se proporciona.
                if (!string.IsNullOrEmpty(NombreHotel))
                {
                    query = query.Where(p => p.NombreHotel.Contains(NombreHotel));
                }

                // Filtrar por cantidad de personas si se proporciona.
                var cantpersonas = db.Hoteles.Select(p => p.CantidadDePersonas).Distinct().ToList();
                if (cantpersonas.Count > 0)
                {
                    ViewBag.cantidadPersonas = new SelectList(cantpersonas);
                }
                else
                {
                    ViewBag.cantidadPersonas = new SelectList(new List<int>());
                }

                if (cantidadDePersonas.HasValue)
                {
                    query = query.Where(h => h.CantidadDePersonas == cantidadDePersonas.Value);
                }

                // Pasa los parámetros seleccionados a la vista.
                ViewBag.PersonasSeleccionada = cantidadDePersonas;
                ViewBag.tipoDeHabitacion = tipoDeHabitacion;
                ViewBag.NombreSeleccionado = NombreHotel;

                // Convierte la consulta en una lista de hoteles.
                lstHoteles = query.ToList();
            }

            // Devuelve la vista con la lista de hoteles filtrados.
            return View(lstHoteles);
        }

        //---------------------------------------------- FUNCIONALIDAD DEL BOTÓN "ADD HABITACIONES" -------------------------------- 

        // Método que muestra la vista para agregar una nueva habitación.
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        // Método que maneja la lógica para agregar una nueva habitación.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(HotelesTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para agregar la nueva habitación.
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

                // Crea una nueva instancia de la entidad Hoteles con los datos proporcionados.
                var hotelTO = new Hoteles
                {
                    NombreHotel = model.NombreHotel,
                    TipoDeHabitacion = model.TipoDeHabitacion,
                    web = model.web,
                    CantidadDePersonas = (int)model.CantidadDePersonas,
                    Telefono = model.Telefono,
                    Imagen = imagenBytes,
                    Imagen2 = imagenBytes2,
                    Imagen3 = imagenBytes3,
                    Descripcion = model.Descripcion,
                    ComentarioHotel = model.ComentarioHotel,
                    CalificacionHotel = model.CalificacionHotel
                };

                // Agrega el hotel a la base de datos y guarda los cambios.
                db.Hoteles.Add(hotelTO);
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Hoteles/MostrarInformacion"));
            }
        }

        // -------------------------------------------- EDICIÓN DE HABITACIONES -----------------------------------------------

        // Método que muestra la vista para editar una habitación específica.
        [HttpGet]
        public ActionResult Edit(int IDHotel)
        {
            // Usa la base de datos para buscar el hotel por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var hotel = db.Hoteles.Find(IDHotel);

                // Guarda el nombre del hotel en un ViewBag para usarlo en la vista de edición.
                ViewBag.HotelNombre = hotel.NombreHotel;

                // Si el hotel no se encuentra, redirige a la vista de mostrar información.
                if (hotel == null)
                {
                    return RedirectToAction("MostrarInformacion");
                }

                // Crea un modelo de vista con los datos del hotel.
                var model = new HotelesTableViewModel
                {
                    NombreHotel = hotel.NombreHotel,
                    TipoDeHabitacion = hotel.TipoDeHabitacion,
                    web = hotel.web,
                    CantidadDePersonas = (int)hotel.CantidadDePersonas,
                    Telefono = hotel.Telefono,
                    Descripcion = hotel.Descripcion,
                    Imagen = hotel.Imagen,
                    Imagen2 = hotel.Imagen2,
                    Imagen3 = hotel.Imagen3
                };

                // Devuelve la vista con el modelo de edición.
                return View(model);
            }
        }

        // Método que maneja la lógica para editar una habitación.
        [HttpPost]
        public ActionResult Edit(HotelesTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para actualizar los datos del hotel.
            using (var db = new TRAVEL2Entities())
            {
                var hotelTO = db.Hoteles.Find(model.IDHotel);

                // Actualiza las propiedades del hotel con los valores del modelo.
                hotelTO.NombreHotel = model.NombreHotel;
                hotelTO.TipoDeHabitacion = model.TipoDeHabitacion;
                hotelTO.CantidadDePersonas = model.CantidadDePersonas;
                hotelTO.Telefono = model.Telefono;
                hotelTO.Descripcion = model.Descripcion;

                // Procesa y actualiza las imágenes según las acciones del usuario.

                // Para la primera imagen.
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        hotelTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                // Para la segunda imagen.
                if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        hotelTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                // Elimina la segunda imagen si el usuario lo indica.
                if (action == "EliminarImagen2" || model.EliminarImagen2)
                {
                    hotelTO.Imagen2 = null;
                }

                // Para la tercera imagen.
                if (NuevaImagen3 != null && NuevaImagen3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen3.InputStream))
                    {
                        hotelTO.Imagen3 = binaryReader.ReadBytes(NuevaImagen3.ContentLength);
                    }
                }

                // Elimina la tercera imagen si el usuario lo indica.
                if (action == "EliminarImagen3" || model.EliminarImagen3)
                {
                    hotelTO.Imagen3 = null;
                }

                // Elimina la primera imagen si el usuario lo indica.
                if (action == "EliminarImagen" || model.EliminarImagen)
                {
                    hotelTO.Imagen = null;
                }

                // Guarda los cambios en la base de datos.
                db.SaveChanges();

                // Redirige a la vista de mostrar información.
                return Redirect(Url.Content("~/Hoteles/MostrarInformacion"));
            }
        }

        // -------------------------------------------- ELIMINACIÓN DE HABITACIONES -----------------------------------------------

        // Método que maneja la eliminación de una habitación.
        [HttpGet]
        public ActionResult Delete(int IDHotel)
        {
            // Usa la base de datos para eliminar un hotel por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var hotelTO = db.Hoteles.Find(IDHotel);

                // Si el hotel no se encuentra, devuelve un error 404.
                if (hotelTO == null)
                {
                    return HttpNotFound();
                }

                // Elimina el hotel de la base de datos y guarda los cambios.
                db.Hoteles.Remove(hotelTO);
                db.SaveChanges();
            }

            // Redirige a la vista de mostrar información.
            return RedirectToAction("MostrarInformacion");
        }
    }
}
