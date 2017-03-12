class RockTrace:
    _colored = False

    def trace(self, package):
        if package['Level'] == 'Error':
            self.print_as_error('[' + package['Level'] + "]: " + package['Message'])
        elif package['Level'] == 'Warning':
            self.print_as_warning('[' + package['Level'] + "]: " + package['Message'])
        elif package['Level'] == 'Info':
            self.print_as_info('[' + package['Level'] + "]: " + package['Message'])
        else:
            self.print_as_verbose('[' + package['Level'] + "]: " + package['Message'])

    def print_as_error(self, text):
        if self._colored:
            print('\033[91m' + text + '\033[0m')
        else:
            print(text)

    def print_as_warning(self, text):
        if self._colored:
            print('\033[92m' + text + '\033[0m')
        else:
            print(text)

    def print_as_info(self, text):
        if self._colored:
            print('\033[34m' + text + '\033[0m')
        else:
            print(text)

    def print_as_verbose(self, text):
        if self._colored:
            print('\033[90m' + text + '\033[0m')
        else:
            print(text)
