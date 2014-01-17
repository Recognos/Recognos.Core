namespace Recognos.Core.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security;

    /// <summary>
    /// Exception throw if when the executor finished work there are errors that wore not handeled
    /// </summary>
    [Serializable]
    public class ParallelExecutionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ParallelExecutionException class
        /// </summary>
        public ParallelExecutionException()
        {
            Errors = new TaskErrorEventArgs[] { };
        }

        /// <summary>
        /// Initializes a new instance of the ParallelExecutionException class
        /// </summary>
        /// <param name="message">Exception message</param>
        public ParallelExecutionException(string message)
            : base(message)
        {
            Errors = new TaskErrorEventArgs[] { };
        }

        /// <summary>
        /// Initializes a new instance of the ParallelExecutionException class
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public ParallelExecutionException(string message, Exception innerException)
            : base(message, innerException)
        {
            Errors = new TaskErrorEventArgs[] { };
        }

        /// <summary>
        /// Initializes a new instance of the ParallelExecutionException class
        /// </summary>
        /// <param name="errors">Collection of unhandeled errors</param>
        public ParallelExecutionException(IEnumerable<TaskErrorEventArgs> errors)
        {
            Errors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the ParallelExecutionException class
        /// </summary>
        /// <param name="info">the serialization info</param>
        /// <param name="context">the serialization context</param>
        protected ParallelExecutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the collection of unhandeled errors
        /// </summary>
        public IEnumerable<TaskErrorEventArgs> Errors { get; private set; }

        /// <summary>
        /// Helper method for exception serialization
        /// </summary>
        /// <param name="info">the serialization info</param>
        /// <param name="context">the serialization context</param>
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
