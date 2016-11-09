using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Duncan.PEMS.Entities.WebServices.FieldMaintenance
{
   public abstract class BaseAlarmResponse
    {
       public bool IsValid { get; set; }
       public string ErrorMessage { get; set; }
    }
}
