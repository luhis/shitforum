# shitforum

## Background

This project was written as a kata, recreating an image board using a variety of technologies I wanted to learn more about.

It uses:
- .Net Core 2.1
- .Net Standard 2.0
- Libman for client side libararies
- Linux for hosting, or Windows if you choose
- Razor pages
- Ffmpeg

## Deployment

Database cofiguration
The hosting user needs to have write permissions to the folder containin the database file, otherwise it will be impossible to write to the DB, but reads are still possible.  This is because a lock files is added to the folder.

Use the create-publish.bat script to create the deployment package
Transfer the package to the target machine
Use the script to decompress the file and set read permissions
Now configure nginx to run the website
Configure the service to keep the applicaiton running
Use Let's Encrypt for certificates
Configure NGINX for larger uploads, remove the server header, and add GZIP
