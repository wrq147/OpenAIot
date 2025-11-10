/**
 * Syntax context type at caret position
 */
export var EntityContextType;
(function (EntityContextType) {
    /** catalog name */
    EntityContextType["CATALOG"] = "catalog";
    /** catalog name that will be created  */
    EntityContextType["CATALOG_CREATE"] = "catalogCreate";
    /** database name path, such as catalog.db */
    EntityContextType["DATABASE"] = "database";
    /** database name path that will be created  */
    EntityContextType["DATABASE_CREATE"] = "databaseCreate";
    /** table name path, such as catalog.db.tb */
    EntityContextType["TABLE"] = "table";
    /** table name path that will be created */
    EntityContextType["TABLE_CREATE"] = "tableCreate";
    /** view name path, such as db.tb.view */
    EntityContextType["VIEW"] = "view";
    /** view name path that will be created */
    EntityContextType["VIEW_CREATE"] = "viewCreate";
    /** function name */
    EntityContextType["FUNCTION"] = "function";
    /** function name that will be created */
    EntityContextType["FUNCTION_CREATE"] = "functionCreate";
    /** procedure name */
    EntityContextType["PROCEDURE"] = "procedure";
    /** procedure name that will be created */
    EntityContextType["PROCEDURE_CREATE"] = "procedureCreate";
    /** column name */
    EntityContextType["COLUMN"] = "column";
    /** column name that will be created */
    EntityContextType["COLUMN_CREATE"] = "columnCreate";
})(EntityContextType || (EntityContextType = {}));
