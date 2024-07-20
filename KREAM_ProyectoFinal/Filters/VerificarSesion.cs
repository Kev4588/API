using System.Web.Mvc;
using KREAM_ProyectoFinal.Controllers;
using KREAM_ProyectoFinal.Models.ViewModel;

public class VerificarSesion : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        // Verifica si el usuario está autenticado
        var usuarioActual = filterContext.HttpContext.Session["UsuarioActual"] as UsersViewModel;

        // Si el usuario no está autenticado y no está en la página de login, redirige a la página de login
        if (usuarioActual == null && !(filterContext.Controller is LoginController) && !(filterContext.Controller is RegisterController))
        {
            filterContext.Result = new RedirectResult("~/Login/Index");
            return;
        }


        base.OnActionExecuting(filterContext);
    }
}
