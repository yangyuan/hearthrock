/**
 * Tracer for RockApiService.
 */
class RockTrace {
    static trace(data) {
        console.log('[' + data['Level'] + "]: " + data['Message']);
    }
}

var exports = module.exports = {
    RockTrace:RockTrace
};