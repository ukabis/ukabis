## Installation
git clone git@github.com:Keio-project/shizuoka.git

## Configuration
```sh
cp .env.example .env
cp config/app_local.example.php config/app_local.php
```

Edit config databases info [Datasources.default]
```sh
host => 'db'
username => 'homestead'
database => 'homestead'
password => 'secret'
```

Start the services
```sh
docker-compose build
docker-compose up -d
```

Modify hosts file
```sh
127.0.0.1 fg-sdgs.loc
```

COMPOSER
```sh
cd app
composer install
```

MIGRATION
Create table
```sh
php bin/cake.php bake migration CreateUsers
```
Run migration
```sh
php bin/cake.php migrations migrate
```
Rollback migration
```sh
php bin/cake.php migrations rollback
```