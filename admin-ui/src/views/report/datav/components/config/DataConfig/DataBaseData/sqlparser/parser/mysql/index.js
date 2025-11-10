import { MySqlLexer } from '../../lib/mysql/MySqlLexer';
import { MySqlParser } from '../../lib/mysql/MySqlParser';
import { BasicSQL } from '../common/basicSQL';
import { EntityContextType } from '../common/types';
import { MysqlSplitListener } from './mysqlSplitListener';
import { MySqlEntityCollector } from './mysqlEntityCollector';
export { MySqlEntityCollector, MysqlSplitListener };
export class MySQL extends BasicSQL {
    constructor() {
        super(...arguments);
        this.preferredRules = new Set([
            MySqlParser.RULE_databaseName,
            MySqlParser.RULE_databaseNameCreate,
            MySqlParser.RULE_tableName,
            MySqlParser.RULE_tableNameCreate,
            MySqlParser.RULE_viewName,
            MySqlParser.RULE_viewNameCreate,
            MySqlParser.RULE_functionName,
            MySqlParser.RULE_functionNameCreate,
            MySqlParser.RULE_columnName,
            MySqlParser.RULE_columnNameCreate,
        ]);
    }
    createLexerFromCharStream(charStreams) {
        return new MySqlLexer(charStreams);
    }
    createParserFromTokenStream(tokenStream) {
        return new MySqlParser(tokenStream);
    }
    get splitListener() {
        return new MysqlSplitListener();
    }
    createEntityCollector(input, caretTokenIndex) {
        return new MySqlEntityCollector(input, caretTokenIndex);
    }
    processCandidates(candidates, allTokens, caretTokenIndex, tokenIndexOffset) {
        const originalSyntaxSuggestions = [];
        const keywords = [];
        for (const candidate of candidates.rules) {
            const [ruleType, candidateRule] = candidate;
            const startTokenIndex = candidateRule.startTokenIndex + tokenIndexOffset;
            const tokenRanges = allTokens.slice(startTokenIndex, caretTokenIndex + tokenIndexOffset + 1);
            let syntaxContextType = void 0;
            switch (ruleType) {
                case MySqlParser.RULE_databaseName: {
                    syntaxContextType = EntityContextType.DATABASE;
                    break;
                }
                case MySqlParser.RULE_databaseNameCreate: {
                    syntaxContextType = EntityContextType.DATABASE_CREATE;
                    break;
                }
                case MySqlParser.RULE_tableName: {
                    syntaxContextType = EntityContextType.TABLE;
                    break;
                }
                case MySqlParser.RULE_tableNameCreate: {
                    syntaxContextType = EntityContextType.TABLE_CREATE;
                    break;
                }
                case MySqlParser.RULE_viewName: {
                    syntaxContextType = EntityContextType.VIEW;
                    break;
                }
                case MySqlParser.RULE_viewNameCreate: {
                    syntaxContextType = EntityContextType.VIEW_CREATE;
                    break;
                }
                case MySqlParser.RULE_functionName: {
                    syntaxContextType = EntityContextType.FUNCTION;
                    break;
                }
                case MySqlParser.RULE_functionNameCreate: {
                    syntaxContextType = EntityContextType.FUNCTION_CREATE;
                    break;
                }
                case MySqlParser.RULE_columnName: {
                    syntaxContextType = EntityContextType.COLUMN;
                    break;
                }
                case MySqlParser.RULE_columnNameCreate: {
                    syntaxContextType = EntityContextType.COLUMN_CREATE;
                    break;
                }
                default:
                    break;
            }
            if (syntaxContextType) {
                originalSyntaxSuggestions.push({
                    syntaxContextType,
                    wordRanges: tokenRanges,
                });
            }
        }
        for (const candidate of candidates.tokens) {
            const symbolicName = this._parser.vocabulary.getSymbolicName(candidate[0]);
            const displayName = this._parser.vocabulary.getDisplayName(candidate[0]);
            if (displayName && symbolicName && symbolicName.startsWith('KW_')) {
                const keyword = displayName.startsWith("'") && displayName.endsWith("'")
                    ? displayName.slice(1, -1)
                    : displayName;
                keywords.push(keyword);
            }
        }
        return {
            syntax: originalSyntaxSuggestions,
            keywords,
        };
    }
}
