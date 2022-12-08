// See https://aka.ms/new-console-template for more information
using DevelopmentHell.Hubba.AuthenticationManager;
using DevelopmentHell.Hubba.Cryptography;

Console.WriteLine("Hello, World!");
var authenticationManager = new AuthenticationManager();
var email = "bryantran78@gmail.com";
var password = "12345678";
var passwordHash = DevelopmentHell.Hubba.Cryptography.Hash.HashString(password);
var hashResult = passwordHash.Payload as HashResult;
Console.WriteLine($"Outside: {email} {password}");
Console.WriteLine($"Outside_Hash: {hashResult!.Hash} Outside_Salt: {hashResult!.Salt}");
var loginResult = await authenticationManager.Login(email, password).ConfigureAwait(false);
Console.WriteLine(loginResult.IsSuccessful);
Console.WriteLine(loginResult.ErrorMessage);