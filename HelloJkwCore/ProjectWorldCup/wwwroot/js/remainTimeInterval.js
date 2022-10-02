export function createRemainTimeInterval(instance, selectorClass, remainSeconds) {
    return new RemainTimeInterval(instance, selectorClass, remainSeconds);
}
class RemainTimeInterval {
    timer = null;
    initTime;
    initRemainSeconds;
    remainSeconds;
    constructor(instance, selectorClass, remainSeconds) {
        this.initTime = Date.now() / 1000;
        this.initRemainSeconds = remainSeconds;
        this.remainSeconds = remainSeconds;
        if (this.remainSeconds <= 0) {
            instance.invokeMethodAsync("OnTimeOver");
            return;
        }
        const elements = document.getElementsByClassName(selectorClass);
        for (let elem of elements) {
            elem.innerText = this.text(this.remainSeconds);
        }
        this.timer = setInterval(() => {
            const now = Date.now() / 1000;
            this.remainSeconds = this.initRemainSeconds - Math.floor(now - this.initTime);
            if (this.remainSeconds <= 0) {
                instance.invokeMethodAsync("OnTimeOver");
                this.dispose();
            }
            for (let elem of elements) {
                elem.innerText = this.text(this.remainSeconds);
            }
        }, 1000);
    }
    text(totalSeconds) {
        const days = Math.floor(totalSeconds / 86400);
        const hours = Math.floor(totalSeconds % 86400 / 3600);
        const minutes = Math.floor(totalSeconds % 3600 / 60);
        const seconds = Math.floor(totalSeconds % 60);
        if (days > 0) {
            return `${days}일 ${hours}시간 ${minutes}분 ${seconds}초`;
        }
        if (hours > 0) {
            return `${hours}시간 ${minutes}분 ${seconds}초`;
        }
        if (minutes > 0) {
            return `${minutes}분 ${seconds}초`;
        }
        if (seconds > 0) {
            return `${seconds}초`;
        }
    }
    dispose() {
        if (this.timer) {
            clearInterval(this.timer);
            this.timer = null;
        }
    }
}
//# sourceMappingURL=remainTimeInterval.js.map