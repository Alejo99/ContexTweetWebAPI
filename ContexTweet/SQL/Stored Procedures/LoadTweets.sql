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
CREATE PROCEDURE LoadTweets
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- MERGE SQL statement
	-- Sync target table with data from the source table
	MERGE dbo.Tweets AS TARGET
	USING dbo.Staging_Tweets AS SOURCE 
	ON (TARGET.Id = SOURCE.Id) 
	-- When records are matched, update the records
	WHEN MATCHED THEN 
		UPDATE SET TARGET.SentimentScore = SOURCE.SentimentScore,
			TARGET.[Timestamp] = SOURCE.[Timestamp],
			TARGET.RetweetCount = SOURCE.RetweetCount,
			TARGET.FavoriteCount = SOURCE.FavoriteCount
	-- When no records are matched, insert new data
	WHEN NOT MATCHED BY TARGET THEN 
		INSERT (Id, SentimentScore, [Timestamp], RetweetCount, FavoriteCount) 
		VALUES (SOURCE.Id, SOURCE.SentimentScore, SOURCE.[Timestamp], SOURCE.RetweetCount, SOURCE.FavoriteCount);

	-- Cleanup
	DELETE FROM dbo.Staging_Tweets;
END
