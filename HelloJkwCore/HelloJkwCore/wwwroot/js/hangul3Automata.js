// Hangul3Automata.js - C# Hangul3Automata 1:1 변환

// Keyboard constants
const Keyboard = {
    Shift: "Shift",
    Control: "Control",
    Command: "Command",
    Meta: "Meta",
    Alt: "Alt",
    AltGraph: "AltGraph",
    Option: "Option",
    Escape: "Escape",
    Enter: "Enter",
    Backspace: "Backspace",
    ArrowLeft: "ArrowLeft",
    ArrowRight: "ArrowRight",
    ArrowUp: "ArrowUp",
    ArrowDown: "ArrowDown",
    Insert: "Insert",
    Delete: "Delete",
    Home: "Home",
    End: "End",
    PageUp: "PageUp",
    PageDown: "PageDown",
    Tab: "Tab",
    Space: " ",
    CapsLock: "CapsLock"
};

// Hangul3Type enum
const Hangul3Type = {
    세벌식_390: "세벌식_390",
    세벌식_최종_391: "세벌식_최종_391",
    세벌식_순아래: "세벌식_순아래",
    세벌식_김국38A: "세벌식_김국38A",
    세벌식_3_2015: "세벌식_3_2015",
    세벌식_3_P3: "세벌식_3_P3",
    세벌식_3_D2: "세벌식_3_D2"
};

// JasoType enum
const JasoType = {
    Leading: "Leading",
    Vowel: "Vowel",
    Tailing: "Tailing",
    Util: "Util"
};

// Input2 class
class Input2 {
    constructor(value, shift) {
        this.value = value;
        this.shift = shift;
    }
}

// Input3 class
class Input3 {
    constructor(value, type) {
        this.value = value;
        this.type = type;
    }

    get isLeading() { return this.type === JasoType.Leading; }
    get isVowel() { return this.type === JasoType.Vowel; }
    get isTailing() { return this.type === JasoType.Tailing; }
    get isUtil() { return this.type === JasoType.Util; }

    static leading(value) { return new Input3(value, JasoType.Leading); }
    static vowel(value) { return new Input3(value, JasoType.Vowel); }
    static tailing(value) { return new Input3(value, JasoType.Tailing); }
    static util(value) { return new Input3(value, JasoType.Util); }

    static get enter() { return new Input3(Keyboard.Enter, JasoType.Util); }
    static get backspace() { return new Input3(Keyboard.Backspace, JasoType.Util); }
    static get space() { return new Input3(Keyboard.Space, JasoType.Util); }

    equals(other) {
        if (!other) return false;
        return this.value === other.value && this.type === other.type;
    }

    isDefault() {
        return !this.value && !this.type;
    }
}

// Jaso class
class Jaso {
    constructor(leading, vowel, tailing) {
        this.leading = leading || new Input3(null, null);
        this.vowel = vowel || new Input3(null, null);
        this.tailing = tailing || new Input3(null, null);
    }

    static fromStrings(l, v, t) {
        return new Jaso(
            new Input3(l, JasoType.Leading),
            new Input3(v, JasoType.Vowel),
            new Input3(t, JasoType.Tailing)
        );
    }

    get hasLeading() { return this.leading && this.leading.value; }
    get hasVowel() { return this.vowel && this.vowel.value; }
    get hasTailing() { return this.tailing && this.tailing.value; }
    get hasLeadingOnly() { return this.hasLeading && !this.hasVowel && !this.hasTailing; }

    withLeading(leading) {
        return new Jaso(leading, this.vowel, this.tailing);
    }

    withVowel(vowel) {
        return new Jaso(this.leading, vowel, this.tailing);
    }

    withTailing(tailing) {
        return new Jaso(this.leading, this.vowel, tailing);
    }

    isDefault() {
        return !this.hasLeading && !this.hasVowel && !this.hasTailing;
    }
}

// TableItem class
class TableItem {
    constructor(input1, input2, result) {
        this.input1 = input1;
        this.input2 = input2;
        this.result = result;
    }

    static createItem(type, input1, input2, result) {
        return new TableItem(
            new Input3(input1, type),
            new Input3(input2, type),
            new Input3(result, type)
        );
    }

    static createLeadingItem(input1, input2, result) {
        return TableItem.createItem(JasoType.Leading, input1, input2, result);
    }

    static createVowelItem(input1, input2, result) {
        return TableItem.createItem(JasoType.Vowel, input1, input2, result);
    }

    static createTailingItem(input1, input2, result) {
        return TableItem.createItem(JasoType.Tailing, input1, input2, result);
    }
}

// IHangul3InputConverter interface implementation - 세벌식 390
class Hangul3InputConverter_세벌식_390 {
    get converterType() { return Hangul3Type.세벌식_390; }

    input2ToInput3(input2) {
        const value = input2.value;

        switch (value) {
            case "Enter": return Input3.enter;
            case "Backspace": return Input3.backspace;
            case " ": return Input3.space;

            case "`": return Input3.util("`");
            case "1": return Input3.tailing("ㅎ");
            case "2": return Input3.tailing("ㅆ");
            case "3": return Input3.tailing("ㅂ");
            case "4": return Input3.vowel("ㅛ");
            case "5": return Input3.vowel("ㅠ");
            case "6": return Input3.vowel("ㅑ");
            case "7": return Input3.vowel("ㅖ");
            case "8": return Input3.vowel("ㅢ");
            case "9": return Input3.vowel("ㅜ");
            case "0": return Input3.leading("ㅋ");
            case "-": return Input3.util(")");
            case "=": return Input3.util(">");

            case "~": return Input3.util("~");
            case "!": return Input3.tailing("ㅈ");
            case "@": return Input3.util("@");
            case "#": return Input3.util("#");
            case "$": return Input3.util("$");
            case "%": return Input3.util("%");
            case "^": return Input3.util("^");
            case "&": return Input3.util("&");
            case "*": return Input3.util("*");
            case "(": return Input3.util("(");
            case ")": return Input3.util(")");
            case "_": return Input3.util("-");
            case "+": return Input3.util("+");

            case "q": return Input3.tailing("ㅅ");
            case "w": return Input3.tailing("ㄹ");
            case "e": return Input3.vowel("ㅕ");
            case "r": return Input3.vowel("ㅐ");
            case "t": return Input3.vowel("ㅓ");
            case "y": return Input3.leading("ㄹ");
            case "u": return Input3.leading("ㄷ");
            case "i": return Input3.leading("ㅁ");
            case "o": return Input3.leading("ㅊ");
            case "p": return Input3.leading("ㅍ");
            case "[": return Input3.util("(");
            case "]": return Input3.util("<");
            case "\\": return Input3.util("\\");

            case "Q": return Input3.tailing("ㅍ");
            case "W": return Input3.tailing("ㅌ");
            case "E": return Input3.tailing("ㅋ");
            case "R": return Input3.tailing("ㅒ");
            case "T": return Input3.util(";");
            case "Y": return Input3.util("<");
            case "U": return Input3.util("7");
            case "I": return Input3.util("8");
            case "O": return Input3.util("9");
            case "P": return Input3.util(">");
            case "{": return Input3.util("{");
            case "}": return Input3.util("}");
            case "|": return Input3.util("|");

            case "a": return Input3.tailing("ㅇ");
            case "s": return Input3.tailing("ㄴ");
            case "d": return Input3.vowel("ㅣ");
            case "f": return Input3.vowel("ㅏ");
            case "g": return Input3.vowel("ㅡ");
            case "h": return Input3.leading("ㄴ");
            case "j": return Input3.leading("ㅇ");
            case "k": return Input3.leading("ㄱ");
            case "l": return Input3.leading("ㅈ");
            case ";": return Input3.leading("ㅂ");
            case "'": return Input3.leading("ㅌ");

            case "A": return Input3.tailing("ㄷ");
            case "S": return Input3.tailing("ㄶ");
            case "D": return Input3.tailing("ㄺ");
            case "F": return Input3.tailing("ㄲ");
            case "G": return Input3.util("/");
            case "H": return Input3.util("'");
            case "J": return Input3.util("4");
            case "K": return Input3.util("5");
            case "L": return Input3.util("6");
            case ":": return Input3.util(":");
            case "\"": return Input3.util("\"");

            case "z": return Input3.tailing("ㅁ");
            case "x": return Input3.tailing("ㄱ");
            case "c": return Input3.vowel("ㅔ");
            case "v": return Input3.vowel("ㅗ");
            case "b": return Input3.vowel("ㅜ");
            case "n": return Input3.leading("ㅅ");
            case "m": return Input3.leading("ㅎ");
            case ",": return Input3.util(",");
            case ".": return Input3.util(".");
            case "/": return Input3.vowel("ㅗ");

            case "Z": return Input3.tailing("ㅊ");
            case "X": return Input3.tailing("ㅄ");
            case "C": return Input3.tailing("ㄻ");
            case "V": return Input3.tailing("ㅀ");
            case "B": return Input3.util("!");
            case "N": return Input3.util("0");
            case "M": return Input3.util("1");
            case "<": return Input3.util("2");
            case ">": return Input3.util("3");
            case "?": return Input3.util("?");

            default: return null;
        }
    }
}

// IHangul3InputConverter interface implementation - 세벌식 최종 391
class Hangul3InputConverter_세벌식_최종_391 {
    get converterType() { return Hangul3Type.세벌식_최종_391; }

    input2ToInput3(input2) {
        const value = input2.value;

        switch (value) {
            case "Enter": return Input3.enter;
            case "Backspace": return Input3.backspace;
            case " ": return Input3.space;

            case "`": return Input3.util("`");
            case "1": return Input3.tailing("ㅎ");
            case "2": return Input3.tailing("ㅆ");
            case "3": return Input3.tailing("ㅂ");
            case "4": return Input3.vowel("ㅛ");
            case "5": return Input3.vowel("ㅠ");
            case "6": return Input3.vowel("ㅑ");
            case "7": return Input3.vowel("ㅖ");
            case "8": return Input3.vowel("ㅢ");
            case "9": return Input3.vowel("ㅜ");
            case "0": return Input3.leading("ㅋ");
            case "-": return Input3.util(")");
            case "=": return Input3.util(">");

            case "~": return Input3.util("~");
            case "!": return Input3.tailing("ㄲ");
            case "@": return Input3.tailing("ㄺ");
            case "#": return Input3.tailing("ㅈ");
            case "$": return Input3.tailing("ㄿ");
            case "%": return Input3.tailing("ㄾ");
            case "^": return Input3.util("=");
            case "&": return Input3.util("\"");
            case "*": return Input3.util("\"");
            case "(": return Input3.util("'");
            case ")": return Input3.util("~");
            case "_": return Input3.util(";");
            case "+": return Input3.util("+");

            case "q": return Input3.tailing("ㅅ");
            case "w": return Input3.tailing("ㄹ");
            case "e": return Input3.vowel("ㅕ");
            case "r": return Input3.vowel("ㅐ");
            case "t": return Input3.vowel("ㅓ");
            case "y": return Input3.leading("ㄹ");
            case "u": return Input3.leading("ㄷ");
            case "i": return Input3.leading("ㅁ");
            case "o": return Input3.leading("ㅊ");
            case "p": return Input3.leading("ㅍ");
            case "[": return Input3.util("(");
            case "]": return Input3.util("<");
            case "\\": return Input3.util(":");

            case "Q": return Input3.tailing("ㅍ");
            case "W": return Input3.tailing("ㅌ");
            case "E": return Input3.tailing("ㄵ");
            case "R": return Input3.tailing("ㅀ");
            case "T": return Input3.tailing("ㄽ");
            case "Y": return Input3.util("5");
            case "U": return Input3.util("6");
            case "I": return Input3.util("7");
            case "O": return Input3.util("8");
            case "P": return Input3.util("9");
            case "{": return Input3.util("%");
            case "}": return Input3.util("/");
            case "|": return Input3.util("\\");

            case "a": return Input3.tailing("ㅇ");
            case "s": return Input3.tailing("ㄴ");
            case "d": return Input3.vowel("ㅣ");
            case "f": return Input3.vowel("ㅏ");
            case "g": return Input3.vowel("ㅡ");
            case "h": return Input3.leading("ㄴ");
            case "j": return Input3.leading("ㅇ");
            case "k": return Input3.leading("ㄱ");
            case "l": return Input3.leading("ㅈ");
            case ";": return Input3.leading("ㅂ");
            case "'": return Input3.leading("ㅌ");

            case "A": return Input3.tailing("ㄷ");
            case "S": return Input3.tailing("ㄶ");
            case "D": return Input3.tailing("ㄼ");
            case "F": return Input3.tailing("ㄻ");
            case "G": return Input3.vowel("ㅒ");
            case "H": return Input3.util("0");
            case "J": return Input3.util("1");
            case "K": return Input3.util("2");
            case "L": return Input3.util("3");
            case ":": return Input3.util("4");
            case "\"": return Input3.util(".");

            case "z": return Input3.tailing("ㅁ");
            case "x": return Input3.tailing("ㄱ");
            case "c": return Input3.vowel("ㅔ");
            case "v": return Input3.vowel("ㅗ");
            case "b": return Input3.vowel("ㅜ");
            case "n": return Input3.leading("ㅅ");
            case "m": return Input3.leading("ㅎ");
            case ",": return Input3.util(",");
            case ".": return Input3.util(".");
            case "/": return Input3.vowel("ㅗ");

            case "Z": return Input3.tailing("ㅊ");
            case "X": return Input3.tailing("ㅄ");
            case "C": return Input3.tailing("ㅋ");
            case "V": return Input3.tailing("ㄳ");
            case "B": return Input3.util("?");
            case "N": return Input3.util("-");
            case "M": return Input3.util("\"");
            case "<": return Input3.util(",");
            case ">": return Input3.util(".");
            case "?": return Input3.util("!");

            default: return null;
        }
    }
}

// Hangul3Automata class
class Hangul3Automata {
    static leadings = ["ㄱ", "ㄲ", "ㄴ", "ㄷ", "ㄸ", "ㄹ", "ㅁ", "ㅂ", "ㅃ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅉ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];
    static vowels = ["ㅏ", "ㅐ", "ㅑ", "ㅒ", "ㅓ", "ㅔ", "ㅕ", "ㅖ", "ㅗ", "ㅘ", "ㅙ", "ㅚ", "ㅛ", "ㅜ", "ㅝ", "ㅞ", "ㅟ", "ㅠ", "ㅡ", "ㅢ", "ㅣ"];
    static tailings = ["", "ㄱ", "ㄲ", "ㄳ", "ㄴ", "ㄵ", "ㄶ", "ㄷ", "ㄹ", "ㄺ", "ㄻ", "ㄼ", "ㄽ", "ㄾ", "ㄿ", "ㅀ", "ㅁ", "ㅂ", "ㅄ", "ㅅ", "ㅆ", "ㅇ", "ㅈ", "ㅊ", "ㅋ", "ㅌ", "ㅍ", "ㅎ"];

    static table = [
        TableItem.createLeadingItem("ㄱ", "ㄱ", "ㄲ"),
        TableItem.createLeadingItem("ㄷ", "ㄷ", "ㄸ"),
        TableItem.createLeadingItem("ㅂ", "ㅂ", "ㅃ"),
        TableItem.createLeadingItem("ㅅ", "ㅅ", "ㅆ"),
        TableItem.createLeadingItem("ㅈ", "ㅈ", "ㅉ"),
        TableItem.createVowelItem("ㅗ", "ㅏ", "ㅘ"),
        TableItem.createVowelItem("ㅗ", "ㅐ", "ㅙ"),
        TableItem.createVowelItem("ㅗ", "ㅣ", "ㅚ"),
        TableItem.createVowelItem("ㅜ", "ㅓ", "ㅝ"),
        TableItem.createVowelItem("ㅜ", "ㅔ", "ㅞ"),
        TableItem.createVowelItem("ㅜ", "ㅣ", "ㅟ"),
        TableItem.createVowelItem("ㅡ", "ㅣ", "ㅢ"),
        TableItem.createTailingItem("ㄱ", "ㄱ", "ㄲ"),
        TableItem.createTailingItem("ㄱ", "ㅅ", "ㄳ"),
        TableItem.createTailingItem("ㄴ", "ㅈ", "ㄵ"),
        TableItem.createTailingItem("ㄴ", "ㅎ", "ㄶ"),
        TableItem.createTailingItem("ㄹ", "ㄱ", "ㄺ"),
        TableItem.createTailingItem("ㄹ", "ㅁ", "ㄻ"),
        TableItem.createTailingItem("ㄹ", "ㅂ", "ㄼ"),
        TableItem.createTailingItem("ㄹ", "ㅅ", "ㄽ"),
        TableItem.createTailingItem("ㄹ", "ㅌ", "ㄾ"),
        TableItem.createTailingItem("ㄹ", "ㅍ", "ㄿ"),
        TableItem.createTailingItem("ㄹ", "ㅎ", "ㅀ"),
        TableItem.createTailingItem("ㅂ", "ㅅ", "ㅄ"),
    ];

    static inputConverters = [
        new Hangul3InputConverter_세벌식_390(),
        new Hangul3InputConverter_세벌식_최종_391(),
    ];

    constructor() {
        this._currentState = new Jaso();
        this._history = [];

        // Event handlers
        this.onComposed = null;      // (hangul) => {}
        this.onCurrentChanged = null; // (current) => {}
        this.onEnter = null;         // () => {}
        this.onBackspace = null;     // () => {}
    }

    _commitCurrent() {
        const hangul = Hangul3Automata._compose(this._currentState);
        if (this.onCurrentChanged) this.onCurrentChanged("");
        if (this.onComposed) this.onComposed(hangul);
        this._currentState = new Jaso();
        this._history = [];
    }

    /**
     * 현재까지 조합 중인 문자를 반환하고 내부 상태를 초기화한다.
     * 외부에서 커서 이동/선택 등으로 조합을 강제로 마무리해야 할 때 사용한다.
     */
    flush() {
        const hangul = Hangul3Automata._compose(this._currentState);
        this._currentState = new Jaso();
        this._history = [];
        if (this.onCurrentChanged) this.onCurrentChanged("");
        return hangul;
    }

    static _compose(jaso) {
        const indexLeading = Hangul3Automata.leadings.indexOf(jaso.leading?.value);
        const indexVowel = Hangul3Automata.vowels.indexOf(jaso.vowel?.value);
        const indexTailing = Hangul3Automata.tailings.indexOf(jaso.tailing?.value || "");

        const hasLeading = jaso.hasLeading;
        const hasVowel = jaso.hasVowel;
        const hasTailing = jaso.hasTailing;

        if (hasLeading && hasVowel && hasTailing) {
            const code = 0xAC00 + (indexLeading * 21 + indexVowel) * 28 + indexTailing;
            return String.fromCodePoint(code);
        } else if (hasLeading && hasVowel) {
            const code = 0xAC00 + (indexLeading * 21 + indexVowel) * 28;
            return String.fromCodePoint(code);
        } else if (hasLeading && !hasVowel && !hasTailing) {
            return jaso.leading.value;
        } else if (hasLeading && !hasVowel && hasTailing) {
            throw new Error("초성, 종성 조합으로는 한글을 만들 수 없습니다.");
        } else if (!hasLeading && hasVowel && !hasTailing) {
            return jaso.vowel.value;
        } else if (!hasLeading && hasVowel && hasTailing) {
            throw new Error("모음, 종성 조합으로는 한글을 만들 수 없습니다.");
        } else if (!hasLeading && !hasVowel && hasTailing) {
            return jaso.tailing?.value || "";
        } else {
            return "";
        }
    }

    /** 세벌식 입력 */
    handle3(input) {
        if (input.equals(Input3.enter)) {
            this._commitCurrent();
            if (this.onEnter) this.onEnter();
        } else if (input.equals(Input3.backspace)) {
            if (this._history.length > 0) {
                this._handleBackspace();
            } else {
                if (this.onBackspace) this.onBackspace();
            }
        } else if (input.isUtil) {
            this._commitCurrent();
            if (this.onComposed) this.onComposed(input.value);
        } else if (input.isLeading) {
            this._handleLeading(input);
        } else if (input.isVowel) {
            this._handleVowel(input);
        } else if (input.isTailing) {
            this._handleTailing(input);
        }
    }

    _handleLeading(input) {
        const canDoubleResult = this._canDoubleLeading(input);
        if (canDoubleResult.canDouble) {
            // 기존 입력한 초성과 지금 입력한 초성을 합쳤을 때 새 초성을 만들 수 있는가?
            this._currentState = this._currentState.withLeading(canDoubleResult.nextLeading);
            this._history.push(input);
            if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            return;
        }
        if (!this._currentState.isDefault()) {
            // 앞에 뭔가 입력되고 있었다. 그것을 다 마치고, 새 입력을 시작
            this._commitCurrent();
        }

        // 새 입력 시작
        this._currentState = new Jaso(input, null, null);
        this._history.push(input);
        if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
    }

    _canDoubleLeading(input) {
        if (this._currentState.hasLeadingOnly) {
            // 기존 입력한 초성과 지금 입력한 초성을 합쳤을 때 새 초성을 만들 수 있는가?
            const tableItem = Hangul3Automata.table.find(item =>
                item.input1.value === this._currentState.leading.value &&
                item.input2.value === input.value
            );
            if (tableItem) {
                return { canDouble: true, nextLeading: tableItem.result };
            }
        }
        return { canDouble: false, nextLeading: null };
    }

    _handleVowel(input) {
        if (this._currentState.hasLeadingOnly) {
            // 초성만 있는 경우. 이제 모음을 입력하자.
            this._currentState = this._currentState.withVowel(input);
            this._history.push(input);
            if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
        } else {
            const canDoubleResult = this._canDoubleVowel(input);
            if (canDoubleResult.canDouble) {
                // 기존 입력한 모음과 지금 입력한 모음을 합쳤을 때 새 모음을 만들 수 있는 경우
                this._currentState = this._currentState.withVowel(canDoubleResult.nextVowel);
                this._history.push(input);
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            } else {
                this._commitCurrent();
                this._currentState = new Jaso(null, input, null);
                this._history.push(input);
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            }
        }
    }

    _canDoubleVowel(input) {
        const tableItem = Hangul3Automata.table.find(item =>
            item.input1.value === this._currentState.vowel?.value &&
            item.input2.value === input.value
        );
        if (tableItem) {
            return { canDouble: true, nextVowel: tableItem.result };
        }
        return { canDouble: false, nextVowel: null };
    }

    _handleTailing(input) {
        if (this._currentState.hasLeading && this._currentState.hasVowel) {
            // 초성과 모임이 입력되어 있는 경우
            if (!this._currentState.hasTailing) {
                // 종성이 입력되어 있지 않은 경우. 가장 간단한 경우
                this._currentState = this._currentState.withTailing(input);
                this._history.push(input);
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            } else {
                const canDoubleResult = this._canDoubleTailing(input);
                if (canDoubleResult.canDouble) {
                    // 기존 입력한 종성과 지금 입력한 종성을 합쳤을 때 새 종성을 만들 수 있는 경우
                    this._currentState = this._currentState.withTailing(canDoubleResult.nextTailing);
                    this._history.push(input);
                    if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
                } else {
                    // 기존 입력한 종성과 연결되지 않는 경우.
                    // 지금 것은 commit 시키고, 종성을 입력한다.
                    this._commitCurrent();
                    this._currentState = new Jaso(null, null, input);
                    this._history.push(input);
                    if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
                }
            }
        } else {
            // 초성, 모임 중 하나가 입력되어 있지 않은 경우
            // 지금 것은 commit 시키고, 종성을 입력한다.
            this._commitCurrent();
            this._currentState = new Jaso(null, null, input);
            this._history.push(input);
            if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
        }
    }

    _canDoubleTailing(input) {
        const tableItem = Hangul3Automata.table.find(item =>
            item.input1.value === this._currentState.tailing?.value &&
            item.input2.value === input.value
        );
        if (tableItem) {
            return { canDouble: true, nextTailing: tableItem.result };
        }
        return { canDouble: false, nextTailing: null };
    }

    _handleBackspace() {
        const lastInput = this._history[this._history.length - 1];
        if (lastInput.isLeading) {
            const doubleLeading = Hangul3Automata.table.find(item =>
                item.result.value === this._currentState.leading?.value
            );
            if (doubleLeading) {
                this._currentState = this._currentState.withLeading(doubleLeading.input1);
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            } else {
                this._currentState = this._currentState.withLeading(new Input3(null, null));
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            }
        } else if (lastInput.isVowel) {
            const doubleVowel = Hangul3Automata.table.find(item =>
                item.result.value === this._currentState.vowel?.value
            );
            if (doubleVowel) {
                this._currentState = this._currentState.withVowel(doubleVowel.input1);
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            } else {
                this._currentState = this._currentState.withVowel(new Input3(null, null));
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            }
        } else if (lastInput.isTailing) {
            const doubleTailing = Hangul3Automata.table.find(item =>
                item.result.value === this._currentState.tailing?.value
            );
            if (doubleTailing) {
                this._currentState = this._currentState.withTailing(doubleTailing.input1);
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            } else {
                this._currentState = this._currentState.withTailing(new Input3(null, null));
                this._history.pop();
                if (this.onCurrentChanged) this.onCurrentChanged(Hangul3Automata._compose(this._currentState));
            }
        }
    }

    /** 일반 키보드 입력 (두벌식, 영문) 모두 세벌식으로 처리 */
    handle2(input, shift, hangul3Type = Hangul3Type.세벌식_최종_391) {
        const inputConverter = Hangul3Automata.inputConverters.find(converter =>
            converter.converterType === hangul3Type
        );
        if (!inputConverter) {
            throw new Error(`지원하지 않는 입력기 타입입니다: ${hangul3Type}`);
        }
        const input3 = inputConverter.input2ToInput3(new Input2(input, shift));
        if (input3) {
            this.handle3(input3);
        }
    }
}

// Export for ES modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        Keyboard,
        Hangul3Type,
        JasoType,
        Input2,
        Input3,
        Jaso,
        TableItem,
        Hangul3InputConverter_세벌식_390,
        Hangul3InputConverter_세벌식_최종_391,
        Hangul3Automata
    };
}
