GO

/****** Object:  View [dbo].[pv_EventsDiagnostics]    Script Date: 02/24/2014 10:15:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsDiagnostics]
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

