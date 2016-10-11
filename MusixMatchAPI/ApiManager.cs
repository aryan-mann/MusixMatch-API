using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MusixMatchAPI {
    /*    NOT DONE - ❎ | DONE - ✅
            // CHART    
            * chart.artists.get             ❎
            * chart.tracks.get              ❎
            // TRACK
            * track.search                  ❎
            * track.get                     ✅
            * track.subtitle.get            ❎
            * track.lyrics.get              ✅
            * track.snippet.get             ❎
            * track.lyrics.post             ❎
            * track.lyrics.feedback.post    ❎
            // MATCHER
            * matcher.lyrics.get            ❎
            * matcher.track.get             ❎
            * matcher.subtitle.get          ❎
            // ARTIST
            * artist.get                    ✅
            * artist.search                 ✅
            * artist.albums.get             ✅
            * artist.related.get            ✅
            // ALBUM
            * album.get                     ✅
            * album.tracks.get              ✅
            // MISC
            * tracking.url.get              ❎
            * catalogue.dump.get            ❎
        */

    /// <summary>
    /// MusixMatch API Handler
    /// </summary>
    public static class ApiManager {

        private const string RootUrl = @"http://api.musixmatch.com/ws/1.1/";
        /// <summary>
        /// Developer API Key (https://developer.musixmatch.com/)
        /// </summary>
        public static string ApiKey { get; set; } = "";
        //Either entery ApiKey above or set from the outside.

        /// <summary>
        /// Gets raw HTML data from a webpage
        /// </summary>
        /// <param name="_url"></param>
        /// <returns></returns>
        private static string GetJsonData(string _url) {
            try {
                return new WebClient().DownloadString(new Uri(_url));
            } catch {
                return string.Empty;
            }
        }

        public enum ApiMethod {
            ChartArtistsGet, ChartTracksGet,
            TrackSearch, TrackGet, TrackSubtitleGet, TrackLyricsGet, TrackSnippetGet, TrackLyricsPost, TrackLyricsFeedbackPost,
            MatcherLyricsGet, MatcherTrackGet, MatcherSubtitleGet,
            ArtistGet, ArtistSearch, ArtistAlbumsGet, ArtistRelatedGet,
            AlbumGet, AlbumTracksGet,
            TrackingUrlGet, CatalogueDumpGet
        }

        public class ApiRequest {

            private static string ApiKeyUrl {
                get {
                    return $"&apikey={ApiKey}";
                }
            }
            private static Dictionary<ApiMethod, string> ApiMethodConverter = new Dictionary<ApiMethod, string>() {
                #region Collapse
                [ApiMethod.ChartArtistsGet] = "chart.artists.get",
                [ApiMethod.ChartTracksGet] = "chart.tracks.get",
                [ApiMethod.TrackSearch] = "track.search",
                [ApiMethod.TrackGet] = "track.get",
                [ApiMethod.TrackSubtitleGet] = "track.subtitle.get",
                [ApiMethod.TrackLyricsGet] = "track.lyrics.get",
                [ApiMethod.TrackSnippetGet] = "track.snippet.get",
                [ApiMethod.TrackLyricsPost] = "track.lyrics.post",
                [ApiMethod.TrackLyricsFeedbackPost] = "track.lyrics.feedback.post",
                [ApiMethod.MatcherLyricsGet] = "matcher.lyrics.get",
                [ApiMethod.MatcherTrackGet] = "matcher.track.get",
                [ApiMethod.MatcherSubtitleGet] = "matcher.subtitle.get",
                [ApiMethod.ArtistGet] = "artist.get",
                [ApiMethod.ArtistSearch] = "artist.search",
                [ApiMethod.ArtistAlbumsGet] = "artist.albums.get",
                [ApiMethod.ArtistRelatedGet] = "artist.related.get",
                [ApiMethod.AlbumGet] = "album.get",
                [ApiMethod.AlbumTracksGet] = "album.tracks.get",
                [ApiMethod.TrackingUrlGet] = "tracking.url.get",
                [ApiMethod.CatalogueDumpGet] = "catalogue.dump.get"
                #endregion
            };

            public static ApiRequest CreateRequest(ApiMethod apim, Dictionary<string, string> KvpArgs) {
                if(string.IsNullOrWhiteSpace(ApiKey)) { return null; }
                string Request = "";
                string Args = "";

                ApiRequest req = new ApiRequest();

                int count = 0;
                foreach(var kvp in KvpArgs) {
                    Args += ((count == 0) ? "?" : "&") + $"{kvp.Key}={kvp.Value}";
                    count++;
                }

                Request = $"{RootUrl}{ApiMethodConverter[apim]}{Args}{ApiKeyUrl}";
                req.RequestUrl = Request;

                return req;
            }

            public string RequestUrl { get; private set; }
            public ApiResponse GetResponse() {
                string _query = RequestUrl;

                ApiResponse resp = ApiResponse.GetResponse(new Uri(_query));
                return resp;
            }

        }
        public class ApiResponse {
            /// <summary>
            /// Information about the server response.
            /// </summary>
            public StatusCode Status { get; private set; }
            /// <summary>
            /// Requested Data in JSON form
            /// </summary>
            public string Body { get; private set; }

            private ApiResponse() { }

            /// <summary>
            /// Parse response from JSON data.
            /// </summary>
            /// <param name="data">The data.</param>
            /// <returns></returns>
            public static ApiResponse GetResponse(string data) {
                ApiResponse apiResponse = new ApiResponse();

                JObject jo;
                int status_code;
                float execute_time;
                string body;

                try {
                    jo = JObject.Parse(data);

                    status_code = jo.SelectToken("$..status_code", true).Value<int>();
                    execute_time = jo.SelectToken("$..execute_time", true).Value<float>();
                    body = jo.SelectToken("$..body").ToString();
                } catch { return null; }

                apiResponse.Status = StatusCode.Get(status_code, execute_time);
                apiResponse.Body = body;

                return apiResponse;
            }
            /// <summary>
            /// Retrieve and then parse response from an url.
            /// </summary>
            /// <param name="requestUrl">Request url.</param>
            /// <returns></returns>
            public static ApiResponse GetResponse(Uri requestUrl) {
                return GetResponse(new WebClient().DownloadString(requestUrl));
            }
        }
        public class StatusCode {

            public int Code { get; private set; }
            public string Description { get; private set; }
            public float ExecuteTime { get; private set; }
            public bool IsRequestValid {
                get {
                    return (Code == 200);
                }
            }

            private static Dictionary<int, string> CodeDatabase { get; } = new Dictionary<int, string>() {
                [200] = "The request was successful.",
                [400] = "The request had bad syntax or was inherently impossible to be satisfied.",
                [401] = "Authentication failed, probably because of invalid/missing API key.",
                [402] = "The usage limit has been reached, either you exceeded per day requests limits or your balance is insufficient.",
                [403] = "You are not authorized to perform this operation.",
                [404] = "The requested resource was not found.",
                [405] = "The requested method was not found.",
                [500] = "Oops. Something was wrong.",
                [503] = "Our system is a bit busy at the moment and your request can’t be satisfied."
            };

            private StatusCode() { }
            internal static StatusCode Get(int code, float executeTime) {
                if(!CodeDatabase.Keys.Contains(code)) {
                    return new StatusCode() { Code = 000, Description = "New Error Code?!", ExecuteTime = executeTime };
                }

                StatusCode sc = new StatusCode();
                sc.Code = code;
                sc.Description = CodeDatabase[code];
                sc.ExecuteTime = executeTime;
                return sc;
            }
        }
    }
}
