using ContexTweet.Controllers;
using ContexTweet.Data;
using ContexTweet.Models;
using ContexTweet.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ContexTweet.Test.Controllers
{
    public class UrlsControllerFixture : IDisposable
    {
        public UrlsController Controller { get; private set; }
        public Mock<ITweetRepository> MockRepo { get; private set; }

        public UrlsControllerFixture()
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
                    SentimentScore = (float)-0.3,
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

            //Mock DbSet<UrlTweet>
            var urls = new List<UrlTweet>()
            {
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt1")).FirstOrDefault(),
                    TweetId = "twt1",
                    Url = "http://example.com/url1"

                },
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt2")).FirstOrDefault(),
                    TweetId = "twt2",
                    Url = "http://example.com/url1"

                },
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt3")).FirstOrDefault(),
                    TweetId = "twt3",
                    Url = "http://example.com/url1"

                },
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt4")).FirstOrDefault(),
                    TweetId = "twt4",
                    Url = "http://example.com/url2"

                },
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt5")).FirstOrDefault(),
                    TweetId = "twt5",
                    Url = "http://example.com/url2"

                },
                new UrlTweet()
                {
                    Tweet = tweets.Where(t => t.Id.Equals("twt6")).FirstOrDefault(),
                    TweetId = "twt6",
                    Url = "http://example.com/url3"

                },
            }.AsQueryable();

            //Mock DbSet<NamedEntityTweet>
            var namedEntities = new List<NamedEntityTweet>()
            {
                new NamedEntityTweet()
                {
                    NamedEntityText = "keyword1",
                    Tweet = tweets.Where(t => t.Id.Equals("twt1")).FirstOrDefault(),
                    TweetId = "twt1",

                },
                new NamedEntityTweet()
                {
                    NamedEntityText = "keyword2",
                    Tweet = tweets.Where(t => t.Id.Equals("twt1")).FirstOrDefault(),
                    TweetId = "twt1",

                },
                new NamedEntityTweet()
                {
                    NamedEntityText = "keyword1",
                    Tweet = tweets.Where(t => t.Id.Equals("twt4")).FirstOrDefault(),
                    TweetId = "twt4",

                },
                new NamedEntityTweet()
                {
                    NamedEntityText = "keyword3",
                    Tweet = tweets.Where(t => t.Id.Equals("twt3")).FirstOrDefault(),
                    TweetId = "twt3",

                },
            }.AsQueryable();

            Mock<DbSet<Tweet>> mockTweets = new Mock<DbSet<Tweet>>();
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.Provider).Returns(tweets.Provider);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.Expression).Returns(tweets.Expression);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.ElementType).Returns(tweets.ElementType);
            mockTweets.As<IQueryable<Tweet>>().Setup(m => m.GetEnumerator()).Returns(tweets.GetEnumerator());

            Mock<DbSet<UrlTweet>> mockUrls = new Mock<DbSet<UrlTweet>>();
            mockUrls.As<IQueryable<UrlTweet>>().Setup(m => m.Provider).Returns(urls.Provider);
            mockUrls.As<IQueryable<UrlTweet>>().Setup(m => m.Expression).Returns(urls.Expression);
            mockUrls.As<IQueryable<UrlTweet>>().Setup(m => m.ElementType).Returns(urls.ElementType);
            mockUrls.As<IQueryable<UrlTweet>>().Setup(m => m.GetEnumerator()).Returns(urls.GetEnumerator());

            Mock<DbSet<NamedEntityTweet>> mockNamedEntities = new Mock<DbSet<NamedEntityTweet>>();
            mockNamedEntities.As<IQueryable<NamedEntityTweet>>().Setup(m => m.Provider).Returns(namedEntities.Provider);
            mockNamedEntities.As<IQueryable<NamedEntityTweet>>().Setup(m => m.Expression).Returns(namedEntities.Expression);
            mockNamedEntities.As<IQueryable<NamedEntityTweet>>().Setup(m => m.ElementType).Returns(namedEntities.ElementType);
            mockNamedEntities.As<IQueryable<NamedEntityTweet>>().Setup(m => m.GetEnumerator()).Returns(namedEntities.GetEnumerator());

            //Mock tweet repository
            MockRepo = new Mock<ITweetRepository>();
            MockRepo.Setup(m => m.Tweets).Returns(mockTweets.Object);
            MockRepo.Setup(m => m.Urls).Returns(mockUrls.Object);
            MockRepo.Setup(m => m.NamedEntities).Returns(mockNamedEntities.Object);

            //Create controller
            Controller = new UrlsController(MockRepo.Object);
        }

        public void Dispose()
        {
            Controller.Dispose();
        }
    }

    public class UrlsControllerTests : IClassFixture<UrlsControllerFixture>
    {
        public UrlsController Controller { get; private set; }
        public Mock<ITweetRepository> Repository { get; set; }

        public UrlsControllerTests(UrlsControllerFixture fixture)
        {
            Controller = fixture.Controller;
            Repository = fixture.MockRepo;
        }

        private T GetViewModel<T>(IActionResult result) where T : class
        {
            return (result as OkObjectResult)?.Value as T;
        }

        [Fact]
        public void CanGetDistinctUrls()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = Controller.Get();

            //Assert
            Assert.Equal(3, result.Count());
            Assert.Equal("http://example.com/url1", result.First());
            Assert.Equal("http://example.com/url3", result.Last());
            Assert.Equal("http://example.com/url2", result.ElementAt(1));
        }

        [Fact]
        public void CanGetUrlsByNamedEntity()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<List<string>>(Controller.ByNamedEntity("keyword1"));

            //Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("http://example.com/url1", result.First());
            Assert.Equal("http://example.com/url2", result.Last());
        }

        [Fact]
        public void CanGetUrlsByNamedEntityAndExcludedUrl()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<List<string>>(Controller.ByNamedEntity("keyword1", "http://example.com/url2"));

            //Assert
            Assert.Single(result);
            Assert.Equal("http://example.com/url1", result.First());
        }

        [Fact]
        public void CannotGetUrlsByNonexistingNamedEntity()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = Controller.ByNamedEntity("keyword100");
            var notFoundResult = result as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public void CanGetSentimentByUrl()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = GetViewModel<UrlSentimentViewModel>(Controller.Sentiment("http://example.com/url1"));

            //Assert
            Assert.Equal("http://example.com/url1", result.Url);
            Assert.Equal(3, result.SentimentScores.Count());
            Assert.Equal(0.05, result.SentimentScores.First(), 2);
            Assert.Equal(0.55, result.SentimentScores.Last(), 2);
            Assert.Equal(0.1, result.Average, 2);
        }

        [Fact]
        public void CannotGetSentimentByNonexistingUrl()
        {
            //Arrange
            // controller arranged from the controller fixture

            //Act
            var result = Controller.Sentiment("http://example.com/url100");
            var notFoundResult = result as NotFoundResult;

            //Assert
            Assert.NotNull(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
