GO

/****** Object:  View [dbo].[pv_ActiveAlarms]    Script Date: 02/24/2014 10:12:13 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


				ALTER VIEW [dbo].[pv_ActiveAlarms]
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
                            WHERE      (AlarmStatusId = 2)) AS AlarmStatus, 
                            CASE 
                            when aa.SLADue <> '' then CAST(DATEDIFF(mi, (SELECT dbo.udf_GetCustomerLocalTime(aa.CustomerID) AS Expr1), aa.SLADue) AS int) 
                            else 0
                            end as TimeRemainingUntilTarget,
                            CAST(DATEDIFF(mi,(SELECT dbo.udf_GetCustomerLocalTime(aa.CustomerID) AS Expr1), aa.SLADue) AS int) AS TotalMinutes,
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
					  FROM dbo.ActiveAlarms AS aa INNER JOIN
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

