GO

/****** Object:  View [dbo].[pv_HistoricAlarms]    Script Date: 02/24/2014 10:18:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


				ALTER VIEW [dbo].[pv_HistoricAlarms]
				AS
				SELECT     ha.CustomerID, ha.TimeOfOccurrance, ha.AlarmUID, dbo.EventCodes.EventCode AS AlarmCode, 
                      dbo.EventCodes.EventDescVerbose AS AlarmCodeDesc,
                          (SELECT     MeterGroupDesc
                            FROM          dbo.AssetType
                            WHERE      (MeterGroupId = dbo.Meters.MeterGroup) AND (CustomerId = ha.CustomerID)) AS AssetType, 
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
                            
                            
                            -- Time remaining until Target 
                            CASE 
                            when ha.SLADue <> '' then CAST(DATEDIFF(mi, ha.TimeOfClearance, ha.SLADue) AS int) 
                            else 0
                            end as TimeRemainingUntilTarget,
                            CAST(DATEDIFF(mi, ha.TimeOfOccurrance, ha.TimeOfClearance) AS int) 
								AS TotalMinutes,
                          (SELECT     AreaName
                            FROM          dbo.Areas
                            WHERE      (AreaID = dbo.MeterMap.AreaId2) AND (CustomerID = ha.CustomerID)) AS Area, dbo.Meters.AreaID, dbo.MeterMap.AreaId2,
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
                            WHERE      (WorkOrderId = ha.WorkOrderId)) AS TechnicianId, 
							  ha.TimeType1, 
							  ha.TimeType2, 
							  ha.TimeType3, 
							  ha.TimeType4, 
							  ha.TimeType5, 
							  ha.MeterId,
                           CASE WHEN CAST(DATEDIFF(mi, ha.TimeOfOccurrance, ha.TimeOfClearance) AS bigint) 
                                 <= 0 THEN								-- Closed-Non-Compliant
								 (	SELECT     TargetServiceDesignationDesc
									FROM    dbo.TargetServiceDesignation
									WHERE   CustomerId = ha.CustomerID
									AND TargetServiceDesignationId = 6)    
                            WHEN CAST(DATEDIFF(mi, ha.TimeOfOccurrance, ha.TimeOfClearance) AS bigint)
                                 > 0 THEN								 -- Closed-Compliant
								(	SELECT     TargetServiceDesignationDesc
									FROM    dbo.TargetServiceDesignation
									WHERE   CustomerId = ha.CustomerID
									AND TargetServiceDesignationId = 5)    
									END AS TargetService, 
                      ha.TimeOfClearance, 
                      dz.DemandZoneDesc AS DemandArea
					  FROM dbo.MeterMap INNER JOIN
                      dbo.HistoricalAlarms as ha ON dbo.MeterMap.Customerid = ha.CustomerID INNER JOIN
                      dbo.Meters ON dbo.MeterMap.Customerid = dbo.Meters.CustomerID AND dbo.MeterMap.Areaid = dbo.Meters.AreaID AND 
                      dbo.MeterMap.MeterId = dbo.Meters.MeterId AND ha.CustomerID = dbo.Meters.CustomerID AND 
                      ha.AreaID = dbo.Meters.AreaID AND ha.MeterId = dbo.Meters.MeterId INNER JOIN
                      dbo.EventCodes ON ha.CustomerID = dbo.EventCodes.CustomerID AND ha.EventSource = dbo.EventCodes.EventSource AND 
                      ha.EventCode = dbo.EventCodes.EventCode LEFT OUTER JOIN
                      dbo.DemandZone AS dz ON dz.DemandZoneId = dbo.Meters.DemandZone
GO

