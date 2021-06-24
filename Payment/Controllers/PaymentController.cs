using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;

namespace Payment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        [HttpPost("{charge}")]
        public void Charge(string accountFrom, string accountTo, decimal amount)
        {
            Random random = new Random();
            int externalPaymentSystemProcessingTime = random.Next(200, 2000);
            Thread.Sleep(externalPaymentSystemProcessingTime);
        }
    }
}
