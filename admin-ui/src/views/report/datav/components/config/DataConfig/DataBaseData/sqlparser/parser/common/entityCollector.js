var __rest = (this && this.__rest) || function (s, e) {
    var t = {};
    for (var p in s) if (Object.prototype.hasOwnProperty.call(s, p) && e.indexOf(p) < 0)
        t[p] = s[p];
    if (s != null && typeof Object.getOwnPropertySymbols === "function")
        for (var i = 0, p = Object.getOwnPropertySymbols(s); i < p.length; i++) {
            if (e.indexOf(p[i]) < 0 && Object.prototype.propertyIsEnumerable.call(s, p[i]))
                t[p[i]] = s[p[i]];
        }
    return t;
};
import { EntityContextType } from './types';
import { ctxToText, ctxToWord } from './textAndWord';
import { SimpleStack } from './simpleStack';
/**
 * TODO: more stmt type should be supported.
 */
export var StmtContextType;
(function (StmtContextType) {
    /** A self-contained and complete statement */
    StmtContextType["COMMON_STMT"] = "commonStmt";
    StmtContextType["CREATE_CATALOG_STMT"] = "createCatalogStmt";
    StmtContextType["CREATE_DATABASE_STMT"] = "createDatabaseStmt";
    StmtContextType["CREATE_TABLE_STMT"] = "createTableStmt";
    StmtContextType["CREATE_VIEW_STMT"] = "createViewStmt";
    StmtContextType["SELECT_STMT"] = "selectStmt";
    StmtContextType["INSERT_STMT"] = "insertStmt";
    StmtContextType["CREATE_FUNCTION_STMT"] = "createFunctionStmt";
})(StmtContextType || (StmtContextType = {}));
export function toStmtContext(ctx, type, input, rootStmt, parentStmt, isContainCaret) {
    const text = ctxToText(ctx, input);
    if (!text)
        return null;
    const { text: _ } = text, position = __rest(text, ["text"]);
    return {
        stmtContextType: type,
        position,
        rootStmt: rootStmt !== null && rootStmt !== void 0 ? rootStmt : null,
        parentStmt: parentStmt !== null && parentStmt !== void 0 ? parentStmt : null,
        isContainCaret,
    };
}
const baseAlias = {
    isAlias: false,
    origin: null,
    alias: null,
};
export function toEntityContext(ctx, type, input, belongStmt, alias) {
    const word = ctxToWord(ctx, input);
    if (!word)
        return null;
    const { text } = word, position = __rest(word, ["text"]);
    const finalAlias = Object.assign({}, baseAlias, alias !== null && alias !== void 0 ? alias : {});
    return Object.assign({ entityContextType: type, text,
        position,
        belongStmt, relatedEntities: null, columns: null }, finalAlias);
}
/**
 * @todo: Handle alias, includes column alias, table alias, query as alias and so on.
 * @todo: [may be need] Combine the entities in each clause.
 */
export class EntityCollector {
    constructor(input, caretTokenIndex) {
        this._input = input;
        this._caretTokenIndex = caretTokenIndex !== null && caretTokenIndex !== void 0 ? caretTokenIndex : -1;
        this._entitiesSet = new Set();
        this._stmtStack = new SimpleStack();
        this._entityStack = new SimpleStack();
        this._rootStmt = null;
    }
    visitTerminal() { }
    visitErrorNode() { }
    enterEveryRule() { }
    exitEveryRule() { }
    getEntities() {
        return Array.from(this._entitiesSet);
    }
    enterProgram() {
        this._entitiesSet.clear();
        this._stmtStack.clear();
        this._entityStack.clear();
        this._rootStmt = null;
    }
    pushStmt(ctx, type) {
        var _a;
        let isContainCaret;
        if (this._caretTokenIndex >= 0) {
            isContainCaret =
                !!ctx.start &&
                    !!ctx.stop &&
                    ctx.start.tokenIndex <= this._caretTokenIndex &&
                    ctx.stop.tokenIndex >= this._caretTokenIndex;
        }
        const stmtContext = toStmtContext(ctx, type, this._input, this._rootStmt, this._stmtStack.peek(), isContainCaret);
        if (stmtContext) {
            if (this._stmtStack.isEmpty() ||
                ((_a = this._stmtStack.peek()) === null || _a === void 0 ? void 0 : _a.stmtContextType) === StmtContextType.COMMON_STMT) {
                this._rootStmt = stmtContext;
            }
            this._stmtStack.push(stmtContext);
        }
        return stmtContext;
    }
    popStmt() {
        const stmtContext = this._stmtStack.pop();
        if (stmtContext && this._rootStmt === stmtContext) {
            this._rootStmt = this._stmtStack.peek();
            if (!this._entityStack.isEmpty()) {
                this.combineEntitiesAndAdd(stmtContext);
            }
        }
        return stmtContext;
    }
    pushEntity(ctx, type, alias) {
        const entityContext = toEntityContext(ctx, type, this._input, this._stmtStack.peek(), alias);
        if (entityContext) {
            if (this._stmtStack.isEmpty()) {
                this._entitiesSet.add(entityContext);
            }
            else {
                // If is inside a statement
                this._entityStack.push(entityContext);
            }
        }
        return entityContext;
    }
    /**
     * Combine entities that inside a single statement.
     * e.g. combine tableName and column if they are inside a same createTableStatement.
     * Then add combined entities into result.
     */
    combineEntitiesAndAdd(stmtContext) {
        const entitiesInsideStmt = [];
        while (!this._entityStack.isEmpty() &&
            (this._entityStack.peek().belongStmt === stmtContext ||
                this._entityStack.peek().belongStmt.rootStmt === stmtContext)) {
            entitiesInsideStmt.unshift(this._entityStack.pop());
        }
        const combinedEntities = this.combineRootStmtEntities(stmtContext, entitiesInsideStmt);
        while (combinedEntities.length) {
            const entity = combinedEntities.shift();
            entity && this._entitiesSet.add(entity);
        }
    }
    /**
     * Combined all entities under a rootStmt.
     */
    combineRootStmtEntities(stmtContext, entitiesInsideStmt) {
        if (stmtContext.stmtContextType === StmtContextType.CREATE_VIEW_STMT ||
            stmtContext.stmtContextType === StmtContextType.CREATE_TABLE_STMT) {
            return this.combineCreateTableOrViewStmtEntities(stmtContext, entitiesInsideStmt);
        }
        return entitiesInsideStmt;
    }
    combineCreateTableOrViewStmtEntities(stmtContext, entitiesInsideStmt) {
        const columns = [];
        const relatedEntities = [];
        let mainEntity = null;
        const finalEntities = entitiesInsideStmt.reduce((result, entity) => {
            if (entity.belongStmt !== stmtContext) {
                if (entity.entityContextType !== EntityContextType.COLUMN &&
                    entity.entityContextType !== EntityContextType.COLUMN_CREATE) {
                    relatedEntities.push(entity);
                    result.push(entity);
                }
                return result;
            }
            if (entity.entityContextType === EntityContextType.COLUMN_CREATE) {
                columns.push(entity);
            }
            else if (entity.entityContextType === EntityContextType.TABLE_CREATE ||
                entity.entityContextType === EntityContextType.VIEW_CREATE) {
                mainEntity = entity;
                result.push(entity);
                return result;
            }
            else if (entity.entityContextType !== EntityContextType.COLUMN) {
                relatedEntities.push(entity);
                result.push(entity);
            }
            return result;
        }, []);
        if (mainEntity && columns.length) {
            mainEntity.columns = columns;
        }
        if (mainEntity && relatedEntities.length) {
            mainEntity.relatedEntities = relatedEntities;
        }
        return finalEntities;
    }
}
