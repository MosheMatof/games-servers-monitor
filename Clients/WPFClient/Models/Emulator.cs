using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient.Models
{
    public class Emulator
    {
        public int NumberOfServers { get; set; }
        public int NumberOfGames { get; set; }
        public DateTime StartDate { get; set; }
        public int IntervalTime { get; set; }
        public bool IsRunning { get; set; }
        public bool IsInit { get; set; }
    }
}
