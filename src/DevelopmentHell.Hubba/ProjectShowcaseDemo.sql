USE [DevelopmentHell.Hubba.Users]
GO

INSERT INTO UserAccounts (Email, PasswordHash, PasswordSalt, Disabled, Role, LoginAttempts) Values ('garrett.tsumaki@student.csulb.edu','mlfhbRpyDRDeCvWSbalifJ14O+LUkEWo9b4s+Ib553/wqFBre7viCwBbnHpTevcmc+l6BYMBz9zHk1WZuu0gew==','FT0sjUyafPQA02lcetrvaBOau2STPhHck3fsO7bAAZ4vofI9Olg2sJVef99rbOv8','false','VerifiedUser', 0)
INSERT INTO UserAccounts (Email, PasswordHash, PasswordSalt, Disabled, Role, LoginAttempts) Values ('tsumakig@gmail.com','mlfhbRpyDRDeCvWSbalifJ14O+LUkEWo9b4s+Ib553/wqFBre7viCwBbnHpTevcmc+l6BYMBz9zHk1WZuu0gew==','FT0sjUyafPQA02lcetrvaBOau2STPhHck3fsO7bAAZ4vofI9Olg2sJVef99rbOv8','false','AdminUser', 0)
INSERT INTO UserAccounts (Email, PasswordHash, PasswordSalt, Disabled, Role, LoginAttempts) Values ('noreply.hubba@gmail.com','mlfhbRpyDRDeCvWSbalifJ14O+LUkEWo9b4s+Ib553/wqFBre7viCwBbnHpTevcmc+l6BYMBz9zHk1WZuu0gew==','FT0sjUyafPQA02lcetrvaBOau2STPhHck3fsO7bAAZ4vofI9Olg2sJVef99rbOv8','false','VerifiedUser', 0)

GO

Use [master]
--Insert showcases owned by csulb account
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase1', Id, null, 'Title1', 'Description1', 'true', '652', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase2', Id, null, 'Title2', 'Description1', 'false', '3', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase3', Id, null, 'Title3', 'Description1', 'true', '8345', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase4', Id, null, 'Title4', 'Description1', 'false', '723', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase5', Id, null, 'Title5', 'Description1', 'true', '45', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase6', Id, null, 'Title6', 'Description1', 'false', '94', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase7', Id, null, 'Title7', 'Description1', 'true', '12', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase8', Id, null, 'Title8', 'Description1', 'false', '734', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase9', Id, null, 'Title9', 'Description1', 'true', '32', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
--insert showcases owned by gmail account
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase10', Id, null, 'Title10', 'Description10', 'false', '32', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'tsumakig@gmail.com'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase11', Id, null, 'Title11', 'Description11', 'true', '32', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'tsumakig@gmail.com'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.Showcases (Id, ShowcaseUserId, ListingId, Title, Description, IsPublished, Rating, EditTimestamp)
	SELECT 'Showcase12', Id, null, 'Title12', 'Description12', 'false', '32', '2023-04-24 11:00:00' FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'tsumakig@gmail.com'
GO
--insert comments on csulb's Showcase1  by csulb
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
	SELECT Id, 'Showcase1', CAST('2023-04-24 11:00:01' as DATETIME), 'Comment on Showcase1 1', 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
	SELECT Id, 'Showcase1', CAST('2023-04-24 11:00:02' as DATETIME), 'Comment on Showcase1 2', 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
	SELECT Id, 'Showcase1', CAST('2023-04-24 11:00:05' as DATETIME), 'Comment on Showcase1 5', 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'garrett.tsumaki@student.csulb.edu'

INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
	SELECT Id, 'Showcase1', CAST('2023-04-24 11:00:03' as DATETIME), 'Comment on Showcase1 3', 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'tsumakig@gmail.com'
INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
	SELECT Id, 'Showcase1', CAST('2023-04-24 11:00:04' as DATETIME), 'Comment on Showcase1 4', 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = 'tsumakig@gmail.com'

DECLARE @counter int
SET @counter = 6

WHILE (@counter <= 50)
BEGIN
    DECLARE @timestamp datetime
    SET @timestamp = DATEADD(second, @counter, '2023-04-24 11:00:00')
    DECLARE @comment nvarchar(max)
    SET @comment = CONCAT('Comment on Showcase1 ', @counter)
    DECLARE @email nvarchar(100)
    SET @email = 'tsumakig@gmail.com'
    
    INSERT INTO [DevelopmentHell.Hubba.ProjectShowcases].dbo.ShowcaseComments (CommenterId, ShowcaseId, Timestamp, Text, Rating)
    SELECT Id, 'Showcase1', @timestamp, @comment, 123 FROM [DevelopmentHell.Hubba.Users].dbo.UserAccounts WHERE Email = @email
    
    SET @counter = @counter + 1
END
GO