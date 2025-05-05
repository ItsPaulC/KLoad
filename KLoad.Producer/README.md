# KLoad.Producer README.md

# KLoad.Producer

## Overview

KLoad.Producer is a console application that acts as a Kafka producer, sending messages that represent Employee data. This application is part of the KLoad solution, which also includes a consumer application for processing these messages.

## Prerequisites

- .NET SDK (version 6.0 or later)
- Docker (for running Kafka locally)
- Kafka and Zookeeper services (configured via Docker Compose)

## Setup Instructions

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd KLoad
   ```

2. **Build the project**:
   ```bash
   dotnet build KLoad.Producer/KLoad.Producer.csproj
   ```

3. **Run Docker Compose**:
   To start Kafka and Zookeeper, run:
   ```bash
   docker-compose up -d
   ```

4. **Configure the application**:
   Update the `appsettings.json` file with the appropriate Kafka broker address if necessary.

5. **Run the Producer**:
   Execute the application to start producing Employee messages:
   ```bash
   dotnet run --project KLoad.Producer/KLoad.Producer.csproj
   ```

## Usage

The application will produce Employee messages to the configured Kafka topic. Ensure that the consumer application is running to process these messages.

## Protobuf Definition

The Employee message structure is defined in `Protos/employee.proto`. Ensure that both the producer and consumer applications use the same protobuf definition for compatibility.

## Troubleshooting

- If you encounter issues with Kafka connectivity, ensure that the Docker containers are running and accessible.
- Check the logs for any errors during message production.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.