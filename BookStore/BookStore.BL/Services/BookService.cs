﻿using AutoMapper;
using BookStore.BL.Interfaces;
using BookStore.DL.Interfaces;
using BookStore.Models.Models;
using BookStore.Models.Requests;
using BookStore.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BookStore.BL.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookInMemoryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookInMemoryRepository, IMapper mapper, ILogger<BookService> logger)
        {
            _bookInMemoryRepository = bookInMemoryRepository;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<BookResponse> AddBook(BookRequest bookRequest)
        {
            try
            {
                var book = await _bookInMemoryRepository.GetById(bookRequest.Id);

                if (book != null)
                {
                    return new BookResponse()
                    {
                        Book =  book,
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Message = "Book already exist"
                    };
                }

                if (bookRequest.Id <= 0)
                {
                    return new BookResponse()
                    {
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Message = $"Parameter id:{bookRequest.Id} must be greater than 0"
                    };
                }
                var b = _mapper.Map<Book>(bookRequest);
                var result = _bookInMemoryRepository.AddBook(b);

                return new BookResponse()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Book = await result,
                    Message = "Successfully added book"
                };
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        [Authorize ]
        public async Task<Book?> DeleteBook(int bookId)
        {
            return await _bookInMemoryRepository.DeleteBook(bookId);
        }

        public async Task<IEnumerable<Book>> GetAllBooks()
        {
            return await _bookInMemoryRepository.GetAllBooks();
        }

        public async Task<Book?> GetById(int id)
        {
            return await _bookInMemoryRepository.GetById(id);
        }

        public async Task<BookResponse> UpdateBook(BookRequest bookRequest)
        {
            try
            {
                var book = await _bookInMemoryRepository.GetById(bookRequest.Id);

                if (book == null)
                {
                    return new BookResponse()
                    {
                        Book = book,
                        HttpStatusCode = HttpStatusCode.BadRequest,
                        Message = "Book does not exist"
                    };
                }

                var b = _mapper.Map<Book>(bookRequest);
                var result = await _bookInMemoryRepository.UpdateBook(b);

                return new BookResponse()
                {
                    HttpStatusCode = HttpStatusCode.OK,
                    Book = result,
                    Message = "Successfully updated book"
                };

            }
            catch (Exception e)
            {
                _logger.LogError($"Error when Updating book with Id {bookRequest.Id} : {e}");
                throw new Exception(e.Message); ;
            }
        }
    }
}
