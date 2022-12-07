// See https://aka.ms/new-console-template for more information
using DevelopmentHell.Hubba.OneTimePassword;

Console.WriteLine("Hello, World!");
string expectedDatabaseName = "DevelopmentHell.Hubba.Users";
string connectionString = String.Format(@"Server=.;Database={0};Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.User;Password=password", expectedDatabaseName);
var otpManager = new OTPManager(connectionString);
var otp = otpManager.NewOTP(1).Result.Payload;
Console.Write(otp);