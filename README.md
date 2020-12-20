# Twitter API Demo

This project demonstrates a .NET 5 application which processes
data from the Twitter API v2 Sampled Stream endpoint. It keeps track
of and reports the following:

- Total number of tweets received
- Average tweets per hour/minute/second
- Top emoji in tweets
- Percent of tweets processed that contain emoji
- Top hashtags
- Percent of tweets that contain a URL
- Percent of tweets that contain a photo URL (pic.twitter.com or instagram)
- Top domains of URLs

This application exposes all of the data through a set of endpoints.

## Running

To run the application, simply populate the appsettings.Development.json file
with your API credentials and start it.

You will require .NET 5 in order to run this application.