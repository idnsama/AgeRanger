using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AgeRanger.Web.Application.Exceptions
{
    public class UserFriendlyException : Exception
    {
        private UserFriendlyException() { }

        public UserFriendlyException(string Message) : base(Message)
        {
        }
    }
}