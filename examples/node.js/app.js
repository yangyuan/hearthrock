const service = require('./base/service');
const bot = require('./bots/bot');

service.RockApiService.run(new bot.RockBot(), '127.0.0.1', 7625);