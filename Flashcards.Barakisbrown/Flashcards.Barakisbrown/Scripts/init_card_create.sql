USE [Flash Cards]
GO

/****** Object:  Table [dbo].[cards]    Script Date: 5/26/2023 4:58:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[cards](
	[cardsID] [int] IDENTITY(1,1) NOT NULL,
	[prompt] [varchar](30) NOT NULL,
	[answer] [varchar](30) NOT NULL,
	[stack_id] [int] NOT NULL,
 CONSTRAINT [PK_cards] PRIMARY KEY CLUSTERED 
(
	[cardsID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[cards]  WITH CHECK ADD  CONSTRAINT [FK_cards_stacks] FOREIGN KEY([stack_id])
REFERENCES [dbo].[stacks] ([stackID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[cards] CHECK CONSTRAINT [FK_cards_stacks]
GO