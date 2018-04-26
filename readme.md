To create migration
* Browse to Persistance folder
* dotnet ef migrations add MIRGRATIONNAME --startup-project ../ShitForum -v

To run migrations
* dotnet ef database update --startup-project ../ShitForum -v
