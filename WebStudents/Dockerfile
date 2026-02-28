FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY WebStudents.csproj ./
RUN dotnet restore WebStudents.csproj

COPY . .
RUN dotnet publish WebStudents.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5006

ENTRYPOINT ["dotnet", "WebStudents.dll"]
