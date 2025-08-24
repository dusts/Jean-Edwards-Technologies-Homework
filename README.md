# Blazor WebAssembly App (Movie Search App for homework)

## Overview
The Blazor WebAssembly App (Movie Search App for homework) is a Blazor WebAssembly application with an ASP.NET Core backend that allows users to search for movies and view detailed information using the OMDB API. The app features a responsive search interface, recent search history with keyboard navigation, and unit tests for both frontend and backend components.
**As a note** - I used Grok AI from x.com as a helper and a bit more advanced googling aparatus. It has its great moments, but there are also a lot of typical dev errors, that takes time to find and fix.

## Features
- **Search Movies**: Users can search for movies by title, with results fetched from the OMDB API via the backend.
- **Movie Details**: Displays detailed movie information, including poster, genre, ratings, and more.
- **Recent Searches**: Stores up to 5 recent searches in local storage, with timestamp and keyboard navigation support.
- **Responsive UI**: Clean, modern interface with loading and error states.
- **Unit Tests**: Some tests for frontend (using `bUnit`) and backend (using `xUnit` and `Moq`).

## Project Structure
- **Blazor App - Homework.Blazor WebAssembly App**: Blazor WebAssembly frontend with `SearchMovies` and `MovieDetails` components.
- **Blazor App - Homework.Blazor Backend API**: ASP.NET Core Web API with `MoviesController` for OMDB API integration.
- **Blazor App - Homework.SharedModels**: The `SharedModels` project contains data models (`Movie`, `MovieSearchResult`, `RecentQuery`, `Rating`)  and services (`IMovieService`) used by both frontend and backend.
- **Blazor App - Homework.Blazor Frontend tests**: Frontend unit tests for `SearchMovies.razor`.
- **Blazor App - Homework.Blazor Backend API tests**: Backend unit tests for `MoviesController`.

## Setup
1. **Clone the Repository**: <https://dicismartins.visualstudio.com/Jean%20Edwards%20Technologies%20Homework/_git/Blazor%20App%20-%20Homework>
2. **Don't forget to add api key**: "OmdbApiKey": "your_api_key" to user secrets in **Blazor App - Homework.Blazor Backend API**
3. enjoy

## P.S.
I know that namings in this solution is all over the place, but it is what it is.
And .md files were an aftertaught.
**See Technical Documentation for more info**