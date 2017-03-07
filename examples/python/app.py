#!flask/bin/python
from flask import Flask, jsonify, request
import random

app = Flask(__name__)

@app.route('/')
def index():
    return "Hello, World!"

def random_action(scene):
    cards = all_self_cards(scene);
    action = {}
    action['Source'] = random.choice(cards)
    action['Targets'] = []
    return action
	
def all_self_cards(scene):
    cards = []
    for card in scene['Self']['Cards']:
        cards.append(card['RockId'])
    return cards;

def attack_hero(scene):
    cards = all_self_cards(scene);
    action = {}
    action['Source'] = scene['Self']['Hero']['RockId']
    action['Targets'] = []
    action['Targets'].append(scene['Opponent']['Hero']['RockId'])
    return action	


@app.route('/action', methods=['POST'])
def action():
    print(request.json)
    action = random_action(request.json)
    #action = attack_hero(request.json)
    print(action)
    return jsonify(action)
	
@app.route('/trace', methods=['POST'])
def trace():
    print(request.data)
    return ""

if __name__ == '__main__':
    app.run(debug=True)
	