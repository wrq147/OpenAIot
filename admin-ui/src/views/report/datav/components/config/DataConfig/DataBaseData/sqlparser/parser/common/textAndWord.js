/**
 * Convert Token to Word
 */
export function tokenToWord(token, input) {
    var _a;
    const startIndex = token.start;
    const endIndex = token.stop;
    const text = (_a = token.text) !== null && _a !== void 0 ? _a : '';
    return {
        text,
        line: token.line,
        startIndex,
        endIndex,
        startColumn: token.column + 1,
        endColumn: token.column + 1 + text.length,
    };
}
/**
 * Convert ParserRuleContext to Word
 */
export function ctxToWord(ctx, input) {
    var _a, _b;
    if (!ctx.start || !ctx.stop) {
        return null;
    }
    const startIndex = ctx.start.start;
    const endIndex = ctx.stop.stop;
    const text = input.slice(startIndex, endIndex + 1);
    return {
        text,
        line: ctx.start.line,
        startIndex,
        endIndex,
        startColumn: ctx.start.column + 1,
        endColumn: ctx.stop.column + 1 + ((_b = (_a = ctx.stop.text) === null || _a === void 0 ? void 0 : _a.length) !== null && _b !== void 0 ? _b : 0),
    };
}
/**
 * Convert ParserRuleContext to Text
 */
export function ctxToText(ctx, input) {
    var _a, _b;
    if (!ctx.start || !ctx.stop) {
        return null;
    }
    const startIndex = ctx.start.start;
    const endIndex = ctx.stop.stop;
    const text = input.slice(startIndex, endIndex + 1);
    return {
        text,
        startLine: ctx.start.line,
        endLine: ctx.stop.line,
        startIndex,
        endIndex,
        startColumn: ctx.start.column + 1,
        endColumn: ctx.stop.column + 1 + ((_b = (_a = ctx.stop.text) === null || _a === void 0 ? void 0 : _a.length) !== null && _b !== void 0 ? _b : 0),
    };
}
