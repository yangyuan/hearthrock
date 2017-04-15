const base = require('../base/bot');

/**
 * Sample Bot implementation.
 * Extending RockBotBase is optional.
 */
class RockBot extends base.RockBotBase {
    get_mulligan_action(scene) {
        let mulligan = [];
        for (let card of scene['Self']['Cards']) {
            if (card['Cost'] > 3) {
                mulligan.push(card['RockId']);
            }
        }
        return mulligan;
    }

    get_play_action(scene) {
        if (scene['PlayOptions'].length == 0) {
            return null;
        }

        return scene['PlayOptions'][Math.floor(Math.random() * scene['PlayOptions'].length)];
    }
}

var exports = module.exports = {
    RockBot:RockBot
};