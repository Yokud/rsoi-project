using DS_Project.Rentals.DTO;
using DS_Project.Rentals.Entity;
using DS_Project.Rentals.Service;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Rentals.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RentalController(IRentalsService rentals, LogsProducer producer) : Controller
    {
        readonly IRentalsService _rentals = rentals;
        readonly LogsProducer _producer = producer;

        [HttpGet("rental")]
        public async Task<IActionResult> GetUserRentals([FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rentals = await _rentals.GetUserRentalsAsync(xUserName);
            await _producer.Produce("[RentalService]: get user rentals");
            return Ok(rentals);
        }

        [HttpGet("rental/{rentalUid}")]
        public async Task<IActionResult> GetUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var item = await _rentals.GetAsync(xUserName, rentalUid);
            await _producer.Produce($"[RentalService]: get user rental with uid = {rentalUid}");
            return Ok(item);
        }

        [HttpPost("rental")]
        public async Task<IActionResult> RentCar([FromHeader(Name = "X-User-Name")] string xUserName, [FromBody] CreatePaidRentalRequest request)
        {
            var newRental = new Rental() { CarUid = request.CarUid, DateFrom = request.DateFrom.ToUniversalTime(), DateTo = request.DateTo.ToUniversalTime(), Username = xUserName, Status = "IN_PROGRESS", RentalUid = Guid.NewGuid(), PaymentUid = request.PaymentUid };

            await _rentals.CreateAsync(newRental);
            await _producer.Produce($"[RentalService]: rent car with uid = {request.CarUid}");
            return Ok(newRental);
        }

        [HttpDelete("rental/{rentalUid}")]
        public async Task<IActionResult> CancelUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rental = await _rentals.GetAsync(xUserName, rentalUid);

            rental!.Status = "CANCELED";
            await _rentals.Update(rental);
            await _producer.Produce($"[RentalService]: cancel rent with uid = {rentalUid}");
            return Ok();
        }

        [HttpPost("rental/{rentalUid}/finish")]
        public async Task<IActionResult> FinishUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "X-User-Name")] string xUserName)
        {
            var rental = await _rentals.GetAsync(xUserName, rentalUid);

            rental!.Status = "FINISHED";
            await _rentals.Update(rental);
            await _producer.Produce($"[RentalService]: finished rent with uid = {rentalUid}");
            return Ok();
        }
    }
}
