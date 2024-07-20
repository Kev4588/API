using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KREAM_ProyectoFinal.Models.ViewModel
{
    public class UsersViewModel
    {
        public int Id { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Ingrese una cedula valida.")]
        public int Cedula { get; set; }

        public string Nombre { get; set; }
        public string PrimerApellido { get; set; }
        public string SegundoApellido { get; set; }

        [Range(18, 100, ErrorMessage = "Por favor ingrese una edad valida.")]
        public int Edad { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Ingrese un numero de telefono valido.")]
        public int Telefono { get; set; }

        public string Correo { get; set; }
        public string Sexo { get; set; }
        public string Direccion { get; set; }
        public int TipoDeUsuario { get; set; }
        public Nullable<int> Calificacion { get; set; }

        [Required]
        [Display(Name = "Contrasena")]
        [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d!@#$%^&*]{8,16}$", ErrorMessage = "La contraseña debe tener al menos 8 caracteres e incluir al menos un dígito, una letra minúscula y una letra mayúscula.")]
        public string Contrasena { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarContrasena { get; set; }

        public bool VarAutenticado { get; set; }

        public string ProductosEnCarrito { get; set; }


        public string MarcaVendedor { get; set; }

        public string PreguntaSeguridad { get; set; }

        public string RespuestaPreguntaSeguridad { get; set; }

    }
    public class UserRecuperaContrasenaViewModel
    {
        [Display(Name = "ID:")]
        public int Id { get; set; }
        public string Correo { get; set; }

        [Required(ErrorMessage = "*Requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; }


        [Required(ErrorMessage = "*Requerido")]
        [Display(Name = "Confirmar Contraseña")]
        [DataType(DataType.Password)]
        [Compare("Contrasena", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmarContrasena { get; set; }




    }


}