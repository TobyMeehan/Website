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

## Models

### User

```json
{
	"id": "",
	"username": "Steve",
	"balance": 1600,
	"roles": [
		"id": "",
		"name": "Verified"
	]
}
```

### Partial User

```json
{
	"id": "",
	"username": "Bob"
}
```

### Download

```json
{
	"id": "",
	"title": "Pseudocode Editor",
	"shortDescription": "Simple plaintext description",
	"longDescription": "More detailed HTML description"
}
```

### Application

```json
{
	"id": "",
	"name": "Casino",

}
```

### Connection

```json
{
	"id": "",
	"userid": "",
	"appid": ""
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

### Update User

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

### Get User Transaction

**Definition**

`GET /users/{id}/transactions/{id}`

- Requires `transactions` scope

**Response**

- `200 OK` on success
- `404 Not Found` if the user or transaction does not exist
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

### List User Connections

**Definition**

`GET /users/{id}/connections`

**Response**

- `200 OK` on success
- `404 Not Found` if the user does not exist
- `403 Forbidden` if the `connections` scope is lacking

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
	"authors": [
		{
			"id": ""
		},
		{
			"id", ""
		}
	]
}
```

**Response**

- `201 Created` on success
- `403 Forbidden` if the `downloads` scope is lacking

### Update Download

### Delete Download

**Definition**

`DELETE /downloads/{id}`

**Response**

- `204 No Content` on success
- `404 Not Found` if the download does not exist
- `403 Forbidden` if the `downloads` scope is lacking

### List Download Authors

**Definition**

`GET /downloads/{id}/authors`

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exist

### List Download Comments

**Definition**

`GET /downloads/{id}/comments`

**Response**

- `200 OK` on success
- `404 Not Found` if the download does not exits

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
