﻿using GameGuidanceAPI.Context;
using GameGuidanceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestSharp;
using static GameGuidanceAPI.Helpers.IgdbTokens;

namespace GameGuidanceAPI.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class UserFavoriteController : ControllerBase
    {

        private readonly GameGuidanceDBContext _authContext;

        public UserFavoriteController(GameGuidanceDBContext gameGuidanceDBContext)
        {
            _authContext = gameGuidanceDBContext;
        }

        [HttpPost("addfavorite"), Authorize]
        public async Task<IActionResult> Post(int gameId)
        {
            var authHeader = Request.Headers["Authorization"];
            var tokenString = authHeader.ToString().Split(" ")[1];
            User user = _authContext.Users.Where(u => u.Token == tokenString).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }

            var client = new RestClient(GetBaseUrl());
            RestClientOptions options = new RestClientOptions(GetBaseUrl())
            {
                ThrowOnAnyError = true,
                MaxTimeout = -1
            };
            var request = new RestRequest(GetBaseUrl(), Method.Post);
            request.AddHeader("Client-ID", GetClientID());
            request.AddHeader("Authorization", GetBearer());
            request.AddHeader("Content-Type", "text/plain");
            var body = $"fields *;where id = {gameId};";
            request.AddParameter("text/plain", body, ParameterType.RequestBody);
            RestResponse response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK && response.Content != null)
            {
                List<JsonDeserializer> myDeserializedClass = JsonConvert.DeserializeObject<List<JsonDeserializer>>(response.Content);

                if (myDeserializedClass.Count != null)
                {
                    JsonDeserializer myGame = myDeserializedClass[0];

                    UserFavorite userFavorite = new()
                    {
                        UserId = user.Id,
                        GameId = myGame.id.Value,
                        Name = myGame.name,
                        Summary = myGame.summary,
                    };

                    if (await CheckUserAlreadyFavoritedAsync(userFavorite.UserId, userFavorite.GameId))
                        return Ok(new { message = "Favorite Already Exists!" });

                    await _authContext.UserFavorites.AddAsync(userFavorite);
                    await _authContext.SaveChangesAsync();

                    return Ok(new { Message = $"{myGame.name} added to favorites." });
                }
            }
            return BadRequest(new { Message = "Something went wrong. sucker" });
        }

        [HttpDelete("removefavorite"), Authorize]
        public async Task<IActionResult> Delete(int gameId)
        {
            var authHeader = Request.Headers["Authorization"];
            var tokenString = authHeader.ToString().Split(" ")[1];
            User user = _authContext.Users.Where(u => u.Token == tokenString).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }

            if (await CheckUserAlreadyFavoritedAsync(user.Id, gameId))
            {
                UserFavorite userFavorite = await _authContext.UserFavorites.Where(u => u.UserId == user.Id && u.GameId == gameId).FirstOrDefaultAsync();
                _authContext.UserFavorites.Remove(userFavorite);
                await _authContext.SaveChangesAsync();
                return Ok(new { Message = "Favorite Removed!" });
            }
            return BadRequest();
        }

        [HttpGet("getfavorites"), Authorize]
        public async Task<IActionResult> Get()
        {
            var authHeader = Request.Headers["Authorization"];
            var tokenString = authHeader.ToString().Split(" ")[1];
            User user = _authContext.Users.Where(u => u.Token == tokenString).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }

            var userFavorites = await _authContext.UserFavorites.Where(u => u.UserId == user.Id).ToListAsync();
            return Ok(userFavorites);
        }

        [HttpPut("updaterating"), Authorize]
        public async Task<IActionResult> Put(int gameId, int rating)
        {
            var authHeader = Request.Headers["Authorization"];
            var tokenString = authHeader.ToString().Split(" ")[1];
            User user = _authContext.Users.Where(u => u.Token == tokenString).FirstOrDefault();
            if (user == null)
            {
                return Unauthorized();
            }
            if (await CheckUserAlreadyFavoritedAsync(user.Id, gameId))
            {
                UserFavorite userFavorite = await _authContext.UserFavorites.Where(u => u.UserId == user.Id && u.GameId == gameId).FirstOrDefaultAsync();
                userFavorite.Rating = rating;
                await _authContext.SaveChangesAsync();
                return Ok(new { Message = "Rating Updated!" });
            }
            return BadRequest();
        }

        private async Task<bool> CheckUserAlreadyFavoritedAsync(int userId, int gameId)
           => await _authContext.UserFavorites.AnyAsync(x => x.UserId == userId && x.GameId == gameId);
    }
}
