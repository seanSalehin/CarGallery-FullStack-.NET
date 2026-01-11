using AutoMapper;
using Client.Services;
using Gateway_API_Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class CarController:Controller
    {
        private readonly ICarServices _carService;
        private readonly IMapper _mapper;
            public CarController(ICarServices carServices, IMapper mapper)
        {
            _mapper = mapper;
            _carService = carServices;
        }


        public async Task <IActionResult>Index()
        {
            List<CarsDTO> carList = new();
            try
            {
                var response = await _carService.GetAllAsync<ApiResponse<List<CarsDTO>>>();
                if(response!=null && response.Success && response.Data !=null)
                {
                    carList = response.Data;
                }
            }
            catch(Exception ex)
            {
                TempData["error"] = $"An error occured: {ex.Message}";
            }
            return View(carList);
        }


        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCarsDTO createDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList();

                TempData["error"] = "ModelState invalid: " + string.Join(" | ", errors);
                return View(createDTO);
            }
            try
            {
                var response = await _carService.CreateAsync<ApiResponse<CarsDTO>>(createDTO);
                if (response != null && response.Success)
                {
                    TempData["success"] = "Car created successfully";
                    return RedirectToAction(nameof(Index));
                }
                TempData["error"] = response?.Message ?? "Create failed (API returned no message).";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Create failed: {ex.Message}";
            }
            return View(createDTO);
        }




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid Car Id";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var response = await _carService.GetAsync<ApiResponse<CarsDTO>>(id);
                if (response != null && response.Success &&response.Data !=null)
                {
                    return View(response.Data);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Create failed: {ex.Message}";
            }
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(CarsDTO carDTO)
        {
            try
            {
                var response = await _carService.DeleteAsync<ApiResponse<object>>(carDTO.Id);

                if (response != null && response.Success)
                {
                    TempData["success"] = "Car deleted successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Delete failed: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["error"] = "Invalid Car Id";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var response = await _carService.GetAsync<ApiResponse<CarsDTO>>(id);
                if (response != null && response.Success && response.Data != null)
                {
                    return View(_mapper.Map<UpdateDTO>(response.Data));
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Create failed: {ex.Message}";
            }
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateDTO carDTO)
        {
            try
            {
                var response = await _carService.UpdateAsync<ApiResponse<object>>(carDTO);

                if (response != null && response.Success)
                {
                    TempData["success"] = "Car updated successfully";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Edit failed: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
