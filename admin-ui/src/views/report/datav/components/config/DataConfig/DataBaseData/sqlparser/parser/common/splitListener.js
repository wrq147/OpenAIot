export class SplitListener {
    constructor() {
        this._statementsContext = [];
    }
    visitTerminal() { }
    visitErrorNode() { }
    enterEveryRule() { }
    exitEveryRule() { }
    get statementsContext() {
        return this._statementsContext;
    }
}
