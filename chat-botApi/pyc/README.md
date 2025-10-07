# EXE101 MVP - Setup Guide

This guide provides step-by-step instructions to set up and run the EXE101 MVP application.

## Prerequisites

- Python 3.13 or newer
- Git
- Internet connection for package downloads

## Step 1: Install UV and Set Environment Path

[UV](https://github.com/astral-sh/uv) is a fast Python package installer and resolver.

### Windows
```powershell
# Install UV
powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"

```

### macOS/Linux
```bash
# Install UV
pip install uv

```

## Step 2: Clone the Repository

```bash
git clone https://github.com/Kpoi148/exe101_mvp.git
```

## Step 3: Navigate to Repository Directory

```bash
cd exe101-mvp
```

## Step 4: Synchronize Dependencies with UV

```bash
uv sync
```
## Step 5: Open VSCode

```bash
code .
```
## Step 6: Navigate to pyc dir
```bash
cd pyc
```
## Step 7: Activate the virtual environment:

### Windows
```powershell
.\.venv\Scripts\activate
```

### macOS/Linux
```bash
source .venv/bin/activate
```

## Step 8: Configure API Keys

Edit the `.env` file in the project directory:

```
CEREBRAS_API_KEY = your_cerebras_api_key_here
GOOGLE_API_KEY = your_google_api_key_here
```

## Step 9: Run the Application

```bash
python -m uvicorn pyc.app:app --reload
```

The application will be available at `http://localhost:8000`

## Step 10: Go Live

For production deployment:

1. Stop the development server
2. Run the application without the `--reload` flag:
   ```bash
   python -m uvicorn pyc.app:app --host 0.0.0.0 --port 8000
   ```

## API Endpoints

- `GET /`: Root endpoint with API status
- `GET /health`: Health check endpoint
- `POST /upload`: Upload and process PDF documents
- `POST /query`: Query the RAG system
- `GET /metrics`: Get system performance metrics
- `POST /optimize-memory`: Force memory optimization
- `GET /documents/status`: Check status of processed documents
- `POST /chat`: Simplified chat endpoint for Node.js integration
