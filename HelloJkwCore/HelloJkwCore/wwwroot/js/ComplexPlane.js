// Canvas
var canvas;
var ctx;
var dotNetRef;

// 좌표 평면 상의 중심 좌표
var centerX = 0;
var centerY = 0;
// 좌표 평면 상의 시작 좌표
var beginX = 0;
var beginY = 0;
// 좌표 평면 상의 끝 좌표
var endX = 0;
var endY = 0;
// 좌표 평면 상의 크기
var width = 0;
var height = 0;
// 좌표 평면 상 눈금 간격
var unit = 0;
var unitLength = 0;

// 캔버스 상의 크기
var canvasWidth = 0; // 1920
var canvasHeight = 0; // 1080
var canvasRatio = 0;

// 사각형을 그리기 위한 변수
var isDrawing = false;
var dragStartX = 0;
var dragStartY = 0;
var dragCurrentX = 0;
var dragCurrentY = 0;

var img = undefined; // Image

function convertToCanvasX(x) {
    return (x - beginX) * canvasWidth / width;
}

function convertToCanvasY(y) {
    return (y - beginY) * canvasHeight / height;
}

function convertToPlaneX(x) {
    return x / canvasWidth * width + beginX;
}

function convertToPlaneY(y) {
    return y / canvasHeight * height + beginY;
}

export function canvas_register(_dotNetRef, canvasId, _centerX, _centerY, _width, _height, _canvasWidth, _canvasHeight) {
    dotNetRef = _dotNetRef;
    canvasWidth = _canvasWidth;
    canvasHeight = _canvasHeight;
    canvasRatio = _canvasWidth / _canvasHeight;
    // console.log('canvas-info', { canvasWidth, canvasHeight, canvasRatio });

    canvas = document.getElementById(canvasId);
    ctx = canvas.getContext('2d');

    const dpr = window.devicePixelRatio || 1;
    canvas.width = canvasWidth * dpr;
    canvas.height = canvasHeight * dpr;
    canvas.style.width = `${canvasWidth}px`;
    canvas.style.height = `${canvasHeight}px`;

    ctx.scale(dpr, dpr); // 모든 그리기를 고해상도에 맞게 조정

    updatePosition(_centerX, _centerY, _width, _height);
}

function updatePosition(_centerX, _centerY, _width, _height) {
    centerX = _centerX;
    centerY = _centerY;
    width = _width;
    height = _height;
    beginX = centerX - width / 2;
    beginY = centerY - height / 2;
    endX = centerX + width / 2;
    endY = centerY + height / 2;

    var aaaa = [
        1,
        0.5, 0.1,
        0.05, 0.01,
        0.005, 0.001,
        0.0005, 0.0001,
        0.00005, 0.00001,
        0.000005, 0.000001,
        0.0000005, 0.0000001,
        0.00000005, 0.00000001,
        0.000000005, 0.000000001,
        0.0000000005, 0.0000000001,
        0.00000000005, 0.00000000001,
    ];
    var unitData = aaaa.map((a, i) => ({ a, i })).filter(a => width / a.a >= 2 && width / a.a < 20).sort((a, b) => a.i - b.i).reverse()[0];
    console.log('unit', unitData.a);
    unit = unitData.a;
    unitLength = (unitData.i + 1) / 2;

    // 검정색으로 초기화
    ctx.fillStyle = 'black';
    ctx.fillRect(0, 0, canvas.width, canvas.height);
}

export function initComplexPlane() {
    canvas.addEventListener('mousedown', (e) => {
        dragStartX = e.offsetX;
        dragStartY = e.offsetY;
        isDrawing = true;
    });

    canvas.addEventListener('mousemove', (e) => {
        if (!isDrawing) return;
        dragCurrentX = e.offsetX;
        dragCurrentY = e.offsetY;
        drawRectangle();
    });

    canvas.addEventListener('mouseup', (e) => {
        var mouseUpX = e.offsetX;
        var mouseUpY = e.offsetY;
        if (mouseUpX === dragStartX && mouseUpY === dragStartY) {
            isDrawing = false;
            return;
        }
        if (isDrawing) {
            var startX = convertToPlaneX(dragStartX);
            var startY = convertToPlaneY(dragStartY);
            var width = convertToPlaneX(dragCurrentX) - startX;
            var height = convertToPlaneY(dragCurrentY) - startY;

            var rectangleRatio = width / height;

            if (rectangleRatio < canvasRatio) {
                var nextBeginX = startX + width / 2 - canvasRatio / 2 * height;
                var nextEndX = startX + width / 2 + canvasRatio / 2 * height;
                var nextBeginY = startY;
                var nextEndY = startY + height;
                var nextWidth = nextEndX - nextBeginX;
                var nextHeight = nextEndY - nextBeginY;
                var nextCenterX = nextBeginX + nextWidth / 2;
                var nextCenterY = nextBeginY + nextHeight / 2;
                updatePosition(nextCenterX, nextCenterY, nextWidth, nextHeight);
                dotNetRef.invokeMethodAsync('OnRectangleUpdated', nextBeginX, nextBeginY, nextWidth, nextHeight);
            } else if (rectangleRatio > canvasRatio) {
                var nextBeginY = startY + height / 2 - 0.5 / canvasRatio * width;
                var nextEndY = startY + height / 2 + 0.5 / canvasRatio * width;
                var nextBeginX = startX;
                var nextEndX = startX + width;
                var nextWidth = nextEndX - nextBeginX;
                var nextHeight = nextEndY - nextBeginY;
                var nextCenterX = nextBeginX + nextWidth / 2;
                var nextCenterY = nextBeginY + nextHeight / 2;
                updatePosition(nextCenterX, nextCenterY, nextWidth, nextHeight);
                dotNetRef.invokeMethodAsync('OnRectangleUpdated', nextBeginX, nextBeginY, nextWidth, nextHeight);
            } else {
                var nextCenterX = startX + width / 2;
                var nextCenterY = startY + height / 2;
                updatePosition(nextCenterX, nextCenterY, width, height);
                dotNetRef.invokeMethodAsync('OnRectangleUpdated', startX, startY, width, height);
            }

            if (img) {
                if (rectangleRatio < canvasRatio) {
                    var dragWidth = dragCurrentX - dragStartX;
                    var dragHeight = dragCurrentY - dragStartY;
                    var nextImageBeginX = dragStartX + dragWidth / 2 - canvasRatio / 2 * dragHeight;
                    var nextImageEndX = dragStartX + dragWidth / 2 + canvasRatio / 2 * dragHeight;
                    var nextImageBeginY = dragStartY;
                    var nextImageEndY = dragStartY + dragHeight;
                    var nextImageWidth = nextImageEndX - nextImageBeginX;
                    var nextImageHeight = nextImageEndY - nextImageBeginY;
                    ctx.drawImage(img, nextImageBeginX, nextImageBeginY, nextImageWidth, nextImageHeight, 0, 0, canvasWidth, canvasHeight);
                } else if (rectangleRatio > canvasRatio) {
                    var dragWidth = dragCurrentX - dragStartX;
                    var dragHeight = dragCurrentY - dragStartY;
                    var nextImageBeginX = dragStartX;
                    var nextImageEndX = dragStartX + dragWidth;
                    var nextImageBeginY = dragStartY + dragHeight / 2 - 0.5 / canvasRatio * dragWidth;
                    var nextImageEndY = dragStartY + dragHeight / 2 + 0.5 / canvasRatio * dragWidth;
                    var nextImageWidth = nextImageEndX - nextImageBeginX;
                    var nextImageHeight = nextImageEndY - nextImageBeginY;
                    ctx.drawImage(img, nextImageBeginX, nextImageBeginY, nextImageWidth, nextImageHeight, 0, 0, canvasWidth, canvasHeight);
                } else {
                    var dragWidth = dragCurrentX - dragStartX;
                    var dragHeight = dragCurrentY - dragStartY;
                    ctx.drawImage(img, dragStartX, dragStartY, dragWidth, dragHeight, 0, 0, canvasWidth, canvasHeight);
                }
            }

            drawBasicLines();
        }
        isDrawing = false;
    });

    canvas.addEventListener('mouseleave', () => {
        isDrawing = false;
    });

    drawBasicLines();
}

function drawRectangle() {
    const width = dragCurrentX - dragStartX;
    const height = dragCurrentY - dragStartY;
    ctx.clearRect(0, 0, canvas.width, canvas.height); // 이전 사각형 지우기
    if (img) {
        ctx.drawImage(img, 0, 0);
    }
    ctx.strokeRect(dragStartX, dragStartY, width, height);
    drawBasicLines();
}

function drawBasicLines() {
    var modX = beginX % unit;
    var startX = beginX + unit - (modX < 0 ? modX + unit : modX);
    for (var x = startX; x <= beginX + width; x += unit) {
        drawText(x, beginY + unit / 10, `x = ${x.toFixed(Math.max(0, unitLength))}`, 'gray');
        drawLine(x, beginY, x, endY, 'gray');
    }

    var modY = beginY % unit;
    var startY = beginY + unit - (modY < 0 ? modY + unit : modY);
    for (var y = startY; y <= beginY + height; y += unit) {
        drawText(beginX, y, `y = ${y.toFixed(Math.max(0, unitLength))}`, 'gray');
        drawLine(beginX, y, endX, y, 'gray');
    }
}

// 입력 값은 좌표평면 상의 값이 들어온다.
// 캔버스 상의 값으로 변환해서 그리기
export function drawLine(x1, y1, x2, y2, color = 'black') {
    ctx.beginPath();
    ctx.moveTo(convertToCanvasX(x1), convertToCanvasY(y1));
    ctx.lineTo(convertToCanvasX(x2), convertToCanvasY(y2));
    ctx.strokeStyle = color;
    ctx.stroke();
}

export function drawText(x, y, text, color = 'black') {
    ctx.fillStyle = color;
    ctx.font = '12px Arial';
    ctx.fillText(text, convertToCanvasX(x), convertToCanvasY(y));
}

// canvas에 base64 이미지 그리기
export function drawBase64Image(base64Image, centerX, centerY, width, height) {
    updatePosition(centerX, centerY, width, height);

    img = new Image();
    img.src = `data:image/png;base64,${base64Image}`;
    ctx.drawImage(img, 0, 0, canvasWidth, canvasHeight);

    drawBasicLines();
}