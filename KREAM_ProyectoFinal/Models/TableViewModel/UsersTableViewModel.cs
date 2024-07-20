using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KREAM_ProyectoFinal.Models.TableViewModel
{
    public class UsersTableViewModel
    {
        //Agrego lo de la entidad a esta clase
        public int Id { get; set; }

        [RegularExpression(@"^\d{9}$", ErrorMessage = "La cédula debe contener exactamente 9 dígitos.")]
        public int Cedula { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Por favor introduzca solo letras")]
        public string Nombre { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Por favor introduzca solo letras")]
        [Display(Name = "Primer Apellido")]
        public string PrimerApellido { get; set; }

        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Por favor introduzca solo letras")]
        [Display(Name = "Segundo Apellido")]
        public string SegundoApellido { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Por favor introduzca solo números")]
        public int Edad { get; set; }

        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Por favor introduzca solo números")]
        public int Telefono { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Por favor, introduzca una dirección de correo válida")]
        public string Correo { get; set; }

        [Display(Name = "Genero")]
        public string Sexo { get; set; }
        public string Direccion { get; set; }

        [Display(Name = "Tipo de Usuario")]
        public int TipoDeUsuario { get; set; }


        public string StringTipoDeUsuario { get; set; }

        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }

        public string ProductosEnCarrito { get; set; }

        public string MarcaVendedor { get; set; }

    }
}
