# Mine Backend Engineer Hiring Assignment

## Briefing

* I should scrape all the emails that are mentioned in privacy contracts, or that mentioned in a privacy related context.

This repo contains a coding assignment, which reflects some of the challanges a backend engineer at Mine is expected to solve.

In this assignment, you will implement a scraper based on [Selenium Web Driver](https://www.selenium.dev/documentation/en/webdriver/), that scrapes websites for email addresses. We already provided an example of running Selenium through C# in the `SeleniumExample` class. TODO: I should do the tests in a seperate project, but I don't have to use
                                                                                the files in the example.

To do this, you will be required to implement a few components:

First, provide an implementation for the existing `IScraper` interface, with your own scraper class. TODO: I can completely change the interface according to my preferences.

Second, you will be required to implement `Program.ScrapeAllDomains()` method that runs your scraper on the list of domains and stores the results in the provided collection.
This collection simulates a database, so feel free to expand it with any properties or data you find usefull for debugging, analysis, troubleshooting etc.
TODO: I can change the DB type to any type that I want, although the current one seems to fit.

Finally, verifying that your code works is an integral part of our development process, so you are also expected to add some unit tests in the `ScraperTests` project. This project is based on the [NUnit](https://docs.nunit.org/) framework. TODO: I can change the library to a different one, but I need to have a solid motivation.

### Things to pay attention to:
- Functionality - the code works across a range of acceptable use-cases.
- Edge cases & exception handling.
- Performance - Write code that is efficient, fast and can scale well.
- Efficient resource management - memory, threads, connections, tasks etc.
- Logging. TODO: I should log exceptions with "error" msg, and important operations with "info", and I can also log parts which 
                 I'd want have info about them for prob detection, with "debug" (but I don't want to log with "debug").
- Readable, maintainable code with consistent naming conventions - You can follow the [.Net Core Naming Guidlines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines) TODO: Naming conventions aren't important to them, but only to keep the consistency by using the conventions that are already applied in the given files.


## Development Environment

You will need to have [.Net Core SDK 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1) to run this project.

You will also need to have [Google Chrome](https://www.google.com/chrome/) version 84 installed, to allow Selenium to work correctly. If you want, you can change the version of the Selenium Web Driver Nuget package to different version, if that is more comfortable. 

As an IDE, you can use Microsoft's free cross-platform editor [VSCode](https://code.visualstudio.com/), [JetBrain's Rider](https://www.jetbrains.com/rider/) free-trial or anything else you prefer.
