using System;

namespace Лаба3
{
    public class Game
    {
        public DateTime Date { get; set; }
        public TimeOnly EndTime { get; set; }
        public int Stadium { get; set; }
        public string Weather { get; set; }

        public string HomeTeam { get; set; }
        public string GuestTeam { get; set; }
        public string Referee { get; set; }
        public string Result { get; set; }

        public int Visitors { get; set; }
        public double TicketPrice { get; set; }  
    }
}
