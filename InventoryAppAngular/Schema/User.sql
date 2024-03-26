CREATE TABLE user
(
    Id            INT           NOT NULL PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    IsDarkModeOn  BIT			NOT NULL DEFAULT 0,
    Localization  NVARCHAR(250) NOT NULL DEFAULT 'en',
    InventoryPermissions INT    NOT NULL DEFAULT 0,
    Email         NVARCHAR(250) NOT NULL DEFAULT '',
    PhoneNumber   NVARCHAR(250) NOT NULL DEFAULT '',
    IsOwner       BIT           NOT NULL DEFAULT 0,
    CompanyID     INT           NOT NULL,
    CONSTRAINT FK_CompanyID FOREIGN KEY (CompanyID) REFERENCES company(Id)
);