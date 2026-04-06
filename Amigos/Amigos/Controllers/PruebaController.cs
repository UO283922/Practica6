using Amigos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Amigos.Controllers
{
    public class PruebaController : Controller
    {
        private readonly IInc _inc;

        public PruebaController(IInc inc)
        {
            _inc = inc;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        /*
        public string Index()
        {
            return "Hola mundo";
        }
        */

        public string Hola(string valor, int veces)
        {
            string resultado = "";

            for (int i = 0; i < veces; i++)
            {
                resultado += valor;
            }

            return resultado;
        }

        public IActionResult Interfaces()
        {
            int valor = _inc.Inc();
            return Content("Valor: " + valor);
        }

        public IActionResult Adios(string valor, int veces)
        {
            ViewBag.valor = valor;
            ViewBag.veces = veces;

            return View();
        }
    }
}
