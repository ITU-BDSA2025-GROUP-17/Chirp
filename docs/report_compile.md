---
header-includes:
  - \usepackage{graphicx}
  - \usepackage{float}
  - \floatplacement{figure}{H}
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

\newpage

# Design and Architecture of _Chirp!_

## Domain model

The Chirp domain model consists of four entities:

1. Author (user extending Microsoft.AspNetCore.Identity.IdentityUser), this represents a user of the application.
2. Cheep a 160-character message with a timestamp, which an author can create and post on the Chirp social platform.
3. Follow enables authors to follow each other and see followed users cheeps on their own timeline.
4. SavedCheep are Cheeps saved by the user.

The model implements a blogging platform with social features including following and timeline feeds.

\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/Chirp.Core.png}
\caption{Illustration of the \textit{Chirp!} data model as UML class diagram.}
\end{figure}

## Architecture — In the small

The diagram shown below illustrates the program's onion architecture. The application generally follows the onion structure even though some layers are represented by more than one .NET project. The Chirp.Core .NET project is the core onion layer, on top of that is the Chirp.Repositories .NET project layer. Here the DTO's exist as they define the data contracts used across the repository, services and representation layers.
The Repositories layer providing an abstraction layer on top of the database, making actions like adding or removing users be done through single method calls.

Ontop of the repositories layer is the Chirp.Services .Net project layer, which provides an additional layer of abstraction on top of the Chirp.Repositories layer, where sometimes multiple actions are taken through one method call, like deleting all cheeps by a user before deleting the user itself.

The service and repository layers are located withing a shared folder called Chirp.Infrastructures. The outermost layer contains the frontend Razor Pages, called Chirp.Web, and the application tests.

\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{images/onion_arc.png}
\caption{Onion Architecture}
\end{figure}

### Chirp.Web

- ASP.NET Core Razor Pages
- Controllers and page models
- HTTP concerns and routing
- User interface (HTML/CSS)
- Depends on all inner layers

The reason for Chirp.Web depending on Chirp.Core is that ASP.NET Core Identity Pages require access to Author to function.

### Chirp.Infrastructure.Services

- Service implementations: CheepService, AuthorService
- Defines use-cases for the database operations from repositories
- Workflows for database operations
- Depends on Chirp.Infrastructure.Repositories

### Chirp.Infrastructure.Repositories

Repository Layer / Data Access

- Contains CheepRepository, AuthorRepository implementations
- Database context (CheepDBContext)
- DTO: CheepDTO, AuthorDTO
- Defines small individual operations on the database
- Depends on Chirp.Core

### Chirp.Core

DataModel / Domain Layer (Core)

- Contains domain entities: Author, Cheep, Follow, SavedCheep
- Pure domain logic, no dependencies in Chirp
- The innermost layer with business rules

## Architecture of deployed application

The Chirp application is hosted on Azure App Service. Users interact with the system through the Chirp.Web project, which provides the user interface using ASP.NET Core Razor Pages. All client interaction happens over HTTPS. When a user performs an action in the UI, Chirp.Web delegates the requests to the service layer in Chirp.Infrastructure.Services, where the possible database operations are implemented. The Service layer then calls the repository layer in Chirp.Infrastructure.Repositories to retrieve or modify data. Data persistence are handled via Entity Framework Core, which communicates with an SQLite database through the CheepDbContext, handled by ASP.NET Core Identity.

Autentication is handled in two ways: users can either register with ASP.NET Core Identity using an email, username and password and log in locally using with username and password after they have confirmed their account, or authenticate via GitHub OAuth - here GitHub manages the OAuth flow and returns authentication tokens to Chirp.Web.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/Componentdiagram.png}
\caption{Deployed Components}
\end{figure}

## User activities

When a user visits the application, they start on the public timeline, where they can browse cheeps, navigate between pages, search for content and view other user's timelines by clicking on the author name. From the public timeline, the user can choose to register or log-in. Registration and login can be performed either locally or via GitHub, where OAuth handles autentication and account creation externally.

If the user encounters an issue during registration or login, they are redirected back to the relevant page to retry. Once authentication succeeds, the user becomes logged in and gains access to additional features. Logged-in users can post new cheeps, follow other users and save cheeps. They can also access their personal information through the "About Me" section, where they have the option to delete their account and all associated data using the "Forget Me" functionality.

### Non-authorized user

An unauthortized user has limited access to Chirp!'s functionality. They can view all cheeps under the public timeline, and can search for cheeps containing a specefic substring. They can also click on a users name and view that persons cheeps.

To get authorized a user must register an account if they haven't already and then log in.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/LoggedOutFlow.png}
\caption{Non-authorized user activities}
\end{figure}

### Authenticated user

Authenticated users have access to the same pages as unauthorized users, with some additional pages only for the authenticated.
'My timeline' is one of these pages. On here users can view their own cheeps, as well as the cheeps from users that they follow.

Cheeps can be saved and then viewed under the 'saved' page. The cheeps are ordered by time of saving.

The user can view who they are following under the 'following' page. Here they can also choose to unfollow users.

'About me' can be accessed to view information about the users account, such as email or username. It is also here that the user can delete their account.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/LoggedInFlow.png}
\caption{Authorized user activities}
\end{figure}

\newpage

## Sequence of functionality/calls trough _Chirp!_

### Register sequence

To register a user must give an email, username and password. The email and username must be unique from other users. Accounts are handled by ASP.NET Core Identity.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/RegistrationSequence.png}
\caption{Register sequence}
\end{figure}

\newpage

### Authentication with Git-Hub

If the user does not wish to create an account using an email address, then they can choose to register/login with Github through OAuth. When choosing this option the user does not have to provide email, username or password, as all needed information is given by Github.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/GithubAuthSequence.png}
\caption{Authorizing with Github}
\end{figure}

\newpage

### Login sequence

Once an account is created the user must log in. This is done using the selected username and password. If using Github then this step is replaced by Github authentication through OAuth.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/LoginSequence.png}
\caption{Login sequence}
\end{figure}

\newpage

### Non-authorized user reading the public timeline

An unauthorized user starts by sending an HTTP GET/ request to Chirp.Web. The request invokes the public page handler. The Web layer delegates the request to the service layer, which retrieves public cheeps through the repository layer. The repository queries the SQLite database via Entity Framework Core and returns the most recent cheeps as DTO's (ordered by the timestamps). These are passed back through the service to the web layer, where Razor Pages renders the HTML response. The fully rendered public timeline pages is then returned to the user.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/ViewPublicTimelineSequence.png}
\caption{Getting public timeline sequence}
\end{figure}

\newpage

### Authorized user posting a cheep

An authorized user submits a new cheep by sending an HTTP POST request. The request is handled by Chirp.Web which extracts the authenticated username from the user's identity. The Web layer calls the service layer to create a new cheep for the user. The service resolves the author via the repository and then persists the new cheep through the cheeps repository using Entity Framework Core. After the database transaction succeeds, the user is redirected back to the public timeline, where the newly created cheep is now visible.
\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{diagrams/PostCheepSequence.png}
\caption{Posting a cheep sequence}
\end{figure}

\newpage

# Process

## Build, test, release and deployment

### Build

When the code is pushed to the main branch, the CI pipeline checks out the repository, sets up the correct .NET SDK, restores dependencies and build the application. This ensures the code compiles correctly in a clean environment.
\begin{figure}[H]
\centering
\includegraphics[width=0.3\textwidth]{diagrams/Activity_workflow_diagram.png}
\caption{Workflow sequence}
\end{figure}

\newpage

### Test

After a successful build, automated tests are executed. This includes unit tests, integration tests and end-to-end tests which verify that the repositories and application logic behave as excepted. The goal is to catch errors before code is merged or released. This is done by the workflow called `main.yml`.
\begin{figure}[H]
\centering
\includegraphics[width=0.7\textwidth]{diagrams/WorkflowTests.png}
\caption{Build and Test workflow}
\end{figure}

\newpage

### Release

Once the build and test succeed, a release workflow packages the application in Release mode. The app is published as a self-contained, single-file executable, with a version-tag and uploaded as a GitHub Release artifact. This is handled by the workflow `release.yml`.
\begin{figure}[H]
\centering
\includegraphics[width=0.49\textwidth]{diagrams/WorkflowRelease.png}
\caption{Release workflow}
\end{figure}

\newpage

### Deployment

In the final stage, the deployment workflow publishes the application to Azure App Service. It authenticates securely with Azure, uploads the built artifact and deploys it to the production environment. This keeps the live application automatically synchronized with the main branch. Deployment is handled automatically by the workflow `main_bdsagroup17chirpremotedb.yml`. Deploys to \url{https://bdsagroup17chirpremotedb-dhg0b9fpaya0afa0.swedencentral-01.azurewebsites.net/}

\begin{figure}[H]
\centering
\includegraphics[width=0.44\textwidth]{diagrams/WorkflowDeploy.png}
\caption{Deploy workflow}
\end{figure}

\newpage

## Team work

### Project board

\begin{figure}[H]
\centering
\includegraphics[width=0.9\textwidth]{images/Project_board.png}
\caption{Screenshot of the GitHub Project board before hand-in.}
\end{figure}

We used a GitHub Project board to track and manage all development tasks throughout the project.
Each task was created as a GitHub Issue was moved across the board as work progressed.
The typical workflow for implementing a feature was:

1. A new Issue is created describing the task and acceptance criteria.
2. The Issue is moved to **In progress** when development begins.
3. The feature is implemented on a separate branch.
4. A Pull Request is opened against the `main` branch.
5. Automated CI workflows run build and test pipelines.
6. After review and successful checks, the Pull Request is merged into `main`.
7. The Issue is moved to **Done** on the project board.
   We used a GitHub Project board to track and manage all development tasks throughout the project.
   Each task was created as a GitHub Issue and moved across the board as work progressed.

Most tasks are marked as **Done** at the time of hand-in. The remaining unresolved tasks are the following:

- **AuthorRepository behavior**:  
  The repository currently returns `null` when an author is not found instead of throwing an exception.  
  It is not yet decided whether this represents the expected control flow or an exceptional case.  
  Additional tests are required to validate the chosen behavior and ensure all callers handle it safely.

- **OAuth login refresh issue**:  
  After authenticating via GitHub OAuth the user currently needs to reload the page before the login state takes effect.  
  The expected behavior is that authentication is reflected immediately without a manual reload.

### Development flow

Early in the development of Chirp it was decided that the whole group would work collectively on all the tasks. We felt that many of the tasks depended on each other, and since we had enough time to complete most tasks every week it was best for everyone if we as a group did everything together. This is shown when commiting new code to the project by all present team members being co-authored.
As a consequence of this way of working, most code reviews on the pull requests are sparse, as we all watched the code being written and pitched in, meaning there wasn't much need for further communication in regards to the code.

#### Issues

When starting work on a new set of weekly tasks we began by making issues on Github for each of the tasks, which we would then work through during the week. On account of our way of doing teamwork, where everyone was involved most of the time, pull requests would rarely not be approved, but this option is still illustrated in the diagram below.
\begin{figure}[H]
\centering
\includegraphics[width=1\textwidth]{diagrams/IssueFlow.png}
\caption{Flow diagram from issue creation to resolution}
\end{figure}

## How to make _Chirp!_ work locally

### Prerequirements:

- .NET 9 SDK installed
- Git

### Steps

1. Clone the repository: `git clone https://github.com/ITU-BDSA2025-GROUP-17/Chirp`
2. After the cloning the project, go to the project:
   `cd Chirp`
3. Restore dependencies:
   `dotnet restore src/Chirp.Web/Chirp.Web.csproj`

4. Run the application

If you wish to access the application with Github OAuth (This also requires secrets to be set up):

`dotnet run --project src/Chirp.Web/Chirp.Web.csproj --launch-profile https`

else:

`dotnet run --project src/Chirp.Web/Chirp.Web.csproj`

1. Access the application

   - Open browser and navigate to: http://localhost:7273 or https://localhost:7273 if using https launch profile
   - You should see the Chirp public timeline with seeded cheeps

Notes:

- The application can be run locally without configuring GitHub authentication.
- GitHub login will not work locally unless user secrets are configured.
- This does not affect core functionality such as browsing cheeps or local authentication.

#### GitHub authentication

GitHub authentication relies on OAuth secrets which are not stored in the repository.
To enable GitHub login locally, user secrets must be configured manually:

```bash
dotnet user-secrets set "Authentication:GitHub:ClientId"
   "<your-client-id>" --project src/Chirp.Web
dotnet user-secrets set "Authentication:GitHub:ClientSecret"
   "<your-client-secret>" --project src/Chirp.Web
```

These secrets are provided via GitHub OAuth and are intentionally not included in the repository.
In the deployed Azure environment, the secrets are configured securely using Azure App Service settings.

## How to run test suite locally

To succeed in running the test suite locally it is required to have PlayWright installed.

#### Unit tests:

##### AuthorRepositoryTests.cs

This test suite contains author-related repository tests that verify database operations using an in-memory SQLite database.
The tests cover creating authors, retrieving authors by name and email, following/unfollowing users, checking follow relationships, retrieving followed authors and deleting authors, including verification that related follow data is also removed.

##### CheepRepositoryTests.cs

This test suite focuses on Cheep-related repository operations.
It includes tests for creating and reading cheeps, retrieving cheeps from followed users, pagination, searching, saving and removing saved cheeps, retrieving cheeps by ID and validating enforcement of the 160-character limit.

\
`dotnet test test/Chirp.Repositories.Tests`

#### Integration tests:

This test suite focuses on integration tests which focus on verifying how multiple components work together including the web-, service-, repository layers as wll as the database.

The tests are implemented using the `WebApplicationFactory` pattern to run the application in an in-memory test environment.  
`BasicIntegrationTests.cs` contains HTTP-level tests that verify the public pages return successful responses and include the expected content.  
`DatabaseIntegrationTests.cs` focuses on the behavior of the program on a service-layer, covering retrieval of authors, following and unfollowing, Cheep creation and retrieval with pagination, search functionality, saved Cheeps, and user timeline features using seeded test data.  
`ManualSetupIntegrationTests.cs` was a manual `TestServer` setup for learning and validation purposes, and was used to better understand the requirements before implementing the primary integration tests.

\
`dotnet test test/Chirp.IntegrationTests`

#### End-to-end tests (requires Playwright):

This suite contains UI-based end-to-end tests using Playwright that works on the web application and simulate real user interactions in a browser. Within tests.cs there is created an automated browser test, that covers reading cheeps from the UI, search functionality, pagination navigation, viewing user timelines, complete user registration/login/logout flows, follow/unfollow interactions and posting new cheeps.

\
Windows (using powershell)

```
cd test/ChirpEndToEndTests
dotnet build
bin/Debug/net9.0/playwright.ps1 install
dotnet test
```

Mac/Linux (install powershell)

```
cd test/ChirpEndToEndTests
dotnet build
pwsh ./bin/Debug/net9.0/playwright.sh install
dotnet test
```

# Ethics

## License

The Chirp! Project is released under the MIT license. This is a permissive open-source license that allows others to use, modify, distribute and build upon the software with very few restrictions. The only requirement is that the original copyright notice and license text are included in any copies or substantial portions of the software. The software is provided "as is", with any warranty, which means the developers are not liable for potential issues arising from its use.

## LLMs, ChatGPT, CoPilot, and others

During development of the project, we used several Large Language Models (LLMs), including ChatGPT, GitHub Copilot and Claude.

ChatGPT was used as a support tool for understanding code and concepts. Use cases included clarifying the meaning of specific lines of code, helping with how to write smaller code lines and explaining why certain methods or implementations caused issues, replacing the need to searching through documentation or Stack Overflow in most cases. This helped save time and allowed us to focus more on understanding and writing the code ourselves. Furthermore, ChatGPT was used to help write documentation for the project. It was also used in a theoretical manner to discuss architectural and conceptual decisions before implementation, ensuring that we had a solid understanding before writing code. Additionally, it was used to help rephrase or improve issue descriptions and parts of the written report. 

Claude was used as a kind of teaching assistant after the code had already been reviewed within the group and there was still uncertainty about the solution. We intentionally used it for guidance rather than complete answers. It was also helpful when working with tests, especially for interpreting and applying the guidelines from the course literature when implementing integration tests. In a few cases, small code snippets were used as inspiration rather than finished solutions.

GitHub Copilot was used passively during development, mainly by automatically generating commit or pull request messages, some of which were accepted.

Overall LLMs were used as supportive tools rather than sources of complete solutions. They helped speed up development, reduce time spent on searching for information and improve understanding, while the main implementation and problem solving were still thought out by the group.
