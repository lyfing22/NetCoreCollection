dotnet build
rd /s /q publish
dotnet publish -o ./publish
"C:\Program Files (x86)\7-Zip\7z.exe" a publish.zip publish/**