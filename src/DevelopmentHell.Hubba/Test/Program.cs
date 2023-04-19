using DevelopmentHell.Hubba.SqlDataAccess;

var listingsDao = new ListingsDataAccess("Server=.;Database=DevelopmentHell.Hubba.ListingProfiles;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.ListingProfile;Password=password");
var listingsResult = await listingsDao.Search("woods").ConfigureAwait(false);
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

Console.WriteLine("=================");

var collaboratorsDao = new CollaboratorsDataAccess("Server=.;Database=DevelopmentHell.Hubba.CollaboratorProfiles;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.CollaboratorProfile;Password=password");
var collaboratorsResult = await collaboratorsDao.Search("woods").ConfigureAwait(false);
if (!collaboratorsResult.IsSuccessful)
{
	Console.WriteLine(collaboratorsResult.ErrorMessage);
	return;
}

Console.WriteLine(collaboratorsResult.Payload!.Count);
collaboratorsResult.Payload!.ForEach(item =>
{
	Console.WriteLine("");
	foreach (var kv in item)
	{
		Console.WriteLine($"{kv.Key} {kv.Value}");
	}
});