using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Gateway_API_Client;
using Asp.Versioning;

namespace WebApplication1.Controllers.Version1
{


    [Route("api/v{version:apiVersion}/car")]
    [ApiVersion("1.0")]
    [ApiController]
    public class GallaryController:ControllerBase
    {



        //Dependency Injection
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public GallaryController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }




        //End Points
        [HttpGet]
        //[Authorize(Roles="Customer,Admin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CarsDTO>>), StatusCodes.Status200OK)] //show the possible responses for our endpoints 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        //public  async Task<ActionResult <IEnumerable<Cars>>> GetCars()   => this is a mistake because we are exposing a Cars entity (table) we should never do that so we craeet a DTO
        public  async Task<ActionResult <ApiResponse<IEnumerable<CarsDTO>>>> GetCars()   //return type CarsDTO
        {
            var car = await _db.Cars.ToListAsync();

            // return Ok(_mapper.Map<List<CarsDTO>>(car));   //(car) => this is the source for our Map  => without apiresponse (function helper)

            //with function helper
            var dtoResponse = _mapper.Map<List<CarsDTO>>(car);
            var response = ApiResponse<IEnumerable<CarsDTO>>.Ok(dtoResponse, "car retreved successfully");
            return Ok(response); 
        }



        [HttpGet("{id:int}")]
        [AllowAnonymous] //anyone can access this action (not only customer or admin)
        [ProducesResponseType(typeof(ApiResponse<CarsDTO>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CarsDTO>>> GetCarById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Car ID must be greater than 0")); //ApiResponse<object> => because it is generics but we do not have any specific type so we say just object
                }

                var data = await _db.Cars.FirstOrDefaultAsync(n=>n.Id==id);
                if(data == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Car with Id {id} was not found"));
                }
                return Ok(ApiResponse<CarsDTO>.Ok(_mapper.Map<CarsDTO>(data), "Records Retrived Successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }

        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CarsDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CreateCarsDTO>>> CreateCars([FromBody] CreateCarsDTO carsDTO)
        {
            try
            {
                if(carsDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car Data Is Required"));
                }
                var duplicateCar = await _db.Cars.FirstOrDefaultAsync(u => u.Name.ToLower() == carsDTO.Name.ToLower());
                if (duplicateCar != null)
                {
                    return Conflict(ApiResponse<CarsDTO>.Conflict($"A Car with the same name '{carsDTO.Name}' already exist "));
                }
                Cars data = _mapper.Map<Cars>(carsDTO);
                await _db.Cars.AddAsync(data);
                await _db.SaveChangesAsync();
                var response = ApiResponse<CarsDTO>.CreatedAt(_mapper.Map<CarsDTO>(carsDTO), "car created successfully");
                return CreatedAtAction(nameof(CreateCars), new {id= data.Id}, response );  
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<CarsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CarsDTO>>>UpdateCar(int id, [FromBody]UpdateDTO carsDTO)
        {
            try
            {
                if(carsDTO==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car Data Is Required"));
                }
                if (id!= carsDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car Id in URL Does Not Match Cars Id In Request Body"));
                }
                var existingCar = await _db.Cars.FirstOrDefaultAsync(n => n.Id == id);
                if (existingCar==null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Car ID must be greater than 0")); 
                }
                var duplicateCar = await _db.Cars.FirstOrDefaultAsync(u =>u.Name.ToLower()==carsDTO.Name.ToLower() && u.Id !=id);
                if (duplicateCar !=null)
                {
                    return Conflict(ApiResponse<CarsDTO>.Conflict($"A Car with the same name '{carsDTO.Name}' already exist "));
                }


                //map to existingCar
                _mapper.Map(carsDTO, existingCar);
                existingCar.UpdatedDate=DateTime.Now;
                await _db.SaveChangesAsync();
                var response = ApiResponse<CarsDTO>.Ok(_mapper.Map<CarsDTO>(carsDTO), "Car Updated Successfully");
                return Ok(carsDTO);

            }catch(Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>>DeleteCars(int id)
        {
            try
            {
                var existingCar = await _db.Cars.FirstOrDefaultAsync(n => n.Id == id);
                if(existingCar==null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Car ID must be greater than 0"));
                }
                _db.Cars.Remove(existingCar);
                await _db.SaveChangesAsync();

                var respone = ApiResponse<CarsDTO>.NoContent("Car Deleted Successfully");
                return Ok(respone);    
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
