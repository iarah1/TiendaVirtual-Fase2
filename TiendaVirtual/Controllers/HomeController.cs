using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TiendaVirtual.Models;

namespace TiendaVirtual.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["CurrentPage"] = 1;

            return View();
        }

        public ActionResult Products(int page, int cat, string search = "")
        {
            List<Product> products = new List<Product>();

            //TiendaDBHandle tienda = new TiendaDBHandle();
            //if (string.IsNullOrEmpty(search))
            //{
            //    products = tienda.GetProducts(page, cat, "");
            //}
            //else
            //{
            //    products = tienda.GetProducts(page, cat, search);
            //}

            ///api/Product/products?page=1&cat=1&search=

            string contents = "";
            string requestUri = "api/Product/products?page="+ page.ToString() +"&cat="+ cat.ToString() +"&search=" + search;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5100/");
                var response = client.GetAsync(requestUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    contents = response.Content.ReadAsStringAsync().Result;
                }
            }

            products = JsonConvert.DeserializeObject<List<Product>>(contents);




            Session["CurrentPage"] = page;

            ViewBag.Products = products;

            return View();
        }

        public ActionResult Carrito()
        {

            return View();
        }

        [HttpPost]
        public JsonResult GetListProduct(string productIds)
        {
            //TiendaDBHandle tienda = new TiendaDBHandle();
            //var data = tienda.GetListProducts(productIds);
            //string jsonResult = JsonConvert.SerializeObject(data);

            string jsonResult = "";
            string requestUri = "api/Product/listproducts?ids=" + productIds;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:5100/");
                var response = client.GetAsync(requestUri).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    jsonResult = response.Content.ReadAsStringAsync().Result;
                }
            }

            return Json(jsonResult);
        }

        [HttpPost]
        public JsonResult ProcesarOrden(Cliente cliente, List<PedidoDetalle> detalle)
        {
            //TiendaDBHandle tienda = new TiendaDBHandle();
            //var pedidoId = tienda.ProcesarPedido(cliente, detalle);
            //string jsonResult = JsonConvert.SerializeObject(pedidoId);

            string jsonResult = "";

            Pedido pedido = new Pedido();
            pedido.cliente = cliente;
            pedido.pedidoDetalle = detalle;

            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(pedido);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                string RequestUri = "api/Pedido/procesarpedido";
                
                
                client.BaseAddress = new Uri("http://localhost:5100/");
                var response = client.PostAsync(RequestUri, data).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    jsonResult = response.Content.ReadAsStringAsync().Result;
                }
                else
                    Console.Write("Error");
            }

            return Json(jsonResult);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Contact(FormCollection form)
        {
            Feedback feedback = new Feedback();
            feedback.Nombre = form["Nombre"];
            feedback.Email = form["Email"];
            feedback.FeedbackTypeId = Convert.ToInt32(form["FeedbackTypeId"]);
            feedback.Nota = form["Nota"];


            //Models.TiendaDBHandle tiendaDB = new TiendaDBHandle();
            //tiendaDB.NewFeedback(feedback);

            string jsonResult = "";
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(feedback);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                string RequestUri = "api/Feedback/feedback";


                client.BaseAddress = new Uri("http://localhost:5100/");
                var response = client.PostAsync(RequestUri, data).Result;

                if (response.IsSuccessStatusCode)
                {
                    Console.Write("Success");
                    jsonResult = response.Content.ReadAsStringAsync().Result;
                }
                else
                    Console.Write("Error");
            }


            return View();
        }
    }
}