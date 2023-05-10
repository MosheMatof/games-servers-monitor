
import pytest
import emulator_generator

"""
Code Analysis

Objective:
The objective of the 'stop_emulator' function is to stop the emulator by setting the global variables 'stop_emulator' and 'is_emulator_running' to False.

Inputs:
The function does not have any inputs.

Flow:
1. The function sets the global variable 'stop_emulator' to True.
2. The function sets the global variable 'is_emulator_running' to False.
3. The function returns the message 'Emulator stopped!' with a status code of 200.

Outputs:
The main output of the function is the message 'Emulator stopped!' with a status code of 200.

Additional aspects:
The function is a route function that is triggered when a POST request is made to the '/stop_emulator' endpoint. The function relies on the global variables 'stop_emulator' and 'is_emulator_running' that are defined in the 'emulator_generator' module. The function does not take any parameters and does not modify any data outside of the 'emulator_generator' module.
"""

class TestStopEmulator:
    # Tests that sending a POST request to /stop_emulator returns 'Emulator stopped!' with status code 200. 
    def test_stop_emulator_happy(self, client):
        response = client.post('/stop_emulator')
        assert response.status_code == 200
        assert response.data == b'Emulator stopped!'

    # Tests that the function works correctly when the emulator is not running. 
    def test_stop_emulator_edge(self, client):
        emulator_generator.stop_emulator = True
        emulator_generator.is_emulator_running = False
        response = client.post('/stop_emulator')
        assert response.status_code == 200
        assert response.data == b'Emulator stopped!'

    # Tests that the function sets the global variables stop_emulator and is_emulator_running to False. 
    def test_stop_emulator_behavior(self, client):
        emulator_generator.stop_emulator = False
        emulator_generator.is_emulator_running = True
        client.post('/stop_emulator')
        assert emulator_generator.stop_emulator == True
        assert emulator_generator.is_emulator_running == False

    # Tests that the function works correctly when the request method is not POST. 
    def test_stop_emulator_edge3(self, client):
        response = client.get('/stop_emulator')
        assert response.status_code == 405

    # Tests that the function works correctly when the request URL is incorrect. 
    def test_stop_emulator_edge4(self, client):
        response = client.post('/stop')
        assert response.status_code == 404