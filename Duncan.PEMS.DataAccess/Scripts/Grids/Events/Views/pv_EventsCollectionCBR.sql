GO

/****** Object:  View [dbo].[pv_EventsCollectionCBR]    Script Date: 12/16/2013 15:26:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsCollectionCBR]
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


