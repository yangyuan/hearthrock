import http.server
import json
from base.trace import RockTrace


class RockApiService:
    @classmethod
    def run(cls, bot, host='127.0.0.1', port=7625):
        _bot = bot
        _trace = RockTrace()

        class RockApiRequestHandler(http.server.BaseHTTPRequestHandler):
            def log_message(self, format, *args):
                return

            def do_GET(self):
                self.send_response(200)
                self.end_headers()
                self.wfile.write(bytes("Hearthrock!", 'utf-8'))
                return

            def do_POST(self):
                content_length = int(self.headers['Content-Length'])
                raw_data = self.rfile.read(content_length)
                json_data = json.loads(raw_data)

                if self.path == "/trace":
                    ret = _trace.trace(json_data)
                elif self.path == "/mulligan":
                    ret = _bot.get_mulligan_action(json_data)
                elif self.path == "/play":
                    ret = _bot.get_play_action(json_data)
                self.send_response(200)
                self.end_headers()
                self.wfile.write(bytes(json.dumps(ret), 'utf-8'))
                return

        httpd = http.server.HTTPServer((host, port), RockApiRequestHandler)

        while True:
            httpd.handle_request()
