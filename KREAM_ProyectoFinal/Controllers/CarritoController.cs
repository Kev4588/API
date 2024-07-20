using KREAM_ProyectoFinal.Models.TableViewModel;
using KREAM_ProyectoFinal.Models;
using KREAM_ProyectoFinal.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;

namespace KREAM_ProyectoFinal.Controllers
{
    [VerificarSesion]
    public class CarritoController : Controller
    {
        public ActionResult Index()
        {
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            if (usuarioActual != null && usuarioActual.ProductosEnCarrito != null && usuarioActual.ProductosEnCarrito.Any())
            {
                var productosEnCarritoIds = usuarioActual.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();

                var productosConCantidad = new List<Tuple<int, int>>();

                foreach (var productoId in productosEnCarritoIds)
                {
                    Tuple<int, int> productoEnLista = null;
                    foreach (var tupla in productosConCantidad)
                    {
                        if (tupla.Item1 == productoId)
                        {
                            productoEnLista = tupla;
                            break;
                        }
                    }

                    if (productoEnLista != null)
                    {
                        productosConCantidad.Remove(productoEnLista);
                        productosConCantidad.Add(new Tuple<int, int>(productoId, productoEnLista.Item2 + 1));
                    }
                    else
                    {
                        productosConCantidad.Add(new Tuple<int, int>(productoId, 1));
                    }
                }

                var detallesProductosEnCarrito = new List<ProductosTableViewModel>();

                decimal? totalGeneral = 0; // Inicializa el total general

                using (var db = new TRAVEL2Entities())
                {
                    foreach (var productoConCantidad in productosConCantidad)
                    {
                        var producto = db.Productos.Find(productoConCantidad.Item1);

                        if (producto != null)
                        {
                            var detalleProducto = new ProductosTableViewModel
                            {
                                ProductoID = producto.ProductoID,
                                Nombre = producto.Nombre,
                                Lugar = producto.Lugar,
                                Descripcion = producto.Descripcion,
                                Precio = (decimal)producto.Precio,
                                Categoria = producto.Categoria,
                                Personas = producto.Personas,
                                CantidadEnStock = producto.CantidadEnStock,                                
                                Proveedor = producto.Proveedor,
                                Imagen = producto.Imagen,
                                Imagen2 = producto.Imagen2,
                                Imagen3 = producto.Imagen3,
                                Cantidad = productoConCantidad.Item2
                            };

                            detallesProductosEnCarrito.Add(detalleProducto);

                            // Suma el total multiplicando el precio por la cantidad
                            totalGeneral += detalleProducto.Precio * detalleProducto.Cantidad;
                        }
                    }
                }

                ViewBag.DetallesProductosEnCarrito = detallesProductosEnCarrito;
                ViewBag.TotalGeneral = totalGeneral; // Asigna el total general a ViewBag
            }
            else
            {
                ViewBag.DetallesProductosEnCarrito = new List<ProductosTableViewModel>();
                ViewBag.TotalGeneral = 0; // Si no hay productos, el total general es cero
            }

            return View();
        }


        public ActionResult PasarelaPago()
        {


            return View();
        }



        [HttpPost]
        public ActionResult RealizarCompra()
        {
            // Obtener el usuario actual desde la sesión
            var usuario = Session["UsuarioActual"] as UsersViewModel;

            // Verificar si el usuario tiene productos en el carrito
            if (!string.IsNullOrEmpty(usuario?.ProductosEnCarrito))
            {
                using (var db = new TRAVEL2Entities())
                {
                    // Obtener la lista de IDs y cantidades de productos en el carrito
                    var productosEnCarrito = usuario.ProductosEnCarrito.Split(',')
                .Select(item => item.Split(':'))
                .Where(parts => parts.Length == 2) // Verificar que haya al menos dos partes
                .Select(parts => new { ProductoId = int.Parse(parts[0]), Cantidad = int.Parse(parts[1]) })
                .ToList();

                    // Recorrer la lista de productos en el carrito y actualizar las existencias en la base de datos
                    foreach (var item in productosEnCarrito)
                    {
                        // Obtener el producto desde la base de datos
                        var producto = db.Productos.Find(item.ProductoId);

                        if (producto != null)
                        {
                            // Restar la cantidad comprada de las existencias
                            producto.CantidadEnStock -= item.Cantidad;

                            // Aquí puedes agregar lógica adicional si lo necesitas

                            // Guardar los cambios en la base de datos
                            db.SaveChanges();
                        }
                    }

                    // Actualizar el atributo ProductosEnCarrito a null en la base de datos
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);
                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = null;
                        db.SaveChanges();
                    }
                }

                // Actualizar el carrito en la sesión
                usuario.ProductosEnCarrito = null;
                Session["UsuarioActual"] = usuario;

                // Redirigir a la página principal con el mensaje de compra realizada
                TempData["CompraRealizada"] = "Compra realizada con éxito";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // El usuario no tiene productos en el carrito
                TempData["CompraFallida"] = "No hay productos en el carrito";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult CompraRealizada()
        {
            // Obtener el usuario actual desde la sesión
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Verificar si el usuario y su carrito no son nulos o vacíos
            if (usuarioActual != null && !string.IsNullOrEmpty(usuarioActual.ProductosEnCarrito))
            {
                var productosEnCarritoIds = usuarioActual.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();

                var productosConCantidad = new List<Tuple<int, int>>();

                foreach (var productoId in productosEnCarritoIds)
                {
                    Tuple<int, int> productoEnLista = null;
                    foreach (var tupla in productosConCantidad)
                    {
                        if (tupla.Item1 == productoId)
                        {
                            productoEnLista = tupla;
                            break;
                        }
                    }

                    if (productoEnLista != null)
                    {
                        productosConCantidad.Remove(productoEnLista);
                        productosConCantidad.Add(new Tuple<int, int>(productoId, productoEnLista.Item2 + 1));
                    }
                    else
                    {
                        productosConCantidad.Add(new Tuple<int, int>(productoId, 1));
                    }
                }

                using (var db = new TRAVEL2Entities())
                {
                    foreach (var productoConCantidad in productosConCantidad)
                    {
                        var producto = db.Productos.Find(productoConCantidad.Item1);

                        if (producto != null)
                        {
                            // Restar la cantidad comprada de las existencias
                            producto.CantidadEnStock -= productoConCantidad.Item2;

                            // Otras acciones o lógica adicional si es necesario


                        }
                    }

                    // Limpia el carrito del usuario después de actualizar la base de datos
                    usuarioActual.ProductosEnCarrito = null;
                    Session["UsuarioActual"] = usuarioActual; // Asegúrate de que esta asignación funcione correctamente 
                    // Guardar los cambios en la base de datos
                    db.SaveChanges();



                    // Actualizar el atributo ProductosEnCarrito a null en la base de datos
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuarioActual.Id);
                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = null;
                        db.SaveChanges();
                    }
                }



                TempData["CompraRealizada"] = "Compra realizada con éxito";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["CompraFallida"] = "No hay productos en el carrito";
                return RedirectToAction("Index", "Home");
            }
        }


        [HttpPost]
        public ActionResult QuitarProducto(int productoId)
        {
            var usuario = Session["UsuarioActual"] as UsersViewModel;

            if (usuario != null)
            {
                // Elimina el producto del carrito en la sesión
                var productosEnCarritoIds = usuario.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();
                productosEnCarritoIds.Remove(productoId);

                // Actualiza la sesión solo si quedan productos en el carrito
                if (productosEnCarritoIds.Count > 0)
                {
                    usuario.ProductosEnCarrito = string.Join(",", productosEnCarritoIds);
                }
                else
                {
                    // Si la lista es vacía, asigna null
                    usuario.ProductosEnCarrito = null;
                }

                // Actualiza la sesión
                Session["UsuarioActual"] = usuario;

                using (var db = new TRAVEL2Entities())
                {
                    // Obtén el usuario desde la base de datos y actualiza su carrito
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);

                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = usuario.ProductosEnCarrito;
                        db.SaveChanges();
                    }
                }
            }

            // No redirijas a ninguna acción específica, simplemente refresca la página actual
            return RedirectToAction("Index", "Carrito");
        }




    }
}
