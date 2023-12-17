CREATE TABLE admin
(
    Id            INT           NOT NULL IDENTITY PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    LicenseID     INT           FOREIGN KEY REFERENCES license(Id)
);