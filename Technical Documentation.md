# "Blazor App - Homework" App - Technical Documentation

## Table of Contents
- [Introduction](#introduction)
- [Architecture](#architecture)
- [Implementation Details](#implementation-details)
  - [Frontend (Blazor WebAssembly)](#frontend-blazor-webassembly)
  - [Backend (ASP.NET Core Web API)](#backend-asp-net-core-web-api)
  - [Shared Models](#shared-models)
  - [Unit Tests](#unit-tests)
- [Future Improvements](#future-improvements)

## Introduction
The "Blazor App - Homework" App is a full-stack web application built with Blazor WebAssembly (frontend) and ASP.NET Core (backend) to search for movies using the OMDB API. The app supports movie searches, detailed movie views, recent search history with local storage, and keyboard navigation for accessibility. Comprehensive unit tests ensure reliability for both frontend and backend components.

## Architecture
The application follows a client-server architecture:
- **Frontend**: A Blazor WebAssembly app (`Blazor WebAssembly App`) handles the UI, including search input, recent searches, and movie details display. It communicates with the backend via HTTP requests.
- **Backend**: An ASP.NET Core Web API (`Blazor Backend API`) proxies requests to the OMDB API, using `IHttpClientFactory` for efficient HTTP client management.
- **Shared Models**: The `SharedModels` project contains data models (`Movie`, `MovieSearchResult`, `RecentQuery`, `Rating`)  and services (`IMovieService`) used by both frontend and backend.
- **Testing**: Unit tests are implemented in `Blazor Frontend tests` (using `bUnit`) and `Blazor Backend API tests` (using `xUnit` and `Moq`).

## Implementation Details

### Frontend (Blazor WebAssembly)
- **Components**:
  - **`SearchMovies.razor`**:
    - Handles movie search with a debounced input (300ms delay).
    - Displays recent searches from local storage (up to 5 queries) with timestamps.
    - Supports keyboard navigation (ArrowUp/ArrowDown/Enter) for selecting recent searches.
    - Renders movie results in a grid with clickable cards linking to details.
  - **`MovieDetails.razor`**:
    - Displays detailed movie information (title, poster, ratings, etc.) for a given `ImdbID`.
    - Includes a "Back to Search" button.
- **Services**:
  - **`IMovieService`/`MovieService`**: Abstracts HTTP calls to the backend’s `/api/movies/search` and `/api/movies/details/{id}` endpoints.
  - **`Blazored.LocalStorage`**: Stores and retrieves recent searches (`RecentQuery` objects) in the browser’s local storage.
- **Features**:
  - Debounced search to reduce API calls.
  - Error handling for failed API requests (e.g., "Movie not found").
  - Responsive design with loading states and error messages.

### Backend (ASP.NET Core Web API)
- **Controller**:
  - **`MoviesController`**:
    - Exposes `/api/movies/search?query={query}` to search movies via the OMDB API.
    - Exposes `/api/movies/details/{id}` to fetch movie details by `ImdbID`.
    - Uses `IHttpClientFactory` for efficient HTTP client management.
    - Validates input (e.g., non-empty query or ID) and returns appropriate HTTP responses (`Ok`, `NotFound`, `BadRequest`).
- **Configuration**:
  - OMDB API key is stored in `user secrets` and accessed via `IConfiguration`.
- **Error Handling**:
  - Returns `NotFound` for invalid queries or IDs.
  - Handles HTTP errors from the OMDB API.

### Shared Models
- **MovieSearchResult**: Contains a list of `MovieSummary` objects, `TotalResults`, and `Response` status.
- **MovieSummary**: Represents a movie in search results (`ImdbID`, `Title`, `Year`, `Poster`, `Type`).
- **Movie**: Detailed movie information (e.g., `Title`, `Director`, `Ratings`, `BoxOffice`).
- **Rating**: Detailed movie information (`Source`, `Value`).
- **RecentQuery**: Stores recent search queries with timestamps (`Query`, `Timestamp`).
- **Services.IMovieService**: An interface in `SharedModels.Services` that defines methods for searching movies and fetching details, used by the frontend to interact with the backend API and optionally by the backend for OMDB API calls. It enables loose coupling and simplifies unit testing.).

### Unit Tests
- **Frontend Tests (`Blazor Frontend tests`)**:
  - **Framework**: `bUnit`, `Moq`, `Blazored.LocalStorage.TestExtensions`.
  - **Tests**:
    - `RendersSearchBarAndRecentQueries`: Verifies the search input and recent searches render correctly.
    - `PerformsSearchAndSavesQuery`: Tests search functionality, ensuring results are displayed and queries are saved to local storage.
    - `KeyboardNavigation_SelectsQuery`: Validates arrow key navigation for recent searches.
    - `ClearRecentQueries_RemovesQueries`: Confirms clearing recent searches updates local storage.
- **Backend Tests (`Blazor Backend API tests`)**:
  - **Framework**: `xUnit`, `Moq`.
  - **Tests**:
    - `SearchMovies_ReturnsOkWithResults_WhenApiSucceeds`: Verifies successful movie searches.
    - `SearchMovies_ReturnsNotFound_WhenApiFails`: Tests error handling for invalid queries.
    - `SearchMovies_ReturnsBadRequest_WhenQueryIsEmpty`: Validates input checks.
    - Similar tests for `GetMovieDetails`.
  - **Mocking**: Uses `Moq` to mock `IHttpClientFactory` and simulate OMDB API responses.


## Future Improvements
- **Integration Tests**: Add end-to-end tests using `Microsoft.AspNetCore.Mvc.Testing` or Playwright to test the full stack.
- **Caching**: Implement caching for OMDB API responses to reduce external calls.
- **Pagination**: Support pagination for search results with large `TotalResults`.
- **Accessibility**: Enhance keyboard navigation and ARIA attributes for better accessibility.
- **Error Handling**: Add more robust error messages and retry logic for API failures.

## Conclusion
The "Blazor App - Homework" App demonstrates a modern full-stack web application with Blazor WebAssembly and ASP.NET Core, integrated with the OMDB API. Its modular architecture, comprehensive tests, and user-friendly features make it a solid foundation for further enhancements.