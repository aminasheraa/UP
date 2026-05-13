using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace UP.Models
{
    public partial class Book
    {
        public double AverageRating
        {
            get
            {
                if (this.Review == null || this.Review.Count == 0)
                    return 0;

                return Math.Round(this.Review.Average(r => r.Rating), 1);
            }
        }
        public string GenresText => "Жанры: " + string.Join(", ", GenreBook.Select(gb => gb.Genre.Name));
    }
}