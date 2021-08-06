RESTful API for input differences
.NET Core 3.1

Application exposes the following API endpoints:
GET /v1/diff --> gets all supported endpoints
GET /v1/diff/{ID} --> gets comparison result for left and right string of ID
PUT /v1/diff/{ID}/right --> updates right string value of ID with base64 encoded string value
PUT /v1/diff/{ID}/left --> updates left string value of ID	with base64 encoded string value

Solution contains:
1. Main project named DiffApp. Classes:
- Controllers\DiffController.cs --> REST API implementation
- DiffLogic.cs --> classes, implementing comparison logic
- Helpers.cs --> general helper functions and global variables
2. Test Project, named DiffUnitTests, containing Unit tests