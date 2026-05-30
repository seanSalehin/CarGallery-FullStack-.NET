using Asp.Versioning;
using AutoMapper;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
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
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public GallaryController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CarsDTO>>), StatusCodes.Status200OK)] 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CarsDTO>>>> GetCars([FromQuery]string? filterBy, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] string? sortOrder = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)   
        {
            //filtering logic
            var carQuery = _db.Cars.AsQueryable();
            if(!string.IsNullOrEmpty(filterBy) && !string.IsNullOrEmpty(filterQuery))
            {
                 switch(filterBy.ToLower())
                {
                    case "name":
                        carQuery = carQuery.Where(u => u.Name.Contains(filterQuery.ToLower()));
                        break;

                    case "details":
                        carQuery = carQuery.Where(u => u.Details != null && u.Details.Contains(filterQuery.ToLower()));
                        break;

                    case "rate":
                        if(double.TryParse(filterQuery, out double rateValue))
                        {
                            carQuery = carQuery.Where(u => u.Rate == rateValue);
                        }
                        break;


                    case "minRate":
                        if (double.TryParse(filterQuery, out double minRate))
                        {
                            carQuery = carQuery.Where(u => u.Rate >= minRate);
                        }
                        break;


                    case "maxRate":
                        if (double.TryParse(filterQuery, out double maxRate))
                        {
                            carQuery = carQuery.Where(u => u.Rate <= maxRate);
                        }
                        break;
                }
            }

            //sorting logic
            if (!string.IsNullOrEmpty(sortBy))
            {
                var isDescending = sortOrder?.ToLower() == "desc";
                carQuery = sortBy.ToLower() switch
                {
                    "name" => isDescending ? carQuery.OrderByDescending(u => u.Name) : carQuery.OrderBy(u => u.Name),
                    "rate" => isDescending ? carQuery.OrderByDescending(u => u.Rate) : carQuery.OrderBy(u => u.Rate),
                    "price" => isDescending ? carQuery.OrderByDescending(u => u.price) : carQuery.OrderBy(u => u.price),
                    _ => carQuery.OrderBy(u => u.Id)
                };
            }
            else
            {
                carQuery = carQuery.OrderBy(u => u.Id);
            }

            //Pagination  logic
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;
            var skip = (page - 1) * pageSize;
            var totalRecords = await carQuery.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
            var car = await carQuery.Skip(skip).Take(pageSize).ToListAsync();
            var dtoResponsiveCar = _mapper.Map<List<CarsDTO>>(car);


            //Pagination  Messge
            var messageBuilder = new System.Text.StringBuilder();
            messageBuilder.Append($"Page {page} of {totalPages}. Total Records: {totalRecords}.");
            messageBuilder.Append($" Returned {car.Count} records.");
            if(!string.IsNullOrEmpty(filterBy) && !string.IsNullOrEmpty(filterQuery))
            {
                messageBuilder.Append($" Filtered by {filterBy} with query '{filterQuery}'.");
            }
            Response.Headers.Append("X-Pagination-CurrentPage", page.ToString());
            Response.Headers.Append("X-Pagination-PSageSize", pageSize.ToString());
            Response.Headers.Append("X-Pagination-TotalCount", totalPages.ToString());
            Response.Headers.Append("X-Pagination-TotalCount", totalRecords.ToString());

            //execution logic
            var dtoResponse = _mapper.Map<List<CarsDTO>>(car);
            var response = ApiResponse<IEnumerable<CarsDTO>>.Ok(dtoResponse, "car retreved successfully");
            return Ok(response);
        }



        [HttpGet("{id:int}")]
        [AllowAnonymous]
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

                var data = await _db.Cars.FirstOrDefaultAsync(n => n.Id == id);
                if (data == null)
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
                if (carsDTO == null)
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
                return CreatedAtAction(nameof(CreateCars), new { id = data.Id }, response);
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
        public async Task<ActionResult<ApiResponse<CarsDTO>>> UpdateCar(int id, [FromBody] UpdateDTO carsDTO)
        {
            try
            {
                if (carsDTO == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car Data Is Required"));
                }
                if (id != carsDTO.Id)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car Id in URL Does Not Match Cars Id In Request Body"));
                }
                var existingCar = await _db.Cars.FirstOrDefaultAsync(n => n.Id == id);
                if (existingCar == null)
                {
                    return NotFound(ApiResponse<object>.NotFound("Car ID must be greater than 0"));
                }
                var duplicateCar = await _db.Cars.FirstOrDefaultAsync(u => u.Name.ToLower() == carsDTO.Name.ToLower() && u.Id != id);
                if (duplicateCar != null)
                {
                    return Conflict(ApiResponse<CarsDTO>.Conflict($"A Car with the same name '{carsDTO.Name}' already exist "));
                }


                //map to existingCar
                _mapper.Map(carsDTO, existingCar);
                existingCar.UpdatedDate = DateTime.Now;
                await _db.SaveChangesAsync();
                var response = ApiResponse<CarsDTO>.Ok(_mapper.Map<CarsDTO>(carsDTO), "Car Updated Successfully");
                return Ok(carsDTO);

            }
            catch (Exception ex)
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
        public async Task<ActionResult<ApiResponse<object>>> DeleteCars(int id)
        {
            try
            {
                var existingCar = await _db.Cars.FirstOrDefaultAsync(n => n.Id == id);
                if (existingCar == null)
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