CREATE TABLE company
(
    Id     INT           NOT NULL IDENTITY PRIMARY KEY, 
    Name   NVARCHAR(250) NOT NULL DEFAULT '',
    Address1      NVARCHAR(250) NOT NULL DEFAULT '',
    Address2      NVARCHAR(250) NOT NULL DEFAULT '',
    Address3      NVARCHAR(250) NOT NULL DEFAULT '',
    City          NVARCHAR(250) NOT NULL DEFAULT '',
    Country       NVARCHAR(250) NOT NULL DEFAULT '',    
    State         NVARCHAR(250) NOT NULL DEFAULT '',
    Zip           NVARCHAR(250) NOT NULL DEFAULT '',
    LicenseExpiresOn DATETIME NOT NULL 
);