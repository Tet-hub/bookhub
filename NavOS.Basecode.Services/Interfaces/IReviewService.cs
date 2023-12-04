using NavOS.Basecode.Data.Models;
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
        void AddReview(ReviewViewModel review, string host);
        List<ReviewViewModel> GetReviews();
        List<ReviewViewModel> GetReviews(string bookId);
        Task<bool> CheckEmailValidAsync(string Email);

	}
}
