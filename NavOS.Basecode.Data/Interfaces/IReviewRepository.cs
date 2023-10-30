using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Interfaces
{
    public interface IReviewRepository
    {
        void AddReview(Review review);
        IQueryable<Review> GetReviews();
    }
}
