//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KREAM_ProyectoFinal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Comentarios
    {
        public int Id { get; set; }
        public int IdProducto { get; set; }
        public string Comentario { get; set; }
    
        public virtual Productos Productos { get; set; }
    }
}
