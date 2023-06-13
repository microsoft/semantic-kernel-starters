import logging
from flask import Flask, request
from semantic_kernel.kernel_exception import KernelException

from sk_python_flask.kernel_utils import (
    create_kernel_for_request,
    create_context_variables_from_request,
)


app = Flask(__name__)


@app.route("/skills/<skill_name>/functions/<function_name>", methods=["POST"])
def execute_semantic_function(skill_name, function_name):
    logging.info(
        f"Received request for skill {skill_name} and function {function_name}"
    )

    try:
        kernel = create_kernel_for_request(request.headers, skill_name)
    except ValueError as e:
        logging.exception(f"Could not create kernel for request.")
        return f"Could not create kernel for request. {e}", 404
    try:
        sk_func = kernel.skills.get_function(skill_name, function_name)
    except KernelException:
        logging.exception(f"Could not find function {function_name} in skill {skill_name}")
        return f"Could not find function {function_name} in skill {skill_name}", 404

    context_variables = create_context_variables_from_request(request)

    result = sk_func(variables=context_variables)

    logging.info(f"Result: {result}")
    return str(result)
