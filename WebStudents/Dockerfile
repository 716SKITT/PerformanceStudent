FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .

RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

RUN dotnet restore

EXPOSE 5006

CMD ["sh", "-c", "dotnet ef database update && dotnet run --project WebStudents.csproj"]
