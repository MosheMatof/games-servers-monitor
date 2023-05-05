import random
from quart import Quart, jsonify, request, Response, has_request_context
import logging
import uvicorn

import emulator_generator

logging.basicConfig(level=logging.DEBUG, format='%(asctime)s %(levelname)s %(message)s')


app = Quart(__name__)
@app.route('/start_emulator', methods=['GET'])
async def start_emulator():
    # Check whether we have an active request context
    if not has_request_context():
        return "No active request context", 400

    #if game_servers list is empty then return error
    if emulator_generator.game_servers:
        get_interval = request.args.get('interval', type=float) / 1000
        emulator_generator.interval = get_interval if get_interval else emulator_generator.interval
        # log that the emulator has started
        logging.info("Emulator started")
        emulator_generator.is_emulator_running = True
        
        return Response(emulator_generator.stream_response(emulator_generator.generate_server_updates()), content_type="text/event-stream")

    else:
        # log that no game servers were found
        logging.error("No game servers found")

        return "No game servers's found! please initialize the emulator first", 400

@app.route('/resume_emulator', methods=['GET'])
async def resume_emulator():
    # Check whether we have an active request context
    if not has_request_context():
        return "No active request context", 400

    #chek if emulator is already running
    if emulator_generator.is_emulator_running:
        return "Emulator is already running", 400
    
    #if game_servers list is empty then return error
    if emulator_generator.game_servers:
        get_interval = request.args.get('interval', type=float) / 1000
        emulator_generator.interval = get_interval if get_interval and get_interval > 0 else emulator_generator.interval        
        # log that the emulator has started
        logging.info("Emulator continued")

        return Response(emulator_generator.stream_response(emulator_generator.generate_server_updates()), content_type="text/event-stream")

    else:
        # log that no game servers were found
        logging.error("No game servers found")

        return "No game servers's found! please initialize the emulator first", 400

@app.route('/stop_emulator', methods=['POST'])
def stop_emulator():
    emulator_generator.stop_emulator = True
    emulator_generator.is_emulator_running = False
    return 'Emulator stopped!', 200

@app.route('/init_emulator', methods=['POST'])
async def generate_game_servers():
    # Check whether we have an active request context
    if not has_request_context():
        return "No active request context", 400

    if emulator_generator.is_init:
        return "Emulator is already initialized", 400

    json_data = await request.get_json()
    n = int(json_data.get("n"))
    game_ids = json_data.get("game_ids")
    if game_ids:
        for i in range(n):
            game_id = random.choice(game_ids)
            game_server = emulator_generator.GameServer(game_id, i+1)
            emulator_generator.game_servers.append(game_server.__dict__)
        emulator_generator.is_init = True
        return jsonify(emulator_generator.game_servers)
    else:
        return "No game ids were found", 400


if __name__ == "__main__":
    uvicorn.run(app, host="0.0.0.0", port=5000)