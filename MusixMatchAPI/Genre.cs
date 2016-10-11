using System.Collections.Generic;
using System.Linq;

namespace MusixMatchAPI {

    public class MusicGenre {
        public int music_genre_id { get; set; }
        public int music_genre_parent_id { get; set; }
        public string music_genre_name { get; set; }
        public string music_genre_name_extended { get; set; }
    }
    public class MusicGenreList {
        public MusicGenre music_genre { get; set; }
    }
    public class PrimaryGenres {
        public List<MusicGenreList> music_genre_list { get; set; }
    }
    public class SecondaryGenres {
        public List<MusicGenreList> music_genre_list { get; set; }
    }

    public class Genre {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Name { get; set; }
        public string ExtendedName { get; set; }

        public static List<Genre> FromMusicGenreList(PrimaryGenres pg, SecondaryGenres sg) {
            List<Genre> GenreList = new List<Genre>();

            List<MusicGenreList> AllGenres = new List<MusicGenreList>();
            pg.music_genre_list.ForEach(mgl => {
                if(!AllGenres.Contains(mgl, new GenreByIDComparer())) {
                    AllGenres.Add(mgl);
                }
            });
            sg.music_genre_list.ForEach(mgl => {
                if(!AllGenres.Contains(mgl, new GenreByIDComparer())) {
                    AllGenres.Add(mgl);
                }
            });

            AllGenres.ForEach(mgl => {
                Genre gnr = new Genre();
                gnr.Name = mgl.music_genre.music_genre_name;
                gnr.ExtendedName = mgl.music_genre.music_genre_name_extended;
                gnr.ID = mgl.music_genre.music_genre_id;
                gnr.ParentID = mgl.music_genre.music_genre_parent_id;

                if(!GenreList.Contains(gnr)) {
                    GenreList.Add(gnr);
                }
            });
            return GenreList;
        }


        class GenreByIDComparer : IEqualityComparer<MusicGenreList> {
            public bool Equals(MusicGenreList x, MusicGenreList y) {
                return x.music_genre.music_genre_id == y.music_genre.music_genre_id;
            }

            public int GetHashCode(MusicGenreList obj) {
                return obj.music_genre.music_genre_id.GetHashCode();
            }
        }
    }

}
