# Microservices Demo Application

A complete microservices application demonstrating modern containerized architecture with .NET 8, Redis, PostgreSQL, and Docker. This project simulates a real-world data processing pipeline with form submission, background processing, and result display.

## 🏗️ Architecture Overview

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│  Form App   │───▶│    Redis    │───▶│   Worker    │───▶│ PostgreSQL  │───▶│ Result App  │
│   (8081)    │    │   (Queue)   │    │  Service    │    │ (Database)  │    │   (8082)    │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
```

### Components

1. **Form App** - .NET 8 MVC web application for data submission
2. **Redis** - Message queue for temporary data storage
3. **Worker Service** - .NET 8 console app for background processing
4. **PostgreSQL** - Persistent database storage
5. **Result App** - .NET 8 MVC web application for data display with destruction simulator

## 🚀 Features

- **Asynchronous Processing** - Form submissions are queued and processed in background
- **Containerized Architecture** - Each service runs in its own Docker container
- **Data Pipeline** - Complete flow from form → Redis → Worker → PostgreSQL → Display
- **Production Simulation** - Built-in failure simulation for testing and learning
- **Responsive UI** - Modern Bootstrap-based interfaces
- **Real-time Monitoring** - Comprehensive logging and error handling

## 📋 Prerequisites

- Docker Desktop installed and running
- .NET 8 SDK (for local development)
- Git (for cloning the repository)
- Web browser for testing

## 🛠️ Project Structure

```
k8s-form-application/
├── FormApp/                          # Form submission application
│   ├── Controllers/
│   │   └── HomeController.cs         # Handles form submissions
│   ├── Models/
│   │   └── PersonModel.cs            # Data model for person information
│   ├── Services/
│   │   ├── IRedisService.cs          # Redis service interface
│   │   └── RedisService.cs           # Redis implementation
│   ├── Views/
│   │   └── Home/
│   │       └── Index.cshtml          # Form submission page
│   ├── Program.cs                    # Application startup
│   ├── appsettings.json             # Configuration
│   ├── FormApp.csproj               # Project file
│   └── Dockerfile                    # Container definition
├── WorkerService/                    # Background processing service
│   ├── PersonWorkerService.cs        # Main worker logic
│   ├── Program.cs                    # Worker startup
│   ├── appsettings.json             # Configuration
│   ├── WorkerService.csproj         # Project file
│   └── Dockerfile                    # Container definition
├── ResultApp/                        # Results display application
│   ├── Controllers/
│   │   └── HomeController.cs         # Results display + destruction simulator
│   ├── Models/
│   │   └── PersonViewModel.cs        # View model for display
│   ├── Services/
│   │   ├── IPersonService.cs         # Data service interface
│   │   └── PersonService.cs          # PostgreSQL data access
│   ├── Views/
│   │   └── Home/
│   │       └── Index.cshtml          # Results display page
│   ├── Program.cs                    # Application startup
│   ├── appsettings.json             # Configuration
│   ├── ResultApp.csproj             # Project file
│   └── Dockerfile                    # Container definition
└── README.md                         # This file
```

## 🐳 Installation & Setup

### Step 1: Create Docker Network

```
docker network create form-network
```

### Step 2: Start Infrastructure Services

**Start Redis:**
```
docker run -d --name redis --network form-network -p 6379:6379 redis:7-alpine
```

**Start PostgreSQL:**
```
docker run -d --name postgres --network form-network -e POSTGRES_DB=PersonsDB -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin -p 5432:5432 postgres:15
```

### Step 3: Build and Run Applications

**Build and run Form App:**
```
cd FormApp
docker build -t form-app .
docker run -d --name form-app --network form-network -p 8081:8081 form-app
```

**Build and run Worker Service:**
```
cd ../WorkerService
docker build -t worker-service .
docker run -d --name worker-service --network form-network worker-service
```

**Build and run Result App:**
```
cd ../ResultApp
docker build -t result-app .
docker run -d --name result-app --network form-network -p 8081:80 result-app
```

### Step 4: Verify Installation

Check all containers are running:
```
docker ps
```

You should see 5 containers: redis, postgres, form-app, worker-service, result-app

## 🎯 Usage

### Basic Workflow

1. **Submit Data**
   - Open: `http://localhost:8081`
   - Fill out the person form (First Name, Last Name, Address)
   - Click Submit

2. **Background Processing**
   - Data is automatically queued in Redis
   - Worker service picks up and processes the data
   - Data is saved to PostgreSQL database

3. **View Results**
   - Open: `http://localhost:8082`
   - Click "Refresh" to see submitted data
   - View statistics and all submitted persons

### Testing the Data Flow

```bash
# Submit a test person via form at http://localhost:8081

# Check Redis queue (should be empty if worker processed it)
docker exec -it redis redis-cli
LLEN persons_queue
exit

# Check PostgreSQL data
docker exec -it postgres psql -U postgres -d PersonsDB
SELECT * FROM persons ORDER BY created_at DESC LIMIT 5;
\q

# Check worker logs
docker logs worker-service -f

# View results at http://localhost:8082
```

## 💥 Destruction Simulator

The Result App includes a production failure simulator for testing and learning purposes.

### Available Destruction Types

1. **Divide by Zero** - Classic arithmetic exception
2. **Null Reference** - Accessing null object properties
3. **Out of Memory** - Memory exhaustion attack
4. **Infinite Loop** - CPU consumption and unresponsiveness
5. **Stack Overflow** - Recursive function calls
6. **System Exit** - Forced application shutdown
7. **Unhandled Exception** - Generic application crash
8. **Database Failure** - Simulated database connectivity issues

### Using the Destructor

1. Go to `http://localhost:8082`
2. Click the red "Destroy Container" button
3. Choose your destruction type
4. Monitor the container crash:
   ```
   docker logs result-app -f
   docker ps -a
   ```
5. Restart the crashed container:
   ```
   docker start result-app
   # or completely rebuild:
   docker rm result-app
   docker run -d --name result-app --network form-network -p 8081:8081 result-app
   ```

## 🔍 Monitoring & Debugging

### Container Logs

```
# View logs for each service
docker logs form-app -f
docker logs worker-service -f
docker logs result-app -f
docker logs redis -f
docker logs postgres -f
```

### Database Access

```
# Connect to PostgreSQL
docker exec -it postgres psql -U postgres -d PersonsDB

# Useful queries
SELECT COUNT(*) FROM persons;
SELECT * FROM persons ORDER BY created_at DESC LIMIT 10;
DELETE FROM persons; -- Clear all data
```

### Redis Access

```
# Connect to Redis
docker exec -it redis redis-cli

# Useful commands
LLEN persons_queue          # Check queue length
LRANGE persons_queue 0 -1   # View all queued items
FLUSHALL                    # Clear all Redis data
```

### Resource Monitoring

```
# Monitor container resource usage
docker stats

# Monitor specific container
docker stats result-app

# Check container health
docker inspect result-app
```

## 🔧 Configuration

### Environment Variables

Each application can be configured through environment variables:

**Form App:**
- `ConnectionStrings__Redis`: Redis connection string (default: `redis:6379`)

**Worker Service:**
- `ConnectionStrings__Redis`: Redis connection string (default: `redis:6379`)
- `ConnectionStrings__PostgreSQL`: PostgreSQL connection string

**Result App:**
- `ConnectionStrings__PostgreSQL`: PostgreSQL connection string

### Custom Configuration

Create custom appsettings.json or pass environment variables:

```
docker run -d --name result-app --network form-network -p 8081:80 -e ConnectionStrings__PostgreSQL="Host=postgres;Database=PersonsDB;Username=postgres;Password=yourpassword" result-app
```

## 🚨 Troubleshooting

### Common Issues

**Containers can't communicate:**
```
# Ensure all containers are on the same network
docker network ls
docker network inspect form-network
```

**Database connection issues:**
```
# Check PostgreSQL is ready
docker logs postgres
docker exec -it postgres pg_isready -U postgres
```

**Redis connection issues:**
```
# Test Redis connectivity
docker exec -it redis redis-cli ping
```

**Worker not processing data:**
```
# Check worker logs
docker logs worker-service -f
# Check Redis queue
docker exec -it redis redis-cli LLEN persons_queue
```

### Reset Everything

```
# Stop and remove all containers
docker stop form-app worker-service result-app redis postgres
docker rm form-app worker-service result-app redis postgres

# Remove network
docker network rm form-network

# Clean up images (optional)
docker rmi form-app worker-service result-app
```

## 🔄 Restart Policies

For production-like behavior, add restart policies:

```
docker run -d --name result-app --network form-network --restart unless-stopped -p 8081:80 result-app
```

Restart policy options:
- `no`: Default, don't restart
- `always`: Always restart
- `unless-stopped`: Restart unless manually stopped
- `on-failure`: Restart only on failure

## 📊 Performance Testing

### Load Testing Form Submissions

```
# Install curl for testing
# Submit multiple forms quickly
for i in {1..10}; do
  curl -X POST http://localhost:8080/ \
    -H "Content-Type: application/x-www-form-urlencoded" \
    -d "FirstName=Test$i&LastName=User$i&Address=Address$i"
done
```

### Monitor Processing Performance

```
# Watch queue length change
watch 'docker exec redis redis-cli LLEN persons_queue'

# Monitor database growth
watch 'docker exec postgres psql -U postgres -d PersonsDB -c "SELECT COUNT(*) FROM persons;"'
```

## 🎓 Learning Objectives

This project demonstrates:

1. **Microservices Architecture** - Loosely coupled services
2. **Message Queuing** - Asynchronous processing with Redis
3. **Containerization** - Docker best practices
4. **Data Persistence** - PostgreSQL integration
5. **Error Handling** - Graceful failure management
6. **Production Simulation** - Chaos engineering basics
7. **Monitoring** - Logging and observability
8. **Service Communication** - Inter-service networking

## 🤝 Contributing

Feel free to extend this project:

1. Add authentication and authorization
2. Implement API endpoints (Web API)
3. Add data validation and sanitization
4. Implement caching strategies
5. Add monitoring dashboards (Grafana/Prometheus)
6. Create Kubernetes deployment manifests
7. Add automated testing
8. Implement CI/CD pipelines

## 📄 License

This project is for educational purposes. Feel free to use, modify, and distribute.

## 🔗 Additional Resources

- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Docker Documentation](https://docs.docker.com/)
- [Redis Documentation](https://redis.io/documentation)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Microservices Patterns](https://microservices.io/)

---

**Happy Coding! 🚀**

For questions or issues, check the logs first, then the troubleshooting section. Remember: breaking things is part of learning!