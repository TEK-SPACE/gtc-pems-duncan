using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Duncan.PEMS.Business.Grids;
using Duncan.PEMS.Entities.General;
using Duncan.PEMS.Entities.Grids;
using Duncan.PEMS.Utilities;
using Kendo.Mvc;
using NLog;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Duncan.PEMS.Business.Exports
{
    /// <summary>
    /// The <see cref="Duncan.PEMS.Business.Exports"/> namespace contains classes for managing exports of grids.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {
    }

    /// <summary>
    /// Class to encapsulate the management of the file streams used in exporting grids to
    /// PDFs, Excel or CSV.
    /// </summary>
    public  class ExportFactory
    {
        #region NLog Logger

        /// <summary>
        /// NLog logger instance.
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
       /// Copies one list of items to another list
       /// </summary>
       public static T1 CopyFrom<T1, T2>(T1 destination, T2 source)where T1 : class where T2 : class
       {
           PropertyInfo[] srcFields = source.GetType().GetProperties(
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

           PropertyInfo[] destFields = destination.GetType().GetProperties(
               BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);

           foreach (var property in srcFields)
           {
               var dest = destFields.FirstOrDefault(x => x.Name == property.Name);
               if (dest != null && dest.CanWrite)
                   dest.SetValue(destination, property.GetValue(source, null), null);
           }
           return destination;
       }

       /// <summary>
       /// Gets the memory stream for a csv file - used in exporting data
       /// </summary>
       public MemoryStream GetCsvFileMemoryStream<T>(IEnumerable<T> items, string controller, string action, int customerId)
       {
           var output = new MemoryStream();
           var writer = new StreamWriter(output, Encoding.UTF8);
          
           //get the grid data for this controller/action. this iwll get all non hidden columns
           var dbGridData = (new GridFactory()).GetExportGridData(controller, action, customerId);

           //now get a list of name / position items based on the properties of the current object type passed in with the correct positional attribute.
           //get the properties of the object that have the position attribute and order them by original position
           //these should be sorted by original position!
           var itemPositions = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(OriginalGridPositionAttribute), true).OrderBy(x=>((OriginalGridPositionAttribute)x).Position).Any())
               .ToList().Select(x => new ItemPositions { Name = x.Name, OriginalPosition = ((OriginalGridPositionAttribute)x.GetCustomAttribute(typeof(OriginalGridPositionAttribute))).Position }).ToList();

           var gridData = new List<GridData>();
           foreach (var dbGridItem in dbGridData)
           {
               //need to filter out any grid data items that do not exist in the object we are using <T>
               var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == dbGridItem.OriginalPosition);
              //it exists, add it to our list of columsn to use / add
               if (itemPosition != null)
                   gridData.Add(dbGridItem);
           }
           //check to make sure that the grid data pulled back has an assocated property on the object with the correct attribute. if it doesnt exist in our itempositions list, then we dont need to add it as a header column.
           int totalGridDataCount = gridData.Count();
           //first thing we have to do it spit out only columns as headers that have possible positions. Some columns (like differnce flag on the agg meters) will never be displayed and are not defined in the CustomerGrids table.
          
           for (int i = 0; i < gridData.Count; i++)
           {
               var data = gridData[i];
              
               if ((i+1) == totalGridDataCount)
                   AddCsvValue(writer, data.Title, true);
               else
                   AddCsvValue(writer, data.Title);
           }

           //then generate a list of original positions for this class. 
           if (itemPositions.Any())
           {
               foreach (var item in items)
               {
                   //now for each item, foreach grid data itme, get the property of the object that has teh corrosponding original position (custom attribute) and add that items value
                   int currentIteration = 1;
                   foreach (var data in gridData)
                   {
                       var valueToAdd = string.Empty;
                       //go grab the property with the same ORIGINAL position, this will be the value we need
                       var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == data.OriginalPosition);
                       if (itemPosition != null)
                       {
                           //now that we have the name of the property to use, gt teh value of the property
                           var propertyValue = item.GetType().GetProperty(itemPosition.Name).GetValue(item, null);
                           if (propertyValue != null)
                               valueToAdd = propertyValue.ToString();
                       }
                       if (currentIteration == totalGridDataCount)
                           AddCsvValue(writer, valueToAdd, true);
                       else
                           AddCsvValue(writer, valueToAdd);
                       currentIteration++;
                   }
               }
           }
           writer.Flush();
           output.Position = 0;
           return output;
       }

       /// <summary>
       /// Gets the memory stream for a excel file - used in exporting data
       /// </summary>
       public MemoryStream GetExcelFileMemoryStream<T>(IEnumerable<T> items, string controller, string action, int customerId, List<FilterDescriptor> filters)
       {
           //Create new Excel workbook
           var workbook = new HSSFWorkbook();
           //Create new Excel sheet
           var sheet = workbook.CreateSheet();
           //add the filtered values to the top of the excel file
           // Add the filtered values to the top of the excel file

           var rowNumber = AddFiltersToExcelSheet(sheet, filters);

           //get the grid data for this controller/action. this iwll get all non hidden columns
           var dbGridData = (new GridFactory()).GetExportGridData(controller, action, customerId);

           //now get a list of name / position items based on the properties of the current object type passed in with the correct positional attribute.
           //get the properties of the object that have the position attribute and order them by original position
           //these should be sorted by original position!
           var itemPositions = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(OriginalGridPositionAttribute), true).OrderBy(x => ((OriginalGridPositionAttribute)x).Position).Any())
               .ToList().Select(x => new ItemPositions { Name = x.Name, OriginalPosition = ((OriginalGridPositionAttribute)x.GetCustomAttribute(typeof(OriginalGridPositionAttribute))).Position }).ToList();

           var gridData = new List<GridData>();
           foreach (var dbGridItem in dbGridData)
           {
               //need to filter out any grid data items that do not exist in the object we are using <T>
               var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == dbGridItem.OriginalPosition);
               //it exists, add it to our list of columsn to use / add
               if (itemPosition != null)
                   gridData.Add(dbGridItem);
           }
           //check to make sure that the grid data pulled back has an assocated property on the object with the correct attribute. if it doesnt exist in our itempositions list, then we dont need to add it as a header column.
           //first thing we have to do it spit out only columns as headers that have possible positions. Some columns (like differnce flag on the agg meters) will never be displayed and are not defined in the CustomerGrids table.
           //Create a header row
           var headerRow = sheet.CreateRow(rowNumber++);
           for (int i = 0; i < gridData.Count; i++)
           {
               var data = gridData[i];
               headerRow.CreateCell(i).SetCellValue(data.Title);
           }

           //then generate a list of original positions for this class. 
           if (itemPositions.Any())
           {
               foreach (var item in items)
               {
                   int columnNumber = 0;
                   var row = sheet.CreateRow(rowNumber++);
                   //now for each item, foreach grid data itme, get the property of the object that has teh corrosponding original position (custom attribute) and add that items value
                   foreach (var data in gridData)
                   {
                       var valueToAdd = string.Empty;
                     
                       //go grab the property with the same ORIGINAL position, this will be the value we need
                       var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == data.OriginalPosition);
                       if (itemPosition != null)
                       {
                           //now that we have the name of the property to use, gt teh value of the property
                           var propertyValue = item.GetType().GetProperty(itemPosition.Name).GetValue(item, null);
                           if (propertyValue != null)
                               valueToAdd = propertyValue.ToString();
                       }
                       row.CreateCell(columnNumber++).SetCellValue(valueToAdd);
                   }
               }
           }

           //Write the workbook to a memory stream
           var output = new MemoryStream();
           workbook.Write(output);
           return output;
       }

       /// <summary>
       /// Gets the memory stream for a pdf file - used in exporting data
       /// </summary>
       public MemoryStream GetPDFFileMemoryStream<T>(IEnumerable<T> items, string controller, string action, int customerId, List<FilterDescriptor> filters, int headerRows)
       {

           // step 1: creation of a document-object
           var document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);

           //step 2: we create a memory stream that listens to the document
           var output = new MemoryStream();
           PdfWriter.GetInstance(document, output);

           //step 3: we open the document
           document.Open();
           //step 4: we add content to the document

           //first we have to add the filters they used
           document = AddFiltersToPdf(document, filters);

           //then we add the data
           document.Add(new Paragraph("Results:  "));
           document.Add(new Paragraph(" "));

           //get the grid data for this controller/action. this iwll get all non hidden columns
           var dbGridData = (new GridFactory()).GetExportGridData(controller, action, customerId);

           //now get a list of name / position items based on the properties of the current object type passed in with the correct positional attribute.
           //get the properties of the object that have the position attribute and order them by original position
           //these should be sorted by original position!
           var itemPositions = typeof(T).GetProperties().Where(p => p.GetCustomAttributes(typeof(OriginalGridPositionAttribute), true).OrderBy(x => ((OriginalGridPositionAttribute)x).Position).Any())
               .ToList().Select(x => new ItemPositions { Name = x.Name, OriginalPosition = ((OriginalGridPositionAttribute)x.GetCustomAttribute(typeof(OriginalGridPositionAttribute))).Position }).ToList();

           var gridData = new List<GridData>();
           foreach (var dbGridItem in dbGridData)
           {
               //need to filter out any grid data items that do not exist in the object we are using <T>
               var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == dbGridItem.OriginalPosition);
               //it exists, add it to our list of columsn to use / add
               if (itemPosition != null)
                   gridData.Add(dbGridItem);
           }

           //assuming that the column cnt will be the same as the actual grid data count
           //determine the column count. it is the total griddata count / header row count rounded up.

           var columnCount = gridData.Count();
           if (headerRows > 1)
               columnCount = (int)Math.Ceiling((double)columnCount / (double)headerRows);


           var dataTable = new PdfPTable(columnCount) { WidthPercentage = 100 };
           dataTable.DefaultCell.Padding = 3;
           dataTable.DefaultCell.BorderWidth = 2;
           dataTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
           //
        //check to make sure that the grid data pulled back has an assocated property on the object with the correct attribute. if it doesnt exist in our itempositions list, then we dont need to add it as a header column.
           //first thing we have to do it spit out only columns as headers that have possible positions. Some columns (like differnce flag on the agg meters) will never be displayed and are not defined in the CustomerGrids table.
        
           // Adding headers
           foreach (var data in gridData)
               dataTable.AddCell(data.Title);
           dataTable.CompleteRow();

           dataTable.HeaderRows = headerRows;
           dataTable.DefaultCell.BorderWidth = 1;

        //then generate a list of original positions for this class. 
           if (itemPositions.Any())
           {
               int rowIndex = 0;
               foreach (var item in items)
               {
                   dataTable.DefaultCell.BackgroundColor = rowIndex % 2 != 0 ? new BaseColor(ColorTranslator.FromHtml("#ccc")) : new BaseColor(ColorTranslator.FromHtml("#fff"));
                   rowIndex++;
                   //now for each item, foreach grid data itme, get the property of the object that has teh corrosponding original position (custom attribute) and add that items value
                   foreach (var data in gridData)
                   {
                       var valueToAdd = string.Empty;

                       //go grab the property with the same ORIGINAL position, this will be the value we need
                       var itemPosition = itemPositions.FirstOrDefault(x => x.OriginalPosition == data.OriginalPosition);
                       if (itemPosition != null)
                       {
                           //now that we have the name of the property to use, gt teh value of the property
                           var propertyValue = item.GetType().GetProperty(itemPosition.Name).GetValue(item, null);
                           if (propertyValue != null)
                               valueToAdd = propertyValue.ToString();
                       }
                       dataTable.AddCell(valueToAdd);
                   }
                   dataTable.CompleteRow();
               }
           }

           // Add table to the document
           document.Add(dataTable);
           //This is important don't forget to close the document
           document.Close();
           return output;
       }

       /// <summary>
       /// Adds a list of filters in the header to a pdf document
       /// </summary>
       protected Document AddFiltersToPdf(Document document, List<FilterDescriptor> customFilters)
       {
           var filterTable = new PdfPTable(2);
           filterTable.DefaultCell.Padding = 3;
           filterTable.DefaultCell.BorderWidth = 2;
           filterTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

           // Adding headers
           filterTable.AddCell("Filter Name");
           filterTable.AddCell("Filter Value");
           filterTable.CompleteRow();
           filterTable.HeaderRows = 1;
           filterTable.DefaultCell.BorderWidth = 1;

           if (customFilters.Any())
           {
               CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
               TextInfo textInfo = cultureInfo.TextInfo;
               foreach (var item in customFilters)
               {
                   if (!string.IsNullOrEmpty(item.Value.ToString()))
                   {
                       //get the friendly name for the filter (Asset Status instead of assetStatus)
                       var friendlyName = HelperMethods.GetFriendlyLocalizedName(item.Member, textInfo);
                       filterTable.AddCell(friendlyName);
                       filterTable.AddCell(item.Value.ToString());
                   }
               }
               // Add table to the document
               document.Add(new Paragraph("Filters:  "));
               document.Add(filterTable);
               document.Add(new Paragraph(" "));
           }
           return document;
       }

       /// <summary>
       /// Adds a list of filters in the header to a excel document
       /// </summary>
       protected int AddFiltersToExcelSheet(ISheet sheet, List<FilterDescriptor> customFilters)
       {
           int rowNumber = 0;
           //filters can be filter descriptor, or composit filter descriptor, so we need to recursively generate this
           if (customFilters.Any())
           {
               CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
               TextInfo textInfo = cultureInfo.TextInfo;

               //Create a header row
               var filterRow = sheet.CreateRow(rowNumber++);

               //Set the column names in the header row
               filterRow.CreateCell(0).SetCellValue("Filter Name");
               filterRow.CreateCell(1).SetCellValue("Filter Value");
               foreach (var item in customFilters)
               {
                   if (!string.IsNullOrEmpty(item.Value.ToString()))
                   {
                       int columnNumber = 0;
                       var row = sheet.CreateRow(rowNumber++);
                       var friendlyName = HelperMethods.GetFriendlyLocalizedName(item.Member, textInfo);
                       row.CreateCell(columnNumber++).SetCellValue(friendlyName);
                       row.CreateCell(columnNumber++).SetCellValue(item.Value.ToString());
                   }
               }

               //if we added filters, lets add a blank row for readability
               var blankRow = sheet.CreateRow(rowNumber++);
               blankRow.CreateCell(0).SetCellValue(" ");
           }
           return rowNumber;
       }


       /// <summary>
       /// Adds a value to a csv file
       /// </summary>
       protected static void AddCsvValue(StreamWriter writer, string value, bool lastCellInRow = false)
       {
           //last one doesnt get a comma at the end!
           string formatted = String.Format("{0}{1}{2}{3}",Constants.Export.EscapeChar,value,Constants.Export.EscapeChar,lastCellInRow ? "" : Constants.Export.DelimiterChar);
           writer.Write(formatted);
           if (lastCellInRow)
               writer.WriteLine();
       }

       private class ItemPositions
       {
           public string Name { get; set; }
           public int OriginalPosition { get; set; }
       }

    }
}
