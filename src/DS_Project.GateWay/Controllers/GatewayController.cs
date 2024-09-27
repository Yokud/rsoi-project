using DS_Project.Gateway;
using DS_Project.GateWay.DTO;
using DS_Project.GateWay.Services;
using DS_Project.GateWay.Utils;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace DS_Project.GateWay.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class GatewayController : Controller
    {
        readonly HttpClient _httpClient;
        readonly CircuitBreaker _circuitBreaker;
        readonly RequestQueueService _requestQueueService;

        public GatewayController()
        {
            _httpClient = new HttpClient();
            _circuitBreaker = CircuitBreaker.Instance;
            _requestQueueService = new RequestQueueService();
            _requestQueueService.StartWorker();
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetPageOfAvailableCars([FromQuery] int page, [FromQuery] int size, [FromQuery] bool showAll = false)
        {
            var carsServiceHealth = await HealthCheckAsync("cars:8070");

            if (!carsServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Cars Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            var res = await _httpClient.GetFromJsonAsync<PaginationResponse>($"http://cars:8070/cars?page={page}&size={size}&showAll={showAll}");

            return Ok(res);
        }

        [HttpGet("rental")]
        public async Task<IActionResult> GetUserRentals([FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            var rentalServiceHealth = await HealthCheckAsync("rentals:8060");

            if (!rentalServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Rental Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var rentalsReq = new HttpRequestMessage(HttpMethod.Get, "http://rentals:8060/rental");
            rentalsReq.Headers.Add("scopeToken", scopeToken);
            using var rentalsRes = await _httpClient.SendAsync(rentalsReq);
            if (rentalsRes.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var rentals = await rentalsRes.Content.ReadFromJsonAsync<IEnumerable<Rental>>();

            if (rentals is null || !rentals.Any())
                return NotFound($"User with token = {scopeToken} has no rentals");

            var responses = new List<RentalResponse>(rentals.Count());
                
            foreach (var rental in rentals)
            {
                object? car;
                var carsServiceHealth = await HealthCheckAsync("cars:8070");
                if (carsServiceHealth)
                    car = await _httpClient.GetFromJsonAsync<CarInfo>($"http://cars:8070/cars/{rental.CarUid}");
                else
                    car = Array.Empty<object>();

                object? payment;
                var paymentServiceCheck = await HealthCheckAsync("payments:8050");
                if (paymentServiceCheck)
                    payment = await _httpClient.GetFromJsonAsync<PaymentInfo>($"http://payments:8050/payment/{rental.PaymentUid}");
                else
                    payment = Array.Empty<object>();

                responses.Add(new RentalResponse { RentalUid = rental.RentalUid, Status = rental.Status, DateFrom = DateOnly.FromDateTime(rental.DateFrom), DateTo = DateOnly.FromDateTime(rental.DateTo), Car = car, Payment = payment });
            }

            return Ok(responses);
        }

        [HttpGet("rental/{rentalUid}")]
        public async Task<IActionResult> GetUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            var rentalServiceHealth = await HealthCheckAsync("rentals:8060");

            if (!rentalServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Rental Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var rentalsReq = new HttpRequestMessage(HttpMethod.Get, $"http://rentals:8060/rental/{rentalUid}");
            rentalsReq.Headers.Add("scopeToken", scopeToken);
            using var rentalsRes = await _httpClient.SendAsync(rentalsReq);
            if (rentalsRes.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var rental = await rentalsRes.Content.ReadFromJsonAsync<Rental>();

            if (rental is null)
                return NotFound($"User with token = {scopeToken} has no rental with UUID = {rentalUid}");

            object? car;
            var carsServiceHealth = await HealthCheckAsync("cars:8070");
            if (carsServiceHealth)
                car = await _httpClient.GetFromJsonAsync<CarInfo>($"http://cars:8070/cars/{rental.CarUid}");
            else
                car = Array.Empty<object>();

            object? payment;
            var paymentServiceCheck = await HealthCheckAsync("payments:8050");
            if (paymentServiceCheck)
                payment = await _httpClient.GetFromJsonAsync<PaymentInfo>($"http://payments:8050/payment/{rental.PaymentUid}");
            else
                payment = Array.Empty<object>();

            var response = new RentalResponse { RentalUid = rental.RentalUid, Status = rental.Status, DateFrom = DateOnly.FromDateTime(rental.DateFrom), DateTo = DateOnly.FromDateTime(rental.DateTo), Car = car, Payment = payment };

            return Ok(response);
        }

        [HttpPost("rental")]
        public async Task<IActionResult> RentCar([FromHeader(Name = "scopeToken"), Required] string scopeToken, [FromBody] CreateRentalRequest request)
        {
            var carsServiceHealth = await HealthCheckAsync("cars:8070");

            if (!carsServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Cars Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var carReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{request.CarUid}/rent");
            using var carRes = await _httpClient.SendAsync(carReq);

            if (carRes is null || carRes.StatusCode == HttpStatusCode.NotFound)
                return BadRequest();


            var priceReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{request.CarUid}/price");
            var priceRes = await _httpClient.SendAsync(priceReq);
            var price = await priceRes.Content.ReadFromJsonAsync<int>();
            var priceTotal = (request.DateTo.DayNumber - request.DateFrom.DayNumber) * price;

            PaymentInfo? payment;
            try
            {
                using var paymentReq = new HttpRequestMessage(HttpMethod.Post, $"http://payments:8050/payment?price={priceTotal}");
                using var paymentRes = await _httpClient.SendAsync(paymentReq);
                payment = await paymentRes.Content.ReadFromJsonAsync<PaymentInfo>();
            }
            catch
            {
                using var carUndoReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{request.CarUid}/undo_rent");
                using var carUndoRes = await _httpClient.SendAsync(carUndoReq);

                var resp = new ErrorResponse()
                {
                    Message = "Payment Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            var createRentalReqBody = new CreatePaidRentalRequest { CarUid = request.CarUid, DateFrom = request.DateFrom.ToDateTime(new TimeOnly()), DateTo = request.DateTo.ToDateTime(new TimeOnly()), PaymentUid = payment.PaymentUid };

            try
            {
                using var rentalReq = new HttpRequestMessage(HttpMethod.Post, $"http://rentals:8060/rental");
                rentalReq.Headers.Add("scopeToken", scopeToken);
                rentalReq.Content = JsonContent.Create(createRentalReqBody, typeof(CreatePaidRentalRequest));
                using var rentalRes = await _httpClient.SendAsync(rentalReq);
                if (!rentalRes.IsSuccessStatusCode)
                {
                    using var carUndoReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{request.CarUid}/undo_rent");
                    using var carUndoRes = await _httpClient.SendAsync(carUndoReq);

                    using var paymentUndoReq = new HttpRequestMessage(HttpMethod.Post, $"http://payments:8050/payment/{payment.PaymentUid}");
                    using var paymentUndoRes = await _httpClient.SendAsync(paymentUndoReq);

                    var resp = new ErrorResponse()
                    {
                        Message = "Rental Service unavailable",
                    };
                    Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return new ObjectResult(resp);
                }

                if (rentalRes.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                var rentalResponse = await rentalRes.Content.ReadFromJsonAsync<Rental>();

                return Ok(new CreateRentalResponse { RentalUid = rentalResponse.RentalUid, DateFrom = DateOnly.FromDateTime(rentalResponse.DateFrom), DateTo = DateOnly.FromDateTime(rentalResponse.DateTo), Payment = payment, Status = rentalResponse.Status, CarUid = request.CarUid });
            }
            catch
            {
                using var carUndoReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{request.CarUid}/undo_rent");
                using var carUndoRes = await _httpClient.SendAsync(carUndoReq);

                using var paymentUndoReq = new HttpRequestMessage(HttpMethod.Post, $"http://payments:8050/payment/{payment.PaymentUid}");
                using var paymentUndoRes = await _httpClient.SendAsync(paymentUndoReq);

                var resp = new ErrorResponse()
                {
                    Message = "Rental Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }
        }

        [HttpDelete("rental/{rentalUid}")]
        public async Task<IActionResult> CancelUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            var rentalServiceHealth = await HealthCheckAsync("rentals:8060");

            if (!rentalServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Rental Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var rentalsReq = new HttpRequestMessage(HttpMethod.Get, $"http://rentals:8060/rental/{rentalUid}");
            rentalsReq.Headers.Add("scopeToken", scopeToken);
            using var rentalsRes = await _httpClient.SendAsync(rentalsReq);
            if (rentalsRes.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var rental = await rentalsRes.Content.ReadFromJsonAsync<Rental>();

            if (rental is null)
                return NotFound($"User with token = {scopeToken} has no rental with UUID = {rentalUid}");

            using var carReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{rental.CarUid}/undo_rent");
            using var carRes = await _httpClient.SendAsync(carReq);

            if (carRes is null || carRes.StatusCode == HttpStatusCode.NotFound)
                return NotFound();

            using var rentalsCancelReq = new HttpRequestMessage(HttpMethod.Delete, $"http://rentals:8060/rental/{rentalUid}");
            rentalsCancelReq.Headers.Add("scopeToken", scopeToken);
            try
            {
                using var rentalsCancelRes = await _httpClient.SendAsync(rentalsCancelReq);
                if (!rentalsCancelRes.IsSuccessStatusCode)
                {
                    var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(rentalsCancelReq);
                    _requestQueueService.AddRequestToQueue(reqClone);
                }
            }
            catch
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(rentalsCancelReq);
                _requestQueueService.AddRequestToQueue(reqClone);
            }

            using var paymentReq = new HttpRequestMessage(HttpMethod.Post, $"http://payments:8050/payment/{rental.PaymentUid}");
            try
            {
                using var paymentRes = await _httpClient.SendAsync(paymentReq);
                if (!paymentRes.IsSuccessStatusCode)
                {
                    var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(paymentReq);
                    _requestQueueService.AddRequestToQueue(reqClone);
                }
            }
            catch
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(paymentReq);
                _requestQueueService.AddRequestToQueue(reqClone);
                return NoContent();
            }

            return NoContent();
        }

        [HttpPost("rental/{rentalUid}/finish")]
        public async Task<IActionResult> FinishUserRental([FromRoute] Guid rentalUid, [FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            var rentalServiceHealth = await HealthCheckAsync("rentals:8060");

            if (!rentalServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Rental Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var rentalsReq = new HttpRequestMessage(HttpMethod.Get, $"http://rentals:8060/rental/{rentalUid}");
            rentalsReq.Headers.Add("scopeToken", scopeToken);
            using var rentalsRes = await _httpClient.SendAsync(rentalsReq);
            if (rentalsRes.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            var rental = await rentalsRes.Content.ReadFromJsonAsync<Rental>();

            if (rental is null)
                return NotFound($"User with token = {scopeToken} has no rental with UUID = {rentalUid}");

            using var carReq = new HttpRequestMessage(HttpMethod.Get, $"http://cars:8070/cars/{rental.CarUid}/undo_rent");
            using var carRes = await _httpClient.SendAsync(carReq);
            carRes.EnsureSuccessStatusCode();

            using var rentalsFinishReq = new HttpRequestMessage(HttpMethod.Post, $"http://rentals:8060/rental/{rentalUid}/finish");
            rentalsFinishReq.Headers.Add("scopeToken", scopeToken);

            try
            {
                using var rentalsFinishRes = await _httpClient.SendAsync(rentalsFinishReq);
                if (!rentalsFinishRes.IsSuccessStatusCode)
                {
                    var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(rentalsFinishReq);
                    _requestQueueService.AddRequestToQueue(reqClone);
                    return NoContent();
                }
            }
            catch
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(rentalsFinishReq);
                _requestQueueService.AddRequestToQueue(reqClone);
                return NoContent();
            }

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest loginRequest)
        {
            var authServiceHealth = await HealthCheckAsync("auth:8040");

            if (!authServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Auth Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var loginReq = new HttpRequestMessage(HttpMethod.Post, "http://auth:8040/login")
            {
                Content = JsonContent.Create(loginRequest)
            };

            using var tokenResp = await _httpClient.SendAsync(loginReq);

            if (!tokenResp.IsSuccessStatusCode)
                return Unauthorized();

            var token = await tokenResp.Content.ReadFromJsonAsync<JwtToken>();

            return Ok(token);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateRequest registerRequest)
        {
            var authServiceHealth = await HealthCheckAsync("auth:8040");

            if (!authServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Auth Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var regReq = new HttpRequestMessage(HttpMethod.Post, "http://auth:8040/register")
            {
                Content = JsonContent.Create(registerRequest)
            };

            using var newIdResp = await _httpClient.SendAsync(regReq);
            var newId = newIdResp.Content.ReadFromJsonAsync<Guid>();

            return Ok(newId);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromHeader(Name = "scopeToken"), Required] string scopeToken)
        {
            var statServiceHealth = await HealthCheckAsync("statistics:8030");

            if (!statServiceHealth)
            {
                var resp = new ErrorResponse()
                {
                    Message = "Statistics Service unavailable",
                };
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(resp);
            }

            using var getRequest = new HttpRequestMessage(HttpMethod.Get, $"http://statistics:8030/statistics/get");

            try
            {
                using var getResponse = await _httpClient.SendAsync(getRequest);

                if (!getResponse.IsSuccessStatusCode)
                {
                    var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(getRequest);
                    _requestQueueService.AddRequestToQueue(reqClone);
                    return NoContent();
                }

                var resultList = await getResponse.Content.ReadFromJsonAsync<IEnumerable<string>>();
                return Ok(resultList);
            }
            catch (HttpRequestException e)
            {
                var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(getRequest);
                _requestQueueService.AddRequestToQueue(reqClone);
                return NoContent();
            }
        }

        async Task<bool> HealthCheckAsync(string base_adress)
        {
            if (_circuitBreaker.IsOpened())
                return false;

            using var req = new HttpRequestMessage(HttpMethod.Get, $"http://{base_adress}/manage/health");
            try
            {
                using var res = await _httpClient.SendAsync(req);
                _circuitBreaker.ResetFailureCount();
                return res.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                _circuitBreaker.IncrementFailureCount();
                if (_circuitBreaker.IsOpened())
                {
                    var reqClone = await HttpRequestMessageHelper.CloneHttpRequestMessageAsync(req);
                    _requestQueueService.AddRequestToQueue(reqClone);
                }
                return false;
            }
        }
    }
}
