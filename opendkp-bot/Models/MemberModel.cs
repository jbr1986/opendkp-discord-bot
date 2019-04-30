namespace opendkp_bot.Models
{
    class MemberModel
    {
        public string Name { get; set; }
        public string Rank { get; set; }
        public string Class { get; set; }
        public double DKP_CURRENT { get; set; }
        public double DKP_EARNED { get; set; }
        public double DKP_SPENT { get; set; }
        public string LAST_RAID { get; set; }
        public double DKP_ADJUSTMENT { get; set; }
        public string RA_30Day { get; set; }
        public double RA_30DayPercent { get; set; }

        public string RA_60Day { get; set; }
        public double RA_60DayPercent { get; set; }
        public string RA_90Day { get; set; }
        public double RA_90DayPercent { get; set; }
        public string RA_LifeTime { get; set; }
        public double RA_LifeTimePercent { get; set; }

    }
}
