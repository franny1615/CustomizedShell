# NGINX
```
sudo apt update
sudo apt install nginx
sudo systemctl start nginx
```

# HTTPS SELF SIGNED CERT 
```
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /etc/ssl/private/nginx-selfsigned.key -out /etc/ssl/certs/nginx-selfsigned.crt
sudo openssl dhparam -out /etc/ssl/certs/dhparam.pem 2048 
```

# setup some variables inside new file 
```
sudo vim /etc/nginx/snippets/self-signed.conf
# content 
ssl_certificate /etc/ssl/certs/nginx-selfsigned.crt;
ssl_certificate_key /etc/ssl/private/nginx-selfsigned.key;

# setup more varibles inside new file 
sudo nano /etc/nginx/snippets/ssl-params.conf
# content 
ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
ssl_prefer_server_ciphers on;
ssl_ciphers "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
ssl_ecdh_curve secp384r1;
ssl_session_cache shared:SSL:10m;
ssl_session_tickets off;
ssl_stapling on;
ssl_stapling_verify on;
resolver 8.8.8.8 8.8.4.4 valid=300s;
resolver_timeout 5s;
add_header Strict-Transport-Security "max-age=63072000; includeSubdomains";
add_header X-Frame-Options DENY;
add_header X-Content-Type-Options nosniff;
ssl_dhparam /etc/ssl/certs/dhparam.pem;

# modify nginx.conf
sudo vim /etc/nginx/nginx.conf

# should look like 
user www-data;
worker_processes auto;
pid /run/nginx.pid;
include /etc/nginx/sites-enabled/*.conf

# update default
cd /etc/nginx/sites-enabled
sudo vim default

# should look like 
http {
  server {
    listen 80;
    listen [::]:80 default_server;
    return 301 https://$host$requested_uri;
  }

  map $http_connection $connection_upgrade {
    "~*Upgrade" $http_connection;
    default keep-alive;
  }

  server {
    listen        433 ssl http2 default_server;
    listen        [::]:443 ssl http2 default_server;
    include       snippets/self-signed.conf;
    include       snippets/ssl-params.conf;
    location / {
        proxy_pass         http://127.0.0.1:5000/;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection $connection_upgrade;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }
  }
}

# simlink to sites-enabled so nginx can see it 
sudo ln -s /etc/nginx/sites-available/default /etc/nginx/sites-enabled/defaults.conf

# allow nginx to do its thing
sudo ufw allow 'Nginx Full'

# reload nginx
sudo nginx -t
sudo nginx -s reload

# create linux service to monitor server 
sudo vim /etc/systemd/system/kestrel-inventoryapp.service 

# paste the following 
[Unit]
Description=Example .NET Web API App running on Linux

[Service]
WorkingDirectory=/var/www/inventoryApp/publish
ExecStart=/var/www/inventoryApp/publish/Inventory.API 
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=dotnet-inventoryapp
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_NOLOGO=true
Environment=DB_CS=<insert database connection, remember to escape spaces with \x20>
Environment=JWT_AUD=<insert audience>
Environment=JWT_ISS=<insert issuer>
Environment=JWT_KEY=<insert private key>

[Install]
WantedBy=multi-user.target

# start the service 
sudo systemctl enable kestrel-inventoryapp.service

# check start/status checks 
sudo systemctl start kestrel-inventoryapp.service 
sudo systemctl status kestrel-inventoryapp.service 
```

# view logs 
```
sudo journalctl -fu kestrel-inventoryapp.service 
```

# SQL SETUP
```
curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | sudo gpg --dearmor -o /usr/share/keyrings/microsoft-prod.gpg
curl https://packages.microsoft.com/keys/microsoft.asc | sudo tee /etc/apt/trusted.gpg.d/microsoft.asc
curl -fsSL https://packages.microsoft.com/config/ubuntu/22.04/mssql-server-2022.list | sudo tee /etc/apt/sources.list.d/mssql-server-2022.list

sudo apt-get update
sudo apt-get install -y mssql-server

sudo /opt/mssql/bin/mssql-conf setup
systemctl status mssql-server --no-pager # verify that its running
```

# tools
```
curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list | sudo tee /etc/apt/sources.list.d/mssql-release.list

sudo apt-get update
sudo apt-get install mssql-tools18 unixodbc-dev

echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bash_profile
```