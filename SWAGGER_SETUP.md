# Swagger/Swashbuckle Setup Guide

## Overview
This project now includes **Swagger/Swashbuckle** integration, which provides:
- 📚 Interactive API documentation
- 🧪 Built-in endpoint testing in the browser
- 📊 OpenAPI/Swagger specification file
- 💡 Auto-generated documentation from XML comments

## What Was Added

### 1. NuGet Package
- **Swashbuckle.AspNetCore** v7.0.0 - provides Swagger UI and code generation

### 2. Program.cs Configuration
- SwaggerGen middleware for generating OpenAPI documentation
- SwaggerUI middleware for the interactive browser interface
- XML documentation support for method and parameter descriptions

### 3. XML Documentation
- Enabled `GenerateDocumentationFile` in `.csproj`
- Updated all controllers with XML documentation comments

## How to Access Swagger

### 1. Run the Application
```powershell
dotnet run
```

or from Visual Studio:
- Press `F5` to start debugging
- The application will start on the default HTTPS port (usually `https://localhost:7XXX`)

### 2. Access Swagger UI
Navigate to:
```
https://localhost:7XXX/swagger
```

### 3. View OpenAPI Specification
The raw OpenAPI/Swagger JSON is available at:
```
https://localhost:7XXX/swagger/v1/swagger.json
```

## Using Swagger UI

### Testing Endpoints

1. **Browse Endpoints**: Swagger UI displays all available API endpoints organized by controller
2. **Expand Endpoint**: Click on any endpoint to see:
   - Endpoint description (from XML comments)
   - Required/optional parameters
   - Expected response format
   - Response codes and descriptions

3. **Try It Out**: 
   - Click the "Try it out" button on any endpoint
   - Fill in any required parameters
   - Click "Execute" to send the request
   - View the response, status code, and response headers

### Example: Testing the Best Stories Endpoint

1. Navigate to `/swagger`
2. Expand the **Stories** controller section
3. Click on the **GET /api/stories/best** endpoint
4. Click "Try it out"
5. Modify the `n` parameter if desired (default: 10)
6. Click "Execute"
7. View the response with the top stories

## Available Endpoints

### StoriesController
- **GET /api/stories/best**
  - Query Parameter: `n` (int, default: 10, range: 1-500)
  - Returns: Array of top n stories sorted by score
  - Response Codes: 200 (OK), 400 (Bad Request), 500 (Server Error)

### WeatherForecastController
- **GET /weatherforecast**
  - Returns: 5-day weather forecast
  - Response Code: 200 (OK)

## Documentation Features

### XML Documentation Comments
All endpoints include:
```csharp
/// <summary>
/// Brief description of what the endpoint does
/// </summary>
/// <param name="paramName">Description of the parameter</param>
/// <returns>Description of the return value</returns>
/// <response code="200">Success response description</response>
/// <response code="400">Error response description</response>
```

These comments are automatically converted to Swagger documentation.

### Response Type Attributes
Endpoints use `[ProducesResponseType]` attributes to describe possible responses:
```csharp
[ProducesResponseType(typeof(List<StoryResponse>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
```

## Development vs Production

- **Development**: Swagger UI is fully accessible at `/swagger`
- **Production**: Swagger is disabled for security reasons (only in development environment)

To change this behavior, modify `Program.cs`:
```csharp
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hacker News API v1");
		options.RoutePrefix = "swagger";
	});
}
```

## Benefits

✅ **No Manual API Documentation** - Generated automatically from code
✅ **Interactive Testing** - Test endpoints directly in the browser
✅ **Client Code Generation** - Swagger files can be used to generate client SDKs
✅ **Standardized Format** - OpenAPI specification is industry standard
✅ **Easy Integration** - Tools like Postman can import the Swagger JSON
✅ **Better Team Communication** - Developers can easily understand available endpoints

## Troubleshooting

### Swagger UI not appearing at /swagger
- Ensure application is running in Development mode
- Check that you're using HTTPS (default for ASP.NET Core)
- Verify port number matches your application's launch settings

### XML documentation not showing
- Ensure `GenerateDocumentationFile` is set to `true` in `.csproj`
- Rebuild the project
- Check that XML file is in the same directory as the DLL

### Missing endpoint documentation
- Add XML documentation comments to your controller methods
- Rebuild and refresh the Swagger UI

## Further Customization

To customize Swagger appearance and behavior, modify the `SwaggerGen` configuration in `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Your API Title",
		Version = "v1",
		Description = "Your API Description"
	});

	// Add more customizations here
});
```

## References

- [Swashbuckle Documentation](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [OpenAPI Specification](https://swagger.io/specification/)
- [ASP.NET Core API Documentation](https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-10.0)
