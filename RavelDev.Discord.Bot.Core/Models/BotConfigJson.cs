using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Models
{
    public class BotConfigJson
    {

        public BotConfigJson()
        {
            AuthorizedUsersOnly = false;
        }
        public bool AuthorizedUsersOnly { get; set; }
    }
}
