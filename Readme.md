Para criação do banco de dados com docker:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Str0ngP455W0RD" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-CU14-ubuntu-20.04

