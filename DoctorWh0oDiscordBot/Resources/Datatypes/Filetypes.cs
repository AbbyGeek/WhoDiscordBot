﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DoctorWh0oDiscordBot.Resources.Datatypes
{
    public class Setting
    {
        public string token { get; set; }
        public ulong owner { get; set;}
        public List<ulong> log { get; set; }
        public string version { get; set; }
        public List<ulong> banned { get; set; }
    }
}
