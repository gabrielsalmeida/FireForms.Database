using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FireForms.Database
{
    public class FireFormsException : Exception
    {
        public FireFormsException()
        {
        }

        public FireFormsException(string message) : base(message)
        {
        }

        public FireFormsException(string message, Exception innerException) : base(message, innerException)
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
