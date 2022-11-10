using DevelopmentHell.Hubba.Models;
namespace DevelopmentHell.Hubba.Registration.Abstractions
{
    public interface IAccountManager
    {
        public Result Register();
        public Result SignIn();
        public Result SignOut();
        public Result Delete();
    }
}