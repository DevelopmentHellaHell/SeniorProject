using DevelopmentHell.Hubba.Models;

namespace DevelopmentHell.Hubba.WebAPI.DTO.Notification
{
	public class UpdatePhoneDetailsDTO
	{
		public CellPhoneProviders? CellPhoneProvider { get; set; }
		public string? CellPhoneNumber { get; set; }
	}
}
