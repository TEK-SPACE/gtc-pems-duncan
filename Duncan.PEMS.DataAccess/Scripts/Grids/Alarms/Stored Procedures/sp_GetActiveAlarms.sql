GO

/****** Object:  StoredProcedure [dbo].[sp_GetActiveAlarms]    Script Date: 02/24/2014 10:19:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_GetActiveAlarms]
		@orderBy nvarchar(max),
		@PageNumber int,
		@PageSize  int,
		@CustomerId nvarchar(200),
		@assetType nvarchar(max) = '',
		@assetStatus nvarchar(max) = '',
		@alarmCode nvarchar(max)= '',
		@targetService nvarchar(max)= '',
		@assetId nvarchar(max)= '',
		@operationalState nvarchar(max)= '',
		@alarmSource nvarchar(max)= '',
		@assetName nvarchar(max)= '',
		@alarmSeverity nvarchar(max)= '',
		@technicianID nvarchar(max)= '',
		@zone nvarchar(max)= '',
		@suburb nvarchar(max)= '',
		@area nvarchar(max)= '',
		@DemandArea nvarchar(max)= '',
		@location nvarchar(max)='',
		@StartDate nvarchar(max)= '', --required
		@EndDate nvarchar(max)= '',--required
		@timeType nvarchar(max)= ''


	AS
	BEGIN


	DECLARE @StartNumber nvarchar(200), @EndNumber nvarchar(200);
	SET @StartNumber = ( (@PageNumber - 1) * @PageSize ) ;
	SET @EndNumber = ( (@PageNumber) * @PageSize ) ;

	--build the where clause here
	Declare @WhereClause as Nvarchar(max);

	set @WhereClause = ' Where CustomerID  = ''' + @CustomerId + ''' ';
	If LEN(@alarmCode) > 0 Set @WhereClause = @WhereClause + ' And AlarmCode = ''' + @alarmCode + ''' ';
	If LEN(@assetType) > 0 Set @WhereClause = @WhereClause + ' And AssetType = ''' + @assetType + ''' ';
	If  LEN(@targetService) > 0 Set @WhereClause = @WhereClause + ' And TargetService = ''' + @targetService + ''' ';
	If ( LEN(@StartDate) > 0 AND LEN(@EndDate) > 0) Set @WhereClause = @WhereClause + ' And (TimeOfOccurrance BETWEEN '''+@StartDate+''' AND '''+@EndDate+''')  '
	If  LEN(@assetId) > 0 Set @WhereClause = @WhereClause + ' And MeterId LIKE '''+ '%' + @assetId + '%' + '''' ;
	If  LEN(@operationalState) > 0 Set @WhereClause = @WhereClause + ' And AssetState  = ''' + @operationalState + ''' ';
	If  LEN(@alarmSource) > 0 Set @WhereClause = @WhereClause + ' And AlarmSourceDesc   = ''' + @alarmSource + ''' ';
	If  LEN(@timeType) > 0  Set @WhereClause = @WhereClause + ' And (TimeType1=' + @timeType + ' Or TimeType2=' + @timeType + ' Or TimeType3=' + @timeType + ' Or TimeType4=' + @timeType + ' Or TimeType5=' + @timeType + ') ';
	If  LEN(@assetName) > 0 Set @WhereClause = @WhereClause + ' And AssetName LIKE '''+ '%' + @assetName + '%' + '''' ;
	If  LEN(@alarmSeverity) > 0 Set @WhereClause = @WhereClause + ' And AlarmSeverity = ''' + @alarmSeverity + ''' ';
	If  LEN(@technicianID) > 0 Set @WhereClause = @WhereClause + ' And TechnicianId = ''' + @technicianID + ''' ';
	If  LEN(@zone) > 0 Set @WhereClause = @WhereClause + ' And Zone = ''' + @zone + ''' ';
	If  LEN(@suburb) > 0 Set @WhereClause = @WhereClause + ' And Suburb = ''' + @suburb + ''' ';
	If  LEN(@area) > 0 Set @WhereClause = @WhereClause + ' And Area  LIKE '''+ '%' + @area + '%' + '''' ;
	If  LEN(@DemandArea) > 0 Set @WhereClause = @WhereClause + ' And DemandArea  LIKE ''' + '%' + @DemandArea + '%' + '''' ;
	If  LEN(@location) > 0 Set @WhereClause = @WhereClause + ' And Location  LIKE ''' + '%' + @location + '%' + '''' ;

	-- print @WhereClause;
	 --set your view name here
	DECLARE @viewName nvarchar(200);
	set @viewName = 'pv_ActiveAlarms';
	if @assetStatus = 'Open' set @viewName = 'pv_ActiveAlarms';
	if @assetStatus = 'Closed' set @viewName = 'pv_HistoricAlarms';
	if @assetStatus = '' set @viewName = 'pv_CombineActiveHistoricalAlarms';

	--determine the total count
	declare @totalCount Nvarchar(max);
	declare @totalTable AS TABLE (col int);  
	DECLARE @CountQuery AS NVARCHAR(MAX);
	set @CountQuery = 'SELECT Count(*) FROM ['+@viewName+'] ' + @WhereClause
	 INSERT into @totalTable EXECUTE sp_executesql @CountQuery;
	set @totalCount = (select top 1 * from @totalTable);
	--build the order by clause
	Declare @OrderByClause as Nvarchar(max);
	set @OrderByClause = @orderBy;

	--build the select query - 
	DECLARE @SelectQuery AS NVARCHAR(MAX);
	set @SelectQuery = 'Select * from (SELECT *, '+@totalCount+' as Count, ROW_NUMBER() OVER (ORDER BY '+@OrderByClause+' ) as RowNumber FROM ['+@viewName+']   '+ @WhereClause+'  ) Seq Where ( Seq.RowNumber BETWEEN '+@StartNumber+' AND '+@EndNumber+' )';
	EXECUTE sp_executesql @SelectQuery;
	--print @SelectQuery;


		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

	END
GO

