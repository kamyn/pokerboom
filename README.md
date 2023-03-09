## Средства для запуска приложения

- Docker
- MySQL (образ в docker)
- .Net Core 3.1 SDK
- Visual Studio 2019 и выше

## Развёртывание

- Для запуска приложения нужно создать контейнер
базы данных MySQL в docker. Для этого нужно выполнить команду:
```
docker run --env=MYSQL_ROOT_HOST=% \
		--env=MYSQL_ROOT_PASSWORD=1234 \
		--env=PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin \
		--env=MYSQL_UNIX_PORT=/var/lib/mysql/mysql.sock -p 3307:3306 \
		--restart=no --runtime=runc -d mysql/mysql-server:latest
```
В этой команде можно указать имя пользователя и пароль для базы данных, а также локальный порт (в данном случае к базе можно подключиться локально через 3007 порт, например с помощью программы MySQL Workbench).
Необходимо указать ip-адрес контейнера (узнать его можно с помощью команды `docker inspect {название контейнера}`), название и пароль базы данных в файле `appsettings.json`

- Добавить файл конфигурации для авторизации через сторонний сервис (VK) `AuthOptions.cs` в проекте `PokerBoom.Server` следующего содержания:
```
public static class AuthOptions
    {
        public const string ISSUER = "server";
        public const string AUDIENCE = "pokerboom";
        public const int EXPIRY_IN_DAY = 1;
        public const string SECURITY_KEY = "***";

        public const string VK_CLIENT_ID = "***";
        public const string VK_CLIENT_SECRET = "***";
        public const string VK_CALLBACK_PATH = "/signin-vk-token";
        public const string VK_AUTH_END_POINT = "https://oauth.vk.com/authorize";
        public const string VK_TOKEN_END_POINT = "https://oauth.vk.com/access_token";
    }
```

Изменить значение порта и клиентского идентификатора стороннего сервиса авторизации в файле `Constants.cs` в проекте `PokerBoom.Shared`:
```
public static class Constants
    {
        public const string VK_AUTH_URL = "https://oauth.vk.com/authorize";
        public const string VK_CLIENT_ID = "***";
        public const string VK_REDIRECT_URI = "https://localhost:{port}/vklogin";
    }
``` 

- Создать docker-контейнер проекта из docker-файла в Visual Studio.

## Тестовые данные

По умолчанию создаются два пользователя `root` и `user` с паролями `123` и правами администратора и пользователя соответственно. Данные пользователей по умолчанию можно изменить в файле `DbInit.cs`. 

Также по умолчанию создается игровой стол `стол #1` в файле контекста базы данных `AppDbContext.cs`.

Изменения структуры базы данных задаются в файлах миграции, которые находятся в папке `Migrations`, для создания которой нужно выполнить команду `add-migration {название миграции}`, а для создания нужных таблиц в базе данных нужно применить миграцию с помощью команды `update-database` в терминале. По умолчанию все миграции будут применены автоматически при первом запуске проекта.

## UI

Для работы с приложением необходимо авторизация, это возможно сделать через пароль или сторонний сервис аутентификации (VK). После авторизации на главной странице будут доступны игровые столы, за которыми можно играть с другими людьми.