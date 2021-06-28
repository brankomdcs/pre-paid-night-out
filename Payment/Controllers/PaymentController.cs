using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
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
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Random random = new Random();
            int externalPaymentSystemProcessingTime = random.Next(500, 2000);
            await Task.Delay(externalPaymentSystemProcessingTime);

            stopwatch.Stop();
            Payment.RegisterRequestForMetrics(stopwatch.ElapsedMilliseconds);

            return new OkResult();
        }
    }
}
