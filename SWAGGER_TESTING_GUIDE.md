# Quick Reference: Swagger Testing Guide

## 🚀 Quick Start

### 1. Run the Application
```powershell
# From Visual Studio
Press F5

# From PowerShell
cd "C:\Users\saumi\source\repos\Santander-Hacker-News-Service"
dotnet run
```

### 2. Open Swagger UI
Once running, navigate to:
```
https://localhost:7XXX/swagger
```

Look at the console output to find the exact port number.

---

## 📋 Test Scenarios

### Test 1: Get Top 10 Stories (Default)
1. Open Swagger UI
2. Click on **GET /api/stories/best**
3. Click **Try it out**
4. Click **Execute**
5. Review the top 10 stories returned

### Test 2: Get Custom Number of Stories
1. Open Swagger UI
2. Click on **GET /api/stories/best**
3. Click **Try it out**
4. Change `n` parameter from `10` to `25`
5. Click **Execute**
6. Review the top 25 stories returned

### Test 3: Validate Error Handling (n too small)
1. Click **Try it out**
2. Change `n` to `0`
3. Click **Execute**
4. Observe **400 Bad Request** response with error message

### Test 4: Validate Error Handling (n too large)
1. Click **Try it out**
2. Change `n` to `600`
3. Click **Execute**
4. Observe **400 Bad Request** response with error message

### Test 5: Get Weather Forecast
1. Click on **GET /weatherforecast**
2. Click **Try it out**
3. Click **Execute**
4. Review the 5-day forecast returned

---

## 📊 Response Codes

| Code | Meaning | Example |
|------|---------|---------|
| **200** | Success | Stories retrieved successfully |
| **400** | Bad Request | Invalid parameter (n < 1 or n > 500) |
| **408** | Request Timeout | Request took too long |
| **500** | Server Error | Internal application error |

---

## 🔗 Useful URLs

| Purpose | URL |
|---------|-----|
| Swagger UI | `https://localhost:7XXX/swagger` |
| OpenAPI Spec | `https://localhost:7XXX/swagger/v1/swagger.json` |
| Stories Endpoint | `https://localhost:7XXX/api/stories/best?n=10` |
| Weather Endpoint | `https://localhost:7XXX/weatherforecast` |

---

## 💡 Tips & Tricks

### Viewing Response Details
- **Status Code**: Shows HTTP status (200, 400, 500, etc.)
- **Headers**: Click "Headers" tab to see response headers
- **Body**: See the actual JSON response data

### Copying Request Details
- Click the copy icon next to the request to copy:
  - cURL command
  - Full URL
  - Request body

### Importing to Postman
1. Get the Swagger JSON: `https://localhost:7XXX/swagger/v1/swagger.json`
2. In Postman, select **Import**
3. Paste the URL
4. Click **Continue**
5. All endpoints are now available in Postman

---

## 🛠️ Troubleshooting

### Swagger not loading?
- Check you're in Development environment
- Verify HTTPS is being used
- Check the correct port number
- Try clearing browser cache and refreshing

### Endpoints showing but no documentation?
- Rebuild the project: `dotnet build`
- Ensure `GenerateDocumentationFile` is `true` in `.csproj`
- Restart the application

### Getting 404 on endpoints?
- Verify the application is running
- Check endpoint path matches: `/api/stories/best`
- Ensure capitalization matches

---

## 📝 Example cURL Commands

### Get Top 10 Stories
```bash
curl -X GET "https://localhost:7XXX/api/stories/best?n=10" -H "accept: application/json"
```

### Get Top 25 Stories
```bash
curl -X GET "https://localhost:7XXX/api/stories/best?n=25" -H "accept: application/json"
```

### Get Weather Forecast
```bash
curl -X GET "https://localhost:7XXX/weatherforecast" -H "accept: application/json"
```

---

## 🔍 Understanding the Response

### Stories Response Format
```json
[
  {
	"title": "Story Title",
	"uri": "https://example.com",
	"postedBy": "username",
	"time": "2024-01-15T10:30:00Z",
	"score": 1250,
	"commentCount": 85
  }
]
```

### Weather Response Format
```json
[
  {
	"date": "2024-01-16",
	"temperatureC": 25,
	"temperatureF": 77,
	"summary": "Warm"
  }
]
```

---

## 📞 Support

For issues or questions:
1. Check `SWAGGER_SETUP.md` for detailed documentation
2. Review application logs for errors
3. Verify all dependencies are installed: `dotnet restore`
4. Rebuild project: `dotnet clean && dotnet build`

---

**Happy testing! 🚀**
