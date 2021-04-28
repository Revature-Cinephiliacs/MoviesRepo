
# CINEPHILIACS MOVIE REPO

## Project Description

This is the microservice used to display and override movie information retrieved from a third party API, the [Movie Database API]( https://rapidapi.com/rapidapi/api/movie-database-imdb-alternative/). It also handles movie tags and allows users to search movies by genre, director, and actor as well as title.  

## Technologies Used

* C#
* Entity Framework
* xUnit Testing
* SQL
* Visual Studio
* Git

## Features

List of current features
* Retrieve movie
* Search by director, actor, genre, language, tags
* Update movie information
* Add/delete movie tags 

## Endpoint Objects
* Movie
  * imdbId: string,
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
  * JS_Object["key1"] = ["value1", "value2", "value3"];
  * JS_Object["key2"] = ["value4"];
#### Where the keys are one of these keywords: "Tags", "Rating", "Actors", "Directors", "Genres", "Languages"
#### and the values are strings that belong to the key category, i.e.: "Tags":["Scary","Funny"] or "Actors":["Harrison Ford"]

## Endpoints
| Description                                      | Type   | Path                              | Request Body | Returned | Comments                                                                                                                                         |
|--------------------------------------------------|--------|-----------------------------------|--------------|----------|--------------------------------------------------------------------------------------------------------------------------------------------------|
| Gets movie details by id                         | GET    | movie/{movieid}                   |              | (Movie)  | If the movie does not exist, returns the data from the public movie API.                                                                         |
| Creates a movie                                  | POST   | movie                             | (Movie)      |          | Fails if the movie already exists.                                                                                                               |
| Replaces or creates a movie                      | PUT    | movie/{movieid}                   | (Movie)      |          | All movie properties are overwritten to match the provided Movie object.                                                                         |
| Appends new data to a movie                      | PATCH  | movie/{movieid}                   | (Movie)      |          | Only the provided properties are updated, missing properties remain unchanged. If movie does not exist, uses data from public movie API as base. |
| Deletes a movie                                  | DELETE | movie/{movieid}                   |              |          |                                                                                                                                                  |
| Gets all movies that match all search values     | POST   |                                   | (JS_Object)  | string[] | Returns an array of movieId strings. Does not search the public movie API.                                                                       |
| Submits a user's vote for/against a tag          | POST   | movie/tag/movie                   | (Tag)        |          |                                                                                                                                                  |
| (Admin) Ban a tag                                | PUT    | movie/tag/ban/{tagname}           |              |          | Banned tags are not returned with movie details                                                                                                  |
| (Admin) Unban a tag                              | PUT    | movie/tag/unban/{tagname}         |              |          |                                                                                                                                                  |
| Adds the movie to the user's following list      | PUT    | movie/follow/{movieid}/{userid}   |              |          |                                                                                                                                                  |
| Removes the movie from the user's following list | DELETE | movie/follow/{movieid}/{userid} |              |          |                                                                                                                                                  |
| Gets all movies that the user is following       | GET    | movie/follow/{userid}          |              |          | Returns a list containing the movieid of each movie.                                                                                             |
| A Test method for deployment                     | GET    | movie/test                        |              |          |                                                                                                                                                  |
### Object usage within an endpoint is denoted by placing the object name with parenthesis: (Object)

## Contributors

> Tristyn Linde, Anis Medini, Matthew Grimsley, Beau Crumley, and Christopher Trimmer.

## License

This project uses the following license: [MIT License]( https://mit-license.org/).