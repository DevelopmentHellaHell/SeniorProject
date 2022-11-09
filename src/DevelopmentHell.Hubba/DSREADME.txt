To export db schema scripts: 
- open ssms
- right click database (not server, not the folder, should just be black cylinder icon with nothing else on it)
- Tasks->Generate Scripts
- Next
- Script entire database and all database objects -> Next
- Save as script file/Save to clipboard/Open in new query window -> Advanced -> Types of data to script = Schema only
- Save wherever you want
- OK -> Next -> Next -> Finish
- go to visual studio -> solution explorer -> DAL -> right click DS_schemas
- Add -> New Item -> search "sql" -> add new sql file with name appropriate to database (i.e. "DevelopmentHell.Hubba.Logging.sql")
- paste generated script into there
- add the following to the top of the script (disconnects all users and drops the db):

USE [master]
GO

alter database [<insert db name>] set single_user with rollback immediate
DROP DATABASE [<insert db name>]
GO

to run schema script:
- open ssms
- make sure to delete the database you want to run the sql file for (not just table)
- open sql file in ssms, run it
- close and reopen ssms and check contents of newly made db