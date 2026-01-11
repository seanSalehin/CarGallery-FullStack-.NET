using Asp.Versioning;
using AutoMapper;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers.Version2
{


    [Route("api/v{version:apiVersion}/car")]
    [ApiVersion("2.0")]
    [ApiController]
    public class GallaryController : ControllerBase
    {


        //Dependency Injection
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public GallaryController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<String>> GetCars() 
        {
            return "this is v2";
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<String>> GetCarById(int id)
        {
            return "this is just a test" + id;
        }

    }
}
