using System;
using System.Web;
using Duncan.PEMS.Business.WebServices;
using Duncan.PEMS.WebServices.ServiceManagers;
namespace Duncan.PEMS.WebServices.FieldMaintenance
{
    public class GetWorkOrders : BaseFieldMaintenanceService, IHttpHandler
    {
        #region Handler

        bool IHttpHandler.IsReusable
        {
            get { throw new NotImplementedException(); }
        }
        void IHttpHandler.ProcessRequest(HttpContext context)
        {
            try
            {
                //Handling CRUD - they are only POSTing data to us, so thats all we care about.
                switch (context.Request.HttpMethod)
                {
                    case "GET":
                        //comment
                        WriteResponse("You must POST Data");
                        break;
                    case "POST":
                        //Perform CREATE Operation
                        ProcessRequest(context);
                        break;
                }
            }
            catch (Exception ex)
            {
                var errorData = ErrorHandler.GenerateError(ex, "500");
                WriteResponse(Serialize(errorData));
            }
        }
        #endregion

        #region Process Request
        private void ProcessRequest(HttpContext context)
        {
            try
            {
                // HTTP POST sends name/value pairs to a web server
                // data is sent in message body
                // Extract the content of the Request and make a Data class
                // The message body is posted as bytes. read the bytes
                byte[] postData = context.Request.BinaryRead(context.Request.ContentLength);
                // deserialize post data into data class
                Data data = Deserialize(postData);

                //now that we have our request, we need to pass it to the web wervice factory to modify it. 
                //It will return a Data itht he correct return types, this will perform no busienss logic.
                var response = (new WebServiceFactory()).GetWorkOrders(data);
                WriteResponse(Serialize(response));
            }
            catch (Exception ex)
            {
                var errorData = ErrorHandler.GenerateError(ex, "500");
                WriteResponse(Serialize(errorData));
            }
        }
        #endregion
    }
}
