#!/bin/bash
# Deploy CRQC Index site to Cloudflare Pages
# Automatically builds if source has changed since last deploy.
# Usage: ./scripts/deploy-pages.sh [-f|--force] [-s|--skip-build] [-v|--verbose]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Default values
SITE_DIR=""
PROJECT_NAME=""
FORCE=""
SKIP_BUILD=""
VERBOSE=""

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -d|--site-dir)
            SITE_DIR="--site-dir $2"
            shift 2
            ;;
        -n|--project-name)
            PROJECT_NAME="--project-name $2"
            shift 2
            ;;
        -f|--force)
            FORCE="--force"
            shift
            ;;
        -s|--skip-build)
            SKIP_BUILD="--skip-build"
            shift
            ;;
        -v|--verbose)
            VERBOSE="--verbose"
            shift
            ;;
        -h|--help)
            echo "Deploy CRQC Index site to Cloudflare Pages"
            echo ""
            echo "Automatically detects changes since last deploy."
            echo "Runs Fable + Vite build, then uploads to Cloudflare."
            echo ""
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -d, --site-dir DIR         Site directory (default: ./site)"
            echo "  -n, --project-name NAME    Pages project name (default: crqcindex)"
            echo "  -f, --force                Deploy even if no changes detected"
            echo "  -s, --skip-build           Skip build, deploy existing dist/"
            echo "  -v, --verbose              Enable verbose output"
            echo "  -h, --help                 Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

cd "$PROJECT_ROOT"
dotnet run --project cli/CRQCIndex.CLI.fsproj -- deploy-pages $SITE_DIR $PROJECT_NAME $FORCE $SKIP_BUILD $VERBOSE
