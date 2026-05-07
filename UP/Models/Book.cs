using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP
{
    public partial class Book
    {
        public double AverageRating
        {
            get
            {
                if (Review == null || !Review.Any())
                    return 0;

                return Math.Round(Review.Average(r => r.Rating), 1);
            }
        }

        public string GenresText
        {
            get
            {
                if (GenreBook == null || !GenreBook.Any())
                    return "Жанры: отсутствуют";

                return "Жанры: " + string.Join(", ", GenreBook.Where(gb => gb.Genre != null).Select(gb => gb.Genre.Name));
            }
        }
    }
}
