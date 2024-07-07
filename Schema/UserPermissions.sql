CREATE TABLE user_permissions
(
    Id                      INT NOT NULL IDENTITY PRIMARY KEY,
    UserId                  INT NOT NULL REFERENCES app_user(Id),
    CompanyId               INT NOT NULL REFERENCES company(Id),
    InventoryPermissions    INT NOT NULL DEFAULT 0
);