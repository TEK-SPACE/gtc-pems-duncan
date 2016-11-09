/******************* CHANGE LOG *************************************************************************************************************************************
 * DATE                 NAME                        DESCRIPTION
 * ___________      ___________________             _________________________________________________________________________________________________________
 * 
 * 12/18/2013       Sergey Ostrerov                 Issue: DPTXPEMS-143 and 196. PDF/CSV/XL shows more records compared to Grid.
 *                                                         PDF/CSV/XL show date-time under 'Last Modified On' column where time is in HH:MM:SS format 
 *                                                         while grid shows in HH:MM format. Shouldn't grid also show in HH:MM:SS format?
 *                                                         Updated: public string LastModifiedOnDisplay {}
 *                                                         
 * 03/20/2014       Sergey Ostrerov                 Issue: DPTXPEMS-261 'Manage Roles' grid page shows date in US format (MM/DD/YYYY)
 * 
 * ******************************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Duncan.PEMS.Entities.Roles
{
    public class ListRoleModel
    {
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Internal Name")]
        public string CustomerInternalName { get; set; }

        [Display(Name = "Role Id")]
        public int RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Last Modified On")]
        public DateTime LastModifiedOn { get; set; }
        public string LastModifiedOnDisplay { get { return LastModifiedOn == DateTime.MinValue ? string.Empty : LastModifiedOn.ToString("dd/MM/yyyy HH:mm:ss"); } }
        
        [Display(Name = "Last Modified By")]
        public string LastModifiedBy { get; set; }
    }

    public class EditRoleModel
    {
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Internal Name")]
        public string CustomerInternalName { get; set; }

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Role Id")]
        public int RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Items")]
        public List<AuthorizationItem> Items { get; set; }
    }

    public class CreateRoleModel
    {
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Internal Name")]
        public string CustomerInternalName { get; set; }

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Role Id")]
        public int RoleId { get; set; }

        [Display(Name = "Role Name")]
        public string RoleName { get; set; }

        [Display(Name = "Items")]
        public List<AuthorizationItem> Items { get; set; }
    }

    public class CopyRoleModel
    {
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Customer Id")]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Internal Name")]
        public string CustomerInternalName { get; set; }

        [Display(Name = "TO Role Name")]
        public string ToRoleName { get; set; }
    }

    /// <summary>
    ///     This class represents a displayable authorization item that
    ///     encapsulates the NetSqlAzMan Operations into a human-readable hiearchy
    ///     An <see cref="AuthorizationItem" /> is a list of <see cref="AuthorizationItem" />
    ///     to allow for a parent-child relationship
    /// </summary>
    public class AuthorizationItem : List<AuthorizationItem>
    {
        public string Name { get; set; }
        public bool Authorized { get; set; }
        public bool Required { get; set; }
        public int Id { get; set; }

        public List<AuthorizationItem> Items
        {
            get { return this; }
        }
    }
}