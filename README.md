# AI Multi-Agent Orchestration with Azure OpenAI & Semantic Kernel

This project demonstrates a multi-agent orchestration system using [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel) and Azure OpenAI. The goal is to process study material and generate exam questions through structured agent collaboration.

It was developed as part of my preparation and successful completion of the [**Microsoft Certified: Azure AI Engineer Associate**](https://learn.microsoft.com/en-us/credentials/certifications/azure-ai-engineer/?practice-assessment-type=certification) certification, in which I scored **870/1000**.

[Link to certificate credential](https://learn.microsoft.com/api/credentials/share/en-us/HenriqueLima-4698/494AF595214E052D?sharingId=EAE8279767FDEB3B)

---

## What It Does

This program sets up a team of AI agents that collaborate to:

- Parse input material (e.g., study documents, JSON files)
- Generate exam-style questions
- Supervise and refine the generated output
- Stream their discussion live in the terminal
- Save the full AI conversation to `conversation.txt`

Each agent is defined using YAML-based configurations, making it modular and easy to extend.

In this example, the study material file contains copy pasted material from the [Develop a RAG-based solution with your own data using Azure AI Foundry](https://learn.microsoft.com/en-us/training/modules/build-copilot-ai-studio/) lesson.
You can see the generated conversation and final questions and answers in csv format in `conversation.txt`.
In this case, I exported the csv's to [Flashka.ai](https://flashka.ai), which has a csv import mode to create flashcards.

---

## Technologies Used

- [.NET 8 / C#](https://dotnet.microsoft.com/)
- [Azure OpenAI](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/)
- [Microsoft Semantic Kernel](https://github.com/microsoft/semantic-kernel)

---

## How to Run

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/azure-openai-agent-orchestration.git
cd azure-openai-agent-orchestration
````

### 2. Install Dependencies

Make sure you have [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.

Then install the required NuGet packages:

```bash
dotnet restore
```

### 3. Configure Your API Credentials

Create a file called `appsettings.Development.json` (this is ignored by `.gitignore`) with the following structure:

```json
{
  "Endpoint": "https://your-openai-resource.openai.azure.com/",
  "Key": "your-azure-openai-key",
  "DeploymentName": "your-deployment-name"
}
```

Or add this information to `appsettings.json`.

Although we use the class `AzureOpenAIClient`, we can actually use any deployed LLM in your Azure AI Hub/Project, meaning we can also use other models outside OpenAI, like `DeepSeek-R1`. Most of the questions generated for my study where generated using `DeepSeek-R1`. All you have to do is change the `DeploymentName` setting. 

### 4. Add Your Custom Data

Replace the contents of the files in `/UserMessage/StudyMaterial.txt` with your own study material and exam question data. Just drop everything and the AI will sort it out!

### 5. Run the Program

```bash
dotnet run
```

Watch the agents collaborate in real time. The full conversation will be saved to `conversation.txt`.