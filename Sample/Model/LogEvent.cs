using System;
using System.ComponentModel.DataAnnotations;

namespace TypedRestSample.Model
{
    /// <summary>
    /// A single log event.
    /// </summary>
    public class LogEvent
    {
        /// <summary>
        /// Indicates when this event occured.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// A human-readable message describing what happened.
        /// </summary>
        [Required]
        public string Message { get; set; }
    }
}