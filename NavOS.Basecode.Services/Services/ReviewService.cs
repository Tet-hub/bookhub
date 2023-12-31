﻿using NavOS.Basecode.Data;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using NavOS.Basecode.Data.Repositories;
using NavOS.Basecode.Services.Interfaces;
using NavOS.Basecode.Services.ServiceModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NavOS.Basecode.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IEmailChecker _emailChecker;
        private readonly IEmailSender _emailSender;
        public ReviewService(IReviewRepository reviewRepository, IEmailChecker emailChecker, IEmailSender emailSender, IBookRepository bookRepository)
        {
            _reviewRepository = reviewRepository;
            _emailChecker = emailChecker;
            _emailSender = emailSender;
            _bookRepository = bookRepository;
        }
        /// <summary>
        /// Adds the review.
        /// </summary>
        /// <param name="review">The review.</param>
        public void AddReview(ReviewViewModel review, string host)
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
            var book = _bookRepository.GetBooks().FirstOrDefault(s => s.BookId == review.BookId);
            _emailSender.UserFeedBackReview(model.UserEmail, host, model.UserName, book.BookTitle);
        }
        /// <summary>
        /// Determines whether [contains bad words] [the specified text].
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        ///   <c>true</c> if [contains bad words] [the specified text]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsBadWords(string text)
        {
            return BadWordList(text);
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
		/// <summary>
		/// Checks the email valid asynchronous.
		/// </summary>
		/// <param name="Email">The email.</param>
		/// <returns></returns>
		public async Task<bool> CheckEmailValidAsync(string Email)
		{
			var isEmailValid = await _emailChecker.IsEmailValidAsync(Email);
			if (isEmailValid)
			{
				return true;
			}
			return false;
		}

        #region Private methods
        private bool BadWordList(string text)
        {
            try
            {
                string profanityTextFile = PathManager.DirectoryPath.ProfanityTextDirectory;
                string profanityTextFilePath = Path.Combine(profanityTextFile, "profanity") + ".html";

                if (File.Exists(profanityTextFilePath))
                {
                    string htmlContent = File.ReadAllText(profanityTextFilePath);

                    // Extract bad words from the script tag
                    var match = Regex.Match(htmlContent, @"<script>[\s\S]*?var badWords = (\[.*?\]);[\s\S]*?</script>");
                    if (match.Success)
                    {
                        string badWordsJson = match.Groups[1].Value;
                        var badWords = JsonConvert.DeserializeObject<List<string>>(badWordsJson);

                        bool containsProfanity = badWords.Any(word => text.Trim().Contains(word.Trim(), StringComparison.OrdinalIgnoreCase));

                        return containsProfanity;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Profanity file not found!");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading profanity words: {ex.Message}");
                return false;
            }
        }
        #endregion

    }
}
