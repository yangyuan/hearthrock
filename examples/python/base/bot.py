class RockBotBase:
    def get_mulligan_action(self, scene):
        raise NotImplementedError

    def get_play_action(self, scene):
        raise NotImplementedError
