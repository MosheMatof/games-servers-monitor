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
        public int numberOfServers { get; set; }
        public int numberOfGames { get; set; }
        public DateTime startDate { get; set; }
        public int intervalTime { get; set; }
        public bool isStarting { get; set; }
    }
}
