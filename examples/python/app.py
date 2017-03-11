#!flask/bin/python
from flask import Flask, jsonify, request, json
import random

app = Flask(__name__)

@app.route('/')
def index():
    return "Hello, World!"

def do_play(scene):
    if (len(scene['PlayOptions']) == 0):
        return None
    return random.choice(scene['PlayOptions'])
	
def do_mulligan(scene):
    mulligan = []
    for card in scene['Self']['Cards']:
        if (card['Cost']>3):
            mulligan.append(card['RockId'])
    return mulligan	

@app.route('/mulligan', methods=['POST'])
def mulligan():
    print(request.json)
    mulligan = do_mulligan(request.json)
    print(mulligan)
    return jsonify(mulligan)

@app.route('/play', methods=['POST'])
def play():
    print(request.json)
    action = do_play(request.json)
    print(action)
    return jsonify(action)
	
@app.route('/trace', methods=['POST'])
def trace():
    print(request.data)
    return ""

if __name__ == '__main__':
    app.run(host="127.0.0.1", port=int("7625"), debug=True)
	