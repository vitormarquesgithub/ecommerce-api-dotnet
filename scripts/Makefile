SHELL := /bin/bash
COMPOSE_FILE := ./docker/docker-compose.yml
SERVICE := api
LOCALHOST := http://localhost:8080/swagger/index.html

.PHONY: help build up down logs clean prune run migrate seed test format

# ──────────────── Docker ────────────────
run:
	@printf "${BLUE}■ Building docker images...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) up --build --force-recreate -d
	@printf "${CYAN}▼ Waiting for services to stabilize ▼${RESET}\n"
	@timeout=60; \
	total=$$timeout; \
	while ! curl -s ${LOCALHOST} >/dev/null; do \
		if [ "$$timeout" -le 0 ]; then \
			printf "\n${RED}✗ Service did not become available in time${RESET}\n"; \
			exit 1; \
		fi; \
		progress=$$(( (total - timeout) * 30 / total )); \
		printf "\r${CYAN}[%-30s] %d%%${RESET}" "$$(printf '=%.0s' $$(seq 1 $$progress))" "$$(( (total - timeout) * 100 / total ))"; \
		timeout=$$((timeout-2)); \
		sleep 2; \
	done
	@printf "\r${CYAN}[==============================] 100%%${RESET}\n"
	@printf "${GREEN}✓ Services are now available at ${LOCALHOST}${RESET}\n"
	
build:
	@printf "${BLUE}■ Building docker images...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) build

up:
	@printf "${GREEN}▶ Starting containers...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) up -d
	@printf "${GREEN}✓ Containers started${RESET}\n"

down:
	@printf "${YELLOW}■ Stopping containers...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) down
	@printf "${YELLOW}✓ Containers stopped${RESET}\n"

restart:
	@printf "${CYAN}↻ Restarting containers...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) restart
	@printf "${CYAN}✓ Containers restarted${RESET}\n"

logs:
	@printf "${BLUE}▼ Showing live logs for $(SERVICE)...${RESET}\n"
	@docker-compose -f $(COMPOSE_FILE) logs --tail=10 $(SERVICE)

clean:
	@printf "${RED}⚠ WARNING: Hard cleaning all Docker resources...${RESET}\n"
	docker-compose -f $(COMPOSE_FILE) down -v --rmi all
	docker system prune -af
	@printf "${RED}✓ All Docker resources cleaned${RESET}\n"
	@printf "${YELLOW}Note: You'll need to rebuild and run migrations after this${RESET}\n"

prune:
	@printf "${MAGENTA}♻ Pruning unused Docker resources...${RESET}\n"
	docker system prune -f
	@printf "${MAGENTA}✓ Unused resources pruned${RESET}\n"

status:
	@printf "${CYAN}■ Container status:${RESET}\n"
	@docker-compose -f $(COMPOSE_FILE) ps

# ──────────────── Help ────────────────
help:
	@printf "${RESET}\n${MAGENTA}Usage: make -f scripts/Makefile <target>${RESET}\n"
	@printf "\n${CYAN}Docker commands:${RESET}\n"
	@printf "  build   build docker images\n"
	@printf "  run     build, start and show last log entry\n"
	@printf "  up      start containers\n"
	@printf "  down    stop and remove containers\n"
	@printf "  logs    show logs for api\n"
	@printf "  clean   stop containers and remove volumes\n"
	@printf "  prune   prune unused docker resources\n\n"
	@printf "${CYAN}Database commands:${RESET}\n"
	@printf "  migrate apply database migrations\n"
	@printf "  seed    seed initial data\n\n"

#Console colors
BLUE := \x1b[34m
GREEN := \x1b[32m
YELLOW := \x1b[33m
RED := \x1b[31m
CYAN := \x1b[36m
MAGENTA := \x1b[35m
WHITE := \x1b[37m
BRIGHT_WHITE := \x1b[97m
DARK_GRAY := \x1b[38;5;238m
RESET := \x1b[0m