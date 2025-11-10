import * as antlr from "../antlr4ng";
export class SQLParserBase extends antlr.Parser {
    constructor(input) {
        super(input);
        this.caretTokenIndex = -1;
        this.entityCollecting = false;
    }
    shouldMatchEmpty() {
        var _a, _b, _c, _d;
        return this.entityCollecting
            && ((_b = (_a = this.tokenStream.LT(-1)) === null || _a === void 0 ? void 0 : _a.tokenIndex) !== null && _b !== void 0 ? _b : Infinity) <= this.caretTokenIndex
            && ((_d = (_c = this.tokenStream.LT(1)) === null || _c === void 0 ? void 0 : _c.tokenIndex) !== null && _d !== void 0 ? _d : -Infinity) >= this.caretTokenIndex;
    }
}
