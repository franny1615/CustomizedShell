CREATE TABLE user_table
(
    Id            INT           NOT NULL IDENTITY PRIMARY KEY, 
    UserName      NVARCHAR(50)  NOT NULL,
    PasswordHash  BINARY(64)    NOT NULL,
    Salt          NVARCHAR(250) NOT NULL,
    IsAdmin       BIT           NOT NULL
);

