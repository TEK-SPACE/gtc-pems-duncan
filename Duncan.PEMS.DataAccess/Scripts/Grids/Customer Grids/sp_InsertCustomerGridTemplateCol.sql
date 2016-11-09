GO

/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateCol]    Script Date: 12/16/2013 15:18:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertCustomerGridTemplateCol]
      @CustomerGridTemplateId int, @Title nvarchar(255), @Position int, @OriginalTitle nvarchar(255), @OriginalPosition int, @IsHidden bit
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

  INSERT [dbo].[CustomerGridTemplateCol] ([CustomerGridTemplateId], [Title], [Position], [OriginalTitle], [OriginalPosition], [IsHidden])
   VALUES (@CustomerGridTemplateId, @Title, @Position,@OriginalTitle, @OriginalPosition, @IsHidden)
Return  SCOPE_IDENTITY()
END

GO

