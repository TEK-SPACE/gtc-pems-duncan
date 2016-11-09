using System.Text;

namespace Duncan.PEMS.Entities.Users
{
    public class User
    {
        public User()
        {
            Id = -1;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public bool IsValid
        {
            get { return Id > 0; }
        }

        public string FullName()
        {
            var sb = new StringBuilder();

            if ( IsValid )
            {
                sb.Append( FirstName );
                if ( !string.IsNullOrEmpty( MiddleName ) )
                    sb.Append( " " + MiddleName[0] ).Append( "." );
                sb.Append( " " ).Append( LastName );
            }
            else
            {
                sb.Append( "[Invalid]" );
            }
            return sb.ToString();
        }
    }
}