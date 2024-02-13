CREATE TABLE locations
(
    Id           INT NOT NULL IDENTITY PRIMARY KEY,
    AdminId      INT NOT NULL FOREIGN KEY REFERENCES admin(Id),
    Description  NVARCHAR(1024) NOT NULL DEFAULT '',
    Barcode      NVARCHAR(300)  NOT NULL DEFAULT ''
)