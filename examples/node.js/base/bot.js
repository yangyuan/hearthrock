/**
 * Base class for RockBot
 */
class RockBotBase {
    get_mulligan_action(scene) {
        throw new Error("Not Implemented.")
    }

    get_play_action(scene) {
        throw new Error("Not Implemented.")
    }

    report(scene) {
        throw new Error("Not Implemented.")
    }
}

var exports = module.exports = {
    RockBotBase:RockBotBase
};