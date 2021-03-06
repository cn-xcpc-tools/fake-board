﻿namespace Board.Models
{
#pragma warning disable IDE1006
    public class ProblemModel
    {
        public int ordinal { get; set; }
        public string short_name { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public double time_limit { get; set; }
        public string externalid { get; set; }
        public string name { get; set; }
        public string rgb { get; set; }
        public string color { get; set; }
        public int test_data_count { get; set; }
    }
#pragma warning disable
}
