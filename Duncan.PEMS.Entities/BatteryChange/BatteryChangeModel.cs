using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.BatteryChange
{
    public class BatteryChangeModel
    {

        public List<BatteryChangeModel> chartData = new List<BatteryChangeModel>();

        public long value { get; set; }
        public string Text { get; set; }

        [OriginalGridPosition(Position = 0)]
        public string TimeOfLastStatus { get; set; } //** changes done on may 5th 2016 for battery change

        [OriginalGridPosition(Position = 1)]
        public string AssetType { get; set; }

        [OriginalGridPosition(Position = 2)]
        public long? MID { get; set; } //** changes done on may 5th 2016 for battery change


        [OriginalGridPosition(Position = 3)]
        public string MeterName { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string Area { get; set; }

        [OriginalGridPosition(Position = 5)]
        public string Zone { get; set; }


        [OriginalGridPosition(Position = 6)]
        public string Street { get; set; }

        [OriginalGridPosition(Position = 7)]
        public string Suburb { get; set; }

        [OriginalGridPosition(Position = 8)]
        public double? avgVoltage { get; set; }

        public int? AssetID { get; set; } //** changes done on may 5th 2016 for battery change

        public string color { get; set; }
        public string category { get; set; }
        public DateTime? TimeOfLastStatus_Date { get; set; } //** changes done on may 5th 2016 for battery change



    }

    public class sensorProfileModel
    {
        public long Value { get; set; }
        public string Text { get; set; }
        public int? CustomerID { get; set; }

        [OriginalGridPosition(Position = 0)]
        public string LastUpdatedTS { get; set; }

        [OriginalGridPosition(Position = 1)]
        public long? MID { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string Area { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string Zone { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string Street { get; set; }

        [OriginalGridPosition(Position = 5)]
        public string OccupancyStatus { get; set; }

        [OriginalGridPosition(Position = 6)]
        public string ProfileStatus { get; set; }


        [OriginalGridPosition(Position = 7)]
        public string TMPDesc { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string RUDesc { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string ATDesc { get; set; }

        [OriginalGridPosition(Position = 10)]
        public string SWDesc { get; set; }

        [OriginalGridPosition(Position = 11)]
        public string TRPKDesc { get; set; }

        public string FSW { get; set; }
        public string SW { get; set; }
        public string BL { get; set; }

        public int? LastStatus { get; set; }


        public int?[] Diag { get; set; }
        public int?[] Threshold { get; set; }


    }

    public class OccupancyRateModel
    {
        public List<OccupancyRateModel> overAllData = new List<OccupancyRateModel>();

        public int totalPercentOccupied { get; set; }
        public int totalAreasCnt { get; set; }
        public int totalSpacesCnt { get; set; }

        public long value { get; set; }
        public string Text { get; set; }

        public string SensorStatus { get; set; }

        [OriginalGridPosition(Position = 0)]
        public string LastUpdatedDateTime { get; set; }

        [OriginalGridPosition(Position = 1)]
        public int? MetersCnt { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string Area { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string Zone { get; set; }

        [OriginalGridPosition(Position = 4)]
        public string Street { get; set; }

        [OriginalGridPosition(Position = 5)]
        public int? OccupiedCnt { get; set; }

        [OriginalGridPosition(Position = 6)]
        public string OccupiedPercent { get; set; }

        [OriginalGridPosition(Position = 7)]
        public int? VacantCnt { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string VacantPercent { get; set; }

        [OriginalGridPosition(Position = 9)]
        public int? ViolatedCnt { get; set; }

        [OriginalGridPosition(Position = 10)]
        public string VioaltedPercent { get; set; }

        public string OccupiedPercentNoSymbol { get; set; }

        public string VacantPercentNoSymbol { get; set; }


        public string VioaltedPercentNoSymbol { get; set; }


        public long? MID { get; set; } //** changes done on may 5th 2016 for battery change



        public string MeterName { get; set; }



        public string OccStatus { get; set; }
        public string SpaceStatus { get; set; }

    }

    public class BatteryAnalysisModel
    {

        public List<BatteryAnalysisModel> chartData = new List<BatteryAnalysisModel>();
        //  public List<BatteryChangeModel> commsData = new List<BatteryChangeModel>();

        public long value { get; set; }
        public string Text { get; set; }

        [OriginalGridPosition(Position = 5)]
        public string SensorStatus { get; set; }

        [OriginalGridPosition(Position = 6)]
        public int? BatChangesCnt { get; set; }

        public int? commsForFirstChange { get; set; }
        public int? commsForSecondChange { get; set; }
        public int? commsForThirdChange { get; set; }

        public int? commsBtw_Start_FirstChange { get; set; }

        [OriginalGridPosition(Position = 10)]
        public int? commsBtw_FirstChange_SecondChange { get; set; }

        [OriginalGridPosition(Position = 11)]
        public int? commsBtw_SecondChange_ThirdChange { get; set; }

        public DateTime[] commsForFirstChangeDT = new DateTime[] { };

        [OriginalGridPosition(Position = 7)]
        public string firstChange { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string secondChange { get; set; }

        [OriginalGridPosition(Position = 9)]
        public string thirdChange { get; set; }


        public string TimeOfLastStatus { get; set; } //** changes done on may 5th 2016 for battery change


        public string AssetType { get; set; }

        [OriginalGridPosition(Position = 0)]
        public long? MID { get; set; } //** changes done on may 5th 2016 for battery change


        [OriginalGridPosition(Position = 1)]
        public string MeterName { get; set; }

        [OriginalGridPosition(Position = 2)]
        public string Area { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string Zone { get; set; }



        [OriginalGridPosition(Position = 4)]
        public string Street { get; set; }



        public decimal? avgVoltage { get; set; }

        public int? AssetID { get; set; } //** changes done on may 5th 2016 for battery change

        public string color { get; set; }
        public string category { get; set; }
        public DateTime? TimeOfLastStatus_Date { get; set; } //** changes done on may 5th 2016 for battery change



    }

    public class BatteryChartModel
    {

        public long value { get; set; }
        public string Text { get; set; }
        public int? MID { get; set; } //** changes done on may 5th 2016 for battery change
        public string MeterName { get; set; }
        public string VoltageDate { get; set; } //** changes done on may 5th 2016 for battery change
        public DateTime? VoltageDate_Dt { get; set; } //** changes done on may 5th 2016 for battery change
        public double? minVoltage { get; set; }
        public double? maxVoltage { get; set; }
        public int? CommsCnt { get; set; } //** changes done on may 5th 2016 for battery change
    }

}