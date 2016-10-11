using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static MusixMatchAPI.ApiManager;

namespace MusixMatchAPI {

    public class Album {
        protected struct RawAlbum {
            public int album_id { get; set; }
            public string album_mbid { get; set; }
            public string album_name { get; set; }
            public int album_rating { get; set; }
            public int album_track_count { get; set; }
            public string album_release_date { get; set; }
            public string album_release_type { get; set; }//
            public int artist_id { get; set; }
            public string artist_name { get; set; }
            public string album_pline { get; set; }//
            public string album_copyright { get; set; }
            public string album_label { get; set; }
            public string album_coverart_100x100 { get; set; }

            public PrimaryGenres primary_genres { get; set; }
            public SecondaryGenres secondary_genres { get; set; }

            public string updated_time { get; set; }
        }

        private RawAlbum _Album;

        public int ID {
            get {
                return _Album.album_id;
            }
        }
        public string MusicBrainzID {
            get {
                return _Album.album_mbid;
            }
        }
        public string Name {
            get {
                return _Album.album_name;
            }
        }
        public int Rating {
            get {
                return _Album.album_rating;
            }
        }
        public int TrackCount {
            get {
                return _Album.album_track_count;
            }
        }
        public string Label {
            get {
                return _Album.album_label;
            }
        }
        public string Copyright {
            get {
                return _Album.album_copyright;
            }
        }
        public string ReleaseDate {
            get {
                return _Album.album_release_date;
            }
        }

        private int ArtistID {
            get {
                return _Album.artist_id;
            }
        }

        public string LastUpdated {
            get {
                return _Album.updated_time;
            }
        }

        public List<Genre> _Genres = null;
        public List<Genre> Genres {
            get {
                if(_Genres != null) { return _Genres; }

                _Genres = Genre.FromMusicGenreList(_Album.primary_genres, _Album.secondary_genres) ?? new List<Genre>();
                return _Genres;
            }
        }

        protected Album(RawAlbum ri) {
            _Album = ri;
            _Genres = Genres;
        }

        public List<Track> GetTracks(List<string> Filter = null) {
            List<Track> tra = Track.FromAlbum(this);
            if(tra == null) { return new List<Track>(); }

            if(Filter == null || Filter?.Count <= 0) {
                return tra;
            }

            Filter = Filter.ConvertAll(sc => sc.ToLower());

            List<Track> retTra = new List<Track>();
            foreach(Track track in tra) {
                if(Filter.Contains(track.Name.ToLower())) {
                    retTra.Add(track);
                }
            }
            return retTra.ToList();
        }

        #region album.get
        public static Album FromID(int albumID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.AlbumGet, new Dictionary<string, string>() {
                ["album_id"] = albumID.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            return new Album(jo.SelectToken("$..album").ToObject<RawAlbum>());
        }
        public static Album FromMusicBrainzID(string mbid) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.AlbumGet, new Dictionary<string, string>() {
                ["album_mbid"] = mbid
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            return new Album(jo.SelectToken("$..album").ToObject<RawAlbum>());
        }
        #endregion

        #region artist.album.get
        /// <summary>
        /// Retrieves a list of albums by an artist.
        /// </summary>
        /// <param name="_artist">Artist instance.</param>
        /// <param name="sort_desc">Sort albums in descending order?</param>
        /// <param name="group_album">Group albums by name?</param>
        /// <param name="page_size">Numbers of albums to retrieve.</param>
        /// <param name="page">Get data from page number __?</param>
        /// <returns></returns>
        public static List<Album> FromArtist(Artist _artist, bool sort_desc = true, bool group_album = true, int page_size = 100, int page = 1) {
            return FromArtistID(_artist.ID, sort_desc, group_album, page_size, page);
        }
        /// <summary>
        /// Retrieves a list of albums by an artist.
        /// </summary>
        /// <param name="artistID">ID of artist.</param>
        /// <param name="sort_desc">Sort albums in descending order?</param>
        /// <param name="group_album">Group albums by name?</param>
        /// <param name="page_size">Numbers of albums to retrieve.</param>
        /// <param name="page">Get data from page number __?</param>
        /// <returns></returns>
        public static List<Album> FromArtistID(int artistID, bool sort_desc = true, bool group_album = true, int page_size = 100, int page = 1) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistAlbumsGet, new Dictionary<string, string>() {
                ["artist_id"] = artistID.ToString(),
                ["page_size"] = page_size.ToString(),
                ["page"] = page.ToString(),
                ["s_release_date"] = (sort_desc ? "desc" : "asc"),
                ["g_album_name"] = (group_album ? "1" : "0")
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            List<Album> ReturnAlbums = new List<Album>();
            jo.SelectTokens("$..album").ToList().ForEach(tAlbum => {
                ReturnAlbums.Add(new Album(tAlbum.ToObject<RawAlbum>()));
            });
            return ReturnAlbums;
        }
        /// <summary>
        /// Retrieves a list of albums by an artist.
        /// </summary>
        /// <param name="artistMBID">MusicBrainz ID of artist.</param>
        /// <param name="sort_desc">Sort albums in descending order?</param>
        /// <param name="group_album">Group albums by name?</param>
        /// <param name="page_size">Numbers of albums to retrieve.</param>
        /// <param name="page">Get data from page number __?</param>
        /// <returns></returns>
        public static List<Album> FromArtistMusicBrainzID(string artistMBID, bool sort_desc = true, bool group_album = true, int page_size = 100, int page = 1) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistAlbumsGet, new Dictionary<string, string>() {
                ["artist_mbid"] = artistMBID.ToString(),
                ["page_size"] = page_size.ToString(),
                ["page"] = page.ToString(),
                ["s_release_date"] = (sort_desc ? "desc" : "asc"),
                ["g_album_name"] = (group_album ? "1" : "0")
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            List<Album> ReturnAlbums = new List<Album>();
            jo.SelectTokens("$..album").ToList().ForEach(tAlbum => {
                ReturnAlbums.Add(new Album(tAlbum.ToObject<RawAlbum>()));
            });
            return ReturnAlbums;
        }
        #endregion

    }


}
