dotnet clean Inventory.API
dotnet restore Inventory.API 
dotnet publish Inventory.API -c release -r linux-x64 --self-contained

# the following will only work when rsa passwordless auth is setup to target machine
# inventory_user must also have rights to /var/www/inventoryApp folder
DIR="$( cd "$( dirname "$0" )" && pwd )"
scp -r $DIR/Inventory.API/bin/Release/net8.0/linux-x64/publish inventory_user@192.168.1.141:/var/www/inventoryApp