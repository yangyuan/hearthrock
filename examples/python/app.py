#!flask/bin/python
from flask import Flask, jsonify, request, json
import random

app = Flask(__name__)

@app.route('/')
def index():
    return "Hello, World!"

def random_action(scene):
    cards = all_self_cards(scene);
    if (len(cards) == 0):
        return None
    action = []
    action.append(random.choice(cards))
    return action
	
def all_self_cards(scene):
    cards = []
    for card in scene['Self']['Cards']:
        if card['Cost'] <= scene['Self']['Resources']:
            cards.append(card['RockId'])
    return cards;

def attack_hero(scene):
    cards = all_self_cards(scene);
    action = []
    action.append(scene['Self']['Hero']['RockId'])
    action.append(scene['Opponent']['Hero']['RockId'])
    return action
	
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
    action = random_action(request.json)
    #action = attack_hero(request.json)
    print("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")
    print(json.dumps(action))
    return jsonify(action)
	
@app.route('/trace', methods=['POST'])
def trace():
    print(request.data)
    return ""

if __name__ == '__main__':
    app.run(host="127.0.0.1", port=int("7625"), debug=True)
	