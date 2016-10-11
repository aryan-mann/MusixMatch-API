using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using static MusixMatchAPI.ApiManager;

namespace MusixMatchAPI {

    /// <summary>
    /// Retrieves only 30% of lyrics. Why? Ask MusixMatch..
    /// </summary>
    public class Lyrics {
        protected struct RawLyrics {
            public int lyrics_id { get; set; }
            public int restricted { get; set; }
            public int instrumental { get; set; }
            public string lyrics_body { get; set; }
            public string lyrics_language { get; set; }
            public string script_tracking_url { get; set; }
            public string pixel_tracking_url { get; set; }
            public string html_tracking_url { get; set; }
            public string lyrics_copyright { get; set; }
            public string updated_time { get; set; }
        }

        private RawLyrics _Lyrics;

        public int ID {
            get {
                return _Lyrics.lyrics_id;
            }
        }
        public bool Restricted {
            get {
                return (_Lyrics.restricted == 1);
            }
        }
        public bool Instrumental {
            get {
                return (_Lyrics.instrumental == 1);
            }
        }
        /// <summary>
        /// Lyrics with junk ending.
        /// </summary>
        public string RawBody {
            get {
                return _Lyrics.lyrics_body;
            }
        }
        /// <summary>
        /// Lyrics with junk ending removed.
        /// </summary>
        public string Body {
            get {
                return Regex.Replace(RawBody, @"(\n\*+.+\n\(\d+\))|(\.\.\.)", string.Empty);
            }
        }
        public string Language {
            get {
                return _Lyrics.lyrics_language;
            }
        }
        public string Copyright {
            get {
                return _Lyrics.lyrics_copyright;
            }
        }

        public string ScriptTrackingUrl {
            get {
                return _Lyrics.script_tracking_url;
            }
        }
        public string PixelTrackingUrl {
            get {
                return _Lyrics.pixel_tracking_url;
            }
        }
        public string HtmlTrackingUrl {
            get {
                return _Lyrics.html_tracking_url;
            }
        }

        public string LastUpdated {
            get {
                return _Lyrics.updated_time;
            }
        }

        private Lyrics(RawLyrics rl) {
            _Lyrics = rl;
        }

        #region track.lyrics.get
        public static Lyrics FromID(int trackID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.TrackLyricsGet, new Dictionary<string, string>() {
                ["track_id"] = trackID.ToString()
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            string body = res.Body;
            JObject jo = JObject.Parse(body);

            RawLyrics rl = (RawLyrics)jo.SelectToken("$..lyrics").ToObject<RawLyrics>();
            return new Lyrics(rl);
        }
        public static Lyrics FromMusicBrainzID(string trackMBID) {
            ApiRequest req = ApiRequest.CreateRequest(ApiMethod.TrackLyricsGet, new Dictionary<string, string>() {
                ["track_mbid"] = trackMBID
            });
            ApiResponse res = req.GetResponse();
            if(!res.Status.IsRequestValid) { return null; }

            string body = res.Body;
            JObject jo = JObject.Parse(body);

            RawLyrics rl = (RawLyrics)jo.SelectToken("$..lyrics").ToObject<RawLyrics>();
            return new Lyrics(rl);
        }
        #endregion
    }

}
