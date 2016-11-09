using System;

namespace Duncan.PEMS.Entities.Audit
{
    public class AuditRecord
    {
        public AuditRecord()
        {
            Id = 0;
            CreatedBy = 0;
            ModifiedBy = 0;
        }

        public AuditRecord(int id)
        {
            Id = id;
            CreatedBy = -1;
            CreatedOn = DateTime.MinValue;
            ModifiedBy = -1;
            ModifiedOn = DateTime.MinValue;
        }

        public bool IsValid
        {
            get { return Id > 0; }
        }

        public int Id { get; private set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}