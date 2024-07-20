using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KREAM_ProyectoFinal.Models.TableViewModel
{
    public class HotelesTableViewModel
    {
        // El ID se asigna solo no hace falta regex aqui 
        public int IDHotel { get; set; }

        
        public string NombreHotel { get; set; }

        
        public string web { get; set; }

        
        public decimal Precio { get; set; }

        
        public int CalificacionHotel { get; set; }
        


        public int CantidadDePersonas { get; set; }

        
        

        public string TipoDeHabitacion { get; set; }



       public string Descripcion { get; set; }
        public string ComentarioHotel { get; set; }
       


        public byte[] Imagen { get; set; }

        public byte[] Imagen2 { get; set; }

        public byte[] Imagen3 { get; set; }

        // Nuevas propiedades para indicar si se deben eliminar las imágenes
        public bool EliminarImagen { get; set; }
        public bool EliminarImagen2 { get; set; }
        public bool EliminarImagen3 { get; set; }





        // COSAS NECESARIAS PARA CARRITO 


        public int Cantidad { get; set; }

        // Calcula el total multiplicando el precio por la cantidad
        public decimal Total
        {
            get { return Precio * Cantidad; }
        }
    }
}