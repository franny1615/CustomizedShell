CREATE TABLE inventory_history
(
    Id           INT NOT NULL IDENTITY PRIMARY KEY,
    AdminId      INT NOT NULL FOREIGN KEY REFERENCES admin(Id),
    InvId        INT NOT NULL FOREIGN KEY REFERENCES inventory(Id),
    Description  NVARCHAR(1024) NOT NULL DEFAULT '',
    Status       NVARCHAR(300)  NOT NULL DEFAULT '',
    Quantity     INT NOT NULL DEFAULT 0,
    QuantityType NVARCHAR(124)  NOT NULL DEFAULT '',
    Barcode      NVARCHAR(300)  NOT NULL DEFAULT '',
    Location     NVARCHAR(300)  NOT NULL DEFAULT '',
    LastEditedOn DATE NOT NULL DEFAULT GETDATE(),
    CreatedOn    DATE NOT NULL DEFAULT GETDATE()
);