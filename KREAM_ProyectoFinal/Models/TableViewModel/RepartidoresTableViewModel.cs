using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KREAM_ProyectoFinal.Models.TableViewModel
{
    public class RepartidoresTableViewModel
    {
        public int IDRepartidor { get; set; }

        public string TipoDeTransporte { get; set; }

        public string Descripcion { get; set; }

        public int Telefono { get; set; }

        public string NombreRepartidor { get; set; }

        public byte[] Imagen { get; set; }

        public byte[] Imagen2 { get; set; }

        public byte[] Imagen3 { get; set; }
        public bool EliminarImagen3 { get; internal set; }
        public bool EliminarImagen { get; internal set; }
        public bool EliminarImagen2 { get; internal set; }
    }
}