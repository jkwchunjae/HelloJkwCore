const swipeThreshold = 56;

export function initialize(root, dotNetReference) {
    const previousBodyOverflow = document.body.style.overflow;
    const previousRootOverflow = document.documentElement.style.overflow;
    let startY = null;
    let startedAtTop = false;
    let startedAtBottom = false;
    let wheelDistance = 0;
    let invoking = false;

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
        if (!isTextItem() && startY !== null) {
            event.preventDefault();
        }
    };

    const onTouchEnd = (event) => {
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

    const onWheel = (event) => {
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

    root.addEventListener("touchstart", onTouchStart, { passive: true });
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
            root.removeEventListener("touchstart", onTouchStart);
            root.removeEventListener("touchmove", onTouchMove);
            root.removeEventListener("touchend", onTouchEnd);
            root.removeEventListener("wheel", onWheel);
            document.body.style.overflow = previousBodyOverflow;
            document.documentElement.style.overflow = previousRootOverflow;
        },
    };
}
