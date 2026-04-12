#!/bin/bash
# Full migration: provision -> deploy site
# Usage: ./scripts/migrate.sh [-v|--verbose] [--skip-provision] [--skip-deploy]

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Default values
VERBOSE=""
SKIP_PROVISION=""
SKIP_DEPLOY=""

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--verbose)
            VERBOSE="--verbose"
            shift
            ;;
        --skip-provision)
            SKIP_PROVISION="--skip-provision"
            shift
            ;;
        --skip-deploy)
            SKIP_DEPLOY="--skip-deploy"
            shift
            ;;
        -h|--help)
            echo "Full migration: provision -> deploy site"
            echo ""
            echo "Usage: $0 [OPTIONS]"
            echo ""
            echo "Options:"
            echo "  -v, --verbose      Enable verbose output"
            echo "  --skip-provision   Skip resource provisioning"
            echo "  --skip-deploy      Skip site deployment"
            echo "  -h, --help         Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

cd "$PROJECT_ROOT"
dotnet run --project cli/CRQCIndex.CLI.fsproj -- migrate $SKIP_PROVISION $SKIP_DEPLOY $VERBOSE
