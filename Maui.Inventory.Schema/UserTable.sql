CREATE TABLE app_user
(
    Id            INT           NOT NULL IDENTITY PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    AdminID       INT           FOREIGN KEY REFERENCES admin(Id),
    IsDarkModeOn  BIT			NOT NULL DEFAULT 0,
    EditInventoryPermissions INT    NOT NULL DEFAULT 0
);