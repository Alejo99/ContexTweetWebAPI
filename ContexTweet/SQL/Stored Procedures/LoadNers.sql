USING [ContexTweet]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[LoadNers] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.NamedEntities(Text, Type) 
		SELECT S.Text, MIN(S.Type) AS Type
		FROM dbo.Staging_NamedEntities S
		WHERE NOT EXISTS (SELECT DISTINCT NE.Text FROM dbo.NamedEntities NE WHERE S.Text = NE.Text)
		GROUP BY S.Text;

	DELETE FROM dbo.Staging_NamedEntities;
END
