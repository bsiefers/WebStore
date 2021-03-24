# Web Store
## A ready to go webstore for a variety of contexts. 

### How To Setup


1. First pull/download the repo.
2. Add WebStore.API/AppSettings.json
** Sample AppSetttings.json**bold

```json
{
"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=WebStore;Trusted_Connection=true;  MultipleActiveResultSets=true",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "JWT": {
    "Key": "JWT KEY",
    "Issuer": "WebStore.com"
  },
  "Stripe": {
    "PubliceKey": "STRIPE PUBLIC KEY",
    "SecretKey": "STRIPE PRIVATE KEY"
  }
}
```

3. Create/Connect MySQL database.
> dotnet ef --startup-project ../WebStore.API migrations add Data

> dotnet ef --startup-project ../WebStore.API database update``
4. Start the program.