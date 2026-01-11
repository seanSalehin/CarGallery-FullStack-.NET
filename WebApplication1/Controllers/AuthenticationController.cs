using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using Gateway_API_Client;
using WebApplication1.Services;
using Asp.Versioning;

namespace WebApplication1.Controllers
{
    [Route("api/auth")]
    [ApiVersionNeutral]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthService _authService;
        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<CarsDTO>), StatusCodes.Status200OK)] //show the possible responses for our endpoints 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<UserDTO>>> Register([FromBody] RegisterationRequestDTO registarationRequest)
        {
            try
            {
                if (registarationRequest == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Car data Is Required"));
                }
                if (await _authService.IsEmailExistAsync(registarationRequest.Email))
                {
                    return Conflict(ApiResponse<object>.Conflict($"User with email  {registarationRequest.Email} already exists"));
                }
                var user = await _authService.RegisterAsync(registarationRequest);
                if (user == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Registration Faild"));
                }
                var response = ApiResponse<UserDTO>.CreatedAt(user, "User Created Successfully");
                return CreatedAtAction(nameof(Register), response);
            }
            catch (Exception ex)
            {
                var errorReponse = ApiResponse<object>.Error(500, "An Error Occured Durring Registration", ex.Message);
                return StatusCode(500, errorReponse);
            }
            }




        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LoginResponseDTO>>), StatusCodes.Status201Created)] //show the possible responses for our endpoints 
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<LoginResponseDTO>>> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                if(loginRequest == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Data Is Required"));
                }

                if (string.IsNullOrWhiteSpace(loginRequest.Email) || string.IsNullOrWhiteSpace(loginRequest.Password))
                    return BadRequest(ApiResponse<object>.BadRequest("Email and Password are required"));

                var loginResponse = await _authService.LoginAsync(loginRequest);
                if(loginResponse == null)
                {
                    return BadRequest(ApiResponse<object>.BadRequest("Login Failed"));
                }
                var response = ApiResponse<LoginResponseDTO>.Ok(loginResponse, "Login Successfully");
                return Ok(response);
            }
            catch(Exception ex)
            {
                var errorResponse = ApiResponse<object>.Error(500, "An Error Occured During Login", ex.Message);
                return StatusCode(500, errorResponse);
            }
        }
    }
}
