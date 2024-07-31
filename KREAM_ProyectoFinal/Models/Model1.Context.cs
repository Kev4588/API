﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KREAM_ProyectoFinal.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class TRAVEL2Entities : DbContext
    {
        public TRAVEL2Entities()
            : base("name=TRAVEL2Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Comentarios> Comentarios { get; set; }
        public virtual DbSet<Hoteles> Hoteles { get; set; }
        public virtual DbSet<Productos> Productos { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UserType> UserType { get; set; }
    
        public virtual int DeleteUserbyId(Nullable<int> var_Id)
        {
            var var_IdParameter = var_Id.HasValue ?
                new ObjectParameter("var_Id", var_Id) :
                new ObjectParameter("var_Id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("DeleteUserbyId", var_IdParameter);
        }
    
        public virtual int FilterProduct(string var_genero, string var_color, string var_marca)
        {
            var var_generoParameter = var_genero != null ?
                new ObjectParameter("var_genero", var_genero) :
                new ObjectParameter("var_genero", typeof(string));
    
            var var_colorParameter = var_color != null ?
                new ObjectParameter("var_color", var_color) :
                new ObjectParameter("var_color", typeof(string));
    
            var var_marcaParameter = var_marca != null ?
                new ObjectParameter("var_marca", var_marca) :
                new ObjectParameter("var_marca", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("FilterProduct", var_generoParameter, var_colorParameter, var_marcaParameter);
        }
    
        public virtual int GuardarComentarios(Nullable<int> idProducto, string comentario)
        {
            var idProductoParameter = idProducto.HasValue ?
                new ObjectParameter("IdProducto", idProducto) :
                new ObjectParameter("IdProducto", typeof(int));
    
            var comentarioParameter = comentario != null ?
                new ObjectParameter("Comentario", comentario) :
                new ObjectParameter("Comentario", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("GuardarComentarios", idProductoParameter, comentarioParameter);
        }
    
        public virtual int obtenerComentariosPorProducto(Nullable<int> idProducto)
        {
            var idProductoParameter = idProducto.HasValue ?
                new ObjectParameter("IdProducto", idProducto) :
                new ObjectParameter("IdProducto", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("obtenerComentariosPorProducto", idProductoParameter);
        }
    
        public virtual ObjectResult<string> ShowProducts()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("ShowProducts");
        }
    
        public virtual int UpdateUser(Nullable<int> id, Nullable<int> cedula, string nombre, string primerApellido, string segundoApellido, Nullable<int> edad, string telefono, string correo, string direccion, string sexo)
        {
            var idParameter = id.HasValue ?
                new ObjectParameter("Id", id) :
                new ObjectParameter("Id", typeof(int));
    
            var cedulaParameter = cedula.HasValue ?
                new ObjectParameter("Cedula", cedula) :
                new ObjectParameter("Cedula", typeof(int));
    
            var nombreParameter = nombre != null ?
                new ObjectParameter("Nombre", nombre) :
                new ObjectParameter("Nombre", typeof(string));
    
            var primerApellidoParameter = primerApellido != null ?
                new ObjectParameter("PrimerApellido", primerApellido) :
                new ObjectParameter("PrimerApellido", typeof(string));
    
            var segundoApellidoParameter = segundoApellido != null ?
                new ObjectParameter("SegundoApellido", segundoApellido) :
                new ObjectParameter("SegundoApellido", typeof(string));
    
            var edadParameter = edad.HasValue ?
                new ObjectParameter("Edad", edad) :
                new ObjectParameter("Edad", typeof(int));
    
            var telefonoParameter = telefono != null ?
                new ObjectParameter("Telefono", telefono) :
                new ObjectParameter("Telefono", typeof(string));
    
            var correoParameter = correo != null ?
                new ObjectParameter("Correo", correo) :
                new ObjectParameter("Correo", typeof(string));
    
            var direccionParameter = direccion != null ?
                new ObjectParameter("Direccion", direccion) :
                new ObjectParameter("Direccion", typeof(string));
    
            var sexoParameter = sexo != null ?
                new ObjectParameter("Sexo", sexo) :
                new ObjectParameter("Sexo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateUser", idParameter, cedulaParameter, nombreParameter, primerApellidoParameter, segundoApellidoParameter, edadParameter, telefonoParameter, correoParameter, direccionParameter, sexoParameter);
        }
    
        public virtual int VerifyPassword(string cedula, string contrasena, ObjectParameter autenticado)
        {
            var cedulaParameter = cedula != null ?
                new ObjectParameter("Cedula", cedula) :
                new ObjectParameter("Cedula", typeof(string));
    
            var contrasenaParameter = contrasena != null ?
                new ObjectParameter("Contrasena", contrasena) :
                new ObjectParameter("Contrasena", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("VerifyPassword", cedulaParameter, contrasenaParameter, autenticado);
        }
    
        public virtual ObjectResult<VerifyUser_Result> VerifyUser(string var_cedula, string var_contrasena)
        {
            var var_cedulaParameter = var_cedula != null ?
                new ObjectParameter("var_cedula", var_cedula) :
                new ObjectParameter("var_cedula", typeof(string));
    
            var var_contrasenaParameter = var_contrasena != null ?
                new ObjectParameter("var_contrasena", var_contrasena) :
                new ObjectParameter("var_contrasena", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VerifyUser_Result>("VerifyUser", var_cedulaParameter, var_contrasenaParameter);
        }
    
        public virtual ObjectResult<VerifyUserCedula_Result> VerifyUserCedula(string var_cedula)
        {
            var var_cedulaParameter = var_cedula != null ?
                new ObjectParameter("var_cedula", var_cedula) :
                new ObjectParameter("var_cedula", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VerifyUserCedula_Result>("VerifyUserCedula", var_cedulaParameter);
        }
    
        public virtual ObjectResult<VerifyUserGmail_Result> VerifyUserGmail(string var_correo)
        {
            var var_correoParameter = var_correo != null ?
                new ObjectParameter("var_correo", var_correo) :
                new ObjectParameter("var_correo", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VerifyUserGmail_Result>("VerifyUserGmail", var_correoParameter);
        }
    
        public virtual ObjectResult<VerifyUserSocialMedia_Result> VerifyUserSocialMedia(string var_socialMediaID)
        {
            var var_socialMediaIDParameter = var_socialMediaID != null ?
                new ObjectParameter("var_socialMediaID", var_socialMediaID) :
                new ObjectParameter("var_socialMediaID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<VerifyUserSocialMedia_Result>("VerifyUserSocialMedia", var_socialMediaIDParameter);
        }
    }
}
