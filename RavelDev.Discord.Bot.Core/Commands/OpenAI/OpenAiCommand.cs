using Discord.WebSocket;
using OpenAI_API;

namespace RavelDev.Discord.Bot.Core.Commands.OpenAI
{
    internal class OpenAiCommand : CustomCommand
    {
        public OpenAiCommand(
            OpenAIAPI openAiApi) 
        {
            OpenAiApi = openAiApi;
        }

        public OpenAIAPI OpenAiApi { get; }

        public async override Task ExecuteAsync(SocketUserMessage msg, SocketGuildUser user)
        {
            var chat = OpenAiApi.Chat.CreateConversation();
            if(chat == null) return;
            var messageText = msg.Content;
            chat.AppendUserInput(messageText);
            var botResponse = string.Empty;

            await chat.StreamResponseFromChatbotAsync(res =>
            {
                botResponse += res;
            });
            await msg.Channel.SendMessageAsync(botResponse);
        }

        public override bool IsCommand(string messagePrefix)
        {
            var splitMaurcine = messagePrefix.ToLower().Split(CommandPrefix);
            return (splitMaurcine.Any() && splitMaurcine[0].ToLower() == CommandPrefix) && messagePrefix.EndsWith("?");
        }
    }
}
