using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Duncan.PEMS.SpaceStatus.DataShapes
{
    // For historical payment record from DB
    public class PaymentRecord
    {
        // We will use [XmlElement] attributes to trigger our DataMapper source code generator
        // to know when the DB column name differs from the property name.

        // Also note that we can use the [XmlIgnore] attribute if we don't want to attempt
        // populating a property from a database field

        public int CustomerId { get; set; }
        public int MeterId { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDateTime { get; set; }

        [XmlElement("AmountInCents")]
        public USD Amount { get; set; }
        
        public TimeSpan TimePaid { get; set; }
        public PaymentMethod Method { get; set; }
        public int BayNumber { get; set; }

        [XmlElement("TransactionType")]
        public string PaymentType { get; set; }
    }

    public enum PaymentMethod
    {
        // This is the convention used in the DB
        Cash = 0,
        CreditCard = 1,
        MPark = 2,
        SmartCard = 3,
        CashKey = 4,
        FreeParking = 5,
        ZeroOut = 6
    }

    /// <summary>
    /// Summary description for Currency
    /// </summary>
    public class USD
    {
        int _AmountInCents;

        public USD(int AmountInCents)
        {
            _AmountInCents = AmountInCents;
        }

        public int Cents
        {
            get
            {
                return _AmountInCents;
            }
            set
            {
                _AmountInCents = value;
            }
        }

        override public string ToString()
        {
            if (_AmountInCents > 100)
            {
                return "$" + (_AmountInCents / 100).ToString() + " " + (_AmountInCents % 100) + "C";

            }
            else
                return _AmountInCents + "C";
        }
    }
}