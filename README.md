# Game Server Monitoring System

This project is a game server monitoring system that retrieves data from game servers, displays server information and game details, and provides a user-friendly interface for monitoring server performance. The system is built using a microservices architecture, with each service following the Domain-Driven Design (DDD) pattern and communicating through RabbitMQ message broker.

### Table of Contents

1. [Repositories](#repositories)
   - [Backend](#backend)
     - [GamesServersMonitor Service](#gamesserversmonitor-service)
     - [IGDB Service](#igdb-service)
   - [Frontend](#frontend)
     - [WPF App](#wpf-app)
     - [Service Agent](#service-agent)
   - [Business Entities](#business-entities)
   - [Servers Emulator](#servers-emulator)
   - [Docker](#docker)

the architecture diagram:
![Main diagram](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/ebf6725a-2a90-48dd-bbca-fd95ae4926a9)

### <a name="repositories"></a>Repositories

The project consists of the following repositories:

- clients
- GamesServersMonitor service
- IGDB service
- servers emulator

### <a name="backend"></a>Backend

The backend is built using a microservices architecture, with each service following the DDD pattern and communicating through RabbitMQ message broker.

#### <a name="gamesserversmonitor-service"></a>GamesServersMonitor Service

This service is responsible for retrieving data from the servers emulator and saving it in the database through Server-Sent Events (SSE). It is based on the DDD architecture, with layers communicating through MediatR (Mediator pattern) for loose coupling. The API layer exposes an API for the Service Agent, while the infrastructure layer handles data storage, messaging, and caching.

Key features:

- Local SQL database using Entity Framework, Unit of Work, and Repository patterns
- Redis server for caching
- MediatR, RabbitMQ, and SignalR for messaging

The service diagram:
![GamesMonitorService diagram](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/ccef5f5d-9e89-4ce6-8f45-7ec1cfa926cb)

#### <a name="igdb-service"></a>IGDB Service

This service is responsible for retrieving game data from the IGDB API and downloading images. It is also based on the DDD architecture.

### <a name="frontend"></a>Frontend

#### <a name="wpf-app"></a>WPF App

The WPF app is built using the MVVM architecture and utilizes the CommunityToolkit.Mvvm package for observable properties, commands, messaging, etc. The app displays graphs and information about the servers and the game.
| light mode                 | dark mode                 |
| :---------------------: | :---------------------: |
|![graphs](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/466b76a8-6430-4632-9ec7-75dae6bdb261) |![graphs-dark mode](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/ea0f12e3-0ed1-4436-b327-bfb6d15f3e60)|
| emulator setup                 | games details                 |
|![start emulator](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/c01ff763-1337-4265-916a-9e8327415114) | ![games](https://github.com/MosheMatof/games-servers-monitor/assets/73488759/8cd9351d-bd95-411a-9fe8-9d21a028c00c) |

#### <a name="service-agent"></a>Service Agent

The Service Agent is responsible for communicating with the services through HTTP requests, WebSocket using SignalR, and SSE. It acts as an API gateway for the system.

### <a name="business-entities"></a>Business Entities

The Business Entities are contained within a class library and are used across the system.

### <a name="servers-emulator"></a>Servers Emulator

The servers emulator generates data for the GamesServersMonitor service, both randomly and using real computer metrics.

### <a name="docker"></a>Docker

The services and the servers emulator are deployed in Docker containers for easy deployment and management.
