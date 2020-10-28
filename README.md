# Podplayer

A browser-based Podcatcher.

### Features
* Make an account and follow the Podcasts you love.
* Play an episode right in your browser.
* If a Podcast isn't in our database you can always add it via a URL.

### Tech

This project is structured in an MVC pattern.
* Podplayer.ASP contains the presentation layer. It uses ASP.NET Core 3.1, and Razor to generate the HTML.
* Podplayer.Entity contains the database implementation. This uses Entity Framework to retieve podcasts from the database, and Identity framework for user management.
* Podplayer.Core this defines our Model objects and common service interfaces so we can swap out our database if need-be. As well as the logic for parsing a RSS XML feed.

### Screenshots
