﻿using Microsoft.AspNetCore.Mvc;

namespace TicTacToe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController: ControllerBase
    {
        [HttpGet]
        [Route("")]
        public IActionResult Get() 
        { 
            return Ok(); 
        }
    }
}
