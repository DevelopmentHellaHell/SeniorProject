To run schema scripts, open the solution file in Visual Studio then navigate to the DAL >> DS_schemas folder in the solution explorer to find the schema scripts to setup the database.

Ensure you have the App.config in the "Solution Items" folder in the solution explorer. If you are missing the App.config file in the "Solution Items" folder, please paste the provided App.config into the root of the project (the same directory as the solution file). This file holds all of the configurations for the database.

Next open SSMS and make sure to connect to a Database Engine with the server name as "."
- If you are unable to change your server name to '.', change all server connection strings in the App.config to your own server name under the "Solution Items" folder. 
ex: For the logging connection string, the new string should read <add key="LogsConnectionString" value="Server={YOUR SERVER NAME HERE};Database=DevelopmentHell.Hubba.Logs;Encrypt=false;User Id=DevelopmentHell.Hubba.SqlUser.Logging;Password=password" />
- Do not make any change if your server name is "."

Once you are connected to the SSMS server, create a new query by clicking on the "New Query" button.

Copy the contents of one of the files in the DS_schemas folder into your query on SSMS and run the query.
- If running on sqlexpress, change 'MSSQL15.MSSQLSERVER' to 'MSSQL15.SQLEXPRESS' in both of the FILENAME fields, located in the CREATE DATABASE command.

Repeat the step above until you have ran all of the scripts in the "DS_schemas" folder.

NOTE:
- There may be errors that show up in the console on SSMS - this is perfectly normal.
- Restart the SSMS server and check contents of newly made DB to ensure the database is setup correctly.
- If needed, you may be required to enable the status of the security login. To do this, follow the steps below.


ENABLING THE STATUS OF THE SECURITY LOGIN
If you need to enable the status of the security login, on the SSMS explorer, navigate to the Security >> Logins folder.
There will be multiple security users prefixed with "DevelopmentHell.Hubba.SqlUser".
For each of these users, right click a user, then go to "Properties".
On the "General" tab, enter the string "password" into both the "Password" and "Confirm password" fields.
Navigate to the "Status" tab, and ensure the "Enabled" field under "Login" is selected.

Once all of the security users are enabled, refresh the database. The database should be ready for use.