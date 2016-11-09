GO

/****** Object:  StoredProcedure [dbo].[sp_InsertCustomerGridsFromTemplateMap]    Script Date: 12/16/2013 15:18:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[sp_InsertCustomerGridsFromTemplateMap]
	-- Add the parameters for the stored procedure here
	@CustomerId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--Map Items
	DECLARE @Controller nvarchar(128);
	DECLARE @Action nvarchar(128);
	DECLARE @Version int;

	--Template Items
	DECLARE @GridTemplateId int;

	--Template Column Items
	DECLARE @Title nvarchar(128);
	DECLARE @Position int;
	DECLARE @OriginalTitle nvarchar(128);
	DECLARE @OriginalPosition int;
	DECLARE @IsHidden bit;

--this will blow away any exisint items int he CustomerGrid table for this customer ID, then reset that data back in based on the customer grid template map. 
--it will be used when saving the customer girds page for a customer, when they are newly created, or if we are adding columns to the grids, or when adding new grids to the system

--delete all existing grid data for this customer
delete from CustomerGrids where CustomerId = @CustomerId;

--re-insert the data based on what is in the customer template mapping table
--Get the maps for this customer
	DECLARE cursorCustomerGridMap cursor for SELECT cgtm.Controller ,cgtm.Action,cgtm.Version  FROM CustomerGridTemplateMap cgtm where CustomerId = @CustomerId;
print 'Inserting Customer Grid Map for '+Convert(varchar(11), @CustomerId);
OPEN cursorCustomerGridMap
	FETCH NEXT FROM cursorCustomerGridMap into @Controller, @Action, @Version
	WHILE @@FETCH_STATUS = 0
	BEGIN		
print 'Controller : ' + @Controller + ' Action: ' + @Action + ' Version: ' + +Convert(varchar(11), @Version);

	
	--now we have to get the grid tempate for this map
	set @GridTemplateId = (Select CustomerGridTemplateId from CustomerGridTemplate where Controller = @Controller and  Action = @Action and Version = @Version);
	print @GridTemplateId;
	--now get all of the customer template columns for this template
	DECLARE cursorCustomerGridColumns cursor for SELECT Title ,Position, OriginalTitle ,OriginalPosition,IsHidden  
	FROM CustomerGridTemplateCol where CustomerGridTemplateId = @GridTemplateId;
	
	--roll throught he columns and insert into the customerGrid table with this action/controller/customerId
	
	OPEN cursorCustomerGridColumns
	FETCH NEXT FROM cursorCustomerGridColumns into @Title ,@Position, @OriginalTitle ,@OriginalPosition,@IsHidden
	WHILE @@FETCH_STATUS = 0
	BEGIN		

-- Insert this grid row
			INSERT INTO [dbo].[CustomerGrids] 
				([CustomerId],[Controller],[Action],[Title],[Position],[OriginalTitle],[OriginalPosition], [IsHidden])
			VALUES
				(@CustomerId,@Controller,@Action,@Title,@Position,@OriginalTitle,@OriginalPosition, @IsHidden)

		-- Get next row
		 FETCH NEXT FROM cursorCustomerGridColumns into @Title ,@Position, @OriginalTitle ,@OriginalPosition,@IsHidden
	END
	CLOSE cursorCustomerGridColumns
	DEALLOCATE cursorCustomerGridColumns

		-- Get next row
		 FETCH NEXT FROM cursorCustomerGridMap into @Controller, @Action, @Version
	END
	CLOSE cursorCustomerGridMap
	DEALLOCATE cursorCustomerGridMap
END


GO

