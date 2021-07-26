using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BankingDemo.Controllers
{
    [Route("api/demo")]
    [Authorize]
    public class HomeController : Controller
    {
        [Produces("text/plain")]
        [Route("demo1")]

        // GET: /<controller>/
        public IActionResult Demo1()
        {
            try
            {
                return Ok("Demo 1");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        // demo 2
        [Produces("text/html")]
        [Route("demo2")]

        // GET: /<controller>/
        public IActionResult Demo2()
        {
            try
            {
                return new ContentResult()
                {
                    ContentType = "text/html",
                    Content = "<b><i>Demo 2</i></b>"
                };
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
