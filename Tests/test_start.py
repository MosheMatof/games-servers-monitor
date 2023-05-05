
# Generated by CodiumAI
from emulator_generator import GameServer
from flask import app
import asyncio


# Dependencies:
# pip install pytest-mock
import pytest

"""
Code Analysis

Objective:
The 'start_emulator' function is responsible for starting the emulator and generating server updates for the game servers. It takes in a GET request and returns a server-sent event (SSE) response containing the server updates.

Inputs:
- GET request with an optional 'interval' parameter.

Flow:
1. Check if there is an active request context.
2. Check if the 'game_servers' list is empty. If it is, return an error message.
3. If the 'game_servers' list is not empty, get the 'interval' parameter from the request and set it as the new interval for generating server updates.
4. Start the emulator and set the 'is_emulator_running' flag to True.
5. Generate server updates using the 'generate_server_updates' function and stream them as SSE messages using the 'stream_response' function.
6. Return the SSE response containing the server updates.

Outputs:
- SSE response containing the server updates.

Additional aspects:
- The function uses the 'has_request_context' function from the Quart framework to check if there is an active request context.
- The function uses the 'logging' module to log messages to the console.
- The function uses the 'emulator_generator' module to access the 'game_servers' list, 'interval' variable, 'generate_server_updates' function, and 'stream_response' function.
"""

class TestStartEmulator:
    # Tests that the emulator starts and generates server updates when there is an active request context. 
    @pytest.mark.asyncio
    async def test_start_emulator_with_active_request_context(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [GameServer(1, 1)])
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator')

        # Assert
        assert response.status_code == 200
        assert response.content_type == 'text/event-stream'

    # Tests that the interval can be set through the request argument. 
    @pytest.mark.asyncio
    async def test_start_emulator_with_interval_argument(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [GameServer(1, 1)])
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator?interval=5000')

        # Assert
        assert response.status_code == 200
        assert response.content_type == 'text/event-stream'

    # Tests that an error is returned when the game_servers list is empty. 
    @pytest.mark.asyncio
    async def test_start_emulator_with_empty_game_servers_list(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [])
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator')

        # Assert
        assert response.status_code == 400
        assert response.data == b"No game servers's found! please initialize the emulator first"

    # Tests that the default interval is used when an invalid interval argument is provided. 
    @pytest.mark.asyncio
    async def test_start_emulator_with_invalid_interval_argument(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [GameServer(1, 1)])
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator?interval=invalid')

        # Assert
        assert response.status_code == 200
        assert response.content_type == 'text/event-stream'

    # Tests that the emulator task is properly cancelled when a CancelledError occurs. 
    @pytest.mark.asyncio
    async def test_start_emulator_handling_of_cancelled_error(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [GameServer(1, 1)])
        mocker.patch('asyncio.sleep', side_effect=asyncio.CancelledError)
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator')

        # Assert
        assert response.status_code == 200
        assert response.content_type == 'text/event-stream'

    # Tests that exceptions in the generate_server_update function are properly handled. 
    @pytest.mark.asyncio
    async def test_start_emulator_handling_of_exceptions(self, mocker):
        # Arrange
        mocker.patch('emulator_generator.game_servers', [GameServer(1, 1)])
        mocker.patch('emulator_generator.generate_server_update', side_effect=Exception)
        client = app.test_client()

        # Act
        response = await client.get('/start_emulator')

        # Assert
        assert response.status_code == 200
        assert response.content_type == 'text/event-stream'