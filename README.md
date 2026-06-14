# BibleStudy

A full stack Bible study application built with a .NET Web API backend
(Clean Architecture) and a React + TypeScript frontend.

## Features

The app has three connected parts, linked together by the Bible reference
(book, chapter, verse):

- **Reading plans** — Create reading plans and track progress as you mark
  each day's reading as done.
- **Bible reader** — Read the Bible text directly in the app. Text is fetched
  live from a free public Bible API and is not stored locally.
- **Notes** — Write notes tied to specific verses. Each note links back to the
  reader so the verse can be opened in its full context.

## Architecture

The backend follows Clean Architecture with four projects, where dependencies
always point inward toward the Domain:

- **Domain** — Core entities (Plan, Reading, Note). No dependencies.
- **Application** — Service and repository interfaces, DTOs, business logic.
  Depends on Domain.
- **Infrastructure** — EF Core, DbContext, repository implementations, and the
  Bible API provider. Depends on Application and Domain.
- **Api** — Controllers and dependency injection setup. Depends on Application
  and Infrastructure.

## Tech stack

- .NET 10 Web API with controllers
- Entity Framework Core with SQL Server / SQL Express
- Repository pattern with a generic repository
- xUnit and NSubstitute for unit testing
- React + TypeScript frontend

## Testing

The solution includes a separate test project using xUnit. Services are tested
in isolation by mocking the repository interfaces (and the Bible text provider)
with NSubstitute.

## Getting started

_To be added: setup instructions for running the API and frontend locally._
