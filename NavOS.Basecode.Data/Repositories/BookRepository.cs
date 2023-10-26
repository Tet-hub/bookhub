﻿using Basecode.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using NavOS.Basecode.Data.Interfaces;
using NavOS.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NavOS.Basecode.Data.Repositories
{
    public class BookRepository : BaseRepository, IBookRepository
    {
        public BookRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {
        
        }
        //retrieve all books
        public IQueryable<Book> GetBooks()
        {
            return this.GetDbSet<Book>();
        }
        //retrieve single book
        public IQueryable<Book> GetBook(string BookId)
        {
            var book = this.GetDbSet<Book>().Where(x => x.BookId == BookId);
            return book;
        }

    }
}
