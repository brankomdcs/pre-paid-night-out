using MobileRequestMocker.Models;
using MobileRequestMocker.Requests.Generators;
using System;
using System.Collections.Generic;

namespace MobileRequestMocker.Requests
{
    public class RandomHttpRequestGenerator
    {
        public static Request GenerateRandomRequestForTheSet(List<User> listOfUsers) {
            Random random = new Random();
            User randomUser = listOfUsers[random.Next(0, listOfUsers.Count)];

            List<IRequestGenerator> requestGenerators = null;

            RequestType requestType = (RequestType) random.Next(0, 2);
            switch (requestType) {
                case RequestType.HttpGet:
                    requestGenerators = new List<IRequestGenerator>() { new GetUserRequestGenerator(), new GetBalanceRequestGenerator(), new GetHistoryRequestGenerator() };
                    break;
                case RequestType.HttpPost:
                    requestGenerators = new List<IRequestGenerator>() { new AddCreditRequestGenerator(), new ChargeUserRequestGenerator() };
                    break;
            }

            IRequestGenerator randomGenerator = requestGenerators[random.Next(0, requestGenerators.Count)];
            return new Request(requestType, randomGenerator.GenerateFor(randomUser));
        }
    }
}
