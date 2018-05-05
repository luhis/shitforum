
del publish.7z
dotnet publish -o ./publish
"C:\Program Files\7-Zip\7z.exe" a -t7z ./publish.7z .\publish\*
rmdir /S /Q .\publish
