using DevelopmentHell.Hubba.Models;
using System.Runtime.InteropServices;

namespace DevelopmentHell.Hubba.Cryptography.Service
{
	public class AuthTicketConversionService
	{
		public static byte[] ToBytes(AuthTicket ticket)
		{
			AuthTicket ticket_copy = ticket;
			ticket_copy.Self = null;
			//https://stackoverflow.com/questions/3278827/how-to-convert-a-structure-to-a-byte-array-in-c
			//https://stackoverflow.com/questions/27282307/c-sharp-marshaling-of-a-struct-with-an-array
			int size = Marshal.SizeOf(ticket);
			byte[] output = new byte[size];

			IntPtr ptr = IntPtr.Zero;
			try
			{
				ptr = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(ticket, ptr, true);
				Marshal.Copy(ptr, output, 0, size);
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
			return output;
		}
		public static AuthTicket FromBytes(byte[] bytes)
		{
			AuthTicket output = new AuthTicket();

			int size = Marshal.SizeOf(output);
			IntPtr ptr = IntPtr.Zero;
			try
			{
				ptr = Marshal.AllocHGlobal(size);

				Marshal.Copy(bytes, 0, ptr, size);

				output = (AuthTicket)Marshal.PtrToStructure(ptr, output.GetType());
			}
			finally
			{
				Marshal.FreeHGlobal(ptr);
			}
			return output;

		}
	}
}