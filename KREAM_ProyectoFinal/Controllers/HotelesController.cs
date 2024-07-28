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
    [VerificarSesion]
    public class HotelesController : Controller
    {
        public ActionResult Index(string nombre, string habitacion)
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            List<HotelesTableViewModel> lstHoteles = new List<HotelesTableViewModel>();

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
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

                // Filtrar por nombre
                if (!string.IsNullOrEmpty(nombre))
                {
                    query = query.Where(h => h.NombreHotel.Contains(nombre));
                }
                ViewBag.NombreHotelSeleccionado = nombre;

                // Filtrar por habitacion
                if (!string.IsNullOrEmpty(habitacion))
                {
                    query = query.Where(h => h.TipoDeHabitacion.Contains(habitacion));
                }

                // Obtener habitaciones distintas de la base de datos
                var habitaciones = db.Hoteles.Select(h => h.TipoDeHabitacion).Distinct().ToList();
                ViewBag.Categoria = new SelectList(habitaciones);

                // Esta línea es redundante ya que el filtro ya se aplicó
                // if (!string.IsNullOrEmpty(habitacion))
                // {
                //     query = query.Where(h => h.TipoDeHabitacion == habitacion);
                // }

                ViewBag.tipoDeHabitacionSeleccionada = habitacion;
                lstHoteles = query.ToList();
            }
            return View(lstHoteles);
        }






        // ------------------------------------------- MENU PRINCIPAL DE ADMIN HABITACIONES / MOSTRAR INFORMACION ----------------------------------------------------------------

        // ESTE LO QUE HACE ES SOLO JALAR TODAS LAS HABITACIONES PARA QUE EL ADMINISTRADOR PUEDA ELIMINAR, EDITAR O CREAR UNA NUEVA HABITACION 


        //---------------------------------------------- MOSTRAR LAS HABITACIONES  --------------------------------------------------  
        public ActionResult MostrarInformacion(string tipoDeHabitacion, int? cantidadDePersonas, int? Telefono, int? calificacionHotel, string web, string NombreHotel)
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            ViewBag.UsuarioActual = usuarioActual.Nombre;
            ViewBag.SexoUsuario = usuarioActual.Sexo;
            ViewBag.TipoUsuario = usuarioActual.TipoDeUsuario;

            List<HotelesTableViewModel> lstHoteles = new List<HotelesTableViewModel>();

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
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

                // Filtrar por nombre
                if (!string.IsNullOrEmpty(NombreHotel))
                {
                    query = query.Where(p => p.NombreHotel.Contains(NombreHotel));
                }

                

                // Filtrar por cantidad de personas 
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


                ViewBag.PersonasSeleccionada = cantidadDePersonas;
                ViewBag.tipoDeHabitacion = tipoDeHabitacion;
              
                ViewBag.NombreSeleccionado = NombreHotel;
               
                lstHoteles = query.ToList();
            }
            return View(lstHoteles);
        }

        
        //---------------------------------------------- FUNCIONALIDAD DEL BOTON ADD HABITACIONES / ACCION DE BOTON  -------------------------------------------------- 
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(HotelesTableViewModel model, HttpPostedFileBase ImagenFile, HttpPostedFileBase ImagenFile2, HttpPostedFileBase ImagenFile3)
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

                // Crear una nueva instancia de la entidad Hoteles
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
                    //CheckOut = model.CheckOut,
                    ComentarioHotel = model.ComentarioHotel,
                    CalificacionHotel = model.CalificacionHotel

                };

                db.Hoteles.Add(hotelTO);
                db.SaveChanges();

                return Redirect(Url.Content("~/Hoteles/MostrarInformacion")); // Corregir la redirección aquí
            }
        }


        // --------------------------------------------  EDIT  -----------------------------------------------

        [HttpGet]
        public ActionResult Edit(int IDHotel)
        {
            using (var db = new TRAVEL2Entities())
            {
                var hotel = db.Hoteles.Find(IDHotel); // El ProductID viene como parametro para poder buscar el Producto

                ViewBag.HotelNombre = hotel.NombreHotel; // SALVAMOS EL NOMBRE DEL PRODUCTO EN UN VIEWBAG PARA USARLO LUEGO EN LA VISTA DE EDIT
                                
                if (hotel == null)
                {
                    return RedirectToAction("MostrarInformacion");
                }

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

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Edit(HotelesTableViewModel model, HttpPostedFileBase NuevaImagen, HttpPostedFileBase NuevaImagen2, HttpPostedFileBase NuevaImagen3, string action) //
        {
            if (!ModelState.IsValid) return View(model);


            using (var db = new TRAVEL2Entities())
            {

                // DECLARANDO LA SESION ACTIVA AL USUARIO ACTUAL PARA PASAR POR VIEWBAGS
                var usuario = Session["UsuarioActual"] as UsersViewModel;
                               
                var hotelTO = db.Hoteles.Find(model.IDHotel);

                hotelTO.NombreHotel = model.NombreHotel;
                hotelTO.TipoDeHabitacion = model.TipoDeHabitacion;
                hotelTO.CantidadDePersonas = model.CantidadDePersonas;
                hotelTO.Telefono = model.Telefono;
                hotelTO.Descripcion = model.Descripcion;

                // IMAGENES  -- Esto tambien aplica para eliminarlas o agregar otra 

                // Actualiza las propiedades de las imágenes según la acción del usuario
                if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    // El usuario ha proporcionado una nueva imagen
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        hotelTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        hotelTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                if (action == "EliminarImagen2" || model.EliminarImagen2)
                {
                    hotelTO.Imagen2 = null;

                }

                else if (NuevaImagen2 != null && NuevaImagen2.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen2.InputStream))
                    {
                        hotelTO.Imagen2 = binaryReader.ReadBytes(NuevaImagen2.ContentLength);
                    }
                }

                // Actualiza las propiedades de las imágenes según la acción del usuario para Imagen3
                if (action == "EliminarImagen3" || model.EliminarImagen3)
                {
                    hotelTO.Imagen3 = null;
                }
                else if (NuevaImagen3 != null && NuevaImagen3.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(NuevaImagen3.InputStream))
                    {
                        hotelTO.Imagen3 = binaryReader.ReadBytes(NuevaImagen3.ContentLength);
                    }
                }

                if (action == "EliminarImagen" || model.EliminarImagen)
                {
                    hotelTO.Imagen = null;
                }
                else if (NuevaImagen != null && NuevaImagen.ContentLength > 0)
                {
                    // El usuario ha proporcionado una nueva imagen
                    using (var binaryReader = new BinaryReader(NuevaImagen.InputStream))
                    {
                        hotelTO.Imagen = binaryReader.ReadBytes(NuevaImagen.ContentLength);
                    }
                }

                db.SaveChanges();

                return Redirect(Url.Content("~/Hoteles/MostrarInformacion"));
            }
        }




        // --------------------------------------------  DELETE  -----------------------------------------------
        [HttpGet]
        public ActionResult Delete(int IDHotel)
        {
            using (var db = new TRAVEL2Entities())
            {
                var usuario = Session["UsuarioActual"] as UsersViewModel;

                var hotelTO = db.Hoteles.Find(IDHotel);

                if (hotelTO == null)
                {
                    return HttpNotFound();
                }

                db.Hoteles.Remove(hotelTO);
                db.SaveChanges();
            }

            return RedirectToAction("MostrarInformacion");
        }

    }
}
