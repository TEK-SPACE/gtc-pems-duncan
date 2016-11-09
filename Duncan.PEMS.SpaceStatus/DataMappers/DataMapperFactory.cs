using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Duncan.PEMS.SpaceStatus.Models;
using Duncan.PEMS.SpaceStatus.DataShapes;

namespace Duncan.PEMS.SpaceStatus.DataMappers
{
    class DataMapperFactory
    {
        public IDataMapper GetMapper(Type dtoType)
        {
            // I don't like the style of the commented-out section below because it will break if a class is renamed
            /*
            switch (dtoType.Name)
            {
                case "RegulatedHours_SchemaV1DTO":
                    return new RegulatedHours_ExperimentalDBSchemaV1_DTOMapper();
                default:
                    return new GenericMapper(dtoType);
            }
            */

            if (string.Compare(typeof(RegulatedHours_ExperimentalDBSchemaV1DTO).Name, dtoType.Name, false) == 0)
                return new RegulatedHours_ExperimentalDBSchemaV1_DTOMapper();
            else if (string.Compare(typeof(OverstayVioActionsDTO).Name, dtoType.Name, false) == 0)
                return new OverstayVioActionsMapper();
            else if (string.Compare(typeof(AreaAsset).Name, dtoType.Name, false) == 0)
                return new AreaAssetDataMapper();
            else if (string.Compare(typeof(MeterAsset).Name, dtoType.Name, false) == 0)
                return new MeterAssetDataMapper();
            else if (string.Compare(typeof(SpaceAsset).Name, dtoType.Name, false) == 0)
                return new SpaceAssetDataMapper();
            else if (string.Compare(typeof(PAMClusterAsset).Name, dtoType.Name, false) == 0)
                return new PAMClusterAssetDataMapper();
            else if (string.Compare(typeof(PaymentRecord).Name, dtoType.Name, false) == 0)
                return new PaymentRecordDataMapper();
            else if (string.Compare(typeof(HistoricalSensingRecord).Name, dtoType.Name, false) == 0)
                return new HistoricalSensingRecordDataMapper();
            else if (string.Compare(typeof(CurrentSpaceOccupancyInformation).Name, dtoType.Name, false) == 0)
                return new CurrentSpaceOccupancyInformationDataMapper();
            else if (string.Compare(typeof(SensorEventAndCommsRecord).Name, dtoType.Name, false) == 0)
                return new SensorEventAndCommsRecordDataMapper();
            else if (string.Compare(typeof(SensorHeartbeatRecord).Name, dtoType.Name, false) == 0)
                return new SensorHeartbeatRecordDataMapper();
            else if (string.Compare(typeof(SensorBatteryDiagnostics).Name, dtoType.Name, false) == 0)
                return new SensorBatteryDiagnosticsDataMapper();
            else if (string.Compare(typeof(CustomerNameAndId).Name, dtoType.Name, false) == 0)
                return new CustomerNameAndIdDataMapper();
            else if (string.Compare(typeof(CustomGroup1Asset).Name, dtoType.Name, false) == 0)
                return new CustomGroup1AssetDataMapper();
            else
                return new GenericMapper(dtoType);  // Last resort is generic mapper based entirely on Reflection -- so it is slow and not customizable
        }
    }
}