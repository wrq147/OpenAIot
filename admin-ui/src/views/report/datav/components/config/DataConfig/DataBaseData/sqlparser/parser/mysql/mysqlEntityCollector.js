import { EntityContextType } from '../common/types';
import { StmtContextType, EntityCollector } from '../common/entityCollector';
export class MySqlEntityCollector extends EntityCollector {
    /** ====== Entity Begin */
    exitDatabaseName(ctx) {
        this.pushEntity(ctx, EntityContextType.DATABASE);
    }
    exitDatabaseNameCreate(ctx) {
        this.pushEntity(ctx, EntityContextType.DATABASE_CREATE);
    }
    exitTableName(ctx) {
        this.pushEntity(ctx, EntityContextType.TABLE);
    }
    exitTableNameCreate(ctx) {
        this.pushEntity(ctx, EntityContextType.TABLE_CREATE);
    }
    exitViewName(ctx) {
        this.pushEntity(ctx, EntityContextType.VIEW);
    }
    exitViewNameCreate(ctx) {
        this.pushEntity(ctx, EntityContextType.VIEW_CREATE);
    }
    exitFunctionNameCreate(ctx) {
        this.pushEntity(ctx, EntityContextType.FUNCTION_CREATE);
    }
    exitColumnNameCreate(ctx) {
        this.pushEntity(ctx, EntityContextType.COLUMN_CREATE);
    }
    /** ===== Statement begin */
    enterSingleStatement(ctx) {
        this.pushStmt(ctx, StmtContextType.COMMON_STMT);
    }
    exitSingleStatement(ctx) {
        this.popStmt();
    }
    enterQueryCreateTable(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_TABLE_STMT);
    }
    exitQueryCreateTable(ctx) {
        this.popStmt();
    }
    enterColumnCreateTable(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_TABLE_STMT);
    }
    exitColumnCreateTable(ctx) {
        this.popStmt();
    }
    enterCopyCreateTable(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_TABLE_STMT);
    }
    exitCopyCreateTable(ctx) {
        this.popStmt();
    }
    enterCreateView(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_VIEW_STMT);
    }
    exitCreateView(ctx) {
        this.popStmt();
    }
    enterSimpleSelect(ctx) {
        this.pushStmt(ctx, StmtContextType.SELECT_STMT);
    }
    exitSimpleSelect(ctx) {
        this.popStmt();
    }
    enterUnionAndLateralSelect(ctx) {
        this.pushStmt(ctx, StmtContextType.SELECT_STMT);
    }
    exitUnionAndLateralSelect(ctx) {
        this.popStmt();
    }
    enterSelectExpression(ctx) {
        this.pushStmt(ctx, StmtContextType.SELECT_STMT);
    }
    exitSelectExpression(ctx) {
        this.popStmt();
    }
    enterInsertStatement(ctx) {
        this.pushStmt(ctx, StmtContextType.INSERT_STMT);
    }
    exitInsertStatement(ctx) {
        this.popStmt();
    }
    enterCreateDatabase(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_DATABASE_STMT);
    }
    exitCreateDatabase(ctx) {
        this.popStmt();
    }
    enterCreateFunction(ctx) {
        this.pushStmt(ctx, StmtContextType.CREATE_FUNCTION_STMT);
    }
    exitCreateFunction(ctx) {
        this.popStmt();
    }
}
