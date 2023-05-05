import json
import psutil
from datetime import datetime
import random
from quart import Quart, jsonify, request, Response, has_request_context
import asyncio
import logging
import uvicorn

# Set up logging
logging.basicConfig(level=logging.DEBUG, format='%(asctime)s %(levelname)s %(message)s')


app = Quart(__name__)

game_servers = []
is_init = False
is_emulator_running = False
stop_emulator = False
interval = 10

class GameServer:
    def __init__(self, game_id, server_id):
        self.Id = server_id
        self.GameId = game_id
        self.Name = f"Server-{server_id}"
        self.CurrentPlayers = random.randint(0, 100)
        self.PlayersCapacity = random.randint(100, 200)
        self.GameMode = random.choice(["Free for All", "Capture the Flag", "Team Deathmatch"])
        self.HighScore = random.uniform(0.0, 150.0)
        self.AvgScore = random.uniform(0.0, 100.0)
        self.IsRunning = random.choice([True, False])
        self.CpuTemperature = random.uniform(20, 70)
        self.CpuSpeed = psutil.cpu_freq().current
        self.MemoryUsage = psutil.virtual_memory().percent
        self.MemoryCapacity = psutil.virtual_memory().total * 0.000001 # in MB
        self.TimeStamp = datetime.now()

# function to generate ServerUpdate data for a given server ID
def generate_server_update(game_server):
    # log that the function is being called
    logging.info(f"Generating ServerUpdate for server {game_server.get('Id')}")
    
    is_running = random.randint(0, 100) < 90 # 90% chance that the server is running
    if is_running:        
        current_players = random.randint(0, game_server.get('PlayersCapacity'))
        cpu_temperature = random.uniform(20, 70)
        cpu_speed = psutil.cpu_freq().current
        high_score = random.randint(0, 150)
        avg_score = random.uniform(0, 100)
        memory_usage = psutil.virtual_memory().percent
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    else:
        current_players = 0
        cpu_temperature = 0
        cpu_speed = 0
        high_score = 0
        avg_score = 0
        memory_usage = 0
        timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")              

        # create the ServerUpdate dictionary
    server_update = {
        'ServerId': game_server.get('Id'),
        'CurrentPlayers': current_players,
        'IsRunning': is_running,
        'CpuTemperature': cpu_temperature,
        'CpuSpeed': cpu_speed,
        'HighScore': high_score,
        'AvgScore': avg_score,
        'MemoryUsage': memory_usage,
        'TimeStamp': timestamp
    }
    # send the ServerUpdate data to the client    
    return json.dumps(server_update)

# define an async generator function that generates server updates
async def generate_server_updates():
    global stop_emulator
    global interval
    # Reset the stop_emulator variable
    stop_emulator = False
    while not stop_emulator:
        try:
            for game_server in game_servers:
                json_server_update = generate_server_update(game_server)
                sse_message = f"data: {json_server_update}\n\n"
                logging.info("Sending SSE message: %s", sse_message)
                yield sse_message
                await asyncio.sleep(0)
            await asyncio.sleep(interval)
        except asyncio.CancelledError:
            logging.info("Emulator task cancelled")     

async def stream_response(generator):
    async for sse_message in generator:
        yield sse_message
        await asyncio.sleep(0)
