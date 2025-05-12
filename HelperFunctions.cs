using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;
using Microsoft.SemanticKernel.Agents.Chat;
using Microsoft.SemanticKernel.ChatCompletion;

#pragma warning disable SKEXP0110, SKEXP0001
internal class HelperClass
{
    public static ChatCompletionAgent GetChatCompletionAgent(Kernel kernel, string file, KernelArguments arguments)
    {
        arguments ??= [];
        KernelPromptTemplateFactory factory = new();
        string agentYaml = File.ReadAllText(file);
        
        PromptTemplateConfig config = KernelFunctionYaml.ToPromptTemplateConfig(agentYaml);
        ChatCompletionAgent agent = new(config, factory)
        {
            Kernel = kernel,
            Arguments = arguments
        };
        return agent;
    }

    public static string GetUserInitialChatMessage(string templateFile, string studyMaterialFile, string exampleQuestionsFile)
    {
        string template = File.ReadAllText(templateFile);
        string studyMaterial = File.ReadAllText(studyMaterialFile);
        string exampleQuestions = File.ReadAllText(exampleQuestionsFile);
        return string.Format(template, studyMaterial, exampleQuestions);
    }

    public static KernelFunctionSelectionStrategy GetSelectionStrategy(List<ChatCompletionAgent> groupChat, Kernel kernel, Agent initialAgent)
    {
        string selectionPrompt = File.ReadAllText("./Agents/SelectionStrategyPrompt.txt");
        selectionPrompt = string.Format(selectionPrompt, string.Join("\n", groupChat.Select(x => $"AgentName: {x.Name}; Description: {x.Description}")));
        KernelFunction selectionFunction = AgentGroupChat.CreatePromptFunctionForStrategy(selectionPrompt, null, "history");
        KernelFunctionSelectionStrategy selectionStrategy = new(selectionFunction, kernel)
        {
            ResultParser = (result) => result.GetValue<string>().Split("\"\"\"")?[1] ?? initialAgent.Name,
            HistoryVariableName = "history",
            HistoryReducer = new ChatHistoryTruncationReducer(2),
            InitialAgent = initialAgent
        };
        return selectionStrategy;
    }
}