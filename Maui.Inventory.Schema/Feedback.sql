CREATE TABLE feedback
(
    Id           INT NOT NULL IDENTITY PRIMARY KEY,
    AdminId      INT NOT NULL,
    UserId       INT NOT NULL, 
    WasAdmin     BIT NOT NULL DEFAULT 0,
    Subject      NVARCHAR(1024) NOT NULL DEFAULT '',
    Body         NVARCHAR(1024) NOT NULL DEFAULT '',
    CreatedOn    DATETIME NOT NULL DEFAULT GETDATE()
);