CREATE TABLE app_user
(
    Id             INT           NOT NULL IDENTITY PRIMARY KEY, 
    CompanyID      INT           NOT NULL FOREIGN KEY REFERENCES company(Id),
    UserName       NVARCHAR(50)  NOT NULL,
    PasswordHash   BINARY(64)    NOT NULL,
    Salt           NVARCHAR(250) NOT NULL,
    IsDarkModeOn   BIT			NOT NULL DEFAULT 0,
    Localization   NVARCHAR(250) NOT NULL DEFAULT 'en',
    Email          NVARCHAR(250) NOT NULL DEFAULT '',
    PhoneNumber    NVARCHAR(250) NOT NULL DEFAULT '',
    IsCompanyOwner BIT           NOT NULL DEFAULT 0
);