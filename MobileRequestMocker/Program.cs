using MobileRequestMocker.Models;
using MobileRequestMocker.Repositories;
using MobileRequestMocker.Requests;
using MobileRequestMocker.Requests.Generators;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MobileRequestMocker
{
    class Program
    {
        const int numberOfRandomRequestsToSend = 30;
        const int maxConcurrentRequests = 5;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press 's' if you want to skip user registration or any key to start normaly");
            string enteredOption = Console.ReadLine();

            HttpClient httpClient = new HttpClient();
            UserRepository userRepository = new UserRepository();

            ServicePointManager.FindServicePoint(new Uri("http://localhost:8080")).ConnectionLimit = maxConcurrentRequests;

            if (enteredOption != "s") {
                RegisterUserRequestGenerator registerUserRequestGenerator = new RegisterUserRequestGenerator();
                AddCreditRequestGenerator addCreditRequestGenerator = new AddCreditRequestGenerator();

                foreach (User user in userRepository.Users)
                {
                    await httpClient.PostAsync(registerUserRequestGenerator.GenerateFor(user), null);
                    await httpClient.PostAsync(addCreditRequestGenerator.GenerateFor(user), null);
                }
            }

            for (int i = 0; i < numberOfRandomRequestsToSend; i++) {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(async () =>
                {
                    Request randomRequest = RandomHttpRequestGenerator.GenerateRandomRequestForTheSet(userRepository.Users);
                    if (randomRequest.Type == RequestType.HttpGet) {
                        await httpClient.GetAsync(randomRequest.Url);
                    } else
                    {
                        await httpClient.PostAsync(randomRequest.Url, null);
                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }

        }
    }
}
