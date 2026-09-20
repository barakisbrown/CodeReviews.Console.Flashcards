USE [flashcards]
GO

/****** Object:  Table [dbo].[Sessions]    Script Date: 9/20/2026 12:44:10 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Sessions](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[StudyDate] [datetime] NOT NULL,
	[StudyScore] [smallint] NOT NULL,
	[StackID] [int] NOT NULL,
	[StackName] [nchar](20) NOT NULL,
	[NumQuestions] [smallint] NOT NULL,
 CONSTRAINT [PK_Sessions] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Sessions]  WITH CHECK ADD  CONSTRAINT [FK_Sessions_Stack] FOREIGN KEY([StackID])
REFERENCES [dbo].[Stack] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Sessions] CHECK CONSTRAINT [FK_Sessions_Stack]
GO


