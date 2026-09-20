USE [flashcards]
GO

/****** Object:  View [dbo].[CardsPerStack]    Script Date: 9/20/2026 12:48:16 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[CardsPerStack]
AS
SELECT        dbo.Stack.Name, COUNT(dbo.Cards.ID) AS TotalCards
FROM            dbo.Stack LEFT OUTER JOIN
                         dbo.Cards ON dbo.Stack.ID = dbo.Cards.StackID
GROUP BY dbo.Stack.Name
GO