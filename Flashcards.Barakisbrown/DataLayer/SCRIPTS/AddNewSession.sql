SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Matthew Brown
-- Create date: 9/20/2026
-- Description:	Adds a new session
-- =============================================
CREATE PROCEDURE AddNewSession 
	@StudyDate datetime,
	@StudyScore smallint,
	@StackID int,
	@StackName nchar(20),
	@NumQuestions smallint,
	@RowsInserted smallint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO Sessions(StudyDate,StudyScore,StackID,StackName,NumQuestions)
	VALUES(@StudyDate,@StudyScore,@StackID,@StackName,@NumQuestions);

	SET @RowsInserted = @@ROWCOUNT;
END
GO
