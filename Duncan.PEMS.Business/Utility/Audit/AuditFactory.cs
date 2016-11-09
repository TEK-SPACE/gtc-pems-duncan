using System;
using System.Linq;
using Duncan.PEMS.DataAccess;
using Duncan.PEMS.DataAccess.RBAC;
using Duncan.PEMS.Entities.Audit;

namespace Duncan.PEMS.Business.Utility.Audit
{
    public class AuditFactory : RbacBaseFactory
    {
        /// <summary>
        /// Gets an audit record
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        public  AuditRecord Get(string tableName, int primaryKey)
        {
            // Need to switch on tableName.  Table may be located elsewhere
            // besides RbacEntities.SystemInformations

            if ( tableName.Equals( "CustomerProfile" ) )
            {
                return GetCustomerProfile( primaryKey );
            }


            // Default to use SystemInformation table.
            AuditRecord auditRecord;

            var rbacAuditRecord = RbacEntities.SystemInformations.FirstOrDefault( ar => ar.TableName == tableName && ar.PrimaryKey == primaryKey );

            if (rbacAuditRecord != null)
            {
                auditRecord = new AuditRecord(rbacAuditRecord.SystemInformationId)
                    {
                        CreatedBy = rbacAuditRecord.CreatedBy,
                        CreatedOn = rbacAuditRecord.CreatedOn,
                        ModifiedBy = rbacAuditRecord.LastModifiedBy ?? -1,
                        ModifiedOn = rbacAuditRecord.LastModifiedOn ?? DateTime.MinValue
                    };
            }
            else
            {
                auditRecord = new AuditRecord(-1)
                {
                    CreatedBy = -1,
                    CreatedOn = DateTime.MinValue,
                    ModifiedBy = -1,
                    ModifiedOn = DateTime.MinValue
                };
            }

            return auditRecord;
        }

        /// <summary>
        /// Saves a new audit record
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="auditRecord"></param>
        public  void Set(string tableName, int primaryKey, AuditRecord auditRecord)
        {
            // Handle table CustomerProfile
            if (tableName.Equals("CustomerProfile"))
            {
                SetCustomerProfile(primaryKey, auditRecord);
                return;
            }

            // Default handling.
            SystemInformation si = new SystemInformation()
                {
                    TableName = tableName,
                    PrimaryKey = primaryKey,
                    CreatedBy = auditRecord.CreatedBy,
                    CreatedOn = auditRecord.CreatedOn
                };

            if ( auditRecord.ModifiedBy > 0 )
            {
                si.LastModifiedBy = auditRecord.ModifiedBy;
                si.LastModifiedOn = auditRecord.ModifiedOn;
            }

            RbacEntities.SystemInformations.Add(si);
            RbacEntities.SaveChanges();
        }

        /// <summary>
        /// Sets / Updates the creation information for an item
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="userId"></param>
        public  void CreatedBy(string tableName, int primaryKey, int userId)
        {
            // Handle table CustomerProfile
            if (tableName.Equals("CustomerProfile"))
            {
                CreatedByCustomerProfile(primaryKey, userId);
                return;
            }

            // Default handling.
            var rbacAuditRecord = RbacEntities.SystemInformations.FirstOrDefault(ar => ar.TableName == tableName && ar.PrimaryKey == primaryKey);
            if ( rbacAuditRecord == null )
            {
                AuditRecord auditRecord = new AuditRecord()
                    {
                        CreatedBy = userId,
                        CreatedOn = DateTime.Now
                    };

                Set( tableName, primaryKey, auditRecord );
            }
        }

        /// <summary>
        /// Sets / Updates the modification information for an item
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKey"></param>
        /// <param name="userId"></param>
        public  void ModifiedBy(string tableName, int primaryKey, int userId)
        {
            // Handle table CustomerProfile
            if (tableName.Equals("CustomerProfile"))
            {
                ModifiedByCustomerProfile(primaryKey, userId);
                return;
            }

            // Default handling.
            var rbacAuditRecord = RbacEntities.SystemInformations.FirstOrDefault(ar => ar.TableName == tableName && ar.PrimaryKey == primaryKey);

            if (rbacAuditRecord != null)
            {
                rbacAuditRecord.LastModifiedBy = userId;
                rbacAuditRecord.LastModifiedOn = DateTime.Now;
                RbacEntities.SaveChanges();
            }
            else
            {
                //if it doesnt exist, create and update
                AuditRecord auditRecord = new AuditRecord()
                {
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = userId
                };
                Set(tableName, primaryKey, auditRecord);
            }
        }


        #region External/Custom table support

        #region CustomerProfile
        /// <summary>
        /// Gets a customer profile audit record
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <returns></returns>
        private  AuditRecord GetCustomerProfile(int primaryKey)
        {
            AuditRecord auditRecord;

            var rbacAuditRecord = RbacEntities.CustomerProfiles.FirstOrDefault( cp => cp.CustomerId == primaryKey );

            if (rbacAuditRecord != null)
            {
                auditRecord = new AuditRecord(rbacAuditRecord.CustomerId)
                    {
                        CreatedBy = rbacAuditRecord.CreatedBy ?? -1,
                        CreatedOn = rbacAuditRecord.CreatedOn ?? DateTime.MinValue,
                        ModifiedBy = rbacAuditRecord.ModifiedBy ?? -1,
                        ModifiedOn = rbacAuditRecord.ModifiedOn ?? DateTime.MinValue
                    };
            }
            else
            {
                auditRecord = new AuditRecord(-1)
                {
                    CreatedBy = -1,
                    CreatedOn = DateTime.MinValue,
                    ModifiedBy = -1,
                    ModifiedOn = DateTime.MinValue
                };
            }

            return auditRecord;
        }

        /// <summary>
        /// udpates a customer profile creation and modification time
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="auditRecord"></param>
        private  void SetCustomerProfile(int primaryKey, AuditRecord auditRecord)
        {
            var table = RbacEntities.CustomerProfiles.FirstOrDefault(cp => cp.CustomerId == primaryKey);

            if ( table == null )
                return;

            table.CreatedBy = auditRecord.CreatedBy;
            table.CreatedOn = auditRecord.CreatedOn;
            table.ModifiedBy = auditRecord.ModifiedBy;
            table.ModifiedOn = auditRecord.ModifiedOn;

            RbacEntities.SaveChanges();
        }

        /// <summary>
        /// Updates the creation inforamtion for a customer profile
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="userId"></param>
        private  void CreatedByCustomerProfile(int primaryKey, int userId)
        {
            var table = RbacEntities.CustomerProfiles.FirstOrDefault(cp => cp.CustomerId == primaryKey);

            if (table == null)
                return;

            table.CreatedBy = userId;
            table.CreatedOn = DateTime.Now;

            RbacEntities.SaveChanges();
        }

        /// <summary>
        /// Updates the modified information for a customer profile
        /// </summary>
        /// <param name="primaryKey"></param>
        /// <param name="userId"></param>
        private  void ModifiedByCustomerProfile(int primaryKey, int userId)
        {
            var table = RbacEntities.CustomerProfiles.FirstOrDefault(cp => cp.CustomerId == primaryKey);

            if (table == null)
                return;

            if ( table.CreatedBy == null )
            {
                table.CreatedBy = userId;
                table.CreatedOn = DateTime.Now;
            }
            table.ModifiedBy = userId;
            table.ModifiedOn = DateTime.Now;

            RbacEntities.SaveChanges();
        }


        #endregion


        #endregion





    }
}
