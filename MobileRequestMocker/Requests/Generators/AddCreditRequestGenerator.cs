﻿using MobileRequestMocker.Models;
using System;

namespace MobileRequestMocker.Requests.Generators
{
    public class AddCreditRequestGenerator : IRequestGenerator
    {
        public string GenerateFor(User user)
        {
            Random random = new Random();
            int randomAmount = random.Next(50, 100);
            return $"{Configuration.GetInstance().ReverseProxyUrl}/Services/Orchestrator/api/credit/add?userId={user.Id}&userAccount={user.Account}&amount={randomAmount}";
        }
    }
}
