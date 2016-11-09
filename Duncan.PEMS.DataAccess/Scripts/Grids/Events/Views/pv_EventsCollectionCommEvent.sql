GO

/****** Object:  View [dbo].[pv_EventsCollectionCommEvent]    Script Date: 02/24/2014 10:14:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[pv_EventsCollectionCommEvent]
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

