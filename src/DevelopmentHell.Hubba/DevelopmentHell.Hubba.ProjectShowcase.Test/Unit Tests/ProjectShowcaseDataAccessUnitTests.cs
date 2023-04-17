using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopmentHell.Hubba.ProjectShowcase.Test.Unit_Tests
{
    internal class ProjectShowcaseDataAccessUnitTests
    {
    }
}

//Write unit tests for the ProjectShowcaseDataAccess class. This class will be used to access the database.  The unit tests should test the following:
//1.  The constructor should throw an exception if the connection string is null or empty.
//2.  The constructor should throw an exception if the connection string is not a valid connection string.
//3.  The constructor should not throw an exception if the connection string is valid.
//4.  The GetProjects method should return a list of projects.
//5.  The GetProjects method should return an empty list if there are no projects in the database.
//6.  The GetProject method should return a project with the specified ID.
//7.  The GetProject method should return null if the project does not exist.
//8.  The GetProject method should throw an exception if the ID is less than 1.
//9.  The GetProject method should throw an exception if the ID is greater than 1000.
//10.  The GetProject method should throw an exception if the ID is not an integer.
//11.  The GetProject method should throw an exception if the ID is null.
//12.  The GetProject method should throw an exception if the ID is empty.
//13.  The GetProject method should throw an exception if the ID is not a valid integer.
    