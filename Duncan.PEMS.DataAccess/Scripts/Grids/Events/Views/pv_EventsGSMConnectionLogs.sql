GO

/****** Object:  View [dbo].[pv_EventsGSMConnectionLogs]    Script Date: 02/24/2014 10:15:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsGSMConnectionLogs]
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

