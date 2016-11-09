using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.FileUpload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using Kendo.Mvc.UI;
using Duncan.PEMS.Utilities;
using Duncan.PEMS.Business.Users;
using NLog;
using System.Data;
using System.Web.Mvc;
using WebMatrix.WebData;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Enumerations;


namespace Duncan.PEMS.Business.FileUpload
{
    public class FileUploadFactory : BaseFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        #endregion

        public FileUploadFactory(string connectionStringName)
        {
            ConnectionStringName = connectionStringName;
        }

        /// <summary>
        ///  Description: This Method will addition of files to the system. 
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)    
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filedata"></param>
        /// <param name="filehash"></param>
        /// <param name="filetype"></param>
        /// <param name="UserId"></param>
        /// <param name="FileComment"></param>
        /// <returns></returns>
        public long UploadFile(string filename, byte[] filedata, string filehash, int filetype, int UserId, string FileComment)
        {
            // If the File is listed as No Longer Active, it may be uploaded again and the NEW entry will be set to Active, the old entry will stay at No Longer Active.
            var fdsetNoLonger = (from s in PemsEntities.FDFilesAudits
                                 join FD_File in PemsEntities.FDFiles on s.FileId equals FD_File.FileID
                                 where FD_File.FileHash == filehash && s.FileStatus == 1
                                 select s).ToList();
            fdsetNoLonger.ForEach(a => a.FileStatus = 0); // set no longer active

            int num = PemsEntities.SaveChanges();


            foreach (FDFilesAudit fdaud in fdsetNoLonger)
            {
                fdaud.FileStatus = 0; //Inactive
                fdaud.UploadedBy = UserId;
                fdaud.DateCanceled = DateTime.Now;
                PemsEntities.FDFilesAudits.Add(fdaud);
                PemsEntities.SaveChanges();

            }

            //fdjob
            var fdJobsetstatus = (from s in PemsEntities.FDJobs
                                  join FD_File in PemsEntities.FDFiles on s.FileID equals FD_File.FileID
                                  where FD_File.FileHash == filehash && s.ActiveJob == 1
                                  select s).ToList();
            fdJobsetstatus.ForEach(a => a.ActiveJob = 0); // set no longer active                
            int num2 = PemsEntities.SaveChanges();

            //addition of file to the system
            FDFile FDFileUpload = new FDFile();

            DateTime todaydate = DateTime.Now;

            FDFileUpload.FileType = filetype;
            FDFileUpload.FileHash = filehash;
            FDFileUpload.FileComments = FileComment;
            FDFileUpload.FileSizeBytes = filedata.Length;
            FDFileUpload.FileAdditionDate = todaydate;
            FDFileUpload.FileRawData = filedata;
            FDFileUpload.FileName = filename;

            PemsEntities.FDFiles.Add(FDFileUpload);
            PemsEntities.SaveChanges();



            var items = from sub in PemsEntities.FDFiles
                        where sub.FileHash == filehash
                        select new FDFilesModel
                        {
                            FileID = sub.FileID,


                        };


            FDFilesModel fd = new FDFilesModel();
            var listitems = items.ToList();
            if (listitems.Any())
                fd = listitems.Last();

            if (fd.FileID > 0)
            {
                //FDFilesAudit : stores username and active state
                FDFilesAudit FDFilesAuditobj1 = new FDFilesAudit();

                FDFilesAuditobj1.FileId = fd.FileID;
                FDFilesAuditobj1.FileStatus = 1; //active
                FDFilesAuditobj1.UploadedBy = UserId;
                PemsEntities.FDFilesAudits.Add(FDFilesAuditobj1);
                PemsEntities.SaveChanges();
            }
            return fd.FileID;
        }


        public ulong HexLiteral2Unsigned(string hex)
        {
            if (string.IsNullOrEmpty(hex)) throw new ArgumentException("hex");

            int i = hex.Length > 1 && hex[0] == '0' && (hex[1] == 'x' || hex[1] == 'X') ? 2 : 0;
            ulong value = 0;

            while (i < hex.Length)
            {
                uint x = hex[i++];

                if (x >= '0' && x <= '9') x = x - '0';
                else if (x >= 'A' && x <= 'F') x = (x - 'A') + 10;
                else if (x >= 'a' && x <= 'f') x = (x - 'a') + 10;
                else throw new ArgumentOutOfRangeException("hex");

                value = 16 * value + x;

            }

            return value;
        }

        /// <summary>
        /// Description:This method is used to get the FileHash for the selected File in the Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FDFilesModel GetFileHash(long fileId)
        {
            var ddlItems = new FDFilesModel();
            try
            {
                var fileHashIs = (from fh in PemsEntities.FDFiles
                                  where fh.FileID == fileId
                                  select new FDFilesModel
                                  {
                                      FileHash = fh.FileHash
                                  }
                                  );

                var listitems = fileHashIs.ToList();
                if (listitems.Any())
                    ddlItems = listitems.Last();
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetFileHash factory method (SChedule FD)", ex);
            }

            return ddlItems;
        }

        /// <summary>
        ///  Description: This Method will update file state with comments for particular file    
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)   
        /// </summary>
        /// <param name="Griddata"></param>
        /// <param name="FileStatus"></param>
        /// <param name="UserId"></param>
        public void UpdateFile(FDFilesModel Griddata, string FileStatus, int UserId)
        {

            int myfilestatus;
            if (FileStatus.Trim().Length != 0)
            {
                myfilestatus = Convert.ToInt32(FileStatus);
            }
            else
            {
                myfilestatus = -1;
            }
            if (myfilestatus != -1)
            {
                //fdfiles
                var fd = (from s in PemsEntities.FDFiles
                          where s.FileID == Griddata.FileID
                          select s).FirstOrDefault();

                fd.FileComments = Griddata.FileComments;
                int num = PemsEntities.SaveChanges();


                if (myfilestatus == 1)
                {

                    //fdfiles 
                    var fdfileauditsetstatus = (from s in PemsEntities.FDFilesAudits
                                                where s.FileId == fd.FileID
                                                select s).ToList();
                    fdfileauditsetstatus.ForEach(a => a.FileStatus = myfilestatus); // set active 
                    fdfileauditsetstatus.ForEach(a => a.UploadedBy = UserId);
                    fdfileauditsetstatus.ForEach(a => a.DateCanceled = null);
                    int num2 = PemsEntities.SaveChanges();


                    //fdjob

                    var fdJobsetstatus = (from s in PemsEntities.FDJobs
                                          join FD_File in PemsEntities.FDFiles on s.FileID equals FD_File.FileID
                                          where FD_File.FileID == fd.FileID
                                          select s).ToList();
                    fdJobsetstatus.ForEach(a => a.ActiveJob = myfilestatus); // set active              
                    int num3 = PemsEntities.SaveChanges();
                }
                if (myfilestatus == 0)
                {

                    //fdfiles
                    var fdfileauditsetstatus = (from s in PemsEntities.FDFilesAudits
                                                where s.FileId == fd.FileID
                                                select s).ToList();
                    fdfileauditsetstatus.ForEach(a => a.FileStatus = myfilestatus); // set Inactive    
                    fdfileauditsetstatus.ForEach(a => a.UploadedBy = UserId);
                    fdfileauditsetstatus.ForEach(a => a.DateCanceled = DateTime.Now);
                    int num2 = PemsEntities.SaveChanges();


                    //fdjob

                    var fdJobsetstatus = (from s in PemsEntities.FDJobs
                                          join FD_File in PemsEntities.FDFiles on s.FileID equals FD_File.FileID
                                          where FD_File.FileID == fd.FileID
                                          select s).ToList();
                    fdJobsetstatus.ForEach(a => a.ActiveJob = myfilestatus); // set Inactive              
                    int num3 = PemsEntities.SaveChanges();

                }

            }

        }

        public void UpdateFile_inactive(FDFilesModel Griddata)
        {

            var fd = (from s in PemsEntities.FDFiles
                      where s.FileID == Griddata.FileID
                      select s).FirstOrDefault();

            fd.FileComments = Griddata.FileComments;
            UpdateFileStatus(Griddata);
            int num = PemsEntities.SaveChanges();


        }

        public void UpdateFileStatus(FDFilesModel Griddata)
        {

            var stud = (from s in PemsEntities.FDJobs
                        where s.FileID == Griddata.FileID
                        && s.JobID == Griddata.JobID
                        select s).FirstOrDefault();

            stud.ActiveJob = 0;
            int num = PemsEntities.SaveChanges();
        }

        //public FDFilesModel FileExists(string filehash)
        //{
        //    var ddlItems = new FDFilesModel();
        //    try
        //    {


        //        var items = from sub in PemsEntities.FDFiles
        //                    join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
        //                    where sub.FileHash == filehash && aud.ScheduledBy == null
        //                    select new FDFilesModel
        //                    {
        //                        FileID = sub.FileID,
        //                        FileName = sub.FileName,
        //                        FileType = sub.FileType,
        //                       Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")
        //                    };

        //        var listitems = items.ToList();
        //        if (listitems.Any())
        //        ddlItems = listitems.Last();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return ddlItems;

        //}

        /// <summary>
        /// Description:This method is used to get the list of file names for the Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="Roletype"></param>
        /// <param name="fileTypeVal"></param>
        /// <returns></returns>
        public IQueryable<FDFilesModel> GetFileNames(int fileTypeVal)
        {
            var Files = (from FDFilesType in PemsEntities.FDFiles
                         join aud in PemsEntities.FDFilesAudits on FDFilesType.FileID equals aud.FileId
                         where FDFilesType.FileType == fileTypeVal && aud.JobId == 0 && aud.FileStatus == 1
                         select new FDFilesModel
                         {
                             FileName = FDFilesType.FileName + ", " + FDFilesType.FileComments,
                             FileID = FDFilesType.FileID
                         });
            return Files;
        }

        /// <summary>
        /// Description: This Method will check file already exists with state
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="filehash"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public FDFilesModel FileExists(string filehash, int customerID)
        {
            var ddlItems = new FDFilesModel();
            try
            {


                var items = from sub in PemsEntities.FDFiles
                            join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                            where sub.FileHash == filehash
                            select new FDFilesModel
                            {
                                FileID = sub.FileID,
                                FileName = sub.FileName,
                                FileType = sub.FileType,
                                CustmerID = customerID,
                                FileHash = sub.FileHash,
                                FileSizeBytes = sub.FileSizeBytes,
                                FileAdditionDate = sub.FileAdditionDate,
                                Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")

                            };

                var listitems = items.ToList();
                if (listitems.Any())
                    ddlItems = listitems.Last();



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN FileExists factory method (Schedule FD)", ex);
            }
            return ddlItems;

        }

        public FDFilesModel FileExists1(long fileid, int customerID)
        {
            var ddlItems = new FDFilesModel();
            try
            {


                var items = from sub in PemsEntities.FDFiles
                            join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                            where sub.FileID == fileid && aud.JobId == 0
                            select new FDFilesModel
                            {
                                FileID = sub.FileID,
                                FileName = sub.FileName,
                                FileType = sub.FileType,
                                CustmerID = customerID,
                                FileHash = sub.FileHash,
                                FileSizeBytes = sub.FileSizeBytes,
                                FileAdditionDate = sub.FileAdditionDate,
                                Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")

                            };

                var listitems = items.ToList();
                if (listitems.Any())
                    ddlItems = listitems.Last();



            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN FileExists factory method (Schedule FD)", ex);
            }
            return ddlItems;

        }

        /// <summary>
        /// Description:This method is used to schedule the job for the selected assets and files in the Schedule FD screen;
        /// Modified By: Sairam on July 23rd 2014
        /// </summary>
        /// <param name="locTypeVal"></param>
        /// <param name="assetSelected"></param>
        /// <param name="fileSelected"></param>
        /// <param name="dateScheduled"></param>
        /// <param name="dateActivated"></param>
        /// <param name="filename"></param>
        /// <param name="fileSize"></param>
        /// <param name="filehash"></param>
        /// <param name="FileType"></param>
        /// <param name="ActiveStatus"></param>
        /// <param name="FileAdditionDate"></param>
        /// <param name="UserID"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<string> ScheduleJob(string chosenAsset, string locTypeVal, string[] assetSelected, long fileSelected, DateTime dateScheduled, DateTime dateActivated, string filename, long fileSize, string filehash, int FileType, string ActiveStatus, DateTime FileAdditionDate, int UserID, int customerID)
        {

            DateTime submittedDateIs = DateTime.Now;
            int assetId = 0;
            string assetNameIs = "";
            List<string> assetsDownloaded = new List<string>();


            //** Loop thru the no. of assets selected
            for (var i = 0; i < assetSelected.Length; i++)
            {

                try
                {

                    //** Find out the asset ID if the asset name is given

                    if (locTypeVal == "AssetName")
                    {
                        //** Asset Name chosen, then 

                        //** find the coresponding Meter ID
                        assetNameIs = assetSelected[i];
                        var item = (from assetname in PemsEntities.Meters
                                    where assetname.MeterName == assetNameIs && assetname.MeterName != null
                                    select new FDFilesModel
                                    {
                                        MeterID = assetname.MeterId
                                    }
                                          );

                        var meterIDIs = item.ToList();
                        if (meterIDIs.Any())
                            assetId = meterIDIs.Last().MeterID;

                    }
                    else
                    {
                        //** asset ID is chosen
                        assetId = Convert.ToInt32(assetSelected[i]);
                    }



                    //** Conversions of data types for some fields are done before saving it to database

                    string fileTypeIs = Convert.ToString(FileType);

                    byte ActiveIs = new byte();
                    var activeJob = 0;
                    if (ActiveStatus == "Active")
                    {
                        activeJob = 1;
                    }
                    else
                    {
                        activeJob = 0;
                    }
                    ActiveIs = (byte)activeJob;


                    //***************************************************************************************************************************

                    //** First Update the 'FDJobs' table to indicate that the job has been scheduled.

                    FDJob FDJobInst = new FDJob();

                    FDJobInst.FileID = fileSelected;
                    FDJobInst.CustomerID = customerID;

                    //** Check the chosen asset and based on it, give the Areaid
                    var myChosenAsset = Convert.ToInt32(chosenAsset);

                    if (myChosenAsset == 0)
                    {
                        //Liberty OR Eagle
                        FDJobInst.AreaID = (int)AssetAreaId.Meter;
                    }
                    else if (myChosenAsset == 13)
                    {
                        //Gateway
                        FDJobInst.AreaID = (int)AssetAreaId.Gateway;
                    }
                    else if (myChosenAsset == 31)
                    {
                        //Mechanism
                        FDJobInst.AreaID = (int)AssetAreaId.Mechanism;
                    }
                    else if (myChosenAsset == 10)
                    {
                        //Sensor
                        FDJobInst.AreaID = (int)AssetAreaId.Sensor;
                    }
                    else if (myChosenAsset == 16)
                    {
                        //CSPark
                        FDJobInst.AreaID = (int)AssetAreaId.CSPark;
                    }

                    FDJobInst.MeterId = assetId;
                    FDJobInst.SubmittedDate = submittedDateIs;// submittedDateTime;// DateTime.Now;
                    FDJobInst.AvailableDate = dateScheduled;
                    FDJobInst.ActiveJob = ActiveIs;
                    FDJobInst.ActivationDate = dateActivated;
                    FDJobInst.JobStatus = 1; //** 1 indicates that the job has been sumitted.

                    //** Open the FDJobs table and save it
                    PemsEntities.FDJobs.Add(FDJobInst);
                    PemsEntities.SaveChanges();


                    //***************************************************************************************************************************

                    //** In order to save the Scheduled job in FDSummary table, first create a FDSummary model.

                    FDSummary fds = new FDSummary();

                    //** Get the Job ID from FDJobs table
                    var jobs = (from job in PemsEntities.FDJobs
                                where job.MeterId == assetId && job.CustomerID == customerID && job.FileID == fileSelected// && job.SubmittedDate <= submittedDateIs
                                select job).ToList().Last();

                    //** Check if the asset name is passed
                    if (locTypeVal == "AssetName")
                    {
                        //** Asset Name chosen, then 

                        //** find the coresponding Meter ID
                        var item = (from assetname in PemsEntities.Meters
                                    where assetname.MeterName == assetNameIs && assetname.MeterName != null
                                    select new FDFilesModel
                                    {
                                        MeterID = assetname.MeterId
                                    }
                                          );

                        var meterIDIs = item.ToList();
                        if (meterIDIs.Any())
                            fds.meterid = meterIDIs.Last().MeterID;

                    }
                    else
                    {
                        //** Only Asset IDs chosen

                        fds.meterid = Convert.ToInt32(assetSelected[i]);
                    }

                    //****************************************

                    if (myChosenAsset == 0)
                    {
                        //Liberty OR Eagle
                        fds.areaid = (int)AssetAreaId.Meter;
                    }
                    else if (myChosenAsset == 13)
                    {
                        //Gateway
                        fds.areaid = (int)AssetAreaId.Gateway;
                    }
                    else if (myChosenAsset == 31)
                    {
                        //Mechanism
                        fds.areaid = (int)AssetAreaId.Mechanism;
                    }
                    else if (myChosenAsset == 10)
                    {
                        //Sensor
                        fds.areaid = (int)AssetAreaId.Sensor;
                    }
                    else if (myChosenAsset == 16)
                    {
                        //CSPark
                        fds.areaid = (int)AssetAreaId.CSPark;
                    }

                    //****************************

                    // fds.areaid = (int)AssetAreaId.Meter;
                    fds.fileid = fileSelected;
                    fds.fillename = filename;
                    fds.customerid = (short)customerID;
                    fds.filesizebytes = fileSize;
                    fds.fileadditiondate = FileAdditionDate;
                    fds.filetype = fileTypeIs;
                    fds.filehash = filehash;
                    fds.submitteddate = DateTime.Now;
                    fds.availabledate = dateScheduled;
                    fds.activejob = ActiveIs;
                    fds.jobid = jobs.JobID;
                    fds.activationdate = dateActivated;

                    long jobIDForAudit = jobs.JobID;  //** Sairam added this on 25th sep 2014 for storing in fdfilesaudit

                    //** Open the FDSummary table and add the scheduled job details and save it
                    PemsEntities.FDSummaries.Add(fds);
                    PemsEntities.SaveChanges();

                    //***************************************************************************************************************************

                    //** Update the FDFilesAudit table too to record the user name who has scheduled the job

                    var fd = (from s in PemsEntities.FDSummaries
                              where s.filehash == filehash && s.jobid == jobIDForAudit
                              select s).ToList().Last();
                    if (fd.fileid > 0)
                    {
                        FDFilesAudit FDFilesAuditobj = new FDFilesAudit();

                        FDFilesAuditobj.FileId = fd.fileid;
                        FDFilesAuditobj.FileStatus = 1; //active
                        FDFilesAuditobj.JobId = fd.jobid;
                        FDFilesAuditobj.ScheduledBy = UserID;
                        PemsEntities.FDFilesAudits.Add(FDFilesAuditobj);
                        PemsEntities.SaveChanges();
                    }


                    assetsDownloaded.Add(assetSelected[i]); //** Store Asset Names which worked well only

                }
                catch (Exception ex)
                {
                    _logger.ErrorException("ERROR IN ScheduleJob Factory Method (Schedule FD)", ex);
                }

            } //** End of for loop

            return assetsDownloaded;

        }

        public FDFilesModel FileActiveExists(FDFilesModel Griddata)
        {
            var ddlItems = new FDFilesModel();
            try
            {
                var filehash = PemsEntities.FDFiles.FirstOrDefault(m => m.FileID == Griddata.FileID);

                var items = from sub in PemsEntities.FDFiles
                            join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                            where sub.FileHash == filehash.FileHash && aud.FileStatus == 1
                            select new FDFilesModel
                            {
                                FileID = sub.FileID,
                                FileName = sub.FileName,
                                FileType = sub.FileType,
                                Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")
                            };

                var listitems = items.ToList();
                if (listitems.Any())
                    ddlItems = listitems.Last();
            }
            catch (Exception ex)
            {

            }
            return ddlItems;

        }

        public FDFilesModel FileIDActiveExists(string FileID)
        {
            long Fid = long.Parse(FileID);
            var ddlItems = new FDFilesModel();
            try
            {


                var items = from sub in PemsEntities.FDFiles
                            join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                            where sub.FileID == Fid && aud.FileStatus == 1
                            select new FDFilesModel
                            {
                                FileID = sub.FileID,
                                FileName = sub.FileName,
                                FileType = sub.FileType,
                                Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")
                            };

                var listitems = items.ToList();
                if (listitems.Any())
                    ddlItems = listitems.Last();
            }
            catch (Exception ex)
            {

            }
            return ddlItems;

        }

        /// <summary>
        ///  Description: This Method will return list of files uploaded to system with details
        ///  When in Duncan Admin these files will be binary files for assets which are system wide. 
        ///  When in a customer the PGM, RPG, CCF, and MPBConfig files 
        ///   ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Roletype"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FDFilesModel> GetFDFileDetails(DataSourceRequest request, string Roletype, out int total)
        {
            total = 0;
            string paramValues = string.Empty;
            var spParams = GetSpParams(request, "TimeOfOccurrance desc", out paramValues);
            var UserId = WebSecurity.CurrentUserId;
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();
            string FileType = spParams[5].Value.ToString();
            string FileStatus = spParams[6].Value.ToString();
            int myfiletype;
            int myfilestatus;
            if (FileType.Trim().Length != 0)
            {
                myfiletype = Convert.ToInt32(FileType);
            }
            else
            {
                myfiletype = -1;
            }
            //file status not being used as of now
            if (FileStatus.Trim().Length != 0)
            {
                myfilestatus = Convert.ToInt32(FileStatus);
            }
            else
            {
                myfilestatus = -1;
            }

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                // ** Valid start date 
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date 
            }

            List<string> FileExtns;
            if (Roletype.ToLower() == "admin")
            {
                //FileExtns = new List<string> { "BIN", "SNSR" };
                FileExtns = new List<string> { "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM" };
            }
            else
            {
                //  FileExtns = new List<string> { "CCF", "RPG", "PGM", "BIN", "SNSR" };Liberty_SSM_MPB_Config", "Liberty_SSM_CCF"
                FileExtns = new List<string> { "Liberty_SSM_MPB_Config", "Liberty_SSM_CCF", "Liberty_SSM_RPG", "Liberty_SSM_MB_Pgm", "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM", "ST Sensor Config", "CS-Park Config", "ST Sensor Binary", "CS-Park Bin", "Gateway Config", "Gateway Bin", "Gateway GPRS" };

            };
            var ddlItems = new List<FDFilesModel>();
            List<FDFilesModel> FileUploadInfo = new List<FDFilesModel>();
            try
            {
                if (myfilestatus == 1)
                {
                    // ACTIVE State
                    var items = (from sub in PemsEntities.FDFiles
                                 join Ftype in PemsEntities.FDFileTypes on sub.FileType equals Ftype.FileType

                                 join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                                 where (aud.FileStatus == myfilestatus)
                      && (sub.FileAdditionDate >= startTimeOutput && sub.FileAdditionDate <= endTimeOutput)
                      && (sub.FileType == myfiletype || myfiletype == -1)
                       && (aud.DateCanceled == null) && (aud.ScheduledBy == null)
                                     //  && FileExtns.Contains(Ftype.FileExtension)
                      && FileExtns.Contains(Ftype.FileDesc)
                       && (aud.UploadedBy == UserId)


                                 select new FDFilesModel
                                 {
                                     FileAdditionDate = sub.FileAdditionDate,
                                     FileID = (Roletype.ToLower() != "admin" ? (Ftype.FileDesc == "Liberty_SSM_MPB_Pgm" || Ftype.FileDesc == "LIBERTY_SSM_SENSOR_PGM" ? 0 : sub.FileID) : sub.FileID),
                                     FileName = sub.FileName,
                                     FileTypeText = Ftype.FileDesc,
                                     FileComments = sub.FileComments,
                                     Userid = (int)aud.UploadedBy,
                                     Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")
                                 }).Distinct();
                    //  items = items.ApplyFiltering(request.Filters);
                    total = items.Count();
                    items = items.ApplySorting(request.Groups, request.Sorts);
                    items = items.ApplyPaging(request.Page, request.PageSize);
                    FileUploadInfo = items.ToList();

                    if (FileUploadInfo.Any())
                        ddlItems = FileUploadInfo;
                }

                else if (myfilestatus == 0)
                {
                    // InACTIVE State
                    var items = (from sub in PemsEntities.FDFiles
                                 join Ftype in PemsEntities.FDFileTypes on sub.FileType equals Ftype.FileType

                                 join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                                 where (aud.FileStatus == myfilestatus)
                      && (sub.FileAdditionDate >= startTimeOutput && sub.FileAdditionDate <= endTimeOutput)
                      && (sub.FileType == myfiletype || myfiletype == -1)
                       && (aud.ScheduledBy == null)
                                     //&& FileExtns.Contains(Ftype.FileExtension)
                         && FileExtns.Contains(Ftype.FileDesc)
                        && (aud.UploadedBy == UserId)


                                 select new FDFilesModel
                                 {
                                     FileAdditionDate = sub.FileAdditionDate,
                                     FileID = (Roletype.ToLower() != "admin" ? (Ftype.FileDesc == "Liberty_SSM_MPB_Pgm" || Ftype.FileDesc == "LIBERTY_SSM_SENSOR_PGM" ? 0 : sub.FileID) : sub.FileID),
                                     FileName = sub.FileName,
                                     FileTypeText = Ftype.FileDesc,
                                     FileComments = sub.FileComments,
                                     Userid = (int)aud.UploadedBy,
                                     Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")
                                 }).Distinct();
                    //  items = items.ApplyFiltering(request.Filters);
                    total = items.Count();
                    items = items.ApplySorting(request.Groups, request.Sorts);
                    items = items.ApplyPaging(request.Page, request.PageSize);
                    FileUploadInfo = items.ToList();

                    if (FileUploadInfo.Any())
                        ddlItems = FileUploadInfo;
                }


                else
                {
                    // ALL
                    var items = (from sub in PemsEntities.FDFiles
                                 join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                                 join Ftype in PemsEntities.FDFileTypes on sub.FileType equals Ftype.FileType
                                 where
                       (sub.FileAdditionDate >= startTimeOutput && sub.FileAdditionDate <= endTimeOutput)
                      && (sub.FileType == myfiletype || myfiletype == -1)
                       && (aud.ScheduledBy == null)
                                     // && FileExtns.Contains(Ftype.FileExtension)
                        && FileExtns.Contains(Ftype.FileDesc)
                        && (aud.UploadedBy == UserId)

                                 // (from sub in PemsEntities.FDFiles
                                 //                    where sub.FileType == filetype
                                 //                    select sub.FileID).Max()
                                 select new FDFilesModel
                                 {
                                     FileAdditionDate = sub.FileAdditionDate,
                                     FileID = (Roletype.ToLower() != "admin" ? (Ftype.FileDesc == "Liberty_SSM_MPB_Pgm" || Ftype.FileDesc == "LIBERTY_SSM_SENSOR_PGM" ? 0 : sub.FileID) : sub.FileID),
                                     FileName = sub.FileName,
                                     FileTypeText = Ftype.FileDesc,
                                     FileComments = sub.FileComments,
                                     Userid = (int)aud.UploadedBy,
                                     Activestaus = (aud.FileStatus == 1 ? "Active" : "InActive")

                                 }).Distinct(); ;



                    // items = items.ApplyFiltering(request.Filters);
                    total = items.Count();
                    items = items.ApplySorting(request.Groups, request.Sorts);
                    items = items.ApplyPaging(request.Page, request.PageSize);


                    FileUploadInfo = items.ToList();



                    if (FileUploadInfo.Any())
                        ddlItems = FileUploadInfo;
                }
            }
            catch (Exception ex)
            {

            }

            return ddlItems;
        }

        /// <summary>
        /// Description: This Method will return any jobs active/inactive for file        
        /// ModifiedBy: Santhosh  (04/Apr/2014 - 07/Apr/2014)  
        /// </summary>
        /// <param name="FileId"></param>
        /// <returns></returns>
        public List<FDFilesModel> GetFileInfo(long FileId)
        {
            var ddlItems = new List<FDFilesModel>();
            try
            {
                var items = from at in PemsEntities.FDJobs
                            join jobstat in PemsEntities.FDJobStatus on at.JobStatus equals jobstat.JobStatus
                            where at.FileID == FileId
                            select new FDFilesModel
                            {
                                JobStatus = jobstat.JobStatus,
                                StatusDesc = jobstat.StatusDesc,
                                Active = at.ActiveJob,
                                CustmerID = at.CustomerID,
                                AreaID = at.AreaID,
                                MeterID = at.MeterId

                            };

                List<FDFilesModel> EnforcVehVIN = items.ToList();

                if (EnforcVehVIN.Any())
                    ddlItems = EnforcVehVIN;
            }
            catch (Exception ex)
            {

            }

            return ddlItems;
        }


        public FDFilesModel FileIsActive(long FileId)
        {
            var filehash = PemsEntities.FDFiles.FirstOrDefault(x => x.FileID == FileId);
            var items = (from sub in PemsEntities.FDFiles
                         join aud in PemsEntities.FDFilesAudits on sub.FileID equals aud.FileId
                         where sub.FileHash == filehash.FileHash && aud.FileStatus == 1 && sub.FileID != FileId
                         select new FDFilesModel
                         {
                             FileID = sub.FileID
                         }).FirstOrDefault();
            return items;

        }

        public void UpdateIndexFile(FDFilesModel Griddata, string status)
        {
            int myfilestatus;
            if (status.Trim().Length != 0)
            {
                myfilestatus = Convert.ToInt32(status);
            }
            else
            {
                myfilestatus = -1;
            }
            if (myfilestatus != -1)
            {
                var stud = (from s in PemsEntities.FDFiles
                            where s.FileID == Griddata.FileID
                            select s).FirstOrDefault();

                stud.FileComments = Griddata.FileComments;
                UpdateIndexFileStatus(Griddata, myfilestatus);
                int num = PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Description:This method is used to get the locations for the 'Available Assets' list box in Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="meterGrpID"></param>
        /// <returns></returns>
        public IEnumerable<FDFilesModel> GetLocationTypeIdListBox(string locationType, int CurrentCity, int meterGrpID)
        {
            IEnumerable<FDFilesModel> details = Enumerable.Empty<FDFilesModel>();

            try
            {

                if (locationType == "Area")
                {
                    details = (from area in PemsEntities.Areas

                               where area.CustomerID == CurrentCity && area.AreaID != null
                               select new FDFilesModel
                               {
                                   Text = area.AreaName
                               });


                }
                else if (locationType == "AssetID")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == meterGrpID
                               select new FDFilesModel
                               {
                                   Value = meter.MeterId,
                               });


                }
                else if (locationType == "AssetName")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.MeterName != null && meter.MeterGroup == meterGrpID
                               select new FDFilesModel
                               {
                                   Text = meter.MeterName,
                               });


                }
                else if (locationType == "Zone")
                {

                    details = (from zone in PemsEntities.Zones

                               where zone.customerID == CurrentCity && zone.ZoneId != null
                               select new FDFilesModel
                               {
                                   Text = zone.ZoneName
                               });

                }
                else if (locationType == "Street")
                {
                    details = (from meter in PemsEntities.Meters
                               where meter.CustomerID == CurrentCity && meter.Location != null && meter.MeterGroup == meterGrpID
                               select new FDFilesModel
                               {
                                   Text = meter.Location,
                               });
                }
                else if (locationType == "Suburb")
                {
                    details = (from customGroup in PemsEntities.CustomGroup1

                               where customGroup.CustomerId == CurrentCity && customGroup.DisplayName != null
                               select new FDFilesModel
                               {
                                   Text = customGroup.DisplayName
                               });


                }
                else if (locationType == "Demand Area")
                {
                    details = (from demandZone in PemsEntities.DemandZones

                               join DZone in PemsEntities.Meters
                               on demandZone.DemandZoneId equals DZone.DemandZone

                               join metergrp in PemsEntities.MeterGroups
                               on DZone.MeterGroup equals metergrp.MeterGroupId

                               select new FDFilesModel
                               {
                                   Text = demandZone.DemandZoneDesc
                               });
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetLocationTypeIdListBox Method (Asset Configuration - Schedule FD Report)", ex);
            }


            return details;
        }

        /// <summary>
        /// Description:This method is used to get the Group locations for the 'Selected Assets' list box in Schedule FD screen;
        /// Modified By: Sairam on July 21st 2014
        /// </summary>
        /// <param name="locationType"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="groupItems"></param>
        /// <param name="assetTypeVal"></param>
        /// <returns></returns>
        //public IEnumerable<FDFilesModel> GetTargetGroupLocations(string locationType, int CurrentCity, string groupItems, int? assetTypeVal)
        //{
        //    IEnumerable<FDFilesModel> details = Enumerable.Empty<FDFilesModel>();

        //    try
        //    {

        //        if (locationType == "Area")
        //        {

        //            string[] str = groupItems.Split(',');
        //            List<string> ids = new List<string>();
        //            for (var i = 0; i < str.Length; i++)
        //            {
        //                ids.Add(str[i]);
        //            }

        //            details = (from x in PemsEntities.Meters
        //                       join y in PemsEntities.Areas on x.AreaID equals y.AreaID
        //                       where ids.Contains(y.AreaName) && x.CustomerID == CurrentCity && x.MeterId != null && x.MeterGroup == assetTypeVal
        //                       select new FDFilesModel
        //                       {
        //                           Value = x.MeterId,
        //                       });


        //        }
        //        else if (locationType == "AssetName")
        //        {

        //        }
        //        else if (locationType == "Zone")
        //        {


        //            string[] str = groupItems.Split(',');
        //            List<string> ids = new List<string>();
        //            for (var i = 0; i < str.Length; i++)
        //            {
        //                ids.Add(str[i]);
        //            }



        //            details = (from meter in PemsEntities.Meters

        //                       join metermap in PemsEntities.MeterMaps
        //                       on meter.MeterId equals metermap.MeterId

        //                       join zone in PemsEntities.Zones
        //                       on metermap.ZoneId equals zone.ZoneId

        //                       where ids.Contains(zone.ZoneName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
        //                       select new FDFilesModel
        //                       {
        //                           Value = meter.MeterId,
        //                       });


        //        }
        //        else if (locationType == "Street")
        //        {

        //            string[] street = groupItems.Split(',');
        //            List<string> groupIds = new List<string>();
        //            for (var i = 0; i < street.Length; i++)
        //            {
        //                groupIds.Add(street[i]);
        //            }


        //            details = (from meter in PemsEntities.Meters
        //                       where groupIds.Contains(meter.Location) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
        //                       select new FDFilesModel
        //                       {
        //                           Value = meter.MeterId,
        //                       });

        //        }
        //        else if (locationType == "Suburb")
        //        {
        //            string[] str = groupItems.Split(',');
        //            List<string> ids = new List<string>();
        //            for (var i = 0; i < str.Length; i++)
        //            {
        //                ids.Add(str[i]);
        //            }

        //            details = (from meter in PemsEntities.Meters

        //                       join metermap in PemsEntities.MeterMaps
        //                       on meter.MeterId equals metermap.MeterId

        //                       join customgroup in PemsEntities.CustomGroup1
        //                       on metermap.CustomGroup1 equals customgroup.CustomGroupId

        //                       where ids.Contains(customgroup.DisplayName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
        //                       select new FDFilesModel
        //                       {
        //                           Value = meter.MeterId,
        //                       });
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorException("ERROR IN GetTargetGroupLocations Method (Asset Configuration Menu - Schedule File Download Report)", ex);
        //    }

        //    return details;

        //}

        public IEnumerable<FDFilesModel> GetTargetGroupLocations(string locationType, int CurrentCity, string groupItems, int? assetTypeVal)
        {
            IEnumerable<FDFilesModel> details = Enumerable.Empty<FDFilesModel>();

            try
            {

                if (locationType == "Area")
                {

                    string[] str = groupItems.Split(',');
                    List<string> ids = new List<string>();
                    for (var i = 0; i < str.Length; i++)
                    {
                        ids.Add(str[i]);
                    }

                    //details = (from meter in PemsEntities.Meters
                    //           join y in PemsEntities.Areas on meter.AreaID equals y.AreaID
                    //           where ids.Contains(y.AreaName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
                    //           select new FDFilesModel
                    //           {
                    //               Value = meter.MeterId,
                    //           });

                    details = (from meter in PemsEntities.Meters

                               //**Sairam modified this code on Nov 18th 2014 to do correct mapping
                               join mm in PemsEntities.MeterMaps on
                                   new { MeterId = meter.MeterId, AreaId = meter.AreaID, CustomerId = meter.CustomerID } equals
                                   new { MeterId = mm.MeterId, AreaId = mm.Areaid, CustomerId = mm.Customerid } into metermap
                               from l1 in metermap.DefaultIfEmpty()

                               join area in PemsEntities.Areas
                               on new { AreaID = (int)l1.AreaId2, CustomerId = l1.Customerid } equals new { AreaID = area.AreaID, CustomerId = area.CustomerID } into mmArea
                               from A1 in mmArea.DefaultIfEmpty()

                               where ids.Contains(A1.AreaName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
                               select new FDFilesModel
                               {
                                   Value = meter.MeterId,
                               });



                }
                else if (locationType == "AssetName")
                {

                }
                else if (locationType == "Zone")
                {


                    string[] str = groupItems.Split(',');
                    List<string> ids = new List<string>();
                    for (var i = 0; i < str.Length; i++)
                    {
                        ids.Add(str[i]);
                    }



                    details = (from meter in PemsEntities.Meters

                               join metermap in PemsEntities.MeterMaps
                               on meter.MeterId equals metermap.MeterId

                               join zone in PemsEntities.Zones
                               on metermap.ZoneId equals zone.ZoneId

                               where ids.Contains(zone.ZoneName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
                               select new FDFilesModel
                               {
                                   Value = meter.MeterId,
                               });


                }
                else if (locationType == "Street")
                {

                    string[] street = groupItems.Split(',');
                    List<string> groupIds = new List<string>();
                    for (var i = 0; i < street.Length; i++)
                    {
                        groupIds.Add(street[i]);
                    }


                    details = (from meter in PemsEntities.Meters
                               where groupIds.Contains(meter.Location) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
                               select new FDFilesModel
                               {
                                   Value = meter.MeterId,
                               });

                }
                else if (locationType == "Suburb")
                {
                    string[] str = groupItems.Split(',');
                    List<string> ids = new List<string>();
                    for (var i = 0; i < str.Length; i++)
                    {
                        ids.Add(str[i]);
                    }

                    details = (from meter in PemsEntities.Meters

                               join metermap in PemsEntities.MeterMaps
                               on meter.MeterId equals metermap.MeterId

                               join customgroup in PemsEntities.CustomGroup1
                               on metermap.CustomGroup1 equals customgroup.CustomGroupId

                               where ids.Contains(customgroup.DisplayName) && meter.CustomerID == CurrentCity && meter.MeterId != null && meter.MeterGroup == assetTypeVal
                               select new FDFilesModel
                               {
                                   Value = meter.MeterId,
                               });
                }

            }
            catch (Exception ex)
            {
                _logger.ErrorException("ERROR IN GetTargetGroupLocations Method (Asset Configuration Menu - Schedule File Download Report)", ex);
            }

            return details;

        }

        public void UpdateIndexFileStatus(FDFilesModel Griddata, int FDStatus)
        {


            var stud = (from s in PemsEntities.FDJobs
                        where s.FileID == Griddata.FileID
                        && s.JobID == Griddata.JobID
                        select s).FirstOrDefault();
            //stud.ActiveJob =FDStatus== 0 ? 0 : 1,
            if (FDStatus == 0)
            { stud.ActiveJob = 0; }
            else
            { stud.ActiveJob = 1; }
            int num = PemsEntities.SaveChanges();
        }

        #region FDInquiry
        /// <summary>
        ///Description:GetFileTypes will get the types of file that can be selected to filter the grid data. This is to populate the 'File Type' drop down of 'FDInquiry' filter Panel.
        ///This is to be populated differently based on the logged in user.
        ///Modified:Prita()
        /// </summary>
        /// <param name="Roletype"></param>
        /// <returns></returns>
        public IQueryable<FDFilesModel> GetFileTypes(string Roletype)
        {

            {
                List<string> FileExtns;
                if (Roletype.ToLower() == "admin")
                {
                    //   FileExtns = new List<string> { "BIN", "SNSR" };
                    FileExtns = new List<string> { "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM" };
                    var Files = (from FDFiles in PemsEntities.FDFileTypes
                                 // where FileExtns.Contains(FDFiles.FileExtension) && !FDFiles.FileDesc.ToUpper().Contains("VX meter executable code") && !FDFiles.FileDesc.ToUpper().Contains("Liberty_SSM_MPB_Config")
                                 where FileExtns.Contains(FDFiles.FileDesc)
                                 select new FDFilesModel
                                 {
                                     FileName = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                     FileType = FDFiles.FileType,
                                     //selected = true
                                 });
                    return Files;
                }
                else
                {
                    //  FileExtns = new List<string> { "CCF", "RPG", "PGM", "BIN", "SNSR" };
                    FileExtns = new List<string> { "Liberty_SSM_MPB_Config", "Liberty_SSM_CCF", "Liberty_SSM_RPG", "Liberty_SSM_MB_Pgm", "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM", "ST Sensor Config", "CS-Park Config", "ST Sensor Binary", "CS-Park Bin", "Gateway Config", "Gateway Bin", "Gateway GPRS" };
                    var Files = (from FDFiles in PemsEntities.FDFileTypes
                                 // where FileExtns.Contains(FDFiles.FileExtension) && !FDFiles.FileDesc.ToUpper().Contains("VX meter executable code")
                                 where FileExtns.Contains(FDFiles.FileDesc)
                                 select new FDFilesModel
                                 {
                                     FileName = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                     FileType = FDFiles.FileType,
                                     //selected = true
                                 });
                    return Files;
                };


            }
        }



        public List<SelectListItemWrapper> GetFileTypesForUpload1(string Roletype)
        {
            List<string> FileExtns;
            if (Roletype.ToLower() == "admin")
            {
                FileExtns = new List<string> { "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM" };
                List<SelectListItemWrapper> list = (from FDFiles in PemsEntities.FDFileTypes
                                                    where FileExtns.Contains(FDFiles.FileDesc)

                                                    select new SelectListItemWrapper()
                                                    {

                                                        Text = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                                        ValueInt = FDFiles.FileType
                                                    }).ToList();

                list.Insert(0, new SelectListItemWrapper()
                {

                    Text = "",
                    Value = "-1"
                });

                return list;
            }
            else
            {

                FileExtns = new List<string> { "Liberty_SSM_MPB_Config", "Liberty_SSM_CCF", "Liberty_SSM_RPG", "Liberty_SSM_MB_Pgm", "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM", "ST Sensor Config", "CS-Park Config", "ST Sensor Binary", "CS-Park Bin", "Gateway Config", "Gateway Bin", "Gateway GPRS" };
                List<SelectListItemWrapper> list = (from FDFiles in PemsEntities.FDFileTypes
                                                    where FileExtns.Contains(FDFiles.FileDesc)

                                                    select new SelectListItemWrapper()
                                                    {

                                                        Text = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                                        ValueInt = FDFiles.FileType
                                                    }).ToList();

                list.Insert(0, new SelectListItemWrapper()
                {

                    Text = "",
                    Value = "-1"
                });

                return list;
            }
        }


        public IQueryable<FDFilesModel> GetFileTypesForUpload(string Roletype)
        {

            {
                List<string> FileExtns;
                if (Roletype.ToLower() == "admin")
                {
                    //   FileExtns = new List<string> { "BIN", "SNSR" };
                    FileExtns = new List<string> { "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM" };
                    var Files = (from FDFiles in PemsEntities.FDFileTypes
                                 // where FileExtns.Contains(FDFiles.FileExtension) && !FDFiles.FileDesc.ToUpper().Contains("VX meter executable code") && !FDFiles.FileDesc.ToUpper().Contains("Liberty_SSM_MPB_Config")
                                 where FileExtns.Contains(FDFiles.FileDesc)
                                 select new FDFilesModel
                                 {
                                     FileName = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                     FileType = FDFiles.FileType,
                                     //selected = true
                                 });
                    return Files;
                }
                else
                {
                    //  FileExtns = new List<string> { "CCF", "RPG", "PGM", "BIN", "SNSR" };
                    FileExtns = new List<string> { "Liberty_SSM_MPB_Config", "Liberty_SSM_CCF", "Liberty_SSM_RPG", "Liberty_SSM_MB_Pgm", "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM", "ST Sensor Config", "CS-Park Config", "ST Sensor Binary", "CS-Park Bin", "Gateway Config", "Gateway Bin", "Gateway GPRS" };
                    var Files = (from FDFiles in PemsEntities.FDFileTypes
                                 // where FileExtns.Contains(FDFiles.FileExtension) && !FDFiles.FileDesc.ToUpper().Contains("VX meter executable code")
                                 where FileExtns.Contains(FDFiles.FileDesc)
                                 select new FDFilesModel
                                 {
                                     FileName = FDFiles.FileDesc + "(." + FDFiles.FileExtension + ")",
                                     FileType = FDFiles.FileType,
                                     //selected = true
                                 });
                    return Files;
                };


            }
        }

        /// <summary>
        ///Description:UpdateFile_Active method will execute a query to set the file as canceled is 'JobStatusID' of 'FDJobs' and call UpdateFDFilesAudit to set the reason for cancellation.
        ///Modified:Prita()
        /// </summary>
        /// <param name="Griddata"></param>
        /// <param name="status"></param>
        /// <param name="userId"></param>
        public void UpdateFile_Active(FDFilesModel1 Griddata, string status, int userId)
        {
            int myfilestatus;
            if (status.Trim().Length != 0)
            {
                myfilestatus = Convert.ToInt32(status);
            }
            else
            {
                myfilestatus = -1;
            }
            if (myfilestatus != -1)
            {
                var stud = (from s in PemsEntities.FDJobs
                            where s.FileID == Griddata.FileID
                            && s.JobID == Griddata.JobID
                            select s).FirstOrDefault();
                //stud.ActiveJob =FDStatus== 0 ? 0 : 1,
                if (myfilestatus == 0)
                { stud.ActiveJob = 0; }
                else
                { stud.ActiveJob = 1; }
                stud.JobStatus = Griddata.JobStatusID;
                UpdateFDFilesAudit(Griddata, myfilestatus, userId);
                int num = PemsEntities.SaveChanges();
            }
        }

        /// <summary>
        /// Description:This will set the comment/reason for cancelling the file in FDFilesAudits table.
        /// Modified:Prita()
        /// </summary>
        /// <param name="Griddata"></param>
        /// <param name="FDStatus"></param>
        /// <param name="cancelledById"></param>
        public void UpdateFDFilesAudit(FDFilesModel1 Griddata, int FDStatus, int cancelledById)
        {


            var Audit = (from s in PemsEntities.FDFilesAudits
                         where s.FileId == Griddata.FileID
                         && s.JobId == Griddata.JobID
                         select s).FirstOrDefault();
            //stud.ActiveJob =FDStatus== 0 ? 0 : 1,
            Audit.FileStatus = 0;
            Audit.DateCanceled = System.DateTime.Now;
            Audit.UploadedBy = cancelledById;
            int num = PemsEntities.SaveChanges();
        }
        /// <summary>
        ///Description:This will Query a number of joined tables including meters,FDJobs,FDFiles,FDFileAudits etc to fetch file download details and send it to UI to be visible in the 'FDInquiry' grid.
        ///Modified:Prita()
        /// </summary>
        /// <param name="request"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<FDFilesModel1> GetFilesummary(DataSourceRequest request, int CurrentCity, string Roletype, out int total)
        {
            string paramValues = string.Empty;
            var spParams = GetSpParams(request, "TimeOfOccurrance desc", out paramValues);
            total = 0;
            string startDate = spParams[3].Value.ToString();
            string endDate = spParams[4].Value.ToString();
            string filetype = spParams[5].Value.ToString();
            string Filestatus = spParams[6].Value.ToString();
            string AssetID = spParams[7].Value.ToString();

            int myfiletype;
            int myfilestatus;
            int myAssetID;
            if (filetype.Trim().Length != 0)
            {
                myfiletype = Convert.ToInt32(filetype);
            }
            else
            {
                myfiletype = -1;
            }
            //file status not being used as of now
            if (Filestatus.Trim().Length != 0)
            {
                myfilestatus = Convert.ToInt32(Filestatus);
            }
            else
            {
                myfilestatus = -1;
            }
            if (AssetID.Trim().Length != 0)
            {
                myAssetID = Convert.ToInt32(AssetID);
            }
            else
            {
                myAssetID = -1;
            }

            DateTime startTimeOutput;
            DateTime endTimeOutput;

            if (DateTime.TryParse(startDate, out startTimeOutput))
            {
                // ** Valid start date
            }

            if (DateTime.TryParse(endDate, out endTimeOutput))
            {
                //** Valid end date
            }
            List<string> FileExtns;
            if (Roletype.ToLower() == "admin")
            {
                //FileExtns = new List<string> { "BIN", "SNSR" };
                FileExtns = new List<string> { "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM" };
                CurrentCity = -1;
            }
            else
            {
                //  FileExtns = new List<string> { "CCF", "RPG", "PGM", "BIN", "SNSR" };
                FileExtns = new List<string> { "Liberty_SSM_MPB_Config", "Liberty_SSM_CCF", "Liberty_SSM_RPG", "Liberty_SSM_MB_Pgm", "Liberty_SSM_MPB_Pgm", "LIBERTY_SSM_SENSOR_PGM", "ST Sensor Config", "CS-Park Config", "ST Sensor Binary", "CS-Park Bin", "Gateway Config", "Gateway Bin", "Gateway GPRS" };

            };
            var ddlItems = new List<FDFilesModel1>();
            List<FDFilesModel1> FileUploadInfo = new List<FDFilesModel1>();
            try
            {
                //IQueryable<FDFilesModel1> items = null;

                var query = (from FD_Jobs in PemsEntities.FDJobs

                             join FD_Files in PemsEntities.FDFiles
                             on FD_Jobs.FileID equals FD_Files.FileID

                             join FD_Audit in PemsEntities.FDFilesAudits
                              on FD_Jobs.JobID equals FD_Audit.JobId

                             join FD_FileTypes in PemsEntities.FDFileTypes
                             on FD_Files.FileType equals FD_FileTypes.FileType

                             join FD_JobStatus in PemsEntities.FDJobStatus
                             on FD_Jobs.JobStatus equals FD_JobStatus.JobStatus

                             join FD_Meters in PemsEntities.Meters
                              on new { FD_Jobs.CustomerID, FD_Jobs.AreaID, FD_Jobs.MeterId }
                              equals new { FD_Meters.CustomerID, FD_Meters.AreaID, FD_Meters.MeterId }

                             join Customer in PemsEntities.Customers
                             on FD_Jobs.CustomerID equals Customer.CustomerID

                             where (FD_Jobs.CustomerID == CurrentCity || CurrentCity == -1)
                             && (FD_Jobs.ActiveJob == myfilestatus || myfilestatus == -1)
                             && (FD_Jobs.MeterId == myAssetID || myAssetID == -1)
                             && (FD_Meters.MeterName != null)
                             && (FD_Jobs.SubmittedDate >= startTimeOutput && FD_Jobs.SubmittedDate <= endTimeOutput)
                             && (FD_Files.FileType == myfiletype || myfiletype == -1)
                              && FileExtns.Contains(FD_FileTypes.FileDesc)
                             select new FDFilesModel1
                             {
                                 FileID = FD_Jobs.FileID,
                                 JobID = FD_Jobs.JobID,
                                 FileName = FD_Files.FileName,
                                 FileComments = FD_Files.FileComments,
                                 FileTypeName = FD_FileTypes.FileDesc,
                                 FileType = FD_Files.FileType,
                                 Active = FD_Jobs.ActiveJob,
                                 StatusDesc = FD_Jobs.ActiveJob == 0 ? "Inactive" :
                                 FD_Jobs.ActiveJob == 1 ? "Active" : "-",
                                 JobStatusID = FD_Jobs.JobStatus,
                                 JobStatusDesc = FD_JobStatus.StatusDesc,
                                 FileAdditionDate = FD_Jobs.AvailableDate,
                                 DateCompleted = FD_Jobs.ActivationDate,
                                 MeterID = FD_Jobs.MeterId,
                                 MeterName = FD_Meters.MeterName,
                                 UploadedBy = Customer.Name,
                                 CanceledById = FD_Jobs.ActiveJob == 1 ? 0 : FD_Audit.UploadedBy,//** change made on 19th sep 2014
                                 FileCancelationDate = FD_Jobs.ActiveJob == 1 ? null : FD_Audit.DateCanceled
                             });

                //    query = query.ApplyFiltering(request.Filters);


                // Step 3: Get count of filtered items
                total = query.Count();

                // Step 4: Apply sorting
                query = query.ApplySorting(request.Groups, request.Sorts);
                //applyPaging=true
                // Step 5: Apply paging
                //if (applyPaging)
                query = query.ApplyPaging(request.Page, request.PageSize);
                // Step 6: Get the data!
                return query.ToList();
            }


            catch (Exception ex)
            {
            }
            return new List<FDFilesModel1>();
            //return Filesummary;
        }

        /// <summary>
        ///Description:GetAssetName will get corresponding MeterName of the parameter's meter ID form meters table.
        ///Modified:Prita()
        /// </summary>
        /// <param name="outNum"></param>
        /// <returns></returns>
        public string GetAssetName(int MeterID)
        {

            return PemsEntities.Meters.Where(a => a.MeterId == MeterID).Select(a => a.MeterName).FirstOrDefault();

        }

        /// <summary>
        ///Description:GetAssetName will get corresponding MeterID of the parameter's meter Name form meters table.
        ///Modified:Prita()
        /// </summary>
        /// <param name="meterName"></param>
        /// <returns></returns>
        public int GetAssetID(string meterName)
        {

            return PemsEntities.Meters.Where(a => a.MeterName == meterName).Select(a => a.MeterId).FirstOrDefault();

        }
        /// <summary>
        ///Description:GetAllAssetTypes will return meterIDs for the selected file type. If no file type in particular is selected then all meterIDs in the Meters will be visible in the Asset ID and Name auto Complete
        ///Modified:Prita()
        /// </summary>
        /// <param name="FileType"></param>
        /// <param name="CurrentCity"></param>
        /// <param name="searchby"></param>
        /// <param name="typedtext"></param>
        /// <returns></returns>
        public List<FDFilesModel1> GetAllAssetTypes(String FileType, int CurrentCity, string Roletype, int searchby = 2, string typedtext = "")
        {
            int myfiletype;
            if (FileType.Trim().Length != 0)
            {
                myfiletype = Convert.ToInt32(FileType);
            }
            else
            {
                myfiletype = -1;
            }
            if (Roletype.ToLower() == "admin")
            {
                CurrentCity = -1;
            }
            if (myfiletype != -1)
            {
                return (from FD_Jobs in PemsEntities.FDJobs
                        join FD_Files in PemsEntities.FDFiles
                        on FD_Jobs.FileID equals FD_Files.FileID
                        join Meters in PemsEntities.Meters
                       on FD_Jobs.MeterId equals Meters.MeterId
                        where (FD_Jobs.CustomerID == CurrentCity || CurrentCity == -1)
                        && (FD_Files.FileType == myfiletype)
                        select new FDFilesModel1
                        {
                            Text = Meters.MeterName,
                            Value = FD_Jobs.MeterId
                        }).ToList();
            }
            else
            {
                return (from Meters in PemsEntities.Meters
                        where (Meters.CustomerID == CurrentCity || CurrentCity == -1)
                        select new FDFilesModel1
                        {
                            Text = Meters.MeterName,
                            Value = Meters.MeterId
                        }).ToList();
            }
        }

        /// <summary>
        ///Description:GetAllJobComments will query 'FDJobStatus' table to get all comments that can possibly be a reason to cancel a download.
        ///Modified:Prita()
        /// </summary>
        /// <returns></returns>
        public List<FDFilesModel1> GetAllJobComments()
        {
            return (from FD_JobStatus in PemsEntities.FDJobStatus
                    select new FDFilesModel1
                    {
                        JobStatusID = FD_JobStatus.JobStatus,
                        JobStatusName = FD_JobStatus.StatusDesc
                    }).ToList();

        }



        /// <summary>
        /// Description: This function is used to calculate date and time for the selected Time zone for the customer
        /// Modified By: Sairam on 25th sep 2014
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public List<DateTime> GetTimeZoneNames(int customerID)
        {
            List<SelectListItem> Result = new List<SelectListItem>();
            List<DateTime> TimeZoneResult = new List<DateTime>();

            try
            {
                //** Get the path of the xml file to load;
                String xmlFilePath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Timezones/TimeZones.xml");

                //** Create the Dataset and store the xml content in it;
                DataSet mydata = new DataSet();
                mydata.ReadXml(xmlFilePath);

                DataTable DestTable = mydata.Tables[4]; // Get the destination table data.

                Result = processDataTable(DestTable);

                //** Get the Timezone ID from the customer table and send to the function

                var customerInfo = (from cus in PemsEntities.Customers
                                    where cus.CustomerID == customerID
                                    select cus).FirstOrDefault();

                var shortTimeZoneName = (from tz in PemsEntities.TimeZones
                                         where tz.TimeZoneID == customerInfo.TimeZoneID
                                         select tz).FirstOrDefault();


                var toBeSearched = shortTimeZoneName.TimeZoneName;
                var zoneId = "";


                //** Now process the Result List to find 'toBeSearched' item. If it matches, get the Time zone name.
                for (var i = 0; i < Result.Count(); i++)
                {
                    if (Result[i].Value.ToString().Trim().ToUpper() == toBeSearched.Trim().ToUpper())
                    {
                        zoneId = Result[i].Text;
                        break;
                    }
                }

                var localtime = DateTime.Now;

                var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(zoneId);




                //** Now calculate the time for the selected time zone
                var dataTimeByZoneId = TimeZoneInfo.ConvertTime(localtime, TimeZoneInfo.Local, timeZoneInfo);

                //**** Since the 'TimeZoneInfo.ConvertTime' gives Day light saving time by default, hence no need to check time.
                TimeZoneResult.Add(Convert.ToDateTime(dataTimeByZoneId));

            }
            catch (Exception e)
            {

            }



            return TimeZoneResult;
        }




        /// <summary>
        /// Description: This function is used to process the Timezone.xml file to fetch the full / correct time zone names from the short timezone names
        /// Modified By: Sairam on 25th 2014;
        /// </summary>
        /// <param name="TargetTable"></param>
        /// <returns></returns>
        public List<SelectListItem> processDataTable(DataTable TargetTable)
        {
            List<SelectListItem> result = new List<SelectListItem>();

            foreach (DataRow row in TargetTable.Rows) // Loop over the rows.
            {
                foreach (DataColumn col in row.Table.Columns)  //loop through the columns. 
                {

                    string columnToBeSearchedIs = "other";
                    string colNameFromDS = col.ColumnName;

                    if (columnToBeSearchedIs == colNameFromDS)
                    {
                        string TimeZoneNameIs = row["other"].ToString();
                        string TimeZoneShortNameIs = row["type"].ToString();
                        if (!string.IsNullOrEmpty(TimeZoneNameIs))
                        {
                            //** Only if the column name starts with "Col" has some value, insert into list;
                            result.Add(new SelectListItem { Text = TimeZoneNameIs, Value = TimeZoneShortNameIs });


                        }
                    }

                }

            };

            return result;
        }



        #endregion

    }
}
