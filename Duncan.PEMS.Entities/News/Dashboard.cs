using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.News
{
    public class Dashboard
    {
        public int PaymentAccepted { get; set; }
        public int PaymentPending { get; set; }
        public int PaymentRefunded { get; set; }
        public int PaymentAcceptedPending { get; set; }
        public int PaymentDeclained { get; set; }
        public int PaymentNoTransaction { get; set; }
        public int PaymentDeclainedPending { get; set; }
        public int PaymentAttempted { get; set; }
        public int TotelPayment
        {
            get
            {
                //return this.PaymentAccepted + this.PaymentPending + this.PaymentRefunded + this.PaymentAcceptedPending + this.PaymentDeclained + this.PaymentNoTransaction + this.PaymentDeclainedPending + this.PaymentAttempted;
                return this.PaymentAccepted + this.PaymentRefunded + this.PaymentAcceptedPending + this.PaymentDeclained + this.PaymentNoTransaction;
            }
        }
        public string Month { get; set; }
        public string Month2 { get; set; }
        public string Month3 { get; set; }
        public string Month4 { get; set; }
        public string Month5 { get; set; }
        public int CashTransaction { get; set; }
        public int CreditTransaction { get; set; }
        public int CellTransaction { get; set; }
        public int SmartTransaction { get; set; }
        public int D5CashTransaction { get; set; }
        public int D5CreditTransaction { get; set; }
        public int D5CellTransaction { get; set; }
        public int D5SmartTransaction { get; set; }
        public int D4CashTransaction { get; set; }
        public int D4CreditTransaction { get; set; }
        public int D4CellTransaction { get; set; }
        public int D4SmartTransaction { get; set; }
        public int D3CashTransaction { get; set; }
        public int D3CreditTransaction { get; set; }
        public int D3CellTransaction { get; set; }
        public int D3SmartTransaction { get; set; }
        public int D2CashTransaction { get; set; }
        public int D2CreditTransaction { get; set; }
        public int D2CellTransaction { get; set; }
        public int D2SmartTransaction { get; set; }
        public int ActSensor { get; set; }
        public int CommSensor { get; set; }
        public int MinPurchased { get; set; }
        public int TotalMeterCount { get; set; }
        public int ActMeterCount { get; set; }
        public int SevereAlarmCount { get; set; }
        public int MajAlarmCount { get; set; }
        public int ActMeterCommCount { get; set; }
        public int ActiveMeterAlarmCount { get; set; }
        //public int ActMeterNonCommCount { get; set; }
        public int ActMeterNonCommCount
        {
            get
            {
                if ((this.ActMeterCount - this.ActMeterCommCount) < 0)
                    return 0;
                else
                    return this.ActMeterCount - this.ActMeterCommCount;
            }

        }

        public float ActMeterPercentage
        {
            get
            {
                float retValue = 0;
                if (this.TotalMeterCount != 0)
                {

                    float res = (float)(100 / (float)this.TotalMeterCount) * (float)this.ActMeterCount;
                    retValue = (float)Math.Round(res, 1);
                }
                return retValue;
            }
        }

        public float AlaramPercentage
        {
            get
            {
                float retValue = 0;
                if (this.ActMeterCount != 0)
                {

                    float res = (float)((float)100 / (float)(this.ActMeterCount)) * (float)this.SevereAlarmCount;
                    retValue = (float)Math.Round(res, 1);
                }
                return retValue;
            }
        }

        public float ActNonCommMeterPercentage
        {
            get
            {
                float retValue = 0;
                if (this.ActMeterCount != 0)
                {
                    float res = (float)((float)100 / (float)this.ActMeterCount) * (float)this.ActMeterNonCommCount;
                    retValue = (float)Math.Round(res, 1);
                }
                return retValue;
            }
        }

        public int ChangeBatteryCount { get; set; }
        public int PlanChangeBatteryCount { get; set; }
        public float BatteryPercentage { get; set; }
        //public float BatteryPercentage
        //{
        //    get
        //    {
        //        float retValue = 0;
        //        if (this.ActMeterCount != 0)
        //        {
        //            float res = (float)((float)100 / (float)this.ActMeterCount) * (float)this.ChangeBatteryCount;
        //            retValue = (float)Math.Round(res, 1);
        //        }
        //        return retValue;
        //    }
        //}
        public int ActWithSensor { get; set; }
        public int SensorComm { get; set; }
        public int SensorNoComm
        {
            get
            {
                int retValue = 0;
                if (this.ActWithSensor != 0)
                {
                    retValue = (int)(ActWithSensor) - (int)this.SensorComm;
                }
                return retValue;
            }

        }
        public int TotalMinPurchased { get; set; }
        public int MinZeroedOut { get; set; }
        public int MinResold { get; set; }
        public int SpaceStatusSensor { get; set; }
        public int SpacesWithPayment { get; set; }
        public int EnforceableSpaces { get; set; }
        public List<ChartData> Data { get; set; }
        public double[] Key { get; set; }
        public int myCustomerID { get; set; }
        public float ActSensorPercentage
        {
            get
            {
                float retValue = 0;
                if (this.ActMeterCount != 0)
                {

                    float res = (float)(100 / (float)this.ActMeterCount) * (float)this.ActWithSensor;
                    retValue = (float)Math.Round(res, 1);
                }
                return retValue;
            }
        }
        public float ActCommSensorPercentage
        {
            get
            {
                float retValue = 0;
                if (this.ActWithSensor != 0)
                {

                    float res = (float)(100 / (float)this.ActWithSensor) * (float)this.ActCommSensor;
                    retValue = (float)Math.Round(res, 1);
                }
                return retValue;
            }
        }
        public int HasSensor { get; set; }

        public int SingleSpaceMeter { get; set; }
        public int MultiSpaceMeter { get; set; }
        public int Sensor { get; set; }
        public int Cashbox { get; set; }
        public int Smartcard { get; set; }
        public int Gateway { get; set; }
        public int CSPark { get; set; }
        public int ParkingSpaces { get; set; }
        public int Mechanism { get; set; }
        public int DataKey { get; set; }
        public int LibertyComm { get; set; }
        public int TotalLibertyComm { get; set; }
        public int GatewayComm { get; set; }
        public int TotalGatewayComm { get; set; }
        public int CSPComm { get; set; }
        public int TotalCSPComm { get; set; }
        public int ActCommSensor { get; set; }
        public int TotalActCommSensor { get; set; }
        public int LibertyNoComm
        {
            get
            {
                int retValue = 0;
                if (this.TotalLibertyComm != 0)
                {
                    retValue = (int)(TotalLibertyComm) - (int)this.LibertyComm;
                }
                return retValue;
            }
        }
        public int CSPNoComm
        {
            get
            {
                int retValue = 0;
                if (this.TotalCSPComm != 0)
                {
                    retValue = (int)(TotalCSPComm) - (int)this.CSPComm;
                }
                return retValue;
            }
        }
        public int GatewayNoComm
        {
            get
            {
                int retValue = 0;
                if (this.TotalGatewayComm != 0)
                {
                    retValue = (int)(TotalGatewayComm) - (int)this.GatewayComm;
                }
                return retValue;
            }
        }
        public int ActNoCommSensor
        {
            get
            {
                int retValue = 0;
                if (this.TotalActCommSensor != 0)
                {
                    retValue = (int)(TotalActCommSensor) - (int)this.ActCommSensor;
                }
                return retValue;
            }
        }
    }

    public class ChartData
    {
        public int Key { get; set; }
        //public int Value { get; set; }
    }

}
