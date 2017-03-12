from base import RockBotBase
import random


def do_mulligan(scene):
    mulligan = []
    for card in scene['Self']['Cards']:
        if card['Cost'] > 3:
            mulligan.append(card['RockId'])
    return mulligan


def do_play(scene):
    if len(scene['PlayOptions']) == 0:
        return None
    return random.choice(scene['PlayOptions'])


class RockBot(RockBotBase):
    def get_mulligan_action(self, scene):
        return do_mulligan(scene)

    def get_play_action(self, scene):
        return do_play(scene)
