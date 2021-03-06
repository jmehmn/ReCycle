USE [ReCycle]
GO
/****** Object:  StoredProcedure [dbo].[Users_Update]    Script Date: 12/24/2019 1:58:35 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[Users_Update]
			@Email nvarchar(100)
			,@IsConfirmed bit
			,@UserStatusId int 
			,@Id int 

as

/*	::Test::

    Declare 
			@Id int = 1

    Declare    
			@Email nvarchar(100) = 'new@re-cycle.co'
			,@IsConfirmed bit = 1
			,@UserStatusId int  = 1
    
    Execute dbo.Users_Update
			@Email
			,@IsConfirmed
			,@UserStatusId
			,@Id

		Select *
		From dbo.Users
		
		Where Id=@Id

*/

BEGIN

    UPDATE [dbo].[Users]
       
	   SET	
			[Email] = @Email
			,[IsConfirmed] = @IsConfirmed
			,[UserStatusId] = @UserStatusId


     WHERE Id = @Id

END
GO
