using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FireForms.Database.Exceptions
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

        public static Exception from (HttpStatusCode statusCode) 
        {
            switch (statusCode)
            {
                case System.Net.HttpStatusCode.BadRequest:
                    return new BadRequestException();

                case System.Net.HttpStatusCode.Unauthorized:
                    return new UnauthorizedException();

                case System.Net.HttpStatusCode.NotFound:
                    return new NotFoundException("Not found or no internet access(No cached instance found too)");

                case System.Net.HttpStatusCode.InternalServerError:
                    return new InternalServerErrorException();

                case System.Net.HttpStatusCode.ServiceUnavailable:
                    return new ServiceUnavailableException();

                case System.Net.HttpStatusCode.PreconditionFailed:
                    return new PreconditionFailedException();
                default:
                    return new Exception();
            }
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
