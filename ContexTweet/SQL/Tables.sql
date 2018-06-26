USE [ContexTweet]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/* Staging NamedEntities */
CREATE TABLE [dbo].[Staging_NamedEntities](
	[Text] [nvarchar](450) NOT NULL,
	[Type] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

/* Staging Tweets */
CREATE TABLE [dbo].[Staging_Tweets](
	[Id] [nvarchar](450) PRIMARY KEY,
	[SentimentScore] [real] NOT NULL,
	[Timestamp] [datetime2](7) NOT NULL,
	[FavoriteCount] [int] NOT NULL,
	[RetweetCount] [int] NOT NULL
) ON [PRIMARY]
GO

/* Staging Named Entities - Tweets */
CREATE TABLE [dbo].[Staging_NamedEntitiesTweets](
	[NamedEntityText] [nvarchar](450) NOT NULL,
	[TweetId] [nvarchar](450) NOT NULL,
	PRIMARY KEY([NamedEntityText], [TweetId])
) ON [PRIMARY]
GO

/* Staging Urls - Tweets */
CREATE TABLE [dbo].[Staging_UrlsTweets](
	[Url] [nvarchar](450) NOT NULL,
	[TweetId] [nvarchar](450) NOT NULL,
	PRIMARY KEY ([Url], [TweetId])
) ON [PRIMARY]
GO


