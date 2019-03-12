using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingContext
{
    public enum StatusCode
    {
        //
        // generic success/failure codes
        //

        Success = 0,
        Failure = 1,

        // Todo, assign more error codes for various services.

    }

    public static class EnumExtensions
    {
        public static string Description(this StatusCode status)
        {
            switch (status)
            {
                case StatusCode.Success:
                    return "The operation was successful";
                case StatusCode.Failure:
                    return "The operation failed";
                default:
                    return "Unknown error code";
            }
        }
    }
}
