GO

/****** Object:  StoredProcedure [dbo].[sp_GetEventsItems]    Script Date: 02/24/2014 10:24:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[sp_GetEventsItems]
		  @orderBy nvarchar(max),
		  @PageNumber int,
		  @PageSize  int,
		  @CustomerId nvarchar(200),
		  @AssetType nvarchar(max) = '',
		  @AssetId nvarchar(max)= '',
		  @AssetName nvarchar(max)= '',
		  @EventCode nvarchar(max)= '',
		  @EventClass nvarchar(max)= '',
		  @Area nvarchar(max)= '',
		  @Zone nvarchar(max)= '',
		  @DemandArea nvarchar(max)= '',
		  @Street nvarchar(max)= '',
		  @Suburb nvarchar(max)= '',
		  @SoftwareVersion nvarchar(max)= '',
		  @CoinRejectCount nvarchar(max)= '',
		  @SignalStrength nvarchar(max)= '',
		  @VoltageMin nvarchar(max)= '',
		  @VoltageMax nvarchar(max)= '',
		  @TempMin nvarchar(max)= '',
		  @TempMax nvarchar(max)= '',
		  --todo add the other filters here
		  @StartDate nvarchar(max)= '', --required
		  @EndDate nvarchar(max)= '',--required
		  @timeType nvarchar(max)= '',
		  @viewName nvarchar(max) =''--required     
	AS
	BEGIN

	DECLARE @StartNumber nvarchar(200), @EndNumber nvarchar(200);
	SET @StartNumber = ( (@PageNumber - 1) * @PageSize ) ;
	SET @EndNumber = ( (@PageNumber) * @PageSize ) ;

	--build the where clause here
	Declare @WhereClause as Nvarchar(max);

	set @WhereClause = ' Where CustomerID  = ''' + @CustomerId + ''' ';
	If LEN(@AssetType) > 0 Set @WhereClause = @WhereClause + ' And AssetType = ''' + @AssetType + ''' ';
	If  LEN(@AssetId) > 0 Set @WhereClause = @WhereClause + ' And AssetId LIKE '''+ '%' + @AssetId + '%' + '''' ;
	If  LEN(@AssetName) > 0 Set @WhereClause = @WhereClause + ' And AssetName LIKE '''+ '%' + @AssetName + '%' + '''' ;
	If LEN(@EventCode) > 0 Set @WhereClause = @WhereClause + ' And EventCode = ''' + @EventCode + ''' ';
	If LEN(@EventClass) > 0 Set @WhereClause = @WhereClause + ' And EventClass = ''' + @EventClass + ''' ';
	If  LEN(@Area) > 0 Set @WhereClause = @WhereClause + ' And Area LIKE '''+ '%' + @Area + '%' + '''' ;
	If  LEN(@Zone) > 0 Set @WhereClause = @WhereClause + ' And Zone LIKE '''+ '%' + @Zone + '%' + '''' ;
	If  LEN(@Street) > 0 Set @WhereClause = @WhereClause + ' And Street LIKE '''+ '%' + @Street + '%' + '''' ;
	If  LEN(@Suburb) > 0 Set @WhereClause = @WhereClause + ' And Suburb LIKE '''+ '%' + @Suburb + '%' + '''' ;
	If  LEN(@DemandArea) > 0 Set @WhereClause = @WhereClause + ' And DemandArea = ''' + @DemandArea + ''' ';
	If ( LEN(@StartDate) > 0 AND LEN(@EndDate) > 0) Set @WhereClause = @WhereClause + ' And (DateTime BETWEEN '''+@StartDate+''' AND '''+@EndDate+''')  '
	If  LEN(@TimeType) > 0  Set @WhereClause = @WhereClause + ' And (TimeType1=' + @TimeType + ' Or TimeType2=' + @TimeType + ' Or TimeType3=' + @TimeType + ' Or TimeType4=' + @TimeType + ' Or TimeType5=' + @TimeType + ') ';

	If LEN(@SoftwareVersion) > 0 Set @WhereClause = @WhereClause + ' And SoftwareVersion = ''' + @SoftwareVersion + ''' ';
	If LEN(@CoinRejectCount) > 0 Set @WhereClause = @WhereClause + ' And CoinRejectCount > ''' + @CoinRejectCount + ''' ';
	If LEN(@SignalStrength) > 0 Set @WhereClause = @WhereClause + ' And SignalStrength > ''' + @SignalStrength + ''' ';
	If LEN(@VoltageMin) > 0 Set @WhereClause = @WhereClause + ' And Voltage >= ' + @VoltageMin;
	If LEN(@VoltageMax) > 0 Set @WhereClause = @WhereClause + ' And Voltage <= ' + @VoltageMax;
	If LEN(@TempMin) > 0 Set @WhereClause = @WhereClause + ' And Temperature >= ' + @TempMin;
	If LEN(@TempMax) > 0 Set @WhereClause = @WhereClause + ' And Temperature <= ' + @TempMax;
		--todo add the other filters here
         
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
	print @SelectQuery;


		  -- SET NOCOUNT ON added to prevent extra result sets from
		  -- interfering with SELECT statements.
		  SET NOCOUNT ON;

	END
GO

