CREATE TABLE user
(
    Id            INT           NOT NULL IDENTITY PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    IsDarkModeOn  BIT			NOT NULL DEFAULT 0,
    Localization  NVARCHAR(250) NOT NULL DEFAULT 'en'
);