using Microsoft.SemanticKernel.ChatCompletion;
using WarehouseFileArchiverAPI.Interfaces;
namespace WarehouseFileArchiverAPI.Services
{

    public class SummarizationService : ISummarizationService
    {
        private readonly IChatCompletionService _chatService;
        private readonly ChatHistory _chatMessages;

        public SummarizationService(IChatCompletionService service)
        {
            _chatService = service;
            _chatMessages =
            [
                new()
                {
                    Role = AuthorRole.System,
                    Content = "You are a Document summarizer, STRICTLY summarizes the given text content from the document precisely, without losing key points and data from the document, Do not deviate from the context of the document. PRODUCE A BEAUTIFULLY FORMATED OUTPUT. Be sure to not ask any questions to the user! this is not a conversation, the user expects ONLY to VIEW the SUMMARY and avoid asking questions at any cost"
                }
,
            ];
        }

        public async Task<string> SummarizeAsync(string text)
        {
            string prompt = $"Summarize the following document content in a concise paragraph:\n\n{text}";

            _chatMessages.AddUserMessage(prompt);
            var response = await _chatService.GetChatMessageContentAsync(_chatMessages);
            return response.Content ?? "";
        }
    }

}