package com.microsoft.semantickernel.helloworld;

import com.azure.ai.openai.OpenAIAsyncClient;
import com.microsoft.semantickernel.Kernel;
import com.microsoft.semantickernel.SKBuilders;
import com.microsoft.semantickernel.connectors.ai.openai.util.OpenAIClientProvider;
import com.microsoft.semantickernel.exceptions.ConfigurationException;
import com.microsoft.semantickernel.orchestration.SKContext;
import com.microsoft.semantickernel.textcompletion.CompletionSKFunction;
import reactor.core.publisher.Mono;

public class Main {
    public static void main(String[] args) throws ConfigurationException {
        // Configure OpenAI client. Load settings from conf.properties
        OpenAIAsyncClient client = OpenAIClientProvider.getClient();

        // Build Kernel with Text Completion service
        Kernel kernel = SKBuilders.kernel()
                        .withDefaultAIService(SKBuilders.textCompletion()
                                .withOpenAIClient(client)
                                .withModelId("text-davinci-003")
                                .build())
                        .build();

        // Import semantic skill
        kernel.importSkillFromResources("skills", "FunSkill", "Joke");

        CompletionSKFunction joke = (CompletionSKFunction) kernel.getFunction("FunSkill", "Joke");
        // Set input variable for Joke function
        SKContext context = SKBuilders.context().build()
                .setVariable("input", "Time travel to dinosaur age")
                .setVariable("style", "Wacky");

        // Invoke function and get result
        Mono<SKContext> result = joke.invokeAsync(context);

        System.out.println(result.block().getResult());
    }
}