using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace FivePeaks.Shared.Models
{
    public class LeaderboardItem
    {

        [PrimaryKey, AutoIncrement]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PostDate")] public DateTime PostDate { get; set; } = DateTime.Now;

        [Column("TrekDate")] public DateTime TrekDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Name of the group or individual who submitted the entry
        /// </summary>
        [Column("EntryName")] public string EntryName { get; set; }
        
        [Column("StartTime")] public DateTime StartTime { get; set; }
        
        [Column("FinishTime")] public DateTime FinishTime { get; set; }
        
        /// <summary>
        /// "How did it go?" Users can write whatever they want
        /// </summary>
        [Column("Notes")] public string Notes { get; set; }

        /// <summary>
        /// Any data to back up the submission, e.g. a link to a Facebook photo album, blog post etc
        /// </summary>
        [Column("SubmissionNotes")] public string SubmissionNotes { get; set; }

        [Column("Direction")] public TrekDirection Direction { get; set; }

        /// <summary>
        /// List of IDs of users who were part of the trek
        /// </summary>
        [Column("TrekkerIDs")]
        public string TrekkerIDs { get; set; }

        [Column("PartySize")] public int PartySize { get; set; }

        /// <summary>
        /// Accepted / Rejected / Needs More Info
        /// </summary>
        [Column("Status")] public LeaderboardEntryState Status { get; set; }

        /// <summary>
        /// GPX or TCX data, as an XML-style string
        /// </summary>
        [Column("RouteData")] public string RouteData { get; set; }

        /// <summary>
        /// ".gpx" or ".tcx"
        /// </summary>
        [Column("RouteDataType")] public string RouteDataType { get; set; }

        [Column("PostedByUserId")] public int PostedByUserId { get; set; }
        
        [Column("SeasonComplete")] public string SeasonComplete { get; set; }


        [Ignore]
        public UserAccount PostedByUser { get; set; }

        /// <summary>
        /// Automatically calculates the season based on the trek date.
        /// </summary>
        [Ignore]
        public string Season => TrekDate.DayOfYear switch
        {
            { } day when day > 79 && day < 172 => "Spring",
            { } day when day > 171 && day < 264 => "Summer",
            { } day when day > 263 && day < 355 => "Autumn",
            { } day when day > 354 || day < 80 => "Winter",
            _ => "Season Unknown"
        };

    }

    public enum LeaderboardEntryState
    {
        Pending,
        Accepted,
        Rejected,
        NeedMoreInfo
    }

    public enum TrekDirection
    {
        Unknown,
        Clockwise,
        Anticlockwise
    }
}
