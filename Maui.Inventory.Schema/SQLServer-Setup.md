## Ubuntu 22.04 
#### SQL SERVER 2022 INSTALLATION

```
// Fetches key
curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg

// Adds Microsoft repo for SQL Server
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list)"

// Installs SQL Server
sudo apt-get update
sudo apt-get install -y mssql-server

// Runs the setup process, we're using (3) SQL Express
sudo /opt/mssql/bin/mssql-conf setup

// Check status to make sure server is running
systemctl status mssql-server --no-pager
```

#### OTHER COMMAND LINE TOOLS

```
// Fetch Keys
curl https://packages.microsoft.com/keys/microsoft.asc | sudo tee /etc/apt/trusted.gpg.d/microsoft.asc

// Add repo
curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list

// Install tools
sudo apt-get update
sudo apt-get install mssql-tools18 unixodbc-dev

// Add tools to path
echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc
source ~/.bashrc

// Keep tools up to date
sudo apt-get update  
sudo apt-get install mssql-tools18
```

#### DISABLE SA ACCOUNT

```
// Connect using SA
sqlcmd -S localhost -U sa -P '<YourPassword>' -No

// Create a new logon
CREATE LOGIN RENAMED_SA_ACCOUNT
WITH PASSWORD = '<some strong password>';
GO
CREATE USER RENAMED_SA_ACCOUNT FOR LOGIN RENAMED_SA_ACCOUNT;
GO

// Reassign roles to this account
ALTER SERVER ROLE sysadmin ADD MEMBER RENAMED_SA_ACCOUNT;
GO

// Disable old SA account
ALTER LOGIN SA DISABLE;
GO
```

#### CREATE DB

```
CREATE DATABASE db_name;
GO
```

#### CREATE DB LOGIN
```
CREATE LOGIN db_user WITH PASSWORD = '<some strong password>', CHECK_POLICY = OFF;
USE master;
GO

DENY VIEW ANY DATABASE TO db_user;
USE master;
GO

ALTER AUTHORIZATION ON DATABASE::db_name TO db_user;
GO
```

#### MAKE SQL SERVER ACCESIBLE ON LOCAL NETWORK
```
sudo ufw allow 1433/tcp
```

#### ADD ENVIRONMENT VARIABLE TO SYSTEM
```
echo 'export INV_DB_CS="Data Source=<ip>,1433;Initial Catalog=<db name>;User ID=<db user>;Password=<db user password>"' >> ~/.bashrc
source ~/.bashrc

// if developing on mac using vscode, add vscode to the path
// open the project folder from terminal using the `code .`
// otherwise the variables won't be read when running in debug
```
