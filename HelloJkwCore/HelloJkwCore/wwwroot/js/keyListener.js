
var handlers = {};

window.keyListener = {
    initialize: function (dotnetHelper) {
        var keydownHandler = function (e) {
            var h = handlers[dotnetHelper];
            if (h && h.active) {
                e.preventDefault();
            }
            dotnetHelper.invokeMethodAsync('OnKeyDown', e.key);
        };
        var keyupHandler = function (e) {
            var h = handlers[dotnetHelper];
            if (h && h.active) {
                e.preventDefault();
            }
            dotnetHelper.invokeMethodAsync('OnKeyUp', e.key);
        };
        document.addEventListener('keydown', keydownHandler);
        document.addEventListener('keyup', keyupHandler);
        handlers[dotnetHelper] = {
            keydown: keydownHandler,
            keyup: keyupHandler,
            active: false,
        };
    },
    setActive: function (dotnetHelper, active) {
        var h = handlers[dotnetHelper];
        if (h) {
            h.active = !!active;
        }
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
