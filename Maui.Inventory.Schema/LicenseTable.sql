CREATE TABLE license 
(
    Id             INT           NOT NULL IDENTITY PRIMARY KEY,
    ExpirationDate DATETIME      NOT NULL,
    Description    NVARCHAR(250) NOT NULL  
);