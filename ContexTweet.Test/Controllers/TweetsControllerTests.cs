using ContexTweet.Configuration;
using ContexTweet.Controllers;
using ContexTweet.Data;
using ContexTweet.Models;
using ContexTweet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ContexTweet.Test.Controllers
{
    public class TweetsControllerFixture : IDisposable
    {
        public TweetsController Controller { get; private set; }
        public Mock<ITweetRepository> MockRepo { get; private set; }

        public TweetsControllerFixture()
        {
            //Mock DbSet<adverts>
            var tweets = new List<Tweet>()
            {
                new Tweet
                {
                    Id = "twt1",
                    Text = "Tweet 1",
                    SentimentScore = (float)0.05,
                    FavoriteCount = 4,
                    RetweetCount = 2,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr1"
                },
                new Tweet
                {
                    Id = "twt2",
                    Text = "Tweet 2",
                    SentimentScore = (float)-0.4,
                    FavoriteCount = 1,
                    RetweetCount = 0,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr1"
                },
                new Tweet
                {
                    Id = "twt3",
                    Text = "Tweet 3",
                    SentimentScore = (float)0.55,
                    FavoriteCount = 2,
                    RetweetCount = 0,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr2"
                },
                new Tweet
                {
                    Id = "twt4",
                    Text = "Tweet 4",
                    SentimentScore = (float)0.95,
                    FavoriteCount = 7,
                    RetweetCount = 1,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr3"
                },
                new Tweet
                {
                    Id = "twt5",
                    Text = "Tweet 5",
                    SentimentScore = (float)-0.35,
                    FavoriteCount = 3,
                    RetweetCount = 0,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr2"
                },
                new Tweet
                {
                    Id = "twt6",
                    Text = "Tweet 6",
                    SentimentScore = (float)-0.95,
                    FavoriteCount = 6,
                    RetweetCount = 2,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr1"
                },
                new Tweet
                {
                    Id = "twt7",
                    Text = "Tweet 7",
                    SentimentScore = (float)0.25,
                    FavoriteCount = 5,
                    RetweetCount = 1,
                    NamedEntities = null,
                    Timestamp = DateTime.Now,
                    Urls = null,
                    UserId = "usr1"
                }
            }.AsQueryable();

            //TODO: mock indexed urls and urls

            Mock<DbSet<Tweet>> mockTweets = new Mock<DbSet<Tweet>>();
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.Provider).Returns(tweets.Provider);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.Expression).Returns(tweets.Expression);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.ElementType).Returns(tweets.ElementType);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.GetEnumerator()).Returns(tweets.GetEnumerator());

            //Mock advert repository
            MockRepo = new Mock<ITweetRepository>();
            MockRepo.Setup(m => m.Tweets).Returns(mockTweets.Object);

            //Mock pagingoptions
            Mock<PagingOptions> mockPagingOpts = new Mock<PagingOptions>();
            mockPagingOpts.SetupGet(po => po.PageSize).Returns(3);

            Mock<IOptions<PagingOptions>> mockIOptions = new Mock<IOptions<PagingOptions>>();
            mockIOptions.Setup(m => m.Value).Returns(mockPagingOpts.Object);

            //Create controller
            Controller = new TweetsController(mockIOptions.Object, MockRepo.Object);
        }

        public void Dispose()
        {
            Controller.Dispose();
        }
    }

    public class TweetsControllerTests : IClassFixture<TweetsControllerFixture>
    {
        public TweetsController Controller { get; private set; }
        public Mock<ITweetRepository> Repository { get; set; }

        public TweetsControllerTests(TweetsControllerFixture fixture)
        {
            Controller = fixture.Controller;
            Repository = fixture.MockRepo;
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as OkObjectResult)?.Value as T;
        }

        [Fact]
        public void CanGetPagedTweets()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var page1 = GetViewModel<TweetListViewModel>(Controller.Get());
            var page3 = GetViewModel<TweetListViewModel>(Controller.Get(3));

            //Assert
            Assert.Equal(7, page1.PagingInfo.TotalItems);
            Assert.Equal(7, page3.PagingInfo.TotalItems);
            Assert.Equal("twt4", page1.Tweets.First()?.Id);
            Assert.Equal("twt7", page1.Tweets.Last()?.Id);
            Assert.Equal("twt2", page3.Tweets.First()?.Id);
            Assert.Equal("twt2", page3.Tweets.Last()?.Id);
        }

        [Fact]
        public void CanGetNotFoundTweets()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = Controller.Get(4);
            var notFoundResult = result as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
        
        [Fact]
        public void CanPaginateAndOrderResults()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<TweetListViewModel>(Controller.Get(2));

            //Assert
            Assert.Equal(3, result.Tweets.Count());
            var advList = result.Tweets.ToList();
            Assert.Equal("twt1", advList.First()?.Id);
            Assert.Equal("twt3", advList.Last()?.Id);
            Assert.Equal("twt5", advList.ElementAt(1)?.Id);
        }

        [Fact]
        public void CanSendPaginationViewModel()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<TweetListViewModel>(Controller.Get(2));

            //Assert
            PagingInfo pageInfo = result.PagingInfo;
            Assert.Equal(2, pageInfo.CurrentPage);
            Assert.Equal(3, pageInfo.ItemsPerPage);
            Assert.Equal(7, pageInfo.TotalItems);
            Assert.Equal(3, pageInfo.TotalPages);
        }
        
        [Fact]
        public void CanGetTweetById()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<Tweet>(Controller.Get("twt3"));

            //Assert
            Assert.Equal("Tweet 3", result.Text);
            Assert.Equal("twt3", result.Id);
        }

        [Fact]
        public void CannotGetUnexistantTweetById()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = Controller.Get(4);
            var notFoundResult = result as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        //TODO: urls and byurl get requests
        
    }
}
