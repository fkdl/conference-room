﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis.Models;

namespace RightpointLabs.ConferenceRoom.Bot
{
    [Serializable]
    public class RoomBookingCriteria : RoomBaseCriteria
    {
        public RoomBookingCriteria()
        {
        }

        public RoomBookingCriteria(RoomBaseCriteria baseCriteria)
        {
            this.StartTime = baseCriteria.StartTime;
            this.EndTime = baseCriteria.EndTime;
            this.Office = baseCriteria.Office;
        }

        public string Room { get; set; }

        public static IForm<RoomBookingCriteria> BuildForm()
        {
            return new FormBuilder<RoomBookingCriteria>()
                .Message("Let's book a conference room.")
                .AddRemainingFields()
                .Build();
        }

        public override string ToString()
        {
            return $"{this.Room} from {this.StartTime:h:mm tt} to {this.EndTime:h:mm tt}";
        }

        public static RoomBookingCriteria ParseCriteria(LuisResult result)
        {
            var room = result.Entities
                .Where(i => i.Type == "room")
                .Select(i => (string)i.Resolution["value"])
                .FirstOrDefault(i => !string.IsNullOrEmpty(i));
            var timeRange = result.Entities
                .Where(i => i.Type == "builtin.datetimeV2.timerange")
                .SelectMany(i => (List<object>)i.Resolution["values"])
                .Select(i => ParseTimeRange((IDictionary<string, object>)i))
                .FirstOrDefault(i => i.HasValue);
            var time = result.Entities
                .Where(i => i.Type == "builtin.datetimeV2.time")
                .SelectMany(i => (List<object>)i.Resolution["values"])
                .Select(i => ParseTime((IDictionary<string, object>)i))
                .Where(i => i.HasValue)
                .Select(i => i.Value)
                .ToArray();
            var duration = result.Entities
                .Where(i => i.Type == "builtin.datetimeV2.duration")
                .SelectMany(i => (List<object>)i.Resolution["values"])
                .Select(i => ParseDuration((IDictionary<string, object>)i))
                .FirstOrDefault(i => i.HasValue);

            var start = timeRange.HasValue
                ? timeRange.Value.start
                : time.Length >= 2
                    ? time[0]
                    : time.Length == 1 && duration.HasValue
                        ? time[0]
                        : GetAssumedStartTime(DateTime.Now);
            var end = timeRange.HasValue
                ? timeRange.Value.end
                : time.Length >= 2
                    ? time[1]
                    : duration.HasValue
                        ? start.Add(duration.Value)
                        : start.Add(TimeSpan.FromMinutes(30));

            var criteria = new RoomBookingCriteria()
            {
                StartTime = start,
                EndTime = end,
                Room = room,
                Office = RoomSearchCriteria.OfficeOptions.Chicago,
            };
            return criteria;
        }
    }
}