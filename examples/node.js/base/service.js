const trace = require('./trace')
const http = require('http')

/**
 * Api Service implementation.
 */
class RockApiService {
    constructor(bot, host, port) {
        this.bot = bot;
        this.host = host;
        this.port = port;
    }

    run() {
        let server = http.createServer((request, response) => {
            this.doRequest(request, response);
        });

        server.listen(this.port,this.host, (err) => {
            if (err) {
                throw err;
            }

            console.log(`Serving at http://${this.host}:${this.port}`)
        });
    }

    doGet (request, response) {
        response.end('Hearthrock!')
    }

    doPost (request, response) {
        let ret = "";

        let body = "";
        let bot = this.bot;
        request.on('data', function (chunk) {
            body += chunk;
        });
        request.on('end', function () {
            let json = JSON.parse(body);
            if (request.url == "/trace") {
                ret = trace.RockTrace.trace(json);
            } else if (request.url == "/mulligan") {
                ret = bot.get_mulligan_action(json);
            } else if (request.url == "/play") {
                ret = bot.get_play_action(json);
            } else if (request.url == "/report") {
                ret = bot.report(json);
            }

            response.end(JSON.stringify(ret))
        })
    }

    doRequest (request, response) {
        if (request.method == 'GET') {
            this.doGet (request, response);
        } else {
            this.doPost (request, response);
        }
    }

    static run(bot, host, port) {

        let service = new RockApiService(bot, host, port);

        service.run();
    }

}

var exports = module.exports = {
    RockApiService:RockApiService
};