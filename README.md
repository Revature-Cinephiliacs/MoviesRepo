# Cinephiliacs Movie Microservice

## Description
This microservice is part of the Cinephiliacs application. It manages all data directly related to Movies, such as information about movies and tags applied to movies. The endpoints available create, query, or manipulate that data.

## Endpoint Objects
* Movie
  * imdbId: string, [Required]
  * title: string,
  * ratingName: string,
  * releaseDate: string,
  * releaseCountry: string,
  * runtimeMinutes: number,
  * isReleased: boolean,
  * plot: string,
  * postURL: string,
  * movieActors: string[],
  * movieDirectors: string[],
  * movieGenres: string[],
  * movieLanguages: string[],
  * movieTags: string[]

* Tag
  * movieId: string,
  * userId: string,
  * tagName: string,
  * isUpvote: boolean

* JS_Object (JavaScript Object)
  * JS_Object["key1"] = "value1";
  * JS_Object["key2"] = "value2";
Where the keys are one of these keywords: "Tag", "Actor", "Director", "Genre", "Language"
and the values are a string that is an instance of the key, i.e.: "Tag":"Scary" or "Actor":"Harrison Ford"

### Object usage within an endpoint is denoted by placing the object name with parenthesis: (Object)
## Endpoints
| Description                                  | Type   | Path                      | Request Body | Returned | Comments                                                                           |
|----------------------------------------------|--------|---------------------------|--------------|----------|------------------------------------------------------------------------------------|
| Gets movie details by id                     | Get    | movie/{movieid}           |              | (Movie)  |                                                                                    |
| Gets all movies that match all search values | Post   | movie/search              | (JS_Object)  | string[] | Returns an array of movieIDs                                                       |
| Creates or Updates a movie's details         | Patch  | movie/update              | (Movie)      |          | All values overwrite existing values.                                              |
| Appends information to a movie's details     | Patch  | movie/append              | (Movie)      |          | Missing properties remain unchanged. Array values are appended to existing values. |
| Deletes a movie                              | Delete | movie/{movieid}           |              |          | Also deletes associated information                                                |
| Submits a user's vote for/against a tag      | Post   | movie/tag/movie           | (Tag)        |          |                                                                                    |
| (Admin) Ban a tag                            | Post   | movie/tag/ban/{tagname}   |              |          | Banned tags are not returned with movie details                                    |
| (Admin) Unban a tag                          | Post   | movie/tag/unban/{tagname} |              |          |                                                                                    |
| A Test method for deployment                 | Get    | movie/test                |              |          |                                                                                    |