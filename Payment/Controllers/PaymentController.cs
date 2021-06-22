using Microsoft.AspNetCore.Mvc;
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
            // 3RD PARTY PAYMENT COMPONENT INTEGRATION SIMULATION (3s):
            Thread.Sleep(1000);
        }
    }
}
