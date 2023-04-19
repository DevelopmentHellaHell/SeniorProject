using DevelopmentHell.Hubba.SqlDataAccess;

var listingsDao = new ListingsDataAccess("Server=.;Database=DevelopmentHell.Hubba.ListingProfiles;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.ListingProfile;Password=password");
var listingsResult = await listingsDao.SearchListings("woods").ConfigureAwait(false);
if (!listingsResult.IsSuccessful)
{
	Console.WriteLine(listingsResult.ErrorMessage);
	return;
}

Console.WriteLine(listingsResult.Payload!.Count);
listingsResult.Payload!.ForEach(item =>
{
	Console.WriteLine("");
	foreach (var kv in item)
	{
		Console.WriteLine($"{kv.Key} {kv.Value}");
	}
});