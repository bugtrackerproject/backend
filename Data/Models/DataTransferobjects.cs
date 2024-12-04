using System.Data;

namespace bugtracker_backend_net.Data.DataTransferObjects
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }

}
