# Board

我可能看了个假榜（雾

这是一个基于 ASP.NET Core MVC 3.1 写的假榜，可以用于比赛的时候给外网教练们看。

默认模式为显示学校图片，请将affiliations里学校short_name变成小写字母后，放入wwwroot/images/affiliations/{lowercase_short_name}.png。

更新使用了RESTful API / Authorization Basic，需要更改用户名密码请进入Services/AuthorizationServices.cs。

在自己的服务器上运行：

```bash
dotnet run
```

推荐配合 screen -R 使用。



可以用于同步 HDOJ 的 STD Contest 榜单，详情请见 HdojFetcher 文件。



你可以用nginx来反代，这是我的配置文件。

```nginx
server {
    listen                     443 ssl;
    server_name                some.domain;
    ssl                        on;
    ssl_certificate            /some/path/to/fullchain.pem;
    ssl_certificate_key        /some/path/to/privkey.pem;
    ssl_trusted_certificate    /some/path/to/chain.pem;
    ssl_session_timeout        5m;
    ssl_protocols              TLSv1.1 TLSv1.2;
    ssl_ciphers                AESGCM:ALL:!DH:!EXPORT:!RC4:+HIGH:!MEDIUM:!LOW:!aNULL:!eNULL;
    ssl_prefer_server_ciphers  on;

    location / {
        root /some/path/to/this-project/wwwroot;
        try_files $uri @kreskel;
    }

    location @kreskel {
        proxy_pass                http://localhost:59644;
        proxy_http_version        1.1;
        proxy_set_header          Upgrade $http_upgrade;
        proxy_set_header          Connection keep-alive;
        proxy_cache_bypass        $http_upgrade;
    }
}
```

