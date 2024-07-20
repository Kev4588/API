using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KREAM_ProyectoFinal.Models.TableViewModel
{
    public class ProductosTableViewModel
    {
        // El ID se asigna solo no hace falta regex aqui 
        public int ProductoID { get; set; }

        // Usamos Regex para validar las cosas desde este modelo nos ahorra codigo 
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [RegularExpression(@"^[a-zA-Z0-9\s¡!@#$%^&*()_+\-=\[\]{};':"",.<>/?]*$", ErrorMessage = "El nombre solo debe contener letras, números y espacios, y puede incluir caracteres especiales.")]
        
        public string Nombre { get; set; }

        
        
        public string Lugar { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$|^\d+(\,\d{1,2})?$", ErrorMessage = "El formato del precio no es válido.")]
        [Range(0, 9999999.99, ErrorMessage = "El precio debe estar entre 0 y 9999999.99")]
        
        public decimal Precio { get; set; }

        [RegularExpression(@"^[a-zA-Z\s]*$", ErrorMessage = "La categoría solo debe contener letras y espacios.")]
        public string Categoria { get; set; }

        
        public string Personas { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La cantidad en stock debe ser un número entero.")]
        public Nullable<int> CantidadEnStock { get; set; }

        public string Comentario { get; set; }

        [Required(ErrorMessage = "La calificación es obligatoria.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "La calificación debe ser un número entero.")]
        public int Calificacion { get; set; }

        public string Proveedor { get; set; }

        public byte[] Imagen { get; set;}

        public byte[] Imagen2 { get; set; }

        public byte[] Imagen3 { get; set; }

        // Nuevas propiedades para indicar si se deben eliminar las imágenes
        public bool EliminarImagen { get; set; }
        public bool EliminarImagen2 { get; set; }
        public bool EliminarImagen3 { get; set; }


        public List<Comentarios> Comentarios { get; set; }


        // COSAS NECESARIAS PARA CARRITO 


        public int Cantidad { get; set; }

        // Calcula el total multiplicando el precio por la cantidad
        public decimal Total
        {
            get { return Precio * Cantidad; }
        }
    }
}