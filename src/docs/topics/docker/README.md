# Using Docker with Orchard Core source code

## Docker Compose

File example :  

```YML
version: '3.3'
services:
    web:
        build: 
            context: .
            dockerfile: Dockerfile
        ports:
            - "5009:80"
        depends_on:
            - sqlserver
            - mysql
            - postgresql
    sqlserver:
        image: "mcr.microsoft.com/mssql/server"
        environment:
            SA_PASSWORD: "pEKoU!8n123&"
            ACCEPT_EULA: "Y"
    mysql:
        image: mysql:latest
        restart: always
        environment:
            MYSQL_DATABASE: 'orchardcore_database'
            MYSQL_USER: 'orchardcore_user'
            MYSQL_PASSWORD: 'orchardcore_password'
            MYSQL_ROOT_PASSWORD: 'root_password'
        ports:
            - '3306:3306'
        expose:
            - '3306'
        volumes:
            - mysql-data:/var/lib/mysql
    postgresql:
        image: postgres:latest
        volumes:
            - postgresql-data:/var/lib/postgresql/data
        ports:
            - 5432:5432
        environment:
            POSTGRES_USER: orchardcore_user
            POSTGRES_PASSWORD: orchardcore_password
            POSTGRES_DB: orchardcore_database
volumes:
    mysql-data:
    postgresql-data:

```

## Remove intermediary containers

### When using `docker` command (will remove them automatically) : 

```cmd
docker build -t oc --rm .
docker image prune -f --filter label=stage=build-env
```

### When using `docker-compose` command : 

```cmd
docker-compose build
docker image prune -f --filter label=stage=build-env
docker-compose up
```



