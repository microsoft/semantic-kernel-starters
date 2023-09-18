// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

import "./bin/Microsoft.SemanticKernel.Core.js";
import "./bin/Microsoft.SemanticKernel.Connectors.AI.OpenAI.js";
import dotnet from "node-api-dotnet";
import readline from "node:readline";
import { consoleColours } from "./consoleColours.js";

const SK = dotnet.Microsoft.SemanticKernel;

// The JS marshaller does not yet support extension methods.
const kernelBuilder = SK.OpenAIKernelBuilderExtensions.WithAzureChatCompletionService(
    SK.Kernel.Builder,
    process.env['OPENAI_DEPLOYMENT'] || '',
    process.env['OPENAI_ENDPOINT'] || '',
    process.env['OPENAI_KEY'] || '',
);

const kernel = kernelBuilder
    .Build();

const r1 = readline.createInterface({
    input: process.stdin,
    output: process.stdout,
    prompt: 'SK> Hello. Ask me a question or say goodbye to exit.'
});

let chatHistory: string[] = [];

r1.prompt();
console.log("");
r1.on('line', async (userInput: string) => {
    if (userInput.toLowerCase() === 'goodbye') {
        console.log('SK> Goodbye!');
        process.exit(0);
    } else {
        chatHistory.push(userInput);
        // The JS marshaller does not yet support extension methods.
        const chatFunction = SK.InlineFunctionsDefinitionExtension
            .CreateSemanticFunction(kernel, chatHistory.join(""));

        const reply = await kernel.RunAsync("", [chatFunction]);

        chatHistory.push(`${reply}`);
        console.log(consoleColours.green, `Answer> ${reply}`);
        console.log(consoleColours.white, `SK> Ask me another question or say goodbye to exit.`);
    }
}).on('close', () => {
    console.log(consoleColours.white, 'SK> Goodbye!');
    process.exit(0);
});
