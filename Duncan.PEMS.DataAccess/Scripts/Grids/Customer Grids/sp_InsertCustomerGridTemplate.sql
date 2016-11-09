GO

/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplate]    Script Date: 12/16/2013 15:18:20 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertCustomerGridTemplate]
      
      @Controller nvarchar(128),
      @Action nvarchar(128),
        @Version int = 0
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

  INSERT [dbo].[CustomerGridTemplate] ( [Controller], [Action], [Version]) VALUES (@Controller,@Action, @Version)
  Return SCOPE_IDENTITY()
END

GO

