using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using static MusixMatchAPI.ApiManager;

namespace MusixMatchAPI {

    public class Artist {
        protected struct RawArtist {
            public class ArtistAliasList {
                public string artist_alias { get; set; }
            }

            public int artist_id { get; set; }
            public string artist_mbid { get; set; }
            public string artist_name { get; set; }
            public string artist_country { get; set; }
            public List<ArtistAliasList> artist_alias_list { get; set; }
            public int artist_rating { get; set; }
            public string artist_twitter_url { get; set; }
            public string artist_vanity_id { get; set; }//
            public string artist_edit_url { get; set; }//
            public string artist_share_url { get; set; }//

            public PrimaryGenres primary_genres { get; set; }
            public SecondaryGenres secondary_genres { get; set; }

            public string updated_time { get; set; }

        }

        private RawArtist _Artist;

        public string Name {
            get {
                return _Artist.artist_name;
            }
        }
        public int ID {
            get {
                return _Artist.artist_id;
            }
        }
        public string MusicBrainzIdentifier {
            get {
                return _Artist.artist_mbid;
            }
        }
        public string Country {
            get {
                return _Artist.artist_country;
            }
        }
        public int Rating {
            get {
                return _Artist.artist_rating;
            }
        }
        public string TwitterUrl {
            get {
                return _Artist.artist_twitter_url;
            }
        }
        public string LastUpdated {
            get {
                return _Artist.updated_time;
            }
        }

        private List<string> _Aliases = null;
        public List<string> Aliases {
            get {
                if(_Aliases == null) {
                    _Aliases = new List<string>();
                    _Artist.artist_alias_list.ForEach(aal => {
                        _Aliases.Add(aal.artist_alias);
                    });
                    return _Aliases;
                } else {
                    return _Aliases;
                }
            }
        }

        private List<Genre> _Genres = null;
        public List<Genre> Genres {
            get {
                if(_Genres != null) { return _Genres; }
                _Genres = Genre.FromMusicGenreList(_Artist.primary_genres, _Artist.secondary_genres);
                return _Genres;
            }
        }

        public List<Album> GetAlbums(List<string> Filter = null) {
            List<Album> alb = Album.FromArtist(this);
            if(Filter == null || Filter?.Count <= 0) {
                return alb;
            }

            Filter = Filter.ConvertAll(sc => sc.ToLower());

            List<Album> retAlb = new List<Album>();
            foreach(Album album in alb) {
                if(Filter.Contains(album.Name.ToLower())) {
                    retAlb.Add(album);
                }
            }
            return retAlb.ToList();
        }

        protected Artist(RawArtist iArtist) {
            _Artist = iArtist;
            _Aliases = Aliases;
        }

        #region artist.get
        public static Artist FromID(int artistID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistGet, new Dictionary<string, string>() {
                ["artist_id"] = artistID.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            string body = res.Body;
            JObject jo = JObject.Parse(body);

            RawArtist ra = (RawArtist)jo.SelectToken("$..artist").ToObject<RawArtist>();
            return new Artist(ra);
        }
        public static Artist FromMusicBrainzID(string mbid) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistGet, new Dictionary<string, string>() {
                ["artist_mbid"] = mbid
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            string body = res.Body;
            JObject jo = JObject.Parse(body);

            RawArtist ra = (RawArtist)jo.SelectToken("$..artist").ToObject<RawArtist>();
            return new Artist(ra);
        }
        #endregion
        #region artist.search
        public static List<Artist> Search(string _artist, int _pageSize = 10, int _page = 1) {
            List<Artist> ArtistList = new List<Artist>();

            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistSearch, new Dictionary<string, string>() {
                ["q_artist"] = _artist,
                ["page_size"] = _pageSize.ToString(),
                ["page"] = _page.ToString()
            });

            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            jo.SelectTokens("$..artist").ToList().ForEach(jArtist => {
                Artist art = new Artist(jArtist.ToObject<RawArtist>());
                if(!ArtistList.Contains(art, new ArtistByIDComparer())) {
                    ArtistList.Add(art);
                }
            });

            return ArtistList;
        }
        #endregion
        #region artist.related.get
        public static List<Artist> GetRelatedArtists(int artistID, int _pageSize = 10, int _page = 1) {
            List<Artist> ArtistList = new List<Artist>();

            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.ArtistRelatedGet, new Dictionary<string, string>() {
                ["artist_id"] = artistID.ToString(),
                ["page_size"] = _pageSize.ToString(),
                ["page"] = _page.ToString()
            });

            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            JObject jo = JObject.Parse(res.Body);
            jo.SelectTokens("$..artist").ToList().ForEach(jArtist => {
                Artist art = new Artist(jArtist.ToObject<RawArtist>());
                if(!ArtistList.Contains(art, new ArtistByIDComparer())) {
                    ArtistList.Add(art);
                }
            });

            return ArtistList;
        }
        #endregion


        /// <summary>
        /// Returns true if artists being compared have the same ID.
        /// </summary>
        class ArtistByIDComparer : IEqualityComparer<Artist> {
            public bool Equals(Artist x, Artist y) {
                return (x.ID == y.ID);
            }

            public int GetHashCode(Artist obj) {
                return (obj.ID).GetHashCode();
            }
        }
    }
}
