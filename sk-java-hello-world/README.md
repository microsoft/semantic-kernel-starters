# Semantic Kernel Java Hello World Starter

The `sk-java-hello-world` console application demonstrates how to execute a semantic function.

## Prerequisites

- [Java](https://learn.microsoft.com/java/openjdk/download) 11 or above.
- [Maven](https://maven.apache.org/download.cgi)

## Configuring the starter

The starter can be configured with a `conf.properties` file in the project which holds api keys and other secrets and configurations.

Make sure you have an
[Open AI API Key](https://openai.com/api/) or
[Azure Open AI service key](https://learn.microsoft.com/azure/cognitive-services/openai/quickstart?pivots=rest-api).

Copy the `example.conf.properties` file to a new file named `conf.properties`. Then, copy those keys into the `conf.properties` file.

If you are using Open AI:

```
client.openai.key=""
client.openai.organizationid=""
```

Or, if you are using Azure Open AI:

```
client.azureopenai.key=""
client.azureopenai.endpoint=""
client.azureopenai.deploymentname=""
```

## Running the starter

Run the starter using maven:

```
mvn compile exec:java
```
