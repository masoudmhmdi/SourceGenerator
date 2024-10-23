namespace YourApp.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;

    public class MamadController : ControllerBase
    {
        [HttpPost("/getMmd")]
        public string getMmd()
        {
            return "Hello World!";
        }
    }
}