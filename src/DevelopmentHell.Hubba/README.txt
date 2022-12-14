To run schema scripts, open the solution file in Visual studio then go under DAL >> DS_schemas to find the schema scripts to setup the database.

Ensure you have the App.config in the "Solution Items"" folder. If you are missing the App.config file in the "Solution Items" folder, please paste the given App.config into the root of the project (the same directory as the solution file). This file holds all of the configurations for the database.

Next open SSMS and make sure to connect to a Database Engine with the server name as "."
NOTE:
- if running on sqlexpress, change 'MSSQL15.MSSQLSERVER' to 'MSSQL15.SQLEXPRESS' run it
- else if running on dev sqlserver, change 'MSSQL15.SQLEXPRESS' to 'MSSQL15.MSSQLSERVER' run it
- ensure to change the server connection string in the App.config under the "Solution Items" folder if your server name is different.
- if already is the one you need, don't worry

Once you are connected to the SSMS server, create a New Query and run all of the scripts individually in the DS_schemas folder.
NOTE:
- There may be errors that show up in the console on SSMS - this is perfectly normal.
- Close and reopen SSMS and check contents of newly made DB to ensure the database is setup correctly.
- If needed, you may need to enable the status of the security login. To do this, follow the steps below.

If you need to senable the status of the security login, on the SSMS explorer, go under Security >> Logins.
There will be multiple security users prefixed with "DevelopmentHell.Hubba.SqlUser".
For each of these users, right click a user, then go to "Properties", and go to the "Status" page on the list of pages.
Once you have located this page, ensure the "Enabled" field under "Login" is selected.
Once all of the security users are enabled, the database should be ready for use.