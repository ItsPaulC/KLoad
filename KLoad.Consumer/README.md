# KLoad.Consumer README.md

# KLoad.Consumer

## Overview

KLoad.Consumer is a console application that acts as a Kafka consumer, designed to receive and process messages related to Employee data. It works in conjunction with the KLoad.Producer application, which sends Employee messages to a Kafka topic.

## Prerequisites

- .NET SDK (version 6.0 or later)
- Docker (for running Kafka locally)
- Kafka and Zookeeper services (configured via Docker Compose)

## Setup Instructions

1. **Clone the Repository**
   Clone the repository containing the KLoad solution to your local machine.

2. **Build the Project**
   Navigate to the `KLoad.Consumer` directory and build the project using the following command:
   ```
   dotnet build
   ```

3. **Run Docker Compose**
   In the root directory of the KLoad solution, run the following command to start Kafka and Zookeeper:
   ```
   docker-compose up
   ```

4. **Configure the Application**
   Update the `appsettings.json` file in the `KLoad.Consumer` project to specify the Kafka broker address and the topic you want to subscribe to.

5. **Run the Consumer**
   After the Kafka services are running, execute the consumer application with the following command:
   ```
   dotnet run
   ```

## Usage

Once the consumer is running, it will listen for incoming Employee messages from the specified Kafka topic. The application will process and display the received messages in the console.

## Notes

- Ensure that the `employee.proto` file in the `Protos` directory matches the one used in the KLoad.Producer project to maintain compatibility.
- For further customization, refer to the `appsettings.json` file for configuration options.

## Troubleshooting

If you encounter any issues, check the following:

- Ensure that Docker is running and the Kafka services are up.
- Verify the Kafka broker address in the `appsettings.json` file.
- Check the console output for any error messages that may indicate what went wrong.

## License

This project is licensed under the MIT License. See the LICENSE file for more details.