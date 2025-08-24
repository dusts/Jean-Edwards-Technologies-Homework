using AngleSharp.Html.Dom;
using Blazor_WebAssembly_App.Pages;
using Blazored.LocalStorage;
using Blazored.LocalStorage.TestExtensions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SharedModels;
using SharedModels.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Blazor_Frontend_tests
{
    public class SearchMoviesTests : TestContext
    {
        private readonly Mock<IMovieService> _movieServiceMock;
        private readonly Mock<ILocalStorageService> _localStorageMock;

        public SearchMoviesTests()
        {
            _movieServiceMock = new Mock<IMovieService>();
            _localStorageMock = new Mock<ILocalStorageService>();
            Services.AddSingleton(_movieServiceMock.Object);
            Services.AddSingleton(_localStorageMock.Object);
        }

        [Fact]
        public async Task RendersSearchBarAndRecentQueries()
        {
            // Arrange
            _localStorageMock.Setup(l => l.GetItemAsync<List<RecentQuery>>("RecentMovieQueries", default))
                .ReturnsAsync(new List<RecentQuery>
                {
                    new RecentQuery { Query = "Star Wars", Timestamp = new DateTime(2025, 8, 24, 22, 0, 0) }
                });

            // Act
            var component = RenderComponent<SearchMovies>();

            // Assert
            var input = component.Find("input[placeholder='Search for movies...']");
            Assert.NotNull(input);
            var recentQueriesHeader = component.Find("h4");
            Assert.Equal("Recent Searches", recentQueriesHeader.TextContent);
            var recentQuery = component.Find("li");
            Assert.Contains("Star Wars", recentQuery.TextContent);
            Assert.Contains("aug. 24, 2025 22:00", recentQuery.TextContent);
        }

        [Fact]
        public async Task PerformsSearchAndSavesQuery()
        {
            // Arrange
            var query = "Inception";
            var searchResult = new MovieSearchResult
            {
                Search = new List<MovieSummary>
                {
                    new MovieSummary { ImdbID = "tt1375666", Title = "Inception", Year = "2010", Poster = "poster.jpg", Type = "movie" }
                },
                TotalResults = "1",
                Response = "True"
            };
            _movieServiceMock.Setup(s => s.SearchMoviesAsync(It.Is<string>(q => q == query)))
                .ReturnsAsync(searchResult)
                .Verifiable();
            _localStorageMock.Setup(l => l.GetItemAsync<List<RecentQuery>>("RecentMovieQueries", default))
                .ReturnsAsync(new List<RecentQuery>());
            _localStorageMock.Setup(l => l.SetItemAsync("RecentMovieQueries", It.IsAny<List<RecentQuery>>(), default))
                .Returns(ValueTask.CompletedTask);

            // Act
            var component = RenderComponent<SearchMovies>();
            component.Instance.searchQuery = query; // Set directly
            await component.InvokeAsync(() => component.Instance.Search()); // Use InvokeAsync to ensure Dispatcher context

            // Assert
            Assert.NotNull(component.Instance.movies);
            Assert.Equal("True", component.Instance.movies.Response);
            Assert.Single(component.Instance.movies.Search);
            var movieCards = component.FindAll(".movie-card");
            Assert.NotEmpty(movieCards);
            var movieCard = component.Find(".movie-card");
            Assert.Contains("Inception", movieCard.TextContent);
            Assert.Equal(query, component.Instance.searchQuery);
            _movieServiceMock.Verify();
            _localStorageMock.Verify(l => l.SetItemAsync("RecentMovieQueries", It.Is<List<RecentQuery>>(q => q.Any(r => r.Query == query)), default), Times.Once());
        }

        [Fact]
        public async Task KeyboardNavigation_SelectsQuery()
        {
            // Arrange
            _localStorageMock.Setup(l => l.GetItemAsync<List<RecentQuery>>("RecentMovieQueries", default))
                .ReturnsAsync(new List<RecentQuery>
                {
                    new RecentQuery { Query = "Star Wars", Timestamp = DateTime.Now },
                    new RecentQuery { Query = "Inception", Timestamp = DateTime.Now }
                });

            // Act
            var component = RenderComponent<SearchMovies>();
            var input = component.Find("input");
            await input.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            // Assert
            var selected = component.Find(".selected");
            Assert.Contains("Star Wars", selected.TextContent);

            // Act
            await input.KeyDownAsync(new KeyboardEventArgs { Key = "ArrowDown" });

            // Assert
            selected = component.Find(".selected");
            Assert.Contains("Inception", selected.TextContent);
        }

        [Fact]
        public async Task ClearRecentQueries_RemovesQueries()
        {
            // Arrange
            _localStorageMock.Setup(l => l.GetItemAsync<List<RecentQuery>>("RecentMovieQueries", default))
                .ReturnsAsync(new List<RecentQuery> { new RecentQuery { Query = "Star Wars", Timestamp = DateTime.Now } });
            _localStorageMock.Setup(l => l.RemoveItemAsync("RecentMovieQueries", default))
                .Returns(ValueTask.CompletedTask); // Fix: Use ValueTask.CompletedTask

            // Act
            var component = RenderComponent<SearchMovies>();
            var clearButton = component.Find(".btn-secondary");
            await clearButton.ClickAsync(new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

            // Assert
            _localStorageMock.Verify(l => l.RemoveItemAsync("RecentMovieQueries", default), Times.Once());
            var noQueriesMessage = component.Find("p");
            Assert.Equal("No recent searches.", noQueriesMessage.TextContent);
        }
    }
}