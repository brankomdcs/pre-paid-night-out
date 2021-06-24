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
        static int numberOfRandomRequestsToSendInOneBatch = 100;
        static int maxConcurrentRequests = numberOfRandomRequestsToSendInOneBatch;
        static async Task Main(string[] args)
        {
            Console.WriteLine("- Enter 'y' if you would like to run automatical user registration (mandatory for running the app for the first time)\n" +
                              "- Enter anything else to skip the registration.");
            bool registerUsersStepSelected = Console.ReadLine() == "y";

            HttpClient httpClient = new HttpClient();
            UserRepository userRepository = new UserRepository();

            ServicePointManager.FindServicePoint(new Uri("http://localhost:19801")).ConnectionLimit = maxConcurrentRequests;

            if (registerUsersStepSelected) {
                RegisterUserRequestGenerator registerUserRequestGenerator = new RegisterUserRequestGenerator();
                AddCreditRequestGenerator addCreditRequestGenerator = new AddCreditRequestGenerator();

                foreach (User user in userRepository.Users)
                {
                    await httpClient.PostAsync(registerUserRequestGenerator.GenerateFor(user), null);
                    await httpClient.PostAsync(addCreditRequestGenerator.GenerateFor(user), null);
                }
            }

            Console.WriteLine($"- Enter 'b' to send random requests in batches\n" +
                              $"- Enter 'l' to send them in infinite loop\n" +
                              $"- Enter anything else to terminate.");
            string option = Console.ReadLine();

            if (option == "b")
            {
                Console.WriteLine("Enter number of concurent request you would like to send in one batch:");
                numberOfRandomRequestsToSendInOneBatch = int.Parse(Console.ReadLine());

                Console.WriteLine($"Enter 'y' to send the batch of {numberOfRandomRequestsToSendInOneBatch} randomly generated requests or anything else to terminate.");
                while (Console.ReadLine() == "y")
                {
                    for (int i = 0; i < numberOfRandomRequestsToSendInOneBatch; i++)
                    {
                        SendRandomRequest(httpClient, userRepository);
                    }
                    Console.WriteLine($"Enter 'y' to send the batch of {numberOfRandomRequestsToSendInOneBatch} randomly generated requests or anything else to terminate.");
                }
            }
            else if (option == "l") {
                while (true) {
                    SendRandomRequest(httpClient, userRepository);
                }
            }
            

        }

        private static void SendRandomRequest(HttpClient httpClient, UserRepository userRepository)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                Request randomRequest = RandomHttpRequestGenerator.GenerateRandomRequestForTheSet(userRepository.Users);
                if (randomRequest.Type == RequestType.HttpGet)
                {
                    await httpClient.GetAsync(randomRequest.Url);
                }
                else
                {
                    await httpClient.PostAsync(randomRequest.Url, null);
                }
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
