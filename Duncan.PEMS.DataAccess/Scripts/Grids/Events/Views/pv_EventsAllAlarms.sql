GO

/****** Object:  View [dbo].[pv_EventsAllAlarms]    Script Date: 02/24/2014 10:13:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsAllAlarms]
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

