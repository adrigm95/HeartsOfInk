using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NETCoreServer.Models
{
    public class HOIResponseModel<T>
    {
        public short internalResultCode { get; set; }
        public T serviceResponse { get; set; }
    }
}
