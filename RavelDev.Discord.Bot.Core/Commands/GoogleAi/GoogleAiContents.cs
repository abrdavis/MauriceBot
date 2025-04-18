using System.IO;

namespace RavelDev.Discord.Bot.Core.Commands.GoogleAi
{
    public class GoogleAiRequest
    {
        public GoogleAiContents contents { get; set; }
        public GoogleAiRequest(string requestQuery)
        {
            contents = new GoogleAiContents(requestQuery);
        }
    }
    public class GoogleAiContents
    {
        public GoogleAiContents(string requestQuery)
        {
            parts = new GoogleAiRequestParts(requestQuery);
        }

        public GoogleAiRequestParts parts { get; set; }
    }
    public class GoogleAiRequestParts
    {
        public GoogleAiRequestParts(string requestQuery)
        {
            text = requestQuery;
        }

        public string text { get; set; }
    }
}