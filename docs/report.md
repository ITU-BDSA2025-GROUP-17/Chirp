---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2025 Group 17
author:
  - "Peter Dahl Hæstrup <phae@itu.dk>"
  - "Athena Winther <atwi@itu.dk>"
  - "Rasmus Bondo <rabh@itu.dk>"
  - "Ditte Lobo <dsab@itu.dk>"
  - "Nikolaj Schiang <nsee@itu.dk>"

numbersections: true
---

# Design and Architecture of _Chirp!_

## Domain model

The Chirp domain model consists of four entities: Author(users, extending ASP.NET Identity), Cheep (160-character messages with timestamps), Follow (author-to-author relationship), and SavedCheep (messages saved by user). The model implement a blogging platform with social features including following and timeline feeds. Reposititory interfaces (ICheepRepository, IAuthorRepository) provide data access abstraction with support for pagination, search and deletion.

![Illustration of the _Chirp!_ data model as UML class diagram.](docs/images/domain_model.png)

Provide an illustration of your domain model. Make sure that it is correct and complete. In case you are using ASP.NET Identity, make sure to illustrate that accordingly.

## Architecture — In the small

The diagram above illustrates the program's onion architecture. The application generally follows the onion structure even though some layers are represented by more than one .NET project. The Core .NET project is the core onion layer and the Infrastructure .NET project is split across both the repository layer and the service layer. The DTO's exist in the repository layer (Chirp.Infrastructure.Repositories) as they define the data contracts used across the repository, services and representation layers. The outermost layer contains the frontend Razor Pages and the end-to-end tests.

![Onion Architecture](/images/onion_arc.png)

### Chirp.Web

- ASP.NET Core Razor Pages
- Controllers and page models
- HTTP concerns and routing
- User interface (HTML/CSS)
- Depends on all inner layers

### Chirp.Infrastructure.Services

DataTransferObjects = Application Services Layer

- Service implementations: CheepService, AuthorService
- business logic orchestration
- Use cases and workflows
- Depends on Chirp.Core, Chirp.Infrastructure.Repositories

### Chirp.Infrastructure.Repositories

Repository Layer / Data Access

- Contains CheepRepository, AuthorRepository implementations
- Database context (CheepDBContext)
- DTO: CheepDTO, AuthorDTO
- Data persistence and database operations
- Depends: Chirp.Core

### Chirp.Core

DataModel (Pink/Center) = Domain Layer (Core)

- Contains domain entities: Author, Cheep, Follow, SavedCheep
- Pure domain logic, no dependencies
- The innermost layer with business rules

## Architecture of deployed application

The Chirp application is hosted on Azure App Service. Users interact with the system through the Chirp.Web project, which provides the user interface using ASP.NET Core Razor Pages. All client interaction happens over HTTPS. When a user performs an action in the UI, Chirp.Web delegates the requests to the service layer in Chirp.Infrastructure.Services where the business logis is implemented. The Service layer then calls the repository layer in Chirp.Infrastructure.Repositories to retrieve or modify data. Data persistence are handled via Entity Framework Core, which communicates with an SQLite database through the CheepDbContext.

Autentication is handled in two ways: users can either register and log in locally using ASP.NET Core Identity with a username and password after they have confirmed their account, or authenticate via GitHub OAuth - here GitHub manages the OAuth flow and returns authentication tokens to Chirp.Web.

## User activities

When a user visits the application, they start on the public timeline, where they can browse cheeps, navigate between pages, search for content and view other user's timelines by clicking on the author name. From the public timeline, the user can choose to register or log-in. Registration and login can be performed either locally or via GitHub, where OAuth handles autentication and account creation externally.

If the user encounters an issue during registration or login, they are redirected back to the relevant page to retry. Once authentication succeeds, the user becomes logged in and gains access to additional features. Logged-in users can post new cheeps, follow other users and interact more actively with content. They can also access their personal information through the "About Me" section, where they have the option to delete their account and all associated data using the "Forget Me" functionality.

### non-authorized user

photo

### authenticated user

photo

## Sequence of functionality/calls trough _Chirp!_

### non-authorized user

An unauthorized user starts by sending an HTTP GET/ request to Chirp.Web. The request invokes the public page handler. The Web layer delegates the request to the service layer, which retrieves public cheeps through the repository layer. The repository queries the SQLite database via Entity Framework Core and returns the most recent cheeps as DTO's (ordered by the timestamps). These are passed back through the service to the web layer, where Razor Pages renders the HTML response. The fully rendered public timeline pages is then returned to the user.

photo

### authenticated user

An authorized user submits a new cheep by sending an HTTP POST request. The request is handled by Chirp.Web which extracts the authenticated username from the user's identity. The Web layer calls the service layer to create a new cheep for the user. The service resolves the author via the repository and then persists the new cheep through the cheeps repository using Entity Framework Core. After the database transaction succeeds, the user is redirected back to the public timeline, where the newly created cheep is now visible.

photo

# Process

## Build, test, release, and deployment

### build

When the code is pushed to the main branch, the CI pipeline checks out the repository, sets up the correct .NET SDK, restores dependencies and build the application. This ensures the code compiles correctly in a clean environment.

### test

After a successful build, automated tests are executed. This includes unit tests, integration tests and end-to-end tests which verify that the repositories and application logic behave as excepted. The goal is to catch errors before code is merged or released.

### release

Once the build and test succeed, a release workflow packages the application in Release mode. The app is published as a self-contained, single-file executable, versioned with a tag and uploaded as a GitHub Release artifact.

### deployment

In the final stage, the deployment workflow publishes the application to Azure App Service. It authenticates securely with Azure, uploads the built artifact and deploys it to the production environment. This keeps the live application automatically synchronized with the main branch.
--- UML activity diagram

## Team work

Show a screenshot of your project board right before hand-in. Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the main branch of your repository.

## How to make _Chirp!_ work locally

1. Prerequirements:

- .NET 9 SDK installed
- Git

2. Run the following commands in the terminal:

`git clone https://github.com/ITU-BDSA2024-GROUP17/Chirp.git`

3. After the cloning the project, go to the project:

   `cd Chirp`

4. Restore dependencies:

`dotnet restore src/Chirp.Web/Chirp.Web.csproj`

5. Run the application

`dotnet run --project src/Chirp.Web/Chirp.Web.csproj`

6. Access the application

   - Open browser and navigate to: http://localhost:5273 or https://localhost:7273
   - You should see the Chirp public timeline with seeded cheeps

   **\*** USER SECRETS !???

## How to run test suite locally

Unit tests:

`dotnet test test/Chirp.Repositories.Tests`

Integration tests:

`dotnet test test/Chirp.IntegrationTests`

End-to-end tests (requires Playwright):

```cd test/ChirpEndToEndTests
dotnet build
pwsh bin/Debug/net9.0/playwright.ps1 install
dotnet test
```

# Ethics

## License

The Chirp! Project is released under the MIT license. This is a permissive open-source license that allows others to use, modify, distribute and build upon the software with very few restrictions. The only requirement is that the original copyright notice and license text are included in any copies or substantial portions of the software. The software is provided "as is", with any warranty, which means the developers are not liable for potential issues arising from its use.

## LLMs, ChatGPT, CoPilot, and others

State which LLM(s) were used during development of your project. In case you were not using any, just state so. In case you were using an LLM to support your development, briefly describe when and how it was applied. Reflect in writing to which degree the responses of the LLM were helpful. Discuss briefly if application of LLMs sped up your development or if the contrary was the case.
