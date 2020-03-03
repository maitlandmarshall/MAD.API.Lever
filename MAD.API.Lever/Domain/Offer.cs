﻿using Newtonsoft.Json;
using System;

namespace MAD.API.Lever.Domain
{
    public class Offer
    {
        public class Field
        {
            public string Text { get; set; }
            public string Identifier { get; set; }
            public string Value { get; set; }
        }

        public string Id { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; }
        public string Creator { get; set; }

        public Field[] Fields { get; set; }

        public bool? Approved { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? ApprovedAt { get; set; }

        public string Posting { get; set; }

        [JsonConverter(typeof(MillisecondEpochConverter))]
        public DateTime? SentAt { get; set; }
    }
}
