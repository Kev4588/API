using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KREAM_ProyectoFinal.Models.ViewModel
{
    public class HotelesViewModel
    {
        public int IDHotel { get; set; }
        public string NombreHotel { get; set; }
        public string TipoDeHabitacion { get; set; }
        public string web { get; set; }
        public string Descripcion { get; set; }
        public string Descipcion { get; set; }
        public Nullable<int> CantidadDePersonas { get; set; }

        public string ComentarioHotel { get; set; }

        public Nullable<int> CalificacionHotel { get; set; }

        public decimal Precio { get; set; }
        public byte[] Imagen { get; set; }
        public byte[] Imagen2 { get; set; }
        public byte[] Imagen3 { get; set; }
    }
}