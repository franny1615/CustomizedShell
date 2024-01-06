CREATE TABLE admin
(
    Id            INT           NOT NULL IDENTITY PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    Email         NVARCHAR(300) NOT NULL,
    EmailVerified BIT           NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    LicenseID     INT           FOREIGN KEY REFERENCES license(Id),
    IsDarkModeOn  BIT			NOT NULL DEFAULT 0 
);