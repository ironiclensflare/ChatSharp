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

        /// <summary>
        /// Constructs a TwitchMessageEventArgs instance from a raw IRC message.
        /// </summary>
        /// <param name="rawMessage"></param>
        public TwitchMessageEventArgs(string rawMessage)
        {
            var parsedMessage = rawMessage.Split(new[] { " :" }, StringSplitOptions.None);
            var parsedHeaders = TwitchMessageHelpers.ParseTwitchHeaders(parsedMessage[0]);

            // If message contains ':'s we want to preserve them after splitting the raw message.
            Message = string.Join(" :", parsedMessage.Skip(2));
            
            Username = parsedHeaders["display-name"];
            Color = parsedHeaders["color"];
            Mod = parsedHeaders["mod"].Equals("1") || parsedHeaders["room-id"].Equals(parsedHeaders["user-id"]);
            Subscriber = parsedHeaders["subscriber"].Equals("1");
            Turbo = parsedHeaders["turbo"].Equals("1");
        }
    }

    /// <summary>
    /// Describes a Twitch resub notification we have received.
    /// </summary>
    public class TwitchResubEventArgs : EventArgs
    {
        /// <summary>
        /// The user's name.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The number of months the user has been subscribed.
        /// </summary>
        public int Months { get; set; }

        /// <summary>
        /// The message that was sent.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Constructs a TwitchResubEventArgs instance from a raw IRC message.
        /// </summary>
        /// <param name="rawMessage"></param>
        public TwitchResubEventArgs(string rawMessage)
        {
            var parsedMessage = rawMessage.Split(new[] { " :" }, StringSplitOptions.None);
            var parsedHeaders = TwitchMessageHelpers.ParseTwitchHeaders(parsedMessage[0]);

            Username = parsedHeaders["display-name"];
            Months = int.Parse(parsedHeaders["msg-param-months"]);
            Message = string.Join(" :", parsedMessage.Skip(2));
        }
    }

    internal static class TwitchMessageHelpers
    {
        internal static Dictionary<string, string> ParseTwitchHeaders(string rawHeaders)
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
