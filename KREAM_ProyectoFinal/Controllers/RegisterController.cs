using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.Helpers;
using System.Security.Principal;
using System.Data.Entity.Validation;
using Microsoft.Ajax.Utilities;
using System.Web.Optimization;
using KREAM_ProyectoFinal.Models;

namespace PURIS_FLASH.Controllers
{
    [VerificarSesion]
    public class RegisterController : Controller
    {
        // GET: Register

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(UsersViewModel nuevaCuenta)
        {
            if (!ModelState.IsValid)
            {
                if (usuarioExisteCedula(nuevaCuenta.Cedula) == 1)
                {
                    TempData["Mensaje"] = "Cedula ya esta registrado en el sistema, por favor intente de nuevo";

                }
                else if (usuarioExisteEmail(nuevaCuenta.Correo) == 1)
                {
                    TempData["Mensaje"] = "Correo ya esta registrado en el sistema, por favor intente de nuevo";

                }
                else
                {
                    using (var db = new TRAVEL2Entities())
                    {
                        Users userTO = new Users();
                        userTO.Cedula = nuevaCuenta.Cedula;
                        userTO.Contrasena = nuevaCuenta.Contrasena;
                        userTO.Correo = nuevaCuenta.Correo;
                        userTO.Nombre = nuevaCuenta.Nombre;
                        userTO.PrimerApellido = nuevaCuenta.PrimerApellido;
                        userTO.SegundoApellido = nuevaCuenta.SegundoApellido;
                        userTO.Edad = nuevaCuenta.Edad;
                        userTO.Telefono = nuevaCuenta.Telefono;
                        userTO.Sexo = nuevaCuenta.Sexo;
                        userTO.Direccion = nuevaCuenta.Direccion;
                        userTO.TipoDeUsuario = 2;
                        userTO.PreguntaSeguridad = nuevaCuenta.PreguntaSeguridad;
                        userTO.RespuestaPreguntaSeguridad = nuevaCuenta.RespuestaPreguntaSeguridad;
                        db.Users.Add(userTO);
                        db.SaveChanges();
                    }
                    return Redirect("/Login/Login");   // Cambiar página de confirmación
                }
            }
            return View(nuevaCuenta);
        }


        public int usuarioExisteCedula(int cedula)
        {


            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserCedula @var_cedula",
                    new SqlParameter("var_cedula", cedula)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


                if (user != null)
                {

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


        public int usuarioExisteEmail(string email)
        {


            using (TRAVEL2Entities db = new TRAVEL2Entities())
            {
                var user = db.Database.SqlQuery<UsersViewModel>("EXEC VerifyUserGmail @var_Correo",
                    new SqlParameter("var_Correo", email)
                ).SingleOrDefault();


                SqlParameter outputParam = new SqlParameter
                {
                    ParameterName = "@Autenticado",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };


                if (user != null)
                {

                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }


    }




}