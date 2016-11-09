GO

/****** Object:  StoredProcedure [dbo].[sp_RefreshAllCustomerGrids]    Script Date: 12/16/2013 15:19:44 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_RefreshAllCustomerGrids]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


--run the master file to update all the Grid Template and Columns
EXEC  sp_InsertAllCustomGridOptions;
print 'sp_InsertAllCustomGridOptions complete';

	declare @CustomerId int;
	DECLARE @MasterController nvarchar(128);
	DECLARE @MasterAction nvarchar(128);
	declare @CustomerGridTemplateMapId int;
		
	-- disctinct controller/action in the customerGridTemplate table, insert the version 0 into the mapping file if it doesnt exist for the customer.
	DECLARE cursorCustomerGrids cursor for SELECT distinct Controller ,Action  FROM CustomerGridTemplate;


-----------------------------------------------------------------------------------------------------------------------------------------------------------------
------------------This section will update each customers template map to have all the valid options based on the CustomerTemplateMap table ---------------------
-----------------------------------------------------------------------------------------------------------------------------------------------------------------
	print 'Adding All TemplateMaps for each customer';

-------------------------------- FOREACH unique set of controller action -------------------------------------------------------------
OPEN cursorCustomerGrids
FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
WHILE @@FETCH_STATUS = 0
BEGIN		
print 'Controller : ' + @MasterController + ' Action: ' + @MasterAction;

	------------------------------------- for each customer, -------------------------------------------------------------------------
	--list of customer profiles. Each on in the system needs to be refreshed	
	DECLARE customerIds cursor for SELECT CustomerId  FROM CustomerProfile where CustomerId != 0;
	OPEN customerIds
	FETCH NEXT FROM customerIds into @CustomerId
	WHILE @@FETCH_STATUS = 0
	BEGIN		
	
		-- if they dont have a templateMap, insert the version 0 of one for each action / controller.
		IF NOT EXISTS  (select customergridtemplatemapid from CustomerGridTemplateMap 
			where CustomerId = @CustomerId  and Controller = @MasterController  and Action = @MasterAction)
			begin
	print 'CustomerId: ' +Convert(varchar(11), @CustomerId) ;
			
			EXEC  sp_InsertCustomerGridTemplateMapping @CustomerId = @CustomerId, @Controller = @MasterController, @Action = @MasterAction, @Version = 0;
			end
		-- Get next row
	FETCH NEXT FROM customerIds into @CustomerId
	END
	CLOSE customerIds
	DEALLOCATE customerIds
-- Get next row
FETCH NEXT FROM cursorCustomerGrids into @MasterController, @MasterAction
END
CLOSE cursorCustomerGrids
DEALLOCATE cursorCustomerGrids

-----------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------Once they have all the correct options, update the customer grids with the mapping data-------------------------------------------
-----------------------------------------------------------------------------------------------------------------------------------------------------------------

--foreach customer
	print 'sp_InsertCustomerGridsFromTemplateMap for each customer';	
--call the sp_InsertCustomerGridsFromTemplateMap
	DECLARE customers cursor for SELECT CustomerId  FROM CustomerProfile ;
	OPEN customers
	FETCH NEXT FROM customers into @CustomerId
	WHILE @@FETCH_STATUS = 0
	BEGIN	
	print 'CustomerId: ' +Convert(varchar(11), @CustomerId) ;
	
		--call the stored proc to update the customer grids for this customer based on the templateMap
			EXEC  sp_InsertCustomerGridsFromTemplateMap @CustomerId = @CustomerId
	-- Get next row
	FETCH NEXT FROM customers into @CustomerId
	END
	CLOSE customers
	DEALLOCATE customers
	
END

GO

