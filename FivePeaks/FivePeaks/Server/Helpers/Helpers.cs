using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FivePeaks.Shared.Models;

namespace FivePeaks.Server.Helpers
{
    public static class Helpers
    {
        public static LeaderboardItem LeaderboardItemWithoutRouteData(LeaderboardItem s)
        {
            return new LeaderboardItem
            {
                Id = s.Id,
                Direction = s.Direction,
                EntryName = s.EntryName,
                PostedByUserId = s.PostedByUserId,
                FinishTime = s.FinishTime,
                Notes = s.Notes,
                PartySize = s.PartySize,
                PostDate = s.PostDate,
                PostedByUser = s.PostedByUser,
                RouteData = "",
                RouteDataType = s.RouteDataType,
                SeasonComplete = s.SeasonComplete,
                StartTime = s.StartTime,
                Status = s.Status,
                SubmissionNotes = s.SubmissionNotes,
                TrekDate = s.TrekDate,
                TrekkerIDs = s.TrekkerIDs
            };
        }
    }
}
