using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost("{charge}")]
        public async Task<IActionResult> Charge(string accountFrom, string accountTo, decimal amount)
        {
            Random random = new Random();
            int externalPaymentSystemProcessingTime = random.Next(500, 2000);
            await Task.Delay(externalPaymentSystemProcessingTime);
            return new OkResult();
        }
    }
}
