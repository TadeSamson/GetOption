using System;
using System.Collections.Generic;
using System.Text;

namespace GetOption.Infrastructure.Implementations
{
    public class MongoConfig
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string DatabaseName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
