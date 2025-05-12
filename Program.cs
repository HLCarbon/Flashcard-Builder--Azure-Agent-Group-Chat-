using Azure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Agents.Chat;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using System.Text;
#pragma warning disable SKEXP0110
#pragma warning disable SKEXP0001

internal class Program
{
    private static async Task Main(string[] args)
    {
        string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{environment}.json", true)
        .Build();

        var endpoint = new Uri(configuration["Endpoint"]);
        var credential = new AzureKeyCredential(configuration["Key"]);
        var model = configuration["DeploymentName"];

        AzureOpenAIClient openAIClient = new(endpoint, credential);
        IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddAzureOpenAIChatCompletion(model, openAIClient);
        Kernel kernel = kernelBuilder.Build();

        ChatCompletionAgent csvParser = HelperClass.GetChatCompletionAgent(kernel, "./Agents/CSVParser.yaml", []);
        ChatCompletionAgent questionsBuilder = HelperClass.GetChatCompletionAgent(kernel, "./Agents/QuestionsBuilder.yaml", []);
        ChatCompletionAgent questionsSupervisor = HelperClass.GetChatCompletionAgent(kernel, "./Agents/QuestionsSupervisor.yaml", []);

        List<ChatCompletionAgent> agentsList = [csvParser, questionsBuilder, questionsSupervisor];
        SelectionStrategy selectionStrategy = HelperClass.GetSelectionStrategy(agentsList, kernel, questionsBuilder);
        RegexTerminationStrategy regexTerminationStrategy = new(["CSV_FINISHED"]);

        AgentGroupChat chat = new(csvParser, questionsBuilder, questionsSupervisor)
        {
            ExecutionSettings = new()
            {
                TerminationStrategy = regexTerminationStrategy,
                SelectionStrategy = selectionStrategy
            }
        };

        string userMessage = HelperClass.GetUserInitialChatMessage("./UserMessage/MessageTemplate.txt", "./UserMessage/StudyMaterial.txt", "./UserMessage/reworked_exam_question.json");
        chat.AddChatMessage(new ChatMessageContent(AuthorRole.User, userMessage));
        Console.WriteLine(userMessage);



        var conversationHistory = new StringBuilder();
        var author = "assistant";
        await foreach (StreamingChatMessageContent content in chat.InvokeStreamingAsync())
        {

            if (author != content.AuthorName)
            {
                author = content.AuthorName;
                conversationHistory.AppendLine($"# {author}:\n");
                Console.Write($"\n----------{author}----------\n");
            }
            var currentContent = new StringBuilder();

            // Stream individual content chunks
            foreach (var chunk in content.Items)
            {
                if (chunk is null) continue;

                Console.Write(chunk.ToString());
                currentContent.Append(chunk.ToString());
            }

            conversationHistory.Append($"{currentContent}");
        }

        File.WriteAllText(Path.Combine(".", "conversation.txt"), conversationHistory.ToString());
    }
}