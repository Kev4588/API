using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models.ViewModel;
using PURIS_FLASH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using KREAM_ProyectoFinal.Models;

namespace PURIS_FLASH.Controllers
{
    [VerificarSesion]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;
            ViewBag.UsuarioActual = usuarioActual.Nombre;

            List<UsersTableViewModel> lstUsers = null;

            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                lstUsers = (from u in db.Users
                            join ut in db.UserType on u.TipoDeUsuario equals ut.Id // Realiza el join con UserType
                            where u.TipoDeUsuario > 1
                            orderby u.TipoDeUsuario
                            select new UsersTableViewModel
                            {
                                Cedula = u.Cedula,
                                Id = u.Id,
                                Nombre = u.Nombre,
                                PrimerApellido = u.PrimerApellido,
                                SegundoApellido = u.SegundoApellido,
                                Edad = (int)u.Edad,
                                Telefono = (int)u.Telefono,
                                Correo = u.Correo,
                                Sexo = u.Sexo,
                                Direccion = u.Direccion,
                                StringTipoDeUsuario = ut.UserStatus,
                            }).ToList();
            }
            return View(lstUsers);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(UsersTableViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                using (var db = new TRAVEL2Entities())
                {
                    var usuario = Session["UsuarioActual"] as UsersViewModel;

                    Users usersTO = new Users
                    {
                        Cedula = model.Cedula,
                        Nombre = model.Nombre,
                        PrimerApellido = model.PrimerApellido,
                        SegundoApellido = model.SegundoApellido,
                        Edad = model.Edad,
                        Telefono = model.Telefono,
                        Correo = model.Correo,
                        Sexo = model.Sexo,
                        Direccion = model.Direccion,
                        TipoDeUsuario = 2, //Se crean los usuarios de tipo comprador
                        Contrasena = model.Contrasena,
                    };

                    db.Users.Add(usersTO);
                    db.SaveChanges();

                    return Redirect(Url.Content("~/Admin/Index"));
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            UsersViewModel model = new UsersViewModel();
            using (var db = new TRAVEL2Entities())
            {
                var userTO = db.Users.Find(Id);

                model.Cedula = userTO.Cedula;
                model.Nombre = userTO.Nombre;
                model.PrimerApellido = userTO.PrimerApellido;
                model.SegundoApellido = userTO.SegundoApellido;
                model.Edad = (int)userTO.Edad;
                model.Telefono = (int)userTO.Telefono;
                model.Correo = userTO.Correo;
                model.Direccion = userTO.Direccion;


                // Esta linea es totalmente necesaria para que en la parte de EDIT , muestre en el titulo a quien esta editando
                // estos viewbags estan dentro de este catch up de arreglo para tomar los datos de la persona que coinciden con 
                // el ID , una vez eso ya tenemos el nombre , apellido etc de esa persona del ID de quien se le dio click en Edit
                ViewBag.Nombre = userTO.Nombre + " " + userTO.PrimerApellido;
                
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(UsersTableViewModel model)
        {
            //if (!ModelState.IsValid) return View(model);

            using (var db = new TRAVEL2Entities())
            {
                var usuario = Session["UsuarioActual"] as UsersTableViewModel;
               

                //ViewBag.UsuarioActual = usuario.Nombre;

                db.Database.ExecuteSqlCommand(
                    "EXEC UpdateUser @Id, @Cedula, @Nombre, @PrimerApellido, @SegundoApellido, @Edad, @Telefono, @Correo, @Direccion,@Sexo",
                    new SqlParameter("@Id", model.Id),
                    new SqlParameter("@Cedula", model.Cedula),
                    new SqlParameter("@Nombre", model.Nombre),
                    new SqlParameter("@PrimerApellido", model.PrimerApellido),
                    new SqlParameter("@SegundoApellido", model.SegundoApellido),
                    new SqlParameter("@Edad", model.Edad),
                    new SqlParameter("@Telefono", model.Telefono),
                    new SqlParameter("@Correo", model.Correo),
                    new SqlParameter("@Direccion", model.Direccion),
                    new SqlParameter("@Sexo", model.Sexo)


                );

                ViewBag.UsuarioActual = model.Nombre;


                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            using (var db = new TRAVEL2Entities())
            {
                db.Database.ExecuteSqlCommand("EXEC DeleteUserbyId @var_Id", new SqlParameter("@var_Id", Id));
            }
            return Content("200");
        }

    }
}

