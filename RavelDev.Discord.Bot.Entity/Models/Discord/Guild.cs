using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Entity.Models.Discord
{
    public class Guild
    {
        [Column(TypeName = "bigint")]
        public long GuildId { get; set; }
        public required string GuildName { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }
}
