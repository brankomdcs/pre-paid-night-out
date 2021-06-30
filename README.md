# pre-paid-night-out
Application designed to demonstrate some of the main features of Service Fabric.

PPNO is application designed to manage the expanses of night outs. It supports to different roles of users:

* Person can sign up and register their credit card so they could buy the credit in advance which they are going to spend in bars, restaurants, concerts using their mobile phones. Once the credit is bought, person use their mobile phone to pay drinks until they ran out of the credit. Person is able to see all the transactions from the last night for example, inspect charges, buy more credit etc.
* Organizations (such as bars, restaurants, festivals, concerts) can sign up to create an account in PPNO so they could offer the service of buying drinks over mobile phones for their customers.

PPNO is an on-boarding app which simulates real-life application using Service Fabric, architectured in micro-service oriented manner. For this purpose, mobile client is out of the scope and dedicated service is created to mock the HTTP requests coming from the users mobile phones. Integration with payment systems (like Stripe or PayPal) are obviously mocked as well. Users authentication and authorization are also out of the scope. Due to the simplicity, application design assumes orchestration pattern rather than choreography and coding practices are adjusted to application purpose.

PPNO application consist of the following components (services):

![alt text](https://github.com/brankomdcs/pre-paid-night-out/blob/master/architecture_diagram.jpeg?raw=true)

1. Orchestrator - Service responsible for receiving requests from the client, orchestrating other services work (or retrieving state) and contining all the domain logic.
2. Account - Service responsible for managing users and their account details.
3. Transaction - Service responsible for storing all the transactions in the "event sourcing" manner and retrieving transaction history and balance for the users.
4. Payment - Service responsible for integration with 3rd party payment providers - mocked to wait random time (0.5s - 2s) to simulate communication with external components.
5. MobileRequestMocker - Console application designed to send random requests (get user, add credit, pay, retrieve history) for random users with configurable frequency.
