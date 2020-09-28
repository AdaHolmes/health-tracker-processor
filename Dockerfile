FROM mcr.microsoft.com/dotnet/core/sdk:3.1

WORKDIR /app

COPY HealthTrackerProcessor/ ./HealthTrackerProcessor
COPY HealthTrackerProcessorCore/ ./HealthTrackerProcessorCore
COPY HealthTrackerProcessor.sln .

RUN dotnet build HealthTrackerProcessor/HealthTrackerProcessor.csproj -c release -f netcoreapp3.1 -o /out
RUN dotnet build HealthTrackerProcessorCore/HealthTrackerProcessorCore.csproj -c release -f netcoreapp3.1 -o /out

WORKDIR /out

ENTRYPOINT ["dotnet", "HealthTrackerProcessor.dll"]

ENV ASPNETCORE_URLS=http://0.0.0.0:80

EXPOSE 80