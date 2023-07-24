import os
import logging
import semantic_kernel as sk
import semantic_kernel.connectors.ai.open_ai as sk_oai
from semantic_kernel.orchestration.context_variables import ContextVariables

from sk_python_flask_chatgpt_plugin.config import AIService, headers_to_config, dotenv_to_config


SKILLS_DIRECTORY = os.path.join("skills")


def create_kernel_for_request(request_headers, skill_name):
    """
    Creates a kernel for a request.
    :param req: The request.
    :param skills: The skills.
    :param memory_story: The memory story.
    :return: The kernel.
    """
    # Create a kernel.
    kernel = sk.Kernel()
    logging.info(f"Creating kernel and importing skill {skill_name}")

    # Get the API configuration.
    try:
        api_config = headers_to_config(request_headers)
    except ValueError:
        logging.exception("No headers found. Using local .env file for configuration.")
        try:
            api_config = dotenv_to_config()
        except AssertionError:
            logging.exception("No .env file found.")
            return None, ("No valid headers found and no .env file found.", 400)

    try:
        if (
            api_config.serviceid == AIService.OPENAI.value
            or api_config.serviceid == AIService.OPENAI.name
        ):
            # Add an OpenAI backend
            kernel.add_text_completion_service(
                "dv",
                sk_oai.OpenAITextCompletion(
                    model_id=api_config.deployment_model_id,
                    api_key=api_config.key,
                    org_id=api_config.org_id,
                ),
            )
        elif (
            api_config.serviceid == AIService.AZURE_OPENAI.value
            or api_config.serviceid == AIService.AZURE_OPENAI.name
        ):
            # Add an Azure backend
            kernel.add_text_completion_service(
                "dv",
                sk_oai.AzureTextCompletion(
                    deployment_name=api_config.deployment_model_id,
                    api_key=api_config.key,
                    endpoint=api_config.endpoint,
                ),
            )
    except ValueError as e:
        logging.exception(f"Error creating completion service: {e}")
        return None, ("Error creating completion service: {e}", 400)

    try:
        kernel.import_semantic_skill_from_directory(SKILLS_DIRECTORY, skill_name)
    except ValueError as e:
        logging.exception(f"Cannot import skill: {e}")
        return None, (f"Cannot import skill {skill_name}", 404)

    return kernel, None


def create_context_variables_from_request(request) -> sk.ContextVariables:
    """
    Creates context variables from a JSON body.
    :param req_body: The JSON body.
    :return: The context variables.
    """
    req_body = {}
    try:
        req_body = request.get_json()
    except ValueError:
        logging.warning("No JSON body provided in request.")

    context_variables = ContextVariables()
    for k, v in req_body.items():
        context_variables[k] = v
    return context_variables
