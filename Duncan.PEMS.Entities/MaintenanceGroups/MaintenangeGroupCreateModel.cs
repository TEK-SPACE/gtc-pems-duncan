using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Duncan.PEMS.Entities.MaintenanceGroups
{
    public class MaintenangeGroupCreateModel
    {
        [Required]
        public string InternalName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        [Range(1, 65535)]
        public int Id { get; set; }

        [Required]
        public string ConnectionStringName { get; set; }
        public List<SelectListItem> ConnectionStrings { get; set; }
    }

}
