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
# update default
cd /etc/nginx
sudo nano nginx.conf

# should look like 
user www-data;
worker_processes auto;
pid /run/nginx.pid;
error_log /var/log/nginx/error.log;

events {
	worker_connections 768;
}

http {
  limit_req_zone $binary_remote_addr zone=one:10m rate=5r/s;

  server {
    listen 80;
    listen [::]:80 default_server;
    return 301 https://$host$request_uri;
  }

  upstream inventoryApp {
    server 127.0.0.1:5000;
  }

  map $http_connection $connection_upgrade {
    "~*Upgrade" $http_connection;
    default keep-alive;
  }

  server {
    listen        443 ssl http2 default_server;
    listen        [::]:443 ssl http2 default_server;
    ssl_certificate /etc/ssl/certs/nginx-selfsigned.crt;
    ssl_certificate_key /etc/ssl/private/nginx-selfsigned.key;
    ssl_protocols SSLv3 TLSv1 TLSv1.1 TLSv1.2;
    ssl_prefer_server_ciphers off;
    ssl_ciphers ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384;
    ssl_ecdh_curve secp384r1;
    ssl_session_cache shared:SSL:10m;
    ssl_session_tickets off;
    ssl_stapling_verify off;
    ssl_session_cache shared:SSL:10m;
    ssl_stapling              off;
    resolver 8.8.8.8 8.8.4.4 valid=300s;
    resolver_timeout 5s;
    add_header Strict-Transport-Security "max-age=63072000; includeSubdomains";
    add_header X-Frame-Options DENY;
    add_header X-Content-Type-Options nosniff;
    ssl_dhparam /etc/ssl/certs/dhparam.pem;
    location / {
        proxy_pass         http://inventoryApp;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection $connection_upgrade;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
	      limit_req  zone=one burst=10 nodelay;
    }

    location /api/websocket/serverStatus {
        proxy_pass         http://inventoryApp;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection $connection_upgrade;
        proxy_cache off;

        proxy_http_version 1.1;
        
        proxy_buffering off;
        
        proxy_read_timeout 600s;

        proxy_set_header   Host $host;
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
