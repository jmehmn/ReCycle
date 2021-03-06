USE [ReCycle]
GO
/****** Object:  StoredProcedure [dbo].[Users_SelectAll]    Script Date: 12/24/2019 1:58:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Users_SelectAll]
			@pageIndex INT
			,@pageSize INT

as

/*	::Test::	

	Declare 
			@pageIndex int = 0
			,@pageSize int = 3
	
	Execute dbo.Users_SelectAll
			 @pageIndex
			,@pageSize

*/

BEGIN

	DECLARE @offset int = @pageIndex * @pageSize
	
	SELECT 
			[Id]
			,[Email]
			,[UserStatusId]
			,[TotalCount] = COUNT(1) OVER()	

	FROM [dbo].[Users]
	
	ORDER BY Id

	OFFSET @offSet Rows
	Fetch Next @pageSize Rows ONLY

END
GO
