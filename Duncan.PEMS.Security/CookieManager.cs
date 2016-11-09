using System;
using System.Text;
using System.Web;
using System.Web.Security;

namespace Duncan.PEMS.Security
{
    public static class CookieManager
    {
        public static HttpCookie Encode(HttpCookie cookie)
        {
            HttpCookie encodedCookie = CloneCookie( cookie );
            encodedCookie.Value = EncodeString( encodedCookie.Value );
            return encodedCookie;
        }

        public static HttpCookie Decode(HttpCookie cookie)
        {
            HttpCookie encodedCookie = CloneCookie( cookie );
            encodedCookie.Value = DecodeString( encodedCookie.Value );
            return encodedCookie;
        }

        public static HttpCookie CloneCookie(HttpCookie cookie)
        {
            var clonedCookie = new HttpCookie( cookie.Name, cookie.Value )
                                   {
                                       Domain = cookie.Domain,
                                       Expires = cookie.Expires,
                                       HttpOnly = cookie.HttpOnly,
                                       Path = cookie.Path,
                                       Secure = cookie.Secure
                                   };

            return clonedCookie;
        }

        #region "Encryption"

        public static string EncodeString(string value)
        {
            if ( string.IsNullOrEmpty( value ) )
                return null;

            var bytes = Encoding.Unicode.GetBytes( value );
            var endocedBytes = MachineKey.Protect( bytes );
            return Convert.ToBase64String( endocedBytes );
        }

        public static string DecodeString(string value)
        {
            if ( string.IsNullOrEmpty( value ) )
                return null;

            var bytes = Convert.FromBase64String( value );
            var decodedBytes = MachineKey.Unprotect( bytes );
            return decodedBytes != null ? Encoding.Unicode.GetString( decodedBytes ) : null;
        }

        #endregion
    }
}