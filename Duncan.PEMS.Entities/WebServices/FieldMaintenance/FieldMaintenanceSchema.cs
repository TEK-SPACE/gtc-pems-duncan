﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=4.0.30319.17929.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.duncansolutions.com/alarmworkorderv1", IsNullable=false)]
public partial class Data {
    
    private DataRequest requestField;
    
    private DataResponse responseField;
    
    private DataError errorField;
    
    /// <remarks/>
    public DataRequest Request {
        get {
            return this.requestField;
        }
        set {
            this.requestField = value;
        }
    }
    
    /// <remarks/>
    public DataResponse Response {
        get {
            return this.responseField;
        }
        set {
            this.responseField = value;
        }
    }
    
    /// <remarks/>
    public DataError Error {
        get {
            return this.errorField;
        }
        set {
            this.errorField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequest {
    
    private DataRequestWorkOrder[] workOrderField;
    
    private DataRequestActiveAlarms[] activeAlarmsField;
    
    private DataRequestHistoricalAlarm[] historicalAlarmsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("WorkOrder")]
    public DataRequestWorkOrder[] WorkOrder {
        get {
            return this.workOrderField;
        }
        set {
            this.workOrderField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarms")]
    public DataRequestActiveAlarms[] ActiveAlarms {
        get {
            return this.activeAlarmsField;
        }
        set {
            this.activeAlarmsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("HistoricalAlarm", IsNullable=false)]
    public DataRequestHistoricalAlarm[] HistoricalAlarms {
        get {
            return this.historicalAlarmsField;
        }
        set {
            this.historicalAlarmsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequestWorkOrder {
    
    private DataRequestWorkOrderActiveAlarm[] activeAlarmField;
    
    private string cidField;
    
    private string aidField;
    
    private string midField;
    
    private string workOrderIdField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarm")]
    public DataRequestWorkOrderActiveAlarm[] ActiveAlarm {
        get {
            return this.activeAlarmField;
        }
        set {
            this.activeAlarmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string cid {
        get {
            return this.cidField;
        }
        set {
            this.cidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string aid {
        get {
            return this.aidField;
        }
        set {
            this.aidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string mid {
        get {
            return this.midField;
        }
        set {
            this.midField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string WorkOrderId {
        get {
            return this.workOrderIdField;
        }
        set {
            this.workOrderIdField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequestWorkOrderActiveAlarm {
    
    private string eventCodeField;
    
    private string eventSourceField;
    
    private System.DateTime timeOfOccurranceField;
    
    private System.DateTime timeOfNotificationField;
    
    private string notesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventCode {
        get {
            return this.eventCodeField;
        }
        set {
            this.eventCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventSource {
        get {
            return this.eventSourceField;
        }
        set {
            this.eventSourceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfOccurrance {
        get {
            return this.timeOfOccurranceField;
        }
        set {
            this.timeOfOccurranceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Notes {
        get {
            return this.notesField;
        }
        set {
            this.notesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequestActiveAlarms {
    
    private DataRequestActiveAlarmsActiveAlarm[] activeAlarmField;
    
    private System.DateTime timeOfNotificationField;
    
    private bool timeOfNotificationFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarm")]
    public DataRequestActiveAlarmsActiveAlarm[] ActiveAlarm {
        get {
            return this.activeAlarmField;
        }
        set {
            this.activeAlarmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfNotificationSpecified {
        get {
            return this.timeOfNotificationFieldSpecified;
        }
        set {
            this.timeOfNotificationFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequestActiveAlarmsActiveAlarm {
    
    private string cidField;
    
    private string aidField;
    
    private string midField;
    
    private string eventUIDField;
    
    private string eventCodeField;
    
    private string eventSourceField;
    
    private System.DateTime timeOfOccurranceField;
    
    private bool timeOfOccurranceFieldSpecified;
    
    private System.DateTime timeOfNotificationField;
    
    private bool timeOfNotificationFieldSpecified;
    
    private System.DateTime sLADueField;
    
    private bool sLADueFieldSpecified;
    
    private string notesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string cid {
        get {
            return this.cidField;
        }
        set {
            this.cidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string aid {
        get {
            return this.aidField;
        }
        set {
            this.aidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string mid {
        get {
            return this.midField;
        }
        set {
            this.midField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventUID {
        get {
            return this.eventUIDField;
        }
        set {
            this.eventUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventCode {
        get {
            return this.eventCodeField;
        }
        set {
            this.eventCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventSource {
        get {
            return this.eventSourceField;
        }
        set {
            this.eventSourceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfOccurrance {
        get {
            return this.timeOfOccurranceField;
        }
        set {
            this.timeOfOccurranceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfOccurranceSpecified {
        get {
            return this.timeOfOccurranceFieldSpecified;
        }
        set {
            this.timeOfOccurranceFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfNotificationSpecified {
        get {
            return this.timeOfNotificationFieldSpecified;
        }
        set {
            this.timeOfNotificationFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime SLADue {
        get {
            return this.sLADueField;
        }
        set {
            this.sLADueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool SLADueSpecified {
        get {
            return this.sLADueFieldSpecified;
        }
        set {
            this.sLADueFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Notes {
        get {
            return this.notesField;
        }
        set {
            this.notesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataRequestHistoricalAlarm {
    
    private string cidField;
    
    private string aidField;
    
    private string midField;
    
    private string eventCodeField;
    
    private string eventSourceField;
    
    private string clearingEventUIDField;
    
    private string eventUIDField;
    
    private string eventStateField;
    
    private System.DateTime timeOfOccurranceField;
    
    private System.DateTime timeOfNotificationField;
    
    private System.DateTime timeOfClearanceField;
    
    private System.DateTime sLADueField;
    
    private bool sLADueFieldSpecified;
    
    private string notesField;
    
    private string workOrderIdField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string cid {
        get {
            return this.cidField;
        }
        set {
            this.cidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string aid {
        get {
            return this.aidField;
        }
        set {
            this.aidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string mid {
        get {
            return this.midField;
        }
        set {
            this.midField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventCode {
        get {
            return this.eventCodeField;
        }
        set {
            this.eventCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventSource {
        get {
            return this.eventSourceField;
        }
        set {
            this.eventSourceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string ClearingEventUID {
        get {
            return this.clearingEventUIDField;
        }
        set {
            this.clearingEventUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventUID {
        get {
            return this.eventUIDField;
        }
        set {
            this.eventUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventState {
        get {
            return this.eventStateField;
        }
        set {
            this.eventStateField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfOccurrance {
        get {
            return this.timeOfOccurranceField;
        }
        set {
            this.timeOfOccurranceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfClearance {
        get {
            return this.timeOfClearanceField;
        }
        set {
            this.timeOfClearanceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime SLADue {
        get {
            return this.sLADueField;
        }
        set {
            this.sLADueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool SLADueSpecified {
        get {
            return this.sLADueFieldSpecified;
        }
        set {
            this.sLADueFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string notes {
        get {
            return this.notesField;
        }
        set {
            this.notesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string WorkOrderId {
        get {
            return this.workOrderIdField;
        }
        set {
            this.workOrderIdField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponse {
    
    private DataResponseWorkOrder[] workOrderField;
    
    private DataResponseActiveAlarms[] activeAlarmsField;
    
    private DataResponseHistoricalAlarm[] historicalAlarmsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("WorkOrder")]
    public DataResponseWorkOrder[] WorkOrder {
        get {
            return this.workOrderField;
        }
        set {
            this.workOrderField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarms")]
    public DataResponseActiveAlarms[] ActiveAlarms {
        get {
            return this.activeAlarmsField;
        }
        set {
            this.activeAlarmsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("HistoricalAlarm", IsNullable=false)]
    public DataResponseHistoricalAlarm[] HistoricalAlarms {
        get {
            return this.historicalAlarmsField;
        }
        set {
            this.historicalAlarmsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponseWorkOrder {
    
    private DataResponseWorkOrderActiveAlarm[] activeAlarmField;
    
    private string cidField;
    
    private string aidField;
    
    private string midField;
    
    private string workOrderIdField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarm")]
    public DataResponseWorkOrderActiveAlarm[] ActiveAlarm {
        get {
            return this.activeAlarmField;
        }
        set {
            this.activeAlarmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string cid {
        get {
            return this.cidField;
        }
        set {
            this.cidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string aid {
        get {
            return this.aidField;
        }
        set {
            this.aidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string mid {
        get {
            return this.midField;
        }
        set {
            this.midField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string WorkOrderId {
        get {
            return this.workOrderIdField;
        }
        set {
            this.workOrderIdField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponseWorkOrderActiveAlarm {
    
    private string eventCodeField;
    
    private string eventSourceField;
    
    private System.DateTime timeOfOccurranceField;
    
    private System.DateTime timeOfNotificationField;
    
    private string eventUIDField;
    
    private System.DateTime sLADueField;
    
    private bool sLADueFieldSpecified;
    
    private string notesField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventCode {
        get {
            return this.eventCodeField;
        }
        set {
            this.eventCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventSource {
        get {
            return this.eventSourceField;
        }
        set {
            this.eventSourceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfOccurrance {
        get {
            return this.timeOfOccurranceField;
        }
        set {
            this.timeOfOccurranceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventUID {
        get {
            return this.eventUIDField;
        }
        set {
            this.eventUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime SLADue {
        get {
            return this.sLADueField;
        }
        set {
            this.sLADueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool SLADueSpecified {
        get {
            return this.sLADueFieldSpecified;
        }
        set {
            this.sLADueFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Notes {
        get {
            return this.notesField;
        }
        set {
            this.notesField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponseActiveAlarms {
    
    private DataResponseActiveAlarmsActiveAlarm[] activeAlarmField;
    
    private System.DateTime timeOfNotificationField;
    
    private bool timeOfNotificationFieldSpecified;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ActiveAlarm")]
    public DataResponseActiveAlarmsActiveAlarm[] ActiveAlarm {
        get {
            return this.activeAlarmField;
        }
        set {
            this.activeAlarmField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfNotificationSpecified {
        get {
            return this.timeOfNotificationFieldSpecified;
        }
        set {
            this.timeOfNotificationFieldSpecified = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponseActiveAlarmsActiveAlarm {
    
    private string cidField;
    
    private string aidField;
    
    private string midField;
    
    private string eventUIDField;
    
    private string eventCodeField;
    
    private string eventSourceField;
    
    private System.DateTime timeOfOccurranceField;
    
    private bool timeOfOccurranceFieldSpecified;
    
    private System.DateTime timeOfNotificationField;
    
    private bool timeOfNotificationFieldSpecified;
    
    private System.DateTime sLADueField;
    
    private bool sLADueFieldSpecified;
    
    private string notesField;
    
    private string workOrderIdField;
    
    private string closedField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string cid {
        get {
            return this.cidField;
        }
        set {
            this.cidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string aid {
        get {
            return this.aidField;
        }
        set {
            this.aidField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string mid {
        get {
            return this.midField;
        }
        set {
            this.midField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventUID {
        get {
            return this.eventUIDField;
        }
        set {
            this.eventUIDField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventCode {
        get {
            return this.eventCodeField;
        }
        set {
            this.eventCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventSource {
        get {
            return this.eventSourceField;
        }
        set {
            this.eventSourceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfOccurrance {
        get {
            return this.timeOfOccurranceField;
        }
        set {
            this.timeOfOccurranceField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfOccurranceSpecified {
        get {
            return this.timeOfOccurranceFieldSpecified;
        }
        set {
            this.timeOfOccurranceFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime TimeOfNotification {
        get {
            return this.timeOfNotificationField;
        }
        set {
            this.timeOfNotificationField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool TimeOfNotificationSpecified {
        get {
            return this.timeOfNotificationFieldSpecified;
        }
        set {
            this.timeOfNotificationFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public System.DateTime SLADue {
        get {
            return this.sLADueField;
        }
        set {
            this.sLADueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool SLADueSpecified {
        get {
            return this.sLADueFieldSpecified;
        }
        set {
            this.sLADueFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string Notes {
        get {
            return this.notesField;
        }
        set {
            this.notesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string WorkOrderId {
        get {
            return this.workOrderIdField;
        }
        set {
            this.workOrderIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string Closed {
        get {
            return this.closedField;
        }
        set {
            this.closedField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataResponseHistoricalAlarm {
    
    private string statusCodeField;
    
    private string eventUIDField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string statusCode {
        get {
            return this.statusCodeField;
        }
        set {
            this.statusCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="integer")]
    public string EventUID {
        get {
            return this.eventUIDField;
        }
        set {
            this.eventUIDField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.duncansolutions.com/alarmworkorderv1")]
public partial class DataError {
    
    private string[] itemsField;
    
    private ItemsChoiceType[] itemsElementNameField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("Code", typeof(string))]
    [System.Xml.Serialization.XmlElementAttribute("Message", typeof(string))]
    [System.Xml.Serialization.XmlChoiceIdentifierAttribute("ItemsElementName")]
    public string[] Items {
        get {
            return this.itemsField;
        }
        set {
            this.itemsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ItemsElementName")]
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public ItemsChoiceType[] ItemsElementName {
        get {
            return this.itemsElementNameField;
        }
        set {
            this.itemsElementNameField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.17929")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.duncansolutions.com/alarmworkorderv1", IncludeInSchema=false)]
public enum ItemsChoiceType {
    
    /// <remarks/>
    Code,
    
    /// <remarks/>
    Message,
}
