GO

/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridTemplateMappingDefaults]    Script Date: 12/16/2013 15:19:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertCustomerGridTemplateMappingDefaults]
      @CustomerId int
      
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

      DECLARE @MasterController nvarchar(128);
      DECLARE @MasterAction nvarchar(128);



--delete them all for this customer
delete from dbo.CustomerGridTemplateMap where CustomerId = @CustomerId ;


--foreach disctinct controller/action in the customerGridTemplate table, insert the version 0 into the mapping file.
      DECLARE cursorCustomerGrids cursor for SELECT distinct Controller ,Action  FROM CustomerGridTemplate;

OPEN cursorCustomerGrids
      FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
      WHILE @@FETCH_STATUS = 0
      BEGIN       
      EXEC  sp_InsertCustomerGridTemplateMapping @CustomerId = @CustomerId, @Controller = @MasterController, @Action = @MasterAction, @Version = 0


            -- Get next row
            FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
      END
      CLOSE cursorCustomerGrids
      DEALLOCATE cursorCustomerGrids



Return  SCOPE_IDENTITY()
END

GO

