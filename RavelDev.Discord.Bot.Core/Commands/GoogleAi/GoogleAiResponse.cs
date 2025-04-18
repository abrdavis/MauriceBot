using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RavelDev.Discord.Bot.Core.Commands.GoogleAi
{

//    {
//  "candidates": [
//    {
//      "content": {
//        "parts": [
//          {
//            "text": "Not much, just hanging out and ready to answer your questions!  What's up with you?\n"
//          }
//        ],
//        "role": "model"
//      },
//      "finishReason": "STOP",
//      "avgLogprobs": -0.010584558952938427
//    }
//  ],
//  "usageMetadata": {
//    "promptTokenCount": 6,
//    "candidatesTokenCount": 22,
//    "totalTokenCount": 28
//  },
//  "modelVersion": "gemini-1.5-flash-002"
//}

    internal class GoogleAiResponse
    {
    public List<GoogleAiCanidate> candidates { get; set; }
    }

        public class GoogleAiCanidate
        {
        public GoogleAiResponseContent content { get; set; }
        }

    public class GoogleAiResponseContent
    {
        public List<GoogleAiResponsePart> parts { get; set; }
    }

    public class GoogleAiResponsePart
    {
        public string text { get; set; }
    }
}

