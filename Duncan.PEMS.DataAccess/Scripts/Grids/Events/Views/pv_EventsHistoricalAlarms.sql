GO

/****** Object:  View [dbo].[pv_EventsHistoricalAlarms]    Script Date: 02/24/2014 10:16:34 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsHistoricalAlarms]
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

