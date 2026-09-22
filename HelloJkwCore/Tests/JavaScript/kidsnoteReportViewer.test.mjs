import assert from 'node:assert/strict';
import { readFile } from 'node:fs/promises';
import { test } from 'node:test';

const source = await readFile(new URL('../../ProjectKidsnote/wwwroot/js/kidsnoteReportViewer.js', import.meta.url), 'utf8');
const { initialize } = await import(`data:text/javascript;base64,${Buffer.from(source).toString('base64')}`);

test('photo pinch claims native gesture, survives same-photo rendering and resets on photo change', async () => {
    const listeners = new Map();
    let onMutation;
    let navigations = 0;
    let src = 'first.jpg';
    const image = {
        naturalWidth: 400, naturalHeight: 800, clientWidth: 400, clientHeight: 800,
        style: {}, getAttribute: () => src,
        parentElement: { getBoundingClientRect: () => ({ left: 0, top: 0, width: 400, height: 800 }) },
    };
    const root = {
        dataset: { itemKind: 'photo' }, focus() {},
        querySelector: selector => selector.includes('image') ? image : null,
        addEventListener: (name, callback, options) => listeners.set(name, { callback, options }),
        removeEventListener: name => listeners.delete(name),
    };
    globalThis.document = { body: { style: {} }, documentElement: { style: {} } };
    globalThis.window = { addEventListener() {}, removeEventListener() {} };
    globalThis.MutationObserver = class {
        constructor(callback) { onMutation = callback; }
        observe() {} disconnect() {}
    };
    const viewer = initialize(root, { invokeMethodAsync: async () => { navigations++; } });
    const point = (clientX, clientY) => ({ clientX, clientY });
    const fire = (name, touches, changedTouches = [], target = null) => {
        let prevented = false;
        listeners.get(name).callback({ touches, changedTouches, target, cancelable: true,
            preventDefault() { prevented = true; } });
        return prevented;
    };

    assert.equal(listeners.get('touchstart').options.passive, false);
    fire('touchstart', [point(150, 400)]);
    assert.equal(fire('touchstart', [point(150, 400), point(250, 400)]), true);
    fire('touchmove', [point(100, 400), point(300, 400)]);
    assert.equal(image.style.transform, 'translate(0px, 0px) scale(2)');
    onMutation(); // A DOM update retaining the same photo must not interrupt the pinch.
    assert.equal(image.style.transform, 'translate(0px, 0px) scale(2)');
    fire('touchmove', [point(50, 400), point(350, 400)]);
    assert.equal(image.style.transform, 'translate(0px, 0px) scale(3)');
    fire('touchend', [point(50, 400)], [point(350, 400)]);
    fire('touchmove', [point(100, 450)]);
    assert.equal(image.style.transform, 'translate(50px, 50px) scale(3)');
    fire('touchend', [], [point(100, 450)]);
    assert.equal(navigations, 0);

    // Native button clicks must survive touchstart while the image is zoomed.
    const button = { closest: selector => selector === 'button' ? button : null };
    assert.equal(fire('touchstart', [point(350, 40)], [], button), false);
    assert.equal(fire('touchmove', [point(350, 42)]), false);
    fire('touchend', [], [point(350, 42)]);
    assert.equal(navigations, 0);

    fire('touchstart', [point(50, 400), point(350, 400)]);
    fire('touchmove', [point(150, 400), point(250, 400)]);
    assert.equal(image.style.transform, 'translate(0px, 0px) scale(1)');
    fire('touchend', [], [point(150, 400), point(250, 400)]);
    assert.equal(navigations, 0);
    fire('touchstart', [point(200, 500)]);
    fire('touchend', [], [point(200, 400)]);
    assert.equal(navigations, 1);

    fire('touchstart', [point(150, 400), point(250, 400)]);
    fire('touchmove', [point(100, 400), point(300, 400)]);
    src = 'second.jpg';
    onMutation();
    assert.equal(image.style.transform, 'translate(0px, 0px) scale(1)');
    fire('touchcancel', []);
    fire('touchend', [], [point(100, 500)]);
    assert.equal(navigations, 1);

    root.dataset.itemKind = 'text';
    assert.equal(fire('touchstart', [point(150, 400), point(250, 400)]), false);
    viewer.dispose();
    assert.equal(listeners.size, 0);
});
