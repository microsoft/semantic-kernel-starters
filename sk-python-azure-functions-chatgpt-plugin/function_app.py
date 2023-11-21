import sys
import pathlib
import azure.functions as func
import logging
import asyncio
import semantic_kernel as sk
from semantic_kernel.connectors.ai.open_ai import (
    AzureTextCompletion,
    OpenAITextCompletion,
)
from semantic_kernel.orchestration.context_variables import ContextVariables

useAzureOpenAI = True

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

@app.route(route="skills/{skill_name}/functions/{function_name}")
def execute_semantic_function(req: func.HttpRequest) -> func.HttpResponse:
    # asyncio has problems running in existing event loops on windows and python 3.8 to 3.9.1
    if sys.platform == "win32" and (3, 8, 0) <= sys.version_info < (3, 9, 1):
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())

    skill_name = req.route_params.get('skill_name')
    function_name = req.route_params.get('function_name')

    if not skill_name or not function_name:
        logging.error(f"Skill name: {skill_name} or function name: {function_name} not provided")
        return func.HttpResponse("Please pass skill_name and function_name on the URL", status_code=400)

    kernel = create_kernel()

    skills_directory = "skills"
    try:
        skill = kernel.import_semantic_skill_from_directory(
            skills_directory, skill_name
        )
    except ValueError as e:
        logging.exception(f"Skill {skill_name} not found")
        return func.HttpResponse(f"Skill {skill_name} not found", status_code=404)

    if function_name not in skill:
        logging.error(f"Function {function_name} not found in skill {skill_name}")
        return func.HttpResponse(f"Function {function_name} not found in skill {skill_name}", status_code=404)
    sk_function = skill[function_name]

    req_body = {}
    try:
        req_body = req.get_json()
    except ValueError:
        logging.warning(f"No JSON body provided in request.")

    context_variables = ContextVariables()
    for k, v in req_body.items():
        context_variables[k] = v
    result = sk_function(variables=context_variables)

    logging.info(f"Result: {result}")

    return func.HttpResponse(str(result))

@app.route(route="hello")
def hello(req: func.HttpRequest) -> func.HttpResponse:
    name = req.params.get('name')
    if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

    if name:
        return func.HttpResponse(f"Hello, {name} from a Python native function!")
    else:
        return func.HttpResponse(
             "This Python native function executed successfully. Pass a name in the query string or in the request body for a personalized response.",
             status_code=200
        )

@app.route(".well-known/ai-plugin.json", methods=["GET"])
def get_ai_plugin(req: func.HttpRequest) -> func.HttpResponse:
    with open("./.well-known/ai-plugin.json", "r") as f:
        text = f.read()
        return func.HttpResponse(text, status_code=200, mimetype="text/json")


@app.route("logo.png", methods=["GET"])
def get_logo(req: func.HttpRequest) -> func.HttpResponse:
    file_path = "./logo.png"
    with open(file_path, "rb") as file:
        file_data = file.read()
    
    return func.HttpResponse(file_data, status_code=200, mimetype="image/png")

@app.route("openapi.yaml", methods=["GET"])
def get_openapi(req: func.HttpRequest) -> func.HttpResponse:
    with open("./openapi.yaml", "r") as f:
        text = f.read()
        return func.HttpResponse(text, status_code=200, mimetype="text/yaml")

def create_kernel():
    kernel = sk.Kernel()

    # Configure AI service used by the kernel. Load settings from the .env file.
    if useAzureOpenAI:
        if pathlib.Path(".env").is_file():
            deployment, api_key, endpoint = sk.azure_openai_settings_from_dot_env()
        else:
            # Get from Azure Function settings
            logging.error(".env file not found")
        kernel.add_text_completion_service(
            "dv", AzureTextCompletion(deployment, endpoint, api_key)
        )
    else:
        if pathlib.Path(".env").is_file():
            api_key, org_id = sk.openai_settings_from_dot_env()
        else:
            # Get from Azure Function settings
            logging.error(".env file not found")
        kernel.add_text_completion_service(
            "dv", OpenAITextCompletion("text-davinci-003", api_key, org_id)
        )

    return kernel