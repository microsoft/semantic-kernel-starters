# Semantic Kernel Python Flask Starter

The `sk-python-flask` flask application demonstrates how to execute a semantic function within a Flask backend.
The application can also be used as a [ChatGPT plugin](https://platform.openai.com/docs/plugins/introduction).
If not using the starter as a ChatGPT plugin, you may not need the following files and the app routes that serve them:
`openapi.yaml`, `.well-known/ai-plugin.json`, `logo.png`

## Prerequisites

- [Python](https://www.python.org/downloads/) 3.8 and above
- [Poetry](https://python-poetry.org/) is used for packaging and dependency management
- [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the starter

This starter can be configured in two ways:

- A `.env` file in the project which holds api keys and other secrets and configurations
- Or with HTTP Headers on each request

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api)

### Configure with a .env file

Copy the `.env.example` file to a new file named `.env`. Then, copy those keys into the `.env` file:

```
OPENAI_API_KEY=""
OPENAI_ORG_ID=""
AZURE_OPENAI_DEPLOYMENT_NAME=""
AZURE_OPENAI_ENDPOINT=""
AZURE_OPENAI_API_KEY=""
```

### Configure with HTTP Headers

On each HTTP request, use these headers:

```
"x-ms-sk-completion-model" # e.g. text-davinci-003
"x-ms-sk-completion-endpoint" # e.g. https://my-endpoint.openai.azure.com
"x-ms-sk-completion-backend" # AZURE_OPENAI or OPENAI
"x-ms-sk-completion-key" # Your API key
```

## Running the starter

To run the console application within Visual Studio Code, just hit `F5`.
As configured in `launch.json` and `tasks.json`, Visual Studio Code will run `poetry install` followed by `python -m flask run sk_python_flask/app.py`

A POST endpoint exists at `localhost:5000/skills/{skill_name}/functions/{function_name}`
For example, send a POST request to `localhost:5000/skills/FunSkill/functions/Joke` with the configuration headers
and a JSON request body containing your prompt parameters such as:
`{"input": "time traveling to dinosaur age", "style": "wacky"}`

## Using the starter as a ChatGPT plugin

First, run your Flask app locally.
Then, follow instructions on the [OpenAI website](https://platform.openai.com/docs/plugins/introduction) to install your plugin into ChatGPT.
You may need to join a waitlist for developer access.
