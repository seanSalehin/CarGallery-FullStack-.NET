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


    [Route("api/v{version:apiVersion}/features")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiController]
    public class FeaturesController : ControllerBase
    {



        //Dependency Injection
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public FeaturesController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }




        //End Points
        [HttpGet]
        [Authorize(Roles="Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<FeaturesDTO>>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public  async Task<ActionResult <ApiResponse<IEnumerable<FeaturesDTO>>>> GetFeatures() 
        {
            var car = await _db.Features.ToListAsync();

            var dtoResponse = _mapper.Map<List<FeaturesDTO>>(car);
            var response = ApiResponse<IEnumerable<FeaturesDTO>>.Ok(dtoResponse, "Feature retreved successfully");
            return Ok(response); 
        }



        [HttpGet("{id:int}")]
        [AllowAnonymous] //anyone can access this action (not only customer or admin)
        [ProducesResponseType(typeof(ApiResponse<FeaturesDTO>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<FeaturesDTO>>> GetFeatureById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return NotFound(ApiResponse<object>.NotFound("Feature ID must be greater than 0")); //ApiResponse<object> => because it is generics but we do not have any specific type so we say just object
                }

                var data = await _db.Features.FirstOrDefaultAsync(n=>n.Id==id);
                if(data == null)
                {
                    return NotFound(ApiResponse<object>.NotFound($"Feature with Id {id} was not found"));
                }
                return Ok(ApiResponse<FeaturesDTO>.Ok(_mapper.Map<FeaturesDTO>(data), "Records Retrived Successfully"));
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }

        }




        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<FeaturesDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<FeaturesDTO>>> CreateFeature([FromBody] FeaturesCreateDTO carsDTO)
        {
            try
            {
                if(carsDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Feature Data Is Required"));
                }
                var duplicateCar = await _db.Features.FirstOrDefaultAsync(u => u.Id == carsDTO.Id);
                if (duplicateCar == null)
                {
                    return Conflict(ApiResponse<FeaturesDTO>.Conflict($"A Feature with the same name '{carsDTO.Id}' already exist "));
                }
                Features data = _mapper.Map<Features>(carsDTO);
                data.CreatedDate=DateTime.Now;
                await _db.Features.AddAsync(data);
                await _db.SaveChangesAsync();
                var response = ApiResponse<FeaturesDTO>.CreatedAt(_mapper.Map<FeaturesDTO>(carsDTO), "car created successfully");
                return CreatedAtAction(nameof(CreateFeature), new {id= data.Id}, response );  
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the car: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<FeaturesDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<FeaturesDTO>>> UpdateFeature(int id, [FromBody] FeaturesUpdateDTO carsDTO)
        {
            try
            {
                if(carsDTO==null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Feature Data Is Required"));
                }
                if (id!= carsDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Feature Id in URL Does Not Match Features Id In Request Body"));
                }
                var existingCar = await _db.Features.FirstOrDefaultAsync(n => n.Id == id);
                if (existingCar==null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Feature ID  must be greater than 0")); 
                }

                var duplicateCar = await _db.Features.FirstOrDefaultAsync(u =>u.Id==carsDTO.Id);
                if (duplicateCar ==null)
                {
                    return Conflict(ApiResponse<FeaturesDTO>.Conflict($"A Feature with the same Id '{carsDTO.Id}' already exist "));
                }


                //map to existingCar
                _mapper.Map(carsDTO, existingCar);
                existingCar.UpdatedDate=DateTime.Now;
                await _db.SaveChangesAsync();
                var response = ApiResponse<FeaturesDTO>.Ok(_mapper.Map<FeaturesDTO>(existingCar), "Car Updated Successfully");
                return Ok(carsDTO);

            }catch(Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the Feature: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteFeature(int id)
        {
            try
            {
                var existingCar = await _db.Features.FirstOrDefaultAsync(n => n.Id == id);
                if(existingCar==null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Feature ID must be greater than 0"));
                }
                _db.Features.Remove(existingCar);
                await _db.SaveChangesAsync();

                var respone = ApiResponse<FeaturesDTO>.NoContent("Feature Deleted Successfully");
                return Ok(respone);    
            }
            catch (Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An error occured while creating the Feature: ", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
