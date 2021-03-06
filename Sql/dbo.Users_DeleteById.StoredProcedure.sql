USE [ReCycle]
GO
/****** Object:  StoredProcedure [dbo].[Users_DeleteById]    Script Date: 12/24/2019 1:58:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Users_DeleteById]
			@Id INT
as

/* ::Test::

	Declare 
			@Id int = 1
	
	Execute dbo.Users_DeleteById @Id
	
		Select *
		From dbo.Users
	
	Where Id = @Id;
	
*/

BEGIN

	DELETE FROM [dbo].[Users]
	
	WHERE Id = @Id;
	   
END 
GO
