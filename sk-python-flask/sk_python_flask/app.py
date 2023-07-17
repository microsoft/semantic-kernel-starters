import sys
import asyncio
import logging
from flask import Flask, request, Response, send_file
from flask_cors import CORS
from semantic_kernel.kernel_exception import KernelException

from sk_python_flask.kernel_utils import (
    create_kernel_for_request,
    create_context_variables_from_request,
)


app = Flask(__name__)
CORS(app)


@app.route("/skills/<skill_name>/functions/<function_name>", methods=["POST"])
def execute_semantic_function(skill_name, function_name):
    if sys.platform == "win32" and sys.version_info >= (3, 8, 0):
        asyncio.set_event_loop_policy(asyncio.WindowsSelectorEventLoopPolicy())
    logging.info(
        f"Received request for skill {skill_name} and function {function_name}"
    )

    kernel, error = create_kernel_for_request(request.headers, skill_name)
    if error:
        return error
    try:
        sk_func = kernel.skills.get_function(skill_name, function_name)
    except KernelException:
        logging.exception(
            f"Could not find function {function_name} in skill {skill_name}"
        )
        return f"Could not find function {function_name} in skill {skill_name}", 404

    context_variables = create_context_variables_from_request(request)

    result = sk_func(variables=context_variables)

    logging.info(f"Result: {result}")
    return str(result)


@app.route("/.well-known/ai-plugin.json", methods=["GET"])
def get_ai_plugin():
    with open("./.well-known/ai-plugin.json", "r") as f:
        text = f.read()
        return Response(text, status=200, mimetype="text/json")


@app.route("/logo.png")
def get_logo():
    return send_file("../logo.png", mimetype="image/png")


@app.route("/openapi.yaml", methods=["GET"])
def get_openapi():
    with open("./openapi.yaml", "r") as f:
        text = f.read()
        return Response(text, status=200, mimetype="text/yaml")
