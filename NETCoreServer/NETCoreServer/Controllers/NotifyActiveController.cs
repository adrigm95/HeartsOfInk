using Microsoft.AspNetCore.Mvc;
using NETCoreServer.Data;
using NETCoreServer.InternalLogic;
using NETCoreServer.Models;
using System;

namespace NETCoreServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyActiveController : ControllerBase
    {
        private KeyGenerator keyGenerator = new KeyGenerator();

        [HttpPost]
        public ActionResult<HOIResponseModel<bool>> Post([FromBody] string gamekey)
        {
            HOIResponseModel<bool> response = new HOIResponseModel<bool>();

            try
            {
                response.serviceResponse = GamesData.NotifyActive(gamekey);
                response.internalResultCode = InternalStatusCodes.OKCode;
            }
            catch (Exception ex)
            {
                response.serviceResponse = false;
                response.internalResultCode = InternalStatusCodes.KOCode;
            }

            return response;
        }
    }
}
