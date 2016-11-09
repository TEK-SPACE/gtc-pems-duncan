namespace Duncan.PEMS.Entities.General
{
    /// <summary>
    ///     Base Event Model
    /// </summary>
    public class TSD
    {
        public int Id { get; set; }
        public int MasterId { get; set; }
        public string Description { get; set; }
    }
    public class TimeTypeDDL
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class  StringIdTextDDLModel
    {
        public string Id { get; set; }
        public string Text { get; set; }
    }

    public class MenuModel
    {
        public string Target { get; set; }
        public string BackgroundStyle { get; set; }
        public string ActiveClass { get; set; }
        public string OpenClass { get; set; }
        public string MenuUrl { get; set; }
        public string ID { get; set; }
        public string Url { get; set; }
        public string Label { get; set; }
    }
}