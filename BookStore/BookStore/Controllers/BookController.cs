﻿using AutoMapper;
using BookStore.BL.Interfaces;
using BookStore.BL.Services;
using BookStore.Models.MediatR.Commands;
using BookStore.Models.Models;
using BookStore.Models.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BookController> _logger;
        private readonly IMediator _mediator;

        private static readonly List<Book> _testModels = new List<Book>()
     {
        new Book()
        {
            Id = 1,
            Title = "Book 1",
            AuthorId = 1
        },
        new Book()
        {
            Id = 2,
            Title = "Book2",
            AuthorId = 2
        } };

        public BookController(ILogger<BookController> logger, IBookService bookService, IMediator mediator )
        {
            _logger = logger;
            _bookService = bookService;
            _mediator = mediator;
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("Get all")]
        public async Task<IActionResult> Get()
        {
            throw new Exception("Test exception");

            var result = await _mediator.Send(new GetAllBooksCommand());
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet(nameof(GetById))]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest($"Parameter id:{id} must be greater than 0");
            var result = await _mediator.Send(new GetByIdCommand(id));
            if (result == null) return NotFound(id);
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("Add book")]
        public async Task<IActionResult> AddBook([FromBody] BookRequest bookRequest)
        {
            var result = await _mediator.Send( new AddBookCommand(bookRequest));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("Update book")]
        public async Task<IActionResult> Update(BookRequest bookRequest)
        {
            var result = await _mediator.Send(new UpdateBookCommand(bookRequest));

            if (result.HttpStatusCode == HttpStatusCode.BadRequest)
                return BadRequest(result);

            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("Delete book")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0 && _bookService.GetById != null)
            {
                return Ok(await _mediator.Send(new DeleteBookCommand(id)));
            }
            return BadRequest();
        }

    }
}
