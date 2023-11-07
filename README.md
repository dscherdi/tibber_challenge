
# Robot Cleaning Microservice

This microservice simulates a robot moving and cleaning in an office space. It receives movement commands, calculates the path, performs a cleaning simulation, and records the number of unique places cleaned.

## Features

- REST API to accept robot movement commands.
- PostgreSQL for result persistence.
- Docker configuration for easy deployment.
- Unit tests to ensure functionality.

## Getting Started

1. Navigate to the project directory:

   ```bash
   cd TibberRobotService
   ```

3. Build the Docker containers:

   ```bash
   docker-compose build
   ```

4. Run the Docker containers:

   ```bash
   docker-compose up
   ```

### Using the API

To send commands to the robot cleaning microservice, use the following HTTP request as a template:

```http
POST http://localhost:5001/tibber-developer-test/enter-path HTTP/1.1
Host: localhost:5000
Content-Type: application/json

{
  "start": {
    "x": 10,
    "y": 22
  },
  "commands": [
    {
      "direction": "east",
      "steps": 2
    },
    {
      "direction": "north",
      "steps": 1
    }
  ]
}
```

### Testing

Run the unit tests with the following command:

```bash
dotnet test
```
