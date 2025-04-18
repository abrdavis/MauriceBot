using Discord.WebSocket;
using RavelDev.Discord.Bot.Core.Utility;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace RavelDev.Discord.Bot.Core.Commands.GoogleAi
{
    public class GoogleAiCommand : CustomCommand
    {
        public GoogleAiCommand(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public HttpClient HttpClient { get; }
        public required string ApiKey { get; set; }

        public async override Task ExecuteAsync(SocketUserMessage msg, SocketGuildUser user)
        {
            try
            {
                var messageText = msg.Content;
                if (string.IsNullOrEmpty(ApiKey))
                {
                    Console.WriteLine("Google AI API required for command.");
                    return;
                }

                var requestQuery = messageText.ToLower().Split(CommandPrefix)[1];
                var jsonPostData = new GoogleAiRequest(requestQuery);
                var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-thinking-exp:generateContent?key={ApiKey}";
                using HttpResponseMessage response = await HttpClient.PostAsJsonAsync(requestUrl,
                                                                    jsonPostData);

                response.EnsureSuccessStatusCode();

                var googleAiResponse = await response.Content.ReadFromJsonAsync<GoogleAiResponse>();
                if (googleAiResponse == null || !googleAiResponse.candidates.Any()) return;

                var noResponse = false;
                if (googleAiResponse.candidates[0].content == null)
                {
                    noResponse = true;
                }

                if (!noResponse && googleAiResponse.candidates[0].content.parts.Count == 0)
                {
                    noResponse = true;
                }

                var responseText = string.Empty;
                if (!noResponse)
                {
                    responseText = googleAiResponse.candidates[0].content.parts[0].text;
                    if (string.IsNullOrEmpty(responseText))
                    {
                        noResponse = true;
                    }
                }

                if (noResponse)
                {
                    await msg.Channel.SendMessageAsync($"I'm sure I have no idea.");
                    return;
                }

                if (responseText.Length > MaxDiscordMessageLength)
                {
                    var stringLength = 0;
                    for (int i = 0; i < responseText.Length; i += stringLength)
                    {
                        stringLength = Math.Min(MaxDiscordMessageLength, responseText.Length - i);

                        var msgText = $"{responseText.Substring(i, stringLength)}";
                        var ix = i + stringLength;

                        if (responseText.Length > ix)
                        {
                            var lastNonWordReg = new Regex(RegExPatterns.LastNonWordCharacter, RegexOptions.IgnoreCase);
                            var lastNo = lastNonWordReg.Match(msgText);
                            if (lastNo.Success)
                            {
                                stringLength = lastNo.Groups[0].Index;
                            }
                        }

                        await msg.Channel.SendMessageAsync($"{responseText.Substring(i, stringLength)}");
                    }

                }
                else
                {
                    await msg.Channel.SendMessageAsync($"{responseText}");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

        }

        public override bool IsCommand(string messagePrefix)
        {
            var splitMaurcine = messagePrefix.ToLower().Split(CommandPrefix);
            return (splitMaurcine.Any() && splitMaurcine[0].ToLower() ==string.Empty);
        }
    }
}
