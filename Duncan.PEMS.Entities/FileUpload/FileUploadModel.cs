using Duncan.PEMS.DataAccess.PEMS;
using Duncan.PEMS.Entities.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Duncan.PEMS.Entities.FileUpload
{


    public class FDFilesModel
    {
        public long FileID { get; set; }

        public long JobID { get; set; }


        public string Text { get; set; }

        public int FileType { get; set; }

        [OriginalGridPosition(Position = 1)]
        public string FileTypeText { get; set; }
        public string FileHash { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string FileComments { get; set; }

        public long FileSizeBytes { get; set; }

        public DateTime FileAdditionDate { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string FileDate { get; set; }
        //  public string FileAdditionDateFinal { get; set; }


        public byte[] FileRawData { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string FileName { get; set; }

        public Nullable<long> OriginalFileID { get; set; }
        public int Active { get; set; }
        public long JobStatus { get; set; }
        public string StatusDesc { get; set; }
        public int CustmerID { get; set; }
        public string CustmerName { get; set; }
        public int AreaID { get; set; }
        public int MeterID { get; set; }

        [OriginalGridPosition(Position = 5)]
        public string UploadedBy { get; set; }

        public int Userid { get; set; }

        [OriginalGridPosition(Position = 3)]
        public string Activestaus { get; set; }



        public int Value { get; set; }
    }
    public class FDFilesModel1
    {

        public long FileID { get; set; }
        public long JobID { get; set; }
        public int FileType { get; set; }
        public string FileHash { get; set; }
        public string FileComments { get; set; }
        public long FileSizeBytes { get; set; }
        public DateTime? FileAdditionDate { get; set; }
        public string FileDate { get; set; }
        [OriginalGridPosition(Position = 7)]
        public string FileAdditionDateFinal { get; set; }
        public string FileDateCompleted { get; set; }
        public byte[] FileRawData { get; set; }
        [OriginalGridPosition(Position = 0)]
        public string FileName { get; set; }
        public Nullable<long> OriginalFileID { get; set; }
        public int Active { get; set; }
        public long JobStatus { get; set; }
        [OriginalGridPosition(Position = 5)]
        public string StatusDesc { get; set; }
        public int CustmerID { get; set; }
        public string CustmerName { get; set; }
        public int AreaID { get; set; }
        [OriginalGridPosition(Position = 3)]
        public int MeterID { get; set; }

        [OriginalGridPosition(Position = 8)]
        public string UploadedBy { get; set; }
        public string Activestaus { get; set; }
        [OriginalGridPosition(Position = 4)]
        public string MeterName { get; set; }
        [OriginalGridPosition(Position = 2)]
        public string JobStatusDesc { get; set; }
        //added by prita on 27/5/2014 for asset types
        public long Value { get; set; }
        public string Text { get; set; }
        [OriginalGridPosition(Position = 1)]
        public string FileTypeName { set; get; }

        public int JobStatusID { set; get; }

        public string JobStatusName { set; get; }
        public DateTime? DateCompleted { get; set; }
        [OriginalGridPosition(Position = 6)]
        public string DateCompletedFinal { get; set; }
        [OriginalGridPosition(Position = 10)]
        public string CanceledBy { set; get; }
        public int? CanceledById { set; get; }
        public DateTime? FileCancelationDate { get; set; }
        [OriginalGridPosition(Position = 9)]
        public string FileCancelationDateFinal { get; set; }
    }
    public class FileAsset
    {
        public string AssetIDIs { get; set; }
        public string FileNameIs { get; set; }
    }
}