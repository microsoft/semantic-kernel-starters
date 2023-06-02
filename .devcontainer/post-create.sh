#!/usr/bin/env bash
pwd

# loop through all the python directories and install the dependencies
for d in */ ; do
    echo "Installing dependencies for $d"
    cd $d

    # Check for a requirements.txt file
    if [ -f "requirements.txt" ]; then

        # Create a virtual environment
        python -m venv .venv

        # Activate the virtual environment
        source .venv/bin/activate

        pip install -r requirements.txt

        # Deactivate the virtual environment
        deactivate
    fi

    # check for a poetry.lock file
    if [ -f "poetry.lock" ]; then
        poetry install
    fi

    cd ..
done

# Restore the dotnet projects
dotnet restore
