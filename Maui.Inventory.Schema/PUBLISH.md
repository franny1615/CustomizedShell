# PUBLISH

Compile release version of code
```
dotnet publish --configuration Release
```

Navigate to ```/bin/Release/{TARGET FRAMEWORK MONIKER}/```
```
// the path above will contain a "publish" folder
// copy that over to server
scp -r publish dbuser@<ip address of server>:Desktop

// then ssh into the server
ssh username@<ip address of server>

// then transfer folder to /var/www/inventoryapp
sudo mv -v publish ../../../var/www/inventoryapp/
```

Install Nginx (if not already installed)
```
sudo apt install nginx

// check apps available
sudo ufw app list

// allow HTTPS version of Nginx
sudo ufw allow 'Nginx HTTPS'

// check status
sudo ufw status
```

Configure Nginx
```
// replace contents at /etc/nginx/nginx.conf with the following
events { }

http {
  map $http_connection $connection_upgrade {
    "~*Upgrade" $http_connection;
    default keep-alive;
  }

  server {
    listen        80;
    server_name   doesnotmatter;
    location / {
        proxy_pass         http://127.0.0.1:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection $connection_upgrade;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
  }
}

// you might also need to clear out nginx.conf
```
Validate configuration file
```
sudo nginx -t
```
Apply the changes
```
sudo nginx -s reload
```
Install dotnet runtime
```
sudo apt-get install -y dotnet-sdk-8.0
```
Run the app
```
cd var/www/inventoryapp/publish

dotnet Maui.Invetory.Api.dll
```

