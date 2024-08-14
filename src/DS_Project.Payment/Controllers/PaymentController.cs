using DS_Project.Payments.Entity;
using DS_Project.Payments.Service;
using DS_Project.Payments.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Payments.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController : Controller
    {
        readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment([FromQuery] int price)
        {
            var item = new Payment() { PaymentUid = Guid.NewGuid(), Price = price, Status = "PAID" };

            await _paymentService.CreateAsync(item);

            return Ok(new PaymentInfo() { PaymentUid = item.PaymentUid, Price = item.Price, Status = item.Status });
        }

        [HttpGet("payment/{paymentUid}")]
        public async Task<IActionResult> GetPayment(Guid paymentUid)
        {
            var item = await _paymentService.GetAsync(paymentUid);

            return item is not null ? Ok(new PaymentInfo() { PaymentUid = item.PaymentUid, Price = item.Price, Status = item.Status }) : NotFound();
        }

        [HttpPost("payment/{paymentUid}")]
        public async Task<IActionResult> CancelPayment(Guid paymentUid)
        {
            var item = await _paymentService.GetAsync(paymentUid);

            item!.Status = "CANCELED";

            await _paymentService.Update(item);

            return Ok();
        }
    }
}
