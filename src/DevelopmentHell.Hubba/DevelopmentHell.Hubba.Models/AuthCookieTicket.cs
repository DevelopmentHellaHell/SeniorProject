using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AuthCookieTicket
    {
        public int? SessionId { get; set; }
        public int AccountId { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime? Expiration { get; set; }
        //Encrypted self to prevent tampering
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[]? Self;
        public bool Equals(AuthCookieTicket other)
        {
            if (SessionId != other.SessionId 
                || AccountId != other.AccountId
                || LastActivity != other.LastActivity
                || Expiration != other.Expiration
            )
            {
                return false;
            }
            return true;
        }
    }
}
