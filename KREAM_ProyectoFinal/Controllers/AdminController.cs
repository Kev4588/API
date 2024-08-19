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
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class AdminController : Controller
    {
        // Método que maneja la vista principal de administración, mostrando una lista de usuarios.
        public ActionResult Index()
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;
            ViewBag.UsuarioActual = usuarioActual.Nombre;

            // Inicializa una lista de usuarios.
            List<UsersTableViewModel> lstUsers = null;

            // Usa la base de datos para obtener y listar los usuarios.
            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                // Realiza una consulta para obtener usuarios, uniéndolos con el tipo de usuario (UserType).
                lstUsers = (from u in db.Users
                            join ut in db.UserType on u.TipoDeUsuario equals ut.Id // Realiza el join con UserType
                            where u.TipoDeUsuario > 1 // Filtra para mostrar solo usuarios cuyo tipo es mayor que 1.
                            orderby u.TipoDeUsuario // Ordena por tipo de usuario.
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
                                StringTipoDeUsuario = ut.UserStatus, // Mapea el estado del usuario (UserType) a un string.
                            }).ToList();
            }
            // Devuelve la vista con la lista de usuarios.
            return View(lstUsers);
        }

        // Método que muestra la vista para agregar un nuevo usuario.
        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        // Método que maneja la lógica de agregar un nuevo usuario.
        [HttpPost]
        public ActionResult Add(UsersTableViewModel model)
        {
            // Si el modelo no es válido, devuelve la vista con el modelo actual.
            if (!ModelState.IsValid) return View(model);

            try
            {
                // Usa la base de datos para agregar el nuevo usuario.
                using (var db = new TRAVEL2Entities())
                {
                    // Crea un nuevo objeto de usuario con los datos proporcionados en el modelo.
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
                        TipoDeUsuario = 2, // Se crean los usuarios con tipo comprador (tipo 2).
                        Contrasena = model.Contrasena,
                    };

                    // Agrega el usuario a la base de datos y guarda los cambios.
                    db.Users.Add(usersTO);
                    db.SaveChanges();

                    // Redirige a la vista principal de administración.
                    return Redirect(Url.Content("~/Admin/Index"));
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Captura y maneja cualquier error de validación en la entidad.
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        // Escribe los errores en la consola de depuración.
                        System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
                }
                throw; // Lanza nuevamente la excepción para su manejo.
            }
        }

        // Método que muestra la vista para editar un usuario específico.
        [HttpGet]
        public ActionResult Edit(int Id)
        {
            UsersViewModel model = new UsersViewModel();
            // Usa la base de datos para encontrar el usuario por su ID.
            using (var db = new TRAVEL2Entities())
            {
                var userTO = db.Users.Find(Id);

                // Mapea los datos del usuario al modelo de vista.
                model.Cedula = userTO.Cedula;
                model.Nombre = userTO.Nombre;
                model.PrimerApellido = userTO.PrimerApellido;
                model.SegundoApellido = userTO.SegundoApellido;
                model.Edad = (int)userTO.Edad;
                model.Telefono = (int)userTO.Telefono;
                model.Correo = userTO.Correo;
                model.Sexo = userTO.Sexo;
                model.Direccion = userTO.Direccion;

                // Almacena el nombre completo del usuario en un ViewBag para su uso en la vista.
                // Esta linea es totalmente necesaria para que en la parte de EDIT , muestre en el titulo a quien esta editando
                // estos viewbags estan dentro de este catch up de arreglo para tomar los datos de la persona que coinciden con 
                // el ID , una vez eso ya tenemos el nombre , apellido etc de esa persona del ID de quien se le dio click en Edit
                ViewBag.Nombre = userTO.Nombre + " " + userTO.PrimerApellido;
            }
            // Devuelve la vista con el modelo de edición.
            return View(model);
        }

        // Método que maneja la lógica para editar un usuario.
        [HttpPost]
        public ActionResult Edit(UsersTableViewModel model)
        {
            //if (!ModelState.IsValid) return View(model);

            // Usa la base de datos para actualizar los datos del usuario.
            using (var db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para actualizar el usuario en la base de datos.
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

                // Actualiza el ViewBag con el nombre del usuario actual.
                ViewBag.UsuarioActual = model.Nombre;

                // Redirige a la vista principal de administración.
                return RedirectToAction("Index", "Admin");
            }
        }

        // Método que maneja la eliminación de un usuario.
        [HttpPost]
        public ActionResult Delete(int Id)
        {
            // Usa la base de datos para eliminar el usuario por su ID.
            using (var db = new TRAVEL2Entities())
            {
                // Ejecuta un procedimiento almacenado para eliminar el usuario de la base de datos.
                db.Database.ExecuteSqlCommand("EXEC DeleteUserbyId @var_Id", new SqlParameter("@var_Id", Id));
            }
            // Devuelve un código de estado 200 indicando éxito.
            return Content("200");
        }
    }
}
