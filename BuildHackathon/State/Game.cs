using System;
using System.Web;

namespace BuildHackathon.State
{
    public class Game
    {
        public Guid ID { get; set; }
        public string QRCodeImage { get; set; }

        public Game()
        {
            this.ID = Guid.NewGuid();

            this.QRCodeImage = string.Format("http://chart.apis.google.com/chart?cht=qr&chs=300x300&chl={0}&chld=H|0)",
                                             HttpUtility.UrlEncode(
                                                 "http://buildhackathon.cloudapp.net/client.html?id=" +
                                                 this.ID.ToString()));
        }
    }
}