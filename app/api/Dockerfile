FROM microsoft/aspnetcore
WORKDIR /app
COPY ./bin/Release/netcoreapp2.0/publish .
ENTRYPOINT ["dotnet", "api.dll"]