using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Threading.Tasks;
using TicTacToe.Core.Configuration;
using TicTacToe.Core.DTOs;
using TicTacToe.Core.Exceptions;
using TicTacToe.Core.Exceptions.Game;
using TicTacToe.Core.Interfaces.Services;

namespace TicTacToe.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService )
        {
            _gameService = gameService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameAsync(CreateGameDTO dto, CancellationToken cancellationToken=default)
        {

            GameDTO game = await _gameService.CreateGameAsync(dto, cancellationToken);

            return Created(
        uri: $"/api/games/{game.Id}",
        value: new
        {
            status = StatusCodes.Status201Created,
            title = "Game created",
            game = game
        });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameAsync(Guid id,CancellationToken cancellationToken)
        {
            try
            {
                var game = await _gameService.GetGameAsync(id, cancellationToken);
                Response.Headers.ETag = game.Etag;
                return Ok(game);
            }
            catch (GameNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("{id}/moves")]
        public async Task<IActionResult> MakeMoveAsync(Guid id, [FromBody] MakeMoveDTO request, [FromHeader(Name = "If-Match")] string ifMatchHeader, CancellationToken cancellationToken)
        {
            
            try
            {
                (MoveResultDTO? result, string etag) = await _gameService.MakeMoveAsync(id, request, ifMatchHeader, cancellationToken);

                Response.Headers.ETag = etag;
                return Ok((result,etag));
            }
            catch (ConcurrencyException)
            {
                return Ok(ifMatchHeader);
            }
            catch (GameNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidMoveException)
            {
                return BadRequest();
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            
        }
        

    }
}