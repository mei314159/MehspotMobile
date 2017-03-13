using System;
namespace Mehspot.Core.DTO
{

    public class ApplicationUserBaseDto
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string ProfilePicturePath { get; set; }

        public bool IsElite { get; set; }
    }
}
