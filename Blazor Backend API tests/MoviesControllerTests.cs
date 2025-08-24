using Blazor_Backend_API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SharedModels;
using SharedModels.Services;
using System.Net;
using System.Text.Json;

namespace Blazor_Backend_API_tests
{
    public class MoviesControllerTests
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly MoviesController _controller;

        public MoviesControllerTests()
        {
            _movieServiceMock = new Mock<IMovieService>();
            _controller = new MoviesController(_movieServiceMock.Object);
        }

        [Fact]
        public async Task SearchMovies_ReturnsOkWithResults_WhenServiceSucceeds()
        {
            // Arrange
            var query = "Star Wars";
            var response = new MovieSearchResult
            {
                Search = new List<MovieSummary>
                {
                    new MovieSummary { ImdbID = "tt0076759", Title = "Star Wars", Year = "1977", Poster = "poster.jpg", Type = "movie" }
                },
                TotalResults = "1",
                Response = "True"
            };
            _movieServiceMock.Setup(s => s.SearchMoviesAsync(query)).ReturnsAsync(response);

            // Act
            var result = await _controller.SearchMovies(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<MovieSearchResult>(okResult.Value);
            Assert.Equal("True", returnValue.Response);
            Assert.Single(returnValue.Search);
            Assert.Equal("tt0076759", returnValue.Search[0].ImdbID);
        }

        [Fact]
        public async Task SearchMovies_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var query = "Invalid Movie";
            _movieServiceMock.Setup(s => s.SearchMoviesAsync(query)).ReturnsAsync(new MovieSearchResult { Response = "False" });

            // Act
            var result = await _controller.SearchMovies(query);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task SearchMovies_ReturnsBadRequest_WhenQueryIsEmpty()
        {
            // Arrange
            var query = "";

            // Act
            var result = await _controller.SearchMovies(query);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Query is required.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetMovieDetails_ReturnsOkWithDetails_WhenServiceSucceeds()
        {
            // Arrange
            var id = "tt0076759";
            var response = new Movie
            {
                ImdbID = id,
                Title = "Star Wars",
                Year = "1977",
                Rated = "PG",
                Released = "25 May 1977",
                Runtime = "121 min",
                Genre = "Action, Adventure, Fantasy",
                Director = "George Lucas",
                Writer = "George Lucas",
                Actors = "Mark Hamill, Harrison Ford",
                Plot = "A young farmboy joins a rebellion.",
                Language = "English",
                Country = "USA",
                Awards = "Won 6 Oscars",
                Poster = "poster.jpg",
                Ratings = new List<Rating> { new Rating { Source = "IMDb", Value = "8.6/10" } },
                Metascore = "90",
                ImdbRating = "8.6",
                ImdbVotes = "1,400,000",
                Type = "movie",
                DVD = "21 Sep 2004",
                BoxOffice = "$460,998,007",
                Production = "Lucasfilm",
                Website = "http://starwars.com"
            };
            _movieServiceMock.Setup(s => s.GetMovieDetailsAsync(id)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetMovieDetails(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Movie>(okResult.Value);
            Assert.Equal("tt0076759", returnValue.ImdbID);
            Assert.Equal("Star Wars", returnValue.Title);
        }

        [Fact]
        public async Task GetMovieDetails_ReturnsNotFound_WhenServiceFails()
        {
            // Arrange
            var id = "tt0000000";
            _movieServiceMock.Setup(s => s.GetMovieDetailsAsync(id)).ReturnsAsync(new Movie { Response = "False" });

            // Act
            var result = await _controller.GetMovieDetails(id);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetMovieDetails_ReturnsBadRequest_WhenIdIsEmpty()
        {
            // Arrange
            var id = "";

            // Act
            var result = await _controller.GetMovieDetails(id);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("ID is required.", badRequestResult.Value);
        }
    }
}
