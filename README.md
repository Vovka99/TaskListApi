# TaskListAPI
TaskListAPI is a simple .NET Core Web API service that allows users to manage a list of tasks. Supports creating, editing, deleting task lists, and sharing them between users.

## Getting Started

To run the service using Docker Compose:

```bash
docker-compose up -d --build
```

This will:

- Build the .NET application
- Start a Mongo database container
- Start the TaskListAPI service
