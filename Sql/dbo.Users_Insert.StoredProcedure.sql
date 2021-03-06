USE [ReCycle]
GO
/****** Object:  StoredProcedure [dbo].[Users_Insert]    Script Date: 12/24/2019 1:58:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Users_Insert]
			@Email NVARCHAR(100)
			,@Password VARCHAR(100)
			,@UserRole INT
			,@Id INT OUTPUT

AS

/* ::Test::

	Declare 
			@Id int = 0

	Declare 
			@Email nvarchar(100) = 'email@re-cycle.co'
			,@Password varchar(100) = 'Superuser123!'
			,@UserRole int = 3


	Execute dbo.Users_Insert
			@Email
			,@Password	
			,@UserRole
			,@Id OUTPUT
			
		Select *
		From dbo.UserRoles
			
		Select *
		From dbo.Users
			
		Where Id=@Id
		
*/

BEGIN

	INSERT INTO [dbo].[Users]
			([Email]
			,[Password])

		VALUES
			(@Email
			,@Password)

	SET @Id = SCOPE_IDENTITY()

	INSERT INTO [dbo].[UserRoles]
			([UserId]
			,[RoleId])

		VALUES
			(@Id
			,@UserRole)
			
END
GO
