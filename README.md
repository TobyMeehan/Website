This repository contains the internal REST API backend for [tobymeehan.com/downloads](https://tobymeehan.com/downloads). For the frontend and other parts of the website see [tobymeehan.com](https://github.com/TobyMeehan/tobymeehan.com).

## Configuration

Example **appsettings.json** - environment variables are also supported.
```json
{
  "Data": {
    "Postgres": {
      "ConnectionString": "postgres-connection-string"
    },
    "S3": {
      "Bucket": "S3-bucket-for-download-files",
      "Credentials": {
        "AccessKey": "aws-access-key",
        "SecretKey": "aws-secret-key"
      },
      "Configuration": {
        "Region": "aws region (e.g us-east-1)",
        "ServiceUrl": "custom service url (overrides region)"
      }
    }
  }
}
```

## Run

In `src/TobyMeehan.Com`:

- Start postgres
```sh
docker compose up
```
- Apply migrations
```sh
dotnet ef database update
```
- Run app
```sh
dotnet run
```

## Test

```sh
dotnet test
```

Integration tests require docker for Postgres and LocalStack test containers. Test coverage still has some way to go.
