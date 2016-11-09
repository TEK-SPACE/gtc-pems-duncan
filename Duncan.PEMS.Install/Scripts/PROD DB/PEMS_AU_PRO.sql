--****************************************************************************************
-- Created 04/01/2014 - Sergey Ostrerov. Version v. 01.00.000.0001
-- Production database PEMS_AU_PRO script: all database objects.
--****************************************************************************************

USE [PEMS_AU_PRO]
GO
/****** Object:  User [UDPPEMSAUPROD]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [UDPPEMSAUPROD] FOR LOGIN [UDPPEMSAUPROD] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [RipNetAdminAUPROD]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [RipNetAdminAUPROD] FOR LOGIN [RipNetAdminAUPROD] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\ssomanchi]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\ssomanchi] FOR LOGIN [CM\ssomanchi] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\sostrerov]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\sostrerov] FOR LOGIN [CM\sostrerov] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\sa_sqlaccount]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\sa_sqlaccount] FOR LOGIN [CM\sa_sqlaccount] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rpauli]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\rpauli] FOR LOGIN [CM\rpauli] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rhoward2]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\rhoward2] FOR LOGIN [CM\rhoward2] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\rgallardo]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\rgallardo] FOR LOGIN [CM\rgallardo] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\pbouch]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\pbouch] FOR LOGIN [CM\pbouch] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\mneupane]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\mneupane] FOR LOGIN [CM\mneupane] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [CM\amyint]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [CM\amyint] FOR LOGIN [CM\amyint] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [AUPEMSPRODEXTDB]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [AUPEMSPRODEXTDB] FOR LOGIN [AUPEMSPRODEXTDB] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [au_duncan]    Script Date: 04/01/2014 21:58:10 ******/
CREATE USER [au_duncan] FOR LOGIN [au_duncan] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[SLA_RegulatedSchedule]    Script Date: 04/01/2014 21:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SLA_RegulatedSchedule](
	[SLA_RegulatedScheduleID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[RegulatedStartMinuteOfDay] [int] NOT NULL,
	[RegulatedEndMinuteOfDay] [int] NOT NULL,
	[ScheduleStartDate] [datetime] NULL,
	[ScheduleEndDate] [datetime] NULL,
 CONSTRAINT [PK_SLA_RegulatedSchedule] PRIMARY KEY CLUSTERED 
(
	[SLA_RegulatedScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SLA_MaintenanceSchedule]    Script Date: 04/01/2014 21:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SLA_MaintenanceSchedule](
	[SLA_MaintenanceScheduleID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[MaintenanceStartMinuteOfDay] [int] NOT NULL,
	[MaintenanceEndMinuteOfDay] [int] NOT NULL,
	[ScheduleStartDate] [datetime] NULL,
	[ScheduleEndDate] [datetime] NULL,
 CONSTRAINT [PK_SLA_MaintenanceSchedule] PRIMARY KEY CLUSTERED 
(
	[SLA_MaintenanceScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DayOfWeek]    Script Date: 04/01/2014 21:59:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DayOfWeek](
	[DayOfWeekId] [int] NOT NULL,
	[DayOfWeekDesc] [varchar](50) NULL,
 CONSTRAINT [PK_DayOfWeek] PRIMARY KEY CLUSTERED 
(
	[DayOfWeekId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_DayOfWeek]    Script Date: 04/01/2014 22:05:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DayOfWeek] 	
	@ts datetime
	,@dow int output
AS
BEGIN
	select @dow = d.DayOfWeekId from DayOfWeek d
	where Upper(d.DayOfWeekDesc)  = datename(dw,@ts)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getROMScheudle]    Script Date: 04/01/2014 22:05:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getROMScheudle] 
/**
Get Regulated , Operation , Maintenance Schedule for the specified dat
**/
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@Ts datetime
	,@TableName varchar(25)
	,@start int OUTPUT
	,@end int OUTPUT
	,@startTs datetime = null output
	,@endTS datetime = null output
	
AS
Declare 
	@dow int		
	,@t int
BEGIN
	Print '------ ===============   Getting ROM Schedule : begin    =========== -------------'
	Print @TableName
		
	--set @dow = DATEPART(dw,@ts)
	exec sp_DayOfWeek @ts,@dow output
	
	set @Ts = DATEADD(dd, DATEDIFF(dd, 0, @TS), 0) -- strip hour minute
	Print '@TS = ' + convert(Varchar,@ts) + ' , @dow = ' + convert(varchar,@dow)
	set @startTS = @Ts
	set @endTS = @Ts
	
	if (@TableName = 'Regulated') begin
		select @start = r.RegulatedStartMinuteOfDay
			,@end = r.RegulatedEndMinuteOfDay
			from SLA_RegulatedSchedule r
			where r.CustomerId = @CustomerId
			and @ts between r.ScheduleStartDate and r.ScheduleEndDate
			and r.DayOfWeek = @dow
	end else if (@TableName = 'Maintenance') begin
		select @start = r.MaintenanceStartMinuteOfDay
			,@end = r.MaintenanceEndMinuteOfDay
			from SLA_MaintenanceSchedule r
			where r.CustomerId = @CustomerId
			and @ts between r.ScheduleStartDate and r.ScheduleEndDate
			and r.DayOfWeek = @dow
	end
	
	
		
	if (@start is not null) begin
		Print '@start = ' + convert(varchar,@start)
		set @startTS = DATEADD(minute,@start, @Ts)
	end else begin 
		print 'Start not scheduled!'		
	end
	
	if (@end is not null) begin
		Print '@end = ' + convert(varchar,@end)	
		set @endTS = DATEADD(minute,@end, @Ts)
	end else begin 
		print 'End not scheduled!'
	end
	
	Print '@StartTs = ' + convert(varchar,@startTs) 
	Print '@EdTs = ' + convert(varchar,@endTs)	
			
	Print '------ ===============   Getting ROM Schedule : end   =========== -------------'
	
END
GO
/****** Object:  Table [dbo].[SLA_AssetDownTime]    Script Date: 04/01/2014 22:05:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SLA_AssetDownTime](
	[SLA_AssetDownTimeID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[MeterGroup] [int] NOT NULL,
	[ReportingDate] [datetime] NOT NULL,
	[RegulatedStartMinuteOfDay] [int] NULL,
	[RegulatedEndMinuteOfDay] [int] NULL,
	[TotalRegulatedMinutes] [int] NULL,
	[TotalRegulatedDowntimeMinutes] [int] NULL,
	[TotalRegulatedNFFMinutes] [int] NULL,
	[RegulatedUpTimePCT] [int] NULL,
	[OperationStartMinuteOfDay] [int] NULL,
	[OperationEndMinuteOfDay] [int] NULL,
	[TotalOperationMinutes] [int] NULL,
	[TotalOperationDowntimeMinutes] [int] NULL,
	[TotalOperationNFFMinutes] [int] NULL,
	[OperationUpTimePC] [int] NULL,
	[MaintenanceStartMinuteOfDay] [int] NULL,
	[MaintenanceEndMinuteOfDay] [int] NULL,
	[TotalMaintenanceMinutes] [int] NULL,
	[TotalMaintenanceDowntimeMinutes] [int] NULL,
	[TotalMaintenanceNFFMinutes] [int] NULL,
	[MaintenanceUpTimePCT] [int] NULL,
	[SLAStartTimeMinuteOfDay] [int] NULL,
	[SLAEndTimeMinuteOfDay] [int] NULL,
	[TotalSLAMinutes] [int] NULL,
	[TotalSLADowntimeMinutes] [int] NULL,
	[TotalSLANFFMinutes] [int] NULL,
	[SLAUpTimePCT] [int] NULL,
 CONSTRAINT [PK_SLA_AssetDownTime] PRIMARY KEY CLUSTERED 
(
	[SLA_AssetDownTimeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Meters]    Script Date: 04/01/2014 22:05:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Meters](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[SMSNumber] [varchar](22) NULL,
	[MeterStatus] [int] NOT NULL,
	[TimeZoneID] [int] NOT NULL,
	[MeterRef] [int] NOT NULL,
	[EmporiaKey] [char](32) NULL,
	[MeterName] [varchar](20) NULL,
	[Location] [varchar](50) NULL,
	[BayStart] [int] NULL,
	[BayEnd] [int] NULL,
	[Description] [varchar](50) NULL,
	[GSMNumber] [varchar](16) NOT NULL,
	[SchedServTime] [int] NOT NULL,
	[RSFName] [varchar](128) NULL,
	[RSFDateTime] [smalldatetime] NULL,
	[BarCode] [varchar](20) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[ProgramName] [varchar](16) NULL,
	[MaxBaysEnabled] [int] NULL,
	[MeterType] [int] NULL,
	[MeterGroup] [int] NULL,
	[MParkID] [int] NULL,
	[MeterState] [int] NULL,
	[DemandZone] [int] NULL,
	[TypeCode] [int] NULL,
	[OperationalStatusID] [int] NULL,
	[InstallDate] [datetime] NULL,
	[FreeParkingMinute] [int] NULL,
	[RegulatedStatusID] [int] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
 CONSTRAINT [PK_Meters] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Meters_IDX_CMA] ON [dbo].[Meters] 
(
	[CustomerID] ASC,
	[MeterId] ASC,
	[AreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Meters_IDXGlobalMeterID] ON [dbo].[Meters] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_pct]    Script Date: 04/01/2014 22:05:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_pct] 
	@startMinuteOfDay int,
	@endMinuteOfDay int,
	@downtime int,
	@totalMinuteOfDay int output,
	@totalUpTime int output,
	@upTimePct int output
AS
BEGIN
	Print '-- ===================================    sp_SLA_pct:begin  ==============================='
	if (@startMinuteOfDay is not null) and (@endMinuteOfDay is not null) begin
		print 'Calculating percentage'
		set @totalMinuteOfDay = @endMinuteOfDay - @startMinuteOfDay
		set @totalUpTime = @totalMinuteOfDay - @downtime			
		set @upTimePct = (@totalUpTime*100)/@totalMinuteOfDay
	end else begin 
		print 'startMinuteOfDay or endMinuteOfDay is null'
		set @totalMinuteOfDay = 0
		set @totalUpTime = 0
		set @upTimePct = 0
	end 
	Print '-- ===================================    sp_SLA_pct:end  ==============================='
END
GO
/****** Object:  StoredProcedure [dbo].[sp_util_MinMax]    Script Date: 04/01/2014 22:05:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_util_MinMax] 
	@txt varchar(200),
	@num1 int,
	@num2 int,
	@ts1 datetime,
	@ts2 datetime,
	@result int output,
	@resultTs datetime output
AS
BEGIN
	if (@num1 is null) or (@num2 is null) begin
		Print 'Num1 or Num2 is null'		
	end else begin
		if (@txt = 'min') begin
			if (@num1 < @num2) begin
				set @result = @num1
			end else begin
				set @result = @num2
			end
		end	else begin -- MAX
			if (@num1 > @num2) begin
				set @result = @num1
			end else begin
				set @result = @num2
			end
		end
		Print '@num1= ' + convert(varchar,@num1)
			+' ,@num2= ' + convert(varchar,@num2)
			+ ' ,@result= ' + convert(varchar,@num1)
	end 
	
	
	if (@ts1 is null) or (@ts2 is null) begin
		Print '@ts1 or @ts2 is null'		
	end else begin
		if (@txt = 'min') begin
			if (@ts1 < @ts2) begin
				set @resultTs = @ts1
			end else begin
				set @resultTs = @ts2
			end
		end	else begin -- MAX
			if (@ts1 > @num2) begin
				set @resultTs = @ts1
			end else begin
				set @resultTs = @ts2
			end
		end
		
		Print '@ts1= ' + convert(varchar,@ts1)
			+' ,@ts2= ' + convert(varchar,@ts2)
			+ ' ,@resultTs= ' + convert(varchar,@resultTs)
	end 
	
	
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_util_PrintDate]    Script Date: 04/01/2014 22:05:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_util_PrintDate] 
	@txt varchar(200),
	@ts datetime
AS
BEGIN
	if (@ts is null) begin
		Print @txt + ' is null'
	end else begin
		Print @txt + ' = ' + convert(varchar,@ts)
	end	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_util_PrintNum]    Script Date: 04/01/2014 22:05:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_util_PrintNum] 
	@txt varchar(200),
	@num bigint
AS
BEGIN
	if (@num is null) begin
		Print @txt + ' is null'
	end else begin
		Print @txt + ' = ' + convert(varchar,@num)
	end	
END
GO
/****** Object:  Table [dbo].[ServerInstance]    Script Date: 04/01/2014 22:05:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServerInstance](
	[Instanceid] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[GenGlobalID]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[GenGlobalID](
			@cid int,
			@aid int,
			@mid int=0 ,
			@baynum int=0

			)
			RETURNS bigInt  AS  
			BEGIN 
			Declare 
			@RetVal bigINT,
			@sid int

			select top 1 @sid=instanceid from serverinstance
			if (@sid is null) begin
				set @sid = 1
			end

				if len(@sid)=1 and len(@mid)<=6
				begin

					set @RetVal=convert(bigint,cast(@sid as varchar(1))+right(replicate('0',5) + cast(@cid as varchar(5)),5)
					+ right(replicate('0',3) + cast(@aid as varchar(3)),3) + right(replicate('0',6) + cast(@mid as varchar(6)),6)
					+ right(replicate('0',4) + cast(@baynum as varchar(4)),4))
				end
				else
				begin
					set @Retval=NULL
				end

				return @RetVal	

			END
GO
/****** Object:  Table [dbo].[SFMeterMaintenanceEvent]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFMeterMaintenanceEvent](
	[EventId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[MaintenanceCode] [int] NOT NULL,
	[TechnicianId] [int] NOT NULL,
	[WorkOrderID] [int] NULL,
 CONSTRAINT [PK_SFMeterMaintenanceEvent] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [SFMeterMaintenanceEventUnique] UNIQUE NONCLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [SFMeterMaintenanceEvent_IDXGlobalMeterID] ON [dbo].[SFMeterMaintenanceEvent] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrder]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkOrder](
	[WorkOrderId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AssignedBy] [int] NOT NULL,
	[AssignedTS] [datetime] NOT NULL,
	[TechnicianID] [int] NULL,
 CONSTRAINT [PK_WorkOrder] PRIMARY KEY CLUSTERED 
(
	[WorkOrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MaintenanceCodes]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MaintenanceCodes](
	[MaintenanceCode] [int] NOT NULL,
	[Description] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MaintenanceCodes] PRIMARY KEY CLUSTERED 
(
	[MaintenanceCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NSC_Triggers_Logs]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NSC_Triggers_Logs](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[MSG] [varchar](500) NULL,
	[TS] [datetime] NULL,
	[TRG] [varchar](100) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_Trigger_Log]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Trigger_Log] 
	-- Add the parameters for the stored procedure here
	@MSG Varchar(500)
	,@TRG varchar(100)
AS
BEGIN
	INSERT INTO NSC_Triggers_Logs (MSG,TRG) values (@MSG,@TRG)	
END
GO
/****** Object:  Table [dbo].[MeterMap]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterMap](
	[Customerid] [int] NOT NULL,
	[Areaid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ZoneId] [int] NULL,
	[HousingId] [int] NOT NULL,
	[MechId] [int] NULL,
	[AreaId2] [int] NULL,
	[CollRouteId] [int] NULL,
	[EnfRouteId] [int] NULL,
	[MaintRouteId] [int] NULL,
	[id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomGroup1] [int] NULL,
	[CustomGroup2] [int] NULL,
	[CustomGroup3] [int] NULL,
	[SubAreaID] [int] NULL,
	[GatewayID] [int] NULL,
	[SensorID] [int] NULL,
	[CashBoxID] [int] NULL,
	[CollectionRunId] [bigint] NULL,
 CONSTRAINT [PK_MeterMap] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_MeterMap] UNIQUE NONCLUSTERED 
(
	[Customerid] ASC,
	[Areaid] ASC,
	[MeterId] ASC,
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sensors]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Sensors](
	[CustomerID] [int] NOT NULL,
	[SensorID] [int] NOT NULL,
	[BarCodeText] [varbinary](100) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[GSMNumber] [varchar](100) NULL,
	[GlobalMeterID] [bigint] NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Location] [varchar](100) NOT NULL,
	[SensorName] [varchar](100) NOT NULL,
	[SensorState] [int] NOT NULL,
	[SensorType] [int] NULL,
	[InstallDateTime] [datetime] NOT NULL,
	[DemandZone] [int] NULL,
	[Comments] [varchar](1000) NULL,
	[RoadWayType] [int] NULL,
	[ParkingSpaceId] [bigint] NULL,
	[SensorModel] [int] NULL,
	[OperationalStatus] [int] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
 CONSTRAINT [PK_Sensors] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[SensorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_OccupancyRateSummary_Perf4] ON [dbo].[Sensors] 
(
	[CustomerID] ASC,
	[ParkingSpaceId] ASC
)
INCLUDE ( [SensorID],
[SensorName]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Gateways]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Gateways](
	[GateWayID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Location] [varchar](250) NULL,
	[GatewayState] [int] NULL,
	[GatewayType] [int] NULL,
	[InstallDateTime] [datetime] NULL,
	[TimeZoneID] [int] NULL,
	[DemandZone] [int] NULL,
	[CAMID] [varchar](250) NULL,
	[CELID] [varchar](250) NULL,
	[PowerSource] [int] NULL,
	[HWVersion] [varchar](250) NULL,
	[Manufacturer] [varchar](250) NULL,
	[GatewayModel] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
 CONSTRAINT [PK_Gateways] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[GateWayID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MetersAudit]    Script Date: 04/01/2014 22:05:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MetersAudit](
	[MetersAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[SMSNumber] [varchar](22) NULL,
	[MeterStatus] [int] NOT NULL,
	[TimeZoneID] [int] NOT NULL,
	[MeterRef] [int] NOT NULL,
	[EmporiaKey] [char](32) NULL,
	[MeterName] [varchar](20) NULL,
	[Location] [varchar](50) NULL,
	[BayStart] [int] NULL,
	[BayEnd] [int] NULL,
	[Description] [varchar](50) NULL,
	[GSMNumber] [varchar](16) NOT NULL,
	[SchedServTime] [int] NOT NULL,
	[RSFName] [varchar](128) NULL,
	[RSFDateTime] [smalldatetime] NULL,
	[BarCode] [varchar](20) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[ProgramName] [varchar](16) NULL,
	[MaxBaysEnabled] [int] NULL,
	[MeterType] [int] NULL,
	[MeterGroup] [int] NULL,
	[MParkID] [int] NULL,
	[MeterState] [int] NULL,
	[DemandZone] [int] NULL,
	[TypeCode] [int] NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[OperationalStatus] [int] NULL,
	[InstallDate] [datetime] NULL,
	[OperationalStatusID] [int] NULL,
	[FreeParkingMinute] [int] NULL,
	[RegulatedStatusID] [int] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_MetersAudit] PRIMARY KEY CLUSTERED 
(
	[MetersAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_UpdateOperationalStatus]    Script Date: 04/01/2014 22:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UpdateOperationalStatus] 
	  @CustomerID INT,
	  @AreaID INT,
	  @MeterID INT,
	  @OperationalStatusId INT,
	  @OperationalStatusTime DATETIME
AS
	DECLARE 
		@SensorID INT,
		@GatewayID INT
	  
BEGIN
	if (@OperationalStatusTime is null) set @OperationalStatusTime = GETDATE()

	BEGIN TRY
		Print 'Updating Operational Status in Meters'
		UPDATE meters
			SET operationalstatusid = @OperationalStatusId,
			operationalstatustime = @OperationalStatusTime
			WHERE customerid = @CustomerID
			AND areaid = @AreaID
			AND meterid = @MeterID
			
		Print 'Auditing Meters'
		INSERT INTO MetersAudit
			   ([GlobalMeterId]
			   ,[CustomerID]
			   ,[AreaID]
			   ,[MeterId]
			   ,[SMSNumber]
			   ,[MeterStatus]
			   ,[TimeZoneID]
			   ,[MeterRef]
			   ,[EmporiaKey]
			   ,[MeterName]
			   ,[Location]
			   ,[BayStart]
			   ,[BayEnd]
			   ,[Description]
			   ,[GSMNumber]
			   ,[SchedServTime]
			   ,[RSFName]
			   ,[RSFDateTime]
			   ,[BarCode]
			   ,[Latitude]
			   ,[Longitude]
			   ,[ProgramName]
			   ,[MaxBaysEnabled]
			   ,[MeterType]
			   ,[MeterGroup]
			   ,[MParkID]
			   ,[MeterState]
			   ,[DemandZone]
			   ,[TypeCode]
			   ,[UserId]
			   ,[UpdateDateTime]
			   ,[OperationalStatus]
			   ,[InstallDate]
			   ,[OperationalStatusID]
			   ,[FreeParkingMinute]
			   ,[RegulatedStatusID]
			   ,[WarrantyExpiration]
			   ,[OperationalStatusTime]
			   ,[LastPreventativeMaintenance]
			   ,[NextPreventativeMaintenance]
			   ,[OperationalStatusEndTime]
			   ,[OperationalStatusComment]
			   ,[AssetPendingReasonId])
			 select 
			 [GlobalMeterId]
			   ,[CustomerID]
			   ,[AreaID]
			   ,[MeterId]
			   ,[SMSNumber]
			   ,[MeterStatus]
			   ,[TimeZoneID]
			   ,[MeterRef]
			   ,[EmporiaKey]
			   ,[MeterName]
			   ,[Location]
			   ,[BayStart]
			   ,[BayEnd]
			   ,[Description]
			   ,[GSMNumber]
			   ,[SchedServTime]
			   ,[RSFName]
			   ,[RSFDateTime]
			   ,[BarCode]
			   ,[Latitude]
			   ,[Longitude]
			   ,[ProgramName]
			   ,[MaxBaysEnabled]
			   ,[MeterType]
			   ,[MeterGroup]
			   ,[MParkID]
			   ,[MeterState]
			   ,[DemandZone]
			   ,[TypeCode]
			   ,0--[UserId]
			   ,GETDATE()--[UpdateDateTime]
			   ,[OperationalStatusId]
			   ,[InstallDate]
			   ,[OperationalStatusID]
			   ,[FreeParkingMinute]
			   ,[RegulatedStatusID]
			   ,[WarrantyExpiration]
			   ,[OperationalStatusTime]
			   ,[LastPreventativeMaintenance]
			   ,[NextPreventativeMaintenance]
			   ,[OperationalStatusEndTime]
			   ,[OperationalStatusComment]
			   ,Null--@AssetPendingReasonId
			  from Meters m
			  where m.CustomerID = @CustomerId
					and m.AreaID = @AreaId
					and m.MeterId = @MeterId		

		Print 'Audited'

		SELECT @SensorID = sensorid,
			@GatewayID = gatewayid
			FROM metermap
			WHERE customerid = @CustomerID
			AND areaid = @AreaID
			AND meterid = @MeterID

		IF( @SensorID IS NOT NULL )	BEGIN
			Print 'Updating Operational Status in Sensors'
			UPDATE sensors
				SET operationalstatus = @OperationalStatusId,
				operationalstatustime = @OperationalStatusTime
				WHERE customerid = @CustomerID
				AND sensorid = @SensorID
		END

		IF( @GatewayID IS NOT NULL )BEGIN
			Print 'Updating Operational Status in Gateways'
			UPDATE gateways
				SET operationalstatus = @OperationalStatusId,
				operationalstatustime = @OperationalStatusTime
				WHERE customerid = @CustomerID
				AND gatewayid = @GatewayID
		END	
	
	END TRY
	BEGIN CATCH
		print 'ERROR:' + ERROR_MESSAGE()
	END CATCH

END
GO
/****** Object:  Table [dbo].[EventUIDGen]    Script Date: 04/01/2014 22:05:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventUIDGen](
	[EventUID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GenDate] [datetime] NOT NULL,
 CONSTRAINT [PK_EventUIDGen] PRIMARY KEY CLUSTERED 
(
	[EventUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetEventUID]    Script Date: 04/01/2014 22:05:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetEventUID] 
	-- Add the parameters for the stored procedure here
	  @EventUID bigint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	Insert INTO dbo.EventUIDGen	([GenDate]) VALUES	(GETDATE()) ;
	
	set @EventUID = SCOPE_IDENTITY();
	
	Return @EventUID
END
GO
/****** Object:  Table [dbo].[EventLogs]    Script Date: 04/01/2014 22:05:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventLogs](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[TechnicianKeyID] [varchar](100) NULL,
	[EventUID] [bigint] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[MigratedTs] [datetime] NULL,
 CONSTRAINT [PK_EventLogs] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC,
	[EventCode] ASC,
	[EventSource] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [EventLogs_IDX_CAMEE] ON [dbo].[EventLogs] 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC,
	[EventCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EventLogs] ON [dbo].[EventLogs] 
(
	[MeterId] ASC,
	[AreaID] ASC,
	[CustomerID] ASC,
	[EventDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_EventLogs_Time] ON [dbo].[EventLogs] 
(
	[EventDateTime] DESC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimeTypeCustomer]    Script Date: 04/01/2014 22:05:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TimeTypeCustomer](
	[TimeTypeCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TimeTypeId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[TimeTypeStartMinOfDay] [int] NOT NULL,
	[TimeTypeEndMinOfDay] [int] NOT NULL,
	[TimeTypeStartDayofWeek] [int] NULL,
	[TimeTypeEndDayofWeek] [int] NULL,
	[IsDisplayed] [bit] NOT NULL,
 CONSTRAINT [PK_TimeTypeCustomer] PRIMARY KEY CLUSTERED 
(
	[TimeTypeCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimeType]    Script Date: 04/01/2014 22:05:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeType](
	[TimeTypeId] [int] NOT NULL,
	[TimeTypeDesc] [varchar](250) NOT NULL,
	[ColumnMap] [varchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[TimeTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_TimeTypeCustomer]    Script Date: 04/01/2014 22:05:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_TimeTypeCustomer] 
	@cid INT
	,@ts DateTime
	,@tt varchar(50)
	,@TimeTypeId int OUTPUT
AS

DECLARE 
	@TimeTypeCustomerId     INT,
	@CustomerId             INT,
	@TimeTypeStartMinOfDay  INT,
	@TimeTypeEndMinOfDay    INT,
	@TimeTypeStartDayofWeek INT,
	@TimeTypeEndDayofWeek   INT,
	@DOW INT,
	@MinuteOfTheDay int
				  
BEGIN
	BEGIN TRY
		set @dow = DATEPART(dw,@ts)
		set @MinuteOfTheDay = (DATEPART(HOUR,@ts) * 60) + DATEPART(MINUTE,@TS)
		Print 'Dow = ' + convert(Varchar,@dow) + ' , Minutes = '  + convert(Varchar,@MinuteOfTheDay) + ' , tt = ' + @tt


		SELECT @TimeTypeId = tt.timetypeid 
		FROM   timetypecustomer ttc join TimeType tt 
		on ttc.TimeTypeId = tt.TimeTypeId
		WHERE  customerid = @cid
		and @DOW between TimeTypeStartDayofWeek and TimeTypeEndDayofWeek
		and @MinuteOfTheDay between TimeTypeStartMinOfDay and TimeTypeEndMinOfDay
		and Upper(ColumnMap)  = Upper(@tt)
		
		Print 'TimeTypeId = ' + convert(varchar,@TimeTypeId)
	  
	END TRY
	BEGIN CATCH
		print 'ERROR:' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[Sp_inserteventlog2]    Script Date: 04/01/2014 22:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[Sp_inserteventlog2] 
	@uid BIGINT,
	@cid INT,
	@aid INT,
	@mid INT,
	@EventCode INT,
	@EventDateTime DATETIME,
	@Type1 INT=null output,
	@Type2 INT=null output,
	@Type3 INT=null output,
	@Type4 INT=null output,
	@Type5 INT=null output
AS
Declare @gid bigint
  BEGIN
	Print 'Sp_inserteventlog2'
  BEGIN TRY
	  DECLARE @TimeType1 INT,
			  @TimeType2 INT,
			  @TimeType3 INT,
			  @TimeType4 INT
			  
		IF (@EventDateTime is null) set @EventDateTime = GETDATE()

	  IF( ( Datepart(dw, @EventDateTime) ) = 1 )
		 OR ( ( Datepart (dw, @EventDateTime) ) = 7 )
		BEGIN
			SELECT @TimeType1 = timetypeid
			FROM   timetype
			WHERE  timetypedesc LIKE 'Weekend'
		END
	  ELSE
		BEGIN
			SELECT @TimeType1 = timetypeid
			FROM   timetype
			WHERE  timetypedesc LIKE 'Weekday'
		END

	  SELECT @TimeType2 = timetypeid
	  FROM   timetype
	  WHERE  timetypedesc LIKE ( Datename (dw, @EventDateTime) )
	  
	  -- Morning/Evening
	  exec sp_TimeTypeCustomer @cid,@EventDateTime,'TimeType3',@TimeType3 output
	  
	  --Peak	  
	  exec sp_TimeTypeCustomer @cid,@EventDateTime,'TimeType4',@TimeType4 output
	  
	  
	  if not exists(select * from EventLogs where CustomerID = @cid and MeterId = @mid and AreaID = @aid and 
	  EventDateTime = @EventDateTime and EventCode = @EventCode and EventSource = 0)
	  begin
		print 'Inserting into EventLogs ' 
		print convert(varchar,@cid) + '/' + convert(varchar,@aid) + '/' + convert(varchar,@mid) 
		select @gid= dbo.GenGlobalID(@cid,@aid,@mid,default)				
		  INSERT INTO [eventlogs]
					  ([globalmeterid],
					   [customerid],
					   [areaid],
					   [meterid],
					   [eventdatetime],
					   [eventcode],
					   [eventsource],
					   [eventuid],
					   [timetype1],
					   [timetype2],
					   [timetype3],
					   [timetype4])
		  VALUES     ( @gid,
					   @cid,
					   @aid,
					   @mid,
					   @EventDateTime,
					   @EventCode,
					   0,
					   @uid,
					   @TimeType1,
					   @TimeType2,
					   @TimeType3,
					   @TimeType4 );  
					   print 'Inserted EventLogs'
	  end
	  else begin
		print 'Updating into EventLogs'
		Update EventLogs
		set EventUID  = @uid
		where CustomerID = @cid and MeterId = @mid and AreaID = @aid and 
		EventDateTime = @EventDateTime and EventCode = @EventCode and EventSource = 0
	  end 

	        
				   				   
	select @Type1=@TimeType1,@Type2=@TimeType2,@Type3=@TimeType3,@Type4=@TimeType4,@Type5=null
	
	END TRY	
	BEGIN CATCH
		print 'ERROR:' + ERROR_MESSAGE()
	END CATCH   
			   
  END
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getSLAStartOrEnd]    Script Date: 04/01/2014 22:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getSLAStartOrEnd] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int,
	@TS datetime,
	@Start bit, --1 indicates 'start' and 0 indicates 'end'
	@SLATarget	datetime output
AS
Declare
	@dow int
	,@justdate datetime -- date without time
	,@maintenanceStart int
	,@regulatedStart int
	,@maintenanceEnd int
	,@regulatedEnd int	
	,@mdow int
	,@rdow int
	,@dowOffset int
	,@midnight datetime
BEGIN
	if (@Start = 1) begin
		Print '------------------------------------ sp_SLA_getSLAStartOrEnd : SLA start -----------------' 
	end else begin
		Print '------------------------------------ sp_SLA_getSLAStartOrEnd : SLA end -----------------' 
	end 
	if (@TS is null) begin
	Print 'Incoming is null return 24 hours from now'
		set @SLATarget = dateadd(dd,1,getdate())
		return 
	end
	
	Print 'Incoming = '  + convert(varchar,@TS)	
	set @midnight = DATEADD(dd, DATEDIFF(dd, 0, @TS), 0) -- strip hour minute
	Print '@midnight = '  + convert(varchar,@midnight)
	
	
	set @SLATarget = @midnight
		
	--set @dow = DATEPART(dw,@midnight)
	exec sp_DayOfWeek @midnight,@dow output
	
	Print 'DOW = '  + convert(varchar,@dow)
	
	select @maintenanceStart = m.MaintenanceStartMinuteOfDay
			,@mdow = m.DayOfWeek
			,@maintenanceEnd = m.MaintenanceEndMinuteOfDay
		from SLA_MaintenanceSchedule m 
		where m.CustomerId = @CustomerId
		and @midnight between m.ScheduleStartDate and m.ScheduleEndDate
		and m.DayOfWeek = @dow
		
	select @regulatedStart = r.RegulatedStartMinuteOfDay
		,@rdow = r.DayOfWeek
		,@regulatedEnd = r.RegulatedEndMinuteOfDay
		from SLA_RegulatedSchedule r
		where r.CustomerId = @CustomerId
		and r.AreaId  = @AreaId
		and r.MeterId = @MeterId		
		and @midnight between r.ScheduleStartDate and r.ScheduleEndDate
		and r.DayOfWeek = @dow
	
	if (@mdow is null) begin
		set @mdow = @dow
	end 
	
	if (@rdow is null) begin
		set @rdow = @dow	
	end 
	
		
	Print 'MaintenanceStart = '  + convert(varchar,(CASE WHEN @maintenanceStart is null THEN -1 ELSE @maintenanceStart END))
	Print 'RegulatedStart = '  + convert(varchar,(CASE WHEN @regulatedStart is null THEN -1 ELSE @regulatedStart END))
	Print 'MaintenanceEnd = '  + convert(varchar,(CASE WHEN @maintenanceEnd is null THEN -1 ELSE @maintenanceEnd END))
	Print 'RegulatedEnd = '  + convert(varchar,(CASE WHEN @regulatedend is null THEN -1 ELSE @regulatedend END))
	
	Print ' @rdow = ' + convert(varchar,@rdow) + ', @mdow =  ' + convert(varchar,@mdow) 
		
	
	if (@Start = 1) begin
		print 'getting SLA start'
		--assign earlier one for start
		if (@maintenanceStart is null) begin
			Print 'No maintenance scheduled in next day'
			if (@regulatedStart is null) begin
				Print 'and No regulation scheduledin next day'			
				set @SLATarget = NULL
			end else begin
				Print 'but regulation scheduled  and adding ' + convert(varchar,@regulatedStart) 
				set @SLATarget = Dateadd(MINUTE,@regulatedStart,@midnight)
				set @dowOffset = @rdow  - @dow			
			end 			
		end else begin
			--print 'getting SLA end'
			Print 'Maintenance starts at '  + convert(varchar,@maintenanceStart) 
			if (@regulatedStart is null) begin
				Print 'No regulation scheduled in next day and add maintenance start '
				set @SLATarget = Dateadd(MINUTE,@maintenanceStart,@midnight)
				set @dowOffset = @mdow  - @dow
				set @SLATarget = DATEADD(dd,@dowOffset,@SLATarget)
			end else begin
				-- both are not null at this stage. take which ever is later.
				if  (@mdow <= @rdow) begin
					Print 'adding earlier @maintenanceStart '+ convert(varchar,@maintenanceStart)  
					set @SLATarget = Dateadd(MINUTE,@maintenanceStart,@midnight)
					set @dowOffset = @mdow  - @dow
				end else begin
					-- get later one
					if  (@maintenanceStart < @regulatedStart)  begin
						Print 'adding @maintenanceStart '+ convert(varchar,@maintenanceStart)  
						set @SLATarget = Dateadd(MINUTE,@maintenanceStart,@midnight)
						set @dowOffset = @mdow  - @dow
					end else begin
						Print 'adding @regulatedStart ' + convert(varchar,@regulatedStart) 
						set @SLATarget = Dateadd(MINUTE,@regulatedStart,@midnight)
						set @dowOffset = @rdow  - @dow
				end
				end 			 
			end 
		end 		
	end else 
	begin 		
		print 'getting SLA END'
		if (@maintenanceEnd is null) begin
			Print 'No maintenance scheduled in next day'
			if (@regulatedEnd is null) begin
				Print 'and No regulation scheduledin next day'			
				set @SLATarget = NULL
			end if (@rdow = @dow) begin
				Print 'but regulation scheduled  and adding ' + convert(varchar,@regulatedEnd) 
				set @SLATarget = Dateadd(MINUTE,@regulatedEnd,@midnight)
				set @dowOffset = @rdow  - @dow			
			end 
		end else begin
			Print 'Maintenance ends at '  + convert(varchar,@maintenanceEnd) 			
			if (@regulatedEnd is null) begin
				Print 'No regulated end time'
				if  (@mdow = @dow)  begin
					Print 'adding @maintenanceEnd '+ convert(varchar,@maintenanceEnd)  
					set @SLATarget = Dateadd(MINUTE,@maintenanceEnd,@midnight)
					set @dowOffset = @mdow  - @dow
				end else begin
					Print 'Not the same day for maintenance'					
				end 
			end else begin
				-- both are not null at this stage. take which ever is earlier.			
				if  (@maintenanceEnd < @regulatedEnd) and (@mdow = @dow)  begin
					Print 'adding @maintenanceEnd '+ convert(varchar,@maintenanceEnd)  
					set @SLATarget = Dateadd(MINUTE,@maintenanceEnd,@midnight)
					set @dowOffset = @mdow  - @dow
				end else if (@rdow = @dow) begin
					Print 'adding @regulatedEnd ' + convert(varchar,@regulatedEnd) 
					set @SLATarget = Dateadd(MINUTE,@regulatedEnd,@midnight)
					set @dowOffset = @rdow  - @dow
				end		
			end	
		end		
	end 
	

	
	if (@SLATarget is not null) and (@dowOffset > 0)begin
		Print 'DOW Offset ' + convert(varchar,@dowOffset) 
		set @SLATarget = DATEADD(dd,@dowOffset,@SLATarget)
	end 
		
	Print '------------------------------------ sp_SLA_getSLAStartOrEnd : done -----------------' 
	
END
GO
/****** Object:  Table [dbo].[SLA_Holiday]    Script Date: 04/01/2014 22:05:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SLA_Holiday](
	[SLA_HolidayID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[HolidayDate] [datetime] NOT NULL,
	[HolidyName] [varchar](250) NOT NULL,
 CONSTRAINT [PK_SLA_Holiday] PRIMARY KEY CLUSTERED 
(
	[SLA_HolidayID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_isApplicable]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_isApplicable] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int,
	@TimeOfOccurance datetime,
	@SLAMinute int,
	@SLATarget datetime output	
AS
Declare
	@dow int
	,@justdate datetime -- date without time
	,@hname varchar(50)		
	,@continue bit
	,@resultant datetime
	,@slaend datetime
	,@slastart datetime
	,@i int
BEGIN
	Print '---------------------------------- sp_SLA_isApplicable : start --------------------------------' 
	if (@TimeOfOccurance is null) begin
		Print '@TimeOfOccurance is null. No more proceed.'
		return
	end 
	
	Print '@TimeOfOccurance = '  + convert(varchar,@TimeOfOccurance)
	
	set @continue = 1 
	
	--set @dow =  DATEPART(dw,@TimeOfOccurance)
	exec sp_DayOfWeek @TimeOfOccurance,@dow output
	
	Print 'DOW = '  + convert(varchar,@dow)
	
	select @justdate = DATEADD(dd, DATEDIFF(dd, 0, @TimeOfOccurance), 0)
	
	-- If no maintenance for today
	if (@continue = 1) begin 
		if not exists (select * from SLA_MaintenanceSchedule m
					where m.CustomerId = @CustomerId and m.DayOfWeek = @dow
					and @TimeOfOccurance between m.ScheduleStartDate and m.ScheduleEndDate				
					and @TimeOfOccurance between DATEADD(MINUTE,m.MaintenanceStartMinuteOfDay,@justdate)
								and DATEADD(MINUTE,m.MaintenanceEndMinuteOfDay,@justdate)
				)begin 
			Print 'No maintenance'
			set @continue = 0
		end else begin
			Print 'Maintenance Scheduled'
			set @continue = 1
		end
	end
	
	
	
	if (@continue = 1) begin 
		--if today is holiday
		select @hname = h.HolidyName from SLA_Holiday h
				where h.CustomerId = @CustomerId 
					and @TimeOfOccurance between DATEADD(dd, DATEDIFF(dd, 0, h.HolidayDate), 0) --strip date
									and DATEADD(hour,24,DATEADD(dd, DATEDIFF(dd, 0, h.HolidayDate), 0)) --add 24 hours
		if (@hname is not null)begin
			Print 'Holiday today - ' + @hname
			set @continue = 0			
		end else begin
			Print 'Not Holiday'
			set @continue = 1
		end
	end
	
	if (@continue = 1) begin 
		--if today is regulated
		if exists(select * from SLA_RegulatedSchedule r
					where r.CustomerId = @CustomerId 
						and r.DayOfWeek = @dow
						and @TimeOfOccurance between r.ScheduleStartDate and r.scheduleEndDate
						and @TimeOfOccurance between DATEADD(MINUTE,r.RegulatedStartMinuteOfDay,@justdate)
								and DATEADD(MINUTE,r.RegulatedEndMinuteOfDay,@justdate)
						
					
		)begin
			Print 'Regulated'	
			set @continue = 1	
		end else begin
			Print 'Unregulated'
			set @continue = 0
		end 
	end 
	
	-- if resultant 
	if (@continue = 1)		
		and (@SLAMinute is not null)
		begin 
		set @resultant = dateadd(MINUTE,@SLAMinute,@TimeOfOccurance)	
		set @TimeOfOccurance = @resultant
		Print 'validating resultant sla target ' + convert(varchar,@resultant) 
		exec sp_SLA_getSLAStartOrEnd @customerid,@areaid, @meterid,@EventCode,@eventsource,@resultant,0,@slaend OUTPUT
		
		Print 'Resultant ' + convert(varchar,@resultant) + ' , SLA End ' + convert(varchar,@slaend) 			
		if (@resultant > @slaend) begin
			Print 'Resultant after SLAEnd ' 
			set @continue = 0
		end		
	end 
	
	if (@continue = 0) begin
		Print 'Next day conditions met'
		
		set @TimeOfOccurance = DATEADD(HOUR,24,@TimeOfOccurance) -- add 1 day /next day -
		Print 'Next Day = '  + convert(varchar,@TimeOfOccurance)
	
		exec sp_SLA_getSLAStartOrEnd @customerid,@areaid, @meterid,@EventCode,@eventsource,@TimeOfOccurance,1,@SLATarget OUTPUT
		Print '@SLATarget for Next Day = '  + convert(varchar,@SLATarget)
		
		if (@SLATarget is null) begin	
			set @i = 0
			while (@i < 7)begin
				Print 'No next day SLA. Get one more day with ' + convert(varchar,@Timeofoccurance)			
				set @TimeOfOccurance = DATEADD(HOUR,24,@TimeOfOccurance) -- add 1 day /next day -
				set @TimeOfOccurance = DATEADD(dd, DATEDIFF(dd, 0, @TimeOfOccurance), 0) -- strip hour minute
				
				exec sp_SLA_getSLAStartOrEnd @customerid,@areaid, @meterid,@EventCode,@eventsource,@TimeOfOccurance,1,@SLATarget OUTPUT
				Print '@SLATarget returned ' + convert(varchar,@SLATarget)			
				
				if (@SLATarget is null) begin
					set @i = @i + 1
				end else begin
					set @TimeOfOccurance = @SLATarget		
					set @i = 7 -- break the while loop
				end 
			end			
		end else begin 
			set @TimeOfOccurance = @SLATarget		
			Print 'recursive call @SLATarget= ' + convert(varchar,@SLATarget)
			exec sp_SLA_isApplicable @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@TimeOfOccurance,@SLAMinute,@SLATarget OUTPUT					
			return
		end 
	end else begin
		Print 'Next day conditions not met and validating occurance_time earlier than sla start for ' + convert(varchar,@TimeOfOccurance)
		exec sp_SLA_getSLAStartOrEnd @customerid,@areaid, @meterid,@EventCode,@eventsource,@TimeOfOccurance,1,@slastart OUTPUT
		Print 'SLA Start ' + convert(varchar,@slastart) 			
		if (@TimeOfOccurance < @slastart) begin
			Print 'Time of occurrance earlier than sla start'
			set @TimeOfOccurance = @slastart
		end
	end
	
	Print ' ' 
	Print ' ---- finalising ----'
	
	if (@SLATarget is null)begin
		Print '@SLATarget is null'
		set @SLATarget = @TimeOfOccurance
	end 
	
	if (@SLAMinute is not null) begin
		Print 'Addding @SLAMinute ' + convert(varchar,@SLAMinute) + ' to ' + convert(varchar,@SLATarget)
		set @SLATarget = dateadd(MINUTE,@SLAMinute,@SLATarget)
	end
	
	Print 'returning ' + convert(varchar,@SLATarget)
	
	Print '---------------------------------- sp_SLA_isApplicable : done --------------------------------' 
	Print ' ' 
	
	 
END
GO
/****** Object:  Table [dbo].[AssetType]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetType](
	[AssetTypeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MeterGroupId] [int] NOT NULL,
	[CustomerId] [int] NULL,
	[IsDisplay] [bit] NULL,
	[SLAMinutes] [int] NULL,
	[MeterGroupDesc] [varchar](25) NULL,
	[PreventativeMaintenanceScheduleDays] [int] NULL,
 CONSTRAINT [PK_AssetType] PRIMARY KEY CLUSTERED 
(
	[AssetTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MechanismMasterCustomer]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MechanismMasterCustomer](
	[MechanismMasterCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MechanismId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[MechanismDesc] [varchar](255) NOT NULL,
	[IsDisplay] [bit] NOT NULL,
	[SLAMinutes] [int] NULL,
	[PreventativeMaintenanceScheduleDays] [int] NULL,
 CONSTRAINT [PK_MechanismMasterCustomer] PRIMARY KEY CLUSTERED 
(
	[MechanismMasterCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MechanismMaster]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MechanismMaster](
	[MechanismId] [int] NOT NULL,
	[MechanismDesc] [varchar](255) NOT NULL,
	[MeterGroupId] [int] NULL,
 CONSTRAINT [PK_MechanismMaster] PRIMARY KEY CLUSTERED 
(
	[MechanismId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[FromEmailAddress] [varchar](128) NULL,
	[BankHostAddr] [varchar](128) NULL,
	[BankHostPort] [int] NULL,
	[BankMaxThreads] [int] NULL,
	[BankServiceProvider] [int] NULL,
	[BankStoreID] [varchar](255) NULL,
	[ResubmitProfileID] [int] NULL,
	[UnReconcileCleanupLag] [int] NULL,
	[DoesCreditCashKeys] [bit] NULL,
	[doesBlacklistViaFD] [int] NULL,
	[BlackListCC] [bit] NULL,
	[TxTiming] [int] NULL,
	[CityCode] [varchar](25) NULL,
	[Status] [int] NULL,
	[CreateDateTime] [datetime] NULL,
	[City] [varchar](255) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[SLAMinutes] [int] NULL,
	[IsEMV] [bit] NOT NULL,
	[IsPayByPhone] [bit] NOT NULL,
	[GracePeriodMinute] [int] NOT NULL,
	[FreeParkingMinute] [int] NOT NULL,
	[DateTimeFormat] [varchar](255) NULL,
	[TimeZoneID] [int] NULL,
	[ZeroOutMeter] [bit] NULL,
	[Streetline] [bit] NULL,
	[DiscountScheme] [bit] NULL,
	[CountryCode] [varchar](3) NULL,
	[SLAMethod] [varchar](5) NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MeterGroup]    Script Date: 04/01/2014 22:05:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterGroup](
	[MeterGroupId] [int] NOT NULL,
	[MeterGroupDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_MeterGroup] PRIMARY KEY CLUSTERED 
(
	[MeterGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[SLA_Minutes]    Script Date: 04/01/2014 22:06:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create View [dbo].[SLA_Minutes] as
	select distinct m.CustomerID,m.AreaID,m.MeterId	
		,c.SLAMinutes CustomerSlaMinutes
		,m.SchedServTime MeterSlaMinutes
		,mmc.MechanismId,mmc.MechanismDesc,mmc.SLAMinutes MechanismSlaMinutes, 
		mgc.SLAMinutes MeterGroupSlaMinutes, mgc.MeterGroupDesc,mgc.MeterGroupId		
		-- to get one SLA Minutes for all table
		,(case when mmc.SLAMinutes is null 
			then (case when mgc.SLAMinutes is null 
				then (case when m.SchedServTime is null 
					then  c.SLAMinutes 
					else m.SchedServTime end)	
				else mgc.SLAMinutes end
			)else mmc.SLAMinutes end) SLAMinutes	
		from Meters m
		join Customers c
			on m.CustomerID = c.CustomerID						
		left join (select mg.MeterGroupId,ast.MeterGroupDesc,ast.CustomerId,ast.SLAMinutes
					from MeterGroup mg 
					join AssetType ast
					on mg.MeterGroupId = ast.MeterGroupId) mgc
			on mgc.CustomerId = m.CustomerID
			and mgc.MeterGroupId = m.MeterGroup		
		left join (select mc.MechanismId,mc.MechanismDesc,mc.CustomerId,mc.SLAMinutes
					from MechanismMaster mm
					join MechanismMasterCustomer mc
					on mm.MechanismId = mc.MechanismId) mmc
			on mmc.CustomerId = m.CustomerID
			and mmc.MechanismId = m.MeterType
GO
/****** Object:  Table [dbo].[EventCodes]    Script Date: 04/01/2014 22:06:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventCodes](
	[CustomerID] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[EventCode] [int] NOT NULL,
	[AlarmTier] [int] NOT NULL,
	[EventDescAbbrev] [varchar](16) NULL,
	[EventDescVerbose] [varchar](50) NULL,
	[SLAMinutes] [int] NULL,
	[IsAlarm] [bit] NULL,
	[EventType] [int] NULL,
	[ApplySLA] [bit] NULL,
	[EventCategory] [int] NULL,
 CONSTRAINT [PK_EventDetails] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[EventSource] ASC,
	[EventCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getSLAMinute]    Script Date: 04/01/2014 22:06:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getSLAMinute] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int
	,@SLAMinute int OUTPUT
AS
Declare 
	@MeterGroup int	
	,@MechanismId int
BEGIN

	-- check EventCode first
	print 'getting SLAMinute from EventCodes'
	SELECT @SLAMinute = SLAMinutes
		FROM   EventCodes
		WHERE  customerid = @CustomerID
		AND eventCode = @EventCode
		AND EventSource = @EventSource

	if (@SLAMinute is null) begin
		
		-- then check AssetType
		print 'getting SLAMinute from AssetType'
		SELECT @SLAMinute = SLAMinutes
			FROM SLA_Minutes
			WHERE customerid = @CustomerID
			and MeterId = @MeterId
			and AreaID = @AreaId
	end	

	if (@SLAMinute is null) begin
		-- then check Asset/Meter
		print 'getting SLAMinute from Asset'
		SELECT @SLAMinute = schedservtime
			FROM meters
			WHERE customerid = @CustomerID
			AND areaid = @AreaID
			AND meterid = @MeterID
	end
		
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getSLATargetTime]    Script Date: 04/01/2014 22:06:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getSLATargetTime] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int,
	@TimeOfOccurance datetime
	,@SLATarget DateTime output
AS
Declare 
	@MeterGroup int
	,@SLAMinute int
	
BEGIN
	Print ' '
	Print '--- =================================================   sp_SLA_getSLATargetTime   ============================================ --'
	Print convert(vARCHAR,@CustomerId) + '/'
			+ convert(vARCHAR,@AreaId) + '/' 
			+ convert(vARCHAR,@MeterId) + '/' 
			+ convert(vARCHAR,@EventCode) + '/' 
			+ convert(vARCHAR,@EventSource) + ' @'
			+ convert(varchar,@TimeOfOccurance) 	
									
	EXEC sp_SLA_getSLAMinute @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@SLAMinute OUTPUT
		
	if (@SLAMinute is not null) and (@SLAMinute > 0 ) 
	begin
		Print '@SLAMinute = ' + convert(varchar,@SLAMinute)
		exec sp_SLA_isApplicable @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@TimeOfOccurance,@SLAMinute,@SLATarget OUTPUT
		if (@SLATarget is not null)begin
			Print 'RESULT- @SLATarget = ' + convert(varchar,@SLATarget)
		end else begin
			Print 'RESULT- @SLATarget is null'
		end		
	end else begin
		Print 'SLAMinute is null or not defined'								
	end
END

--
GO
/****** Object:  Table [dbo].[AlamrUIDGen]    Script Date: 04/01/2014 22:06:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlamrUIDGen](
	[AlamrUID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GenDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AlamrUIDGen] PRIMARY KEY CLUSTERED 
(
	[AlamrUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetAlamrUID]    Script Date: 04/01/2014 22:06:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetAlamrUID]
	-- Add the parameters for the stored procedure here
	  @AlamrUID bigint OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
	Insert INTO dbo.AlamrUIDGen	([GenDate]) VALUES	(GETDATE()) ;
	
	
	set @AlamrUID = SCOPE_IDENTITY();
	
	Return @AlamrUID
END
GO
/****** Object:  StoredProcedure [dbo].[sp_ActiveAlarm_Helper]    Script Date: 04/01/2014 22:06:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ActiveAlarm_Helper] 
	@CustomerID          INT,
	@AreaID              INT,
	@MeterID             INT,
	@EventCode           INT,
	@EventSource         INT,
	@TimeOfOccurrance    DATETIME
AS
	DECLARE @slaTarget			 DATETIME,
			@meterGrp            INT,
			@EventUID            BIGINT,
			@AlamrUID            BIGINT,
			@TimeType1           INT,
			@TimeType2           INT,
			@TimeType3           INT,
			@TimeType4           INT,
			@SensorID            INT,
			@GatewayID           INT,
			@AlarmTier           INT,
			@GlobalMeterId		BIGINT,
			@operationalstatusid INT,
			@ERROR_MESSAGE       NVARCHAR(4000),
			@ERR                 INT;
BEGIN
	Print 'sp_ActiveAlarm_Helper'
	BEGIN try	  

	  EXEC Sp_geteventuid
		@EventUID output;

	  EXEC Sp_getalamruid
		@AlamrUID output;

	  EXEC Sp_inserteventlog2
		@EventUID,
		@CustomerID,
		@AreaID,
		@MeterID,
		2002,
		@TimeOfOccurrance,
		@TimeType1 output,
		@TimeType2 output,
		@TimeType3 output,
		@TimeType4 output;
		
	select @GlobalMeterId= dbo.GenGlobalID(@CustomerID,@AreaID,@MeterID,default)
	
	exec sp_SLA_getSLATargetTime @customerid,@areaid, @meterid,@EventCode,@eventsource,@TimeOfOccurrance,@SLATarget output
		if (@SLATarget is null) begin
			Print '@SLATarget is null'
		end else begin
			Print '@SLATarget=  '+ convert(varchar,@slatarget)
		end
	
	  UPDATE activealarms
	  SET    sladue = @SLATarget,
			 eventuid = @EventUID,
			 alarmuid = @AlamrUID,
			 timetype1 = @TimeType1,
			 timetype2 = @TimeType2,
			 timetype3 = @TimeType3,
			 timetype4 = @TimeType4,
			 GlobalMeterID = @GlobalMeterId
	  WHERE  customerid = @CustomerID
			 AND areaid = @AreaID
			 AND meterid = @MeterID
			 AND eventcode = @EventCode
			 AND eventsource = @EventSource
			 AND timeofoccurrance = @TimeOfOccurrance;

	  SELECT @AlarmTier = alarmtier
	  FROM   eventcodes
	  WHERE  customerid = @CustomerID
			 AND eventcode = @EventCode
			 AND eventsource = @EventSource

	  IF( @AlarmTier = 0)
		BEGIN
			SET @operationalstatusid =2
		END
	  ELSE
		BEGIN
			SET @operationalstatusid =3
		END
		
	EXEC sp_UpdateOperationalStatus
		@CustomerID,
		@AreaID,
		@MeterID,
		@operationalstatusid,
		@TimeOfOccurrance			

  END try

  BEGIN catch
	  BEGIN		
		SET @ERROR_MESSAGE = Error_message()
		Print 'Error'		
		exec sp_Trigger_Log @ERROR_MESSAGE,'trg_activealaram_update'		
	  END
  END catch
END
GO
/****** Object:  Table [dbo].[ActiveAlarms]    Script Date: 04/01/2014 22:06:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ActiveAlarms](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[TimeOfOccurrance] [datetime] NOT NULL,
	[TimeOfNotification] [datetime] NOT NULL,
	[EventUID] [bigint] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[SLADue] [datetime] NULL,
	[WorkOrderId] [int] NULL,
	[AlarmUID] [int] NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_ActiveAlarms] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventCode] ASC,
	[EventSource] ASC,
	[TimeOfOccurrance] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [ActiveAlarms_IDXGlobalMeterID] ON [dbo].[ActiveAlarms] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_ActiveAlarms_Time] ON [dbo].[ActiveAlarms] 
(
	[TimeOfOccurrance] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_HistoricalAlarm_Helper]    Script Date: 04/01/2014 22:06:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_HistoricalAlarm_Helper] 
	@CustomerID          INT,
	@AreaID              INT,
	@MeterID             INT,
	@EventCode           INT,
	@EventSource         INT,	
	@EventState			INT,
	@EventUID			BIGINT,
	@AlarmUID			BIGINT,
	@TimeOfOccurrance    DATETIME,
	@TimeOfClearance DATETIME
AS
DECLARE 
	@TimeType1        INT,
	@TimeType2        INT,
	@TimeType3        INT,
	@TimeType4        INT,
	@SensorID         INT,
	@GatewayID        INT, 
	@GlobalMeterID	  BIGINT,           
	@ERROR_MESSAGE    NVARCHAR(4000),	
	--@EventUID			BIGINT,
	@WorkOrderId	int,
	@SLATarget	DateTime,
	@OperationalStatus int,
	@ERR              INT;
BEGIN
BEGIN try  
	  Print 'Executing sp_HistoricalAlarm_Helper'
	  
	  /*
	  EXEC Sp_geteventuid
		@EventUID output;
	 */
	 
 		select @AlarmUID = max(alarmuid), @WorkOrderId = max(workorderId),@SLATarget = MAX(SLADue)
 			from ActiveAlarms a
			where a.CustomerID = @CustomerID 
				and a.AreaID = @AreaID 
				and a.MeterId = @MeterID
				and a.EventCode = @EventCode
				and a.TimeOfOccurrance = @TimeOfOccurrance
	
	  if (@AlarmUID is null) begin
	  	  set @AlarmUID = @EventUID
	  end 
	 
	  EXEC Sp_inserteventlog2
		@EventUID,
		@CustomerID,
		@AreaID,
		@MeterID,
		2002,
		@TimeOfOccurrance,
		@TimeType1 output,
		@TimeType2 output,
		@TimeType3 output,
		@TimeType4 output;
		
		select @GlobalMeterId= dbo.GenGlobalID(@CustomerID,@AreaID,@MeterID,default)
		
		
		Print 'Updating historicalalarms'
		
		
		
		
	 UPDATE historicalalarms
	  SET    TimeType1= @TimeType1,
			 TimeType2= @TimeType2,
			 TimeType3= @TimeType3,
			 TimeType4= @TimeType4,
			 AlarmUID = @AlarmUID,
			 WorkOrderId = @WorkOrderId,
			 SLADue = @SLATarget,
			 TargetServiceDesignation = (CASE WHEN (@TimeOfClearance > @SLATarget) THEN 6 ELSE 5 END), --6= Closed-Non-Compliant,5=Closed-Compliant
			 GlobalMeterId = @GlobalMeterId			 
	  WHERE  customerid = @CustomerID
			 AND areaid = @AreaID
			 AND meterid = @MeterID
			 AND eventcode = @EventCode
			 AND eventsource = @EventSource
			 AND EventState=@EventState
			 AND timeofoccurrance = @TimeOfOccurrance;            
		
		--@OperationalStatus is borrowed here. Actual value will be set later.
		select @OperationalStatus = COUNT(*) 
				from ActiveAlarms
				WHERE  customerid = @CustomerID
					 AND areaid = @AreaID
					 AND meterid = @MeterID	 
		--3 = Operational With alarm. 1 = Operational
		-- 1 alarm is for clearing alarm itself. if more than one, there is at least one other alarm.
		if (@OperationalStatus > 1 ) begin
			set @OperationalStatus = 3
			Print ' @OperationalStatus > 0'
		end else begin
			set @OperationalStatus = 1
			Print 'else @OperationalStatus '
		end
	
	EXEC sp_UpdateOperationalStatus
		@CustomerID,
		@AreaID,
		@MeterID,
		@OperationalStatus,
		@TimeOfClearance		

  END try

  BEGIN catch
	  BEGIN
		  SET @ERROR_MESSAGE = Error_message()
	  END
  END catch
END
GO
/****** Object:  Table [dbo].[HistoricalAlarms]    Script Date: 04/01/2014 22:06:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HistoricalAlarms](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[TimeOfOccurrance] [datetime] NOT NULL,
	[EventState] [int] NOT NULL,
	[TimeOfNotification] [datetime] NOT NULL,
	[TimeOfClearance] [datetime] NULL,
	[ClearingEventUID] [int] NULL,
	[EventUID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[SLADue] [datetime] NULL,
	[WorkOrderId] [int] NULL,
	[AlarmUID] [int] NULL,
	[TargetServiceDesignation] [int] NULL,
	[ClearedByUserId] [int] NULL,
	[ClosureNote] [varchar](200) NULL,
	[Notes] [varchar](255) NULL,
	[DownTimeMinute] [int] NOT NULL,
	[ProcessedTime] [datetime] NULL,
 CONSTRAINT [PK_HistoricalAlarms] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventCode] ASC,
	[EventSource] ASC,
	[TimeOfOccurrance] ASC,
	[EventState] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [HistoricalAlarms_IDX_CTAMEETTCEE] ON [dbo].[HistoricalAlarms] 
(
	[CustomerID] ASC,
	[TimeOfOccurrance] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventCode] ASC,
	[EventSource] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [HistoricalAlarms_IDX_EETC] ON [dbo].[HistoricalAlarms] 
(
	[EventCode] ASC,
	[EventUID] ASC,
	[TimeOfClearance] ASC,
	[ClearingEventUID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [HistoricalAlarms_IDXGlobalMeterID] ON [dbo].[HistoricalAlarms] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_HistoricalAlarms_Time] ON [dbo].[HistoricalAlarms] 
(
	[TimeOfOccurrance] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[sla_AlarmWorkOrder]    Script Date: 04/01/2014 22:06:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
create view sla_AlarmWorkOrder as 
	select top 1000 ha.CustomerID,ha.AreaID,ha.MeterId
	,mm.EventDateTime WorkOrderCompletionDate
	,ha.TimeOfClearance 
	,ha.TimeOfOccurrance 
	,DATEADD(MINUTE,rs.RegulatedStartMinuteOfDay,DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfOccurrance), 0)) RegulatedStartTime
	,DATEADD(MINUTE,rs.RegulatedEndMinuteOfDay,DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfOccurrance), 0)) RegulatedEndTime
	,DATEADD(MINUTE,ms.MaintenanceStartMinuteOfDay,DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfOccurrance), 0)) MaintenanceStartTime
	,DATEADD(MINUTE,ms.MaintenanceEndMinuteOfDay,DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfOccurrance), 0)) MaintenanceEndTime
	,DATEPART(dw,ha.TimeOfOccurrance) as dow
	-------------------------------------------
	--Historical Alarm
	-------------------------------------------
	from HistoricalAlarms ha 
	-------------------------------------------
	-- Work Order
	-------------------------------------------
	left join WorkOrder wo
	on ha.CustomerID = wo.CustomerID
	and ha.WorkOrderId = wo.WorkOrderId
	-------------------------------------------
	-- Meter Maintenance Event
	-------------------------------------------
	left join SFMeterMaintenanceEvent mm
	on wo.WorkOrderId = mm.WorkOrderID
	and mm.CustomerId = wo.CustomerID
	-------------------------------------------
	-- Regulated Schedule
	-------------------------------------------
	left join SLA_RegulatedSchedule rs
	on ha.TimeOfOccurrance between rs.ScheduleStartDate and rs.ScheduleEndDate
	and  DATEPART(dw,ha.TimeOfOccurrance) = rs.DayOfWeek
	and ha.MeterId = rs.MeterId
	and ha.AreaID = rs.AreaId
	and ha.CustomerID = rs.CustomerId
	-------------------------------------------
	-- Meintenance Schecdule
	-------------------------------------------
	left join SLA_MaintenanceSchedule ms
	on ha.TimeOfOccurrance between ms.ScheduleStartDate and ms.ScheduleEndDate
	and DATEPART(dw,ha.TimeOfOccurrance) = ms.DayOfWeek
	and ha.Customerid = ms.CustomerId
*/
create view [dbo].[sla_AlarmWorkOrder] as 
			select ha.customerid,ha.areaid,ha.meterid
			,ha.EventCode,ha.eventsource,ha.TimeOfOccurrance,ha.TimeOfClearance 
			,DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfOccurrance), 0) OccurranceDay
			,DATEADD(dd, 1, DATEADD(dd, DATEDIFF(dd, 0, ha.TimeOfClearance), 0)) ClearanceDay
			,case when mc.MaintenanceCode is null then -1 else mc.MaintenanceCode end MaintenanceCode
			,wo.WorkOrderId
			-------------------------------------------
			-- Alarm
			-------------------------------------------
			from HistoricalAlarms ha
			-------------------------------------------
			-- Work Order
			-------------------------------------------
			left join WorkOrder wo
			on ha.CustomerID = wo.CustomerID
			and ha.WorkOrderId = wo.WorkOrderId
			-------------------------------------------
			-- Meter Maintenance Event
			-------------------------------------------
			left join SFMeterMaintenanceEvent mm
			on wo.WorkOrderId = mm.WorkOrderID
			and mm.CustomerId = wo.CustomerID	
			-------------------------------------------
			-- Maintenance Codes
			-------------------------------------------
			left join MaintenanceCodes mc
			on mm.MaintenanceCode = mc.MaintenanceCode					
			-------------------------------------------
			-- HA
			------------------------------------------
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getDownTimeOfADay]    Script Date: 04/01/2014 22:06:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getDownTimeOfADay] 
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@Ts datetime
	,@SlaStart datetime
	,@slaEnd datetime
	,@DownTime int OUTPUT
	,@NffDownTime int OUTPUT	
AS 
Declare 
	@EventCode int,
	@eventsource int,
	@TimeOfOccurance datetime,
	@TimeOfClearance datetime,
	@TsDay datetime,
	@StartDay datetime,
	@EndDay datetime,
	@EachDownTime int,
	@StartCalculation datetime,
	@EndCalculation datetime,
	@TempToc datetime,
	@MaintenanceCode int,
	@cnt int
	

BEGIN
	print ' '
	print '------------ ================= sp_SLA_getDownTimeOfADay:begin     ==================================== -----------'
	
	print ' @TS  = ' + convert(varchar,@ts)
	set @StartDay = DATEADD(dd, DATEDIFF(dd, 0, @TS), 0) -- strip hour minute
	set @TsDay = DATEADD(SECOND, 1, @StartDay)
	exec sp_util_PrintDate '@TsDay',@TsDay
	exec sp_util_PrintDate 'TSDay' ,@TsDay
	set @EndDay = DATEADD(dd, 1, @StartDay)
	set @EndCalculation  = @SlaStart
	set @DownTime = 0	
	set @NffDownTime = 0	
	
	
	exec sp_util_PrintDate '@SlaStart',@SlaStart
	exec sp_util_PrintDate '@SlaEnd',@SlaEnd
	Declare
		curalarmDown cursor for 
			select 
			customerid,areaid,meterid
			,EventCode,eventsource,TimeOfOccurrance,TimeOfClearance 
			,MaintenanceCode
			from sla_AlarmWorkOrder					
			-------------------------------------------
			-- HA
			------------------------------------------
			where @TsDay between OccurranceDay and 
				(case when ClearanceDay IS null then @slaEnd else ClearanceDay end)
			--where TimeOfOccurrance between @StartDay and @EndDay
			and CustomerID  = @customerid
			and AreaID = @AreaId
			and MeterId = @MeterId
			order by TimeOfOccurrance 
			
	OPEN curalarmDown 
		fetch next from curalarmDown into @customerid,@areaid,@meterid,@EventCode,@eventsource,@TimeOfOccurance,@TimeOfClearance,@MaintenanceCode
				
		WHILE @@FETCH_STATUS = 0
		BEGIN			
			print ' ' 
			print '-- ==== LOOP : start ==== ---- ' 
			print 'Timeofccurance= ' + convert(varchar,@TimeOfOccurance) 
			exec sp_util_PrintDate '@TimeOfOccurance',@TimeOfOccurance
			exec sp_util_PrintDate '@TimeofClearance',@TimeOfClearance
			exec sp_util_PrintDate '@EndCalculation',@EndCalculation
			
			-- START
			
			set @StartCalculation = @TimeOfOccurance
			
			if (@TimeOfOccurance < @SlaStart) begin
				Print 'Occurred before SLA Start and using SLAStart'
				set @StartCalculation = @SlaStart
			end
			
			if (@StartCalculation < @EndCalculation) begin
				Print 'Overlapped. Start from previous end'
				set @StartCalculation = @EndCalculation -- this is previous value in the loop
			end 
			
			exec sp_util_PrintDate '@StartCalculation',@StartCalculation
			
			-- END
			
			if (@TimeOfOccurance > @EndCalculation) begin
				set @EndCalculation = @TimeOfClearance		
			end 
			
			if (@TimeOfClearance is not null) and (@TimeOfClearance > @slaEnd ) begin
				Print 'Closed after SLA End'
				set @EndCalculation = @SlaEnd
			end 
			
			if (@TimeOfClearance is null)begin
				Print 'Not closed'
				set @EndCalculation = @SlaEnd				
			end 
			
			exec sp_util_PrintDate 'Start',@StartCalculation
			exec sp_util_PrintDate 'End',@EndCalculation
			
			set @EachDownTime = DateDiff(Minute,@StartCalculation,@EndCalculation)			
			
			exec sp_util_PrintNum 'EachDownTime',@EachDownTime			
			exec sp_util_PrintNum 'MaintenanceCode',@MaintenanceCode
			
			if (@MaintenanceCode = 9) begin --NFF Code				
				set @NffDownTime = @NffDownTime  + @EachDownTime
			end else begin
				set @DownTime = @DownTime  + @EachDownTime
			end				
			
			fetch next from curalarmDown into @customerid,@areaid,@meterid,@EventCode,@eventsource,@TimeOfOccurance,@TimeOfClearance,@MaintenanceCode
		END 
	CLOSE curalarmDown
	DEALLOCATE curalarmDown

	exec sp_util_PrintNum 'Total @Downtime',@DownTime			
	exec sp_util_PrintNum 'Total @NffDowntime',@NffDownTime			
	
	print '------------ ================= sp_SLA_getDownTimeOfADay:end     ========================================== -----------'
	print ' '
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_populateMaintenanceDowntime_D]    Script Date: 04/01/2014 22:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_populateMaintenanceDowntime_D] 
	@d datetime
AS
Declare 
	@MeterGroup int	
	,@Customerid int
	,@AreaId int
	,@MeterId int
	,@ts datetime	
	--- REGULATED ---
	,@regulatedStartMinuteOfDay int
	,@regulatedEndMinuteOfDay int
	,@totalRegulatedMinute int	
	,@regulatedDowntime int
	,@regStart datetime
	,@regEnd datetime
	,@regNffDownTime int
	,@regUpTime int
	,@regUpTimePCT int
	-- MAINTENANCE ---
	,@mntStartMinuteOfDay int
	,@mntEndMinuteOfDay int
	,@mntTotalMinute int	
	,@mntDowntime int
	,@mntStart datetime
	,@mntEnd datetime
	,@mntNffDownTime int
	,@mntUpTime int
	,@mntUpTimePCT int
	-- SLA ---
	,@slaStartMinuteOfDay int
	,@slaEndMinuteOfDay int
	,@slaTotalMinute int	
	,@slaDowntime int
	,@slaStart datetime
	,@slaEnd datetime
	,@slaNffDownTime int
	,@slaUpTime int
	,@slaUpTimePCT int	
	
BEGIN
	Print '-- ===================================    sp_SLA_populateMaintenanceDowntime  ==============================='
	declare 
		curmeter cursor for 
		select customerid,areaid,meterid,MeterGroup
		from Meters
		where MeterGroup is not null
				
	OPEN curmeter 
		fetch next from curmeter into @customerid,@areaid,@meterid,@MeterGroup
		WHILE @@FETCH_STATUS = 0
		BEGIN
			Print convert(varchar,@Customerid) + '/' +
					convert(varchar,@Areaid) + '/' +
					convert(varchar,@MeterId) + ' - ' +
					convert(varchar,@MeterGroup)
			
			--set @ts = '2013/09/09' 
			set @ts = DateAdd(dd,-1,@d) -- Yesterday
			
			-------------------------------------------------------------------------------------------------------------------------------
			print '-- REGULATED SCHEDULE --'
			-------------------------------------------------------------------------------------------------------------------------------		
			exec sp_SLA_getROMScheudle @customerid,@areaid,@meterid,@ts,'Regulated',
				@regulatedStartMinuteOfDay output,@regulatedEndMinuteOfDay output
				,@regStart output,@regEnd output
				
			exec sp_SLA_getDownTimeOfADay @customerid,@areaid,@meterid,@ts,@regStart,@regEnd,
				@regulatedDowntime output,@regNffDownTime output
			
			exec sp_SLA_pct @regulatedStartMinuteOfDay,@regulatedEndMinuteOfDay,@regulatedDowntime,
				@totalRegulatedMinute output,@regUpTime output,@regUpTimePCT output
			
			-------------------------------------------------------------------------------------------------------------------------------
			print '-- MAINTENANCE SCHEDULE --'
			-------------------------------------------------------------------------------------------------------------------------------
			exec sp_SLA_getROMScheudle @customerid,@areaid,@meterid,@ts,'Maintenance',
				@mntStartMinuteOfDay output,@mntEndMinuteOfDay output
				,@mntStart output,@mntEnd output
			
			exec sp_SLA_getDownTimeOfADay @customerid,@areaid,@meterid,@ts,@mntStart,@mntEnd,@mntDowntime output,@mntNffDownTime output
			
			exec sp_SLA_pct @mntStartMinuteOfDay,@mntEndMinuteOfDay,@mntDowntime,
				@mntTotalMinute output,@mntUpTime output,@mntUpTimePCT output
				
			-------------------------------------------------------------------------------------------------------------------------------			
			print '-- SLA--'
			-------------------------------------------------------------------------------------------------------------------------------			
			exec sp_util_MinMax 'max',@mntStartMinuteOfDay,@regulatedStartMinuteOfDay,null,null,@slaStartMinuteOfDay output,null
			exec sp_util_MinMax 'min',@mntEndMinuteOfDay,@regulatedEndMinuteOfDay,null,null,@slaEndMinuteOfDay output,null
			exec sp_util_MinMax 'max',null,null,@regStart,@mntStart,null,@slaStart output
			exec sp_util_MinMax 'min',null,null,@regEnd,@mntEnd,null,@slaEnd output
			
			exec sp_SLA_getDownTimeOfADay @customerid,@areaid,@meterid,@ts,@slaStart,@slaEnd,@slaDowntime output,@slaNffDownTime output
			
			exec sp_SLA_pct @slaStartMinuteOfDay,@slaEndMinuteOfDay,@slaDowntime,
				@slaTotalMinute output,@slaUpTime output,@slaUpTimePCT output
			
			-------------------------------------------------------------------------------------------------------------------------------			
			-- INSERT
			-------------------------------------------------------------------------------------------------------------------------------
			Insert into SLA_AssetDownTime
			(CustomerId, AreaId, MeterId,MeterGroup
			,ReportingDate
			,RegulatedStartMinuteOfDay,RegulatedEndMinuteOfDay,TotalRegulatedMinutes,TotalRegulatedDowntimeMinutes,TotalRegulatedNFFMinutes,RegulatedUpTimePCT
			,MaintenanceStartMinuteOfDay,MaintenanceEndMinuteOfDay,TotalMaintenanceMinutes,TotalMaintenanceDowntimeMinutes,TotalMaintenanceNFFMinutes,MaintenanceUpTimePCT
			,SLAStartTimeMinuteOfDay,SLAEndTimeMinuteOfDay,TotalSLAMinutes,TotalSLADowntimeMinutes,TotalSLANFFMinutes,SLAUpTimePCT
			)
			values
			(@Customerid,@AreaId,@MeterId,@MeterGroup
			,@ts
			,@regulatedStartMinuteOfDay,@regulatedEndMinuteOfDay,@totalRegulatedMinute,@regulatedDowntime,@regNffDownTime,@regUpTimePCT
			,@mntStartMinuteOfDay,@mntEndMinuteOfDay,@mntTotalMinute,@mntDowntime,@mntNffDownTime,@mntUpTimePCT
			,@slaStartMinuteOfDay,@slaEndMinuteOfDay,@slaTotalMinute,@slaDowntime,@slaNffDownTime,@SLAUpTimePCT
			)
			
			
			fetch next from curmeter into @customerid,@areaid,@meterid,@MeterGroup
		END 
	CLOSE curmeter
	DEALLOCATE curmeter	
end
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_populateMaintenanceDowntime]    Script Date: 04/01/2014 22:06:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_populateMaintenanceDowntime] 
As
Declare @d datetime
Begin
	set @d = getdate()
	exec sp_SLA_populateMaintenanceDowntime_D @d
end
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_SearchHistory]    Script Date: 04/01/2014 22:06:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
* Delphi : RetrieveClearingEventForMeter 
*/
CREATE PROC [dbo].[sp_Alarm_SearchHistory]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int		
	,@EventSource int
	,@TimeOfOccurrance DateTime	
	,@EventUID int output
	,@TimeOfClearance DateTime output
AS
BEGIN
	BEGIN TRY
		Print 'sp_Alarm_SearchHistory : toc = ' + convert(varchar,@TimeOfOccurrance)
		select top 1 @EventUID=EventUID,@TimeOfClearance = TimeOfClearance	 From HistoricalAlarms
			where CustomerID = @CustomerId and AreaID = @AreaId and MeterId = @MeterId
			and 
				(
					(EventCode = @EventCode and EventState = 0) -- Cleared
					or 
					(EventCode = 0) -- General Alarm Cleared
				)
			and EventSource = @EventSource
			and TimeOfOccurrance >= @TimeOfOccurrance			
			order by TimeOfOccurrance 
			
		if (@EventUID is null) begin
			set @EventUID = 0
		end
			
	END TRY
	BEGIN CATCH
		set @EventUID = 0
		print 'sp_Alarm_SearchHistory : ERROR :' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_InsertActive]    Script Date: 04/01/2014 22:06:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_InsertActive]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime
	,@WorkOrderId int
AS
BEGIN
	BEGIN TRY
		if not exists (select * from ActiveAlarms 
						where CustomerID = @CustomerId and AreaID = @AreaId and MeterId = @MeterId
							and EventCode = @EventCode and EventSource = @EventSource 
							and TimeOfOccurrance = @TimeOfOccurrance)
		begin
			Insert into ActiveAlarms
				([CustomerID]
			   ,[AreaID]
			   ,[MeterId]
			   ,[EventCode]
			   ,[EventSource]
			   ,[TimeOfOccurrance]
			   ,[TimeOfNotification]
			   ,[WorkOrderId])
			 values
				 (@CustomerId
				 ,@AreaId
				 ,@MeterId
				 ,@EventCode
				 ,@EventSource
				 ,@TimeOfOccurrance
				 ,@TimeOfNotification
				 ,@WorkOrderId)
			Print 'Inserted'
		end
	END TRY
	BEGIN CATCH
		print 'sp_Alarm_InsertActive : ERROR :' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_InsertHistorical]    Script Date: 04/01/2014 22:06:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_InsertHistorical]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@EventState int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime
	,@TimeofClearance DateTime
	,@ClearingEventUID int
	,@EventUID int output	
AS
BEGIN
	BEGIN TRY
		if not exists (select * from HistoricalAlarms 
						where CustomerID = @CustomerId and AreaID = @AreaId and MeterId = @MeterId
							and EventCode = @EventCode and EventSource = @EventSource and EventState = @EventState
							and TimeOfOccurrance = @TimeOfOccurrance)
		begin
			Insert into HistoricalAlarms
				([CustomerID]
			   ,[AreaID]
			   ,[MeterId]
			   ,[EventCode]
			   ,[EventSource]
			   ,[EventState]
			   ,[TimeOfOccurrance]
			   ,[TimeOfNotification]
			   ,[ClearingEventUID]
			   ,[TimeOfClearance]			  
			   )
			 values
				 (@CustomerId
				 ,@AreaId
				 ,@MeterId
				 ,@EventCode
				 ,@EventSource
				 ,@EventState
				 ,@TimeOfOccurrance
				 ,@TimeOfNotification
				 ,@ClearingEventUID
				 ,@TimeofClearance
				 )
			
			select @EventUID = EventUID
			from HistoricalAlarms where CustomerID = @CustomerId and MeterId = @MeterId and AreaID = @AreaId
			and EventCode = @EventCode and EventState = @EventState 
			and TimeOfOccurrance = @TimeOfOccurrance and TimeOfNotification = @TimeOfNotification
			
			
			Print 'Inserted Historical Alarm with EventUID = ' + convert(varchar,@EventUID)
		end
	END TRY
	BEGIN CATCH
		print 'sp_Alarm_InsertHistorical : ERROR :' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_Raised]    Script Date: 04/01/2014 22:06:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_Raised] 
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime
	,@WorkOrderId int
	,@Message varchar(500) output
AS

DECLARE 
	@TimeTypeCustomerId     INT
	,@EventUID int
	,@TimeOfClearance DateTime
	
BEGIN
	BEGIN TRY
		set @Message = 'sp_Alarm_Raised :'
	
		exec sp_Alarm_SearchHistory	@CustomerId,@AreaId,@MeterId
					,@EventCode,@EventSource,@TimeOfOccurrance
					,@EventUID output
					,@TimeOfClearance output
					
		set @Message = @Message + '@EventUID=' + convert(varchar,@EventUID)
		
		if (@TimeOfClearance is null) begin
			set @TimeOfClearance = @TimeOfNotification
		end 	
		
		set @Message = @Message + ', @ToC=' + convert(varchar,@TimeOfClearance)
				
					
		if (@EventUID is null or @EventUID = 0) begin
		
			set @Message = @Message +  '. New Alarm and Not Cleared.'
			exec sp_Alarm_InsertActive @CustomerId,@AreaId,@MeterId
				,@EventCode,@EventSource,@TimeOfOccurrance,@TimeOfNotification				
				,@WorkOrderId
				
			set @Message = @Message +  ' Inserted in Acitve.'
			
		end else begin
			set @Message = @Message +  ' Already cleared.'			
			exec sp_Alarm_InsertHistorical	
				@CustomerId,@AreaId,@MeterId,@EventCode,@EventSource
				,1 -- @EventState/Raised
				,@TimeOfOccurrance
				,@TimeOfNotification
				,@TimeOfClearance -- for @TimeofClearance
				,@EventUID -- for ClearingEventUID 
				,@EventUID output
			set @Message = @Message +  ' Insert into Historical.'
		end 
		
	END TRY
	BEGIN CATCH
		print 'sp_Alarm_Raised : ERROR:' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_DeleteActive]    Script Date: 04/01/2014 22:06:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_DeleteActive]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@State int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime
	,@EventSource int
AS
BEGIN
	BEGIN TRY
		Print 'sp_Alarm_DeleteActive : Deleting ActiveAlarms toc = ' + convert(varchar,@TimeOfOccurrance)
		Delete From ActiveAlarms
			where CustomerID = @CustomerId and AreaID = @AreaId and MeterId = @MeterId
			and EventCode = @EventCode and EventSource = @EventSource
			and TimeOfOccurrance = @TimeOfOccurrance
			
	END TRY
	BEGIN CATCH
		print 'sp_Alarm_DeleteActive : ERROR :' + ERROR_MESSAGE()
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_MoveActiveToHistorical]    Script Date: 04/01/2014 22:06:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_MoveActiveToHistorical]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@EventState int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime
	,@TimeofClearance DateTime
	,@ClearingEventUID int
	,@EventUID int output
AS
BEGIN
	BEGIN TRY
		Print 'sp_Alarm_MoveActiveToHistorical : Inserting Historical Alarms'
		
	
		
		exec sp_Alarm_InsertHistorical @CustomerId,@AreaId,@MeterId
				,@EventCode,@EventSource ,@EventState
				,@TimeOfOccurrance,@TimeOfNotification
				,@TimeofClearance
				,@ClearingEventUID -- Clearing EventUID		
				,@EventUID Output
		
		
		Print 'sp_Alarm_MoveActiveToHistorical : Deleting Active Alarms'
		
		exec sp_Alarm_DeleteActive  @Customerid,@AreaId,@MeterId
			,@EventCode,@EventState
			,@TimeOfOccurrance,@TimeOfNotification
			,@EventSource
	END TRY
	BEGIN CATCH
		Print 'ERRIR'
	END CATCH
END
GO
/****** Object:  Table [dbo].[CollectionRunMeter]    Script Date: 04/01/2014 22:06:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollectionRunMeter](
	[CollectionRunMeterId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CollectionRunId] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[VendorId] [varchar](255) NULL,
	[ExipryDate] [datetime] NULL,
 CONSTRAINT [PK_CollectionRunMeter] PRIMARY KEY CLUSTERED 
(
	[CollectionRunMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CollectionRun]    Script Date: 04/01/2014 22:06:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollectionRun](
	[CollectionRunId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CollectionRunName] [varchar](255) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[CreatedBy] [int] NULL,
	[LastEdited] [datetime] NULL,
	[LastEditedBy] [int] NULL,
	[ActivationDate] [datetime] NULL,
	[DaysBetweenCol] [int] NULL,
	[SkipPublicHolidays] [bit] NULL,
	[SkipSpecificDaysOfWeek] [bit] NULL,
	[CollectionRunStatus] [int] NOT NULL,
	[VendorId] [int] NULL,
	[SkipSpecificDaysOfWeekSunday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekMonday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekTuesday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekWednesday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekThursday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekFriday] [bit] NOT NULL,
	[SkipSpecificDaysOfWeekSaturday] [bit] NOT NULL,
	[DeleteDate] [datetime] NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_CollectionRun] PRIMARY KEY CLUSTERED 
(
	[CollectionRunId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[v_ActiveCollectionRunMeter]    Script Date: 04/01/2014 22:06:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[v_ActiveCollectionRunMeter] as 
	select r.CustomerId,rm.AreaId,rm.MeterId,r.ActivationDate,rm.ExipryDate,r.CollectionRunId,r.CollectionRunStatus from 
	CollectionRun r join CollectionRunMeter rm
	on r.CollectionRunId = rm.CollectionRunId
	and r.CollectionRunStatus = 1 --Active
GO
/****** Object:  Table [dbo].[CollectionRunReport]    Script Date: 04/01/2014 22:06:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectionRunReport](
	[CollectionRunReportId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CollectionRunId] [bigint] NOT NULL,
	[CollectionDate] [datetime] NOT NULL,
	[MeterCoinType1Count] [int] NULL,
	[MeterCoinType2Count] [int] NULL,
	[MeterCoinType3Count] [int] NULL,
	[MeterCoinType4Count] [int] NULL,
	[MeterCoinType5Count] [int] NULL,
	[MeterCoinType6Count] [int] NULL,
	[MeterCoinType7Count] [int] NULL,
	[MeterCoinType8Count] [int] NULL,
	[TotalMeterCount] [int] NOT NULL,
	[ManualCoinType1Count] [int] NULL,
	[ManualCoinType2Count] [int] NULL,
	[ManualCoinType3Count] [int] NULL,
	[ManualCoinType4Count] [int] NULL,
	[ManualCoinType5Count] [int] NULL,
	[ManualCoinType6Count] [int] NULL,
	[ManualCoinType7Count] [int] NULL,
	[ManualCoinType8Count] [int] NULL,
	[TotalManualMeterCount] [int] NOT NULL,
	[ProcessedTS] [datetime] NULL,
	[VendorId] [int] NULL,
	[TotalByChip] [int] NULL,
	[ChipAutoCoinType1Count] [int] NULL,
	[ChipAutoCoinType2Count] [int] NULL,
	[ChipAutoCoinType3Count] [int] NULL,
	[ChipAutoCoinType4Count] [int] NULL,
	[ChipAutoCoinType5Count] [int] NULL,
	[ChipAutoCoinType6Count] [int] NULL,
	[ChipAutoCoinType7Count] [int] NULL,
	[ChipAutoCoinType8Count] [int] NULL,
	[TotalByChipMeterCount] [int] NULL,
	[TotalByChipCoinCount] [int] NULL,
	[TotalManualCoinCount] [int] NOT NULL,
	[TotalManualCashAmt] [int] NOT NULL,
	[TotalMeterCashAmt] [int] NOT NULL,
	[TotalMeterCoinCount] [int] NOT NULL,
 CONSTRAINT [PK_CollectionRunReport] PRIMARY KEY CLUSTERED 
(
	[CollectionRunReportId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_CashBoxDataImport_CollectionRunReport]    Script Date: 04/01/2014 22:06:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CashBoxDataImport_CollectionRunReport]
	@CollectionRunId int 
	,@cid int
	
AS
	Declare 
		@collRunReportId bigint
		,@collectionDate DateTime
		,@chipAutoCoinType1Count int
		,@chipAutoCoinType2Count int
		,@chipAutoCoinType3Count int
		,@chipAutoCoinType4Count int
		,@chipAutoCoinType5Count int
		,@chipAutoCoinType6Count int
		,@chipAutoCoinType7Count int
		,@chipAutoCoinType8Count int
		,@TotalByChip int
		,@TotalByChipMeterCount int
		,@TotalByChipCoinCount int
		,@TotalManulaCashAmount int
		
	
BEGIN
	SET NOCOUNT ON;
	if @CollectionRunId is null return
	
	select @collectionDate = ActivationDate
			from CollectionRun
			where CollectionRunId = @CollectionRunId

	select @collRunReportId = CollectionRunReportId from CollectionRunReport
	if (@collRunReportId is null)
	begin
		Insert into CollectionRunReport
		(CustomerId,CollectionRunId,CollectionDate
		,TotalManualCashAmt, TotalManualCoinCount,TotalManualMeterCount
		,TotalMeterCount,TotalMeterCoinCount,TotalMeterCashAmt)
		values
		(@cid,@CollectionRunId,@collectionDate
		,0,0,0,0
		,0,0)
		
		set @collRunReportId = SCOPE_IDENTITY()
	end
	
	if (@collRunReportId is not null)
	begin
		select @TotalByChipMeterCount = COUNT(distinct tx.MeterId)
		,@chipAutoCoinType1Count = SUM(IsNull(Cents5Coins,0))
		,@chipAutoCoinType2Count = SUM(IsNull(Cents10Coins,0))
		,@chipAutoCoinType3Count = SUM(IsNull(Cents20Coins,0))
		--,@chipAutoCoinType4Count = SUM(CoinCent25)
		,@chipAutoCoinType5Count = SUM(IsNull(Cents50Coins,0))
		,@chipAutoCoinType6Count = SUM(IsNull(Dollar1Coins,0))
		,@chipAutoCoinType7Count = SUM(IsNull(Dollar2Coins,0))
		,@TotalByChip = SUM(IsNull(AmtAuto,0))
		,@TotalManulaCashAmount=SUM(IsNull(AmtManual,0))
		from CashBoxDataImport tx		
		where tx.CollectionRunId = @CollectionRunId
		
		if (@TotalByChipMeterCount is null) set @TotalByChipMeterCount = 0
		if (@TotalByChip is null) set @TotalByChip = 0
		
		set @TotalByChipCoinCount = @chipAutoCoinType1Count +@chipAutoCoinType2Count+@chipAutoCoinType3Count+@chipAutoCoinType4Count+@chipAutoCoinType5Count+@chipAutoCoinType6Count+@chipAutoCoinType7Count+@chipAutoCoinType8Count
		
		Update CollectionRunReport
		set TotalByChipMeterCount = @TotalByChipMeterCount
		,TotalByChipCoinCount = @TotalByChipCoinCount
		,TotalByChip = @TotalByChip
		,chipAutoCoinType1Count = @chipAutoCoinType1Count
		,chipAutoCoinType2Count = @chipAutoCoinType2Count
		,chipAutoCoinType3Count = @chipAutoCoinType3Count
		,chipAutoCoinType4Count = @chipAutoCoinType4Count
		,chipAutoCoinType5Count = @chipAutoCoinType5Count
		,chipAutoCoinType6Count = @chipAutoCoinType6Count
		,chipAutoCoinType7Count = @chipAutoCoinType7Count
		,chipAutoCoinType8Count = @chipAutoCoinType8Count
		,TotalManualCashAmt = @TotalManulaCashAmount
		where CollectionRunId = @CollectionRunId
	end
END
GO
/****** Object:  Table [dbo].[CashBoxDataImport]    Script Date: 04/01/2014 22:06:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CashBoxDataImport](
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DateTimeIns] [datetime] NOT NULL,
	[CashBoxId] [varchar](14) NOT NULL,
	[CashboxSequenceNo] [int] NOT NULL,
	[DateTimeRead] [datetime] NOT NULL,
	[DateTimeRem] [datetime] NULL,
	[OperatorId] [varchar](24) NULL,
	[AutoFlag] [char](1) NULL,
	[Dollar2Coins] [int] NULL,
	[Dollar1Coins] [int] NULL,
	[Cents50Coins] [int] NULL,
	[Cents20Coins] [int] NULL,
	[Cents10Coins] [int] NULL,
	[Cents5Coins] [int] NULL,
	[AmtCashless] [float] NULL,
	[AmtAuto] [float] NULL,
	[AmtManual] [float] NULL,
	[AmtDiff] [real] NULL,
	[PercentFull] [float] NULL,
	[MeterStatus] [varchar](16) NULL,
	[TallyRejects] [int] NULL,
	[CreditCounter] [int] NULL,
	[TimeActive] [bigint] NULL,
	[MinVolts] [real] NULL,
	[MaxTemp] [real] NULL,
	[FirmwareVer] [int] NULL,
	[FirmwareRev] [int] NULL,
	[EventCode] [int] NULL,
	[FileName] [varchar](50) NULL,
	[FileProcessId] [bigint] NULL,
	[EventUID] [bigint] NULL,
	[CollectionRunId] [bigint] NULL,
	[UnscheduledFlag] [bit] NOT NULL,
 CONSTRAINT [PK_CashBoxDataImport] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[DateTimeIns] ASC,
	[CashBoxId] ASC,
	[CashboxSequenceNo] ASC,
	[DateTimeRead] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [CashBoxDataImport_IDX_DAAM] ON [dbo].[CashBoxDataImport] 
(
	[DateTimeRem] DESC,
	[AmtManual] ASC,
	[AreaId] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CashBoxDataImport_IDX_MDA] ON [dbo].[CashBoxDataImport] 
(
	[MeterId] ASC,
	[DateTimeRem] ASC,
	[AreaId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CashBoxDataImport_IDX_MDADA] ON [dbo].[CashBoxDataImport] 
(
	[MeterId] ASC,
	[DateTimeRem] ASC,
	[AreaId] ASC,
	[DateTimeRead] ASC,
	[AmtManual] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CashBoxDataImport_IDX_MDCCAFPAAOCDA] ON [dbo].[CashBoxDataImport] 
(
	[MeterId] ASC,
	[DateTimeRem] ASC,
	[CashboxSequenceNo] ASC,
	[CustomerId] ASC,
	[AreaId] ASC,
	[FileProcessId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CashBoxDataImport_IDXGlobalMeterID] ON [dbo].[CashBoxDataImport] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[CashboxDataImportV]    Script Date: 04/01/2014 22:06:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CashboxDataImportV]
AS
SELECT TOP 100 PERCENT CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12))) AS xFileProcessID, 
		dbo.CashBoxDataImport.* 
				FROM dbo.CashBoxDataImport 
				ORDER BY CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12)))
GO
/****** Object:  View [dbo].[CashBoxImportSubV]    Script Date: 04/01/2014 22:06:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CashBoxImportSubV]
AS
SELECT TOP 100 PERCENT AreaId, MeterId, DateTimeRem, AmtManual, COALESCE
                          ((SELECT AVG(AmtManual)
                              FROM CashboxDataImportV b
                              WHERE b.MeterId = a.MeterId AND b.AreaId = a.AreaId AND b.DateTimeRem <= a.DateTimeRem AND b.DateTimeRem IN 
									(SELECT TOP 10 DateTimeRem 
									FROM CashboxDataImport c 
									WHERE c.MeterId = a.MeterId 
									AND b.AreaId = a.AreaId 
									AND c.DateTimeRem <= a.DateTimeRem 
									ORDER BY c.DateTimeRem DESC)), 0) AS AvgL10, COALESCE
                          ((SELECT STDEV(AmtManual)
                              FROM CashboxDataImportV b
                              WHERE b.MeterId = a.MeterId 
							  AND b.AreaId = a.AreaId 
							  AND b.DateTimeRem <= a.DateTimeRem 
							  AND b.DateTimeRem IN
                                                        (SELECT TOP 10 DateTimeRem
                                                          FROM CashboxDataImport c
                                                          WHERE c.MeterId = a.MeterId AND b.AreaId = a.AreaId AND c.DateTimeRem <= a.DateTimeRem
                                                          ORDER BY c.DateTimeRem DESC)), 0) AS StdDevL10, CollectionRunId, CustomerId, UnscheduledFlag, DateTimeIns, CashBoxId, CashboxSequenceNo, DateTimeRead, OperatorId, AutoFlag, Dollar2Coins, Dollar1Coins, Cents50Coins, Cents20Coins, Cents10Coins, Cents5Coins, AmtCashless, AmtAuto, AmtDiff, PercentFull, MeterStatus, TallyRejects, CreditCounter, TimeActive, MinVolts, MaxTemp, FirmwareVer, FirmwareRev, EventCode, FileName, xFileProcessID 
														  FROM dbo.CashboxDataImportV a
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_Clear]    Script Date: 04/01/2014 22:06:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_Clear]
@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@TimeOfOccurrance DateTime
	,@TimeOfClearance DateTime	
	,@Message varchar(500) output
AS
Declare
	@EventUID int
	,@EventUIDOutput int
	,@EventState int
	,@TimeOfActiveNotification DateTime		
	,@CNT int
	,@CursorStatus int
BEGIN

	set @EventState = 0 -- Cleared
	set @CNT = 0
	set @Message = 'sp_Alarm_Clear :'
	
	BEGIN TRY
		-- Insert a general cleared event
		exec sp_Alarm_InsertHistorical @CustomerId,@AreaId,@MeterId
				,@EventCode -- EventCode
				,@EventSource -- EventSource
				,0 -- EventState/0=Cleared,1=Raised/0 for initial
				,@TimeOfOccurrance
				,@TimeOfClearance -- for TimeOfNotification
				,@TimeOfClearance -- for TimeOfClearance
				,0 -- Clearing EventUID
				,@EventUID Output
				
		
		
		set @Message = @Message + '@EventUID=' + convert(varchar,@EventUID)
		print @Message
		
		-- get all alarms occured 
		Declare curActives  cursor	
			--LOCAL STATIC READ_ONLY FORWARD_ONLY
			STATIC
			for
			SELECT CustomerID,AreaID,MeterId,EventCode,EventSource,TimeOfOccurrance,TimeOfNotification
				FROM ActiveAlarms a
				where a.CustomerID = @customerid
				and a.AreaID = @AreaId
				and a.MeterId = @MeterId
				and 
				(
					(a.EventCode = @EventCode and a.EventSource = @EventSource)-- Specific Clear
					or 
					@EventCode = 0  -- Generic Clear
				)
				and a.TimeOfOccurrance < @TimeOfOccurrance;
				
				
		open curActives 	
		set @Message = @Message + ' alarms=' + Convert(varchar,@@Cursor_Rows)		
			
			fetch next from curActives into @CustomerID,@AreaID,@MeterId,@EventCode,@EventSource,@TimeOfOccurrance,@TimeOfActiveNotification
			WHILE @@FETCH_STATUS = 0 BEGIN		
				BEGIN TRY
				
					exec sp_Alarm_MoveActiveToHistorical @CustomerId,@AreaId,@MeterId
								,@EventCode,@EventSource
								,1 -- @EventState/0=Cleared,1=Raised (for moving alarm)
								,@TimeOfOccurrance
								,@TimeOfActiveNotification
								,@TimeOfClearance -- for @TimeofClearance
								,@EventUID --from above general cleared event
								,@EventUIDOutput -- output
					set @CNT = @CNT + 1
				END TRY
				BEGIN CATCH
					print 'sp_Alarm_Clear: cursor : ERROR:' + ERROR_MESSAGE()
				END CATCH
				fetch next from curActives into @CustomerID,@AreaID,@MeterId,@EventCode,@EventSource,@TimeOfOccurrance,@TimeOfActiveNotification
			END	--while @@FETCH_STATUS			
			set @Message = @Message + ' moved=' + CONVERT(varchar,@CNT)
		CLOSE curActives 	
		DEALLOCATE curActives 
		
	END TRY	
	BEGIN CATCH
		set @Message = @Message +  ' ERROR=' + ERROR_MESSAGE()
		print @Message		
		SELECT @CursorStatus= (CURSOR_STATUS('global','curActives'))
		IF (@CursorStatus >= -1) BEGIN
			DEALLOCATE curActives
		END		
	END CATCH
END
GO
/****** Object:  Table [dbo].[SensorPaymentTransactionAudit]    Script Date: 04/01/2014 22:06:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SensorPaymentTransactionAudit](
	[AuditID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SensorPaymentTransactionID] [bigint] NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[ArrivalTime] [datetime] NOT NULL,
	[ArrivalPSOAuditId] [bigint] NOT NULL,
	[DepartureTime] [datetime] NULL,
	[DeparturePSOAuditId] [bigint] NULL,
	[FirstTxPaymentTime] [datetime] NULL,
	[FirstTxStartTime] [datetime] NULL,
	[FirstTxExpiryTime] [datetime] NULL,
	[FirstTxAmountInCent] [int] NULL,
	[FirstTxTimePaidMinute] [int] NULL,
	[FirstTxPaymentMethod] [int] NULL,
	[FirstTxID] [int] NULL,
	[LastTxPaymentTime] [datetime] NULL,
	[LastTxExpiryTime] [datetime] NULL,
	[LastTxAmountInCent] [int] NULL,
	[LastTxTimePaidMinute] [int] NULL,
	[LastTxPaymentMethod] [int] NULL,
	[LastTxID] [int] NULL,
	[TotalAmountInCent] [int] NULL,
	[TotalNumberOfPayment] [int] NULL,
	[TotalTimePaidMinute] [int] NULL,
	[TotalOccupiedMinute] [int] NULL,
	[DiscountSchema] [int] NULL,
	[GracePeriodMinute] [int] NULL,
	[ViolationMinute] [int] NULL,
	[OccupancyStatus] [int] NULL,
	[NonCompliantStatus] [int] NULL,
	[RemaingPaidTimeMinute] [int] NULL,
	[ZeroOutTime] [datetime] NULL,
	[OperationalStatus] [int] NULL,
	[InfringementLink] [varchar](500) NULL,
	[RecordCreatTime] [datetime] NULL,
	[FreeParkingMinute] [int] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[ViolationSegmentCount] [int] NULL,
	[FreeParkingTime] [int] NULL,
	[GracePeriodUsed] [bit] NULL,
	[ChangeDate] [datetime] NOT NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[SensorId] [int] NULL,
	[GatewayId] [int] NULL,
	[CustomerId] [int] NULL,
	[OccupancyDate] [datetime] NULL,
 CONSTRAINT [PK_SensorPaymentTransactionAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SensorPaymentTransactionCurrent]    Script Date: 04/01/2014 22:06:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SensorPaymentTransactionCurrent](
	[ParkingSpaceId] [bigint] NOT NULL,
	[SensorPaymentTransactionID] [bigint] NOT NULL,
	[ArrivalTime] [datetime] NOT NULL,
	[ArrivalPSOAuditId] [bigint] NOT NULL,
	[DepartureTime] [datetime] NULL,
	[DeparturePSOAuditId] [bigint] NULL,
	[FirstTxPaymentTime] [datetime] NULL,
	[FirstTxStartTime] [datetime] NULL,
	[FirstTxExpiryTime] [datetime] NULL,
	[FirstTxAmountInCent] [int] NULL,
	[FirstTxTimePaidMinute] [int] NULL,
	[FirstTxPaymentMethod] [int] NULL,
	[FirstTxID] [int] NULL,
	[LastTxPaymentTime] [datetime] NULL,
	[LastTxExpiryTime] [datetime] NULL,
	[LastTxAmountInCent] [int] NULL,
	[LastTxTimePaidMinute] [int] NULL,
	[LastTxPaymentMethod] [int] NULL,
	[LastTxID] [int] NULL,
	[TotalAmountInCent] [int] NULL,
	[TotalNumberOfPayment] [int] NULL,
	[TotalTimePaidMinute] [int] NULL,
	[TotalOccupiedMinute] [int] NULL,
	[DiscountSchema] [int] NULL,
	[GracePeriodMinute] [int] NULL,
	[ViolationMinute] [int] NULL,
	[OccupancyStatus] [int] NULL,
	[NonCompliantStatus] [int] NULL,
	[RemaingPaidTimeMinute] [int] NULL,
	[ZeroOutTime] [datetime] NULL,
	[OperationalStatus] [int] NULL,
	[InfringementLink] [varchar](500) NULL,
	[RecordCreatTime] [datetime] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[FreeParkingMinute] [int] NULL,
	[OccupancyDate] [datetime] NULL,
	[ViolationSegmentCount] [int] NULL,
	[FreeParkingTime] [int] NULL,
	[GracePeriodUsed] [bit] NULL,
	[SensorId] [int] NULL,
	[GatewayId] [int] NULL,
	[CustomerId] [int] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
 CONSTRAINT [PK_SensorPaymentTransactionCurrent] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_SensorPaymentTransactionAudit]    Script Date: 04/01/2014 22:06:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SensorPaymentTransactionAudit] 
	@SensorPaymentTransactionID bigint
	,@ParkingSpaceID bigint
AS
	DECLARE
		@ERROR_MESSAGE varchar(2000)	
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			if (@ParkingSpaceID is not null) and (@SensorPaymentTransactionID is not null) begin
			
				--Update current only for newer SPTxID
				if exists(select * from SensorPaymentTransactionCurrent where ParkingSpaceId = @ParkingSpaceID
								and [SensorPaymentTransactionId] < @SensorPaymentTransactionID) 
				begin
					Print 'Exists and deleting SensorPaymentTransactionCurrent for PSID ' + convert(Varchar,@ParkingSpaceID)
					Delete [SensorPaymentTransactionCurrent] where ParkingSpaceId = @ParkingSpaceID
					
					Print 'Inserting into [SensorPaymentTransactionCurrent] for ' + convert(varchar,@SensorPaymentTransactionID)
					Insert into [SensorPaymentTransactionCurrent] (
						[SensorPaymentTransactionId],
						[ParkingSpaceId],[ArrivalTime],[ArrivalPSOAuditId],[DepartureTime],[DeparturePSOAuditId],[FirstTxPaymentTime],
						[FirstTxStartTime],[FirstTxExpiryTime],[FirstTxAmountInCent],[FirstTxTimePaidMinute],[FirstTxPaymentMethod],[FirstTxID],
						[LastTxPaymentTime],[LastTxExpiryTime],[LastTxAmountInCent],[LastTxTimePaidMinute],[LastTxPaymentMethod],[LastTxID],
						[TotalAmountInCent],[TotalNumberOfPayment],[TotalTimePaidMinute],[TotalOccupiedMinute],
						[DiscountSchema],[GracePeriodMinute],[ViolationMinute],[OccupancyStatus],[NonCompliantStatus],[RemaingPaidTimeMinute],
						[ZeroOutTime],[OperationalStatus],[InfringementLink],[RecordCreatTime]
						,[TimeType1],[TimeType2],[TimeType3],[TimeType4],[TimeType5],
						[FreeParkingMinute],[OccupancyDate],[ViolationSegmentCount],[FreeParkingTime],[GracePeriodUsed],[PrepayUsed],[FreeParkingUsed]
						,[SensorId],[GatewayId],[CustomerId])
					SELECT 
						@SensorPaymentTransactionID as 
						[SensorPaymentTransactionId],
						[ParkingSpaceId],[ArrivalTime],[ArrivalPSOAuditId],[DepartureTime],[DeparturePSOAuditId],[FirstTxPaymentTime],
						[FirstTxStartTime],[FirstTxExpiryTime],[FirstTxAmountInCent],[FirstTxTimePaidMinute],[FirstTxPaymentMethod],[FirstTxID],
						[LastTxPaymentTime],[LastTxExpiryTime],[LastTxAmountInCent],[LastTxTimePaidMinute],[LastTxPaymentMethod],[LastTxID],
						[TotalAmountInCent],[TotalNumberOfPayment],[TotalTimePaidMinute],[TotalOccupiedMinute],
						[DiscountSchema],[GracePeriodMinute],[ViolationMinute],[OccupancyStatus],[NonCompliantStatus],[RemaingPaidTimeMinute],
						[ZeroOutTime],[OperationalStatus],[InfringementLink],[RecordCreatTime]
						,[TimeType1],[TimeType2],[TimeType3],[TimeType4],[TimeType5],
						[FreeParkingMinute],[OccupancyDate],[ViolationSegmentCount],[FreeParkingTime],[GracePeriodUsed],[PrepayUsed],[FreeParkingUsed]
						,[SensorId],[GatewayId],[CustomerId]  
						FROM [SensorPaymentTransaction]
						where [SensorPaymentTransactionID] = @SensorPaymentTransactionID
				end 
				
				Print 'Inserting into [SensorPaymentTransactionAudit]  for ' + convert(varchar,@SensorPaymentTransactionID)												
				Insert into [SensorPaymentTransactionAudit] (
						[SensorPaymentTransactionId],
						[ParkingSpaceId],[ArrivalTime],[ArrivalPSOAuditId],[DepartureTime],[DeparturePSOAuditId],[FirstTxPaymentTime],
						[FirstTxStartTime],[FirstTxExpiryTime],[FirstTxAmountInCent],[FirstTxTimePaidMinute],[FirstTxPaymentMethod],[FirstTxID],
						[LastTxPaymentTime],[LastTxExpiryTime],[LastTxAmountInCent],[LastTxTimePaidMinute],[LastTxPaymentMethod],[LastTxID],
						[TotalAmountInCent],[TotalNumberOfPayment],[TotalTimePaidMinute],[TotalOccupiedMinute],
						[DiscountSchema],[GracePeriodMinute],[ViolationMinute],[OccupancyStatus],[NonCompliantStatus],[RemaingPaidTimeMinute],
						[ZeroOutTime],[OperationalStatus],[InfringementLink],[RecordCreatTime]
						,[TimeType1],[TimeType2],[TimeType3],[TimeType4],[TimeType5],
						[FreeParkingMinute],[OccupancyDate],[ViolationSegmentCount],[FreeParkingTime],[GracePeriodUsed],[PrepayUsed],[FreeParkingUsed]
						,[SensorId],[GatewayId],[CustomerId])
					SELECT 
						@SensorPaymentTransactionID as [SensorPaymentTransactionId],
						[ParkingSpaceId],[ArrivalTime],[ArrivalPSOAuditId],[DepartureTime],[DeparturePSOAuditId],[FirstTxPaymentTime],
						[FirstTxStartTime],[FirstTxExpiryTime],[FirstTxAmountInCent],[FirstTxTimePaidMinute],[FirstTxPaymentMethod],[FirstTxID],
						[LastTxPaymentTime],[LastTxExpiryTime],[LastTxAmountInCent],[LastTxTimePaidMinute],[LastTxPaymentMethod],[LastTxID],
						[TotalAmountInCent],[TotalNumberOfPayment],[TotalTimePaidMinute],[TotalOccupiedMinute],
						[DiscountSchema],[GracePeriodMinute],[ViolationMinute],[OccupancyStatus],[NonCompliantStatus],[RemaingPaidTimeMinute],
						[ZeroOutTime],[OperationalStatus],[InfringementLink],[RecordCreatTime]
						,[TimeType1],[TimeType2],[TimeType3],[TimeType4],[TimeType5],
						[FreeParkingMinute],[OccupancyDate],[ViolationSegmentCount],[FreeParkingTime],[GracePeriodUsed],[PrepayUsed],[FreeParkingUsed]
						,[SensorId],[GatewayId],[CustomerId]  
						FROM [SensorPaymentTransaction]
						where [SensorPaymentTransactionID] = @SensorPaymentTransactionID								
				Print 'Inserted'	
			end else begin
				Print '@ParkingSpaceID or @SensorPaymentTransactionID is null'
			end 
		COMMIT
	END TRY 
	BEGIN CATCH
		SET @ERROR_MESSAGE = Error_message()
		Print 'ERROR IN sp_SensorPaymentTransactionCurrent' + @ERROR_MESSAGE
	END CATCH
END
GO
/****** Object:  View [dbo].[qvActMaxAlmTime]    Script Date: 04/01/2014 22:06:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qvActMaxAlmTime]
AS
SELECT     MAX(TimeOfOccurrance) AS MaxAlmTime, CustomerID, AreaID, MeterID, EventCode, EventSource, MAX(TimeOfNotification) AS MaxNotfnTime
FROM         dbo.ActiveAlarms
GROUP BY CustomerID, AreaID, MeterID, EventCode, EventSource
GO
/****** Object:  View [dbo].[qCBImpFiles]    Script Date: 04/01/2014 22:06:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qCBImpFiles]
AS
SELECT     CONVERT(datetime, DATEDIFF([day], 0, DateTimeRead), 1) AS DateTimeImp, CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12))) 
                      AS xFileProcessID, CustomerId, ' Collection Run ' + CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12)) AS FileNameImp
FROM         dbo.CashBoxDataImport
GROUP BY CONVERT(datetime, DATEDIFF([day], 0, DateTimeRead), 1), CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12))), CustomerId, 
                      ' Collection Run ' + CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12))
GO
/****** Object:  View [dbo].[qCashBoxDataImport]    Script Date: 04/01/2014 22:06:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qCashBoxDataImport]
AS
SELECT     TOP 100 PERCENT CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12))) AS xFileProcessID, dbo.CashBoxDataImport.*
FROM         dbo.CashBoxDataImport
ORDER BY CHECKSUM(CAST(CONVERT(datetime, DateTimeRead, 1) AS varchar(12)))
GO
/****** Object:  View [dbo].[qCashBoxImport_subV235]    Script Date: 04/01/2014 22:06:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qCashBoxImport_subV235]
AS
SELECT     TOP 100 PERCENT AreaId, MeterId, DateTimeRem, AmtManual, COALESCE
                          ((SELECT     AVG(AmtManual)
                              FROM         qCashboxDataImport b
                              WHERE     b.MeterId = a.MeterId AND b.AreaId = a.AreaId AND b.DateTimeRem <= a.DateTimeRem AND b.DateTimeRem IN
                                                        (SELECT     TOP 10 DateTimeRem
                                                          FROM          CashboxDataImport c
                                                          WHERE      c.MeterId = a.MeterId AND b.AreaId = a.AreaId AND c.DateTimeRem <= a.DateTimeRem
                                                          ORDER BY c.DateTimeRem DESC)), 0) AS AvgL10, COALESCE
                          ((SELECT     STDEV(AmtManual)
                              FROM         qCashboxDataImport b
                              WHERE     b.MeterId = a.MeterId AND b.AreaId = a.AreaId AND b.DateTimeRem <= a.DateTimeRem AND b.DateTimeRem IN
                                                        (SELECT     TOP 10 DateTimeRem
                                                          FROM          CashboxDataImport c
                                                          WHERE      c.MeterId = a.MeterId AND b.AreaId = a.AreaId AND c.DateTimeRem <= a.DateTimeRem
                                                          ORDER BY c.DateTimeRem DESC)), 0) AS StdDevL10, CustomerId, DateTimeIns, CashBoxId, CashboxSequenceNo, DateTimeRead, 
                      OperatorId, AutoFlag, Dollar2Coins, Dollar1Coins, Cents50Coins, Cents20Coins, Cents10Coins, Cents5Coins, AmtCashless, AmtAuto, AmtDiff, 
                      PercentFull, MeterStatus, TallyRejects, CreditCounter, TimeActive, MinVolts, MaxTemp, FirmwareVer, FirmwareRev, EventCode, FileName, 
                      xFileProcessID
FROM         dbo.qCashBoxDataImport a
GO
/****** Object:  StoredProcedure [dbo].[sp_GetZoneID_From_MeterMap]    Script Date: 04/01/2014 22:06:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetZoneID_From_MeterMap] 
	-- Add the parameters for the stored procedure here
	@MeterID		int	= 0, 
	@CustomerID		int	= 0,
	@AreaID			int		= 0,
	@ZoneID			int			= 0 OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT OFF;

	-- Insert statements for procedure here
	SELECT @ZoneID=ZoneId FROM [dbo].[MeterMap]
		WHERE MeterId = @MeterID AND Customerid=@CustomerID AND Areaid=@AreaID
		
	if (@ZoneID is null) set @ZoneID = 0
		
	Return @ZoneID
END
GO
/****** Object:  Table [dbo].[SensorMapping]    Script Date: 04/01/2014 22:06:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorMapping](
	[SensorMappingID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[SensorID] [int] NOT NULL,
	[ParkingSpaceID] [bigint] NULL,
	[GatewayID] [int] NOT NULL,
	[IsPrimaryGateway] [bit] NOT NULL,
	[MappingState] [int] NOT NULL,
 CONSTRAINT [PK_SensorMapping] PRIMARY KEY CLUSTERED 
(
	[SensorMappingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingSpaces]    Script Date: 04/01/2014 22:06:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkingSpaces](
	[ParkingSpaceId] [bigint] NOT NULL,
	[ServerID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AddedDateTime] [datetime] NOT NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[HasSensor] [bit] NULL,
	[SpaceStatus] [int] NULL,
	[DateActivated] [datetime] NULL,
	[Comments] [varchar](1000) NULL,
	[DisplaySpaceNum] [varchar](50) NULL,
	[DemandZoneId] [int] NULL,
	[InstallDate] [datetime] NULL,
	[ParkingSpaceType] [int] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
 CONSTRAINT [PK_ParkingSpace] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_OccupancyRateSummary_Perf1] ON [dbo].[ParkingSpaces] 
(
	[CustomerID] ASC
)
INCLUDE ( [ParkingSpaceId],
[AreaId],
[MeterId],
[OperationalStatus]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_GetParkingSpaceId]    Script Date: 04/01/2014 22:06:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetParkingSpaceId] 
	@CustomerId INT
	,@AreaId INT
	,@MeterId INT
	,@BayNumber INT
	,@ParkingSpaceId BIGINT OUTPUT
AS
	Declare
		@ServerID INT
BEGIN
	select @ParkingSpaceId = ParkingSpaceID from ParkingSpaces
		where CustomerID= @CustomerId and AreaId = @AreaId and MeterId = @MeterId and BayNumber = @BayNumber
	
	if (@ParkingSpaceId is null) begin
		Print 'ParkingSpaceID is null'
		SET @ParkingSpaceId = dbo.Genglobalid(@CustomerID, @AreaID, @MeterID,@BayNumber);            
		
		select @ServerId = MAX(InstanceID) from ServerInstance
		
		Insert into ParkingSpaces (ParkingSpaceid,ServerID,CustomerID,AreaId,MeterId,BayNumber,AddedDateTime)
		values
		(@ParkingSpaceId,@ServerID,@CustomerId,@AreaId,@MeterId,@BayNumber,GETDATE())		
	end	else begin
		Print 'ParkingSpaceID found ' + convert(varchar,@ParkingSpaceId)	
	end
END
GO
/****** Object:  Table [dbo].[Transactions]    Script Date: 04/01/2014 22:06:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transactions](
	[TransactionsID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NULL,
	[AreaID] [int] NULL,
	[MeterID] [int] NULL,
	[ZoneID] [int] NULL,
	[ParkingSpaceID] [bigint] NULL,
	[SensorID] [int] NULL,
	[TransactionType] [int] NULL,
	[TransDateTime] [datetime] NULL,
	[ExpiryTime] [datetime] NULL,
	[AmountInCents] [int] NULL,
	[TxValue] [varchar](50) NULL,
	[TimePaid] [int] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[EventUID] [bigint] NULL,
	[BayNumber] [int] NULL,
	[CreditCardType] [int] NULL,
	[TransactionStatus] [int] NULL,
	[ReceiptNo] [int] NULL,
	[OriginalTxId] [bigint] NULL,
	[TransType] [int] NULL,
	[MeterGroupId] [int] NULL,
	[GatewayId] [int] NULL,
	[SensorPaymentTransactionId] [bigint] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[DiscountSchemeId] [int] NULL,
 CONSTRAINT [PK_Transactions] PRIMARY KEY CLUSTERED 
(
	[TransactionsID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_Transactions_Time] ON [dbo].[Transactions] 
(
	[TransDateTime] DESC,
	[MeterID] ASC,
	[AreaID] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_TxTrigger_Helper]    Script Date: 04/01/2014 22:06:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TxTrigger_Helper] 
	-- Add the parameters for the stored procedure here
	@TransactionXxxID     BIGINT,
	@TransactionType    INT,
	@ParkingSpaceId     BIGINT,
	@CustomerID         INT,
	@AreaID             INT,
	@MeterID            INT,
	@TransDateTime      DATETIME,
	@BayNumber          INT,
	@AmountInCents      INT,
	@TimePaid           INT,
	@CcType				INT,
	@TxStatus			INT,
	@ReceiptNo			INT,
	@TxValue VARCHAR(50)
		
AS
	DECLARE
	@SensorID           INT,
	@LastStatus         BIGINT,
	@ZoneID             INT,
	@EventUID           INT,
	@TimeType1          INT,
	@TimeType2          INT,
	@TimeType3          INT,
	@TimeType4          INT,
	@ExpiryTime			DATETIME,
	@Message            VARCHAR(50),
	@ERROR_MESSAGE      NVARCHAR(4000),
	@ERR                INT

BEGIN
	BEGIN TRY
		IF( @TimePaid IS NULL )	BEGIN
			SET @ExpiryTime=@TransDateTime
		END
		ELSE BEGIN
			SET @ExpiryTime=Dateadd(second, @TimePaid, @TransDateTime)
		END	
		
		IF (@AreaID is Null) Begin
			Print 'Getting AreaID' 			 
			SELECT @AreaID = areaid
				FROM   meters
				WHERE  customerid = @CustomerID
				AND meterid = @MeterID
				--AND metergroup IN ( 0, 1 )		
			Print 'AreaID = ' + convert(Varchar,@AreaID) 
		END
		
	
		--Get ZoneID from MeterMap
		EXEC Sp_getzoneid_from_metermap 
			@MeterID,
			@CustomerID,
			@AreaID,
			@ZoneID output;   

		Print 'getting @ParkingSpaceId' 			 
		IF ( @ParkingSpaceId IS NULL )
		BEGIN
			exec sp_GetParkingSpaceId @CustomerID,@AreaID,@MeterID,@BayNumber,@ParkingSpaceId OUTPUT			
		END   
		
		Print 'getting @SensorID' 			 
			
		SELECT @SensorID=SensorID FROM SensorMapping
			WHERE ParkingSpaceID = @ParkingSpaceID

		
		if exists (select * from Transactions where ParkingSpaceID = @ParkingSpaceId
									and OriginalTxId  = @TransactionXxxID
									and TransactionType = @TransactionType)
		begin
			Update transactions
				set [expirytime] = @ExpiryTime,
					[amountincents] = @AmountInCents,
					   [txvalue] = @TxValue,
					   [timepaid] = @TimePaid,					  
					   [baynumber] = @BayNumber,
					   [creditcardtype] = @CcType,
					   [transactionstatus] = @TxStatus,
					   [receiptno] = @ReceiptNo,
					   [originaltxid] = @TransactionXxxID					   
					  where ParkingSpaceID = @ParkingSpaceId
									and OriginalTxId  = @TransactionXxxID
									and TransactionType = @TransactionType
		end else begin		
			Print '@Sp_geteventuid'
			
			EXEC Sp_geteventuid
				@EventUID output;		
				
			Print 'Sp_inserteventlog2'
				
			EXEC Sp_inserteventlog2
				  @EventUID,
				  @CustomerId,
				  @AreaId,
				  @MeterId,
				  2004,
				  @TransDateTime,
				  @TimeType1 output,
				  @TimeType2 output,
				  @TimeType3 output,
				  @TimeType4 output;
		   
			Print 'Inserting into transactions'	
			INSERT INTO transactions
					  ([customerid],
					   [areaid],
					   [meterid],
					   [zoneid],
					   [parkingspaceid],
					   [sensorid],
					   [transactiontype],
					   [transdatetime],
					   [expirytime],
					   [amountincents],
					   [txvalue],
					   [timepaid],
					   [timetype1],
					   [timetype2],
					   [timetype3],
					   [timetype4],
					   [timetype5],
					   [eventuid],
					   [baynumber],
					   [creditcardtype],
					   [transactionstatus],
					   [receiptno],
					   [originaltxid],
					   [transtype],
					   [metergroupid],
					   [gatewayid])
		  VALUES      ( @CustomerID,
						@AreaID,
						@MeterID,
						@ZoneID,
						@ParkingSpaceID,
						@SensorID,
						@TransactionType,
						@TransDateTime,
						@ExpiryTime,--for Expiry time                    
						@AmountInCents,
						@TxValue,--TX VALUE for sensor only 
						@TimePaid,
						@TimeType1,
						@TimeType2,
						@TimeType3,
						@TimeType4,
						NULL, -- TimeType 5
						@EventUID,
						@BayNumber,
						@CcType,
						@TxStatus,
						@ReceiptNo,
						@TransactionXxxID,
						CASE WHEN (@TransactionType = 21) THEN 1 ELSE 2 END,--[transtype],
						NULL,--MeterGroupID
						NULL --GatewayID
						);		
		END
			
	END TRY
	BEGIN CATCH
		SET @ERROR_MESSAGE = Error_message()          
		Print 'ERROR in sp_TxTrigger_Helper : ' + @ERROR_MESSAGE
	END CATCH	
END
GO
/****** Object:  Table [dbo].[TransactionsCash]    Script Date: 04/01/2014 22:06:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionsCash](
	[TransactionsCashID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceID] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[TimePaid] [int] NOT NULL,
	[CoinDollar2] [int] NULL,
	[CoinDollar1] [int] NULL,
	[CoinCent50] [int] NULL,
	[CoinCent20] [int] NULL,
	[CoinCent10] [int] NULL,
	[CoinCent5] [int] NULL,
	[CoinCent25] [int] NULL,
	[CoinCent1] [int] NULL,
	[TimePaidByRate] [int] NULL,
	[CreateDateTime] [datetime] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[TopUp] [bit] NULL,
	[CoinUnknown] [int] NULL,
	[CoinRejected] [int] NULL,
 CONSTRAINT [PK_TransactionsCash] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCash_IDX_TTAMAC] ON [dbo].[TransactionsCash] 
(
	[TransDateTime] ASC,
	[TransactionsCashID] ASC,
	[AmountInCents] ASC,
	[MeterId] ASC,
	[AreaId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCash_IDXParkingSpaceID] ON [dbo].[TransactionsCash] 
(
	[ParkingSpaceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_CollDataSumm_CollectionRunReport]    Script Date: 04/01/2014 22:06:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_CollDataSumm_CollectionRunReport]
	@CollectionRunId int 
	,@cid int
	
AS
	Declare 
		@collRunReportId bigint
		,@collectionDate DateTime
		,@TotalMeterCount int
		,@metercointype1Count int
		,@metercointype2Count int
		,@metercointype3Count int
		,@metercointype4Count int
		,@metercointype5Count int
		,@metercointype6Count int
		,@metercointype7Count int
		,@metercointype8Count int
		,@TotalMeterCashAmt int
		,@TotalMeterCoinCount int
	
BEGIN

	
	
	if @CollectionRunId is null begin 
		Print '@CollectionRunId is null'
		return
	End
	
	SET NOCOUNT ON;	
	
	select @collectionDate = ActivationDate
			from CollectionRun
			where CollectionRunId = @CollectionRunId

	select @collRunReportId = CollectionRunReportId from CollectionRunReport
	if (@collRunReportId is null)
	begin
		Insert into CollectionRunReport
		(CustomerId,CollectionRunId,CollectionDate
		,TotalManualCashAmt, TotalManualCoinCount,TotalManualMeterCount
		,TotalMeterCount,TotalMeterCoinCount,TotalMeterCashAmt)
		values
		(@cid,@CollectionRunId,@collectionDate
		,0,0,0,0
		,0,0)
		
		set @collRunReportId = SCOPE_IDENTITY()
	end
	
	if (@collRunReportId is not null)
	begin
		select @TotalMeterCount = COUNT(distinct tx.MeterId)
		,@metercointype8Count =SUM(IsNull(CoinCent1,0))
		,@metercointype1Count = SUM(IsNull(CoinCent5,0))
		,@metercointype2Count = SUM(IsNull(CoinCent10,0))
		,@metercointype3Count = SUM(IsNull(CoinCent20,0))
		,@metercointype4Count = SUM(IsNull(CoinCent25,0))
		,@metercointype5Count = SUM(IsNull(CoinCent50,0))
		,@metercointype6Count = SUM(IsNull(CoinDollar1,0))
		,@metercointype7Count = SUM(IsNull(CoinDollar2,0))
		,@TotalMeterCashAmt = SUM(IsNull(AmountInCents,0))
		from TransactionsCash tx,		
		(select CustomerID,AreaId,MeterId,CollDateTime,InsertionDateTime from CollDataSumm where CollectionRunId = @CollectionRunId)coll
		where tx.CustomerId = coll.CustomerId
		and tx.AreaId = coll.AreaId
		and tx.MeterId = coll.MeterId
		and tx.TransDateTime between coll.InsertionDateTime and coll.CollDateTime		
		
		if (@TotalMeterCount is null) set @TotalMeterCount = 0
		
		set @TotalMeterCoinCount = @metercointype1Count +@metercointype2Count+@metercointype3Count+@metercointype4Count+@metercointype5Count+@metercointype6Count+@metercointype7Count+@metercointype8Count
		
		Update CollectionRunReport
		set TotalMeterCount = @TotalMeterCount
		,TotalMeterCoinCount = @TotalMeterCoinCount
		,TotalMeterCashAmt = @TotalMeterCashAmt
		,MeterCoinType1Count = @metercointype1Count
		,MeterCoinType2Count = @metercointype2Count
		,MeterCoinType3Count = @metercointype3Count
		,MeterCoinType4Count = @metercointype4Count
		,MeterCoinType5Count = @metercointype5Count
		,MeterCoinType6Count = @metercointype6Count
		,MeterCoinType7Count = @metercointype7Count
		,MeterCoinType8Count = @metercointype8Count
		where CollectionRunId = @CollectionRunId
	end
END
GO
/****** Object:  Table [dbo].[CollDataSumm]    Script Date: 04/01/2014 22:06:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollDataSumm](
	[GlobalMeterID] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[CollDateTime] [datetime] NOT NULL,
	[PaymentType] [int] NOT NULL,
	[Amount] [float] NOT NULL,
	[OldCashBoxID] [varchar](14) NULL,
	[NewCashBoxID] [varchar](14) NULL,
	[CashboxSequenceNo] [int] NULL,
	[InsertionDateTime] [datetime] NULL,
	[Processed] [bit] NULL,
	[EventUID] [bigint] NULL,
	[CreateTimestamp] [datetime] NULL,
	[CollectionRunId] [bigint] NULL,
	[CollectorId] [int] NULL,
 CONSTRAINT [PK_CollDataSumm] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[CollDateTime] ASC,
	[PaymentType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [CollDataSumm_IDXGlobalMeterID] ON [dbo].[CollDataSumm] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qCollReconDetCBRCOMMS_SubV235]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qCollReconDetCBRCOMMS_SubV235]
AS
SELECT     a.xFileProcessID, b.Amount AS AmtComms, a.CustomerId, a.AreaId, a.MeterId, a.DateTimeIns, a.CashBoxId, b.OldCashBoxID, a.CashboxSequenceNo, 
                      b.CashboxSequenceNo AS seqNumComms, a.DateTimeRem, b.CollDateTime, a.OperatorId, a.AmtAuto, a.AmtManual, a.AmtDiff, a.PercentFull, a.AvgL10,
                       a.StdDevL10
FROM         dbo.qCashBoxImport_subV235 a LEFT OUTER JOIN
                      dbo.CollDataSumm b ON a.CashboxSequenceNo = b.CashboxSequenceNo AND a.CustomerId = b.CustomerId AND a.AreaId = b.AreaId AND 
                      a.MeterId = b.MeterId AND CONVERT(datetime, DATEDIFF(week, 0, a.DateTimeRem)) = CONVERT(datetime, DATEDIFF(week, 0, b.CollDateTime))
GO
/****** Object:  Table [dbo].[TimeZones]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeZones](
	[TimeZoneID] [int] NOT NULL,
	[LocalTimeUTCDifference] [int] NOT NULL,
	[DaylightSavingAdjustment] [int] NOT NULL,
	[UTCSummerTimeStart] [smalldatetime] NOT NULL,
	[UTCSummerTimeEnd] [smalldatetime] NOT NULL,
	[TimeZoneName] [varchar](50) NULL,
 CONSTRAINT [PK_TimeZones] PRIMARY KEY CLUSTERED 
(
	[TimeZoneID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[udf_GetCustomerLocalTime]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[udf_GetCustomerLocalTime] 
(
	-- Add the parameters for the function here
	@CustomerId int
)
RETURNS DateTime
AS
BEGIN
-- Declare the return variable here
DECLARE @localTime Datetime;
DECLARE @offsetInMinutes int;
--get the current utc timespamt for the server, this will be our default local time
set @localTime = SYSUTCDATETIME();

--get the customers timezone offset
set @offsetInMinutes = (select tz.LocalTimeUTCDifference from TimeZones tz join Customers c on tz.TimeZoneID = c.TimeZoneID where c.CustomerID = @CustomerId);
If @offsetInMinutes is not Null
   Begin
     
--add the utc time and the customer timezone offset
SET @localTime = DATEADD(mi, @offsetInMinutes, @localTime)
--return the result of that
   End

return @localTime;
END
GO
/****** Object:  Table [dbo].[Zones]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Zones](
	[ZoneId] [int] NOT NULL,
	[customerID] [int] NOT NULL,
	[ZoneName] [varchar](50) NOT NULL,
	[ZoneStatus] [int] NULL,
	[RoadID] [int] NULL,
	[StreetName] [varchar](500) NULL,
	[BlockFrom] [varchar](500) NULL,
	[BlockTo] [varchar](500) NULL,
	[AreaId] [int] NULL,
	[SubArea] [int] NULL,
	[LengthOfZone] [int] NULL,
	[TCMinuteNumber] [int] NULL,
	[TCDate] [datetime] NULL,
	[InstallDate] [datetime] NULL,
	[DataWorks] [int] NULL,
	[SupresededDate] [datetime] NULL,
 CONSTRAINT [PK_Zones] PRIMARY KEY CLUSTERED 
(
	[ZoneId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_Zones] UNIQUE NONCLUSTERED 
(
	[ZoneId] ASC,
	[customerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomGroup1]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomGroup1](
	[CustomGroupId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_CustomGroup_1] PRIMARY KEY CLUSTERED 
(
	[CustomGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_CustomGroup_1] UNIQUE NONCLUSTERED 
(
	[CustomGroupId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Areas]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Areas](
	[GlobalAreaID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[AreaName] [varchar](25) NOT NULL,
	[Description] [varchar](50) NULL,
	[AreaState] [int] NULL,
 CONSTRAINT [PK_Areas] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [Areas_IDXGlobalAreaID] ON [dbo].[Areas] 
(
	[GlobalAreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TargetServiceDesignationMaster]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TargetServiceDesignationMaster](
	[TargetServiceDesignationId] [int] NOT NULL,
	[TargetServiceDesignationDesc] [varchar](250) NULL,
 CONSTRAINT [PK_TargetServiceDesignationMaster] PRIMARY KEY CLUSTERED 
(
	[TargetServiceDesignationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DemandZone]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DemandZone](
	[DemandZoneId] [int] NOT NULL,
	[DemandZoneDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_DemandZone] PRIMARY KEY CLUSTERED 
(
	[DemandZoneId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventSources]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventSources](
	[EventSourceCode] [int] NOT NULL,
	[EventSourceDesc] [varchar](25) NULL,
 CONSTRAINT [PK_EventSources] PRIMARY KEY CLUSTERED 
(
	[EventSourceCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlarmStatus]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AlarmStatus](
	[AlarmStatusId] [int] NOT NULL,
	[AlarmStatusDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_AlarmStatus] PRIMARY KEY CLUSTERED 
(
	[AlarmStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AlarmTier]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AlarmTier](
	[Tier] [int] NOT NULL,
	[TierDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_AlarmTier] PRIMARY KEY CLUSTERED 
(
	[Tier] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OperationalStatus]    Script Date: 04/01/2014 22:06:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OperationalStatus](
	[OperationalStatusId] [int] NOT NULL,
	[OperationalStatusDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_OperationalStatus] PRIMARY KEY CLUSTERED 
(
	[OperationalStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[pv_ActiveAlarms]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_ActiveAlarms]
AS
SELECT     aa.CustomerID, aa.TimeOfOccurrance, aa.AlarmUID, aa.EventCode AS AlarmCode, ec.EventDescVerbose AS AlarmCodeDesc, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, m.Location,
                          (SELECT     TierDesc
                            FROM          dbo.AlarmTier
                            WHERE      (Tier = ec.AlarmTier)) AS AlarmSeverity, CASE m.MeterGroup WHEN 0 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId = m.OperationalStatusID) WHEN 1 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId = m.OperationalStatusID) WHEN 10 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId =
                                                       (SELECT     OperationalStatus
                                                         FROM          Sensors
                                                         WHERE      SensorID = mm.SensorID)) WHEN 13 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId =
                                                       (SELECT     OperationalStatus
                                                         FROM          Gateways
                                                         WHERE      GateWayID = mm.GatewayID)) END AS AssetState,
                          (SELECT     AlarmStatusDesc
                            FROM          dbo.AlarmStatus
                            WHERE      (AlarmStatusId = 2)) AS AlarmStatus, CAST(DATEDIFF(mi,
                          (SELECT     dbo.udf_GetCustomerLocalTime(aa.CustomerID) AS Expr1), aa.SLADue) AS int) AS TotalMinutes,
                          (SELECT     AreaName
                            FROM          dbo.Areas
                            WHERE      (AreaID = mm.AreaId2) AND (CustomerID = aa.CustomerID)) AS Area, aa.AreaID, mm.AreaId2, z.ZoneName AS Zone, cg1.DisplayName AS Suburb, 
                      es.EventSourceDesc AS AlarmSourceDesc, es.EventSourceCode AS AlarmSourceCode,
                          (SELECT     TechnicianID
                            FROM          dbo.WorkOrder
                            WHERE      (WorkOrderId = aa.WorkOrderId)) AS TechnicianId, aa.TimeType1, aa.TimeType2, aa.TimeType3, aa.TimeType4, aa.TimeType5, aa.MeterId, 
                      CASE WHEN DATEDIFF(mi,
                          (SELECT     [dbo].[udf_GetCustomerLocalTime](aa.CustomerID)), aa.SLADue) <= 0 THEN
                          (SELECT     TargetServiceDesignationDesc
                            FROM          TargetServiceDesignationMaster
                            WHERE      TargetServiceDesignationId = 2) WHEN DATEDIFF(mi,
                          (SELECT     [dbo].[udf_GetCustomerLocalTime](aa.CustomerID)), aa.SLADue) BETWEEN 1 AND 60 THEN
                          (SELECT     TargetServiceDesignationDesc
                            FROM          TargetServiceDesignationMaster
                            WHERE      TargetServiceDesignationId = 3) WHEN DATEDIFF(mi,
                          (SELECT     [dbo].[udf_GetCustomerLocalTime](aa.CustomerID)), aa.SLADue) BETWEEN 61 AND 120 THEN
                          (SELECT     TargetServiceDesignationDesc
                            FROM          TargetServiceDesignationMaster
                            WHERE      TargetServiceDesignationId = 4) ELSE
                          (SELECT     TargetServiceDesignationDesc
                            FROM          TargetServiceDesignationMaster
                            WHERE      TargetServiceDesignationId = 1) END AS TargetService, NULL AS TimeOfClearance, dz.DemandZoneDesc AS DemandArea
FROM         dbo.ActiveAlarms AS aa INNER JOIN
                      dbo.EventCodes AS ec ON aa.CustomerID = ec.CustomerID AND aa.EventSource = ec.EventSource AND aa.EventCode = ec.EventCode INNER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = aa.CustomerID AND mm.Areaid = aa.AreaID AND mm.MeterId = aa.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON aa.CustomerID = m.CustomerID AND aa.AreaID = m.AreaID AND aa.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.EventSources AS es ON es.EventSourceCode = aa.EventSource LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[21] 4[7] 2[50] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "MeterMap"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 198
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ActiveAlarms"
		    Begin Extent = 
		       Top = 6
		       Left = 236
		       Bottom = 125
		       Right = 414
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "Meters"
		    Begin Extent = 
		       Top = 6
		       Left = 452
		       Bottom = 125
		       Right = 650
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "EventCodes"
		    Begin Extent = 
		       Top = 6
		       Left = 688
		       Bottom = 125
		       Right = 867
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 28
		 Width = 284
		 Width = 1500
		 Width = 2625
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 150' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_ActiveAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'0
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 2100
		 Table = 1620
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_ActiveAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_ActiveAlarms'
GO
/****** Object:  Table [dbo].[TargetServiceDesignation]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TargetServiceDesignation](
	[TargetServiceDesignationId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TargetServiceDesignationDesc] [varchar](250) NULL,
	[CustomerId] [int] NULL,
	[IsDisplay] [bit] NULL,
 CONSTRAINT [PK_TargetServiceDesignation] PRIMARY KEY CLUSTERED 
(
	[TargetServiceDesignationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[pv_HistoricAlarms]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_HistoricAlarms]
AS
SELECT     dbo.HistoricalAlarms.CustomerID, dbo.HistoricalAlarms.TimeOfOccurrance, dbo.HistoricalAlarms.AlarmUID, dbo.EventCodes.EventCode AS AlarmCode, 
                      dbo.EventCodes.EventDescVerbose AS AlarmCodeDesc,
                          (SELECT     MeterGroupDesc
                            FROM          dbo.AssetType
                            WHERE      (MeterGroupId = dbo.Meters.MeterGroup) AND (CustomerId = dbo.HistoricalAlarms.CustomerID)) AS AssetType, 
                      CASE Meters.MeterGroup WHEN 0 THEN Meters.MeterName WHEN 1 THEN Meters.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = MeterMap.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = MeterMap.GatewayID) END AS AssetName, dbo.Meters.Location,
                          (SELECT     TierDesc
                            FROM          dbo.AlarmTier
                            WHERE      (Tier = dbo.EventCodes.AlarmTier)) AS AlarmSeverity, CASE Meters.MeterGroup WHEN 0 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId = Meters.OperationalStatusID) WHEN 1 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId = Meters.OperationalStatusID) WHEN 10 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId =
                                                       (SELECT     OperationalStatus
                                                         FROM          Sensors
                                                         WHERE      SensorID = MeterMap.SensorID)) WHEN 13 THEN
                          (SELECT     OperationalStatusDesc
                            FROM          OperationalStatus
                            WHERE      OperationalStatusId =
                                                       (SELECT     OperationalStatus
                                                         FROM          Gateways
                                                         WHERE      GateWayID = MeterMap.GatewayID)) END AS AssetState,
                          (SELECT     AlarmStatusDesc
                            FROM          dbo.AlarmStatus
                            WHERE      (AlarmStatusId = 3)) AS AlarmStatus, 
                            CAST(DATEDIFF(mi, dbo.HistoricalAlarms.TimeOfOccurrance, dbo.HistoricalAlarms.TimeOfClearance) AS int) 
								AS TotalMinutes,
                          (SELECT     AreaName
                            FROM          dbo.Areas
                            WHERE      (AreaID = dbo.MeterMap.AreaId2) AND (CustomerID = dbo.HistoricalAlarms.CustomerID)) AS Area, dbo.Meters.AreaID, dbo.MeterMap.AreaId2,
                          (SELECT     ZoneName
                            FROM          dbo.Zones
                            WHERE      (ZoneId = dbo.MeterMap.ZoneId) AND (customerID = dbo.MeterMap.Customerid)) AS Zone,
                          (SELECT     DisplayName
                            FROM          dbo.CustomGroup1
                            WHERE      (dbo.MeterMap.CustomGroup1 = CustomGroupId)) AS Suburb,
                          (SELECT     EventSourceDesc
                            FROM          dbo.EventSources
                            WHERE      (EventSourceCode = dbo.EventCodes.EventSource)) AS AlarmSourceDesc,
                          (SELECT     EventSourceCode
                            FROM          dbo.EventSources AS EventSources_1
                            WHERE      (EventSourceCode = dbo.EventCodes.EventSource)) AS AlarmSourceCode,
                          (SELECT     TechnicianID
                            FROM          dbo.WorkOrder
                            WHERE      (WorkOrderId = dbo.HistoricalAlarms.WorkOrderId)) AS TechnicianId, 
							  dbo.HistoricalAlarms.TimeType1, 
							  dbo.HistoricalAlarms.TimeType2, 
							  dbo.HistoricalAlarms.TimeType3, 
							  dbo.HistoricalAlarms.TimeType4, 
							  dbo.HistoricalAlarms.TimeType5, 
							  dbo.HistoricalAlarms.MeterId,
                           CASE WHEN CAST(DATEDIFF(mi, dbo.HistoricalAlarms.TimeOfOccurrance, dbo.HistoricalAlarms.TimeOfClearance) AS bigint) 
                                 <= 0 THEN								-- Closed-Non-Compliant
								 (	SELECT     TargetServiceDesignationDesc
									FROM    dbo.TargetServiceDesignation
									WHERE   CustomerId = dbo.HistoricalAlarms.CustomerID
									AND TargetServiceDesignationId = 6)    
                            WHEN CAST(DATEDIFF(mi, dbo.HistoricalAlarms.TimeOfOccurrance, dbo.HistoricalAlarms.TimeOfClearance) AS bigint)
                                 > 0 THEN								 -- Closed-Compliant
								(	SELECT     TargetServiceDesignationDesc
									FROM    dbo.TargetServiceDesignation
									WHERE   CustomerId = dbo.HistoricalAlarms.CustomerID
									AND TargetServiceDesignationId = 5)    
									END AS TargetService, 
                      dbo.HistoricalAlarms.TimeOfClearance, 
                      dz.DemandZoneDesc AS DemandArea
FROM				  dbo.MeterMap INNER JOIN
                      dbo.HistoricalAlarms ON dbo.MeterMap.Customerid = dbo.HistoricalAlarms.CustomerID INNER JOIN
                      dbo.Meters ON dbo.MeterMap.Customerid = dbo.Meters.CustomerID AND dbo.MeterMap.Areaid = dbo.Meters.AreaID AND 
                      dbo.MeterMap.MeterId = dbo.Meters.MeterId AND dbo.HistoricalAlarms.CustomerID = dbo.Meters.CustomerID AND 
                      dbo.HistoricalAlarms.AreaID = dbo.Meters.AreaID AND dbo.HistoricalAlarms.MeterId = dbo.Meters.MeterId INNER JOIN
                      dbo.EventCodes ON dbo.HistoricalAlarms.CustomerID = dbo.EventCodes.CustomerID AND dbo.HistoricalAlarms.EventSource = dbo.EventCodes.EventSource AND 
                      dbo.HistoricalAlarms.EventCode = dbo.EventCodes.EventCode LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = dbo.Meters.DemandZone
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
		  Begin PaneConfiguration = 0
			 NumPanes = 4
			 Configuration = "(H (1[18] 4[31] 2[31] 3) )"
		  End
		  Begin PaneConfiguration = 1
			 NumPanes = 3
			 Configuration = "(H (1 [50] 4 [25] 3))"
		  End
		  Begin PaneConfiguration = 2
			 NumPanes = 3
			 Configuration = "(H (1 [50] 2 [25] 3))"
		  End
		  Begin PaneConfiguration = 3
			 NumPanes = 3
			 Configuration = "(H (4 [30] 2 [40] 3))"
		  End
		  Begin PaneConfiguration = 4
			 NumPanes = 2
			 Configuration = "(H (1 [56] 3))"
		  End
		  Begin PaneConfiguration = 5
			 NumPanes = 2
			 Configuration = "(H (2 [66] 3))"
		  End
		  Begin PaneConfiguration = 6
			 NumPanes = 2
			 Configuration = "(H (4 [50] 3))"
		  End
		  Begin PaneConfiguration = 7
			 NumPanes = 1
			 Configuration = "(V (3))"
		  End
		  Begin PaneConfiguration = 8
			 NumPanes = 3
			 Configuration = "(H (1[56] 4[18] 2) )"
		  End
		  Begin PaneConfiguration = 9
			 NumPanes = 2
			 Configuration = "(H (1 [75] 4))"
		  End
		  Begin PaneConfiguration = 10
			 NumPanes = 2
			 Configuration = "(H (1[66] 2) )"
		  End
		  Begin PaneConfiguration = 11
			 NumPanes = 2
			 Configuration = "(H (4 [60] 2))"
		  End
		  Begin PaneConfiguration = 12
			 NumPanes = 1
			 Configuration = "(H (1) )"
		  End
		  Begin PaneConfiguration = 13
			 NumPanes = 1
			 Configuration = "(V (4))"
		  End
		  Begin PaneConfiguration = 14
			 NumPanes = 1
			 Configuration = "(V (2))"
		  End
		  ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
		  Begin Origin = 
			 Top = -288
			 Left = 0
		  End
		  Begin Tables = 
			 Begin Table = "MeterMap"
				Begin Extent = 
				   Top = 6
				   Left = 38
				   Bottom = 125
				   Right = 198
				End
				DisplayFlags = 280
				TopColumn = 0
			 End
			 Begin Table = "HistoricalAlarms"
				Begin Extent = 
				   Top = 6
				   Left = 236
				   Bottom = 125
				   Right = 448
				End
				DisplayFlags = 280
				TopColumn = 0
			 End
			 Begin Table = "Meters"
				Begin Extent = 
				   Top = 6
				   Left = 486
				   Bottom = 125
				   Right = 684
				End
				DisplayFlags = 280
				TopColumn = 0
			 End
			 Begin Table = "EventCodes"
				Begin Extent = 
				   Top = 6
				   Left = 722
				   Bottom = 125
				   Right = 901
				End
				DisplayFlags = 280
				TopColumn = 0
			 End
		  End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
		  Begin ParameterDefaults = ""
		  End
		  Begin ColumnWidths = 28
			 Width = 284
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Width = 1500
			 Wid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_HistoricAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'th = 1500
			 Width = 1500
			 Width = 1500
		  End
	   End
	   Begin CriteriaPane = 
		  Begin ColumnWidths = 11
			 Column = 1440
			 Alias = 900
			 Table = 1170
			 Output = 720
			 Append = 1400
			 NewValue = 1170
			 SortType = 1350
			 SortOrder = 1410
			 GroupBy = 1350
			 Filter = 1350
			 Or = 1350
			 Or = 1350
			 Or = 1350
		  End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_HistoricAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_HistoricAlarms'
GO
/****** Object:  View [dbo].[pv_CombineActiveHistoricalAlarms]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_CombineActiveHistoricalAlarms]
		AS
		Select * From pv_ActiveAlarms
		UNION All
		Select * From pv_HistoricAlarms
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
		Begin DesignProperties = 
		   Begin PaneConfigurations = 
			  Begin PaneConfiguration = 0
				 NumPanes = 4
				 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
			  End
			  Begin PaneConfiguration = 1
				 NumPanes = 3
				 Configuration = "(H (1 [50] 4 [25] 3))"
			  End
			  Begin PaneConfiguration = 2
				 NumPanes = 3
				 Configuration = "(H (1 [50] 2 [25] 3))"
			  End
			  Begin PaneConfiguration = 3
				 NumPanes = 3
				 Configuration = "(H (4 [30] 2 [40] 3))"
			  End
			  Begin PaneConfiguration = 4
				 NumPanes = 2
				 Configuration = "(H (1 [56] 3))"
			  End
			  Begin PaneConfiguration = 5
				 NumPanes = 2
				 Configuration = "(H (2 [66] 3))"
			  End
			  Begin PaneConfiguration = 6
				 NumPanes = 2
				 Configuration = "(H (4 [50] 3))"
			  End
			  Begin PaneConfiguration = 7
				 NumPanes = 1
				 Configuration = "(V (3))"
			  End
			  Begin PaneConfiguration = 8
				 NumPanes = 3
				 Configuration = "(H (1[56] 4[18] 2) )"
			  End
			  Begin PaneConfiguration = 9
				 NumPanes = 2
				 Configuration = "(H (1 [75] 4))"
			  End
			  Begin PaneConfiguration = 10
				 NumPanes = 2
				 Configuration = "(H (1[66] 2) )"
			  End
			  Begin PaneConfiguration = 11
				 NumPanes = 2
				 Configuration = "(H (4 [60] 2))"
			  End
			  Begin PaneConfiguration = 12
				 NumPanes = 1
				 Configuration = "(H (1) )"
			  End
			  Begin PaneConfiguration = 13
				 NumPanes = 1
				 Configuration = "(V (4))"
			  End
			  Begin PaneConfiguration = 14
				 NumPanes = 1
				 Configuration = "(V (2))"
			  End
			  ActivePaneConfig = 0
		   End
		   Begin DiagramPane = 
			  Begin Origin = 
				 Top = 0
				 Left = 0
			  End
			  Begin Tables = 
				 Begin Table = "pv_ActiveAlarms"
					Begin Extent = 
					   Top = 6
					   Left = 38
					   Bottom = 125
					   Right = 216
					End
					DisplayFlags = 280
					TopColumn = 0
				 End
			  End
		   End
		   Begin SQLPane = 
		   End
		   Begin DataPane = 
			  Begin ParameterDefaults = ""
			  End
		   End
		   Begin CriteriaPane = 
			  Begin ColumnWidths = 11
				 Column = 1440
				 Alias = 900
				 Table = 1170
				 Output = 720
				 Append = 1400
				 NewValue = 1170
				 SortType = 1350
				 SortOrder = 1410
				 GroupBy = 1350
				 Filter = 1350
				 Or = 1350
				 Or = 1350
				 Or = 1350
			  End
		   End
		End
		' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_CombineActiveHistoricalAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_CombineActiveHistoricalAlarms'
GO
/****** Object:  Table [dbo].[TechnicianDetails]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TechnicianDetails](
	[TechnicianId] [int] NOT NULL,
	[Name] [varchar](40) NULL,
	[Contact] [varchar](20) NULL,
	[TechnicianKeyID] [int] NULL,
 CONSTRAINT [PK_TechnicianDetails] PRIMARY KEY CLUSTERED 
(
	[TechnicianId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventType]    Script Date: 04/01/2014 22:06:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventType](
	[EventTypeId] [int] NOT NULL,
	[EventTypeDesc] [varchar](50) NULL,
 CONSTRAINT [PK_EventType] PRIMARY KEY CLUSTERED 
(
	[EventTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[pv_EventsHistoricalAlarms]    Script Date: 04/01/2014 22:06:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsHistoricalAlarms]
AS
SELECT     aa.CustomerID, aa.TimeOfOccurrance AS DateTime, aa.EventUID, aa.MeterId AS AssetId,
                          (SELECT     MeterGroupDesc
                            FROM          dbo.AssetType AS at
                            WHERE      (CustomerId = aa.CustomerID) AND (MeterGroupId = m.MeterGroup)) AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, aa.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, aa.EventCode, mm.AreaId2, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, aa.TimeType1, aa.TimeType2, aa.TimeType3, aa.TimeType4, 
                      aa.TimeType5, ec.EventDescVerbose AS AlarmDescription, almt.TierDesc AS AlarmSeverity, mme.MaintenanceCode AS ResolutionCode,
                          (SELECT     EventSourceDesc
                            FROM          dbo.EventSources
                            WHERE      (EventSourceCode = ec.EventSource)) AS Source,
                          (SELECT     Name
                            FROM          dbo.TechnicianDetails
                            WHERE      (TechnicianId =
                                                       (SELECT     TechnicianID
                                                         FROM          dbo.WorkOrder
                                                         WHERE      (WorkOrderId = aa.WorkOrderId)))) AS Technician, aa.SLADue AS TimeDueSLA, CAST(DATEDIFF(mi, aa.TimeOfOccurrance, 
                      aa.TimeOfClearance) AS int) AS TotalMinutes, aa.TimeOfNotification AS TimeNotified, aa.WorkOrderId, aa.TimeOfClearance AS TimeCleared, CAST('1' AS bit) 
                      AS IsClosed, z.ZoneName AS Zone, a.AreaName AS Area
FROM         dbo.HistoricalAlarms AS aa INNER JOIN
                      dbo.EventCodes AS ec ON aa.CustomerID = ec.CustomerID AND aa.EventSource = ec.EventSource AND aa.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = aa.CustomerID AND mm.Areaid = aa.AreaID AND mm.MeterId = aa.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON aa.CustomerID = m.CustomerID AND aa.AreaID = m.AreaID AND aa.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.AlarmTier AS almt ON almt.Tier = ec.AlarmTier LEFT OUTER JOIN
                      dbo.SFMeterMaintenanceEvent AS mme ON mme.WorkOrderID = aa.WorkOrderId LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "aa"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 250
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 288
		       Bottom = 125
		       Right = 448
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 486
		       Bottom = 125
		       Right = 654
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 692
		       Bottom = 125
		       Right = 927
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 965
		       Bottom = 125
		       Right = 1144
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 6
		       Left = 1182
		       Bottom = 125
		       Right = 1346
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 215
		       Right = 213
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 En' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsHistoricalAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'd
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 126
		       Left = 251
		       Bottom = 215
		       Right = 415
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "almt"
		    Begin Extent = 
		       Top = 126
		       Left = 453
		       Bottom = 215
		       Right = 613
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mme"
		    Begin Extent = 
		       Top = 126
		       Left = 651
		       Bottom = 245
		       Right = 826
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 29
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsHistoricalAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsHistoricalAlarms'
GO
/****** Object:  View [dbo].[pv_EventsActiveAlarms]    Script Date: 04/01/2014 22:06:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsActiveAlarms]
AS
SELECT     aa.CustomerID, aa.TimeOfOccurrance AS DateTime, aa.EventUID, aa.MeterId AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, aa.EventCode, 
                      mm.AreaId2 AS AreaID, mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, aa.TimeType1, aa.TimeType2, 
                      aa.TimeType3, aa.TimeType4, aa.TimeType5, ec.EventDescVerbose AS AlarmDescription, almt.TierDesc AS AlarmSeverity, NULL AS ResolutionCode,
                          (SELECT     EventSourceDesc
                            FROM          dbo.EventSources
                            WHERE      (EventSourceCode = ec.EventSource)) AS Source,
                          (SELECT     Name
                            FROM          dbo.TechnicianDetails
                            WHERE      (TechnicianId =
                                                       (SELECT     TechnicianID
                                                         FROM          dbo.WorkOrder
                                                         WHERE      (WorkOrderId = aa.WorkOrderId)))) AS Technician, aa.SLADue AS TimeDueSLA, CAST(DATEDIFF(mi,
                          (SELECT     dbo.udf_GetCustomerLocalTime(aa.CustomerID) AS Expr1), aa.SLADue) AS int) AS TotalMinutes, aa.TimeOfNotification AS TimeNotified, 
                      aa.WorkOrderId, NULL AS TimeCleared, CAST(0 AS bit) AS IsClosed, z.ZoneName AS Zone, a.AreaName AS Area
FROM         dbo.ActiveAlarms AS aa INNER JOIN
                      dbo.EventCodes AS ec ON aa.CustomerID = ec.CustomerID AND aa.EventSource = ec.EventSource AND aa.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = aa.CustomerID AND mm.Areaid = aa.AreaID AND mm.MeterId = aa.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON mm.Customerid = m.CustomerID AND mm.Areaid = m.AreaID AND mm.MeterId = m.MeterId AND aa.CustomerID = m.CustomerID AND 
                      aa.AreaID = m.AreaID AND aa.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.CustomerId = aa.CustomerID AND at.MeterGroupId = m.MeterGroup LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.Areaid AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.AlarmTier AS almt ON almt.Tier = ec.AlarmTier
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "aa"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 216
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 254
		       Bottom = 125
		       Right = 414
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 452
		       Bottom = 125
		       Right = 620
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 658
		       Bottom = 125
		       Right = 893
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 931
		       Bottom = 125
		       Right = 1110
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 6
		       Left = 1148
		       Bottom = 125
		       Right = 1427
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "a"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 245
		       Right = 198
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsActiveAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
		 Begin Table = "z"
		    Begin Extent = 
		       Top = 126
		       Left = 236
		       Bottom = 245
		       Right = 407
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 126
		       Left = 445
		       Bottom = 245
		       Right = 609
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 126
		       Left = 647
		       Bottom = 215
		       Right = 822
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 126
		       Left = 860
		       Bottom = 215
		       Right = 1024
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "almt"
		    Begin Extent = 
		       Top = 126
		       Left = 1062
		       Bottom = 215
		       Right = 1222
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsActiveAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsActiveAlarms'
GO
/****** Object:  View [dbo].[pv_EventsAllAlarms]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsAllAlarms]
		AS
		SELECT
		[CustomerID]
		,[DateTime]
		,[EventUID]
		,[AssetId]
		,[AssetType]
		,[AssetName]
		,[AlarmCode]
		,[EventClass]
		,[EventCode]
		,[AreaId2]
		,[ZoneId]
		,[Street]
		,[Suburb]
		,[DemandArea]
		,[TimeType1]
		,[TimeType2]
		,[TimeType3]
		,[TimeType4]
		,[TimeType5]
		,[AlarmDescription]
		,[AlarmSeverity]
		,[ResolutionCode]
		,[Source]
		,[Technician]
		,[TimeDueSLA]
		,[TotalMinutes]
		,[TimeNotified]
		,[WorkOrderId]
		,[TimeCleared]
		,[IsClosed]
		,[Zone]
		,[Area]
		FROM  pv_EventsHistoricalAlarms
		UNION ALL
		SELECT
		[CustomerID]
		,[DateTime]
		,[EventUID]
		,[AssetId]
		,[AssetType]
		,[AssetName]
		,[AlarmCode]
		,[EventClass]
		,[EventCode]
		,[AreaID]
		,[ZoneId]
		,[Street]
		,[Suburb]
		,[DemandArea]
		,[TimeType1]
		,[TimeType2]
		,[TimeType3]
		,[TimeType4]
		,[TimeType5]
		,[AlarmDescription]
		,[AlarmSeverity]
		,[ResolutionCode]
		,[Source]
		,[Technician]
		,[TimeDueSLA]
		,[TotalMinutes]
		,[TimeNotified]
		,[WorkOrderId]
		,[TimeCleared]
		,[IsClosed]
		,[Zone]
		,[Area]
		 FROM pv_EventsActiveAlarms
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsAllAlarms'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsAllAlarms'
GO
/****** Object:  Table [dbo].[GIS]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GIS](
	[Meter ID] [float] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[GetUserName]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[GetUserName]
    @UserName varchar(128) OUTPUT
As
    Select @UserName = USER_NAME()
return
GO
/****** Object:  Table [dbo].[GSMConnectionStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GSMConnectionStatus](
	[StatusID] [int] NOT NULL,
	[StatusName] [varchar](65) NULL,
 CONSTRAINT [PK_GSMConnectionStatus] PRIMARY KEY NONCLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[InactiveRemarks]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[InactiveRemarks](
	[InactiveRemarkID] [int] NOT NULL,
	[Description] [varchar](30) NOT NULL,
 CONSTRAINT [PK_InactiveRemarks] PRIMARY KEY CLUSTERED 
(
	[InactiveRemarkID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ImportDirectories]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ImportDirectories](
	[IncomingDir] [varchar](256) NOT NULL,
	[OutgoingDir] [varchar](256) NULL,
	[CustomerID] [int] NOT NULL,
	[PollTime] [int] NULL,
 CONSTRAINT [PK_ImportDirectories] PRIMARY KEY NONCLUSTERED 
(
	[CustomerID] ASC,
	[IncomingDir] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ID_GEN]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ID_GEN](
	[GEN_KEY] [varchar](10) NOT NULL,
	[GEN_VALUE] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[GEN_KEY] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HousingTypes]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HousingTypes](
	[HousingTypeID] [int] NOT NULL,
	[Description] [varchar](30) NOT NULL,
 CONSTRAINT [PK_HousingTypes] PRIMARY KEY CLUSTERED 
(
	[HousingTypeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MechHistory]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MechHistory](
	[MechId] [int] NOT NULL,
	[HousingId] [int] NULL,
	[Status] [varchar](30) NOT NULL,
	[HistoryDate] [datetime] NOT NULL,
	[Notes] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HousingAudit]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HousingAudit](
	[HousingId] [int] NOT NULL,
	[HousingName] [varchar](20) NOT NULL,
	[Customerid] [int] NOT NULL,
	[Block] [varchar](25) NOT NULL,
	[StreetName] [varchar](50) NOT NULL,
	[StreetType] [varchar](20) NOT NULL,
	[StreetDirection] [varchar](10) NOT NULL,
	[StreetNotes] [varchar](50) NULL,
	[HousingTypeID] [int] NULL,
	[DoorLockId] [varchar](15) NULL,
	[MechLockId] [varchar](15) NULL,
	[IsActive] [bit] NOT NULL,
	[InactiveRemarkID] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[Notes] [varchar](50) NULL,
	[AuditTS] [datetime] NULL,
	[UserId] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LocationTierMaster]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LocationTierMaster](
	[LocationTierId] [int] NOT NULL,
	[LocationTierName] [varchar](255) NOT NULL,
 CONSTRAINT [PK_LocationTierMaster] PRIMARY KEY CLUSTERED 
(
	[LocationTierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[LocationTier]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LocationTier](
	[CustomerID] [int] NOT NULL,
	[LocationTierId] [int] NOT NULL,
	[LocationTierName] [varchar](250) NOT NULL,
	[IsDisplay] [bit] NOT NULL,
 CONSTRAINT [PK_LocationTier] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[LocationTierId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HolidayRate]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HolidayRate](
	[HolidayRateId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[HolidayName] [varchar](255) NOT NULL,
	[HolidayDateTime] [datetime] NOT NULL,
	[RateScheduleConfigurationId] [bigint] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
 CONSTRAINT [PK_HolidayRate] PRIMARY KEY CLUSTERED 
(
	[HolidayRateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MaintRoute]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MaintRoute](
	[MaintRouteId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_MaintRoute] PRIMARY KEY CLUSTERED 
(
	[MaintRouteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_MaintRoute] UNIQUE NONCLUSTERED 
(
	[MaintRouteId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MagneticFlux]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MagneticFlux](
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[NodeID] [int] NOT NULL,
	[StatusTime] [datetime] NOT NULL,
	[FluxValue] [int] NOT NULL,
 CONSTRAINT [PK_MagneticFlux] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterID] ASC,
	[NodeID] ASC,
	[StatusTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[metermap_bk]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[metermap_bk](
	[Customerid] [int] NOT NULL,
	[Areaid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ZoneId] [int] NULL,
	[HousingId] [int] NOT NULL,
	[MechId] [int] NULL,
	[AreaId2] [int] NULL,
	[CollRouteId] [int] NULL,
	[EnfRouteId] [int] NULL,
	[MaintRouteId] [int] NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
	[CustomGroup1] [int] NULL,
	[CustomGroup2] [int] NULL,
	[CustomGroup3] [int] NULL,
	[SubAreaID] [int] NULL,
	[GatewayID] [int] NULL,
	[SensorID] [int] NULL,
	[CashBoxID] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterDiagnosticType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterDiagnosticType](
	[ID] [int] NOT NULL,
	[DiagnosticDesc] [varchar](100) NOT NULL,
 CONSTRAINT [PK_MeterDiagnosticType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PPOStatusCodes]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PPOStatusCodes](
	[PPOStatusCode] [int] NOT NULL,
	[PPOStatusDesc] [varchar](48) NOT NULL,
	[PPOStatusType] [char](1) NOT NULL,
	[PPOStatusShortDesc] [varchar](4) NOT NULL,
 CONSTRAINT [PK_PPOImport] PRIMARY KEY CLUSTERED 
(
	[PPOStatusCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RejectedDiscount]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RejectedDiscount](
	[ID ] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [smallint] NULL,
	[AreaId] [tinyint] NULL,
	[MeterId] [int] NULL,
	[TransDateTime] [datetime] NULL,
	[ReceiptNo] [int] NULL,
	[AmountInCents] [int] NULL,
	[BayNumber] [int] NULL,
	[CardNumHash] [varchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID ] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReinoParameters]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReinoParameters](
	[ParameterID] [char](20) NOT NULL,
	[Description] [varchar](100) NULL,
	[ParameterValue] [varchar](128) NOT NULL,
 CONSTRAINT [PK_ReinoParameters] PRIMARY KEY CLUSTERED 
(
	[ParameterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RegulatedStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RegulatedStatus](
	[RegulatedStatusID] [int] NOT NULL,
	[RegulatedStatusDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_RegulatedStatus] PRIMARY KEY CLUSTERED 
(
	[RegulatedStatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SensorEvents]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SensorEvents](
	[SensorEventId] [int] NOT NULL,
	[SensorDesc] [varchar](30) NOT NULL,
 CONSTRAINT [PK_SensorEvents] PRIMARY KEY CLUSTERED 
(
	[SensorEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RateTransmission]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateTransmission](
	[TransmissionID] [bigint] NOT NULL,
	[ImplementationDate] [datetime] NOT NULL,
	[ReceivedTS] [datetime] NOT NULL,
	[ChangeType] [int] NULL,
	[Emailed] [datetime] NULL,
	[SuccessTs] [datetime] NULL,
	[SlaAckTs] [datetime] NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_RateTransmission] PRIMARY KEY CLUSTERED 
(
	[TransmissionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SFMeterMap]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFMeterMap](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [smallint] NOT NULL,
	[AreaId] [tinyint] NOT NULL,
	[MeterId] [smallint] NOT NULL,
	[BayNum] [int] NULL,
	[SFId] [varchar](20) NOT NULL,
	[MultiSpace] [bit] NOT NULL,
	[SpaceNum] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [SFMeterMap_IDX_MCB] ON [dbo].[SFMeterMap] 
(
	[CustomerId] ASC,
	[MeterId] ASC,
	[BayNum] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SFPriceSchedule]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFPriceSchedule](
	[Id] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Feed] [varchar](8000) NOT NULL,
	[Received] [datetime] NOT NULL,
	[Processed] [datetime] NOT NULL,
 CONSTRAINT [PK_SFPriceSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFOperatingScheduleType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFOperatingScheduleType](
	[Id] [int] NOT NULL,
	[TypeDesc] [varchar](20) NOT NULL,
 CONSTRAINT [PK_SFOperatingScheduleType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFNotification]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFNotification](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FileName] [varchar](100) NOT NULL,
	[Message] [varchar](500) NOT NULL,
	[Received] [datetime] NOT NULL,
	[Sent] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFMeterStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFMeterStatus](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [smallint] NOT NULL,
	[AreaId] [tinyint] NOT NULL,
	[MeterId] [smallint] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [SFMeterStatus_IDX_ICAMEE] ON [dbo].[SFMeterStatus] 
(
	[ID] ASC,
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[EventCode] ASC,
	[EventDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RipnetProperties]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RipnetProperties](
	[KeyText] [varchar](50) NOT NULL,
	[ValueText] [varchar](500) NOT NULL,
 CONSTRAINT [PK_RipnetProperties] PRIMARY KEY CLUSTERED 
(
	[KeyText] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFEventType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFEventType](
	[Id] [int] NOT NULL,
	[EventType] [varchar](20) NOT NULL,
	[Verbose] [varchar](50) NULL,
 CONSTRAINT [PK_SFEventType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFCollectionRoute]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFCollectionRoute](
	[CustomerId] [smallint] NOT NULL,
	[AreaId] [tinyint] NOT NULL,
	[MeterId] [smallint] NOT NULL,
	[CollectionRoute] [varchar](20) NOT NULL,
	[CollectionSubRoute] [varchar](30) NOT NULL,
	[LocationId] [varchar](10) NOT NULL,
	[CVSStartDate] [datetime] NOT NULL,
 CONSTRAINT [PK_SFCollectionRoute] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentType](
	[PaymentType] [int] NOT NULL,
	[PaymentTypeDesc] [varchar](24) NOT NULL,
 CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED 
(
	[PaymentType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentTargetType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PaymentTargetType](
	[TargetType] [char](1) NOT NULL,
	[Description] [varchar](15) NOT NULL,
 CONSTRAINT [PK_PaymentTargetType] PRIMARY KEY CLUSTERED 
(
	[TargetType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PayByCellVendor]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PayByCellVendor](
	[VendorID] [int] NOT NULL,
	[VendorName] [varchar](200) NULL,
	[CustomerID] [smallint] NULL,
UNIQUE NONCLUSTERED 
(
	[VendorID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [VendorCustomerid] ON [dbo].[PayByCellVendor] 
(
	[VendorName] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PayByCellAudit]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PayByCellAudit](
	[PayByCellAuditId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AuditDateTime] [datetime] NOT NULL,
	[AuditStart] [datetime] NOT NULL,
	[AuditEnd] [datetime] NOT NULL,
	[VendorTxCount] [int] NOT NULL,
	[VendorTotalAmountCent] [int] NOT NULL,
	[DuncanTxCount] [int] NOT NULL,
	[DuncanTotalAmountCent] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[VendorId] [int] NULL,
 CONSTRAINT [PK_PayByCellAudit] PRIMARY KEY CLUSTERED 
(
	[PayByCellAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PSSImport]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PSSImport](
	[CustomerID] [smallint] NULL,
	[AreaID] [smallint] NULL,
	[MeterID] [smallint] NULL,
	[MeterName] [varchar](50) NULL,
	[Location] [varchar](50) NULL,
	[TimeZone] [tinyint] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PropertyGroup]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PropertyGroup](
	[PropertyGroupId] [int] NOT NULL,
	[PropertyGroupDesc] [varchar](500) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PropertyGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ProcessType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ProcessType](
	[ProcessType] [char](2) NOT NULL,
	[ProcessTypeDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_ProcessType] PRIMARY KEY CLUSTERED 
(
	[ProcessType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Meters_CurrentState]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Meters_CurrentState](
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[LastGSMOK] [datetime] NULL,
	[LastGSMFailed] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkeonAlarm]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkeonAlarm](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Park] [varchar](50) NOT NULL,
	[Meter] [varchar](50) NOT NULL,
	[StatusId] [int] NOT NULL,
	[Status] [varchar](100) NOT NULL,
	[OccurredDate] [datetime] NOT NULL,
	[ReceivedDate] [datetime] NOT NULL,
	[WorkOrderId] [varchar](50) NULL,
 CONSTRAINT [PK_ParkeonAlarm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParkVehicle]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkVehicle](
	[VehicleID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[LPNumber] [varchar](20) NOT NULL,
	[LPLocation] [varchar](20) NOT NULL,
	[VMake] [varchar](20) NULL,
	[VModel] [varchar](20) NULL,
	[VColour] [varchar](20) NULL,
 CONSTRAINT [PK_Parkvehicle] PRIMARY KEY CLUSTERED 
(
	[VehicleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParkingSpotStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpotStatus](
	[id] [uniqueidentifier] NOT NULL,
	[SequenceNumber] [int] NULL,
	[MeterMappingId] [int] NOT NULL,
	[Status] [nvarchar](50) NULL,
	[EventTime] [datetime] NULL,
	[LastModifiedDateTime] [datetime] NULL,
	[CustomerStatus] [nvarchar](50) NULL,
	[CustomerObjectId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OperationMode]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OperationMode](
	[OperationModeId] [int] NOT NULL,
	[OperationModeDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_OperationMode] PRIMARY KEY CLUSTERED 
(
	[OperationModeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OccupancyStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OccupancyStatus](
	[StatusID] [int] NOT NULL,
	[StatusDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_OccupancyStatus] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PAMGracePeriod]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAMGracePeriod](
	[CustomerId] [int] NOT NULL,
	[ClusterId] [int] NOT NULL,
	[GracePeriod] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OLTCardHash]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OLTCardHash](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[InsDate] [datetime] NOT NULL,
	[Fdigit] [int] NOT NULL,
	[Ldigit] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_OLTCardHash] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NSCStreets]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NSCStreets](
	[street names] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NSCMeterList]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NSCMeterList](
	[Asset ID] [float] NULL,
	[Asset Type] [nvarchar](255) NULL,
	[Asset Name] [nvarchar](255) NULL,
	[Asset Model] [nvarchar](255) NULL,
	[Street] [nvarchar](255) NULL,
	[Area Nos] [float] NULL,
	[Street ID] [float] NULL,
	[Suburb] [nvarchar](255) NULL,
	[Zone ID] [float] NULL,
	[Latitude ] [nvarchar](255) NULL,
	[Longitude] [nvarchar](255) NULL,
	[Space Count] [float] NULL,
	[Demand Status] [nvarchar](255) NULL,
	[Phone Number] [float] NULL,
	[Operational Status] [nvarchar](255) NULL,
	[Area Nos1] [float] NULL,
	[Meter Number] [float] NULL,
	[F18] [float] NULL,
	[Street Area Number] [float] NULL,
	[F20] [nvarchar](255) NULL,
	[No# of Bays] [float] NULL,
	[F22] [float] NULL,
	[Street Name] [nvarchar](255) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationTiers]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NotificationTiers](
	[TierID] [int] NOT NULL,
	[TierName] [varchar](50) NULL,
 CONSTRAINT [PK_NotificationTiers] PRIMARY KEY CLUSTERED 
(
	[TierID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NotificationReceipients]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NotificationReceipients](
	[UserID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[EmailGroupID] [int] NULL,
	[SendVerbose] [bit] NULL,
	[EmailAddress] [varchar](128) NULL,
	[SortOrder] [int] NULL,
	[UserName] [varchar](64) NULL,
	[Description] [varchar](128) NULL,
 CONSTRAINT [PK_NotificationReceipients] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[NotificationMeterSchedule]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationMeterSchedule](
	[ScheduleID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[TierID] [int] NOT NULL,
	[StartTime] [smalldatetime] NULL,
	[EndTime] [smalldatetime] NULL,
	[EmailGroupID] [int] NULL,
 CONSTRAINT [PK_NotificationMeterSchedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NotificationAudit]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NotificationAudit](
	[ID] [int] NULL,
	[ReceipientUserId] [int] NULL,
	[AlarmUID] [int] NULL,
	[NotifiedDate] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[NonCompliantStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[NonCompliantStatus](
	[NonCompliantStatusID] [int] NOT NULL,
	[NonCompliantStatusDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_NonCompliantStatus] PRIMARY KEY CLUSTERED 
(
	[NonCompliantStatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MParkImport]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MParkImport](
	[DRecType] [int] NULL,
	[TransID] [varchar](16) NOT NULL,
	[TransAmtInCents] [int] NULL,
	[NumOfMins] [int] NULL,
	[TransTimeStamp] [datetime] NOT NULL,
	[ReinoCustomerID] [int] NULL,
	[ReinoAreaID] [int] NULL,
	[ReinoMeterID] [int] NULL,
	[BayNum] [int] NULL,
	[UserType] [int] NULL,
	[CallerId] [varchar](16) NULL,
	[BatchIdent] [varchar](16) NULL,
	[BatchDate] [datetime] NULL,
	[BatchFileName] [varchar](24) NULL,
 CONSTRAINT [PK_MParkImport] PRIMARY KEY CLUSTERED 
(
	[TransID] ASC,
	[TransTimeStamp] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MeterPushSchedule]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterPushSchedule](
	[PamPushId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[ExpiryTime] [datetime] NOT NULL,
	[CreatedTime] [datetime] NOT NULL,
	[LastPushedTime] [datetime] NULL,
	[Acknowledged] [datetime] NULL,
	[CancelDate] [datetime] NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_MeterPushSchedule] PRIMARY KEY CLUSTERED 
(
	[PamPushId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterServiceStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterServiceStatus](
	[StatusID] [int] NOT NULL,
	[StatusDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_MeterServiceStatus] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BlackListArchive]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BlackListArchive](
	[XCardNum] [varchar](160) NOT NULL,
	[Code] [int] NULL,
	[Action] [bit] NULL,
	[DateTimeAction] [datetime] NOT NULL,
	[UserID] [int] NULL,
 CONSTRAINT [PK_BlackListArchive] PRIMARY KEY NONCLUSTERED 
(
	[XCardNum] ASC,
	[DateTimeAction] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Banks]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Banks](
	[BankID] [int] NOT NULL,
	[Name] [varchar](40) NOT NULL,
	[ToEmailAddress] [varchar](128) NOT NULL,
	[UserID] [varchar](80) NOT NULL,
 CONSTRAINT [PK_Banks] PRIMARY KEY CLUSTERED 
(
	[BankID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AccountStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AccountStatus](
	[AccountStatusId] [int] NOT NULL,
	[AccountStatusDesc] [varchar](100) NULL,
 CONSTRAINT [PK_AccountStatus] PRIMARY KEY CLUSTERED 
(
	[AccountStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AcquirerIF]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AcquirerIF](
	[AcquirerIFId] [int] NOT NULL,
	[AcquirerIFDesc] [varchar](255) NOT NULL,
 CONSTRAINT [PK_AcquirerIF] PRIMARY KEY CLUSTERED 
(
	[AcquirerIFId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AssetState]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetState](
	[AssetStateId] [int] NOT NULL,
	[AssetStateDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_AssetState] PRIMARY KEY CLUSTERED 
(
	[AssetStateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AssetPendingReason]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetPendingReason](
	[AssetPendingReasonId] [int] NOT NULL,
	[AssetPendingReasonDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_AssetPendingReason] PRIMARY KEY CLUSTERED 
(
	[AssetPendingReasonId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CashBoxLocationType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CashBoxLocationType](
	[CashBoxLocationTypeId] [int] NOT NULL,
	[CashBoxLocationTypeDesc] [varchar](255) NOT NULL,
 CONSTRAINT [PK_CashBoxLocationType] PRIMARY KEY CLUSTERED 
(
	[CashBoxLocationTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CoinDenomination]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CoinDenomination](
	[CoinDenominationId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CountryCode] [varchar](3) NOT NULL,
	[CoinValue] [int] NOT NULL,
	[CoinName] [varchar](255) NULL,
	[TransactionsCashMap] [varchar](255) NULL,
	[CashBoxDataImportMap] [varchar](255) NULL,
 CONSTRAINT [PK_CoinDenomination] PRIMARY KEY CLUSTERED 
(
	[CoinDenominationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CardType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CardType](
	[CardTypeCode] [int] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_CardType] PRIMARY KEY CLUSTERED 
(
	[CardTypeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomersAudit]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomersAudit](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[FromEmailAddress] [varchar](128) NOT NULL,
	[BankHostAddr] [varchar](128) NULL,
	[BankHostPort] [int] NULL,
	[BankMaxThreads] [int] NULL,
	[BankServiceProvider] [int] NULL,
	[BankStoreID] [varchar](255) NULL,
	[ResubmitProfileID] [int] NULL,
	[BlackListCC] [bit] NULL,
	[DoesCreditCashKeys] [bit] NULL,
	[UnReconcileCleanupLag] [int] NULL,
	[DoesBlacklistViaFD] [bit] NULL,
	[TxTiming] [int] NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[Longitude] [float] NULL,
	[Status] [int] NULL,
	[CreateDateTime] [datetime] NULL,
	[City] [varchar](255) NULL,
	[Latitude] [float] NULL,
 CONSTRAINT [PK_CustomersAudit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountSchemeType](
	[DiscountSchemeTypeId] [int] NOT NULL,
	[DiscountSchemeTypeDesc] [varchar](250) NOT NULL,
 CONSTRAINT [PK_DiscountSchemeType] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountSchemeStatus](
	[DiscountSchemeStatusId] [int] NOT NULL,
	[DiscountSchemeStatusDesc] [varchar](100) NULL,
 CONSTRAINT [PK_DiscountSchemeStatus] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeExpirationType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountSchemeExpirationType](
	[DiscountSchemeExpirationTypeId] [int] NOT NULL,
	[DiscountSchemeExpirationTypeDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_DiscountSchemeExpirationTypeId] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeExpirationTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmailTemplateType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailTemplateType](
	[EmailTemplateTypeId] [int] NOT NULL,
	[EmailTemplateTypeDesc] [varchar](200) NOT NULL,
 CONSTRAINT [PK_EmailTemplateType] PRIMARY KEY CLUSTERED 
(
	[EmailTemplateTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EmailGroup]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmailGroup](
	[EmailGroupId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GroupName] [varchar](250) NOT NULL,
	[CustomerId] [int] NULL,
	[DateCreated] [datetime] NULL,
 CONSTRAINT [PK_EmailGroup] PRIMARY KEY CLUSTERED 
(
	[EmailGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventCodesAudit]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventCodesAudit](
	[AuditID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[EventCode] [int] NOT NULL,
	[AlarmTier] [int] NOT NULL,
	[EventDescAbbrev] [varchar](16) NULL,
	[EventDescVerbose] [varchar](50) NULL,
	[SLAMinutes] [int] NULL,
	[IsAlarm] [bit] NULL,
	[EventType] [int] NULL,
	[UserId] [int] NOT NULL,
	[UpdatedDateTime] [datetime] NOT NULL,
	[ApplySLA] [bit] NULL,
	[EventCategory] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FDFileType]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FDFileType](
	[FileType] [int] NOT NULL,
	[FileDesc] [varchar](50) NOT NULL,
	[FileExtension] [varchar](10) NOT NULL,
 CONSTRAINT [PK_FDFileType] PRIMARY KEY CLUSTERED 
(
	[FileType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FDJobStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FDJobStatus](
	[JobStatus] [int] NOT NULL,
	[StatusDesc] [varchar](60) NOT NULL,
 CONSTRAINT [PK_FDJobStatus] PRIMARY KEY CLUSTERED 
(
	[JobStatus] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[GatewayResp]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GatewayResp](
	[ResponseType] [varchar](8) NOT NULL,
	[Description] [varchar](15) NULL,
 CONSTRAINT [PK_GatewayResp] PRIMARY KEY CLUSTERED 
(
	[ResponseType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventCategory]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventCategory](
	[EventCategoryId] [int] NOT NULL,
	[EventCategoryDesc] [varchar](50) NULL,
 CONSTRAINT [PK_EventCategory] PRIMARY KEY CLUSTERED 
(
	[EventCategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventState]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventState](
	[EventStateId] [int] NOT NULL,
	[EventStateDesc] [varchar](255) NOT NULL,
 CONSTRAINT [PK_EventState] PRIMARY KEY CLUSTERED 
(
	[EventStateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CurrentParkingSpotStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CurrentParkingSpotStatus](
	[MeterMappingId] [int] NOT NULL,
	[CurrentStatus] [nvarchar](50) NOT NULL,
	[LastChangeTime] [datetime] NULL,
	[PreviousValidStatus] [nvarchar](50) NULL,
	[Remarks] [nvarchar](50) NULL,
	[LastModifiedDateTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MeterMappingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CreditCardTypes]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CreditCardTypes](
	[CreditCardType] [int] NOT NULL,
	[Name] [varchar](40) NOT NULL,
 CONSTRAINT [PK_CreditCardTypes] PRIMARY KEY CLUSTERED 
(
	[CreditCardType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CollectionRunStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollectionRunStatus](
	[CollectionRunStatusId] [int] NOT NULL,
	[CollectionRunStatusDesc] [varchar](100) NULL,
 CONSTRAINT [PK_CollectionRunStatus] PRIMARY KEY CLUSTERED 
(
	[CollectionRunStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConfigProfile]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ConfigProfile](
	[ConfigProfileId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ConfigurationName] [varchar](255) NOT NULL,
	[Version6] [varchar](255) NULL,
	[TariffPolicyName] [varchar](255) NULL,
	[Minunte15FreeParking] [bit] NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedBy] [int] NULL,
 CONSTRAINT [PK_ConfigProfile] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ConfigurationIDGen]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationIDGen](
	[ConfigurationID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GenDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ConfigurationIDGen] PRIMARY KEY CLUSTERED 
(
	[ConfigurationID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigStatus]    Script Date: 04/01/2014 22:06:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ConfigStatus](
	[ConfigStatusId] [int] NOT NULL,
	[ConfigStatusDest] [varchar](50) NULL,
 CONSTRAINT [PK_ConfigStatus] PRIMARY KEY CLUSTERED 
(
	[ConfigStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_GetActiveAlarms]    Script Date: 04/01/2014 22:06:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetActiveAlarms]
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
/****** Object:  StoredProcedure [dbo].[sp_GetEventsItems]    Script Date: 04/01/2014 22:06:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetEventsItems]
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
/****** Object:  StoredProcedure [dbo].[sp_GetCustomerTransactions]    Script Date: 04/01/2014 22:06:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetCustomerTransactions]
	@orderBy nvarchar(max),
	@PageNumber int,
	@PageSize  int,
	@CustomerId nvarchar(200),
	@AssetType nvarchar(max) = '',
	@AssetId nvarchar(max)= '',
	@AssetName nvarchar(max)= '',
	@TimeType nvarchar(max)= '',
	@Area nvarchar(max)= '',
	@Zone nvarchar(max)= '',
	@DemandArea nvarchar(max)= '',
	@Street nvarchar(max)= '',
	@Suburb nvarchar(max)= '',	
	@BayId nvarchar(max)= '',
	@DiscountSchemaId nvarchar(max)= '',
	@StartDate nvarchar(max)= '', --required
	@EndDate nvarchar(max)= '',--required
	@PaymentStatusType nvarchar(max)= '',
	@TransactionType nvarchar(max)= '',
	@CardNumHash nvarchar(max)= '',
	@CardType nvarchar(max)= '',
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
If LEN(@AssetId) > 0 Set @WhereClause = @WhereClause + ' And AssetId LIKE '''+ '%' + @AssetId + '%' + '''' ;
If LEN(@AssetName) > 0 Set @WhereClause = @WhereClause + ' And AssetName LIKE '''+ '%' + @AssetName + '%' + '''' ;
If LEN(@Area) > 0 Set @WhereClause = @WhereClause + ' And Area LIKE '''+ '%' + @Area + '%' + '''' ;
If LEN(@Zone) > 0 Set @WhereClause = @WhereClause + ' And Zone LIKE '''+ '%' + @Zone + '%' + '''' ;
If LEN(@BayId) > 0 Set @WhereClause = @WhereClause + ' And BayId = ''' + @BayId + ''' ';
If LEN(@DiscountSchemaId) > 0 Set @WhereClause = @WhereClause + ' And DiscountSchemaId = ''' + @DiscountSchemaId + ''' ';
If LEN(@Street) > 0 Set @WhereClause = @WhereClause + ' And Street LIKE '''+ '%' + @Street + '%' + '''' ;
If LEN(@Suburb) > 0 Set @WhereClause = @WhereClause + ' And Suburb LIKE '''+ '%' + @Suburb + '%' + '''' ;
If LEN(@DemandArea) > 0 Set @WhereClause = @WhereClause + ' And DemandArea = ''' + @DemandArea + ''' ';
If (LEN(@StartDate) > 0 AND LEN(@EndDate) > 0) Set @WhereClause = @WhereClause + ' And (DateTime BETWEEN '''+@StartDate+''' AND '''+@EndDate+''')  '
If LEN(@TimeType) > 0  Set @WhereClause = @WhereClause + ' And (TimeType1=' + @TimeType + ' Or TimeType2=' + @TimeType + ' Or TimeType3=' + @TimeType + ' Or TimeType4=' + @TimeType + ' Or TimeType5=' + @TimeType + ') ';
If LEN(@CardNumHash) > 0 Set @WhereClause = @WhereClause + ' And CardNumHash = ''' + @CardNumHash + ''' ';
If LEN(@CardType) > 0 Set @WhereClause = @WhereClause + ' And CardType = ''' + @CardType + ''' ';
If LEN(@TransactionType) > 0 Set @WhereClause = @WhereClause + ' And TransactionType = ''' + @TransactionType + ''' ';
If LEN(@PaymentStatusType) > 0 Set @WhereClause = @WhereClause + ' And PaymentStatusType = ''' + @PaymentStatusType + ''' ';

         
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
/****** Object:  StoredProcedure [dbo].[sp_GetOccupancy]    Script Date: 04/01/2014 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_GetOccupancy] 
		@orderBy nvarchar(max),
		@PageNumber int,
		@PageSize  int,
		@CustomerId nvarchar(200),
		@assetType nvarchar(max) = '',
		@assetName nvarchar(max) = '',
		@assetId nvarchar(max)= '',
		@occupancyStatus nvarchar(max)= '',
		@operationalStatus nvarchar(max)= '',
		@nonCompliantStatus nvarchar(max)= '',
		@timeType nvarchar(max)= '',
		@street nvarchar(max)='',
		@zone nvarchar(max)= '',
		@suburb nvarchar(max)= '',
		@area nvarchar(max)= '',
		@DemandArea nvarchar(max)= '',
		@StartDate nvarchar(max)= '', --required
		@EndDate nvarchar(max)= ''--required
	AS
	BEGIN

	DECLARE @StartNumber nvarchar(200), @EndNumber nvarchar(200);
	SET @StartNumber = ( (@PageNumber - 1) * @PageSize ) ;
	SET @EndNumber = ( (@PageNumber) * @PageSize ) ;

	--build the where clause here
	Declare @WhereClause as Nvarchar(max);

	set @WhereClause = ' Where CustomerID  = ''' + @CustomerId + ''' ';
	If LEN(@assetType) > 0 Set @WhereClause = @WhereClause + ' And AssetType = ''' + @assetType + ''' ';
	If LEN(@occupancyStatus) > 0 Set @WhereClause = @WhereClause + ' And OccupancyStatus = ''' + @occupancyStatus + ''' ';
	If LEN(@operationalStatus) > 0 Set @WhereClause = @WhereClause + ' And OperationalStatus = ''' + @operationalStatus + ''' ';
	If LEN(@nonCompliantStatus) > 0 Set @WhereClause = @WhereClause + ' And NonCompliantStatus = ''' + @nonCompliantStatus + ''' ';
	If ( LEN(@StartDate) > 0 AND LEN(@EndDate) > 0) Set @WhereClause = @WhereClause + ' And (ArrivalTime BETWEEN '''+@StartDate+''' AND '''+@EndDate+''')  '
	If  LEN(@assetId) > 0 Set @WhereClause = @WhereClause + ' And AssetId='+ @assetId ;
	If  LEN(@timeType) > 0  Set @WhereClause = @WhereClause + ' And (TimeType1=' + @timeType + ' Or TimeType2=' + @timeType + ' Or TimeType3=' + @timeType + ' Or TimeType4=' + @timeType + ' Or TimeType5=' + @timeType + ') ';
	If  LEN(@assetName) > 0 Set @WhereClause = @WhereClause + ' And AssetName LIKE '''+ '%' + @assetName + '%' + '''' ;
	If  LEN(@zone) > 0 Set @WhereClause = @WhereClause + ' And Zone  LIKE '''+ '%' + @zone + '%' + '''' ;
	If  LEN(@suburb) > 0 Set @WhereClause = @WhereClause + ' And Suburb  LIKE '''+ '%' + @suburb + '%' + '''' ;
	If  LEN(@area) > 0 Set @WhereClause = @WhereClause + ' And Area  LIKE '''+ '%' + @area + '%' + '''' ;
	If  LEN(@DemandArea) > 0 Set @WhereClause = @WhereClause + ' And DemandArea  LIKE '''+ '%' + @DemandArea + '%' + '''' ;
	If  LEN(@street) > 0 Set @WhereClause = @WhereClause + ' And Street  LIKE '''+ '%' + @street + '%' + '''' ;


	declare @totalCount Nvarchar(max);
	declare @totalTable AS TABLE (col int);  
	DECLARE @CountQuery AS NVARCHAR(MAX);
	set @CountQuery = 'SELECT Count(*) FROM [pv_Occupancy] ' + @WhereClause
	 INSERT into @totalTable EXECUTE sp_executesql @CountQuery;
	set @totalCount = (select top 1 * from @totalTable);
	--build the order by clause
	Declare @OrderByClause as Nvarchar(max);
	set @OrderByClause = @orderBy;
	
	

	--build the select query - 
	DECLARE @SelectQuery AS NVARCHAR(MAX);
	set @SelectQuery = 'Select * from (SELECT *, '+@totalCount+' as Count, ROW_NUMBER() OVER (ORDER BY '+@OrderByClause+' ) as RowNumber FROM [pv_Occupancy]   '+ @WhereClause+'  ) Seq Where ( Seq.RowNumber BETWEEN '+@StartNumber+' AND '+@EndNumber+' )';
	
	print @SelectQuery
	
	EXECUTE sp_executesql @SelectQuery;
	--print @SelectQuery;

	END
GO
/****** Object:  Table [dbo].[SmartCardSerials]    Script Date: 04/01/2014 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SmartCardSerials](
	[SerialNumber] [int] NOT NULL,
	[CardTypeID] [int] NOT NULL,
	[RegionCode] [int] NOT NULL,
	[CustomerID] [int] NULL,
	[CreatedTimeStamp] [datetime] NULL,
	[CreatedByUser] [varchar](80) NULL,
	[ExpirationDate] [datetime] NULL,
	[TechnicianId] [int] NULL,
 CONSTRAINT [PK_SmartCardSerials] PRIMARY KEY CLUSTERED 
(
	[SerialNumber] ASC,
	[RegionCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFTransmission]    Script Date: 04/01/2014 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFTransmission](
	[TransId] [bigint] IDENTITY(1,1) NOT NULL,
	[TransDate] [datetime] NULL,
	[ChildId] [bigint] NOT NULL,
	[type] [smallint] NOT NULL,
	[status] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[TransId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [SFTransmission_IDX_CS] ON [dbo].[SFTransmission] 
(
	[ChildId] ASC,
	[status] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SFTCCAudit]    Script Date: 04/01/2014 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFTCCAudit](
	[TCCID] [bigint] NOT NULL,
	[CSVDate] [datetime] NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[BatchStart] [datetime] NOT NULL,
	[BatchEnd] [datetime] NOT NULL,
 CONSTRAINT [PK_SFTCCAudit] PRIMARY KEY CLUSTERED 
(
	[TCCID] ASC,
	[CSVDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SFScheduleType]    Script Date: 04/01/2014 22:06:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFScheduleType](
	[Id] [int] NOT NULL,
	[TypeDesc] [varchar](20) NOT NULL,
 CONSTRAINT [PK_SFScheduleType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_Update_Pending]    Script Date: 04/01/2014 22:06:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Update_Pending]
As
Begin
	Print ''
end
GO
/****** Object:  StoredProcedure [dbo].[spDailyGPRSTotals]    Script Date: 04/01/2014 22:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spDailyGPRSTotals] 
	
	@custId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @datenow datetime;
	set @datenow = getdate();

	select CardType, sum(convert(money,AmountPaid)) as Total
	from dbo.[dailyGPRSRevenue](@custId,@datenow)
	group by CardType
END
GO
/****** Object:  StoredProcedure [dbo].[spDailyGPRSRevenue]    Script Date: 04/01/2014 22:06:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spDailyGPRSRevenue] 
	-- Add the parameters for the stored procedure here
	@custId int
AS
BEGIN
	declare @datenow datetime;
	set @datenow = getdate();
	-- Add the SELECT statement with parameter references here
	select * from dbo.[dailyGPRSRevenue](@custId,@datenow)
	
END
GO
/****** Object:  Table [dbo].[SpaceType]    Script Date: 04/01/2014 22:06:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SpaceType](
	[SpaceTypeId] [int] NOT NULL,
	[SpaceTypeDesc] [varchar](50) NULL,
 CONSTRAINT [PK_SpaceType] PRIMARY KEY CLUSTERED 
(
	[SpaceTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_tableGen]    Script Date: 04/01/2014 22:06:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_tableGen]
--INPUT THE QUERY FOR THE CURSOR
@cursorString VARCHAR(1000) = ' SELECT CustomerID, CompanyName  FROM CUSTOMERS',
--INPUT ANY TABLE ATTRIBUTES REQUIRED
@tableAttributes  VARCHAR(500) = '   border=5 width=100% cellspacing=1 cellpadding=1  '
AS
DECLARE @column_name  VARCHAR(100)
DECLARE @column_num INT
DECLARE @column_count INT
DECLARE @declaration_String  VARCHAR(1000)
DECLARE  @cursor_variables  VARCHAR(1000)
DECLARE @html_cursor_variables VARCHAR(1000)
-------INITIALIZE VARIABLES TO ZERO LENGTH STRING---------------
 SELECT @declaration_String  = ' '
 SELECT @cursor_variables  = ' '
 SELECT @html_cursor_variables  = ''
------------------DECLARE THE CURSOR USING THE SELECT STATEMENT THAT USER HAS ENTERED-----------------------
EXEC ('DECLARE tabCur CURSOR FOR  '+ @cursorString)
-------------------CREATE THE TEMPORARY TABLE TO BE USED FOR HOLDING THE HTML TABLE
CREATE TABLE #webTable
(  htmlString VARCHAR(2000))
-------------------CREATE A TEMPORARY TABLE TO HOLD THE COLUMN NAMES----------------------------------
CREATE TABLE #cols
(ColNum  INT  IDENTITY (1, 1) NOT NULL ,
ColName VARCHAR(100)
)
---------------INSERT HTML TO BEGIN A TABLE--------------------------------------------------------------------
INSERT INTO  #webTable ( htmlString) VALUES ('<TABLE' + @tableAttributes + ' >')

-------------CREATE A CURSOR FOR THE SELECT STATEMENT'S COLUMN NAMES-----------------
DECLARE colCur  CURSOR FOR SELECT A.column_name, A.ordinal_position, B.column_count
FROM  master.dbo.syscursorcolumns A,  master.dbo.syscursors B
WHERE A.cursor_handle = B.cursor_handle
AND  B.cursor_name  = 'tabCur'
ORDER BY ordinal_position

----BEGIN  THE HEADER  ROW
INSERT INTO  #webTable ( htmlString) VALUES ('<TR>')
-----------OPEN AND FETCH THE CURSOR OF COLUMN NAMES----------------------------------------------------------
OPEN  colCur
FETCH NEXT FROM colCur  INTO  @column_name, @column_num,  @column_count
-------CREATE THE SQL STATEMENTS BASED ON METADATA
WHILE (@@FETCH_STATUS = 0)
BEGIN
	---INSERT COLUMN NAMES INTO THE cols TABLE
	INSERT INTO  #cols( ColName) VALUES ( @column_name)
--------ADD THE COLUMN HEADER NAMES USING THE CURSOR OF COLUMN NAMES --------------------------------------------------
	INSERT INTO  #webTable ( htmlString) VALUES ('<TD>'+  @column_name + '</TD>' )
	 SELECT @declaration_String  =   @declaration_String  + '  DECLARE @' + @column_name +  '  VARCHAR(100)   ' + CHAR(13)

----------------BUILD THE STRINGS FOR DYNAMIC SQL BASED ON COLUMN'S POSITION
	IF ( @column_num +1)  <  @column_count
	BEGIN
-----------------CREATE A STRING OF  VARIABLES TO USE IN DYNAMIC SQL STATEMENT TO LOOP THROUGH  CURSOR
		 SELECT @cursor_variables  =  @cursor_variables + ' @' + @column_name + ', '
-----------------CREATE A STRING OF ONE ROW IN THE HTML TABLE----------------------------------------------------------------------------------
		SELECT @html_cursor_variables =  @html_cursor_variables    +    '   '' <TD>''   + @'+     @column_name    +     '  +   '' </TD>'' +    '
	END
	ELSE
		BEGIN
-----------------CREATE A STRING OF  VARIABLES TO USE IN DYNAMIC SQL STATEMENT TO LOOP THROUGH  CURSOR
		SELECT @cursor_variables  =  @cursor_variables + ' @' + @column_name
-----------------CREATE A STRING OF ONE ROW IN THE HTML TABLE----------------------------------------------------------------------------------
		SELECT @html_cursor_variables =  @html_cursor_variables    +    '   '' <TD>''   + @'+     @column_name    +     '  +   '' </TD>''    '
		END
	FETCH NEXT FROM colCur  INTO  @column_name, @column_num,  @column_count

END

-----------------END  THE HEADER  ROW------------------------------
INSERT INTO  #webTable ( htmlString) VALUES ('</TR>')
--------------OPEN THE CURSOR FOR QUERY ENTERED BY USER AND GENERATE TABLE TAGS-------------------------
OPEN  tabCur
declare @curStr VARCHAR(2000)
--USE DYNAMIC SQL TO DECLARE VARIABLES AND SCROLL THROUGH CURSOR
SELECT  @curStr  =   @declaration_String + CHAR(13) +
'  FETCH NEXT FROM  tabCur INTO '+ @cursor_variables + CHAR(13) +
'  WHILE (@@FETCH_STATUS= 0) '+CHAR(13) +
	'  BEGIN  ' +CHAR(13) +
	--BEGIN ROW
	'INSERT INTO  #webTable ( htmlString) VALUES (''<TR>'')  ' + CHAR(13) +
	    --ADD THE ROW OF COLUMN NAMES THAT HAVE BEEN ASSEMBLED TOGETHER IN A STRING
	     'INSERT INTO  #webTable ( htmlString) VALUES ( '+ @html_cursor_variables +') '+
	--END ROW
	'INSERT INTO  #webTable ( htmlString) VALUES (''</TR>'')  ' +CHAR(13) +
	' FETCH NEXT FROM  tabCur INTO  '+ @cursor_variables +CHAR(13) +
' END'
EXEC  ( @curStr)
--CLOSING CODE FOR THE   HTML TABLE
INSERT INTO  #webTable ( htmlString) VALUES ('</TABLE>')
------CLOSE  AND DEALLOCATE  ALL CURSORS
CLOSE  tabCur
DEALLOCATE  tabCur
CLOSE colCur
DEALLOCATE  colCur
----OUTPUT THE RECORDSET OF HTML TABLE  FROM THE TEMPORARY TABLE
SELECT  htmlString  FROM  #webTable
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getMaintenanceDowntime]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getMaintenanceDowntime] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int
	,@SLAMinute int OUTPUT
AS
Declare 
	@MeterGroup int	
	
BEGIN
	print 'start'	
END
GO
/****** Object:  Table [dbo].[StreetlineSpace]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StreetlineSpace](
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayId] [int] NOT NULL,
	[SpaceId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CurrentSeqNum] [int] NOT NULL,
 CONSTRAINT [PK_StreetlineSpace] PRIMARY KEY CLUSTERED 
(
	[SpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StreetlineEvent]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StreetlineEvent](
	[EventId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransactionId] [bigint] NOT NULL,
	[TypeId] [int] NULL,
	[SpaceId] [int] NOT NULL,
	[SequenceNum] [int] NOT NULL,
	[PaymentStartTime] [datetime] NOT NULL,
	[PaymentExpiryTime] [datetime] NOT NULL,
	[ResponseTime] [datetime] NULL,
	[Amount] [int] NULL,
	[CustomerId] [int] NOT NULL,
	[attempt] [int] NOT NULL,
 CONSTRAINT [PK_StreetlineEvent_1] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TimeUnit]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TimeUnit](
	[TimeUnitId] [int] NOT NULL,
	[TimeUnitDesc] [varchar](50) NULL,
 CONSTRAINT [PK_TimeUnit] PRIMARY KEY CLUSTERED 
(
	[TimeUnitId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TccDeclinedBuffer]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TccDeclinedBuffer](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[TccId] [bigint] NOT NULL,
	[HCardNum] [varchar](50) NOT NULL,
	[XCardNum] [varchar](50) NOT NULL,
	[DeclinedTS] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffState]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffState](
	[TariffStateId] [int] NOT NULL,
	[TariffStateDesc] [varchar](25) NOT NULL,
 CONSTRAINT [PK_TariffState] PRIMARY KEY CLUSTERED 
(
	[TariffStateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Version]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Version](
	[DBVersionNumber] [varchar](100) NOT NULL,
	[CHANGE_DATE] [datetime] NOT NULL,
	[ChangeLog] [varchar](200) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 04/01/2014 22:06:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserLName] [varchar](50) NOT NULL,
	[GroupName] [varchar](64) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_Log]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_Log]
	@msg varchar(2000)
AS
BEGIN
	insert into UdpSqlLogId (logmsg) values (@msg)
END
GO
/****** Object:  Table [dbo].[VersionMaster]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VersionMaster](
	[VersionID] [float] NOT NULL,
	[VersionDesc] [varchar](255) NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_VersionMaster] PRIMARY KEY CLUSTERED 
(
	[VersionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VersionGroup]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VersionGroup](
	[VersionGroupId] [int] NOT NULL,
	[VersionGroupDesc] [varchar](255) NOT NULL,
 CONSTRAINT [PK_VersionGroup] PRIMARY KEY CLUSTERED 
(
	[VersionGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[webpages_Roles]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Roles](
	[RoleId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[RoleName] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_OAuthMembership]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_OAuthMembership](
	[Provider] [nvarchar](30) NOT NULL,
	[ProviderUserId] [nvarchar](100) NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Provider] ASC,
	[ProviderUserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_Membership]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_Membership](
	[UserId] [int] NOT NULL,
	[CreateDate] [datetime] NULL,
	[ConfirmationToken] [nvarchar](128) NULL,
	[IsConfirmed] [bit] NULL,
	[LastPasswordFailureDate] [datetime] NULL,
	[PasswordFailuresSinceLastSuccess] [int] NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[PasswordChangedDate] [datetime] NULL,
	[PasswordSalt] [nvarchar](128) NOT NULL,
	[PasswordVerificationToken] [nvarchar](128) NULL,
	[PasswordVerificationTokenExpirationDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionType]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionType](
	[TransactionTypeId] [int] NOT NULL,
	[TransactionTypeDesc] [varchar](50) NOT NULL,
 CONSTRAINT [PK_TransactionType] PRIMARY KEY CLUSTERED 
(
	[TransactionTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionStatus]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionStatus](
	[StatusID] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
 CONSTRAINT [PK_TransactionStatus] PRIMARY KEY CLUSTERED 
(
	[StatusID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[txbackup]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[txbackup](
	[TransactionsID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[AreaID] [int] NULL,
	[MeterID] [int] NULL,
	[ZoneID] [int] NULL,
	[ParkingSpaceID] [bigint] NULL,
	[SensorID] [int] NULL,
	[TransactionType] [int] NULL,
	[TransDateTime] [datetime] NULL,
	[ExpiryTime] [datetime] NULL,
	[AmountInCents] [int] NULL,
	[TxValue] [varchar](50) NULL,
	[TimePaid] [int] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[EventUID] [bigint] NULL,
	[BayNumber] [int] NULL,
	[CreditCardType] [int] NULL,
	[TransactionStatus] [int] NULL,
	[ReceiptNo] [int] NULL,
	[OriginalTxId] [bigint] NULL,
	[TransType] [int] NULL,
	[MeterGroupId] [int] NULL,
	[GatewayId] [int] NULL,
	[SensorPaymentTransactionId] [bigint] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[DiscountSchemeId] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransUserType]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransUserType](
	[UserType] [int] NOT NULL,
	[Description] [varchar](15) NOT NULL,
 CONSTRAINT [PK_TransUserType] PRIMARY KEY CLUSTERED 
(
	[UserType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransType]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransType](
	[TransTypeId] [int] NOT NULL,
	[TransTypeDesc] [varchar](255) NULL,
 CONSTRAINT [PK_TransType] PRIMARY KEY CLUSTERED 
(
	[TransTypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[UserTypes]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[UserTypes](
	[UserType] [char](1) NOT NULL,
	[Description] [varchar](30) NOT NULL,
 CONSTRAINT [PK_UserTypes] PRIMARY KEY CLUSTERED 
(
	[UserType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[trace]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[trace](
	[RowNumber] [int] IDENTITY(0,1) NOT FOR REPLICATION NOT NULL,
	[EventClass] [int] NULL,
	[TextData] [ntext] NULL,
	[ApplicationName] [nvarchar](128) NULL,
	[NTUserName] [nvarchar](128) NULL,
	[LoginName] [nvarchar](128) NULL,
	[CPU] [int] NULL,
	[Reads] [bigint] NULL,
	[Writes] [bigint] NULL,
	[Duration] [bigint] NULL,
	[ClientProcessID] [int] NULL,
	[SPID] [int] NULL,
	[StartTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[BinaryData] [image] NULL,
PRIMARY KEY CLUSTERED 
(
	[RowNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionPackageStatus]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionPackageStatus](
	[StatusId] [int] NOT NULL,
	[Description] [varchar](20) NOT NULL,
 CONSTRAINT [PK_TransactionPackageStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrderStatus]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderStatus](
	[WorkOrderStatusId] [int] NOT NULL,
	[WorkORderStatusDesc] [varchar](500) NOT NULL,
 CONSTRAINT [PK_WorkOrderStatus] PRIMARY KEY CLUSTERED 
(
	[WorkOrderStatusId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WhilteListFileStaging]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WhilteListFileStaging](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[CardHash] [varchar](200) NOT NULL,
	[DiscountSchemeId] [int] NOT NULL,
	[DiscountPercentage] [int] NOT NULL,
	[DiscountMinute] [int] NOT NULL,
	[MaxAmountInCents] [int] NOT NULL,
	[ValidDate] [datetime] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WhilteListFile]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WhilteListFile](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[CardHash] [varchar](200) NOT NULL,
	[DiscountSchemeId] [int] NOT NULL,
	[DiscountPercentage] [int] NOT NULL,
	[DiscountMinute] [int] NOT NULL,
	[MaxAmountInCents] [int] NOT NULL,
	[Delta] [varchar](10) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrders]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrders](
	[WorkOrderId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ReportingUserId] [int] NULL,
	[MeterGroup] [int] NULL,
	[CustomerId] [int] NULL,
	[AreaId] [int] NULL,
	[MeterId] [int] NULL,
	[Location] [varchar](500) NULL,
	[ParkingSpaceId] [bigint] NULL,
	[Notes] [varchar](2000) NULL,
	[ZoneId] [int] NULL,
	[HighestSeverity] [int] NULL,
	[CreateDateTime] [datetime] NULL,
	[SLADue] [datetime] NULL,
	[Status] [int] NULL,
	[TechnicianId] [int] NULL,
	[AssignedDate] [datetime] NULL,
	[CompletionDate] [datetime] NULL,
	[CreatedById] [int] NULL,
	[CrossStreet] [varchar](2000) NULL,
	[ResolutionCode] [int] NULL,
	[ResolutionDesc] [varchar](2000) NULL,
	[AssignmentState] [int] NULL,
	[Mechanism] [int] NULL,
 CONSTRAINT [PK_WorkOrders] PRIMARY KEY CLUSTERED 
(
	[WorkOrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionsPending]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionsPending](
	[TransPendingID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransactionID] [bigint] NULL,
	[AttemptCount] [int] NULL,
	[Status] [int] NULL,
	[CardTypeCode] [int] NOT NULL,
 CONSTRAINT [PK_TransactionsPending] PRIMARY KEY CLUSTERED 
(
	[TransPendingID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] NOT NULL,
	[UserFName] [varchar](50) NOT NULL,
	[UserLName] [varchar](50) NULL,
	[UserPassword] [varchar](100) NOT NULL,
	[UserType] [char](1) NOT NULL,
	[DefaultCustomerID] [int] NOT NULL,
	[Role] [varchar](20) NULL,
	[PasswordHash] [varchar](40) NULL,
	[ActivationDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[MultipleLogins] [bit] NULL,
	[Enabled] [bit] NULL,
	[UserAddress] [varchar](500) NULL,
	[UserEmail] [varchar](500) NULL,
	[RegisteredTS] [datetime] NULL,
	[Address2] [varchar](255) NULL,
	[City] [varchar](255) NULL,
	[AddressState] [varchar](255) NULL,
	[PostalCode] [varchar](10) NULL,
	[AccountStatus] [int] NOT NULL,
	[AccountStatusUpdated] [datetime] NOT NULL,
	[Created] [datetime] NOT NULL,
	[LastUsed] [datetime] NULL,
	[StreetSuffix] [varchar](50) NULL,
	[MailingNumber] [varchar](50) NULL,
	[ApartmentNumber] [varchar](50) NULL,
	[SecurityQuestion1] [varchar](255) NULL,
	[SecurityAnswer1] [varchar](255) NULL,
	[SecurityQuestion2] [varchar](255) NULL,
	[SecurityAnswer2] [varchar](255) NULL,
	[UserNote] [varchar](2000) NULL,
	[LastEditedUserId] [int] NULL,
	[PhoneNumber] [varchar](20) NULL,
	[PendingCount] [int] NOT NULL,
	[ApprovedCount] [int] NOT NULL,
	[RejectedCount] [int] NOT NULL,
	[UserName] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[WhiteListFile_Unique]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--if there are multiple schemes per card, card with highher percentage will be selected.
--if one with percentage and other with minute, that with percentage will be selected.
create view [dbo].[WhiteListFile_Unique] as 
select w.*
from  WhilteListFile w inner join
	(
	select CustomerId,AreaId,MeterId,CardHash,MAX((case when DiscountPercentage > 0 then 2000+DiscountPercentage else 1000+DiscountMinute end)) rnk from WhilteListFile 
	group by CustomerId,AreaId,MeterId,CardHash
	)v
	on w.customerid = v.CustomerId
	and w.AreaId = v.AreaId
	and w.MeterId = v.MeterId
	and w.CardHash = v.CardHash
	and v.rnk = (case when DiscountPercentage > 0 then 2000+DiscountPercentage else 1000+DiscountMinute end)
GO
/****** Object:  Table [dbo].[VersionDetails]    Script Date: 04/01/2014 22:06:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VersionDetails](
	[VersionID] [float] NULL,
	[ObjectName] [varchar](50) NOT NULL,
	[objectType] [varchar](50) NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[UDP_MeterPushSchedule]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[UDP_MeterPushSchedule] as 
	Select [PamPushId]
	      ,[CustomerId]
	      ,[AreaId]
	      ,[MeterId]
	      ,[BayNumber]
	      ,DateDiff(SECOND,'2000/01/01',[ExpiryTime]) expt
	      from MeterPushSchedule 
	      --for timezone gap, 10 hour in the past is allowed
	      where ExpiryTime > DATEADD(HOUR,-10,GETDATE())
	      and (Acknowledged is null and CancelDate is null)
GO
/****** Object:  Table [dbo].[TransactionsAudit]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsAudit](
	[TransAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransAuditDate] [datetime] NULL,
	[Status] [int] NULL,
	[TransactionID] [bigint] NULL,
	[CardTypeCode] [int] NOT NULL,
	[BatchId] [varchar](20) NULL,
 CONSTRAINT [PK_TransactionsAudit] PRIMARY KEY CLUSTERED 
(
	[TransAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [TransactionsAudit_IDX_ST] ON [dbo].[TransactionsAudit] 
(
	[Status] ASC,
	[TransactionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TariffRateConfigurationProfile]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffRateConfigurationProfile](
	[ConfigProfileId] [bigint] NOT NULL,
	[TariffRateConfigurationID] [bigint] NOT NULL,
	[TariffRateId] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[RateName] [varchar](50) NOT NULL,
	[RateInCents] [int] NOT NULL,
	[PerTimeValue] [int] NULL,
	[PerTimeUnit] [int] NULL,
	[MaxTimeValue] [int] NULL,
	[MaxTimeUnit] [int] NULL,
	[GracePeriodMinute] [int] NULL,
	[LinkedRate] [bigint] NULL,
	[RateDesc] [varchar](255) NULL,
	[LockMaxTime] [bit] NULL,
	[UpdateDateTime] [datetime] NULL,
 CONSTRAINT [PK_TariffRateProfile] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileId] ASC,
	[TariffRateConfigurationID] ASC,
	[TariffRateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffRateConfiguration]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffRateConfiguration](
	[TariffRateConfigurationId] [bigint] NOT NULL,
	[Name] [varchar](50) NULL,
	[Desc] [varchar](255) NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ConfiguredOn] [datetime] NULL,
	[ConfiguredBy] [int] NULL,
	[State] [int] NOT NULL,
	[ConfigProfileId] [bigint] NULL,
 CONSTRAINT [PK_TariffRateConfiguration] PRIMARY KEY CLUSTERED 
(
	[TariffRateConfigurationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TariffRate]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TariffRate](
	[TariffRateId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[RateName] [varchar](50) NOT NULL,
	[RateInCents] [int] NOT NULL,
	[PerTimeValue] [int] NULL,
	[PerTimeUnit] [int] NULL,
	[MaxTimeValue] [int] NULL,
	[MaxTimeUnit] [int] NULL,
	[GracePeriodMinute] [int] NULL,
	[LinkedRate] [bigint] NULL,
	[RateDesc] [varchar](255) NULL,
	[LockMaxTime] [bit] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_TariffRate] PRIMARY KEY CLUSTERED 
(
	[TariffRateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_ActiveAlarm_cleanUp]    Script Date: 04/01/2014 22:06:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_ActiveAlarm_cleanUp] 
as
	Declare cur  cursor	for
			SELECT customerid,
		   areaid,
		   meterid,
		   eventcode,
		   eventsource,
		   timeofoccurrance
	FROM   ActiveAlarms where EventUID is null
	
	Declare curHistorical  cursor	for
			SELECT customerid,
				   areaid,
				   meterid,
				   eventcode,
				   eventsource,
				   EventState,
				   eventuid,
				   timeofoccurrance,
				   TimeOfClearance
	FROM   HistoricalAlarms where GlobalMeterId is null;	
	
	Declare
	@CustomerID          INT,
	@AreaID              INT,
	@MeterID             INT,
	@EventCode           INT,
	@EventSource         INT,
	@EventState			 INT,
	@EventUID			int,
	@TimeOfOccurrance    DATETIME,
	@TimeOfClearance    DATETIME,
	@ERROR_MESSAGE		VARCHAR(500)
	
begin
	open cur 
	fetch next from cur  into @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@TimeOfOccurrance
	WHILE @@FETCH_STATUS = 0 BEGIN		
		BEGIN TRY
			exec sp_ActiveAlarm_Helper @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@TimeOfOccurrance
		END TRY
		BEGIN CATCH
			SET @ERROR_MESSAGE = Error_message()
			Print 'Error'		
			exec sp_Trigger_Log @ERROR_MESSAGE,'trg_activealaram_update'		
		END CATCH
		fetch next from cur  into @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@TimeOfOccurrance
	END	--while @@FETCH_STATUS	
	
	CLOSE cur 	
	DEALLOCATE cur 
	
	
	-------------- HISTORICAL ALARMS ------------	
	open curHistorical 
		fetch next from curHistorical  into @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@EventState,@EventUID,@TimeOfOccurrance,@TimeOfClearance
		WHILE @@FETCH_STATUS = 0 BEGIN		
			BEGIN TRY
				exec sp_HistoricalAlarm_Helper @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@EventState,@EventUID,@TimeOfOccurrance,@TimeOfClearance
			END TRY
			BEGIN CATCH
				SET @ERROR_MESSAGE = Error_message()
				Print 'Error'		
				exec sp_Trigger_Log @ERROR_MESSAGE,'Trg_HistoricalAlaram_update'		
			END CATCH
			fetch next from curHistorical  into  @CustomerId,@AreaId,@MeterID,@EventCode,@EventSource,@EventState,@EventUID,@TimeOfOccurrance,@TimeOfClearance
		END	--while @@FETCH_STATUS	
		
	CLOSE curHistorical 	
	DEALLOCATE curHistorical 
	
end
GO
/****** Object:  StoredProcedure [dbo].[sp_TimeTypeLogic]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_TimeTypeLogic] 
	  @TS DATETIME,
	  @TimeType1 INT OUTPUT,
	  @TimeType2 INT OUTPUT,
	  @TimeType3 INT OUTPUT,
	  @TimeType4 INT OUTPUT,
	  @TimeType5 INT OUTPUT
AS
	Declare @msg varchar(2000)
BEGIN
	IF ((DATEPART(DW, @TS))=1) OR ((DATEPART (DW, @TS))=7) BEGIN
		SET @TIMETYPE1 = 2 -- WEEKEND
	END ELSE BEGIN
		SET @TIMETYPE1 = 1 --WEEKDAY
	END

	IF (RTRIM(RIGHT(CONVERT(VARCHAR, @TS, 100),2))='AM') BEGIN
		SET @TIMETYPE3 = 4 --MORNING
	END ELSE BEGIN
		SET @TIMETYPE3 = 5 --EVENING
	END

	SELECT @TIMETYPE2= TIMETYPEID FROM TIMETYPE WHERE TIMETYPEDESC = DATENAME (DW, @TS)
	
	select @msg = 'TS' from TimeType
	
	Print @msg
END
GO
/****** Object:  Table [dbo].[SFSchedule]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFScheduleTypeID] [int] NOT NULL,
	[CityID] [int] NOT NULL,
	[EffectiveDate] [datetime] NOT NULL,
	[TransmissionDate] [datetime] NOT NULL,
	[Received] [datetime] NOT NULL,
 CONSTRAINT [PK_SFSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ConfigProfileSpaceAudit]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigProfileSpaceAudit](
	[ConfigProfileSpaceAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ConfigProfileSpaceId] [bigint] NOT NULL,
	[ConfigProfileId] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[ScheduledDate] [datetime] NULL,
	[ActivationDate] [datetime] NULL,
	[CreationDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateDatetime] [datetime] NOT NULL,
	[ConfigStatus] [int] NULL,
	[UserId] [int] NULL,
	[AuditDatetime] [datetime] NOT NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_ConfigProfileSpaceAudit] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileSpaceAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CreditCardAttempts]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CreditCardAttempts](
	[TransactionsCreditCardID] [bigint] NOT NULL,
	[ResultMsg] [varchar](48) NULL,
	[ResultDateTime] [datetime] NULL,
	[ResultTrackingCode] [varchar](48) NULL,
	[Status] [int] NULL,
	[BankServiceProviderID] [int] NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventSourceCustomer]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventSourceCustomer](
	[EventSourceCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[EventSourceCode] [int] NOT NULL,
	[EventSourceDesc] [varchar](25) NULL,
	[IsDisplay] [bit] NULL,
 CONSTRAINT [PK_EventSourceCustomer] PRIMARY KEY CLUSTERED 
(
	[EventSourceCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FDFileTypeMeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDFileTypeMeterGroup](
	[FDFileTypeMeterGroupID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[FileType] [int] NOT NULL,
	[MeterGroup] [int] NULL,
	[VersionGroup] [int] NULL,
 CONSTRAINT [PK_FDFileTypeMeterGroup] PRIMARY KEY CLUSTERED 
(
	[FDFileTypeMeterGroupID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FDFiles]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FDFiles](
	[FileID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[FileType] [int] NOT NULL,
	[FileHash] [varchar](50) NOT NULL,
	[FileComments] [varchar](50) NOT NULL,
	[FileSizeBytes] [bigint] NOT NULL,
	[FileAdditionDate] [datetime] NOT NULL,
	[FileRawData] [image] NOT NULL,
	[FileName] [varchar](50) NULL,
	[OriginalFileID] [bigint] NULL,
 CONSTRAINT [PK_FDFiles] PRIMARY KEY CLUSTERED 
(
	[FileID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventCodeMaster]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EventCodeMaster](
	[EventCode] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[AlarmTier] [int] NOT NULL,
	[EventDescAbbrev] [varchar](16) NULL,
	[EventDescVerbose] [varchar](50) NULL,
	[IsAlarm] [bit] NULL,
	[EventType] [int] NULL,
 CONSTRAINT [PK_EventCodeMaster] PRIMARY KEY CLUSTERED 
(
	[EventCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EventCodeAssetType]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EventCodeAssetType](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[EventCode] [int] NOT NULL,
	[MeterGroupId] [int] NOT NULL,
 CONSTRAINT [PK_EventCodeAssetType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CashBoxAudit]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CashBoxAudit](
	[CashBoxAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[CashBoxID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CashBoxSeq] [int] NOT NULL,
	[CashBoxState] [int] NOT NULL,
	[InstallDate] [datetime] NULL,
	[CashBoxModel] [int] NULL,
	[CashBoxType] [int] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[CashBoxName] [varchar](255) NOT NULL,
	[CashBoxLocationTypeId] [int] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
	[WarrantyExpiration] [datetime] NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_CashBoxAudit] PRIMARY KEY CLUSTERED 
(
	[CashBoxAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AssetPending]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetPending](
	[AssetPendingId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[RecordCreateDateTime] [datetime] NOT NULL,
	[RecordMigrationDateTime] [datetime] NULL,
	[AssetId] [bigint] NOT NULL,
	[AssetType] [int] NOT NULL,
	[AssetName] [varchar](500) NULL,
	[AssetModel] [int] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[Street] [varchar](500) NULL,
	[OperationalStatus] [int] NULL,
	[MeterMapAreaId2] [int] NULL,
	[MeterMapZoneId] [int] NULL,
	[MeterMapSuburbId] [int] NULL,
	[MeterMapCollectionRoute] [int] NULL,
	[MeterMapMaintenanceRoute] [int] NULL,
	[MeterMapSensorId] [int] NULL,
	[MeterMapGatewayId] [int] NULL,
	[MeterMapCashBoxId] [int] NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[PhoneNumber] [varchar](500) NULL,
	[SpaceCount] [int] NULL,
	[DemandStatus] [int] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[SpaceType] [int] NULL,
	[AssociatedMeterId] [bigint] NULL,
	[AssocidateSpaceId] [bigint] NULL,
	[DateInstalled] [datetime] NULL,
	[AssetFirmwareVersion] [varchar](500) NULL,
	[AssetSoftwareVersion] [varchar](500) NULL,
	[MPVFirmware] [varchar](500) NULL,
	[GatewayModel] [int] NULL,
	[Manufacturer] [varchar](500) NULL,
	[HardwareVeresion] [varchar](500) NULL,
	[PowerSource] [int] NULL,
	[PrimaryGateway] [bigint] NULL,
	[SecondaryGateway] [bigint] NULL,
	[LocationGateway] [varchar](250) NULL,
	[LocationMeters] [varchar](250) NULL,
	[LocationSensors] [varchar](250) NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[AssetState] [int] NULL,
	[AssociatedMeterAreaId] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[CashboxLocationId] [int] NULL,
	[AssetPendingReasonId] [int] NULL,
	[DisplaySpaceNum] [varchar](50) NULL,
	[MigratedTs] [datetime] NULL,
	[ErrorMessage] [varchar](2000) NULL,
 CONSTRAINT [PK_AssetPending] PRIMARY KEY CLUSTERED 
(
	[AssetPendingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MeterMapping]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterMapping](
	[MeterMappingId] [int] NOT NULL,
	[SpaceId] [int] NOT NULL,
	[DuncanCustomerId] [int] NOT NULL,
	[DuncanAreaId] [int] NOT NULL,
	[DuncanMeterId] [int] NOT NULL,
	[DuncanBayId] [int] NOT NULL,
	[Remark] [nvarchar](50) NULL,
	[CreatedDateTime] [datetime] NOT NULL,
	[VendorId] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OLTAcquirers]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OLTAcquirers](
	[CustomerID] [int] NOT NULL,
	[CardTypeCode] [int] NOT NULL,
	[OLTPActive] [int] NOT NULL,
	[VSignPartner] [char](12) NULL,
	[MigsAC] [varchar](20) NULL,
	[VendorMerchant] [varchar](64) NOT NULL,
	[UserName] [varchar](64) NOT NULL,
	[Password] [varchar](64) NOT NULL,
	[AcquirerIF] [int] NOT NULL,
	[Description] [varchar](50) NULL,
	[Currency] [varchar](25) NULL,
	[DelayedProcessing] [int] NULL,
	[CardPresent] [int] NULL,
	[ReAuthorise] [int] NOT NULL,
 CONSTRAINT [PK_OLTAcquirers] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[CardTypeCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParkingSpacesAudit]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkingSpacesAudit](
	[ParkingSpacesAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[ServerID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AddedDateTime] [datetime] NOT NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[HasSensor] [bit] NULL,
	[SpaceStatus] [int] NULL,
	[DateActivated] [datetime] NULL,
	[Comments] [varchar](1000) NULL,
	[DisplaySpaceNum] [varchar](50) NULL,
	[DemandZoneId] [int] NULL,
	[InstallDate] [datetime] NULL,
	[ParkingSpaceType] [int] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_ParkingSpacesAudit] PRIMARY KEY CLUSTERED 
(
	[ParkingSpacesAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParkSpaceRight]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkSpaceRight](
	[VehicleId] [int] NOT NULL,
	[customerid] [int] NOT NULL,
	[ZoneId] [int] NOT NULL,
	[SpaceNum] [int] NULL,
	[txseqNum] [int] NOT NULL,
	[ReceiptId] [int] NOT NULL,
	[amount] [int] NOT NULL,
	[transactionTime] [datetime] NOT NULL,
	[currentdatetime] [datetime] NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[AutoDeactivateDateTime] [datetime] NOT NULL,
	[ProductDescription] [varchar](30) NULL,
 CONSTRAINT [PK_ParkSpaceRight] PRIMARY KEY CLUSTERED 
(
	[VehicleId] ASC,
	[customerid] ASC,
	[ZoneId] ASC,
	[transactionTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Processes]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Processes](
	[ProcessID] [char](4) NOT NULL,
	[ProcessType] [char](2) NOT NULL,
	[ProcessDescription] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Processes] PRIMARY KEY CLUSTERED 
(
	[ProcessID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SensorsAudit]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SensorsAudit](
	[SensorsAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[SensorID] [int] NOT NULL,
	[BarCodeText] [varbinary](100) NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[GSMNumber] [varchar](100) NULL,
	[GlobalMeterID] [bigint] NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[Location] [varchar](100) NOT NULL,
	[SensorName] [varchar](100) NOT NULL,
	[SensorState] [int] NOT NULL,
	[SensorType] [int] NULL,
	[InstallDateTime] [datetime] NOT NULL,
	[DemandZone] [int] NULL,
	[Comments] [varchar](1000) NULL,
	[RoadWayType] [int] NULL,
	[ParkingSpaceId] [bigint] NULL,
	[SensorModel] [int] NULL,
	[OperationalStatus] [int] NULL,
	[WarrantyExpiration] [datetime] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_SensorsAudit] PRIMARY KEY CLUSTERED 
(
	[SensorsAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RateScheduleConfigurationProfile]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateScheduleConfigurationProfile](
	[ConfigProfileId] [bigint] NOT NULL,
	[RateScheduleId] [bigint] NOT NULL,
	[RateScheduleConfigurationId] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[StartTimeHour] [int] NOT NULL,
	[StartTimeMinute] [int] NOT NULL,
	[OperationMode] [int] NOT NULL,
	[TariffRateConfigurationID] [bigint] NOT NULL,
	[MessageSequence] [int] NULL,
	[LockMaxTime] [bit] NULL,
	[UpdateDateTime] [datetime] NULL,
	[ScheduleNumber] [int] NULL,
 CONSTRAINT [PK_RateScheduleConfigurationProfile] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileId] ASC,
	[RateScheduleConfigurationId] ASC,
	[RateScheduleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RateScheduleConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[RateScheduleConfiguration](
	[RateScheduleConfigurationId] [bigint] NOT NULL,
	[Name] [varchar](50) NULL,
	[Desc] [varchar](255) NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ConfiguredOn] [datetime] NULL,
	[ConfiguredBy] [int] NULL,
	[State] [int] NOT NULL,
	[ConfigProfileId] [bigint] NULL,
 CONSTRAINT [PK_RateScheduleConfiguration] PRIMARY KEY CLUSTERED 
(
	[RateScheduleConfigurationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[RateSchedule]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateSchedule](
	[RateScheduleId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ScheduleNumber] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[StartTimeHour] [int] NOT NULL,
	[StartTimeMinute] [int] NOT NULL,
	[OperationMode] [int] NOT NULL,
	[MessageSequence] [int] NULL,
	[LockMaxTime] [bit] NULL,
	[TariffRateId] [bigint] NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[UpdatedOn] [datetime] NULL,
	[UpdatedBy] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[TariffRateConfigurationID] [bigint] NULL,
 CONSTRAINT [PK_RateSchedule] PRIMARY KEY CLUSTERED 
(
	[RateScheduleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RateTransmissionDetails]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateTransmissionDetails](
	[ID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransmissionID] [bigint] NOT NULL,
	[SpaceNum] [int] NOT NULL,
	[RateInCent] [int] NOT NULL,
 CONSTRAINT [PK_RateTransmissionDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterDiagnostic]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterDiagnostic](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DiagTime] [datetime] NOT NULL,
	[ReceivedTime] [datetime] NOT NULL,
	[DiagnosticType] [int] NOT NULL,
	[DiagnosticValue] [varchar](100) NULL,
	[EventUID] [bigint] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
 CONSTRAINT [PK_MeterDiagnostic] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_MeterDiagnostic] UNIQUE NONCLUSTERED 
(
	[CustomerID] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[DiagTime] ASC,
	[DiagnosticType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IX_MeterDiagnostic_Time] ON [dbo].[MeterDiagnostic] 
(
	[ReceivedTime] DESC,
	[CustomerID] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LookupItem]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[LookupItem](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ItemId] [int] NOT NULL,
	[PropertyGroupId] [int] NOT NULL,
	[ItemDesc] [varchar](250) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_LookupItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HolidayRateConfigurationProfile]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HolidayRateConfigurationProfile](
	[ConfigProfileId] [bigint] NOT NULL,
	[HolidayRateId] [bigint] NOT NULL,
	[HolidayRateConfigurationId] [bigint] NOT NULL,
	[HolidayDateTime] [datetime] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[StartTimeHour] [int] NOT NULL,
	[StartTimeMinute] [int] NOT NULL,
	[OperationMode] [int] NOT NULL,
	[TariffRateConfigurationID] [bigint] NOT NULL,
	[MessageSequence] [int] NULL,
	[LockMaxTime] [bit] NULL,
	[UpdateDateTime] [datetime] NULL,
 CONSTRAINT [PK_HolidayRateConfigurationProfile] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileId] ASC,
	[HolidayRateConfigurationId] ASC,
	[HolidayRateId] ASC,
	[HolidayDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HolidayRateConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HolidayRateConfiguration](
	[HolidayRateConfigurationId] [bigint] NOT NULL,
	[Name] [varchar](50) NULL,
	[Desc] [varchar](255) NULL,
	[CustomerId] [int] NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[ConfiguredOn] [datetime] NULL,
	[ConfiguredBy] [int] NULL,
	[State] [int] NOT NULL,
	[ConfigProfileId] [bigint] NULL,
 CONSTRAINT [PK_HolidayRateConfiguration] PRIMARY KEY CLUSTERED 
(
	[HolidayRateConfigurationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MaintRouteSeq]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MaintRouteSeq](
	[MaintRouteId] [int] NOT NULL,
	[MaintRouteSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[GatewaysAudit]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GatewaysAudit](
	[GatewaysAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[GateWayID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[Description] [varchar](250) NULL,
	[Latitude] [float] NULL,
	[Longitude] [float] NULL,
	[Location] [varchar](250) NULL,
	[GatewayState] [int] NULL,
	[GatewayType] [int] NULL,
	[InstallDateTime] [datetime] NULL,
	[TimeZoneID] [int] NULL,
	[DemandZone] [int] NULL,
	[CAMID] [varchar](250) NULL,
	[CELID] [varchar](250) NULL,
	[PowerSource] [int] NULL,
	[HWVersion] [varchar](250) NULL,
	[Manufacturer] [varchar](250) NULL,
	[GatewayModel] [int] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
	[WarrantyExpiration] [datetime] NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_GatewaysAudit] PRIMARY KEY CLUSTERED 
(
	[GatewaysAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MechMaster]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MechMaster](
	[MechId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MechSerial] [varchar](25) NOT NULL,
	[MechType] [int] NULL,
	[Customerid] [int] NULL,
	[SimNo] [varchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[InactiveRemarkID] [int] NULL,
	[CreateDate] [datetime] NULL,
	[InsertedDate] [datetime] NOT NULL,
	[Notes] [varchar](50) NULL,
 CONSTRAINT [PK_MechMaster] PRIMARY KEY CLUSTERED 
(
	[MechId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_MechSerial] UNIQUE NONCLUSTERED 
(
	[Customerid] ASC,
	[MechSerial] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[HolidayRateForConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[HolidayRateForConfiguration](
	[HolidayRatesId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[HolidayRateConfigurationId] [bigint] NOT NULL,
	[HolidayRateId] [bigint] NOT NULL,
 CONSTRAINT [PK_HolidayRates] PRIMARY KEY CLUSTERED 
(
	[HolidayRatesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[HousingMaster]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[HousingMaster](
	[HousingId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[HousingName] [varchar](20) NOT NULL,
	[Customerid] [int] NOT NULL,
	[Block] [varchar](25) NOT NULL,
	[StreetName] [varchar](50) NOT NULL,
	[StreetType] [varchar](20) NOT NULL,
	[StreetDirection] [varchar](10) NOT NULL,
	[StreetNotes] [varchar](50) NULL,
	[HousingTypeID] [int] NULL,
	[DoorLockId] [varchar](15) NULL,
	[MechLockId] [varchar](15) NULL,
	[IsActive] [bit] NOT NULL,
	[InactiveRemarkID] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[Notes] [varchar](50) NULL,
 CONSTRAINT [PK_HousingMaster] PRIMARY KEY CLUSTERED 
(
	[HousingId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_HousingMaster] UNIQUE NONCLUSTERED 
(
	[Customerid] ASC,
	[HousingName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MeterDiagnosticTypeCustomer]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterDiagnosticTypeCustomer](
	[CustomerId] [int] NOT NULL,
	[DiagnosticType] [int] NOT NULL,
	[IsDisplay] [bit] NULL,
 CONSTRAINT [PK_MeterDiagnosticTypeCustomer] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[DiagnosticType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RateScheduleForConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateScheduleForConfiguration](
	[RateSchedulesId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[RateScheduleConfigurationId] [bigint] NOT NULL,
	[RateScheduleId] [bigint] NOT NULL,
 CONSTRAINT [PK_RateSchedules] PRIMARY KEY CLUSTERED 
(
	[RateSchedulesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[Pending_Meters]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[Pending_Meters] as 
	SELECT [AssetPendingId]
		  ,[CustomerId]
		  ,[AreaId]
		  ,[MeterId]
		  ,[CreateUserId]
		  ,[RecordCreateDateTime]
		  ,[RecordMigrationDateTime]
		  ,[AssetId]
		  ,[AssetType]
		  ,[AssetName]
		  ,[AssetModel]
		  ,[NextPreventativeMaintenance]
		  ,[OperationalStatus]      
		  ,[Latitude]
		  ,[Longitude]
		  ,[PhoneNumber]
		  ,[SpaceCount]
		  ,[DemandStatus]
		  ,[WarrantyExpiration]      
		  ,[DateInstalled]    
		  ,[LocationMeters]
		  ,[LastPreventativeMaintenance]
		  ,[AssetState]
		  ,[OperationalStatusTime]
		  ,[AssetPendingReasonId]
		  ,MeterMapAreaId2 
		  ,MeterMapZoneId
		  ,MeterMapSuburbId
		  ,MeterMapCollectionRoute
		  ,MeterMapMaintenanceRoute
		  ,[AssetFirmwareVersion]
		  ,[AssetSoftwareVersion]
		  ,[MPVFirmware]
	  FROM [AssetPending] 
	  where RecordMigrationDateTime < GETDATE()
	  and AssetType in (0,1) --SSM and MSM
	  and migratedTS is null
GO
/****** Object:  Table [dbo].[Schedules]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Schedules](
	[CustomerID] [int] NOT NULL,
	[ScheduleID] [int] NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[PeriodSec] [int] NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[Mon] [char](1) NOT NULL,
	[Tue] [char](1) NOT NULL,
	[Wed] [char](1) NOT NULL,
	[Thu] [char](1) NOT NULL,
	[Fri] [char](1) NOT NULL,
	[Sat] [char](1) NOT NULL,
	[Sun] [char](1) NOT NULL,
	[DayOfMonth] [int] NOT NULL,
	[MonthNo] [int] NOT NULL,
	[ProcessType] [char](2) NOT NULL,
	[BankID] [int] NULL,
 CONSTRAINT [PK_Schedules] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[ScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ReportMaster]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportMaster](
	[RepId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ReportName] [varchar](75) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[RepDesc] [varchar](100) NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ReportMaster] PRIMARY KEY CLUSTERED 
(
	[RepId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_ReportMaster] UNIQUE NONCLUSTERED 
(
	[CustomerId] ASC,
	[ReportName] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Parts]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Parts](
	[PartId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PartName] [varchar](500) NULL,
	[MeterGroup] [int] NULL,
	[Category] [int] NULL,
	[PartDesc] [varchar](500) NULL,
	[CostInCent] [int] NULL,
	[Status] [int] NULL,
	[Mechanism] [int] NULL,
 CONSTRAINT [PK_Parts] PRIMARY KEY CLUSTERED 
(
	[PartId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PaymentReceived]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PaymentReceived](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[SeqNumber] [int] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[TransactionID] [int] NOT NULL,
	[TransactionTime] [datetime] NOT NULL,
	[ReceivedTime] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PAMMeterAccess]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAMMeterAccess](
	[Customerid] [int] NOT NULL,
	[Clusterid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[LastAccessed] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PAMCustomerMap]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PAMCustomerMap](
	[CustomerId] [int] NOT NULL,
	[CustomerCode] [varchar](50) NOT NULL,
	[ClusterId] [int] NOT NULL,
	[BayNum] [int] NOT NULL,
 CONSTRAINT [PK_PAMCustomerMap] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[CustomerCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PAMClusters]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PAMClusters](
	[Clusterid] [int] NOT NULL,
	[Customerid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[Hostedbaystart] [int] NOT NULL,
	[Hostedbayend] [int] NOT NULL,
	[Description] [varchar](50) NULL,
 CONSTRAINT [PK_PAMClusters_1] PRIMARY KEY CLUSTERED 
(
	[Clusterid] ASC,
	[Customerid] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PAMBayExpt]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAMBayExpt](
	[Customerid] [int] NOT NULL,
	[Clusterid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayId] [int] NOT NULL,
	[ExpiryTime] [numeric](18, 0) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PAMActiveCustomers]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAMActiveCustomers](
	[CustomerID] [int] NOT NULL,
	[ResetImin] [bit] NULL,
	[ExpTimeByPAM] [bit] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssetVersionMaster]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetVersionMaster](
	[AssetVersionMasterId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[FDFileTypeMeterGroupID] [int] NOT NULL,
	[VersionName] [varchar](255) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AssetVersionMaster] PRIMARY KEY CLUSTERED 
(
	[AssetVersionMasterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AssetStateCustomer]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AssetStateCustomer](
	[AssetStateCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AssetStateId] [int] NOT NULL,
	[AssetStateDesc] [varchar](25) NOT NULL,
	[IsDisplayed] [bit] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_AssetStateCustomer] PRIMARY KEY CLUSTERED 
(
	[AssetStateCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AcquirerBatch]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AcquirerBatch](
	[AcquirerBatchId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[BatchRef] [varchar](20) NOT NULL,
	[Status] [int] NOT NULL,
	[OpenDateTime] [datetime] NOT NULL,
	[ClosedDateTime] [datetime] NULL,
	[SettleDateTime] [datetime] NULL,
	[SettleAttempt] [int] NULL,
 CONSTRAINT [UK_AcquirerBatch] UNIQUE NONCLUSTERED 
(
	[BatchRef] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AuditRegistry]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditRegistry](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
	[StartDateTime] [datetime] NOT NULL,
	[CollectinDateTime] [datetime] NOT NULL,
	[CashAmount] [int] NULL,
	[CreditCardAmount] [int] NULL,
	[SmartCardAmount] [int] NULL,
	[UnknownTokenCount] [int] NULL,
	[CashCount] [int] NULL,
	[CreditCardCount] [int] NULL,
	[SmartCardCount] [int] NULL,
	[CardNumber] [varchar](16) NULL,
	[LocationId] [varchar](20) NOT NULL,
	[MechSerial] [varchar](25) NOT NULL,
	[Coins] [varchar](100) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BlackListActive]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BlackListActive](
	[BlackListID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[HCardNum] [varchar](40) NOT NULL,
	[XCardNum] [varchar](160) NOT NULL,
	[Code] [int] NULL,
	[DateTime] [datetime] NULL,
	[CustomerID] [int] NOT NULL,
 CONSTRAINT [PK_BlackListActive] PRIMARY KEY NONCLUSTERED 
(
	[XCardNum] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CashBox]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CashBox](
	[CashBoxID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CashBoxSeq] [int] NOT NULL,
	[CashBoxState] [int] NOT NULL,
	[InstallDate] [datetime] NULL,
	[CashBoxModel] [int] NULL,
	[CashBoxType] [int] NULL,
	[OperationalStatus] [int] NULL,
	[OperationalStatusTime] [datetime] NULL,
	[LastPreventativeMaintenance] [datetime] NULL,
	[NextPreventativeMaintenance] [datetime] NULL,
	[CashBoxName] [varchar](255) NOT NULL,
	[WarrantyExpiration] [datetime] NULL,
	[CashBoxLocationTypeId] [int] NULL,
	[OperationalStatusEndTime] [datetime] NULL,
	[OperationalStatusComment] [varchar](2000) NULL,
 CONSTRAINT [PK_CashBox] PRIMARY KEY CLUSTERED 
(
	[CashBoxID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BlackListFiles]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BlackListFiles](
	[BlackListFilesID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[FileName] [varchar](64) NOT NULL,
	[DateTimeGenerated] [datetime] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[BlackListFormatVersion] [int] NULL,
	[BlackListMAC] [varchar](160) NULL,
	[BlackListChecksum] [int] NULL,
 CONSTRAINT [PK_BlackListFiles] PRIMARY KEY NONCLUSTERED 
(
	[FileName] ASC,
	[DateTimeGenerated] ASC,
	[CustomerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CBImpFiles]    Script Date: 04/01/2014 22:07:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CBImpFiles](
	[CustomerId] [int] NOT NULL,
	[FileProcessId] [bigint] NOT NULL,
	[FileType] [int] NULL,
	[FileNameImp] [varchar](100) NOT NULL,
	[DateTimeImp] [datetime] NOT NULL,
	[MaxDateTimeRem] [datetime] NOT NULL,
	[MinDateTimeRem] [datetime] NOT NULL,
 CONSTRAINT [PK_CBImpFiles] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[FileNameImp] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[spImportCashBox]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[spImportCashBox]
        @CustomerID integer = 29,
	@filename varchar(128),
	@xmlpath varchar(128),
	@xmldoc nText
    As

    CREATE TABLE #CashBoxDataImportX (
        DateCol varchar(16),
        TimeCol varchar(16),
        OperatorId varchar(16),
        MeterIDx int,
        DateIn varchar(16),
        TimeIn varchar(16),
        DateOut varchar(16),
        TimeOut varchar(16),
        CashBoxId varchar(16),
        AutoFlag varchar(1),
        AreaIDx int,
        CBCounter int,
        Dollar2Coins int,
        Dollar1Coins int,
        Cents50Coins int,
        Cents20Coins int,
        Cents10Coins int,
        Cents5Coins int,
        AmtCashless float,
        AmtAuto float,
        AmtManual float,
        AmtDiff float,
        PercentFull int,
        MeterStatusx varchar(2),
        TallyRejects int,
        CreditCounter int,
        TimeActive bigint,
        MinVolts float,
        MaxTemp float,
        Firmware tinyint,
        FirmwareRev tinyint,
        EventCode int,
        ExportFlag varchar(10),
        SequenceFlag varchar(10),
        SeqDupFlag varchar(10),
        FileName varchar(64),
        Nullfield varchar(1),
	ident int IDENTITY
    ) ON [PRIMARY]


EXEC('Insert #CashBoxDataImportX SELECT * FROM OPENROWSET(''MICROSOFT.JET.OLEDB.4.0'',
 ''Text;Database=' + @xmlpath + ';Extended Properties="HDR=NO;";'',
 ''SELECT * FROM [' + @filename + ']'')')

DECLARE @ErrorMsg varchar(250)
SET @ErrorMsg = 'Bad records. Fix lines ['
-- Verify input. Compile list of bad records...
Select @ErrorMsg = @ErrorMsg + CAST(ident as varchar) + ',' from #CashBoxDataImportX a where MeterIDx is null or Cast(DateIn as int) > 320000 or CashBoxId is null order by ident DESC
IF @@ROWCOUNT > 0
BEGIN
        SET @ErrorMsg =  + @ErrorMsg + '] and re-import.'
	RAISERROR (@ErrorMsg,16,1)
	RETURN
END

DECLARE @MaxDate datetime
DECLARE @MinDate datetime
Select
@MaxDate=Max(CONVERT(DATETIME,'0' + DateOut + ' ' + STUFF(REPLICATE('0',4-LEN(TimeOut))+TimeOut,3,0,':'))),
@MinDate=Min(CONVERT(DATETIME,'0' + DateOut + ' ' + STUFF(REPLICATE('0',4-LEN(TimeOut))+TimeOut,3,0,':')))
from #CashBoxDataImportX

DECLARE @LastFileID int
Select @LastFileID=Max(FileProcessId) from CBImpFiles
Select @LastFileID=isnull(@LastFileID,0)+1

BEGIN TRAN
    INSERT CashBoxDataImport Select @CustomerID,AreaIDx,MeterIDx,
	CONVERT(DATETIME,'0' + DateIn + ' ' + STUFF(REPLICATE('0',4-LEN(TimeIn))+TimeIn,3,0,':'))  as DateTimeIns,
	CashBoxId,
	CBCounter as CashBoxSequenceNo,
	CONVERT(DATETIME,'0' + DateCol + ' ' + REVERSE(STUFF(LEFT(REVERSE(TimeCol),4),3,0,':')+':'+SUBSTRING(REVERSE(TimeCol),5,10)))  as DateTimeRead,
	CONVERT(DATETIME,'0' + DateOut + ' ' + STUFF(REPLICATE('0',4-LEN(TimeOut))+TimeOut,3,0,':'))  as DateTimeRem,
	OperatorId,
	AutoFlag,
	Dollar2Coins,
	Dollar1Coins,
	Cents50Coins,
	Cents20Coins,
	Cents10Coins,
	Cents5Coins,
	AmtCashless,
	AmtAuto,
	AmtManual,
	AmtDiff,
	PercentFull,
	MeterStatusx,
	TallyRejects,
	CreditCounter,
	TimeActive,
	MinVolts,
	MaxTemp,
	Firmware,
	FirmwareRev,
	EventCode,
	SUBSTRING(FileName,1,50),
	@LastFileID as FileProcessId
	 from #CashBoxDataImportX
	/*LEFT JOIN dbo.Meters ON  MeterIDx = MeterId*/
    IF (@@ERROR <> 0) GOTO ERR_HANDLER
    INSERT CBImpFiles VALUES (@CustomerID,@LastFileID,1, @filename,GETDATE(),@MaxDate,@MinDate)
    IF (@@ERROR <> 0) GOTO ERR_HANDLER
    DROP TABLE #CashBoxDataImportX
COMMIT TRAN
RETURN 0

ERR_HANDLER:
    ROLLBACK TRAN
    RAISERROR ('An error occurred during import. Rollback has occured. Please manually import the csv file.',16,1)
    RETURN
GO
/****** Object:  Table [dbo].[CustomerPropertyGroup]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerPropertyGroup](
	[CustomerPropertyGroupId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PropertyGroupDesc] [varchar](500) NOT NULL,
	[CustomerID] [int] NOT NULL,
 CONSTRAINT [PK_CustomerPropertyGroup] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[CustomerPropertyGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountUserCard]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountUserCard](
	[CardID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[PAN] [varchar](10) NULL,
	[PANHash] [varchar](500) NULL,
	[CardExpiry] [varchar](4) NULL,
	[UserID] [int] NULL,
	[RegisteredTS] [datetime] NULL,
	[CardNumLast4] [varchar](4) NOT NULL,
 CONSTRAINT [PK_DiscountUserCard] PRIMARY KEY CLUSTERED 
(
	[CardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeEmailTemplate]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountSchemeEmailTemplate](
	[DiscountSchemeEmailTemplateId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[EmailTemplateTypeId] [int] NOT NULL,
	[EmailText] [varchar](2000) NOT NULL,
	[EmailSubject] [varchar](2000) NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_DiscountSchemeEmailTemplate] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeEmailTemplateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EnforceRoute]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EnforceRoute](
	[EnfRouteId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_EnforceRoute] PRIMARY KEY CLUSTERED 
(
	[EnfRouteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_EnforceRoute] UNIQUE NONCLUSTERED 
(
	[EnfRouteId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FileTypeMap]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FileTypeMap](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[MechanismId] [int] NOT NULL,
	[FileType] [int] NOT NULL,
 CONSTRAINT [PK_FileTypeMap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_FileTypeMap] UNIQUE NONCLUSTERED 
(
	[MechanismId] ASC,
	[FileType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CollRoute]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollRoute](
	[CollRouteId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_CollRoute] PRIMARY KEY CLUSTERED 
(
	[CollRouteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_CollRoute] UNIQUE NONCLUSTERED 
(
	[CollRouteId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountScheme]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountScheme](
	[DiscountSchemeID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SchemeName] [varchar](500) NOT NULL,
	[SchemeType] [int] NULL,
	[DiscountPercentage] [int] NULL,
	[DiscountMinute] [int] NULL,
	[MaxAmountInCent] [int] NULL,
	[ActivationDate] [datetime] NULL,
	[ExpirationDate] [datetime] NULL,
	[DiscountSchemeExpirationTypeId] [int] NULL,
	[CustomerId] [int] NULL,
	[IsDisplay] [bit] NOT NULL,
 CONSTRAINT [PK_DiscountScheme] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DemandZoneCustomer]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DemandZoneCustomer](
	[DemandZoneCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DemandZoneId] [int] NOT NULL,
 CONSTRAINT [PK_DemandZoneCustomer] PRIMARY KEY CLUSTERED 
(
	[DemandZoneCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CoinDenominationCustomer]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CoinDenominationCustomer](
	[CoinDenominationCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CoinDenominationId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[CoinName] [varchar](255) NULL,
	[IsDisplay] [bit] NOT NULL,
	[CoinTypeOrdinal] [int] NULL,
 CONSTRAINT [PK_CoinDenominationCustomer] PRIMARY KEY CLUSTERED 
(
	[CoinDenominationCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CreditCardTypesCustomer]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CreditCardTypesCustomer](
	[CreditCardTypesCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CreditCardType] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_CreditCardTypesCustomer] PRIMARY KEY CLUSTERED 
(
	[CreditCardTypesCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomGroup3]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomGroup3](
	[CustomGroupId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_CustomGroup_3] PRIMARY KEY CLUSTERED 
(
	[CustomGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_CustomGroup_3] UNIQUE NONCLUSTERED 
(
	[CustomGroupId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomGroup2]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomGroup2](
	[CustomGroupId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DisplayName] [varchar](50) NOT NULL,
	[Comment] [varchar](50) NULL,
 CONSTRAINT [PK_CustomGroup_2] PRIMARY KEY CLUSTERED 
(
	[CustomGroupId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_CustomGroup_2] UNIQUE NONCLUSTERED 
(
	[CustomGroupId] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CollectionRunVendor]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollectionRunVendor](
	[CollectionRunVendorId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CollectionRunVendorName] [varchar](200) NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_CollectionRunVendor] PRIMARY KEY CLUSTERED 
(
	[CollectionRunVendorId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertMeterDiagnostic]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertMeterDiagnostic] 
	@CustomerId INT,
	@AreaId INT,
	@MeterId INT,
	@StatusDateTime DATETIME,
	@diagnostictype INT,
	@DiagnosticValue VARCHAR(500),
	@EventUid BIGINT,
	@TimeType1 INT,
	@TimeType2 INT,
	@TimeType3 INT,
	@TimeType4 INT
AS
BEGIN
	INSERT INTO meterdiagnostic
		(customerid,
		areaid,
		meterid,
		diagtime,
		receivedtime,
		diagnostictype,
		diagnosticvalue,
		eventuid,
		timetype1,
		timetype2,
		timetype3,
		timetype4)
		VALUES      
		(@CustomerId,
		 @AreaId,
		 @MeterId,
		 @StatusDateTime,
		 Getdate(),
		 @diagnostictype,
		 @DiagnosticValue,
		 @EventUid,
		 @TimeType1,
		 @TimeType2,
		 @TimeType3,
		 @TimeType4)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertEventCodeMaster]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROC [dbo].[sp_InsertEventCodeMaster]
	  @EventCode int,
	  @IsAlarm bit,
	  @AlarmTier int,
	  @EventDescAbbrev varchar(200),
	  @EventDescVerbose varchar(200),
	  @EventSource int,
	  @EventType int
	  as
	begin 
		if not exists(select * from EventCodeMaster where EventCode = @EventCode)
			begin
				INSERT INTO EventCodeMaster(EventSource,EventCode,AlarmTier,EventDescAbbrev,EventDescVerbose,IsAlarm,EventType) VALUES
				(@EventSource,@EventCode,@AlarmTier,@EventDescAbbrev,@EventDescVerbose,@IsAlarm,@EventType)
				Print convert(varchar,@EventCode) + ' - ' + @EventDescVerbose + ' inserted into EventCodeMaster.'				
			end	
		else
			begin
				if exists(select * from EventCodeMaster where EventCode = @EventCode and 
					(EventSource<>@EventSource or AlarmTier<>@AlarmTier 
					or EventDescAbbrev<>@EventDescAbbrev or EventDescVerbose<>@EventDescVerbose
					or IsAlarm<>@IsAlarm or EventType<>@EventType))
					begin
						update EventCodeMaster
							set EventSource=@EventSource,
							AlarmTier=@AlarmTier,
							EventDescAbbrev=@EventDescAbbrev,
							EventDescVerbose=@EventDescVerbose,
							IsAlarm=@IsAlarm,
							EventType=@EventType
						where EventCode = @EventCode
						Print convert(varchar,@EventCode) + ' - ' + @EventDescVerbose + ' updated into EventCodeMaster.'				
					end									
			end
	end
GO
/****** Object:  Table [dbo].[Report]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Report](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ReportId] [int] NOT NULL,
	[Description] [varchar](255) NULL,
	[ReportTemplateRaw] [image] NULL,
	[CustomerID] [int] NULL,
 CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFMeteredSpace]    Script Date: 04/01/2014 22:07:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFMeteredSpace](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFScheduleId] [int] NOT NULL,
	[SFMeterMapId] [int] NOT NULL,
	[EventTypeId] [int] NULL,
	[BasePrice] [float] NULL,
	[Processed] [datetime] NULL,
 CONSTRAINT [PK_SFMeteredSpace] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_MeterPushSchedule]    Script Date: 04/01/2014 22:07:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_MeterPushSchedule]
/*
This sp is used for dual-purpose.
For retrieval, @pushId should be null.
For removal (upon ack from meter), @pushId should be provided.
*/
	@pushId bigint	
AS	
BEGIN
	if (@pushId is not null) begin
		-- REMOVAL
		Update MeterPushSchedule 
		set Acknowledged = GETDATE()
		where PamPushId = @pushId
		
	end 
	else begin
		-- RETRIEVAL
		Update MeterPushSchedule
		set LastPushedTime = GETDATE()
		where PamPushId in (select PamPushId from UDP_MeterPushSchedule)
				
		Select * from UDP_MeterPushSchedule
	end
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_MeterDiagnostic]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_MeterDiagnostic]
	 @cid int
	,@aid int
	,@mid int
	,@diagTime int
	,@diagType int
	,@diagValue varchar(100)	
AS	
Declare 
	@DTime datetime
BEGIN
	set @DTime = DATEADD(second, @diagTime,'2000/01/01')
	if not exists(select * from MeterDiagnostic 
		where CustomerID = @cid 
		and AreaId  = @aid
		and MeterId = @mid
		and DiagTime = @DTime
		and DiagnosticType = @diagType
	)begin
		INSERT INTO [MeterDiagnostic]
           ([CustomerID]
           ,[AreaId]
           ,[MeterId]
           ,[DiagTime]
           ,[ReceivedTime]
           ,[DiagnosticType]
           ,[DiagnosticValue])
     VALUES
           (@cid
           ,@aid
           ,@mid
           ,@DTime
           ,GETDATE()
           ,@diagType
           ,@diagValue)		
	end	
END
GO
/****** Object:  Table [dbo].[TariffRateForConfiguration]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TariffRateForConfiguration](
	[TariffRatesId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TariffRateConfigurationId] [bigint] NOT NULL,
	[TariffRateId] [bigint] NOT NULL,
 CONSTRAINT [PK_TariffRates] PRIMARY KEY CLUSTERED 
(
	[TariffRatesId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsAcquirerResp]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsAcquirerResp](
	[TransAcquirerID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransAuditID] [bigint] NOT NULL,
	[AcquirerResponseCode] [int] NULL,
	[AcquirerResponseDetail] [varchar](200) NULL,
	[AcquirerTransactionRef] [varchar](50) NULL,
	[transType] [varchar](8) NOT NULL,
 CONSTRAINT [PK_TransactionsAcquirerResp] PRIMARY KEY CLUSTERED 
(
	[TransAcquirerID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TechnicianKeyChangeLog]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TechnicianKeyChangeLog](
	[CustomerId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BitNumber] [int] NOT NULL,
	[LastChange] [datetime] NOT NULL,
	[IsOccupied] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubAreas]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SubAreas](
	[CustomerID] [int] NOT NULL,
	[SubAreaID] [int] NOT NULL,
	[SubAreaName] [varchar](16) NOT NULL,
	[Description] [varchar](50) NULL,
	[ParentAreaID] [int] NULL,
	[SubAreaState] [int] NULL,
 CONSTRAINT [PK_SubAreas] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[SubAreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SupportedCreditCards]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SupportedCreditCards](
	[CustomerID] [int] NOT NULL,
	[CreditCardType] [int] NOT NULL,
	[BankID] [int] NOT NULL,
	[MerchantID] [int] NOT NULL,
 CONSTRAINT [PK_SupportedCreditCards] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[CreditCardType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsReconcile]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionsReconcile](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ReceiptNo] [int] NOT NULL,
	[TransDateTime] [datetime] NULL,
	[TransactionID] [bigint] NOT NULL,
	[CardTypeCode] [int] NOT NULL,
 CONSTRAINT [PK_TransactionsReconcile] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[ReceiptNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsReconcile_IDXGlobalMeterID] ON [dbo].[TransactionsReconcile] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[webpages_UsersInRoles]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[webpages_UsersInRoles](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrdersAudit]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrdersAudit](
	[WorkOrderAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[AuditDateTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[ReportingUserId] [int] NULL,
	[MeterGroup] [int] NULL,
	[CustomerId] [int] NULL,
	[AreaId] [int] NULL,
	[MeterId] [int] NULL,
	[Location] [varchar](500) NULL,
	[ParkingSpaceId] [bigint] NULL,
	[Notes] [varchar](2000) NULL,
	[ZoneId] [int] NULL,
	[HighestSeverity] [int] NULL,
	[CreateDateTime] [datetime] NULL,
	[SLADue] [datetime] NULL,
	[Status] [int] NULL,
	[TechnicianId] [int] NULL,
	[AssignedDate] [datetime] NULL,
	[CompletionDate] [datetime] NULL,
	[CreatedById] [int] NULL,
	[CrossStreet] [varchar](2000) NULL,
	[ResolutionCode] [int] NULL,
	[ResolutionDesc] [varchar](2000) NULL,
	[AssignmentState] [int] NULL,
	[Mechanism] [int] NULL,
 CONSTRAINT [PK_WorkOrdersAudit] PRIMARY KEY CLUSTERED 
(
	[WorkOrderAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[WorkOrderImage]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WorkOrderImage](
	[WorkOrderImageId] [bigint] NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[ImageData] [image] NULL,
	[DateTaken] [datetime] NULL,
 CONSTRAINT [PK_WorkOrderImage] PRIMARY KEY CLUSTERED 
(
	[WorkOrderImageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrderEvent]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderEvent](
	[WorkOrderEventId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[EventId] [bigint] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventDateTime] [datetime] NULL,
	[SLADue] [datetime] NULL,
	[EventDesc] [varchar](50) NULL,
	[AlarmTier] [int] NULL,
	[Notes] [varchar](2000) NULL,
	[Automated] [bit] NULL,
	[Vandalism] [bit] NULL,
	[Status] [int] NULL,
 CONSTRAINT [PK_WorkOrderEvent] PRIMARY KEY CLUSTERED 
(
	[WorkOrderEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ZoneSeq]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZoneSeq](
	[ZoneId] [int] NOT NULL,
	[ZoneSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
	[CustomerId] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WorkOrderPart]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[WorkOrderPart](
	[WorkOrderPartId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[WorkOrderId] [bigint] NOT NULL,
	[PartId] [bigint] NULL,
	[Quantity] [int] NULL,
	[Notes] [varchar](255) NULL,
 CONSTRAINT [PK_WorkOrderPart] PRIMARY KEY CLUSTERED 
(
	[WorkOrderPartId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TransactionBatch]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionBatch](
	[TransactionBatchId] [bigint] IDENTITY(1,1) NOT NULL,
	[TransactionID] [bigint] NOT NULL,
	[BatchRef] [varchar](20) NOT NULL,
	[Status] [int] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SubAreaSeq]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubAreaSeq](
	[CustomerID] [int] NOT NULL,
	[SubAreaID] [int] NOT NULL,
	[SubAreaSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL,
 CONSTRAINT [PK_SubAreaSeq] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[SubAreaID] ASC,
	[SubAreaSeq] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SensorPaymentTransaction]    Script Date: 04/01/2014 22:07:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SensorPaymentTransaction](
	[SensorPaymentTransactionID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[ArrivalTime] [datetime] NOT NULL,
	[ArrivalPSOAuditId] [bigint] NOT NULL,
	[DepartureTime] [datetime] NULL,
	[DeparturePSOAuditId] [bigint] NULL,
	[FirstTxPaymentTime] [datetime] NULL,
	[FirstTxStartTime] [datetime] NULL,
	[FirstTxExpiryTime] [datetime] NULL,
	[FirstTxAmountInCent] [int] NULL,
	[FirstTxTimePaidMinute] [int] NULL,
	[FirstTxPaymentMethod] [int] NULL,
	[FirstTxID] [int] NULL,
	[LastTxPaymentTime] [datetime] NULL,
	[LastTxExpiryTime] [datetime] NULL,
	[LastTxAmountInCent] [int] NULL,
	[LastTxTimePaidMinute] [int] NULL,
	[LastTxPaymentMethod] [int] NULL,
	[LastTxID] [int] NULL,
	[TotalAmountInCent] [int] NULL,
	[TotalNumberOfPayment] [int] NULL,
	[TotalTimePaidMinute] [int] NULL,
	[TotalOccupiedMinute] [int] NULL,
	[DiscountSchema] [int] NULL,
	[GracePeriodMinute] [int] NULL,
	[ViolationMinute] [int] NULL,
	[OccupancyStatus] [int] NULL,
	[NonCompliantStatus] [int] NULL,
	[RemaingPaidTimeMinute] [int] NULL,
	[ZeroOutTime] [datetime] NULL,
	[OperationalStatus] [int] NULL,
	[InfringementLink] [varchar](500) NULL,
	[RecordCreatTime] [datetime] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
	[FreeParkingMinute] [int] NULL,
	[OccupancyDate] [datetime] NULL,
	[ViolationSegmentCount] [int] NULL,
	[FreeParkingTime] [int] NULL,
	[GracePeriodUsed] [bit] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[RecordUpdateTime] [datetime] NULL,
	[SensorId] [int] NULL,
	[GatewayId] [int] NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_SensorPaymentTransaction] PRIMARY KEY CLUSTERED 
(
	[SensorPaymentTransactionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [IDX_OccupancyRateSummary_Perf2] ON [dbo].[SensorPaymentTransaction] 
(
	[ArrivalTime] ASC
)
INCLUDE ( [SensorPaymentTransactionID],
[ParkingSpaceId],
[DepartureTime],
[LastTxExpiryTime],
[TotalTimePaidMinute],
[TotalOccupiedMinute],
[OccupancyStatus],
[NonCompliantStatus],
[OperationalStatus],
[TimeType1],
[TimeType2],
[TimeType3],
[TimeType4],
[TimeType5],
[FreeParkingMinute],
[FreeParkingTime],
[FreeParkingUsed]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IDX_OccupancyRateSummary_Perf3] ON [dbo].[SensorPaymentTransaction] 
(
	[ParkingSpaceId] ASC,
	[ArrivalTime] ASC
)
INCLUDE ( [SensorPaymentTransactionID],
[DepartureTime],
[LastTxExpiryTime],
[TotalTimePaidMinute],
[TotalOccupiedMinute],
[OccupancyStatus],
[NonCompliantStatus],
[OperationalStatus],
[TimeType1],
[TimeType2],
[TimeType3],
[TimeType4],
[TimeType5],
[FreeParkingMinute],
[FreeParkingTime],
[FreeParkingUsed]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_Occupancy_Time] ON [dbo].[SensorPaymentTransaction] 
(
	[ArrivalTime] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_PopulateSensorPaymentTransaction]    Script Date: 04/01/2014 22:07:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PopulateSensorPaymentTransaction]
as
	Declare cur cursor 
		for select top 2000
			TransactionsID
			,CustomerID
           ,AreaID
           ,MeterID
           ,ZoneID
           ,ParkingSpaceID
           ,SensorID
           ,TransactionType
           ,TransDateTime
           ,ExpiryTime
           ,AmountInCents
           ,TxValue
           ,TimePaid
           ,TimeType1
           ,TimeType2
           ,TimeType3
           ,TimeType4
           ,TimeType5
           ,EventUID
           ,BayNumber
           ,CreditCardType
           ,TransactionStatus
           ,ReceiptNo
           ,OriginalTxId
           ,TransType
           ,MeterGroupId
           ,GatewayId
           ,SensorPaymentTransactionId
           ,PrepayUsed
           ,FreeParkingUsed
           ,DiscountSchemeId
			from Transactions 
			with (NOLOCK)
			where SensorPaymentTransactionId is null
			--and MeterID = 9888
			-- TRANSACTIONS OCCURED IN LAST 5 DAYS ONLY
			--and TransDateTime > dateadd(D,-2,getdate())
			order by TransDateTime 
	

	Declare 
		@TransactionsID bigint
		,@ParkingSpaceId bigint
		,@ArrivalTime datetime
		,@ArrivalPSOAuditId bigint
		,@DepartureTime datetime
		,@DeparturePSOAuditId bigint
		,@FirstTxPaymentTime datetime
		,@FirstTxStartTime datetime
		,@FirstTxExpiryTime datetime
		,@FirstTxAmountInCent int
		,@FirstTxTimePaidMinute int
		,@FirstTxPaymentMethod int
		,@FirstTxID int
		,@LastTxPaymentTime datetime
		,@LastTxExpiryTime datetime
		,@LastTxAmountInCent int
		,@LastTxTimePaidMinute int
		,@LastTxPaymentMethod int
		,@LastTxID int
		,@TotalAmountInCent int
		,@TotalNumberOfPayment int
		,@TotalTimePaidMinute int
		,@TotalOccupiedMinute int
		,@DiscountSchema int
		,@GracePeriodMinute int
		,@ViolationMinute int
		,@OccupancyStatus int
		,@NonCompliantStatus int
		,@RemaingPaidTimeMinute int
		,@ZeroOutTime datetime
		,@OperationalStatus int
		,@InfringementLink varchar(500)
		,@RecordCreatTime datetime
		,@TimeType1 int
		,@TimeType2 int
		,@TimeType3 int
		,@TimeType4 int
		,@TimeType5 int
		,@FreeParkingMinute int
		,@OccupancyDate datetime
		,@ViolationSegmentCount int
		,@FreeParkingTime int
		,@GracePeriodUsed bit
		,@PrepayUsed bit
		,@FreeParkingUsed bit
		,@SensorPaymentTransactionId bigint 
		,@CustomerID int
       ,@AreaID int
       ,@MeterID int
       ,@ZoneID int
       ,@SensorID int
       ,@TransactionType int
       ,@TransDateTime datetime
       ,@ExpiryTime datetime
       ,@AmountInCents int
       ,@TxValue varchar(50)
       ,@TimePaid int
       ,@EventUID bigint
       ,@BayNumber int
       ,@CreditCardType int
       ,@TransactionStatus int
       ,@ReceiptNo int
       ,@OriginalTxId bigint
       ,@TransType int
       ,@MeterGroupId int
       ,@GatewayId int
       ,@DiscountSchemeId int
       ,@RecordUpdateTime datetime
       ,@Gp int
       ,@Flag bit
       ,@ErrMsg Varchar(2000)
	
BEGIN
	Print 'start'
	
	
	
	open cur 
	
	fetch next from cur  into @TransactionsID,@CustomerID,@AreaID,@MeterID,@ZoneID
				,@ParkingSpaceID,@SensorID,@TransactionType,@TransDateTime,@ExpiryTime
				,@AmountInCents,@TxValue,@TimePaid
				,@TimeType1,@TimeType2,@TimeType3,@TimeType4,@TimeType5
				,@EventUID,@BayNumber,@CreditCardType,@TransactionStatus,@ReceiptNo
				,@OriginalTxId,@TransType
				,@MeterGroupId,@GatewayId
				,@SensorPaymentTransactionId
				,@PrepayUsed,@FreeParkingUsed,@DiscountSchemeId
                      
	Print 'cursor opened'
		Print @@FETCH_STATUS         
		WHILE @@FETCH_STATUS = 0
		BEGIN			
			Print '-------------------'
			BEGIN TRY
			
				set @Gp = 15 -- TODO -- get it from Customer table
				
				if (@TransactionType is not null) begin
					Print 'Tx Type = ' + convert(varchar,@TransactionType)
				end else begin
					Print 'Tx Type is null'
				end
			
				--Determine arrival time first
				if @TransactionType=21 and @TxValue='IN' begin  -- 21=SENSOR			
					set @ArrivalTime = @TransDateTime
				end else begin
					--Latest arrival time just before the TransDateTime
					select @ArrivalTime =  max(ArrivalTime)
						from SensorPaymentTransaction 
						where ArrivalTime < @TransDateTime
						and ParkingSpaceId = @ParkingSpaceId
					
					--Get the @SensorPaymentTransactionID at the same time
					select @SensorPaymentTransactionID = SensorPaymentTransactionID
						,@FirstTxPaymentTime = FirstTxPaymentTime
						,@LastTxExpiryTime = LastTxExpiryTime
						,@DepartureTime = DepartureTime
						,@GracePeriodMinute = GracePeriodMinute
						,@GracePeriodUsed = GracePeriodUsed
						from SensorPaymentTransaction s
						where s.ParkingSpaceId = @ParkingSpaceId
						and s.ArrivalTime = @ArrivalTime					
				end 
							
				if (@ArrivalTime is not null) begin 
					print '@ArrivalTime=' + convert(varchar,@ArrivalTime,0)				
				end else begin
					Print '@ArrivalTime is null'
				end
				
				if (@SensorPaymentTransactionID is not null) begin 
					print 'SPTxID = ' + convert(varchar,@SensorPaymentTransactionID)				
				end else begin 
					Print 'SPTxID is null'
				end
				
				if @TransactionType=21 begin  -- 21=SENSOR			
				
					if @TxValue = 'IN' begin	
						Print '@TxValue = IN'							
						set @ArrivalTime = @TransDateTime
						set @ArrivalPSOAuditId = @OriginalTxId
						set @OccupancyStatus = 1 --Occupied
						set @NonCompliantStatus = 2 --Unpaid at Meter
						set @OperationalStatus = 1 --Operational
						set @RecordCreatTime = GETDATE()
						set @OccupancyDate = @TransDateTime
						
						exec sp_TimeTypeLogic @TransDateTime,@TimeType1 output,@TimeType2 output,@TimeType3 output,@TimeType4 output,@TimeType5 output
						
						print 'Arrival time' + convert(varchar,@ArrivalTime,0)				
						INSERT INTO SensorPaymentTransaction
						   (ParkingSpaceId
						   ,ArrivalTime
						   ,ArrivalPSOAuditId
						   ,OccupancyStatus
						   ,NonCompliantStatus
						   ,OperationalStatus
						   ,InfringementLink
						   ,RecordCreatTime
						   ,OccupancyDate
						   ,TimeType1
						   ,TimeType2
						   ,TimeType3
						   ,CustomerId
						   )
						VALUES
						   (@ParkingSpaceId
						   ,@ArrivalTime
						   ,@ArrivalPSOAuditId
						   ,@OccupancyStatus
						   ,@NonCompliantStatus
						   ,@OperationalStatus
						   ,@InfringementLink
						   ,@RecordCreatTime
						   ,@OccupancyDate
						   ,@TimeType1
						   ,@TimeType2
						   ,@TimeType3
						   ,@CustomerID
						   )			           
						set @SensorPaymentTransactionId =  SCOPE_IDENTITY()
					
					end --if @TxValue = 'IN'
					
					if @TxValue = 'OUT' begin
						Print '@TxValue = OUT'
						set @DeparturePSOAuditId = @OriginalTxId
						set @DepartureTime = @TransDateTime
						set @OccupancyStatus = 2 --Vacant
						set @RecordUpdateTime= GETDATE()
						set @TotalOccupiedMinute = DATEDIFF(MINUTE,@ArrivalTime,@DepartureTime)
						
						print 'Departuretime' + convert(varchar,@ArrivalTime,0)				
						
						Update s
							set s.DepartureTime = @DepartureTime
							,s.DeparturePSOAuditId = @OriginalTxId
							,s.OccupancyStatus = @OccupancyStatus
							,s.NonCompliantStatus = @NonCompliantStatus
							,s.RecordUpdateTime = @RecordUpdateTime
							,s.TotalOccupiedMinute = @TotalOccupiedMinute
							,s.OccupancyDate = @TransDateTime
							,s.GracePeriodUsed  = @GracePeriodUsed
							,s.GracePeriodMinute = @GracePeriodMinute
							,s.RemaingPaidTimeMinute = @RemaingPaidTimeMinute
							from SensorPaymentTransaction s
							where s.SensorPaymentTransactionID = @SensorPaymentTransactionId
					end --if @TxValue = 'OUT'			
				end else if @TransactionType=10 begin  -- 10=Free Parking
					print 'Free Parking' + convert(varchar,@ArrivalTime,0)
					Update s
						set s.FreeParkingUsed = 1 -- true
						,s.FreeParkingMinute = @TimePaid
						from SensorPaymentTransaction s
						where s.SensorPaymentTransactionID = @SensorPaymentTransactionId
				end else begin -- other (payment) transactions
					if (@SensorPaymentTransactionId is not null) begin
						print 'Arrival time ' + convert(varchar,@ArrivalTime,0)			
						print '@SensorPaymentTransactionId  ' + convert(varchar,@SensorPaymentTransactionId)			
						if @FirstTxPaymentTime is null begin
							/*
							At this time, we dont know how many transacation occured for each occupancy.
							That's why we update both first and last.
							*/
							Print 'Updating first and last payment' + convert(varchar,@SensorPaymentTransactionId)
							Update s
								set s.FirstTxPaymentTime = @TransDateTime
								,s.FirstTxStartTime = @TransDateTime
								,s.FirstTxExpiryTime = @ExpiryTime
								,s.FirstTxAmountInCent = @AmountInCents
								,s.FirstTxTimePaidMinute = @TimePaid
								,s.FirstTxPaymentMethod = @TransactionType
								,s.FirstTxID = @OriginalTxId
								,s.LastTxPaymentTime = @TransDateTime
								,s.LastTxExpiryTime = @ExpiryTime
								,s.LastTxAmountInCent = @AmountInCents
								,s.LastTxTimePaidMinute = @TimePaid
								,s.LastTxPaymentMethod = @TransactionType
								,s.LastTxID = @OriginalTxId
								,s.TotalAmountInCent = @AmountInCents
								,s.TotalNumberOfPayment = 1
								,s.TotalTimePaidMinute = @TimePaid
								,s.DiscountSchema = @DiscountSchemeId					
								from SensorPaymentTransaction s
								where s.SensorPaymentTransactionID = @SensorPaymentTransactionId
							
						end else begin 
							Print 'Updating last payment' + convert(varchar,@SensorPaymentTransactionId) + ' ' + convert(varchar,@TransDateTime,9) 
							Update s
								set s.LastTxPaymentTime = @TransDateTime
								,s.LastTxExpiryTime = @ExpiryTime
								,s.LastTxAmountInCent = @AmountInCents
								,s.LastTxTimePaidMinute = @TimePaid
								,s.LastTxPaymentMethod = @TransactionType
								,s.LastTxID = @OriginalTxId
								,s.TotalNumberOfPayment = s.TotalNumberOfPayment + 1 -- Increase payment count
								,s.TotalAmountInCent = s.TotalAmountInCent + @AmountInCents	--  Accumulate payment 		
								,s.TotalTimePaidMinute = s.TotalTimePaidMinute + @TimePaid	-- TODO - Questionable																	
								from SensorPaymentTransaction s
								where s.SensorPaymentTransactionID = @SensorPaymentTransactionId				
						end 
				end	else begin
					Print '@SensorPaymentTransactionId is null'
				end		
				end
				
				Print 'finalising tx'
				
				
			 -- finalise the tx here			 
				if (@SensorPaymentTransactionId is not null) begin
					print '@SensorPaymentTransactionId  ' + convert(varchar,@SensorPaymentTransactionId)			
					Print 'updating for SS ' + convert(varchar,@SensorPaymentTransactionId)
					
					Update Transactions 
					set SensorPaymentTransactionId = @SensorPaymentTransactionId
					where TransactionsID = @TransactionsID	
					
					print 'Updating SPTx'
					Update SensorPaymentTransaction
					set RemaingPaidTimeMinute = (case when LasttxExpiryTime > DepartureTime then DATEDIFF(MINUTE,DepartureTime,LastTxExpiryTime) Else 0 END) 
					,GracePeriodUsed = (case when DepartureTime > LasttxExpiryTime then 1 else 0 end)
					,GracePeriodMinute = (case when DepartureTime > LasttxExpiryTime then DATEDIFF(MINUTE,LasttxExpiryTime,DepartureTime) else 0 end)
					,NonCompliantStatus = (case when FirstTxID IS null then 2 -- Unpaid
												else (	case when DepartureTime > DATEADD(Minute,@gp,LasttxExpiryTime) then 1 -- Overstay at Meter
															else null 
														end) 
											end)				
					where SensorPaymentTransactionId = @SensorPaymentTransactionId
					
				end else begin
					Print 'SPTxId is null. Tx not finalised'
				end 
				
				
			END TRY
			BEGIN CATCH
				Print 'Error in loop ' + Error_message()				
			END CATCH
			
			fetch next from cur  into @TransactionsID,@CustomerID,@AreaID,@MeterID,@ZoneID
					,@ParkingSpaceID,@SensorID,@TransactionType,@TransDateTime,@ExpiryTime
					,@AmountInCents,@TxValue,@TimePaid
					,@TimeType1,@TimeType2,@TimeType3,@TimeType4,@TimeType5
					,@EventUID,@BayNumber,@CreditCardType,@TransactionStatus,@ReceiptNo
					,@OriginalTxId,@TransType
					,@MeterGroupId,@GatewayId
					,@SensorPaymentTransactionId
					,@PrepayUsed,@FreeParkingUsed,@DiscountSchemeId
		END	--while
	CLOSE cur 
	
	DEALLOCATE cur 

	Print 'end of process'
	
END
GO
/****** Object:  StoredProcedure [dbo].[spAddRemoveBLCard]    Script Date: 04/01/2014 22:07:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spAddRemoveBLCard]
	@action int,
	@BlackListID int,
	@HCardNum varchar(40),
	@XCardNum varchar(16),
	@Code int,
	@CustomerID int,
	@UserID int
AS
    if @action = 0 begin
        INSERT BlackListActive
            values (@HCardNum,@XCardNum,@Code,GETDATE(),@CustomerID)
    end
    else begin
        DELETE BlackListActive
            where BlackListID=@BlackListID
    end
    INSERT BlackListArchive
        values (@XCardNum,@Code,@Action,GETDATE(), @UserID)
GO
/****** Object:  StoredProcedure [dbo].[spAddEventCodes]    Script Date: 04/01/2014 22:07:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spAddEventCodes]
As
    DECLARE @CustomerID int
    SELECT @CustomerID=ParameterValue FROM ReinoParameters WHERE ParameterID='CustomerID'

    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,0,0,'AlarmClear','Alarms Cleared at Meter')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,1,1,'CBoxOpen','Cashbox Door Opened')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,2,0,'CBoxRmvd','Cashbox Removed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,3,2,'NewCBox','New Cashbox Inserted')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,4,2,'CBoxClose','Cashbox Door Closed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,5,1,'CBoxFull','Cashbox Full Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,6,2,'Cash','Cash Transaction')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,7,2,'Ccard','Cashless Transaction')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,8,2,'MasterKey','Master Key Log')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,9,2,'KeyLog','Normal Key Log')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,10,2,'BayCredit','Bay Credit Allocated')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,11,2,'Restarted','Meter Restarted')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,12,2,'LogFault','Fault Log')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,13,2,'LogWarn','Warning Log')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,14,2,'LogAlarm','Alarm Log')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,15,1,'TopOpen','Top Door Opened')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,16,2,'Maintenance','Maintenance State Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,17,2,'TopClose','Top Door Closed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,18,0,'TopAlarm','Top Door Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,19,2,'TopTamper','Top Door Tamper Lockout')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,20,2,'Time','Time Stamp')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,21,2,'Return','Return To Service')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,22,2,'Unrestricted','Unrestricted Mode Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,23,2,'PrePay','Pre Pay Mode Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,24,2,'Regulated','Regulated Mode Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,25,2,'NoParking','No Parking Mode Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,26,2,'MixedBay','Mixed Bay Mode Entered')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,27,2,'Voltage','Daily Voltage Measurement')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,28,2,'Inspect','Inspection Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,32,1,'TopOpen','Top Door Open Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,33,2,'TopClose','Top Door Closed Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,34,1,'VaultOpen','Vault Door Open Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,35,2,'VaultClose','Vault Door Closed Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,36,0,'MaxJamAl','Max Jam - Validator Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,37,0,'JamInputAl','Jam - Input Opto Sensor Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,38,0,'JamRejectAl','Jam - Reject Opto Sensor Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,39,0,'JamAcceptAl','Jam - Accept Opto Sensor Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,40,1,'JamAlarmAl','Jam - Validator Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,41,0,'JamStuckAl','Jam - Coin Stuck Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,42,0,'MaxJamStuckAl','Max Jam - Coin Stuck Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,43,2,'CoinFallenAl','Coin Fallen Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,44,1,'MaxFallenAl','Max Coin Fallen Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,45,1,'ReBattery','Rechargeable Battery Warning')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,46,1,'DryBattery','Dry Battery Warning')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,47,2,'JamValidatorEv','Jam - Validator Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,48,2,'RejectEv','Reject Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,49,2,'AcceptEv','Accept Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,50,2,'JamInputEv','Jam - Input Opto Sensor Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,51,2,'JamStuckEv','Jam - Coin Stuck Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,52,2,'JamAcceptEv','Jam - Accept Opto Sensor Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,53,2,'JamRejectEv','Jam - Reject Opto Sensor Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,54,2,'ARM','ARM Activated Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,55,2,'OutputEv','Output Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,56,2,'MaxJamEv','Max Jam - Validator Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,57,2,'CoinClear','Coin Clear Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,58,2,'CoinRacePulse','Capacitive Coin Race Pulse Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,59,2,'MaxJamStuckEv','Max Jam - Coin Stuck Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,60,2,'ConFallenEv','Coin Fallen Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,61,2,'MaxFallenEv','Max Coin Fallen Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,62,2,'CCPaid','Credit Card Paid Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,63,2,'CCInvalid','Credit Card Invalid Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,64,2,'CCExpire','Credit Card Expired Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,65,2,'CCReadErr','Credit Card Read Error Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,66,2,'GSMcall','GSM Call Initiated')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,67,2,'GSMcallRx','GSM Call Rx Cli Ok')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,68,2,'GSMCliBad','GSM Call Rx Cli Not Ok')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,69,2,'GSMReg','GSM Network Reg Status')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,70,2,'GSMsignal','GSM Signal Level')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,71,2,'GSMbusy','GSM Phone Busy')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,72,2,'GSMNoTone','GSM No Dial Tone')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,73,2,'GSMNoCarry','GSM No Carrier')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,74,2,'GSMBadmodem','GSM Unexpected Modem Response')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,75,2,'GSMBadExit','GSM Unexpected Disconnect')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,76,2,'GSMmissingExit','GSM Expected Disconnect')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,80,2,'noSMC','No Smart Media Card Found Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,81,2,'SMCnoBatch','Smart Media Batch Not Present Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,82,2,'SMCfull','Smart Media Batch Max Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,83,2,'SMClow','Smart Media 80% Batch Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,84,2,'SMCwriteErr','Smart Media Batch Write Error Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,85,2,'SMCreadErr','Smart Media Batch Read Error Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,86,2,'SMCLow','Smart Media Batch 80% Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,87,2,'HardwareEv','Hardware Report Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,88,2,'CboxAudit','Cashbox Audit Retrieval')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,89,2,'CboxHistory','Cashbox History Retrieval')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,90,0,'OutofOrder','Out Of Order')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,91,2,'Warning','Warning Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,92,2,'AlarmAction','Alarm Action Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,96,2,'Cctrans','Credit Card Transaction')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,97,2,'CCnoBatch','Credit Card Batch Not Present')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,98,2,'CCmaxBatch','Credit Card Max Batch Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,99,2,'CCLow','Credit Card 80% Batch Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,100,2,'CCwriteError','Credit Card Batch Write Error Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,101,2,'CCreadError','Credit Card Batch Read Error Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,102,2,'CCMinAmt','Credit Card Min Single Amount Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,103,2,'CCMaxAmt','Credit Card Max Single Amount Reached Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,104,2,'CCMaxTot','Credit Card Max Total Amount Reached')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,105,2,'CCLimit','Credit Card Limit No Transaction Reached')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,106,2,'CCBusy','Credit Card Processing')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,107,2,'CCtestCard','Credit Card - Test Card Sucessful')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,112,2,'BadBayConfig','Fault Log - Bay Config Corrupt')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,113,2,'MParkOK','Mobile Payment Accepted')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,114,2,'BayNotAval','Selected Bay Not Available')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,115,2,'SameCBox','Same Cashbox Inserted')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,116,0,'CBoxRemoved','Cashbox Removed Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,117,2,'CB Scan','Cashbox Scan Timestamp');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,118,2,'SMSTest','SMS Service Test Event');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,119,2,'InvSMS','Invalid SMS Event');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,120,2,'EmporiaTest','Emporia Test Event');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,121,2,'WTChdgRst','Watchdog Reset Occurred');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,122,2,'GSMRng','GSM Ring Detected');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,123,2,'SMSInd','GSM SMS Indication');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,124,2,'GSMRegFail','GSM Registration Failed');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,125,2,'GSMACallFail','GSM Alarm Call Failed');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,126,2,'GSMRstTmrFail','GSM Timer Restart Fail');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,127,2,'GSMSlepWkup','GSM Caused Sleep Wakeup');
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,128,2,'GsmAlmNoPhNoEcho','Gsm Alarm No Phone Num Echo')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,129,2,'GsmAtaEchoFailed','Gms ATA Echo Failed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,130,2,'GsmNoAnswerConn','Gsm No Answer Connection')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,131,2,'GsmChkSMSInbox','Gsm Check SMS In Box')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,132,2,'GsmSMSRdCmdTOut','Gsm SMS Read Command Timeout')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,133,2,'GsmSMSURdCmdTOut','Gsm SMS Unread Command Timeout')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,134,2,'GsmSMSDelCmdTOut','Gsm SMS Delete COmmand TimeOut')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,135,2,'SetGSMInactFlg','Set GSM Inactive Flag')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,136,2,'BL Deny','Blacklist- Card Denied')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,137,1,'BL Invalid','Blacklist- file Invalid/Not found')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,138,2,'BL Free','Blacklist- Free Transaction')
INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,139,1,'CB Full','Cashbox Full Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,140,2,'PkgCorrupt','Event Package Corrupt')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,141,2,'CredCrdEncry','CreditCard Trans Encrypted')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,142,2,'BL Denied','Blacklist card denied')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,143,2,'BL Freebie','Blacklist free card')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,144,2,'ECC error','SMC ErrCorrCode inconsistent')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,145,2,'DumpfileFull','NOT ENOUGH DUMPFILE SPACE FOR DOWNLAOD')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,146,2,'NoHandles','NOT ENOUGH HANDLES FOR DOWNLOAD')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,147,2,'ActQueueFull','ACTIVATION QUEUE FULL FOR DOWNLOAD')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,148,2,'ZModemStart','ZMODEM DOWNLOAD PROCESS START')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,149,2,'ZModemOK','ZMODEM DOWNLOAD SUCCESS')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,150,2,'ZModemFail','ZMODEM DOWNLOAD FAILED')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,151,2,'DumpHrdRErr','DUMPFILE HEADER READ ERROR')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,152,2,'DumpInvdHrd','INVALID DUMPFILE HEADER')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,153,2,'FileActvGen','FILE ACTIVATION LIST GENERATED')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,154,2,'FileActvStr','FILE ACTIVATION PROCESS START')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,155,2,'BadFilObHdr','INVALID FILEOBJECT HEADER')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,156,2,'BadBlackLOb','INVALID BLACKLIST OBJECT')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,157,2,'BadConfigOb','INVALID CONFIG OBJECT')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,158,2,'BadBinryObj','INVALID BINARY OBJECT')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,159,2,'BadFileCRC','FILEOBJECT CRC CORRUPTED')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,160,2,'BadFileActn','FILEOBJECT ACTION FAILED')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,161,2,'FileActvOK','FILE ACTIVATION SUCCESS')

   INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,163,2,'SmartCardTrans','Smartcard Transaction')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,164,2,'SmartCardNoFund','Smartcard transaction no funds on card')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,165,2,'SmartCardInv','Invalid smartcard')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,166,2,'SmartCardFail','Smartcard transaction failed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,167,2,'SmartCardRefund','Smartcard Refund')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,168,2,'SmartCardRefFail','SmartCard Refund Failed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,169,2,'SmartCrdRefNoBal','SmartCard no balance for refund')

    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,512,2,'FlshFillng','Flash Log Filling 95%')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,513,2,'FlshLogFul','Flash Log Full')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,514,2,'CPUReset','CPU external Reset')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,515,2,'CPUpwrRst','CPU powerOn Reset')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,516,2,'CPUHaltRst','CPU Halt Monitor Reset')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,517,2,'NoSaleTO','No Sale Timeout')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,518,2,'Escrow Jam','Escrow Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,519,2,'Escrow Fault','ESCROW Fault Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,520,2,'Card Rdr flt','Card Reader Fault')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,521,2,'CardRdrAlm','Card reader fault alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,522,2,'NoPrintComms','Printer-no comms')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,523,2,'PrtIdleTO','Printer-Idle Timeout')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,524,2,'PrtBusy','Printer-Busy')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,525,2,'PrtHeadUP','Printer-Head up')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,526,2,'PrtSrvOK','Printer-Service OK')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,527,2,'PrtNoBlackPoint','Printer-No Black Point')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,528,2,'PrtPaperOut','Printer-Paper Out')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,529,2,'PrtLowTickets','Printer-Low Tickets')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,530,2,'PrtTicketJam','Printer-Ticket Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,531,2,'PrtFault','Printer-Fault')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,532,0,'PrtFaultAlm','Printer-Fault Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,533,0,'PrtBlackPointALM','Print-Black Point Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,534,0,'PrtNoTicketALM','Printer-No Ticket Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,535,1,'PrtLowTicketALM','Printer-Low Ticket Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,536,1,'PrtTicketALM','Printer-Ticket Alarm')
   INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,537,2,'BattFlat','Flat Battery Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,544,2,'BattLow','Low Battery Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,545,2,'BattOK','Battery OK event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,546,0,'BattAlm','Battery Flat Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,547,2,'TstToken','Test Token')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,548,2,'GSMRst','GSM Restart Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,549,2,'GSMWkup','GSM Wakeup Event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,550,2,'BattLowAlarm','Battery Low Warning')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,551,2,'CBMemFlt','Cashbox Memory Fault')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,552,2,'PrtJamClr','Printer Jam Clear')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,553,2,'FileActFail','File activation failed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,554,2,'BlkRen','Failed blacklist rename')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,555,2,'ConfRen','Failed config rename')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,556,2,'CodeRen','Failed code rename')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,557,2,'ObjDisc','Older object code discarded')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,558,2,'CBFull','Cashbox full')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,559,2,'CBEmpty','Cashbox empty')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,560,2,'PrtNoFeed','Printer no feed')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,561,1,'PrtFeedAlm','Printer feed alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,562,2,'CBWarn','Cashbox warning')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,563,2,'CBWarnAlarm','Cashbox Full Warning')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,564,2,'LZTrans','Loading Zone transaction')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,565,2,'CJDetect','Coin jam detected event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,566,2,'CJFix','Coin jam fixed event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,567,2,'PrtMis','Printer misfeed event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,568,2,'ClkAdj','CLock Adjusted')
   INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,582,2,'NoValResp','No result from validator')
  INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,583,2,'TampDetect','Tampering detected in coin path')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,584,2,'CoinValJam','Coin validator jam detected')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,585,2,'CPJamThresh','Coin path jam threshold exceeded')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,586,2,'CPTamperThresh','Coin path tamper threshold exceeded')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,587,2,'TransAfterTamper','Successful purchases after tamper alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,588,1,'ValJamAlm','Jam in validator alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,589,1,'UprCoinPathAlm','Upper coin path warning alarm')
   INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,592,2,'DetApdTamperEvt','Detect APD tamper event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,593,1,'APDTamperAlm','APD Tamper Alarm')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,0,594,1,'RstAPDTamperEvt','APD tamper cleared event')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,0,0,'MeterOK','Meter OK')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,10,0,'CJ','Coin Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,20,1,'Fobj','Foreign Object')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,30,1,'Vandal','Vandal Damage')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,40,1,'NoPower','No Power')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,50,1,'Alarm','Alarm at Meter')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,60,0,'OOS','Out Of Service')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,1,90,0,'OutOfOrder','Out Of Order')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,0,0,'MeterOK','Meter OK')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,10,0,'Jam','Coin Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,20,1,'ForeignObj','Foreign Object')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,30,1,'Vandal','Vandal Damage-Operational')
  INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,31,1,'VDC','Vandal Damage-Cosmetic')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,40,1,'NoPower','No Power')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,50,1,'Alarm','Alarm at Meter')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,60,0,'OOS','Out Of Service')
INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,70,1,'OP','Operational Problem')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,80,1,'LJ','Lock Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,81,1,'CBJ','Cashbox Jam')
    INSERT INTO [EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose])VALUES(@CustomerID,3,90,0,'OutOfOrder','Out Of Order')

return
GO
/****** Object:  StoredProcedure [dbo].[spGenerateBlackList]    Script Date: 04/01/2014 22:07:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGenerateBlackList]
	@UserID int
AS
DECLARE @Filename varchar(64)
DECLARE @CurrentCheckSum int
DECLARE @LastCheckSum int
DECLARE @CustomerID int
DECLARE @cmdline varchar(256)

Select @CurrentCheckSum = CHECKSUM_AGG(CHECKSUM(HCardNum)) from BlackListActive
Select TOP 1 @LastChecksum = BlackListChecksum from BlackListFiles Order by DateTimeGenerated DESC
Select @LastChecksum = ISNULL(@LastChecksum,0)
IF @LastChecksum <> @CurrentCheckSum
BEGIN
	Select @CustomerID = DefaultCustomerID from Users where UserID = @UserID
	SET @cmdline='generator.exe ' + CAST(@CustomerID as varchar(8)) + ' ' + CAST(@CurrentCheckSum as varchar(24))
	EXEC master..xp_cmdshell @cmdline, NO_OUTPUT
END
GO
/****** Object:  Table [dbo].[SFSpecialEvent]    Script Date: 04/01/2014 22:07:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFSpecialEvent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFMeteredSpaceId] [int] NOT NULL,
	[EventDesc] [varchar](100) NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NOT NULL,
	[EventPrice] [float] NOT NULL,
 CONSTRAINT [PK_SFSpecialEvent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_Gateways]    Script Date: 04/01/2014 22:07:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_Gateways] 	
	@CustomerId int,
	@GatewayId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '----------------------------------------------'
	Print 'AUDIT GATEWAY'

	INSERT INTO [GatewaysAudit]
           ([UserId]
           ,[UpdateDateTime]
           ,[GateWayID]
           ,[CustomerID]
           ,[Description]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[GatewayState]
           ,[GatewayType]
           ,[InstallDateTime]
           ,[TimeZoneID]
           ,[DemandZone]
           ,[CAMID]
           ,[CELID]
           ,[PowerSource]
           ,[HWVersion]
           ,[Manufacturer]
           ,[GatewayModel]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,[WarrantyExpiration]
           ,[AssetPendingReasonId])
	SELECT @CreateUserId
           ,GETDATE()
           ,[GateWayID]
           ,[CustomerID]
           ,[Description]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[GatewayState]
           ,[GatewayType]
           ,[InstallDateTime]
           ,[TimeZoneID]
           ,[DemandZone]
           ,[CAMID]
           ,[CELID]
           ,[PowerSource]
           ,[HWVersion]
           ,[Manufacturer]
           ,[GatewayModel]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,[WarrantyExpiration]
           ,@AssetPendingReasonId
           from Gateways           
           Where CustomerID = @CustomerId
			and GateWayID = @GatewayId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_CashBox]    Script Date: 04/01/2014 22:07:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_CashBox] 	
	@CustomerId int,
	@CashBoxId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '----------------------------------------------'
	Print 'AUDIT CASHBOX'
	INSERT INTO [CashBoxAudit]
           ([UserId]
           ,[UpdateDateTime]
           ,[CashBoxID]
           ,[CustomerID]
           ,[CashBoxSeq]
           ,[CashBoxState]
           ,[InstallDate]
           ,[CashBoxModel]
           ,[CashBoxType]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[CashBoxName]
           ,[CashBoxLocationTypeId]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,[WarrantyExpiration]
           ,[AssetPendingReasonId])	
	SELECT @CreateUserId
           ,GETDATE()
           ,[CashBoxID]
           ,[CustomerID]
           ,[CashBoxSeq]
           ,[CashBoxState]
           ,[InstallDate]
           ,[CashBoxModel]
           ,[CashBoxType]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[CashBoxName]
           ,[CashBoxLocationTypeId]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,[WarrantyExpiration]
           ,@AssetPendingReasonId
		FROM CashBox
	WHERE CustomerID = @CustomerId
	and CashBoxID = @CashboxId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_DefaultHousing]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_DefaultHousing] 	
	@CustomerId int,
	@HousingId int output
AS
BEGIN
	select @HousingId = max(HousingId) from HousingMaster where Customerid = @CustomerId
	if (@HousingId is null) begin
		INSERT INTO [HousingMaster]
           ([HousingName]
           ,[Customerid]
           ,[Block]
           ,[StreetName]
           ,[StreetType]
           ,[StreetDirection]
           ,[StreetNotes]
           ,[HousingTypeID]
           ,[DoorLockId]
           ,[MechLockId]
           ,[IsActive]
           ,[InactiveRemarkID]
           ,[CreateDate]
           ,[Notes])
     VALUES		
		('Default Housing'
           ,@CustomerId
           ,'Default value'--[Block]
           ,'Default value'--[StreetName]
           ,'Default value'--[StreetType]
           ,'Default value'--[StreetDirection]
           ,'Default value'--[StreetNotes]
           ,null--[HousingTypeID]
           ,null--[DoorLockId]
           ,null--[MechLockId]
           ,0--[IsActive]
           ,null--[InactiveRemarkID]
           ,GetDate()--[CreateDate]
           ,'Created by sp_AssetPending'--[Notes]
        )
        
        set @HousingId = SCOPE_IDENTITY();
        
        Print 'Housing Created ' + convert(varchar,@HousingId)
	end 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertEventCode]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROC [dbo].[sp_InsertEventCode]
	  @CustomerID int,
	  @EventCode int,
	  @EventDescAbbrev varchar(200),
	  @EventDescVerbose varchar(200),
	  @EventType int
	  as
	begin 
		if not exists(select * from EventCodes where EventCode = @EventCode and CustomerId=@CustomerID)
			begin
				INSERT INTO EventCodes(CustomerID,EventSource,EventCode,AlarmTier,EventDescAbbrev,EventDescVerbose,EventType) VALUES
				(@CustomerId,0,@EventCode,0,@EventDescAbbrev,@EventDescVerbose,@EventType)
				Print @EventDescVerbose + ' inserted for CID - ' + convert(varchar,@CustomerID)
			end	
	end
GO
/****** Object:  Table [dbo].[CollRouteSeq]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollRouteSeq](
	[CollRouteId] [int] NOT NULL,
	[CollSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CollRouteManualAmount]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollRouteManualAmount](
	[Id] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CollRouteId] [int] NOT NULL,
	[CollTime] [datetime] NOT NULL,
	[AmoutInCent] [int] NOT NULL,
 CONSTRAINT [PK_CollRouteManualAmount] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [UK_CollRouteManualAmount] UNIQUE NONCLUSTERED 
(
	[CollRouteId] ASC,
	[CustomerID] ASC,
	[CollTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[EnforceRouteSeq]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EnforceRouteSeq](
	[EnforceRouteId] [int] NOT NULL,
	[EnforceRouteSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FDHousingAudit]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDHousingAudit](
	[HousingId] [int] NOT NULL,
	[FileID] [bigint] NOT NULL,
	[FileType] [int] NOT NULL,
	[UpdateTS] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountSchemeCustomerInfo]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountSchemeCustomerInfo](
	[DiscountSchemeCustomerInfoId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[DiscountSchemeId] [int] NULL,
	[FromEmailAddress] [varchar](200) NULL,
	[CustomerServicePhNo] [varchar](200) NULL,
	[DiscountSchemeEmailTemplateId] [int] NULL,
 CONSTRAINT [PK_DiscountSchemeCustomerInfo] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeCustomerInfoId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeCustomer]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscountSchemeCustomer](
	[DiscountSchemeCustomerId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DiscountSchemeId] [int] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[IsDisplay] [bit] NOT NULL,
	[DiscountPercentage] [int] NULL,
	[DiscountMinute] [int] NULL,
	[MaxAmountInCent] [int] NULL,
	[SchemeState] [int] NULL,
 CONSTRAINT [PK_DiscountSchemeCustomer] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeCustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountUserScheme]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountUserScheme](
	[DiscountUserSchemeId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[CardId] [int] NOT NULL,
	[SchemeId] [int] NOT NULL,
	[ExpiryTS] [datetime] NULL,
	[CreatedTS] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NULL,
	[SchemeStatus] [int] NULL,
	[SchemeStatusDate] [datetime] NULL,
	[StatusNote] [varchar](2000) NULL,
	[CustomerId] [int] NOT NULL,
 CONSTRAINT [PK_DiscountUserScheme] PRIMARY KEY CLUSTERED 
(
	[DiscountUserSchemeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerProperty]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerProperty](
	[CustomerPropertyId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerPropertyGroupId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[PropertyDesc] [varchar](250) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_CustomerProperty] PRIMARY KEY CLUSTERED 
(
	[CustomerPropertyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[AreaSeq]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AreaSeq](
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[AreaSeq] [int] NOT NULL,
	[Latitude] [float] NOT NULL,
	[Longitude] [float] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MeterMapAudit]    Script Date: 04/01/2014 22:07:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterMapAudit](
	[MeterMapAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Customerid] [int] NOT NULL,
	[Areaid] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ZoneId] [int] NULL,
	[HousingId] [int] NOT NULL,
	[MechId] [int] NULL,
	[AreaId2] [int] NULL,
	[CollRouteId] [int] NULL,
	[EnfRouteId] [int] NULL,
	[MaintRouteId] [int] NULL,
	[CustomGroup1] [int] NULL,
	[CustomGroup2] [int] NULL,
	[CustomGroup3] [int] NULL,
	[AuditDateTime] [datetime] NOT NULL,
	[UserId] [int] NULL,
	[AssetPendingReasonId] [int] NULL,
	[SubAreaID] [int] NULL,
	[GatewayID] [int] NULL,
	[SensorID] [int] NULL,
	[CashBoxID] [int] NULL,
	[CollectionRunId] [bigint] NULL,
 CONSTRAINT [PK_MeterMapAudit] PRIMARY KEY CLUSTERED 
(
	[MeterMapAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[MissingPaymentSequence]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create View [dbo].[MissingPaymentSequence] AS
		select distinct l.customerid,l.SeqNumber + 1 as start
		from PaymentReceived as l left outer join PaymentReceived 
		as r on l.SeqNumber + 1 = r.SeqNumber and l.customerid = r.customerid  
		where r.SeqNumber is null and l.SeqNumber is not null
GO
/****** Object:  Table [dbo].[ReportDetail]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ReportDetail](
	[RepId] [int] NOT NULL,
	[RepColumn] [varchar](30) NOT NULL,
	[ColData] [varchar](max) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFMeterSchedule]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFMeterSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFMeteredSpaceId] [int] NOT NULL,
	[SFOptSchType] [int] NOT NULL,
	[ColorRule] [varchar](50) NULL,
	[DaysOfWeek] [varchar](50) NOT NULL,
	[StartTime] [varchar](50) NOT NULL,
	[EndTime] [varchar](50) NOT NULL,
	[TimeLimit] [int] NULL,
	[PrePaymentTime] [varchar](50) NULL,
 CONSTRAINT [PK_SFMeterSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SFMeterPriceSchedule]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SFMeterPriceSchedule](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFMeteredSpaceId] [int] NOT NULL,
	[DaysOfWeek] [varchar](20) NOT NULL,
	[PriceStartTime] [varchar](20) NULL,
	[PriceEndTime] [varchar](20) NULL,
	[Price] [float] NULL,
 CONSTRAINT [PK_SFMeterPriceSchedule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ScheduleProcesses]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ScheduleProcesses](
	[CustomerID] [int] NOT NULL,
	[ScheduleID] [int] NOT NULL,
	[ProcessOrder] [int] NOT NULL,
	[ProcessID] [char](4) NOT NULL,
 CONSTRAINT [PK_ScheduleProcesses] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[ScheduleID] ASC,
	[ProcessOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[MeterStatusEvents]    Script Date: 04/01/2014 22:07:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterStatusEvents](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[TimeOfOccurrance] [datetime] NOT NULL,
	[TimeOfNotification] [datetime] NOT NULL,
	[EventSource] [int] NULL,
 CONSTRAINT [PK_MeterStatusEvents] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[State] ASC,
	[TimeOfOccurrance] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [MeterStatusEvents_IDX_CTAMETS] ON [dbo].[MeterStatusEvents] 
(
	[CustomerID] ASC,
	[TimeOfOccurrance] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventSource] ASC,
	[TimeOfNotification] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [MeterStatusEvents_IDX_MCATS] ON [dbo].[MeterStatusEvents] 
(
	[MeterId] ASC,
	[CustomerID] ASC,
	[AreaID] ASC,
	[TimeOfOccurrance] ASC,
	[State] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm_MeterStatus]    Script Date: 04/01/2014 22:07:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_Alarm_MeterStatus]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@TimeOfOccurrance DateTime
	,@TimeOfNotification DateTime	
	,@Message varchar(500) output
AS
Declare
	@State int
BEGIN
	BEGIN TRY
		set @Message = 'sp_Alarm_MeterStatus :'
	
		if (@EventCode  = 90 ) begin
			set @State = 0 --Out Of Order
			set @Message = @Message + ' Out of order status'
		end else begin
			set @State = 255 -- Return to service (Event code = 21)
			set @Message = @Message + ' Return to service status'
		end
		
		Print 'Inserting into Meter Status'
		INSERT INTO [MeterStatusEvents]
           ([CustomerID]
           ,[AreaID]
           ,[MeterId]
           ,[State]
           ,[TimeOfOccurrance]
           ,[TimeOfNotification]
           ,[EventSource])
		VALUES
           (@CustomerId
           ,@AreaId
           ,@MeterId
           ,@State
           ,@TimeOfOccurrance
           ,@TimeOfNotification
           ,@EventSource
           )
           
          set @Message = @Message + ' inserted.'
           
          Print 'Inserting into EventLogs' 
          
          INSERT INTO [EventLogs]
           ([CustomerID]
           ,[AreaID]
           ,[MeterId]
           ,[EventDateTime]
           ,[EventCode]
           ,[EventSource]
           ,[TechnicianKeyID])
		VALUES
           (@CustomerId
           ,@AreaId
           ,@MeterId
           ,@TimeOfOccurrance --[EventDateTime]
           ,@EventCode
           ,@EventSource
           ,Null --[TechnicianKeyID]
           )
          
          set @Message = @Message + ' EventLogs inserted.'
	END TRY
	BEGIN CATCH
		set @Message = @Message +  ' Error : ' + ERROR_MESSAGE()
		print @Message
	END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Alarm]    Script Date: 04/01/2014 22:07:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
* Wrapper function 
*/
CREATE PROC [dbo].[sp_Alarm]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@EventCode int
	,@EventSource int
	,@TimeOfOccurrance DateTime
	,@CurrentTime DateTime
	,@WorkOrderId int		
	,@Function varchar(50)
AS
DECLARE
	@Message varchar(500)
BEGIN
	set @Message = 'DEFAULT'
	set NOCOUNT ON;
	
	BEGIN TRY
		set @Function = UPPER(@Function)
		print 'sp_Alarm : Function=' + @Function
		if (@Function = 'CLEAR') begin
			exec sp_Alarm_Clear @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@TimeOfOccurrance,@CurrentTime,@Message output		
		end else if (@Function = 'RAISE') begin
			exec sp_Alarm_Raised @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@TimeOfOccurrance,@CurrentTime,@WorkOrderId,@Message output				
		end else if (@Function = 'STATE') begin
			exec sp_Alarm_MeterStatus @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@TimeOfOccurrance,@CurrentTime,@Message output					
		end 		
	END TRY
	BEGIN CATCH
		set @Message = ERROR_MESSAGE()
		print 'sp_Alarm : ERROR:' + @Message
	END CATCH
	select @Message as [Message]
END
GO
/****** Object:  StoredProcedure [dbo].[sp_NextPreventativeAlarm]    Script Date: 04/01/2014 22:07:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[sp_NextPreventativeAlarm]

  AS
  Declare
	@CustomerId int
	,@AreaId int
	,@MeterId int		
	,@NextPreventativeMaintenance DateTime			
	,@EventCode int	
	,@EventSource int	
	,@TimeOfNotification DateTime		
	
	
	BEGIN
    set nocount on;
	set @EventSource = 0 
	set @EventCode=2010	
	set @TimeOfNotification=getdate()		
	
	
	
	Declare curActives  cursor		
	
	for
	select CustomerID,AreaID,MeterId,NextPreventativeMaintenance from meters 
	where (( Datediff(minute, Getdate(),nextpreventativemaintenance) > 0) AND ( Datediff(minute, Getdate(),nextpreventativemaintenance) <=1440 ))
                           AND meterid NOT IN (SELECT meterid
                                               FROM   activealarms
                                               WHERE  eventcode = @EventCode)
	open curActives 
	fetch next from curActives into @CustomerID,@AreaID,@MeterId,@NextPreventativeMaintenance
			WHILE @@FETCH_STATUS = 0 
		 BEGIN
				BEGIN TRY											 
				
				if not exists (select * from ActiveAlarms 
						where CustomerID = @CustomerId and AreaID = @AreaId and MeterId = @MeterId
							and EventCode = @EventCode)	
									BEGIN
									
									exec sp_Alarm @CustomerId,@AreaId,@MeterId,@EventCode,@EventSource,@NextPreventativeMaintenance,@TimeOfNotification,null,'RAISE'

		                          	
		
	       END
		  END TRY
		  BEGIN CATCH
		  print 'sp_NextPreventativeAlarm ERROR :' + ERROR_MESSAGE()
	     END CATCH
	     fetch next from curActives into @CustomerID,@AreaID,@MeterId,@NextPreventativeMaintenance
  
		END
		CLOSE curActives 	
		DEALLOCATE curActives 
	    END
GO
/****** Object:  View [dbo].[qvActUnqAlarms]    Script Date: 04/01/2014 22:07:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qvActUnqAlarms]
AS
SELECT DISTINCT
dbo.ActiveAlarms.CustomerID, dbo.ActiveAlarms.AreaID, dbo.ActiveAlarms.MeterID, dbo.ActiveAlarms.EventSource, dbo.ActiveAlarms.EventCode,
dbo.EventCodes.AlarmTier, dbo.EventCodes.EventDescVerbose, dbo.Meters.MeterName, dbo.Meters.Location
FROM         dbo.ActiveAlarms INNER JOIN
dbo.Meters ON dbo.ActiveAlarms.CustomerID = dbo.Meters.CustomerID AND dbo.ActiveAlarms.AreaID = dbo.Meters.AreaID AND
dbo.ActiveAlarms.MeterID = dbo.Meters.MeterID INNER JOIN
dbo.Areas ON dbo.Meters.CustomerID = dbo.Areas.CustomerID AND dbo.Meters.AreaID = dbo.Areas.AreaID INNER JOIN
dbo.EventCodes ON dbo.ActiveAlarms.CustomerID = dbo.EventCodes.CustomerID AND
dbo.ActiveAlarms.EventSource = dbo.EventCodes.EventSource AND dbo.ActiveAlarms.EventCode = dbo.EventCodes.EventCode
GO
/****** Object:  View [dbo].[qMaxCollDateTime]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qMaxCollDateTime]
AS
SELECT     a.CustomerID, a.AreaID, a.MeterID, MAX(t.CollDateTime) AS CollDateTimeSummMax
FROM         dbo.Meters a LEFT OUTER JOIN
                      dbo.CollDataSumm t ON t.CustomerId = a.CustomerID AND t.MeterId = a.MeterID AND t.AreaId = a.AreaID
GROUP BY a.CustomerID, a.AreaID, a.MeterID
GO
/****** Object:  Table [dbo].[MeterResetSchedule]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MeterResetSchedule](
	[ScheduleId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ResetDateTime] [datetime] NOT NULL,
	[Acknowledged] [datetime] NULL,
	[CancelDate] [datetime] NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_MeterResetSchedule] PRIMARY KEY CLUSTERED 
(
	[ScheduleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [MeterReseSchedule_IDX_ACK] ON [dbo].[MeterResetSchedule] 
(
	[Acknowledged] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [MeterResetSchedule_IDXGlobalMeterID] ON [dbo].[MeterResetSchedule] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ScheduledMeters]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ScheduledMeters](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[ScheduleID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[OrderNo] [int] NOT NULL,
 CONSTRAINT [PK_ScheduledMeters] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[ScheduleID] ASC,
	[AreaID] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ScheduledMeters_IDXGlobalMeterID] ON [dbo].[ScheduledMeters] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PushStatus]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PushStatus](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[Customerid] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[Bay] [int] NOT NULL,
	[Expt] [datetime] NOT NULL,
	[Pushed] [datetime] NOT NULL,
	[Acknowledged] [datetime] NULL,
	[Attempt] [int] NULL,
 CONSTRAINT [PK_Pushstatus] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PublicHoliday]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PublicHoliday](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDate] [smalldatetime] NOT NULL,
	[ScheduleID] [int] NOT NULL,
 CONSTRAINT [PK_PublicHoliday] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [PublicHoliday_IDX_CMSE] ON [dbo].[PublicHoliday] 
(
	[CustomerID] ASC,
	[MeterId] ASC,
	[ScheduleID] ASC,
	[EventDate] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [PublicHoliday_IDXGlobalMeterID] ON [dbo].[PublicHoliday] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qMeter_failedGSM]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE   VIEW [dbo].[qMeter_failedGSM]
AS
SELECT     MS.CustomerID, MS.AreaID, MS.MeterID, MS.LastGSMOK, MS.LastGSMFailed
FROM         Meters_CurrentState MS INNER JOIN
                      Meters M ON MS.CustomerID = M.CustomerID AND MS.AreaID = M.AreaID AND MS.MeterID = M.MeterID
WHERE     (MS.LastGSMOK < MS.LastGSMFailed) AND M.MaxBaysEnabled = 0
GO
/****** Object:  Table [dbo].[PPOImport]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PPOImport](
	[GlobalMeterID] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[InspDateTime] [datetime] NOT NULL,
	[PPOStatusCode] [int] NOT NULL,
	[OfficerName] [varchar](32) NOT NULL,
	[OfficerId] [varchar](8) NULL,
	[Suburb] [varchar](32) NULL,
	[Street] [varchar](48) NULL,
	[SceneKey] [varchar](32) NULL,
	[Location] [varchar](48) NULL,
	[HouseNum] [varchar](8) NULL,
	[Dev] [varchar](8) NULL,
	[VStatus] [varchar](2) NULL,
	[Remark] [varchar](48) NULL,
	[Manual] [char](1) NULL,
	[FileNameImp] [varchar](48) NULL,
	[DateTimeImp] [datetime] NULL,
 CONSTRAINT [PK_PPOImport_1] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[InspDateTime] ASC,
	[PPOStatusCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [PPOImport_IDXGlobalMeterID] ON [dbo].[PPOImport] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qHistoricalMeterStatus]    Script Date: 04/01/2014 22:07:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qHistoricalMeterStatus]
AS
SELECT     CustomerID, AreaID, MeterID, 
	CASE WHEN State = 0 THEN 90 ELSE 21 END AS EventCode, 
	EventSource, TimeOfOccurrance, 1 AS EventState,
						  TimeOfNotification, NULL AS TimeOfClearance, 0 AS ClearingEventUID, 0 AS EventUID
	FROM         dbo.MeterStatusEvents
UNION
	SELECT     
	CustomerID, AreaID, MeterID, 
	EventCode, EventSource, TimeOfOccurrance,EventState,TimeOfNotification,TimeOfClearance,ClearingEventUID, EventUID
	FROM         HistoricalAlarms
GO
/****** Object:  View [dbo].[HistoricalAssetStatusV]    Script Date: 04/01/2014 22:07:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[HistoricalAssetStatusV]
AS
SELECT     		CustomerID, 
				AreaID, 
				MeterID, 
				CASE WHEN State = 0 THEN 90 ELSE 21 END AS EventCode, 
				CAST(CASE WHEN State = 0 THEN 'Out Of Order' ELSE 'Returned To Service' END AS VARCHAR(40)) AS VerboseEvent, 
				EventSource, 
				1 AS EventState, 
				0 AS EventUID, 
				0 AS ClearingEventUID, 
				TimeOfOccurrance AS TimeOfOccurrence, 
                TimeOfNotification, 
				NULL AS TimeOfClearance
FROM	dbo.MeterStatusEvents

UNION

SELECT     		CustomerID, 
				AreaID, 
				MeterID, 
				EventCode, 
				NULL AS VerboseEvent,
				EventSource, 
				EventState, 
				EventUID, 
				ClearingEventUID, 
				TimeOfOccurrance AS TimeOfOccurrence,  
				TimeOfNotification, 
				TimeOfClearance
FROM	dbo.HistoricalAlarms
GO
/****** Object:  Table [dbo].[MeterInventory]    Script Date: 04/01/2014 22:07:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterInventory](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[FileTimeStamp] [datetime] NOT NULL,
	[FileType] [int] NOT NULL,
	[FileHash] [varchar](50) NOT NULL,
	[FileSize] [bigint] NULL,
 CONSTRAINT [PK_MeterInventory] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[FileTimeStamp] ASC,
	[FileType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [MeterInventory_IDXGlobalMeterID] ON [dbo].[MeterInventory] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CashBoxDataHistory]    Script Date: 04/01/2014 22:07:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CashBoxDataHistory](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DateTimeRem] [datetime] NOT NULL,
	[AmtAuto] [real] NOT NULL,
	[FileProcessId] [bigint] NULL,
 CONSTRAINT [PK_CashBoxDataHistory] PRIMARY KEY NONCLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[DateTimeRem] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CashBoxDataHistory_IDXGlobalMeterID] ON [dbo].[CashBoxDataHistory] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spImportMXCashBox]    Script Date: 04/01/2014 22:07:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spImportMXCashBox]
        @CustomerID integer = 29,
	@filename varchar(128),
	@xmlpath varchar(128),
             @xmldoc ntext
    As

DECLARE @MaxDate datetime
DECLARE @MinDate datetime
DECLARE @LastFileID int

print @xmldoc
Select @LastFileID=Max(FileProcessId) from CBImpFiles
Select @LastFileID=isnull(@LastFileID,0)+1

    DECLARE @idoc int
    EXEC sp_xml_preparedocument @idoc OUTPUT, @xmldoc

BEGIN TRAN
    INSERT INTO CashBoxDataImport
    ([CustomerId]
           ,[AreaId]
           ,[MeterId]
           ,[DateTimeIns]
           ,[CashBoxId]
           ,[CashboxSequenceNo]
           ,[DateTimeRead]
           ,[DateTimeRem]
           ,[OperatorId]
           ,[AutoFlag]
           ,[Dollar2Coins]
           ,[Dollar1Coins]
           ,[Cents50Coins]
           ,[Cents20Coins]
           ,[Cents10Coins]
           ,[Cents5Coins]
           ,[AmtCashless]
           ,[AmtAuto]
           ,[AmtManual]
           ,[AmtDiff]
           ,[PercentFull]
           ,[MeterStatus]
           ,[TallyRejects]
           ,[CreditCounter]
           ,[TimeActive]
           ,[MinVolts]
           ,[MaxTemp]
           ,[FirmwareVer]
           ,[FirmwareRev]
           ,[EventCode]
           ,[FileName]
           ,[FileProcessId]          
	)           
    SELECT
	@CustomerID as CustomerID,
        ISNULL(AreaID,0) as AreaID ,
        ISNULL(MeterID,0) as MeterID,
	CASE DateTimeIns WHEN '2005-00-00T00:00:00' THEN DateTimeCol ELSE CAST(ISNULL(DateTimeIns,DateTimeCol) as datetime) END as DateTimeIns,
        CashBoxId ,
        ISNULL(CashboxSequenceNo,0) as CashboxSequenceNo ,
        DateTimeCol ,
        ISNULL(DateTimeOut,DateTimeCol) as DateTimeRem,
	'MXImporter' as OperatorId,
	'0' as AutoFlag,
        Dollar2Coins ,
        Dollar1Coins ,
        Cents50Coins ,
        Cents20Coins ,
        Cents10Coins ,
        Cents5Coins ,
        AmtCashless ,
        ISNULL(AmtAuto,0) as AmtAuto ,
        AmtManual ,
	(AmtManual - AmtAuto) as AmtDiff,
        PercentFull ,
        MeterStatus ,
        TallyRejects ,
        ISNULL(CreditCounter,0) as CreditCounter ,
        TimeActive ,
        MinVolts / 10 as MinVolts,
        (MaxTemp - 128) as MaxTemp,
        CAST(Firmware as int) as FirmwareVer,
        (CAST(Firmware * 100 as int) % 100) as FirmwareRev ,
	0 as EventCode,
        [FileName] as FileName,
	@LastFileID as FileProcessId
    FROM OPENXML(@idoc, @xmlpath, 0)
        WITH (
        AreaID int 'Area_Number',
        MeterID int 'Meter_Number',
        DateTimeIns varchar(20) 'Cashbox_Inserted',
        CashBoxId varchar(16) '../Boxid',
        CashboxSequenceNo int 'Cashbox_Counter',
        DateTimeCol datetime '../File_Modify',
        DateTimeOut datetime 'Cashbox_Removed',
        Dollar2Coins int 'Coin_Totals/Item5',
        Dollar1Coins int 'Coin_Totals/Item4',
        Cents50Coins int 'Coin_Totals/Item3',
        Cents20Coins int 'Coin_Totals/Item2',
        Cents10Coins int 'Coin_Totals/Item1',
        Cents5Coins int 'Coin_Totals/Item0',
        AmtCashless float 'Cashless_Total',
        AmtAuto float 'Cashbox_Total',
        AmtManual float '../Manual_Count',
        PercentFull int 'Percentage_Full',
        MeterStatus varchar(2) 'Meter_Status',
        TallyRejects int 'Reject_Coins',
        CreditCounter int 'Insertion_Counter',
        TimeActive bigint 'Cashbox_Active',
        MinVolts float 'Minimum_Voltage',
        MaxTemp float 'Maximum_Temperature',
        Firmware float 'Firmware_Version',
        FileName varchar(64) '../File_Name'
        )
    	IF (@@ERROR <> 0) GOTO ERR_HANDLER

--	SELECT @MaxDate=Max(DateTimeRem), @MinDate=Min(DateTimeRem) FROM CashBoxDataImport
 --   	INSERT CBImpFiles VALUES (@CustomerID,@LastFileID, 1, @filename,GETDATE(),ISNULL(@MaxDate,0),ISNULL(@MinDate,0))
	IF (@@ERROR <> 0) GOTO ERR_HANDLER
COMMIT TRAN

DECLARE @CashboxData_Temp Table(
        AreaID int,
        MeterID int,
	DateTimeIns0 datetime,
        AmtAuto0 real,
	DateTimeIns1 datetime,
        AmtAuto1 real,
	DateTimeIns2 datetime,
        AmtAuto2 real,
	DateTimeIns3 datetime,
        AmtAuto3 real,
	DateTimeIns4 datetime,
        AmtAuto4 real,
	DateTimeIns5 datetime,
        AmtAuto5 real,
	DateTimeIns6 datetime,
        AmtAuto6 real,
	DateTimeIns7 datetime,
        AmtAuto7 real,
	DateTimeIns8 datetime,
        AmtAuto8 real,
	DateTimeIns9 datetime,
        AmtAuto9 real
)

DECLARE @CashboxData_UnPivot_Temp TABLE (
        RecID int,
	DateTimeIns datetime,
        AmtAuto real
)

INSERT @CashboxData_Temp
  SELECT *
    FROM OPENXML(@idoc, @xmlpath, 0)
        WITH (
        AreaID int 'Area_Number',
        MeterID int 'Meter_Number',
        DateTimeIns0 datetime 'Cashbox_History/Item0/Insertion_Time',
        AmtAuto0 real 'Cashbox_History/Item0/Cashbox_Total',
        DateTimeIns1 datetime 'Cashbox_History/Item1/Insertion_Time',
        AmtAuto1 real 'Cashbox_History/Item1/Cashbox_Total',
        DateTimeIns2 datetime 'Cashbox_History/Item2/Insertion_Time',
        AmtAuto2 real 'Cashbox_History/Item2/Cashbox_Total',
        DateTimeIns3 datetime 'Cashbox_History/Item3/Insertion_Time',
        AmtAuto3 real 'Cashbox_History/Item3/Cashbox_Total',
        DateTimeIns4 datetime 'Cashbox_History/Item4/Insertion_Time',
        AmtAuto4 real 'Cashbox_History/Item4/Cashbox_Total',
        DateTimeIns5 datetime 'Cashbox_History/Item5/Insertion_Time',
        AmtAuto5 real 'Cashbox_History/Item5/Cashbox_Total',
        DateTimeIns6 datetime 'Cashbox_History/Item6/Insertion_Time',
        AmtAuto6 real 'Cashbox_History/Item6/Cashbox_Total',
        DateTimeIns7 datetime 'Cashbox_History/Item7/Insertion_Time',
        AmtAuto7 real 'Cashbox_History/Item7/Cashbox_Total',
        DateTimeIns8 datetime 'Cashbox_History/Item8/Insertion_Time',
        AmtAuto8 real 'Cashbox_History/Item8/Cashbox_Total',
        DateTimeIns9 datetime 'Cashbox_History/Item9/Insertion_Time',
        AmtAuto9 real 'Cashbox_History/Item9/Cashbox_Total'
        )

DECLARE @AreaID int
DECLARE @MeterID int

SELECT @AreaID = AreaID, @MeterID = MeterID 
FROM @CashboxData_Temp 

INSERT @CashboxData_UnPivot_Temp
Select 0, DateTimeIns0, AmtAuto0 From @CashboxData_Temp
UNION
Select 1, DateTimeIns1, AmtAuto1 From @CashboxData_Temp
UNION
Select 2, DateTimeIns2, AmtAuto2 From @CashboxData_Temp
UNION
Select 3, DateTimeIns3, AmtAuto3 From @CashboxData_Temp
UNION
Select 4, DateTimeIns4, AmtAuto4 From @CashboxData_Temp
UNION
Select 5, DateTimeIns5, AmtAuto5 From @CashboxData_Temp
UNION
Select 6, DateTimeIns6, AmtAuto6 From @CashboxData_Temp
UNION
Select 7, DateTimeIns7, AmtAuto7 From @CashboxData_Temp
UNION
Select 8, DateTimeIns8, AmtAuto8 From @CashboxData_Temp
UNION
Select 9, DateTimeIns9, AmtAuto9 From @CashboxData_Temp

DELETE @CashboxData_UnPivot_Temp
WHERE DateTimeIns IN (
	SELECT DateTimeRem FROM CashBoxDataHistory 
	WHERE 
		CustomerID = @CustomerID AND 
		AreaID = @AreaID AND 
		MeterID = @MeterID
	)


INSERT INTO CashBoxDataHistory
 ([CustomerID]
           ,[AreaID]
           ,[MeterId]
           ,[DateTimeRem]
           ,[AmtAuto]
           ,[FileProcessId])
Select  @CustomerID, @AreaID, @MeterID, DateTimeIns, AmtAuto, @LastFileID
From @CashboxData_UnPivot_Temp WHERE DateTimeIns IS NOT NULL
Select * from @CashboxData_Temp 
EXEC sp_xml_removedocument @idoc 
RETURN 0

ERR_HANDLER:
    ROLLBACK TRAN
    RAISERROR ('An error occurred during import. Rollback has occured. Please manually import the xml file.',16,1)
	EXEC sp_xml_removedocument @idoc
    RETURN
GO
/****** Object:  View [dbo].[qMissingCollections]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qMissingCollections]
AS
SELECT     TOP 100 PERCENT *, CASE WHEN CashboxSequenceNo IS NULL THEN 'MISSING COLLECTION' ELSE ' MATCHING COLLECTION' END AS Remarks
FROM         (SELECT     h.CustomerId, h.AreaId, h.MeterId, a.DateTimeRem, h.DateTimeRem AS DateTimeRemHist, a.AmtManual, a.AmtAuto AS AmtAuto, 
                                              h.AmtAuto AS AmtHist, a.CashboxSequenceNo, a.xFileProcessId, h.FileProcessId
                       FROM          CashBoxDataHistory h LEFT JOIN
                                              qCashBoxDataImport a ON a.MeterId = h.MeterId AND a.AreaId = h.AreaId AND a.CustomerId = h.CustomerId AND DATEPART(yy, 
                                              a.DateTimeRem) = DATEPART(yy, h.DateTimeRem) AND DATEPART(mm, a.DateTimeRem) = DATEPART(mm, h.DateTimeRem) AND 
                                              DATEPART(dd, a.DateTimeRem) = DATEPART(dd, h.DateTimeRem) 
                       WHERE      h.AmtAuto > 0) qCbSeqData
ORDER BY meterid ASC, DateTimeRemHist DESC
GO
/****** Object:  Table [dbo].[CollDataSched]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollDataSched](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DateTimeRem] [datetime] NOT NULL,
	[SchedDays] [int] NOT NULL,
 CONSTRAINT [PK_CollDataSched] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[DateTimeRem] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CollDataSched_IDXGlobalMeterID] ON [dbo].[CollDataSched] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qMetersNotCollected]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qMetersNotCollected]
AS
SELECT     CustomerId, AreaId, MeterId, SchedRemovalTime
FROM         (SELECT     S.CustomerId, S.AreaId, S.MeterId, S.DateTimeRem AS SchedRemovalTime, C.CollDateTime AS CommsRemovalTime, 
                                              B.DateTimeRem AS CBRRemovalTime
                       FROM          CollDataSched S LEFT JOIN
                                              CollDataSumm C ON S.CustomerId = C.CustomerId AND S.AreaId = C.AreaId AND S.MeterId = C.MeterId AND CONVERT(datetime, 
                                              DATEDIFF([day], 0, S.DateTimeRem)) = CONVERT(datetime, DATEDIFF([day], 0, C.CollDateTime)) LEFT JOIN
                                              CashBoxDataImport B ON S.CustomerId = B.CustomerId AND S.AreaId = B.AreaId AND S.MeterId = B.MeterId AND CONVERT(datetime, 
                                              DATEDIFF([day], 0, S.DateTimeRem)) = CONVERT(datetime, DATEDIFF([day], 0, B.DateTimeRem))) Q
WHERE     (CBRRemovalTime IS NULL) AND (CommsRemovalTime IS NULL)
GO
/****** Object:  Table [dbo].[BaySnapshot]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BaySnapshot](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayID] [int] NOT NULL,
	[BayState] [int] NOT NULL,
	[TransitionTimeStamp] [datetime] NOT NULL,
 CONSTRAINT [PK_BaySnapshot] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[BayID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [BaySnapshot_IDXGlobalMeterID] ON [dbo].[BaySnapshot] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AuditLog](
	[Id] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NULL,
	[AreaId] [int] NULL,
	[MeterId] [int] NULL,
	[TransDateTime] [datetime] NULL,
	[RequestType] [varchar](50) NULL,
	[OLTPaymentType] [varchar](50) NULL,
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [AuditLog_IDXGlobalMeterID] ON [dbo].[AuditLog] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DiscountUserSchemeAudit]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[DiscountUserSchemeAudit](
	[DiscountUserSchemeAuditId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DiscountUserSchemeId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[CardId] [int] NOT NULL,
	[SchemeId] [int] NOT NULL,
	[ExpiryTS] [datetime] NULL,
	[CreatedTS] [datetime] NOT NULL,
	[ModifiedByUserId] [int] NULL,
	[SchemeStatus] [int] NULL,
	[SchemeStatusDate] [datetime] NULL,
	[StatusNote] [varchar](2000) NULL,
	[ChangedByUserId] [int] NOT NULL,
	[ChangedDate] [datetime] NOT NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_DiscountUserSchemeAudit] PRIMARY KEY CLUSTERED 
(
	[DiscountUserSchemeAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[DiscountSchemeMeter]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DiscountSchemeMeter](
	[DiscountSchemeMeterId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[DiscountSchemeId] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterID] [int] NOT NULL,
 CONSTRAINT [PK_DiscountSchemeMeter] PRIMARY KEY CLUSTERED 
(
	[DiscountSchemeMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FDHousing]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDHousing](
	[HousingId] [int] NOT NULL,
	[FileID] [bigint] NOT NULL,
	[FileType] [int] NOT NULL,
	[UpdateTS] [datetime] NOT NULL,
 CONSTRAINT [UK_FDHousing] UNIQUE NONCLUSTERED 
(
	[HousingId] ASC,
	[FileType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FDJobs]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDJobs](
	[JobID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[FileID] [bigint] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[SubmittedDate] [datetime] NOT NULL,
	[AvailableDate] [datetime] NOT NULL,
	[ActiveJob] [int] NOT NULL,
	[ActivationDate] [datetime] NOT NULL,
	[JobStatus] [int] NOT NULL,
 CONSTRAINT [PK_FDActiveJobs] PRIMARY KEY CLUSTERED 
(
	[JobID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerFileArchive]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerFileArchive](
	[UniqueKey] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ClientMeterID] [varchar](20) NULL,
	[FileType] [int] NOT NULL,
	[FileHash] [varchar](50) NOT NULL,
	[FileSizeBytes] [bigint] NOT NULL,
	[FileAdditionDate] [datetime] NOT NULL,
	[FileRawData] [image] NOT NULL,
	[FileName] [varchar](80) NOT NULL,
	[Flags] [int] NULL,
 CONSTRAINT [PK_CustomerFileArchive] PRIMARY KEY CLUSTERED 
(
	[UniqueKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerDetails]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerDetails](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerID] [int] NOT NULL,
	[CustomerPropertyId] [int] NOT NULL,
	[AdditionalValue] [varchar](250) NULL,
	[ScreenName] [varchar](250) NOT NULL,
	[IsRequired] [bit] NOT NULL,
	[IsDisplay] [bit] NOT NULL,
 CONSTRAINT [PK_CustomerDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CustomerBaseMeterFiles]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CustomerBaseMeterFiles](
	[UniqueKey] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NULL,
	[FileHash] [varchar](50) NOT NULL,
	[FileSizeBytes] [bigint] NOT NULL,
	[FileAdditionDate] [datetime] NOT NULL,
	[FileRawData] [image] NOT NULL,
	[FileName] [varchar](80) NOT NULL,
	[Flags] [int] NULL,
 CONSTRAINT [PK_CustomerBaseMeterFiles] PRIMARY KEY CLUSTERED 
(
	[UniqueKey] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CollDataImport]    Script Date: 04/01/2014 22:07:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollDataImport](
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DateTimeRem] [datetime] NOT NULL,
	[OldCBBarCode] [varchar](12) NOT NULL,
	[NewCBBarCode] [varchar](12) NOT NULL,
	[StatusCode] [varchar](4) NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[CashBoxId] [varchar](14) NOT NULL,
	[Collector] [varchar](20) NULL,
	[FileNameImp] [varchar](48) NOT NULL,
	[DateTimeImp] [datetime] NOT NULL,
 CONSTRAINT [PK_CollDataImport] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[DateTimeRem] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [CollDataImport_IDXGlobalMeterID] ON [dbo].[CollDataImport] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qCollReconDetCBRDLR_SubV235]    Script Date: 04/01/2014 22:07:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qCollReconDetCBRDLR_SubV235]
AS
SELECT     a.CustomerId, a.AreaId, a.MeterId, b.DateTimeRem, b.OldCBBarCode, b.NewCBBarCode, b.Collector, b.AmountInCents / 100.00 AS Amount, 
                      b.FileNameImp, a.xFileProcessID
FROM         dbo.qCashBoxImport_subV235 a LEFT OUTER JOIN
                      dbo.CollDataImport b ON a.CustomerId = b.CustomerId AND a.AreaId = b.AreaId AND a.MeterId = b.MeterId AND CONVERT(datetime, DATEDIFF(week, 0, 
                      a.DateTimeRem)) = CONVERT(datetime, DATEDIFF(week, 0, b.DateTimeRem))
GO
/****** Object:  View [dbo].[CollReconDetCBRDLRsubV]    Script Date: 04/01/2014 22:07:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CollReconDetCBRDLRsubV]
AS
SELECT	a.CustomerId, 
		a.AreaId, 
		a.MeterId, 
		b.DateTimeRem, 
		b.OldCBBarCode, 
		b.NewCBBarCode, 
		b.Collector, 
		b.AmountInCents / 100.00 AS MeterAmount, 
		b.FileNameImp, 
		a.xFileProcessID
FROM	dbo.CashBoxImportSubV a 
		LEFT OUTER JOIN dbo.CollDataImport b 
			ON a.CustomerId = b.CustomerId 
			AND a.AreaId = b.AreaId 
			AND a.MeterId = b.MeterId 
			AND CONVERT(datetime, DATEDIFF(week, 0, a.DateTimeRem)) = CONVERT(datetime, DATEDIFF(week, 0, b.DateTimeRem))
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdate_Gateways]    Script Date: 04/01/2014 22:07:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdate_Gateways] 
	@AssetPendingReasonId int,
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@AssetId bigint,
	@CreateUserId int,
	@AssetName Varchar(500),
	@Latitude float,
	@Longitude float,
	@LocationGateway Varchar(255),
	@AssetType int,
	@AssetModel int,
	@AssetState int,
	@DateInstalled DateTime,
	@OperationalStatus int,
	@OperationalStatusTime DateTime,
	@WarrantyExpiration DateTime,
	@LastPreventativeMaintenance DateTime,
	@NextPreventativeMaintenance DateTime,
	@DemandStatus int
AS
Declare 
	@TimeZoneID int
BEGIN

	if exists (select * from Gateways where [GateWayID] = @AssetId and [CustomerID]  = @CustomerId) begin
		Print 'Updating Gateways'
		UPDATE [Gateways]
		   SET [Description] = Case when @AssetName is null then [Description] else @AssetName end
				,[Latitude] = Case when @Latitude is null then [Latitude] else @Latitude end
				,[Longitude] = Case when @Longitude is null then [Longitude] else @Longitude end
				,[Location] = Case when @LocationGateway is null then [Location] else @LocationGateway end
				,GatewayState = Case when @AssetState is null then GatewayState else @AssetState end
				,GatewayModel = Case when @AssetModel is null then GatewayModel else @AssetModel end
			  --,[GatewayType] = <GatewayType, int,>
			  --,[InstallDateTime] = <InstallDateTime, datetime,>
			  --,[TimeZoneID] = <TimeZoneID, int,>
			  --,[DemandZone] = <DemandZone, int,>
			  --,[CAMID] = <CAMID, varchar(250),>
			  --,[CELID] = <CELID, varchar(250),>
			  --,[PowerSource] = <PowerSource, int,>
			  --,[HWVersion] = <HWVersion, varchar(250),>
			  --,[Manufacturer] = <Manufacturer, varchar(250),>						  
			  ,[DemandZone] = Case when @DemandStatus is null then DemandZone else @DemandStatus end
			  ,[OperationalStatus] = Case when @OperationalStatus is null then OperationalStatus else @OperationalStatus end
			  ,[WarrantyExpiration] = Case when @WarrantyExpiration is null then WarrantyExpiration else @WarrantyExpiration end
			  ,[OperationalStatusTime] = Case when @OperationalStatusTime is null then OperationalStatusTime else @OperationalStatusTime end
			  ,[LastPreventativeMaintenance] = Case when @LastPreventativeMaintenance is null then LastPreventativeMaintenance else @LastPreventativeMaintenance end
			  ,[NextPreventativeMaintenance] = Case when @NextPreventativeMaintenance is null then NextPreventativeMaintenance else @NextPreventativeMaintenance end
			  ,[InstallDateTime] = Case when @DateInstalled is null then [InstallDateTime] else @DateInstalled end
		 WHERE [GateWayID] = @AssetId
				and [CustomerID]  = @CustomerId
	end else begin
	
		select @TimeZoneID TimeZoneID from Customers c where CustomerID = @CustomerId
		
		Print 'Inserting Gateways'
		INSERT INTO [Gateways]
           ([GateWayID]
           ,[CustomerID]
           ,[Description]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[GatewayState]
           ,[GatewayType]
           ,[InstallDateTime]
           ,[TimeZoneID]
           ,[DemandZone]
           ,[CAMID]
           ,[CELID]
           ,[PowerSource]
           ,[HWVersion]
           ,[Manufacturer]
           ,[GatewayModel]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[WarrantyExpiration]
           ,[OperationalStatus]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment])
     VALUES
		(@AssetId
           ,@CustomerId
           ,@AssetName
           ,@Latitude
           ,@Longitude
           ,@LocationGateway
           ,@AssetState
           ,null--[GatewayType]
           ,@DateInstalled
           ,null--[TimeZoneID]
           ,@DemandStatus
           ,null--[CAMID]
           ,null--[CELID]
           ,null--[PowerSource]
           ,null--[HWVersion]
           ,null--[Manufacturer]
           ,@AssetModel
           ,@OperationalStatusTime
           ,@LastPreventativeMaintenance
           ,@NextPreventativeMaintenance
           ,@WarrantyExpiration
           ,@OperationalStatus
           ,null--[OperationalStatusEndTime]
           ,null--[OperationalStatusComment]
           )     
	end 
	
	exec sp_Audit_Gateways @Customerid,@AssetId,@AssetPendingReasonId,@CreateUserId				
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdate_CashBox]    Script Date: 04/01/2014 22:07:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdate_CashBox] 	
	@CustomerId int,
	@AreaId int,
	@MeterId Int,
	@AssetId int,
	@AssetPendingReasonId int,
	@CreateUserId int,	
	@AssetType int,
	@AssetName Varchar(500),
	@AssetModel int,
	@NextPreventativeMaintenance DateTime,
	@OperationalStatus int,
	@WarrantyExpiration DateTime,
	@DateInstalled DateTime,
	@LastPreventativeMaintenance DateTime,
	@AssetState int,
	@OperationalStatusTime DateTime,
	@CashboxLocationTypeId int
AS
BEGIN
	Print '-------------------------------------------------------------------------------------------------'
	Print 'UPDATE and AUDIT CASHBOX'
	Print '-------------------------------------------------------------------------------------------------'
					
	If exists (select * from CashBox where CustomerId = @CustomerId	and CashBoxSeq = @AssetId) begin
		Print 'Updating Cashbox'
		UPDATE [CashBox]
		   SET [CashBoxState] = Case when @AssetState is null then [CashBoxState] else @AssetState end
			  ,[CashBoxModel] = Case when @AssetModel is null then [CashBoxModel] else @AssetModel end
			  ,[CashBoxName] = Case when @AssetName is null then [CashBoxName] else @AssetName end
			  ,[CashBoxLocationTypeId] = Case when @CashboxLocationTypeId is null then [CashBoxLocationTypeId] else @CashboxLocationTypeId end
			  ,[InstallDate] = Case when @DateInstalled is null then [InstallDate] else @DateInstalled end
			  ,[OperationalStatus] = Case when @OperationalStatus is null then OperationalStatus else @OperationalStatus end
			  ,[OperationalStatusTime] = Case when @OperationalStatusTime is null then OperationalStatusTime else @OperationalStatusTime end
			  ,[WarrantyExpiration] = Case when @WarrantyExpiration is null then WarrantyExpiration else @WarrantyExpiration end
			  ,[LastPreventativeMaintenance] = Case when @LastPreventativeMaintenance is null then LastPreventativeMaintenance else @LastPreventativeMaintenance end
			  ,[NextPreventativeMaintenance] = Case when @NextPreventativeMaintenance is null then NextPreventativeMaintenance else @NextPreventativeMaintenance end					  
		WHERE CustomerId = @CustomerId
			and CashBoxSeq = @AssetId
					
	end else begin
		Print 'Inserting Cashbox'
		INSERT INTO [CashBox]
			   ([CustomerID]
			   ,[CashBoxSeq]
			   ,[CashBoxState]
			   ,[InstallDate]
			   ,[CashBoxModel]
			   ,[CashBoxType]
			   ,[OperationalStatus]
			   ,[OperationalStatusTime]
			   ,[LastPreventativeMaintenance]
			   ,[NextPreventativeMaintenance]
			   ,[CashBoxName]
			   ,[WarrantyExpiration]
			   ,[CashBoxLocationTypeId]
			   ,[OperationalStatusEndTime]
			   ,[OperationalStatusComment])
		 VALUES (@CustomerId
			   ,@AssetId--[CashBoxSeq]
			   ,@AssetState
			   ,@DateInstalled
			   ,@AssetModel
			   ,null--[CashBoxType]
			   ,@OperationalStatus
			   ,@OperationalStatusTime
			   ,@LastPreventativeMaintenance
			   ,@NextPreventativeMaintenance
			   ,@AssetName
			   ,@WarrantyExpiration
			   ,@CashboxLocationTypeId
			   ,null--[OperationalStatusEndTime]
			   ,null--[OperationalStatusComment]
			   )
	end
	
	exec sp_Audit_CashBox @Customerid,@AssetId,@AssetPendingReasonId,@CreateUserId				
END
GO
/****** Object:  Table [dbo].[SFLengthOfStay]    Script Date: 04/01/2014 22:07:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SFLengthOfStay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SFMeterPriceScheduleId] [int] NOT NULL,
	[StayLevel] [int] NOT NULL,
	[StayHour] [int] NOT NULL,
	[PricePremiumPct] [int] NOT NULL,
 CONSTRAINT [PK_SFLengthOfStay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_Meters]    Script Date: 04/01/2014 22:07:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_Meters] 	
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS 
BEGIN
	Print 'Auditing Meter'
		
	INSERT INTO MetersAudit
		   ([GlobalMeterId]
		   ,[CustomerID]
		   ,[AreaID]
		   ,[MeterId]
		   ,[SMSNumber]
		   ,[MeterStatus]
		   ,[TimeZoneID]
		   ,[MeterRef]
		   ,[EmporiaKey]
		   ,[MeterName]
		   ,[Location]
		   ,[BayStart]
		   ,[BayEnd]
		   ,[Description]
		   ,[GSMNumber]
		   ,[SchedServTime]
		   ,[RSFName]
		   ,[RSFDateTime]
		   ,[BarCode]
		   ,[Latitude]
		   ,[Longitude]
		   ,[ProgramName]
		   ,[MaxBaysEnabled]
		   ,[MeterType]
		   ,[MeterGroup]
		   ,[MParkID]
		   ,[MeterState]
		   ,[DemandZone]
		   ,[TypeCode]
		   ,[UserId]
		   ,[UpdateDateTime]
		   ,[OperationalStatus]
		   ,[InstallDate]
		   ,[OperationalStatusID]
		   ,[FreeParkingMinute]
		   ,[RegulatedStatusID]
		   ,[WarrantyExpiration]
		   ,[OperationalStatusTime]
		   ,[LastPreventativeMaintenance]
		   ,[NextPreventativeMaintenance]
		   ,[OperationalStatusEndTime]
		   ,[OperationalStatusComment]
		   ,[AssetPendingReasonId])
		 select 
		 [GlobalMeterId]
		   ,[CustomerID]
		   ,[AreaID]
		   ,[MeterId]
		   ,[SMSNumber]
		   ,[MeterStatus]
		   ,[TimeZoneID]
		   ,[MeterRef]
		   ,[EmporiaKey]
		   ,[MeterName]
		   ,[Location]
		   ,[BayStart]
		   ,[BayEnd]
		   ,[Description]
		   ,[GSMNumber]
		   ,[SchedServTime]
		   ,[RSFName]
		   ,[RSFDateTime]
		   ,[BarCode]
		   ,[Latitude]
		   ,[Longitude]
		   ,[ProgramName]
		   ,[MaxBaysEnabled]
		   ,[MeterType]
		   ,[MeterGroup]
		   ,[MParkID]
		   ,[MeterState]
		   ,[DemandZone]
		   ,[TypeCode]
		   ,@CreateUserId--[UserId]
		   ,GETDATE()--[UpdateDateTime]
		   ,[OperationalStatusId]
		   ,[InstallDate]
		   ,[OperationalStatusID]
		   ,[FreeParkingMinute]
		   ,[RegulatedStatusID]
		   ,[WarrantyExpiration]
		   ,[OperationalStatusTime]
		   ,[LastPreventativeMaintenance]
		   ,[NextPreventativeMaintenance]
		   ,[OperationalStatusEndTime]
		   ,[OperationalStatusComment]
		   ,@AssetPendingReasonId
		  from Meters m
		  where m.CustomerID = @CustomerId
				and m.AreaID = @AreaId
				and m.MeterId = @MeterId
END
GO
/****** Object:  Table [dbo].[SLA_OperationSchedule]    Script Date: 04/01/2014 22:07:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SLA_OperationSchedule](
	[SLA_OperationScheduleID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[OperationStartMinuteOfDay] [int] NOT NULL,
	[OperationEndMinuteOfDay] [int] NOT NULL,
	[ScheduleStartDate] [datetime] NULL,
	[ScheduleEndDate] [datetime] NULL,
 CONSTRAINT [PK_SLA_OperationSchedule] PRIMARY KEY CLUSTERED 
(
	[SLA_OperationScheduleID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_getMeterTimeZone]    Script Date: 04/01/2014 22:07:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_getMeterTimeZone]
	@cid int
	,@aid int
	,@mid int
	,@tz varchar(255) OUTPUT
AS
BEGIN
	select @tz = tz.TimeZoneName
	from Meters m 
	,TimeZones tz
	where m.TimeZoneID = tz.TimeZoneID
	and m.Customerid = @cid
	and m.AreaID = @aid
	and m.MeterId = @mid;

	print 'TimeZone = ' + @tz
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Update_Meters]    Script Date: 04/01/2014 22:07:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Update_Meters]
	@CustomerId int
	,@AreaId int
	,@MeterId int
	,@NextPreventativeMaintenance datetime
	,@Street varchar(500)
	,@OperationalStatus int
	,@Latitude float
	,@Longitude float	
	,@UserId int
	
as
Begin
	Update Meters
	set NextPreventativeMaintenance = @NextPreventativeMaintenance
	,Location = @Street
	,OperationalStatusID = @OperationalStatus
	,Latitude = @Latitude
	,Longitude = @Longitude
	where CustomerID = @customerId
	and AreaID = @AreaId
	and MeterId = @MeterId
	
	INSERT INTO [MetersAudit]
           ([GlobalMeterId]
           ,[CustomerID],[AreaID],[MeterId]
           ,[SMSNumber],[GSMNumber]
           ,[MeterStatus],[MeterType],[MeterGroup],[MeterState]
           ,[TimeZoneID],[Location]
           ,[MeterRef],[MeterName],[Description]           
           ,[BayStart],[BayEnd]
           ,[RSFName],[RSFDateTime]
           ,[BarCode]
           ,[Latitude],[Longitude]
           ,[ProgramName]
           ,[MaxBaysEnabled]           
           ,[MParkID],[EmporiaKey]          
           ,[DemandZone]
           ,[TypeCode]
           ,[InstallDate]
           ,[OperationalStatusID],[OperationalStatusTime],[OperationalStatusEndTime],[OperationalStatusComment]
           ,[FreeParkingMinute]
           ,[RegulatedStatusID]
           ,[WarrantyExpiration]           
           ,[LastPreventativeMaintenance],[NextPreventativeMaintenance],[SchedServTime]     
           ,[UpdateDateTime],[UserId]
           )
     SELECT [GlobalMeterId]
      ,[CustomerID],[AreaID],[MeterId]
      ,[SMSNumber],[GSMNumber]
      ,[MeterStatus],[MeterType],[MeterGroup],[MeterState]
      ,[TimeZoneID],[Location]
      ,[MeterRef],[MeterName],[Description]      
      ,[BayStart],[BayEnd]      
      ,[RSFName],[RSFDateTime]
      ,[BarCode]
      ,[Latitude] ,[Longitude]
      ,[ProgramName]
      ,[MaxBaysEnabled]
      ,[MParkID],[EmporiaKey]      
      ,[DemandZone]
      ,[TypeCode]
      ,[InstallDate]
      ,[OperationalStatusID],[OperationalStatusTime],[OperationalStatusEndTime],[OperationalStatusComment]
      ,[FreeParkingMinute]
      ,[RegulatedStatusID]
      ,[WarrantyExpiration]      
      ,[LastPreventativeMaintenance],[NextPreventativeMaintenance],[SchedServTime]     
      ,GetDate(),@UserId
    From [Meters]
    Where CustomerID = @CustomerId and AreaId=@AreaId and MeterId = @MeterId
    
	Print 'Updating meter'
End
GO
/****** Object:  StoredProcedure [dbo].[spDeleteCustomer]    Script Date: 04/01/2014 22:07:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spDeleteCustomer]
    @CustomerID int
AS
    BEGIN TRAN
            DELETE SupportedCreditCards WHERE CustomerID = @CustomerID
            DELETE EventCodes WHERE CustomerID = @CustomerID
            DELETE ScheduleProcesses WHERE CustomerID = @CustomerID
            DELETE Schedules WHERE CustomerID = @CustomerID
            DELETE Areas WHERE CustomerID = @CustomerID
            DELETE Customers WHERE CustomerID = @CustomerID
            IF (@@ERROR <> 0) GOTO ERR_HANDLER
    COMMIT TRAN
RETURN 0

ERR_HANDLER:
    PRINT 'Cannot delete customer, because customer data exists in many tables!'
    ROLLBACK TRAN
RETURN 1
GO
/****** Object:  StoredProcedure [dbo].[spCreateCustomer]    Script Date: 04/01/2014 22:07:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create Procedure [dbo].[spCreateCustomer]
    @CustomerID int = 1,
    @Name varchar(50) = 'default value',
    @FromEmailAddress varchar(128) = 'default value'
As
INSERT INTO Customers (CustomerID, Name, FromEmailAddress ) VALUES(@CustomerID, @Name, @FromEmailAddress )

UPDATE ReinoParameters SET ParameterValue=@CustomerID WHERE ParameterID='CustomerID'

INSERT INTO [Schedules] ([CustomerID],[ScheduleID],StartTime,PeriodSec,Description,Mon,Tue,Wed,Thu,Fri,Sat,Sun,DayOfMonth,MonthNo,ProcessType,BankID)VALUES(@CustomerID,1,'1/1/2003 1am',60,'Upload to Bank','N','N','N','N','N','N','N',0,0,'BU',1)

INSERT INTO [Schedules] ([CustomerID],[ScheduleID],StartTime,PeriodSec,Description,Mon,Tue,Wed,Thu,Fri,Sat,Sun,DayOfMonth,MonthNo,ProcessType,BankID)VALUES(@CustomerID,30000,'1/1/2003 1am',0,'Default Alarm Schedule','N','N','N','N','N','N','N',0,0,'GD',0)

INSERT INTO [ScheduleProcesses] ([CustomerID],[ScheduleID],ProcessOrder,ProcessID) VALUES(@CustomerID,1,10,'UBBF')
INSERT INTO [ScheduleProcesses] ([CustomerID],[ScheduleID],ProcessOrder,ProcessID) VALUES(@CustomerID,30000,10,'PAC1')
INSERT INTO [ScheduleProcesses] ([CustomerID],[ScheduleID],ProcessOrder,ProcessID) VALUES(@CustomerID,30000,20,'DSTL')
INSERT INTO [ScheduleProcesses] ([CustomerID],[ScheduleID],ProcessOrder,ProcessID) VALUES(@CustomerID,30000,30,'DDE1')
INSERT INTO [ScheduleProcesses] ([CustomerID],[ScheduleID],ProcessOrder,ProcessID) VALUES(@CustomerID,30000,40,'BLDL')

EXEC dbo.spAddEventCodes

INSERT INTO [SupportedCreditCards] ([CustomerID],[CreditCardType],[BankID],[MerchantID])VALUES(@CustomerID,0,1,1)
INSERT INTO [SupportedCreditCards] ([CustomerID],[CreditCardType],[BankID],[MerchantID])VALUES(@CustomerID,1,1,1)
INSERT INTO [SupportedCreditCards] ([CustomerID],[CreditCardType],[BankID],[MerchantID])VALUES(@CustomerID,2,1,1)
INSERT INTO [SupportedCreditCards] ([CustomerID],[CreditCardType],[BankID],[MerchantID])VALUES(@CustomerID,3,1,1)
INSERT INTO [SupportedCreditCards] ([CustomerID],[CreditCardType],[BankID],[MerchantID])VALUES(@CustomerID,4,1,1)

INSERT INTO [Areas] ([CustomerID],[AreaID],[AreaName],[Description])VALUES(@CustomerID,1,'Area 1','New Area')

INSERT INTO [ImportDirectories] ([IncomingDir],[OutgoingDir],[CustomerID],[PollTime]) VALUES ('c:\incoming\','c:\incoming\',@CustomerID, 100)

INSERT INTO [OLTAcquirers] ([CustomerID], [CardTypeCode], [OLTPActive], [VSignPartner], [MigsAC], [VendorMerchant], [UserName], [Password], [AcquirerIF])
VALUES     (@CustomerID, 10, 1, NULL, 'FE23DAAB', 'TESTREINOTST', 'reinooperator', '', 1)
--datapark
INSERT INTO OLTAcquirers ([CustomerID], [CardTypeCode], [OLTPActive], [VSignPartner], [MigsAC], [VendorMerchant], [UserName], [Password], [AcquirerIF])
VALUES     (@CustomerID, 20, 1, NULL, NULL, 'TEST_REINO_000000', 'reino', 'reino111$', 2)
return
GO
/****** Object:  Table [dbo].[Tariff]    Script Date: 04/01/2014 22:07:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tariff](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TariffID] [int] NOT NULL,
	[TariffAmount] [int] NOT NULL,
	[TimeUnitAmount] [int] NOT NULL,
	[TariffTimeUnit] [int] NOT NULL,
	[MaxParkingTime] [int] NOT NULL,
	[MaxParkingUnit] [int] NOT NULL,
	[LinkedTariffID] [int] NOT NULL,
 CONSTRAINT [PK_Tariff] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[TariffID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [Tariff_IDXGlobalMeterID] ON [dbo].[Tariff] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[spInitializeCustomerProperties]    Script Date: 04/01/2014 22:07:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spInitializeCustomerProperties]
				@CustomerId int
			As

				SET NOCOUNT ON

				-- Note:  Customers table has already been populated.  This stored procedure initializes customer properties 
				-- values.

				-- This stored procedure can be rerun multiple times against same customer with no loss of data

				-- Target Tables: CustomerPropertyGroup, CustomerProperty
				-- Insert required data in these tables.


				-- Insert Verisign AcquiresVSignPartner group.
				DECLARE @CustomerPropertyGroupId int
	
				SELECT @CustomerPropertyGroupId = CustomerPropertyGroupId FROM [dbo].[CustomerPropertyGroup]
					WHERE CustomerID = @CustomerId AND PropertyGroupDesc = 'AcquiresVSignPartner'

				IF (@CustomerPropertyGroupId IS NULL)
				BEGIN
					INSERT INTO [dbo].[CustomerPropertyGroup] ([PropertyGroupDesc],[CustomerID])
						VALUES ('AcquiresVSignPartner', @CustomerId)
					SELECT @CustomerPropertyGroupId = SCOPE_IDENTITY()
				END

				IF NOT EXISTS (SELECT * FROM [dbo].[CustomerProperty] WHERE [CustomerPropertyGroupId] = @CustomerPropertyGroupId AND CustomerId = @CustomerId
					AND [PropertyDesc] = 'VeriSign')
				BEGIN
					INSERT INTO [dbo].[CustomerProperty] ([CustomerPropertyGroupId],[CustomerID],[PropertyDesc],[SortOrder])
						VALUES (@CustomerPropertyGroupId, @CustomerId, 'VeriSign', 20)
				END

				IF NOT EXISTS (SELECT * FROM [dbo].[CustomerProperty] WHERE [CustomerPropertyGroupId] = @CustomerPropertyGroupId AND CustomerId = @CustomerId
					AND [PropertyDesc] = 'PayPal')
				BEGIN
					INSERT INTO [dbo].[CustomerProperty] ([CustomerPropertyGroupId],[CustomerID],[PropertyDesc],[SortOrder])
						VALUES (@CustomerPropertyGroupId, @CustomerId, 'PayPal', 30)
				END
				
				IF NOT EXISTS (SELECT * FROM [dbo].[CustomerProperty] WHERE [CustomerPropertyGroupId] = @CustomerPropertyGroupId AND CustomerId = @CustomerId
					AND [PropertyDesc] = 'VSA')
				BEGIN
					INSERT INTO [dbo].[CustomerProperty] ([CustomerPropertyGroupId],[CustomerID],[PropertyDesc],[SortOrder])
						VALUES (@CustomerPropertyGroupId, @CustomerId, 'VSA', 10)
				END
			return
GO
/****** Object:  StoredProcedure [dbo].[spInitializeCustomer]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[spInitializeCustomer]
					@CustomerId int
				As

					SET NOCOUNT ON

					-- Note:  Customers table has already been populated.  This stored procedure initializes reference and 
					-- other required tables.

					-- This stored procedure can be rerun multiple times against same customer with no loss of data

					-- Target Table: AssetType
					-- Build AssetType table entries from MeterGroup

					BEGIN
						-- For each row in MeterGroup verify/insert row into AssetType
						DECLARE @MeterGroupId int
						DECLARE @MeterGroupDesc varchar(25)
						DECLARE cursorMeterGroup cursor for select MeterGroupId, MeterGroupDesc from [dbo].[MeterGroup]

						OPEN cursorMeterGroup
						FETCH NEXT FROM cursorMeterGroup into @MeterGroupId, @MeterGroupDesc
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this asset been added to AssetType?
							IF NOT EXISTS (SELECT * FROM [dbo].[AssetType] WHERE MeterGroupId = @MeterGroupId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this asset
								INSERT INTO [dbo].[AssetType] ([MeterGroupId],[CustomerId],[IsDisplay],[SLAMinutes],[MeterGroupDesc],[PreventativeMaintenanceScheduleDays])
								VALUES
									(@MeterGroupId, @CustomerId, 1, 300, @MeterGroupDesc, 180)
							END

							-- Get next row
							FETCH NEXT FROM cursorMeterGroup into @MeterGroupId, @MeterGroupDesc
						END
						CLOSE cursorMeterGroup
						DEALLOCATE cursorMeterGroup
					END


					-- Target Table: DiscountSchemeCustomer
					-- Build DiscountSchemeCustomer table entries from DiscountScheme
					BEGIN
						DECLARE @DiscountSchemeId int
						DECLARE @SchemeName varchar(500)
						DECLARE @SchemeType int
						DECLARE @DiscountPercentage int
						DECLARE @DiscountMinute int
						DECLARE @MaxAmountInCent int

						DECLARE cursorDiscountScheme cursor for 
							select DiscountSchemeID, SchemeName, SchemeType, DiscountPercentage, DiscountMinute, MaxAmountInCent from [dbo].[DiscountScheme]

						OPEN cursorDiscountScheme
						FETCH NEXT FROM cursorDiscountScheme into @DiscountSchemeId, @SchemeName, @SchemeType, @DiscountPercentage, @DiscountMinute, @MaxAmountInCent
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this scheme been added to AssetType?
							IF NOT EXISTS (SELECT * FROM [dbo].[DiscountSchemeCustomer] WHERE DiscountSchemeId = @DiscountSchemeId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this discount scheme
								INSERT INTO [dbo].[DiscountSchemeCustomer] ([DiscountSchemeId],[CustomerId],[IsDisplay],[DiscountPercentage],[DiscountMinute],[MaxAmountInCent])
									VALUES
										(@DiscountSchemeId, @CustomerId, 1, @DiscountPercentage, @DiscountMinute, @MaxAmountInCent)
							END

							-- Get next row
							FETCH NEXT FROM cursorDiscountScheme into @DiscountSchemeId, @SchemeName, @SchemeType, @DiscountPercentage, @DiscountMinute, @MaxAmountInCent
						END
						CLOSE cursorDiscountScheme
						DEALLOCATE cursorDiscountScheme
					END

					-- Target Table: EventCodes
					-- Build EventCodes table entries from EventCodeMaster
					-- Update EventCodesAudit table
					BEGIN
						DECLARE @EventCode int
						DECLARE @EventSource int
						DECLARE @AlarmTier int
						DECLARE @EventDescAbbrev varchar(16)
						DECLARE @EventDescVerbose varchar(50)
						DECLARE @IsAlarm bit
						DECLARE @EventType int

						DECLARE cursorEventCodeMaster cursor for 
							SELECT [EventCode],[EventSource],[AlarmTier],[EventDescAbbrev],[EventDescVerbose],[IsAlarm],[EventType]
							from [dbo].[EventCodeMaster]

						OPEN cursorEventCodeMaster
						FETCH NEXT FROM cursorEventCodeMaster into @EventCode,@EventSource,@AlarmTier,@EventDescAbbrev,@EventDescVerbose,@IsAlarm,@EventType
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this scheme been added to EventCodes?
							IF NOT EXISTS (SELECT * FROM [dbo].[EventCodes] WHERE EventCode = @EventCode AND EventSource = @EventSource AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this event code
								INSERT INTO [dbo].[EventCodes] ([CustomerID],[EventSource],[EventCode],[AlarmTier],[EventDescAbbrev],[EventDescVerbose],[IsAlarm],[EventType])
								VALUES (@CustomerId, @EventSource, @EventCode, @AlarmTier, @EventDescAbbrev, @EventDescVerbose, @IsAlarm, @EventType)
							END

							-- Get next row
							FETCH NEXT FROM cursorEventCodeMaster into @EventCode,@EventSource,@AlarmTier,@EventDescAbbrev,@EventDescVerbose,@IsAlarm,@EventType
						END
						CLOSE cursorEventCodeMaster
						DEALLOCATE cursorEventCodeMaster

					END


					-- Target Table: LocationTier
					-- Build LocationTier table entries from LocationTierMaster
					BEGIN
						DECLARE @LocationTierId int
						DECLARE @LocationTierName varchar(255)

						DECLARE cursorLocationTierMaster cursor for 
							SELECT [LocationTierId],[LocationTierName] from [dbo].[LocationTierMaster]

						OPEN cursorLocationTierMaster
						FETCH NEXT FROM cursorLocationTierMaster into @LocationTierId, @LocationTierName
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this scheme been added to LocationTier?
							IF NOT EXISTS (SELECT * FROM [dbo].[LocationTier] WHERE LocationTierId = @LocationTierId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this location tier
								INSERT INTO [dbo].[LocationTier] ([CustomerID],[LocationTierId],[LocationTierName],[IsDisplay])
								VALUES (@CustomerId, @LocationTierId, @LocationTierName, 1)
							END

							-- Get next row
							FETCH NEXT FROM cursorLocationTierMaster into @LocationTierId, @LocationTierName
						END
						CLOSE cursorLocationTierMaster
						DEALLOCATE cursorLocationTierMaster

					END


					-- Target Table: MechanismMasterCustomer
					-- Build MechanismMasterCustomer table entries from MechanismMaster
					BEGIN
						DECLARE @MechanismId int
						DECLARE @MechanismDesc varchar(255)
						--DECLARE @MeterGroupId int

						DECLARE cursorMechanismMaster cursor for 
							SELECT [MechanismId],[MechanismDesc],[MeterGroupId] from [dbo].[MechanismMaster]

						OPEN cursorMechanismMaster
						FETCH NEXT FROM cursorMechanismMaster into @MechanismId, @MechanismDesc, @MeterGroupId
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this mechanism been added to MechanismMasterCustomer?
							IF NOT EXISTS (SELECT * FROM [dbo].[MechanismMasterCustomer] WHERE MechanismId = @MechanismId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this mechanism master
								INSERT INTO [dbo].[MechanismMasterCustomer] ([MechanismId],[CustomerId],[MechanismDesc],[IsDisplay],[SLAMinutes],[PreventativeMaintenanceScheduleDays])
								VALUES (@MechanismId, @CustomerId, @MechanismDesc, 1, 300, 180)
							END

							-- Get next row
							FETCH NEXT FROM cursorMechanismMaster into @MechanismId, @MechanismDesc, @MeterGroupId
						END
						CLOSE cursorMechanismMaster
						DEALLOCATE cursorMechanismMaster

					END


					-- Target Table: MeterDiagnosticTypeCustomer
					-- Build MeterDiagnosticTypeCustomer table entries from MeterDiagnosticType
					BEGIN
						DECLARE @ID int
						DECLARE @DiagnosticDesc varchar(100)

						DECLARE cursorMeterDiagnosticType cursor for 
							SELECT [ID],[DiagnosticDesc] from [dbo].[MeterDiagnosticType]

						OPEN cursorMeterDiagnosticType
						FETCH NEXT FROM cursorMeterDiagnosticType into @ID, @DiagnosticDesc
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this diagnostic desc. been added to MeterDiagnosticTypeCustomer?
							IF NOT EXISTS (SELECT * FROM [dbo].[MeterDiagnosticTypeCustomer] WHERE DiagnosticType = @ID AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this diagnostic desc.
								INSERT INTO [dbo].[MeterDiagnosticTypeCustomer] ([CustomerId],[DiagnosticType],[IsDisplay])
								VALUES (@CustomerId, @ID, 1)
							END

							-- Get next row
							FETCH NEXT FROM cursorMeterDiagnosticType into @ID, @DiagnosticDesc
						END
						CLOSE cursorMeterDiagnosticType
						DEALLOCATE cursorMeterDiagnosticType

					END


					-- Target Table: OLTAcquirers
					-- Create entries for CardType 10 and 20
					BEGIN
						-- Does a OLTAcquirers row exist for this customer and this card type (10)?
						IF NOT EXISTS (SELECT * FROM [dbo].[OLTAcquirers] WHERE CustomerId = @CustomerId AND CardTypeCode = 10)
						BEGIN
							-- Insert OLTAcquirers of card type 10
							INSERT INTO [dbo].[OLTAcquirers] ([CustomerID],[CardTypeCode],[OLTPActive],[VendorMerchant],[UserName],[Password]
									   ,[AcquirerIF],[ReAuthorise])
								 VALUES
									   (@CustomerId, 10, 0, '', '', '', 0, 0)
						END

						-- Does a OLTAcquirers row exist for this customer and this card type (20)?
						IF NOT EXISTS (SELECT * FROM [dbo].[OLTAcquirers] WHERE CustomerId = @CustomerId AND CardTypeCode = 20)
						BEGIN
							-- Insert OLTAcquirers of card type 20
							INSERT INTO [dbo].[OLTAcquirers] ([CustomerID],[CardTypeCode],[OLTPActive],[VendorMerchant],[UserName],[Password]
									   ,[AcquirerIF],[ReAuthorise])
								 VALUES
									   (@CustomerId, 20, 0, '', '', '', 0, 0)
						END
					END


					-- Target Table: OLTCardHash
					-- Set inital data - Fdigit=6, Ldigit=4
					BEGIN
						-- Does a OLTCardHash row exist for this customer?
						IF NOT EXISTS (SELECT * FROM [dbo].[OLTCardHash] WHERE CustomerId = @CustomerId)
						BEGIN
							-- Insert OLTCardHash 
							INSERT INTO [dbo].[OLTCardHash] ([InsDate],[Fdigit],[Ldigit],[CustomerId])
							VALUES (GetDate(), 6, 4, @CustomerId)
						END
					END


					-- Target Table: PAMActiveCustomers
					-- Initialize - ResetImin=0,ExpTimeByPam=0
					BEGIN
						-- Does a PAMActiveCustomers row exist for this customer?
						IF NOT EXISTS (SELECT * FROM [dbo].[PAMActiveCustomers] WHERE CustomerId = @CustomerId)
						BEGIN
							-- Insert PAMActiveCustomers 
							INSERT INTO [dbo].[PAMActiveCustomers] ([CustomerID],[ResetImin],[ExpTimeByPAM])
							VALUES (@CustomerId, 0, 0)
						END
					END


					-- Target Table: TargetServiceDesignation
					-- Build TargetServiceDesignation table entries from TargetServiceDesignationMaster
					BEGIN
						DECLARE @TargetServiceDesignationId int
						DECLARE @TargetServiceDesignationDesc varchar(250)

						DECLARE cursorTargetServiceDesignationMaster cursor for 
							SELECT [TargetServiceDesignationId],[TargetServiceDesignationDesc] from [dbo].[TargetServiceDesignationMaster]

						OPEN cursorTargetServiceDesignationMaster
						FETCH NEXT FROM cursorTargetServiceDesignationMaster into @TargetServiceDesignationId, @TargetServiceDesignationDesc
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this target service designation been added to TargetServiceDesignation?
							IF NOT EXISTS (SELECT * FROM [dbo].[TargetServiceDesignation] WHERE TargetServiceDesignationId = @TargetServiceDesignationId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this target service designation
								INSERT INTO [dbo].[TargetServiceDesignation] ([TargetServiceDesignationDesc],[CustomerId],[IsDisplay])
								VALUES (@TargetServiceDesignationDesc, @CustomerId, 1)
							END

							-- Get next row
							FETCH NEXT FROM cursorTargetServiceDesignationMaster into @TargetServiceDesignationId, @TargetServiceDesignationDesc
						END
						CLOSE cursorTargetServiceDesignationMaster
						DEALLOCATE cursorTargetServiceDesignationMaster

					END


					-- Target Table: TimeTypeCustomer
					-- Build TimeTypeCustomer table entries from TimeType
					BEGIN
						DECLARE @TimeTypeId int
						DECLARE @TimeTypeDesc varchar(250)

						DECLARE cursorTimeType cursor for 
							SELECT [TimeTypeId], [TimeTypeDesc] from [dbo].[TimeType]

						OPEN cursorTimeType
						FETCH NEXT FROM cursorTimeType into @TimeTypeId, @TimeTypeDesc
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this time type been added to TimeTypeCustomer?
							IF NOT EXISTS (SELECT * FROM [dbo].[TimeTypeCustomer] WHERE TimeTypeId = @TimeTypeId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this time type
								INSERT INTO [dbo].[TimeTypeCustomer]
									([TimeTypeId],[CustomerId],[TimeTypeStartMinOfDay],[TimeTypeEndMinOfDay],[IsDisplayed])
								VALUES
									(@TimeTypeId, @CustomerId, 360, 780, 0)
							END

							-- Get next row
							FETCH NEXT FROM cursorTimeType into @TimeTypeId, @TimeTypeDesc
						END
						CLOSE cursorTimeType
						DEALLOCATE cursorTimeType

					END



					-- Make sure there is an entry in the [HousingMaster] table
					BEGIN

						-- Does this customer already have a default entry?
						-- If not, create a default entry.
						IF NOT EXISTS (SELECT * FROM [dbo].[HousingMaster] WHERE [HousingName] = 'Default' AND [Customerid] = @CustomerId)
						BEGIN

							INSERT INTO [dbo].[HousingMaster]
								   ([HousingName]
								   ,[Customerid]
								   ,[Block]
								   ,[StreetName]
								   ,[StreetType]
								   ,[StreetDirection]
								   ,[IsActive]
								   ,[CreateDate]
								   ,[Notes])
							 VALUES
								   ('Default'
								   ,@CustomerId
								   ,'Default'
								   ,'Default'
								   ,'Default'
								   ,'Default'
								   ,0
								   ,GetDate()
								   ,'Default housing placeholder')
						END
					END


					-- Make sure there are entries in the [Areas] table
					-- for AreaId 1, 97, 98, and 99 for this customer
					BEGIN
						DECLARE @DefaultAreaId int
						-- AreaId 1
						SET @DefaultAreaId = 1
						-- If not, create a default entry.
						IF NOT EXISTS (SELECT * FROM [dbo].[Areas] WHERE [AreaId] = @DefaultAreaId AND [Customerid] = @CustomerId)
						BEGIN
							INSERT INTO [dbo].[Areas]
								([GlobalAreaID],[CustomerID],[AreaID],[AreaName],[Description])
							VALUES
								([dbo].[GenGlobalID] (@CustomerId, @DefaultAreaId, 0, 0), @CustomerId, @DefaultAreaId, 'Area for Meter', 'Default area for a meter.')
						END

						-- AreaId 97
						SET @DefaultAreaId = 97
						-- If not, create a default entry.
						IF NOT EXISTS (SELECT * FROM [dbo].[Areas] WHERE [AreaId] = @DefaultAreaId AND [Customerid] = @CustomerId)
						BEGIN
							INSERT INTO [dbo].[Areas]
								([GlobalAreaID],[CustomerID],[AreaID],[AreaName],[Description])
							VALUES
								([dbo].[GenGlobalID] (@CustomerId, @DefaultAreaId, 0, 0), @CustomerId, @DefaultAreaId, 'Area for CashBox', 'Default area for a cashbox.')
						END

						-- AreaId 98
						SET @DefaultAreaId = 98
						-- If not, create a default entry.
						IF NOT EXISTS (SELECT * FROM [dbo].[Areas] WHERE [AreaId] = @DefaultAreaId AND [Customerid] = @CustomerId)
						BEGIN
							INSERT INTO [dbo].[Areas]
								([GlobalAreaID],[CustomerID],[AreaID],[AreaName],[Description])
							VALUES
								([dbo].[GenGlobalID] (@CustomerId, @DefaultAreaId, 0, 0), @CustomerId, @DefaultAreaId, 'Area for Gateway', 'Default area for a gateway.')
						END

						-- AreaId 99
						SET @DefaultAreaId = 99
						-- If not, create a default entry.
						IF NOT EXISTS (SELECT * FROM [dbo].[Areas] WHERE [AreaId] = @DefaultAreaId AND [Customerid] = @CustomerId)
						BEGIN
							INSERT INTO [dbo].[Areas]
								([GlobalAreaID],[CustomerID],[AreaID],[AreaName],[Description])
							VALUES
								([dbo].[GenGlobalID] (@CustomerId, @DefaultAreaId, 0, 0), @CustomerId, @DefaultAreaId, 'Area for Sensor', 'Default area for a sensor.')
						END
					END



					-- Target Table: AssetStateCustomer
					-- Build AssetStateCustomer table entries from AssetState
					BEGIN
						DECLARE @AssetStateId int
						DECLARE @AssetStateDesc varchar(25)

						DECLARE cursorAssetState cursor for 
							SELECT [AssetStateId],[AssetStateDesc] from [dbo].[AssetState]

						OPEN cursorAssetState
						FETCH NEXT FROM cursorAssetState into @AssetStateId, @AssetStateDesc
						WHILE @@FETCH_STATUS = 0
						BEGIN		
							-- Has this asset state been added to AssetStateCustomer?
							IF NOT EXISTS (SELECT * FROM [dbo].[AssetStateCustomer] WHERE AssetStateId = @AssetStateId AND CustomerId = @CustomerId)
							BEGIN
								-- Insert this asset state
								INSERT INTO [dbo].[AssetStateCustomer] ([CustomerId],[AssetStateId],[AssetStateDesc],[IsDisplayed])
								VALUES (@CustomerId, @AssetStateId, @AssetStateDesc, 1)
							END

							-- Get next row
							FETCH NEXT FROM cursorAssetState into @AssetStateId, @AssetStateDesc
						END
						CLOSE cursorAssetState
						DEALLOCATE cursorAssetState

					END

					-- Target Table: MaintRoute
					-- Build MaintRoute table for New Customer
					BEGIN
						Declare @RouteName nvarchar(50)
						Declare @MaintRouteId smallint
						-- Insert Route #1
						Select @MaintRouteId = ISNULL(MAX(MaintRouteId), 0) From dbo.MaintRoute
						
						Set @MaintRouteId = @MaintRouteId + 1
						Set @RouteName = 'M-Route 1'
						IF Not Exists( Select MaintRouteId From dbo.MaintRoute Where CustomerId = @CustomerId 
										And DisplayName = @RouteName)
							Begin
								Begin transaction
									INSERT INTO [dbo].[MaintRoute]
									   ([MaintRouteId]
									   ,[CustomerId]
									   ,[DisplayName]
									   ,[Comment])
									VALUES
									   (@MaintRouteId
									   ,@CustomerId
									   ,@RouteName
									   ,Null)
								commit transaction
							End				
						
						-- Insert Route #2
						Select @MaintRouteId = MAX(MaintRouteId) From dbo.MaintRoute
						Set @MaintRouteId = @MaintRouteId + 1
						Set @RouteName = 'M-Route 2'
						IF Not Exists( Select MaintRouteId From dbo.MaintRoute Where CustomerId = @CustomerId 
										And DisplayName = @RouteName)
							Begin
								Begin transaction
									INSERT INTO [dbo].[MaintRoute]
									   ([MaintRouteId]
									   ,[CustomerId]
									   ,[DisplayName]
									   ,[Comment])
									VALUES
									   (@MaintRouteId
									   ,@CustomerId
									   ,@RouteName
									   ,Null)
								commit transaction
							End				

						-- Insert Route #3
						Select @MaintRouteId = MAX(MaintRouteId) From dbo.MaintRoute
						Set @MaintRouteId = @MaintRouteId + 1
						Set @RouteName = 'M-Route 3'
						IF Not Exists( Select MaintRouteId From dbo.MaintRoute Where CustomerId = @CustomerId 
										And DisplayName = @RouteName)
							Begin
								Begin transaction
									INSERT INTO [dbo].[MaintRoute]
									   ([MaintRouteId]
									   ,[CustomerId]
									   ,[DisplayName]
									   ,[Comment])
									VALUES
									   (@MaintRouteId
									   ,@CustomerId
									   ,@RouteName
									   ,Null)
								commit transaction
							End				
					END

				return
GO
/****** Object:  Table [dbo].[TransDataSumm]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransDataSumm](
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[CollDateTimeCash] [datetime] NULL,
	[CollDateTimeCredCard] [datetime] NULL,
	[CollDateTimeSmtCard] [datetime] NULL,
	[CollDateTimeOtherMthd] [datetime] NULL,
	[AmountCash] [float] NULL,
	[AmountCredCard] [float] NULL,
	[AmountSmtCard] [float] NULL,
	[AmountOtherMthd] [float] NULL,
	[NoOfTransCash] [int] NULL,
	[NoOfTransCredCard] [int] NULL,
	[NoOfTransSmtCard] [int] NULL,
	[NoOfTransOtherMthd] [int] NULL,
	[PackageIdCash] [int] NULL,
	[PackageIdCredCard] [int] NULL,
	[PackageIdSmtCard] [int] NULL,
	[PackageIdOtherMthd] [int] NULL,
 CONSTRAINT [PK_CollDataCall] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransDataSumm_IDXGlobalMeterID] ON [dbo].[TransDataSumm] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsBlackList]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsBlackList](
	[GlobalMeterId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[ReceiptNo] [int] NULL,
	[BayNumber] [int] NULL,
	[AmountInCents] [int] NULL,
	[TimePaid] [int] NULL,
	[TransType] [int] NOT NULL,
	[CardNumHash] [varchar](40) NOT NULL,
	[CreditCardType] [int] NOT NULL,
 CONSTRAINT [PK_TransactionsBlackList] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [TransactionsBlackList_IDXGlobalMeterID] ON [dbo].[TransactionsBlackList] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionPackages]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionPackages](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BufferID] [int] NOT NULL,
	[TransactionPackageID] [int] NOT NULL,
	[DownloadDateTime] [datetime] NOT NULL,
	[PackageStatus] [int] NOT NULL,
	[StatusDate] [datetime] NOT NULL,
 CONSTRAINT [PK_TransactionPackages] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[BufferID] ASC,
	[TransactionPackageID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionPackages_IDXGlobalMeterID] ON [dbo].[TransactionPackages] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VSNodeStatus]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VSNodeStatus](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BitNumber] [int] NOT NULL,
	[NodeNumber] [int] NOT NULL,
	[LastUpdateTS] [datetime] NOT NULL,
	[Status] [int] NOT NULL,
 CONSTRAINT [PK_VSNodeStatus] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[BitNumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [VSNodeStatus_IDXGlobalMeterID] ON [dbo].[VSNodeStatus] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VehicleSensingEvents]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VehicleSensingEvents](
	[GlobalMeterID] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventCode] [int] NOT NULL,
	[EventSource] [int] NOT NULL,
	[TechnicianKeyID] [varchar](25) NULL,
 CONSTRAINT [PK_VehicleSensingEvents] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC,
	[EventCode] ASC,
	[EventSource] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [VehicleSensingEvents_IDXGlobalMeterID] ON [dbo].[VehicleSensingEvents] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VehicleMovements]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VehicleMovements](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[OccurranceTS] [datetime] NOT NULL,
	[BayID] [int] NOT NULL,
	[VSTransID] [bigint] NOT NULL,
	[SensorEvent] [int] NOT NULL,
	[AmountTime] [int] NOT NULL,
	[AmountCents] [int] NOT NULL,
 CONSTRAINT [PK_VehicleMovements] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[OccurranceTS] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [VehicleMovements_IDXGlobalMeterID] ON [dbo].[VehicleMovements] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[V_UnProcessed_CollRun]    Script Date: 04/01/2014 22:07:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_UnProcessed_CollRun]
AS
	select * from CollectionRun where CollectionRunId in 
	(SELECT COLLECTIONRUNID 
	FROM COLLECTIONRUN
	-- ENDDATE(ACTIVATION + DAYS) < TODAY WITHOUT TIME (00:00:00)
	WHERE DATEADD(DAY,DAYSBETWEENCOL,ACTIVATIONDATE) < DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)
		EXCEPT
	SELECT COLLECTIONRUNID 
	FROM COLLECTIONRUNREPORT
	)
GO
/****** Object:  View [dbo].[WhiteList_All]    Script Date: 04/01/2014 22:07:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[WhiteList_All] as 
	select mtr.CustomerID as [CustomerId],mtr.AreaID as [AreaId],mtr.MeterID as [MeterId]
	,card.PANHash as [CardHash] ,dus.SchemeId as [DiscountSchemeId]
	,sch.DiscountPercentage as [DiscountPercentage],sch.DiscountMinute as [DiscountMinute],sch.MaxAmountInCent as [MaxAmountInCents]
	from 
	(select * 
	from DiscountUserScheme 
	where ExpiryTS > GETDATE()
	and SchemeStatus=2
	)dus
	,(select * 
	from Users 
	where ExpirationDate > GETDATE()
	and accountStatus = 1 -- Active
	)usr
	,(select * 
	from DiscountScheme where ExpirationDate > GETDATE()
	) sch
	,(select *
	from DiscountUserCard
	where dateadd(YEAR,convert(int,substring(cardexpiry,3,2)),dateadd(MONTH,convert(int,substring(cardexpiry,1,2))-1,'2000/01/01')) > GETDATE()
	)card
	,(
		select DiscountSchemeId,CustomerID,AreaID,MeterId from DiscountSchemeMeter
		Union 
		select d.DiscountSchemeID ,m.customerID,AreaId,MeterId
		from Meters m
		,DiscountScheme d
		where m.CustomerID = d.CustomerId
		and d.SchemeType <> 1
		and (m.MeterGroup in (0,1)or m.MeterGroup is null)
	) mtr
 	where dus.UserId = usr.UserID
	and dus.SchemeId = sch.DiscountSchemeID
	and dus.CardId = card.CardID
	and usr.DefaultCustomerID = mtr.CustomerID
GO
/****** Object:  Table [dbo].[TransactionsCashKey]    Script Date: 04/01/2014 22:07:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsCashKey](
	[TransactionsCashKeyId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[TimePaid] [int] NOT NULL,
	[CashKeyID] [varchar](15) NOT NULL,
	[MeterTransReference] [varchar](10) NULL,
 CONSTRAINT [PK_TransactionsCashKey] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  StoredProcedure [dbo].[spReSchedule]    Script Date: 04/01/2014 22:07:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE  Procedure [dbo].[spReSchedule]
@customerID int
As

declare @schedID int
declare @y datetime
set @schedID = 667
set @y = dateadd(mi,datepart(mi,getdate())+5,'1/1/2000')
set @y = dateadd(hh,datepart(hh,getdate()),@y)

delete ScheduledMeters where ScheduleID=@schedID

Insert ScheduledMeters (CustomerID,ScheduleId,AreaId,MeterId,OrderNo) Select CustomerID, 1 as ScheduleID, AreaID, MeterID, 1 as OrderNo from qMeter_failedGSM
where CustomerID = 219

Update Schedules Set StartTime = @y,
Sun = 'N',
Mon = 'N',
Tue = 'N',
Wed = 'N',
Thu = 'N',
Fri = 'N',
Sat = 'N'
where ScheduleID = @schedID

if DATEPART ( dw , CURRENT_TIMESTAMP )  = 1
Update Schedules Set Sun = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 2
Update Schedules Set Mon = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 3
Update Schedules Set Tue = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 4
Update Schedules Set Wed = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 5
Update Schedules Set Thu = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 6
Update Schedules Set Fri = 'Y'
where ScheduleID = @schedID
if DATEPART ( dw , CURRENT_TIMESTAMP )  = 7
Update Schedules Set Sat = 'Y'
where ScheduleID = @schedID
	/* set nocount on */
	return
GO
/****** Object:  StoredProcedure [dbo].[spImportCashBoxHistory]    Script Date: 04/01/2014 22:07:29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spImportCashBoxHistory]
        @CustomerID integer = 29,
	@filename varchar(128),
	@xmlpath varchar(128),
	@xmldoc nText
    As

    CREATE TABLE #CashBoxDataHistoryX (
        AreaIDx int,
        MeterIDx int,
        DateTimeRem datetime,
        AmtAuto real
    ) ON [PRIMARY]



EXEC('Insert #CashBoxDataHistoryX SELECT F1,F2,CONVERT(datetime,F3 + '' '' + CONVERT(varchar(8),F4, 108),3)  as InspDateTime, F5 FROM OPENROWSET(''MICROSOFT.JET.OLEDB.4.0'',
 ''Text;Database=' + @xmlpath + ';Extended Properties="HDR=NO;";'',
 ''SELECT * FROM [' + @filename + ']'')')

DECLARE @MaxDate datetime
DECLARE @MinDate datetime
DECLARE @LastFileID int

Select @MaxDate=Max(DateTimeRem), @MinDate=Min(DateTimeRem) from #CashBoxDataHistoryX
Select @LastFileID=Max(FileProcessId) from CBImpFiles
Select @LastFileID=isnull(@LastFileID,0)+1

BEGIN TRAN
    INSERT CashBoxDataHistory
    SELECT @CustomerID,b.*,@LastFileID FROM CashBoxDataHistory a
    RIGHT OUTER JOIN #CashBoxDataHistoryX b ON
    a.DateTimeRem = b.DateTimeRem AND
    a.MeterID = b.MeterIDx
    WHERE a.DateTimeRem IS NULL
    IF (@@ERROR <> 0) GOTO ERR_HANDLER

    INSERT CBImpFiles
    VALUES (@CustomerID,@LastFileID,2,@filename,GETDATE(),@MaxDate,@MinDate)
    IF (@@ERROR <> 0) GOTO ERR_HANDLER

    DROP TABLE #CashBoxDataHistoryX
COMMIT TRAN
RETURN 0

ERR_HANDLER:
    ROLLBACK TRAN
    RAISERROR ('An error occurred during import. Rollback has occured. Please manually import the csv file.',16,1)
RETURN
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_getParkingSpaceID]    Script Date: 04/01/2014 22:07:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_getParkingSpaceID]
	@cid int
	,@aid int
	,@mid int
	,@bay int
	,@spaceid bigint output
AS
BEGIN
	select @spaceid = parkingspaceid from ParkingSpaces where customerid = @cid and areaid = @aid and meterid = @mid and baynumber =@bay
	
	if (@spaceid is null)begin
		print 'No ParkingSpaceId for '  + convert(varchar,@cid) + '/' + convert(varchar,@aid) + '/' + convert(varchar,@mid) + ':' + convert(varchar,@bay)		
	end 
	
	print 'ParkingSpaceId = ' + convert(varchar,@spaceid)
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_ParkingSpaces]    Script Date: 04/01/2014 22:07:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_ParkingSpaces] 	
	@CustomerId int,
	@CashBoxId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '----------------------------------------------'
	Print 'AUDIT ParkingSpaces'
	INSERT INTO [ParkingSpacesAudit]
           ([UserId]
           ,[UpdateDateTime]
           ,[ParkingSpaceId]
           ,[ServerID]
           ,[CustomerID]
           ,[AreaId]
           ,[MeterId]
           ,[BayNumber]
           ,[AddedDateTime]
           ,[Latitude]
           ,[Longitude]
           ,[HasSensor]
           ,[SpaceStatus]
           ,[DateActivated]
           ,[Comments]
           ,[DisplaySpaceNum]
           ,[DemandZoneId]
           ,[InstallDate]
           ,[ParkingSpaceType]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[AssetPendingReasonId])
	SELECT  @CreateUserId
           ,GETDATE()
           ,[ParkingSpaceId]
           ,[ServerID]
           ,[CustomerID]
           ,[AreaId]
           ,[MeterId]
           ,[BayNumber]
           ,[AddedDateTime]
           ,[Latitude]
           ,[Longitude]
           ,[HasSensor]
           ,[SpaceStatus]
           ,[DateActivated]
           ,[Comments]
           ,[DisplaySpaceNum]
           ,[DemandZoneId]
           ,[InstallDate]
           ,[ParkingSpaceType]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,@AssetPendingReasonId
           from ParkingSpaces       
END
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertEventLog]    Script Date: 04/01/2014 22:07:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROC [dbo].[sp_InsertEventLog]
 @uid bigint,
   @cid int,
   @aid int,
   @mid int,
   @EventCode int,
   @EventDateTime datetime   
   as
 begin 
 
 declare 
 
 @TimeType1 int,
 @TimeType2 int,
 @TimeType3 int
 
  
 if((datepart(dw, @EventDateTime))=1) or ((datepart (dw, @EventDateTime))=7)
 begin
  select @TimeType1=TimeTypeId from TimeType where TimeTypeDesc like 'Weekend'
 end
 
 else
 begin
   select @TimeType1=TimeTypeId from TimeType where TimeTypeDesc like 'Weekday'
 end
 
  select @TimeType2=TimeTypeId from TimeType where TimeTypeDesc like (datename (dw, @EventDateTime))
  if(RTRIM(RIGHT(CONVERT(VARCHAR, @EventDateTime, 100),2))='AM')
  
  begin
     select @TimeType3=TimeTypeId from TimeType where TimeTypeDesc like 'Morning'
  end
  else
  begin
       select @TimeType3=TimeTypeId from TimeType where TimeTypeDesc like 'Evening'
  end
 
  INSERT INTO [EventLogs]
           ([GlobalMeterId],[CustomerID],[AreaID],[MeterId],[EventDateTime],[EventCode],[EventSource] ,[EventUID],[TimeType1],[TimeType2],[TimeType3]
                     )
           values(
           dbo.GenGlobalID(@cid,@aid,@mid,0),
           @cid,@aid,@mid,
           @EventDateTime,
           @EventCode,0,@uid,@TimeType1,@TimeType2,@TimeType3         
           );
 end
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertOrUpdateMeter]    Script Date: 04/01/2014 22:07:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertOrUpdateMeter] 
	@AssetPendingReasonId int,
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@CreateUserId int,
	@RecordCreateDateTime DateTime,
	@RecordMigrationDateTime DateTime,
	@AssetId bigint,
	@AssetType int,
	@AssetName Varchar(500),
	@AssetModel int,
	@NextPreventativeMaintenance DateTime,
	@OperationalStatus int,
	@Latitude float,
	@Longitude float,
	@PhoneNumber varchar(100),
	@SpaceCount int,
	@DemandStatus int,
	@WarrantyExpiration DateTime,
	@DateInstalled DateTime,
	@LocationMeters Varchar(500),
	@LastPreventativeMaintenance DateTime,
	@AssetState int,
	@OperationalStatusTime DateTime
AS
DECLARE @GlobalMeterId bigint
	,@TimeZoneID int
BEGIN
	if exists(select * from Meters m						
				where m.CustomerID = @CustomerId
						and m.AreaID = @AreaId
						and m.MeterId = @MeterId )
	BEGIN
		Update m
			set m.MeterGroup = Case when @AssetType is null then m.MeterGroup else @AssetType end,
			m.MeterName= Case when @AssetName is null then m.MeterName else @AssetName end,
			m.MeterType = Case when @AssetModel is null then m.MeterType else @AssetModel end,
			m.NextPreventativeMaintenance =  Case when @NextPreventativeMaintenance is null then m.NextPreventativeMaintenance else @NextPreventativeMaintenance end,
			m.OperationalStatusID = Case when @OperationalStatus is null then m.OperationalStatusID else @OperationalStatus end,
			m.Latitude = Case when @Latitude is null then m.Latitude else @Latitude end,
			m.Longitude = Case when @Longitude is null then m.Longitude else @Longitude end,
			m.SMSNumber = Case when @PhoneNumber is null then m.SMSNumber else @PhoneNumber end,
			--			@SpaceCount,
			m.DemandZone = Case when @DemandStatus is null then m.DemandZone else @DemandStatus end,
			m.WarrantyExpiration = Case when @WarrantyExpiration is null then m.WarrantyExpiration else @WarrantyExpiration end,
			m.InstallDate = Case when @DateInstalled is null then m.InstallDate else @DateInstalled end,
			m.Location = Case when @LocationMeters is null then m.Location else @LocationMeters end,
			m.LastPreventativeMaintenance = Case when @LastPreventativeMaintenance is null then m.LastPreventativeMaintenance else @LastPreventativeMaintenance end,
			m.MeterState = Case when @AssetState is null then m.MeterState else @AssetState end,
			m.OperationalStatusTime =Case when @OperationalStatusTime is null then m.OperationalStatusTime else @OperationalStatusTime end
		from Meters m						
		where m.CustomerID = @CustomerId
				and m.AreaID = @AreaId
				and m.MeterId = @MeterId
	END ELSE BEGIN
	
		set @GlobalMeterId = dbo.GenGlobalID(@CustomerId,@AreaId,@MeterId,0)
		select @TimeZoneId = TimeZoneId from Customers where CustomerId = @CustomerID
		
		INSERT INTO [Meters]
           ([GlobalMeterId]
           ,[CustomerID]
           ,[AreaID]
           ,[MeterId]
           ,[SMSNumber]
           ,[MeterStatus]
           ,[TimeZoneID]
           --,[MeterRef]
           --,[EmporiaKey]
           ,[MeterName]
           ,[Location]
           --,[BayStart]
           --,[BayEnd]
           --,[Description]
           ,[GSMNumber]
           --,[SchedServTime]
           --,[RSFName]
           --,[RSFDateTime]
           --,[BarCode]
           ,[Latitude]
           ,[Longitude]
           --,[ProgramName]
           --,[MaxBaysEnabled]
           ,[MeterType]
           ,[MeterGroup]
           --,[MParkID]
           ,[MeterState]
           ,[DemandZone]
           --,[TypeCode]
           ,[OperationalStatusID]
           ,[InstallDate]
           --,[FreeParkingMinute]
           --,[RegulatedStatusID]
           ,[WarrantyExpiration]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           --,[OperationalStatusEndTime]
           --,[OperationalStatusComment]
		   )
		Values           
           (@GlobalMeterId
           ,@CustomerId
           ,@AreaId
           ,@MeterId
           ,@PhoneNumber
           ,1--[MeterStatus]
           ,@TimeZoneId
           --,[MeterRef]
           --,[EmporiaKey]
           ,@AssetName
           ,@LocationMeters
           --,[BayStart]
           --,[BayEnd]
           --,[Description]
           ,@PhoneNumber--,[GSMNumber]
           --,[SchedServTime]
           --,[RSFName]
           --,[RSFDateTime]
           --,[BarCode]
           ,@Latitude
           ,@Longitude
           --,[ProgramName]
           --,[MaxBaysEnabled]
           ,@AssetModel
           ,@AssetType
           --,[MParkID]
           ,@AssetState
           ,@DemandStatus
           --,[TypeCode]
           ,@OperationalStatus
           ,@DateInstalled
           --,[FreeParkingMinute]
           --,[RegulatedStatusID]
           ,@WarrantyExpiration
           ,@OperationalStatusTime
           ,@LastPreventativeMaintenance
           ,@NextPreventativeMaintenance
           --,[OperationalStatusEndTime]
           --,[OperationalStatusComment]
           )    
	END
	
	exec sp_Audit_Meters @Customerid,@AreaId,@MeterId,@AssetPendingReasonId,@CreateUserId
		
END
GO
/****** Object:  Table [dbo].[ConfigProfileSpace]    Script Date: 04/01/2014 22:07:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigProfileSpace](
	[ConfigProfileSpaceId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ConfigProfileId] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[ScheduledDate] [datetime] NULL,
	[ActivationDate] [datetime] NULL,
	[CreationDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[CreateDatetime] [datetime] NOT NULL,
	[ConfigStatus] [int] NULL,
	[UserId] [int] NULL,
 CONSTRAINT [PK_ConfigProfileSpace] PRIMARY KEY CLUSTERED 
(
	[ConfigProfileSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FDJobHistory]    Script Date: 04/01/2014 22:07:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FDJobHistory](
	[JobHistID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[JobID] [bigint] NOT NULL,
	[HistoryTS] [datetime] NOT NULL,
	[FDJobStatus] [int] NOT NULL,
 CONSTRAINT [PK_FDJobHistory] PRIMARY KEY CLUSTERED 
(
	[JobHistID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[AddParkingSpaces]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[AddParkingSpaces](
@mCustId int,
@mAreaId int,
@mMeterId int,
@mBayStart int,
@mBayEnd int
)
AS 
declare 
@i int,
@serverid int,
@globalId bigint
Begin	

			set @i=@mBayStart
			select top 1 @serverid=Instanceid  from serverinstance
			
			if @serverid<>0
			begin
				while @i<=@mBayEnd
				begin
						select @globalId= dbo.GenGlobalID(@mCustId,@mAreaId,@mMeterId,@i)
						Insert into ParkingSpaces (ParkingSpaceID,ServerId,CustomerId,AreaId,MeterId,BayNumber)
						Values (@globalId,@serverid,@mCustId,@mAreaId,@mMeterId,@i)
						set @i=@i+1

				end
			end
end
GO
/****** Object:  Table [dbo].[ParkingSpaceOccupancy]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpaceOccupancy](
	[ParkingSpaceOccupancyId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[LastStatus] [int] NOT NULL,
	[LastUpdatedTS] [datetime] NOT NULL,
	[GCustomerid] [int] NULL,
	[GAreaId] [int] NULL,
	[GMeterId] [int] NULL,
	[DiagnosticData] [image] NULL,
 CONSTRAINT [PK_ParkingSpaceOccupancy] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceOccupancyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingSpaceMeterBayMap]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpaceMeterBayMap](
	[ParkingSpaceMeterBayMapId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SensorParkingSpaceID] [bigint] NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[BayNumber] [int] NOT NULL,
 CONSTRAINT [PK_ParkingSpaceMeterBayMap] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceMeterBayMapId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingSpaceExpiryConfirmationEvent]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSpaceExpiryConfirmationEvent](
	[ParkingSpaceExpiryConfirmationEventId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[SpaceStatus] [tinyint] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[CreateDateTime] [datetime] NOT NULL,
 CONSTRAINT [PK_ParkingSpaceExpiryConfirmationEvent] PRIMARY KEY CLUSTERED 
(
	[ParkingSpaceExpiryConfirmationEventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ParkingSpaceDetails]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkingSpaceDetails](
	[ID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[SpaceType] [int] NOT NULL,
	[HBInterval] [int] NOT NULL,
	[ExpiryLevel] [varchar](100) NOT NULL,
	[PercentLevel] [varchar](100) NOT NULL,
 CONSTRAINT [PK_ParkingSpaceDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ParkingSchedules]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ParkingSchedules](
	[GlobalMeterId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[ScheduleID] [int] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[StartTime] [smalldatetime] NOT NULL,
	[TariffID] [int] NOT NULL,
	[OperationMode] [int] NOT NULL,
	[IntervalNum] [int] NULL,
 CONSTRAINT [PK_ParkingSchedules] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[ScheduleID] ASC,
	[DayOfWeek] ASC,
	[StartTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [ParkingSchedules_IDXGlobalMeterID] ON [dbo].[ParkingSchedules] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OLTEventDetails]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OLTEventDetails](
	[ParkingSpaceId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[EventCode] [int] NOT NULL,
	[ReceiptNumber] [int] NOT NULL,
	[PaidValue] [int] NULL,
	[PaidUntil] [int] NULL,
	[BayNumber] [int] NULL,
	[TransReference] [int] NOT NULL,
	[TransStatus] [int] NULL,
	[TransSubcode] [int] NULL,
	[TransType] [int] NULL,
	[PaymentTarget] [char](1) NULL,
	[CashKeySrID] [int] NULL,
	[CashKeyBalanceBefore] [int] NULL,
 CONSTRAINT [PK_OLTEventDetails] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC,
	[EventCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [OLTEventDetails_IDXParkingSpaceID] ON [dbo].[OLTEventDetails] 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qLastMeterStatus_Sub]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qLastMeterStatus_Sub]
AS
SELECT MeterID, AreaID, CustomerID, MAX(TimeOfOccurrance)
    AS MostRecent
FROM dbo.MeterStatusEvents
GROUP BY MeterID, AreaID, CustomerID
GO
/****** Object:  Table [dbo].[RegulatedHours]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegulatedHours](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NOT NULL,
	[DayOfWeek] [int] NOT NULL,
	[RegulatedStartTime] [int] NOT NULL,
	[RegulatedEndTime] [int] NOT NULL,
	[MaxStayMinute] [int] NOT NULL,
 CONSTRAINT [PK_RegulatedHours] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RateTransmissionJob]    Script Date: 04/01/2014 22:07:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RateTransmissionJob](
	[ID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TransmissionDetailsID] [bigint] NOT NULL,
	[JobID] [bigint] NULL,
	[PushQueuedTS] [datetime] NULL,
	[ActivatedTS] [datetime] NULL,
	[AckTS] [datetime] NULL,
	[Emailed] [datetime] NULL,
	[MeterId] [int] NOT NULL,
	[TransmissionID] [bigint] NULL,
 CONSTRAINT [PK_RateTransmissionJob] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qParkingSchedules]    Script Date: 04/01/2014 22:07:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qParkingSchedules]
AS
SELECT     TOP 100 PERCENT CustomerID, AreaID, MeterID, StartTime, OperationMode, CASE WHEN
                          (SELECT     TOP 1 p2.StartTime
                            FROM          ParkingSchedules p2
                            WHERE      p1.CustomerId = p2.CustomerId AND p1.AreaId = p2.AreaId AND p1.MeterId = p2.MeterId AND p1.DayOfWeek = p2.DayOfWeek AND 
                                                   p2.StartTime > P1.startTime AND p1.ScheduleId = P2.ScheduleId) IS NULL THEN '2000/01/01 11:59:00 PM' ELSE
                          (SELECT     TOP 1 p2.StartTime
                            FROM          ParkingSchedules p2
                            WHERE      p1.CustomerId = p2.CustomerId AND p1.AreaId = p2.AreaId AND p1.MeterId = p2.MeterId AND p1.DayOfWeek = p2.DayOfWeek AND 
                                                   p2.StartTime > P1.startTime AND p1.ScheduleId = P2.ScheduleId) END AS EndTime, DayOfWeek, TariffID, IntervalNum, ScheduleID
FROM         dbo.ParkingSchedules p1
GO
/****** Object:  Table [dbo].[GSMConnectionLogs]    Script Date: 04/01/2014 22:07:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[GSMConnectionLogs](
	[GlobalMeterId] [bigint] NULL,
	[StartDateTime] [datetime] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[PortNo] [int] NOT NULL,
	[EndDateTime] [datetime] NOT NULL,
	[ConnectionStatus] [int] NOT NULL,
	[Memo] [varchar](100) NULL,
	[EventUID] [bigint] NULL,
	[TimeType1] [int] NULL,
	[TimeType2] [int] NULL,
	[TimeType3] [int] NULL,
	[TimeType4] [int] NULL,
	[TimeType5] [int] NULL,
 CONSTRAINT [PK_GSMConnectionLogs] PRIMARY KEY CLUSTERED 
(
	[StartDateTime] ASC,
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[PortNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [GSMConnectionLogs_IDX_CAMCS] ON [dbo].[GSMConnectionLogs] 
(
	[ConnectionStatus] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[CustomerID] ASC,
	[StartDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [GSMConnectionLogs_IDXGlobalMeterID] ON [dbo].[GSMConnectionLogs] 
(
	[GlobalMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [IX_GSMConnectionLogs_Time] ON [dbo].[GSMConnectionLogs] 
(
	[CustomerID] DESC,
	[AreaID] DESC,
	[MeterId] DESC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[pv_Occupancy]    Script Date: 04/01/2014 22:07:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_Occupancy]
WITH SCHEMABINDING 
AS
SELECT     ps.ParkingSpaceId AS SpaceId, se.SensorID, me.MeterId, os.StatusDesc AS OccupancyStatus, ops.OperationalStatusDesc AS OperationalStatus, 
                      me.Location AS Street, st.SpaceTypeDesc AS SpaceType, ncs.NonCompliantStatusDesc AS NonCompliantStatus, stpx.TimeType1, stpx.TimeType2, stpx.TimeType3, 
                      stpx.TimeType4, stpx.TimeType5, cg.DisplayName AS Suburb, ar.AreaName AS Area, zo.ZoneName AS Zone, 
                      (CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName WHEN 10 THEN se.SensorName WHEN 11 THEN 'Cashbox' WHEN 13 THEN gw.Description
                       END) AS AssetName, 
                      (CASE me.MeterGroup WHEN 0 THEN me.MeterId WHEN 1 THEN me.MeterId WHEN 10 THEN se.SensorID WHEN 11 THEN cb.CashBoxID WHEN 13 THEN gw.GateWayID
                       END) AS AssetId, mg.MeterGroupDesc AS AssetType, stpx.ArrivalTime, mm.Customerid, stpx.SensorPaymentTransactionID, stpx.DepartureTime, 
                      stpx.TotalOccupiedMinute, stpx.TotalTimePaidMinute, stpx.TotalAmountInCent, ps.DisplaySpaceNum AS BayName, ps.BayNumber, 
                      dz.DemandZoneDesc AS DemandArea
FROM         dbo.SensorPaymentTransaction AS stpx INNER JOIN
                      dbo.ParkingSpaces AS ps ON stpx.ParkingSpaceId = ps.ParkingSpaceId INNER JOIN
                      dbo.Meters AS me ON ps.CustomerID = me.CustomerID AND ps.AreaId = me.AreaID AND ps.MeterId = me.MeterId INNER JOIN
                      dbo.OccupancyStatus AS os ON stpx.OccupancyStatus = os.StatusID INNER JOIN
                      dbo.OperationalStatus AS ops ON stpx.OperationalStatus = ops.OperationalStatusId INNER JOIN
                      dbo.MeterMap AS mm ON ps.MeterId = mm.MeterId AND ps.CustomerID = mm.Customerid AND ps.AreaId = mm.Areaid LEFT OUTER JOIN
                      dbo.Areas AS ar ON ar.AreaID = mm.AreaId2 AND ar.CustomerID = ps.CustomerID LEFT OUTER JOIN
                      dbo.Zones AS zo ON mm.ZoneId = zo.ZoneId LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg ON mm.CustomGroup1 = cg.CustomGroupId LEFT OUTER JOIN
                      dbo.ParkingSpaceDetails AS psd ON psd.ParkingSpaceId = ps.ParkingSpaceId LEFT OUTER JOIN
                      dbo.SpaceType AS st ON st.SpaceTypeId = psd.SpaceType LEFT OUTER JOIN
                      dbo.NonCompliantStatus AS ncs ON stpx.NonCompliantStatus = ncs.NonCompliantStatusID LEFT OUTER JOIN
                      dbo.Sensors AS se ON mm.Customerid = se.CustomerID AND se.SensorID = mm.SensorID LEFT OUTER JOIN
                      dbo.Gateways AS gw ON gw.GateWayID = mm.GatewayID LEFT OUTER JOIN
                      dbo.CashBox AS cb ON cb.CashBoxID = mm.CashBoxID LEFT OUTER JOIN
                      dbo.MeterGroup AS mg ON me.MeterGroup = mg.MeterGroupId LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = me.DemandZone
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "stpx"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 114
		       Right = 260
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ps"
		    Begin Extent = 
		       Top = 6
		       Left = 298
		       Bottom = 114
		       Right = 487
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "me"
		    Begin Extent = 
		       Top = 6
		       Left = 525
		       Bottom = 114
		       Right = 714
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "os"
		    Begin Extent = 
		       Top = 6
		       Left = 752
		       Bottom = 84
		       Right = 903
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ops"
		    Begin Extent = 
		       Top = 6
		       Left = 941
		       Bottom = 84
		       Right = 1131
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 84
		       Left = 752
		       Bottom = 192
		       Right = 903
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ar"
		    Begin Extent = 
		       Top = 84
		       Left = 941
		       Bottom = 192
		       Right = 1092
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 E' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_Occupancy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'nd
		 Begin Table = "zo"
		    Begin Extent = 
		       Top = 84
		       Left = 1130
		       Bottom = 192
		       Right = 1292
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg"
		    Begin Extent = 
		       Top = 114
		       Left = 38
		       Bottom = 222
		       Right = 193
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "st"
		    Begin Extent = 
		       Top = 114
		       Left = 231
		       Bottom = 192
		       Right = 387
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ncs"
		    Begin Extent = 
		       Top = 114
		       Left = 425
		       Bottom = 192
		       Right = 625
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "se"
		    Begin Extent = 
		       Top = 192
		       Left = 231
		       Bottom = 300
		       Right = 420
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "gw"
		    Begin Extent = 
		       Top = 192
		       Left = 458
		       Bottom = 300
		       Right = 647
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cb"
		    Begin Extent = 
		       Top = 192
		       Left = 685
		       Bottom = 300
		       Right = 874
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mg"
		    Begin Extent = 
		       Top = 192
		       Left = 912
		       Bottom = 270
		       Right = 1072
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_Occupancy'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_Occupancy'
GO
/****** Object:  View [dbo].[pv_EventsCollectionCommEvent]    Script Date: 04/01/2014 22:07:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsCollectionCommEvent]
AS
SELECT     cds.CustomerId, el.EventDateTime AS DateTime, cds.EventUID, cds.MeterId AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, el.EventCode, mm.AreaId2 AS AreaID, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, el.TimeType1, el.TimeType2, el.TimeType3, el.TimeType4, 
                      el.TimeType5, cds.Amount, cds.CollDateTime AS CollectionTime, cds.InsertionDateTime AS InsertionTime, cds.OldCashBoxID AS PreviousCBID, 
                      cds.NewCashBoxID AS NewCBID, cds.CashboxSequenceNo AS SequenceNumber, z.ZoneName AS Zone, a.AreaName AS Area
FROM         dbo.CollDataSumm AS cds INNER JOIN
                      dbo.EventLogs AS el ON cds.AreaId = el.AreaID AND cds.CustomerId = el.CustomerID AND el.MeterId = cds.MeterId AND 
                      cds.CollDateTime = el.EventDateTime LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = cds.CustomerId AND mm.Areaid = cds.AreaId AND mm.MeterId = cds.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON cds.CustomerId = m.CustomerID AND cds.AreaId = m.AreaID AND cds.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
WHERE     (ec.EventCode = 2005)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "cds"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 229
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 267
		       Bottom = 125
		       Right = 435
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 245
		       Right = 198
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 126
		       Left = 236
		       Bottom = 245
		       Right = 471
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 246
		       Left = 38
		       Bottom = 365
		       Right = 317
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 246
		       Left = 355
		       Bottom = 365
		       Right = 534
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 366
		       Left = 38
		       Bottom = 455
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCommEvent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 366
		       Left = 240
		       Bottom = 455
		       Right = 415
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 456
		       Left = 38
		       Bottom = 575
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCommEvent'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCommEvent'
GO
/****** Object:  View [dbo].[pv_EventsCollectionCBR]    Script Date: 04/01/2014 22:07:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsCollectionCBR]
AS
SELECT     cbdi.CustomerId, el.EventDateTime AS DateTime, cbdi.EventUID, cbdi.MeterId AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, el.EventCode, mm.AreaId2 AS AreaID, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, el.TimeType1, el.TimeType2, el.TimeType3, el.TimeType4, 
                      el.TimeType5, cbdi.AmtAuto, cbdi.AmtManual, cbdi.AmtDiff AS AmtDifference, cbdi.CashBoxId AS CBID, cbdi.DateTimeIns AS InsertionTime, cbdi.OperatorId, 
                      cbdi.DateTimeRead AS ReadTime, cbdi.DateTimeRem AS RemovalTime, cbdi.CashboxSequenceNo AS SequenceNumber, cbdi.FileName AS TransactionFileName, 
                      cbdi.FirmwareVer AS Version, z.ZoneName AS Zone, a.AreaName AS Area
FROM         dbo.CashBoxDataImport AS cbdi INNER JOIN
                      dbo.EventLogs AS el ON cbdi.AreaId = el.AreaID AND cbdi.CustomerId = el.CustomerID AND el.MeterId = cbdi.MeterId AND 
                      el.EventDateTime = cbdi.DateTimeRead LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = cbdi.CustomerId AND mm.Areaid = cbdi.AreaId AND mm.MeterId = cbdi.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON cbdi.CustomerId = m.CustomerID AND cbdi.AreaId = m.AreaID AND cbdi.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "cbdi"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 229
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 267
		       Bottom = 125
		       Right = 435
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 245
		       Right = 198
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 126
		       Left = 236
		       Bottom = 245
		       Right = 471
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 246
		       Left = 38
		       Bottom = 365
		       Right = 317
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 246
		       Left = 355
		       Bottom = 365
		       Right = 534
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 366
		       Left = 38
		       Bottom = 455
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
	       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCBR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 366
		       Left = 240
		       Bottom = 455
		       Right = 415
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 456
		       Left = 38
		       Bottom = 575
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCBR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsCollectionCBR'
GO
/****** Object:  View [dbo].[OccupancyV]    Script Date: 04/01/2014 22:07:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OccupancyV]
WITH SCHEMABINDING 
AS
SELECT     	mm.Customerid, 
			sptx.ArrivalTime AS DateTime, 
			ps.ParkingSpaceId AS SpaceId, 
			se.SensorID, 
			me.MeterId, 
			os.StatusDesc AS OccupancyStatus, 
			ops.OperationalStatusDesc AS OperationalStatus, 
            me.Location AS Street, 
			st.SpaceTypeDesc AS SpaceType, 
			ncs.NonCompliantStatusDesc AS NonCompliantStatus, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
			cg.DisplayName AS Suburb, 
			ar.AreaName AS Area, 
			zo.ZoneName AS Zone, 
            (CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName WHEN 10 THEN se.SensorName WHEN 11 THEN 'Cashbox' WHEN 13 THEN gw.Description END) AS AssetName, 
            (CASE me.MeterGroup WHEN 0 THEN me.MeterId WHEN 1 THEN me.MeterId WHEN 10 THEN se.SensorID WHEN 11 THEN cb.CashBoxID WHEN 13 THEN gw.GateWayID END) AS AssetId, 
			mg.MeterGroupDesc AS AssetType, 
			sptx.ArrivalTime, 
			sptx.SensorPaymentTransactionID, 
			sptx.DepartureTime, 
            sptx.TotalOccupiedMinute, 
			sptx.TotalTimePaidMinute, 
			sptx.TotalAmountInCent, 
			ps.DisplaySpaceNum AS BayName, 
			ps.BayNumber, 
            dz.DemandZoneDesc AS DemandArea
FROM    dbo.SensorPaymentTransaction AS sptx 
			INNER JOIN dbo.ParkingSpaces AS ps 
				ON sptx.ParkingSpaceId = ps.ParkingSpaceId 
			INNER JOIN dbo.Meters AS me 
				ON ps.CustomerID = me.CustomerID 
				AND ps.AreaId = me.AreaID 
				AND ps.MeterId = me.MeterId 
			INNER JOIN dbo.OccupancyStatus AS os 
				ON sptx.OccupancyStatus = os.StatusID 
			INNER JOIN dbo.OperationalStatus AS ops 
				ON sptx.OperationalStatus = ops.OperationalStatusId 
			INNER JOIN dbo.MeterMap AS mm 
				ON ps.MeterId = mm.MeterId 
				AND ps.CustomerID = mm.Customerid 
				AND ps.AreaId = mm.Areaid 
			LEFT OUTER JOIN dbo.Areas AS ar 
				ON ar.AreaID = mm.AreaId2 
				AND ar.CustomerID = ps.CustomerID 
			LEFT OUTER JOIN dbo.Zones AS zo 
				ON mm.ZoneId = zo.ZoneId 
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg 
				ON mm.CustomGroup1 = cg.CustomGroupId 
			LEFT OUTER JOIN dbo.ParkingSpaceDetails AS psd 
				ON psd.ParkingSpaceId = ps.ParkingSpaceId 
			LEFT OUTER JOIN dbo.SpaceType AS st 
				ON st.SpaceTypeId = psd.SpaceType 
			LEFT OUTER JOIN dbo.NonCompliantStatus AS ncs 
				ON sptx.NonCompliantStatus = ncs.NonCompliantStatusID 
			LEFT OUTER JOIN dbo.Sensors AS se 
				ON mm.Customerid = se.CustomerID 
				AND se.SensorID = mm.SensorID 
			LEFT OUTER JOIN dbo.Gateways AS gw 
				ON gw.GateWayID = mm.GatewayID 
			LEFT OUTER JOIN dbo.CashBox AS cb 
				ON cb.CashBoxID = mm.CashBoxID 
			LEFT OUTER JOIN dbo.MeterGroup AS mg 
				ON me.MeterGroup = mg.MeterGroupId 
			LEFT OUTER JOIN dbo.DemandZone AS dz 
				ON dz.DemandZoneId = me.DemandZone
GO
/****** Object:  View [dbo].[OccupancyRateSummaryV]    Script Date: 04/01/2014 22:07:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OccupancyRateSummaryV]
WITH SCHEMABINDING 
AS
SELECT     				sptx.Customerid, 
						sptx.ArrivalTime, 
						DATEADD(Minute, DATEDIFF(Minute, 0, sptx.ArrivalTime), 0) AS ArrivalTimeRND, 
						CAST((CASE WHEN ArrivalTime IS NULL THEN 0 ELSE 1 END) AS decimal(8, 6)) AS ArrivalCount,
						sptx.DepartureTime, 
						DATEADD(Minute, DATEDIFF(Minute, 0, sptx.DepartureTime), 0) AS DepartureTimeRND,
						CAST((CASE WHEN DepartureTime IS NULL THEN 0 ELSE 1 END) AS decimal(8, 6)) AS DepartureCount,  
						sptx.SensorPaymentTransactionID, 
						sptx.ParkingSpaceId AS SpaceId, 
						st.SpaceTypeDesc AS SpaceType, 
						os.StatusDesc AS OccupancyStatus, 
						ops.OperationalStatusDesc AS OperationalStatus, 
						me.Location AS Street, 
					(SELECT AreaName
						FROM dbo.Areas
						WHERE (AreaID = mm.AreaId2) AND (CustomerID = ps.CustomerID)) AS Area, 
					(SELECT ZoneName
						FROM dbo.Zones
						WHERE (ZoneId = mm.ZoneId) AND (customerID = ps.Customerid)) AS Zone, 
					(SELECT DisplayName
						FROM dbo.CustomGroup1
						WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb,  
						ncs.NonCompliantStatusDesc AS NonCompliantStatus, 
						sptx.LastTxExpiryTime, 
					CASE sptx.NonCompliantStatus 
							WHEN NULL THEN 0
							WHEN 3 THEN NULL
							WHEN 2 THEN NULL
						ELSE (DATEDIFF(minute, sptx.DepartureTime, sptx.LastTxExpiryTime)) END AS OverstayMinutes, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
						sptx.SensorID,
						se.SensorName, 
						(CASE me.MeterGroup WHEN 0 THEN me.MeterId WHEN 1 THEN me.MeterId END) AS MeterId, 
						(CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName END) AS MeterName, 
						(CASE me.MeterGroup WHEN 0 THEN mg.MeterGroupDesc WHEN 1 THEN mg.MeterGroupDesc END) AS MeterType, 
						sptx.TotalOccupiedMinute, 
						sptx.TotalTimePaidMinute,
						(CASE sptx.FreeParkingUsed WHEN 0 THEN 'NO' WHEN 1 THEN 'YES' END) AS FreeParkingUsed, 
						sptx.FreeParkingTime, 
						sptx.FreeParkingMinute,
						dz.DemandZoneDesc AS DemandArea
FROM         dbo.SensorPaymentTransaction AS sptx 
				LEFT OUTER JOIN dbo.ParkingSpaces AS ps 
					  ON sptx.ParkingSpaceId = ps.ParkingSpaceId 
					  AND sptx.CustomerID = ps.Customerid
				LEFT OUTER JOIN dbo.Meters AS me 
					  ON sptx.CustomerID = me.CustomerID 
					  AND ps.AreaId = me.AreaID 
					  AND ps.MeterId = me.MeterId
				LEFT OUTER JOIN dbo.OccupancyStatus AS os 
					  ON sptx.OccupancyStatus = os.StatusID 
				INNER JOIN dbo.OperationalStatus AS ops 
					  ON sptx.OperationalStatus = ops.OperationalStatusId 
				LEFT OUTER JOIN dbo.MeterMap AS mm 
					  ON ps.MeterId = mm.MeterId 
					  AND ps.CustomerID = mm.Customerid 
					  AND ps.AreaId = mm.Areaid 
				LEFT OUTER JOIN dbo.Areas AS ar 
					  ON ar.AreaID = mm.AreaId2 
					  AND ar.CustomerID = ps.CustomerID 
				LEFT OUTER JOIN dbo.Zones AS zo 
					  ON mm.ZoneId = zo.ZoneId 
					  AND sptx.CustomerID = zo.CustomerID 
				LEFT OUTER JOIN dbo.CustomGroup1 AS cg 
					  ON mm.CustomGroup1 = cg.CustomGroupId 
				LEFT OUTER JOIN dbo.ParkingSpaceDetails AS psd 
					  ON psd.ParkingSpaceId = ps.ParkingSpaceId 
				LEFT OUTER JOIN dbo.SpaceType AS st 
					  ON st.SpaceTypeId = psd.SpaceType 
				LEFT OUTER JOIN dbo.NonCompliantStatus AS ncs 
					  ON sptx.NonCompliantStatus = ncs.NonCompliantStatusID 
				LEFT OUTER JOIN dbo.Sensors AS se 
					  ON sptx.ParkingSpaceId = se.ParkingSpaceId 
					  AND sptx.Customerid = se.CustomerID 
					  AND sptx.SensorID = se.SensorID 
				LEFT OUTER JOIN dbo.Gateways AS gw 
					  ON gw.GateWayID = mm.GatewayID 
				LEFT OUTER JOIN dbo.CashBox AS cb 
					  ON cb.CashBoxID = mm.CashBoxID 
				LEFT OUTER JOIN dbo.MeterGroup AS mg 
					  ON me.MeterGroup = mg.MeterGroupId 
				LEFT OUTER JOIN dbo.DemandZone AS dz 
					  ON dz.DemandZoneId = me.DemandZone
GO
/****** Object:  View [dbo].[MaintenanceEventsV]    Script Date: 04/01/2014 22:07:36 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MaintenanceEventsV] 
AS 
SELECT
			aa.CustomerID, 
			aa.AreaID, 
			aa.EventUID, 
			aa.MeterId, 
			aa.MeterId AS AssetId,
			(SELECT
				MeterGroupDesc
			FROM dbo.AssetType AS at
			WHERE (CustomerId = aa.CustomerID) AND (MeterGroupId = me.MeterGroup))
			AS AssetType,
			CASE me.MeterGroup
					WHEN 0 THEN me.MeterName
					WHEN 1 THEN me.MeterName
					WHEN 10 THEN (SELECT
						SensorName
					FROM Sensors
					WHERE SensorID = mm.SensorID)
					WHEN 13 THEN (SELECT
						Description
					FROM Gateways
					WHERE GateWayID = mm.GatewayID)
				END AS AssetName,
			aa.EventCode AS AlarmCode,
			et.EventTypeDesc AS EventClass,
			aa.EventCode,
			mm.AreaId2,
			mm.ZoneId,
			me.Location AS Street,
			cg1.DisplayName AS Suburb,
			dz.DemandZoneDesc AS DemandArea,
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
			ec.EventDescVerbose AS AlarmDescription,
			almt.TierDesc AS AlarmClass,
			mme.MaintenanceCode AS 'Repair Code',
			(SELECT
				EventSourceDesc
			FROM dbo.EventSources
			WHERE (EventSourceCode = ec.EventSource))
			AS Source,
			(SELECT Name
					FROM dbo.TechnicianDetails
					WHERE (TechnicianId = (SELECT TechnicianID
					FROM dbo.WorkOrder
					WHERE (WorkOrderId = aa.WorkOrderId)))) AS 'Technician Name', 
			(SELECT TechnicianID
					FROM dbo.WorkOrder
					WHERE (WorkOrderId = aa.WorkOrderId)) AS TechnicianID, 
			aa.WorkOrderId, 
			aa.TimeOfOccurrance AS DateTime, 
			aa.TimeOfNotification AS TimeOfNotification,
			aa.TimeOfClearance AS TimeOfClearance,
			aa.SLADue AS TimeDueSLA,
			CAST('YES' AS varchar) AS IsClosed,
			z.ZoneName AS Zone,
			a.AreaName AS Area
FROM dbo.HistoricalAlarms AS aa
		LEFT OUTER JOIN dbo.MeterMap AS mm
			ON mm.Customerid = aa.CustomerID 
			AND mm.Areaid = aa.AreaID 
			AND mm.MeterId = aa.MeterId
		LEFT OUTER JOIN dbo.Meters AS me
			ON aa.CustomerID = me.CustomerID 
			AND aa.AreaID = me.AreaID 
			AND aa.MeterId = me.MeterId
		LEFT OUTER JOIN dbo.EventCodes AS ec
			ON aa.CustomerID = ec.CustomerID 
			AND aa.EventSource = ec.EventSource 
			AND aa.EventCode = ec.EventCode
		LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
			ON cg1.CustomGroupId = mm.CustomGroup1
		LEFT OUTER JOIN dbo.DemandZone AS dz
			ON dz.DemandZoneId = me.DemandZone
		LEFT OUTER JOIN dbo.EventType AS et
			ON et.EventTypeId = ec.EventType
		LEFT OUTER JOIN dbo.AlarmTier AS almt
			ON almt.Tier = ec.AlarmTier
		LEFT OUTER JOIN dbo.SFMeterMaintenanceEvent AS mme
			ON mme.WorkOrderID = aa.WorkOrderId
		LEFT OUTER JOIN dbo.Areas AS a 
			ON a.AreaID = mm.AreaId2 
			AND a.CustomerID = mm.Customerid
		LEFT OUTER JOIN dbo.Zones AS z 
			ON z.ZoneId = mm.ZoneId 
			AND z.customerID = mm.Customerid

UNION

SELECT
			aa.CustomerID, 
			aa.AreaID, 
			aa.EventUID, 
			aa.MeterId, 
			aa.MeterId AS AssetId,
			at.MeterGroupDesc AS AssetType,
			CASE me.MeterGroup
					WHEN 0 THEN me.MeterName
					WHEN 1 THEN me.MeterName
					WHEN 10 THEN (SELECT
						SensorName
					FROM Sensors
					WHERE SensorID = mm.SensorID)
					WHEN 13 THEN (SELECT
						Description
					FROM Gateways
					WHERE GateWayID = mm.GatewayID)
				END AS AssetName,
			ec.EventCode AS AlarmCode,
			et.EventTypeDesc AS EventClass,
			aa.EventCode,
			mm.AreaId2 AS AreaID,
			mm.ZoneId,
			me.Location AS Street,
			cg1.DisplayName AS Suburb,
			dz.DemandZoneDesc AS DemandArea,
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
			ec.EventDescVerbose AS AlarmDescription,
			almt.TierDesc AS AlarmClass,
			NULL AS 'Repair Code',
			(SELECT
				EventSourceDesc
			FROM dbo.EventSources
			WHERE (EventSourceCode = ec.EventSource))
			AS Source, 
			(SELECT Name
					FROM dbo.TechnicianDetails
					WHERE (TechnicianId = (SELECT TechnicianID
					FROM dbo.WorkOrder
					WHERE (WorkOrderId = aa.WorkOrderId)))) AS 'Technician Name', 
			(SELECT TechnicianID
					FROM dbo.WorkOrder
					WHERE (WorkOrderId = aa.WorkOrderId)) AS TechnicianID, 
			aa.WorkOrderId, 
			aa.TimeOfOccurrance AS DateTime, 
			aa.TimeOfNotification AS TimeOfNotification, 
			NULL AS TimeOfClearance,
			aa.SLADue AS TimeDueSLA, 
			CAST('NO' AS varchar) AS IsClosed, 
			z.ZoneName AS Zone, 
			a.AreaName AS Area
FROM dbo.ActiveAlarms AS aa
		LEFT OUTER JOIN dbo.MeterMap AS mm
			ON mm.Customerid = aa.CustomerID 
			AND mm.Areaid = aa.AreaID 
			AND mm.MeterId = aa.MeterId
		LEFT OUTER JOIN dbo.Meters AS me
			ON mm.Customerid = me.CustomerID 
			AND mm.Areaid = me.AreaID 
			AND mm.MeterId = me.MeterId 
			AND aa.CustomerID = me.CustomerID 
			AND aa.AreaID = me.AreaID 
			AND aa.MeterId = me.MeterId
		LEFT OUTER JOIN dbo.EventCodes AS ec
			ON aa.CustomerID = ec.CustomerID 
			AND aa.EventSource = ec.EventSource 
			AND aa.EventCode = ec.EventCode
		LEFT OUTER JOIN dbo.AssetType AS at
			ON at.CustomerId = aa.CustomerID 
			AND at.MeterGroupId = me.MeterGroup
		LEFT OUTER JOIN dbo.Areas AS a
			ON a.AreaID = mm.Areaid 
			AND a.CustomerID = mm.Customerid
		LEFT OUTER JOIN dbo.Zones AS z
			ON z.ZoneId = mm.ZoneId 
			AND z.customerID = mm.Customerid
		LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
			ON cg1.CustomGroupId = mm.CustomGroup1
		LEFT OUTER JOIN dbo.DemandZone AS dz
			ON dz.DemandZoneId = me.DemandZone
		LEFT OUTER JOIN dbo.EventType AS et
			ON et.EventTypeId = ec.EventType
		LEFT OUTER JOIN dbo.AlarmTier AS almt
			ON almt.Tier = ec.AlarmTier
GO
/****** Object:  View [dbo].[LastCollectionDateTimeV]    Script Date: 04/01/2014 22:07:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create View [dbo].[LastCollectionDateTimeV] AS
SELECT 
		me.CustomerID, 
		me.AreaID, 
		mm.AreaId2, 
		mm.ZoneId, 
		me.Location, 
		cg1.DisplayName AS Suburb, 
		me.DemandZone, 
		me.MeterName, 
		(SELECT MeterGroupDesc FROM dbo.AssetType WHERE (MeterGroupId = me.MeterGroup) AND (CustomerId = me.CustomerID)) AS MeterType, 
		me.MeterID, 
		MAX(cds.CollDateTime) AS LastCollectionDateTime
FROM dbo.Meters AS me 
	RIGHT OUTER JOIN dbo.CollDataSumm AS cds 
		ON cds.CustomerId = me.CustomerID 
		AND cds.MeterId = me.MeterID 
		AND cds.AreaId = me.AreaID
	LEFT OUTER JOIN dbo.MeterMap AS mm 
		ON me.CustomerId = mm.CustomerID 
		AND me.MeterId = mm.MeterID 
		AND me.AreaId = mm.AreaID
	LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
		ON cg1.CustomGroupId = mm.CustomGroup1
		GROUP BY me.CustomerID, me.AreaID, mm.AreaId2, me.MeterID, mm.ZoneId, me.Location, me.MeterGroup, me.MeterName, cg1.DisplayName, me.DemandZone
		HAVING MAX(cds.CollDateTime) <= GETDATE()
GO
/****** Object:  View [dbo].[HistoricalAlarmsV]    Script Date: 04/01/2014 22:07:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[HistoricalAlarmsV]
AS
SELECT     				ha.CustomerID, 
						ha.MeterId AS AssetID, 
						mm.AreaId2 AS AreaID, 
						mm.ZoneID, 
						me.Location AS Street,
						(SELECT AreaName
                            FROM dbo.Areas
                            WHERE (AreaID = mm.AreaId2) AND (CustomerID = ha.CustomerID)) AS Area, 
                        (SELECT ZoneName
                            FROM dbo.Zones
                            WHERE (ZoneId = mm.ZoneId) AND (customerID = mm.Customerid)) AS Zone,
                        (SELECT DisplayName
                            FROM dbo.CustomGroup1
                            WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb,
						ha.TimeOfOccurrance AS 'Time of Occurrence', 
						(SELECT REPLACE(convert(varchar(16), ha.TimeOfOccurrance, 111), '/', '-')) AS 'Date of Occurence', 
						ha.TimeOfNotification AS 'Time of Report',  
						ha.TimeOfClearance AS 'Time of Clear' , 
						DATEADD(minute, ec.SLAMinutes, ha.TimeOfNotification) as 'Service Target Time', 
						CASE WHEN DATEDIFF(minute, ha.TimeOfOccurrance, ha.TimeOfClearance) <0 THEN 0 ELSE DATEDIFF(minute, ha.TimeOfOccurrance, ha.TimeOfClearance) END AS 'Minute Duration of Alarm', 
						ha.EventUID, 						
						ha.AlarmUID, 
						ec.EventCode AS AlarmCode, 
						ec.EventDescVerbose AS AlarmCodeDesc,
						(SELECT TierDesc
                            FROM dbo.AlarmTier
                            WHERE (Tier = ec.AlarmTier)) AS 'Alarm Class', 
							(SELECT EventSourceDesc
                            FROM dbo.EventSources
                            WHERE (EventSourceCode = ec.EventSource)) AS 'Source of Alarm Desc',
                          (SELECT EventSourceCode
                            FROM dbo.EventSources AS EventSources
                            WHERE (EventSourceCode = ec.EventSource)) AS 'Alarm Source Code',
                          (SELECT MeterGroupDesc
                            FROM dbo.AssetType
                            WHERE (MeterGroupId = me.MeterGroup) AND (CustomerId = ha.CustomerID)) AS AssetType, 
						CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName WHEN 10 THEN
                        (SELECT SensorName
                            FROM Sensors
                            WHERE SensorID = mm.SensorID) WHEN 13 THEN
                        (SELECT Description
                            FROM Gateways
                            WHERE GateWayID = mm.GatewayID) END AS AssetName, 
							CASE me.MeterGroup WHEN 0 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 1 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 10 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                        (SELECT SensorState
                            FROM Sensors
                            WHERE SensorID = mm.SensorID)) WHEN 13 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                        (SELECT GatewayState
                            FROM Gateways
                            WHERE GateWayID = mm.GatewayID)) END AS AssetState,
                        (SELECT AlarmStatusDesc
                            FROM dbo.AlarmStatus
                            WHERE (AlarmStatusId = 3)) AS AlarmStatus, 
                        (SELECT TechnicianID
                            FROM dbo.WorkOrder
                            WHERE (WorkOrderId = ha.WorkOrderId)) AS TechnicianId, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = ha.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = ha.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = ha.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = ha.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = ha.TimeType5) AND (ColumnMap = 'TimeType4')) AS TimeClass5, 
                          (SELECT TargetServiceDesignationDesc
                            FROM dbo.TargetServiceDesignation
                            WHERE (TargetServiceDesignationId = ha.TargetServiceDesignation) AND (CustomerId = ha.CustomerID)) AS TargetService, 
						dz.DemandZoneDesc AS DemandArea
FROM         dbo.MeterMap AS mm
				INNER JOIN
						dbo.HistoricalAlarms AS ha 
						ON mm.Customerid = ha.CustomerID 
						AND mm.Areaid = ha.AreaID 
						AND mm.MeterId = ha.MeterId
				INNER JOIN
						dbo.Meters AS me 
						ON mm.Customerid = me.CustomerID 
						AND mm.Areaid = me.AreaID 
						AND mm.MeterId = me.MeterId 
						AND ha.CustomerID = me.CustomerID 
						AND ha.AreaID = me.AreaID 
						AND ha.MeterId = me.MeterId 
				INNER JOIN
						dbo.EventCodes AS ec 
						ON ha.CustomerID = ec.CustomerID 
						AND ha.EventSource = ec.EventSource 
						AND ha.EventCode = ec.EventCode 
				LEFT OUTER JOIN
						dbo.DemandZone AS dz ON dz.DemandZoneId = me.DemandZone
GO
/****** Object:  View [dbo].[EventsCollectionCommEventV]    Script Date: 04/01/2014 22:07:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventsCollectionCommEventV]
AS
SELECT
				cds.CustomerId,
				el.EventDateTime AS DateTime, 
				cds.EventUID, 
				el.MeterId AS AssetId, 
				at.MeterGroupDesc AS AssetType, 
				CASE me.MeterGroup
				WHEN 0 THEN me.MeterName
				WHEN 1 THEN me.MeterName
				WHEN 10 THEN (SELECT
					SensorName
				FROM Sensors
				WHERE SensorID = mm.SensorID)
				WHEN 13 THEN (SELECT
					Description
				FROM Gateways
				WHERE GateWayID = mm.GatewayID) END AS AssetName, 
				ec.EventCode AS AlarmCode, 
				et.EventTypeDesc AS EventClass, 
				el.EventCode, 
				mm.AreaId2 AS AreaID, 
				mm.ZoneId, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street,
				cg1.DisplayName AS Suburb,
				dz.DemandZoneDesc AS DemandArea,
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				cds.Amount, 
				cds.CollDateTime AS 'DateTime of Collection', 
				cds.InsertionDateTime AS 'DateTime of Insertion', 
				cds.OldCashBoxID, 
				cds.NewCashBoxID, 
				cds.CashboxSequenceNo AS SequenceNumber 
FROM dbo.CollDataSumm AS cds
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = cds.CustomerId 
					AND mm.Areaid = cds.AreaId 
					AND mm.MeterId = cds.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON cds.CustomerId = me.CustomerID 
					AND cds.AreaId = me.AreaID 
					AND cds.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.EventLogs AS el
					ON cds.AreaId = el.AreaID 
					AND cds.CustomerId = el.CustomerID 
					AND el.MeterId = cds.MeterId 
					AND cds.CollDateTime = el.EventDateTime
			LEFT OUTER JOIN dbo.AssetType AS at
					ON at.MeterGroupId = me.MeterGroup 
					AND at.CustomerId = me.CustomerID
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON el.CustomerID = ec.CustomerID 
					AND el.EventSource = ec.EventSource 
					AND el.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.EventType AS et
					ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.Areas AS a 
					ON a.AreaID = mm.AreaId2 
					AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid
GO
/****** Object:  View [dbo].[EventsCollectionCBRV]    Script Date: 04/01/2014 22:07:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventsCollectionCBRV]
AS
SELECT
	cbdi.CustomerId, 
	el.EventDateTime AS DateTime, 
	cbdi.EventUID, 
	el.MeterId AS AssetId, 
	at.MeterGroupDesc AS AssetType, 
	CASE m.MeterGroup
			WHEN 0 THEN m.MeterName
			WHEN 1 THEN m.MeterName
			WHEN 10 THEN (SELECT
				SensorName
			FROM Sensors
			WHERE SensorID = mm.SensorID)
			WHEN 13 THEN (SELECT
				Description
			FROM Gateways
			WHERE GateWayID = mm.GatewayID)
		END AS AssetName,
	ec.EventCode AS AlarmCode, 
	et.EventTypeDesc AS EventType, 
	el.EventCode, 
	mm.AreaId2 AS AreaID, 
	mm.ZoneId, 
	z.ZoneName AS Zone, 
	a.AreaName AS Area, 
	m.Location AS Street, 
	cg1.DisplayName AS Suburb, 
	dz.DemandZoneDesc AS DemandArea, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
	cbdi.AmtAuto AS 'Meter Reported Amount', 
	cbdi.AmtManual AS 'Vendor Reported Amount', 
	cbdi.AmtDiff AS 'Diff Meter - Vendor', 
	cbdi.CashBoxId, 
	cbdi.DateTimeRem AS 'DateTime of Collection', 
	cbdi.DateTimeIns AS 'DateTime of Insertion', 
	cbdi.OperatorId, 
	cbdi.DateTimeRead AS ReadTime, 
	cbdi.TimeActive, 
	cbdi.CashboxSequenceNo AS SequenceNumber, 
	cbdi.FileName AS TransactionFileName, 
	cbdi.FirmwareVer AS Version 
FROM dbo.CashBoxDataImport AS cbdi
		LEFT OUTER JOIN dbo.MeterMap AS mm
			ON mm.Customerid = cbdi.CustomerId 
			AND mm.Areaid = cbdi.AreaId 
			AND mm.MeterId = cbdi.MeterId
		LEFT OUTER JOIN dbo.Meters AS m
			ON cbdi.CustomerId = m.CustomerID 
			AND cbdi.AreaId = m.AreaID 
			AND cbdi.MeterId = m.MeterId
		LEFT OUTER JOIN dbo.EventLogs AS el
			ON cbdi.AreaId = el.AreaID 
			AND cbdi.CustomerId = el.CustomerID 
			AND el.MeterId = cbdi.MeterId 
			AND el.EventDateTime = cbdi.DateTimeRead
		LEFT OUTER JOIN dbo.AssetType AS at
			ON at.MeterGroupId = m.MeterGroup 
			AND at.CustomerId = m.CustomerID
		LEFT OUTER JOIN dbo.EventCodes AS ec
			ON el.CustomerID = ec.CustomerID 
			AND el.EventSource = ec.EventSource 
			AND el.EventCode = ec.EventCode
		LEFT OUTER JOIN dbo.EventType AS et
			ON et.EventTypeId = ec.EventType
		LEFT OUTER JOIN dbo.DemandZone AS dz
			ON dz.DemandZoneId = m.DemandZone
		LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
			ON cg1.CustomGroupId = mm.CustomGroup1
		LEFT OUTER JOIN dbo.Areas AS a 
			ON a.AreaID = mm.AreaId2 
			AND a.CustomerID = mm.Customerid
		LEFT OUTER JOIN dbo.Zones AS z 
			ON z.ZoneId = mm.ZoneId 
			AND z.customerID = mm.Customerid
GO
/****** Object:  View [dbo].[EventsAllAlarmsV]    Script Date: 04/01/2014 22:07:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventsAllAlarmsV]
AS 
SELECT
				aa.CustomerID,
				aa.TimeOfOccurrance AS DateTime,
				aa.EventUID, 
				et.EventTypeDesc AS Description, 
				aa.MeterId AS AssetId,
				(SELECT
					MeterGroupDesc
				FROM dbo.AssetType AS at
				WHERE (CustomerId = aa.CustomerID) AND (MeterGroupId = me.MeterGroup))
				AS AssetType,
				CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName,  
				et.EventTypeDesc AS EventType, 
				aa.EventCode, 
				mm.AreaId2, 
				mm.ZoneId, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street, 
				cg1.DisplayName AS Suburb, 
				dz.DemandZoneDesc AS DemandArea,
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				ec.EventDescVerbose AS AlarmType, 
				ec.EventCode AS AlarmCode, 
				almt.TierDesc AS AlarmSeverity,
				mme.MaintenanceCode AS ResolutionCode,
				(SELECT
					EventSourceDesc
				FROM dbo.EventSources
				WHERE (EventSourceCode = ec.EventSource))
				AS Source,
				(SELECT
					Name
				FROM dbo.TechnicianDetails
				WHERE (TechnicianId = (SELECT
					TechnicianID
				FROM dbo.WorkOrder
				WHERE (WorkOrderId = aa.WorkOrderId))
				))
				AS Technician,
				aa.SLADue AS TimeDueSLA,
				aa.TimeOfNotification AS TimeNotified,
				aa.WorkOrderId,
				aa.TimeOfClearance AS TimeCleared,
				CAST('1' AS bit) AS IsClosed 
FROM dbo.HistoricalAlarms AS aa
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = aa.CustomerID 
					AND mm.Areaid = aa.AreaID 
					AND mm.MeterId = aa.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON aa.CustomerID = me.CustomerID 
					AND aa.AreaID = me.AreaID 
					AND aa.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON aa.CustomerID = ec.CustomerID 
					AND aa.EventSource = ec.EventSource 
					AND aa.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.EventType AS et
					ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.AlarmTier AS almt
					ON almt.Tier = ec.AlarmTier
			LEFT OUTER JOIN dbo.SFMeterMaintenanceEvent AS mme
					ON mme.WorkOrderID = aa.WorkOrderId
			LEFT OUTER JOIN dbo.Areas AS a 
					ON a.AreaID = mm.AreaId2 
					AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid

UNION ALL

SELECT
				aa.CustomerID,
				aa.TimeOfOccurrance AS DateTime,
				aa.EventUID, 
				et.EventTypeDesc AS Description, 
				aa.MeterId AS AssetId,
				at.MeterGroupDesc AS AssetType,
				CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName,  
				et.EventTypeDesc AS EventType, 
				aa.EventCode, 
				mm.AreaId2 AS AreaID, 
				mm.ZoneId, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street, 
				cg1.DisplayName AS Suburb, 
				dz.DemandZoneDesc AS DemandArea, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				ec.EventDescVerbose AS AlarmType, 
				ec.EventCode AS AlarmCode, 
				almt.TierDesc AS AlarmSeverity,
				NULL AS ResolutionCode,
				(SELECT
					EventSourceDesc
				FROM dbo.EventSources
				WHERE (EventSourceCode = ec.EventSource))
				AS Source,
				(SELECT
					Name
				FROM dbo.TechnicianDetails
				WHERE (TechnicianId = (SELECT
					TechnicianID
				FROM dbo.WorkOrder
				WHERE (WorkOrderId = aa.WorkOrderId)))) AS Technician,
				aa.SLADue AS TimeDueSLA,
				aa.TimeOfNotification AS TimeNotified,
				aa.WorkOrderId,
				NULL AS TimeCleared,
				CAST(0 AS bit) AS IsClosed 
FROM dbo.ActiveAlarms AS aa
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = aa.CustomerID 
					AND mm.Areaid = aa.AreaID 
					AND mm.MeterId = aa.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON mm.Customerid = me.CustomerID 
					AND mm.Areaid = me.AreaID 
					AND mm.MeterId = me.MeterId 
					AND aa.CustomerID = me.CustomerID 
					AND aa.AreaID = me.AreaID 
					AND aa.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON aa.CustomerID = ec.CustomerID 
					AND aa.EventSource = ec.EventSource 
					AND aa.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.AssetType AS at
					ON at.CustomerId = aa.CustomerID 
					AND at.MeterGroupId = me.MeterGroup
			LEFT OUTER JOIN dbo.Areas AS a
					ON a.AreaID = mm.Areaid 
					AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.EventType AS et
					ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.AlarmTier AS almt
					ON almt.Tier = ec.AlarmTier
GO
/****** Object:  View [dbo].[CollReconDetCBRCOMMSsubV]    Script Date: 04/01/2014 22:07:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CollReconDetCBRCOMMSsubV]
AS
SELECT		a.xFileProcessID, 
			a.CustomerId, 
			a.AreaId, 
			a.MeterId, 
			me.MeterName, 
			me.Location AS Street, 
			(SELECT AreaName
				FROM dbo.Areas
				WHERE (AreaID = mm.AreaId2) AND (CustomerID = a.CustomerID)) AS Area, 
			(SELECT ZoneName
				FROM dbo.Zones
				WHERE (ZoneId = mm.ZoneId) AND (customerID = a.Customerid)) AS Zone, 
			(SELECT DisplayName
				FROM dbo.CustomGroup1
				WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb, 
			dz.DemandZoneDesc AS DemandArea, 
			cbdiv.EventUID, 
			a.CashBoxId, 
			a.DateTimeRead AS CashBoxReadTime, 
			a.CollectionRunId, 
			a.UnscheduledFlag, 
			b.OldCashBoxID, 
			a.CashboxSequenceNo, 
			b.CashboxSequenceNo AS seqNumComms, 
			a.DateTimeIns AS InsertionTimeCBR,
			a.DateTimeRem AS RemovalTimeCBR, 
			b.CollDateTime AS CollectionDateTime, 
			a.OperatorId, 
			CAST(b.Amount AS decimal(8, 2)) AS MeterReportedAmount, 			
			CAST(a.AmtAuto AS decimal(8, 2)) AS CashboxChipAmount, 
			CAST(a.AmtManual AS decimal(8, 2)) AS VendorReportedAmount, 
			CAST(a.AmtDiff AS decimal(8, 2)) AS DiffVendorChip, 
			CAST((b.Amount - a.AmtAuto) AS decimal(8, 2)) AS DiffMeterChip, 
			CAST((b.Amount - a.AmtManual) AS decimal(8, 2)) AS DiffMeterVendor,  
			CAST(CASE
				WHEN  a.AmtDiff is not null   and
					(a.AmtDiff >= (b.Amount - a.AmtAuto) or (b.Amount - a.AmtAuto) is null) and
					(a.AmtDiff >= (b.Amount - a.AmtManual) or (b.Amount - a.AmtManual) is null)
				THEN 	a.AmtDiff
				WHEN  	(b.Amount - a.AmtAuto) is not null   and
					((b.Amount - a.AmtAuto) >= (b.Amount - a.AmtManual) or (b.Amount - a.AmtManual) is null)
				THEN 	(b.Amount - a.AmtAuto)
				WHEN  	(b.Amount - a.AmtManual) is not null   and
					((b.Amount - a.AmtManual) >= a.AmtDiff or a.AmtDiff is null) and
					((b.Amount - a.AmtManual) >= (b.Amount - a.AmtAuto) or (b.Amount - a.AmtAuto) is null)
				THEN 	(b.Amount - a.AmtManual)
				ELSE 0
			END AS decimal(8, 2)) AS MaxDiscrepancy,  
			a.PercentFull, 
			a.AvgL10, 
			a.StdDevL10,
			(a.AmtManual - a.AvgL10) AS 'Var L10', 
			a.FileName AS CollectionFilename
FROM dbo.CashBoxImportSubV AS a 
		LEFT OUTER JOIN dbo.CollDataSumm b 
			ON a.CashboxSequenceNo = b.CashboxSequenceNo 
			AND a.CustomerId = b.CustomerId 
			AND a.AreaId = b.AreaId 
			AND a.MeterId = b.MeterId 
			AND CONVERT(datetime, DATEDIFF(week, 0, a.DateTimeRem)) = CONVERT(datetime, DATEDIFF(week, 0, b.CollDateTime))
		LEFT OUTER JOIN dbo.Meters AS me
			ON a.Customerid = me.CustomerID 
			AND a.Areaid = me.AreaID 
			AND a.MeterId = me.MeterId
		LEFT OUTER JOIN dbo.MeterMap AS mm
			ON a.Customerid = mm.CustomerID 
			AND a.Areaid = mm.AreaID 
			AND a.MeterId = mm.MeterId
		LEFT OUTER JOIN dbo.DemandZone AS dz 
			ON dz.DemandZoneId = me.DemandZone 
		LEFT OUTER JOIN dbo.CashboxDataImportV AS cbdiv	
			ON a.xFileProcessID = cbdiv.xFileProcessID 
			AND a.CustomerId = cbdiv.CustomerId 
			AND a.AreaId = cbdiv.AreaId 
			AND a.MeterId = cbdiv.MeterId
			AND a.DateTimeRem = cbdiv.DateTimeRem
GO
/****** Object:  View [dbo].[AssetEventListV]    Script Date: 04/01/2014 22:07:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AssetEventListV] 
AS
SELECT 			el.CustomerID, 
			el.EventDateTime AS 'Time of Occurrence', 
			ha.TimeOfNotification AS 'Time of Report', 
			el.EventUID, 
			el.TechnicianKeyID AS 'Tech Key ID', 
			(SELECT MeterGroupDesc
                            FROM dbo.AssetType
                            WHERE (MeterGroupId = me.MeterGroup) AND (CustomerId = el.CustomerID)) AS AssetType, 
                        CASE me.MeterGroup WHEN 0 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 1 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 10 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                        (SELECT SensorState
                            FROM Sensors
                            WHERE SensorID = mm.SensorID)) WHEN 13 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                        (SELECT GatewayState
                            FROM Gateways
                            WHERE GateWayID = mm.GatewayID)) END AS 'State of Asset',   
			CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName WHEN 10 THEN
                        (SELECT SensorName
                            FROM Sensors
                            WHERE SensorID = mm.SensorID) WHEN 11 THEN
                        (SELECT CashBoxName
                            FROM  CashBox
                            WHERE CashBoxID = mm.CashBoxID) WHEN 13 THEN
                        (SELECT Description
                            FROM Gateways
                            WHERE GateWayID = mm.GatewayID) WHEN 20 THEN 'Parking Space' END AS AssetName, 
						(CASE me.MeterGroup WHEN 0 THEN me.MeterId 
							WHEN 1 THEN me.MeterId WHEN 10 THEN se.SensorID 
							WHEN 11 THEN cb.CashBoxID WHEN 13 THEN gw.GateWayID END) AS AssetId, 
						mm.AreaID2 AS AreaID, 
						mm.ZoneId AS ZoneID, 
						me.Location AS Street, 
						(SELECT EventTypeDesc
                            FROM dbo.EventType
                            WHERE (EventTypeId = ec.EventType)) AS EventClass, 
						el.EventCode,
						ec.EventDescVerbose AS 'Description of Event', 
						ec.EventDescAbbrev AS 'Event Name', 
						(SELECT es.EventSourceDesc from dbo.EventSources AS es where el.EventSource = es.EventSourceCode) AS EventSource, 
						(SELECT AreaName
						FROM dbo.Areas
						WHERE (AreaID = mm.AreaId2) AND (CustomerID = el.CustomerID)) AS Area, 
						(SELECT ZoneName
						FROM dbo.Zones
						WHERE (ZoneId = mm.ZoneId) AND (customerID = el.Customerid)) AS Zone, 
						(SELECT DisplayName
						FROM dbo.CustomGroup1
						WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
						(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5,  
						dz.DemandZoneDesc AS DemandArea
FROM         dbo.EventLogs AS el 
				LEFT OUTER JOIN dbo.HistoricalAlarms AS ha
						ON ha.Customerid = el.CustomerID 
						AND ha.Areaid = el.AreaID 
						AND ha.MeterId = el.MeterId 
						AND ha.TimeOfOccurrance = el.EventDateTime 
						AND ha.EventUID = el.EventUID 
				LEFT OUTER JOIN dbo.MeterMap AS mm 
						ON mm.Customerid = el.CustomerID 
						AND mm.Areaid = el.AreaID 
						AND mm.MeterId = el.MeterId 
				LEFT OUTER JOIN dbo.Meters AS me 
						ON mm.Customerid = me.CustomerID 
						AND mm.Areaid = me.AreaID 
						AND mm.MeterId = me.MeterId 
				LEFT OUTER JOIN dbo.EventCodes AS ec 
						ON el.CustomerID = ec.CustomerID 
						AND el.EventSource = ec.EventSource 
						AND el.EventCode = ec.EventCode 
				LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
						ON cg1.CustomGroupId = mm.CustomGroup1 
				LEFT OUTER JOIN dbo.Areas AS a 
						ON a.AreaID = mm.Areaid 
						AND a.CustomerID = mm.Customerid 
				LEFT OUTER JOIN dbo.Zones AS z 
						ON z.ZoneId = mm.ZoneId 
						AND z.customerID = mm.Customerid
				LEFT OUTER JOIN dbo.DemandZone AS dz 
						ON dz.DemandZoneId = me.DemandZone
						LEFT OUTER JOIN dbo.Sensors AS se 
						ON el.Customerid = se.CustomerID 
						AND se.SensorID = mm.SensorID
				LEFT OUTER JOIN dbo.Gateways AS gw 
						ON gw.GateWayID = mm.GatewayID
				LEFT OUTER JOIN dbo.CashBox AS cb 
						ON cb.CashBoxID = mm.CashBoxID
GO
/****** Object:  View [dbo].[ActiveAlarmsV]    Script Date: 04/01/2014 22:07:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [dbo].[ActiveAlarmsV] 
AS
SELECT     			aa.CustomerID, 
					mm.AreaId2 AS AreaID, 
					(CASE me.MeterGroup WHEN 0 THEN me.MeterId WHEN 1 THEN me.MeterId WHEN 10 THEN se.SensorID WHEN 11 THEN cb.CashBoxID WHEN 13 THEN gw.GateWayID END) AS AssetID, 
					aa.GlobalMeterID, 
					mm.ZoneId, 
					me.Location AS Street, 
					(SELECT AreaName
                        FROM dbo.Areas
                        WHERE (AreaID = mm.AreaId2) AND (CustomerID = aa.CustomerID)) AS Area, 
					(SELECT ZoneName
                            FROM dbo.Zones
                            WHERE (ZoneId = mm.ZoneId) AND (customerID = mm.Customerid)) AS Zone, 
					(SELECT DisplayName
                        FROM dbo.CustomGroup1
                        WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb, 
					aa.TimeOfOccurrance AS 'Time of Occurrence', 
					aa.TimeOfNotification AS 'Time of Report', 
					DATEADD(minute, ec.SLAMinutes, aa.TimeOfNotification) as 'Service Target Time', 
					aa.AlarmUID, 
					aa.EventCode AS AlarmCode, 
					ec.EventDescVerbose AS AlarmCodeDesc, 
                    es.EventSourceDesc AS AlarmSourceDesc, 
					es.EventSourceCode AS AlarmSourceCode, 
					(SELECT TierDesc
                        FROM dbo.AlarmTier
                        WHERE (Tier = ec.AlarmTier)) AS 'Alarm Class', 
					at.MeterGroupDesc AS AssetType, 
                    CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName WHEN 10 THEN
                        (SELECT SensorName
                            FROM Sensors
                            WHERE SensorID = mm.SensorID) WHEN 13 THEN
                        (SELECT Description
                            FROM Gateways
                            WHERE GateWayID = mm.GatewayID) END AS AssetName, 
					CASE me.MeterGroup WHEN 0 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 1 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId = me.MeterState) WHEN 10 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                                (SELECT SensorState
                                FROM Sensors
                                WHERE SensorID = mm.SensorID)) WHEN 13 THEN
                        (SELECT OperationalStatusDesc
                            FROM OperationalStatus
                            WHERE OperationalStatusId =
                                (SELECT GatewayState
                                FROM Gateways
                                WHERE GateWayID = mm.GatewayID)) END AS 'State of Asset', 
                    (SELECT AlarmStatusDesc
                        FROM dbo.AlarmStatus
                        WHERE (AlarmStatusId = 2)) AS AlarmStatus, 
                    (SELECT TechnicianID
                        FROM dbo.WorkOrder
                        WHERE (WorkOrderId = aa.WorkOrderId)) AS TechnicianId, 
					(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
					(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
					(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
					(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
					(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType4')) AS TimeClass5, 
                    CASE WHEN DATEDIFF(HH, CURRENT_TIMESTAMP, aa.SLADue) < 0 THEN
                          (SELECT TargetServiceDesignationDesc
                            FROM TargetServiceDesignationMaster
                            WHERE TargetServiceDesignationId = 2) WHEN DATEDIFF(HH, CURRENT_TIMESTAMP, aa.SLADue) BETWEEN 0 AND 1 THEN
                          (SELECT TargetServiceDesignationDesc
                            FROM TargetServiceDesignationMaster
                            WHERE TargetServiceDesignationId = 3) WHEN DATEDIFF(HH, CURRENT_TIMESTAMP, aa.SLADue) BETWEEN 1 AND 2 THEN
                          (SELECT TargetServiceDesignationDesc
                            FROM TargetServiceDesignationMaster
                            WHERE TargetServiceDesignationId = 4) ELSE
                          (SELECT TargetServiceDesignationDesc
                            FROM TargetServiceDesignationMaster
                            WHERE TargetServiceDesignationId = 1) END AS TargetService, 
							CASE WHEN DATEDIFF(minute, aa.TimeOfNotification, GETDATE()) <0 THEN 0 ELSE DATEDIFF(minute, aa.TimeOfNotification, GETDATE()) END AS 'Minute Duration of Alarm', 
							CASE WHEN DATEDIFF(minute, GETDATE(), (DATEADD(minute, ec.SLAMinutes, aa.TimeOfNotification))) <0 THEN 0 ELSE DATEDIFF(minute, GETDATE(), (DATEADD(minute, ec.SLAMinutes, aa.TimeOfNotification))) END AS 'Time Remaining to Service',
					dz.DemandZoneDesc AS DemandArea
FROM         dbo.ActiveAlarms AS aa 
				LEFT OUTER JOIN dbo.MeterMap AS mm 
						ON mm.Customerid = aa.CustomerID 
						AND mm.Areaid = aa.AreaID 
						AND mm.MeterId = aa.MeterId 
				LEFT OUTER JOIN dbo.Meters AS me 
						ON aa.CustomerID = me.CustomerID 
						AND aa.AreaID = me.AreaID 
						AND aa.MeterId = me.MeterId 
				LEFT OUTER JOIN dbo.AssetType AS at 
						ON at.MeterGroupId = me.MeterGroup 
						AND at.CustomerId = me.CustomerID 
				LEFT OUTER JOIN dbo.Areas AS a 
						ON a.AreaID = mm.AreaId2 
						AND a.CustomerID = mm.Customerid 
				LEFT OUTER JOIN dbo.Zones AS z 
						ON z.ZoneId = mm.ZoneId 
						AND z.customerID = mm.Customerid 
				LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
					  ON cg1.CustomGroupId = mm.CustomGroup1 
				LEFT OUTER JOIN dbo.EventCodes AS ec 
					  ON aa.CustomerID = ec.CustomerID 
					  AND aa.EventSource = ec.EventSource 
					  AND aa.EventCode = ec.EventCode 
				LEFT OUTER JOIN dbo.EventSources AS es 
					  ON es.EventSourceCode = aa.EventSource 
				LEFT OUTER JOIN dbo.DemandZone AS dz ON dz.DemandZoneId = me.DemandZone
				LEFT OUTER JOIN dbo.Sensors AS se 
						ON mm.Customerid = se.CustomerID 
						AND se.SensorID = mm.SensorID
				LEFT OUTER JOIN dbo.Gateways AS gw 
						ON gw.GateWayID = mm.GatewayID
				LEFT OUTER JOIN dbo.CashBox AS cb 
						ON cb.CashBoxID = mm.CashBoxID
GO
/****** Object:  View [dbo].[A_LastCollectionAndGSMConnectionV]    Script Date: 04/01/2014 22:07:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create View [dbo].[A_LastCollectionAndGSMConnectionV] AS
SELECT 
				me.CustomerID, 
				me.AreaID, 
				me.MeterID, 
				me.MeterID AS AssetId, 
				me.MeterName AS AssetName, 
				(SELECT MeterGroupDesc FROM dbo.AssetType WHERE (MeterGroupId = me.MeterGroup) AND (CustomerId = me.CustomerID)) AS AssetType,
				me.Location AS Street, 
				a.AreaName AS Area,
				z.ZoneName AS Zone, 
				(SELECT DisplayName
					FROM dbo.CustomGroup1
					WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb,  
				dz.DemandZoneDesc AS DemandArea,    
				MAX(cds.CollDateTime) AS LastTimeOfCollection, 
				NULL AS LastCommunicationTime
FROM 	dbo.Meters AS me 
			RIGHT OUTER JOIN dbo.CollDataSumm AS cds 
				ON cds.CustomerId = me.CustomerID 
				AND cds.MeterId = me.MeterID 
				AND cds.AreaId = me.AreaID
			LEFT OUTER JOIN dbo.MeterMap AS mm 
				ON me.CustomerId = mm.CustomerID 
				AND me.MeterId = mm.MeterID 
				AND me.AreaId = mm.AreaID
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
				ON cg1.CustomGroupId = mm.CustomGroup1 
			LEFT OUTER JOIN dbo.Areas AS a 
				ON a.AreaID = mm.AreaId2 
				AND a.CustomerID = me.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
				ON z.ZoneId = mm.ZoneId 
				AND z.customerID = me.Customerid 
			LEFT OUTER JOIN dbo.DemandZone AS dz
				ON dz.DemandZoneId = me.DemandZone
			GROUP BY me.CustomerID, me.AreaID, me.MeterID, mm.ZoneId, me.Location, me.MeterGroup, me.MeterName, cg1.DisplayName, dz.DemandZoneDesc, z.ZoneName, a.AreaName, mm.CustomGroup1
			HAVING MAX(cds.CollDateTime) <= GETDATE()

UNION

SELECT			gcl.CustomerID, 
				gcl.AreaID, 
				gcl.MeterID, 
				gcl.MeterID AS AssetId, 
				me.MeterName AS AssetName, 
				(SELECT MeterGroupDesc FROM dbo.AssetType WHERE (MeterGroupId = me.MeterGroup) AND (CustomerId = gcl.CustomerID)) AS AssetType,
				me.Location AS Street, 
				a.AreaName AS Area,
				z.ZoneName AS Zone, 
				(SELECT DisplayName
					FROM dbo.CustomGroup1
					WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb,  
				dz.DemandZoneDesc AS DemandArea,  
				NULL AS LastTimeOfCollection, 
				MAX(StartDateTime) AS LastCommunicationTime
FROM 	dbo.GSMConnectionLogs AS gcl
			LEFT OUTER JOIN dbo.Meters AS me 
				ON me.CustomerId = gcl.CustomerID 
				AND me.MeterId = gcl.MeterID 
				AND me.AreaId = gcl.AreaID
			LEFT OUTER JOIN dbo.MeterMap AS mm 
				ON gcl.CustomerId = mm.CustomerID 
				AND gcl.MeterId = mm.MeterID 
				AND gcl.AreaId = mm.AreaID
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
				ON cg1.CustomGroupId = mm.CustomGroup1 
			LEFT OUTER JOIN dbo.Areas AS a 
				ON a.AreaID = mm.AreaId2 
				AND a.CustomerID = gcl.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
				ON z.ZoneId = mm.ZoneId 
				AND z.customerID = gcl.Customerid
			LEFT OUTER JOIN dbo.DemandZone AS dz
				ON dz.DemandZoneId = me.DemandZone
			GROUP BY gcl.MeterID, gcl.AreaID, gcl.CustomerID, a.AreaName, z.ZoneName, cg1.DisplayName, dz.DemandZoneDesc, me.Location, mm.CustomGroup1, me.MeterName, me.MeterGroup
GO
/****** Object:  View [dbo].[A_EventsAndAlarmsV]    Script Date: 04/01/2014 22:07:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[A_EventsAndAlarmsV]
AS 
SELECT
				aa.CustomerID,
				aa.AreaID, 
				aa.MeterID,
				aa.TimeOfOccurrance AS DateTime,
				aa.EventUID,
				aa.MeterId AS AssetId,
				(SELECT
					MeterGroupDesc
				FROM dbo.AssetType AS at
				WHERE (CustomerId = aa.CustomerID) AND (MeterGroupId = me.MeterGroup))
				AS AssetType,
				CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName,  
				et.EventTypeDesc AS EventType, 
				et.EventTypeDesc AS EventClass,
				aa.EventCode, 
				ec.EventDescVerbose, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street, 
				cg1.DisplayName AS Suburb, 
				dz.DemandZoneDesc AS DemandArea,
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				ec.EventDescVerbose AS AlarmType, 
				ec.EventCode AS AlarmCode, 
				almt.TierDesc AS AlarmSeverity,
				mme.MaintenanceCode AS ResolutionCode,
				(SELECT
					EventSourceDesc
				FROM dbo.EventSources
				WHERE (EventSourceCode = ec.EventSource))
				AS Source,
				aa.TimeOfNotification AS TimeNotified, 
				NULL AS Amount,
				NULL AS Bay,
				'' AS CardStatus,
				'' AS PaymentType,
				NULL AS ReceiptNumber,
				NULL AS TimePaid,
				NULL AS TransactionTime,
				NULL AS TransactionId
FROM dbo.HistoricalAlarms AS aa
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = aa.CustomerID 
					AND mm.Areaid = aa.AreaID 
					AND mm.MeterId = aa.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON aa.CustomerID = me.CustomerID 
					AND aa.AreaID = me.AreaID 
					AND aa.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON aa.CustomerID = ec.CustomerID 
					AND aa.EventSource = ec.EventSource 
					AND aa.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.EventType AS et
					ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.AlarmTier AS almt
					ON almt.Tier = ec.AlarmTier
			LEFT OUTER JOIN dbo.SFMeterMaintenanceEvent AS mme
					ON mme.WorkOrderID = aa.WorkOrderId
			LEFT OUTER JOIN dbo.Areas AS a 
					ON a.AreaID = mm.AreaId2 
					AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid

UNION

SELECT
				aa.CustomerID,
				aa.AreaID, 
				aa.MeterID,
				aa.TimeOfOccurrance AS DateTime,
				aa.EventUID,
				aa.MeterId AS AssetId,
				at.MeterGroupDesc AS AssetType,
				CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName,  
				et.EventTypeDesc AS EventType, 
				et.EventTypeDesc AS EventClass,
				aa.EventCode, 
				ec.EventDescVerbose, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street, 
				cg1.DisplayName AS Suburb, 
				dz.DemandZoneDesc AS DemandArea, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = aa.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				ec.EventDescVerbose AS AlarmType, 
				ec.EventCode AS AlarmCode, 
				almt.TierDesc AS AlarmSeverity,
				NULL AS ResolutionCode,
				(SELECT
					EventSourceDesc
				FROM dbo.EventSources
				WHERE (EventSourceCode = ec.EventSource))
				AS Source, 
				aa.TimeOfNotification AS TimeNotified, 
				NULL AS Amount,
				NULL AS Bay,
				'' AS CardStatus,
				'' AS PaymentType,
				NULL AS ReceiptNumber,
				NULL AS TimePaid,
				NULL AS TransactionTime,
				NULL AS TransactionId
FROM dbo.ActiveAlarms AS aa
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = aa.CustomerID 
					AND mm.Areaid = aa.AreaID 
					AND mm.MeterId = aa.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON mm.Customerid = me.CustomerID 
					AND mm.Areaid = me.AreaID 
					AND mm.MeterId = me.MeterId 
					AND aa.CustomerID = me.CustomerID 
					AND aa.AreaID = me.AreaID 
					AND aa.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON aa.CustomerID = ec.CustomerID 
					AND aa.EventSource = ec.EventSource 
					AND aa.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.AssetType AS at
					ON at.CustomerId = aa.CustomerID 
					AND at.MeterGroupId = me.MeterGroup
			LEFT OUTER JOIN dbo.Areas AS a
					ON a.AreaID = mm.Areaid 
					AND a.CustomerID = mm.Customerid 
			LEFT OUTER JOIN dbo.Zones AS z
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.EventType AS et
					ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.AlarmTier AS almt
					ON almt.Tier = ec.AlarmTier
GO
/****** Object:  Table [dbo].[CollDataMeterStatus]    Script Date: 04/01/2014 22:07:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[CollDataMeterStatus](
	[GlobalMeterID] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[StatusDateTime] [datetime] NOT NULL,
	[AlarmStatus1] [int] NULL,
	[AlarmStatus2] [int] NULL,
	[AlarmStatus3] [int] NULL,
	[AlarmStatus4] [int] NULL,
	[MinVoltDryBattery] [float] NULL,
	[CurrVoltDryBattery] [float] NULL,
	[MinVoltSolarbattery] [float] NULL,
	[CurrVoltSolarBattery] [float] NULL,
	[MinVoltSolarPanel] [float] NULL,
	[CurrVoltSolarPanel] [float] NULL,
	[MinAmp] [float] NULL,
	[CurrAmp] [float] NULL,
	[MinTemp] [float] NULL,
	[MaxTemp] [float] NULL,
	[CurrTemp] [float] NULL,
	[FirmwareVer] [int] NULL,
	[FirwareRev] [int] NULL,
	[Status1] [int] NULL,
	[Status2] [int] NULL,
	[Status3] [int] NULL,
	[Status4] [int] NULL,
	[NumOfCoins] [int] NULL,
	[NumOfCoinsRej] [int] NULL,
	[NetworkStrength] [int] NULL,
	[H8_CRC] [varchar](4) NULL,
	[RSF_CRC] [varchar](4) NULL,
	[MSF_CRC] [varchar](4) NULL,
	[MSF_Filename] [varchar](22) NULL,
	[RSF_Filename] [varchar](22) NULL,
	[VER_BOOTLDR] [varchar](20) NULL,
	[VER_EEWR] [varchar](20) NULL,
	[VER_ASW] [varchar](20) NULL,
	[VER_MSW] [varchar](20) NULL,
	[VER_MDMTYPE] [varchar](40) NULL,
	[VER_MDMSWID] [varchar](100) NULL,
	[VER_GCFID] [varchar](40) NULL,
 CONSTRAINT [PK_CollDataMeterStatus] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[StatusDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [CollDataMeterStatus_IDX_CSCAM] ON [dbo].[CollDataMeterStatus] 
(
	[CurrVoltSolarBattery] ASC,
	[StatusDateTime] ASC,
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [CollDataMeterStatus_IDXGlobalMeterID] ON [dbo].[CollDataMeterStatus] 
(
	[GlobalMeterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[ACS_Rate_Transmission_Status]    Script Date: 04/01/2014 22:07:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[ACS_Rate_Transmission_Status] as 
	select ROW_NUMBER() OVER(ORDER BY t.TransmissionID) AS ID,t.TransmissionID,m.DuncanCustomerId,m.DuncanAreaId,m.DuncanMeterId,m.DuncanBayId,td.SpaceNum,td.RateInCent,t.ReceivedTS,t.ImplementationDate,fj.SubmittedDate as 	JobScheudled,tj.ActivatedTS as RateActivated,tj.AckTS as Acknowledged
	from RateTransmission t 
	join RateTransmissionDetails td
	on t.TransmissionID = td.TransmissionID
	left outer join RateTransmissionJob tj 
	on td.ID = tj.TransmissionDetailsID
	join  MeterMapping m
	on td.SpaceNum = m.SpaceId
	left outer join  FDJobs fj
	on tj.JobId = fj.JobId
GO
/****** Object:  View [dbo].[ACS_Rate_File_Version_Status]    Script Date: 04/01/2014 22:07:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[ACS_Rate_File_Version_Status] as 
		select ROW_NUMBER() OVER(ORDER BY submitteddate desc) as ID,FJ.CustomerID,FJ.AreaID,FJ.MeterID,FI.FileID
		,FI.OriginalFileID,fj.jobid,FI.filetype,submitteddate as submitted,FH.Historyts as Activated
		From FDJobs FJ  join FDFiles FI
		on FJ.FileID = FI.FileID
		left outer join FDJobHistory FH
		on FJ.JobID = FH.JobID
		and FH.FDJobstatus = 30
GO
/****** Object:  View [dbo].[ACS_Rate_AllFailed]    Script Date: 04/01/2014 22:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[ACS_Rate_AllFailed] as 
	select rt.*
	from RateTransmission rt
	where 
	slaAckTs is null and 
	NOT EXISTS
	(
	select 1 from RateTransmissionJob rtj
	where rtj.TransmissionID = rt.TransmissionID
	)
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdate_ParkingSpaces]    Script Date: 04/01/2014 22:07:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdate_ParkingSpaces] 	
	@CustomerId int	,
	@AreaId int,
	@MeterId int,
	@assetId bigint,
	@Latitude float,
	@Longitude float,
	@DateInstalled DateTime,
	@DemandStatus int,
	@DisplaySpaceNum Varchar(255),
	@AssetPendingReasonId int,
	@CreateUserId int	
AS
DECLARE
	@ServerId int
	,@BayNum int
BEGIN
	if exists (select * from ParkingSpaces where [ParkingSpaceId] = @assetId
					and [CustomerID] = @customerId)
	begin
		UPDATE [ParkingSpaces]
			SET [Latitude] = Case when @Latitude is null then [Latitude] else @Latitude end
			,[Longitude] = Case when @Longitude is null then [Longitude] else @Longitude end
			,[InstallDate] = Case when @DateInstalled is null then [InstallDate] else @DateInstalled end
			,[DemandZoneId] = Case when @DemandStatus is null then [DemandZoneId] else @DemandStatus end
			,[DisplaySpaceNum] = Case when @DisplaySpaceNum is null then [DisplaySpaceNum] else @DisplaySpaceNum end					
			--,[ParkingSpaceType] = <ParkingSpaceType, int,>
			--,[OperationalStatus] = <OperationalStatus, int,>
			--,[OperationalStatusTime] = <OperationalStatusTime, datetime,>
			--,[OperationalStatusEndTime] = <OperationalStatusEndTime, datetime,>
			--,[OperationalStatusComment] = <OperationalStatusComment, varchar(2000),>
		WHERE [ParkingSpaceId] = @assetId
			and [CustomerID] = @customerId
	end else begin
	
		select @ServerId = InstanceId from ServerInstance
		set @BayNum = RIGHT(@assetId,3)
		
		INSERT INTO [ParkingSpaces]
           ([ParkingSpaceId]
           ,[ServerID]
           ,[CustomerID]
           ,[AreaId]
           ,[MeterId]
           ,[BayNumber]
           ,[AddedDateTime]
           ,[Latitude]
           ,[Longitude]
           ,[HasSensor]
           ,[SpaceStatus]
           ,[DateActivated]
           ,[Comments]
           ,[DisplaySpaceNum]
           ,[DemandZoneId]
           ,[InstallDate]
           ,[ParkingSpaceType]
           ,[OperationalStatus]
           ,[OperationalStatusTime]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment])
     VALUES
		(@assetId
           ,@ServerId
           ,@CustomerId
           ,@AreaId
           ,@MeterId
           ,@BayNum
           ,GETDATE()--[AddedDateTime]
           ,@Latitude
           ,@Longitude
           ,0
           ,0
           ,GETDATE()--[DateActivated]
           ,null--[Comments]
           ,@DisplaySpaceNum
           ,@DemandStatus
           ,@DateInstalled
           ,null--[ParkingSpaceType]
           ,null--[OperationalStatus]
           ,null--[OperationalStatusTime]
           ,null--[OperationalStatusEndTime]
           ,null--[OperationalStatusComment]
        )
	end 	
	
	exec sp_Audit_ParkingSpaces @Customerid,@AssetId,@AssetPendingReasonId,@CreateUserId				
END
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_getDownTime]    Script Date: 04/01/2014 22:07:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_getDownTime] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int,
	@TimeOfOccurance datetime,
	@TimeOfClearance datetime,
	@totalMinute int OUTPUT
AS

Declare
	@TempStart datetime
	,@TempEnd datetime	
	,@day datetime
	,@regulatedStartMinuteOfDay int
	,@regulatedEndMinuteOfDay int
	,@regulatedStartTs datetime
	,@regulatedEndTs datetime
BEGIN
	print '------------ ================================================ sp_SLA_getDownTime:begin  =================================================== -----------'
	
	set @TempStart  = @TimeOfOccurance
	set @TempEnd = @TimeOfClearance
	
	set @day = DATEADD(dd, DATEDIFF(dd, 0, @TempStart), 0) -- strip hour minute
	set @totalMinute  = 0
	
	exec sp_SLA_getROMScheudle @customerid,@areaid,@meterid,@day,'Regulated'
		,@regulatedStartMinuteOfDay output,@regulatedEndMinuteOfDay output
		,@regulatedStartTs output,@regulatedEndTs
	
	if (@TimeofOccurance < @regulatedStartTs) begin
		Print 'Occured earlier than Regulated start - using @regulatedStartTs'
		set @TempStart = @regulatedStartTs
	end else begin
		Print 'using @TimeofOccurance'
		set @TempStart = @TimeofOccurance
	end
	
	
	if (@TimeOfClearance > @regulatedEndTs) begin
		Print 'using regulatedEndTs'
		set @TempEnd =@regulatedEndTs
	end else begin
		Print 'using @TimeOfClearance'
		set @TempEnd =@TimeOfClearance
	end 
	
	set @totalMinute  = DATEDIFF(minute,@TempStart,@TempEnd)
	
	print '------------ ================================================ sp_SLA_getDownTime:end  =================================================== -----------'
	print ' '
	
END
GO
/****** Object:  StoredProcedure [dbo].[sp_PopulateWhiteListFileComplete]    Script Date: 04/01/2014 22:07:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_PopulateWhiteListFileComplete]
as
Begin
	Print 'Deleting white list'
	Delete from WhilteListFile;
	
	-- Every week, full content is to be created. Not just delta.
	if (DATEPART(dw,GETDATE())=7)
	begin
		Print 'Generating full content'
		
		Delete from WhilteListFileStaging;
	
		Insert into WhilteListFile
			([Delta],[CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents])
			select distinct 'C',[CustomerId],[AreaId],[MeterId],'0',0,0,0,0  from WhiteList_All
		
	end
	
	Insert into WhilteListFile
	([Delta],[CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents])
	select 'D' as Delta ,x.* from 
	(
		select [CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents] from WhilteListFileStaging
		EXCEPT 
		select [CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents] from WhiteList_All
	)x
	
	Insert into WhilteListFile
	([Delta],[CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents])
	select 'I' as Delta ,x.* from 
	(
		select [CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents] from WhiteList_All
		EXCEPT 
		select [CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents] from WhilteListFileStaging
	)x
	
	
	--Reverse execution to update Staging
	Delete WhilteListFileStaging where exists 
	(
		select 1 
		from WhilteListFile c
		where c.[CustomerId]=WhilteListFileStaging.[CustomerId]
		and c.[AreaId]= WhilteListFileStaging.[AreaId]
		and c.[MeterId] = WhilteListFileStaging.[MeterId]
		and c.[CardHash] = WhilteListFileStaging.[CardHash]
		and c.[DiscountSchemeId] = WhilteListFileStaging.[DiscountSchemeId]
		and c.[DiscountPercentage] = WhilteListFileStaging.[DiscountPercentage]
		and c.[DiscountMinute] =WhilteListFileStaging.[DiscountMinute]
		and c.[MaxAmountInCents] = WhilteListFileStaging.[MaxAmountInCents]
		and c.Delta = 'D'
	)
	
	Insert into	 WhilteListFileStaging 
		([CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents],[ValidDate])
		select [CustomerId],[AreaId],[MeterId],[CardHash],[DiscountSchemeId],[DiscountPercentage],[DiscountMinute],[MaxAmountInCents],GETDATE() from WhilteListFile
		where Delta = 'I'
		
	--Hibernate required a resultset returned
	select GETDATE() curr
			
End
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_Sensors]    Script Date: 04/01/2014 22:07:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_Sensors] 	
	@CustomerId int,
	@SensorId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '----------------------------------------------'
	Print 'AUDIT SENSOR'
	
	
	INSERT INTO [SensorsAudit]
           ([UserId]
           ,[UpdateDateTime]
           ,[CustomerID]
           ,[SensorID]
           ,[BarCodeText]
           ,[Description]
           ,[GSMNumber]
           ,[GlobalMeterID]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[SensorName]
           ,[SensorState]
           ,[SensorType]
           ,[InstallDateTime]
           ,[DemandZone]
           ,[Comments]
           ,[RoadWayType]
           ,[ParkingSpaceId]
           ,[SensorModel]
           ,[OperationalStatus]
           ,[WarrantyExpiration]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,[AssetPendingReasonId])
	select @CreateUserId
           ,GETDATE()
           ,[CustomerID]
           ,[SensorID]
           ,[BarCodeText]
           ,[Description]
           ,[GSMNumber]
           ,[GlobalMeterID]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[SensorName]
           ,[SensorState]
           ,[SensorType]
           ,[InstallDateTime]
           ,[DemandZone]
           ,[Comments]
           ,[RoadWayType]
           ,[ParkingSpaceId]
           ,[SensorModel]
           ,[OperationalStatus]
           ,[WarrantyExpiration]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment]
           ,@AssetPendingReasonId
          from Sensors           
          where CustomerID = @CustomerId
          and SensorID = @SensorId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_SpaceExpiryConfirmation]    Script Date: 04/01/2014 22:07:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_SpaceExpiryConfirmation]
	@cid int
	,@aid int
	,@mid int
	,@bay int
	,@transTs int
	,@occStatus int
	,@status varchar(200) OUTPUT
	
AS
DECLARE 
	@transtime datetime
	,@psId bigint
	
BEGIN
	Select @psId = ParkingSpaceID from ParkingSpaces
	where customerid = @cid and areaid =@aid and meterid = @mid and bayNumber = @bay

	if @psId is not null begin
		
		set @transtime = DATEADD(SECOND,@transTs,'2000/01/01')
		
		Insert into ParkingSpaceExpiryConfirmationEvent
		(ParkingSpaceId,SpaceStatus,EventDateTime)
		values (@psId,@occStatus,@transtime)
		
		set @status = 'Inserted'
	end
	else begin 
		set @status = 'ParkingSpace not found'
	end	
END
GO
/****** Object:  Table [dbo].[TechCreditEvent]    Script Date: 04/01/2014 22:07:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TechCreditEvent](
	[EventId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[BayStart] [int] NOT NULL,
	[BayEnd] [int] NOT NULL,
	[Note] [varchar](255) NULL,
	[TechnicianKeyID] [int] NULL,
	[EventUID] [bigint] NULL,
 CONSTRAINT [PK_TechCreditEvent] PRIMARY KEY CLUSTERED 
(
	[EventId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY],
 CONSTRAINT [TechCreditEventUnique] UNIQUE NONCLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[EventDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VersionProfileMeter]    Script Date: 04/01/2014 22:07:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VersionProfileMeter](
	[VersionProfileMeterId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ConfigurationName] [varchar](255) NOT NULL,
	[HardwareVersion] [varchar](255) NULL,
	[SoftwareVersion] [varchar](255) NULL,
	[CommunicationVersion] [varchar](255) NULL,
	[Version1] [varchar](255) NULL,
	[Version2] [varchar](255) NULL,
	[Version3] [varchar](255) NULL,
	[Version4] [varchar](255) NULL,
	[Version5] [varchar](255) NULL,
	[Version6] [varchar](255) NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NULL,
	[MeterId] [int] NULL,
	[MeterGroup] [int] NOT NULL,
	[SensorID] [int] NULL,
	[GatewayID] [int] NULL,
 CONSTRAINT [PK_VersionProfileMeter] PRIMARY KEY CLUSTERED 
(
	[VersionProfileMeterId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VersionProfileMeterAudit]    Script Date: 04/01/2014 22:07:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VersionProfileMeterAudit](
	[VersionProfileMeterAuditId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[UserId] [int] NOT NULL,
	[UpdateDateTime] [datetime] NOT NULL,
	[VersionProfileMeterId] [bigint] NOT NULL,
	[ConfigurationName] [varchar](255) NOT NULL,
	[HardwareVersion] [varchar](255) NULL,
	[SoftwareVersion] [varchar](255) NULL,
	[CommunicationVersion] [varchar](255) NULL,
	[Version1] [varchar](255) NULL,
	[Version2] [varchar](255) NULL,
	[Version3] [varchar](255) NULL,
	[Version4] [varchar](255) NULL,
	[Version5] [varchar](255) NULL,
	[Version6] [varchar](255) NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NULL,
	[MeterId] [int] NULL,
	[MeterGroup] [int] NULL,
	[SensorID] [int] NULL,
	[GatewayID] [int] NULL,
 CONSTRAINT [PK_VersionProfileMeterAudit] PRIMARY KEY CLUSTERED 
(
	[VersionProfileMeterAuditId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  View [dbo].[TotalIncomeSummaryV]    Script Date: 04/01/2014 22:07:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TotalIncomeSummaryV] 
AS 
SELECT
t.CustomerID, 
t.TransDateTime AS TransactionDateTime, 
m.MeterName AS AssetName,
t.MeterID AS AssetID,
at.MeterGroupDesc AS AssetType,
mm.AreaId2 AS AreaID,
a.AreaName AS Area,
t.ZoneID,
m.Location AS Street,
z.ZoneName AS Zone,
dz.DemandZoneDesc AS DemandArea,
cg1.DisplayName AS Suburb,
(SELECT SchemeName FROM dbo.DiscountScheme WHERE (SchemeType = t.DiscountSchemeId) AND CustomerID = t.CustomerID) AS DiscountScheme, 
NULL AS DiagDateTime,
t.AmountInCents AS AmountPaid,
(SELECT TransactionTypeDesc FROM dbo.TransactionType WHERE (TransactionTypeId = t.TransactionType)) AS TransactionType,
CASE tt.TransactionTypeId WHEN 1 THEN ts.Description ELSE NULL END AS TransactionStatus,
0 AS CoinRejectCount,
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeType1, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeType2, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeType3, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeType4, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeType5
FROM dbo.Transactions AS t
LEFT OUTER JOIN dbo.Meters AS m
ON t.CustomerID = m.CustomerID AND t.MeterID = m.MeterId AND t.AreaID = m.AreaID
LEFT OUTER JOIN dbo.MeterMap AS mm
ON t.CustomerID = mm.CustomerID AND t.MeterID = mm.MeterId AND t.AreaID = mm.Areaid
LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
ON cg1.CustomGroupId = mm.CustomGroup1
LEFT OUTER JOIN dbo.DemandZone AS dz
ON dz.DemandZoneId = m.DemandZone
LEFT OUTER JOIN dbo.Areas AS a
ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid
LEFT OUTER JOIN dbo.Zones AS z
ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
LEFT OUTER JOIN dbo.TransactionType AS tt
ON t.TransactionType = tt.TransactionTypeId
LEFT OUTER JOIN dbo.TransactionStatus AS ts
ON t.TransactionStatus = ts.StatusID
LEFT OUTER JOIN dbo.AssetType AS at 
ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID
WHERE tt.TransactionTypeDesc IN ('Credit Card', 'Smart Card', 'Cash', 'Pay By Phone')

UNION

SELECT
md.CustomerID, 
NULL  AS TransactionDateTime,
m.MeterName AS AssetName,
md.MeterID AS AssetID,
at.MeterGroupDesc AS AssetType,
mm.AreaId2 AS AreaID,
'' AS Area,
mm.ZoneId,
m.Location AS Street,
'' AS Zone,
'' AS DemandArea,
'' AS Suburb,
'' AS DiscountScheme,
(CASE md.DiagnosticType WHEN 222 THEN md.DiagTime ELSE NULL END) AS DiagDateTime,
0 AS AmountPaid,
'' AS TransactionType,
NULL AS TransactionStatus,
CAST(CASE md.DiagnosticType WHEN 222 THEN md.DiagnosticValue ELSE NULL END AS int) AS CoinRejectCount,
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeType1, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeType2, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeType3, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeType4, 
(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeType5
FROM dbo.[MeterDiagnostic] AS md
LEFT OUTER JOIN dbo.MeterMap AS mm
ON md.CustomerID = mm.CustomerID AND md.MeterID = mm.MeterId AND md.AreaID = mm.Areaid
LEFT OUTER JOIN dbo.Meters AS m
ON md.CustomerID = m.CustomerID AND md.MeterID = m.MeterId AND md.AreaID = m.AreaID
LEFT OUTER JOIN dbo.AssetType AS at 
ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = md.CustomerID
where md.DiagnosticType = 222
GO
/****** Object:  View [dbo].[SpaceMeterAssetV]    Script Date: 04/01/2014 22:07:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SpaceMeterAssetV] 
AS
SELECT 	ps.ParkingSpaceId, 
		me.GlobalMeterId, 
		ps.CustomerID, 
		ps.AreaId, 
		me.MeterId, 
		mm.AreaId2, 
		mm.ZoneId, 
		(SELECT AreaName
            FROM dbo.Areas
            WHERE (AreaID = mm.AreaId2) AND (CustomerID = ps.CustomerID)) AS Area, 
        (SELECT ZoneName
            FROM dbo.Zones
            WHERE (ZoneId = mm.ZoneId) AND (customerID = ps.CustomerID)) AS Zone,
		me.Location AS Street, 
		(SELECT DisplayName FROM dbo.CustomGroup1 WHERE (CustomGroupId = mm.CustomGroup1) AND (CustomerId = ps.CustomerID)) AS Suburb, 
		CASE me.MeterGroup WHEN 0 THEN me.MeterName WHEN 1 THEN me.MeterName END AS MeterName, 
		s.SensorName,  
		CASE me.MeterGroup 	
			WHEN 11 THEN
                (SELECT CashBoxName
                    FROM  CashBox
                    WHERE CashBoxID = mm.CashBoxID) END AS CashBoxName, 
		gw.Description AS GatewayName, 
		ps.BayNumber, 
		(SELECT DemandZoneDesc FROM dbo.DemandZone WHERE DemandZoneId = me.DemandZone) AS DemandArea
FROM dbo.ParkingSpaces AS ps 
	INNER JOIN dbo.Meters AS me 
		ON me.CustomerID = ps.CustomerID 
		AND me.AreaID = ps.AreaId 
		AND me.MeterId = ps.MeterId 
	LEFT OUTER JOIN dbo.MeterMap AS mm 
		ON me.CustomerId = mm.CustomerID 
		AND me.MeterId = mm.MeterID 
		AND me.AreaId = mm.AreaID 
	LEFT OUTER JOIN dbo.SensorMapping AS sm
		ON ps.CustomerID = sm.CustomerID
		AND ps.ParkingSpaceID = sm.ParkingSpaceID
	LEFT OUTER JOIN dbo.Sensors AS s
		ON ps.CustomerID = s.CustomerID
		AND ps.ParkingSpaceID = s.ParkingSpaceID
	LEFT OUTER JOIN dbo.Gateways AS gw
		ON ps.CustomerID = gw.CustomerID
		AND sm.GateWayID = gw.GateWayID
GO
/****** Object:  StoredProcedure [dbo].[sp_SLA_AssetDownTime]    Script Date: 04/01/2014 22:07:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_SLA_AssetDownTime] 
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@EventCode int,
	@EventSource int,
	@TimeOfOccurance datetime,
	@TimeOfNotification datetime,
	@SLATarget datetime
AS
Declare 
	@MeterGroup int	
BEGIN
	Declare 
		curalarm cursor for 
			select customerid,areaid,meterid,EventCode,eventsource,TimeOfOccurrance 
			from HistoricalAlarms
			where DownTimeMinute is null
			order by TimeOfOccurrance
			
	OPEN curalarm 
		fetch next from curalarm into @customerid,@areaid,@meterid,@EventCode,@eventsource,@TimeOfOccurance
		WHILE @@FETCH_STATUS = 0
		BEGIN
			exec sp_SLA_getSLATargetTime @customerid,@areaid, @meterid,@EventCode,@eventsource,@TimeOfOccurance,@SLATarget output
			if (@SLATarget is null) begin
				Print '@SLATarget is null'
			end else begin
				Print 'Updating @SLATarget=  '+ convert(varchar,@slatarget)
				Update ActiveAlarms
				set SLADue = @SLATarget
				where CustomerID = @customerid and AreaId= @areaid and MeterId = @meterid
				and eventcode = @eventcode and eventsource= @eventsource and timeofoccurrance = @TimeOfOccurance			
			end
			fetch next from curalarm into @customerid,@areaid,@meterid,@EventCode,@eventsource,@TimeOfOccurance
		END 
	CLOSE curalarm
	DEALLOCATE curalarm	
	print 'test'
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_MeterMap]    Script Date: 04/01/2014 22:07:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_MeterMap] 	
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '-------------------------------------------------------------'
	Print 'AUDIT METERMAP'
	Print 'C/A/M = ' + convert(varchar,@Customerid) + '/' + convert(varchar,@Areaid) + '/' +  convert(varchar,@MeterId) 
	
	INSERT INTO [MeterMapAudit]
           ([Customerid]
           ,[Areaid]
           ,[MeterId]
           ,[ZoneId]
           ,[HousingId]
           ,[MechId]
           ,[AreaId2]
           ,[CollRouteId]
           ,[EnfRouteId]
           ,[MaintRouteId]
           ,[CustomGroup1]
           ,[CustomGroup2]
           ,[CustomGroup3]
           ,[AuditDateTime]
           ,[UserId]
           ,[AssetPendingReasonId])     
        select [Customerid]
           ,[Areaid]
           ,[MeterId]
           ,[ZoneId]
           ,[HousingId]
           ,[MechId]
           ,[AreaId2]
           ,[CollRouteId]
           ,[EnfRouteId]
           ,[MaintRouteId]
           ,[CustomGroup1]
           ,[CustomGroup2]           
           ,[CustomGroup3] 
           ,GETDATE()
           ,@CreateUserId
           ,@AssetPendingReasonId
           from MeterMap
        Where Customerid = @CustomerId
        and AreaId  = @AreaId
        and MeterId  = @MeterId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_MeterMap_UpdateCollectionRunId]    Script Date: 04/01/2014 22:07:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_MeterMap_UpdateCollectionRunId]
as
Begin
	Print 'Updating MeterMap'
	--Update MeterMap set CollectionRunId = null where CollectionRunId is not null

	Update m
	set m.CollectionRunId = c.CollectionRunId
	from MeterMap m,
	(select CustomerId,AreaId,MeterId,MAX(CollectionRunId) CollectionRunId from v_ActiveCollectionRunMeter
		where (
			(GETDATE() between ActivationDate and ExipryDate) 
			or 
			(GETDATE()> ActivationDate and ExipryDate is null)
		)
		Group by CustomerId,AreaId,meterid 
	) c
	where m.Customerid = c.CustomerId
	and m.Areaid = c.AreaId
	and m.MeterId = c.MeterId
	
	Print 'Updated MeterMap ' + Convert(Varchar,@@ROWCOUNT)
end
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdate_Sensors]    Script Date: 04/01/2014 22:07:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdate_Sensors] 	
	@CustomerId int,
	@AreaId int,
	@MeterId Int,
	@AssetId int,
	@AssetPendingReasonId int,
	@CreateUserId int,
	@Latitude float,
	@Longitude float,
	@LocationSensor Varchar(255),
	@ParkingSpaceId bigint,
	@PrimaryGateway bigint,
	@SecondaryGateway bigint,
	@AssetType int,
	@AssetName Varchar(500),
	@AssetModel int,
	@AssetState int,
	@OperationalStatus int,	
	@OperationalStatusTime DateTime,
	@WarrantyExpiration DateTime,
	@LastPreventativeMaintenance DateTime,
	@NextPreventativeMaintenance DateTime,
	@DemandStatus int,
	@DateInstalled Datetime
AS
BEGIN
	if exists (select * from Sensors where SensorId = @AssetId and CustomerId = @CustomerId) 
	begin
		Print 'Updating Sensor'
		UPDATE [Sensors]
		SET [Latitude] = Case when @Latitude is null then [Latitude] else @Latitude end
		  ,[Longitude] = Case when @Longitude is null then [Longitude] else @Longitude end
		  ,[Location] = Case when @LocationSensor is null then [Location] else @LocationSensor end
		  ,[SensorName] = Case when @AssetName is null then [SensorName] else @AssetName end
		  ,[SensorState] = Case when @AssetState is null then [SensorState] else @AssetState end
		  ,[SensorType] = Case when @AssetModel is null then [SensorType] else @AssetModel end	
		  ,[ParkingSpaceId] = Case when @ParkingSpaceId is null then [ParkingSpaceId] else @ParkingSpaceId end
		  ,[SensorModel] = Case when @AssetModel is null then [SensorModel] else @AssetModel end
		  ,[DemandZone] = Case when @DemandStatus is null then DemandZone else @DemandStatus end
		  ,[OperationalStatus] = Case when @OperationalStatus is null then OperationalStatus else @OperationalStatus end
		  ,[OperationalStatusTime] = Case when @OperationalStatusTime is null then OperationalStatusTime else @OperationalStatusTime end
		  ,[WarrantyExpiration] = Case when @WarrantyExpiration is null then WarrantyExpiration else @WarrantyExpiration end
		  ,[LastPreventativeMaintenance] = Case when @LastPreventativeMaintenance is null then LastPreventativeMaintenance else @LastPreventativeMaintenance end
		  ,[NextPreventativeMaintenance] = Case when @NextPreventativeMaintenance is null then NextPreventativeMaintenance else @NextPreventativeMaintenance end
		  ,[InstallDateTime] = Case when @DateInstalled is null then [InstallDateTime] else @DateInstalled end
		WHERE SensorId = @AssetId
		and CustomerId = @CustomerId
	end else begin
		Print 'Inserting Sensor'
		INSERT INTO [Sensors]
           ([CustomerID]
           ,[SensorID]
           ,[BarCodeText]
           ,[Description]
           ,[GSMNumber]
           ,[GlobalMeterID]
           ,[Latitude]
           ,[Longitude]
           ,[Location]
           ,[SensorName]
           ,[SensorState]
           ,[SensorType]
           ,[InstallDateTime]
           ,[DemandZone]
           ,[Comments]
           ,[RoadWayType]
           ,[ParkingSpaceId]
           ,[SensorModel]
           ,[OperationalStatus]
           ,[WarrantyExpiration]
           ,[OperationalStatusTime]
           ,[LastPreventativeMaintenance]
           ,[NextPreventativeMaintenance]
           ,[OperationalStatusEndTime]
           ,[OperationalStatusComment])
     VALUES
		(@CustomerId
           ,@AssetId
           ,0x00--[BarCodeText]
           ,@AssetName--[Description]
           ,null--[GSMNumber]
           ,null--[GlobalMeterID]
           ,@latitude
           ,@Longitude
           ,@LocationSensor
           ,@AssetName--[SensorName]
           ,@AssetState
           ,@AssetModel
           ,@DateInstalled
           ,@DemandStatus
           ,null--[Comments]
           ,null--[RoadWayType]
           ,@ParkingSpaceId
           ,@AssetModel
           ,@OperationalStatus
           ,@WarrantyExpiration
           ,@OperationalStatusTime
           ,@LastPreventativeMaintenance
           ,@NextPreventativeMaintenance
           ,null--[OperationalStatusEndTime]
           ,null--[OperationalStatusComment]
         )
	end	
	
	exec sp_Audit_Sensors @Customerid,@AssetId,@AssetPendingReasonId,@CreateUserId				
END
GO
/****** Object:  View [dbo].[A_EventsAndTransactionsV]    Script Date: 04/01/2014 22:07:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[A_EventsAndTransactionsV]
AS
SELECT
				t.CustomerID,
				t.AreaID, 
				t.MeterID,
				t.TransDateTime AS DateTime,
				t.EventUID,
				t.MeterID AS AssetId,
				at.MeterGroupDesc AS AssetType,
				CASE m.MeterGroup
						WHEN 0 THEN m.MeterName
						WHEN 1 THEN m.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName, 
				et.EventTypeDesc AS EventType,
				et.EventTypeDesc AS EventClass,
				el.EventCode, 
				ec.EventDescVerbose, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				m.Location AS Street,
				cg1.DisplayName AS Suburb,
				dz.DemandZoneDesc AS DemandArea,
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				'' AS AlarmType, 
				ec.EventCode AS AlarmCode,  
				'' AS AlarmSeverity, 
				NULL AS ResolutionCode, 
				'' AS Source, 
				'' AS TimeNotified, 
				t.AmountInCents AS Amount, 
				t.BayNumber AS Bay, 
				ts.Description AS CardStatus, 
				tt.TransactionTypeDesc AS PaymentType, 
				t.ReceiptNo AS ReceiptNumber, 
				t.TimePaid, 
				t.TransDateTime AS TransactionTime, 
				t.TransactionsID AS TransactionId 
FROM dbo.Transactions AS t
			LEFT OUTER JOIN dbo.MeterMap AS mm
				ON mm.Customerid = t.CustomerID 
				AND mm.Areaid = t.AreaID 
				AND mm.MeterId = t.MeterID
			LEFT OUTER JOIN dbo.Meters AS m
				ON t.CustomerID = m.CustomerID 
				AND t.AreaID = m.AreaID 
				AND t.MeterID = m.MeterId
			LEFT OUTER JOIN dbo.EventLogs AS el
				ON t.AreaID = el.AreaID 
				AND t.CustomerID = el.CustomerID 
				AND el.MeterId = t.MeterID 
				AND t.TransDateTime = el.EventDateTime
			LEFT OUTER JOIN dbo.AssetType AS at
				ON at.MeterGroupId = m.MeterGroup 
				AND at.CustomerId = m.CustomerID
			LEFT OUTER JOIN dbo.EventCodes AS ec
				ON el.CustomerID = ec.CustomerID 
				AND el.EventSource = ec.EventSource 
				AND el.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.EventType AS et
				ON et.EventTypeId = ec.EventType
			LEFT OUTER JOIN dbo.DemandZone AS dz
				ON dz.DemandZoneId = m.DemandZone
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
				ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.TransactionStatus AS ts
				ON ts.StatusID = t.TransactionStatus
			LEFT OUTER JOIN dbo.TransactionType AS tt 
				ON tt.TransactionTypeId = t.TransactionType
			LEFT OUTER JOIN dbo.Areas AS a 
				ON a.AreaID = mm.AreaId2 
				AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z 
				ON z.ZoneId = mm.ZoneId 
				AND z.customerID = mm.Customerid
GO
/****** Object:  View [dbo].[A_EventsAlarmsTransactionsV]    Script Date: 04/01/2014 22:07:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[A_EventsAlarmsTransactionsV]
AS
SELECT * FROM A_EventsAndAlarmsV

UNION

SELECT * FROM A_EventsAndTransactionsV
GO
/****** Object:  View [dbo].[AssetOperationalStatusV]    Script Date: 04/01/2014 22:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AssetOperationalStatusV] 
AS
SELECT 			sadt.CustomerID,
				sadt.AreaID,
				(SELECT AreaName
						FROM dbo.Areas
						WHERE (AreaID = mm.AreaId2) AND (CustomerID = sadt.CustomerID)) AS Area, 
				(SELECT ZoneName
						FROM dbo.Zones
						WHERE (ZoneId = mm.ZoneId) AND (CustomerID = sadt.CustomerID)) AS Zone,
				me.Location AS Street, 
				cg1.DisplayName AS Suburb,  
				at.MeterGroupDesc AS AssetType, 
                (CASE sadt.MeterGroup 
						WHEN 0 THEN me.MeterName 
						WHEN 1 THEN me.MeterName 
						WHEN 10 THEN se.SensorName 
						WHEN 11 THEN cb.CashBoxName 
						WHEN 13 THEN gw.Description END) AS AssetName, 
				(CASE sadt.MeterGroup WHEN 0 THEN me.MeterId 
						WHEN 1 THEN me.MeterId 
						WHEN 10 THEN se.SensorID 
						WHEN 11 THEN cb.CashBoxID 
						WHEN 13 THEN gw.GateWayID END) AS AssetID, 
                CASE sadt.MeterGroup 
						WHEN 0 THEN
                    (SELECT OperationalStatusDesc AS 'State of Asset'
						FROM OperationalStatus
						WHERE OperationalStatusId = me.OperationalStatusID) 
						WHEN 1 THEN
                    (SELECT OperationalStatusDesc AS 'State of Asset'
						FROM OperationalStatus
						WHERE OperationalStatusId = me.OperationalStatusID) 
						WHEN 10 THEN
                    (SELECT OperationalStatusDesc AS 'State of Asset'
						FROM OperationalStatus
						WHERE OperationalStatusId =
                    (SELECT OperationalStatus
                        FROM Sensors
                        WHERE SensorID = mm.SensorID)) 
						WHEN 13 THEN
                    (SELECT OperationalStatusDesc AS 'State of Asset'
						FROM OperationalStatus
						WHERE OperationalStatusId =
                    (SELECT OperationalStatus
                        FROM Gateways
                        WHERE GateWayID = mm.GatewayID)) END AS 'State of Asset', 
				mss.StatusDesc AS StatusClass, 
                me.OperationalStatusTime, 
                sadt.ReportingDate, 
                sadt.RegulatedUpTimePCT AS RegulatedUpTimePercent, 
				el.EventDateTime, 
				el.EventUID,  
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = el.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				dz.DemandZoneDesc AS DemandArea
FROM  dbo.SLA_AssetDownTime AS sadt 
				LEFT OUTER JOIN dbo.MeterStatusEvents AS mse 
						ON sadt.Customerid = mse.CustomerID 
						AND sadt.Areaid = mse.AreaID 
						AND sadt.MeterId = mse.MeterId 
						AND  (DateDiff(DD, mse.TimeOfNotification, sadt.ReportingDate) <= 1)
						AND ( DateDiff(DD, mse.TimeOfNotification, sadt.ReportingDate) > 0)
				LEFT OUTER JOIN dbo.EventLogs AS el 
						ON el.Customerid = mse.CustomerID 
						AND el.Areaid = mse.AreaID 
						AND el.MeterId = mse.MeterId 
						AND  DateDiff(HH, el.EventDateTime, mse.TimeOfNotification) = 0 
				INNER JOIN dbo.Meters AS me 
						ON  sadt.CustomerID = me.CustomerID 
						AND  sadt.AreaID = me.AreaID 
						AND  sadt.MeterId = me.MeterId 
						AND  (DateDiff(DD, me.OperationalStatusTime, sadt.ReportingDate) <= 1)
						AND ( DateDiff(DD, me.OperationalStatusTime, sadt.ReportingDate) > 0)
				LEFT OUTER JOIN dbo.MeterMap AS mm 
						ON sadt.Customerid = mm.CustomerID 
						AND sadt.Areaid = mm.AreaID  
						AND sadt.MeterId = mm.MeterId 
				LEFT OUTER JOIN dbo.MeterServiceStatus AS mss 
						ON  mse.State = mss.StatusID 
				INNER JOIN dbo.AssetType AS at 
						ON at.MeterGroupId = sadt.MeterGroup 
						AND at.CustomerId = sadt.CustomerID 
				INNER JOIN dbo.Areas AS a 
						ON a.AreaID = mm.Areaid 
						AND a.CustomerID = sadt.Customerid 
				INNER JOIN dbo.Zones AS z 
						ON z.ZoneId = mm.ZoneId 
						AND z.customerID = sadt.Customerid 
				LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
						ON cg1.CustomGroupId = mm.CustomGroup1
				LEFT OUTER JOIN dbo.DemandZone AS dz 
						ON dz.DemandZoneId = me.DemandZone 
				LEFT OUTER JOIN dbo.Sensors AS se 
						ON mm.Customerid = se.CustomerID 
						AND se.SensorID = mm.SensorID 
				LEFT OUTER JOIN dbo.Gateways AS gw 
						ON gw.GateWayID = mm.GatewayID 
						AND gw.CustomerID = sadt.CustomerID
				LEFT OUTER JOIN dbo.CashBox AS cb 
						ON cb.CashBoxID = mm.CashBoxID
						AND cb.CustomerID = sadt.CustomerID
GO
/****** Object:  View [dbo].[EventsTransactionsV]    Script Date: 04/01/2014 22:07:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventsTransactionsV]
AS
SELECT
			t.CustomerID, 
			t.TransDateTime AS DateTime, 
			t.EventUID, 
			t.MeterID AS AssetId, 
			at.MeterGroupDesc AS AssetType, 
			CASE me.MeterGroup 
					WHEN 0 THEN me.MeterName
					WHEN 1 THEN me.MeterName
					WHEN 10 THEN (SELECT
						SensorName
					FROM Sensors
					WHERE SensorID = mm.SensorID)
					WHEN 13 THEN (SELECT
						Description
					FROM Gateways
					WHERE GateWayID = mm.GatewayID)
				END AS AssetName, 
			ec.EventCode AS AlarmCode, 
			et.EventTypeDesc AS EventType, 
			el.EventCode, 
			(SELECT EventSourceDesc 
				FROM dbo.EventSources
				WHERE (EventSourceCode = ec.EventSource)) AS Source, 
			mm.AreaId2, 
			mm.ZoneId, 
			z.ZoneName AS Zone, 
			a.AreaName AS Area, 
			me.Location AS Street,  
			cg1.DisplayName AS Suburb, 
			dz.DemandZoneDesc AS DemandArea, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
			t.AmountInCents AS Amount, 
			t.BayNumber AS Bay, 
			ts.Description AS CardStatus, 
			tt.TransactionTypeDesc AS PaymentType, 
			t.ReceiptNo AS ReceiptNumber, 
			t.SensorPaymentTransactionID, 
			t.TimePaid, 
			t.TransDateTime AS 'Time of Transaction', 
			t.TransactionsID AS TransactionId, 
			cct.Name AS CardType 
FROM dbo.Transactions AS t
				LEFT OUTER JOIN dbo.MeterMap AS mm
						ON mm.Customerid = t.CustomerID 
						AND mm.Areaid = t.AreaID 
						AND mm.MeterId = t.MeterID
				LEFT OUTER JOIN dbo.Meters AS me
						ON t.CustomerID = me.CustomerID 
						AND t.AreaID = me.AreaID 
						AND t.MeterID = me.MeterId
				LEFT OUTER JOIN dbo.EventLogs AS el
						ON t.AreaID = el.AreaID 
						AND t.CustomerID = el.CustomerID 
						AND el.MeterId = t.MeterID 
						AND t.TransDateTime = el.EventDateTime
				LEFT OUTER JOIN dbo.AssetType AS at
						ON at.MeterGroupId = me.MeterGroup 
						AND at.CustomerId = me.CustomerID
				LEFT OUTER JOIN dbo.EventCodes AS ec
						ON el.CustomerID = ec.CustomerID 
						AND el.EventSource = ec.EventSource 
						AND el.EventCode = ec.EventCode
				LEFT OUTER JOIN dbo.EventType AS et
						ON et.EventTypeId = ec.EventType
				LEFT OUTER JOIN dbo.DemandZone AS dz
						ON dz.DemandZoneId = me.DemandZone
				LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
						ON cg1.CustomGroupId = mm.CustomGroup1
				LEFT OUTER JOIN dbo.TransactionStatus AS ts
						ON ts.StatusID = t.TransactionStatus
				LEFT OUTER JOIN dbo.TransactionType AS tt 
						ON tt.TransactionTypeId = t.TransactionType
				LEFT OUTER JOIN dbo.Areas AS a 
						ON a.AreaID = mm.AreaId2 
						AND a.CustomerID = mm.Customerid
				LEFT OUTER JOIN dbo.Zones AS z 
						ON z.ZoneId = mm.ZoneId 
						AND z.customerID = mm.Customerid
				LEFT OUTER JOIN dbo.CreditCardTypes AS cct 
						ON cct.CreditCardType = t.CreditCardType
GO
/****** Object:  View [dbo].[EventsGSMConnectionLogsV]    Script Date: 04/01/2014 22:07:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventsGSMConnectionLogsV]
AS
SELECT
				cl.CustomerID, 
				cl.EventUID, 
				et.EventTypeDesc AS Description, 
				el.MeterId AS AssetId, 
				at.MeterGroupDesc AS AssetType, 
				CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
						WHEN 10 THEN (SELECT
							SensorName
						FROM Sensors
						WHERE SensorID = mm.SensorID)
						WHEN 13 THEN (SELECT
							Description
						FROM Gateways
						WHERE GateWayID = mm.GatewayID)
					END AS AssetName, 
				ec.EventCode AS AlarmCode, 
				et.EventTypeDesc AS EventType, 
				el.EventCode, 
				mm.AreaId2, 
				mm.ZoneId, 
				z.ZoneName AS Zone, 
				a.AreaName AS Area, 
				me.Location AS Street, 
				cg1.DisplayName AS Suburb, 
				dz.DemandZoneDesc AS DemandArea, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = cl.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = cl.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = cl.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = cl.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
				(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = cl.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
				cs.StatusName AS ConnectionStatus, 
				cl.StartDateTime AS DateTime, 
				cl.StartDateTime AS 'Start Time of Connection', 
				cl.EndDateTime AS 'End Time of Connection', 
				cl.Memo AS ErrorDescription, 
				cl.PortNo AS Port 
FROM dbo.GSMConnectionLogs AS cl
		LEFT OUTER JOIN dbo.MeterMap AS mm
				ON mm.Customerid = cl.CustomerID 
				AND mm.Areaid = cl.AreaID 
				AND mm.MeterId = cl.MeterId
		LEFT OUTER JOIN dbo.Meters AS me
				ON cl.CustomerID = me.CustomerID 
				AND cl.AreaID = me.AreaID 
				AND cl.MeterId = me.MeterId
		LEFT OUTER JOIN dbo.EventLogs AS el
				ON cl.AreaID = el.AreaID 
				AND cl.CustomerID = el.CustomerID 
				AND el.MeterId = cl.MeterId 
				AND el.EventDateTime = cl.StartDateTime
		LEFT OUTER JOIN dbo.AssetType AS at
				ON at.MeterGroupId = me.MeterGroup 
				AND at.CustomerId = me.CustomerID
		LEFT OUTER JOIN dbo.EventCodes AS ec
				ON el.CustomerID = ec.CustomerID 
				AND el.EventSource = ec.EventSource 
				AND el.EventCode = ec.EventCode
		LEFT OUTER JOIN dbo.EventType AS et
				ON et.EventTypeId = ec.EventType
		LEFT OUTER JOIN dbo.DemandZone AS dz
				ON dz.DemandZoneId = me.DemandZone
		LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
				ON cg1.CustomGroupId = mm.CustomGroup1
		LEFT OUTER JOIN dbo.GSMConnectionStatus AS cs
				ON cl.ConnectionStatus = cs.StatusID
		LEFT OUTER JOIN dbo.Areas AS a 
				ON a.AreaID = mm.AreaId2 
				AND a.CustomerID = mm.Customerid
		LEFT OUTER JOIN dbo.Zones AS z 
				ON z.ZoneId = mm.ZoneId 
				AND z.customerID = mm.Customerid
GO
/****** Object:  Table [dbo].[MeterComm]    Script Date: 04/01/2014 22:07:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MeterComm](
	[ID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[MeterTime] [datetime] NOT NULL,
	[Message] [varchar](50) NULL,
	[Elapsed] [bigint] NULL,
 CONSTRAINT [PK_MeterComm] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE UNIQUE NONCLUSTERED INDEX [Unique_MeterComm] ON [dbo].[MeterComm] 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[MeterTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[MSM_Sensor_Gateway_AttribStatExceptsSummV]    Script Date: 04/01/2014 22:07:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MSM_Sensor_Gateway_AttribStatExceptsSummV]
AS

SELECT     	md.CustomerID,  
			md.MeterId AS AssetId, 
			at.MeterGroupDesc AS AssetType, 
            CASE m.MeterGroup 
					WHEN 1 THEN m.MeterName 
					WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) 
					WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, 
			mm.AreaId2 AS AreaID, 
            mm.ZoneId, 
			m.Location AS Street, 
			cg1.DisplayName AS Suburb, 
			a.AreaName AS Area, 
            z.ZoneName AS Zone, 
			dz.DemandZoneDesc AS DemandArea, 
			md.EventUID,
			ec.EventCode AS AlarmCode, 
			et.EventTypeDesc AS EventClass, 
			el.EventCode, 
			md.DiagTime AS 'Time of Last Status Report', 
			md.ReceivedTime AS ReceivedDateTime, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = md.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5, 
			mdt.DiagnosticDesc AS 'Description of Last Event', 
			md.DiagnosticValue AS Value, 
			CAST(CASE md.DiagnosticType WHEN 204 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_MinVoltDryBattery, 
			CAST(CASE md.DiagnosticType WHEN 205 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_CurrVoltDryBattery, 
			CAST(CASE md.DiagnosticType WHEN 206 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_MinVoltSolarBattery, 
			CAST(CASE md.DiagnosticType WHEN 207 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_CurrVoltSolarBattery, 
			CAST(CASE md.DiagnosticType WHEN 212 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_MinTemp, 
			CAST(CASE md.DiagnosticType WHEN 213 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_MaxTemp, 
			CAST(CASE md.DiagnosticType WHEN 214 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_CurrTemp, 
			CASE md.DiagnosticType WHEN 232 THEN md.DiagnosticValue ELSE NULL END AS SoftwareVersion, 
			CASE md.DiagnosticType WHEN 227 THEN md.DiagnosticValue ELSE NULL END AS ConfigurationVersion, 
			CASE md.DiagnosticType WHEN 231 THEN md.DiagnosticValue ELSE NULL END AS 'MPB Version2', 
			CASE md.DiagnosticType WHEN 215 THEN md.DiagnosticValue ELSE NULL END AS MainboardCodeRevision1, 
			CASE md.DiagnosticType WHEN 216 THEN md.DiagnosticValue ELSE NULL END AS MainboardCodeRevision2,   
            CAST(CASE md.DiagnosticType WHEN 223 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS MSM_NetworkStrength 
FROM    dbo.MeterDiagnostic AS md 
			LEFT OUTER JOIN dbo.MeterMap AS mm 
				ON mm.Customerid = md.CustomerID 
				AND mm.Areaid = md.AreaId 
				AND mm.MeterId = md.MeterId 
			LEFT OUTER JOIN dbo.Meters AS m 
				ON mm.Customerid = m.CustomerID 
				AND mm.Areaid = m.AreaID 
				AND mm.MeterId = m.MeterId 
			LEFT OUTER JOIN dbo.MeterDiagnosticType AS mdt 
				ON mdt.ID = md.DiagnosticType 
			LEFT OUTER JOIN dbo.MeterDiagnosticTypeCustomer AS mdtc 
				ON md.CustomerID = mdtc.CustomerId 
				AND mdt.ID = mdtc.DiagnosticType 
				AND mdtc.IsDisplay = 1
			LEFT OUTER JOIN dbo.EventLogs AS el 
				ON md.AreaId = el.AreaID 
				AND md.CustomerID = el.CustomerID 
				AND el.MeterId = md.MeterId 
				AND el.EventUID = md.EventUID 
				AND el.EventDateTime = md.ReceivedTime 
			LEFT OUTER JOIN dbo.AssetType AS at 
				ON at.MeterGroupId = m.MeterGroup 
				AND at.CustomerId = m.CustomerID 
			LEFT OUTER JOIN dbo.EventCodes AS ec 
				ON el.CustomerID = ec.CustomerID 
				AND el.EventSource = ec.EventSource 
				AND el.EventCode = ec.EventCode 
			LEFT OUTER JOIN dbo.EventType AS et 
				ON et.EventTypeId = ec.EventType 
			LEFT OUTER JOIN dbo.Areas AS a 
				ON a.AreaID = mm.AreaId2 
				AND a.CustomerID = mm.Customerid 
			LEFT OUTER JOIN dbo.Zones AS z 
				ON z.ZoneId = mm.ZoneId 
				AND z.customerID = mm.Customerid 
			LEFT OUTER JOIN dbo.DemandZone AS dz 
				ON dz.DemandZoneId = m.DemandZone 
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1 
				ON cg1.CustomGroupId = mm.CustomGroup1
GO
/****** Object:  View [dbo].[MeterUptimeV]    Script Date: 04/01/2014 22:07:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MeterUptimeV]
AS
SELECT
			sadt.CustomerID,
			sadt.ReportingDate, 
			CASE me.MeterGroup 
				WHEN 1 THEN me.MeterName END AS MeterName, 
				sadt.MeterID, 
			(SELECT MeterGroupDesc 
				FROM dbo.AssetType 
				WHERE (MeterGroupId = me.MeterGroup) 
				AND (CustomerId = sadt.CustomerID) AND (MeterGroupId=1)) AS MeterType,
			mm.AreaId2, 
			mm.ZoneId, 
			a.AreaName AS Area, 
			z.ZoneName AS Zone, 
			me.Location AS Street, 
			cg1.DisplayName AS Suburb, 
			sadt.TotalRegulatedMinutes, 
			sadt.TotalRegulatedDowntimeMinutes, 
			sadt.RegulatedUpTimePCT, 
			CAST(CASE md.DiagnosticType WHEN 222 THEN md.DiagnosticValue ELSE 0 END AS int) AS CoinRejects, 
			CAST(CASE md.DiagnosticType WHEN 221 THEN md.DiagnosticValue ELSE 0 END AS int) AS CoinCollected, 
			(SELECT COUNT(EventCode) 
				FROM dbo.EventLogs 
				WHERE (EventCode = 96) 
				AND sadt.ReportingDate = EventDateTime) AS CCTransactions, 
			(SELECT COUNT(EventCode) 
				FROM dbo.EventLogs 
				WHERE (EventCode = 65) 
				AND sadt.ReportingDate = EventDateTime) AS CCReadErrors, 
			dz.DemandZoneDesc AS SpaceDemandType
FROM dbo.SLA_AssetDownTime AS sadt
			LEFT OUTER JOIN dbo.EventLogs AS el
					ON sadt.Customerid = el.CustomerID 
					AND sadt.Areaid = el.AreaID 
					AND sadt.MeterId = el.MeterId 
					AND sadt.ReportingDate = el.EventDateTime
			LEFT OUTER JOIN dbo.MeterMap AS mm
					ON mm.Customerid = sadt.CustomerID 
					AND mm.Areaid = sadt.AreaID 
					AND mm.MeterId = sadt.MeterId
			LEFT OUTER JOIN dbo.Meters AS me
					ON sadt.Customerid = me.CustomerID 
					AND sadt.Areaid = me.AreaID 
					AND sadt.MeterId = me.MeterId
			LEFT OUTER JOIN dbo.MeterDiagnostic AS md 
					ON sadt.Customerid = md.CustomerID 
					AND sadt.Areaid = md.AreaID 
					AND sadt.MeterId = md.MeterId 
					AND sadt.ReportingDate =  md.DiagTime 
			LEFT OUTER JOIN dbo.MeterDiagnosticType AS mdt 
					ON mdt.ID = md.DiagnosticType 
			LEFT OUTER JOIN dbo.MeterDiagnosticTypeCustomer AS mdtc 
					ON md.CustomerID = mdtc.CustomerId 
					AND mdt.ID = mdtc.DiagnosticType 
					AND mdtc.IsDisplay = 1
			LEFT OUTER JOIN dbo.EventCodes AS ec
					ON el.CustomerID = ec.CustomerID 
					AND el.EventSource = ec.EventSource 
					AND el.EventCode = ec.EventCode
			LEFT OUTER JOIN dbo.DemandZone AS dz
					ON dz.DemandZoneId = me.DemandZone
			LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
					ON cg1.CustomGroupId = mm.CustomGroup1
			LEFT OUTER JOIN dbo.Areas AS a
					ON a.AreaID = mm.AreaId2 
					AND a.CustomerID = mm.Customerid
			LEFT OUTER JOIN dbo.Zones AS z
					ON z.ZoneId = mm.ZoneId 
					AND z.customerID = mm.Customerid
			WHERE sadt.MeterGroup = 1
GO
/****** Object:  Table [dbo].[SensorMappingAudit]    Script Date: 04/01/2014 22:07:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SensorMappingAudit](
	[SensorMappingAuditID] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[SensorMappingID] [int] NOT NULL,
	[CustomerID] [int] NOT NULL,
	[SensorID] [int] NOT NULL,
	[ParkingSpaceID] [bigint] NULL,
	[GatewayID] [int] NOT NULL,
	[IsPrimaryGateway] [bit] NOT NULL,
	[ChangeDate] [datetime] NOT NULL,
	[UserId] [int] NULL,
	[AssetPendingReasonId] [int] NULL,
 CONSTRAINT [PK_SensorMappingAudit] PRIMARY KEY CLUSTERED 
(
	[SensorMappingAuditID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[pv_EventsTransactions]    Script Date: 04/01/2014 22:07:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsTransactions]
AS
SELECT     t.CustomerID, t.TransDateTime AS DateTime, t.EventUID, t.MeterID AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, el.EventCode, mm.AreaId2, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, t.TimeType1, t.TimeType2, t.TimeType3, t.TimeType4, 
                      t.TimeType5, t.AmountInCents AS Amount, t.BayNumber AS Bay, ts.Description AS CardStatus, tt.TransactionTypeDesc AS PaymentType, 
                      t.ReceiptNo AS ReceiptNumber, t.TimePaid, t.TransDateTime AS TransactionTime, t.TransactionsID AS TransactionId, z.ZoneName AS Zone, a.AreaName AS Area, 
                      cct.Name AS CardType
FROM         dbo.Transactions AS t INNER JOIN
                      dbo.EventLogs AS el ON t.AreaID = el.AreaID AND t.CustomerID = el.CustomerID AND el.MeterId = t.MeterID AND t.TransDateTime = el.EventDateTime LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = t.CustomerID AND mm.Areaid = t.AreaID AND mm.MeterId = t.MeterID LEFT OUTER JOIN
                      dbo.Meters AS m ON t.CustomerID = m.CustomerID AND t.AreaID = m.AreaID AND t.MeterID = m.MeterId LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.TransactionStatus AS ts ON ts.StatusID = t.TransactionStatus LEFT OUTER JOIN
                      dbo.TransactionType AS tt ON tt.TransactionTypeId = t.TransactionType LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid LEFT OUTER JOIN
                      dbo.CreditCardTypes AS cct ON cct.CreditCardType = t.CreditCardType
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = -192
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 252
		       Bottom = 125
		       Right = 420
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 458
		       Bottom = 125
		       Right = 618
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 656
		       Bottom = 125
		       Right = 891
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 6
		       Left = 929
		       Bottom = 125
		       Right = 1208
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 1246
		       Bottom = 125
		       Right = 1425
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 215
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 126
		       Left = 240
		       Bottom = 215
		       Right = 415
		    End
		    DisplayFlags = 280
		    TopColumn = 0
	      ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsTransactions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 126
		       Left = 453
		       Bottom = 245
		       Right = 617
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ts"
		    Begin Extent = 
		       Top = 126
		       Left = 655
		       Bottom = 215
		       Right = 815
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "tt"
		    Begin Extent = 
		       Top = 126
		       Left = 853
		       Bottom = 215
		       Right = 1045
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "t"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 214
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsTransactions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsTransactions'
GO
/****** Object:  View [dbo].[pv_EventsSummary]    Script Date: 04/01/2014 22:07:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsSummary]
AS
SELECT     el.CustomerID, el.EventDateTime AS DateTime, el.EventUID,
                          (SELECT     MeterGroupDesc
                            FROM          dbo.AssetType
                            WHERE      (MeterGroupId = m.MeterGroup) AND (CustomerId = el.CustomerID)) AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, m.MeterId AS AssetId,
                          (SELECT     EventTypeDesc
                            FROM          dbo.EventType
                            WHERE      (EventTypeId = ec.EventType)) AS EventClass, el.EventCode, mm.AreaId2, mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, 
                      dz.DemandZoneDesc AS DemandArea, el.TimeType1, el.TimeType2, el.TimeType3, el.TimeType4, el.TimeType5, ec.EventDescVerbose AS Description, 
                      a.AreaName AS Area, z.ZoneName AS Zone
FROM         dbo.EventLogs AS el INNER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = el.CustomerID AND mm.Areaid = el.AreaID AND mm.MeterId = el.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON mm.Customerid = m.CustomerID AND mm.Areaid = m.AreaID AND mm.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 206
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 244
		       Bottom = 125
		       Right = 404
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 442
		       Bottom = 125
		       Right = 677
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 715
		       Bottom = 125
		       Right = 894
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 6
		       Left = 932
		       Bottom = 95
		       Right = 1107
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 6
		       Left = 1145
		       Bottom = 125
		       Right = 1309
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "a"
		    Begin Extent = 
		       Top = 6
		       Left = 1347
		       Bottom = 125
		       Right = 1507
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
		 Begin Table = "z"
		    Begin Extent = 
		       Top = 96
		       Left = 932
		       Bottom = 215
		       Right = 1103
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 21
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsSummary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsSummary'
GO
/****** Object:  View [dbo].[pv_EventsGSMConnectionLogs]    Script Date: 04/01/2014 22:07:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsGSMConnectionLogs]
AS
SELECT     cl.CustomerID, cl.StartDateTime AS DateTime, cl.EventUID, cl.MeterId AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, el.EventCode, mm.AreaId2, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, cl.TimeType1, cl.TimeType2, cl.TimeType3, cl.TimeType4, 
                      cl.TimeType5, cs.StatusName AS ConnectionStatus, cl.StartDateTime AS StartTime, cl.EndDateTime AS EndTime, cl.Memo AS ErrorDescription, cl.PortNo AS Port, 
                      z.ZoneName AS Zone, a.AreaName AS Area
FROM         dbo.GSMConnectionLogs AS cl LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = cl.CustomerID AND mm.Areaid = cl.AreaID AND mm.MeterId = cl.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON cl.CustomerID = m.CustomerID AND cl.AreaID = m.AreaID AND cl.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.EventLogs AS el ON cl.AreaID = el.AreaID AND cl.CustomerID = el.CustomerID AND el.MeterId = cl.MeterId AND 
                      el.EventDateTime = cl.StartDateTime LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.GSMConnectionStatus AS cs ON cl.ConnectionStatus = cs.StatusID LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "cl"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 212
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 250
		       Bottom = 125
		       Right = 418
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 456
		       Bottom = 125
		       Right = 616
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 654
		       Bottom = 125
		       Right = 889
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 6
		       Left = 927
		       Bottom = 125
		       Right = 1206
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 1244
		       Bottom = 125
		       Right = 1423
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 215
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsGSMConnectionLogs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 126
		       Left = 240
		       Bottom = 215
		       Right = 415
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 126
		       Left = 453
		       Bottom = 245
		       Right = 617
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cs"
		    Begin Extent = 
		       Top = 126
		       Left = 655
		       Bottom = 215
		       Right = 815
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 9
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsGSMConnectionLogs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsGSMConnectionLogs'
GO
/****** Object:  View [dbo].[pv_EventsDiagnostics]    Script Date: 04/01/2014 22:07:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_EventsDiagnostics]
AS
SELECT     md.CustomerID, md.DiagTime AS DateTime, md.EventUID, md.MeterId AS AssetId, at.MeterGroupDesc AS AssetType, 
                      CASE m.MeterGroup WHEN 0 THEN m.MeterName WHEN 1 THEN m.MeterName WHEN 10 THEN
                          (SELECT     SensorName
                            FROM          Sensors
                            WHERE      SensorID = mm.SensorID) WHEN 13 THEN
                          (SELECT     Description
                            FROM          Gateways
                            WHERE      GateWayID = mm.GatewayID) END AS AssetName, ec.EventCode AS AlarmCode, et.EventTypeDesc AS EventClass, el.EventCode, mm.AreaId2 AS AreaID, 
                      mm.ZoneId, m.Location AS Street, cg1.DisplayName AS Suburb, dz.DemandZoneDesc AS DemandArea, md.TimeType1, md.TimeType2, md.TimeType3, 
                      md.TimeType4, md.TimeType5, md.ReceivedTime AS ReceivedDateTime, mdt.DiagnosticDesc AS Type, md.DiagnosticValue AS Value, a.AreaName AS Area, 
                      z.ZoneName AS Zone, CAST(CASE md.DiagnosticType WHEN 204 THEN md.DiagnosticValue WHEN 205 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) 
                      AS Voltage, CAST(CASE md.DiagnosticType WHEN 212 THEN md.DiagnosticValue WHEN 213 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) 
                      AS Temperature, CASE md.DiagnosticType WHEN 215 THEN md.DiagnosticValue ELSE NULL END AS SoftwareVersion, 
                      CAST(CASE md.DiagnosticType WHEN 222 THEN md.DiagnosticValue ELSE NULL END AS int) AS CoinRejectCount, 
                      CAST(CASE md.DiagnosticType WHEN 223 THEN md.DiagnosticValue ELSE NULL END AS decimal(8, 2)) AS SignalStrength
FROM         dbo.MeterDiagnostic AS md INNER JOIN
                      dbo.EventLogs AS el ON md.AreaId = el.AreaID AND md.CustomerID = el.CustomerID AND el.MeterId = md.MeterId AND el.EventUID = md.EventUID AND 
                      el.EventDateTime = md.DiagTime LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON mm.Customerid = md.CustomerID AND mm.Areaid = md.AreaId AND mm.MeterId = md.MeterId LEFT OUTER JOIN
                      dbo.Meters AS m ON mm.Customerid = m.CustomerID AND mm.Areaid = m.AreaID AND mm.MeterId = m.MeterId LEFT OUTER JOIN
                      dbo.MeterDiagnosticType AS mdt ON mdt.ID = md.DiagnosticType LEFT OUTER JOIN
                      dbo.MeterDiagnosticTypeCustomer AS mdtc ON md.CustomerID = mdtc.CustomerId AND mdt.ID = mdtc.DiagnosticType AND mdtc.IsDisplay = 1 LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID LEFT OUTER JOIN
                      dbo.EventCodes AS ec ON el.CustomerID = ec.CustomerID AND el.EventSource = ec.EventSource AND el.EventCode = ec.EventCode LEFT OUTER JOIN
                      dbo.EventType AS et ON et.EventTypeId = ec.EventType LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
	Begin DesignProperties = 
	   Begin PaneConfigurations = 
	      Begin PaneConfiguration = 0
		 NumPanes = 4
		 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
	      End
	      Begin PaneConfiguration = 1
		 NumPanes = 3
		 Configuration = "(H (1 [50] 4 [25] 3))"
	      End
	      Begin PaneConfiguration = 2
		 NumPanes = 3
		 Configuration = "(H (1 [50] 2 [25] 3))"
	      End
	      Begin PaneConfiguration = 3
		 NumPanes = 3
		 Configuration = "(H (4 [30] 2 [40] 3))"
	      End
	      Begin PaneConfiguration = 4
		 NumPanes = 2
		 Configuration = "(H (1 [56] 3))"
	      End
	      Begin PaneConfiguration = 5
		 NumPanes = 2
		 Configuration = "(H (2 [66] 3))"
	      End
	      Begin PaneConfiguration = 6
		 NumPanes = 2
		 Configuration = "(H (4 [50] 3))"
	      End
	      Begin PaneConfiguration = 7
		 NumPanes = 1
		 Configuration = "(V (3))"
	      End
	      Begin PaneConfiguration = 8
		 NumPanes = 3
		 Configuration = "(H (1[56] 4[18] 2) )"
	      End
	      Begin PaneConfiguration = 9
		 NumPanes = 2
		 Configuration = "(H (1 [75] 4))"
	      End
	      Begin PaneConfiguration = 10
		 NumPanes = 2
		 Configuration = "(H (1[66] 2) )"
	      End
	      Begin PaneConfiguration = 11
		 NumPanes = 2
		 Configuration = "(H (4 [60] 2))"
	      End
	      Begin PaneConfiguration = 12
		 NumPanes = 1
		 Configuration = "(H (1) )"
	      End
	      Begin PaneConfiguration = 13
		 NumPanes = 1
		 Configuration = "(V (4))"
	      End
	      Begin PaneConfiguration = 14
		 NumPanes = 1
		 Configuration = "(V (2))"
	      End
	      ActivePaneConfig = 0
	   End
	   Begin DiagramPane = 
	      Begin Origin = 
		 Top = 0
		 Left = 0
	      End
	      Begin Tables = 
		 Begin Table = "md"
		    Begin Extent = 
		       Top = 6
		       Left = 38
		       Bottom = 125
		       Right = 202
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mdt"
		    Begin Extent = 
		       Top = 6
		       Left = 240
		       Bottom = 95
		       Right = 401
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "el"
		    Begin Extent = 
		       Top = 6
		       Left = 439
		       Bottom = 125
		       Right = 607
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "mm"
		    Begin Extent = 
		       Top = 6
		       Left = 645
		       Bottom = 125
		       Right = 805
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "m"
		    Begin Extent = 
		       Top = 6
		       Left = 843
		       Bottom = 125
		       Right = 1078
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "at"
		    Begin Extent = 
		       Top = 126
		       Left = 651
		       Bottom = 245
		       Right = 930
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "ec"
		    Begin Extent = 
		       Top = 6
		       Left = 1116
		       Bottom = 125
		       Right = 1295
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 En' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsDiagnostics'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'd
		 Begin Table = "et"
		    Begin Extent = 
		       Top = 126
		       Left = 968
		       Bottom = 215
		       Right = 1132
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "a"
		    Begin Extent = 
		       Top = 6
		       Left = 1333
		       Bottom = 125
		       Right = 1493
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "z"
		    Begin Extent = 
		       Top = 96
		       Left = 240
		       Bottom = 215
		       Right = 411
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "dz"
		    Begin Extent = 
		       Top = 126
		       Left = 38
		       Bottom = 215
		       Right = 213
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
		 Begin Table = "cg1"
		    Begin Extent = 
		       Top = 126
		       Left = 449
		       Bottom = 245
		       Right = 613
		    End
		    DisplayFlags = 280
		    TopColumn = 0
		 End
	      End
	   End
	   Begin SQLPane = 
	   End
	   Begin DataPane = 
	      Begin ParameterDefaults = ""
	      End
	      Begin ColumnWidths = 23
		 Width = 284
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
		 Width = 1500
	      End
	   End
	   Begin CriteriaPane = 
	      Begin ColumnWidths = 11
		 Column = 1440
		 Alias = 900
		 Table = 1170
		 Output = 720
		 Append = 1400
		 NewValue = 1170
		 SortType = 1350
		 SortOrder = 1410
		 GroupBy = 1350
		 Filter = 1350
		 Or = 1350
		 Or = 1350
		 Or = 1350
	      End
	   End
	End
	' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsDiagnostics'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_EventsDiagnostics'
GO
/****** Object:  View [dbo].[qGSMFailed_SubSub]    Script Date: 04/01/2014 22:07:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qGSMFailed_SubSub]
AS
SELECT CustomerID, AreaID, MeterID, ConnectionStatus,
    MAX(StartDateTime) AS LastTime
FROM dbo.GSMConnectionLogs
GROUP BY MeterID, ConnectionStatus, CustomerID, AreaID
GO
/****** Object:  View [dbo].[qGSMFailed_Sub]    Script Date: 04/01/2014 22:07:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qGSMFailed_Sub]
AS
SELECT     dbo.qGSMFailed_SubSub.CustomerID, dbo.qGSMFailed_SubSub.AreaID, dbo.qGSMFailed_SubSub.MeterID, MAX(dbo.qGSMFailed_SubSub.LastTime) 
                      AS LastFailed, MAX(qGSMFailed_SubSub1.LastTime) AS LastSuccess, MAX(qGSMFailed_SubSub2.LastTime) AS LastAlarmCall, 
                      MAX(dbo.EventLogs.EventDateTime) AS LastMeterCall
FROM         dbo.qGSMFailed_SubSub LEFT OUTER JOIN
                      dbo.qGSMFailed_SubSub qGSMFailed_SubSub2 ON dbo.qGSMFailed_SubSub.AreaID = qGSMFailed_SubSub2.AreaID AND 
                      dbo.qGSMFailed_SubSub.MeterID = qGSMFailed_SubSub2.MeterID AND qGSMFailed_SubSub2.ConnectionStatus = 2 LEFT OUTER JOIN
                      dbo.qGSMFailed_SubSub qGSMFailed_SubSub1 ON dbo.qGSMFailed_SubSub.MeterID = qGSMFailed_SubSub1.MeterID AND 
                      qGSMFailed_SubSub1.ConnectionStatus < 3 LEFT OUTER JOIN
                      dbo.EventLogs ON dbo.qGSMFailed_SubSub.AreaID = dbo.EventLogs.AreaID AND dbo.qGSMFailed_SubSub.MeterID = dbo.EventLogs.MeterID AND 
                      dbo.EventLogs.EventCode = 66
WHERE     (dbo.qGSMFailed_SubSub.ConnectionStatus > 2) AND (dbo.qGSMFailed_SubSub.LastTime > DATEADD(m, - 1, CURRENT_TIMESTAMP)) AND 
                      (qGSMFailed_SubSub1.LastTime > DATEADD(m, - 1, CURRENT_TIMESTAMP)) AND (qGSMFailed_SubSub2.LastTime > DATEADD(m, - 1, 
                      CURRENT_TIMESTAMP))
GROUP BY dbo.qGSMFailed_SubSub.MeterID, dbo.qGSMFailed_SubSub.CustomerID, dbo.qGSMFailed_SubSub.AreaID
GO
/****** Object:  StoredProcedure [dbo].[sp_InsertUpdate_MeterMap]    Script Date: 04/01/2014 22:07:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_InsertUpdate_MeterMap] 	
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@MeterMapZoneId int,
	@MeterMapAreaId2 int,
	@CollRouteId int,
	@MeterMapSuburbId int,
	@MeterMapMaintenanceRoute int,
	@GatewayId int,
	@SensorId int,
	@CashboxId int,
	@AssetPendingReasonId int,
	@CreateUserId int	
AS
DECLARE
	@HousingId int
BEGIN
	Print '--------------------------------------------------------------------------------'
	Print 'INSERT/UPDATE and AUDIT METERMAP'
	Print 'C/A/M = ' + convert(varchar,@Customerid) + '/' + convert(varchar,@Areaid) + '/' +  convert(varchar,@MeterId) 
	
	if exists(select * from MeterMap where Customerid = @CustomerId and AreaId = @AreaId and MeterId = @MeterId)
	BEGIN
		Print 'Updating MeterMap'
		Update mm
		set mm.[ZoneId] = Case when @MeterMapZoneId is null then mm.[ZoneId] else @MeterMapZoneId end
			  ,mm.[AreaId2] = Case when @MeterMapAreaId2 is null then mm.[AreaId2] else @MeterMapAreaId2 end
			  ,mm.[CollRouteId] = Case when @CollRouteId is null then mm.[CollRouteId] else @CollRouteId end
			  ,mm.[MaintRouteId] = Case when @MeterMapMaintenanceRoute is null then mm.[MaintRouteId] else @MeterMapMaintenanceRoute end
			  ,mm.[CustomGroup1] = Case when @MeterMapSuburbId is null then mm.[CustomGroup1] else @MeterMapSuburbId end
		From MeterMap mm where Customerid = @CustomerId
			and 
			(
				(AreaId = @AreaId and MeterId  = @MeterId and (@MeterId is not null) and (@AreaId is not null))
				--or 
				--(SensorID = @SensorId)
			)
	END ELSE BEGIN
		Print 'Inserting MeterMap'
		
		exec sp_DefaultHousing @CustomerId,@HousingId output
				
		INSERT INTO [MeterMap]
			   ([Customerid]
			   ,[Areaid]
			   ,[MeterId]
			   ,[ZoneId]
			   ,[HousingId]
			   ,[MechId]
			   ,[AreaId2]
			   ,[CollRouteId]
			   ,[EnfRouteId]
			   ,[MaintRouteId]
			   ,[CustomGroup1]
			   ,[CustomGroup2]
			   ,[CustomGroup3]
			   ,[SubAreaID]
			   ,[GatewayID]
			   ,[SensorID]
			   ,[CashBoxID]
			   ,[CollectionRunId])
		 VALUES
			   (@CustomerId
			   ,@Areaid
			   ,@MeterId
			   ,@MeterMapZoneId
			   ,@HousingId
			   ,null--@MechId
			   ,@MeterMapAreaId2
			   ,@CollRouteId
			   ,null--@EnfRouteId
			   ,@MeterMapMaintenanceRoute
			   ,@MeterMapSuburbId
			   ,null--@CustomGroup2
			   ,null--@CustomGroup3
			   ,null--@SubAreaID
			   ,@GatewayID
			   ,@SensorID
			   ,@CashBoxID
			   ,null--@CollectionRunId
			   )
	END
		
	exec sp_Audit_MeterMap @Customerid,@AreaId,@MeterId,@AssetPendingReasonId,@CreateUserId
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Audit_VersionProfileMeter]    Script Date: 04/01/2014 22:07:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Audit_VersionProfileMeter] 	
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '------------------------------------------------------------'
	Print 'AUDIT VERSION PROFILE METER'
	
	INSERT INTO [VersionProfileMeterAudit]
           ([UserId]
           ,[UpdateDateTime]
           ,[VersionProfileMeterId]
           ,[ConfigurationName]
           ,[HardwareVersion]
           ,[SoftwareVersion]
           ,[CommunicationVersion]
           ,[Version1]
           ,[Version2]
           ,[Version3]
           ,[Version4]
           ,[Version5]
           ,[Version6]
           ,[CustomerId]
           ,[AreaId]
           ,[MeterId]
           ,[MeterGroup]
           ,[SensorID]
           ,[GatewayID])
	select @CreateUserId
           ,GETDATE()
           ,[VersionProfileMeterId]
           ,[ConfigurationName]
           ,[HardwareVersion]
           ,[SoftwareVersion]
           ,[CommunicationVersion]
           ,[Version1]
           ,[Version2]
           ,[Version3]
           ,[Version4]
           ,[Version5]
           ,[Version6]
           ,[CustomerId]
           ,[AreaId]
           ,[MeterId]
           ,[MeterGroup]
           ,[SensorID]
           ,[GatewayID]
        from VersionProfileMeter
        where CustomerId = @CustomerId
        and AreaId = @AreaId
        and MeterId = @MeterId         
END
GO
/****** Object:  StoredProcedure [dbo].[sp_activateCollectionRun]    Script Date: 04/01/2014 22:08:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_activateCollectionRun] 
/*
REQUIREMENT  

2.	For each configuration going active a check will be performed if there is an active Configuration ID with the same Configuration Name. If so:
a.	Update the Old Configuration ID for this Configuration Name to Inactive
b.	Update the New Configuration ID for this Configuration Name to Active
c.	Update the appropriate tables.

*/
as
	Declare cur  cursor	for
		select CustomerId,CollectionRunName,MAX(activationdate) activationDate
		from CollectionRun
		where ActivationDate < GETDATE()
				and CollectionRunStatus = 2 --Pending
		group by CustomerId,CollectionRunName
		
	Declare
		@collrunname varchar(200),
		@cid int,
		@activationDate datetime
		
begin
	open cur 
	
	fetch next from cur  into @cid,@collrunname,@activationDate
	WHILE @@FETCH_STATUS = 0 BEGIN		
		BEGIN TRY
			Print 'Updating ' + @collrunname
	
			-- Deactive the active ones first
			Update c
			set c.CollectionRunStatus = 0 -- Inactive
			from CollectionRun  c
			where c.CustomerId = @cid
			and c.CollectionRunName =  @collrunname
			and c.CollectionRunStatus in (1,2) -- (Active,Pending)
			and c.ActivationDate <> @activationDate
			
			-- And Activate
			Update c
			set c.CollectionRunStatus = 1 -- Active
			from CollectionRun  c
			where c.CustomerId = @cid
			and c.CollectionRunName =  @collrunname
			and c.CollectionRunStatus in (2) -- (Pending)
			and c.ActivationDate = @activationDate --must be equal
			
			-- Update MeterMap
			exec sp_MeterMap_UpdateCollectionRunId
			
		END TRY
		BEGIN CATCH
			Print 'Error in loop ' + Error_message()
			BREAK
		END CATCH
		fetch next from cur  into @cid,@collrunname,@activationDate
	END	--while @@FETCH_STATUS	
	
	CLOSE cur 
	
	DEALLOCATE cur 
		
end
GO
/****** Object:  StoredProcedure [dbo].[SP_UDP_METERCOMM]    Script Date: 04/01/2014 22:08:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[SP_UDP_METERCOMM]
	  (@CUSTOMERID INT
	   ,@AREAID INT 
	  ,@METERID INT
	  ,@MESSAGE VARCHAR(50)
	  ,@ELAPSED INT
	  ,@TS DateTIME
	  )
AS
BEGIN 
	if exists(select * from meters where customerid = @CUSTOMERID and areaid = @AREAID and meterid = @METERID) begin
		INSERT INTO [METERCOMM]
			   ([CUSTOMERID]
			   ,[AREAID]
			   ,[METERID]
			   ,[METERTIME]
			  ,[MESSAGE]
			  ,[ELAPSED]
			   )
		 VALUES
			  (@CUSTOMERID,
			  @AREAID,
			  @METERID,
			  @TS,
			  @MESSAGE,
			  @ELAPSED)
	end 
END
GO
/****** Object:  StoredProcedure [dbo].[sp_Update_Audit_SensorMapping]    Script Date: 04/01/2014 22:08:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Update_Audit_SensorMapping] 	
	@CustomerId int,
	@SensorId bigint,
	@ParkingSpaceId bigint,
	@GatewayId bigint,
	@IsPrimaryGateway int,
	@AssetPendingReasonId int,
	@CreateUserId int
AS
BEGIN
	Print '----------------------------------------------'
	Print 'Updating SENSOR MAPPING'
	
	if (@GatewayId is null)begin
		Print 'GatewayId is null. Primary =' + convert(Varchar,@IsPrimaryGateway)
		return;
	end
		
		
	Print 'GatewayId = ' + convert(Varchar,@GatewayId)
	
	if exists(select * from SensorMapping WHERE SensorId = @SensorId and [IsPrimaryGateway] = @IsPrimaryGateway)
	begin
		UPDATE [SensorMapping]
			SET [ParkingSpaceID] = @ParkingSpaceId
				,[GatewayID] = @GatewayId						
		WHERE SensorId = @SensorId
				and [IsPrimaryGateway] = @IsPrimaryGateway
				and CustomerID = @CustomerId	
	end else begin
		INSERT INTO [SensorMapping]
           ([CustomerID]
           ,[SensorID]
           ,[ParkingSpaceID]
           ,[GatewayID]
           ,[IsPrimaryGateway]
           ,[MappingState])
     VALUES
		(@CustomerId
           ,@SensorId
           ,@ParkingSpaceId
           ,@GatewayId
           ,@IsPrimaryGateway
           ,1--[MappingState]
           )		
	end
		
	Print 'Auditing SENSOR MAPPING'
	
	INSERT INTO [SensorMappingAudit]
           ([SensorMappingID]
           ,[CustomerID]
           ,[SensorID]
           ,[ParkingSpaceID]
           ,[GatewayID]
           ,[IsPrimaryGateway]
           ,[ChangeDate]
           ,[UserId]
           ,[AssetPendingReasonId])	
          select [SensorMappingID]
			   ,[CustomerID]
			   ,[SensorID]
			   ,[ParkingSpaceID]
			   ,[GatewayID]
			   ,[IsPrimaryGateway]
			   ,GETDATE()
			   ,@CreateUserId
			   ,@AssetPendingReasonId
          from SensorMapping
          WHERE SensorId = @SensorId
				and [IsPrimaryGateway] = @IsPrimaryGateway
				and CustomerID = @CustomerId
			
END
GO
/****** Object:  Table [dbo].[TransactionsSmartCard]    Script Date: 04/01/2014 22:08:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsSmartCard](
	[TransactionsSmartCardId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceID] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[TimePaid] [int] NOT NULL,
	[Status] [int] NULL,
	[TransType] [int] NOT NULL,
	[SerialNo] [int] NULL,
	[CardID] [varchar](40) NOT NULL,
	[AcquirerTransReference] [varchar](50) NULL,
	[MeterTransReference] [varchar](50) NULL,
	[TimePaidByRate] [int] NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[TopUp] [bit] NULL,
	[GatewayMethod] [int] NULL,
 CONSTRAINT [PK_TransactionsSmartCard] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [TransactionsSmartCard_IDXParkingSpaceID] ON [dbo].[TransactionsSmartCard] 
(
	[ParkingSpaceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsMPark]    Script Date: 04/01/2014 22:08:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TransactionsMPark](
	[TransactionsMParkID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NULL,
	[CustomerID] [int] NOT NULL,
	[AreaID] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[TransactionID] [bigint] NOT NULL,
	[BayNumber] [int] NOT NULL,
	[AmountInCents] [int] NOT NULL,
	[TimePaid] [int] NOT NULL,
	[UserType] [int] NOT NULL,
	[OriginateDateTime] [datetime] NULL,
	[UserCLI] [bigint] NULL,
	[ReconFileID] [bigint] NULL,
	[ReconDateTime] [datetime] NULL,
	[NumberOfMinutes] [int] NULL,
	[TimePaidByRate] [int] NULL,
	[VendorId] [int] NULL,
 CONSTRAINT [PK_TransactionsMPark] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsMPark_IDX_TCAMTA] ON [dbo].[TransactionsMPark] 
(
	[TransDateTime] ASC,
	[CustomerID] ASC,
	[AreaID] ASC,
	[MeterId] ASC,
	[TransactionsMParkID] ASC,
	[AmountInCents] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsMPark_IDX_TTABTACM] ON [dbo].[TransactionsMPark] 
(
	[TransactionID] ASC,
	[TimePaid] ASC,
	[AmountInCents] ASC,
	[BayNumber] ASC,
	[TransDateTime] ASC,
	[AreaID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsMpark_IDXParkingSpaceID] ON [dbo].[TransactionsMPark] 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TransactionsCreditCard]    Script Date: 04/01/2014 22:08:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TransactionsCreditCard](
	[TransactionsCreditCardID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceId] [bigint] NULL,
	[CustomerId] [int] NOT NULL,
	[AreaId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[TransDateTime] [datetime] NOT NULL,
	[ReceiptNo] [int] NOT NULL,
	[BayNumber] [int] NULL,
	[AmountInCents] [int] NOT NULL,
	[TimePaid] [int] NULL,
	[Status] [int] NOT NULL,
	[CardNumHash] [varchar](40) NULL,
	[CreditCardType] [int] NULL,
	[XSwipeData] [varchar](160) NULL,
	[AcquirerTransReference] [varchar](50) NULL,
	[PaymentTarget] [char](1) NULL,
	[CashKeySrID] [int] NULL,
	[CashKeyBalanceBefore] [int] NULL,
	[BatchId] [varchar](20) NULL,
	[Reconciled] [bit] NULL,
	[ResultDateTime] [datetime] NULL,
	[HashScheme] [int] NOT NULL,
	[TimePaidByRate] [int] NULL,
	[OLTStatusCode] [int] NULL,
	[OLTOriginalStatusCode] [int] NULL,
	[OLTCardBalance] [int] NULL,
	[Track64] [varchar](500) NULL,
	[CreateDateTime] [datetime] NULL,
	[DiscountInCent] [int] NULL,
	[DiscountSchemeId] [int] NULL,
	[Last4] [varchar](4) NULL,
	[PrepayUsed] [bit] NULL,
	[FreeParkingUsed] [bit] NULL,
	[TopUp] [bit] NULL,
	[DiscountrejectInCent] [int] NULL,
	[GatewayMethod] [int] NULL,
	[ParkAndSwimLink] [bigint] NULL,
 CONSTRAINT [PK_TransactionsCreditCard] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[TransDateTime] ASC,
	[ReceiptNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
CREATE NONCLUSTERED INDEX [TransactionsCreditCard_IDX_STMACTA] ON [dbo].[TransactionsCreditCard] 
(
	[Status] ASC,
	[TransDateTime] ASC,
	[MeterId] ASC,
	[AreaId] ASC,
	[CustomerId] ASC,
	[TransactionsCreditCardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCreditCard_IDX_TC] ON [dbo].[TransactionsCreditCard] 
(
	[TransactionsCreditCardID] ASC,
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCreditCard_IDX_TCAMSC] ON [dbo].[TransactionsCreditCard] 
(
	[TransactionsCreditCardID] ASC,
	[CustomerId] ASC,
	[AreaId] ASC,
	[MeterId] ASC,
	[Status] ASC,
	[CreditCardType] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCreditCard_IDX_TCSATRTBAMAC] ON [dbo].[TransactionsCreditCard] 
(
	[TransDateTime] ASC,
	[CreditCardType] ASC,
	[Status] ASC,
	[AcquirerTransReference] ASC,
	[TransactionsCreditCardID] ASC,
	[ReceiptNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [TransactionsCreditCard_IDXParkingSpaceID] ON [dbo].[TransactionsCreditCard] 
(
	[ParkingSpaceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
/****** Object:  View [dbo].[CustomerPaymentTransactionV]    Script Date: 04/01/2014 22:08:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CustomerPaymentTransactionV]
AS 
SELECT 
			t.CustomerID, 
			t.MeterId, 
			t.SensorID, 
			CASE me.MeterGroup
						WHEN 0 THEN me.MeterName
						WHEN 1 THEN me.MeterName
					END AS MeterName, 
			at.MeterGroupDesc AS MeterType, 
			(SELECT DisplayName FROM dbo.CustomGroup1 WHERE (CustomGroupId = mm.CustomGroup1) AND (CustomerId = sptx.CustomerID)) AS Suburb,   
			mm.AreaId2, 
			t.ZoneId, 
			(SELECT AreaName
				FROM dbo.Areas
				WHERE (AreaID = mm.AreaId2) AND (CustomerID = t.CustomerID)) AS Area, 
			(SELECT ZoneName
				FROM dbo.Zones
				WHERE (ZoneId = mm.ZoneId) AND (customerID = t.CustomerID)) AS Zone,
			me.Location AS Street, 
			t.ParkingSpaceId AS SpaceId, 
			t.BayNumber AS BayId, 
			dz.DemandZoneDesc AS DemandArea, 
			sptx.SensorPaymentTransactionId,
			sptx.ArrivalTime, 
			CASE t.TxValue WHEN 'IN' THEN 'IN'
						WHEN 'OUT' THEN 'OUT'
						ELSE 'N/A'
					END  AS VehicleStatus, 
			sptx.DepartureTime, 
			t.TransactionsID, 
			t.TransDateTime AS TimeOfTransaction, 
			t.ExpiryTime AS ExpirationTime, 
			sptx.FirstTxPaymentTime AS TimeProcessed, 
			CASE t.TxValue 	WHEN 'IN' THEN ''
				WHEN 'OUT' THEN ''
				ELSE (SELECT PaymentTypeDesc FROM dbo.PaymentType WHERE (PaymentType = sptx.FirstTxPaymentMethod))
					END  AS PaymentMethod, 
			sptx.FirstTxTimePaidMinute AS TimePaid, 
			sptx.LastTxPaymentTime AS LastTimeProcessed, 
			sptx.LastTxExpiryTime AS LastExpirationTime, 
			sptx.LastTxAmountInCent, 
			sptx.LastTxTimePaidMinute AS LastTimePaid, 
			CASE t.TxValue 	WHEN 'IN' THEN ''
				WHEN 'OUT' THEN ''
				ELSE (SELECT PaymentTypeDesc FROM dbo.PaymentType WHERE (PaymentType = sptx.LastTxPaymentMethod))
					END  AS LastPaymentMethod, 
			sptx.TotalNumberOfPayment AS TotalNumberOfPayments,   
			cdms.NumOfCoins AS CoinsAccepted, 
			cdms.NumOfCoinsRej AS CoinsRejected, 
			CASE tt.TransactionTypeId WHEN 21 THEN 'Sensor'
						ELSE 'NO'
						END AS SensorTransactions,  
			ts.Description AS TransactionStatus,  
			t.ReceiptNo AS ReceiptID, 
			tcc.BatchID, 
			sptx.TotalTimePaidMinute AS NetTimePaid, 
			(CASE t.FreeParkingUsed WHEN 1 THEN 'YES' WHEN 0 THEN 'NO' END) AS '15 minutes free Used', 
			sptx.ZeroOutTime AS SensorZeroTime, 
			sptx.TotalOccupiedMinute, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeClass1, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeClass2, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeClass3, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeClass4, 
			(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = sptx.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeClass5 
FROM dbo.SensorPaymentTransaction AS sptx 
		LEFT OUTER JOIN dbo.Transactions AS t
			ON sptx.Customerid = t.CustomerID 
			AND sptx.SensorPaymentTransactionId = t.SensorPaymentTransactionId
			AND sptx.SensorId = t.SensorId
		LEFT OUTER JOIN dbo.MeterMap AS mm
			ON t.CustomerID = mm.CustomerID 
			AND t.MeterID = mm.MeterId 
			AND t.AreaID = mm.Areaid 
		LEFT OUTER JOIN dbo.TransactionsCreditCard AS tcc 
			ON t.CustomerID = tcc.CustomerID 
			AND t.MeterID = tcc.MeterId 
			AND t.AreaID = tcc.AreaID 
			AND t.TransDateTime = tcc.TransDateTime 
			AND t.ReceiptNo = tcc.ReceiptNo 
		LEFT OUTER JOIN dbo.Meters AS me 
			ON t.CustomerID = me.CustomerID 
			AND t.MeterID = me.MeterId 
			AND t.AreaID = me.AreaID
		LEFT OUTER JOIN dbo.TransactionStatus AS ts
			ON t.TransactionStatus = ts.StatusID 
		LEFT OUTER JOIN dbo.CollDataMeterStatus AS cdms
			ON t.CustomerID = cdms.CustomerId 
			AND t.AreaID = cdms.AreaId 
			AND t.MeterId = cdms.MeterId 
			AND t.TransDateTime = cdms.StatusDateTime 
		LEFT OUTER JOIN dbo.AssetType AS at
			ON at.MeterGroupId = me.MeterGroup 
			AND at.CustomerId = me.CustomerID
		LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
			ON cg1.CustomGroupId = mm.CustomGroup1
		LEFT OUTER JOIN dbo.DemandZone AS dz
			ON dz.DemandZoneId = me.DemandZone 
		LEFT OUTER JOIN dbo.TransactionType AS tt
			ON t.TransactionType = tt.TransactionTypeId
GO
/****** Object:  StoredProcedure [dbo].[spLastMeterState31]    Script Date: 04/01/2014 22:08:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spLastMeterState31]
  @LiveMeters bit,
  @CustomerId int,
  @AreaId  int,
  @MeterId  int AS

/* Transaction data Cash*/
declare @TransactionsCash table
(
    [tCustomerId] [int] NULL ,
	[tMeterId] [int] NULL ,
	[tTransDateTime] [datetime] NULL,
	[tAmount] [float] NULL
)
Insert @TransactionsCash
SELECT   Q.CustomerId,Q.MeterId, Q.TransDateTime, Q.AmountInCents/100.00 As Amount
FROM TransactionsCash As Q INNER JOIN
        (SELECT CustomerId,MeterId, Max(TransDateTime) As MaxDateTime
         FROM TransactionsCash
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId = T.CustomerId AND Q.MeterId=T.MeterId AND Q.TransDateTime = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.TransDateTime < CURRENT_TIMESTAMP

/* Transaction data CCard*/
declare @TransactionsCCard table
(
    [pCustomerId] [int] NULL ,
	[pMeterId] [int] NULL ,
	[pTransDateTime] [datetime] NULL,
	[pAmount] [float] NULL
)
Insert @TransactionsCCard
SELECT  Q.CustomerId,Q.MeterId, Q.TransDateTime, Q.AmountInCents/100.00 As Amount
FROM TransactionsCreditCard As Q INNER JOIN
        (SELECT  CustomerId,MeterId, Max(TransDateTime) As MaxDateTime
         FROM TransactionsCreditCard
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId = T.CustomerId AND Q.MeterId=T.MeterId AND Q.TransDateTime = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.TransDateTime < CURRENT_TIMESTAMP

/* Transaction data MPark*/
declare @TransactionsMPark table
(
    [rCustomerId] [int] NULL ,
	[rMeterId] [int] NULL ,
	[rTransDateTime] [datetime] NULL,
	[rAmount] [float] NULL
)
Insert @TransactionsMPark
SELECT Q.CustomerId,Q.MeterId, Q.TransDateTime, Q.AmountInCents/100.00 As Amount
FROM TransactionsMPark As Q INNER JOIN
        (SELECT   CustomerId,MeterId, Max(TransDateTime) As MaxDateTime
         FROM TransactionsMPark
         GROUP BY CustomerId, MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.TransDateTime = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.TransDateTime < CURRENT_TIMESTAMP
/* Active AlarmsAlarms data*/
/* - Add more columns here to appear in final output */
declare @ActiveAlarms table
(
    [aCustomerId][int] NULL,
	[aMeterId] [int] NULL ,
    [aEventCode] [int] NULL ,
    [aEventSource] [int] NULL ,
	[aTimeOfOccurrance] [datetime] NULL,
    [aEventDescVerbose] varchar(50) NULL
)
Insert @ActiveAlarms
SELECT  Q.CustomerId,Q.MeterId, Q.EventCode,Q.EventSource,Q.TimeOfOccurrance,X.EventDescVerbose
FROM ActiveAlarms As Q INNER JOIN
        (SELECT  MeterId, Max(TimeOfOccurrance) As MaxDateTime
         FROM ActiveAlarms
         GROUP BY CustomerId,MeterId) As T
         ON Q.MeterId=T.MeterId AND Q.TimeOfOccurrance = T.MaxDateTime
INNER JOIN EventCodes As X ON Q.CustomerId = X.CustomerId AND Q.EventCode = X.EventCode AND Q.EventSource = X.EventSource
WHERE     Q.CustomerId = @CustomerId
and  Q.TimeOfOccurrance < CURRENT_TIMESTAMP

/* HistoricalAlarms data*/
declare @HistoricalAlarms table
(
    [hCustomerId][int] NULL,
	[hMeterId] [int] NULL ,
    [hEventCode] [int] NULL ,
    [hEventSource] [int] NULL ,
	[hTimeOfOccurrance] [datetime] NULL,
    [hEventDescVerbose] varchar(50) NULL
)
Insert @HistoricalAlarms
SELECT  Q.CustomerId,Q.MeterId, Q.EventCode,Q.EventSource,Q.TimeOfOccurrance,X.EventDescVerbose
FROM HistoricalAlarms As Q INNER JOIN
        (/*SELECT MeterId, Max(TimeOfOccurrance) As MaxDateTime*/
        SELECT  CustomerId,MeterId, Max(EventUId) As MaxEventUId
         FROM HistoricalAlarms
         GROUP BY CustomerId,MeterId) As T
  /*      ON Q.MeterId=T.MeterId AND Q.TimeOfOccurrance = T.MaxDateTime */
      ON Q.CustomerId = T.CustomerId AND Q.MeterId=T.MeterId AND Q.EventUId = T.MaxEventUId
INNER JOIN EventCodes As X ON Q.CustomerId = X.CustomerId AND Q.EventCode = X.EventCode AND Q.EventSource = X.EventSource
WHERE     Q.CustomerId = @CustomerId
and  Q.TimeOfOccurrance < CURRENT_TIMESTAMP

/* CollDataMeterStatus data*/
DECLARE @CollDataMeterStatus TABLE (
    [cCustomerId] [int] NULL ,
	[cMeterId] [int] NULL ,
	[cStatusDateTime] [datetime] NULL ,
	[cCurrVoltDryBattery] [float] NULL ,
	[cCurrVoltSolarBattery] [float] NULL ,
    [cMinVoltDryBattery] [float] NULL,
    [cMinVoltSolarBattery] [float] NULL,
    [cCurrVoltSolarPanel] [float] NULL ,
    [cMinVoltSolarPanel] [float] NULL,
    [cNetworkStrength][int]NULL
)
Insert @CollDataMeterStatus
SELECT  Q.CustomerId,Q.MeterId, Q.StatusDateTime,Q.CurrVoltDryBattery,Q.CurrVoltSolarBattery,Q.MinVoltDryBattery,Q.MinVoltSolarBattery,Q.MinVoltSolarPanel,
Q.CurrVoltSolarPanel,Q.NetworkStrength
FROM CollDataMeterStatus As Q INNER JOIN
        (SELECT  CustomerId,MeterId, Max(StatusDateTime) As MaxDateTime
         FROM CollDataMeterStatus
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.StatusDateTime = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.StatusDateTime < CURRENT_TIMESTAMP

/* CollDataSumm data*/
DECLARE @CollDataSumm TABLE (
    [sCustomerId] [int] NULL ,
	[sMeterId] [int] NULL ,
	[sCollDateTime] [datetime] NULL ,
	[sAmount] [float] NULL
)
Insert @CollDataSumm
SELECT  Q.CustomerId,Q.MeterId, Q.CollDateTime,Q.Amount
FROM CollDataSumm As Q INNER JOIN
        (SELECT   CustomerId,MeterId, Max(CollDateTime) As MaxDateTime
         FROM CollDataSumm
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId = T.CustomerId AND Q.MeterId=T.MeterId AND Q.CollDateTime = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.CollDateTime < CURRENT_TIMESTAMP

/*CashBoxDataImport data*/
DECLARE @CashBoxDataImport TABLE (
    [dCustomerId] [int] NULL ,
	[dMeterId] [int] NULL ,
	[dDateTimeRem] [datetime] NULL ,
	[dAmount] [float] NULL
)
Insert @CashBoxDataImport
SELECT Q.CustomerId,Q.MeterId, Q.DateTimeRem,Q.AmtManual
FROM CashBoxDataImport As Q INNER JOIN
        (SELECT   CustomerId,MeterId, Max(DateTimeRem) As MaxDateTime
         FROM CashBoxDataImport
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.DateTimeRem = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.DateTimeRem < CURRENT_TIMESTAMP

/* CollDataImporta*/
DECLARE @CollDataImport TABLE (
    [kCustomerId] [int] NULL ,
	[kMeterId] [int] NULL ,
	[kDateTimeRem] [datetime] NULL ,
	[kAmount] [float] NULL
)
Insert @CollDataImport
SELECT  Q.CustomerId,Q.MeterId, Q.DateTimeRem,Q.AmountInCents/100.00
FROM CollDataImport As Q INNER JOIN
        (SELECT  CustomerId,MeterId, Max(DateTimeRem) As MaxDateTime
         FROM CollDataImport
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.DateTimeRem = T.MaxDateTime
Where Q.CustomerId = @CustomerId
and  Q.DateTimeRem < CURRENT_TIMESTAMP

/* GSMConnectionLogs*/


DECLARE @GSMConnectionLogs TABLE (
    [kCustomerId] [int] NULL ,
	[mMeterId] [int] NULL ,
	[mStartDateTime] [datetime] NULL ,
    [mEndDateTime] [datetime] NULL ,
	[mConnectionStatus] [int] NULL,
    [mStatusName] varchar(64)NULL
)
Insert @GSMConnectionLogs
SELECT  Q.CustomerId,Q.MeterId, Q.StartDateTime,Q.EndDateTime,Q.ConnectionStatus,X.StatusName
FROM GSMConnectionLogs As Q INNER JOIN
        (SELECT  CustomerId,MeterId, Max(StartDateTime) As MaxDateTime
         FROM GSMConnectionLogs
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.StartDateTime = T.MaxDateTime
INNER JOIN GSMConnectionStatus X
ON Q.ConnectionStatus = X.StatusId
Where Q.CustomerId = @CustomerId
and  Q.StartDateTime < CURRENT_TIMESTAMP
and  Q.EndDateTime < CURRENT_TIMESTAMP

/* Event logs*/
DECLARE @EventLogs TABLE (
    [eCustomerId] [int] NULL ,
	[eMeterId] [int] NULL ,
	[eEventDateTime] [datetime] NULL ,
    [eEventCode] [int] NULL ,
	[eEventSource] [int] NULL,
    [eTechnicianKeyID] varchar(64)NULL,
    [eEventDescVerbose] varchar(50) NULL
)
Insert @EventLogs
SELECT  Q.CustomerId, Q.MeterId, Q.EventDateTime, Q.EventCode,Q.EventSource, Q.TechnicianKeyID, X.EventDescVerbose
FROM EventLogs As Q INNER JOIN
        (SELECT  CustomerId,MeterId, Max(EventDateTime) As MaxDateTime
         FROM EventLogs
         GROUP BY CustomerId,MeterId) As T
        ON Q.CustomerId=T.CustomerId AND Q.MeterId=T.MeterId AND Q.EventDateTime = T.MaxDateTime
INNER JOIN EventCodes As X ON Q.CustomerId = X.CustomerId AND Q.EventCode = X.EventCode AND Q.EventSource = X.EventSource
Where Q.CustomerId = @CustomerId
and  Q.EventDateTime < CURRENT_TIMESTAMP

/* final output  */
if @LiveMeters = 0 /* live and non live meters */
begin
if @AreaId = -1 AND @MeterId = -1
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT OUTER JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT OUTER JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT OUTER JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT OUTER JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT OUTER JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT OUTER JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT OUTER JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT OUTER JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT OUTER JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT OUTER JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT OUTER JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId)
else if @AreaId  > -1 and @MeterId = -1
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,CollStatus.cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId
And dbo.Meters.AreaId = @AreaId)
else
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,CollStatus.cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId
And dbo.Meters.AreaId = @AreaId
And dbo.Meters.MeterId = @MeterId)
end

else if @LiveMeters = 1  /*  live meters only */
begin
if @AreaId = -1 AND @MeterId = -1
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT OUTER JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT OUTER JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT OUTER JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT OUTER JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT OUTER JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT OUTER JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT OUTER JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT OUTER JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT OUTER JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT OUTER JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT OUTER JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId
And dbo.Meters.MaxBaysEnabled >= 0)
else if @AreaId  > -1 and @MeterId = -1
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,CollStatus.cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId
And dbo.Meters.AreaId = @AreaId
And dbo.Meters.MaxBaysEnabled >= 0)
else
(SELECT  CustomerId,AreaId,MeterID,MeterName,Location,
    TransCash.tTransDateTime,TransCash.tAmount,
	TransCCard.pTransDateTime,TransCCard.pAmount,
	TransMPark.rTransDateTime,TransMPark.rAmount,
    ActAlarms.aEventCode,ActAlarms.aEventSource,ActAlarms.aTimeOfOccurrance,ActAlarms.aEventDescVerbose,
    HistAlarms.hEventCode,HistAlarms.hEventSource,HistAlarms.hTimeOfOccurrance,HistAlarms.hEventDescVerbose,
	CollStatus.cStatusDateTime,CollStatus.cCurrVoltDryBattery,CollStatus.cCurrVoltSolarBattery,CollStatus.cCurrVoltSolarPanel,CollStatus.cMinVoltDryBattery,CollStatus.cMinVoltSolarBattery,CollStatus.cMinVoltSolarPanel,CollStatus.cNetworkStrength,
	CollSumm.sCollDateTime,CollSumm.sAmount,
	CollCBR.dDateTimeRem,CollCBR.dAmount,
	CollDLR.kDateTimeRem,CollDLR.kAmount,
    ConnLogs.mStartDateTime,ConnLogs.mEndDateTime,ConnLogs.mConnectionStatus,ConnLogs.mStatusName,
    EvtLogs.eEventDateTime, EvtLogs.eEventCode,EvtLogs.eEventSource,EvtLogs.eTechnicianKeyID,EvtLogs.eEventDescVerbose
FROM Meters
LEFT JOIN @TransactionsCash TransCash ON dbo.Meters.MeterID = tMeterId
LEFT JOIN @TransactionsCCard TransCCard ON dbo.Meters.MeterID = pMeterId
LEFT JOIN @TransactionsMPark TransMPark ON dbo.Meters.MeterID = rMeterId
LEFT JOIN @ActiveAlarms ActAlarms ON dbo.Meters.MeterID = aMeterId
LEFT JOIN @HistoricalAlarms HistAlarms ON dbo.Meters.MeterID = hMeterId
LEFT JOIN @CollDataMeterStatus CollStatus ON dbo.Meters.MeterID = cMeterId
LEFT JOIN @CollDataSumm CollSumm ON dbo.Meters.MeterID = sMeterId
LEFT JOIN @CashBoxDataImport CollCBR ON dbo.Meters.MeterID = dMeterId
LEFT JOIN @CollDataImport CollDLR ON dbo.Meters.MeterID = kMeterId
LEFT JOIN @GSMConnectionLogs ConnLogs ON dbo.Meters.MeterID = mMeterId
LEFT JOIN @EventLogs EvtLogs ON dbo.Meters.MeterID = eMeterId
Where dbo.Meters.CustomerId = @CustomerId
And dbo.Meters.AreaId = @AreaId
And dbo.Meters.MeterId = @MeterId
And dbo.Meters.MaxBaysEnabled >= 0)
end
/* ORDER BY dbo.Meters.MeterId - does not have any effect. orders always by AreaId, MeterId*/
GO
/****** Object:  StoredProcedure [dbo].[sp_GenerateCollectionRunReport]    Script Date: 04/01/2014 22:08:03 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create PROC [dbo].[sp_GenerateCollectionRunReport]
As
Declare 
	cur_coll cursor for select CollectionRunId from [V_UnProcessed_CollRun]
	
	
Begin
	Declare
		@runid bigint
		,@startDate datetime
		,@endDate datetime
		,@collRunId bigint
		,@vendorid int 
		,@userid int
		,@days int
		,@cid int
		,@aid int
		,@mid int
		,@dollar2 int
		,@dollar1 int
		,@cent50 int
		,@cent25 int
		,@cent20 int
		,@cent10 int
		,@cent5 int
		,@cent1 int
		,@totalMeterAmt int
		,@totalAutoAmt int
		,@totalManualAmt int
		,@totalAutoMeter int
		,@totalMeterCnt int
		,@totalAutoCoinCnt int
		
	OPEN cur_coll 
	fetch next from cur_coll into @runid
	WHILE @@FETCH_STATUS = 0
	BEGIN
		Print 'Processing ' + convert(varchar,@runid)		
		
		select @startDate =  c.ActivationDate
		,@endDate = DATEADD(DAY,c.DaysBetweenCol,c.ActivationDate)
		,@cid = c.CustomerId
		from CollectionRun c
		where CollectionRunId = @runid
		
		--CashBoxDataImport--
		select @dollar2= sum(Dollar2Coins)
		,@dollar1 = SUM(dollar1coins)
		,@cent50 = SUM(cents50coins)
		,@cent20 = SUM(Cents20Coins)
		,@cent10 =SUM(cents10coins)
		,@totalAutoAmt =  SUM(amtauto * 100.00)
		,@totalManualAmt = SUM(amtmanual * 100.00)
		,@totalAutoMeter = count(distinct MeterId)
		from CashBoxDataImport
		where CustomerId = @cid
		and DateTimeRem between @startDate and @endDate
		
		
		set @totalAutoCoinCnt = @dollar2+@dollar1+@cent50+@cent20+@cent10
		
		--TransactionsCash--
		select 
		@totalMeterAmt = sum(AmountIncents)
		,@totalMeterCnt = COUNT(distinct meterid)
		from TransactionsCash t
		where t.TransDateTime  between @startDate and @endDate 
		and CustomerId = @cid 
		
		if (@totalMeterAmt is null) set @totalMeterAmt = 0
		if (@totalMeterCnt is null) set @totalMeterCnt = 0
		if (@totalAutoCoinCnt is null) set @totalAutoCoinCnt = 0
		if (@totalManualAmt is null) set @totalManualAmt = 0
		if (@totalAutoAmt is null) set @totalAutoAmt = 0
		
		-- Populate data 
		Insert into CollectionRunReport
		 ([CustomerId],[CollectionRunId],[CollectionDate],
		 [ChipAutoCoinType1Count],[ChipAutoCoinType2Count],[ChipAutoCoinType3Count],[ChipAutoCoinType4Count],[ChipAutoCoinType5Count],[ChipAutoCoinType6Count],[ChipAutoCoinType7Count],[ChipAutoCoinType8Count],
         [TotalMeterCashAmt],
         [TotalMeterCoinCount],[TotalMeterCount],
         [MeterCointype1Count],[MeterCointype2Count],[MeterCointype3Count],[MeterCointype4Count],[MeterCointype5Count],[MeterCointype6Count],[MeterCointype7Count],[MeterCointype8Count],
         [TotalManualCashAmt],[TotalManualCoinCount],[TotalManualMeterCount],
         [ProcessedTS]
         ,TotalByChipMeterCount,TotalByChipCoinCount
         ,[VendorId],[TotalByChip])
         values
         (@cid,@collRunId,@startDate,
         --CHIP AUTO
         @cent5,@cent10,@cent20,@cent25,@cent50,@dollar1,@dollar2,@cent1,
         @totalMeterAmt,
         @totalAutoCoinCnt,@totalMeterCnt,
         --METER (todo)
         @cent5,@cent10,@cent20,@cent25,@cent50,@dollar1,@dollar2,@cent1,
         @totalManualAmt,@totalAutoAmt,@totalMeterCnt,         
         @startDate,
         @totalAutoMeter,@totalAutoCoinCnt,
         @vendorid,@totalAutoAmt)
		
		fetch next from cur_coll into @runid
	END
	CLOSE cur_coll
	DEALLOCATE cur_coll
	Print 'Executed sp_GenerateCollectionRunReport'
End

--------------------------------------------------
-- SLA Target Time
--------------------------------------------------

--------------------------------------------------------------------------------------------------------------
--             THIS SCRIPT DEPENDS DATABASE OBJECTS CREATED BY NSC_Changes.sql FILE.
--
--
--
--------------------------------------------------------------------------------------------------------------
GO
/****** Object:  View [dbo].[CurrentMeterAmountsV]    Script Date: 04/01/2014 22:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CurrentMeterAmountsV] AS
SELECT			lct.CustomerID, 
				lct.AreaID, 
				lct.MeterID AS AssetID, 
				lct.MeterName AS AssetName, 
				lct.Location AS Street,  
				lct.Suburb, 
				lct.ZoneID, 
				(SELECT AreaName
                        FROM dbo.Areas
                        WHERE (AreaID = lct.AreaId2) AND (CustomerID = lct.CustomerID)) AS Area, 
				(SELECT ZoneName
                        FROM dbo.Zones
                        WHERE (ZoneId = lct.ZoneId) AND (customerID = lct.Customerid)) AS Zone, 
				(SELECT DemandZoneDesc FROM dbo.DemandZone WHERE DemandZoneId = lct.DemandZone) AS DemandArea, 
				(SELECT ISNULL(SUM(AmountInCents), 0) from dbo.TransactionsMPark WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS MPInCents,
				(SELECT COUNT(AmountInCents) from dbo.TransactionsMPark WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS MPTransactions, 
				(SELECT ISNULL(SUM(AmountInCents), 0) from dbo.TransactionsCreditCard WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS CCInCents, 
				(SELECT COUNT(AmountInCents) from dbo.TransactionsCreditCard WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS CCTransactions, 
				(SELECT ISNULL(SUM(AmountInCents), 0) from dbo.TransactionsCash WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS CashInCents, 
				(SELECT COUNT(AmountInCents) from dbo.TransactionsCash WHERE  lct.CustomerID = CustomerID AND lct.AreaID = AreaID AND lct.MeterID = MeterID AND TransDateTime >= lct.LastCollectionDateTime) AS CashTransactions
	from dbo.LastCollectionDateTimeV AS lct
			LEFT OUTER JOIN dbo.TransactionsCreditCard AS tc
				ON lct.CustomerID = tc.CustomerID 
				AND lct.AreaID = tc.AreaID 
				AND lct.MeterID = tc.MeterId 
			LEFT OUTER JOIN	dbo.TransactionsCash AS tch
				ON lct.CustomerID = tch.CustomerID 
				AND lct.AreaID = tch.AreaID 
				AND lct.MeterID = tch.MeterID 
				AND tch.MeterID = tc.MeterId 
			LEFT OUTER JOIN dbo.TransactionsMPark AS tmp 
				ON lct.CustomerID = tmp.CustomerID 
				AND lct.AreaID = tmp.AreaID 
				AND lct.MeterID = tmp.MeterID 
			GROUP BY lct.MeterID, lct.CustomerID, lct.AreaID, lct.AreaId2, lct.MeterName, lct.Location, lct.Suburb, lct.ZoneID, lct.LastCollectionDateTime, lct.DemandZone
GO
/****** Object:  StoredProcedure [dbo].[sp_Update_VersionProfileMeter]    Script Date: 04/01/2014 22:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_Update_VersionProfileMeter] 	
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@AssetFirmwareVersion varchar(255),
	@AssetSoftwareVersion varchar(255),
	@MPVFirmware varchar(255),
	@AssetPendingReasonId int,
	@CreateUserId int,
	@MeterGroup int
AS

	
BEGIN
	Print '--------------------------------------------------------------------------------'
	Print 'UPDATE and AUDIT VERSION PROFILE METER'
	if exists (select * from VersionProfileMeter v
				where v.CustomerId = @CustomerId
				and v.AreaId = @AreaId
				and v.MeterId = @MeterId)
	begin
		Print 'Exists and update'
		update v
				set v.HardwareVersion = Case when @AssetFirmwareVersion is null then v.HardwareVersion else @AssetFirmwareVersion end,
				v.SoftwareVersion =  Case when @AssetSoftwareVersion is null then v.SoftwareVersion else @AssetSoftwareVersion end,
				v.CommunicationVersion =  Case when @MPVFirmware is null then v.CommunicationVersion else @MPVFirmware end
				from VersionProfileMeter v
				where v.CustomerId = @CustomerId
				and v.AreaId = @AreaId
				and v.MeterId = @MeterId
	end else begin
		Print 'Not Exists and Insert'
		
		select @MeterGroup = m.MeterGroup
		from Meters m
		where m.CustomerID = @CustomerId and m.AreaID  = @AreaId and m.MeterId = @MeterId
		
		INSERT INTO [VersionProfileMeter]
           ([ConfigurationName]
           ,[HardwareVersion]
           ,[SoftwareVersion]
           ,[CommunicationVersion]
           ,[Version1]
           ,[Version2]
           ,[Version3]
           ,[Version4]
           ,[Version5]
           ,[Version6]
           ,[CustomerId]
           ,[AreaId]
           ,[MeterId]
           ,[MeterGroup]
           ,[SensorID]
           ,[GatewayID]
           )
		 VALUES
			   (
			   'NEW CONFIGURATION'
			   ,@AssetFirmwareVersion
			   ,@AssetSoftwareVersion
			   ,@MPVFirmware
			   ,null--[Version1]
			   ,null--[Version2]
			   ,null--[Version3]
			   ,null--[Version4]
			   ,null--[Version5]
			   ,null--[Version6]
			   ,@CustomerId
			   ,@AreaId
			   ,@MeterId
			   ,@MeterGroup
			   ,null--[SensorID]
			   ,null--[GatewayID]
			   )
	end 
	
				
			exec sp_Audit_VersionProfileMeter @Customerid,@AreaId,@MeterId,@AssetPendingReasonId,@CreateUserId
END
GO
/****** Object:  Table [dbo].[ParkingSpaceOccupancyAudit]    Script Date: 04/01/2014 22:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ParkingSpaceOccupancyAudit](
	[AuditID] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[ParkingSpaceOccupancyId] [bigint] NOT NULL,
	[LastStatus] [int] NOT NULL,
	[LastUpdatedTS] [datetime] NOT NULL,
	[RecCreationDate] [datetime] NULL,
	[ResultStatus] [varchar](64) NULL,
	[GCustomerid] [int] NULL,
	[GAreaId] [int] NULL,
	[GMeterId] [int] NULL,
	[DiagnosticData] [image] NULL,
 CONSTRAINT [PK_ParkingSpaceOccupancyAudit] PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[PAMTx]    Script Date: 04/01/2014 22:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PAMTx](
	[SequenceId] [int] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CustomerId] [int] NOT NULL,
	[MeterId] [int] NOT NULL,
	[Bay] [int] NOT NULL,
	[Expiry] [datetime] NOT NULL,
	[Duration] [int] NOT NULL,
	[TopUp] [bit] NOT NULL,
	[TransactionType] [int] NOT NULL,
	[AmountCent] [int] NOT NULL,
	[Received] [datetime] NOT NULL,
	[LastProcessed] [datetime] NULL,
	[LastResult] [int] NULL,
	[Attempt] [int] NOT NULL,
	[TRANSDATETIME] [datetime] NULL,
	[MaxReachedAt] [datetime] NULL,
	[TIMECREDIT] [datetime] NULL,
 CONSTRAINT [PK_PAMTx] PRIMARY KEY CLUSTERED 
(
	[SequenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[qTransactionsCreditCard]    Script Date: 04/01/2014 22:08:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qTransactionsCreditCard]
AS
SELECT     CustomerId, TransDateTime, BayNumber, AmountInCents, TimePaid, CardNumHash, Status, CreditCardType, ReceiptNo, AreaId, MeterId, 
                      TransactionsCreditCardId, 100 as TransType
FROM         dbo.TransactionsCreditCard
UNION ALL
SELECT     CustomerId, TransDateTime, BayNumber, AmountInCents, TimePaid, CardNumHash, 0 AS Status, CreditCardType, ReceiptNo, AreaId, MeterId, 
                      0 AS TransactionsCreditCardId, TransType
FROM         dbo.TransactionsBlacklist
GO
/****** Object:  View [dbo].[qTransactionsCk]    Script Date: 04/01/2014 22:08:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qTransactionsCk]
AS
SELECT     0 AS ReconFlags, CustomerId, AreaId, MeterId, TransDateTime, 1 AS PaymentType, 'Cash' AS PaymentTypeDesc, BayNumber AS BayNumVal, 
                      AmountInCents / 100.0 AS Amount, TimePaid AS TimePaidVal, 0 AS Status, '' AS StatusDesc, 0 AS ReceiptNo, '0' AS TransactionID, 0 AS CreditCardType,
                       '' AS CreditCardTypeDesc, 0 AS TransactionsCardId, '' AS Source, NULL AS AcquirerTransReference, '' AS PaymentTarget, null AS BatchId, 
                      null AS Reconciled
FROM         dbo.TransactionsCash
UNION ALL
SELECT     0 AS ReconFlags, dbo.TransactionsCreditCard.CustomerId, dbo.TransactionsCreditCard.AreaId, dbo.TransactionsCreditCard.MeterId, 
                      dbo.TransactionsCreditCard.TransDateTime, 2 AS PaymentType, 'Credit Card' AS PaymentTypeDesc, 
                      CASE WHEN dbo.TransactionsCreditCard.PaymentTarget = 'C' THEN dbo.TransactionsCreditCard.CashKeySrID ELSE dbo.TransactionsCreditCard.BayNumber
                       END AS BayNumVal, dbo.TransactionsCreditCard.AmountInCents / 100.0 AS Amount, 
                      CASE WHEN dbo.TransactionsCreditCard.PaymentTarget = 'C' THEN dbo.TransactionsCreditCard.CashKeyBalanceBefore ELSE dbo.TransactionsCreditCard.TimePaid
                       END AS TimePaidVal, dbo.TransactionsCreditCard.Status, dbo.TransactionStatus.Description AS StatusDesc, dbo.TransactionsCreditCard.ReceiptNo, 
                      '0' AS TransactionID, dbo.TransactionsCreditCard.CreditCardType, dbo.CreditCardTypes.Name AS CreditCardTypeDesc, 
                      dbo.TransactionsCreditCard.TransactionsCreditCardID AS TransactionsCardId, '' AS Source, dbo.TransactionsCreditCard.AcquirerTransReference, 
                      dbo.TransactionsCreditCard.PaymentTarget, dbo.TransactionsCreditCard.BatchId, dbo.TransactionsCreditCard.Reconciled
FROM         dbo.TransactionsCreditCard LEFT OUTER JOIN
                      dbo.TransactionStatus ON dbo.TransactionStatus.StatusID = dbo.TransactionsCreditCard.Status LEFT OUTER JOIN
                      dbo.CreditCardTypes ON dbo.CreditCardTypes.CreditCardType = dbo.TransactionsCreditCard.CreditCardType
UNION ALL
SELECT     0 AS ReconFlags, dbo.TransactionsSmartCard.CustomerId, dbo.TransactionsSmartCard.AreaId, dbo.TransactionsSmartCard.MeterId, 
                      dbo.TransactionsSmartCard.TransDateTime, 3 AS PaymentType, 'Smart Card' AS PaymentTypeDesc, 
                      dbo.TransactionsSmartCard.BayNumber AS BayNumVal, dbo.TransactionsSmartCard.AmountInCents / 100.0 AS Amount, 
                      dbo.TransactionsSmartCard.TimePaid AS TimePaidVal, dbo.TransactionsSmartCard.Status, TransactionStatus_1.Description AS StatusDesc, 
                      0 AS ReceiptNo, '0' AS TransactionID, dbo.TransactionsSmartCard.TransType AS CreditCardType, '' AS CreditCardTypeDesc, 
                      dbo.TransactionsSmartCard.TransactionsSmartCardID AS TransactionsCardId, '' AS Source, dbo.TransactionsSmartCard.AcquirerTransReference, 
                      '' AS PaymentTarget, null AS BatchId, null AS Reconciled
FROM         dbo.TransactionsSmartCard LEFT OUTER JOIN
                      dbo.TransactionStatus AS TransactionStatus_1 ON TransactionStatus_1.StatusID = dbo.TransactionsSmartCard.Status

UNION ALL

SELECT     0 AS ReconFlags, dbo.TransactionsCashKey.CustomerId, dbo.TransactionsCashKey.AreaId, dbo.TransactionsCashKey.MeterId, 
                      dbo.TransactionsCashKey.TransDateTime, 6 AS PaymentType, 'Cash Key' AS PaymentTypeDesc, 
                      dbo.TransactionsCashKey.BayNumber AS BayNumVal, dbo.TransactionsCashKey.AmountInCents / 100.0 AS Amount, 
                      dbo.TransactionsCashKey.TimePaid AS TimePaidVal, 0 as Status, ' ' AS StatusDesc, 
                      0 AS ReceiptNo, '0' AS TransactionID, ' ' AS CreditCardType, '' AS CreditCardTypeDesc, 
                      dbo.TransactionsCashKey.TransactionsCashKeyId AS TransactionsCardId, '' AS Source, dbo.TransactionsCashKey.MeterTransReference, 
                      '' AS PaymentTarget, null AS BatchId, null AS Reconciled
FROM         dbo.TransactionsCashKey  

UNION ALL
SELECT     CASE WHEN (MeterID IS NOT NULL AND ReinoCustomerID IS NOT NULL) THEN ' 3 ' ELSE CASE WHEN ReinoCustomerID IS NULL 
                      THEN ' 1 ' ELSE ' 2 ' END END AS ReconFlags, CASE WHEN CustomerID IS NULL THEN ReinoCustomerID ELSE CustomerID END AS CustomerId, 
                      CASE WHEN AreaID IS NULL THEN ReinoAreaID ELSE AreaID END AS AreaId, CASE WHEN MeterID IS NULL 
                      THEN ReinoMeterID ELSE MeterID END AS MeterId, CASE WHEN TransDateTime IS NULL 
                      THEN TransTimeStamp ELSE TransDateTime END AS TransDateTime, 4 AS PaymentType, 'MPark' AS PaymentTypeDesc, 
         CASE WHEN BayNumber IS NULL THEN BayNum ELSE BayNumber END AS BayNumVal, CASE WHEN AmountInCents IS NULL 
                      THEN TransAmtInCents / 100.0 ELSE AmountInCents / 100.00 END AS Amount, CASE WHEN TimePaid IS NULL 
                      THEN NumOfMins * 60 ELSE TimePaid END AS TimePaidVal, 0 AS Status, '' AS StatusDesc, 0 AS ReceiptNo, CASE WHEN TransactionID IS NULL 
                      THEN CAST(TransID AS nvarchar(20)) ELSE CAST(TransactionID AS nvarchar(20)) END AS TransactionID, 0 AS CreditCardType, 
                      '' AS CreditCardTypeDesc, 0 AS TransactionsCardId, CASE WHEN (MeterID IS NOT NULL AND ReinoCustomerID IS NOT NULL) 
                      THEN ' From Meter And Emporia ' ELSE CASE WHEN ReinoCustomerID IS NULL THEN ' From Meter ' ELSE ' From Emporia ' END END AS Source, NULL
                       AS AcquirerTransReference, '' AS PaymentTarget, null AS BatchId, null AS Reconciled
FROM         dbo.MParkImport FULL OUTER JOIN dbo.TransactionsMPark ON dbo.TransactionsMPark.TransactionID = dbo.MParkImport.TransID
GO
/****** Object:  View [dbo].[qTransactions]    Script Date: 04/01/2014 22:08:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[qTransactions]
AS
SELECT     0 AS ReconFlags, CustomerId, AreaId, MeterId, TransDateTime, 1 AS PaymentType, 'Cash' AS PaymentTypeDesc, BayNumber, 
                      AmountInCents / 100.0 AS Amount, TimePaid, 0 AS Status, '' AS StatusDesc, 0 AS ReceiptNo, '0' AS TransactionID, 0 AS CreditCardType, 
                      '' AS CreditCardTypeDesc, 0 AS TransactionsCardId, '' AS Source, NULL AS AcquirerTransReference, '' AS PaymentTarget
FROM         dbo.TransactionsCash
UNION ALL
SELECT     0 AS ReconFlags, dbo.TransactionsCreditCard.CustomerId, dbo.TransactionsCreditCard.AreaId, dbo.TransactionsCreditCard.MeterId, 
                      dbo.TransactionsCreditCard.TransDateTime, 2 AS PaymentType, 'Credit Card' AS PaymentTypeDesc, dbo.TransactionsCreditCard.BayNumber, 
                      dbo.TransactionsCreditCard.AmountInCents / 100.0 AS Amount, dbo.TransactionsCreditCard.TimePaid, dbo.TransactionsCreditCard.Status, 
                      dbo.TransactionStatus.Description AS StatusDesc, dbo.TransactionsCreditCard.ReceiptNo, '0' AS TransactionID, 
                      dbo.TransactionsCreditCard.CreditCardType, dbo.CreditCardTypes.Name AS CreditCardTypeDesc, 
                      dbo.TransactionsCreditCard.TransactionsCreditCardID AS TransactionsCardId, '' AS Source, dbo.TransactionsCreditCard.AcquirerTransReference, 
                      dbo.TransactionsCreditCard.PaymentTarget
FROM         dbo.TransactionsCreditCard LEFT OUTER JOIN
                      dbo.TransactionStatus ON dbo.TransactionStatus.StatusID = dbo.TransactionsCreditCard.Status LEFT OUTER JOIN
                      dbo.CreditCardTypes ON dbo.CreditCardTypes.CreditCardType = dbo.TransactionsCreditCard.CreditCardType
UNION ALL
SELECT     0 AS ReconFlags, dbo.TransactionsSmartCard.CustomerId, dbo.TransactionsSmartCard.AreaId, dbo.TransactionsSmartCard.MeterId, 
                      dbo.TransactionsSmartCard.TransDateTime, 3 AS PaymentType, 'Smart Card' AS PaymentTypeDesc, dbo.TransactionsSmartCard.BayNumber, 
                      dbo.TransactionsSmartCard.AmountInCents / 100.0 AS Amount, dbo.TransactionsSmartCard.TimePaid, dbo.TransactionsSmartCard.Status, 
                      TransactionStatus_1.Description AS StatusDesc, 0 AS TransType, '0' AS TransactionID, dbo.TransactionsSmartCard.TransType AS Expr1, 
                      '' AS CreditCardTypeDesc, dbo.TransactionsSmartCard.TransactionsSmartCardId AS TransactionsCardId, '' AS Source, 
                      dbo.TransactionsSmartCard.AcquirerTransReference, '' AS PaymentTarget
FROM         dbo.TransactionsSmartCard LEFT OUTER JOIN
                      dbo.TransactionStatus AS TransactionStatus_1 ON TransactionStatus_1.StatusID = dbo.TransactionsSmartCard.Status

UNION ALL


SELECT     0 AS ReconFlags, dbo.TransactionsCashKey.CustomerId, dbo.TransactionsCashKey.AreaId, dbo.TransactionsCashKey.MeterId, 
                      dbo.TransactionsCashKey.TransDateTime, 6 AS PaymentType, 'Cash Key' AS PaymentTypeDesc, dbo.TransactionsCashKey.BayNumber, 
                      dbo.TransactionsCashKey.AmountInCents / 100.0 AS Amount, dbo.TransactionsCashKey.TimePaid, ' ' as Status, 
                      ' ' AS StatusDesc, 0 AS TransType, '0' AS TransactionID, NULL as Expr1, 
                      '' AS CreditCardTypeDesc, dbo.TransactionsCashKey.TransactionsCashKeyId AS TransactionsCardId, '' AS Source, 
                      dbo.TransactionsCashKey.MeterTransReference, '' AS PaymentTarget
FROM         dbo.TransactionsCashKey 

UNION ALL
SELECT     CASE WHEN (MeterID IS NOT NULL AND ReinoCustomerID IS NOT NULL) THEN ' 3 ' ELSE CASE WHEN ReinoCustomerID IS NULL 
                      THEN ' 1 ' ELSE ' 2 ' END END AS ReconFlags, CASE WHEN CustomerID IS NULL THEN ReinoCustomerID ELSE CustomerID END AS CustomerId, 
                      CASE WHEN AreaID IS NULL THEN ReinoAreaID ELSE AreaID END AS AreaId, CASE WHEN MeterID IS NULL 
                      THEN ReinoMeterID ELSE MeterID END AS MeterId, CASE WHEN TransDateTime IS NULL 
                      THEN TransTimeStamp ELSE TransDateTime END AS TransDateTime, 4 AS PaymentType, 'MPark' AS PaymentTypeDesc, 
                      CASE WHEN BayNumber IS NULL THEN BayNum ELSE BayNumber END AS BayNumber, CASE WHEN AmountInCents IS NULL 
                      THEN TransAmtInCents / 100.0 ELSE AmountInCents / 100.00 END AS Amount, CASE WHEN TimePaid IS NULL 
                      THEN NumOfMins * 60 ELSE TimePaid END AS TimePaid, 0 AS Status, '' AS StatusDesc, 0 AS ReceiptNo, CASE WHEN TransactionID IS NULL 
                      THEN CAST(TransID AS nvarchar(16)) ELSE CAST(TransactionID AS nvarchar(16)) END AS TransactionID, 0 AS CreditCardType, 
                      '' AS CreditCardTypeDesc, 0 AS TransactionsCardId, CASE WHEN (MeterID IS NOT NULL AND ReinoCustomerID IS NOT NULL) 
                      THEN ' From Meter And Emporia ' ELSE CASE WHEN ReinoCustomerID IS NULL THEN ' From Meter ' ELSE ' From Emporia ' END END AS Source, NULL
                       AS AcquirerTransReference, '' AS PaymentTarget
FROM         dbo.MParkImport FULL OUTER JOIN
                      dbo.TransactionsMPark ON dbo.TransactionsMPark.TransactionID = dbo.MParkImport.TransID
GO
/****** Object:  View [dbo].[pv_CustomerTransactions]    Script Date: 04/01/2014 22:08:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[pv_CustomerTransactions]
AS
SELECT     t.TransactionsID AS TransactionId, t.TransDateTime AS DateTime, t.CustomerID, m.MeterName AS AssetName, m.MeterId AS AssetId, 
                      at.MeterGroupDesc AS AssetType, mm.AreaId2 AS AreaId, a.AreaName AS Area, t.BayNumber AS BayId, mm.ZoneId, z.ZoneName AS Zone, 
                      dz.DemandZoneDesc AS DemandArea, cg1.DisplayName AS Suburb, t.GatewayId, t.ParkingSpaceID AS SpaceId, m.Location AS Street, 
                      t.DiscountSchemeId AS DiscountSchemaId, t.AmountInCents AS AmountPaid, t.TxValue AS SpaceStatus, tt.TransactionTypeDesc AS TransactionType, 
                      ts.Description AS PaymentStatusType, t.TimeType1, t.TimeType2, t.TimeType3, t.TimeType4, t.TimeType5, cc.CardNumHash, t.SensorPaymentTransactionId, 
                      CAST(CASE WHEN t .TxValue IS NOT NULL AND t .SensorPaymentTransactionId IS NOT NULL THEN 1 ELSE 0 END AS bit) AS IsSensorTransaction, 
                      cc.Last4 AS CcLast4, t.TimePaid, cc.CreditCardType AS CardType
FROM         dbo.Transactions AS t LEFT OUTER JOIN
                      dbo.Meters AS m ON t.CustomerID = m.CustomerID AND t.MeterID = m.MeterId AND t.AreaID = m.AreaID LEFT OUTER JOIN
                      dbo.MeterMap AS mm ON t.CustomerID = mm.Customerid AND t.MeterID = mm.MeterId AND t.AreaID = mm.Areaid LEFT OUTER JOIN
                      dbo.CustomGroup1 AS cg1 ON cg1.CustomGroupId = mm.CustomGroup1 LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = m.DemandZone LEFT OUTER JOIN
                      dbo.Areas AS a ON a.AreaID = mm.AreaId2 AND a.CustomerID = mm.Customerid LEFT OUTER JOIN
                      dbo.Zones AS z ON z.ZoneId = mm.ZoneId AND z.customerID = mm.Customerid LEFT OUTER JOIN
                      dbo.TransactionType AS tt ON t.TransactionType = tt.TransactionTypeId LEFT OUTER JOIN
                      dbo.TransactionStatus AS ts ON t.TransactionStatus = ts.StatusID LEFT OUTER JOIN
                      dbo.TransactionsCreditCard AS cc ON cc.CustomerId = t.CustomerID AND cc.MeterId = t.MeterID AND cc.AreaId = t.AreaID AND cc.TransDateTime = t.TransDateTime AND
                       cc.ReceiptNo = t.ReceiptNo LEFT OUTER JOIN
                      dbo.AssetType AS at ON at.MeterGroupId = m.MeterGroup AND at.CustomerId = m.CustomerID
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
		Begin DesignProperties = 
		   Begin PaneConfigurations = 
			  Begin PaneConfiguration = 0
				 NumPanes = 4
				 Configuration = "(H (1[40] 4[20] 2[20] 3) )"
			  End
			  Begin PaneConfiguration = 1
				 NumPanes = 3
				 Configuration = "(H (1 [50] 4 [25] 3))"
			  End
			  Begin PaneConfiguration = 2
				 NumPanes = 3
				 Configuration = "(H (1 [50] 2 [25] 3))"
			  End
			  Begin PaneConfiguration = 3
				 NumPanes = 3
				 Configuration = "(H (4 [30] 2 [40] 3))"
			  End
			  Begin PaneConfiguration = 4
				 NumPanes = 2
				 Configuration = "(H (1 [56] 3))"
			  End
			  Begin PaneConfiguration = 5
				 NumPanes = 2
				 Configuration = "(H (2 [66] 3))"
			  End
			  Begin PaneConfiguration = 6
				 NumPanes = 2
				 Configuration = "(H (4 [50] 3))"
			  End
			  Begin PaneConfiguration = 7
				 NumPanes = 1
				 Configuration = "(V (3))"
			  End
			  Begin PaneConfiguration = 8
				 NumPanes = 3
				 Configuration = "(H (1[56] 4[18] 2) )"
			  End
			  Begin PaneConfiguration = 9
				 NumPanes = 2
				 Configuration = "(H (1 [75] 4))"
			  End
			  Begin PaneConfiguration = 10
				 NumPanes = 2
				 Configuration = "(H (1[66] 2) )"
			  End
			  Begin PaneConfiguration = 11
				 NumPanes = 2
				 Configuration = "(H (4 [60] 2))"
			  End
			  Begin PaneConfiguration = 12
				 NumPanes = 1
				 Configuration = "(H (1) )"
			  End
			  Begin PaneConfiguration = 13
				 NumPanes = 1
				 Configuration = "(V (4))"
			  End
			  Begin PaneConfiguration = 14
				 NumPanes = 1
				 Configuration = "(V (2))"
			  End
			  ActivePaneConfig = 0
		   End
		   Begin DiagramPane = 
			  Begin Origin = 
				 Top = 0
				 Left = 0
			  End
			  Begin Tables = 
			  End
		   End
		   Begin SQLPane = 
		   End
		   Begin DataPane = 
			  Begin ParameterDefaults = ""
			  End
			  Begin ColumnWidths = 9
				 Width = 284
				 Width = 1500
				 Width = 1500
				 Width = 1500
				 Width = 1500
				 Width = 1500
				 Width = 1500
				 Width = 1500
				 Width = 1500
			  End
		   End
		   Begin CriteriaPane = 
			  Begin ColumnWidths = 11
				 Column = 1440
				 Alias = 900
				 Table = 1170
				 Output = 720
				 Append = 1400
				 NewValue = 1170
				 SortType = 1350
				 SortOrder = 1410
				 GroupBy = 1350
				 Filter = 1350
				 Or = 1350
				 Or = 1350
				 Or = 1350
			  End
		   End
		End
		' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_CustomerTransactions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'pv_CustomerTransactions'
GO
/****** Object:  View [dbo].[meters_bays]    Script Date: 04/01/2014 22:08:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [dbo].[meters_bays] as
  select CustomerId,AreaId,MeterId,MIN(baynumber) baystart,MAX(baynumber) bayend from 
  (
  select distinct CustomerId,AreaId,MeterId,BayNumber from TransactionsCash
  union 
  select distinct CustomerId,AreaId,MeterId,BayNumber from TransactionsCreditCard)
  a group by  CustomerId,AreaId,MeterId
GO
/****** Object:  View [dbo].[CreditCardReconciliationV]    Script Date: 04/01/2014 22:08:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CreditCardReconciliationV] 
AS 
SELECT
	tcc.CustomerID,
	tcc.TransactionsCreditCardID,
	ta.TransAuditId,
	tcc.TransDateTime AS TransactionDateTime,
	DATEADD(MI,2,tcc.TransDateTime) AS TransactionAuditTime,
	t.TransactionsID,
	m.MeterName,
	m.MeterId AS MeterID,
	mm.AreaId2 AS AreaID,
	mm.ZoneId,
	m.Location AS Street,
	(SELECT AreaName
            FROM dbo.Areas
            WHERE (AreaID = mm.AreaId2) AND (CustomerID = t.CustomerID)) AS Area, 
    (SELECT ZoneName
            FROM dbo.Zones
            WHERE (ZoneId = mm.ZoneId) AND (customerID = t.Customerid)) AS Zone, 
	(SELECT DisplayName
            FROM dbo.CustomGroup1
            WHERE (mm.CustomGroup1 = CustomGroupId)) AS Suburb,
	dz.DemandZoneDesc AS DemandArea,
	tcc.ParkingSpaceId AS SpaceId,
	tcc.status,
	ts.Description AS PaymentStatusType,
	tcc.BatchId,
	(SELECT Name FROM dbo.CreditCardTypes WHERE (CreditCardType = tcc.CreditCardType)) AS CreditCardType,
	tcc.ReceiptNo,
	CASE tcc.Status WHEN 106 THEN (tcc.AmountInCents * -1) 
					WHEN 108 THEN (tcc.AmountInCents * -1) 
					WHEN 156 THEN (tcc.AmountInCents * -1) 
					ELSE tcc.AmountInCents END AS AmountPaidInCents,
	(CONVERT(varchar(8), DATEADD(SECOND, tcc.TimePaid, 0), 108)) AS TimePaid, 
	tar.TransAcquirerID,
	tcc.AcquirerTransReference,
	tar.AcquirerResponseDetail,
	tar.AcquirerResponseCode,
	(SELECT TransTypeDesc from TransType WHERE TransTypeId = t.transType) AS transType,
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeType1, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeType2, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeType3, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeType4, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeType5
FROM dbo.Transactions AS t
INNER JOIN TransactionsCreditCard AS tcc
	ON t.CustomerID = tcc.CustomerID 
	AND t.MeterID = tcc.MeterId 
	AND t.AreaID = tcc.AreaID 
	AND t.TransDateTime = tcc.TransDateTime 
	AND t.ReceiptNo = tcc.ReceiptNo
INNER JOIN dbo.Meters AS m
	ON t.CustomerID = m.CustomerID 
	AND t.MeterID = m.MeterId 
	AND t.AreaID = m.AreaID
INNER JOIN dbo.TransactionsAudit AS ta
	ON t.OriginalTxId = ta.TransAuditId
INNER JOIN dbo.MeterMap AS mm
	ON t.CustomerID = mm.CustomerID 
	AND t.MeterID = mm.MeterId 
	AND t.AreaID = mm.Areaid
LEFT OUTER JOIN dbo.TransactionsAcquirerResp AS tar
	ON tcc.AcquirerTransReference = tar.AcquirerTransactionRef 
	AND ta.TransAuditId = tar.TransAuditID
LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
	ON cg1.CustomGroupId = mm.CustomGroup1
LEFT OUTER JOIN dbo.DemandZone AS dz
	ON dz.DemandZoneId = m.DemandZone
LEFT OUTER JOIN dbo.TransactionStatus AS ts
	ON tcc.Status = ts.StatusID
GO
/****** Object:  View [dbo].[DailyFinancialTransactionV]    Script Date: 04/01/2014 22:08:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DailyFinancialTransactionV] AS 
SELECT
	t.TransactionsID, 
	t.TransDateTime AS DateTime, 
	t.CustomerID, 
	CASE m.MeterGroup
		WHEN 0 THEN m.MeterName
		WHEN 1 THEN m.MeterName
	END AS MeterName, 
	m.MeterId, 
	at.MeterGroupDesc AS MeterType, 
	mm.AreaId2, 
	a.AreaName AS Area, 
	t.BayNumber AS BayId, 
	t.ParkingSpaceId AS SpaceId, 
	t.TxValue AS SpaceStatus, 
	mm.ZoneId, 
	z.ZoneName AS Zone, 
	dz.DemandZoneDesc AS DemandArea, 
	cg1.DisplayName AS Suburb, 
	t.GatewayID AS GatewayId, 
	m.Location AS Street, 
	ds.SchemeName AS DiscountSchemeName, 
	t.AmountInCents AS AmountPaid, 
	t.TimePaid, 
	tt.TransactionTypeDesc AS PaymentType, 
	ts.Description AS TransactionStatus, 
	t.ReceiptNo AS ReceiptNumber, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType1) AND (ColumnMap = 'TimeType1')) AS TimeType1, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType2) AND (ColumnMap = 'TimeType2')) AS TimeType2, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType3) AND (ColumnMap = 'TimeType3')) AS TimeType3, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType4) AND (ColumnMap = 'TimeType4')) AS TimeType4, 
	(SELECT TimeTypeDesc FROM dbo.TimeType WHERE (TimeTypeId = t.TimeType5) AND (ColumnMap = 'TimeType5')) AS TimeType5, 
	t.TransDateTime AS TransactionTime, 
	ta.BatchId,
	et.EventTypeDesc AS EventClass,
	el.EventCode
FROM dbo.Transactions AS t
LEFT OUTER JOIN dbo.Meters AS m
	ON t.CustomerID = m.CustomerID 
	AND t.MeterID = m.MeterId 
	AND t.AreaID = m.AreaID
LEFT OUTER JOIN dbo.MeterMap AS mm
	ON t.CustomerID = mm.CustomerID 
	AND t.MeterID = mm.MeterId 
	AND t.AreaID = mm.Areaid
LEFT OUTER JOIN dbo.CustomGroup1 AS cg1
	ON cg1.CustomGroupId = mm.CustomGroup1
LEFT OUTER JOIN dbo.DemandZone AS dz
	ON dz.DemandZoneId = m.DemandZone
LEFT OUTER JOIN dbo.Areas AS a
	ON a.AreaID = mm.AreaId2 
	AND a.CustomerID = mm.Customerid
LEFT OUTER JOIN dbo.Zones AS z
	ON z.ZoneId = mm.ZoneId 
	AND z.customerID = mm.Customerid
LEFT OUTER JOIN dbo.TransactionType AS tt
	ON t.TransactionType = tt.TransactionTypeId
LEFT OUTER JOIN dbo.EventLogs AS el
	ON t.AreaID = el.AreaID 
	AND t.CustomerID = el.CustomerID 
	AND t.MeterId = el.MeterID 
	AND t.TransDateTime = el.EventDateTime
LEFT OUTER JOIN dbo.EventCodes AS ec
	ON el.CustomerID = ec.CustomerID 
	AND el.EventSource = ec.EventSource 
	AND el.EventCode = ec.EventCode
LEFT OUTER JOIN dbo.EventType AS et
	ON et.EventTypeId = ec.EventType
LEFT OUTER JOIN dbo.TransactionStatus AS ts
	ON t.TransactionStatus = ts.StatusID
LEFT OUTER JOIN dbo.TransactionsCreditCard AS cc
	ON cc.CustomerID = t.CustomerID 
	AND cc.MeterID = t.MeterId 
	AND cc.AreaID = t.AreaID 
	AND cc.TransDateTime = t.TransDateTime 
	AND cc.ReceiptNo = t.ReceiptNo
LEFT OUTER JOIN dbo.AssetType AS at
	ON at.MeterGroupId = m.MeterGroup 
	AND at.CustomerId = m.CustomerID
LEFT OUTER JOIN dbo.TransactionsAudit AS ta 
	ON t.OriginalTxId = ta.TransAuditId
LEFT OUTER JOIN dbo.DiscountScheme AS ds 
	ON t.CustomerID = ds.CustomerId 
	AND t.DiscountSchemeId=ds.DiscountSchemeId
WHERE tt.TransactionTypeDesc IN ('Credit Card', 'Smart Card', 'Cash', 'Pay By Phone')
GO
/****** Object:  StoredProcedure [dbo].[sp_AssetPending]    Script Date: 04/01/2014 22:08:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_AssetPending] 
AS	
DECLARE
	--- METERS ----
	@AssetPendingId bigint,
	@CustomerId int,
	@AreaId int,
	@MeterId int,
	@CreateUserId int,
	@RecordCreateDateTime DateTime,
	@RecordMigrationDateTime DateTime,
	@AssetId bigint,
	@AssetType int,
	@AssetName Varchar(500),
	@AssetModel int,
	@NextPreventativeMaintenance DateTime,
	@OperationalStatus int,
	@Latitude float,
	@Longitude float,
	@PhoneNumber varchar(100),
	@SpaceCount int,
	@DemandStatus int,
	@WarrantyExpiration DateTime,
	@DateInstalled DateTime,
	@LocationMeters Varchar(500),
	@LastPreventativeMaintenance DateTime,
	@AssetState int,
	@OperationalStatusTime DateTime,
	------- METER MAP ----
	@MeterMapAreaId2 int,
	@MeterMapZoneId int,
	@MeterMapSuburbId int,
	@MeterMapCollectionRoute int,
	@MeterMapMaintenanceRoute int,	
	----- SENSORS ----
	@LocationSensor Varchar(255),
	@ParkingSpaceId bigint,
	@PrimaryGateway bigint,
	@SecondaryGateway bigint,
	---- GATEWAYS ---
	@LocationGateway Varchar(255),
	--- CASHBOX ----
	@CashboxLocationTypeId int,
	---- VERSION PROFILE -----
	@AssetFirmwareVersion Varchar(255),
	@AssetSoftwareVersion Varchar(255),
	@MPVFirmware Varchar(255),
	----- PARKING SPACE ---
	@DisplaySpaceNum Varchar(255),
	--------------
	@AssetPendingReasonId int	
	--
	,@MMGatewayId int
	,@MMSensorId int
	,@MMCashBoxId int
BEGIN


Declare 
		cur cursor 
		STATIC
		for SELECT [AssetPendingId]
			,[CustomerId]
			,[AreaId]
			,[MeterId]
			,[CreateUserId]
			,[RecordCreateDateTime]
			,[RecordMigrationDateTime]
			,[AssetId]
			,[AssetType]
			,[AssetName]
			,[AssetModel]
			,[NextPreventativeMaintenance]
			,[OperationalStatus]
			,[Latitude]
			,[Longitude]
			,[PhoneNumber]
			,[SpaceCount]
			,[DemandStatus]
			,[WarrantyExpiration]
			,[DateInstalled]
			,[LocationMeters]
			,[LastPreventativeMaintenance]
			,[AssetState]
			,[OperationalStatusTime]
			,[AssetPendingReasonId]
			,MeterMapAreaId2
			,MeterMapZoneId
			,MeterMapSuburbId
			,MeterMapCollectionRoute
			,MeterMapMaintenanceRoute	
			,[AssetFirmwareVersion]
			,[AssetSoftwareVersion]
			,[MPVFirmware]
			----- SENSOR
			,[LocationSensors]
			,[AssocidateSpaceId]
			,[PrimaryGateway]
			,SecondaryGateway
			--- GATEAWYS ----
			,LocationGateway
			----  CASH BOX
			,CashboxLocationId
			------ PARKINGSPACE
			,DisplaySpaceNum
			 FROM [AssetPending] 
			 where (RecordMigrationDateTime < GETDATE())
			 --and AssetType in (0,1) --SSM and MSM
			 --and migratedTS is null
			
	
	
	OPEN cur 
	Print 'Rows = ' + convert(Varchar,@@Cursor_Rows)
		fetch next from cur into @AssetPendingId,
									@CustomerId,
									@AreaId,
									@MeterId,
									@CreateUserId,
									@RecordCreateDateTime,
									@RecordMigrationDateTime,
									@AssetId,
									@AssetType,
									@AssetName,
									@AssetModel,
									@NextPreventativeMaintenance,
									@OperationalStatus,
									@Latitude,
									@Longitude,
									@PhoneNumber,
									@SpaceCount,
									@DemandStatus,
									@WarrantyExpiration,
									@DateInstalled,
									@LocationMeters,
									@LastPreventativeMaintenance,
									@AssetState,
									@OperationalStatusTime,
									@AssetPendingReasonId,	
									@MeterMapAreaId2,
									@MeterMapZoneId,
									@MeterMapSuburbId,
									@MeterMapCollectionRoute,
									@MeterMapMaintenanceRoute,
									@AssetFirmwareVersion,
									@AssetSoftwareVersion,
									@MPVFirmware,
									@LocationSensor,
									@ParkingSpaceId,
									@PrimaryGateway,
									@SecondaryGateway,
									@LocationGateway,
									@CashboxLocationTypeId,
									@DisplaySpaceNum
		WHILE @@FETCH_STATUS = 0
		BEGIN
			Print 'C/A/M = ' + convert(varchar,@Customerid) + '/' + convert(varchar,@Areaid) + '/' +  convert(varchar,@MeterId) 
			
			exec sp_util_PrintNum '@AssetId', @AssetId
			exec sp_util_PrintNum '@AssetType', @AssetType
			
			BEGIN TRY
				
				if @AssetType in (0,1,2,10,13) begin
					Print '-------------------------------------------------------------------------------------------------'
					Print 'UPDATE and AUDIT METERS'
					Print '-------------------------------------------------------------------------------------------------'
					
					exec sp_InsertOrUpdateMeter
						@AssetPendingReasonId,
						@CustomerId,
						@AreaId,
						@MeterId,
						@CreateUserId,
						@RecordCreateDateTime,
						@RecordMigrationDateTime,
						@AssetId,
						@AssetType,
						@AssetName,
						@AssetModel,
						@NextPreventativeMaintenance,
						@OperationalStatus,
						@Latitude,
						@Longitude ,
						@PhoneNumber,
						@SpaceCount,
						@DemandStatus,
						@WarrantyExpiration,
						@DateInstalled,
						@LocationMeters,
						@LastPreventativeMaintenance,
						@AssetState,
						@OperationalStatusTime
				end 
				
				if @AssetType in  (10,13) begin
					Print '-------------------------------------------------------------------------------------------------'
					Print 'UPDATE and AUDIT SENSORS/GATEWAY'
					Print '-------------------------------------------------------------------------------------------------'
					if @AssetType = 10 begin
						Print 'Sensor'				
						exec sp_InsertUpdate_Sensors @CustomerId,@AreaId,@MeterId,@AssetId,@AssetPendingReasonId,@CreateUserId,
										@Latitude,@Longitude,@LocationSensor,@ParkingSpaceId,@PrimaryGateway,@SecondaryGateway,
										@AssetType,@AssetName,@AssetModel,@AssetState,@OperationalStatus,@OperationalStatusTime,@WarrantyExpiration,
										@LastPreventativeMaintenance,@NextPreventativeMaintenance,@DemandStatus,@DateInstalled
						
						-- PRIMARY Gateway ---
						exec sp_Update_Audit_SensorMapping @CustomerId,@AssetId,@ParkingspaceId,
															@PrimaryGateway,1,
															@AssetPendingReasonId,@CreateUserId				
															
						-- SECONDARY Gateway ---
						exec sp_Update_Audit_SensorMapping @CustomerId,@AssetId,@ParkingspaceId,
															@SecondaryGateway,0,
															@AssetPendingReasonId,@CreateUserId		
															
					end else if @AssetType = 13 begin
						Print 'Gateway'
						exec sp_InsertUpdate_Gateways 	@AssetPendingReasonId ,	@CustomerId ,	@AreaId ,	@MeterId ,	@AssetId ,	@CreateUserId ,
												@AssetName,	@Latitude,	@Longitude,	@LocationGateway,	@AssetType ,	@AssetModel ,	@AssetState ,	@DateInstalled ,
												@OperationalStatus ,	@OperationalStatusTime ,	@WarrantyExpiration ,	@LastPreventativeMaintenance ,	@NextPreventativeMaintenance ,	@DemandStatus 
						
					end 
				end 
				
				if @AssetType in (11) begin
					exec sp_InsertUpdate_CashBox 	
							@CustomerId,
							@AreaId,
							@MeterId,
							@AssetId,
							@AssetPendingReasonId,
							@CreateUserId,	
							@AssetType,
							@AssetName,
							@AssetModel,
							@NextPreventativeMaintenance,
							@OperationalStatus,
							@WarrantyExpiration ,
							@DateInstalled ,
							@LastPreventativeMaintenance ,
							@AssetState,
							@OperationalStatusTime ,
							@CashboxLocationTypeId		
				end 
				
				if @AssetType in (20) begin
					Print '-------------------------------------------------------------------------------------------------'
					Print 'UPDATE and AUDIT PARKING SPACES'
					Print '-------------------------------------------------------------------------------------------------'
					
					
					
					exec sp_InsertUpdate_ParkingSpaces @Customerid,@AreaId,@MeterId,@AssetId,
														@latitude,@longitude,@DateInstalled,@Demandstatus,@DisplaySpaceNum
														,@AssetPendingReasonId,@CreateUserId
						
					exec sp_Audit_ParkingSpaces @Customerid,@AssetId,@AssetPendingReasonId,@CreateUserId				
	 				
				end
				
				if @AssetType in (0,1,2,10,13,20) begin
					-------------------------------------------------------------------------------------------------
					--                    UPDATE and AUDIT METERMAP for SENSORS
					-------------------------------------------------------------------------------------------------					
					set @MMCashBoxId = null
					set @MMGatewayId = null 
					set @MMSensorId= null
					
					if @AssetType = 10 set @MMSensorId = @AssetId
					if @AssetType = 13 set @MMGatewayId = @AssetId
					if @AssetType = 11 set @MMCashBoxId = @AssetId
					
					
					exec sp_InsertUpdate_MeterMap @CustomerId,@AreaId,@MeterId
								,@MeterMapZoneId,@MeterMapAreaId2,@MeterMapCollectionRoute,@MeterMapSuburbId,@MeterMapMaintenanceRoute
								,@MMGatewayId,@MMSensorId,@MMCashboxId
								,@AssetPendingReasonId,@CreateUserId
				end 
				
				if @AssetType in (0,1,2,10,13) begin
					-------------------------------------------------------------------------------------------------
					--                    UPDATE and AUDIT VersionProfile
					-------------------------------------------------------------------------------------------------				
					
					exec sp_Update_VersionProfileMeter 	
								@CustomerId,@AreaId,@MeterId,
								@AssetFirmwareVersion,@AssetSoftwareVersion,@MPVFirmware,
								@AssetPendingReasonId,@CreateUserId
								,@AssetType						
				end
				
				-------------------------------------------------------------------------------------------------
				--  FINALISE
				-------------------------------------------------------------------------------------------------
				Print 'Finalising'
				Delete AssetPending
					where AssetPendingId = @AssetPendingId
			
			
			END TRY
			BEGIN CATCH
			
				Print 'ERROR:' + Error_Message() 
				Update AssetPending
					set ErrorMessage = Error_Message() 
					where AssetPendingId = @AssetPendingId
			END CATCH
			
			
			
			-------------------------------------------------------------------------------------------------
			--                    LOOP
			-------------------------------------------------------------------------------------------------
			fetch next from cur into @AssetPendingId,
							@CustomerId,
							@AreaId,
							@MeterId,
							@CreateUserId,
							@RecordCreateDateTime,
							@RecordMigrationDateTime,
							@AssetId,
							@AssetType,
							@AssetName,
							@AssetModel,
							@NextPreventativeMaintenance,
							@OperationalStatus,
							@Latitude,
							@Longitude,
							@PhoneNumber,
							@SpaceCount,
							@DemandStatus,
							@WarrantyExpiration,
							@DateInstalled,
							@LocationMeters,
							@LastPreventativeMaintenance,
							@AssetState,
							@OperationalStatusTime,
							@AssetPendingReasonId,
							@MeterMapAreaId2,
							@MeterMapZoneId,
							@MeterMapSuburbId,
							@MeterMapCollectionRoute,
							@MeterMapMaintenanceRoute,
							@AssetFirmwareVersion,
							@AssetSoftwareVersion,
							@MPVFirmware,
							@LocationSensor,
							@ParkingSpaceId,
							@PrimaryGateway,
							@SecondaryGateway,
							@LocationGateway,
							@CashboxLocationTypeId,
							@DisplaySpaceNum
		END 
	CLOSE cur
	DEALLOCATE cur	

	
		
END
GO
/****** Object:  StoredProcedure [dbo].[spResubmitReport]    Script Date: 04/01/2014 22:08:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
----RESUBMIT CCARD TRANSACTIONS
CREATE Procedure [dbo].[spResubmitReport]
	@CustomerID int,
	@startDate datetime,
	@endDate datetime
As
	declare @Trans table (
		TransactionsCreditCardID bigint,
		NumAttempts int)

	declare @Attempts table (
		TransactionsCreditCardID bigint,
		ResultDateTime datetime,
		Status int,
		NumAttempts int)

  -- Make @Trans. This is a list of transactions in the date range and customer.
  -- Only reference this list in future
	INSERT @Trans
	SELECT DISTINCT a.TransactionsCreditCardID, 0 AS Cnt FROM CreditCardAttempts a
	LEFT OUTER JOIN TransactionsCreditCard
	ON a.TransactionsCreditCardID = TransactionsCreditCard.TransactionsCreditCardID
	wHERE a.ResultDateTime < @endDate and a.ResultDateTime > @startDate AND CustomerId = @CustomerId

  -- Set initial Attempts for each value in #Trans
	declare @InitTrans table (
		initTransactionsCreditCardID bigint,
		InitNumAttempts int)

	Insert @InitTrans
	SELECT TransactionsCreditCardID, COUNT(TransactionsCreditCardID) FROM CreditCardAttempts
	WHERE ResultDateTime < @startDate AND
	TransactionsCreditCardID In
		(Select TransactionsCreditCardID from @Trans)
	GROUP BY TransactionsCreditCardID

	UPDATE @Trans
	SET NumAttempts = InitNumAttempts
	FROM @InitTrans
	WHERE TransactionsCreditCardID=initTransactionsCreditCardID

  -- Copy CreditCardAttempts into @Attempts table, but counting NumAttempts column
	DECLARE rt_cursor CURSOR FOR
	Select TransactionsCreditCardID,ResultDateTime,Status,0 from CreditCardAttempts 
	where ResultDateTime < @endDate and
		ResultDateTime > @startDate AND
		TransactionsCreditCardID In (Select TransactionsCreditCardID from @Trans)
	Order by ResultDateTIme
	OPEN rt_cursor

	DECLARE @TransactionsCreditCardID bigint,
	        @ResultDateTime datetime,
	        @Status int,
	        @NumAttempts int

	FETCH NEXT FROM rt_cursor INTO @TransactionsCreditCardID,@ResultDateTime,@Status,@NumAttempts

	WHILE @@FETCH_STATUS = 0
	 BEGIN
	  UPDATE @Trans
	  SET NumAttempts = NumAttempts + 1,
	      @NumAttempts = NumAttempts + 1
	  WHERE TransactionsCreditCardID=@TransactionsCreditCardID

	  INSERT @Attempts VALUES (@TransactionsCreditCardID,@ResultDateTime, @Status,@NumAttempts)
	  FETCH NEXT FROM rt_cursor INTO @TransactionsCreditCardID,@ResultDateTime,@Status,@NumAttempts
	 END

	CLOSE rt_cursor
	DEALLOCATE rt_cursor

  -- Now display @Attempts table using Resubmit query...
	SELECT
                           SUM(CASE WHEN (Status >= 3  AND Status <=4 AND NumAttempts >=1 AND NumAttempts <= 3) THEN 1 ELSE 0 END) AS TotalUploads,
                           SUM(CASE WHEN (Status >= 3  AND Status <=4 AND NumAttempts = 1) THEN 1 ELSE 0 END) AS NumUploads1,
		SUM(CASE WHEN (Status = 3 AND NumAttempts = 1) THEN 1 ELSE 0 END) AS NumAccepts1,
		SUM(CASE WHEN (Status = 4 AND NumAttempts = 1) THEN 1 ELSE 0 END) AS NumDeclines1,
                           SUM(CASE WHEN (Status >= 3  AND Status <=4 AND NumAttempts = 2) THEN 1 ELSE 0 END) AS NumUploads2,
		SUM(CASE WHEN (Status = 3 AND NumAttempts = 2) THEN 1 ELSE 0 END) AS NumAccepts2,
		SUM(CASE WHEN (Status = 4 AND NumAttempts = 2) THEN 1 ELSE 0 END) AS NumDeclines2,
                           SUM(CASE WHEN (Status >= 3  AND Status <=4 AND NumAttempts = 3) THEN 1 ELSE 0 END) AS NumUploads3,
		SUM(CASE WHEN (Status = 3 AND NumAttempts = 3) THEN 1 ELSE 0 END) AS NumAccepts3,
		SUM(CASE WHEN (Status = 4 AND NumAttempts = 3) THEN 1 ELSE 0 END) AS NumDeclines3,
		CONVERT(datetime, DATEDIFF([day], 0, ResultDateTime)) AS UploadDate
	FROM @Attempts
	GROUP BY CONVERT(datetime, DATEDIFF([day], 0, ResultDateTime))
return
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_ParkingSpaceOccunpancy]    Script Date: 04/01/2014 22:08:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_ParkingSpaceOccunpancy] 
	@spaceid bigint
	,@status int
	,@ts datetime
	,@gcid int
	,@gaid int
	,@gmid int	
	,@diag image
	,@currentTs datetime
	,@psoId bigint	output
	,@result int output
AS
DECLARE
	@existingTS datetime 
	,@errmsg varchar(2000)
	,@resultMsg varchar(100)
BEGIN
	set @result = 0
	set @errmsg = null
	
	select @psoid = ParkingSpaceOccupancyId from ParkingSpaceOccupancy where ParkingSpaceId = @spaceid
	
	if (@ts > @currentTs) begin
		set @result = 4		
		set @errmsg = 'Meter Time ' + convert(varchar,@ts) + ' in future. Current time= ' + convert(varchar,@currentTs)
		set @resultMsg = 'FUTURE'
		print @errmsg	
	end 
	else begin
		if (@psoId is null) begin
			insert into ParkingSpaceOccupancy (ParkingSpaceId,LastStatus,LastUpdatedTS,GCustomerid,GAreaId,GMeterId,DiagnosticData) 
			values (@spaceid,@status,@ts,@gcid,@gaid,@gmid,@diag)
			set @psoId  = SCOPE_IDENTITY()
			set @result = 1
			set @resultMsg = 'INSERT'
			print 'Inserted ' + convert(varchar,@psoid)
		end
		else begin
			select @existingTS = LastUpdatedTS from ParkingSpaceOccupancy where ParkingSpaceOccupancyId = @psoId
			if (@ts > @existingTS) begin
				update ParkingSpaceOccupancy 
				set LastStatus = @status,LastUpdatedTS=@ts 
				,GCustomerid=@gcid,GAreaId=@gaid,GMeterId=@gmid
				,DiagnosticData=@diag
				where ParkingSpaceId = @spaceid
				set @result = 2
				set @resultMsg = 'UPDATE'
				print 'Updated ' + convert(varchar,@psoid)
			end 
			else begin 
				set @result = 3
				set @resultMsg = 'LATE'
				print 'Late event'
			end 
		end
	end 	
	
	print 'PSOID ' + convert(varchar,@psoid)
	
	if (@psoId is not null) begin
		print 'Auditing ' + convert(varchar,@psoId) + ' result=' + convert(varchar,@result)
		insert into ParkingSpaceOccupancyAudit 
		(ParkingSpaceOccupancyId,LastStatus,LastUpdatedTS,RecCreationDate,ResultStatus,GCustomerid,GAreaId,GMeterId,DiagnosticData)
		values
		(@psoId,@status,@ts,@currentTs,@resultMsg,@gcid,@gaid,@gmid,@diag)		
	end else begin
		set @errmsg = '@psoId is null. ' + @resultMsg
		RAISERROR (@errmsg,16,1)		
	end 
	

	if (@errmsg is not null)begin
		RAISERROR (@errmsg,16,1)		
	end 
	
	print 'done ' + convert(varchar,@psoid)
END
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_SensorStatus]    Script Date: 04/01/2014 22:08:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_SensorStatus]
	-- Add the parameters for the stored procedure here
	 @cid int
	,@aid int
	,@mid int
	,@bay int
	,@time int
	,@status int
	,@gcid int =1
	,@gaid int=1
	,@gmid int=1
	,@diag image = NULL
	,@currentTs datetime	
	,@result int OUTPUT
AS
Declare
	 @spaceid bigint
	,@meterTS datetime
	,@psoid bigint	
	,@errmsg varchar(255)
BEGIN
	
	
	EXEC sp_UDP_getParkingSpaceID @cid,@aid,@mid,@bay,@spaceid OUTPUT;
	
	
	if (@spaceid is null)begin
		set @errmsg = 'Space ID not found found for' + convert(varchar,@cid) + '/' +  CONVERT(varchar,@aid) + '/' + CONVERT(varchar,@mid) + ':' + CONVERT(varchar,@bay)
		RAISERROR (@errmsg,16,1)		
	end 
	
	
	-- Vacant Status : (0 in UDP, 2 in DB)
	-- Occupied : same for UDP and DB
	if @status = 0 set @status=2
	
	set @meterTS = DATEADD(second,@time,'2000/01/01')
	
	EXEC sp_UDP_ParkingSpaceOccunpancy 
	@spaceid,@status,@meterTS,
	@gcid,@gaid,@gmid,@diag,
	@currentTs,
	@psoid OUTPUT,@result OUTPUT;

	print 'executed to the end '  + convert(varchar,@result)
	return 1;
	
END
GO
/****** Object:  StoredProcedure [dbo].[spGetCardNumForSalt]    Script Date: 04/01/2014 22:08:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetCardNumForSalt]
 	@CustomerId int,
	@CreditCardType int,
    @CardNum varchar(40),
    @FromDateTime datetime,
    @ToDateTime datetime,
    @FromToDateTime datetime,
    @Type int,
    @HashTemp varchar(8000) 
AS
if @Type = 1
begin
  SELECT a.CustomerId, a.TransDateTime, c.AreaName, b.MeterName, a.TransactionsCreditCardId, a.TransType,
       b.Location,a.BayNumber, a.AmountInCents/100.00 AS Amount, a.TimePaid,
       a.CardNumHash, a.Status,e.Description, d.Name as CreditCardTypeDesc,
       a.CreditCardType,a.ReceiptNo,a.AreaId, a.MeterId,d.Name,
       COALESCE ((SELECT MAX(f.ResultDateTime) FROM  CreditCardAttempts f
       WHERE f.TransactionsCreditCardID = a.TransactionsCreditCardId),0) As UploadTimeBatch, 
       COALESCE ((SELECT MAX(f.TransAuditDate) FROM  TransactionsAudit f
       left join TransactionsAcquirerResp r on r.TransAuditID = f.TransAuditId
       WHERE f.TransactionId = a.TransactionsCreditCardId),0) As UploadTimeOnline
       FROM qTransactionsCreditCard a INNER JOIN Meters b ON a.MeterId = b.MeterID
       AND a.AreaId = b.AreaID
       AND a.CustomerId = b.CustomerID
       INNER JOIN Areas c ON b.AreaID = c.AreaID
       AND b.CustomerID = c.CustomerID
       INNER JOIN CreditCardTypes d ON a.CreditCardType = d.CreditCardType
       INNER JOIN TransactionStatus e ON a.Status = e.StatusId
       WHERE (@CreditCardType <> -1 AND a.CustomerID = @CustomerId
       AND a.CreditCardType = @CreditCardType
       AND a.CardNumHash = @CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       OR  ( a.CustomerID = @CustomerId
       AND @CreditCardType = -1
       AND a.CardNumHash =@CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       ORDER BY a.TransDateTime DESC  
end

if @Type = 2
begin
  SELECT a.CustomerId, a.TransDateTime, c.AreaName, b.MeterName, a.TransactionsCreditCardId, a.TransType,
       b.Location,a.BayNumber, a.AmountInCents/100.00 AS Amount, a.TimePaid,
       a.CardNumHash, a.Status,e.Description, d.Name as CreditCardTypeDesc,
       a.CreditCardType,a.ReceiptNo,a.AreaId, a.MeterId,d.Name,
       COALESCE ((SELECT MAX(f.ResultDateTime) FROM  CreditCardAttempts f
       WHERE f.TransactionsCreditCardID = a.TransactionsCreditCardId),0) As UploadTimeBatch, 
       COALESCE ((SELECT MAX(f.TransAuditDate) FROM  TransactionsAudit f
       left join TransactionsAcquirerResp r on r.TransAuditID = f.TransAuditId
       WHERE f.TransactionId = a.TransactionsCreditCardId),0) As UploadTimeOnline
       FROM qTransactionsCreditCard a INNER JOIN Meters b ON a.MeterId = b.MeterID
       AND a.AreaId = b.AreaID
       AND a.CustomerId = b.CustomerID
       INNER JOIN Areas c ON b.AreaID = c.AreaID
       AND b.CustomerID = c.CustomerID
       INNER JOIN CreditCardTypes d ON a.CreditCardType = d.CreditCardType
       INNER JOIN TransactionStatus e ON a.Status = e.StatusId
       WHERE (@CreditCardType <> -1 AND a.CustomerID = @CustomerId
       AND a.CreditCardType = @CreditCardType
       AND a.CardNumHash in (Select Item from SplitString(@HashTemp,',') union select @CardNum )
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       OR  ( a.CustomerID = @CustomerId
       AND @CreditCardType = -1
       AND a.CardNumHash in (Select Item from SplitString(@HashTemp,',') union select @CardNum)
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       ) ORDER BY a.TransDateTime DESC
        
end

if @Type = 3
begin
SELECT a.CustomerId, a.TransDateTime, c.AreaName, b.MeterName, a.TransactionsCreditCardId, a.TransType,
       b.Location,a.BayNumber, a.AmountInCents/100.00 AS Amount, a.TimePaid,
       a.CardNumHash, a.Status,e.Description, d.Name as CreditCardTypeDesc,
       a.CreditCardType,a.ReceiptNo,a.AreaId, a.MeterId,d.Name,
       COALESCE ((SELECT MAX(f.ResultDateTime) FROM  CreditCardAttempts f
       WHERE f.TransactionsCreditCardID = a.TransactionsCreditCardId),0) As UploadTimeBatch, 
       COALESCE ((SELECT MAX(f.TransAuditDate) FROM  TransactionsAudit f
       left join TransactionsAcquirerResp r on r.TransAuditID = f.TransAuditId
       WHERE f.TransactionId = a.TransactionsCreditCardId),0) As UploadTimeOnline
       FROM qTransactionsCreditCard a INNER JOIN Meters b ON a.MeterId = b.MeterID
       AND a.AreaId = b.AreaID
       AND a.CustomerId = b.CustomerID
       INNER JOIN Areas c ON b.AreaID = c.AreaID
       AND b.CustomerID = c.CustomerID
       INNER JOIN CreditCardTypes d ON a.CreditCardType = d.CreditCardType
       INNER JOIN TransactionStatus e ON a.Status = e.StatusId
       WHERE (@CreditCardType <> -1 AND a.CustomerID = @CustomerId
       AND a.CreditCardType = @CreditCardType
       AND a.CardNumHash in (Select Item from SplitString(@HashTemp,',') union select @CardNum)
       AND a.TransDateTime >= @FromToDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       OR  ( a.CustomerID = @CustomerId
       AND @CreditCardType = -1
       AND a.CardNumHash in (Select Item from SplitString(@HashTemp,',') union Select @CardNum)
       AND a.TransDateTime >= @FromToDateTime
       AND a.TransDateTime <= @ToDateTime
       )
UNION ALL
      SELECT a.CustomerId, a.TransDateTime, c.AreaName, b.MeterName, a.TransactionsCreditCardId, a.TransType,
       b.Location,a.BayNumber, a.AmountInCents/100.00 AS Amount, a.TimePaid,
       a.CardNumHash, a.Status,e.Description, d.Name as CreditCardTypeDesc,
       a.CreditCardType,a.ReceiptNo,a.AreaId, a.MeterId,d.Name,
       COALESCE ((SELECT MAX(f.ResultDateTime) FROM  CreditCardAttempts f
       WHERE f.TransactionsCreditCardID = a.TransactionsCreditCardId),0) As UploadTimeBatch, 
       COALESCE ((SELECT MAX(f.TransAuditDate) FROM  TransactionsAudit f
       left join TransactionsAcquirerResp r on r.TransAuditID = f.TransAuditId
       WHERE f.TransactionId = a.TransactionsCreditCardId),0) As UploadTimeOnline
       FROM qTransactionsCreditCard a INNER JOIN Meters b ON a.MeterId = b.MeterID
       AND a.AreaId = b.AreaID
       AND a.CustomerId = b.CustomerID
       INNER JOIN Areas c ON b.AreaID = c.AreaID
       AND b.CustomerID = c.CustomerID
       INNER JOIN CreditCardTypes d ON a.CreditCardType = d.CreditCardType
       INNER JOIN TransactionStatus e ON a.Status = e.StatusId
       WHERE (@CreditCardType <> -1 AND a.CustomerID = @CustomerId
       AND a.CreditCardType = @CreditCardType
       AND a.CardNumHash = @CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @FromToDateTime
       )
       OR  ( a.CustomerID = @CustomerId
       AND @CreditCardType = -1
       AND a.CardNumHash =@CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @FromToDateTime
       )
       ORDER BY a.TransDateTime DESC  
end   
return
GO
/****** Object:  StoredProcedure [dbo].[spGetCardNum]    Script Date: 04/01/2014 22:08:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spGetCardNum]
 	@CustomerId int,
	@CreditCardType int,
    @CardNum varchar(40),
    @FromDateTime datetime,
    @ToDateTime datetime
AS

SELECT a.CustomerId, a.TransDateTime, c.AreaName, b.MeterName, a.TransactionsCreditCardId, a.TransType,
       b.Location,a.BayNumber, a.AmountInCents/100.00 AS Amount, a.TimePaid,
       a.CardNumHash, a.Status,e.Description, d.Name as CreditCardTypeDesc,
       a.CreditCardType,a.ReceiptNo,a.AreaId, a.MeterId,d.Name,
       COALESCE ((SELECT MAX(f.ResultDateTime) FROM  CreditCardAttempts f
       WHERE f.TransactionsCreditCardID = a.TransactionsCreditCardId),0) As UploadTimeBatch, 
       COALESCE ((SELECT MAX(f.TransAuditDate) FROM  TransactionsAudit f
       left join TransactionsAcquirerResp r on r.TransAuditID = f.TransAuditId
       WHERE f.TransactionId = a.TransactionsCreditCardId),0) As UploadTimeOnline
       FROM qTransactionsCreditCard a INNER JOIN Meters b ON a.MeterId = b.MeterID
       AND a.AreaId = b.AreaID
       AND a.CustomerId = b.CustomerID
       INNER JOIN Areas c ON b.AreaID = c.AreaID
       AND b.CustomerID = c.CustomerID
       INNER JOIN CreditCardTypes d ON a.CreditCardType = d.CreditCardType
       INNER JOIN TransactionStatus e ON a.Status = e.StatusId
       WHERE (@CreditCardType <> -1 AND a.CustomerID = @CustomerId
       AND a.CreditCardType = @CreditCardType
       AND a.CardNumHash = @CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       OR  ( a.CustomerID = @CustomerId
       AND @CreditCardType = -1
       AND a.CardNumHash =@CardNum
       AND a.TransDateTime >= @FromDateTime
       AND a.TransDateTime <= @ToDateTime
       )
       ORDER BY a.TransDateTime DESC
return
GO
/****** Object:  StoredProcedure [dbo].[sp_UDP_SensorStatus_2]    Script Date: 04/01/2014 22:08:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[sp_UDP_SensorStatus_2]
	-- Add the parameters for the stored procedure here
	 @cid int
	,@aid int
	,@mid int
	,@bay int
	,@time int
	,@status int
	,@gcid int =1
	,@gaid int=1
	,@gmid int=1
	,@diag image = NULL
	,@currentTs datetime	
	,@result int OUTPUT
	,@mcid int OUTPUT
	,@maid int OUTPUT
	,@mmid int OUTPUT
	,@mbay int OUTPUT
AS
Declare
	 @spaceid bigint
	,@meterTS datetime
	,@psoid bigint	
	,@errmsg varchar(255)
BEGIN
	exec sp_UDP_SensorStatus @cid,@aid,@mid,@bay,@time,@status,@gcid,@gaid,@gmid,@diag,@currentTs,@result output
	
	-- for sensor
	EXEC sp_UDP_getParkingSpaceID @cid,@aid,@mid,@bay,@spaceid OUTPUT;	
	
	-- Return mapped bay
	select @mcid = Customerid,@maid = AreaId, @mmid = MeterId, @mbay = BayNumber 
	from [ParkingSpaceMeterBayMap]
	where SensorParkingSpaceID = @spaceid
	
	--for Meter
	if (@mbay is not null) begin
	
		if not exists (
			select * from ParkingSpaces p where p.CustomerID = @mcid and p.AreaId = @maid and p.MeterId = @mmid and p.BayNumber = @mbay
		)begin
			exec [AddParkingSpaces] @mcid,@maid,@mmid,@mbay,@mbay
		end
		
		exec sp_UDP_SensorStatus @mcid,@maid,@mmid,@mbay,@time,@status,@gcid,@gaid,@gmid,@diag,@currentTs,@result output	
	end	
	
END
GO
/****** Object:  DdlTrigger [tr_MStran_alterschemaonly]    Script Date: 04/01/2014 22:08:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [tr_MStran_alterschemaonly] on database for ALTER_FUNCTION, ALTER_PROCEDURE as 

							set ANSI_NULLS ON
							set ANSI_PADDING ON
							set ANSI_WARNINGS ON
							set ARITHABORT ON
							set CONCAT_NULL_YIELDS_NULL ON
							set NUMERIC_ROUNDABORT OFF
							set QUOTED_IDENTIFIER ON

							declare @EventData xml
							set @EventData=EventData()    
							exec sys.sp_MStran_ddlrepl @EventData, 3
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
DISABLE TRIGGER [tr_MStran_alterschemaonly] ON DATABASE
GO
/****** Object:  DdlTrigger [tr_MStran_altertable]    Script Date: 04/01/2014 22:08:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [tr_MStran_altertable] on database for ALTER_TABLE as 

							set ANSI_NULLS ON
							set ANSI_PADDING ON
							set ANSI_WARNINGS ON
							set ARITHABORT ON
							set CONCAT_NULL_YIELDS_NULL ON
							set NUMERIC_ROUNDABORT OFF
							set QUOTED_IDENTIFIER ON

							declare @EventData xml
							set @EventData=EventData()    
							exec sys.sp_MStran_ddlrepl @EventData, 1
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
DISABLE TRIGGER [tr_MStran_altertable] ON DATABASE
GO
/****** Object:  DdlTrigger [tr_MStran_altertrigger]    Script Date: 04/01/2014 22:08:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [tr_MStran_altertrigger] on database for ALTER_TRIGGER as 

							set ANSI_NULLS ON
							set ANSI_PADDING ON
							set ANSI_WARNINGS ON
							set ARITHABORT ON
							set CONCAT_NULL_YIELDS_NULL ON
							set NUMERIC_ROUNDABORT OFF
							set QUOTED_IDENTIFIER ON

							declare @EventData xml
							set @EventData=EventData()    
							exec sys.sp_MStran_ddlrepl @EventData, 4
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
DISABLE TRIGGER [tr_MStran_altertrigger] ON DATABASE
GO
/****** Object:  DdlTrigger [tr_MStran_alterview]    Script Date: 04/01/2014 22:08:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create trigger [tr_MStran_alterview] on database for ALTER_VIEW as 

							set ANSI_NULLS ON
							set ANSI_PADDING ON
							set ANSI_WARNINGS ON
							set ARITHABORT ON
							set CONCAT_NULL_YIELDS_NULL ON
							set NUMERIC_ROUNDABORT OFF
							set QUOTED_IDENTIFIER ON

							declare @EventData xml
							set @EventData=EventData()    
							exec sys.sp_MStran_ddlrepl @EventData, 2
GO
SET ANSI_NULLS OFF
GO
SET QUOTED_IDENTIFIER OFF
GO
DISABLE TRIGGER [tr_MStran_alterview] ON DATABASE
GO
/****** Object:  Default [DF_Meters_MeterStatus]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_MeterStatus]  DEFAULT ((0)) FOR [MeterStatus]
GO
/****** Object:  Default [DF_Meters_MeterRef]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_MeterRef]  DEFAULT ((0)) FOR [MeterRef]
GO
/****** Object:  Default [DF_Meters_MeterName]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_MeterName]  DEFAULT ('Meter Name') FOR [MeterName]
GO
/****** Object:  Default [DF_Meters_Location]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_Location]  DEFAULT ('Location') FOR [Location]
GO
/****** Object:  Default [DF_Meters_Description]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_Description]  DEFAULT ('Description') FOR [Description]
GO
/****** Object:  Default [DF_Meters_ScheduledServicePeriod]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_ScheduledServicePeriod]  DEFAULT ((0)) FOR [SchedServTime]
GO
/****** Object:  Default [DF_Meters_Latitude]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_Latitude]  DEFAULT ((0)) FOR [Latitude]
GO
/****** Object:  Default [DF_Meters_Logitude]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_Meters_Logitude]  DEFAULT ((0)) FOR [Longitude]
GO
/****** Object:  Default [DF_MetersGroup]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  CONSTRAINT [DF_MetersGroup]  DEFAULT ((0)) FOR [MeterGroup]
GO
/****** Object:  Default [DF__Meters__InstallD__3C1FE2D6]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters] ADD  DEFAULT (getdate()) FOR [InstallDate]
GO
/****** Object:  Default [DF__NSC_Triggers__TS__3D14070F]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[NSC_Triggers_Logs] ADD  DEFAULT (getdate()) FOR [TS]
GO
/****** Object:  Default [DF__EventUIDG__GenDa__2724C5F0]    Script Date: 04/01/2014 22:05:53 ******/
ALTER TABLE [dbo].[EventUIDGen] ADD  DEFAULT (getdate()) FOR [GenDate]
GO
/****** Object:  Default [DF__Mechanism__IsDis__2DD1C37F]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[MechanismMasterCustomer] ADD  DEFAULT ((1)) FOR [IsDisplay]
GO
/****** Object:  Default [DF_Customers_FromEmailAddress]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_FromEmailAddress]  DEFAULT ('From Email') FOR [FromEmailAddress]
GO
/****** Object:  Default [DF_Customers_UnReconcileCleanupLag]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  CONSTRAINT [DF_Customers_UnReconcileCleanupLag]  DEFAULT ((5)) FOR [UnReconcileCleanupLag]
GO
/****** Object:  Default [DF__Customers__Creat__19CACAD2]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
/****** Object:  Default [DF__Customers__IsEMV__1ABEEF0B]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((0)) FOR [IsEMV]
GO
/****** Object:  Default [DF__Customers__IsPay__1BB31344]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((0)) FOR [IsPayByPhone]
GO
/****** Object:  Default [DF__Customers__Grace__1CA7377D]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((0)) FOR [GracePeriodMinute]
GO
/****** Object:  Default [DF__Customers__FreeP__1D9B5BB6]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((0)) FOR [FreeParkingMinute]
GO
/****** Object:  Default [DF_EventDetails_CustomerID]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes] ADD  CONSTRAINT [DF_EventDetails_CustomerID]  DEFAULT ((0)) FOR [CustomerID]
GO
/****** Object:  Default [DF_EventDetails_EventSource]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes] ADD  CONSTRAINT [DF_EventDetails_EventSource]  DEFAULT ((0)) FOR [EventSource]
GO
/****** Object:  Default [DF_EventDetails_EventCode]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes] ADD  CONSTRAINT [DF_EventDetails_EventCode]  DEFAULT ((0)) FOR [EventCode]
GO
/****** Object:  Default [DF_EventDetails_AlarmTier]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes] ADD  CONSTRAINT [DF_EventDetails_AlarmTier]  DEFAULT ((0)) FOR [AlarmTier]
GO
/****** Object:  Default [DF__AlamrUIDG__GenDa__7869D707]    Script Date: 04/01/2014 22:06:22 ******/
ALTER TABLE [dbo].[AlamrUIDGen] ADD  DEFAULT (getdate()) FOR [GenDate]
GO
/****** Object:  Default [DF__Historica__DownT__2818EA29]    Script Date: 04/01/2014 22:06:25 ******/
ALTER TABLE [dbo].[HistoricalAlarms] ADD  DEFAULT ((0)) FOR [DownTimeMinute]
GO
/****** Object:  Default [DF__Collectio__Colle__05C3D225]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((1)) FOR [CollectionRunStatus]
GO
/****** Object:  Default [DF__Collectio__SkipS__06B7F65E]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekSunday]
GO
/****** Object:  Default [DF__Collectio__SkipS__07AC1A97]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekMonday]
GO
/****** Object:  Default [DF__Collectio__SkipS__08A03ED0]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekTuesday]
GO
/****** Object:  Default [DF__Collectio__SkipS__09946309]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekWednesday]
GO
/****** Object:  Default [DF__Collectio__SkipS__0A888742]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekThursday]
GO
/****** Object:  Default [DF__Collectio__SkipS__0B7CAB7B]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekFriday]
GO
/****** Object:  Default [DF__Collectio__SkipS__0C70CFB4]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun] ADD  DEFAULT ((0)) FOR [SkipSpecificDaysOfWeekSaturday]
GO
/****** Object:  Default [DF__Collectio__Total__0D64F3ED]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport] ADD  DEFAULT ((0)) FOR [TotalManualCoinCount]
GO
/****** Object:  Default [DF__Collectio__Total__0E591826]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport] ADD  DEFAULT ((0)) FOR [TotalManualCashAmt]
GO
/****** Object:  Default [DF__Collectio__Total__0F4D3C5F]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport] ADD  DEFAULT ((0)) FOR [TotalMeterCashAmt]
GO
/****** Object:  Default [DF__Collectio__Total__10416098]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport] ADD  DEFAULT ((0)) FOR [TotalMeterCoinCount]
GO
/****** Object:  Default [DF__CashBoxDa__Unsch__000AF8CF]    Script Date: 04/01/2014 22:06:37 ******/
ALTER TABLE [dbo].[CashBoxDataImport] ADD  DEFAULT ((0)) FOR [UnscheduledFlag]
GO
/****** Object:  Default [DF__SensorPay__Chang__59B045BD]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionAudit] ADD  DEFAULT (getdate()) FOR [ChangeDate]
GO
/****** Object:  Default [DF__SensorMap__Mappi__57C7FD4B]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[SensorMapping] ADD  DEFAULT ((1)) FOR [MappingState]
GO
/****** Object:  Default [DF_AddedDateTime]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces] ADD  CONSTRAINT [DF_AddedDateTime]  DEFAULT (getdate()) FOR [AddedDateTime]
GO
/****** Object:  Default [DF_Sensor]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces] ADD  CONSTRAINT [DF_Sensor]  DEFAULT ((0)) FOR [HasSensor]
GO
/****** Object:  Default [DF__ParkingSp__Insta__5026DB83]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces] ADD  DEFAULT (getdate()) FOR [InstallDate]
GO
/****** Object:  Default [DF_CoinDollar2]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinDollar2]  DEFAULT ((0)) FOR [CoinDollar2]
GO
/****** Object:  Default [DF_CoinDollar1]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinDollar1]  DEFAULT ((0)) FOR [CoinDollar1]
GO
/****** Object:  Default [DF_CoinCent50]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinCent50]  DEFAULT ((0)) FOR [CoinCent50]
GO
/****** Object:  Default [DF_CoinCent20]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinCent20]  DEFAULT ((0)) FOR [CoinCent20]
GO
/****** Object:  Default [DF_CoinCent10]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinCent10]  DEFAULT ((0)) FOR [CoinCent10]
GO
/****** Object:  Default [DF_CoinCent25]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinCent25]  DEFAULT ((0)) FOR [CoinCent25]
GO
/****** Object:  Default [DF_CoinCent1]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  CONSTRAINT [DF_CoinCent1]  DEFAULT ((0)) FOR [CoinCent1]
GO
/****** Object:  Default [DF__Transacti__Creat__6521F869]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
/****** Object:  Default [DF__Transacti__CoinU__47B19113]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  DEFAULT ((0)) FOR [CoinUnknown]
GO
/****** Object:  Default [DF__Transacti__CoinR__48A5B54C]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash] ADD  DEFAULT ((0)) FOR [CoinRejected]
GO
/****** Object:  Default [DF_CollDataCashSummOnCall_Amount]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[CollDataSumm] ADD  CONSTRAINT [DF_CollDataCashSummOnCall_Amount]  DEFAULT ((0)) FOR [Amount]
GO
/****** Object:  Default [DF_Areas_Description]    Script Date: 04/01/2014 22:06:46 ******/
ALTER TABLE [dbo].[Areas] ADD  CONSTRAINT [DF_Areas_Description]  DEFAULT ('Default Description') FOR [Description]
GO
/****** Object:  Default [DF_PPOImport_PPOStatusType]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[PPOStatusCodes] ADD  CONSTRAINT [DF_PPOImport_PPOStatusType]  DEFAULT ('K') FOR [PPOStatusType]
GO
/****** Object:  Default [DF__RateTrans__Recei__54EB90A0]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[RateTransmission] ADD  DEFAULT (getdate()) FOR [ReceivedTS]
GO
/****** Object:  Default [DF__RateTrans__Custo__55DFB4D9]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[RateTransmission] ADD  DEFAULT ((217)) FOR [CustomerId]
GO
/****** Object:  Default [DF__SFMeterMa__Multi__2003926C]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[SFMeterMap] ADD  DEFAULT ((0)) FOR [MultiSpace]
GO
/****** Object:  Default [DF_SFPriceSchedule_Received]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[SFPriceSchedule] ADD  CONSTRAINT [DF_SFPriceSchedule_Received]  DEFAULT (getdate()) FOR [Received]
GO
/****** Object:  Default [DF__SFCollect__CVSSt__2F45D5FC]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[SFCollectionRoute] ADD  DEFAULT (getdate()) FOR [CVSStartDate]
GO
/****** Object:  Default [DF__ParkingSpotS__id__5A303401]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[ParkingSpotStatus] ADD  DEFAULT (newid()) FOR [id]
GO
/****** Object:  Default [DF__ParkingSp__LastM__5B24583A]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[ParkingSpotStatus] ADD  DEFAULT (getdate()) FOR [LastModifiedDateTime]
GO
/****** Object:  Default [DF_OLTCardHash_InsDate]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[OLTCardHash] ADD  CONSTRAINT [DF_OLTCardHash_InsDate]  DEFAULT (getdate()) FOR [InsDate]
GO
/****** Object:  Default [DF__OLTCardHa__Custo__40E497F3]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[OLTCardHash] ADD  DEFAULT ((0)) FOR [CustomerId]
GO
/****** Object:  Default [DF__MeterPush__Creat__3296789C]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[MeterPushSchedule] ADD  DEFAULT (getdate()) FOR [CreatedTime]
GO
/****** Object:  Default [DF__Customers__Creat__1E8F7FEF]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[CustomersAudit] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
/****** Object:  Default [DF__EmailGrou__DateC__216BEC9A]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[EmailGroup] ADD  DEFAULT (getdate()) FOR [DateCreated]
GO
/****** Object:  Default [DF__EventCode__Updat__2630A1B7]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[EventCodesAudit] ADD  DEFAULT (getdate()) FOR [UpdatedDateTime]
GO
/****** Object:  Default [DF__CurrentPa__LastM__150615B5]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[CurrentParkingSpotStatus] ADD  DEFAULT (getdate()) FOR [LastModifiedDateTime]
GO
/****** Object:  Default [DF__Configura__GenDa__1411F17C]    Script Date: 04/01/2014 22:06:49 ******/
ALTER TABLE [dbo].[ConfigurationIDGen] ADD  DEFAULT (getdate()) FOR [GenDate]
GO
/****** Object:  Default [DF__SFTransmi__Trans__2C696951]    Script Date: 04/01/2014 22:06:52 ******/
ALTER TABLE [dbo].[SFTransmission] ADD  DEFAULT (getdate()) FOR [TransDate]
GO
/****** Object:  Default [DF_StreetlineSpace_CurrentSeqNum]    Script Date: 04/01/2014 22:06:57 ******/
ALTER TABLE [dbo].[StreetlineSpace] ADD  CONSTRAINT [DF_StreetlineSpace_CurrentSeqNum]  DEFAULT ((0)) FOR [CurrentSeqNum]
GO
/****** Object:  Default [DF_StreetlineEvent_attempt]    Script Date: 04/01/2014 22:06:57 ******/
ALTER TABLE [dbo].[StreetlineEvent] ADD  CONSTRAINT [DF_StreetlineEvent_attempt]  DEFAULT ((0)) FOR [attempt]
GO
/****** Object:  Default [DF__Version__CHANGE___7093AB15]    Script Date: 04/01/2014 22:06:57 ******/
ALTER TABLE [dbo].[Version] ADD  DEFAULT (getdate()) FOR [CHANGE_DATE]
GO
/****** Object:  Default [DF__webpages___IsCon__727BF387]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [IsConfirmed]
GO
/****** Object:  Default [DF__webpages___Passw__737017C0]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[webpages_Membership] ADD  DEFAULT ((0)) FOR [PasswordFailuresSinceLastSuccess]
GO
/****** Object:  Default [DF__WhilteLis__Disco__764C846B]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[WhilteListFileStaging] ADD  DEFAULT ((0)) FOR [DiscountPercentage]
GO
/****** Object:  Default [DF__WhilteLis__Disco__7740A8A4]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[WhilteListFileStaging] ADD  DEFAULT ((0)) FOR [DiscountMinute]
GO
/****** Object:  Default [DF__WhilteLis__Disco__74643BF9]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[WhilteListFile] ADD  DEFAULT ((0)) FOR [DiscountPercentage]
GO
/****** Object:  Default [DF__WhilteLis__Disco__75586032]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[WhilteListFile] ADD  DEFAULT ((0)) FOR [DiscountMinute]
GO
/****** Object:  Default [DF__Users__AccountSt__69E6AD86]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [AccountStatus]
GO
/****** Object:  Default [DF__Users__AccountSt__6ADAD1BF]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [AccountStatusUpdated]
GO
/****** Object:  Default [DF__Users__Created__6BCEF5F8]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [Created]
GO
/****** Object:  Default [DF__Users__PendingCo__6CC31A31]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [PendingCount]
GO
/****** Object:  Default [DF__Users__ApprovedC__6DB73E6A]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [ApprovedCount]
GO
/****** Object:  Default [DF__Users__RejectedC__6EAB62A3]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [RejectedCount]
GO
/****** Object:  Default [DF__Users__UserName__6F9F86DC]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('') FOR [UserName]
GO
/****** Object:  Default [DF__TariffRat__IsAct__5D80D6A1]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRate] ADD  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF__SFSchedul__Recei__39C3646F]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[SFSchedule] ADD  DEFAULT (getdate()) FOR [Received]
GO
/****** Object:  Default [DF__ConfigPro__Creat__1229A90A]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ConfigProfileSpaceAudit] ADD  DEFAULT (getdate()) FOR [CreateDatetime]
GO
/****** Object:  Default [DF__ConfigPro__Audit__131DCD43]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ConfigProfileSpaceAudit] ADD  DEFAULT (getdate()) FOR [AuditDatetime]
GO
/****** Object:  Default [DF__MeterMapp__Creat__31A25463]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterMapping] ADD  DEFAULT (getdate()) FOR [CreatedDateTime]
GO
/****** Object:  Default [DF_OLTCardPresent]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[OLTAcquirers] ADD  CONSTRAINT [DF_OLTCardPresent]  DEFAULT ((0)) FOR [CardPresent]
GO
/****** Object:  Default [DF__OLTAcquir__ReAut__3EFC4F81]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[OLTAcquirers] ADD  DEFAULT ((0)) FOR [ReAuthorise]
GO
/****** Object:  Default [DF__RateSched__IsAct__53F76C67]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateSchedule] ADD  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_MeterDiagnostic_ReceivedTime]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterDiagnostic] ADD  CONSTRAINT [DF_MeterDiagnostic_ReceivedTime]  DEFAULT (getdate()) FOR [ReceivedTime]
GO
/****** Object:  Default [DF_MechMasterStatus]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MechMaster] ADD  CONSTRAINT [DF_MechMasterStatus]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_MechMasterInsertedDate]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MechMaster] ADD  CONSTRAINT [DF_MechMasterInsertedDate]  DEFAULT (getdate()) FOR [InsertedDate]
GO
/****** Object:  Default [DF_HousingMaster_Block]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster] ADD  CONSTRAINT [DF_HousingMaster_Block]  DEFAULT ('Block') FOR [Block]
GO
/****** Object:  Default [DF_HousingMaster_StreetName]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster] ADD  CONSTRAINT [DF_HousingMaster_StreetName]  DEFAULT ('StreetName') FOR [StreetName]
GO
/****** Object:  Default [DF_HousingMaster_StreetType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster] ADD  CONSTRAINT [DF_HousingMaster_StreetType]  DEFAULT ('StreetType') FOR [StreetType]
GO
/****** Object:  Default [DF_HousingMaster_StreetDirection]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster] ADD  CONSTRAINT [DF_HousingMaster_StreetDirection]  DEFAULT ('DIR') FOR [StreetDirection]
GO
/****** Object:  Default [DF_HousingMaster_IsActive]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster] ADD  CONSTRAINT [DF_HousingMaster_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
/****** Object:  Default [DF_DateTime]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PaymentReceived] ADD  CONSTRAINT [DF_DateTime]  DEFAULT (getdate()) FOR [ReceivedTime]
GO
/****** Object:  Default [DF_PAMActiveCustomers_ResetImin]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMActiveCustomers] ADD  CONSTRAINT [DF_PAMActiveCustomers_ResetImin]  DEFAULT ((1)) FOR [ResetImin]
GO
/****** Object:  Default [DF_PAMActiveCustomers_ExpTimeByPAM]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMActiveCustomers] ADD  CONSTRAINT [DF_PAMActiveCustomers_ExpTimeByPAM]  DEFAULT ((1)) FOR [ExpTimeByPAM]
GO
/****** Object:  Default [DF__AssetStat__IsDis__7A521F79]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AssetStateCustomer] ADD  DEFAULT ((1)) FOR [IsDisplayed]
GO
/****** Object:  Default [DF__AuditRegi__Locat__7B4643B2]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AuditRegistry] ADD  DEFAULT ('LocationId') FOR [LocationId]
GO
/****** Object:  Default [DF__AuditRegi__MechS__7C3A67EB]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AuditRegistry] ADD  DEFAULT ('MechSerial') FOR [MechSerial]
GO
/****** Object:  Default [DF__AuditRegi__Coins__7D2E8C24]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AuditRegistry] ADD  DEFAULT ('Coins') FOR [Coins]
GO
/****** Object:  Default [DF__CashBox__Install__7F16D496]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox] ADD  DEFAULT (getdate()) FOR [InstallDate]
GO
/****** Object:  Default [DF__BlackList__DateT__7E22B05D]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[BlackListFiles] ADD  DEFAULT (getdate()) FOR [DateTimeGenerated]
GO
/****** Object:  Default [DF__DiscountS__IsDis__1F83A428]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountScheme] ADD  DEFAULT ((1)) FOR [IsDisplay]
GO
/****** Object:  Default [DF__CoinDenom__IsDis__00FF1D08]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CoinDenominationCustomer] ADD  DEFAULT ((1)) FOR [IsDisplay]
GO
/****** Object:  Default [DF__DiscountS__IsDis__2077C861]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomer] ADD  DEFAULT ((1)) FOR [IsDisplay]
GO
/****** Object:  Default [DF__PushStatu__Attem__5303482E]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[PushStatus] ADD  DEFAULT ((0)) FOR [Attempt]
GO
/****** Object:  Default [DF_CollDataSched_SchedDays]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CollDataSched] ADD  CONSTRAINT [DF_CollDataSched_SchedDays]  DEFAULT ((0)) FOR [SchedDays]
GO
/****** Object:  Default [DF__CustomerD__IsReq__15FA39EE]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerDetails] ADD  DEFAULT ((1)) FOR [IsRequired]
GO
/****** Object:  Default [DF__CustomerD__IsDis__16EE5E27]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerDetails] ADD  DEFAULT ((1)) FOR [IsDisplay]
GO
/****** Object:  Default [DF_CollDataImport_MeterStatus]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CollDataImport] ADD  CONSTRAINT [DF_CollDataImport_MeterStatus]  DEFAULT ((0)) FOR [StatusCode]
GO
/****** Object:  Default [DF_CollDataImport_AmountInCents]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CollDataImport] ADD  CONSTRAINT [DF_CollDataImport_AmountInCents]  DEFAULT ((0)) FOR [AmountInCents]
GO
/****** Object:  Default [DF__ConfigPro__Creat__113584D1]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[ConfigProfileSpace] ADD  DEFAULT (getdate()) FOR [CreateDatetime]
GO
/****** Object:  Default [DF__ParkingSp__Creat__4C564A9F]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceExpiryConfirmationEvent] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
/****** Object:  Default [DF__ParkingSp__Space__4885B9BB]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails] ADD  DEFAULT ((0)) FOR [SpaceType]
GO
/****** Object:  Default [DF__ParkingSp__HBInt__4979DDF4]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails] ADD  DEFAULT ((10)) FOR [HBInterval]
GO
/****** Object:  Default [DF__ParkingSp__Expir__4A6E022D]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails] ADD  DEFAULT ('-5=2,5=1') FOR [ExpiryLevel]
GO
/****** Object:  Default [DF__ParkingSp__Perce__4B622666]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails] ADD  DEFAULT ('30=1,60=2') FOR [PercentLevel]
GO
/****** Object:  Default [DF__RateTrans__Meter__56D3D912]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[RateTransmissionJob] ADD  DEFAULT ((0)) FOR [MeterId]
GO
/****** Object:  Default [DF__VersionPr__Meter__7187CF4E]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeter] ADD  DEFAULT ((1)) FOR [MeterGroup]
GO
/****** Object:  Default [DF__SensorMap__Chang__58BC2184]    Script Date: 04/01/2014 22:07:56 ******/
ALTER TABLE [dbo].[SensorMappingAudit] ADD  DEFAULT (getdate()) FOR [ChangeDate]
GO
/****** Object:  Default [DF__Transacti__Gatew__68F2894D]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsSmartCard] ADD  DEFAULT ((0)) FOR [GatewayMethod]
GO
/****** Object:  Default [DF_TransactionsCreditCard_HashScheme]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard] ADD  CONSTRAINT [DF_TransactionsCreditCard_HashScheme]  DEFAULT ((0)) FOR [HashScheme]
GO
/****** Object:  Default [DF__Transacti__Creat__670A40DB]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard] ADD  DEFAULT (getdate()) FOR [CreateDateTime]
GO
/****** Object:  Default [DF__Transacti__Gatew__67FE6514]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard] ADD  DEFAULT ((0)) FOR [GatewayMethod]
GO
/****** Object:  Default [DF_RecDate]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[ParkingSpaceOccupancyAudit] ADD  CONSTRAINT [DF_RecDate]  DEFAULT (getdate()) FOR [RecCreationDate]
GO
/****** Object:  Default [DF_PAMTx_TopUp]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx] ADD  CONSTRAINT [DF_PAMTx_TopUp]  DEFAULT ((0)) FOR [TopUp]
GO
/****** Object:  Default [DF_PAMTx_TransactionType]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx] ADD  CONSTRAINT [DF_PAMTx_TransactionType]  DEFAULT ((0)) FOR [TransactionType]
GO
/****** Object:  Default [DF_PAMTx_AmountCent]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx] ADD  CONSTRAINT [DF_PAMTx_AmountCent]  DEFAULT ((0)) FOR [AmountCent]
GO
/****** Object:  Default [DF_PAMTx_Received]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx] ADD  CONSTRAINT [DF_PAMTx_Received]  DEFAULT (getdate()) FOR [Received]
GO
/****** Object:  Default [DF_PAMTx_Attempt]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx] ADD  CONSTRAINT [DF_PAMTx_Attempt]  DEFAULT ((0)) FOR [Attempt]
GO
/****** Object:  Check [ServerInstance_check]    Script Date: 04/01/2014 22:05:37 ******/
ALTER TABLE [dbo].[ServerInstance]  WITH NOCHECK ADD  CONSTRAINT [ServerInstance_check] CHECK  (([Instanceid]>=(100) OR [Instanceid]<=(999)))
GO
ALTER TABLE [dbo].[ServerInstance] NOCHECK CONSTRAINT [ServerInstance_check]
GO
/****** Object:  ForeignKey [FK_SLA_RegulatedSchedule_DOW]    Script Date: 04/01/2014 21:59:10 ******/
ALTER TABLE [dbo].[SLA_RegulatedSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_RegulatedSchedule_DOW] FOREIGN KEY([DayOfWeek])
REFERENCES [dbo].[DayOfWeek] ([DayOfWeekId])
GO
ALTER TABLE [dbo].[SLA_RegulatedSchedule] CHECK CONSTRAINT [FK_SLA_RegulatedSchedule_DOW]
GO
/****** Object:  ForeignKey [FK_SLA_RegulatedSchedule_Meters]    Script Date: 04/01/2014 21:59:10 ******/
ALTER TABLE [dbo].[SLA_RegulatedSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_RegulatedSchedule_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[SLA_RegulatedSchedule] CHECK CONSTRAINT [FK_SLA_RegulatedSchedule_Meters]
GO
/****** Object:  ForeignKey [FK_SLA_MaintenanceSchedule_Customer]    Script Date: 04/01/2014 21:59:10 ******/
ALTER TABLE [dbo].[SLA_MaintenanceSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_MaintenanceSchedule_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SLA_MaintenanceSchedule] CHECK CONSTRAINT [FK_SLA_MaintenanceSchedule_Customer]
GO
/****** Object:  ForeignKey [FK_SLA_MaintenanceSchedule_DOW]    Script Date: 04/01/2014 21:59:10 ******/
ALTER TABLE [dbo].[SLA_MaintenanceSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_MaintenanceSchedule_DOW] FOREIGN KEY([DayOfWeek])
REFERENCES [dbo].[DayOfWeek] ([DayOfWeekId])
GO
ALTER TABLE [dbo].[SLA_MaintenanceSchedule] CHECK CONSTRAINT [FK_SLA_MaintenanceSchedule_DOW]
GO
/****** Object:  ForeignKey [FK_SLA_AssetDownTime_Meters]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[SLA_AssetDownTime]  WITH CHECK ADD  CONSTRAINT [FK_SLA_AssetDownTime_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[SLA_AssetDownTime] CHECK CONSTRAINT [FK_SLA_AssetDownTime_Meters]
GO
/****** Object:  ForeignKey [FK_Meter_DemandZone]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meter_DemandZone] FOREIGN KEY([DemandZone])
REFERENCES [dbo].[DemandZone] ([DemandZoneId])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meter_DemandZone]
GO
/****** Object:  ForeignKey [FK_Meters_Areas]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_Areas] FOREIGN KEY([CustomerID], [AreaID])
REFERENCES [dbo].[Areas] ([CustomerID], [AreaID])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_Areas]
GO
/****** Object:  ForeignKey [FK_Meters_AssetState]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH CHECK ADD  CONSTRAINT [FK_Meters_AssetState] FOREIGN KEY([MeterState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[Meters] CHECK CONSTRAINT [FK_Meters_AssetState]
GO
/****** Object:  ForeignKey [FK_Meters_Customers]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_Customers]
GO
/****** Object:  ForeignKey [FK_Meters_MechanismMaster]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_MechanismMaster] FOREIGN KEY([MeterType])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_MechanismMaster]
GO
/****** Object:  ForeignKey [FK_Meters_MeterGroup]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_MeterGroup] FOREIGN KEY([MeterGroup])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_MeterGroup]
GO
/****** Object:  ForeignKey [FK_Meters_OperationalStatus]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_OperationalStatus] FOREIGN KEY([OperationalStatusID])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_Meters_RegulatedStatus]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_RegulatedStatus] FOREIGN KEY([RegulatedStatusID])
REFERENCES [dbo].[RegulatedStatus] ([RegulatedStatusID])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_RegulatedStatus]
GO
/****** Object:  ForeignKey [FK_Meters_TimeZones]    Script Date: 04/01/2014 22:05:33 ******/
ALTER TABLE [dbo].[Meters]  WITH NOCHECK ADD  CONSTRAINT [FK_Meters_TimeZones] FOREIGN KEY([TimeZoneID])
REFERENCES [dbo].[TimeZones] ([TimeZoneID])
GO
ALTER TABLE [dbo].[Meters] NOCHECK CONSTRAINT [FK_Meters_TimeZones]
GO
/****** Object:  ForeignKey [FK_SFMeterMaintenanceEvent_Meters]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[SFMeterMaintenanceEvent]  WITH NOCHECK ADD  CONSTRAINT [FK_SFMeterMaintenanceEvent_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[SFMeterMaintenanceEvent] NOCHECK CONSTRAINT [FK_SFMeterMaintenanceEvent_Meters]
GO
/****** Object:  ForeignKey [FK_MeterMap_CollectionRun]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH CHECK ADD  CONSTRAINT [FK_MeterMap_CollectionRun] FOREIGN KEY([CollectionRunId])
REFERENCES [dbo].[CollectionRun] ([CollectionRunId])
GO
ALTER TABLE [dbo].[MeterMap] CHECK CONSTRAINT [FK_MeterMap_CollectionRun]
GO
/****** Object:  ForeignKey [FK_MeterMap_CollRoute]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_CollRoute] FOREIGN KEY([CollRouteId])
REFERENCES [dbo].[CollRoute] ([CollRouteId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_CollRoute]
GO
/****** Object:  ForeignKey [FK_MeterMap_CustomGroup1]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_CustomGroup1] FOREIGN KEY([CustomGroup1])
REFERENCES [dbo].[CustomGroup1] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_CustomGroup1]
GO
/****** Object:  ForeignKey [FK_MeterMap_CustomGroup2]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_CustomGroup2] FOREIGN KEY([CustomGroup2])
REFERENCES [dbo].[CustomGroup2] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_CustomGroup2]
GO
/****** Object:  ForeignKey [FK_MeterMap_CustomGroup3]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_CustomGroup3] FOREIGN KEY([CustomGroup3])
REFERENCES [dbo].[CustomGroup3] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_CustomGroup3]
GO
/****** Object:  ForeignKey [FK_MeterMap_EnforceRoute]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_EnforceRoute] FOREIGN KEY([EnfRouteId])
REFERENCES [dbo].[EnforceRoute] ([EnfRouteId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_EnforceRoute]
GO
/****** Object:  ForeignKey [FK_MeterMap_GATEWAY]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_GATEWAY] FOREIGN KEY([Customerid], [GatewayID])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_GATEWAY]
GO
/****** Object:  ForeignKey [FK_MeterMap_HousingMaster]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_HousingMaster] FOREIGN KEY([HousingId])
REFERENCES [dbo].[HousingMaster] ([HousingId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_HousingMaster]
GO
/****** Object:  ForeignKey [FK_MeterMap_MaintRoute]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_MaintRoute] FOREIGN KEY([MaintRouteId])
REFERENCES [dbo].[MaintRoute] ([MaintRouteId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_MaintRoute]
GO
/****** Object:  ForeignKey [FK_MeterMap_MechMaster]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_MechMaster] FOREIGN KEY([MechId])
REFERENCES [dbo].[MechMaster] ([MechId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_MechMaster]
GO
/****** Object:  ForeignKey [FK_MeterMap_Meters]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_Meters] FOREIGN KEY([Customerid], [Areaid], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_Meters]
GO
/****** Object:  ForeignKey [FK_MeterMap_Sensor]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_Sensor] FOREIGN KEY([Customerid], [SensorID])
REFERENCES [dbo].[Sensors] ([CustomerID], [SensorID])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_Sensor]
GO
/****** Object:  ForeignKey [FK_MeterMap_SubArea]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_SubArea] FOREIGN KEY([Customerid], [SubAreaID])
REFERENCES [dbo].[SubAreas] ([CustomerID], [SubAreaID])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_SubArea]
GO
/****** Object:  ForeignKey [FK_MeterMap_Zones]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MeterMap]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterMap_Zones] FOREIGN KEY([ZoneId])
REFERENCES [dbo].[Zones] ([ZoneId])
GO
ALTER TABLE [dbo].[MeterMap] NOCHECK CONSTRAINT [FK_MeterMap_Zones]
GO
/****** Object:  ForeignKey [FK_Sensors_AssetState]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH CHECK ADD  CONSTRAINT [FK_Sensors_AssetState] FOREIGN KEY([SensorState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[Sensors] CHECK CONSTRAINT [FK_Sensors_AssetState]
GO
/****** Object:  ForeignKey [FK_Sensors_Customers]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH NOCHECK ADD  CONSTRAINT [FK_Sensors_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Sensors] NOCHECK CONSTRAINT [FK_Sensors_Customers]
GO
/****** Object:  ForeignKey [FK_Sensors_MechanismMaster]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH NOCHECK ADD  CONSTRAINT [FK_Sensors_MechanismMaster] FOREIGN KEY([SensorModel])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[Sensors] NOCHECK CONSTRAINT [FK_Sensors_MechanismMaster]
GO
/****** Object:  ForeignKey [FK_Sensors_MeterGroup]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH NOCHECK ADD  CONSTRAINT [FK_Sensors_MeterGroup] FOREIGN KEY([SensorType])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[Sensors] NOCHECK CONSTRAINT [FK_Sensors_MeterGroup]
GO
/****** Object:  ForeignKey [FK_Sensors_OperationalStatus]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH NOCHECK ADD  CONSTRAINT [FK_Sensors_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[Sensors] NOCHECK CONSTRAINT [FK_Sensors_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_SensorSpace]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Sensors]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorSpace] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[Sensors] NOCHECK CONSTRAINT [FK_SensorSpace]
GO
/****** Object:  ForeignKey [FK_Gateway_Customers]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Gateways]  WITH NOCHECK ADD  CONSTRAINT [FK_Gateway_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Gateways] NOCHECK CONSTRAINT [FK_Gateway_Customers]
GO
/****** Object:  ForeignKey [FK_Gateway_TimeZones]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Gateways]  WITH NOCHECK ADD  CONSTRAINT [FK_Gateway_TimeZones] FOREIGN KEY([TimeZoneID])
REFERENCES [dbo].[TimeZones] ([TimeZoneID])
GO
ALTER TABLE [dbo].[Gateways] NOCHECK CONSTRAINT [FK_Gateway_TimeZones]
GO
/****** Object:  ForeignKey [FK_GatewayModel_MechanismMaster]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Gateways]  WITH NOCHECK ADD  CONSTRAINT [FK_GatewayModel_MechanismMaster] FOREIGN KEY([GatewayModel])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[Gateways] NOCHECK CONSTRAINT [FK_GatewayModel_MechanismMaster]
GO
/****** Object:  ForeignKey [FK_Gateways_AssetState]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Gateways]  WITH CHECK ADD  CONSTRAINT [FK_Gateways_AssetState] FOREIGN KEY([GatewayState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[Gateways] CHECK CONSTRAINT [FK_Gateways_AssetState]
GO
/****** Object:  ForeignKey [FK_Gateways_OperationalStatus]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[Gateways]  WITH CHECK ADD  CONSTRAINT [FK_Gateways_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[Gateways] CHECK CONSTRAINT [FK_Gateways_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_MetersAudit_AssetPendingReason]    Script Date: 04/01/2014 22:05:52 ******/
ALTER TABLE [dbo].[MetersAudit]  WITH CHECK ADD  CONSTRAINT [FK_MetersAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[MetersAudit] CHECK CONSTRAINT [FK_MetersAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_EventLogs_Meters]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_Meters]
GO
/****** Object:  ForeignKey [FK_EventLogs_TimeType1]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_TimeType1] FOREIGN KEY([TimeType1])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_TimeType1]
GO
/****** Object:  ForeignKey [FK_EventLogs_TimeType2]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_TimeType2] FOREIGN KEY([TimeType2])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_TimeType2]
GO
/****** Object:  ForeignKey [FK_EventLogs_TimeType3]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_TimeType3] FOREIGN KEY([TimeType3])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_TimeType3]
GO
/****** Object:  ForeignKey [FK_EventLogs_TimeType4]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_TimeType4] FOREIGN KEY([TimeType4])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_TimeType4]
GO
/****** Object:  ForeignKey [FK_EventLogs_TimeType5]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[EventLogs]  WITH CHECK ADD  CONSTRAINT [FK_EventLogs_TimeType5] FOREIGN KEY([TimeType5])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[EventLogs] CHECK CONSTRAINT [FK_EventLogs_TimeType5]
GO
/****** Object:  ForeignKey [FK_TimeTypeCustomer]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[TimeTypeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_TimeTypeCustomer] FOREIGN KEY([TimeTypeId])
REFERENCES [dbo].[TimeType] ([TimeTypeId])
GO
ALTER TABLE [dbo].[TimeTypeCustomer] CHECK CONSTRAINT [FK_TimeTypeCustomer]
GO
/****** Object:  ForeignKey [FK_TimeTypeCustomer_Customer]    Script Date: 04/01/2014 22:05:55 ******/
ALTER TABLE [dbo].[TimeTypeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_TimeTypeCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[TimeTypeCustomer] CHECK CONSTRAINT [FK_TimeTypeCustomer_Customer]
GO
/****** Object:  ForeignKey [FK_SLA_Holiday_Customer]    Script Date: 04/01/2014 22:05:57 ******/
ALTER TABLE [dbo].[SLA_Holiday]  WITH CHECK ADD  CONSTRAINT [FK_SLA_Holiday_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SLA_Holiday] CHECK CONSTRAINT [FK_SLA_Holiday_Customer]
GO
/****** Object:  ForeignKey [FK_AssetType_Customer]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[AssetType]  WITH NOCHECK ADD  CONSTRAINT [FK_AssetType_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[AssetType] NOCHECK CONSTRAINT [FK_AssetType_Customer]
GO
/****** Object:  ForeignKey [FK_AssetType_MeterGroup]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[AssetType]  WITH NOCHECK ADD  CONSTRAINT [FK_AssetType_MeterGroup] FOREIGN KEY([MeterGroupId])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[AssetType] NOCHECK CONSTRAINT [FK_AssetType_MeterGroup]
GO
/****** Object:  ForeignKey [FK_MechanismMaster_Customer]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[MechanismMasterCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_MechanismMaster_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[MechanismMasterCustomer] NOCHECK CONSTRAINT [FK_MechanismMaster_Customer]
GO
/****** Object:  ForeignKey [FK_MechanismMasterCustomer]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[MechanismMasterCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_MechanismMasterCustomer] FOREIGN KEY([MechanismId])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[MechanismMasterCustomer] NOCHECK CONSTRAINT [FK_MechanismMasterCustomer]
GO
/****** Object:  ForeignKey [FK_MechanismMaster_MeterGroup]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[MechanismMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_MechanismMaster_MeterGroup] FOREIGN KEY([MeterGroupId])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[MechanismMaster] NOCHECK CONSTRAINT [FK_MechanismMaster_MeterGroup]
GO
/****** Object:  ForeignKey [FK_Customers_TimeZone]    Script Date: 04/01/2014 22:05:58 ******/
ALTER TABLE [dbo].[Customers]  WITH NOCHECK ADD  CONSTRAINT [FK_Customers_TimeZone] FOREIGN KEY([TimeZoneID])
REFERENCES [dbo].[TimeZones] ([TimeZoneID])
GO
ALTER TABLE [dbo].[Customers] NOCHECK CONSTRAINT [FK_Customers_TimeZone]
GO
/****** Object:  ForeignKey [FK_EventCodes_AlarmTier]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodes_AlarmTier] FOREIGN KEY([AlarmTier])
REFERENCES [dbo].[AlarmTier] ([Tier])
GO
ALTER TABLE [dbo].[EventCodes] NOCHECK CONSTRAINT [FK_EventCodes_AlarmTier]
GO
/****** Object:  ForeignKey [FK_EventCodes_EventCategory]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodes_EventCategory] FOREIGN KEY([EventCategory])
REFERENCES [dbo].[EventCategory] ([EventCategoryId])
GO
ALTER TABLE [dbo].[EventCodes] NOCHECK CONSTRAINT [FK_EventCodes_EventCategory]
GO
/****** Object:  ForeignKey [FK_EventCodes_EventSources]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodes_EventSources] FOREIGN KEY([EventSource])
REFERENCES [dbo].[EventSources] ([EventSourceCode])
GO
ALTER TABLE [dbo].[EventCodes] NOCHECK CONSTRAINT [FK_EventCodes_EventSources]
GO
/****** Object:  ForeignKey [FK_EventCodes_EventType]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodes_EventType] FOREIGN KEY([EventType])
REFERENCES [dbo].[EventType] ([EventTypeId])
GO
ALTER TABLE [dbo].[EventCodes] NOCHECK CONSTRAINT [FK_EventCodes_EventType]
GO
/****** Object:  ForeignKey [FK_EventDetails_Customers]    Script Date: 04/01/2014 22:06:21 ******/
ALTER TABLE [dbo].[EventCodes]  WITH NOCHECK ADD  CONSTRAINT [FK_EventDetails_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[EventCodes] NOCHECK CONSTRAINT [FK_EventDetails_Customers]
GO
/****** Object:  ForeignKey [FK_ActiveAlarms_EventCodes]    Script Date: 04/01/2014 22:06:25 ******/
ALTER TABLE [dbo].[ActiveAlarms]  WITH NOCHECK ADD  CONSTRAINT [FK_ActiveAlarms_EventCodes] FOREIGN KEY([CustomerID], [EventSource], [EventCode])
REFERENCES [dbo].[EventCodes] ([CustomerID], [EventSource], [EventCode])
GO
ALTER TABLE [dbo].[ActiveAlarms] NOCHECK CONSTRAINT [FK_ActiveAlarms_EventCodes]
GO
/****** Object:  ForeignKey [FK_ActiveAlarms_Meters]    Script Date: 04/01/2014 22:06:25 ******/
ALTER TABLE [dbo].[ActiveAlarms]  WITH NOCHECK ADD  CONSTRAINT [FK_ActiveAlarms_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[ActiveAlarms] NOCHECK CONSTRAINT [FK_ActiveAlarms_Meters]
GO
/****** Object:  ForeignKey [FK_HistoricalAlarms_Meters]    Script Date: 04/01/2014 22:06:25 ******/
ALTER TABLE [dbo].[HistoricalAlarms]  WITH NOCHECK ADD  CONSTRAINT [FK_HistoricalAlarms_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[HistoricalAlarms] NOCHECK CONSTRAINT [FK_HistoricalAlarms_Meters]
GO
/****** Object:  ForeignKey [FK_HistoricalAlarms_TargetServiceDesignation]    Script Date: 04/01/2014 22:06:25 ******/
ALTER TABLE [dbo].[HistoricalAlarms]  WITH NOCHECK ADD  CONSTRAINT [FK_HistoricalAlarms_TargetServiceDesignation] FOREIGN KEY([TargetServiceDesignation])
REFERENCES [dbo].[TargetServiceDesignation] ([TargetServiceDesignationId])
GO
ALTER TABLE [dbo].[HistoricalAlarms] NOCHECK CONSTRAINT [FK_HistoricalAlarms_TargetServiceDesignation]
GO
/****** Object:  ForeignKey [FK_CollectionRunMeter]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRunMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunMeter] FOREIGN KEY([CollectionRunId])
REFERENCES [dbo].[CollectionRun] ([CollectionRunId])
GO
ALTER TABLE [dbo].[CollectionRunMeter] NOCHECK CONSTRAINT [FK_CollectionRunMeter]
GO
/****** Object:  ForeignKey [FK_CollectionRunMeter_Meters]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRunMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunMeter_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CollectionRunMeter] NOCHECK CONSTRAINT [FK_CollectionRunMeter_Meters]
GO
/****** Object:  ForeignKey [FK_CollectionRun_Status]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRun_Status] FOREIGN KEY([CollectionRunStatus])
REFERENCES [dbo].[CollectionRunStatus] ([CollectionRunStatusId])
GO
ALTER TABLE [dbo].[CollectionRun] NOCHECK CONSTRAINT [FK_CollectionRun_Status]
GO
/****** Object:  ForeignKey [FK_CollectionRun_Vendor]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRun_Vendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[CollectionRunVendor] ([CollectionRunVendorId])
GO
ALTER TABLE [dbo].[CollectionRun] NOCHECK CONSTRAINT [FK_CollectionRun_Vendor]
GO
/****** Object:  ForeignKey [FK_CollectionRunMeter_Customer]    Script Date: 04/01/2014 22:06:35 ******/
ALTER TABLE [dbo].[CollectionRun]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunMeter_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CollectionRun] NOCHECK CONSTRAINT [FK_CollectionRunMeter_Customer]
GO
/****** Object:  ForeignKey [FK_CollectionRunReport_CollectionRun]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunReport_CollectionRun] FOREIGN KEY([CollectionRunId])
REFERENCES [dbo].[CollectionRun] ([CollectionRunId])
GO
ALTER TABLE [dbo].[CollectionRunReport] NOCHECK CONSTRAINT [FK_CollectionRunReport_CollectionRun]
GO
/****** Object:  ForeignKey [FK_CollectionRunReport_Customer]    Script Date: 04/01/2014 22:06:36 ******/
ALTER TABLE [dbo].[CollectionRunReport]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunReport_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CollectionRunReport] NOCHECK CONSTRAINT [FK_CollectionRunReport_Customer]
GO
/****** Object:  ForeignKey [FK__SensorPay__Parki__6A70BD6B]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionAudit]  WITH NOCHECK ADD FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionAudit]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionAudit] FOREIGN KEY([SensorPaymentTransactionID])
REFERENCES [dbo].[SensorPaymentTransaction] ([SensorPaymentTransactionID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionAudit] NOCHECK CONSTRAINT [FK_SensorPaymentTransactionAudit]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent] FOREIGN KEY([SensorPaymentTransactionID])
REFERENCES [dbo].[SensorPaymentTransaction] ([SensorPaymentTransactionID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_ArrivalPSOAuditId]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_ArrivalPSOAuditId] FOREIGN KEY([ArrivalPSOAuditId])
REFERENCES [dbo].[ParkingSpaceOccupancyAudit] ([AuditID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_ArrivalPSOAuditId]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_Customer]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_Customer]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_DeparturePSOAuditId]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_DeparturePSOAuditId] FOREIGN KEY([DeparturePSOAuditId])
REFERENCES [dbo].[ParkingSpaceOccupancyAudit] ([AuditID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_DeparturePSOAuditId]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_Gateway]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_Gateway] FOREIGN KEY([CustomerId], [GatewayId])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_Gateway]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_NonCompliantStatus]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_NonCompliantStatus] FOREIGN KEY([NonCompliantStatus])
REFERENCES [dbo].[NonCompliantStatus] ([NonCompliantStatusID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_NonCompliantStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_OccupancyStatus]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_OccupancyStatus] FOREIGN KEY([OccupancyStatus])
REFERENCES [dbo].[OccupancyStatus] ([StatusID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_OccupancyStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_OperationalStatus]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransactionCurrent_Sensor]    Script Date: 04/01/2014 22:06:39 ******/
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransactionCurrent_Sensor] FOREIGN KEY([CustomerId], [SensorId])
REFERENCES [dbo].[Sensors] ([CustomerID], [SensorID])
GO
ALTER TABLE [dbo].[SensorPaymentTransactionCurrent] CHECK CONSTRAINT [FK_SensorPaymentTransactionCurrent_Sensor]
GO
/****** Object:  ForeignKey [FK_SensorMapping_GATEWAY]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[SensorMapping]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorMapping_GATEWAY] FOREIGN KEY([CustomerID], [GatewayID])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[SensorMapping] NOCHECK CONSTRAINT [FK_SensorMapping_GATEWAY]
GO
/****** Object:  ForeignKey [FK_SensorMapping_SENSOR]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[SensorMapping]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorMapping_SENSOR] FOREIGN KEY([CustomerID], [SensorID])
REFERENCES [dbo].[Sensors] ([CustomerID], [SensorID])
GO
ALTER TABLE [dbo].[SensorMapping] NOCHECK CONSTRAINT [FK_SensorMapping_SENSOR]
GO
/****** Object:  ForeignKey [FK_SensorMapping_State]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[SensorMapping]  WITH CHECK ADD  CONSTRAINT [FK_SensorMapping_State] FOREIGN KEY([MappingState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[SensorMapping] CHECK CONSTRAINT [FK_SensorMapping_State]
GO
/****** Object:  ForeignKey [FK_ParkingSpaces_DemandZone]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaces_DemandZone] FOREIGN KEY([DemandZoneId])
REFERENCES [dbo].[DemandZone] ([DemandZoneId])
GO
ALTER TABLE [dbo].[ParkingSpaces] NOCHECK CONSTRAINT [FK_ParkingSpaces_DemandZone]
GO
/****** Object:  ForeignKey [FK_ParkingSpaces_MeterGroup]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaces_MeterGroup] FOREIGN KEY([ParkingSpaceType])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[ParkingSpaces] NOCHECK CONSTRAINT [FK_ParkingSpaces_MeterGroup]
GO
/****** Object:  ForeignKey [FK_ParkingSpaces_Meters]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaces_Meters] FOREIGN KEY([CustomerID], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[ParkingSpaces] NOCHECK CONSTRAINT [FK_ParkingSpaces_Meters]
GO
/****** Object:  ForeignKey [FK_ParkingSpaces_OperationalStatus]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaces_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[ParkingSpaces] NOCHECK CONSTRAINT [FK_ParkingSpaces_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_ParkingSpaces_SpaceStatus]    Script Date: 04/01/2014 22:06:43 ******/
ALTER TABLE [dbo].[ParkingSpaces]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaces_SpaceStatus] FOREIGN KEY([SpaceStatus])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[ParkingSpaces] NOCHECK CONSTRAINT [FK_ParkingSpaces_SpaceStatus]
GO
/****** Object:  ForeignKey [FK_Transactions_AssetType]    Script Date: 04/01/2014 22:06:44 ******/
ALTER TABLE [dbo].[Transactions]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_AssetType] FOREIGN KEY([MeterGroupId])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[Transactions] NOCHECK CONSTRAINT [FK_Transactions_AssetType]
GO
/****** Object:  ForeignKey [FK_Transactions_CreditCardType]    Script Date: 04/01/2014 22:06:44 ******/
ALTER TABLE [dbo].[Transactions]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_CreditCardType] FOREIGN KEY([CreditCardType])
REFERENCES [dbo].[CreditCardTypes] ([CreditCardType])
GO
ALTER TABLE [dbo].[Transactions] NOCHECK CONSTRAINT [FK_Transactions_CreditCardType]
GO
/****** Object:  ForeignKey [FK_Transactions_Gateway]    Script Date: 04/01/2014 22:06:44 ******/
ALTER TABLE [dbo].[Transactions]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_Gateway] FOREIGN KEY([CustomerID], [GatewayId])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[Transactions] NOCHECK CONSTRAINT [FK_Transactions_Gateway]
GO
/****** Object:  ForeignKey [FK_Transactions_TransactionType]    Script Date: 04/01/2014 22:06:44 ******/
ALTER TABLE [dbo].[Transactions]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_TransactionType] FOREIGN KEY([TransactionType])
REFERENCES [dbo].[TransactionType] ([TransactionTypeId])
GO
ALTER TABLE [dbo].[Transactions] NOCHECK CONSTRAINT [FK_Transactions_TransactionType]
GO
/****** Object:  ForeignKey [FK_Transactions_TransType]    Script Date: 04/01/2014 22:06:44 ******/
ALTER TABLE [dbo].[Transactions]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_TransType] FOREIGN KEY([TransType])
REFERENCES [dbo].[TransType] ([TransTypeId])
GO
ALTER TABLE [dbo].[Transactions] NOCHECK CONSTRAINT [FK_Transactions_TransType]
GO
/****** Object:  ForeignKey [FK_TransactionsCash_Meters]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsCash_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsCash] NOCHECK CONSTRAINT [FK_TransactionsCash_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionsCash_ParkingSpaces]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[TransactionsCash]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsCash_ParkingSpaces] FOREIGN KEY([ParkingSpaceID])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[TransactionsCash] NOCHECK CONSTRAINT [FK_TransactionsCash_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_CollDataSumm_Meters]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[CollDataSumm]  WITH NOCHECK ADD  CONSTRAINT [FK_CollDataSumm_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CollDataSumm] NOCHECK CONSTRAINT [FK_CollDataSumm_Meters]
GO
/****** Object:  ForeignKey [FK_CollDataSumm_PaymentType]    Script Date: 04/01/2014 22:06:45 ******/
ALTER TABLE [dbo].[CollDataSumm]  WITH NOCHECK ADD  CONSTRAINT [FK_CollDataSumm_PaymentType] FOREIGN KEY([PaymentType])
REFERENCES [dbo].[PaymentType] ([PaymentType])
GO
ALTER TABLE [dbo].[CollDataSumm] NOCHECK CONSTRAINT [FK_CollDataSumm_PaymentType]
GO
/****** Object:  ForeignKey [FK_Zones_Customers]    Script Date: 04/01/2014 22:06:46 ******/
ALTER TABLE [dbo].[Zones]  WITH NOCHECK ADD  CONSTRAINT [FK_Zones_Customers] FOREIGN KEY([customerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Zones] NOCHECK CONSTRAINT [FK_Zones_Customers]
GO
/****** Object:  ForeignKey [FK_CustomGroup1_Customers]    Script Date: 04/01/2014 22:06:46 ******/
ALTER TABLE [dbo].[CustomGroup1]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomGroup1_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomGroup1] NOCHECK CONSTRAINT [FK_CustomGroup1_Customers]
GO
/****** Object:  ForeignKey [FK_Areas_Customers]    Script Date: 04/01/2014 22:06:46 ******/
ALTER TABLE [dbo].[Areas]  WITH NOCHECK ADD  CONSTRAINT [FK_Areas_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Areas] NOCHECK CONSTRAINT [FK_Areas_Customers]
GO
/****** Object:  ForeignKey [FK_WorkOrders_Status]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[WorkOrders]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrders_Status] FOREIGN KEY([Status])
REFERENCES [dbo].[WorkOrderStatus] ([WorkOrderStatusId])
GO
ALTER TABLE [dbo].[WorkOrders] CHECK CONSTRAINT [FK_WorkOrders_Status]
GO
/****** Object:  ForeignKey [FK_TransactionsPending_CardType]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[TransactionsPending]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsPending_CardType] FOREIGN KEY([CardTypeCode])
REFERENCES [dbo].[CardType] ([CardTypeCode])
GO
ALTER TABLE [dbo].[TransactionsPending] NOCHECK CONSTRAINT [FK_TransactionsPending_CardType]
GO
/****** Object:  ForeignKey [FK_TransactionsPending_TransactionStatus]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[TransactionsPending]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsPending_TransactionStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[TransactionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[TransactionsPending] NOCHECK CONSTRAINT [FK_TransactionsPending_TransactionStatus]
GO
/****** Object:  ForeignKey [FK_Users_AccountStatus]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users]  WITH NOCHECK ADD  CONSTRAINT [FK_Users_AccountStatus] FOREIGN KEY([AccountStatus])
REFERENCES [dbo].[AccountStatus] ([AccountStatusId])
GO
ALTER TABLE [dbo].[Users] NOCHECK CONSTRAINT [FK_Users_AccountStatus]
GO
/****** Object:  ForeignKey [FK_Users_UserTypes]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[Users]  WITH NOCHECK ADD  CONSTRAINT [FK_Users_UserTypes] FOREIGN KEY([UserType])
REFERENCES [dbo].[UserTypes] ([UserType])
GO
ALTER TABLE [dbo].[Users] NOCHECK CONSTRAINT [FK_Users_UserTypes]
GO
/****** Object:  ForeignKey [FK_VersionDetails_VersionMaster]    Script Date: 04/01/2014 22:06:58 ******/
ALTER TABLE [dbo].[VersionDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionDetails_VersionMaster] FOREIGN KEY([VersionID])
REFERENCES [dbo].[VersionMaster] ([VersionID])
GO
ALTER TABLE [dbo].[VersionDetails] NOCHECK CONSTRAINT [FK_VersionDetails_VersionMaster]
GO
/****** Object:  ForeignKey [FK_TransactionsAudit_CardType]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TransactionsAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsAudit_CardType] FOREIGN KEY([CardTypeCode])
REFERENCES [dbo].[CardType] ([CardTypeCode])
GO
ALTER TABLE [dbo].[TransactionsAudit] NOCHECK CONSTRAINT [FK_TransactionsAudit_CardType]
GO
/****** Object:  ForeignKey [FK_TransactionsAudit_TransactionStatus]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TransactionsAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsAudit_TransactionStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[TransactionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[TransactionsAudit] NOCHECK CONSTRAINT [FK_TransactionsAudit_TransactionStatus]
GO
/****** Object:  ForeignKey [FK_TariffRateProfile_ConfigProfile]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRateConfigurationProfile]  WITH CHECK ADD  CONSTRAINT [FK_TariffRateProfile_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[TariffRateConfigurationProfile] CHECK CONSTRAINT [FK_TariffRateProfile_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_TariffRateConfiguration_ConfigProfile]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TariffRateConfiguration_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[TariffRateConfiguration] CHECK CONSTRAINT [FK_TariffRateConfiguration_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_TariffRateConfiguration_ID]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TariffRateConfiguration_ID] FOREIGN KEY([TariffRateConfigurationId])
REFERENCES [dbo].[ConfigurationIDGen] ([ConfigurationID])
GO
ALTER TABLE [dbo].[TariffRateConfiguration] CHECK CONSTRAINT [FK_TariffRateConfiguration_ID]
GO
/****** Object:  ForeignKey [FK_TariffRateConfiguration_TariffState]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TariffRateConfiguration_TariffState] FOREIGN KEY([State])
REFERENCES [dbo].[TariffState] ([TariffStateId])
GO
ALTER TABLE [dbo].[TariffRateConfiguration] CHECK CONSTRAINT [FK_TariffRateConfiguration_TariffState]
GO
/****** Object:  ForeignKey [FK_TariffRate_LinkedRate]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRate]  WITH CHECK ADD  CONSTRAINT [FK_TariffRate_LinkedRate] FOREIGN KEY([LinkedRate])
REFERENCES [dbo].[TariffRate] ([TariffRateId])
GO
ALTER TABLE [dbo].[TariffRate] CHECK CONSTRAINT [FK_TariffRate_LinkedRate]
GO
/****** Object:  ForeignKey [FK_TariffRate_MaxTimeUnit]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRate]  WITH CHECK ADD  CONSTRAINT [FK_TariffRate_MaxTimeUnit] FOREIGN KEY([MaxTimeUnit])
REFERENCES [dbo].[TimeUnit] ([TimeUnitId])
GO
ALTER TABLE [dbo].[TariffRate] CHECK CONSTRAINT [FK_TariffRate_MaxTimeUnit]
GO
/****** Object:  ForeignKey [FK_TariffRate_PerTimeUnit]    Script Date: 04/01/2014 22:06:59 ******/
ALTER TABLE [dbo].[TariffRate]  WITH CHECK ADD  CONSTRAINT [FK_TariffRate_PerTimeUnit] FOREIGN KEY([PerTimeUnit])
REFERENCES [dbo].[TimeUnit] ([TimeUnitId])
GO
ALTER TABLE [dbo].[TariffRate] CHECK CONSTRAINT [FK_TariffRate_PerTimeUnit]
GO
/****** Object:  ForeignKey [FK_SFSchedule_SFScheduleType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[SFSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SFSchedule_SFScheduleType] FOREIGN KEY([SFScheduleTypeID])
REFERENCES [dbo].[SFScheduleType] ([Id])
GO
ALTER TABLE [dbo].[SFSchedule] CHECK CONSTRAINT [FK_SFSchedule_SFScheduleType]
GO
/****** Object:  ForeignKey [FK_ConfigProfileSpaceAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ConfigProfileSpaceAudit]  WITH CHECK ADD  CONSTRAINT [FK_ConfigProfileSpaceAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[ConfigProfileSpaceAudit] CHECK CONSTRAINT [FK_ConfigProfileSpaceAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_CreditCardAttempts_TransactionStatus]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CreditCardAttempts]  WITH NOCHECK ADD  CONSTRAINT [FK_CreditCardAttempts_TransactionStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[TransactionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[CreditCardAttempts] NOCHECK CONSTRAINT [FK_CreditCardAttempts_TransactionStatus]
GO
/****** Object:  ForeignKey [FK_EventSourceCustomerId]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[EventSourceCustomer]  WITH CHECK ADD  CONSTRAINT [FK_EventSourceCustomerId] FOREIGN KEY([EventSourceCode])
REFERENCES [dbo].[EventSources] ([EventSourceCode])
GO
ALTER TABLE [dbo].[EventSourceCustomer] CHECK CONSTRAINT [FK_EventSourceCustomerId]
GO
/****** Object:  ForeignKey [FK_FDFileTypeMeterGroup_FDFileType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[FDFileTypeMeterGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_FDFileTypeMeterGroup_FDFileType] FOREIGN KEY([FileType])
REFERENCES [dbo].[FDFileType] ([FileType])
GO
ALTER TABLE [dbo].[FDFileTypeMeterGroup] NOCHECK CONSTRAINT [FK_FDFileTypeMeterGroup_FDFileType]
GO
/****** Object:  ForeignKey [FK_FDFileTypeMeterGroup_MeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[FDFileTypeMeterGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_FDFileTypeMeterGroup_MeterGroup] FOREIGN KEY([MeterGroup])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[FDFileTypeMeterGroup] NOCHECK CONSTRAINT [FK_FDFileTypeMeterGroup_MeterGroup]
GO
/****** Object:  ForeignKey [FK_FDFileTypeMeterGroup_VersionGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[FDFileTypeMeterGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_FDFileTypeMeterGroup_VersionGroup] FOREIGN KEY([VersionGroup])
REFERENCES [dbo].[VersionGroup] ([VersionGroupId])
GO
ALTER TABLE [dbo].[FDFileTypeMeterGroup] NOCHECK CONSTRAINT [FK_FDFileTypeMeterGroup_VersionGroup]
GO
/****** Object:  ForeignKey [FK_FDFiles_FDFileType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[FDFiles]  WITH NOCHECK ADD  CONSTRAINT [FK_FDFiles_FDFileType] FOREIGN KEY([FileType])
REFERENCES [dbo].[FDFileType] ([FileType])
GO
ALTER TABLE [dbo].[FDFiles] NOCHECK CONSTRAINT [FK_FDFiles_FDFileType]
GO
/****** Object:  ForeignKey [FK_EventCodeMaster_AlarmTier]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[EventCodeMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodeMaster_AlarmTier] FOREIGN KEY([AlarmTier])
REFERENCES [dbo].[AlarmTier] ([Tier])
GO
ALTER TABLE [dbo].[EventCodeMaster] NOCHECK CONSTRAINT [FK_EventCodeMaster_AlarmTier]
GO
/****** Object:  ForeignKey [FK_EventCodeMaster_EventSource]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[EventCodeMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodeMaster_EventSource] FOREIGN KEY([EventSource])
REFERENCES [dbo].[EventSources] ([EventSourceCode])
GO
ALTER TABLE [dbo].[EventCodeMaster] NOCHECK CONSTRAINT [FK_EventCodeMaster_EventSource]
GO
/****** Object:  ForeignKey [FK_EventCodeMaster_EventType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[EventCodeMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodeMaster_EventType] FOREIGN KEY([EventType])
REFERENCES [dbo].[EventType] ([EventTypeId])
GO
ALTER TABLE [dbo].[EventCodeMaster] NOCHECK CONSTRAINT [FK_EventCodeMaster_EventType]
GO
/****** Object:  ForeignKey [FK_EventCodeAssetType_MeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[EventCodeAssetType]  WITH NOCHECK ADD  CONSTRAINT [FK_EventCodeAssetType_MeterGroup] FOREIGN KEY([MeterGroupId])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[EventCodeAssetType] NOCHECK CONSTRAINT [FK_EventCodeAssetType_MeterGroup]
GO
/****** Object:  ForeignKey [FK_CashBoxAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBoxAudit]  WITH CHECK ADD  CONSTRAINT [FK_CashBoxAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[CashBoxAudit] CHECK CONSTRAINT [FK_CashBoxAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_AssetPending_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AssetPending]  WITH CHECK ADD  CONSTRAINT [FK_AssetPending_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[AssetPending] CHECK CONSTRAINT [FK_AssetPending_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_MeterMapping_PayByCellVendor]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterMapping]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapping_PayByCellVendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[PayByCellVendor] ([VendorID])
GO
ALTER TABLE [dbo].[MeterMapping] CHECK CONSTRAINT [FK_MeterMapping_PayByCellVendor]
GO
/****** Object:  ForeignKey [FK_OLTAcquirers_CardType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[OLTAcquirers]  WITH NOCHECK ADD  CONSTRAINT [FK_OLTAcquirers_CardType] FOREIGN KEY([CardTypeCode])
REFERENCES [dbo].[CardType] ([CardTypeCode])
GO
ALTER TABLE [dbo].[OLTAcquirers] NOCHECK CONSTRAINT [FK_OLTAcquirers_CardType]
GO
/****** Object:  ForeignKey [FK_ParkingSpace_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ParkingSpacesAudit]  WITH CHECK ADD  CONSTRAINT [FK_ParkingSpace_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[ParkingSpacesAudit] CHECK CONSTRAINT [FK_ParkingSpace_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_ParkSpaceRight_ParkVehicle]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ParkSpaceRight]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkSpaceRight_ParkVehicle] FOREIGN KEY([VehicleId])
REFERENCES [dbo].[ParkVehicle] ([VehicleID])
GO
ALTER TABLE [dbo].[ParkSpaceRight] NOCHECK CONSTRAINT [FK_ParkSpaceRight_ParkVehicle]
GO
/****** Object:  ForeignKey [FK_Processes_ProcessType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[Processes]  WITH NOCHECK ADD  CONSTRAINT [FK_Processes_ProcessType] FOREIGN KEY([ProcessType])
REFERENCES [dbo].[ProcessType] ([ProcessType])
GO
ALTER TABLE [dbo].[Processes] NOCHECK CONSTRAINT [FK_Processes_ProcessType]
GO
/****** Object:  ForeignKey [FK_SensorsAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[SensorsAudit]  WITH CHECK ADD  CONSTRAINT [FK_SensorsAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[SensorsAudit] CHECK CONSTRAINT [FK_SensorsAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_RateScheduleConfigurationProfile_ConfigProfile]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleConfigurationProfile]  WITH CHECK ADD  CONSTRAINT [FK_RateScheduleConfigurationProfile_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[RateScheduleConfigurationProfile] CHECK CONSTRAINT [FK_RateScheduleConfigurationProfile_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_RateScheduleConfiguration_ConfigProfile]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_RateScheduleConfiguration_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[RateScheduleConfiguration] CHECK CONSTRAINT [FK_RateScheduleConfiguration_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_RateScheduleConfiguration_ID]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_RateScheduleConfiguration_ID] FOREIGN KEY([RateScheduleConfigurationId])
REFERENCES [dbo].[ConfigurationIDGen] ([ConfigurationID])
GO
ALTER TABLE [dbo].[RateScheduleConfiguration] CHECK CONSTRAINT [FK_RateScheduleConfiguration_ID]
GO
/****** Object:  ForeignKey [FK_RateScheduleConfiguration_TariffState]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_RateScheduleConfiguration_TariffState] FOREIGN KEY([State])
REFERENCES [dbo].[TariffState] ([TariffStateId])
GO
ALTER TABLE [dbo].[RateScheduleConfiguration] CHECK CONSTRAINT [FK_RateScheduleConfiguration_TariffState]
GO
/****** Object:  ForeignKey [FK_RateSchedule_DayOfWeek]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateSchedule]  WITH CHECK ADD  CONSTRAINT [FK_RateSchedule_DayOfWeek] FOREIGN KEY([DayOfWeek])
REFERENCES [dbo].[DayOfWeek] ([DayOfWeekId])
GO
ALTER TABLE [dbo].[RateSchedule] CHECK CONSTRAINT [FK_RateSchedule_DayOfWeek]
GO
/****** Object:  ForeignKey [FK_RateSchedule_OperationMode]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateSchedule]  WITH CHECK ADD  CONSTRAINT [FK_RateSchedule_OperationMode] FOREIGN KEY([OperationMode])
REFERENCES [dbo].[OperationMode] ([OperationModeId])
GO
ALTER TABLE [dbo].[RateSchedule] CHECK CONSTRAINT [FK_RateSchedule_OperationMode]
GO
/****** Object:  ForeignKey [FK_RateDetail_Transmsn]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateTransmissionDetails]  WITH CHECK ADD  CONSTRAINT [FK_RateDetail_Transmsn] FOREIGN KEY([TransmissionID])
REFERENCES [dbo].[RateTransmission] ([TransmissionID])
GO
ALTER TABLE [dbo].[RateTransmissionDetails] CHECK CONSTRAINT [FK_RateDetail_Transmsn]
GO
/****** Object:  ForeignKey [FK_MeterDiagnosticType_MeterDiagnostic]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterDiagnostic]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterDiagnosticType_MeterDiagnostic] FOREIGN KEY([DiagnosticType])
REFERENCES [dbo].[MeterDiagnosticType] ([ID])
GO
ALTER TABLE [dbo].[MeterDiagnostic] NOCHECK CONSTRAINT [FK_MeterDiagnosticType_MeterDiagnostic]
GO
/****** Object:  ForeignKey [FK_PropertyGroup_LookupItem]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[LookupItem]  WITH NOCHECK ADD  CONSTRAINT [FK_PropertyGroup_LookupItem] FOREIGN KEY([PropertyGroupId])
REFERENCES [dbo].[PropertyGroup] ([PropertyGroupId])
GO
ALTER TABLE [dbo].[LookupItem] NOCHECK CONSTRAINT [FK_PropertyGroup_LookupItem]
GO
/****** Object:  ForeignKey [FK_HolidayRateConfigurationProfile_ConfigProfile]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateConfigurationProfile]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRateConfigurationProfile_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[HolidayRateConfigurationProfile] CHECK CONSTRAINT [FK_HolidayRateConfigurationProfile_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_HolidayRateConfiguration_ConfigProfile]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRateConfiguration_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[HolidayRateConfiguration] CHECK CONSTRAINT [FK_HolidayRateConfiguration_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_HolidayRateConfiguration_ID]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRateConfiguration_ID] FOREIGN KEY([HolidayRateConfigurationId])
REFERENCES [dbo].[ConfigurationIDGen] ([ConfigurationID])
GO
ALTER TABLE [dbo].[HolidayRateConfiguration] CHECK CONSTRAINT [FK_HolidayRateConfiguration_ID]
GO
/****** Object:  ForeignKey [FK_HolidayRateConfiguration_TariffState]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRateConfiguration_TariffState] FOREIGN KEY([State])
REFERENCES [dbo].[TariffState] ([TariffStateId])
GO
ALTER TABLE [dbo].[HolidayRateConfiguration] CHECK CONSTRAINT [FK_HolidayRateConfiguration_TariffState]
GO
/****** Object:  ForeignKey [FK_MaintRouteSeq_MaintRoute]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MaintRouteSeq]  WITH NOCHECK ADD  CONSTRAINT [FK_MaintRouteSeq_MaintRoute] FOREIGN KEY([MaintRouteId])
REFERENCES [dbo].[MaintRoute] ([MaintRouteId])
GO
ALTER TABLE [dbo].[MaintRouteSeq] NOCHECK CONSTRAINT [FK_MaintRouteSeq_MaintRoute]
GO
/****** Object:  ForeignKey [FK_GatewaysAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[GatewaysAudit]  WITH CHECK ADD  CONSTRAINT [FK_GatewaysAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[GatewaysAudit] CHECK CONSTRAINT [FK_GatewaysAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_MechMaster_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MechMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_MechMaster_Customers] FOREIGN KEY([Customerid])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[MechMaster] NOCHECK CONSTRAINT [FK_MechMaster_Customers]
GO
/****** Object:  ForeignKey [FK_MechMaster_InactiveRemarks]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MechMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_MechMaster_InactiveRemarks] FOREIGN KEY([InactiveRemarkID])
REFERENCES [dbo].[InactiveRemarks] ([InactiveRemarkID])
GO
ALTER TABLE [dbo].[MechMaster] NOCHECK CONSTRAINT [FK_MechMaster_InactiveRemarks]
GO
/****** Object:  ForeignKey [FK_MechMaster_MechType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MechMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_MechMaster_MechType] FOREIGN KEY([MechType])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[MechMaster] NOCHECK CONSTRAINT [FK_MechMaster_MechType]
GO
/****** Object:  ForeignKey [FK_HolidayRates_HolidayRate]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRates_HolidayRate] FOREIGN KEY([HolidayRateId])
REFERENCES [dbo].[HolidayRate] ([HolidayRateId])
GO
ALTER TABLE [dbo].[HolidayRateForConfiguration] CHECK CONSTRAINT [FK_HolidayRates_HolidayRate]
GO
/****** Object:  ForeignKey [FK_HolidayRates_HolidayRateConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HolidayRateForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_HolidayRates_HolidayRateConfiguration] FOREIGN KEY([HolidayRateConfigurationId])
REFERENCES [dbo].[HolidayRateConfiguration] ([HolidayRateConfigurationId])
GO
ALTER TABLE [dbo].[HolidayRateForConfiguration] CHECK CONSTRAINT [FK_HolidayRates_HolidayRateConfiguration]
GO
/****** Object:  ForeignKey [FK_HousingMaster_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_HousingMaster_Customers] FOREIGN KEY([Customerid])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[HousingMaster] NOCHECK CONSTRAINT [FK_HousingMaster_Customers]
GO
/****** Object:  ForeignKey [FK_HousingMaster_HousingTypes]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_HousingMaster_HousingTypes] FOREIGN KEY([HousingTypeID])
REFERENCES [dbo].[HousingTypes] ([HousingTypeID])
GO
ALTER TABLE [dbo].[HousingMaster] NOCHECK CONSTRAINT [FK_HousingMaster_HousingTypes]
GO
/****** Object:  ForeignKey [FK_HousingMaster_InactiveRemarks]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[HousingMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_HousingMaster_InactiveRemarks] FOREIGN KEY([InactiveRemarkID])
REFERENCES [dbo].[InactiveRemarks] ([InactiveRemarkID])
GO
ALTER TABLE [dbo].[HousingMaster] NOCHECK CONSTRAINT [FK_HousingMaster_InactiveRemarks]
GO
/****** Object:  ForeignKey [FK_MeterDiagnosticTypeCustomer_CustomerID]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterDiagnosticTypeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_MeterDiagnosticTypeCustomer_CustomerID] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[MeterDiagnosticTypeCustomer] CHECK CONSTRAINT [FK_MeterDiagnosticTypeCustomer_CustomerID]
GO
/****** Object:  ForeignKey [FK_MeterDiagnosticTypeCustomer_DiagnosticType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[MeterDiagnosticTypeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_MeterDiagnosticTypeCustomer_DiagnosticType] FOREIGN KEY([DiagnosticType])
REFERENCES [dbo].[MeterDiagnosticType] ([ID])
GO
ALTER TABLE [dbo].[MeterDiagnosticTypeCustomer] CHECK CONSTRAINT [FK_MeterDiagnosticTypeCustomer_DiagnosticType]
GO
/****** Object:  ForeignKey [FK_RateSchedules_RateSchedule]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_RateSchedules_RateSchedule] FOREIGN KEY([RateScheduleId])
REFERENCES [dbo].[RateSchedule] ([RateScheduleId])
GO
ALTER TABLE [dbo].[RateScheduleForConfiguration] CHECK CONSTRAINT [FK_RateSchedules_RateSchedule]
GO
/****** Object:  ForeignKey [FK_RateSchedules_RateScheduleConfiguration]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[RateScheduleForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_RateSchedules_RateScheduleConfiguration] FOREIGN KEY([RateScheduleConfigurationId])
REFERENCES [dbo].[RateScheduleConfiguration] ([RateScheduleConfigurationId])
GO
ALTER TABLE [dbo].[RateScheduleForConfiguration] CHECK CONSTRAINT [FK_RateSchedules_RateScheduleConfiguration]
GO
/****** Object:  ForeignKey [FK_Schedules_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[Schedules]  WITH NOCHECK ADD  CONSTRAINT [FK_Schedules_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Schedules] NOCHECK CONSTRAINT [FK_Schedules_Customers]
GO
/****** Object:  ForeignKey [FK_ReportMaster_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[ReportMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_ReportMaster_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[ReportMaster] NOCHECK CONSTRAINT [FK_ReportMaster_Customers]
GO
/****** Object:  ForeignKey [FK_Parts_Mechanism]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[Parts]  WITH CHECK ADD  CONSTRAINT [FK_Parts_Mechanism] FOREIGN KEY([Mechanism])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[Parts] CHECK CONSTRAINT [FK_Parts_Mechanism]
GO
/****** Object:  ForeignKey [FK_Parts_MeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[Parts]  WITH CHECK ADD  CONSTRAINT [FK_Parts_MeterGroup] FOREIGN KEY([MeterGroup])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[Parts] CHECK CONSTRAINT [FK_Parts_MeterGroup]
GO
/****** Object:  ForeignKey [FK_PaymentReceived_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PaymentReceived]  WITH NOCHECK ADD  CONSTRAINT [FK_PaymentReceived_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PaymentReceived] NOCHECK CONSTRAINT [FK_PaymentReceived_Customers]
GO
/****** Object:  ForeignKey [FK_PAMMeterAccess_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMMeterAccess]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMMeterAccess_Customers] FOREIGN KEY([Customerid])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMMeterAccess] NOCHECK CONSTRAINT [FK_PAMMeterAccess_Customers]
GO
/****** Object:  ForeignKey [FK_PAMCustomerMap_Customer]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMCustomerMap]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMCustomerMap_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMCustomerMap] NOCHECK CONSTRAINT [FK_PAMCustomerMap_Customer]
GO
/****** Object:  ForeignKey [FK_PAMClusters_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMClusters]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMClusters_Customers] FOREIGN KEY([Customerid])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMClusters] NOCHECK CONSTRAINT [FK_PAMClusters_Customers]
GO
/****** Object:  ForeignKey [FK_PAMBayExpt_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMBayExpt]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMBayExpt_Customers] FOREIGN KEY([Customerid])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMBayExpt] NOCHECK CONSTRAINT [FK_PAMBayExpt_Customers]
GO
/****** Object:  ForeignKey [FK_PAMActiveCustomers_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[PAMActiveCustomers]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMActiveCustomers_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMActiveCustomers] NOCHECK CONSTRAINT [FK_PAMActiveCustomers_Customers]
GO
/****** Object:  ForeignKey [FK_AssetVersionMaster_FDFileTypeMeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AssetVersionMaster]  WITH NOCHECK ADD  CONSTRAINT [FK_AssetVersionMaster_FDFileTypeMeterGroup] FOREIGN KEY([FDFileTypeMeterGroupID])
REFERENCES [dbo].[FDFileTypeMeterGroup] ([FDFileTypeMeterGroupID])
GO
ALTER TABLE [dbo].[AssetVersionMaster] NOCHECK CONSTRAINT [FK_AssetVersionMaster_FDFileTypeMeterGroup]
GO
/****** Object:  ForeignKey [FK_AssetState_Customer]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AssetStateCustomer]  WITH CHECK ADD  CONSTRAINT [FK_AssetState_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[AssetStateCustomer] CHECK CONSTRAINT [FK_AssetState_Customer]
GO
/****** Object:  ForeignKey [FK_AssetStateCustomer]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AssetStateCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_AssetStateCustomer] FOREIGN KEY([AssetStateId])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[AssetStateCustomer] NOCHECK CONSTRAINT [FK_AssetStateCustomer]
GO
/****** Object:  ForeignKey [FK_AcquirerBatch_Customer]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AcquirerBatch]  WITH NOCHECK ADD  CONSTRAINT [FK_AcquirerBatch_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[AcquirerBatch] NOCHECK CONSTRAINT [FK_AcquirerBatch_Customer]
GO
/****** Object:  ForeignKey [FK_AuditRegistry_Customer]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[AuditRegistry]  WITH NOCHECK ADD  CONSTRAINT [FK_AuditRegistry_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[AuditRegistry] NOCHECK CONSTRAINT [FK_AuditRegistry_Customer]
GO
/****** Object:  ForeignKey [FK_BlackListActive_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[BlackListActive]  WITH NOCHECK ADD  CONSTRAINT [FK_BlackListActive_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[BlackListActive] NOCHECK CONSTRAINT [FK_BlackListActive_Customers]
GO
/****** Object:  ForeignKey [FK_CashBox_AssetState]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH CHECK ADD  CONSTRAINT [FK_CashBox_AssetState] FOREIGN KEY([CashBoxState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[CashBox] CHECK CONSTRAINT [FK_CashBox_AssetState]
GO
/****** Object:  ForeignKey [FK_CashBox_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBox_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CashBox] NOCHECK CONSTRAINT [FK_CashBox_Customers]
GO
/****** Object:  ForeignKey [FK_CashBox_LocType]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBox_LocType] FOREIGN KEY([CashBoxLocationTypeId])
REFERENCES [dbo].[CashBoxLocationType] ([CashBoxLocationTypeId])
GO
ALTER TABLE [dbo].[CashBox] NOCHECK CONSTRAINT [FK_CashBox_LocType]
GO
/****** Object:  ForeignKey [FK_CashBox_MechanismMaster]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBox_MechanismMaster] FOREIGN KEY([CashBoxModel])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[CashBox] NOCHECK CONSTRAINT [FK_CashBox_MechanismMaster]
GO
/****** Object:  ForeignKey [FK_CashBox_MeterGroup]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBox_MeterGroup] FOREIGN KEY([CashBoxType])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[CashBox] NOCHECK CONSTRAINT [FK_CashBox_MeterGroup]
GO
/****** Object:  ForeignKey [FK_CashBox_OperationalStatus]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CashBox]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBox_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[CashBox] NOCHECK CONSTRAINT [FK_CashBox_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_BlackListFiles_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[BlackListFiles]  WITH NOCHECK ADD  CONSTRAINT [FK_BlackListFiles_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[BlackListFiles] NOCHECK CONSTRAINT [FK_BlackListFiles_Customers]
GO
/****** Object:  ForeignKey [FK_CBImpFiles_Customers]    Script Date: 04/01/2014 22:07:00 ******/
ALTER TABLE [dbo].[CBImpFiles]  WITH NOCHECK ADD  CONSTRAINT [FK_CBImpFiles_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CBImpFiles] NOCHECK CONSTRAINT [FK_CBImpFiles_Customers]
GO
/****** Object:  ForeignKey [FK_PK_CustomerPropertyGroup_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CustomerPropertyGroup]  WITH NOCHECK ADD  CONSTRAINT [FK_PK_CustomerPropertyGroup_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerPropertyGroup] NOCHECK CONSTRAINT [FK_PK_CustomerPropertyGroup_Customers]
GO
/****** Object:  ForeignKey [FK_DiscountUserCard_User]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountUserCard]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountUserCard_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[DiscountUserCard] NOCHECK CONSTRAINT [FK_DiscountUserCard_User]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeEmailTemplate_Customer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountSchemeEmailTemplate]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeEmailTemplate_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[DiscountSchemeEmailTemplate] CHECK CONSTRAINT [FK_DiscountSchemeEmailTemplate_Customer]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeEmailTemplate_Type]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountSchemeEmailTemplate]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeEmailTemplate_Type] FOREIGN KEY([EmailTemplateTypeId])
REFERENCES [dbo].[EmailTemplateType] ([EmailTemplateTypeId])
GO
ALTER TABLE [dbo].[DiscountSchemeEmailTemplate] CHECK CONSTRAINT [FK_DiscountSchemeEmailTemplate_Type]
GO
/****** Object:  ForeignKey [FK_EnforceRoute_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[EnforceRoute]  WITH NOCHECK ADD  CONSTRAINT [FK_EnforceRoute_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[EnforceRoute] NOCHECK CONSTRAINT [FK_EnforceRoute_Customers]
GO
/****** Object:  ForeignKey [FK_FileTypeMap_FDFileType]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[FileTypeMap]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeMap_FDFileType] FOREIGN KEY([FileType])
REFERENCES [dbo].[FDFileType] ([FileType])
GO
ALTER TABLE [dbo].[FileTypeMap] CHECK CONSTRAINT [FK_FileTypeMap_FDFileType]
GO
/****** Object:  ForeignKey [FK_FileTypeMap_MechanismMaster]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[FileTypeMap]  WITH CHECK ADD  CONSTRAINT [FK_FileTypeMap_MechanismMaster] FOREIGN KEY([MechanismId])
REFERENCES [dbo].[MechanismMaster] ([MechanismId])
GO
ALTER TABLE [dbo].[FileTypeMap] CHECK CONSTRAINT [FK_FileTypeMap_MechanismMaster]
GO
/****** Object:  ForeignKey [FK_CollRoute_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CollRoute]  WITH NOCHECK ADD  CONSTRAINT [FK_CollRoute_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CollRoute] NOCHECK CONSTRAINT [FK_CollRoute_Customers]
GO
/****** Object:  ForeignKey [FK_DiscountScheme_CustomerId]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountScheme]  WITH CHECK ADD  CONSTRAINT [FK_DiscountScheme_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[DiscountScheme] CHECK CONSTRAINT [FK_DiscountScheme_CustomerId]
GO
/****** Object:  ForeignKey [FK_DiscountScheme_DiscountSchemeType]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountScheme_DiscountSchemeType] FOREIGN KEY([SchemeType])
REFERENCES [dbo].[DiscountSchemeType] ([DiscountSchemeTypeId])
GO
ALTER TABLE [dbo].[DiscountScheme] NOCHECK CONSTRAINT [FK_DiscountScheme_DiscountSchemeType]
GO
/****** Object:  ForeignKey [FK_DiscountScheme_ExpirationType]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DiscountScheme]  WITH CHECK ADD  CONSTRAINT [FK_DiscountScheme_ExpirationType] FOREIGN KEY([DiscountSchemeExpirationTypeId])
REFERENCES [dbo].[DiscountSchemeExpirationType] ([DiscountSchemeExpirationTypeId])
GO
ALTER TABLE [dbo].[DiscountScheme] CHECK CONSTRAINT [FK_DiscountScheme_ExpirationType]
GO
/****** Object:  ForeignKey [FK_DemandZone_Customer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DemandZoneCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DemandZone_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[DemandZoneCustomer] CHECK CONSTRAINT [FK_DemandZone_Customer]
GO
/****** Object:  ForeignKey [FK_DemandZone_DemandZone]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[DemandZoneCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DemandZone_DemandZone] FOREIGN KEY([DemandZoneId])
REFERENCES [dbo].[DemandZone] ([DemandZoneId])
GO
ALTER TABLE [dbo].[DemandZoneCustomer] CHECK CONSTRAINT [FK_DemandZone_DemandZone]
GO
/****** Object:  ForeignKey [FK_CoinDenominationCustomer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CoinDenominationCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_CoinDenominationCustomer] FOREIGN KEY([CoinDenominationId])
REFERENCES [dbo].[CoinDenomination] ([CoinDenominationId])
GO
ALTER TABLE [dbo].[CoinDenominationCustomer] NOCHECK CONSTRAINT [FK_CoinDenominationCustomer]
GO
/****** Object:  ForeignKey [FK_CoinDenominationCustomer_Customer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CoinDenominationCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_CoinDenominationCustomer_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CoinDenominationCustomer] NOCHECK CONSTRAINT [FK_CoinDenominationCustomer_Customer]
GO
/****** Object:  ForeignKey [FK_CreditCardTypes_Customer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CreditCardTypesCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_CreditCardTypes_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CreditCardTypesCustomer] NOCHECK CONSTRAINT [FK_CreditCardTypes_Customer]
GO
/****** Object:  ForeignKey [FK_CreditCardTypesCustomer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CreditCardTypesCustomer]  WITH NOCHECK ADD  CONSTRAINT [FK_CreditCardTypesCustomer] FOREIGN KEY([CreditCardType])
REFERENCES [dbo].[CreditCardTypes] ([CreditCardType])
GO
ALTER TABLE [dbo].[CreditCardTypesCustomer] NOCHECK CONSTRAINT [FK_CreditCardTypesCustomer]
GO
/****** Object:  ForeignKey [FK_CustomGroup3_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CustomGroup3]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomGroup3_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomGroup3] NOCHECK CONSTRAINT [FK_CustomGroup3_Customers]
GO
/****** Object:  ForeignKey [FK_CustomGroup2_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CustomGroup2]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomGroup2_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomGroup2] NOCHECK CONSTRAINT [FK_CustomGroup2_Customers]
GO
/****** Object:  ForeignKey [FK_CollectionRunVendor_Customer]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[CollectionRunVendor]  WITH NOCHECK ADD  CONSTRAINT [FK_CollectionRunVendor_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CollectionRunVendor] NOCHECK CONSTRAINT [FK_CollectionRunVendor_Customer]
GO
/****** Object:  ForeignKey [FK_Report_Customers]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[Report]  WITH NOCHECK ADD  CONSTRAINT [FK_Report_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[Report] NOCHECK CONSTRAINT [FK_Report_Customers]
GO
/****** Object:  ForeignKey [FK_SFMeteredSpace_SFEventType]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[SFMeteredSpace]  WITH CHECK ADD  CONSTRAINT [FK_SFMeteredSpace_SFEventType] FOREIGN KEY([EventTypeId])
REFERENCES [dbo].[SFEventType] ([Id])
GO
ALTER TABLE [dbo].[SFMeteredSpace] CHECK CONSTRAINT [FK_SFMeteredSpace_SFEventType]
GO
/****** Object:  ForeignKey [FK_SFMeteredSpace_SFMeterMap]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[SFMeteredSpace]  WITH CHECK ADD  CONSTRAINT [FK_SFMeteredSpace_SFMeterMap] FOREIGN KEY([SFMeterMapId])
REFERENCES [dbo].[SFMeterMap] ([Id])
GO
ALTER TABLE [dbo].[SFMeteredSpace] CHECK CONSTRAINT [FK_SFMeteredSpace_SFMeterMap]
GO
/****** Object:  ForeignKey [FK_SFMeteredSpace_SFSchedule]    Script Date: 04/01/2014 22:07:01 ******/
ALTER TABLE [dbo].[SFMeteredSpace]  WITH CHECK ADD  CONSTRAINT [FK_SFMeteredSpace_SFSchedule] FOREIGN KEY([SFScheduleId])
REFERENCES [dbo].[SFSchedule] ([Id])
GO
ALTER TABLE [dbo].[SFMeteredSpace] CHECK CONSTRAINT [FK_SFMeteredSpace_SFSchedule]
GO
/****** Object:  ForeignKey [FK_TariffRates_TariffRate]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TariffRateForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TariffRates_TariffRate] FOREIGN KEY([TariffRateId])
REFERENCES [dbo].[TariffRate] ([TariffRateId])
GO
ALTER TABLE [dbo].[TariffRateForConfiguration] CHECK CONSTRAINT [FK_TariffRates_TariffRate]
GO
/****** Object:  ForeignKey [FK_TariffRates_TariffRateConfiguration]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TariffRateForConfiguration]  WITH CHECK ADD  CONSTRAINT [FK_TariffRates_TariffRateConfiguration] FOREIGN KEY([TariffRateConfigurationId])
REFERENCES [dbo].[TariffRateConfiguration] ([TariffRateConfigurationId])
GO
ALTER TABLE [dbo].[TariffRateForConfiguration] CHECK CONSTRAINT [FK_TariffRates_TariffRateConfiguration]
GO
/****** Object:  ForeignKey [FK_TransactionsAcquirerResp_GatewayResp]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TransactionsAcquirerResp]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsAcquirerResp_GatewayResp] FOREIGN KEY([transType])
REFERENCES [dbo].[GatewayResp] ([ResponseType])
GO
ALTER TABLE [dbo].[TransactionsAcquirerResp] NOCHECK CONSTRAINT [FK_TransactionsAcquirerResp_GatewayResp]
GO
/****** Object:  ForeignKey [FK_TransactionsAcquirerResp_TransactionsAudit]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TransactionsAcquirerResp]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsAcquirerResp_TransactionsAudit] FOREIGN KEY([TransAuditID])
REFERENCES [dbo].[TransactionsAudit] ([TransAuditId])
GO
ALTER TABLE [dbo].[TransactionsAcquirerResp] NOCHECK CONSTRAINT [FK_TransactionsAcquirerResp_TransactionsAudit]
GO
/****** Object:  ForeignKey [FK_TechnicianKeyChangeLog_Customers]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TechnicianKeyChangeLog]  WITH NOCHECK ADD  CONSTRAINT [FK_TechnicianKeyChangeLog_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[TechnicianKeyChangeLog] NOCHECK CONSTRAINT [FK_TechnicianKeyChangeLog_Customers]
GO
/****** Object:  ForeignKey [FK_SubArea_CustomerId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SubAreas]  WITH NOCHECK ADD  CONSTRAINT [FK_SubArea_CustomerId] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SubAreas] NOCHECK CONSTRAINT [FK_SubArea_CustomerId]
GO
/****** Object:  ForeignKey [FK_SupportedCreditCards_Banks]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SupportedCreditCards]  WITH NOCHECK ADD  CONSTRAINT [FK_SupportedCreditCards_Banks] FOREIGN KEY([BankID])
REFERENCES [dbo].[Banks] ([BankID])
GO
ALTER TABLE [dbo].[SupportedCreditCards] NOCHECK CONSTRAINT [FK_SupportedCreditCards_Banks]
GO
/****** Object:  ForeignKey [FK_SupportedCreditCards_CreditCardTypes]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SupportedCreditCards]  WITH NOCHECK ADD  CONSTRAINT [FK_SupportedCreditCards_CreditCardTypes] FOREIGN KEY([CreditCardType])
REFERENCES [dbo].[CreditCardTypes] ([CreditCardType])
GO
ALTER TABLE [dbo].[SupportedCreditCards] NOCHECK CONSTRAINT [FK_SupportedCreditCards_CreditCardTypes]
GO
/****** Object:  ForeignKey [FK_SupportedCreditCards_Customers]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SupportedCreditCards]  WITH NOCHECK ADD  CONSTRAINT [FK_SupportedCreditCards_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SupportedCreditCards] NOCHECK CONSTRAINT [FK_SupportedCreditCards_Customers]
GO
/****** Object:  ForeignKey [FK_TransactionsReconcile_CardType]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TransactionsReconcile]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsReconcile_CardType] FOREIGN KEY([CardTypeCode])
REFERENCES [dbo].[CardType] ([CardTypeCode])
GO
ALTER TABLE [dbo].[TransactionsReconcile] NOCHECK CONSTRAINT [FK_TransactionsReconcile_CardType]
GO
/****** Object:  ForeignKey [fk_RoleId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[webpages_Roles] ([RoleId])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_RoleId]
GO
/****** Object:  ForeignKey [fk_UserId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[webpages_UsersInRoles]  WITH CHECK ADD  CONSTRAINT [fk_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[webpages_UsersInRoles] CHECK CONSTRAINT [fk_UserId]
GO
/****** Object:  ForeignKey [FK_WorkOrdersAudit]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[WorkOrdersAudit]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrdersAudit] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrdersAudit] CHECK CONSTRAINT [FK_WorkOrdersAudit]
GO
/****** Object:  ForeignKey [FK_WorkOrderImage]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[WorkOrderImage]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderImage] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderImage] CHECK CONSTRAINT [FK_WorkOrderImage]
GO
/****** Object:  ForeignKey [FK_WorkOrderEvent]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[WorkOrderEvent]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderEvent] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderEvent] CHECK CONSTRAINT [FK_WorkOrderEvent]
GO
/****** Object:  ForeignKey [FK_ZoneSeq_CustomerId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[ZoneSeq]  WITH CHECK ADD  CONSTRAINT [FK_ZoneSeq_CustomerId] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[ZoneSeq] CHECK CONSTRAINT [FK_ZoneSeq_CustomerId]
GO
/****** Object:  ForeignKey [FK_Zoneseq_Zones]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[ZoneSeq]  WITH CHECK ADD  CONSTRAINT [FK_Zoneseq_Zones] FOREIGN KEY([ZoneId])
REFERENCES [dbo].[Zones] ([ZoneId])
GO
ALTER TABLE [dbo].[ZoneSeq] CHECK CONSTRAINT [FK_Zoneseq_Zones]
GO
/****** Object:  ForeignKey [FK_WorkOrder_Part]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[WorkOrderPart]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrder_Part] FOREIGN KEY([PartId])
REFERENCES [dbo].[Parts] ([PartId])
GO
ALTER TABLE [dbo].[WorkOrderPart] CHECK CONSTRAINT [FK_WorkOrder_Part]
GO
/****** Object:  ForeignKey [FK_WorkOrderPart]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[WorkOrderPart]  WITH CHECK ADD  CONSTRAINT [FK_WorkOrderPart] FOREIGN KEY([WorkOrderId])
REFERENCES [dbo].[WorkOrders] ([WorkOrderId])
GO
ALTER TABLE [dbo].[WorkOrderPart] CHECK CONSTRAINT [FK_WorkOrderPart]
GO
/****** Object:  ForeignKey [FK_TransactionBatch_AcquirerBatch]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[TransactionBatch]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionBatch_AcquirerBatch] FOREIGN KEY([BatchRef])
REFERENCES [dbo].[AcquirerBatch] ([BatchRef])
GO
ALTER TABLE [dbo].[TransactionBatch] NOCHECK CONSTRAINT [FK_TransactionBatch_AcquirerBatch]
GO
/****** Object:  ForeignKey [FK_SubAreaSeq_Subarea]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SubAreaSeq]  WITH NOCHECK ADD  CONSTRAINT [FK_SubAreaSeq_Subarea] FOREIGN KEY([CustomerID], [SubAreaID])
REFERENCES [dbo].[SubAreas] ([CustomerID], [SubAreaID])
GO
ALTER TABLE [dbo].[SubAreaSeq] NOCHECK CONSTRAINT [FK_SubAreaSeq_Subarea]
GO
/****** Object:  ForeignKey [FK__SensorPay__Parki__61DB776A]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_ArrivalPSOAuditId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_ArrivalPSOAuditId] FOREIGN KEY([ArrivalPSOAuditId])
REFERENCES [dbo].[ParkingSpaceOccupancyAudit] ([AuditID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] NOCHECK CONSTRAINT [FK_SensorPaymentTransaction_ArrivalPSOAuditId]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_Customer]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] CHECK CONSTRAINT [FK_SensorPaymentTransaction_Customer]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_DeparturePSOAuditId]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_DeparturePSOAuditId] FOREIGN KEY([DeparturePSOAuditId])
REFERENCES [dbo].[ParkingSpaceOccupancyAudit] ([AuditID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] NOCHECK CONSTRAINT [FK_SensorPaymentTransaction_DeparturePSOAuditId]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_Gateway]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_Gateway] FOREIGN KEY([CustomerId], [GatewayId])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] CHECK CONSTRAINT [FK_SensorPaymentTransaction_Gateway]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_NonCompliantStatus]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_NonCompliantStatus] FOREIGN KEY([NonCompliantStatus])
REFERENCES [dbo].[NonCompliantStatus] ([NonCompliantStatusID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] NOCHECK CONSTRAINT [FK_SensorPaymentTransaction_NonCompliantStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_OccupancyStatus]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_OccupancyStatus] FOREIGN KEY([OccupancyStatus])
REFERENCES [dbo].[OccupancyStatus] ([StatusID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] NOCHECK CONSTRAINT [FK_SensorPaymentTransaction_OccupancyStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_OperationalStatus]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_OperationalStatus] FOREIGN KEY([OperationalStatus])
REFERENCES [dbo].[OperationalStatus] ([OperationalStatusId])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] NOCHECK CONSTRAINT [FK_SensorPaymentTransaction_OperationalStatus]
GO
/****** Object:  ForeignKey [FK_SensorPaymentTransaction_Sensor]    Script Date: 04/01/2014 22:07:04 ******/
ALTER TABLE [dbo].[SensorPaymentTransaction]  WITH CHECK ADD  CONSTRAINT [FK_SensorPaymentTransaction_Sensor] FOREIGN KEY([CustomerId], [SensorId])
REFERENCES [dbo].[Sensors] ([CustomerID], [SensorID])
GO
ALTER TABLE [dbo].[SensorPaymentTransaction] CHECK CONSTRAINT [FK_SensorPaymentTransaction_Sensor]
GO
/****** Object:  ForeignKey [FK_SFSpecialEvent_SFMeteredSpace]    Script Date: 04/01/2014 22:07:08 ******/
ALTER TABLE [dbo].[SFSpecialEvent]  WITH CHECK ADD  CONSTRAINT [FK_SFSpecialEvent_SFMeteredSpace] FOREIGN KEY([SFMeteredSpaceId])
REFERENCES [dbo].[SFMeteredSpace] ([Id])
GO
ALTER TABLE [dbo].[SFSpecialEvent] CHECK CONSTRAINT [FK_SFSpecialEvent_SFMeteredSpace]
GO
/****** Object:  ForeignKey [FK_CollRouteSeq_CollRoute]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[CollRouteSeq]  WITH CHECK ADD  CONSTRAINT [FK_CollRouteSeq_CollRoute] FOREIGN KEY([CollRouteId])
REFERENCES [dbo].[CollRoute] ([CollRouteId])
GO
ALTER TABLE [dbo].[CollRouteSeq] CHECK CONSTRAINT [FK_CollRouteSeq_CollRoute]
GO
/****** Object:  ForeignKey [FK_CollRouteManualAmount_CollRoute]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[CollRouteManualAmount]  WITH NOCHECK ADD  CONSTRAINT [FK_CollRouteManualAmount_CollRoute] FOREIGN KEY([CollRouteId])
REFERENCES [dbo].[CollRoute] ([CollRouteId])
GO
ALTER TABLE [dbo].[CollRouteManualAmount] NOCHECK CONSTRAINT [FK_CollRouteManualAmount_CollRoute]
GO
/****** Object:  ForeignKey [FK_CollRouteManualAmount_Customers]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[CollRouteManualAmount]  WITH NOCHECK ADD  CONSTRAINT [FK_CollRouteManualAmount_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CollRouteManualAmount] NOCHECK CONSTRAINT [FK_CollRouteManualAmount_Customers]
GO
/****** Object:  ForeignKey [FK_EnforceRouteSeq_EnforceRoute]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[EnforceRouteSeq]  WITH NOCHECK ADD  CONSTRAINT [FK_EnforceRouteSeq_EnforceRoute] FOREIGN KEY([EnforceRouteId])
REFERENCES [dbo].[EnforceRoute] ([EnfRouteId])
GO
ALTER TABLE [dbo].[EnforceRouteSeq] NOCHECK CONSTRAINT [FK_EnforceRouteSeq_EnforceRoute]
GO
/****** Object:  ForeignKey [FK_FDHousingAudit_FDFiles]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[FDHousingAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousingAudit_FDFiles] FOREIGN KEY([FileID])
REFERENCES [dbo].[FDFiles] ([FileID])
GO
ALTER TABLE [dbo].[FDHousingAudit] NOCHECK CONSTRAINT [FK_FDHousingAudit_FDFiles]
GO
/****** Object:  ForeignKey [FK_FDHousingAudit_FDFileType]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[FDHousingAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousingAudit_FDFileType] FOREIGN KEY([FileType])
REFERENCES [dbo].[FDFileType] ([FileType])
GO
ALTER TABLE [dbo].[FDHousingAudit] NOCHECK CONSTRAINT [FK_FDHousingAudit_FDFileType]
GO
/****** Object:  ForeignKey [FK_FDHousingAudit_HousingMaster]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[FDHousingAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousingAudit_HousingMaster] FOREIGN KEY([HousingId])
REFERENCES [dbo].[HousingMaster] ([HousingId])
GO
ALTER TABLE [dbo].[FDHousingAudit] NOCHECK CONSTRAINT [FK_FDHousingAudit_HousingMaster]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeCustomerInfo_Customer]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeCustomerInfo_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo] CHECK CONSTRAINT [FK_DiscountSchemeCustomerInfo_Customer]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeCustomerInfo_EmailTemplate]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeCustomerInfo_EmailTemplate] FOREIGN KEY([DiscountSchemeEmailTemplateId])
REFERENCES [dbo].[DiscountSchemeEmailTemplate] ([DiscountSchemeEmailTemplateId])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo] CHECK CONSTRAINT [FK_DiscountSchemeCustomerInfo_EmailTemplate]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeCustomerInfo_Scheme]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeCustomerInfo_Scheme] FOREIGN KEY([DiscountSchemeId])
REFERENCES [dbo].[DiscountScheme] ([DiscountSchemeID])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomerInfo] CHECK CONSTRAINT [FK_DiscountSchemeCustomerInfo_Scheme]
GO
/****** Object:  ForeignKey [FK_DiscountScheme_Customer]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DiscountScheme_Customer] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomer] CHECK CONSTRAINT [FK_DiscountScheme_Customer]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeCustomer]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeCustomer] FOREIGN KEY([DiscountSchemeId])
REFERENCES [dbo].[DiscountScheme] ([DiscountSchemeID])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomer] CHECK CONSTRAINT [FK_DiscountSchemeCustomer]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeCustomer_AssetState]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountSchemeCustomer]  WITH CHECK ADD  CONSTRAINT [FK_DiscountSchemeCustomer_AssetState] FOREIGN KEY([SchemeState])
REFERENCES [dbo].[AssetState] ([AssetStateId])
GO
ALTER TABLE [dbo].[DiscountSchemeCustomer] CHECK CONSTRAINT [FK_DiscountSchemeCustomer_AssetState]
GO
/****** Object:  ForeignKey [FK_DiscountUserScheme_DiscountScheme]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountUserScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountUserScheme_DiscountScheme] FOREIGN KEY([SchemeId])
REFERENCES [dbo].[DiscountScheme] ([DiscountSchemeID])
GO
ALTER TABLE [dbo].[DiscountUserScheme] NOCHECK CONSTRAINT [FK_DiscountUserScheme_DiscountScheme]
GO
/****** Object:  ForeignKey [FK_DiscountUserScheme_SchemeStatus]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountUserScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountUserScheme_SchemeStatus] FOREIGN KEY([SchemeStatus])
REFERENCES [dbo].[DiscountSchemeStatus] ([DiscountSchemeStatusId])
GO
ALTER TABLE [dbo].[DiscountUserScheme] NOCHECK CONSTRAINT [FK_DiscountUserScheme_SchemeStatus]
GO
/****** Object:  ForeignKey [FK_PK_DiscountUserScheme_DiscountUserCard]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountUserScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_PK_DiscountUserScheme_DiscountUserCard] FOREIGN KEY([CardId])
REFERENCES [dbo].[DiscountUserCard] ([CardID])
GO
ALTER TABLE [dbo].[DiscountUserScheme] NOCHECK CONSTRAINT [FK_PK_DiscountUserScheme_DiscountUserCard]
GO
/****** Object:  ForeignKey [FK_PK_DiscountUserScheme_Users]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[DiscountUserScheme]  WITH NOCHECK ADD  CONSTRAINT [FK_PK_DiscountUserScheme_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[DiscountUserScheme] NOCHECK CONSTRAINT [FK_PK_DiscountUserScheme_Users]
GO
/****** Object:  ForeignKey [FK_CustomerProperty_CustomerPropertyGroup]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[CustomerProperty]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomerProperty_CustomerPropertyGroup] FOREIGN KEY([CustomerID], [CustomerPropertyGroupId])
REFERENCES [dbo].[CustomerPropertyGroup] ([CustomerID], [CustomerPropertyGroupId])
GO
ALTER TABLE [dbo].[CustomerProperty] NOCHECK CONSTRAINT [FK_CustomerProperty_CustomerPropertyGroup]
GO
/****** Object:  ForeignKey [FK_AreaSeq_Areas]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[AreaSeq]  WITH NOCHECK ADD  CONSTRAINT [FK_AreaSeq_Areas] FOREIGN KEY([CustomerID], [AreaId])
REFERENCES [dbo].[Areas] ([CustomerID], [AreaID])
GO
ALTER TABLE [dbo].[AreaSeq] NOCHECK CONSTRAINT [FK_AreaSeq_Areas]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_CustomGroup1]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_CustomGroup1] FOREIGN KEY([CustomGroup1])
REFERENCES [dbo].[CustomGroup1] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_CustomGroup1]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_CustomGroup2]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_CustomGroup2] FOREIGN KEY([CustomGroup2])
REFERENCES [dbo].[CustomGroup2] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_CustomGroup2]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_CustomGroup3]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_CustomGroup3] FOREIGN KEY([CustomGroup3])
REFERENCES [dbo].[CustomGroup3] ([CustomGroupId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_CustomGroup3]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_EnforceRoute]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_EnforceRoute] FOREIGN KEY([EnfRouteId])
REFERENCES [dbo].[EnforceRoute] ([EnfRouteId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_EnforceRoute]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_HousingMaster]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_HousingMaster] FOREIGN KEY([HousingId])
REFERENCES [dbo].[HousingMaster] ([HousingId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_HousingMaster]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_MaintRoute]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_MaintRoute] FOREIGN KEY([MaintRouteId])
REFERENCES [dbo].[MaintRoute] ([MaintRouteId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_MaintRoute]
GO
/****** Object:  ForeignKey [FK_MeterMapAudit_MechMaster]    Script Date: 04/01/2014 22:07:11 ******/
ALTER TABLE [dbo].[MeterMapAudit]  WITH CHECK ADD  CONSTRAINT [FK_MeterMapAudit_MechMaster] FOREIGN KEY([MechId])
REFERENCES [dbo].[MechMaster] ([MechId])
GO
ALTER TABLE [dbo].[MeterMapAudit] CHECK CONSTRAINT [FK_MeterMapAudit_MechMaster]
GO
/****** Object:  ForeignKey [FK_ReportDetail_ReportMaster]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[ReportDetail]  WITH NOCHECK ADD  CONSTRAINT [FK_ReportDetail_ReportMaster] FOREIGN KEY([RepId])
REFERENCES [dbo].[ReportMaster] ([RepId])
GO
ALTER TABLE [dbo].[ReportDetail] NOCHECK CONSTRAINT [FK_ReportDetail_ReportMaster]
GO
/****** Object:  ForeignKey [FK_SFMeterSchedule_SFMeteredSpace]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[SFMeterSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SFMeterSchedule_SFMeteredSpace] FOREIGN KEY([SFMeteredSpaceId])
REFERENCES [dbo].[SFMeteredSpace] ([Id])
GO
ALTER TABLE [dbo].[SFMeterSchedule] CHECK CONSTRAINT [FK_SFMeterSchedule_SFMeteredSpace]
GO
/****** Object:  ForeignKey [FK_SFMeterSchedule_SFOperatingScheduleType]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[SFMeterSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SFMeterSchedule_SFOperatingScheduleType] FOREIGN KEY([SFOptSchType])
REFERENCES [dbo].[SFOperatingScheduleType] ([Id])
GO
ALTER TABLE [dbo].[SFMeterSchedule] CHECK CONSTRAINT [FK_SFMeterSchedule_SFOperatingScheduleType]
GO
/****** Object:  ForeignKey [FK_SFMeterPriceSchedule_SFMeteredSpace]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[SFMeterPriceSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SFMeterPriceSchedule_SFMeteredSpace] FOREIGN KEY([SFMeteredSpaceId])
REFERENCES [dbo].[SFMeteredSpace] ([Id])
GO
ALTER TABLE [dbo].[SFMeterPriceSchedule] CHECK CONSTRAINT [FK_SFMeterPriceSchedule_SFMeteredSpace]
GO
/****** Object:  ForeignKey [FK_ScheduleProcesses_Processes]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[ScheduleProcesses]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduleProcesses_Processes] FOREIGN KEY([ProcessID])
REFERENCES [dbo].[Processes] ([ProcessID])
GO
ALTER TABLE [dbo].[ScheduleProcesses] NOCHECK CONSTRAINT [FK_ScheduleProcesses_Processes]
GO
/****** Object:  ForeignKey [FK_ScheduleProcesses_Schedules]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[ScheduleProcesses]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduleProcesses_Schedules] FOREIGN KEY([CustomerID], [ScheduleID])
REFERENCES [dbo].[Schedules] ([CustomerID], [ScheduleID])
GO
ALTER TABLE [dbo].[ScheduleProcesses] NOCHECK CONSTRAINT [FK_ScheduleProcesses_Schedules]
GO
/****** Object:  ForeignKey [FK_MeterStatusEvents_Meters]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[MeterStatusEvents]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterStatusEvents_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[MeterStatusEvents] NOCHECK CONSTRAINT [FK_MeterStatusEvents_Meters]
GO
/****** Object:  ForeignKey [FK_MeterStatusEvents_MeterServiceStatus]    Script Date: 04/01/2014 22:07:12 ******/
ALTER TABLE [dbo].[MeterStatusEvents]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterStatusEvents_MeterServiceStatus] FOREIGN KEY([State])
REFERENCES [dbo].[MeterServiceStatus] ([StatusID])
GO
ALTER TABLE [dbo].[MeterStatusEvents] NOCHECK CONSTRAINT [FK_MeterStatusEvents_MeterServiceStatus]
GO
/****** Object:  ForeignKey [FK_MeterResetSchedule_Meters]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[MeterResetSchedule]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterResetSchedule_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[MeterResetSchedule] NOCHECK CONSTRAINT [FK_MeterResetSchedule_Meters]
GO
/****** Object:  ForeignKey [FK_ScheduledMeters_Meters]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[ScheduledMeters]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledMeters_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[ScheduledMeters] NOCHECK CONSTRAINT [FK_ScheduledMeters_Meters]
GO
/****** Object:  ForeignKey [FK_ScheduledMeters_Schedules]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[ScheduledMeters]  WITH NOCHECK ADD  CONSTRAINT [FK_ScheduledMeters_Schedules] FOREIGN KEY([CustomerID], [ScheduleID])
REFERENCES [dbo].[Schedules] ([CustomerID], [ScheduleID])
GO
ALTER TABLE [dbo].[ScheduledMeters] NOCHECK CONSTRAINT [FK_ScheduledMeters_Schedules]
GO
/****** Object:  ForeignKey [FK_PushStatus_Meters]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[PushStatus]  WITH NOCHECK ADD  CONSTRAINT [FK_PushStatus_Meters] FOREIGN KEY([Customerid], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[PushStatus] NOCHECK CONSTRAINT [FK_PushStatus_Meters]
GO
/****** Object:  ForeignKey [FK_PublicHoliday_Meters]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[PublicHoliday]  WITH NOCHECK ADD  CONSTRAINT [FK_PublicHoliday_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[PublicHoliday] NOCHECK CONSTRAINT [FK_PublicHoliday_Meters]
GO
/****** Object:  ForeignKey [FK_PPOImport_Meters]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[PPOImport]  WITH NOCHECK ADD  CONSTRAINT [FK_PPOImport_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[PPOImport] NOCHECK CONSTRAINT [FK_PPOImport_Meters]
GO
/****** Object:  ForeignKey [FK_PPOImport_PPOStatusCodes]    Script Date: 04/01/2014 22:07:17 ******/
ALTER TABLE [dbo].[PPOImport]  WITH NOCHECK ADD  CONSTRAINT [FK_PPOImport_PPOStatusCodes] FOREIGN KEY([PPOStatusCode])
REFERENCES [dbo].[PPOStatusCodes] ([PPOStatusCode])
GO
ALTER TABLE [dbo].[PPOImport] NOCHECK CONSTRAINT [FK_PPOImport_PPOStatusCodes]
GO
/****** Object:  ForeignKey [FK_MeterInventory_Meters]    Script Date: 04/01/2014 22:07:18 ******/
ALTER TABLE [dbo].[MeterInventory]  WITH NOCHECK ADD  CONSTRAINT [FK_MeterInventory_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[MeterInventory] NOCHECK CONSTRAINT [FK_MeterInventory_Meters]
GO
/****** Object:  ForeignKey [FK_CashBoxDataHistory_Meters]    Script Date: 04/01/2014 22:07:18 ******/
ALTER TABLE [dbo].[CashBoxDataHistory]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBoxDataHistory_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CashBoxDataHistory] NOCHECK CONSTRAINT [FK_CashBoxDataHistory_Meters]
GO
/****** Object:  ForeignKey [FK_CashBoxDataImport_Meters]    Script Date: 04/01/2014 22:07:18 ******/
ALTER TABLE [dbo].[CashBoxDataHistory]  WITH NOCHECK ADD  CONSTRAINT [FK_CashBoxDataImport_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CashBoxDataHistory] NOCHECK CONSTRAINT [FK_CashBoxDataImport_Meters]
GO
/****** Object:  ForeignKey [FK_CollDataSched_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CollDataSched]  WITH NOCHECK ADD  CONSTRAINT [FK_CollDataSched_Meters] FOREIGN KEY([CustomerID], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CollDataSched] NOCHECK CONSTRAINT [FK_CollDataSched_Meters]
GO
/****** Object:  ForeignKey [FK_Baysnapshot_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[BaySnapshot]  WITH NOCHECK ADD  CONSTRAINT [FK_Baysnapshot_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[BaySnapshot] NOCHECK CONSTRAINT [FK_Baysnapshot_Meters]
GO
/****** Object:  ForeignKey [FK_AuditLog_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[AuditLog]  WITH NOCHECK ADD  CONSTRAINT [FK_AuditLog_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[AuditLog] NOCHECK CONSTRAINT [FK_AuditLog_Meters]
GO
/****** Object:  ForeignKey [FK_DiscountUserSchemeAudit]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[DiscountUserSchemeAudit]  WITH CHECK ADD  CONSTRAINT [FK_DiscountUserSchemeAudit] FOREIGN KEY([DiscountUserSchemeId])
REFERENCES [dbo].[DiscountUserScheme] ([DiscountUserSchemeId])
GO
ALTER TABLE [dbo].[DiscountUserSchemeAudit] CHECK CONSTRAINT [FK_DiscountUserSchemeAudit]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeMeter_Meter]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[DiscountSchemeMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountSchemeMeter_Meter] FOREIGN KEY([CustomerID], [AreaID], [MeterID])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[DiscountSchemeMeter] NOCHECK CONSTRAINT [FK_DiscountSchemeMeter_Meter]
GO
/****** Object:  ForeignKey [FK_DiscountSchemeMeter_Scheme]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[DiscountSchemeMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_DiscountSchemeMeter_Scheme] FOREIGN KEY([DiscountSchemeId])
REFERENCES [dbo].[DiscountScheme] ([DiscountSchemeID])
GO
ALTER TABLE [dbo].[DiscountSchemeMeter] NOCHECK CONSTRAINT [FK_DiscountSchemeMeter_Scheme]
GO
/****** Object:  ForeignKey [FK_FDHousing_FDFiles]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDHousing]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousing_FDFiles] FOREIGN KEY([FileID])
REFERENCES [dbo].[FDFiles] ([FileID])
GO
ALTER TABLE [dbo].[FDHousing] NOCHECK CONSTRAINT [FK_FDHousing_FDFiles]
GO
/****** Object:  ForeignKey [FK_FDHousing_FDFileType]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDHousing]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousing_FDFileType] FOREIGN KEY([FileType])
REFERENCES [dbo].[FDFileType] ([FileType])
GO
ALTER TABLE [dbo].[FDHousing] NOCHECK CONSTRAINT [FK_FDHousing_FDFileType]
GO
/****** Object:  ForeignKey [FK_FDHousing_HousingMaster]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDHousing]  WITH NOCHECK ADD  CONSTRAINT [FK_FDHousing_HousingMaster] FOREIGN KEY([HousingId])
REFERENCES [dbo].[HousingMaster] ([HousingId])
GO
ALTER TABLE [dbo].[FDHousing] NOCHECK CONSTRAINT [FK_FDHousing_HousingMaster]
GO
/****** Object:  ForeignKey [FK_FDJobs_FDFiles]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDJobs]  WITH NOCHECK ADD  CONSTRAINT [FK_FDJobs_FDFiles] FOREIGN KEY([FileID])
REFERENCES [dbo].[FDFiles] ([FileID])
GO
ALTER TABLE [dbo].[FDJobs] NOCHECK CONSTRAINT [FK_FDJobs_FDFiles]
GO
/****** Object:  ForeignKey [FK_FDJobs_FDJobStatus]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDJobs]  WITH NOCHECK ADD  CONSTRAINT [FK_FDJobs_FDJobStatus] FOREIGN KEY([JobStatus])
REFERENCES [dbo].[FDJobStatus] ([JobStatus])
GO
ALTER TABLE [dbo].[FDJobs] NOCHECK CONSTRAINT [FK_FDJobs_FDJobStatus]
GO
/****** Object:  ForeignKey [FK_FDJobs_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[FDJobs]  WITH NOCHECK ADD  CONSTRAINT [FK_FDJobs_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[FDJobs] NOCHECK CONSTRAINT [FK_FDJobs_Meters]
GO
/****** Object:  ForeignKey [FK_CustomerFileArchive_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerFileArchive]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomerFileArchive_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CustomerFileArchive] NOCHECK CONSTRAINT [FK_CustomerFileArchive_Meters]
GO
/****** Object:  ForeignKey [FK_CustomerDetails_CustomerPropertyId]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomerDetails_CustomerPropertyId] FOREIGN KEY([CustomerPropertyId])
REFERENCES [dbo].[CustomerProperty] ([CustomerPropertyId])
GO
ALTER TABLE [dbo].[CustomerDetails] NOCHECK CONSTRAINT [FK_CustomerDetails_CustomerPropertyId]
GO
/****** Object:  ForeignKey [FK_CustomerDetails_Customers]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_CustomerDetails_Customers] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[CustomerDetails] NOCHECK CONSTRAINT [FK_CustomerDetails_Customers]
GO
/****** Object:  ForeignKey [FK_CustomerBaseMeterFiles_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CustomerBaseMeterFiles]  WITH CHECK ADD  CONSTRAINT [FK_CustomerBaseMeterFiles_Meters] FOREIGN KEY([CustomerID], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CustomerBaseMeterFiles] CHECK CONSTRAINT [FK_CustomerBaseMeterFiles_Meters]
GO
/****** Object:  ForeignKey [FK_CollDataImport_Meters]    Script Date: 04/01/2014 22:07:19 ******/
ALTER TABLE [dbo].[CollDataImport]  WITH NOCHECK ADD  CONSTRAINT [FK_CollDataImport_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CollDataImport] NOCHECK CONSTRAINT [FK_CollDataImport_Meters]
GO
/****** Object:  ForeignKey [FK_SFLengthOfStay_SFMeterPriceSchedule]    Script Date: 04/01/2014 22:07:21 ******/
ALTER TABLE [dbo].[SFLengthOfStay]  WITH CHECK ADD  CONSTRAINT [FK_SFLengthOfStay_SFMeterPriceSchedule] FOREIGN KEY([SFMeterPriceScheduleId])
REFERENCES [dbo].[SFMeterPriceSchedule] ([Id])
GO
ALTER TABLE [dbo].[SFLengthOfStay] CHECK CONSTRAINT [FK_SFLengthOfStay_SFMeterPriceSchedule]
GO
/****** Object:  ForeignKey [FK_SLA_OperationSchedule_DOW]    Script Date: 04/01/2014 22:07:21 ******/
ALTER TABLE [dbo].[SLA_OperationSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_OperationSchedule_DOW] FOREIGN KEY([DayOfWeek])
REFERENCES [dbo].[DayOfWeek] ([DayOfWeekId])
GO
ALTER TABLE [dbo].[SLA_OperationSchedule] CHECK CONSTRAINT [FK_SLA_OperationSchedule_DOW]
GO
/****** Object:  ForeignKey [FK_SLA_OperationSchedule_Meters]    Script Date: 04/01/2014 22:07:21 ******/
ALTER TABLE [dbo].[SLA_OperationSchedule]  WITH CHECK ADD  CONSTRAINT [FK_SLA_OperationSchedule_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[SLA_OperationSchedule] CHECK CONSTRAINT [FK_SLA_OperationSchedule_Meters]
GO
/****** Object:  ForeignKey [FK_Tariff_Meters]    Script Date: 04/01/2014 22:07:25 ******/
ALTER TABLE [dbo].[Tariff]  WITH CHECK ADD  CONSTRAINT [FK_Tariff_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[Tariff] CHECK CONSTRAINT [FK_Tariff_Meters]
GO
/****** Object:  ForeignKey [FK_TransDataSumm_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[TransDataSumm]  WITH NOCHECK ADD  CONSTRAINT [FK_TransDataSumm_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransDataSumm] NOCHECK CONSTRAINT [FK_TransDataSumm_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionsBlackList_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[TransactionsBlackList]  WITH CHECK ADD  CONSTRAINT [FK_TransactionsBlackList_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsBlackList] CHECK CONSTRAINT [FK_TransactionsBlackList_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionPackages_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[TransactionPackages]  WITH CHECK ADD  CONSTRAINT [FK_TransactionPackages_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionPackages] CHECK CONSTRAINT [FK_TransactionPackages_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionPackages_TransactionPackageStatus]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[TransactionPackages]  WITH CHECK ADD  CONSTRAINT [FK_TransactionPackages_TransactionPackageStatus] FOREIGN KEY([PackageStatus])
REFERENCES [dbo].[TransactionPackageStatus] ([StatusId])
GO
ALTER TABLE [dbo].[TransactionPackages] CHECK CONSTRAINT [FK_TransactionPackages_TransactionPackageStatus]
GO
/****** Object:  ForeignKey [FK_VSNodeStatus_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[VSNodeStatus]  WITH CHECK ADD  CONSTRAINT [FK_VSNodeStatus_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[VSNodeStatus] CHECK CONSTRAINT [FK_VSNodeStatus_Meters]
GO
/****** Object:  ForeignKey [FK_VehicleSensingEvents_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[VehicleSensingEvents]  WITH NOCHECK ADD  CONSTRAINT [FK_VehicleSensingEvents_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[VehicleSensingEvents] NOCHECK CONSTRAINT [FK_VehicleSensingEvents_Meters]
GO
/****** Object:  ForeignKey [FK_VehicleMovements_Meters]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[VehicleMovements]  WITH NOCHECK ADD  CONSTRAINT [FK_VehicleMovements_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[VehicleMovements] NOCHECK CONSTRAINT [FK_VehicleMovements_Meters]
GO
/****** Object:  ForeignKey [FK_VehicleMovements_SensorEvents]    Script Date: 04/01/2014 22:07:27 ******/
ALTER TABLE [dbo].[VehicleMovements]  WITH NOCHECK ADD  CONSTRAINT [FK_VehicleMovements_SensorEvents] FOREIGN KEY([SensorEvent])
REFERENCES [dbo].[SensorEvents] ([SensorEventId])
GO
ALTER TABLE [dbo].[VehicleMovements] NOCHECK CONSTRAINT [FK_VehicleMovements_SensorEvents]
GO
/****** Object:  ForeignKey [FK_TransactionsCashKey_Meters]    Script Date: 04/01/2014 22:07:28 ******/
ALTER TABLE [dbo].[TransactionsCashKey]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsCashKey_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsCashKey] NOCHECK CONSTRAINT [FK_TransactionsCashKey_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionsCashKey_ParkingSpaces]    Script Date: 04/01/2014 22:07:28 ******/
ALTER TABLE [dbo].[TransactionsCashKey]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsCashKey_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[TransactionsCashKey] NOCHECK CONSTRAINT [FK_TransactionsCashKey_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_ConfigProfileSpace_ConfigProfile]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[ConfigProfileSpace]  WITH NOCHECK ADD  CONSTRAINT [FK_ConfigProfileSpace_ConfigProfile] FOREIGN KEY([ConfigProfileId])
REFERENCES [dbo].[ConfigProfile] ([ConfigProfileId])
GO
ALTER TABLE [dbo].[ConfigProfileSpace] NOCHECK CONSTRAINT [FK_ConfigProfileSpace_ConfigProfile]
GO
/****** Object:  ForeignKey [FK_ConfigProfileSpace_Space]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[ConfigProfileSpace]  WITH NOCHECK ADD  CONSTRAINT [FK_ConfigProfileSpace_Space] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ConfigProfileSpace] NOCHECK CONSTRAINT [FK_ConfigProfileSpace_Space]
GO
/****** Object:  ForeignKey [FK_ConfigProfileSpace_Status]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[ConfigProfileSpace]  WITH NOCHECK ADD  CONSTRAINT [FK_ConfigProfileSpace_Status] FOREIGN KEY([ConfigStatus])
REFERENCES [dbo].[ConfigStatus] ([ConfigStatusId])
GO
ALTER TABLE [dbo].[ConfigProfileSpace] NOCHECK CONSTRAINT [FK_ConfigProfileSpace_Status]
GO
/****** Object:  ForeignKey [FK_FDJobHistory_FDJobs]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[FDJobHistory]  WITH CHECK ADD  CONSTRAINT [FK_FDJobHistory_FDJobs] FOREIGN KEY([JobID])
REFERENCES [dbo].[FDJobs] ([JobID])
GO
ALTER TABLE [dbo].[FDJobHistory] CHECK CONSTRAINT [FK_FDJobHistory_FDJobs]
GO
/****** Object:  ForeignKey [FK_FDJobHistory_FDJobStatus]    Script Date: 04/01/2014 22:07:32 ******/
ALTER TABLE [dbo].[FDJobHistory]  WITH CHECK ADD  CONSTRAINT [FK_FDJobHistory_FDJobStatus] FOREIGN KEY([FDJobStatus])
REFERENCES [dbo].[FDJobStatus] ([JobStatus])
GO
ALTER TABLE [dbo].[FDJobHistory] CHECK CONSTRAINT [FK_FDJobHistory_FDJobStatus]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceOccupancy_OccupancyStatus]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceOccupancy]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaceOccupancy_OccupancyStatus] FOREIGN KEY([LastStatus])
REFERENCES [dbo].[OccupancyStatus] ([StatusID])
GO
ALTER TABLE [dbo].[ParkingSpaceOccupancy] NOCHECK CONSTRAINT [FK_ParkingSpaceOccupancy_OccupancyStatus]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceOccupancy_ParkingSpaces]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceOccupancy]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaceOccupancy_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ParkingSpaceOccupancy] NOCHECK CONSTRAINT [FK_ParkingSpaceOccupancy_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_ParkingSpace_MeterBayMap]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceMeterBayMap]  WITH CHECK ADD  CONSTRAINT [FK_ParkingSpace_MeterBayMap] FOREIGN KEY([SensorParkingSpaceID])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ParkingSpaceMeterBayMap] CHECK CONSTRAINT [FK_ParkingSpace_MeterBayMap]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceExpiryConfirmationEvent]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceExpiryConfirmationEvent]  WITH CHECK ADD  CONSTRAINT [FK_ParkingSpaceExpiryConfirmationEvent] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ParkingSpaceExpiryConfirmationEvent] CHECK CONSTRAINT [FK_ParkingSpaceExpiryConfirmationEvent]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceDetails_ParkingSpaces]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaceDetails_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[ParkingSpaceDetails] NOCHECK CONSTRAINT [FK_ParkingSpaceDetails_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceDetails_SpaceType]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSpaceDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaceDetails_SpaceType] FOREIGN KEY([SpaceType])
REFERENCES [dbo].[SpaceType] ([SpaceTypeId])
GO
ALTER TABLE [dbo].[ParkingSpaceDetails] NOCHECK CONSTRAINT [FK_ParkingSpaceDetails_SpaceType]
GO
/****** Object:  ForeignKey [FK_ParkingSchedules_Meters]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSchedules]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSchedules_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[ParkingSchedules] NOCHECK CONSTRAINT [FK_ParkingSchedules_Meters]
GO
/****** Object:  ForeignKey [FK_ParkingSchedules_Tariff]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[ParkingSchedules]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSchedules_Tariff] FOREIGN KEY([CustomerID], [AreaID], [MeterId], [TariffID])
REFERENCES [dbo].[Tariff] ([CustomerID], [AreaID], [MeterId], [TariffID])
GO
ALTER TABLE [dbo].[ParkingSchedules] NOCHECK CONSTRAINT [FK_ParkingSchedules_Tariff]
GO
/****** Object:  ForeignKey [FK_OLTEventDetails_CardType]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[OLTEventDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_OLTEventDetails_CardType] FOREIGN KEY([TransType])
REFERENCES [dbo].[CardType] ([CardTypeCode])
GO
ALTER TABLE [dbo].[OLTEventDetails] NOCHECK CONSTRAINT [FK_OLTEventDetails_CardType]
GO
/****** Object:  ForeignKey [FK_OLTEventDetails_Meters]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[OLTEventDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_OLTEventDetails_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[OLTEventDetails] NOCHECK CONSTRAINT [FK_OLTEventDetails_Meters]
GO
/****** Object:  ForeignKey [FK_OLTEventDetails_ParkingSpaces]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[OLTEventDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_OLTEventDetails_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[OLTEventDetails] NOCHECK CONSTRAINT [FK_OLTEventDetails_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_OLTEventDetails_PaymentTargetType]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[OLTEventDetails]  WITH NOCHECK ADD  CONSTRAINT [FK_OLTEventDetails_PaymentTargetType] FOREIGN KEY([PaymentTarget])
REFERENCES [dbo].[PaymentTargetType] ([TargetType])
GO
ALTER TABLE [dbo].[OLTEventDetails] NOCHECK CONSTRAINT [FK_OLTEventDetails_PaymentTargetType]
GO
/****** Object:  ForeignKey [FK_RegulatedHours_ParkingSpaces]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[RegulatedHours]  WITH NOCHECK ADD  CONSTRAINT [FK_RegulatedHours_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[RegulatedHours] NOCHECK CONSTRAINT [FK_RegulatedHours_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_RateDetail]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[RateTransmissionJob]  WITH CHECK ADD  CONSTRAINT [FK_RateDetail] FOREIGN KEY([TransmissionDetailsID])
REFERENCES [dbo].[RateTransmissionDetails] ([ID])
GO
ALTER TABLE [dbo].[RateTransmissionJob] CHECK CONSTRAINT [FK_RateDetail]
GO
/****** Object:  ForeignKey [FK_RateDetail_Job]    Script Date: 04/01/2014 22:07:33 ******/
ALTER TABLE [dbo].[RateTransmissionJob]  WITH CHECK ADD  CONSTRAINT [FK_RateDetail_Job] FOREIGN KEY([JobID])
REFERENCES [dbo].[FDJobs] ([JobID])
GO
ALTER TABLE [dbo].[RateTransmissionJob] CHECK CONSTRAINT [FK_RateDetail_Job]
GO
/****** Object:  ForeignKey [FK_GSMConnectionLogs_GSMConnectioNStatus]    Script Date: 04/01/2014 22:07:34 ******/
ALTER TABLE [dbo].[GSMConnectionLogs]  WITH NOCHECK ADD  CONSTRAINT [FK_GSMConnectionLogs_GSMConnectioNStatus] FOREIGN KEY([ConnectionStatus])
REFERENCES [dbo].[GSMConnectionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[GSMConnectionLogs] NOCHECK CONSTRAINT [FK_GSMConnectionLogs_GSMConnectioNStatus]
GO
/****** Object:  ForeignKey [FK_GSMConnectionLogs_Meters]    Script Date: 04/01/2014 22:07:34 ******/
ALTER TABLE [dbo].[GSMConnectionLogs]  WITH NOCHECK ADD  CONSTRAINT [FK_GSMConnectionLogs_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[GSMConnectionLogs] NOCHECK CONSTRAINT [FK_GSMConnectionLogs_Meters]
GO
/****** Object:  ForeignKey [FK_CollDataMeterStatus_Meters]    Script Date: 04/01/2014 22:07:41 ******/
ALTER TABLE [dbo].[CollDataMeterStatus]  WITH NOCHECK ADD  CONSTRAINT [FK_CollDataMeterStatus_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[CollDataMeterStatus] NOCHECK CONSTRAINT [FK_CollDataMeterStatus_Meters]
GO
/****** Object:  ForeignKey [FK_TechCredit_Meters]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[TechCreditEvent]  WITH NOCHECK ADD  CONSTRAINT [FK_TechCredit_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TechCreditEvent] NOCHECK CONSTRAINT [FK_TechCredit_Meters]
GO
/****** Object:  ForeignKey [FK_VersionProfileMeter]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionProfileMeter] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[VersionProfileMeter] NOCHECK CONSTRAINT [FK_VersionProfileMeter]
GO
/****** Object:  ForeignKey [FK_VersionProfileMeter_Gateways]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionProfileMeter_Gateways] FOREIGN KEY([CustomerId], [GatewayID])
REFERENCES [dbo].[Gateways] ([CustomerID], [GateWayID])
GO
ALTER TABLE [dbo].[VersionProfileMeter] NOCHECK CONSTRAINT [FK_VersionProfileMeter_Gateways]
GO
/****** Object:  ForeignKey [FK_VersionProfileMeter_MeterGroup]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionProfileMeter_MeterGroup] FOREIGN KEY([MeterGroup])
REFERENCES [dbo].[MeterGroup] ([MeterGroupId])
GO
ALTER TABLE [dbo].[VersionProfileMeter] NOCHECK CONSTRAINT [FK_VersionProfileMeter_MeterGroup]
GO
/****** Object:  ForeignKey [FK_VersionProfileMeter_Sensors]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeter]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionProfileMeter_Sensors] FOREIGN KEY([CustomerId], [SensorID])
REFERENCES [dbo].[Sensors] ([CustomerID], [SensorID])
GO
ALTER TABLE [dbo].[VersionProfileMeter] NOCHECK CONSTRAINT [FK_VersionProfileMeter_Sensors]
GO
/****** Object:  ForeignKey [FK_VersionProfileMeterAudit]    Script Date: 04/01/2014 22:07:45 ******/
ALTER TABLE [dbo].[VersionProfileMeterAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_VersionProfileMeterAudit] FOREIGN KEY([VersionProfileMeterId])
REFERENCES [dbo].[VersionProfileMeter] ([VersionProfileMeterId])
GO
ALTER TABLE [dbo].[VersionProfileMeterAudit] NOCHECK CONSTRAINT [FK_VersionProfileMeterAudit]
GO
/****** Object:  ForeignKey [FK_SensorMappingAudit_AssetPendingReason]    Script Date: 04/01/2014 22:07:56 ******/
ALTER TABLE [dbo].[SensorMappingAudit]  WITH CHECK ADD  CONSTRAINT [FK_SensorMappingAudit_AssetPendingReason] FOREIGN KEY([AssetPendingReasonId])
REFERENCES [dbo].[AssetPendingReason] ([AssetPendingReasonId])
GO
ALTER TABLE [dbo].[SensorMappingAudit] CHECK CONSTRAINT [FK_SensorMappingAudit_AssetPendingReason]
GO
/****** Object:  ForeignKey [FK_SensorMappingAudit_SensorMapping]    Script Date: 04/01/2014 22:07:56 ******/
ALTER TABLE [dbo].[SensorMappingAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_SensorMappingAudit_SensorMapping] FOREIGN KEY([SensorMappingID])
REFERENCES [dbo].[SensorMapping] ([SensorMappingID])
GO
ALTER TABLE [dbo].[SensorMappingAudit] NOCHECK CONSTRAINT [FK_SensorMappingAudit_SensorMapping]
GO
/****** Object:  ForeignKey [FK_TransactionsSmartCard_Meters]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsSmartCard]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsSmartCard_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsSmartCard] NOCHECK CONSTRAINT [FK_TransactionsSmartCard_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionsSmartCard_ParkingSpaces]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsSmartCard]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsSmartCard_ParkingSpaces] FOREIGN KEY([ParkingSpaceID])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[TransactionsSmartCard] NOCHECK CONSTRAINT [FK_TransactionsSmartCard_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_TransactionsSmartCard_TransactionStatus]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsSmartCard]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsSmartCard_TransactionStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[TransactionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[TransactionsSmartCard] NOCHECK CONSTRAINT [FK_TransactionsSmartCard_TransactionStatus]
GO
/****** Object:  ForeignKey [FK_TransactionsMPark_Meters]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsMPark]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsMPark_Meters] FOREIGN KEY([CustomerID], [AreaID], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsMPark] NOCHECK CONSTRAINT [FK_TransactionsMPark_Meters]
GO
/****** Object:  ForeignKey [FK_TransactionsMpark_ParkingSpaces]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsMPark]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsMpark_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[TransactionsMPark] NOCHECK CONSTRAINT [FK_TransactionsMpark_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_TransactionsMPark_PayByCellVendor]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsMPark]  WITH CHECK ADD  CONSTRAINT [FK_TransactionsMPark_PayByCellVendor] FOREIGN KEY([VendorId])
REFERENCES [dbo].[PayByCellVendor] ([VendorID])
GO
ALTER TABLE [dbo].[TransactionsMPark] CHECK CONSTRAINT [FK_TransactionsMPark_PayByCellVendor]
GO
/****** Object:  ForeignKey [FK_TransactionsMPark_TransUserType]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsMPark]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsMPark_TransUserType] FOREIGN KEY([UserType])
REFERENCES [dbo].[TransUserType] ([UserType])
GO
ALTER TABLE [dbo].[TransactionsMPark] NOCHECK CONSTRAINT [FK_TransactionsMPark_TransUserType]
GO
/****** Object:  ForeignKey [FK_TCC_DiscountScheme]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_TCC_DiscountScheme] FOREIGN KEY([DiscountSchemeId])
REFERENCES [dbo].[DiscountScheme] ([DiscountSchemeID])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_TCC_DiscountScheme]
GO
/****** Object:  ForeignKey [FK_Transactions_CreditCardTypes]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_CreditCardTypes] FOREIGN KEY([CreditCardType])
REFERENCES [dbo].[CreditCardTypes] ([CreditCardType])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_Transactions_CreditCardTypes]
GO
/****** Object:  ForeignKey [FK_Transactions_Meters]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_Meters] FOREIGN KEY([CustomerId], [AreaId], [MeterId])
REFERENCES [dbo].[Meters] ([CustomerID], [AreaID], [MeterId])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_Transactions_Meters]
GO
/****** Object:  ForeignKey [FK_Transactions_PaymentTargetType]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_PaymentTargetType] FOREIGN KEY([PaymentTarget])
REFERENCES [dbo].[PaymentTargetType] ([TargetType])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_Transactions_PaymentTargetType]
GO
/****** Object:  ForeignKey [FK_Transactions_TransactionStatus]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_Transactions_TransactionStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[TransactionStatus] ([StatusID])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_Transactions_TransactionStatus]
GO
/****** Object:  ForeignKey [FK_TransactionsCreditCard_ParkingSpaces]    Script Date: 04/01/2014 22:08:02 ******/
ALTER TABLE [dbo].[TransactionsCreditCard]  WITH NOCHECK ADD  CONSTRAINT [FK_TransactionsCreditCard_ParkingSpaces] FOREIGN KEY([ParkingSpaceId])
REFERENCES [dbo].[ParkingSpaces] ([ParkingSpaceId])
GO
ALTER TABLE [dbo].[TransactionsCreditCard] NOCHECK CONSTRAINT [FK_TransactionsCreditCard_ParkingSpaces]
GO
/****** Object:  ForeignKey [FK_ParkingSpaceOccupancyAudit_ParkingSpaceOccupancy]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[ParkingSpaceOccupancyAudit]  WITH NOCHECK ADD  CONSTRAINT [FK_ParkingSpaceOccupancyAudit_ParkingSpaceOccupancy] FOREIGN KEY([ParkingSpaceOccupancyId])
REFERENCES [dbo].[ParkingSpaceOccupancy] ([ParkingSpaceOccupancyId])
GO
ALTER TABLE [dbo].[ParkingSpaceOccupancyAudit] NOCHECK CONSTRAINT [FK_ParkingSpaceOccupancyAudit_ParkingSpaceOccupancy]
GO
/****** Object:  ForeignKey [FK_PAMTx_Customers]    Script Date: 04/01/2014 22:08:04 ******/
ALTER TABLE [dbo].[PAMTx]  WITH NOCHECK ADD  CONSTRAINT [FK_PAMTx_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerID])
GO
ALTER TABLE [dbo].[PAMTx] NOCHECK CONSTRAINT [FK_PAMTx_Customers]
GO
/****** Object:  DdlTrigger [tr_MStran_alterschemaonly]    Script Date: 04/01/2014 22:08:17 ******/
Enable Trigger [tr_MStran_alterschemaonly] ON Database
GO
/****** Object:  DdlTrigger [tr_MStran_altertable]    Script Date: 04/01/2014 22:08:20 ******/
Enable Trigger [tr_MStran_altertable] ON Database
GO
/****** Object:  DdlTrigger [tr_MStran_altertrigger]    Script Date: 04/01/2014 22:08:22 ******/
Enable Trigger [tr_MStran_altertrigger] ON Database
GO
/****** Object:  DdlTrigger [tr_MStran_alterview]    Script Date: 04/01/2014 22:08:25 ******/
Enable Trigger [tr_MStran_alterview] ON Database
GO
