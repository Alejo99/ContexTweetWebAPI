USE [ContexTweet]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE LoadUrlsTweets
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.UrlsTweets([TweetId], [Url]) 
		SELECT SUT.[TweetId], SUT.[Url]
		FROM dbo.Staging_UrlsTweets SUT
		WHERE NOT EXISTS (SELECT UT.[TweetId], UT.[Url] 
							FROM dbo.UrlsTweets UT 
							WHERE SUT.[TweetId] = UT.[TweetId] AND 
								SUT.[Url] = UT.[Url]);
	
	-- Cleanup
	DELETE FROM dbo.Staging_UrlsTweets;
END
