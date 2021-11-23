using Microsoft.AspNetCore.Mvc;
using NETCoreServer.Data;
using NETCoreServer.InternalLogic;
using NETCoreServer.Models;
using System;
using System.Linq;

namespace NETCoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private KeyGenerator keyGenerator = new KeyGenerator();

        [HttpGet]
        public ActionResult<HOIResponseModel<BasicGameInfo>> Get(bool isPublic, string gameKey)
        {
            HOIResponseModel<BasicGameInfo> response = new HOIResponseModel<BasicGameInfo>();

            try
            {
                BasicGameInfo game = GamesData.GetGameInfo(isPublic, gameKey);
                response.serviceResponse = game;
                response.internalResultCode = InternalStatusCodes.OKCode;
            }
            catch (Exception ex)
            {
                response.serviceResponse = null;
                response.internalResultCode = InternalStatusCodes.KOCode;
            }

            return response;
        }

        [HttpPost]
        public ActionResult<HOIResponseModel<BasicGameInfo>> Post([FromBody] CreateGameModel requestModel)
        {
            bool isKeyUnique = false;
            int retryCounter = 0;
            string gamekey = string.Empty;
            BasicGameInfo newGame = null;
            HOIResponseModel<BasicGameInfo> response = new HOIResponseModel<BasicGameInfo>();

            try
            {
                newGame = new BasicGameInfo
                {
                    name = requestModel.name,
                    isPublic = requestModel.isPublic,
                    mapId = requestModel.mapId,
                    ping = 34,
                    playersInside = 1,
                    hostIp = Request.HttpContext.Connection.RemoteIpAddress.ToString()
                };

                while (!isKeyUnique)
                {
                    gamekey = keyGenerator.BuildKeyForGame(requestModel.name, retryCounter);

                    isKeyUnique = GamesData.KeyExists(gamekey);
                    retryCounter++;

                    if (retryCounter == 100)
                    {
                        throw new Exception("Too much retrys for generate gameKey");
                    }
                }

                newGame.gameKey = gamekey;
                GamesData.AddGame(newGame);
                response.serviceResponse = newGame;
                response.internalResultCode = InternalStatusCodes.OKCode;
            }
            catch (Exception ex)
            {
                response.serviceResponse = null;
                response.internalResultCode = InternalStatusCodes.KOCode;
            }

            return response;
        }
    }
}
