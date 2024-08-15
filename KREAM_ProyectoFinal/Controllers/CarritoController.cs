using PURIS_FLASH.Models.TableViewModel;
using PURIS_FLASH.Models;
using PURIS_FLASH.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System;
using KREAM_ProyectoFinal.Models;

namespace PURIS_FLASH.Controllers
{
    // Atributo que verifica si hay una sesión activa antes de permitir el acceso al controlador.
    [VerificarSesion]
    public class CarritoController : Controller
    {
        // Método que maneja la vista principal del carrito, mostrando los productos en el carrito del usuario.
        public ActionResult Index()
        {
            // Recupera el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Verifica si el usuario tiene productos en el carrito.
            if (usuarioActual != null && usuarioActual.ProductosEnCarrito != null && usuarioActual.ProductosEnCarrito.Any())
            {
                // Obtiene los IDs de los productos en el carrito y los convierte en una lista de enteros.
                var productosEnCarritoIds = usuarioActual.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();

                // Lista que almacenará los productos junto con sus cantidades.
                var productosConCantidad = new List<Tuple<int, int>>();

                // Recorre los IDs de productos en el carrito para contar las cantidades.
                foreach (var productoId in productosEnCarritoIds)
                {
                    Tuple<int, int> productoEnLista = productosConCantidad.FirstOrDefault(tupla => tupla.Item1 == productoId);

                    // Si el producto ya está en la lista, incrementa su cantidad.
                    if (productoEnLista != null)
                    {
                        productosConCantidad.Remove(productoEnLista);
                        productosConCantidad.Add(new Tuple<int, int>(productoId, productoEnLista.Item2 + 1));
                    }
                    else
                    {
                        // Si no está, agrégalo con una cantidad de 1.
                        productosConCantidad.Add(new Tuple<int, int>(productoId, 1));
                    }
                }

                // Lista para almacenar los detalles de los productos en el carrito.
                var detallesProductosEnCarrito = new List<ProductosTableViewModel>();
                bool necesitaCamion = false; // Indicador de si se necesita un camión para la entrega.
                decimal? totalGeneral = 0; // Inicializa el total general de la compra.

                // Usa la base de datos para obtener los detalles de los productos.
                using (var db = new TRAVEL2Entities())
                {
                    foreach (var productoConCantidad in productosConCantidad)
                    {
                        var producto = db.Productos.Find(productoConCantidad.Item1);

                        if (producto != null)
                        {
                            // Crea un modelo para el detalle del producto y lo agrega a la lista.
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

                            // Suma el total multiplicando el precio por la cantidad.
                            totalGeneral += detalleProducto.Precio * detalleProducto.Cantidad;

                            // Verifica si el producto requiere un camión para la entrega.
                            if (detalleProducto.Nombre.IndexOf("Barbacoa", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                detalleProducto.Nombre.IndexOf("Refrigerador", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                detalleProducto.Nombre.IndexOf("Muebles", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                detalleProducto.Nombre.IndexOf("Bicicleta", StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                necesitaCamion = true;
                            }
                        }
                    }

                    // Determina el tipo de vehículo necesario para la entrega.
                    string tipoDeVehiculoNecesario;

                    if (necesitaCamion)
                    {
                        tipoDeVehiculoNecesario = "Camion";
                    }
                    else
                    {
                        // Determina el tipo de vehículo basado en la cantidad de productos.
                        int cantidadTotal = productosConCantidad.Sum(p => p.Item2);

                        if (cantidadTotal >= 1 && cantidadTotal <= 3)
                        {
                            tipoDeVehiculoNecesario = "Moto";
                        }
                        else if (cantidadTotal >= 4 && cantidadTotal <= 5)
                        {
                            tipoDeVehiculoNecesario = "Carro";
                        }
                        else
                        {
                            tipoDeVehiculoNecesario = "Camion";
                        }
                    }

                    // Busca un repartidor que tenga el vehículo adecuado para la entrega.
                    Repartidores repartidorSeleccionado = db.Repartidores
                        .FirstOrDefault(r => r.TipoDeTransporte == tipoDeVehiculoNecesario);

                    if (repartidorSeleccionado != null)
                    {
                        // Determina el artículo ("un" o "una") para el tipo de vehículo.
                        string articulo = "un";
                        if (repartidorSeleccionado.TipoDeTransporte.Equals("Moto", StringComparison.OrdinalIgnoreCase) ||
                            repartidorSeleccionado.TipoDeTransporte.Equals("Bicicleta", StringComparison.OrdinalIgnoreCase))
                        {
                            articulo = "una";
                        }

                        // Almacena el mensaje sobre el tipo de transporte en ViewBag.
                        ViewBag.TipoDeTransporte = $"Tus artículos serán llevados por {repartidorSeleccionado.NombreRepartidor} en {articulo} {repartidorSeleccionado.TipoDeTransporte}.";
                    }
                    else
                    {
                        ViewBag.TipoDeTransporte = "No se encontró un repartidor con un vehículo adecuado para su pedido.";
                    }
                }

                // Almacena los detalles de los productos y el total general en ViewBag.
                ViewBag.DetallesProductosEnCarrito = detallesProductosEnCarrito;
                ViewBag.TotalGeneral = totalGeneral;
            }
            else
            {
                // Si no hay productos en el carrito, inicializa las listas y el total.
                ViewBag.DetallesProductosEnCarrito = new List<ProductosTableViewModel>();
                ViewBag.TotalGeneral = 0;
            }

            // Devuelve la vista del carrito.
            return View();
        }

        // Método que muestra la vista de la pasarela de pago.
        public ActionResult PasarelaPago()
        {
            return View();
        }

        // Método para realizar la compra, actualizando la base de datos y vaciando el carrito.
        [HttpPost]
        public ActionResult RealizarCompra()
        {
            // Obtiene el usuario actual desde la sesión.
            var usuario = Session["UsuarioActual"] as UsersViewModel;

            // Verifica si el usuario tiene productos en el carrito.
            if (!string.IsNullOrEmpty(usuario?.ProductosEnCarrito))
            {
                using (var db = new TRAVEL2Entities())
                {
                    // Obtiene la lista de IDs y cantidades de productos en el carrito.
                    var productosEnCarrito = usuario.ProductosEnCarrito.Split(',')
                        .Select(item => item.Split(':'))
                        .Where(parts => parts.Length == 2) // Verifica que haya al menos dos partes.
                        .Select(parts => new { ProductoId = int.Parse(parts[0]), Cantidad = int.Parse(parts[1]) })
                        .ToList();

                    // Recorre la lista de productos en el carrito y actualiza las existencias en la base de datos.
                    foreach (var item in productosEnCarrito)
                    {
                        // Obtiene el producto desde la base de datos.
                        var producto = db.Productos.Find(item.ProductoId);

                        if (producto != null)
                        {
                            // Resta la cantidad comprada de las existencias.
                            producto.CantidadEnStock -= item.Cantidad;

                            // Guarda los cambios en la base de datos.
                            db.SaveChanges();
                        }
                    }

                    // Actualiza el atributo ProductosEnCarrito a null en la base de datos.
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);
                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = null;
                        db.SaveChanges();
                    }
                }

                // Actualiza el carrito en la sesión.
                usuario.ProductosEnCarrito = null;
                Session["UsuarioActual"] = usuario;

                // Redirige a la página principal con el mensaje de compra realizada.
                TempData["CompraRealizada"] = "Compra realizada con éxito";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si no hay productos en el carrito, muestra un mensaje de error.
                TempData["CompraFallida"] = "No hay productos en el carrito";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para confirmar la compra, actualizando las existencias y vaciando el carrito.
        [HttpPost]
        public ActionResult CompraRealizada()
        {
            // Obtiene el usuario actual desde la sesión.
            var usuarioActual = Session["UsuarioActual"] as UsersViewModel;

            // Verifica si el usuario y su carrito no son nulos o vacíos.
            if (usuarioActual != null && !string.IsNullOrEmpty(usuarioActual.ProductosEnCarrito))
            {
                var productosEnCarritoIds = usuarioActual.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();

                // Lista para almacenar los productos y sus cantidades.
                var productosConCantidad = new List<Tuple<int, int>>();

                foreach (var productoId in productosEnCarritoIds)
                {
                    Tuple<int, int> productoEnLista = productosConCantidad.FirstOrDefault(tupla => tupla.Item1 == productoId);

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
                    // Actualiza las existencias de los productos en la base de datos.
                    foreach (var productoConCantidad in productosConCantidad)
                    {
                        var producto = db.Productos.Find(productoConCantidad.Item1);

                        if (producto != null)
                        {
                            // Resta la cantidad comprada de las existencias.
                            producto.CantidadEnStock -= productoConCantidad.Item2;
                        }
                    }

                    // Limpia el carrito del usuario y guarda los cambios.
                    usuarioActual.ProductosEnCarrito = null;
                    Session["UsuarioActual"] = usuarioActual;
                    db.SaveChanges();

                    // Actualiza el atributo ProductosEnCarrito a null en la base de datos.
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuarioActual.Id);
                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = null;
                        db.SaveChanges();
                    }
                }

                // Redirige a la página principal con el mensaje de compra realizada.
                TempData["CompraRealizada"] = "Compra realizada con éxito";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Si no hay productos en el carrito, muestra un mensaje de error.
                TempData["CompraFallida"] = "No hay productos en el carrito";
                return RedirectToAction("Index", "Home");
            }
        }

        // Método para quitar un producto del carrito.
        [HttpPost]
        public ActionResult QuitarProducto(int productoId)
        {
            // Obtiene el usuario actual desde la sesión.
            var usuario = Session["UsuarioActual"] as UsersViewModel;

            if (usuario != null)
            {
                // Elimina el producto del carrito en la sesión.
                var productosEnCarritoIds = usuario.ProductosEnCarrito.Split(',').Select(int.Parse).ToList();
                productosEnCarritoIds.Remove(productoId);

                // Actualiza la sesión solo si quedan productos en el carrito.
                if (productosEnCarritoIds.Count > 0)
                {
                    usuario.ProductosEnCarrito = string.Join(",", productosEnCarritoIds);
                }
                else
                {
                    // Si la lista es vacía, asigna null.
                    usuario.ProductosEnCarrito = null;
                }

                // Actualiza la sesión.
                Session["UsuarioActual"] = usuario;

                using (var db = new TRAVEL2Entities())
                {
                    // Obtén el usuario desde la base de datos y actualiza su carrito.
                    var usuarioEnBD = db.Users.FirstOrDefault(u => u.Id == usuario.Id);

                    if (usuarioEnBD != null)
                    {
                        usuarioEnBD.ProductosEnCarrito = usuario.ProductosEnCarrito;
                        db.SaveChanges();
                    }
                }
            }

            // Redirige a la página del carrito.
            return RedirectToAction("Index", "Carrito");
        }
    }
}
