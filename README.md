# techdays-2017-demo
Demo project for the Techdays 2017 presentation: "Be prepared for the intelligent agent march"

It is a transportation grid, where agents can come in and arrange their transport.

When time permits, independent cars agents for self-driving cars can enter to offer their services as well.

And when there is playtime: add scam agents that pretend to be legitimate self-driving cars in order to take the user agent's money.

## Components to be implemented
* A delegated identity Ethereum Contract (Solidity)
* A public Customs Microservice (Web API, C#)
* A private agenda Microservice (Web API, C#)
* A private payment Microservice (Web API, C#)
* A private reservation Microservice (Web API, C#)
* A user agent that is spawned on user request (C#)

### Customs Service
Used to grant access inside the network. An agent that has a delegated identity is validated, and 'stamped'. Its contract is extended so all agents and services know this agent is acknowledged.

### Agenda Service
It provides information on availability of transportation means. This information is fueled from various transportation providers, both companies and individuals.

### Payment Service
Pay all open tabs, so the services will be provided to the end user.

### Reservation Service
Make all required reservations, so user is provided with transportation when needed.

### User Agent
An agent sent by some user, that is supposed to enter the transportation grid with a cryptocurrency budget and a delegated identity token, and arrange transportation for the coming month. It is supposed to make intelligent decisions and an optimal match given the available budget and any constraints.

## Products used to create this demo
* Truffle - The Ethereum blockchain bootstrapper, interface and testing framework
* TestRPC - Used as local ethereum node, for rapid development
* Nethereum - NuGet package to interface with ethereum from .Net code
* Azure - Hosting a private ethereum network
* Azure Service Fabric - the microservices and agents landscape
