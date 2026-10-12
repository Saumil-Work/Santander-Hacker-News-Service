# Santander Hacker News Service

A RESTful API that retrieves the best "n" stories from the Hacker News API, sorted by score in descending order.

## Features

- **RESTful API Endpoint**: Get top n stories with a simple query parameter
- **Performance Optimizations**:
  - In-memory caching (5-minute TTL) to reduce API calls to Hacker News
  - Concurrency limiting (max 10 concurrent requests) to avoid overwhelming the Hacker News API
  - Asynchronous operations for efficient resource utilization
- **Error Handling**: Comprehensive error handling with appropriate HTTP status codes
- **Logging**: Built-in logging for debugging and monitoring
- **Input Validation**: Validates request parameters (1-500 stories)
- **Async/Await Pattern**: Fully async implementation for better scalability

## Requirements

- .NET 10.0 SDK or later
- Visual Studio 2026 or any text editor

## Getting Started

### Clone the Repository
git clone https://github.com/Saumil-Work/Santander-Hacker-News-Service.git cd Santander-Hacker-News-Service

### Build the Project
dotnet build

### Run the Application
dotnet run

The API will start on `https://localhost:7299` (or `http://localhost:5106` depending on your configuration).

## Running Tests

### Run All Tests
dotnet test
### Run Tests with Verbose Output
dotnet test --verbosity normal

### Run Tests for Specific Project
dotnet test Santander-Hacker-News-Service.Tests/Santander-Hacker-News-Service.Tests.csproj

### Run Specific Test Class
dotnet test --filter "FullyQualifiedName~Santander_Hacker_News_Service_Tests.Services.HackerNewsServiceTests"

### Run Tests with Code Coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=lcov
## Test Coverage

The project includes comprehensive unit and integration tests:

### Unit Tests

- **HackerNewsServiceTests** (8 tests)
  - Valid count returns stories sorted by score
  - Invalid count throws ArgumentException
  - Deleted/dead stories are excluded
  - Empty best stories list handling
  - HTTP exception handling
  - Operation cancellation handling
  - Partial results handling
  - Caching functionality

- **StoriesControllerTests** (14 tests)
  - Valid requests return OK with stories
  - Default n value of 10
  - Invalid n values return BadRequest (0, negative, >500)
  - Boundary values (1, 500)
  - Operation timeout handling
  - Service exceptions handling
  - Empty results handling
  - Theory tests for valid/invalid ranges

### Integration Tests

- **StoriesIntegrationTests**
  - End-to-end API testing
  - Default parameter handling
  - Invalid parameter validation

## API Usage

### Endpoint



## API Usage

### Endpoint
GET /api/stories/best?n={count}

### Parameters

- `n` (query parameter, optional): Number of top stories to retrieve. Defaults to 10. Must be between 1 and 500.

### Example Requests

**Get top 10 stories (default)**
curl https://localhost:7132/api/stories/best

**Get top 30 stories**
curl https://localhost:7132/api/stories/best?n=30


**Using PowerShell**
Invoke-WebRequest -Uri "https://localhost:7132/api/stories/best?n=20" -Method Get

### Response Format
[
    {
        "title": "A uBlock Origin update was rejected from the Chrome Web Store",
        "uri": "https://github.com/uBlockOrigin/uBlock-issues/issues/745",
        "postedBy": "ismaildonmez",
        "time": "2019-10-12T13:43:01+00:00",
        "score": 1716,
        "commentCount": 572
    },
    {
        "title": "Another story title",
        "uri": "https://example.com",
        "postedBy": "username",
        "time": "2019-10-12T12:00:00+00:00",
        "score": 1500,
        "commentCount": 450
    }
]

### Response Codes

- `200 OK`: Successfully retrieved stories
- `400 Bad Request`: Invalid parameters (n < 1 or n > 500)
- `408 Request Timeout`: Request exceeded timeout threshold
- `500 Internal Server Error`: Server error occurred

## Architecture

### Key Components

1. **StoriesController**: Handles HTTP requests and responses
2. **IHackerNewsService**: Interface defining the service contract
3. **HackerNewsService**: Implementation with caching and rate limiting
4. **Models**:
   - `StoryResponse`: API response model
   - `HackerNewsItem`: Hacker News API model

### Design Patterns

- **Dependency Injection**: Services are injected via constructor
- **Repository Pattern**: Data fetching abstracted through service layer
- **Caching Strategy**: In-memory caching with TTL to reduce API calls
- **Concurrency Control**: Semaphore-based limiting to prevent API overload

## Performance Considerations

### Current Optimizations

1. **Caching**: Responses cached for 5 minutes to minimize API calls
2. **Concurrency Limiting**: Maximum 10 concurrent requests to Hacker News API
3. **Batch Fetching**: Story IDs fetched once, then fetched in parallel
4. **Early Exit**: Deleted/dead stories are filtered out

### Rate Limiting

The Hacker News API has no documented rate limits but can be overwhelmed by excessive requests. The service implements:
- Semaphore-based concurrency control (10 concurrent requests max)
- Connection pooling through HttpClientFactory
- Reasonable timeout (30 seconds)

## Assumptions

1. **Hacker News API Availability**: The API is assumed to be available and stable
2. **Story Data Completeness**: All fetched stories have the required fields (title, url, by, time, score, descendants)
3. **Cache Duration**: 5-minute cache TTL is appropriate for the use case
4. **Concurrency Limit**: 10 concurrent requests is a safe limit for the Hacker News API
5. **Story Count**: Maximum 500 stories can be requested to prevent excessive resource usage
6. **Timeout**: 30-second timeout per HTTP request is reasonable

## Future Enhancements

### Short Term

1. **Distributed Caching**: Replace in-memory cache with Redis for multi-instance deployments
2. **Configurable Parameters**: Make cache duration, concurrency limits, and timeouts configurable via appsettings.json
3. **API Key Support**: If Hacker News introduces API keys, support them for higher rate limits
4. **Story Filtering**: Add ability to filter by story type (story, job, poll, etc.)
5. **Custom Sorting**: Allow sorting by score, time, or comment count

### Medium Term

1. **Database Persistence**: Store story history for analytics and offline access
2. **WebSocket Support**: Real-time story updates via WebSocket connections
3. **Advanced Caching**: Implement cache strategies like LRU with configurable eviction
4. **Request Queue**: Implement request queue for high-concurrency scenarios
5. **Metrics and Monitoring**: Add Prometheus metrics and Application Insights integration

### Long Term

1. **ML-Based Ranking**: Use machine learning to improve story recommendations
2. **Multi-Source Support**: Support other news APIs (Reddit, HN-like platforms)
3. **Full-Text Search**: Implement search functionality with Elasticsearch
4. **User Preferences**: Allow users to save preferences and get personalized recommendations
5. **GraphQL Support**: Alongside REST API for more flexible querying

## Development

### Running Tests in Visual Studio

1. Open Test Explorer: __Test > Test Explorer__
2. Build the solution (tests appear in Test Explorer)
3. Click __Run All__ or right-click specific tests to run
4. View results in the Test Explorer pane

### Running Tests from Command Line
dotnet test

### Building for Production
dotnet publish -c Release -o ./publish

## Troubleshooting

### Timeout Errors

If you receive timeout errors:
1. Check internet connectivity
2. Verify Hacker News API is accessible: https://hacker-news.firebaseio.com/v0/beststories.json
3. Try with a smaller n value (e.g., n=5)

### Too Many Requests

The service limits concurrent requests to protect the Hacker News API. If you see this:
1. Wait a moment and retry
2. Reduce the n parameter
3. The cache will help subsequent requests

## License

This project is provided as-is for educational purposes.

## External APIs

- Hacker News API: https://github.com/HackerNews/API
- Best Stories Endpoint: https://hacker-news.firebaseio.com/v0/beststories.json
- Story Details Endpoint: https://hacker-news.firebaseio.com/v0/item/{id}.json

Summary
Implemented a complete RESTful API with:
✅ API Endpoint: GET /api/stories/best?n={count} returns top n stories by score (descending)
✅ Performance Optimization:
•	In-memory caching (5-minute TTL)
•	Semaphore-based concurrency limiting (max 10 concurrent requests)
•	Async/await throughout
✅ Robust Error Handling: Input validation, timeout handling, and comprehensive logging
✅ Clean Architecture: Dependency injection, service layer, proper models