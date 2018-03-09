FROM microsoft/aspnetcore-build AS builder
LABEL Author="MadDonkeySoftware"
LABEL WebSite="http://github.com/MadDonkeySoftware/corp-hq"

 # caches restore result by copying csproj file separately
WORKDIR /source
COPY *.csproj .
RUN dotnet restore

# copies the rest of your code
COPY . .
RUN dotnet publish --output /app/ --configuration Release

# Stage 2
FROM microsoft/aspnetcore
WORKDIR /app
COPY --from=builder /app .
ENTRYPOINT ["dotnet", "api.dll"]]
