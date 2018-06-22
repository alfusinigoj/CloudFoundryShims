# Cloud Foundry Shims
Shim library to make porting existing ASP.NET 4.x apps to Cloud Foundry easier.

## Features
This library provides a few things out of the box to make your app run better inside Cloud Foundry without needing
to make any code changes.

- Default unhandled exception error logger, so any uncaught exceptions are logged to Cloud Foundry's logs.
- Adds any bound SQL Server services to the web.config's connection strings section.
- Override appSetting values via environment variables.
- Configure the custom error mode using the `CUSTOM_ERRORS_MODE` environment variable.

## How to Use
Reference PivotalServices.CloudFoundryShims from your ASP.NET web project before `cf push`ing your app.
No other code changes are required.

### SQL Connection Strings
This library internally uses the [Steeltoe Microsoft SQL Server connector](http://steeltoe.io/docs/steeltoe-connectors/#3-0-microsoft-sql-server)
to build connection strings from any bound SQL Server services. This allows you to bind a SQL Server service using
the MS SQL Server Service Broker or using a user provided service without needing to change how your app gets
a SQL connection string. Just make sure your bound service name matches what your app expects in the connectionStrings
section.

For example if my app had a web.config connectionString section like so, which works for local development:
```
<connectionStrings>
  <add name="SchoolContext" connectionString="Data Source=.\SQLEXPRESS;Initial Catalog=ContosoUniversity;Integrated Security=true" />
</connectionStrings>
```

I could override the connection string in Cloud Foundry using a user provided service like so:
```
cf cups SchoolContext -p '{"pw": "secret","uid": "schooluser","uri": "jdbc:sqlserver://10.0.0.100:1433;databaseName=ContosoUniversity"}'
```

### AppSettings
Your application likely already uses app setting values to configure various settings and features flags. Instead
of rewriting your existing system to use Config Server or relying upon your CD system, you can override these
settings using environment variables.

For example you may have a couple of feature flags configured in your web config appSettings section like do:
```
<appSettings>
  <add key="feature_flag_books_module" value="true" />
  <add key="feature_flag_grades_module" value="true" />
  <add key="max_upload_size_in_mb" value="100" />
</appSettings>
```

You can override these on an per-app basis in Cloud Foundry using environment variables set using the CF CLI,
Pivotal Apps Manager, or the application manifest file. Here's an example turning the feature_flag_books_module
off using the CF CLI:

```
cf set-env ContosoUniversity feature_flag_books_module false
```

### Custom Error Mode
Perhaps your application is failing to start _really_ early in the ASP.NET pipeline, for example in Application Start.
If you want to temporarily see the ASP.NET Yellow Screen of Death page from your workstation browser you
could enable this without re-pushing the application.

```
cf set-env ContosoUniversity CUSTOM_ERRORS_MODE Off
cf restart ContosoUniversity
```
