using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingContext
{
    /// <summary>
    /// General purpose status object for error status and description
    /// </summary>
    public class Status
    {
        /// <summary>
        /// Construct a success status
        /// </summary>
        public static Status OK => new Status();

        /// <summary>
        /// Construct a failure status
        /// </summary>
        public static Status Error => new Status(StatusCode.Failure);

        protected int code;

        // Default constructor set for success
        public Status()
        {
            code = 0;
        }

        // Initialise with error code
        public Status(StatusCode code)
        {
            this.code = (int)code;
        }

        // Initialise with error code
        public Status(uint code)
        {
            this.code = (int)code;
        }

        public Status(Status other)
        {
            code = other.code;
        }

        /// <summary>
        /// Returns true if operation was successful
        /// </summary>
        public bool IsOK => code == 0;

        /// <summary>
        /// Returns true if operation failed
        /// </summary> 
        public bool IsErr => code != 0;

        /// <summary>
        /// Returns the error code
        /// </summary>
        public int ErrorCode => code;

        /// <summary>
        /// Returns the success/failure description
        /// </summary>
        public string Description
        {
            get
            {
                if (Enum.IsDefined(typeof(StatusCode), code))
                    return ((StatusCode)code).Description();

                return "Unknown Status";
            }
        }

        // Implicitly converts error code to status
        public static implicit operator Status(StatusCode err)
        {
            return new Status(err);
        }

        // Implicitly converts error code to int
        public static implicit operator int(Status status)
        {
            return status.code;
        }

        public static bool operator ==(Status s1, Status s2)
        {
            if (ReferenceEquals(s1, null) && ReferenceEquals(s2, null)) return true;
            if (ReferenceEquals(s1, null) || ReferenceEquals(s2, null)) return false;
            return s1.Equals(s2);
        }

        public static bool operator !=(Status status1, Status status2)
        {
            return !(status1 == status2);
        }

        public override bool Equals(object rhs)
        {
            if (rhs is Status r)
                return code == r.code;

            return false;
        }

        public override int GetHashCode()
        {
            return code;
        }

        public override string ToString()
        {
            return code == 0 ? Description : $"StatusCode [{code}], Desc [{Description}]";
        }
    }

}

