using System.Diagnostics;
using System.Net.Http.Json;
using AutoMapper;
using Client.Models;
using Client.Services;
using Gateway_API_Client;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarServices _carServices;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ICarServices carServices, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _mapper = mapper;
            _carServices = carServices;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index(string? filterBy, string? filterQuery, string? sortBy, string? sortOrder = "asc", int page = 1, int pageSize = 6)
        {
            var carList = new List<CarsDTO>();

            try
            {
                var client = _httpClientFactory.CreateClient("CarGallaryAPI");
                var query = BuildCarQuery(filterBy, filterQuery, sortBy, sortOrder, page, pageSize);
                var responseMessage = await client.GetAsync($"api/{SD.CurrentApiVersion}/car?{query}");
                responseMessage.EnsureSuccessStatusCode();
                var response = await responseMessage.Content.ReadFromJsonAsync<ApiResponse<List<CarsDTO>>>();

                if (response != null && response.Success && response.Data != null)
                {
                    carList = response.Data;
                }

                var pagination = ReadPagination(responseMessage, page, pageSize, carList.Count);
                SetPaginationViewBag(filterBy, filterQuery, sortBy, sortOrder, page, pageSize, pagination.totalPages, pagination.totalRecords);
            }
            catch (Exception ex)
            {
                TempData["error"] = $"An error occured: {ex.Message}";
                SetPaginationViewBag(filterBy, filterQuery, sortBy, sortOrder, page, pageSize, 1, carList.Count);
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

        private static string BuildCarQuery(string? filterBy, string? filterQuery, string? sortBy, string? sortOrder, int page, int pageSize)
        {
            var query = new Dictionary<string, string?>
            {
                ["filterBy"] = string.IsNullOrWhiteSpace(filterBy) ? null : filterBy,
                ["filterQuery"] = string.IsNullOrWhiteSpace(filterQuery) ? null : filterQuery,
                ["sortBy"] = string.IsNullOrWhiteSpace(sortBy) ? null : sortBy,
                ["sortOrder"] = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder,
                ["page"] = page < 1 ? "1" : page.ToString(),
                ["pageSize"] = pageSize < 1 ? "6" : pageSize.ToString()
            };

            return string.Join("&", query
                .Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => $"{Uri.EscapeDataString(x.Key)}={Uri.EscapeDataString(x.Value!)}"));
        }

        private static (int totalPages, int totalRecords) ReadPagination(HttpResponseMessage responseMessage, int page, int pageSize, int returnedCount)
        {
            var numbers = new List<int>();

            if (responseMessage.Headers.TryGetValues("X-Pagination-TotalCount", out var values))
            {
                numbers = values
                    .SelectMany(v => v.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    .Select(v => int.TryParse(v.Trim(), out var number) ? number : 0)
                    .Where(v => v > 0)
                    .ToList();
            }

            var totalPages = numbers.Count > 0 ? numbers[0] : Math.Max(page, 1);
            var totalRecords = numbers.Count > 1 ? numbers[1] : Math.Max(returnedCount, ((Math.Max(page, 1) - 1) * Math.Max(pageSize, 1)) + returnedCount);

            return (Math.Max(totalPages, 1), Math.Max(totalRecords, returnedCount));
        }

        private void SetPaginationViewBag(string? filterBy, string? filterQuery, string? sortBy, string? sortOrder, int page, int pageSize, int totalPages, int totalRecords)
        {
            ViewBag.FilterBy = filterBy ?? "";
            ViewBag.FilterQuery = filterQuery ?? "";
            ViewBag.SortBy = sortBy ?? "";
            ViewBag.SortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "asc" : sortOrder;
            ViewBag.Page = page < 1 ? 1 : page;
            ViewBag.PageSize = pageSize < 1 ? 6 : pageSize;
            ViewBag.TotalPages = totalPages < 1 ? 1 : totalPages;
            ViewBag.TotalRecords = totalRecords < 0 ? 0 : totalRecords;
        }
    }
}
