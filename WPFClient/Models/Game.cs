using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.BE;
using WPFClient.Contracts.Models;

namespace WPFClient.Models
{
    public class Game : IGame
    {
        public List<BEGame> Games { get; set; } = new List<BEGame>();
    }
}
