using System;
namespace Duncan.PEMS.Entities.General
{
    [AttributeUsage(AttributeTargets.All)]
    public class OriginalGridPositionAttribute : Attribute
    {
        public int Position { get; set; }
    }
}
