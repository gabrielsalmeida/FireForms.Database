﻿using System;
using System.Collections;
namespace FireForms.Database.Exceptions
{
    public class PreconditionFailedException : Exception
    {
        public PreconditionFailedException()
        {
        }

        public PreconditionFailedException(string message) : base(message)
        {
        }

        public PreconditionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public override IDictionary Data => base.Data;

        public override string HelpLink { get => base.HelpLink; set => base.HelpLink = value; }

        public override string Message => base.Message;

        public override string Source { get => base.Source; set => base.Source = value; }

        public override string StackTrace => base.StackTrace;



        public override Exception GetBaseException()
        {
            return base.GetBaseException();
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
