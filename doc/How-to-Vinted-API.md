[Home](../README.md)

# How Vinted API is used

Last update : september 2025

# Get user informations (account, favourites, messages)
Base URL used for API call is the following : https://www.vinted.fr/api/v2

Vinted API is used for the following endpoints:

| Endpoint |  Description
|----------|------------------------
| users/{userId}?localize=false | Get user informations
| users/{userId}/items/favourites?page={pageCounter}&include_sold=true&per_page={perPages} | Iterrate on favourites list by using pagination
| users/{userId}/msg_threads/{threadId} | Get information of a message thread

API call should include the following headers

| Header name | Value
|-------------|----------------
| Cookie      | access_token_web=TOKEN1; refresh_token_web=TOKEN2 (where TOKEN1 and TOKEN2 corresponds to the cookies with the same name from host .www.vinted.fr)
| Accept-Encoding | identity |
| Accept-Language | fr-FR,fr;q=0.9,en-US;q=0.8,en;q=0.7 |
| User-Agent | (use a valid browser user agent) |

A Proof of Concept can be found in project [Sandbox](../src/Sandbox)

## Get Item informations

### Without authentication, by parsing public web page
Items information are retrieved directly from the public item page, without authentication

https://www.vinted.fr/items/{itemId}

From the page body, item information are extracted from a `script` element containing a serialized `item` and `brand_dto`

Data are stored in stringified json, from Next.js hydration scripts functions like `self.__next_f.push`. We need to :
- extract the stringified json
- convert it to json object
- extract `item` and `brand_dto` from the json Tree.

### By using API endpoint, but need authentication
API can be used but endpoint call needs headers for authentication as described in API section above.

Base URL : https://www.vinted.fr/api/v2

| Endpoint | Description
|----------|------------
| /items/{itemId}/more?content_source=other_user_items | Get item informations

[Home](../README.md)