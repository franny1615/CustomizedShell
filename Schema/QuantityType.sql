CREATE TABLE quantity_type
(
    Id           INT NOT NULL IDENTITY PRIMARY KEY,
    CompanyId    INT NOT NULL FOREIGN KEY REFERENCES company(Id),
    Description  NVARCHAR(1024) NOT NULL DEFAULT ''
)