FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. Копируем ТОЛЬКО файлы проектов (сначала!)
COPY ["TicTacToe/TicTacToe.API.csproj", "TicTacToe/"]
COPY ["TicTacToe.Application/TicTacToe.Application.csproj", "TicTacToe.Application/"]
COPY ["TicTacToe.Core/TicTacToe.Core.csproj", "TicTacToe.Core/"]
COPY ["TicTacToe.Infrastructure/TicTacToe.Infrastructure.csproj", "TicTacToe.Infrastructure/"]

# 2. Восстанавливаем зависимости основного проекта
RUN dotnet restore "TicTacToe/TicTacToe.API.csproj"

# 3. Копируем ВСЕ остальные файлы
COPY . .

# 4. Собираем основной проект
WORKDIR "/src/TicTacToe"
RUN dotnet build "TicTacToe.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TicTacToe.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicTacToe.API.dll"]