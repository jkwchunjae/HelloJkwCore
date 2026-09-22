const swipeThreshold = 56;

export function initialize(root, dotNetReference) {
    const previousBodyOverflow = document.body.style.overflow;
    const previousRootOverflow = document.documentElement.style.overflow;
    let startY = null;
    let startedAtTop = false;
    let startedAtBottom = false;
    let wheelDistance = 0;
    let invoking = false;
    let scale = 1;
    let offsetX = 0;
    let offsetY = 0;
    let gesture = null;
    let photoGesture = false;

    const getImage = () => root.querySelector(".kidsnote-viewer-image");
    const applyTransform = () => {
        const image = getImage();
        if (!image) return;
        // Clamp to the visible photo, including object-fit letterboxing.
        const fit = image.naturalWidth && image.naturalHeight
            ? Math.min(image.clientWidth / image.naturalWidth,
                image.clientHeight / image.naturalHeight)
            : 0;
        const maxX = Math.max(0, (image.naturalWidth * fit * scale - image.clientWidth) / 2);
        const maxY = Math.max(0, (image.naturalHeight * fit * scale - image.clientHeight) / 2);
        offsetX = Math.max(-maxX, Math.min(maxX, offsetX));
        offsetY = Math.max(-maxY, Math.min(maxY, offsetY));
        image.style.transform = `translate(${offsetX}px, ${offsetY}px) scale(${scale})`;
    };
    const resetZoom = () => {
        scale = 1;
        offsetX = offsetY = 0;
        gesture = null;
        startY = null;
        applyTransform();
    };
    const beginPhotoGesture = (touches) => {
        const image = getImage();
        if (!image || !touches.length) {
            gesture = null;
            return;
        }
        const rect = image.parentElement.getBoundingClientRect();
        const first = touches[0];
        const second = touches[1] ?? first;
        gesture = {
            x: (first.clientX + second.clientX) / 2 - rect.left - rect.width / 2,
            y: (first.clientY + second.clientY) / 2 - rect.top - rect.height / 2,
            distance: touches.length > 1
                ? Math.hypot(first.clientX - second.clientX, first.clientY - second.clientY)
                : 0,
            scale, offsetX, offsetY,
        };
    };

    document.body.style.overflow = "hidden";
    document.documentElement.style.overflow = "hidden";
    root.focus({ preventScroll: true });

    const getTextScroller = () =>
        root.querySelector(".kidsnote-viewer-text-scroll");

    const isTextItem = () => root.dataset.itemKind === "text";

    const getBoundaries = () => {
        const scroller = getTextScroller();
        if (!scroller) {
            return { atTop: false, atBottom: false };
        }

        return {
            atTop: scroller.scrollTop <= 2,
            atBottom:
                scroller.scrollHeight - scroller.clientHeight - scroller.scrollTop <= 2,
        };
    };

    const navigate = async (direction) => {
        if (invoking) {
            return;
        }

        invoking = true;
        try {
            await dotNetReference.invokeMethodAsync("HandleSwipe", direction);
        } finally {
            invoking = false;
        }
    };

    const onTouchStart = (event) => {
        // Keep native taps on close/navigation buttons, even while zoomed in.
        if (event.touches.length === 1 && event.target?.closest?.("button")) {
            startY = null;
            return;
        }
        if (!isTextItem() && (event.touches.length > 1 || scale > 1 || photoGesture)) {
            // Claim the gesture before the browser starts native pinch/scroll handling.
            if (event.cancelable) event.preventDefault();
            photoGesture = true;
            startY = null;
            beginPhotoGesture(event.touches);
            return;
        }
        if (event.touches.length !== 1) {
            startY = null;
            return;
        }

        startY = event.touches[0].clientY;
        const boundaries = getBoundaries();
        startedAtTop = boundaries.atTop;
        startedAtBottom = boundaries.atBottom;
    };

    const onTouchMove = (event) => {
        if (photoGesture && gesture && !isTextItem()) {
            event.preventDefault();
            const image = getImage();
            if (!image || !event.touches.length) return;
            const rect = image.parentElement.getBoundingClientRect();
            const first = event.touches[0];
            const second = event.touches[1] ?? first;
            const x = (first.clientX + second.clientX) / 2 - rect.left - rect.width / 2;
            const y = (first.clientY + second.clientY) / 2 - rect.top - rect.height / 2;
            if (event.touches.length > 1 && gesture.distance > 0) {
                const distance = Math.hypot(first.clientX - second.clientX, first.clientY - second.clientY);
                scale = Math.max(1, Math.min(5, gesture.scale * distance / gesture.distance));
            }
            const ratio = scale / gesture.scale;
            offsetX = x - (gesture.x - gesture.offsetX) * ratio;
            offsetY = y - (gesture.y - gesture.offsetY) * ratio;
            applyTransform();
            return;
        }
        if (!isTextItem() && startY !== null) {
            event.preventDefault();
        }
    };

    const onTouchEnd = (event) => {
        if (photoGesture) {
            startY = null;
            beginPhotoGesture(event.touches);
            if (!event.touches.length) photoGesture = false;
            return;
        }
        if (startY === null || event.changedTouches.length === 0) {
            return;
        }

        const deltaY = event.changedTouches[0].clientY - startY;
        startY = null;
        if (Math.abs(deltaY) < swipeThreshold) {
            return;
        }

        const direction = deltaY < 0 ? 1 : -1;
        if (!isTextItem() ||
            (direction > 0 && startedAtBottom) ||
            (direction < 0 && startedAtTop)) {
            void navigate(direction);
        }
    };

    const onTouchCancel = () => {
        startY = null;
        gesture = null;
        photoGesture = false;
    };

    const onWheel = (event) => {
        if (!isTextItem() && scale > 1) {
            event.preventDefault();
            return;
        }
        const direction = event.deltaY > 0 ? 1 : -1;
        if (isTextItem()) {
            const boundaries = getBoundaries();
            if ((direction > 0 && !boundaries.atBottom) ||
                (direction < 0 && !boundaries.atTop)) {
                wheelDistance = 0;
                return;
            }
        }

        event.preventDefault();
        wheelDistance += event.deltaY;
        if (Math.abs(wheelDistance) < swipeThreshold) {
            return;
        }

        const wheelDirection = wheelDistance > 0 ? 1 : -1;
        wheelDistance = 0;
        void navigate(wheelDirection);
    };

    // Blazor can reuse the img element when moving to another photo.
    let observedImage = getImage();
    let observedSource = observedImage?.getAttribute("src");
    const observer = new MutationObserver(() => {
        const image = getImage();
        const source = image?.getAttribute("src");
        // A Blazor render can mutate the stage without changing the photo.
        if (image !== observedImage || source !== observedSource) {
            observedImage = image;
            observedSource = source;
            resetZoom();
        }
    });
    observer.observe(root.querySelector(".kidsnote-viewer-stage"), {
        childList: true, subtree: true, attributes: true, attributeFilter: ["src"],
    });
    root.addEventListener("load", applyTransform, true);
    window.addEventListener("resize", applyTransform);
    root.addEventListener("touchcancel", onTouchCancel, { passive: true });
    root.addEventListener("touchstart", onTouchStart, { passive: false });
    root.addEventListener("touchmove", onTouchMove, { passive: false });
    root.addEventListener("touchend", onTouchEnd, { passive: true });
    root.addEventListener("wheel", onWheel, { passive: false });

    return {
        setTextBoundary(boundary) {
            const scroller = getTextScroller();
            if (!scroller) {
                return;
            }

            scroller.scrollTop = boundary === "end"
                ? scroller.scrollHeight
                : 0;
        },
        dispose() {
            observer.disconnect();
            root.removeEventListener("load", applyTransform, true);
            window.removeEventListener("resize", applyTransform);
            root.removeEventListener("touchcancel", onTouchCancel);
            root.removeEventListener("touchstart", onTouchStart);
            root.removeEventListener("touchmove", onTouchMove);
            root.removeEventListener("touchend", onTouchEnd);
            root.removeEventListener("wheel", onWheel);
            document.body.style.overflow = previousBodyOverflow;
            document.documentElement.style.overflow = previousRootOverflow;
        },
    };
}
