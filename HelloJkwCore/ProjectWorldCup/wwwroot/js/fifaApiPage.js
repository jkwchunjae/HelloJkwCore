export function scrollToInitialMatch(targetSelector) {
    let attempts = 0;

    const tryScroll = () => {
        if (scrollToTarget(targetSelector)) {
            return;
        }

        attempts++;
        if (attempts < 10) {
            window.setTimeout(tryScroll, 50);
        }
    };

    window.requestAnimationFrame(tryScroll);
}

function scrollToTarget(targetSelector) {
    const target = findVisibleElement(document.querySelectorAll(targetSelector));
    if (!target) {
        return false;
    }

    const container = target.closest(".mud-table-container");
    if (!container) {
        target.scrollIntoView({ block: "start", inline: "nearest" });
        return true;
    }

    const header = container.querySelector("thead");
    const headerHeight = header?.getBoundingClientRect().height ?? 0;
    const containerRect = container.getBoundingClientRect();
    const targetRect = target.getBoundingClientRect();
    const top = container.scrollTop + targetRect.top - containerRect.top - headerHeight;

    container.scrollTo({
        top: Math.max(0, top),
        behavior: "auto",
    });

    return true;
}

function findVisibleElement(elements) {
    for (const element of elements) {
        const style = window.getComputedStyle(element);
        if (element.getClientRects().length > 0 && style.display !== "none" && style.visibility !== "hidden") {
            return element;
        }
    }

    return null;
}
