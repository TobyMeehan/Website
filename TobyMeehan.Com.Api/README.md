## Responses

### Success

```json
{

}
```

### Error

```json
{
	"error": "Error Title",
	"message": "Error Message"
}
```

## User

### Get User

**Definition**

`GET /users/{id}`

- Requires `identify` scope for full user

**Response**

- `200 OK` on success
- `404 Not Found` if the user does not exist
- Will return a partial user without `identify` scope

### Get User Downloads

**Definition**

`GET /users/{id}/downloads`

- Requires `downloads` scope

**Response**

- `200 OK` on success
- `404 Not Found` if the user does not exist
- `403 Forbidden` without `downloads` scope

### Leave Download

**Definition**

`DELETE /users/{id}/downloads/{id}`

- Requires `downloads` scope

**Response**

- `200 OK` on success
- `404 Not Found` if the user or download does not exist
- `403 Forbidden` without `downloads` scope

### List User Transactions

**Definition**

`GET /users/{id}/transactions`

- Requires `transactions` scope

**Response**

- `200 OK` on success
- `404 Not Found` if the user does not exist
- `403 Forbidden` without `transactions` scope

### Send User Transaction

**Definition**

`POST /users/{id}/transactions`

```json
{
	"description": "Simple Description",
	"amount": 500
}
```

- Requires `transactions` scope

**Response**

- `201 Created` on success
- `404 Not Found` if the user does not exist
- `403 Forbidden` without `transactions` scope

## Download

### List Downloads

**Definition**

`GET /downloads`

**Response**

- `200 OK` on success

### Get Download

**Definition**

`GET /downloads/{id}`

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exist

### Create Download

**Definition**

`POST /downloads`

```json
{
	"title": "New Download",
	"shortDescription": "Short simple description",
	"longDescription": "Longer HTML description without script or style tags"
}
```

**Response**

- `201 Created` on success
- `403 Forbidden` without `downloads` scope

### Update Download

**Definition**

`PUT /downloads`

```json
{
	"title": "Updated Download",
	"shortDescription": "New short description",
	"longDescription": "New long Description"
}
```

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exist
- `403 Forbidden` without `downloads` scope or if the user is not an author

### Delete Download

**Definition**

`DELETE /downloads/{id}`

**Response**

- `204 No Content` on success
- `404 Not Found` if the download does not exist
- `403 Forbidden` without `downloads` scope or if the user is not an author

### List Download Authors

**Definition**

`GET /downloads/{id}/authors`

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exist

### List Download Files

**Definition**

`GET /downloads/{id}/files`

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exist

### Create Download File

**Definition**

`POST /downloads/{id}/files`

```json
{
	"filename": "filename.txt"
}
```

+ attached file

**Response**

- `201 Created` on success
- `404 Not Found` if the download does not exist

### Get/Delete download file are not implemented pending changes to the file system

## Application

### Get Application

**Definition**

`GET /applications/{id}`

**Response**

- `200 OK` on success
- `404 Not Found` if the application does not exist

## Scoreboard

### Get Scoreboard

**Definition**

`GET /applications/@me/scoreboard`

**Response**

- `200 OK` on success

### Add Objective

**Definition**

`POST /applications/@me/scoreboard`

**Response**

- `201 Created` on success

### Delete Objective

**Definition**

`DELETE /applications/@me/scoreboard/{id}`

**Response**

- `204 No Content` on success
- `404 Not Found` if the objective does not exist

### Set Score

**Definition**

`PUT /applications/@me/scoreboard/{id}/users/{id}`

**Response**

- `204 No Content` on success
- `404 Not Found` if the user or objective does not exist
