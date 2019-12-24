USE [ReCycle]
GO
/****** Object:  StoredProcedure [dbo].[Users_SelectById]    Script Date: 12/24/2019 1:58:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Users_SelectById]
			@Id int
as

/*	::Test::

	Declare 
			@Id int = 1
	
	Execute dbo.Users_SelectById 
			@Id
			
*/

BEGIN
		
	SELECT 
			[Id]
			,[Email]
			,[UserStatusId]		

	FROM [dbo].[Users]
	
	WHERE Id = @Id

END
GO
