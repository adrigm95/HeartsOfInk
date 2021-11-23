using Microsoft.AspNetCore.Mvc;
using NETCoreServer.Data;
using NETCoreServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicGamesController : ControllerBase
    {
        [HttpGet]
        public ActionResult<HOIResponseModel<List<BasicGameInfo>>> Get()
        {
            HOIResponseModel<List<BasicGameInfo>> response = new HOIResponseModel<List<BasicGameInfo>>();

            response.internalResultCode = InternalStatusCodes.OKCode;
            response.serviceResponse = GamesData.GetPublicGames();

            return response;
        }
    }
}
