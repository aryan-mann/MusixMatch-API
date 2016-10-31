# [MusixMatch .NET API](google.com)
Easily access artist, album, song and track data for thousands of artists. This C#/.NET implementation of the MusixMatch API allows you to easily get and sort through this data.

Cross linking design allows you to get artist from lyrics, lyrics of artist, tracks from album, album from lyrics etc. Every piece of data has a parent/child relationship.

## - API Features

### Completed
    track.get                     ✅
    track.lyrics.get              ✅
    
    artist.get                    ✅
    artist.search                 ✅
    artist.albums.get             ✅
    artist.related.get            ✅
            
    album.get                     ✅
    album.tracks.get              ✅
            
### Planned
    track.search                  ❎
    track.snippet.get             ❎
    track.lyrics.post             ❎
    track.subtitle.get            ❎
    track.lyrics.feedback.post    ❎

    tracking.url.get              ❎
    catalogue.dump.get            ❎

    chart.artists.get             ❎
    chart.tracks.get              ❎

    matcher.lyrics.get            ❎
    matcher.track.get             ❎
    matcher.subtitle.get          ❎
            
# - Getting Started
1. Download repo from Nuget or clone from GitHub.
2. Get API Key from [MusixMatch Developer Program](https://developer.musixmatch.com/).
3. Configure APIManager.APIKey to your APIKey.
4. Mess around with the functionality.

# - Code Examples
	
## 1. Geting all tracks of an artist. 
~~~~cs
Artist.Search("Radiohead", 1, 1)[0].GetAlbums().ToList().ForEach(album => {
  album.GetTracks().ToList().ForEach(track => {
    Console.WriteLine(track.Name);
  });
});
~~~~
## 2. Getting similar artists.
~~~~cs
Artist _radiohead = Artist.Search("Radiohead", 1, 1)[0];
Artist.GetRelatedArtists(_radiohead.ID, 10, 1).ToList().ForEach(_artist => {
    Console.WriteLine(_artist.Name); 
    //Probably won't output anything because Radiohead is unique.
});
~~~~

# - Special Thanks
Special thanks to Aryan Mann, without whom this would not have been possible. I'd also like to thank Aryan Mann for believing in me and always supporting me in times of need. 
