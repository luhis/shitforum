To create migration
* Browse to Persistance folder
* dotnet ef migrations add MIRGRATIONNAME --startup-project ../ShitForum -v

To run migrations
* dotnet ef database update --startup-project ../ShitForum -v

Database cofiguration
The hosting user needs to have write permissions to the folder containin the database file, otherwise it will be impossible to write to the DB

Use the script to decompress the file and set read permissions
Now configure nginx to run the website
Configure the service to keep the applicaiton running
Use Let's Encrypt for certificates
Configure NGINX for larger uploads, remove the server header, and add GZIP