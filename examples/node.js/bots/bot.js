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
		
        return this.create_action(mulligan);
    }

    get_play_action(scene) {
        if (scene['PlayOptions'].length == 0) {
            return null;
        }

        return this.create_action(scene['PlayOptions'][Math.floor(Math.random() * scene['PlayOptions'].length)]);
    }
	
	create_action(objects) {
		let action = {};
		action.Version = 1;
		action.Objects = objects;
		action.Slot = -1;
        return action;
	}
}

var exports = module.exports = {
    RockBot:RockBot
};