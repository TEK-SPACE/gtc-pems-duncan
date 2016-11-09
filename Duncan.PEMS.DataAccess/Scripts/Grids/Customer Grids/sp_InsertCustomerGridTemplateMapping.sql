
GO

/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateMapping]    Script Date: 12/16/2013 15:19:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertCustomerGridTemplateMapping] 
      @CustomerId int,
      @Controller nvarchar(128),
      @Action nvarchar(128),
      @Version int = 0
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;


delete from dbo.CustomerGridTemplateMap where CustomerId = @CustomerId 
and Controller = @Controller 
and Action = @Action


  -- Insert statements for procedure
      INSERT INTO CustomerGridTemplateMap
           ([CustomerId]
           ,[Controller]
           ,[Action]
           ,[Version])
     VALUES           (@CustomerId,          @Controller           ,@Action           ,@Version)
Return  SCOPE_IDENTITY()
END

GO

