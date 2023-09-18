
## Example: Using .NET Semantic Kernel to call Azure OpenAI
The `consoleChat.js` script dynamically loads the [`Microsoft.SemanticKernel`](https://github.com/microsoft/semantic-kernel) .NET assembly and uses it
to call Azure OpenAI.

This application allows you to have a conversation with Azure OpenAI. It will ask you to enter your question
and return the response. You can continue the conversation by entering another question. When finished, type `goodbye`
to end the conversation.

To run this example, first set the following environment variables, as System variables, referencing your
[Azure OpenAI deployment](https://learn.microsoft.com/en-us/azure/cognitive-services/openai/quickstart):
 - `OPENAI_ENDPOINT`
 - `OPENAI_DEPLOYMENT`
 - `OPENAI_KEY`

Then run the following commands in sequence:

| Command                          | Explanation
|----------------------------------|--------------------------------------------------
| `dotnet build`                   | Install [`SemanticKernel`](https://www.nuget.org/packages/Microsoft.SemanticKernel) nuget packages into the project and generate type definitions.
| `npm install`                    | Install [`node-api-dotnet`](https://www.npmjs.com/package/node-api-dotnet) npm package into the project.
| `npm run build`                  | Transpile the typescript to javascript.
| `node consoleChat.js`            | Run consoleChat JS code that uses the above packages to call the Azure OpenAI service.
