using System;

namespace Duncan.PEMS.WebServices.ServiceManagers
{
   public static class ErrorHandler
    {

        //returns 400 (Bad Request) if the create request could not be satisfied. 	
    //    <Error>
    //    <Message>XXXXXX</Message>
    //    <Code>400</Code>
    //</Error>

        //returns 500 Internal Server Error if there is any issue at sever side.	
    //   <Error>
    //    <Message>XXXXXX</Message>
    //    <Code>500</Code>
    //</Error>

        
       /// <summary>
       /// Return a data element with the exception data included
       /// </summary>
       /// <param name="ex"></param>
       /// <param name="code"></param>
       /// <returns></returns>
       public static Data GenerateError(Exception ex, string code)
       {
           var data = new Data
               {
                   Error = new DataError {ItemsElementName = new ItemsChoiceType[2], Items = new string[2]}
               };

           //add the message
           data.Error.ItemsElementName[0] = ItemsChoiceType.Message;
           data.Error.Items[0] = ex.Message + ": " + ex.StackTrace;
           //add the code
           data.Error.ItemsElementName[0] = ItemsChoiceType.Code;
           data.Error.Items[0] = code;

           return data;

       }


    }
}
