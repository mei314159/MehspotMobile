using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mehspot.Core.Dto
{

    public class ModelStateDto
    {
        public string Message { get; set; }

        public Dictionary<string, string[]> ModelState { get; set; }
    }
}