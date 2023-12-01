﻿using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }
        /// <summary>
        /// Adds the review.
        /// </summary>
        /// <param name="review">The review.</param>
        public void AddReview(ReviewViewModel review)
        {
            var model = new Review();

            model.ReviewId = Guid.NewGuid().ToString();
            model.BookId = review.BookId;
            model.UserEmail = review.UserEmail;
            model.UserName = review.UserName;
            model.ReviewText = review.ReviewText;
            model.Rate = review.Rate;
            model.DateReviewed = DateTime.Now;

            _reviewRepository.AddReview(model);

        }
        /// <summary>
        /// Gets the reviews.
        /// </summary>
        /// <returns></returns>
        public List<ReviewViewModel> GetReviews()
        {
            var data = _reviewRepository.GetReviews().Select(s => new ReviewViewModel
            {
                BookId = s.BookId,
                UserName = s.UserName,
                ReviewText = s.ReviewText,
                Rate = s.Rate,
                DateReviewed = s.DateReviewed,
            })
            .ToList();

            return data;
        }
        /// <summary>
        /// Gets the reviews based on bookId
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <returns></returns>
        public List<ReviewViewModel> GetReviews(string bookId)
        {
            var data = _reviewRepository.GetReviews()
                .Where(s => s.BookId == bookId)
                .Select(s => new ReviewViewModel
                {
                    BookId = s.BookId,
                    UserName = s.UserName,
                    ReviewText = s.ReviewText,
                    Rate = s.Rate,
                    DateReviewed = s.DateReviewed,
                })
                .ToList();

            return data;
        }


    }
}
