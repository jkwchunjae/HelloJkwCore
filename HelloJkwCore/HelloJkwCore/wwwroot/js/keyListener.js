
var handlers = {};

window.keyListener = {
    initialize: function (dotnetHelper) {
        var keydownHandler = function (e) {
            dotnetHelper.invokeMethodAsync('OnKeyDown', e.key);
        };
        var keyupHandler = function (e) {
            dotnetHelper.invokeMethodAsync('OnKeyUp', e.key);
        };
        document.addEventListener('keydown', keydownHandler);
        document.addEventListener('keyup', keyupHandler);
        handlers[dotnetHelper] = {
            keydown: keydownHandler,
            keyup: keyupHandler,
        };
    },
    dispose: function (dotnetHelper) {
        var handler = handlers[dotnetHelper];
        if (handler) {
            document.removeEventListener('keydown', handler.keydown);
            document.removeEventListener('keyup', handler.keyup);
            delete handlers[dotnetHelper];
        }
    }
};
