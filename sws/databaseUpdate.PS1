# Execute the below code if the database schema changed
# dotnet ef migrations add UpdateFileContentToByteArray
dotnet ef database update --connection "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=changeme"