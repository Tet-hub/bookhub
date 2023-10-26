using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Interfaces
{
    public interface IReviewService
    {
        public void AddReview(ReviewViewModel review);
        public List<ReviewViewModel> GetReviews();
    }
}
