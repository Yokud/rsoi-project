using Microsoft.AspNetCore.Mvc;
using DS_Project.Cars.DTO;

namespace DS_Project.Cars.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CarsController : Controller
    {
        readonly ICarsService _carsService;

        public CarsController(ICarsService carsService)
        {
            _carsService = carsService;
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetPageOfAvailableCars([FromQuery] int page, [FromQuery] int size, [FromQuery] bool showAll = false)
        {
            var availableCars = await _carsService.GetPageOfAvailableCars(page, size, showAll);
            var count = await _carsService.CountAsync();

            return Ok(new PaginationResponse { Page = page, PageSize = size, TotalElements = count, Items = availableCars.Select(CarResponse.FromCar) });
        }

        [HttpGet("cars/{carUid}")]
        public async Task<IActionResult> GetCar(Guid carUid)
        {
            var res = await _carsService.GetAsync(carUid);

            return res is not null ? Ok(new CarInfo { CarUid = res.CarUid, Brand = res.Brand, Model = res.Model, RegistrationNumber = res.RegistrationNumber }) : NotFound();
        }

        [HttpGet("cars/{carUid}/price")]
        public async Task<IActionResult> GetCarPrice(Guid carUid)
        {
            var res = await _carsService.GetAsync(carUid);

            return res is not null ? Ok(res.Price) : NotFound();
        }

        [HttpGet("cars/{carUid}/rent")]
        public async Task<IActionResult> RentCar(Guid carUid)
        {
            var item = await _carsService.GetAsync(carUid);

            if (item is not null)
            {
                item.Availability = false;
                await _carsService.UpdateAsync(item);
                return Ok();
            }
            else
                return NotFound();
        }

        [HttpGet("cars/{carUid}/undo_rent")]
        public async Task<IActionResult> UndoRentCar(Guid carUid)
        {
            var item = await _carsService.GetAsync(carUid);

            if (item is not null)
            {
                item.Availability = true;
                await _carsService.UpdateAsync(item);
                return Ok();
            }
            else
                return NotFound();
        }
    }
}
