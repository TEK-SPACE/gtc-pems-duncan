
using System.Drawing;
using System.IO;

namespace Duncan.PEMS.Business.Utility
{
    /// <summary>
    /// Factory to manipulate iamges within the system
    /// </summary>
    public static class ImageFactory
    {
        /// <summary>
        /// Creates a byte array out of the image passed in (to store in the db)
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts the byte array to a usable image
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns></returns>
        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            using (var ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }
        }

        /// <summary>
        /// Converts a stream to an byte array
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] StreamToByteArray(Stream stream)
        {
            // create byte array 
            byte[] attachmentBytes = new byte[stream.Length];
            // read attachment into byte array 
            stream.Read(attachmentBytes, 0, attachmentBytes.Length);
            return attachmentBytes;
        }
    }
}
