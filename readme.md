To create migration
* Browse to Persistance folder
* dotnet ef migrations add MIRGRATIONNAME --startup-project ../ShitForum -v

To run migrations
* dotnet ef database update --startup-project ../ShitForum -v

Database cofiguration
The hosting user needs to have write permissions to the folder containin the database file, otherwise it will be impossible to write to the DB