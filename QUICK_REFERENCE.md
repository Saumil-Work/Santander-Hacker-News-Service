# ⚡ SWAGGER QUICK REFERENCE CARD

## 🚀 Start Here

```
1. Run Application:
   dotnet run

2. Open Swagger UI:
   https://localhost:7XXX/swagger

3. Test Endpoint:
   - Click endpoint name
   - Click "Try it out"
   - Click "Execute"
   - View response
```

---

## 📍 Key URLs

| Purpose | URL |
|---------|-----|
| **Interactive Testing** | `/swagger` |
| **API Specification** | `/swagger/v1/swagger.json` |
| **Best Stories API** | `/api/stories/best?n=10` |
| **Weather API** | `/weatherforecast` |

---

## 🔧 API Endpoints

### Stories API
```
GET /api/stories/best

Parameters:
  n (query) - Number of stories
	Type: integer
	Default: 10
	Range: 1-500

Response: List of stories
```

### Weather API
```
GET /weatherforecast

Parameters: None

Response: 5-day forecast
```

---

## 📊 HTTP Status Codes

| Code | Meaning |
|------|---------|
| **200** | ✅ Success |
| **400** | ❌ Bad Request (invalid parameter) |
| **404** | ❌ Not Found |
| **408** | ❌ Timeout |
| **500** | ❌ Server Error |

---

## 🧪 Test Examples

### Example 1: Get 10 Stories (Default)
```
Click: GET /api/stories/best
Try it out → Execute
Result: Top 10 stories
```

### Example 2: Get 25 Stories
```
Click: GET /api/stories/best
Try it out
Set n=25
Execute
Result: Top 25 stories
```

### Example 3: Error Test (Invalid n)
```
Click: GET /api/stories/best
Try it out
Set n=0
Execute
Result: 400 Bad Request error
```

---

## 🎯 What You Can Do

✅ **Test** endpoints in browser (no tools needed)
✅ **See** response data formatted nicely
✅ **View** HTTP status codes
✅ **Check** response headers
✅ **Copy** request URLs or cURL commands
✅ **Understand** API documentation

---

## 💻 cURL Commands

### Get 10 Stories
```bash
curl -X GET "https://localhost:7XXX/api/stories/best?n=10" \
  -H "accept: application/json"
```

### Get 25 Stories
```bash
curl -X GET "https://localhost:7XXX/api/stories/best?n=25" \
  -H "accept: application/json"
```

### Get Weather
```bash
curl -X GET "https://localhost:7XXX/weatherforecast" \
  -H "accept: application/json"
```

---

## 🔐 Important Notes

- ✅ Swagger UI only available in **Development** environment
- ✅ API endpoints work in **all** environments
- ✅ No code changes required after setup
- ✅ Fully backward compatible with existing code

---

## 🆘 Quick Fixes

| Problem | Solution |
|---------|----------|
| Swagger not loading | Use HTTPS, check port |
| Endpoints missing | Restart app |
| No documentation | Rebuild: `dotnet build` |
| 404 error | Verify app is running |

---

## 📚 Documentation Files

| File | Purpose | Read Time |
|------|---------|-----------|
| SUMMARY.md | Quick overview | 5 min |
| SWAGGER_README.md | Getting started | 10 min |
| SWAGGER_TESTING_GUIDE.md | How to test | 15 min |
| SWAGGER_SETUP.md | Technical guide | 20 min |

See **DOCUMENTATION_INDEX.md** for complete index.

---

## 🎯 Common Tasks

### "I want to test the API"
→ Open `/swagger`
→ Click endpoint
→ Click "Try it out"
→ Click "Execute"

### "I want to see documentation"
→ Open `/swagger`
→ Expand any endpoint
→ Read description and parameters

### "I want the raw API spec"
→ Go to `/swagger/v1/swagger.json`
→ Download or view JSON

### "I want to share with team"
→ Share URL: `https://localhost:7XXX/swagger`
→ They can test endpoints too

---

## ⚙️ Command Reference

```powershell
# Run the application
dotnet run

# Rebuild project
dotnet build

# Clean build
dotnet clean && dotnet build

# Restore packages
dotnet restore

# Run with release build
dotnet run --configuration Release
```

---

## 📱 Browser Tips

- **Chrome/Edge**: Works great
- **Firefox**: Fully supported
- **Safari**: Works fine
- **Mobile**: Swagger UI responsive

---

## 🔍 Finding Information

| Need | Look Here |
|------|-----------|
| Quick start | SUMMARY.md |
| How to use | SWAGGER_TESTING_GUIDE.md |
| Setup details | SWAGGER_SETUP.md |
| What changed | BEFORE_AND_AFTER.md |
| Troubleshooting | SWAGGER_SETUP.md → Troubleshooting |
| Verification | IMPLEMENTATION_CHECKLIST.md |

---

## ✅ Setup Verification

Did everything work?

- [ ] App builds: `dotnet build`
- [ ] App runs: `dotnet run`
- [ ] Open `/swagger` in browser
- [ ] See Swagger UI interface
- [ ] Click endpoint → "Try it out"
- [ ] See response when execute

If all checked, you're done! ✅

---

## 🎓 Learning Resources

- Swagger Docs: https://swagger.io/
- OpenAPI Spec: https://swagger.io/specification/
- Swashbuckle GitHub: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- ASP.NET Docs: https://docs.microsoft.com/en-us/aspnet/

---

## 💡 Pro Tips

1. **Use Swagger UI for testing** - Beats manual Postman setup
2. **Check response codes** - Understand success/error
3. **Try different parameters** - See how API behaves
4. **Copy cURL commands** - Use in scripts/documentation
5. **Export the spec** - Use with code generators

---

## 📞 Getting Help

1. Check **DOCUMENTATION_INDEX.md** for all guides
2. Review **SWAGGER_SETUP.md** troubleshooting section
3. Verify setup with **IMPLEMENTATION_CHECKLIST.md**
4. Check application logs for errors

---

## 🎉 You're All Set!

Everything is configured and ready to use.

**Next Step**: Run `dotnet run` and visit `/swagger`

**Happy Testing!** 🚀

---

## Print This Card!

Save this as a PDF or print it out:
- Post near your desk
- Share with team members
- Reference while testing
- Quick lookup guide

---

**Framework**: .NET 10.0
**Swagger**: Swashbuckle 7.0.0
**Status**: ✅ Ready to Use
**Date**: 2024

---

For detailed documentation, see: **DOCUMENTATION_INDEX.md**
