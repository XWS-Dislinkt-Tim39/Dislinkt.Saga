using Dislinkt.Saga.Data;
using Dislinkt.Saga.Menager;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dislinkt.Saga.Controller
{
 
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController:ControllerBase
    {

        private readonly IRegistrationMenager _registrationMenager;

        public RegisterController(IRegistrationMenager registrationMenager)
        {
           this._registrationMenager= registrationMenager;
        }

        [Route("")]
        [HttpGet]
        public string Index()
        {
            return "Hello from Saga/Controller";
        }

        [HttpPost]
        [Route("/register")]
        public bool Register([FromBody] UserData user)
        {
            return _registrationMenager.Register(user);
        }
    }

  
}
