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
CREATE PROCEDURE LoadNersTweets
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.NamedEntitiesTweets(TweetId, NamedEntityText) 
		SELECT SNET.TweetId, SNET.NamedEntityText
		FROM dbo.Staging_NamedEntitiesTweets SNET
		WHERE NOT EXISTS (SELECT NET.TweetId, NET.NamedEntityText 
							FROM dbo.NamedEntitiesTweets NET 
							WHERE SNET.TweetId = NET.TweetId AND 
								SNET.NamedEntityText = NET.NamedEntityText);
	
	-- Cleanup
	DELETE FROM dbo.Staging_NamedEntitiesTweets;
END
