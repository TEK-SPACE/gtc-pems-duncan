GO
/****** Object:  StoredProcedure [dbo].[sp_InsertAllCustomGridOptions]    Script Date: 02/10/2014 10:18:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[sp_InsertAllCustomGridOptions]
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

--remove all the existing custom options
delete from CustomerGridTemplateCol where CustomerGridTemplateId in (select CustomerGridTemplateId from CustomerGridTemplate )
delete from CustomerGridTemplate 

--
DECLARE @LastId int


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ALARMS / GETITEMS ------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Alarms', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time of Complaint', @Position = 0, @OriginalTitle  = 'Time of Complaint',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Id', @Position = 1, @OriginalTitle  = 'Alarm Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Code', @Position = 2, @OriginalTitle  = 'Alarm Code',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 3, @OriginalTitle  = 'Description',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 4, @OriginalTitle  = 'Asset Type',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 5, @OriginalTitle  = 'Asset Id',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 6, @OriginalTitle  = 'Asset Name',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity', @Position = 8, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 9, @OriginalTitle  = 'Asset State',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Status', @Position = 10, @OriginalTitle  = 'Alarm Status',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Remaining Until Target Time', @Position = 11, @OriginalTitle  = 'Time Remaining Until Target Time',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 12, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 13, @OriginalTitle  = 'Area',@OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 14, @OriginalTitle  = 'Zone',@OriginalPosition = 14, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 15, @OriginalTitle  = 'Suburb',@OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Source', @Position = 16, @OriginalTitle  = 'Alarm Source',@OriginalPosition = 16, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician Id', @Position = 17, @OriginalTitle  = 'Technician Id',@OriginalPosition = 17, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETSUMMARYASSETS ----------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetSummaryAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 4, @OriginalTitle  = 'Street',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6, @OriginalTitle  = 'Area Id2',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 8, @OriginalTitle  = 'Suburb',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Latitude', @Position = 9, @OriginalTitle  = 'Latitude',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Longitude', @Position = 10, @OriginalTitle  = 'Longitude',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Spaces Count', @Position = 11, @OriginalTitle  = 'Spaces Count',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Status',@OriginalPosition = 12, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetSummaryAssets', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 4, @OriginalTitle  = 'Street',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6, @OriginalTitle  = 'Area Id2',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 12, @OriginalTitle  = 'Suburb',@OriginalPosition = 8, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Latitude', @Position = 9, @OriginalTitle  = 'Latitude',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Longitude', @Position = 10, @OriginalTitle  = 'Longitude',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Spaces Count', @Position = 8, @OriginalTitle  = 'Spaces Count',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Status',@OriginalPosition = 12, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETOCCUPANCYASSETS --------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetOccupancyAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 3, @OriginalTitle  = 'Meter Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Name', @Position = 4, @OriginalTitle  = 'Sensor Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5, @OriginalTitle  = 'Operational Status',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status Date', @Position = 6, @OriginalTitle  = 'Operational Status Date',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 7, @OriginalTitle  = 'Occupancy Status',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status Date', @Position = 8, @OriginalTitle  = 'Occupancy Status Date',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Non Compliance Status', @Position = 9, @OriginalTitle  = 'Non Compliance Status',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Non Compliance Status Date', @Position = 10, @OriginalTitle  = 'Non Compliance Status Date',@OriginalPosition = 10, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETFUNCTIONALSTATUSASSETS -------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetFunctionalStatusAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 4, @OriginalTitle  = 'Operational Status',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status Date', @Position = 5, @OriginalTitle  = 'Operational Status Date',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Class', @Position = 6, @OriginalTitle  = 'Alarm Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Code', @Position = 7, @OriginalTitle  = 'Alarm Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Duration', @Position = 8, @OriginalTitle  = 'Alarm Duration',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Repair Target Time', @Position = 9, @OriginalTitle  = 'Alarm Repair Target Time',@OriginalPosition = 9, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- ASSETS / GETCONFIGURATIONASSETS ----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Assets', @Action = 'GetConfigurationAssets', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0, @OriginalTitle  = '',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 1, @OriginalTitle  = 'Asset Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 2, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Model', @Position = 3, @OriginalTitle  = 'Asset Model',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Installed', @Position = 4, @OriginalTitle  = 'Date Installed',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration Id', @Position = 5, @OriginalTitle  = 'Configuration Id',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 6, @OriginalTitle  = 'Config Creation Date',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Schedule Date', @Position = 7, @OriginalTitle  = 'Config Schedule Date',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Activation Date', @Position = 8, @OriginalTitle  = 'Config Activation Date',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Firmware Version', @Position = 9, @OriginalTitle  = 'Firmware Version',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Software Version', @Position = 10, @OriginalTitle  = 'Software Version',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Mpv Version', @Position = 11, @OriginalTitle  = 'Mpv Version',@OriginalPosition =11, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETAGGMETERS ---------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggMeters', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 0, @OriginalTitle  = 'Meter Id',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 1, @OriginalTitle  = 'Meter Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 2, @OriginalTitle  = 'Date Time',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area Id', @Position = 3, @OriginalTitle  = 'Area Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone Id', @Position = 4, @OriginalTitle  = 'Zone Id',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 5, @OriginalTitle  = 'Suburb',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6, @OriginalTitle  = 'Street',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Meter', @Position = 7, @OriginalTitle  = 'Amt Meter',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Vendor', @Position = 8, @OriginalTitle  = 'Amt Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Chip', @Position = 9, @OriginalTitle  = 'Amt Chip',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Difference Flag', @Position = 10, @OriginalTitle  = 'Difference Flag',@OriginalPosition =10, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggMeters', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 0, @OriginalTitle  = 'Meter Id',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Name', @Position = 1, @OriginalTitle  = 'Meter Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 2, @OriginalTitle  = 'Date Time',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area Id', @Position = 6, @OriginalTitle  = 'Area Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone Id', @Position = 7, @OriginalTitle  = 'Zone Id',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 9, @OriginalTitle  = 'Suburb',@OriginalPosition = 5, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8, @OriginalTitle  = 'Street',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Meter', @Position = 3, @OriginalTitle  = 'Amt Meter',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Vendor', @Position = 4, @OriginalTitle  = 'Amt Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Chip', @Position = 5, @OriginalTitle  = 'Amt Chip',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Difference Flag', @Position = 10, @OriginalTitle  = 'Difference Flag',@OriginalPosition =10, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETAGGREGATIONS ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggregations', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 1, @OriginalTitle  = 'Date Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 2, @OriginalTitle  = 'Route Id',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Vendor Name', @Position = 3, @OriginalTitle  = 'Vendor Id',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters To Collect', @Position = 4, @OriginalTitle  = 'Meters To Collect',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters Collected', @Position = 5, @OriginalTitle  = 'Meters Collected',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Meter', @Position = 6, @OriginalTitle  = 'Total Collected Meter',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Chip', @Position = 7, @OriginalTitle  = 'Total Collected Chip',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Vendor', @Position = 8, @OriginalTitle  = 'Total Collected Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Variance Amount', @Position = 9, @OriginalTitle  = 'Amount Diff',@OriginalPosition = 9, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetAggregations', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 1, @OriginalTitle  = 'Date Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 3, @OriginalTitle  = 'Route Id',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Vendor Name', @Position = 9, @OriginalTitle  = 'Vendor Id',@OriginalPosition = 3, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters To Collect', @Position = 4, @OriginalTitle  = 'Meters To Collect',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meters Collected', @Position = 5, @OriginalTitle  = 'Meters Collected',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Meter', @Position = 6, @OriginalTitle  = 'Total Collected Meter',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Chip', @Position = 7, @OriginalTitle  = 'Total Collected Chip',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Collected Vendor', @Position = 8, @OriginalTitle  = 'Total Collected Vendor',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Variance Amount', @Position = 2, @OriginalTitle  = 'Amount Diff',@OriginalPosition = 9, @IsHidden = 0


---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- COLLECTIONS / GETROUTES ------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Collections', @Action = 'GetRoutes', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Collection Route Name', @Position = 0, @OriginalTitle  = 'Route Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Configuration ID', @Position = 1, @OriginalTitle  = 'Route Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Status', @Position = 2, @OriginalTitle  = 'Status',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Created', @Position = 3, @OriginalTitle  = 'Date Created',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Activated', @Position = 4, @OriginalTitle  = 'Date Activated',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Number Of Meters', @Position = 5, @OriginalTitle  = 'Number Of Meters',@OriginalPosition = 5, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- DISCOUNTS / GETITEMS ---------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Discounts', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Last Name', @Position = 0, @OriginalTitle  = 'Last Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'First Name', @Position = 1, @OriginalTitle  = 'First Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Email', @Position = 2, @OriginalTitle  = 'Email',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Pending', @Position = 3, @OriginalTitle  = 'Pending Count',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Approved', @Position = 4, @OriginalTitle  = 'Approved Count',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Rejected', @Position = 5, @OriginalTitle  = 'Rejected Count',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Account Status', @Position = 6, @OriginalTitle  = 'Account Status',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 7, @OriginalTitle  = 'Creation Date',@OriginalPosition = 7, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Discounts', @Action = 'GetItems', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Last Name', @Position = 0, @OriginalTitle  = 'Last Name',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'First Name', @Position = 1, @OriginalTitle  = 'First Name',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Email', @Position = 2, @OriginalTitle  = 'Email',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Pending', @Position = 5, @OriginalTitle  = 'Pending Count',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Approved', @Position =6, @OriginalTitle  = 'Approved Count',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Rejected', @Position = 7, @OriginalTitle  = 'Rejected Count',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Account Status', @Position = 3, @OriginalTitle  = 'Account Status',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 4, @OriginalTitle  = 'Creation Date',@OriginalPosition = 7, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETSUMMARYEVENTS ----------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID =   [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetSummaryEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 2, @OriginalTitle  = 'Description',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3, @OriginalTitle  = 'Asset Type',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4, @OriginalTitle  = 'Asset ID',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 5, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6, @OriginalTitle  = 'Street',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Class', @Position = 7, @OriginalTitle  = 'Event Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Code', @Position = 8, @OriginalTitle  = 'Event Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 9, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 10, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 11, @OriginalTitle  = 'Suburb',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Area',@OriginalPosition =11, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetSummaryEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Description', @Position = 2, @OriginalTitle  = 'Description',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3, @OriginalTitle  = 'Asset Type',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4, @OriginalTitle  = 'Asset ID',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 5, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Class', @Position = 6, @OriginalTitle  = 'Event Class',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Code', @Position = 7, @OriginalTitle  = 'Event Code',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8, @OriginalTitle  = 'Street',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 9, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 10, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 11, @OriginalTitle  = 'Suburb',@OriginalPosition =10, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 12, @OriginalTitle  = 'Demand Area',@OriginalPosition =11, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETDIAGNOSTICEVENTS (Functional status)------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC  @LastID =   [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetDiagnosticEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Date/Time', @Position = 0, @OriginalTitle  = 'Event Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Received Date/Time', @Position = 1, @OriginalTitle  = 'Received Date/Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 2, @OriginalTitle  = 'Event ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position =4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Type', @Position = 5, @OriginalTitle  = 'Type',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Value', @Position = 6, @OriginalTitle  = 'Value',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 8, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 9, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 10, @OriginalTitle  = 'Suburb',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Area',@OriginalPosition = 11, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetDiagnosticEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Date/Time', @Position = 0, @OriginalTitle  = 'Event Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Received Date/Time', @Position = 1, @OriginalTitle  = 'Received Date/Time',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 2, @OriginalTitle  = 'Event ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position =4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Type', @Position = 5, @OriginalTitle  = 'Type',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Value', @Position = 6, @OriginalTitle  = 'Value',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7, @OriginalTitle  = 'Street',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 8, @OriginalTitle  = 'Area',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 9, @OriginalTitle  = 'Zone',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 10, @OriginalTitle  = 'Suburb',@OriginalPosition = 10, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Demand Area', @Position = 11, @OriginalTitle  = 'Demand Area',@OriginalPosition = 11, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETALARMEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetAlarmEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Notified', @Position = 5, @OriginalTitle  = 'Time Notified',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 6, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Due SLA', @Position = 7, @OriginalTitle  = 'Time Due SLA',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Description', @Position = 8, @OriginalTitle  = 'Alarm Description',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Source' , @Position = 9, @OriginalTitle  = 'Source',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity' , @Position = 10, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order Id', @Position = 11, @OriginalTitle  ='Work Order Id',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Resolution Code', @Position = 12, @OriginalTitle  ='Resolution Code',@OriginalPosition =11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 13, @OriginalTitle  ='Technician',@OriginalPosition =12, @IsHidden = 0

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetAlarmEvents', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event ID', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Notified', @Position = 5, @OriginalTitle  = 'Time Notified',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Cleared', @Position = 6, @OriginalTitle  = 'Time Cleared',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Description', @Position = 7, @OriginalTitle  = 'Alarm Description',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Source' , @Position = 8, @OriginalTitle  = 'Source',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Alarm Severity' , @Position = 9, @OriginalTitle  = 'Alarm Severity',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order Id', @Position = 10, @OriginalTitle  ='Work Order Id',@OriginalPosition =10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Resolution Code', @Position = 11, @OriginalTitle  ='Resolution Code',@OriginalPosition =11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 12, @OriginalTitle  ='Technician',@OriginalPosition =12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Due SLA', @Position = 13, @OriginalTitle  = 'Time Due SLA',@OriginalPosition = 6, @IsHidden = 1





---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCOLLECTIONCBREVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetCollectionCBREvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position =1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 2, @OriginalTitle  = 'Asset ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 3, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Removal Time', @Position = 4, @OriginalTitle  = 'Removal Time',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Insertion Time', @Position = 5, @OriginalTitle  = 'Insertion Time',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Active', @Position = 6, @OriginalTitle  = 'Time Active',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CBID', @Position = 7, @OriginalTitle  = 'CBID',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sequence Number', @Position = 8, @OriginalTitle  = 'Sequence Number',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Auto', @Position = 9, @OriginalTitle  = 'Amt Auto',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Manual', @Position = 10, @OriginalTitle  = 'Amt Manual',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amt Difference', @Position = 11, @OriginalTitle  = 'Amt Difference',@OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operator Id', @Position = 12, @OriginalTitle  = 'Operator Id',@OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Version', @Position = 13, @OriginalTitle  = 'Version',@OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction File Name', @Position = 14, @OriginalTitle  = 'Transaction File Name',@OriginalPosition = 13, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCOLLECTIONCOMMEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetCollectionCommEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position = 0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 2, @OriginalTitle  = 'Asset ID',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 3, @OriginalTitle  = 'Asset Name',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Insertion Time', @Position = 4, @OriginalTitle  = 'Insertion Time',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount', @Position = 5, @OriginalTitle  = 'Amount',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Previous CBID', @Position = 6, @OriginalTitle  = 'Previous CBID',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'New CBID', @Position = 7, @OriginalTitle  = 'New CBID',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sequence Number', @Position = 8, @OriginalTitle  = 'Sequence Number',@OriginalPosition = 7, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETCONNECTIONEVENTS -------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetConnectionEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position =0, @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Event Id', @Position = 1, @OriginalTitle  = 'Event ID',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2, @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4, @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Connection Status', @Position = 5, @OriginalTitle  = 'Connection Status',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Error Description', @Position = 6, @OriginalTitle  = 'Error Description',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'End Time', @Position = 7, @OriginalTitle  = 'End Time',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Period', @Position = 8, @OriginalTitle  = 'Period',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Port', @Position =9, @OriginalTitle  = 'Port',@OriginalPosition = 8, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- EVENTS / GETTRANSACTIONEVENTS ------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Events', @Action = 'GetTransactionEvents', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date/Time', @Position =0,  @OriginalTitle  = 'Date/Time',@OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position =1,  @OriginalTitle  = 'Transaction Id',@OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position =2,  @OriginalTitle  = 'Asset Type',@OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 3, @OriginalTitle  = 'Asset ID',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,  @OriginalTitle  = 'Asset Name',@OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Payment Type', @Position = 5,  @OriginalTitle  = 'Payment Type',@OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay', @Position = 6,  @OriginalTitle  = 'Bay',@OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 7,  @OriginalTitle  = 'Time Paid',@OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount', @Position = 8,  @OriginalTitle  = 'Amount',@OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Card Type', @Position = 9,  @OriginalTitle  = 'Card Type',@OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Card Status', @Position = 10,  @OriginalTitle  = 'Card Status',@OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Receipt Number', @Position = 11, @OriginalTitle  = 'Receipt Number',@OriginalPosition = 10, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- OCCUPANCY / GETITEMS ---------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Occupancy', @Action = 'GetItems', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Arrival Time', @Position = 0,   @OriginalTitle  = 'Arrival Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Number', @Position = 1,   @OriginalTitle  = 'Bay Number', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Name', @Position = 2,   @OriginalTitle  = 'Bay Name', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Departure Time', @Position = 3,   @OriginalTitle  = 'Departure Time', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 4,   @OriginalTitle  = 'Occupancy Status', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5,   @OriginalTitle  = 'Operational Status', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 6,   @OriginalTitle  = 'Meter Id', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Id', @Position = 7,   @OriginalTitle  = 'Sensor Id', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8,   @OriginalTitle  = 'Street', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Occupied', @Position = 9,   @OriginalTitle  = 'Duration Occupied', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Purchased', @Position = 10,  @OriginalTitle  = 'Duration Purchased', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Paid', @Position = 11,  @OriginalTitle  = 'Total Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 12,  @OriginalTitle  = 'Area', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 13,  @OriginalTitle  = 'Zone', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 14,  @OriginalTitle  = 'Suburb', @OriginalPosition = 14, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Type', @Position = 15,  @OriginalTitle  = 'Space Type', @OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 16,  @OriginalTitle  = 'Space Id', @OriginalPosition = 16, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Occupancy', @Action = 'GetItems', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Arrival Time', @Position = 0,   @OriginalTitle  = 'Arrival Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Number', @Position = 1,   @OriginalTitle  = 'Bay Number', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Bay Name', @Position = 2,   @OriginalTitle  = 'Bay Name', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Departure Time', @Position = 3,   @OriginalTitle  = 'Departure Time', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Occupancy Status', @Position = 4,   @OriginalTitle  = 'Occupancy Status', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Operational Status', @Position = 5,   @OriginalTitle  = 'Operational Status', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Meter Id', @Position = 6,   @OriginalTitle  = 'Meter Id', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Sensor Id', @Position = 7,   @OriginalTitle  = 'Sensor Id', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 8,   @OriginalTitle  = 'Street', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Occupied', @Position = 9,   @OriginalTitle  = 'Duration Occupied', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Duration Purchased', @Position = 10,  @OriginalTitle  = 'Duration Purchased', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Total Paid', @Position = 11,  @OriginalTitle  = 'Total Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 12,  @OriginalTitle  = 'Area', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 13,  @OriginalTitle  = 'Zone', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Suburb', @Position = 16,  @OriginalTitle  = 'Suburb', @OriginalPosition = 14, @IsHidden = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Type', @Position = 14,  @OriginalTitle  = 'Space Type', @OriginalPosition = 15, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 16,  @OriginalTitle  = 'Space Id', @OriginalPosition = 16, @IsHidden = 0

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- TRANSACTIONS / GETTRANSACTIONS -----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------
EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'Transactions', @Action = 'GetTransactions', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 0,    @OriginalTitle  = 'Date Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position = 1,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 3,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 5,    @OriginalTitle  = 'Area', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 6,    @OriginalTitle  = 'Street', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Type', @Position = 7,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 8,    @OriginalTitle  = 'Space Id', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Gateway Id', @Position = 9,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CC Last 4', @Position = 10,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Paid', @Position = 11,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 12,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Status', @Position = 13,   @OriginalTitle  = 'Space Status', @OriginalPosition = 13, @IsHidden = 0

EXEC    @LastID = [sp_InsertCustomerGridTemplate] @Controller = 'Transactions', @Action = 'GetTransactions', @Version = 1
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Date Time', @Position = 0,    @OriginalTitle  = 'Date Time', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Id', @Position = 1,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 2,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Id', @Position = 3,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Name', @Position = 4,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6,    @OriginalTitle  = 'Area', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 7,    @OriginalTitle  = 'Street', @OriginalPosition = 6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Transaction Type', @Position = 5,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Id', @Position = 8,    @OriginalTitle  = 'Space Id', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Gateway Id', @Position = 9,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'CC Last 4', @Position = 10,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Amount Paid', @Position = 11,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Time Paid', @Position = 12,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Space Status', @Position = 13,   @OriginalTitle  = 'Space Status', @OriginalPosition = 13, @IsHidden = 1

---------------------------------------------------------------------------------------------------------------------------------------------------------
-------------------------------------------------------------------- WORKORDERS / GETWORKORDERS -----------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------

EXEC   @LastID =  [sp_InsertCustomerGridTemplate] @Controller = 'WorkOrders', @Action = 'GetWorkOrders', @Version = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'', @Position = 0,    @OriginalTitle  = '', @OriginalPosition = 0, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order', @Position = 1,    @OriginalTitle  = 'Date Time', @OriginalPosition = 1, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Customer', @Position = 2,    @OriginalTitle  = 'Transaction Id', @OriginalPosition = 2, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset Type', @Position = 3,    @OriginalTitle  = 'Asset Type', @OriginalPosition = 3, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Asset ID', @Position = 4,    @OriginalTitle  = 'Asset Id', @OriginalPosition = 4, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Street', @Position = 5,    @OriginalTitle  = 'Asset Name', @OriginalPosition = 5, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Area', @Position = 6,    @OriginalTitle  = 'Area', @OriginalPosition =6, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Zone', @Position = 7,    @OriginalTitle  = 'Street', @OriginalPosition = 7, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Highest Priority', @Position = 8,    @OriginalTitle  = 'Transaction Type', @OriginalPosition = 8, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Creation Date', @Position = 9,    @OriginalTitle  = 'Space Id', @OriginalPosition = 9, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Deadline', @Position = 10,    @OriginalTitle  = 'Gateway Id', @OriginalPosition = 10, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'# of Event IDs', @Position = 11,   @OriginalTitle  = 'CC Last 4', @OriginalPosition = 11, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Work Order State', @Position = 12,   @OriginalTitle  = 'Amount Paid', @OriginalPosition = 12, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Technician', @Position = 13,   @OriginalTitle  = 'Time Paid', @OriginalPosition = 13, @IsHidden = 0
EXEC  [sp_InsertCustomerGridTemplateCol] @CustomerGridTemplateId = @LastID, @Title = N'Assigned Date', @Position = 14,   @OriginalTitle  = 'Space Status', @OriginalPosition = 14, @IsHidden = 0
END
GO

