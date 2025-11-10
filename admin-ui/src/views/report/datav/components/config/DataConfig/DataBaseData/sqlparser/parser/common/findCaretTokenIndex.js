/**
 * find token index via caret position (cursor position)
 * @param caretPosition
 * @param allTokens all the tokens
 * @returns caretTokenIndex
 */
export function findCaretTokenIndex(caretPosition, allTokens) {
    var _a, _b;
    const { lineNumber: caretLine, column: caretCol } = caretPosition;
    let left = 0;
    let right = allTokens.length - 1;
    while (left <= right) {
        const mid = left + ((right - left) >> 1);
        const token = allTokens[mid];
        if (token.line > caretLine || (token.line === caretLine && token.column + 1 >= caretCol)) {
            right = mid - 1;
        }
        else if (token.line < caretLine ||
            (token.line === caretLine && token.column + ((_b = (_a = token.text) === null || _a === void 0 ? void 0 : _a.length) !== null && _b !== void 0 ? _b : 0) + 1 < caretCol)) {
            left = mid + 1;
        }
        else {
            return allTokens[mid].tokenIndex;
        }
    }
    return void 0;
}
