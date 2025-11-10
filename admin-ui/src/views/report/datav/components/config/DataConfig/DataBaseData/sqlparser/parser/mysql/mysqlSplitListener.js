import { SplitListener } from '../common/splitListener';
export class MysqlSplitListener extends SplitListener {
    exitSingleStatement(ctx) {
        this._statementsContext.push(ctx);
    }
}
