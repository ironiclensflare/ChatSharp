using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatSharp.Events
{
    /// <summary>
    /// Describes a Twitch message we have received.
    /// </summary>
    public class TwitchMessageEventArgs : EventArgs
    {
        /// <summary>
        /// The user's chat color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The user's name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Indicates whether the user is a mod for this channel.
        /// </summary>
        public bool Mod { get; set; }

        /// <summary>
        /// Indicates whether the user is subscribed to this channel.
        /// </summary>
        public bool Subscriber { get; set; }

        /// <summary>
        /// Indicates whether the user has Twitch Turbo.
        /// </summary>
        public bool Turbo { get; set; }

        /// <summary>
        /// The message that was sent.
        /// </summary>
        public string Message { get; set; }

        internal TwitchMessageEventArgs(string rawMessage)
        {
            var parsedResponse = rawMessage.Split(':');
            var rawHeaders = parsedResponse[0];

            // If message contains ':'s we want to preserve them after splitting the raw message.
            if (parsedResponse.Length == 3)
            {
                Message = parsedResponse[2];
            }
            else
            {
                Message = string.Join(":", parsedResponse.Skip(2));
            }

            var headers = ParseTwitchHeaders(rawHeaders);
            Username = headers["display-name"];
            Color = headers["@color"];
            Mod = headers["mod"].Equals("1") || headers["room-id"].Equals(headers["user-id"]);
            Subscriber = headers["subscriber"].Equals("1");
            Turbo = headers["turbo"].Equals("1");
        }

        private Dictionary<string, string> ParseTwitchHeaders(string rawHeaders)
        {
            var headers = new Dictionary<string, string>();

            foreach (var header in rawHeaders.Split(';'))
            {
                var headerParams = header.Split('=');
                var key = headerParams[0];
                var value = headerParams[1];
                headers.Add(key, value);
            }

            return headers;
        }
    }
}
