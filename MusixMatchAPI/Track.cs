using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static MusixMatchAPI.ApiManager;

namespace MusixMatchAPI {

    public class Track {
        protected struct RawTrack {
            public int track_id { get; set; }
            public string track_mbid { get; set; }
            public string track_spotify_id { get; set; }
            public int track_soundcloud_id { get; set; }
            public string track_name { get; set; }
            public int track_rating { get; set; }
            public int track_length { get; set; }
            public int commontrack_id { get; set; }
            public int instrumental { get; set; }
            public int @explicit { get; set; }
            public int has_lyrics { get; set; }
            public int has_subtitles { get; set; }
            public int num_favourite { get; set; }
            public int lyrics_id { get; set; }
            public int subtitle_id { get; set; }
            public int album_id { get; set; }
            public string album_name { get; set; }
            public int artist_id { get; set; }
            public string artist_mbid { get; set; }//
            public string artist_name { get; set; }
            public string album_coverart_100x100 { get; set; }
            public string album_coverart_350x350 { get; set; }
            public string album_coverart_500x500 { get; set; }
            public string album_coverart_800x800 { get; set; }
            public string track_share_url { get; set; }//
            public string track_edit_url { get; set; }//
            public string updated_time { get; set; }
            public PrimaryGenres primary_genres { get; set; }
            public SecondaryGenres secondary_genres { get; set; }
        }

        private RawTrack _Track;

        public string Name {
            get {
                return _Track.track_name;
            }
        }
        public int ID {
            get {
                return _Track.track_id;
            }
        }
        public string MusicBrainzID {
            get {
                return _Track.artist_mbid;
            }
        }
        public string SpotifyID {
            get {
                return _Track.track_spotify_id;
            }
        }
        public int SoundCloudID {
            get {
                return _Track.track_soundcloud_id;
            }
        }
        public int CommonTrackID {
            get {
                return _Track.commontrack_id;
            }
        }
        public int Rating {
            get {
                return _Track.track_rating;
            }
        }
        public int Length {
            get {
                return _Track.track_length;
            }
        }
        public bool IsInstrumental {
            get {
                return (_Track.instrumental == 1);
            }
        }
        public bool IsExplicit {
            get {
                return (_Track.@explicit == 1);
            }
        }
        public bool HasLyrics {
            get {
                return (_Track.has_lyrics == 1);
            }
        }
        public bool HasSubtitles {
            get {
                return (_Track.has_subtitles == 1);
            }
        }
        /// <summary>
        /// Number of times favourited by users.
        /// </summary>
        public int Favourites {
            get {
                return _Track.num_favourite;
            }
        }

        /// <summary>
        /// URL to the 100x100 Cover Art
        /// </summary>
        public string CoverArt100 {
            get {
                return _Track.album_coverart_100x100;
            }
        }
        /// <summary>
        /// URL to the 350x350 Cover Art
        /// </summary>
        public string CoverArt350 {
            get {
                return _Track.album_coverart_350x350;
            }
        }
        /// <summary>
        /// URL to the 500x500 Cover Art
        /// </summary>
        public string CoverArt500 {
            get {
                return _Track.album_coverart_500x500;
            }
        }
        /// <summary>
        /// URL to the 800x800 Cover Art
        /// </summary>
        public string CoverArt800 {
            get {
                return _Track.album_coverart_800x800;
            }
        }

        private int ArtistID {
            get {
                return _Track.artist_id;
            }
        }
        private int AlbumId {
            get {
                return _Track.album_id;
            }
        }
        private int LyricsID {
            get {
                return (_Track.lyrics_id);
            }
        }
        private int SubtitleID {
            get {
                return (_Track.subtitle_id);
            }
        }

        private List<Genre> _Genres = null;
        public List<Genre> Genres {
            get {
                if(_Genres != null) { return _Genres; }

                _Genres = Genre.FromMusicGenreList(_Track.primary_genres, _Track.secondary_genres);
                return _Genres;
            }
        }

        public string LastUpdate {
            get {
                return _Track.updated_time;
            }
        }

        private Track(RawTrack rt) {
            _Track = rt;
        }

        public Lyrics GetLyrics() {
            return Lyrics.FromID(ID);
        }

        #region track.get
        public static Track FromID(int trackID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.TrackGet, new Dictionary<string, string>() {
                ["track_id"] = trackID.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);

            return new Track(jo.SelectToken("$..track").ToObject<RawTrack>());
        }
        public static Track FromMusicBrainzID(string trackMBID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.TrackGet, new Dictionary<string, string>() {
                ["track_id"] = trackMBID.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);

            return new Track(jo.SelectToken("$..track").ToObject<RawTrack>());
        }
        #endregion

        #region album.tracks.get
        public static List<Track> FromAlbum(Album album, bool hasLyrics = false, int page_size = 100, int page = 1) {
            return FromAlbumID(album.ID, hasLyrics, page_size, page);
        }
        public static List<Track> FromAlbumID(int albumID, bool hasLyrics = false, int page_size = 100, int page = 1) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.AlbumTracksGet, new Dictionary<string, string>() {
                ["album_id"] = albumID.ToString(),
                ["f_has_lyrics"] = (hasLyrics ? "1" : "0"),
                ["page_size"] = page_size.ToString(),
                ["page"] = page.ToString()
            });

            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);

            List<Track> ReturnTracks = new List<Track>();
            jo.SelectTokens("$..track").ToList().ForEach(jTrack => {
                ReturnTracks.Add(new Track(jTrack.ToObject<RawTrack>()));
            });
            return ReturnTracks;
        }
        public static List<Track> FromAlbumMusicBrainzID(string albumMBID, bool hasLyrics = false, int page_size = 100, int page = 1) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.AlbumTracksGet, new Dictionary<string, string>() {
                ["album_mbid"] = albumMBID.ToString(),
                ["f_has_lyrics"] = (hasLyrics ? "1" : "0"),
                ["page_size"] = page_size.ToString(),
                ["page"] = page.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);

            List<Track> ReturnTracks = new List<Track>();
            jo.SelectTokens("$..track").ToList().ForEach(jTrack => {
                ReturnTracks.Add(new Track(jTrack.ToObject<RawTrack>()));
            });
            return ReturnTracks;
        }
        #endregion

    }

}
