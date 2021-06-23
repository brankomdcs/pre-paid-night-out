﻿using MobileRequestMocker.Models;

namespace MobileRequestMocker.Requests.Generators
{
    public class GetBalanceRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            return $"http://localhost:8080/api/transaction/balance?userId={user.Id}";
        }
    }
}
