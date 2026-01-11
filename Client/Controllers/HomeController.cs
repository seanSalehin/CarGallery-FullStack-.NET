using System.Diagnostics;
using System.Threading.Tasks;
using AutoMapper;
using Client.Models;
using Client.Services;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarServices _carServices;
        private readonly IMapper _mapper;

        public HomeController(ICarServices carServices, IMapper mapper)
        {
            _mapper = mapper;
            _carServices= carServices;
        }

        public async Task<IActionResult> Index()
        {
            List<CarsDTO> carList = new();
            try
            {
                var response = await _carServices.GetAllAsync<ApiResponse<List<CarsDTO>>>();
                if(response !=null && response.Success && response.Data!=null)
                {
                    carList = response.Data;
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occured:{ex.Message}";
            }
            return View(carList);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
