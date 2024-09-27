using DS_Project.Payments.Entity;
using DS_Project.Payments.Service;
using DS_Project.Payments.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DS_Project.Payments.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PaymentController(IPaymentService paymentService, LogsProducer producer) : Controller
    {
        readonly IPaymentService _paymentService = paymentService;
        readonly LogsProducer _producer = producer;

        [HttpPost("payment")]
        public async Task<IActionResult> CreatePayment([FromQuery] int price)
        {
            var item = new Entity.Payment() { PaymentUid = Guid.NewGuid(), Price = price, Status = "PAID" };

            await _paymentService.CreateAsync(item);
            await _producer.Produce($"[PaymentService]: create payment with uid = {item.Id} and price = {price}");
            return Ok(new PaymentInfo() { PaymentUid = item.PaymentUid, Price = item.Price, Status = item.Status });
        }

        [HttpGet("payment/{paymentUid}")]
        public async Task<IActionResult> GetPayment(Guid paymentUid)
        {
            var item = await _paymentService.GetAsync(paymentUid);
            await _producer.Produce($"[PaymentService]: get payment with uid = {paymentUid}");
            return item is not null ? Ok(new PaymentInfo() { PaymentUid = item.PaymentUid, Price = item.Price, Status = item.Status }) : NotFound();
        }

        [HttpPost("payment/{paymentUid}")]
        public async Task<IActionResult> CancelPayment(Guid paymentUid)
        {
            var item = await _paymentService.GetAsync(paymentUid);

            item!.Status = "CANCELED";

            await _paymentService.Update(item);
            await _producer.Produce($"[PaymentService]: cancel payment with uid = {paymentUid}");
            return Ok();
        }
    }
}
