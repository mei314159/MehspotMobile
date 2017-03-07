using System.Collections.Generic;
using Newtonsoft.Json;

namespace mehspot.Core.Dto
{

    public class ModelStateDto
    {
        public string Message { get; set; }

        public Dictionary<string, string[]> ModelState { get; set; }
    }
}