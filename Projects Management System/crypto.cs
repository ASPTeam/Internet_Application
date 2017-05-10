using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Projects_Management_System
{
    public static class crypto
    {
        public static String Hash(string value)
        {
            return Convert.ToBase64String(System.Security.Cryptography.SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}