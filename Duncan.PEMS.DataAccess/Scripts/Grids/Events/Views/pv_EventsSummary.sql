GO

/****** Object:  View [dbo].[pv_EventsSummary]    Script Date: 02/24/2014 10:17:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsSummary]
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

