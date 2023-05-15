# Semantic Kernel Python Hello World Starter

The `sk-python-hello-world` console application demonstrates how to execute a semantic function.

## Prerequisites

- [Python](https://www.python.org/downloads/) 3.8 and above
  - [Poetry](https://python-poetry.org/) is used for packaging and dependency management
  - [Semantic Kernel Tools](https://marketplace.visualstudio.com/items?itemName=ms-semantic-kernel.semantic-kernel)

## Configuring the starter

The starter can be configured with a `.env` file in the project which holds api keys and other secrets and configurations.

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api)

Copy those keys into the `.env` file:

```
OPENAI_API_KEY=""
OPENAI_ORG_ID=""
AZURE_OPENAI_DEPLOYMENT_NAME=""
AZURE_OPENAI_ENDPOINT=""
AZURE_OPENAI_API_KEY=""
```

## Running the starter

To install dependencies and create a virtual environment, run `poetry install`.

To run the console application just hit `F5`. You may need to select the poetry interpreter created by poetry.

To build and run the console application from the terminal use the following commands:

```powershell
poetry install
poetry run python hello_world/main.py
```
