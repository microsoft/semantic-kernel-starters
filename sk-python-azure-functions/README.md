# Semantic Kernel Python Azure Functions Starter

The `sk-python-azure-functions` application demonstrates how to execute a semantic function within an Azure Function.

## Prerequisites

- [Python](https://www.python.org/downloads/) >=3.8 and <3.11
- [Azure Functions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions)
- [Azurite](https://marketplace.visualstudio.com/items?itemName=Azurite.azurite). Run the services from the VS Code status bar.
- [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the starter

The starter can be configured with a `.env` file in the project which holds api keys and other secrets and configurations.

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api)

Copy the `.env.example` file to a new file named `.env`. Then, copy those keys into the `.env` file:

```
OPENAI_API_KEY=""
OPENAI_ORG_ID=""
AZURE_OPENAI_DEPLOYMENT_NAME=""
AZURE_OPENAI_ENDPOINT=""
AZURE_OPENAI_API_KEY=""
```

## Running the starter

To run the console application within Visual Studio Code, run Azurite from the status bar or command pallette, then just hit `F5`.
As configured in `launch.json` and `tasks.json`, Visual Studio Code will create a virtual environment at `.venv` and run `pip install requirements.txt`.

To run from command line, run the following:

```
python -m venv .venv
.venv\Scripts\python -m pip install -r requirements.txt # Location of python within venv depends on OS
.venv\Scripts\activate
func host start
```

## Using the starter

In your browser, visit `http://localhost:7071/api/skills/{skill_name/functions/{function_name}` to execute a skill.
For example, `http://localhost:7071/api/skills/FunSkill/functions/Joke` will execute the example Joke function in the FunSkill skill.

To provide input, send a POST request with a JSON body, e.g. provide this as input to the Joke function:
`{"input": "time traveling to dinosaur age", "style": "deadpan"}`
