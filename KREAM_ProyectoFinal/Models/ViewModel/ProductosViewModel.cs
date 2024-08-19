using KREAM_ProyectoFinal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PURIS_FLASH.Models;

namespace PURIS_FLASH.Models.ViewModel
{
    public class ProductosViewModel
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; }
        public string Lugar { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
        public string Personas { get; set; }
        public Nullable<int> CantidadEnStock { get; set; }
        public string Comentario { get; set; }
        public int Calificacion { get; set; }
        public string Proveedor { get; set; }
        public Nullable<int> Vendedor { get; set; }
        public byte[] Imagen { get; set; }
        public byte[] Imagen2 { get; set; }
        public byte[] Imagen3 { get; set; }
    }
}