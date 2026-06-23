#!/usr/bin/env bash
set -euo pipefail

# Nothing to bootstrap before start; database migrations run on app startup.
exec "$@"