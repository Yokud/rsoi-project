using DS_Project.Rentals.DTO;
using DS_Project.Rentals.Entity;
using DS_Project.Rentals.Service;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Rentals.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RentalController : Controller
    {
        readonly IRentalsService _rentals;

        public RentalController(IRentalsService rentals)
        {
            _rentals = rentals;
        }

        [HttpGet("rental")]
        public async Task<IActionResult> GetUserRentals([FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rentals = await _rentals.GetUserRentalsAsync(xUserName);

            return Ok(rentals);
        }

        [HttpGet("rental/{rentalUid}")]
        public async Task<IActionResult> GetUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var item = await _rentals.GetAsync(xUserName, rentalUid);

            return Ok(item);
        }

        [HttpPost("rental")]
        public async Task<IActionResult> RentCar([FromHeader(Name = "X-User-Name")] string xUserName, [FromBody] CreatePaidRentalRequest request)
        {
            var newRental = new Rental() { CarUid = request.CarUid, DateFrom = request.DateFrom.ToUniversalTime(), DateTo = request.DateTo.ToUniversalTime(), Username = xUserName, Status = "IN_PROGRESS", RentalUid = Guid.NewGuid(), PaymentUid = request.PaymentUid };

            await _rentals.CreateAsync(newRental);

            return Ok(newRental);
        }

        [HttpDelete("rental/{rentalUid}")]
        public async Task<IActionResult> CancelUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rental = await _rentals.GetAsync(xUserName, rentalUid);

            rental!.Status = "CANCELED";
            await _rentals.Update(rental);

            return Ok();
        }

        [HttpPost("rental/{rentalUid}/finish")]
        public async Task<IActionResult> FinishUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rental = await _rentals.GetAsync(xUserName, rentalUid);

            rental!.Status = "FINISHED";
            await _rentals.Update(rental);

            return Ok();
        }
    }
}
