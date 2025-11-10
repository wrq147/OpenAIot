var __defProp = Object.defineProperty;
var __name = (target, value) => __defProp(target, "name", { value, configurable: true });

// src/atn/Transition.ts
var Transition = class {
  static {
    __name(this, "Transition");
  }
  static serializationNames = [
    "INVALID",
    "EPSILON",
    "RANGE",
    "RULE",
    "PREDICATE",
    "ATOM",
    "ACTION",
    "SET",
    "NOT_SET",
    "WILDCARD",
    "PRECEDENCE"
  ];
  /** The target of this transition. */
  target;
  constructor(target) {
    this.target = target;
  }
  /**
   * Determines if the transition is an "epsilon" transition.
   *
   * <p>The default implementation returns {@code false}.</p>
   *
   * @returns `true` if traversing this transition in the ATN does not
   * consume an input symbol; otherwise, {@code false} if traversing this
   * transition consumes (matches) an input symbol.
   */
  get isEpsilon() {
    return false;
  }
  get label() {
    return null;
  }
};

// src/atn/AbstractPredicateTransition.ts
var AbstractPredicateTransition = class extends Transition {
  static {
    __name(this, "AbstractPredicateTransition");
  }
  constructor(target) {
    super(target);
  }
};

// src/atn/TransitionType.ts
var TransitionType = {
  // constants for serialization
  EPSILON: 1,
  RANGE: 2,
  RULE: 3,
  PREDICATE: 4,
  // e.g., {isType(input.LT(1))}?
  ATOM: 5,
  ACTION: 6,
  SET: 7,
  // ~(A|B) or ~atom, wildcard, which convert to next 2
  NOT_SET: 8,
  WILDCARD: 9,
  PRECEDENCE: 10
};

// src/atn/ActionTransition.ts
var ActionTransition = class extends Transition {
  static {
    __name(this, "ActionTransition");
  }
  ruleIndex;
  actionIndex;
  isCtxDependent;
  constructor(target, ruleIndex, actionIndex, isCtxDependent) {
    super(target);
    this.ruleIndex = ruleIndex;
    this.actionIndex = actionIndex === void 0 ? -1 : actionIndex;
    this.isCtxDependent = isCtxDependent === void 0 ? false : isCtxDependent;
  }
  get isEpsilon() {
    return true;
  }
  get serializationType() {
    return TransitionType.ACTION;
  }
  matches(_symbol, _minVocabSymbol, _maxVocabSymbol) {
    return false;
  }
  toString() {
    return "action_" + this.ruleIndex + ":" + this.actionIndex;
  }
};

// src/atn/PredictionContext.ts
var PredictionContext = class _PredictionContext {
  static {
    __name(this, "PredictionContext");
  }
  /**
   * Represents {@code $} in an array in full context mode, when {@code $}
   * doesn't mean wildcard: {@code $ + x = [$,x]}. Here,
   * {@code $} = {@link EMPTY_RETURN_STATE}.
   */
  static EMPTY_RETURN_STATE = 2147483647;
  // TODO: Temporarily here. Should be moved to EmptyPredictionContext. It's initialized in that context class.
  static EMPTY;
  static trace_atn_sim = false;
  cachedHashCode;
  constructor(cachedHashCode) {
    this.cachedHashCode = cachedHashCode;
  }
  isEmpty() {
    return false;
  }
  hasEmptyPath() {
    return this.getReturnState(this.length - 1) === _PredictionContext.EMPTY_RETURN_STATE;
  }
  hashCode() {
    return this.cachedHashCode;
  }
  updateHashCode(hash) {
    hash.update(this.cachedHashCode);
  }
  toString(_recog) {
    return "";
  }
};

// src/misc/HashCode.ts
var c1 = 3432918353;
var c2 = 461845907;
var r1 = 15;
var r2 = 13;
var m = 5;
var n = 3864292196;
var HashCode = class _HashCode {
  static {
    __name(this, "HashCode");
  }
  count;
  hash;
  constructor() {
    this.count = 0;
    this.hash = 0;
  }
  static hashStuff(...values) {
    const hash = new _HashCode();
    hash.update(values);
    return hash.finish();
  }
  static hashString(hash = 0, str) {
    let h1 = 3735928559 ^ hash;
    let h2 = 1103547991 ^ hash;
    for (const c of str) {
      const ch = c.charCodeAt(0);
      h1 = Math.imul(h1 ^ ch, 2654435761);
      h2 = Math.imul(h2 ^ ch, 1597334677);
    }
    h1 = Math.imul(h1 ^ h1 >>> 16, 2246822507) ^ Math.imul(h2 ^ h2 >>> 13, 3266489909);
    h2 = Math.imul(h2 ^ h2 >>> 16, 2246822507) ^ Math.imul(h1 ^ h1 >>> 13, 3266489909);
    return Math.imul(4294967296, 2097151 & h2) + (h1 >>> 0);
  }
  update(...values) {
    for (const value of values) {
      if (value == null) {
        continue;
      }
      if (Array.isArray(value)) {
        this.update(...value);
      } else {
        let k = 0;
        switch (typeof value) {
          case "undefined":
          case "function": {
            continue;
          }
          case "number": {
            k = value;
            break;
          }
          case "boolean": {
            k = value ? 1237 : 4327;
            break;
          }
          case "string": {
            k = _HashCode.hashString(this.hash, value);
            break;
          }
          default: {
            value.updateHashCode(this);
            continue;
          }
        }
        this.count = this.count + 1;
        k = Math.imul(k, c1);
        k = k << r1 | k >>> 32 - r1;
        k = Math.imul(k, c2);
        let hash = this.hash ^ k;
        hash = hash << r2 | hash >>> 32 - r2;
        this.hash = Math.imul(hash, m) + n;
      }
    }
  }
  /**
   * Update the internal hash value with a pre-computed hash value.
   *
   * @param k The pre-computed hash value.
   */
  updateWithHashCode(k) {
    this.count = this.count + 1;
    k = Math.imul(k, c1);
    k = k << r1 | k >>> 32 - r1;
    k = Math.imul(k, c2);
    let hash = this.hash ^ k;
    hash = hash << r2 | hash >>> 32 - r2;
    this.hash = Math.imul(hash, m) + n;
  }
  finish() {
    let hash = this.hash ^ this.count * 4;
    hash ^= hash >>> 16;
    hash = Math.imul(hash, 2246822507);
    hash ^= hash >>> 13;
    hash = Math.imul(hash, 3266489909);
    hash ^= hash >>> 16;
    return hash;
  }
};

// src/utils/helpers.ts
var isComparable = /* @__PURE__ */ __name((candidate) => {
  return typeof candidate.equals === "function";
}, "isComparable");
var valueToString = /* @__PURE__ */ __name((v) => {
  return v === null ? "null" : v;
}, "valueToString");
var arrayToString = /* @__PURE__ */ __name((value) => {
  return Array.isArray(value) ? "[" + value.map(valueToString).join(", ") + "]" : "null";
}, "arrayToString");
var equalArrays = /* @__PURE__ */ __name((a, b) => {
  if (a === b) {
    return true;
  }
  if (a.length !== b.length) {
    return false;
  }
  for (let i = 0; i < a.length; i++) {
    const left = a[i];
    const right = b[i];
    if (left === right) {
      continue;
    }
    if (isComparable(left) && !left.equals(right)) {
      return false;
    }
  }
  return true;
}, "equalArrays");
var escapeWhitespace = /* @__PURE__ */ __name((s, escapeSpaces = false) => {
  s = s.replace(/\t/g, "\\t").replace(/\n/g, "\\n").replace(/\r/g, "\\r");
  if (escapeSpaces) {
    s = s.replace(/ /g, "\xB7");
  }
  return s;
}, "escapeWhitespace");
var standardEqualsFunction = /* @__PURE__ */ __name((a, b) => {
  return a ? a.equals(b) : a === b;
}, "standardEqualsFunction");
var stringSeedHashCode = Math.round(Math.random() * Math.pow(2, 32));
var stringHashCode = /* @__PURE__ */ __name((value) => {
  let h1b;
  let k1;
  const remainder = value.length & 3;
  const bytes = value.length - remainder;
  let h1 = stringSeedHashCode;
  const c12 = 3432918353;
  const c22 = 461845907;
  let i = 0;
  while (i < bytes) {
    k1 = value.charCodeAt(i) & 255 | (value.charCodeAt(++i) & 255) << 8 | (value.charCodeAt(++i) & 255) << 16 | (value.charCodeAt(++i) & 255) << 24;
    ++i;
    k1 = (k1 & 65535) * c12 + (((k1 >>> 16) * c12 & 65535) << 16) & 4294967295;
    k1 = k1 << 15 | k1 >>> 17;
    k1 = (k1 & 65535) * c22 + (((k1 >>> 16) * c22 & 65535) << 16) & 4294967295;
    h1 ^= k1;
    h1 = h1 << 13 | h1 >>> 19;
    h1b = (h1 & 65535) * 5 + (((h1 >>> 16) * 5 & 65535) << 16) & 4294967295;
    h1 = (h1b & 65535) + 27492 + (((h1b >>> 16) + 58964 & 65535) << 16);
  }
  k1 = 0;
  switch (remainder) {
    case 3:
      k1 ^= (value.charCodeAt(i + 2) & 255) << 16;
    case 2:
      k1 ^= (value.charCodeAt(i + 1) & 255) << 8;
    case 1:
      k1 ^= value.charCodeAt(i) & 255;
      k1 = (k1 & 65535) * c12 + (((k1 >>> 16) * c12 & 65535) << 16) & 4294967295;
      k1 = k1 << 15 | k1 >>> 17;
      k1 = (k1 & 65535) * c22 + (((k1 >>> 16) * c22 & 65535) << 16) & 4294967295;
      h1 ^= k1;
    default:
  }
  h1 ^= value.length;
  h1 ^= h1 >>> 16;
  h1 = (h1 & 65535) * 2246822507 + (((h1 >>> 16) * 2246822507 & 65535) << 16) & 4294967295;
  h1 ^= h1 >>> 13;
  h1 = (h1 & 65535) * 3266489909 + (((h1 >>> 16) * 3266489909 & 65535) << 16) & 4294967295;
  h1 ^= h1 >>> 16;
  return h1 >>> 0;
}, "stringHashCode");
var standardHashCodeFunction = /* @__PURE__ */ __name((a) => {
  return a ? typeof a === "string" ? stringHashCode(a) : a.hashCode() : -1;
}, "standardHashCodeFunction");

// src/atn/ArrayPredictionContext.ts
var ArrayPredictionContext = class _ArrayPredictionContext extends PredictionContext {
  static {
    __name(this, "ArrayPredictionContext");
  }
  parents = [];
  returnStates = [];
  constructor(parents, returnStates) {
    const h = new HashCode();
    h.update(parents, returnStates);
    const hashCode = h.finish();
    super(hashCode);
    this.parents = parents;
    this.returnStates = returnStates;
    return this;
  }
  isEmpty() {
    return this.returnStates[0] === PredictionContext.EMPTY_RETURN_STATE;
  }
  getParent(index) {
    return this.parents[index];
  }
  getReturnState(index) {
    return this.returnStates[index];
  }
  equals(other) {
    if (this === other) {
      return true;
    }
    if (!(other instanceof _ArrayPredictionContext)) {
      return false;
    }
    if (this.hashCode() !== other.hashCode()) {
      return false;
    }
    return equalArrays(this.returnStates, other.returnStates) && equalArrays(this.parents, other.parents);
  }
  toString() {
    if (this.isEmpty()) {
      return "[]";
    } else {
      let s = "[";
      for (let i = 0; i < this.returnStates.length; i++) {
        if (i > 0) {
          s = s + ", ";
        }
        if (this.returnStates[i] === PredictionContext.EMPTY_RETURN_STATE) {
          s = s + "$";
          continue;
        }
        s = s + this.returnStates[i];
        if (this.parents[i] !== null) {
          s = s + " " + this.parents[i];
        } else {
          s = s + "null";
        }
      }
      return s + "]";
    }
  }
  get length() {
    return this.returnStates.length;
  }
};

// src/IntStream.ts
var IntStream;
((IntStream2) => {
  IntStream2.EOF = -1;
  IntStream2.UNKNOWN_SOURCE_NAME = "<unknown>";
})(IntStream || (IntStream = {}));

// src/Token.ts
var Token;
((Token2) => {
  Token2.INVALID_TYPE = 0;
  Token2.EPSILON = -2;
  Token2.MIN_USER_TOKEN_TYPE = 1;
  Token2.EOF = IntStream.EOF;
  Token2.DEFAULT_CHANNEL = 0;
  Token2.HIDDEN_CHANNEL = 1;
  Token2.MIN_USER_CHANNEL_VALUE = 2;
})(Token || (Token = {}));
var isToken = /* @__PURE__ */ __name((candidate) => {
  const token = candidate;
  return token.tokenSource !== void 0 && token.channel !== void 0;
}, "isToken");

// src/misc/HashSet.ts
var HashSet = class {
  static {
    __name(this, "HashSet");
  }
  data;
  hashFunction;
  equalsFunction;
  constructor(hashFunction, equalsFunction) {
    this.data = {};
    this.hashFunction = hashFunction ?? standardHashCodeFunction;
    this.equalsFunction = equalsFunction ?? standardEqualsFunction;
  }
  add(value) {
    const key = this.hashFunction(value);
    if (key in this.data) {
      const entries = this.data[key];
      for (const entry of entries) {
        if (this.equalsFunction(value, entry)) {
          return entry;
        }
      }
      entries.push(value);
      return value;
    } else {
      this.data[key] = [value];
      return value;
    }
  }
  has(value) {
    return this.get(value) != null;
  }
  get(value) {
    const key = this.hashFunction(value);
    if (key in this.data) {
      const entries = this.data[key];
      for (const entry of entries) {
        if (this.equalsFunction(value, entry)) {
          return entry;
        }
      }
    }
    return null;
  }
  values() {
    return Object.keys(this.data).flatMap((key) => {
      return this.data[key];
    }, this);
  }
  toString() {
    return arrayToString(this.values());
  }
  get length() {
    return Object.keys(this.data).map((key) => {
      return this.data[key].length;
    }, this).reduce((accumulator, item) => {
      return accumulator + item;
    }, 0);
  }
};

// src/atn/SemanticContext.ts
var SemanticContext = class _SemanticContext {
  static {
    __name(this, "SemanticContext");
  }
  static andContext(a, b) {
    if (a === null || a === _SemanticContext.NONE) {
      return b;
    }
    if (b === null || b === _SemanticContext.NONE) {
      return a;
    }
    const result = new AND(a, b);
    if (result.opnds.length === 1) {
      return result.opnds[0];
    } else {
      return result;
    }
  }
  static orContext(a, b) {
    if (a === null) {
      return b;
    }
    if (b === null) {
      return a;
    }
    if (a === _SemanticContext.NONE || b === _SemanticContext.NONE) {
      return _SemanticContext.NONE;
    }
    const result = new OR(a, b);
    if (result.opnds.length === 1) {
      return result.opnds[0];
    } else {
      return result;
    }
  }
  static filterPrecedencePredicates(set) {
    const result = [];
    set.values().forEach((context) => {
      if (context instanceof _SemanticContext.PrecedencePredicate) {
        result.push(context);
      }
    });
    return result;
  }
  hashCode() {
    const hash = new HashCode();
    this.updateHashCode(hash);
    return hash.finish();
  }
  /**
   * Evaluate the precedence predicates for the context and reduce the result.
   *
   * @param _parser The parser instance.
   * @param _parserCallStack The current parser context object.
   * @returns The simplified semantic context after precedence predicates are
   * evaluated, which will be one of the following values.
   * <ul>
   * <li>{@link NONE}: if the predicate simplifies to {@code true} after
   * precedence predicates are evaluated.</li>
   * <li>{@code null}: if the predicate simplifies to {@code false} after
   * precedence predicates are evaluated.</li>
   * <li>{@code this}: if the semantic context is not changed as a result of
   * precedence predicate evaluation.</li>
   * <li>A non-{@code null} {@link SemanticContext}: the new simplified
   * semantic context after precedence predicates are evaluated.</li>
   * </ul>
   */
  evalPrecedence(_parser, _parserCallStack) {
    return this;
  }
};
var AND = class _AND extends SemanticContext {
  static {
    __name(this, "AND");
  }
  opnds;
  /**
   * A semantic context which is true whenever none of the contained contexts
   * is false
   */
  constructor(a, b) {
    super();
    const operands = new HashSet();
    if (a instanceof _AND) {
      a.opnds.forEach((o) => {
        operands.add(o);
      });
    } else {
      operands.add(a);
    }
    if (b instanceof _AND) {
      b.opnds.forEach((o) => {
        operands.add(o);
      });
    } else {
      operands.add(b);
    }
    const precedencePredicates = SemanticContext.filterPrecedencePredicates(operands);
    if (precedencePredicates.length > 0) {
      let reduced = null;
      precedencePredicates.forEach((p) => {
        if (reduced === null || p.precedence < reduced.precedence) {
          reduced = p;
        }
      });
      if (reduced) {
        operands.add(reduced);
      }
    }
    this.opnds = operands.values();
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _AND)) {
      return false;
    } else {
      return equalArrays(this.opnds, other.opnds);
    }
  }
  updateHashCode(hash) {
    hash.update(this.opnds);
    hash.updateWithHashCode(3813686060);
  }
  /**
   * {@inheritDoc}
   *
   * <p>
   * The evaluation of predicates by this context is short-circuiting, but
   * unordered.</p>
   */
  evaluate(parser, parserCallStack) {
    for (const operand of this.opnds) {
      if (!operand.evaluate(parser, parserCallStack)) {
        return false;
      }
    }
    return true;
  }
  evalPrecedence(parser, parserCallStack) {
    let differs = false;
    const operands = [];
    for (const context of this.opnds) {
      const evaluated = context.evalPrecedence(parser, parserCallStack);
      differs ||= evaluated !== context;
      if (evaluated === null) {
        return null;
      } else if (evaluated !== SemanticContext.NONE) {
        operands.push(evaluated);
      }
    }
    if (!differs) {
      return this;
    }
    if (operands.length === 0) {
      return SemanticContext.NONE;
    }
    let result = null;
    operands.forEach((o) => {
      result = result === null ? o : SemanticContext.andContext(result, o);
    });
    return result;
  }
  toString() {
    const s = this.opnds.map((o) => {
      return o.toString();
    });
    return (s.length > 3 ? s.slice(3) : s).join("&&");
  }
};
var OR = class _OR extends SemanticContext {
  static {
    __name(this, "OR");
  }
  opnds;
  /**
   * A semantic context which is true whenever at least one of the contained
   * contexts is true
   */
  constructor(a, b) {
    super();
    const operands = new HashSet();
    if (a instanceof _OR) {
      a.opnds.forEach((o) => {
        operands.add(o);
      });
    } else {
      operands.add(a);
    }
    if (b instanceof _OR) {
      b.opnds.forEach((o) => {
        operands.add(o);
      });
    } else {
      operands.add(b);
    }
    const precedencePredicates = SemanticContext.filterPrecedencePredicates(operands);
    if (precedencePredicates.length > 0) {
      const s = precedencePredicates.sort((a2, b2) => {
        return a2.compareTo(b2);
      });
      const reduced = s[s.length - 1];
      operands.add(reduced);
    }
    this.opnds = Array.from(operands.values());
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _OR)) {
      return false;
    } else {
      return equalArrays(this.opnds, other.opnds);
    }
  }
  updateHashCode(hash) {
    hash.update(this.opnds);
    hash.updateWithHashCode(3383313031);
  }
  /**
   * <p>
   * The evaluation of predicates by this context is short-circuiting, but
   * unordered.</p>
   */
  evaluate(parser, parserCallStack) {
    for (const operand of this.opnds) {
      if (operand.evaluate(parser, parserCallStack)) {
        return true;
      }
    }
    return false;
  }
  evalPrecedence(parser, parserCallStack) {
    let differs = false;
    const operands = [];
    for (const context of this.opnds) {
      const evaluated = context.evalPrecedence(parser, parserCallStack);
      differs ||= evaluated !== context;
      if (evaluated === SemanticContext.NONE) {
        return SemanticContext.NONE;
      } else if (evaluated !== null) {
        operands.push(evaluated);
      }
    }
    if (!differs) {
      return this;
    }
    if (operands.length === 0) {
      return null;
    }
    let result = null;
    operands.forEach((o) => {
      result = result === null ? o : SemanticContext.orContext(result, o);
    });
    return result;
  }
  toString() {
    const s = this.opnds.map((o) => {
      return o.toString();
    });
    return (s.length > 3 ? s.slice(3) : s).join("||");
  }
};
((SemanticContext2) => {
  class Predicate extends SemanticContext2 {
    static {
      __name(this, "Predicate");
    }
    ruleIndex;
    predIndex;
    isCtxDependent;
    // e.g., $i ref in pred
    constructor(ruleIndex, predIndex, isCtxDependent) {
      super();
      this.ruleIndex = ruleIndex ?? -1;
      this.predIndex = predIndex ?? -1;
      this.isCtxDependent = isCtxDependent ?? false;
    }
    evaluate(parser, outerContext) {
      const localctx = this.isCtxDependent ? outerContext : null;
      return parser.sempred(localctx, this.ruleIndex, this.predIndex);
    }
    updateHashCode(hash) {
      hash.update(this.ruleIndex, this.predIndex, this.isCtxDependent);
    }
    equals(other) {
      if (this === other) {
        return true;
      } else if (!(other instanceof Predicate)) {
        return false;
      } else {
        return this.ruleIndex === other.ruleIndex && this.predIndex === other.predIndex && this.isCtxDependent === other.isCtxDependent;
      }
    }
    toString() {
      return "{" + this.ruleIndex + ":" + this.predIndex + "}?";
    }
  }
  SemanticContext2.Predicate = Predicate;
  class PrecedencePredicate extends SemanticContext2 {
    static {
      __name(this, "PrecedencePredicate");
    }
    precedence;
    constructor(precedence) {
      super();
      this.precedence = precedence ?? 0;
    }
    evaluate(parser, outerContext) {
      return parser.precpred(outerContext, this.precedence);
    }
    evalPrecedence(parser, outerContext) {
      if (parser.precpred(outerContext, this.precedence)) {
        return SemanticContext2.NONE;
      } else {
        return null;
      }
    }
    compareTo(other) {
      return this.precedence - other.precedence;
    }
    updateHashCode(hash) {
      hash.update(this.precedence);
    }
    equals(other) {
      if (this === other) {
        return true;
      } else if (!(other instanceof PrecedencePredicate)) {
        return false;
      } else {
        return this.precedence === other.precedence;
      }
    }
    toString() {
      return "{" + this.precedence + ">=prec}?";
    }
  }
  SemanticContext2.PrecedencePredicate = PrecedencePredicate;
  SemanticContext2.NONE = new Predicate();
})(SemanticContext || (SemanticContext = {}));

// src/atn/ATNConfig.ts
var checkParams = /* @__PURE__ */ __name((params) => {
  if (params === null) {
    return {
      state: null,
      alt: null,
      context: null,
      semanticContext: null,
      reachesIntoOuterContext: null
    };
  } else {
    return {
      state: params.state ?? null,
      alt: params.alt ?? null,
      context: params.context ?? null,
      semanticContext: params.semanticContext ?? null,
      reachesIntoOuterContext: null
    };
  }
}, "checkParams");
var checkConfig = /* @__PURE__ */ __name((params) => {
  if (params === null) {
    return {
      state: null,
      alt: null,
      context: null,
      semanticContext: null,
      reachesIntoOuterContext: 0
    };
  } else {
    const props = {
      state: params.state ?? null,
      alt: params.alt ?? null,
      context: params.context ?? null,
      semanticContext: params.semanticContext ?? null,
      reachesIntoOuterContext: params.reachesIntoOuterContext ?? 0,
      precedenceFilterSuppressed: params.precedenceFilterSuppressed ?? false
    };
    return props;
  }
}, "checkConfig");
var ATNConfig = class _ATNConfig {
  static {
    __name(this, "ATNConfig");
  }
  /** The ATN state associated with this configuration */
  state;
  /** What alt (or lexer rule) is predicted by this configuration */
  alt;
  /**
   * The stack of invoking states leading to the rule/states associated
   *  with this config.  We track only those contexts pushed during
   *  execution of the ATN simulator.
   */
  context;
  /**
   * We cannot execute predicates dependent upon local context unless
   * we know for sure we are in the correct context. Because there is
   * no way to do this efficiently, we simply cannot evaluate
   * dependent predicates unless we are in the rule that initially
   * invokes the ATN simulator.
   *
   * closure() tracks the depth of how far we dip into the outer context:
   * depth &gt; 0.  Note that it may not be totally accurate depth since I
   * don't ever decrement. TODO: make it a boolean then</p>
   */
  reachesIntoOuterContext;
  precedenceFilterSuppressed = false;
  semanticContext;
  /**
   * @param {object} params A tuple: (ATN state, predicted alt, syntactic, semantic context).
   * The syntactic context is a graph-structured stack node whose
   * path(s) to the root is the rule invocation(s)
   * chain used to arrive at the state.  The semantic context is
   * the tree of semantic predicates encountered before reaching
   * an ATN state
   */
  constructor(params, config) {
    this.checkContext(params, config);
    const checkedParams = checkParams(params);
    const checkedConfig = checkConfig(config);
    this.state = checkedParams.state ?? checkedConfig.state;
    this.alt = checkedParams.alt ?? checkedConfig.alt ?? 0;
    this.context = checkedParams.context ?? checkedConfig.context;
    this.semanticContext = checkedParams.semanticContext ?? (checkedConfig.semanticContext ?? SemanticContext.NONE);
    this.reachesIntoOuterContext = checkedConfig.reachesIntoOuterContext ?? 0;
    this.precedenceFilterSuppressed = checkedConfig.precedenceFilterSuppressed ?? false;
  }
  hashCode() {
    const hash = new HashCode();
    this.updateHashCode(hash);
    return hash.finish();
  }
  updateHashCode(hash) {
    hash.update(this.state.stateNumber, this.alt, this.context, this.semanticContext);
  }
  /**
   * An ATN configuration is equal to another if both have
   * the same state, they predict the same alternative, and
   * syntactic/semantic contexts are the same
   */
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _ATNConfig)) {
      return false;
    } else {
      return this.state.stateNumber === other.state.stateNumber && this.alt === other.alt && (this.context === null ? other.context === null : this.context.equals(other.context)) && this.semanticContext.equals(other.semanticContext) && this.precedenceFilterSuppressed === other.precedenceFilterSuppressed;
    }
  }
  hashCodeForConfigSet() {
    const hash = new HashCode();
    hash.update(this.state.stateNumber, this.alt, this.semanticContext);
    return hash.finish();
  }
  equalsForConfigSet(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _ATNConfig)) {
      return false;
    } else {
      return this.state.stateNumber === other.state.stateNumber && this.alt === other.alt && this.semanticContext.equals(other.semanticContext);
    }
  }
  toString(_recog, showAlt = true) {
    let alt = "";
    if (showAlt) {
      alt = "," + this.alt;
    }
    return "(" + this.state + alt + (this.context !== null ? ",[" + this.context.toString() + "]" : "") + (this.semanticContext !== SemanticContext.NONE ? "," + this.semanticContext.toString() : "") + (this.reachesIntoOuterContext > 0 ? ",up=" + this.reachesIntoOuterContext : "") + ")";
  }
  checkContext(params, config) {
    if ((params.context === null || params.context === void 0) && (config === null || config.context === null || config.context === void 0)) {
      this.context = null;
    }
  }
};

// src/misc/Interval.ts
var Interval = class _Interval {
  static {
    __name(this, "Interval");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INVALID_INTERVAL = new _Interval(-1, -2);
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INTERVAL_POOL_MAX_VALUE = 1e3;
  static cache;
  start;
  stop;
  static {
    _Interval.cache = new Array(_Interval.INTERVAL_POOL_MAX_VALUE + 1);
    _Interval.cache.fill(null);
  }
  constructor(start, stop) {
    this.start = start;
    this.stop = stop;
  }
  /**
   * Creates a new interval from the given values.
   *
   * Interval objects are used readonly so share all with the
   * same single value a==b up to some max size. Use an array as a perfect hash.
   * Return shared object for 0..INTERVAL_POOL_MAX_VALUE or a new
   * Interval object with a..a in it.  On Java.g4, 218623 IntervalSets
   * have a..a (set with 1 element).
   *
   * @param a The start of the interval.
   * @param b The end of the interval (inclusive).
   *
   * @returns A cached or new interval.
   */
  static of(a, b) {
    if (a !== b || a < 0 || a > _Interval.INTERVAL_POOL_MAX_VALUE) {
      return new _Interval(a, b);
    }
    if (_Interval.cache[a] === null) {
      _Interval.cache[a] = new _Interval(a, a);
    }
    return _Interval.cache[a];
  }
  equals(o) {
    if (!(o instanceof _Interval)) {
      return false;
    }
    return this.start === o.start && this.stop === o.stop;
  }
  hashCode() {
    let hash = 23;
    hash = hash * 31 + this.start;
    hash = hash * 31 + this.stop;
    return hash;
  }
  /** Does this start completely before other? Disjoint */
  startsBeforeDisjoint(other) {
    return this.start < other.start && this.stop < other.start;
  }
  /** Does this start at or before other? Nondisjoint */
  startsBeforeNonDisjoint(other) {
    return this.start <= other.start && this.stop >= other.start;
  }
  /** Does this.start start after other.stop? May or may not be disjoint */
  startsAfter(other) {
    return this.start > other.start;
  }
  /** Does this start completely after other? Disjoint */
  startsAfterDisjoint(other) {
    return this.start > other.stop;
  }
  /** Does this start after other? NonDisjoint */
  startsAfterNonDisjoint(other) {
    return this.start > other.start && this.start <= other.stop;
  }
  /** Are both ranges disjoint? I.e., no overlap? */
  disjoint(other) {
    return this.startsBeforeDisjoint(other) || this.startsAfterDisjoint(other);
  }
  /** Are two intervals adjacent such as 0..41 and 42..42? */
  adjacent(other) {
    return this.start === other.stop + 1 || this.stop === other.start - 1;
  }
  properlyContains(other) {
    return other.start >= this.start && other.stop <= this.stop;
  }
  /** Return the interval computed from combining this and other */
  union(other) {
    return _Interval.of(Math.min(this.start, other.start), Math.max(this.stop, other.stop));
  }
  /** Return the interval in common between this and o */
  intersection(other) {
    return _Interval.of(Math.max(this.start, other.start), Math.min(this.stop, other.stop));
  }
  /**
   * Return the interval with elements from this not in other;
   *  other must not be totally enclosed (properly contained)
   *  within this, which would result in two disjoint intervals
   *  instead of the single one returned by this method.
   */
  differenceNotProperlyContained(other) {
    let diff = null;
    if (other.startsBeforeNonDisjoint(this)) {
      diff = _Interval.of(Math.max(this.start, other.stop + 1), this.stop);
    } else if (other.startsAfterNonDisjoint(this)) {
      diff = _Interval.of(this.start, other.start - 1);
    }
    return diff;
  }
  toString() {
    if (this.start === this.stop) {
      return this.start.toString();
    } else {
      return this.start.toString() + ".." + this.stop.toString();
    }
  }
  get length() {
    if (this.stop < this.start) {
      return 0;
    }
    return this.stop - this.start + 1;
  }
};

// src/BaseErrorListener.ts
var BaseErrorListener = class {
  static {
    __name(this, "BaseErrorListener");
  }
  syntaxError(recognizer, offendingSymbol, line, column, msg, e) {
  }
  reportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs) {
  }
  reportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs) {
  }
  reportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs) {
  }
};

// src/ConsoleErrorListener.ts
var ConsoleErrorListener = class _ConsoleErrorListener extends BaseErrorListener {
  static {
    __name(this, "ConsoleErrorListener");
  }
  /**
   * Provides a default instance of {@link ConsoleErrorListener}.
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INSTANCE = new _ConsoleErrorListener();
  syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, _e) {
    console.error("line " + line + ":" + charPositionInLine + " " + msg);
  }
};

// src/ProxyErrorListener.ts
var ProxyErrorListener = class extends BaseErrorListener {
  constructor(delegates) {
    super();
    this.delegates = delegates;
    return this;
  }
  static {
    __name(this, "ProxyErrorListener");
  }
  syntaxError(recognizer, offendingSymbol, line, column, msg, e) {
    this.delegates.forEach((d) => {
      d.syntaxError(recognizer, offendingSymbol, line, column, msg, e);
    });
  }
  reportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs) {
    this.delegates.forEach((d) => {
      d.reportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
    });
  }
  reportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs) {
    this.delegates.forEach((d) => {
      d.reportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs);
    });
  }
  reportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs) {
    this.delegates.forEach((d) => {
      d.reportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs);
    });
  }
};

// src/Recognizer.ts
var Recognizer = class _Recognizer {
  static {
    __name(this, "Recognizer");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static EOF = -1;
  static tokenTypeMapCache = /* @__PURE__ */ new Map();
  static ruleIndexMapCache = /* @__PURE__ */ new Map();
  interpreter;
  #listeners = [ConsoleErrorListener.INSTANCE];
  #stateNumber = -1;
  checkVersion(toolVersion) {
    const runtimeVersion = "4.13.1";
    if (runtimeVersion !== toolVersion) {
      console.error("ANTLR runtime and generated code versions disagree: " + runtimeVersion + "!=" + toolVersion);
    }
  }
  addErrorListener(listener) {
    this.#listeners.push(listener);
  }
  removeErrorListeners() {
    this.#listeners = [];
  }
  removeErrorListener(listener) {
    for (let i = 0; i < this.#listeners.length; i++) {
      if (this.#listeners[i] === listener) {
        this.#listeners.splice(i, 1);
        return;
      }
    }
  }
  getErrorListeners() {
    return this.#listeners;
  }
  getTokenTypeMap() {
    const vocabulary = this.vocabulary;
    let result = _Recognizer.tokenTypeMapCache.get(vocabulary);
    if (!result) {
      result = /* @__PURE__ */ new Map();
      for (let i = 0; i <= this.atn.maxTokenType; i++) {
        const literalName = vocabulary.getLiteralName(i);
        if (literalName) {
          result.set(literalName, i);
        }
        const symbolicName = vocabulary.getSymbolicName(i);
        if (symbolicName) {
          result.set(symbolicName, i);
        }
      }
      result.set("EOF", Token.EOF);
      _Recognizer.tokenTypeMapCache.set(vocabulary, result);
    }
    return result;
  }
  /**
   * Get a map from rule names to rule indexes.
   * <p>Used for XPath and tree pattern compilation.</p>
   */
  getRuleIndexMap() {
    const ruleNames = this.ruleNames;
    let result = _Recognizer.ruleIndexMapCache.get(ruleNames);
    if (!result) {
      result = /* @__PURE__ */ new Map();
      ruleNames.forEach((ruleName, idx) => {
        return result.set(ruleName, idx);
      });
      _Recognizer.ruleIndexMapCache.set(ruleNames, result);
    }
    return result;
  }
  getTokenType(tokenName) {
    const ttype = this.getTokenTypeMap().get(tokenName);
    if (ttype) {
      return ttype;
    }
    return Token.INVALID_TYPE;
  }
  /** What is the error header, normally line/character position information? */
  getErrorHeader(e) {
    const line = e.offendingToken?.line;
    const column = e.offendingToken?.column;
    return "line " + line + ":" + column;
  }
  getErrorListenerDispatch() {
    return new ProxyErrorListener(this.#listeners);
  }
  /**
   * subclass needs to override these if there are semantic predicates or actions
   * that the ATN interp needs to execute
   */
  sempred(_localctx, _ruleIndex, _actionIndex) {
    return true;
  }
  precpred(_localctx, _precedence) {
    return true;
  }
  action(_localctx, _ruleIndex, _actionIndex) {
  }
  get atn() {
    return this.interpreter.atn;
  }
  get state() {
    return this.#stateNumber;
  }
  set state(state) {
    this.#stateNumber = state;
  }
  getSerializedATN() {
    throw new Error("there is no serialized ATN");
  }
  getParseInfo() {
    return null;
  }
};

// src/CommonToken.ts
var CommonToken = class _CommonToken {
  static {
    __name(this, "CommonToken");
  }
  /**
   * An empty tuple which is used as the default value of
   * {@link source} for tokens that do not have a source.
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static EMPTY_SOURCE = [null, null];
  /**
   * This is the backing field for {@link #getTokenSource} and
   * {@link #getInputStream}.
   *
   * <p>
   * These properties share a field to reduce the memory footprint of
   * {@link CommonToken}. Tokens created by a {@link CommonTokenFactory} from
   * the same source and input stream share a reference to the same
   * {@link Pair} containing these values.</p>
   */
  source;
  tokenIndex = -1;
  start = 0;
  stop = 0;
  /**
   * This is the backing field for {@link #getType} and {@link #setType}.
   */
  type = 0;
  /**
   * The (one-based) line number on which the 1st character of this token was.
   */
  line = 0;
  /**
   * The zero-based index of the first character position in its line.
   */
  column = -1;
  // set to invalid position
  /**
   * The token's channel.
   */
  channel = Token.DEFAULT_CHANNEL;
  /**
   * This is the backing field for {@link #getText} when the token text is
   * explicitly set in the constructor or via {@link #setText}.
   *
   * @see #getText()
   */
  #text = null;
  constructor(source, type, channel, start, stop) {
    this.source = source;
    this.type = type;
    this.channel = channel ?? Token.DEFAULT_CHANNEL;
    this.start = start;
    this.stop = stop;
    if (this.source[0] !== null) {
      this.line = source[0].line;
      this.column = source[0]._tokenStartColumn;
    } else {
      this.column = -1;
    }
  }
  get tokenSource() {
    return this.source[0] ?? null;
  }
  get inputStream() {
    return this.source[1] ?? null;
  }
  /**
   * Constructs a new {@link CommonToken} as a copy of another {@link Token}.
   *
   * <p>
   * If {@code oldToken} is also a {@link CommonToken} instance, the newly
   * constructed token will share a reference to the {@link text} field and
   * the {@link Pair} stored in {@link source}. Otherwise, {@link text} will
   * be assigned the result of calling {@link getText}, and {@link source}
   * will be constructed from the result of {@link Token//getTokenSource} and
   * {@link Token//getInputStream}.</p>
   */
  clone() {
    const t = new _CommonToken(this.source, this.type, this.channel, this.start, this.stop);
    t.tokenIndex = this.tokenIndex;
    t.line = this.line;
    t.column = this.column;
    t.#text = this.#text;
    return t;
  }
  cloneWithType(type) {
    const t = new _CommonToken(this.source, type, this.channel, this.start, this.stop);
    t.tokenIndex = this.tokenIndex;
    t.line = this.line;
    t.column = this.column;
    if (type === Token.EOF) {
      t.#text = "";
    }
    return t;
  }
  toString(recognizer) {
    let channelStr = "";
    if (this.channel > 0) {
      channelStr = ",channel=" + this.channel;
    }
    let text = this.text;
    if (text) {
      text = text.replace(/\n/g, "\\n");
      text = text.replace(/\r/g, "\\r");
      text = text.replace(/\t/g, "\\t");
    } else {
      text = "<no text>";
    }
    let typeString = String(this.type);
    if (recognizer) {
      typeString = recognizer.vocabulary.getDisplayName(this.type) ?? "<unknown>";
    }
    return "[@" + this.tokenIndex + "," + this.start + ":" + this.stop + "='" + text + "',<" + typeString + ">" + channelStr + "," + this.line + ":" + this.column + "]";
  }
  get text() {
    if (this.#text !== null) {
      return this.#text;
    }
    const input = this.inputStream;
    if (input === null) {
      return null;
    }
    const n2 = input.size;
    if (this.start < n2 && this.stop < n2) {
      return input.getText(this.start, this.stop);
    } else {
      return "<EOF>";
    }
  }
  set text(text) {
    this.#text = text;
  }
  // WritableToken implementation
  setText(text) {
    this.#text = text;
  }
  setType(ttype) {
    this.type = ttype;
  }
  setLine(line) {
    this.line = line;
  }
  setCharPositionInLine(pos) {
    this.column = pos;
  }
  setChannel(channel) {
    this.channel = channel;
  }
  setTokenIndex(index) {
    this.tokenIndex = index;
  }
};

// src/CommonTokenFactory.ts
var CommonTokenFactory = class _CommonTokenFactory {
  static {
    __name(this, "CommonTokenFactory");
  }
  /**
   * The default {@link CommonTokenFactory} instance.
   *
   * <p>
   * This token factory does not explicitly copy token text when constructing
   * tokens.</p>
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static DEFAULT = new _CommonTokenFactory();
  /**
   * Indicates whether {@link CommonToken#setText} should be called after
   * constructing tokens to explicitly set the text. This is useful for cases
   * where the input stream might not be able to provide arbitrary substrings
   * of text from the input after the lexer creates a token (e.g. the
   * implementation of {@link CharStream#getText} in
   * {@link UnbufferedCharStream} throws an
   * {@link UnsupportedOperationException}). Explicitly setting the token text
   * allows {@link Token#getText} to be called at any time regardless of the
   * input stream implementation.
   *
   * <p>
   * The default value is {@code false} to avoid the performance and memory
   * overhead of copying text for every token unless explicitly requested.</p>
   */
  copyText = false;
  constructor(copyText) {
    this.copyText = copyText ?? false;
  }
  create(source, type, text, channel, start, stop, line, column) {
    const t = new CommonToken(source, type, channel, start, stop);
    t.line = line;
    t.column = column;
    if (text !== null) {
      t.text = text;
    } else if (this.copyText && source[1] !== null) {
      t.text = source[1].getText(start, stop);
    }
    return t;
  }
};

// src/RecognitionException.ts
var RecognitionException = class _RecognitionException extends Error {
  static {
    __name(this, "RecognitionException");
  }
  ctx;
  /**
   * The current {@link Token} when an error occurred. Since not all streams
   * support accessing symbols by index, we have to track the {@link Token}
   * instance itself
   */
  offendingToken = null;
  /**
   * Get the ATN state number the parser was in at the time the error
   * occurred. For {@link NoViableAltException} and
   * {@link LexerNoViableAltException} exceptions, this is the
   * {@link DecisionState} number. For others, it is the state whose outgoing
   * edge we couldn't match.
   */
  offendingState = -1;
  recognizer;
  input;
  constructor(params) {
    super(params.message);
    if (Error.captureStackTrace) {
      Error.captureStackTrace(this, _RecognitionException);
    }
    this.message = params.message;
    this.recognizer = params.recognizer;
    this.input = params.input;
    this.ctx = params.ctx;
    if (this.recognizer !== null) {
      this.offendingState = this.recognizer.state;
    }
  }
  /**
   * Gets the set of input symbols which could potentially follow the
   * previously matched symbol at the time this exception was thrown.
   *
   * <p>If the set of expected tokens is not known and could not be computed,
   * this method returns {@code null}.</p>
   *
   * @returns The set of token types that could potentially follow the current
   * state in the ATN, or {@code null} if the information is not available.
   */
  getExpectedTokens() {
    if (this.recognizer !== null && this.ctx !== null) {
      return this.recognizer.atn.getExpectedTokens(this.offendingState, this.ctx);
    } else {
      return null;
    }
  }
  // <p>If the state number is not known, this method returns -1.</p>
  toString() {
    return this.message;
  }
};

// src/LexerNoViableAltException.ts
var LexerNoViableAltException = class extends RecognitionException {
  static {
    __name(this, "LexerNoViableAltException");
  }
  startIndex;
  deadEndConfigs;
  constructor(lexer, input, startIndex, deadEndConfigs) {
    super({ message: "", recognizer: lexer, input, ctx: null });
    this.startIndex = startIndex;
    this.deadEndConfigs = deadEndConfigs;
  }
  toString() {
    let symbol = "";
    if (this.input && this.startIndex >= 0 && this.startIndex < this.input.size) {
      symbol = this.input.getText(this.startIndex, this.startIndex);
    }
    return "LexerNoViableAltException" + symbol;
  }
};

// src/atn/SingletonPredictionContext.ts
var SingletonPredictionContext = class _SingletonPredictionContext extends PredictionContext {
  static {
    __name(this, "SingletonPredictionContext");
  }
  parent;
  returnState;
  constructor(parent, returnState) {
    let hashCode = 0;
    const hash = new HashCode();
    if (parent !== null) {
      hash.update(parent, returnState);
    } else {
      hash.update(1);
    }
    hashCode = hash.finish();
    super(hashCode);
    this.parent = parent;
    this.returnState = returnState;
  }
  static create(parent, returnState) {
    if (returnState === PredictionContext.EMPTY_RETURN_STATE && parent === null) {
      return PredictionContext.EMPTY;
    } else {
      return new _SingletonPredictionContext(parent, returnState);
    }
  }
  getParent(_index) {
    return this.parent;
  }
  getReturnState(_index) {
    return this.returnState;
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _SingletonPredictionContext)) {
      return false;
    } else if (this.hashCode() !== other.hashCode()) {
      return false;
    } else {
      if (this.returnState !== other.returnState) {
        return false;
      } else if (this.parent == null) {
        return other.parent == null;
      } else {
        return this.parent.equals(other.parent);
      }
    }
  }
  toString() {
    const up = this.parent === null ? "" : this.parent.toString();
    if (up.length === 0) {
      if (this.returnState === PredictionContext.EMPTY_RETURN_STATE) {
        return "$";
      } else {
        return "" + this.returnState;
      }
    } else {
      return "" + this.returnState + " " + up;
    }
  }
  get length() {
    return 1;
  }
};

// src/atn/EmptyPredictionContext.ts
var EmptyPredictionContext = class _EmptyPredictionContext extends SingletonPredictionContext {
  static {
    __name(this, "EmptyPredictionContext");
  }
  /**
   * Represents {@code $} in local context prediction, which means wildcard.
   * {@code *+x = *}.
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static Instance = new _EmptyPredictionContext();
  constructor() {
    super(null, PredictionContext.EMPTY_RETURN_STATE);
  }
  isEmpty() {
    return true;
  }
  getParent(_index) {
    return null;
  }
  getReturnState(_index) {
    return this.returnState;
  }
  equals(other) {
    return this === other;
  }
  toString() {
    return "$";
  }
  static {
    PredictionContext.EMPTY = new _EmptyPredictionContext();
  }
};

// src/misc/HashMap.ts
var HashMap = class {
  static {
    __name(this, "HashMap");
  }
  data;
  hashFunction;
  equalsFunction;
  constructor(hashFunction, equalsFunction) {
    this.data = {};
    this.hashFunction = hashFunction ?? standardHashCodeFunction;
    this.equalsFunction = equalsFunction ?? standardEqualsFunction;
  }
  set(key, value) {
    const hashKey = this.hashFunction(key);
    if (hashKey in this.data) {
      const entries = this.data[hashKey];
      for (const entry of entries) {
        if (this.equalsFunction(key, entry.key)) {
          const oldValue = entry.value;
          entry.value = value;
          return oldValue;
        }
      }
      entries.push({ key, value });
      return value;
    } else {
      this.data[hashKey] = [{ key, value }];
      return value;
    }
  }
  containsKey(key) {
    const hashKey = this.hashFunction(key);
    if (hashKey in this.data) {
      const entries = this.data[hashKey];
      for (const entry of entries) {
        if (this.equalsFunction(key, entry.key)) {
          return true;
        }
      }
    }
    return false;
  }
  get(key) {
    const hashKey = this.hashFunction(key);
    if (hashKey in this.data) {
      const entries = this.data[hashKey];
      for (const entry of entries) {
        if (this.equalsFunction(key, entry.key)) {
          return entry.value;
        }
      }
    }
    return null;
  }
  entries() {
    return Object.keys(this.data).flatMap((key) => {
      return this.data[key];
    }, this);
  }
  getKeys() {
    return this.entries().map((e) => {
      return e.key;
    });
  }
  getValues() {
    return this.entries().map((e) => {
      return e.value;
    });
  }
  toString() {
    const ss = this.entries().map((e) => {
      return "{" + e.key + ":" + e.value + "}";
    });
    return "[" + ss.join(", ") + "]";
  }
  get length() {
    return Object.keys(this.data).map((key) => {
      return this.data[key].length;
    }, this).reduce((accumulator, item) => {
      return accumulator + item;
    }, 0);
  }
};

// src/tree/TerminalNode.ts
var TerminalNode = class {
  static {
    __name(this, "TerminalNode");
  }
  parent = null;
  symbol;
  constructor(symbol) {
    this.symbol = symbol;
  }
  getChild(_i) {
    return null;
  }
  getSymbol() {
    return this.symbol;
  }
  getPayload() {
    return this.symbol;
  }
  getSourceInterval() {
    if (this.symbol === null) {
      return Interval.INVALID_INTERVAL;
    }
    const tokenIndex = this.symbol.tokenIndex;
    return new Interval(tokenIndex, tokenIndex);
  }
  getChildCount() {
    return 0;
  }
  accept(visitor) {
    return visitor.visitTerminal(this);
  }
  getText() {
    return this.symbol?.text ?? "";
  }
  toString() {
    if (this.symbol?.type === Token.EOF) {
      return "<EOF>";
    } else {
      return this.symbol?.text ?? "";
    }
  }
  toStringTree() {
    return this.toString();
  }
};

// src/tree/ErrorNode.ts
var ErrorNode = class extends TerminalNode {
  static {
    __name(this, "ErrorNode");
  }
  accept(visitor) {
    return visitor.visitErrorNode(this);
  }
};

// src/tree/Trees.ts
var Trees = class _Trees {
  static {
    __name(this, "Trees");
  }
  /**
   * Print out a whole tree in LISP form. {@link getNodeText} is used on the
   * node payloads to get the text for the nodes.  Detect
   * parse trees and extract data appropriately.
   */
  static toStringTree(tree, ruleNames, recog) {
    ruleNames = ruleNames ?? null;
    recog = recog ?? null;
    if (recog !== null) {
      ruleNames = recog.ruleNames;
    }
    let s = _Trees.getNodeText(tree, ruleNames);
    s = escapeWhitespace(s, false);
    const c = tree.getChildCount();
    if (c === 0) {
      return s;
    }
    let res = "(" + s + " ";
    if (c > 0) {
      s = _Trees.toStringTree(tree.getChild(0), ruleNames);
      res = res.concat(s);
    }
    for (let i = 1; i < c; i++) {
      s = _Trees.toStringTree(tree.getChild(i), ruleNames);
      res = res.concat(" " + s);
    }
    res = res.concat(")");
    return res;
  }
  static getNodeText(t, ruleNames, recog) {
    ruleNames = ruleNames ?? null;
    recog = recog ?? null;
    if (recog !== null) {
      ruleNames = recog.ruleNames;
    }
    if (ruleNames !== null) {
      if (t instanceof RuleContext) {
        const context = t.ruleContext;
        const altNumber = context.getAltNumber();
        if (altNumber !== 0) {
          return ruleNames[t.ruleIndex] + ":" + altNumber;
        }
        return ruleNames[t.ruleIndex];
      } else if (t instanceof ErrorNode) {
        return t.toString();
      } else if (t instanceof TerminalNode) {
        if (t.symbol !== null) {
          return t.symbol.text;
        }
      }
    }
    const payload = t.getPayload();
    if (isToken(payload)) {
      return payload.text;
    }
    return String(t.getPayload());
  }
  /**
   * Return ordered list of all children of this node
   */
  static getChildren(t) {
    const list = [];
    for (let i = 0; i < t.getChildCount(); i++) {
      list.push(t.getChild(i));
    }
    return list;
  }
  /**
   * Return a list of all ancestors of this node.  The first node of
   * list is the root and the last is the parent of this node.
   */
  static getAncestors(t) {
    if (t.parent === null) {
      return [];
    }
    let ancestors = [];
    let p = t.parent;
    while (p !== null) {
      ancestors = [p].concat(ancestors);
      p = p.parent;
    }
    return ancestors;
  }
  static findAllTokenNodes(t, ttype) {
    return _Trees.findAllNodes(t, ttype, true);
  }
  static findAllRuleNodes(t, ruleIndex) {
    return _Trees.findAllNodes(t, ruleIndex, false);
  }
  static findAllNodes(t, index, findTokens) {
    const nodes = [];
    _Trees.doFindAllNodes(t, index, findTokens, nodes);
    return nodes;
  }
  static descendants(t) {
    let nodes = [t];
    for (let i = 0; i < t.getChildCount(); i++) {
      nodes = nodes.concat(_Trees.descendants(t.getChild(i)));
    }
    return nodes;
  }
  static doFindAllNodes(t, index, findTokens, nodes) {
    if (findTokens && t instanceof TerminalNode) {
      if (t.symbol?.type === index) {
        nodes.push(t);
      }
    } else if (!findTokens && t instanceof RuleContext) {
      if (t.ruleIndex === index) {
        nodes.push(t);
      }
    }
    for (let i = 0; i < t.getChildCount(); i++) {
      _Trees.doFindAllNodes(t.getChild(i), index, findTokens, nodes);
    }
  }
};

// src/RuleContext.ts
var RuleContext = class {
  static {
    __name(this, "RuleContext");
  }
  // TODO: move to ParserRuleContext.
  children = null;
  /**
   * What state invoked the rule associated with this context?
   *  The "return address" is the followState of invokingState
   *  If parent is null, this should be -1 this context object represents
   *  the start rule.
   */
  invokingState;
  #parent = null;
  /**
   * A rule context is a record of a single rule invocation. It knows
   * which context invoked it, if any. If there is no parent context, then
   * naturally the invoking state is not valid.  The parent link
   * provides a chain upwards from the current rule invocation to the root
   * of the invocation tree, forming a stack. We actually carry no
   * information about the rule associated with this context (except
   * when parsing). We keep only the state number of the invoking state from
   * the ATN submachine that invoked this. Contrast this with the s
   * pointer inside ParserRuleContext that tracks the current state
   * being "executed" for the current rule.
   *
   * The parent contexts are useful for computing lookahead sets and
   * getting error information.
   *
   * These objects are used during parsing and prediction.
   * For the special case of parsers, we use the subclass
   * ParserRuleContext.
   *
   * @see ParserRuleContext
   */
  constructor(parent, invokingState) {
    this.parent = parent ?? null;
    this.invokingState = invokingState ?? -1;
  }
  get parent() {
    return this.#parent;
  }
  set parent(parent) {
    this.#parent = parent;
  }
  depth() {
    let n2 = 0;
    let p = this;
    while (p !== null) {
      p = p.parent;
      n2 += 1;
    }
    return n2;
  }
  /**
   * A context is empty if there is no invoking state; meaning nobody call
   * current context.
   */
  isEmpty() {
    return this.invokingState === -1;
  }
  // satisfy the ParseTree / SyntaxTree interface
  getSourceInterval() {
    return Interval.INVALID_INTERVAL;
  }
  get ruleContext() {
    return this;
  }
  get ruleIndex() {
    return -1;
  }
  getPayload() {
    return this;
  }
  /**
   * Return the combined text of all child nodes. This method only considers
   * tokens which have been added to the parse tree.
   * <p>
   * Since tokens on hidden channels (e.g. whitespace or comments) are not
   * added to the parse trees, they will not appear in the output of this
   * method.
   */
  getText() {
    if (!this.children || this.getChildCount() === 0) {
      return "";
    } else {
      return this.children.map((child) => {
        return child.getText();
      }).join("");
    }
  }
  /**
   * For rule associated with this parse tree internal node, return
   * the outer alternative number used to match the input. Default
   * implementation does not compute nor store this alt num. Create
   * a subclass of ParserRuleContext with backing field and set
   * option contextSuperClass.
   * to set it.
   */
  getAltNumber() {
    return ATN.INVALID_ALT_NUMBER;
  }
  /**
   * Set the outer alternative number for this context node. Default
   * implementation does nothing to avoid backing field overhead for
   * trees that don't need it.  Create
   * a subclass of ParserRuleContext with backing field and set
   * option contextSuperClass.
   */
  setAltNumber(_altNumber) {
  }
  getChild(_i) {
    return null;
  }
  getChildCount() {
    return 0;
  }
  accept(visitor) {
    return visitor.visitChildren(this);
  }
  toStringTree(...args) {
    if (args.length === 1) {
      return Trees.toStringTree(this, null, args[0]);
    }
    return Trees.toStringTree(this, args[0], args[1]);
  }
  toString(ruleNames, stop) {
    ruleNames = ruleNames ?? null;
    stop = stop ?? null;
    let p = this;
    let s = "[";
    while (p !== null && p !== stop) {
      if (ruleNames === null) {
        if (!p.isEmpty()) {
          s += p.invokingState;
        }
      } else {
        const ri = p.ruleIndex;
        const ruleName = ri >= 0 && ri < ruleNames.length ? ruleNames[ri] : "" + ri;
        s += ruleName;
      }
      if (p.parent !== null && (ruleNames !== null || !p.parent.isEmpty())) {
        s += " ";
      }
      p = p.parent;
    }
    s += "]";
    return s;
  }
};

// src/ParserRuleContext.ts
var ParserRuleContext = class _ParserRuleContext extends RuleContext {
  static {
    __name(this, "ParserRuleContext");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static EMPTY = new _ParserRuleContext();
  start = null;
  stop = null;
  /**
   * The exception that forced this rule to return. If the rule successfully
   * completed, this is {@code null}.
   */
  exception = null;
  constructor(parent, invokingStateNumber) {
    if (!parent) {
      super();
    } else {
      super(parent, invokingStateNumber);
    }
    this.start = null;
    this.stop = null;
  }
  get parent() {
    return super.parent;
  }
  set parent(parent) {
    super.parent = parent;
  }
  // COPY a ctx (I'm deliberately not using copy constructor)
  copyFrom(ctx) {
    this.parent = ctx.parent;
    this.invokingState = ctx.invokingState;
    this.children = null;
    this.start = ctx.start;
    this.stop = ctx.stop;
    if (ctx.children) {
      this.children = [];
      ctx.children.forEach((child) => {
        if (child instanceof ErrorNode) {
          this.children.push(child);
          child.parent = this;
        }
      }, this);
    }
  }
  // Double dispatch methods for listeners
  enterRule(_listener) {
  }
  exitRule(_listener) {
  }
  /**
   * Add a parse tree node to this as a child.  Works for
   *  internal and leaf nodes. Does not set parent link;
   *  other add methods must do that. Other addChild methods
   *  call this.
   *
   *  We cannot set the parent pointer of the incoming node
   *  because the existing interfaces do not have a setParent()
   *  method and I don't want to break backward compatibility for this.
   */
  addAnyChild(t) {
    if (this.children == null) {
      this.children = [];
    }
    this.children.push(t);
    return t;
  }
  addChild(child) {
    return this.addAnyChild(child);
  }
  /**
   * Used by enterOuterAlt to toss out a RuleContext previously added as
   * we entered a rule. If we have // label, we will need to remove
   * generic ruleContext object.
   */
  removeLastChild() {
    if (this.children !== null) {
      this.children.pop();
    }
  }
  addTokenNode(token) {
    const node = new TerminalNode(token);
    this.addAnyChild(node);
    node.parent = this;
    return node;
  }
  /**
   * Add a child to this node based upon badToken.  It
   *  creates a ErrorNode rather than using
   *  {@link Parser#createErrorNode(ParserRuleContext, Token)}. I'm leaving this
   *  in for compatibility but the parser doesn't use this anymore.
   *
   * @deprecated
   */
  addErrorNode(errorNode) {
    errorNode.parent = this;
    return this.addAnyChild(errorNode);
  }
  getChild(i, type) {
    if (this.children === null || i < 0 || i >= this.children.length) {
      return null;
    }
    if (!type) {
      return this.children[i];
    } else {
      for (const child of this.children) {
        if (child instanceof type) {
          if (i === 0) {
            return child;
          } else {
            i -= 1;
          }
        }
      }
      return null;
    }
  }
  getToken(ttype, i) {
    if (this.children === null || i < 0 || i >= this.children.length) {
      return null;
    }
    for (const child of this.children) {
      if (child instanceof TerminalNode) {
        if (child.symbol?.type === ttype) {
          if (i === 0) {
            return child;
          } else {
            i -= 1;
          }
        }
      }
    }
    return null;
  }
  getTokens(ttype) {
    if (this.children === null) {
      return [];
    } else {
      const tokens = [];
      for (const child of this.children) {
        if (child instanceof TerminalNode) {
          if (child.symbol?.type === ttype) {
            tokens.push(child);
          }
        }
      }
      return tokens;
    }
  }
  getRuleContext(index, ctxType) {
    return this.getChild(index, ctxType);
  }
  getRuleContexts(ctxType) {
    if (this.children === null) {
      return [];
    } else {
      const contexts = [];
      for (const child of this.children) {
        if (child instanceof ctxType) {
          contexts.push(child);
        }
      }
      return contexts;
    }
  }
  getChildCount() {
    if (this.children === null) {
      return 0;
    } else {
      return this.children.length;
    }
  }
  getSourceInterval() {
    if (this.start === null || this.stop === null) {
      return Interval.INVALID_INTERVAL;
    } else {
      return new Interval(this.start.tokenIndex, this.stop.tokenIndex);
    }
  }
};

// src/atn/PredictionContextUtils.ts
var predictionContextFromRuleContext = /* @__PURE__ */ __name((atn, outerContext) => {
  if (outerContext === void 0 || outerContext === null) {
    outerContext = ParserRuleContext.EMPTY;
  }
  if (outerContext.parent === null || outerContext === ParserRuleContext.EMPTY) {
    return PredictionContext.EMPTY;
  }
  const parent = predictionContextFromRuleContext(atn, outerContext.parent);
  const state = atn.states[outerContext.invokingState];
  const transition = state.transitions[0];
  return SingletonPredictionContext.create(parent, transition.followState.stateNumber);
}, "predictionContextFromRuleContext");
var getCachedPredictionContext = /* @__PURE__ */ __name((context, contextCache, visited) => {
  if (context.isEmpty()) {
    return context;
  }
  let existing = visited.get(context) || null;
  if (existing !== null) {
    return existing;
  }
  existing = contextCache.get(context);
  if (existing !== null) {
    visited.set(context, existing);
    return existing;
  }
  let changed = false;
  let parents = [];
  for (let i = 0; i < parents.length; i++) {
    const parent = getCachedPredictionContext(context.getParent(i), contextCache, visited);
    if (changed || parent !== context.getParent(i)) {
      if (!changed) {
        parents = [];
        for (let j = 0; j < context.length; j++) {
          parents[j] = context.getParent(j);
        }
        changed = true;
      }
      parents[i] = parent;
    }
  }
  if (!changed) {
    contextCache.add(context);
    visited.set(context, context);
    return context;
  }
  let updated = null;
  if (parents.length === 0) {
    updated = PredictionContext.EMPTY;
  } else if (parents.length === 1) {
    updated = SingletonPredictionContext.create(parents[0], context.getReturnState(0));
  } else {
    updated = new ArrayPredictionContext(parents, context.returnStates);
  }
  contextCache.add(updated);
  visited.set(updated, updated);
  visited.set(context, updated);
  return updated;
}, "getCachedPredictionContext");
var merge = /* @__PURE__ */ __name((a, b, rootIsWildcard, mergeCache) => {
  if (a === b) {
    return a;
  }
  if (a instanceof SingletonPredictionContext && b instanceof SingletonPredictionContext) {
    return mergeSingletons(a, b, rootIsWildcard, mergeCache);
  }
  if (rootIsWildcard) {
    if (a instanceof EmptyPredictionContext) {
      return a;
    }
    if (b instanceof EmptyPredictionContext) {
      return b;
    }
  }
  if (a instanceof SingletonPredictionContext) {
    a = new ArrayPredictionContext([a.parent], [a.returnState]);
  }
  if (b instanceof SingletonPredictionContext) {
    b = new ArrayPredictionContext([b.parent], [b.returnState]);
  }
  return mergeArrays(a, b, rootIsWildcard, mergeCache);
}, "merge");
var mergeArrays = /* @__PURE__ */ __name((a, b, rootIsWildcard, mergeCache) => {
  if (mergeCache !== null) {
    let previous = mergeCache.get(a, b);
    if (previous !== null) {
      if (PredictionContext.trace_atn_sim) {
        console.log("mergeArrays a=" + a + ",b=" + b + " -> previous");
      }
      return previous;
    }
    previous = mergeCache.get(b, a);
    if (previous !== null) {
      if (PredictionContext.trace_atn_sim) {
        console.log("mergeArrays a=" + a + ",b=" + b + " -> previous");
      }
      return previous;
    }
  }
  let i = 0;
  let j = 0;
  let k = 0;
  let mergedReturnStates = new Array(a.returnStates.length + b.returnStates.length).fill(0);
  let mergedParents = new Array(a.returnStates.length + b.returnStates.length).fill(null);
  while (i < a.returnStates.length && j < b.returnStates.length) {
    const aParent = a.parents[i];
    const bParent = b.parents[j];
    if (a.returnStates[i] === b.returnStates[j]) {
      const payload = a.returnStates[i];
      const bothDollars = payload === PredictionContext.EMPTY_RETURN_STATE && aParent === null && bParent === null;
      const axAx = aParent !== null && bParent !== null && aParent === bParent;
      if (bothDollars || axAx) {
        mergedParents[k] = aParent;
        mergedReturnStates[k] = payload;
      } else {
        mergedParents[k] = merge(aParent, bParent, rootIsWildcard, mergeCache);
        mergedReturnStates[k] = payload;
      }
      i += 1;
      j += 1;
    } else if (a.returnStates[i] < b.returnStates[j]) {
      mergedParents[k] = aParent;
      mergedReturnStates[k] = a.returnStates[i];
      i += 1;
    } else {
      mergedParents[k] = bParent;
      mergedReturnStates[k] = b.returnStates[j];
      j += 1;
    }
    k += 1;
  }
  if (i < a.returnStates.length) {
    for (let p = i; p < a.returnStates.length; p++) {
      mergedParents[k] = a.parents[p];
      mergedReturnStates[k] = a.returnStates[p];
      k += 1;
    }
  } else {
    for (let p = j; p < b.returnStates.length; p++) {
      mergedParents[k] = b.parents[p];
      mergedReturnStates[k] = b.returnStates[p];
      k += 1;
    }
  }
  if (k < mergedParents.length) {
    if (k === 1) {
      const aNew = SingletonPredictionContext.create(mergedParents[0], mergedReturnStates[0]);
      if (mergeCache !== null) {
        mergeCache.set(a, b, aNew);
      }
      return aNew;
    }
    mergedParents = mergedParents.slice(0, k);
    mergedReturnStates = mergedReturnStates.slice(0, k);
  }
  const merged = new ArrayPredictionContext(mergedParents, mergedReturnStates);
  if (merged.equals(a)) {
    if (mergeCache !== null) {
      mergeCache.set(a, b, a);
    }
    if (PredictionContext.trace_atn_sim) {
      console.log("mergeArrays a=" + a + ",b=" + b + " -> a");
    }
    return a;
  }
  if (merged.equals(b)) {
    if (mergeCache !== null) {
      mergeCache.set(a, b, b);
    }
    if (PredictionContext.trace_atn_sim) {
      console.log("mergeArrays a=" + a + ",b=" + b + " -> b");
    }
    return b;
  }
  combineCommonParents(mergedParents);
  if (mergeCache !== null) {
    mergeCache.set(a, b, merged);
  }
  if (PredictionContext.trace_atn_sim) {
    console.log("mergeArrays a=" + a + ",b=" + b + " -> " + merged);
  }
  return merged;
}, "mergeArrays");
var combineCommonParents = /* @__PURE__ */ __name((parents) => {
  const uniqueParents = new HashMap();
  for (const parent of parents) {
    if (parent) {
      if (!uniqueParents.containsKey(parent)) {
        uniqueParents.set(parent, parent);
      }
    }
  }
  for (let q = 0; q < parents.length; q++) {
    if (parents[q]) {
      parents[q] = uniqueParents.get(parents[q]);
    }
  }
}, "combineCommonParents");
var mergeSingletons = /* @__PURE__ */ __name((a, b, rootIsWildcard, mergeCache) => {
  if (mergeCache !== null) {
    let previous = mergeCache.get(a, b);
    if (previous !== null) {
      return previous;
    }
    previous = mergeCache.get(b, a);
    if (previous !== null) {
      return previous;
    }
  }
  const rootMerge = mergeRoot(a, b, rootIsWildcard);
  if (rootMerge !== null) {
    if (mergeCache !== null) {
      mergeCache.set(a, b, rootMerge);
    }
    return rootMerge;
  }
  if (a.returnState === b.returnState) {
    const parent = merge(a.parent, b.parent, rootIsWildcard, mergeCache);
    if (parent === a.parent) {
      return a;
    }
    if (parent === b.parent) {
      return b;
    }
    const spc = SingletonPredictionContext.create(parent, a.returnState);
    if (mergeCache !== null) {
      mergeCache.set(a, b, spc);
    }
    return spc;
  } else {
    let singleParent = null;
    if (a === b || a.parent !== null && a.parent === b.parent) {
      singleParent = a.parent;
    }
    if (singleParent !== null) {
      const payloads2 = [a.returnState, b.returnState];
      if (a.returnState > b.returnState) {
        payloads2[0] = b.returnState;
        payloads2[1] = a.returnState;
      }
      const parents2 = [singleParent, singleParent];
      const apc = new ArrayPredictionContext(parents2, payloads2);
      if (mergeCache !== null) {
        mergeCache.set(a, b, apc);
      }
      return apc;
    }
    const payloads = [a.returnState, b.returnState];
    let parents = [a.parent, b.parent];
    if (a.returnState > b.returnState) {
      payloads[0] = b.returnState;
      payloads[1] = a.returnState;
      parents = [b.parent, a.parent];
    }
    const aNew = new ArrayPredictionContext(parents, payloads);
    if (mergeCache !== null) {
      mergeCache.set(a, b, aNew);
    }
    return aNew;
  }
}, "mergeSingletons");
var mergeRoot = /* @__PURE__ */ __name((a, b, rootIsWildcard) => {
  if (rootIsWildcard) {
    if (a === PredictionContext.EMPTY) {
      return PredictionContext.EMPTY;
    }
    if (b === PredictionContext.EMPTY) {
      return PredictionContext.EMPTY;
    }
  } else {
    if (a === PredictionContext.EMPTY && b === PredictionContext.EMPTY) {
      return PredictionContext.EMPTY;
    } else if (a === PredictionContext.EMPTY) {
      const payloads = [
        b.returnState,
        PredictionContext.EMPTY_RETURN_STATE
      ];
      const parents = [b.parent, null];
      return new ArrayPredictionContext(parents, payloads);
    } else if (b === PredictionContext.EMPTY) {
      const payloads = [a.returnState, PredictionContext.EMPTY_RETURN_STATE];
      const parents = [a.parent, null];
      return new ArrayPredictionContext(parents, payloads);
    }
  }
  return null;
}, "mergeRoot");
var getAllContextNodes = /* @__PURE__ */ __name((context, nodes, visited) => {
  if (nodes === null) {
    nodes = [];
    return getAllContextNodes(context, nodes, visited);
  } else if (visited === null) {
    visited = new HashMap();
    return getAllContextNodes(context, nodes, visited);
  } else {
    if (context === null || visited.containsKey(context)) {
      return nodes;
    }
    visited.set(context, context);
    nodes.push(context);
    for (let i = 0; i < context.length; i++) {
      getAllContextNodes(context.getParent(i), nodes, visited);
    }
    return nodes;
  }
}, "getAllContextNodes");

// src/misc/BitSet.ts
var BitSet = class {
  static {
    __name(this, "BitSet");
  }
  data;
  /**
   * Creates a new bit set. All bits are initially `false`.
   *
   * @param data Optional initial data.
   */
  constructor(data) {
    if (data) {
      this.data = new Uint32Array(data.map((value) => {
        return value >>> 0;
      }));
    } else {
      this.data = new Uint32Array(1);
    }
  }
  /**
   * @returns an iterator over all set bits.
   */
  [Symbol.iterator]() {
    const length = this.data.length;
    let currentIndex = 0;
    let currentWord = this.data[currentIndex];
    const words = this.data;
    return {
      [Symbol.iterator]() {
        return this;
      },
      next: () => {
        while (currentIndex < length) {
          if (currentWord !== 0) {
            const t = currentWord & -currentWord;
            const value = (currentIndex << 5) + this.bitCount(t - 1);
            currentWord ^= t;
            return { done: false, value };
          } else {
            currentIndex++;
            if (currentIndex < length) {
              currentWord = words[currentIndex];
            }
          }
        }
        return { done: true, value: void 0 };
      }
    };
  }
  /**
   * Sets a single bit or all of the bits in this `BitSet` to `false`.
   *
   * @param index the index of the bit to be cleared, or undefined to clear all bits.
   */
  clear(index) {
    if (index === void 0) {
      this.data = new Uint32Array();
    } else {
      this.resize(index);
      this.data[index >>> 5] &= ~(1 << index);
    }
  }
  /**
   * Performs a logical **OR** of this bit set with the bit set argument. This bit set is modified so that a bit in it
   * has the value `true` if and only if it either already had the value `true` or the corresponding bit in the bit
   * set argument has the value `true`.
   *
   * @param set the bit set to be ORed with.
   */
  or(set) {
    const minCount = Math.min(this.data.length, set.data.length);
    for (let k = 0; k < minCount; ++k) {
      this.data[k] |= set.data[k];
    }
    if (this.data.length < set.data.length) {
      this.resize((set.data.length << 5) - 1);
      const c = set.data.length;
      for (let k = minCount; k < c; ++k) {
        this.data[k] = set.data[k];
      }
    }
  }
  /**
   * Returns the value of the bit with the specified index. The value is `true` if the bit with the index `bitIndex`
   * is currently set in this `BitSet`; otherwise, the result is `false`.
   *
   * @param index the bit index
   *
   * @returns the value of the bit with the specified index.
   */
  get(index) {
    if (index < 0) {
      throw new RangeError("index cannot be negative");
    }
    const slot = index >>> 5;
    if (slot >= this.data.length) {
      return false;
    }
    return (this.data[slot] & 1 << index % 32) !== 0;
  }
  /**
   * @returns the number of set bits.
   */
  get length() {
    let result = 0;
    const c = this.data.length;
    const w = this.data;
    for (let i = 0; i < c; i++) {
      result += this.bitCount(w[i]);
    }
    return result;
  }
  /**
   * @returns an array with indices of set bits.
   */
  values() {
    const result = new Array(this.length);
    let pos = 0;
    const length = this.data.length;
    for (let k = 0; k < length; ++k) {
      let w = this.data[k];
      while (w !== 0) {
        const t = w & -w;
        result[pos++] = (k << 5) + this.bitCount(t - 1);
        w ^= t;
      }
    }
    return result;
  }
  /**
   * @returns the index of the first bit that is set to `true` that occurs on or after the specified starting index.
   * If no such bit exists then undefined is returned.
   *
   * @param fromIndex the index to start checking from (inclusive)
   */
  nextSetBit(fromIndex) {
    if (fromIndex < 0) {
      throw new RangeError("index cannot be negative");
    }
    for (const index of this) {
      if (index > fromIndex) {
        return index;
      }
    }
    return void 0;
  }
  /**
   * Sets the bit at the specified index to `true`.
   *
   * @param index a bit index
   */
  set(index) {
    if (index < 0) {
      throw new RangeError("index cannot be negative");
    }
    this.resize(index);
    this.data[index >>> 5] |= 1 << index % 32;
  }
  /**
   * @returns a string representation of this bit set.
   */
  toString() {
    return "{" + this.values().join(", ") + "}";
  }
  resize(index) {
    const count = index + 32 >>> 5;
    if (count <= this.data.length) {
      return;
    }
    const data = new Uint32Array(count);
    data.set(this.data);
    data.fill(0, this.data.length);
    this.data = data;
  }
  bitCount(v) {
    v = v - (v >> 1 & 1431655765);
    v = (v & 858993459) + (v >> 2 & 858993459);
    v = v + (v >> 4) & 252645135;
    v = v + (v >> 8);
    v = v + (v >> 16);
    return v & 63;
  }
};

// src/atn/ATNConfigSet.ts
var hashATNConfig = /* @__PURE__ */ __name((c) => {
  return c.hashCodeForConfigSet();
}, "hashATNConfig");
var equalATNConfigs = /* @__PURE__ */ __name((a, b) => {
  if (a === b) {
    return true;
  } else if (a === null || b === null) {
    return false;
  } else {
    return a.equalsForConfigSet(b);
  }
}, "equalATNConfigs");
var ATNConfigSet = class _ATNConfigSet {
  static {
    __name(this, "ATNConfigSet");
  }
  // Track the elements as they are added to the set; supports get(i)///
  configs = [];
  /**
   * Used in parser and lexer. In lexer, it indicates we hit a pred
   * while computing a closure operation. Don't make a DFA state from this
   */
  hasSemanticContext = false;
  dipsIntoOuterContext = false;
  /**
   * Indicates that this configuration set is part of a full context
   * LL prediction. It will be used to determine how to merge $. With SLL
   * it's a wildcard whereas it is not for LL context merge
   */
  fullCtx;
  uniqueAlt = 0;
  /**
   * The reason that we need this is because we don't want the hash map to use
   * the standard hash code and equals. We need all configurations with the
   * same
   * {@code (s,i,_,semctx)} to be equal. Unfortunately, this key effectively
   * doubles
   * the number of objects associated with ATNConfigs. The other solution is
   * to
   * use a hash table that lets us specify the equals/hashCode operation.
   * All configs but hashed by (s, i, _, pi) not including context. Wiped out
   * when we go readonly as this set becomes a DFA state
   */
  configLookup = new HashSet(hashATNConfig, equalATNConfigs);
  conflictingAlts = null;
  /**
   * Indicates that the set of configurations is read-only. Do not
   * allow any code to manipulate the set; DFA states will point at
   * the sets and they must not change. This does not protect the other
   * fields; in particular, conflictingAlts is set after
   * we've made this readonly
   */
  readOnly = false;
  cachedHashCode = -1;
  // TODO: add iterator for configs.
  constructor(fullCtx) {
    this.fullCtx = fullCtx ?? true;
  }
  /**
   * Adding a new config means merging contexts with existing configs for
   * {@code (s, i, pi, _)}, where {@code s} is the
   * {@link ATNConfig//state}, {@code i} is the {@link ATNConfig//alt}, and
   * {@code pi} is the {@link ATNConfig//semanticContext}. We use
   * {@code (s,i,pi)} as key.
   *
   * <p>This method updates {@link dipsIntoOuterContext} and
   * {@link hasSemanticContext} when necessary.</p>
   */
  add(config, mergeCache) {
    if (mergeCache === void 0) {
      mergeCache = null;
    }
    if (this.readOnly) {
      throw new Error("This set is readonly");
    }
    if (config.semanticContext !== SemanticContext.NONE) {
      this.hasSemanticContext = true;
    }
    if (config.reachesIntoOuterContext > 0) {
      this.dipsIntoOuterContext = true;
    }
    const existing = this.configLookup.add(config);
    if (existing === config) {
      this.cachedHashCode = -1;
      this.configs.push(config);
      return true;
    }
    const rootIsWildcard = !this.fullCtx;
    const merged = merge(existing.context, config.context, rootIsWildcard, mergeCache);
    existing.reachesIntoOuterContext = Math.max(existing.reachesIntoOuterContext, config.reachesIntoOuterContext);
    if (config.precedenceFilterSuppressed) {
      existing.precedenceFilterSuppressed = true;
    }
    existing.context = merged;
    return true;
  }
  getStates() {
    const states = new HashSet();
    for (const config of this.configs) {
      states.add(config.state);
    }
    return states;
  }
  getPredicates() {
    const preds = [];
    for (const config of this.configs) {
      if (config.semanticContext !== SemanticContext.NONE) {
        preds.push(config.semanticContext);
      }
    }
    return preds;
  }
  optimizeConfigs(interpreter) {
    if (this.readOnly) {
      throw new Error("This set is readonly");
    }
    if (this.configLookup.length === 0) {
      return;
    }
    for (const config of this.configs) {
      config.context = interpreter.getCachedContext(config.context);
    }
  }
  addAll(coll) {
    for (const config of coll) {
      this.add(config, null);
    }
    return false;
  }
  equals(other) {
    return this === other || other instanceof _ATNConfigSet && equalArrays(this.configs, other.configs) && this.fullCtx === other.fullCtx && this.uniqueAlt === other.uniqueAlt && this.conflictingAlts === other.conflictingAlts && this.hasSemanticContext === other.hasSemanticContext && this.dipsIntoOuterContext === other.dipsIntoOuterContext;
  }
  hashCode() {
    const hash = new HashCode();
    hash.update(this.configs);
    return hash.finish();
  }
  updateHashCode(hash) {
    if (this.readOnly) {
      if (this.cachedHashCode === -1) {
        this.cachedHashCode = this.hashCode();
      }
      hash.update(this.cachedHashCode);
    } else {
      hash.update(this.hashCode());
    }
  }
  isEmpty() {
    return this.configs.length === 0;
  }
  contains(item) {
    if (this.configLookup === null) {
      throw new Error("This method is not implemented for readonly sets.");
    }
    return this.configLookup.has(item);
  }
  containsFast(item) {
    if (this.configLookup === null) {
      throw new Error("This method is not implemented for readonly sets.");
    }
    return this.configLookup.has(item);
  }
  clear() {
    if (this.readOnly) {
      throw new Error("This set is readonly");
    }
    this.configs = [];
    this.cachedHashCode = -1;
    this.configLookup = new HashSet();
  }
  setReadonly(readOnly) {
    this.readOnly = readOnly;
    if (readOnly) {
      this.configLookup = new HashSet();
    }
  }
  toString() {
    return arrayToString(this.configs) + (this.hasSemanticContext ? ",hasSemanticContext=" + this.hasSemanticContext : "") + (this.uniqueAlt !== ATN.INVALID_ALT_NUMBER ? ",uniqueAlt=" + this.uniqueAlt : "") + (this.conflictingAlts !== null ? ",conflictingAlts=" + this.conflictingAlts : "") + (this.dipsIntoOuterContext ? ",dipsIntoOuterContext" : "");
  }
  get items() {
    return this.configs;
  }
  get length() {
    return this.configs.length;
  }
  getAlts() {
    let alts = new BitSet();
    for (let config of this.configs) {
      alts.set(config.alt);
    }
    return alts;
  }
};

// src/dfa/DFAState.ts
var DFAState = class _DFAState {
  static {
    __name(this, "DFAState");
  }
  stateNumber = -1;
  configs = new ATNConfigSet();
  /**
   * {@code edges[symbol]} points to target of symbol. Shift up by 1 so (-1)
   *  {@link Token#EOF} maps to {@code edges[0]}.
   */
  edges = null;
  isAcceptState = false;
  /**
   * if accept state, what ttype do we match or alt do we predict?
   *  This is set to {@link ATN#INVALID_ALT_NUMBER} when {@link #predicates}{@code !=null} or
   *  {@link #requiresFullContext}.
   */
  prediction = -1;
  lexerActionExecutor = null;
  /**
   * Indicates that this state was created during SLL prediction that
   * discovered a conflict between the configurations in the state. Future
   * {@link ParserATNSimulator#execATN} invocations immediately jumped doing
   * full context prediction if this field is true.
   */
  requiresFullContext = false;
  /**
   * During SLL parsing, this is a list of predicates associated with the
   *  ATN configurations of the DFA state. When we have predicates,
   *  {@link #requiresFullContext} is {@code false} since full context prediction evaluates predicates
   *  on-the-fly. If this is not null, then {@link #prediction} is
   *  {@link ATN#INVALID_ALT_NUMBER}.
   *
   *  <p>We only use these for non-{@link #requiresFullContext} but conflicting states. That
   *  means we know from the context (it's $ or we don't dip into outer
   *  context) that it's an ambiguity not a conflict.</p>
   *
   *  <p>This list is computed by {@link ParserATNSimulator#predicateDFAState}.</p>
   */
  predicates = null;
  constructor(stateNumberOrConfigs) {
    if (stateNumberOrConfigs) {
      if (typeof stateNumberOrConfigs === "number") {
        this.stateNumber = stateNumberOrConfigs;
      } else {
        this.configs = stateNumberOrConfigs;
      }
    }
  }
  hashCode() {
    const hash = new HashCode();
    hash.update(this.configs.configs);
    return hash.finish();
  }
  /**
   * Two {@link DFAState} instances are equal if their ATN configuration sets
   * are the same. This method is used to see if a state already exists.
   *
   * <p>Because the number of alternatives and number of ATN configurations are
   * finite, there is a finite number of DFA states that can be processed.
   * This is necessary to show that the algorithm terminates.</p>
   *
   * <p>Cannot test the DFA state numbers here because in
   * {@link ParserATNSimulator#addDFAState} we need to know if any other state
   * exists that has this exact set of ATN configurations. The
   * {@link #stateNumber} is irrelevant.</p>
   *
   * @param o tbd
   *
   * @returns tbd
   */
  equals(o) {
    if (this === o) {
      return true;
    }
    if (!(o instanceof _DFAState)) {
      return false;
    }
    if (this.configs === null) {
      if (o.configs === null) {
        return true;
      }
      return false;
    }
    return this.configs.equals(o.configs);
  }
  toString() {
    let buf = "";
    buf += this.stateNumber;
    buf += ":";
    buf += this.configs ? this.configs.toString() : "";
    if (this.isAcceptState) {
      buf += "=>";
      if (this.predicates) {
        buf += arrayToString(this.predicates);
      } else {
        buf += this.prediction;
      }
    }
    return buf.toString();
  }
};
((DFAState2) => {
  class PredPrediction {
    static {
      __name(this, "PredPrediction");
    }
    pred;
    // never null; at least SemanticContext.NONE
    alt;
    constructor(pred, alt) {
      this.alt = alt;
      this.pred = pred;
    }
    toString() {
      return `(${this.pred}, ${this.alt})`;
    }
  }
  DFAState2.PredPrediction = PredPrediction;
  ;
})(DFAState || (DFAState = {}));

// src/atn/ATNSimulator.ts
var ATNSimulator = class {
  static {
    __name(this, "ATNSimulator");
  }
  /** Must distinguish between missing edge and edge we know leads nowhere */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static ERROR = new DFAState(2147483647);
  atn;
  /**
   * The context cache maps all PredictionContext objects that are ==
   * to a single cached copy. This cache is shared across all contexts
   * in all ATNConfigs in all DFA states.  We rebuild each ATNConfigSet
   * to use only cached nodes/graphs in addDFAState(). We don't want to
   * fill this during closure() since there are lots of contexts that
   * pop up but are not used ever again. It also greatly slows down closure().
   *
   * <p>This cache makes a huge difference in memory and a little bit in speed.
   * For the Java grammar on java.*, it dropped the memory requirements
   * at the end from 25M to 16M. We don't store any of the full context
   * graphs in the DFA because they are limited to local context only,
   * but apparently there's a lot of repetition there as well. We optimize
   * the config contexts before storing the config set in the DFA states
   * by literally rebuilding them with cached subgraphs only.</p>
   *
   * <p>I tried a cache for use during closure operations, that was
   * whacked after each adaptivePredict(). It cost a little bit
   * more time I think and doesn't save on the overall footprint
   * so it's not worth the complexity.</p>
   */
  sharedContextCache = null;
  constructor(atn, sharedContextCache) {
    this.atn = atn;
    this.sharedContextCache = sharedContextCache;
    return this;
  }
  /**
   * Clear the DFA cache used by the current instance. Since the DFA cache may
   * be shared by multiple ATN simulators, this method may affect the
   * performance (but not accuracy) of other parsers which are being used
   * concurrently.
   *
   * @throws UnsupportedOperationException if the current instance does not
   * support clearing the DFA.
   */
  clearDFA() {
    throw new Error("This ATN simulator does not support clearing the DFA.");
  }
  getCachedContext(context) {
    if (this.sharedContextCache === null) {
      return context;
    }
    const visited = new HashMap();
    return getCachedPredictionContext(context, this.sharedContextCache, visited);
  }
  getSharedContextCache() {
    return this.sharedContextCache;
  }
};

// src/atn/OrderedATNConfigSet.ts
var OrderedATNConfigSet = class extends ATNConfigSet {
  static {
    __name(this, "OrderedATNConfigSet");
  }
  constructor() {
    super();
    this.configLookup = new HashSet();
  }
};

// src/atn/ATNState.ts
var ATNState = class _ATNState {
  static {
    __name(this, "ATNState");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INVALID_STATE_NUMBER = -1;
  atn;
  stateNumber;
  ruleIndex;
  epsilonOnlyTransitions;
  nextTokenWithinRule;
  transitions;
  constructor() {
    this.atn = null;
    this.ruleIndex = 0;
    this.epsilonOnlyTransitions = false;
    this.transitions = [];
    this.nextTokenWithinRule = null;
  }
  get stateType() {
    return _ATNState.INVALID_STATE_NUMBER;
  }
  toString() {
    return `${this.stateNumber}`;
  }
  hashCode() {
    return this.stateNumber;
  }
  equals(other) {
    if (other instanceof _ATNState) {
      return this.stateNumber === other.stateNumber;
    } else {
      return false;
    }
  }
  isNonGreedyExitState() {
    return false;
  }
  addTransition(trans, index) {
    if (index === void 0) {
      index = -1;
    }
    if (this.transitions.length === 0) {
      this.epsilonOnlyTransitions = trans.isEpsilon;
    } else if (this.epsilonOnlyTransitions !== trans.isEpsilon) {
      this.epsilonOnlyTransitions = false;
    }
    if (index === -1) {
      this.transitions.push(trans);
    } else {
      this.transitions.splice(index, 1, trans);
    }
  }
};

// src/atn/ATNStateType.ts
var ATNStateType = {
  INVALID_TYPE: 0,
  BASIC: 1,
  RULE_START: 2,
  BLOCK_START: 3,
  PLUS_BLOCK_START: 4,
  STAR_BLOCK_START: 5,
  TOKEN_START: 6,
  RULE_STOP: 7,
  BLOCK_END: 8,
  STAR_LOOP_BACK: 9,
  STAR_LOOP_ENTRY: 10,
  PLUS_LOOP_BACK: 11,
  LOOP_END: 12
};

// src/atn/RuleStopState.ts
var RuleStopState = class extends ATNState {
  static {
    __name(this, "RuleStopState");
  }
  get stateType() {
    return ATNStateType.RULE_STOP;
  }
};

// src/atn/DecisionState.ts
var DecisionState = class extends ATNState {
  static {
    __name(this, "DecisionState");
  }
  decision = -1;
  nonGreedy = false;
};

// src/atn/LexerATNConfig.ts
var LexerATNConfig = class _LexerATNConfig extends ATNConfig {
  static {
    __name(this, "LexerATNConfig");
  }
  /**
   * This is the backing field for {@link #getLexerActionExecutor}.
   */
  lexerActionExecutor;
  passedThroughNonGreedyDecision;
  constructor(params, config) {
    super(params, config);
    this.lexerActionExecutor = params.lexerActionExecutor ?? config?.lexerActionExecutor ?? null;
    this.passedThroughNonGreedyDecision = config !== null ? this.checkNonGreedyDecision(config, this.state) : false;
    return this;
  }
  updateHashCode(hash) {
    hash.update(
      this.state.stateNumber,
      this.alt,
      this.context,
      this.semanticContext,
      this.passedThroughNonGreedyDecision,
      this.lexerActionExecutor
    );
  }
  equals(other) {
    return this === other || other instanceof _LexerATNConfig && this.passedThroughNonGreedyDecision === other.passedThroughNonGreedyDecision && (this.lexerActionExecutor ? this.lexerActionExecutor.equals(other.lexerActionExecutor) : !other.lexerActionExecutor) && super.equals(other);
  }
  hashCodeForConfigSet() {
    return this.hashCode();
  }
  equalsForConfigSet(other) {
    return this.equals(other);
  }
  checkNonGreedyDecision(source, target) {
    return source.passedThroughNonGreedyDecision || target instanceof DecisionState && target.nonGreedy;
  }
};

// src/atn/LexerAction.ts
var LexerAction = class {
  static {
    __name(this, "LexerAction");
  }
  actionType;
  isPositionDependent;
  constructor(action) {
    this.actionType = action;
    this.isPositionDependent = false;
  }
  hashCode() {
    const hash = new HashCode();
    this.updateHashCode(hash);
    return hash.finish();
  }
  updateHashCode(hash) {
    hash.update(this.actionType);
  }
  equals(other) {
    return this === other;
  }
};

// src/atn/LexerIndexedCustomAction.ts
var LexerIndexedCustomAction = class _LexerIndexedCustomAction extends LexerAction {
  static {
    __name(this, "LexerIndexedCustomAction");
  }
  offset;
  action;
  constructor(offset, action) {
    super(action.actionType);
    this.offset = offset;
    this.action = action;
    this.isPositionDependent = true;
  }
  /**
   * <p>This method calls {@link execute} on the result of {@link getAction}
   * using the provided {@code lexer}.</p>
   */
  execute(lexer) {
    this.action.execute(lexer);
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.offset, this.action);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerIndexedCustomAction)) {
      return false;
    } else {
      return this.offset === other.offset && this.action === other.action;
    }
  }
};

// src/atn/LexerActionExecutor.ts
var LexerActionExecutor = class _LexerActionExecutor extends LexerAction {
  static {
    __name(this, "LexerActionExecutor");
  }
  lexerActions;
  cachedHashCode;
  /**
   * Represents an executor for a sequence of lexer actions which traversed during
   * the matching operation of a lexer rule (token).
   *
   * <p>The executor tracks position information for position-dependent lexer actions
   * efficiently, ensuring that actions appearing only at the end of the rule do
   * not cause bloating of the {@link DFA} created for the lexer.</p>
   */
  constructor(lexerActions) {
    super(-1);
    this.lexerActions = lexerActions === null ? [] : lexerActions;
    this.cachedHashCode = HashCode.hashStuff(lexerActions);
    return this;
  }
  /**
   * Creates a {@link LexerActionExecutor} which executes the actions for
   * the input {@code lexerActionExecutor} followed by a specified
   * {@code lexerAction}.
   *
   * @param lexerActionExecutor The executor for actions already traversed by
   * the lexer while matching a token within a particular
   * {@link LexerATNConfig}. If this is {@code null}, the method behaves as
   * though it were an empty executor.
   * @param lexerAction The lexer action to execute after the actions
   * specified in {@code lexerActionExecutor}.
   *
   * @returns {LexerActionExecutor} A {@link LexerActionExecutor} for executing the combine actions
   * of {@code lexerActionExecutor} and {@code lexerAction}.
   */
  static append(lexerActionExecutor, lexerAction) {
    if (lexerActionExecutor === null) {
      return new _LexerActionExecutor([lexerAction]);
    }
    const lexerActions = lexerActionExecutor.lexerActions.concat([lexerAction]);
    return new _LexerActionExecutor(lexerActions);
  }
  /**
   * Creates a {@link LexerActionExecutor} which encodes the current offset
   * for position-dependent lexer actions.
   *
   * <p>Normally, when the executor encounters lexer actions where
   * {@link LexerAction//isPositionDependent} returns {@code true}, it calls
   * {@link IntStream//seek} on the input {@link CharStream} to set the input
   * position to the <em>end</em> of the current token. This behavior provides
   * for efficient DFA representation of lexer actions which appear at the end
   * of a lexer rule, even when the lexer rule matches a variable number of
   * characters.</p>
   *
   * <p>Prior to traversing a match transition in the ATN, the current offset
   * from the token start index is assigned to all position-dependent lexer
   * actions which have not already been assigned a fixed offset. By storing
   * the offsets relative to the token start index, the DFA representation of
   * lexer actions which appear in the middle of tokens remains efficient due
   * to sharing among tokens of the same length, regardless of their absolute
   * position in the input stream.</p>
   *
   * <p>If the current executor already has offsets assigned to all
   * position-dependent lexer actions, the method returns {@code this}.</p>
   *
   * @param offset The current offset to assign to all position-dependent
   * lexer actions which do not already have offsets assigned.
   *
   * @returns {LexerActionExecutor} A {@link LexerActionExecutor} which stores input stream offsets
   * for all position-dependent lexer actions.
   */
  fixOffsetBeforeMatch(offset) {
    let updatedLexerActions = null;
    for (let i = 0; i < this.lexerActions.length; i++) {
      if (this.lexerActions[i].isPositionDependent && !(this.lexerActions[i] instanceof LexerIndexedCustomAction)) {
        if (updatedLexerActions === null) {
          updatedLexerActions = this.lexerActions.concat([]);
        }
        updatedLexerActions[i] = new LexerIndexedCustomAction(
          offset,
          this.lexerActions[i]
        );
      }
    }
    if (updatedLexerActions === null) {
      return this;
    } else {
      return new _LexerActionExecutor(updatedLexerActions);
    }
  }
  /**
   * Execute the actions encapsulated by this executor within the context of a
   * particular {@link Lexer}.
   *
   * <p>This method calls {@link IntStream//seek} to set the position of the
   * {@code input} {@link CharStream} prior to calling
   * {@link LexerAction//execute} on a position-dependent action. Before the
   * method returns, the input position will be restored to the same position
   * it was in when the method was invoked.</p>
   *
   * @param lexer The lexer instance.
   * @param input The input stream which is the source for the current token.
   * When this method is called, the current {@link IntStream//index} for
   * {@code input} should be the start of the following token, i.e. 1
   * character past the end of the current token.
   * @param startIndex The token start index. This value may be passed to
   * {@link IntStream//seek} to set the {@code input} position to the beginning
   * of the token.
   */
  execute(lexer, input, startIndex) {
    if (input === void 0 || startIndex === void 0) {
      return;
    }
    let requiresSeek = false;
    const stopIndex = input.index;
    try {
      for (const lexerAction of this.lexerActions) {
        let action = lexerAction;
        if (lexerAction instanceof LexerIndexedCustomAction) {
          const offset = lexerAction.offset;
          input.seek(startIndex + offset);
          action = lexerAction.action;
          requiresSeek = startIndex + offset !== stopIndex;
        } else if (lexerAction.isPositionDependent) {
          input.seek(stopIndex);
          requiresSeek = false;
        }
        action.execute(lexer);
      }
    } finally {
      if (requiresSeek) {
        input.seek(stopIndex);
      }
    }
  }
  hashCode() {
    return this.cachedHashCode;
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerActionExecutor)) {
      return false;
    } else if (this.cachedHashCode !== other.cachedHashCode) {
      return false;
    } else if (this.lexerActions.length !== other.lexerActions.length) {
      return false;
    } else {
      const numActions = this.lexerActions.length;
      for (let idx = 0; idx < numActions; ++idx) {
        if (!this.lexerActions[idx].equals(other.lexerActions[idx])) {
          return false;
        }
      }
      return true;
    }
  }
};

// src/dfa/DFASerializer.ts
var DFASerializer = class {
  static {
    __name(this, "DFASerializer");
  }
  dfa;
  vocabulary;
  constructor(dfa, vocabulary) {
    this.dfa = dfa;
    this.vocabulary = vocabulary;
  }
  toString() {
    if (this.dfa.s0 === null) {
      return "";
    }
    let buf = "";
    const states = this.dfa.getStates();
    for (const s of states) {
      let n2 = 0;
      if (s.edges !== null) {
        n2 = s.edges.length;
        for (let i = 0; i < n2; i++) {
          const t = s.edges[i];
          if (t !== null && t.stateNumber !== 2147483647) {
            buf += this.getStateString(s);
            const label = this.getEdgeLabel(i);
            buf += "-";
            buf += label;
            buf += "->";
            buf += this.getStateString(t);
            buf += "\n";
          }
        }
      }
    }
    return buf;
  }
  getEdgeLabel(i) {
    const name = this.vocabulary.getDisplayName(i - 1);
    return `${name}`;
  }
  getStateString(s) {
    const n2 = s.stateNumber;
    const baseStateStr = (s.isAcceptState ? ":" : "") + "s" + n2 + (s.requiresFullContext ? "^" : "");
    if (s.isAcceptState) {
      if (s.predicates !== null) {
        return `${baseStateStr}=>${s.predicates.toString()}`;
      } else {
        return `${baseStateStr}=>${s.prediction}`;
      }
    } else {
      return `${baseStateStr}`;
    }
  }
};

// src/Vocabulary.ts
var Vocabulary = class _Vocabulary {
  static {
    __name(this, "Vocabulary");
  }
  static EMPTY_NAMES = [];
  /**
   * Gets an empty {@link Vocabulary} instance.
   *
   * <p>
   * No literal or symbol names are assigned to token types, so
   * {@link #getDisplayName(int)} returns the numeric value for all tokens
   * except {@link Token#EOF}.</p>
   */
  static EMPTY_VOCABULARY = new _Vocabulary(_Vocabulary.EMPTY_NAMES, _Vocabulary.EMPTY_NAMES, _Vocabulary.EMPTY_NAMES);
  maxTokenType;
  literalNames;
  symbolicNames;
  displayNames;
  /**
   * Constructs a new instance of {@link Vocabulary} from the specified
   * literal, symbolic, and display token names.
   *
   * @param literalNames The literal names assigned to tokens, or {@code null}
   * if no literal names are assigned.
   * @param symbolicNames The symbolic names assigned to tokens, or
   * {@code null} if no symbolic names are assigned.
   * @param displayNames The display names assigned to tokens, or {@code null}
   * to use the values in {@code literalNames} and {@code symbolicNames} as
   * the source of display names, as described in
   * {@link #getDisplayName(int)}.
   */
  constructor(literalNames, symbolicNames, displayNames) {
    this.literalNames = literalNames != null ? literalNames : _Vocabulary.EMPTY_NAMES;
    this.symbolicNames = symbolicNames != null ? symbolicNames : _Vocabulary.EMPTY_NAMES;
    this.displayNames = displayNames != null ? displayNames : _Vocabulary.EMPTY_NAMES;
    this.maxTokenType = Math.max(this.displayNames.length, Math.max(
      this.literalNames.length,
      this.symbolicNames.length
    )) - 1;
  }
  /**
   * Returns a {@link Vocabulary} instance from the specified set of token
   * names. This method acts as a compatibility layer for the single
   * {@code tokenNames} array generated by previous releases of ANTLR.
   *
   * <p>The resulting vocabulary instance returns {@code null} for
   * {@link #getLiteralName(int)} and {@link #getSymbolicName(int)}, and the
   * value from {@code tokenNames} for the display names.</p>
   *
   * @param tokenNames The token names, or {@code null} if no token names are
   * available.
   * @returns A {@link Vocabulary} instance which uses {@code tokenNames} for
   * the display names of tokens.
   */
  static fromTokenNames(tokenNames) {
    if (tokenNames == null || tokenNames.length === 0) {
      return _Vocabulary.EMPTY_VOCABULARY;
    }
    const literalNames = [...tokenNames];
    const symbolicNames = [...tokenNames];
    for (let i = 0; i < tokenNames.length; i++) {
      const tokenName = tokenNames[i];
      if (tokenName == null) {
        continue;
      }
      if (tokenName?.length > 0) {
        const firstChar = tokenName.charAt(0);
        if (firstChar === "'") {
          symbolicNames[i] = null;
          continue;
        } else if (firstChar.toUpperCase() === firstChar) {
          literalNames[i] = null;
          continue;
        }
      }
      literalNames[i] = null;
      symbolicNames[i] = null;
    }
    return new _Vocabulary(literalNames, symbolicNames, tokenNames);
  }
  getMaxTokenType() {
    return this.maxTokenType;
  }
  getLiteralName(tokenType) {
    if (tokenType >= 0 && tokenType < this.literalNames.length) {
      return this.literalNames[tokenType];
    }
    return null;
  }
  getSymbolicName(tokenType) {
    if (tokenType >= 0 && tokenType < this.symbolicNames.length) {
      return this.symbolicNames[tokenType];
    }
    if (tokenType === Token.EOF) {
      return "EOF";
    }
    return null;
  }
  getDisplayName(tokenType) {
    if (tokenType >= 0 && tokenType < this.displayNames.length) {
      const displayName = this.displayNames[tokenType];
      if (displayName != null) {
        return displayName;
      }
    }
    const literalName = this.getLiteralName(tokenType);
    if (literalName != null) {
      return literalName;
    }
    const symbolicName = this.getSymbolicName(tokenType);
    if (symbolicName != null) {
      return symbolicName;
    }
    return `${tokenType}`;
  }
  getLiteralNames() {
    return this.literalNames;
  }
  getSymbolicNames() {
    return this.symbolicNames;
  }
  getDisplayNames() {
    return this.displayNames;
  }
};

// src/dfa/LexerDFASerializer.ts
var LexerDFASerializer = class extends DFASerializer {
  static {
    __name(this, "LexerDFASerializer");
  }
  constructor(dfa) {
    super(dfa, Vocabulary.EMPTY_VOCABULARY);
  }
  getEdgeLabel = (i) => {
    return "'" + String.fromCharCode(i) + "'";
  };
};

// src/atn/StarLoopEntryState.ts
var StarLoopEntryState = class extends DecisionState {
  static {
    __name(this, "StarLoopEntryState");
  }
  // This is always set during ATN deserialization
  loopBackState;
  /**
   * Indicates whether this state can benefit from a precedence DFA during SLL
   * decision making.
   *
   * This is a computed property that is calculated during ATN deserialization
   * and stored for use in {@link ParserATNSimulator} and
   * {@link ParserInterpreter}.
   *
   * @see `DFA.isPrecedenceDfa`
   */
  precedenceRuleDecision;
  constructor() {
    super();
    this.precedenceRuleDecision = false;
  }
  get stateType() {
    return ATNStateType.STAR_LOOP_ENTRY;
  }
};

// src/dfa/DFA.ts
var DFA = class {
  static {
    __name(this, "DFA");
  }
  /**
   * A set of all DFA states. Use {@link Map} so we can get old state back
   *  ({@link Set} only allows you to see if it's there).
   */
  states = new HashSet();
  s0 = null;
  decision;
  /** From which ATN state did we create this DFA? */
  atnStartState;
  /**
   * {@code true} if this DFA is for a precedence decision; otherwise,
   * {@code false}. This is the backing field for {@link #isPrecedenceDfa}.
   */
  precedenceDfa;
  constructor(atnStartState, decision) {
    this.atnStartState = atnStartState;
    this.decision = decision ?? 0;
    let precedenceDfa = false;
    if (atnStartState instanceof StarLoopEntryState) {
      if (atnStartState.precedenceRuleDecision) {
        precedenceDfa = true;
        const precedenceState = new DFAState();
        precedenceState.edges = [];
        precedenceState.isAcceptState = false;
        precedenceState.requiresFullContext = false;
        this.s0 = precedenceState;
      }
    }
    this.precedenceDfa = precedenceDfa;
  }
  /**
   * Gets whether this DFA is a precedence DFA. Precedence DFAs use a special
   * start state {@link #s0} which is not stored in {@link #states}. The
   * {@link DFAState#edges} array for this start state contains outgoing edges
   * supplying individual start states corresponding to specific precedence
   * values.
   *
    @returns `true` if this is a precedence DFA; otherwise,
   * {@code false}.
   * @see Parser#getPrecedence()
   */
  isPrecedenceDfa = () => {
    return this.precedenceDfa;
  };
  /**
   * Get the start state for a specific precedence value.
   *
   * @param precedence The current precedence.
    @returns The start state corresponding to the specified precedence, or
   * {@code null} if no start state exists for the specified precedence.
   *
   * @throws IllegalStateException if this is not a precedence DFA.
   * @see #isPrecedenceDfa()
   */
  getPrecedenceStartState = (precedence) => {
    if (!this.isPrecedenceDfa()) {
      throw new Error(`Only precedence DFAs may contain a precedence start state.`);
    }
    if (!this.s0 || !this.s0.edges || precedence < 0 || precedence >= this.s0.edges.length) {
      return null;
    }
    return this.s0.edges[precedence];
  };
  /**
   * Set the start state for a specific precedence value.
   *
   * @param precedence The current precedence.
   * @param startState The start state corresponding to the specified
   * precedence.
   *
   * @throws IllegalStateException if this is not a precedence DFA.
   * @see #isPrecedenceDfa()
   */
  setPrecedenceStartState = (precedence, startState) => {
    if (!this.isPrecedenceDfa()) {
      throw new Error(`Only precedence DFAs may contain a precedence start state.`);
    }
    if (precedence < 0 || !this.s0?.edges) {
      return;
    }
    if (precedence >= this.s0.edges.length) {
      const start = this.s0.edges.length;
      this.s0.edges.length = precedence + 1;
      this.s0.edges.fill(null, start, precedence);
    }
    this.s0.edges[precedence] = startState;
  };
  /**
   * Sets whether this is a precedence DFA.
   *
   * @param precedenceDfa {@code true} if this is a precedence DFA; otherwise,
   * {@code false}
   *
   * @throws UnsupportedOperationException if {@code precedenceDfa} does not
   * match the value of {@link #isPrecedenceDfa} for the current DFA.
   *
   * @deprecated This method no longer performs any action.
   */
  setPrecedenceDfa(precedenceDfa) {
    if (precedenceDfa !== this.isPrecedenceDfa()) {
      throw new Error(
        `The precedenceDfa field cannot change after a DFA is constructed.`
      );
    }
  }
  /**
   * @returns a list of all states in this DFA, ordered by state number.
   */
  getStates() {
    const result = this.states.values();
    result.sort((o1, o2) => {
      return o1.stateNumber - o2.stateNumber;
    });
    return result;
  }
  toString(vocabulary) {
    if (!vocabulary) {
      return this.toString(Vocabulary.EMPTY_VOCABULARY);
    }
    if (this.s0 === null) {
      return "";
    }
    const serializer = new DFASerializer(this, vocabulary);
    return serializer.toString() ?? "";
  }
  toLexerString() {
    if (this.s0 === null) {
      return "";
    }
    const serializer = new LexerDFASerializer(this);
    return serializer.toString() ?? "";
  }
};

// src/atn/LexerATNSimulator.ts
var LexerATNSimulator = class _LexerATNSimulator extends ATNSimulator {
  static {
    __name(this, "LexerATNSimulator");
  }
  static MIN_DFA_EDGE = 0;
  static MAX_DFA_EDGE = 127;
  // forces unicode to stay in ATN
  static debug = false;
  static dfa_debug = false;
  decisionToDFA;
  recog = null;
  /**
   * The current token's starting index into the character stream.
   *  Shared across DFA to ATN simulation in case the ATN fails and the
   *  DFA did not have a previous accept state. In this case, we use the
   *  ATN-generated exception object.
   */
  startIndex = -1;
  /** line number 1..n within the input */
  line = 1;
  /** The index of the character relative to the beginning of the line 0..n-1 */
  column = 0;
  mode = Lexer.DEFAULT_MODE;
  /** Used during DFA/ATN exec to record the most recent accept configuration info */
  prevAccept = new _LexerATNSimulator.SimState();
  /**
   * When we hit an accept state in either the DFA or the ATN, we
   * have to notify the character stream to start buffering characters
   * via {@link IntStream//mark} and record the current state. The current sim state
   * includes the current index into the input, the current line,
   * and current character position in that line. Note that the Lexer is
   * tracking the starting line and characterization of the token. These
   * variables track the "state" of the simulator when it hits an accept state.
   *
   * <p>We track these variables separately for the DFA and ATN simulation
   * because the DFA simulation often has to fail over to the ATN
   * simulation. If the ATN simulation fails, we need the DFA to fall
   * back to its previously accepted state, if any. If the ATN succeeds,
   * then the ATN does the accept and the DFA simulator that invoked it
   * can simply return the predicted token type.</p>
   */
  constructor(recog, atn, decisionToDFA, sharedContextCache) {
    super(atn, sharedContextCache);
    this.decisionToDFA = decisionToDFA;
    this.recog = recog;
  }
  copyState(simulator) {
    this.column = simulator.column;
    this.line = simulator.line;
    this.mode = simulator.mode;
    this.startIndex = simulator.startIndex;
  }
  match(input, mode) {
    this.mode = mode;
    const mark = input.mark();
    try {
      this.startIndex = input.index;
      this.prevAccept.reset();
      const dfa = this.decisionToDFA[mode];
      if (dfa.s0 === null) {
        return this.matchATN(input);
      } else {
        return this.execATN(input, dfa.s0);
      }
    } finally {
      input.release(mark);
    }
  }
  reset() {
    this.prevAccept.reset();
    this.startIndex = -1;
    this.line = 1;
    this.column = 0;
    this.mode = Lexer.DEFAULT_MODE;
  }
  getDFA(mode) {
    return this.decisionToDFA[mode];
  }
  // Get the text matched so far for the current token.
  getText(input) {
    return input.getText(this.startIndex, input.index - 1);
  }
  consume(input) {
    const curChar = input.LA(1);
    if (curChar === "\n".charCodeAt(0)) {
      this.line += 1;
      this.column = 0;
    } else {
      this.column += 1;
    }
    input.consume();
  }
  getTokenName(tt) {
    if (tt === -1) {
      return "EOF";
    } else {
      return "'" + String.fromCharCode(tt) + "'";
    }
  }
  clearDFA() {
    for (let d = 0; d < this.decisionToDFA.length; d++) {
      this.decisionToDFA[d] = new DFA(this.atn.getDecisionState(d), d);
    }
  }
  matchATN(input) {
    const startState = this.atn.modeToStartState[this.mode];
    if (_LexerATNSimulator.debug) {
      console.log("matchATN mode " + this.mode + " start: " + startState);
    }
    const old_mode = this.mode;
    const s0_closure = this.computeStartState(input, startState);
    const suppressEdge = s0_closure.hasSemanticContext;
    s0_closure.hasSemanticContext = false;
    const next = this.addDFAState(s0_closure);
    if (!suppressEdge) {
      this.decisionToDFA[this.mode].s0 = next;
    }
    const predict = this.execATN(input, next);
    if (_LexerATNSimulator.debug) {
      console.log("DFA after matchATN: " + this.decisionToDFA[old_mode].toLexerString());
    }
    return predict;
  }
  execATN(input, ds0) {
    if (_LexerATNSimulator.debug) {
      console.log("start state closure=" + ds0.configs);
    }
    if (ds0.isAcceptState) {
      this.captureSimState(this.prevAccept, input, ds0);
    }
    let t = input.LA(1);
    let s = ds0;
    for (; ; ) {
      if (_LexerATNSimulator.debug) {
        console.log("execATN loop starting closure: " + s.configs);
      }
      let target = this.getExistingTargetState(s, t);
      if (target === null) {
        target = this.computeTargetState(input, s, t);
      }
      if (target === ATNSimulator.ERROR) {
        break;
      }
      if (t !== Token.EOF) {
        this.consume(input);
      }
      if (target.isAcceptState) {
        this.captureSimState(this.prevAccept, input, target);
        if (t === Token.EOF) {
          break;
        }
      }
      t = input.LA(1);
      s = target;
    }
    return this.failOrAccept(this.prevAccept, input, s.configs, t);
  }
  /**
   * Get an existing target state for an edge in the DFA. If the target state
   * for the edge has not yet been computed or is otherwise not available,
   * this method returns {@code null}.
   *
   * @param s The current DFA state
   * @param t The next input symbol
   * @returns The existing target DFA state for the given input symbol
   * {@code t}, or {@code null} if the target state for this edge is not
   * already cached
   */
  getExistingTargetState(s, t) {
    if (s.edges === null || t < _LexerATNSimulator.MIN_DFA_EDGE || t > _LexerATNSimulator.MAX_DFA_EDGE) {
      return null;
    }
    let target = s.edges[t - _LexerATNSimulator.MIN_DFA_EDGE];
    if (target === void 0) {
      target = null;
    }
    if (_LexerATNSimulator.debug && target !== null) {
      console.log("reuse state " + s.stateNumber + " edge to " + target.stateNumber);
    }
    return target;
  }
  /**
   * Compute a target state for an edge in the DFA, and attempt to add the
   * computed state and corresponding edge to the DFA.
   *
   * @param input The input stream
   * @param s The current DFA state
   * @param t The next input symbol
   *
   * @returns The computed target DFA state for the given input symbol
   * {@code t}. If {@code t} does not lead to a valid DFA state, this method
   * returns {@link ERROR}.
   */
  computeTargetState(input, s, t) {
    const reach = new OrderedATNConfigSet();
    this.getReachableConfigSet(input, s.configs, reach, t);
    if (reach.items.length === 0) {
      if (!reach.hasSemanticContext) {
        this.addDFAEdge(s, t, ATNSimulator.ERROR);
      }
      return ATNSimulator.ERROR;
    }
    return this.addDFAEdge(s, t, null, reach);
  }
  failOrAccept(prevAccept, input, reach, t) {
    if (prevAccept.dfaState !== null) {
      const lexerActionExecutor = prevAccept.dfaState.lexerActionExecutor;
      this.accept(
        input,
        lexerActionExecutor,
        this.startIndex,
        prevAccept.index,
        prevAccept.line,
        prevAccept.column
      );
      return prevAccept.dfaState.prediction;
    } else {
      if (t === Token.EOF && input.index === this.startIndex) {
        return Token.EOF;
      }
      throw new LexerNoViableAltException(this.recog, input, this.startIndex, reach);
    }
  }
  /**
   * Given a starting configuration set, figure out all ATN configurations
   * we can reach upon input {@code t}. Parameter {@code reach} is a return
   * parameter.
   */
  getReachableConfigSet(input, closure, reach, t) {
    let skipAlt = ATN.INVALID_ALT_NUMBER;
    for (const cfg of closure.items) {
      const currentAltReachedAcceptState = cfg.alt === skipAlt;
      if (currentAltReachedAcceptState && cfg.passedThroughNonGreedyDecision) {
        continue;
      }
      if (_LexerATNSimulator.debug) {
        console.log("testing %s at %s\n", this.getTokenName(t), cfg.toString(this.recog, true));
      }
      for (const trans of cfg.state.transitions) {
        const target = this.getReachableTarget(trans, t);
        if (target !== null) {
          let lexerActionExecutor = cfg.lexerActionExecutor;
          if (lexerActionExecutor !== null) {
            lexerActionExecutor = lexerActionExecutor.fixOffsetBeforeMatch(input.index - this.startIndex);
          }
          const treatEofAsEpsilon = t === Token.EOF;
          const config = new LexerATNConfig({ state: target, lexerActionExecutor }, cfg);
          if (this.closure(
            input,
            config,
            reach,
            currentAltReachedAcceptState,
            true,
            treatEofAsEpsilon
          )) {
            skipAlt = cfg.alt;
          }
        }
      }
    }
  }
  accept(input, lexerActionExecutor, startIndex, index, line, charPos) {
    if (_LexerATNSimulator.debug) {
      console.log("ACTION %s\n", lexerActionExecutor);
    }
    input.seek(index);
    this.line = line;
    this.column = charPos;
    if (lexerActionExecutor !== null && this.recog !== null) {
      lexerActionExecutor.execute(this.recog, input, startIndex);
    }
  }
  getReachableTarget(trans, t) {
    if (trans.matches(t, 0, Lexer.MAX_CHAR_VALUE)) {
      return trans.target;
    } else {
      return null;
    }
  }
  computeStartState(input, p) {
    const initialContext = PredictionContext.EMPTY;
    const configs = new OrderedATNConfigSet();
    for (let i = 0; i < p.transitions.length; i++) {
      const target = p.transitions[i].target;
      const cfg = new LexerATNConfig({ state: target, alt: i + 1, context: initialContext }, null);
      this.closure(input, cfg, configs, false, false, false);
    }
    return configs;
  }
  /**
   * Since the alternatives within any lexer decision are ordered by
   * preference, this method stops pursuing the closure as soon as an accept
   * state is reached. After the first accept state is reached by depth-first
   * search from {@code config}, all other (potentially reachable) states for
   * this rule would have a lower priority.
   *
   * @returns {boolean} {@code true} if an accept state is reached, otherwise
   * {@code false}.
   */
  closure(input, config, configs, currentAltReachedAcceptState, speculative, treatEofAsEpsilon) {
    let cfg = null;
    if (_LexerATNSimulator.debug) {
      console.log("closure(" + config.toString(this.recog, true) + ")");
    }
    if (config.state instanceof RuleStopState) {
      if (_LexerATNSimulator.debug) {
        if (this.recog !== null) {
          console.log("closure at %s rule stop %s\n", this.recog.ruleNames[config.state.ruleIndex], config);
        } else {
          console.log("closure at rule stop %s\n", config);
        }
      }
      if (config.context === null || config.context.hasEmptyPath()) {
        if (config.context === null || config.context.isEmpty()) {
          configs.add(config);
          return true;
        } else {
          configs.add(new LexerATNConfig({ state: config.state, context: PredictionContext.EMPTY }, config));
          currentAltReachedAcceptState = true;
        }
      }
      if (config.context !== null && !config.context.isEmpty()) {
        for (let i = 0; i < config.context.length; i++) {
          if (config.context.getReturnState(i) !== PredictionContext.EMPTY_RETURN_STATE) {
            const newContext = config.context.getParent(i);
            const returnState = this.atn.states[config.context.getReturnState(i)];
            cfg = new LexerATNConfig({ state: returnState, context: newContext }, config);
            currentAltReachedAcceptState = this.closure(
              input,
              cfg,
              configs,
              currentAltReachedAcceptState,
              speculative,
              treatEofAsEpsilon
            );
          }
        }
      }
      return currentAltReachedAcceptState;
    }
    if (!config.state.epsilonOnlyTransitions) {
      if (!currentAltReachedAcceptState || !config.passedThroughNonGreedyDecision) {
        configs.add(config);
      }
    }
    for (const trans of config.state.transitions) {
      cfg = this.getEpsilonTarget(input, config, trans, configs, speculative, treatEofAsEpsilon);
      if (cfg !== null) {
        currentAltReachedAcceptState = this.closure(
          input,
          cfg,
          configs,
          currentAltReachedAcceptState,
          speculative,
          treatEofAsEpsilon
        );
      }
    }
    return currentAltReachedAcceptState;
  }
  // side-effect: can alter configs.hasSemanticContext
  getEpsilonTarget(input, config, trans, configs, speculative, treatEofAsEpsilon) {
    let cfg = null;
    if (trans.serializationType === TransitionType.RULE) {
      const newContext = SingletonPredictionContext.create(
        config.context,
        trans.followState.stateNumber
      );
      cfg = new LexerATNConfig({ state: trans.target, context: newContext }, config);
    } else if (trans.serializationType === TransitionType.PRECEDENCE) {
      throw new Error("Precedence predicates are not supported in lexers.");
    } else if (trans.serializationType === TransitionType.PREDICATE) {
      const pt = trans;
      if (_LexerATNSimulator.debug) {
        console.log("EVAL rule " + pt.ruleIndex + ":" + pt.predIndex);
      }
      configs.hasSemanticContext = true;
      if (this.evaluatePredicate(input, pt.ruleIndex, pt.predIndex, speculative)) {
        cfg = new LexerATNConfig({ state: trans.target }, config);
      }
    } else if (trans.serializationType === TransitionType.ACTION) {
      if (config.context === null || config.context.hasEmptyPath()) {
        const lexerActionExecutor = LexerActionExecutor.append(
          config.lexerActionExecutor,
          this.atn.lexerActions[trans.actionIndex]
        );
        cfg = new LexerATNConfig({ state: trans.target, lexerActionExecutor }, config);
      } else {
        cfg = new LexerATNConfig({ state: trans.target }, config);
      }
    } else if (trans.serializationType === TransitionType.EPSILON) {
      cfg = new LexerATNConfig({ state: trans.target }, config);
    } else if (trans.serializationType === TransitionType.ATOM || trans.serializationType === TransitionType.RANGE || trans.serializationType === TransitionType.SET) {
      if (treatEofAsEpsilon) {
        if (trans.matches(Token.EOF, 0, Lexer.MAX_CHAR_VALUE)) {
          cfg = new LexerATNConfig({ state: trans.target }, config);
        }
      }
    }
    return cfg;
  }
  /**
   * Evaluate a predicate specified in the lexer.
   *
   * <p>If {@code speculative} is {@code true}, this method was called before
   * {@link consume} for the matched character. This method should call
   * {@link consume} before evaluating the predicate to ensure position
   * sensitive values, including {@link Lexer//getText}, {@link Lexer//getLine},
   * and {@link Lexer}, properly reflect the current
   * lexer state. This method should restore {@code input} and the simulator
   * to the original state before returning (i.e. undo the actions made by the
   * call to {@link consume}.</p>
   *
   * @param input The input stream.
   * @param ruleIndex The rule containing the predicate.
   * @param predIndex The index of the predicate within the rule.
   * @param speculative {@code true} if the current index in {@code input} is
   * one character before the predicate's location.
   *
   * @returns `true` if the specified predicate evaluates to
   * {@code true}.
   */
  evaluatePredicate(input, ruleIndex, predIndex, speculative) {
    if (this.recog === null) {
      return true;
    }
    if (!speculative) {
      return this.recog.sempred(null, ruleIndex, predIndex);
    }
    const savedColumn = this.column;
    const savedLine = this.line;
    const index = input.index;
    const marker = input.mark();
    try {
      this.consume(input);
      return this.recog.sempred(null, ruleIndex, predIndex);
    } finally {
      this.column = savedColumn;
      this.line = savedLine;
      input.seek(index);
      input.release(marker);
    }
  }
  captureSimState(settings, input, dfaState) {
    settings.index = input.index;
    settings.line = this.line;
    settings.column = this.column;
    settings.dfaState = dfaState;
  }
  addDFAEdge(from_, tk, to, configs) {
    if (to === void 0) {
      to = null;
    }
    if (configs === void 0) {
      configs = null;
    }
    if (to === null && configs !== null) {
      const suppressEdge = configs.hasSemanticContext;
      configs.hasSemanticContext = false;
      to = this.addDFAState(configs);
      if (suppressEdge) {
        return to;
      }
    }
    if (tk < _LexerATNSimulator.MIN_DFA_EDGE || tk > _LexerATNSimulator.MAX_DFA_EDGE) {
      return to;
    }
    if (_LexerATNSimulator.debug) {
      console.log("EDGE " + from_ + " -> " + to + " upon " + tk);
    }
    if (from_.edges === null) {
      from_.edges = new Array(_LexerATNSimulator.MAX_DFA_EDGE - _LexerATNSimulator.MIN_DFA_EDGE + 1);
      from_.edges.fill(null);
    }
    from_.edges[tk - _LexerATNSimulator.MIN_DFA_EDGE] = to;
    return to;
  }
  /**
   * Add a new DFA state if there isn't one with this set of
   * configurations already. This method also detects the first
   * configuration containing an ATN rule stop state. Later, when
   * traversing the DFA, we will know which rule to accept.
   */
  addDFAState(configs) {
    const proposed = new DFAState(configs);
    let firstConfigWithRuleStopState = null;
    for (const cfg of configs.items) {
      if (cfg.state instanceof RuleStopState) {
        firstConfigWithRuleStopState = cfg;
        break;
      }
    }
    if (firstConfigWithRuleStopState !== null) {
      proposed.isAcceptState = true;
      proposed.lexerActionExecutor = firstConfigWithRuleStopState.lexerActionExecutor;
      proposed.prediction = this.atn.ruleToTokenType[firstConfigWithRuleStopState.state.ruleIndex];
    }
    const dfa = this.decisionToDFA[this.mode];
    const existing = dfa.states.get(proposed);
    if (existing !== null) {
      return existing;
    }
    const newState = proposed;
    newState.stateNumber = dfa.states.length;
    configs.setReadonly(true);
    newState.configs = configs;
    dfa.states.add(newState);
    return newState;
  }
};
((LexerATNSimulator2) => {
  class SimState {
    static {
      __name(this, "SimState");
    }
    index = -1;
    line = 0;
    column = -1;
    dfaState = null;
    reset() {
      this.index = -1;
      this.line = 0;
      this.column = -1;
      this.dfaState = null;
    }
  }
  LexerATNSimulator2.SimState = SimState;
})(LexerATNSimulator || (LexerATNSimulator = {}));

// src/Lexer.ts
var Lexer = class _Lexer extends Recognizer {
  static {
    __name(this, "Lexer");
  }
  static DEFAULT_MODE = 0;
  static MORE = -2;
  static SKIP = -3;
  static DEFAULT_TOKEN_CHANNEL = Token.DEFAULT_CHANNEL;
  static HIDDEN = Token.HIDDEN_CHANNEL;
  static MIN_CHAR_VALUE = 0;
  static MAX_CHAR_VALUE = 1114111;
  _input;
  /**
   * The goal of all lexer rules/methods is to create a token object.
   *  This is an instance variable as multiple rules may collaborate to
   *  create a single token.  nextToken will return this object after
   *  matching lexer rule(s).  If you subclass to allow multiple token
   *  emissions, then set this to the last token to be matched or
   *  something nonnull so that the auto token emit mechanism will not
   *  emit another token.
   */
  _token = null;
  /**
   * What character index in the stream did the current token start at?
   *  Needed, for example, to get the text for current token.  Set at
   *  the start of nextToken.
   */
  _tokenStartCharIndex = -1;
  /** The line on which the first character of the token resides */
  _tokenStartLine = 0;
  /** The character position of first character within the line */
  _tokenStartColumn = 0;
  /**
   * Once we see EOF on char stream, next token will be EOF.
   *  If you have DONE : EOF ; then you see DONE EOF.
   */
  _hitEOF = false;
  /** The channel number for the current token */
  _channel = 0;
  /** The token type for the current token */
  _type = 0;
  _modeStack = [];
  _mode = _Lexer.DEFAULT_MODE;
  /**
   * You can set the text for the current token to override what is in
   *  the input char buffer.  Use setText() or can set this instance var.
   */
  _text = null;
  _factory;
  constructor(input) {
    super();
    this._input = input;
    this._factory = CommonTokenFactory.DEFAULT;
  }
  reset(seekBack = true) {
    if (seekBack) {
      this._input.seek(0);
    }
    this._token = null;
    this._type = Token.INVALID_TYPE;
    this._channel = Token.DEFAULT_CHANNEL;
    this._tokenStartCharIndex = -1;
    this._tokenStartColumn = -1;
    this._tokenStartLine = -1;
    this._text = null;
    this._hitEOF = false;
    this._mode = _Lexer.DEFAULT_MODE;
    this._modeStack = [];
    this.interpreter.reset();
  }
  // Return a token from this source; i.e., match a token on the char stream.
  nextToken() {
    if (this._input === null) {
      throw new Error("nextToken requires a non-null input stream.");
    }
    const tokenStartMarker = this._input.mark();
    try {
      for (; ; ) {
        if (this._hitEOF) {
          this.emitEOF();
          return this._token;
        }
        this._token = null;
        this._channel = Token.DEFAULT_CHANNEL;
        this._tokenStartCharIndex = this._input.index;
        this._tokenStartColumn = this.interpreter.column;
        this._tokenStartLine = this.interpreter.line;
        this._text = null;
        let continueOuter = false;
        for (; ; ) {
          this._type = Token.INVALID_TYPE;
          let ttype = _Lexer.SKIP;
          try {
            ttype = this.interpreter.match(this._input, this._mode);
          } catch (e) {
            if (e instanceof LexerNoViableAltException) {
              this.notifyListeners(e);
              this.recover(e);
            } else {
              throw e;
            }
          }
          if (this._input.LA(1) === Token.EOF) {
            this._hitEOF = true;
          }
          if (this._type === Token.INVALID_TYPE) {
            this._type = ttype;
          }
          if (this._type === _Lexer.SKIP) {
            continueOuter = true;
            break;
          }
          if (this._type !== _Lexer.MORE) {
            break;
          }
        }
        if (continueOuter) {
          continue;
        }
        if (this._token === null) {
          this.emit();
        }
        return this._token;
      }
    } finally {
      this._input.release(tokenStartMarker);
    }
  }
  /**
   * Instruct the lexer to skip creating a token for current lexer rule
   * and look for another token. nextToken() knows to keep looking when
   * a lexer rule finishes with token set to SKIP_TOKEN. Recall that
   * if token==null at end of any token rule, it creates one for you
   * and emits it.
   */
  skip() {
    this._type = _Lexer.SKIP;
  }
  more() {
    this._type = _Lexer.MORE;
  }
  mode(m2) {
    this._mode = m2;
  }
  pushMode(m2) {
    if (LexerATNSimulator.debug) {
      console.log("pushMode " + m2);
    }
    this._modeStack.push(this._mode);
    this.mode(m2);
  }
  popMode() {
    if (this._modeStack.length === 0) {
      throw new Error("Empty Stack");
    }
    if (LexerATNSimulator.debug) {
      console.log("popMode back to " + this._modeStack.slice(0, -1));
    }
    this.mode(this._modeStack.pop());
    return this._mode;
  }
  /**
   * By default does not support multiple emits per nextToken invocation
   * for efficiency reasons. Subclass and override this method, nextToken,
   * and getToken (to push tokens into a list and pull from that list
   * rather than a single variable as this implementation does).
   */
  emitToken(token) {
    this._token = token;
  }
  /**
   * The standard method called to automatically emit a token at the
   * outermost lexical rule. The token object should point into the
   * char buffer start..stop. If there is a text override in 'text',
   * use that to set the token's text. Override this method to emit
   * custom Token objects or provide a new factory.
   */
  emit() {
    const t = this._factory.create(
      [this, this._input],
      this._type,
      this._text,
      this._channel,
      this._tokenStartCharIndex,
      this.getCharIndex() - 1,
      this._tokenStartLine,
      this._tokenStartColumn
    );
    this.emitToken(t);
    return t;
  }
  emitEOF() {
    const cpos = this.column;
    const lpos = this.line;
    const eof = this._factory.create(
      [this, this._input],
      Token.EOF,
      null,
      Token.DEFAULT_CHANNEL,
      this._input.index,
      this._input.index - 1,
      lpos,
      cpos
    );
    this.emitToken(eof);
    return eof;
  }
  // What is the index of the current character of lookahead?///
  getCharIndex() {
    return this._input.index;
  }
  /**
   * Return a list of all Token objects in input char stream.
   * Forces load of all tokens. Does not include EOF token.
   */
  getAllTokens() {
    const tokens = [];
    let t = this.nextToken();
    while (t.type !== Token.EOF) {
      tokens.push(t);
      t = this.nextToken();
    }
    return tokens;
  }
  notifyListeners(e) {
    const start = this._tokenStartCharIndex;
    const stop = this._input.index;
    const text = this._input.getText(start, stop);
    const msg = "token recognition error at: '" + this.getErrorDisplay(text) + "'";
    const listener = this.getErrorListenerDispatch();
    listener.syntaxError(
      this,
      null,
      this._tokenStartLine,
      this._tokenStartColumn,
      msg,
      e
    );
  }
  getErrorDisplay(s) {
    return s;
  }
  getErrorDisplayForChar(c) {
    if (c.charCodeAt(0) === Token.EOF) {
      return "<EOF>";
    } else if (c === "\n") {
      return "\\n";
    } else if (c === "	") {
      return "\\t";
    } else if (c === "\r") {
      return "\\r";
    } else {
      return c;
    }
  }
  getCharErrorDisplay(c) {
    return "'" + this.getErrorDisplayForChar(c) + "'";
  }
  /**
   * Lexers can normally match any char in it's vocabulary after matching
   * a token, so do the easy thing and just kill a character and hope
   * it all works out. You can instead use the rule invocation stack
   * to do sophisticated error recovery if you are in a fragment rule.
   */
  recover(re) {
    if (this._input.LA(1) !== Token.EOF) {
      if (re instanceof LexerNoViableAltException) {
        this.interpreter.consume(this._input);
      } else {
        this._input.consume();
      }
    }
  }
  get inputStream() {
    return this._input;
  }
  set inputStream(input) {
    this.reset(false);
    this._input = input;
  }
  set tokenFactory(factory) {
    this._factory = factory;
  }
  get tokenFactory() {
    return this._factory;
  }
  get sourceName() {
    return this._input.getSourceName();
  }
  get type() {
    return this._type;
  }
  set type(type) {
    this._type = type;
  }
  get line() {
    return this.interpreter.line;
  }
  set line(line) {
    this.interpreter.line = line;
  }
  get column() {
    return this.interpreter.column;
  }
  set column(column) {
    this.interpreter.column = column;
  }
  get text() {
    if (this._text !== null) {
      return this._text;
    } else {
      return this.interpreter.getText(this._input);
    }
  }
  set text(text) {
    this._text = text;
  }
};

// src/misc/IntervalSet.ts
var IntervalSet = class _IntervalSet {
  static {
    __name(this, "IntervalSet");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static COMPLETE_CHAR_SET = _IntervalSet.of(Lexer.MIN_CHAR_VALUE, Lexer.MAX_CHAR_VALUE);
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static EMPTY_SET = new _IntervalSet();
  /** The list of sorted, disjoint intervals. */
  intervals = [];
  readOnly = false;
  static {
    _IntervalSet.COMPLETE_CHAR_SET.readOnly = true;
    _IntervalSet.EMPTY_SET.readOnly = true;
  }
  constructor(set) {
    if (set) {
      this.addSet(set);
    }
  }
  /** Create a set with all ints within range [a..b] (inclusive) */
  static of(a, b) {
    const s = new _IntervalSet();
    s.addRange(a, b);
    return s;
  }
  [Symbol.iterator]() {
    return this.intervals[Symbol.iterator]();
  }
  /**
   * Returns the minimum value contained in the set if not isNil().
   *
   * @returns the minimum value contained in the set.
   */
  get minElement() {
    if (this.intervals.length === 0) {
      return Token.INVALID_TYPE;
    }
    return this.intervals[0].start;
  }
  /**
   * Returns the maximum value contained in the set if not isNil().
   *
   * @returns the maximum value contained in the set.
   */
  get maxElement() {
    if (this.intervals.length === 0) {
      return Token.INVALID_TYPE;
    }
    return this.intervals[this.intervals.length - 1].stop;
  }
  get isNil() {
    return this.intervals.length === 0;
  }
  clear() {
    if (this.readOnly) {
      throw new Error("can't alter readonly IntervalSet");
    }
    this.intervals = [];
  }
  /**
   * Add a single element to the set.  An isolated element is stored
   *  as a range el..el.
   */
  addOne(v) {
    this.addInterval(new Interval(v, v));
  }
  /**
   * Add interval; i.e., add all integers from a to b to set.
   *  If b < a, do nothing.
   *  Keep list in sorted order (by left range value).
   *  If overlap, combine ranges. For example,
   *  If this is {1..5, 10..20}, adding 6..7 yields
   *  {1..5, 6..7, 10..20}. Adding 4..8 yields {1..8, 10..20}.
   */
  addRange(l, h) {
    this.addInterval(new Interval(l, h));
  }
  addInterval(addition) {
    if (this.readOnly) {
      throw new Error("can't alter readonly IntervalSet");
    }
    if (this.intervals.length === 0) {
      this.intervals.push(addition);
    } else {
      for (let pos = 0; pos < this.intervals.length; pos++) {
        const existing = this.intervals[pos];
        if (addition.equals(existing)) {
          return;
        }
        if (addition.adjacent(existing) || !addition.disjoint(existing)) {
          const bigger = addition.union(existing);
          this.intervals[pos] = bigger;
          for (let sub = pos + 1; sub < this.intervals.length; ) {
            const next = this.intervals[sub];
            if (!bigger.adjacent(next) && bigger.disjoint(next)) {
              break;
            }
            this.intervals.splice(sub, 1);
            this.intervals[pos] = bigger.union(next);
          }
          return;
        }
        if (addition.startsBeforeDisjoint(existing)) {
          this.intervals.splice(pos, 0, addition);
          return;
        }
      }
      this.intervals.push(addition);
    }
  }
  addSet(other) {
    other.intervals.forEach((toAdd) => {
      return this.addInterval(toAdd);
    }, this);
    return this;
  }
  complement(vocabularyOrMinElement, maxElement) {
    if (!vocabularyOrMinElement) {
      return new _IntervalSet();
    }
    const result = new _IntervalSet();
    if (vocabularyOrMinElement instanceof _IntervalSet) {
      if (vocabularyOrMinElement.isNil) {
        return new _IntervalSet();
      }
      result.addSet(vocabularyOrMinElement);
    } else {
      result.addInterval(new Interval(vocabularyOrMinElement, maxElement ?? 0));
    }
    return result.subtract(this);
  }
  /** combine all sets in the array returned the or'd value */
  or(sets) {
    const result = new _IntervalSet();
    result.addSet(this);
    sets.forEach((set) => {
      return result.addSet(set);
    });
    return result;
  }
  and(other) {
    if (other.isNil) {
      return new _IntervalSet();
    }
    const myIntervals = this.intervals;
    const theirIntervals = other.intervals;
    let intersection;
    const mySize = myIntervals.length;
    const theirSize = theirIntervals.length;
    let i = 0;
    let j = 0;
    while (i < mySize && j < theirSize) {
      const mine = myIntervals[i];
      const theirs = theirIntervals[j];
      if (mine.startsBeforeDisjoint(theirs)) {
        i++;
      } else if (theirs.startsBeforeDisjoint(mine)) {
        j++;
      } else if (mine.properlyContains(theirs)) {
        if (!intersection) {
          intersection = new _IntervalSet();
        }
        intersection.addInterval(mine.intersection(theirs));
        j++;
      } else if (theirs.properlyContains(mine)) {
        if (!intersection) {
          intersection = new _IntervalSet();
        }
        intersection.addInterval(mine.intersection(theirs));
        i++;
      } else if (!mine.disjoint(theirs)) {
        if (!intersection) {
          intersection = new _IntervalSet();
        }
        intersection.addInterval(mine.intersection(theirs));
        if (mine.startsAfterNonDisjoint(theirs)) {
          j++;
        } else if (theirs.startsAfterNonDisjoint(mine)) {
          i++;
        }
      }
    }
    if (!intersection) {
      return new _IntervalSet();
    }
    return intersection;
  }
  /**
   * Compute the set difference between two interval sets. The specific
   * operation is {@code left - right}. If either of the input sets is
   * {@code null}, it is treated as though it was an empty set.
   */
  subtract(other) {
    if (this.isNil) {
      return new _IntervalSet();
    }
    const result = new _IntervalSet(this);
    if (other.isNil) {
      return result;
    }
    let resultI = 0;
    let rightI = 0;
    while (resultI < result.intervals.length && rightI < other.intervals.length) {
      const resultInterval = result.intervals[resultI];
      const rightInterval = other.intervals[rightI];
      if (rightInterval.stop < resultInterval.start) {
        rightI++;
        continue;
      }
      if (rightInterval.start > resultInterval.stop) {
        resultI++;
        continue;
      }
      let beforeCurrent = null;
      let afterCurrent = null;
      if (rightInterval.start > resultInterval.start) {
        beforeCurrent = new Interval(resultInterval.start, rightInterval.start - 1);
      }
      if (rightInterval.stop < resultInterval.stop) {
        afterCurrent = new Interval(rightInterval.stop + 1, resultInterval.stop);
      }
      if (beforeCurrent != null) {
        if (afterCurrent != null) {
          result.intervals[resultI] = beforeCurrent;
          result.intervals.splice(resultI + 1, 0, afterCurrent);
          resultI++;
          rightI++;
          continue;
        } else {
          result.intervals[resultI] = beforeCurrent;
          resultI++;
          continue;
        }
      } else {
        if (afterCurrent != null) {
          result.intervals[resultI] = afterCurrent;
          rightI++;
          continue;
        } else {
          result.intervals.splice(resultI, 1);
          continue;
        }
      }
    }
    return result;
  }
  contains(el) {
    const n2 = this.intervals.length;
    let l = 0;
    let r = n2 - 1;
    while (l <= r) {
      const m2 = Math.floor((l + r) / 2);
      const interval = this.intervals[m2];
      if (interval.stop < el) {
        l = m2 + 1;
      } else if (interval.start > el) {
        r = m2 - 1;
      } else {
        return true;
      }
    }
    return false;
  }
  removeRange(toRemove) {
    if (this.readOnly) {
      throw new Error("can't alter readonly IntervalSet");
    }
    if (toRemove.start === toRemove.stop) {
      this.removeOne(toRemove.start);
    } else if (this.intervals !== null) {
      let pos = 0;
      for (const existing of this.intervals) {
        if (toRemove.stop <= existing.start) {
          return;
        } else if (toRemove.start > existing.start && toRemove.stop < existing.stop) {
          this.intervals[pos] = new Interval(existing.start, toRemove.start);
          const x = new Interval(toRemove.stop, existing.stop);
          this.intervals.splice(pos, 0, x);
          return;
        } else if (toRemove.start <= existing.start && toRemove.stop >= existing.stop) {
          this.intervals.splice(pos, 1);
          pos = pos - 1;
        } else if (toRemove.start < existing.stop) {
          this.intervals[pos] = new Interval(existing.start, toRemove.start);
        } else if (toRemove.stop < existing.stop) {
          this.intervals[pos] = new Interval(toRemove.stop, existing.stop);
        }
        pos += 1;
      }
    }
  }
  removeOne(value) {
    if (this.readOnly) {
      throw new Error("can't alter readonly IntervalSet");
    }
    for (let i = 0; i < this.intervals.length; i++) {
      const existing = this.intervals[i];
      if (value < existing.start) {
        return;
      } else if (value === existing.start && value === existing.stop) {
        this.intervals.splice(i, 1);
        return;
      } else if (value === existing.start) {
        this.intervals[i] = new Interval(existing.start + 1, existing.stop);
        return;
      } else if (value === existing.stop) {
        this.intervals[i] = new Interval(existing.start, existing.stop);
        return;
      } else if (value < existing.stop) {
        const replace = new Interval(existing.start, value);
        this.intervals[i] = new Interval(value + 1, existing.stop);
        this.intervals.splice(i, 0, replace);
        return;
      }
    }
  }
  toString(elementsAreCharOrVocabulary) {
    if (this.intervals.length === 0) {
      return "{}";
    }
    let result = "";
    if (this.length > 1) {
      result += "{";
    }
    let elementsAreChar = false;
    let vocabulary;
    if (elementsAreCharOrVocabulary instanceof Vocabulary) {
      vocabulary = elementsAreCharOrVocabulary;
    } else if (Array.isArray(elementsAreCharOrVocabulary)) {
      vocabulary = Vocabulary.fromTokenNames(elementsAreCharOrVocabulary);
    } else {
      elementsAreChar = elementsAreCharOrVocabulary ?? false;
    }
    for (let i = 0; i < this.intervals.length; ++i) {
      const interval = this.intervals[i];
      const start = interval.start;
      const stop = interval.stop;
      if (start === stop) {
        if (start === Token.EOF) {
          result += "<EOF>";
        } else if (elementsAreChar) {
          result += "'" + String.fromCodePoint(start) + "'";
        } else if (vocabulary) {
          result += this.elementName(vocabulary, start);
        } else {
          result += start;
        }
      } else {
        if (elementsAreChar) {
          result += "'" + String.fromCodePoint(start) + "'..'" + String.fromCodePoint(stop) + "'";
        } else if (vocabulary) {
          for (let i2 = start; i2 <= stop; ++i2) {
            if (i2 > start) {
              result += ", ";
            }
            result += this.elementName(vocabulary, i2);
          }
        } else {
          result += start + ".." + stop;
        }
      }
      if (i < this.intervals.length - 1) {
        result += ", ";
      }
    }
    if (this.length > 1) {
      result += "}";
    }
    return result;
  }
  toArray() {
    const data = [];
    for (const interval of this.intervals) {
      for (let j = interval.start; j <= interval.stop; j++) {
        data.push(j);
      }
    }
    return data;
  }
  get length() {
    let result = 0;
    const intervalCount = this.intervals.length;
    if (intervalCount === 1) {
      const firstInterval = this.intervals[0];
      return firstInterval.stop - firstInterval.start + 1;
    }
    for (const interval of this.intervals) {
      result += interval.length;
    }
    return result;
  }
  isReadonly() {
    return this.readOnly;
  }
  setReadonly(readonly) {
    if (this.readOnly && !readonly) {
      throw new Error("can't alter readonly IntervalSet");
    }
    this.readOnly = readonly;
  }
  elementName(vocabulary, token) {
    if (token === Token.EOF) {
      return "<EOF>";
    } else if (token === Token.EPSILON) {
      return "<EPSILON>";
    } else {
      return vocabulary.getDisplayName(token);
    }
  }
};

// src/atn/RuleTransition.ts
var RuleTransition = class extends Transition {
  static {
    __name(this, "RuleTransition");
  }
  ruleIndex;
  precedence;
  followState;
  constructor(ruleStart, ruleIndex, precedence, followState) {
    super(ruleStart);
    this.ruleIndex = ruleIndex;
    this.precedence = precedence;
    this.followState = followState;
  }
  get isEpsilon() {
    return true;
  }
  get serializationType() {
    return TransitionType.RULE;
  }
  matches(_symbol, _minVocabSymbol, _maxVocabSymbol) {
    return false;
  }
};

// src/atn/SetTransition.ts
var SetTransition = class extends Transition {
  static {
    __name(this, "SetTransition");
  }
  #label;
  constructor(target, set) {
    super(target);
    if (set !== void 0 && set !== null) {
      this.#label = set;
    } else {
      this.#label = new IntervalSet();
      this.#label.addOne(Token.INVALID_TYPE);
    }
  }
  get label() {
    return this.#label;
  }
  get serializationType() {
    return TransitionType.SET;
  }
  matches(symbol, _minVocabSymbol, _maxVocabSymbol) {
    return this.label.contains(symbol);
  }
  toString() {
    return this.#label.toString();
  }
};

// src/atn/NotSetTransition.ts
var NotSetTransition = class extends SetTransition {
  static {
    __name(this, "NotSetTransition");
  }
  constructor(target, set) {
    super(target, set);
  }
  get serializationType() {
    return TransitionType.NOT_SET;
  }
  matches(symbol, minVocabSymbol, maxVocabSymbol) {
    return symbol >= minVocabSymbol && symbol <= maxVocabSymbol && !super.matches(symbol, minVocabSymbol, maxVocabSymbol);
  }
  toString() {
    return "~" + super.toString();
  }
};

// src/atn/WildcardTransition.ts
var WildcardTransition = class extends Transition {
  static {
    __name(this, "WildcardTransition");
  }
  constructor(target) {
    super(target);
  }
  get serializationType() {
    return TransitionType.WILDCARD;
  }
  matches(symbol, minVocabSymbol, maxVocabSymbol) {
    return symbol >= minVocabSymbol && symbol <= maxVocabSymbol;
  }
  toString() {
    return ".";
  }
};

// src/atn/LL1Analyzer.ts
var LL1Analyzer = class _LL1Analyzer {
  static {
    __name(this, "LL1Analyzer");
  }
  /**
   * Special value added to the lookahead sets to indicate that we hit
   *  a predicate during analysis if {@code seeThruPreds==false}.
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static HIT_PRED = Token.INVALID_TYPE;
  atn;
  constructor(atn) {
    this.atn = atn;
  }
  /**
   * Calculates the SLL(1) expected lookahead set for each outgoing transition
   * of an {@link ATNState}. The returned array has one element for each
   * outgoing transition in {@code s}. If the closure from transition
   * <em>i</em> leads to a semantic predicate before matching a symbol, the
   * element at index <em>i</em> of the result will be {@code null}.
   *
   * @param s the ATN state
   * @returns the expected symbols for each outgoing transition of {@code s}.
   */
  getDecisionLookahead(s) {
    if (s === null) {
      return null;
    }
    const count = s.transitions.length;
    const look = new Array(count);
    look.fill(null);
    for (let alt = 0; alt < count; alt++) {
      look[alt] = new IntervalSet();
      const lookBusy = new HashSet();
      const seeThruPreds = false;
      this._LOOK(
        s.transitions[alt].target,
        null,
        PredictionContext.EMPTY,
        look[alt],
        lookBusy,
        new BitSet(),
        seeThruPreds,
        false
      );
      if (look[alt].length === 0 || look[alt].contains(_LL1Analyzer.HIT_PRED)) {
        look[alt] = null;
      }
    }
    return look;
  }
  /**
   * Compute set of tokens that can follow {@code s} in the ATN in the
   * specified {@code ctx}.
   *
   * <p>If {@code ctx} is {@code null} and the end of the rule containing
   * {@code s} is reached, {@link Token//EPSILON} is added to the result set.
   * If {@code ctx} is not {@code null} and the end of the outermost rule is
   * reached, {@link Token//EOF} is added to the result set.</p>
   *
   * @param s the ATN state
   * @param stopState the ATN state to stop at. This can be a
   * {@link BlockEndState} to detect epsilon paths through a closure.
   * @param ctx the complete parser context, or {@code null} if the context
   * should be ignored
   *
   * @returns The set of tokens that can follow {@code s} in the ATN in the
   * specified {@code ctx}.
   */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  LOOK(s, stopState, ctx) {
    const r = new IntervalSet();
    const seeThruPreds = true;
    ctx = ctx || null;
    const lookContext = ctx !== null ? predictionContextFromRuleContext(s.atn, ctx) : null;
    this._LOOK(s, stopState, lookContext, r, new HashSet(), new BitSet(), seeThruPreds, true);
    return r;
  }
  /**
   * Compute set of tokens that can follow {@code s} in the ATN in the
   * specified {@code ctx}.
   *
   * <p>If {@code ctx} is {@code null} and {@code stopState} or the end of the
   * rule containing {@code s} is reached, {@link Token//EPSILON} is added to
   * the result set. If {@code ctx} is not {@code null} and {@code addEOF} is
   * {@code true} and {@code stopState} or the end of the outermost rule is
   * reached, {@link Token//EOF} is added to the result set.</p>
   *
   * @param s the ATN state.
   * @param stopState the ATN state to stop at. This can be a
   * {@link BlockEndState} to detect epsilon paths through a closure.
   * @param ctx The outer context, or {@code null} if the outer context should
   * not be used.
   * @param look The result lookahead set.
   * @param lookBusy A set used for preventing epsilon closures in the ATN
   * from causing a stack overflow. Outside code should pass
   * {@code new CustomizedSet<ATNConfig>} for this argument.
   * @param calledRuleStack A set used for preventing left recursion in the
   * ATN from causing a stack overflow. Outside code should pass
   * {@code new BitSet()} for this argument.
   * @param seeThruPreds {@code true} to true semantic predicates as
   * implicitly {@code true} and "see through them", otherwise {@code false}
   * to treat semantic predicates as opaque and add {@link HIT_PRED} to the
   * result if one is encountered.
   * @param addEOF Add {@link Token//EOF} to the result if the end of the
   * outermost context is reached. This parameter has no effect if {@code ctx}
   * is {@code null}.
   */
  _LOOK(s, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF) {
    const c = new ATNConfig({ state: s, alt: 0, context: ctx }, null);
    if (lookBusy.has(c)) {
      return;
    }
    lookBusy.add(c);
    if (s === stopState) {
      if (ctx === null) {
        look.addOne(Token.EPSILON);
        return;
      } else if (ctx.isEmpty() && addEOF) {
        look.addOne(Token.EOF);
        return;
      }
    }
    if (s instanceof RuleStopState) {
      if (ctx === null) {
        look.addOne(Token.EPSILON);
        return;
      } else if (ctx.isEmpty() && addEOF) {
        look.addOne(Token.EOF);
        return;
      }
      if (ctx !== PredictionContext.EMPTY) {
        const removed = calledRuleStack.get(s.ruleIndex);
        try {
          calledRuleStack.clear(s.ruleIndex);
          for (let i = 0; i < ctx.length; i++) {
            const returnState = this.atn.states[ctx.getReturnState(i)];
            this._LOOK(
              returnState,
              stopState,
              ctx.getParent(i),
              look,
              lookBusy,
              calledRuleStack,
              seeThruPreds,
              addEOF
            );
          }
        } finally {
          if (removed) {
            calledRuleStack.set(s.ruleIndex);
          }
        }
        return;
      }
    }
    for (const t of s.transitions) {
      if (t instanceof RuleTransition) {
        if (calledRuleStack.get(t.target.ruleIndex)) {
          continue;
        }
        const newContext = SingletonPredictionContext.create(ctx, t.followState.stateNumber);
        try {
          calledRuleStack.set(t.target.ruleIndex);
          this._LOOK(t.target, stopState, newContext, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
        } finally {
          calledRuleStack.clear(t.target.ruleIndex);
        }
      } else if (t instanceof AbstractPredicateTransition) {
        if (seeThruPreds) {
          this._LOOK(t.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
        } else {
          look.addOne(_LL1Analyzer.HIT_PRED);
        }
      } else if (t.isEpsilon) {
        this._LOOK(t.target, stopState, ctx, look, lookBusy, calledRuleStack, seeThruPreds, addEOF);
      } else if (t.constructor === WildcardTransition) {
        look.addRange(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType);
      } else {
        let set = t.label;
        if (set !== null) {
          if (t instanceof NotSetTransition) {
            set = set.complement(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType);
          }
          look.addSet(set);
        }
      }
    }
  }
};

// src/atn/ATN.ts
var ATN = class {
  static {
    __name(this, "ATN");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INVALID_ALT_NUMBER = 0;
  /**
   * Used for runtime deserialization of ATNs from strings
   * The type of the ATN.
   */
  grammarType;
  /** The maximum value for any symbol recognized by a transition in the ATN. */
  maxTokenType;
  states = [];
  /**
   * Each subrule/rule is a decision point and we must track them so we
   * can go back later and build DFA predictors for them.  This includes
   * all the rules, subrules, optional blocks, ()+, ()* etc...
   */
  decisionToState = [];
  /** Maps from rule index to starting state number. */
  ruleToStartState = [];
  // Initialized by the ATN deserializer.
  /** Maps from rule index to stop state number. */
  ruleToStopState = [];
  // Initialized by the ATN deserializer.
  modeNameToStartState = /* @__PURE__ */ new Map();
  /**
   * For lexer ATNs, this maps the rule index to the resulting token type.
   * For parser ATNs, this maps the rule index to the generated bypass token
   * type if the {@link ATNDeserializationOptions//isGenerateRuleBypassTransitions}
   * deserialization option was specified; otherwise, this is {@code null}
   */
  ruleToTokenType = [];
  // Initialized by the ATN deserializer.
  /**
   * For lexer ATNs, this is an array of {@link LexerAction} objects which may
   * be referenced by action transitions in the ATN
   */
  lexerActions = [];
  modeToStartState = [];
  constructor(grammarType, maxTokenType) {
    this.grammarType = grammarType;
    this.maxTokenType = maxTokenType;
  }
  addState(state) {
    if (state !== null) {
      state.atn = this;
      state.stateNumber = this.states.length;
    }
    this.states.push(state);
  }
  removeState(state) {
    this.states[state.stateNumber] = null;
  }
  defineDecisionState(s) {
    this.decisionToState.push(s);
    s.decision = this.decisionToState.length - 1;
    return s.decision;
  }
  getDecisionState(decision) {
    if (this.decisionToState.length === 0) {
      return null;
    } else {
      return this.decisionToState[decision];
    }
  }
  /**
   * Computes the set of input symbols which could follow ATN state number
   * {@code stateNumber} in the specified full {@code context}. This method
   * considers the complete parser context, but does not evaluate semantic
   * predicates (i.e. all predicates encountered during the calculation are
   * assumed true). If a path in the ATN exists from the starting state to the
   * {@link RuleStopState} of the outermost context without matching any
   * symbols, {@link Token//EOF} is added to the returned set.
   *
   * <p>If {@code context} is {@code null}, it is treated as
   * {@link ParserRuleContext//EMPTY}.</p>
   *
   * @param stateNumber the ATN state number
   * @param context the full parse context
   *
   * @returns {IntervalSet} The set of potentially valid input symbols which could follow the
   * specified state in the specified context.
   *
   * @throws IllegalArgumentException if the ATN does not contain a state with
   * number {@code stateNumber}
   */
  getExpectedTokens(stateNumber, context) {
    if (stateNumber < 0 || stateNumber >= this.states.length) {
      throw new Error("Invalid state number.");
    }
    const s = this.states[stateNumber];
    let following = this.nextTokens(s);
    if (!following.contains(Token.EPSILON)) {
      return following;
    }
    let ctx = context;
    const expected = new IntervalSet();
    expected.addSet(following);
    expected.removeOne(Token.EPSILON);
    while (ctx !== null && ctx.invokingState >= 0 && following.contains(Token.EPSILON)) {
      const invokingState = this.states[ctx.invokingState];
      const rt = invokingState.transitions[0];
      following = this.nextTokens(rt.followState);
      expected.addSet(following);
      expected.removeOne(Token.EPSILON);
      ctx = ctx.parent;
    }
    if (following.contains(Token.EPSILON)) {
      expected.addOne(Token.EOF);
    }
    return expected;
  }
  nextTokens(atnState, ctx) {
    if (ctx === void 0) {
      if (atnState.nextTokenWithinRule !== null) {
        return atnState.nextTokenWithinRule;
      }
      atnState.nextTokenWithinRule = this.nextTokens(atnState, null);
      atnState.nextTokenWithinRule.setReadonly(true);
      return atnState.nextTokenWithinRule;
    }
    const analyzer = new LL1Analyzer(this);
    return analyzer.LOOK(atnState, null, ctx);
  }
};

// src/atn/ATNDeserializationOptions.ts
var ATNDeserializationOptions = class _ATNDeserializationOptions {
  static {
    __name(this, "ATNDeserializationOptions");
  }
  static defaultOptions = new _ATNDeserializationOptions();
  readOnly = false;
  verifyATN;
  generateRuleBypassTransitions;
  constructor(copyFrom) {
    this.readOnly = false;
    this.verifyATN = copyFrom == null ? true : copyFrom.verifyATN;
    this.generateRuleBypassTransitions = copyFrom == null ? false : copyFrom.generateRuleBypassTransitions;
  }
  static {
    _ATNDeserializationOptions.defaultOptions.readOnly = true;
  }
};

// src/atn/ATNType.ts
var ATNType = {
  LEXER: 0,
  PARSER: 1
};

// src/atn/BasicState.ts
var BasicState = class extends ATNState {
  static {
    __name(this, "BasicState");
  }
  get stateType() {
    return ATNStateType.BASIC;
  }
};

// src/atn/BlockStartState.ts
var BlockStartState = class extends DecisionState {
  static {
    __name(this, "BlockStartState");
  }
  endState;
  constructor() {
    super();
    this.endState = null;
  }
};

// src/atn/BlockEndState.ts
var BlockEndState = class extends ATNState {
  static {
    __name(this, "BlockEndState");
  }
  startState = null;
  get stateType() {
    return ATNStateType.BLOCK_END;
  }
};

// src/atn/LoopEndState.ts
var LoopEndState = class extends ATNState {
  static {
    __name(this, "LoopEndState");
  }
  loopBackState = null;
  get stateType() {
    return ATNStateType.LOOP_END;
  }
};

// src/atn/RuleStartState.ts
var RuleStartState = class extends ATNState {
  static {
    __name(this, "RuleStartState");
  }
  stopState;
  isLeftRecursiveRule;
  isPrecedenceRule;
  constructor() {
    super();
    this.isPrecedenceRule = false;
  }
  get stateType() {
    return ATNStateType.RULE_START;
  }
};

// src/atn/TokensStartState.ts
var TokensStartState = class extends DecisionState {
  static {
    __name(this, "TokensStartState");
  }
  get stateType() {
    return ATNStateType.TOKEN_START;
  }
};

// src/atn/PlusLoopbackState.ts
var PlusLoopbackState = class extends DecisionState {
  static {
    __name(this, "PlusLoopbackState");
  }
  get stateType() {
    return ATNStateType.PLUS_LOOP_BACK;
  }
};

// src/atn/StarLoopbackState.ts
var StarLoopbackState = class extends ATNState {
  static {
    __name(this, "StarLoopbackState");
  }
  get stateType() {
    return ATNStateType.STAR_LOOP_BACK;
  }
};

// src/atn/PlusBlockStartState.ts
var PlusBlockStartState = class extends BlockStartState {
  static {
    __name(this, "PlusBlockStartState");
  }
  loopBackState;
  get stateType() {
    return ATNStateType.PLUS_BLOCK_START;
  }
};

// src/atn/StarBlockStartState.ts
var StarBlockStartState = class extends BlockStartState {
  static {
    __name(this, "StarBlockStartState");
  }
  get stateType() {
    return ATNStateType.STAR_BLOCK_START;
  }
};

// src/atn/BasicBlockStartState.ts
var BasicBlockStartState = class extends BlockStartState {
  static {
    __name(this, "BasicBlockStartState");
  }
  get stateType() {
    return ATNStateType.BLOCK_START;
  }
};

// src/atn/AtomTransition.ts
var AtomTransition = class extends Transition {
  static {
    __name(this, "AtomTransition");
  }
  /** The token type or character value; or, signifies special label. */
  labelValue;
  #label;
  constructor(target, label) {
    super(target);
    this.labelValue = label;
    this.#label = IntervalSet.of(label, label);
  }
  get label() {
    return this.#label;
  }
  matches(symbol, _minVocabSymbol, _maxVocabSymbol) {
    return this.labelValue === symbol;
  }
  get serializationType() {
    return TransitionType.ATOM;
  }
  toString() {
    return this.labelValue.toString();
  }
};

// src/atn/RangeTransition.ts
var RangeTransition = class extends Transition {
  static {
    __name(this, "RangeTransition");
  }
  start;
  stop;
  #label = new IntervalSet();
  constructor(target, start, stop) {
    super(target);
    this.start = start;
    this.stop = stop;
    this.#label.addRange(start, stop);
  }
  get label() {
    return this.#label;
  }
  get serializationType() {
    return TransitionType.RANGE;
  }
  matches(symbol, _minVocabSymbol, _maxVocabSymbol) {
    return symbol >= this.start && symbol <= this.stop;
  }
  toString() {
    return "'" + String.fromCharCode(this.start) + "'..'" + String.fromCharCode(this.stop) + "'";
  }
};

// src/atn/EpsilonTransition.ts
var EpsilonTransition = class extends Transition {
  static {
    __name(this, "EpsilonTransition");
  }
  #outermostPrecedenceReturn;
  constructor(target, outermostPrecedenceReturn = -1) {
    super(target);
    this.#outermostPrecedenceReturn = outermostPrecedenceReturn;
  }
  get isEpsilon() {
    return true;
  }
  matches(_symbol, _minVocabSymbol, _maxVocabSymbol) {
    return false;
  }
  /**
   * @returns the rule index of a precedence rule for which this transition is
   * returning from, where the precedence value is 0; otherwise, -1.
   *
   * @see ATNConfig#isPrecedenceFilterSuppressed()
   * @see ParserATNSimulator#applyPrecedenceFilter(ATNConfigSet)
   * @since 4.4.1
   */
  get outermostPrecedenceReturn() {
    return this.#outermostPrecedenceReturn;
  }
  get serializationType() {
    return TransitionType.EPSILON;
  }
  toString() {
    return "epsilon";
  }
};

// src/atn/PredicateTransition.ts
var PredicateTransition = class extends AbstractPredicateTransition {
  static {
    __name(this, "PredicateTransition");
  }
  ruleIndex;
  predIndex;
  isCtxDependent;
  // e.g., $i ref in pred
  constructor(target, ruleIndex, predIndex, isCtxDependent) {
    super(target);
    this.ruleIndex = ruleIndex;
    this.predIndex = predIndex;
    this.isCtxDependent = isCtxDependent;
  }
  get isEpsilon() {
    return true;
  }
  matches(_symbol, _minVocabSymbol, _maxVocabSymbol) {
    return false;
  }
  get serializationType() {
    return TransitionType.PREDICATE;
  }
  getPredicate() {
    return new SemanticContext.Predicate(this.ruleIndex, this.predIndex, this.isCtxDependent);
  }
  toString() {
    return "pred_" + this.ruleIndex + ":" + this.predIndex;
  }
};

// src/atn/PrecedencePredicateTransition.ts
var PrecedencePredicateTransition = class extends AbstractPredicateTransition {
  static {
    __name(this, "PrecedencePredicateTransition");
  }
  precedence;
  constructor(target, precedence) {
    super(target);
    this.precedence = precedence;
  }
  get isEpsilon() {
    return true;
  }
  matches(_symbol, _minVocabSymbol, _maxVocabSymbol) {
    return false;
  }
  getPredicate() {
    return new SemanticContext.PrecedencePredicate(this.precedence);
  }
  get serializationType() {
    return TransitionType.PRECEDENCE;
  }
  toString() {
    return this.precedence + " >= _p";
  }
};

// src/atn/LexerActionType.ts
var LexerActionType = {
  // The type of a {@link LexerChannelAction} action.
  CHANNEL: 0,
  // The type of a {@link LexerCustomAction} action
  CUSTOM: 1,
  // The type of a {@link LexerModeAction} action.
  MODE: 2,
  //The type of a {@link LexerMoreAction} action.
  MORE: 3,
  //The type of a {@link LexerPopModeAction} action.
  POP_MODE: 4,
  //The type of a {@link LexerPushModeAction} action.
  PUSH_MODE: 5,
  //The type of a {@link LexerSkipAction} action.
  SKIP: 6,
  //The type of a {@link LexerTypeAction} action.
  TYPE: 7
};

// src/atn/LexerSkipAction.ts
var LexerSkipAction = class _LexerSkipAction extends LexerAction {
  static {
    __name(this, "LexerSkipAction");
  }
  /** Provides a singleton instance of this parameter-less lexer action. */
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INSTANCE = new _LexerSkipAction();
  constructor() {
    super(LexerActionType.SKIP);
  }
  execute(lexer) {
    lexer.skip();
  }
  toString() {
    return "skip";
  }
};

// src/atn/LexerChannelAction.ts
var LexerChannelAction = class _LexerChannelAction extends LexerAction {
  static {
    __name(this, "LexerChannelAction");
  }
  channel;
  constructor(channel) {
    super(LexerActionType.CHANNEL);
    this.channel = channel;
  }
  /**
   * <p>This action is implemented by calling {@link Lexer//setChannel} with the
   * value provided by {@link getChannel}.</p>
   */
  execute(lexer) {
    lexer._channel = this.channel;
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.channel);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerChannelAction)) {
      return false;
    } else {
      return this.channel === other.channel;
    }
  }
  toString() {
    return "channel(" + this.channel + ")";
  }
};

// src/atn/LexerCustomAction.ts
var LexerCustomAction = class _LexerCustomAction extends LexerAction {
  static {
    __name(this, "LexerCustomAction");
  }
  ruleIndex;
  actionIndex;
  /**
   * Constructs a custom lexer action with the specified rule and action
   * indexes.
   *
   * @param ruleIndex The rule index to use for calls to
   * {@link Recognizer//action}.
   * @param actionIndex The action index to use for calls to
   * {@link Recognizer//action}.
   */
  constructor(ruleIndex, actionIndex) {
    super(LexerActionType.CUSTOM);
    this.ruleIndex = ruleIndex;
    this.actionIndex = actionIndex;
    this.isPositionDependent = true;
  }
  /**
   * <p>Custom actions are implemented by calling {@link Lexer//action} with the
   * appropriate rule and action indexes.</p>
   */
  execute(lexer) {
    lexer.action(null, this.ruleIndex, this.actionIndex);
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.ruleIndex, this.actionIndex);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerCustomAction)) {
      return false;
    } else {
      return this.ruleIndex === other.ruleIndex && this.actionIndex === other.actionIndex;
    }
  }
};

// src/atn/LexerMoreAction.ts
var LexerMoreAction = class _LexerMoreAction extends LexerAction {
  static {
    __name(this, "LexerMoreAction");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INSTANCE = new _LexerMoreAction();
  constructor() {
    super(LexerActionType.MORE);
  }
  /**
   * <p>This action is implemented by calling {@link Lexer//popMode}.</p>
   */
  execute(lexer) {
    lexer.more();
  }
  toString() {
    return "more";
  }
};

// src/atn/LexerTypeAction.ts
var LexerTypeAction = class _LexerTypeAction extends LexerAction {
  static {
    __name(this, "LexerTypeAction");
  }
  type;
  constructor(type) {
    super(LexerActionType.TYPE);
    this.type = type;
  }
  execute(lexer) {
    lexer._type = this.type;
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.type);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerTypeAction)) {
      return false;
    } else {
      return this.type === other.type;
    }
  }
  toString() {
    return "type(" + this.type + ")";
  }
};

// src/atn/LexerPushModeAction.ts
var LexerPushModeAction = class _LexerPushModeAction extends LexerAction {
  static {
    __name(this, "LexerPushModeAction");
  }
  mode;
  constructor(mode) {
    super(LexerActionType.PUSH_MODE);
    this.mode = mode;
  }
  /**
   * <p>This action is implemented by calling {@link Lexer//pushMode} with the
   * value provided by {@link getMode}.</p>
   */
  execute(lexer) {
    lexer.pushMode(this.mode);
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.mode);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerPushModeAction)) {
      return false;
    } else {
      return this.mode === other.mode;
    }
  }
  toString() {
    return "pushMode(" + this.mode + ")";
  }
};

// src/atn/LexerPopModeAction.ts
var LexerPopModeAction = class _LexerPopModeAction extends LexerAction {
  static {
    __name(this, "LexerPopModeAction");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static INSTANCE = new _LexerPopModeAction();
  constructor() {
    super(LexerActionType.POP_MODE);
  }
  /**
   * <p>This action is implemented by calling {@link Lexer//popMode}.</p>
   */
  execute(lexer) {
    lexer.popMode();
  }
  toString() {
    return "popMode";
  }
};

// src/atn/LexerModeAction.ts
var LexerModeAction = class _LexerModeAction extends LexerAction {
  static {
    __name(this, "LexerModeAction");
  }
  mode;
  constructor(mode) {
    super(LexerActionType.MODE);
    this.mode = mode;
  }
  /**
   * <p>This action is implemented by calling {@link Lexer//mode} with the
   * value provided by {@link getMode}.</p>
   */
  execute(lexer) {
    lexer.mode(this.mode);
  }
  updateHashCode(hash) {
    hash.update(this.actionType, this.mode);
  }
  equals(other) {
    if (this === other) {
      return true;
    } else if (!(other instanceof _LexerModeAction)) {
      return false;
    } else {
      return this.mode === other.mode;
    }
  }
  toString() {
    return "mode(" + this.mode + ")";
  }
};

// src/atn/ATNDeserializer.ts
var initArray = /* @__PURE__ */ __name((length, value) => {
  const tmp = new Array(length - 1);
  tmp[length - 1] = value;
  return tmp.map(() => {
    return value;
  });
}, "initArray");
var ATNDeserializer = class _ATNDeserializer {
  static {
    __name(this, "ATNDeserializer");
  }
  static SERIALIZED_VERSION = 4;
  data = [];
  pos = 0;
  deserializationOptions;
  stateFactories = null;
  actionFactories = null;
  constructor(options) {
    if (!options) {
      options = ATNDeserializationOptions.defaultOptions;
    }
    this.deserializationOptions = options;
    this.stateFactories = null;
    this.actionFactories = null;
  }
  deserialize(data) {
    this.reset(data);
    this.checkVersion();
    const atn = this.readATN();
    this.readStates(atn);
    this.readRules(atn);
    this.readModes(atn);
    const sets = [];
    this.readSets(atn, sets);
    this.readEdges(atn, sets);
    this.readDecisions(atn);
    this.readLexerActions(atn);
    this.markPrecedenceDecisions(atn);
    this.verifyATN(atn);
    if (this.deserializationOptions.generateRuleBypassTransitions && atn.grammarType === ATNType.PARSER) {
      this.generateRuleBypassTransitions(atn);
      this.verifyATN(atn);
    }
    return atn;
  }
  reset(data) {
    this.data = data;
    this.pos = 0;
    return false;
  }
  checkVersion() {
    const version = this.readInt();
    if (version !== _ATNDeserializer.SERIALIZED_VERSION) {
      throw new Error("Could not deserialize ATN with version " + version + " (expected " + _ATNDeserializer.SERIALIZED_VERSION + ").");
    }
  }
  readATN() {
    const grammarType = this.readInt();
    const maxTokenType = this.readInt();
    return new ATN(grammarType, maxTokenType);
  }
  readStates(atn) {
    let j;
    let stateNumber;
    const loopBackStateNumbers = [];
    const endStateNumbers = [];
    const stateCount = this.readInt();
    for (let i = 0; i < stateCount; i++) {
      const stateType = this.readInt();
      if (stateType === ATNStateType.INVALID_TYPE) {
        atn.addState(null);
        continue;
      }
      const ruleIndex = this.readInt();
      const s = this.stateFactory(stateType, ruleIndex);
      if (stateType === ATNStateType.LOOP_END) {
        const loopBackStateNumber = this.readInt();
        loopBackStateNumbers.push([s, loopBackStateNumber]);
      } else if (s instanceof BlockStartState) {
        const endStateNumber = this.readInt();
        endStateNumbers.push([s, endStateNumber]);
      }
      atn.addState(s);
    }
    for (j = 0; j < loopBackStateNumbers.length; j++) {
      const pair = loopBackStateNumbers[j];
      pair[0].loopBackState = atn.states[pair[1]];
    }
    for (j = 0; j < endStateNumbers.length; j++) {
      const pair = endStateNumbers[j];
      pair[0].endState = atn.states[pair[1]];
    }
    const numNonGreedyStates = this.readInt();
    for (j = 0; j < numNonGreedyStates; j++) {
      stateNumber = this.readInt();
      atn.states[stateNumber].nonGreedy = true;
    }
    const numPrecedenceStates = this.readInt();
    for (j = 0; j < numPrecedenceStates; j++) {
      stateNumber = this.readInt();
      atn.states[stateNumber].isPrecedenceRule = true;
    }
  }
  readRules(atn) {
    let i;
    const ruleCount = this.readInt();
    if (atn.grammarType === ATNType.LEXER) {
      atn.ruleToTokenType = initArray(ruleCount, 0);
    }
    atn.ruleToStartState = initArray(ruleCount, null);
    for (i = 0; i < ruleCount; i++) {
      const s = this.readInt();
      atn.ruleToStartState[i] = atn.states[s];
      if (atn.grammarType === ATNType.LEXER) {
        const tokenType = this.readInt();
        atn.ruleToTokenType[i] = tokenType;
      }
    }
    atn.ruleToStopState = initArray(ruleCount, null);
    for (i = 0; i < atn.states.length; i++) {
      const state = atn.states[i];
      if (!(state instanceof RuleStopState)) {
        continue;
      }
      atn.ruleToStopState[state.ruleIndex] = state;
      atn.ruleToStartState[state.ruleIndex].stopState = state;
    }
  }
  readModes(atn) {
    const modeCount = this.readInt();
    for (let i = 0; i < modeCount; i++) {
      const s = this.readInt();
      atn.modeToStartState.push(atn.states[s]);
    }
  }
  readSets(atn, sets) {
    const m2 = this.readInt();
    for (let i = 0; i < m2; i++) {
      const intervalSet = new IntervalSet();
      sets.push(intervalSet);
      const n2 = this.readInt();
      const containsEof = this.readInt();
      if (containsEof !== 0) {
        intervalSet.addOne(-1);
      }
      for (let j = 0; j < n2; j++) {
        const i1 = this.readInt();
        const i2 = this.readInt();
        intervalSet.addRange(i1, i2);
      }
    }
  }
  readEdges(atn, sets) {
    let i;
    let j;
    let state;
    let trans;
    let target;
    const edgeCount = this.readInt();
    for (i = 0; i < edgeCount; i++) {
      const src = this.readInt();
      const trg = this.readInt();
      const ttype = this.readInt();
      const arg1 = this.readInt();
      const arg2 = this.readInt();
      const arg3 = this.readInt();
      trans = this.edgeFactory(atn, ttype, trg, arg1, arg2, arg3, sets);
      const srcState = atn.states[src];
      srcState.addTransition(trans);
    }
    for (i = 0; i < atn.states.length; i++) {
      state = atn.states[i];
      for (j = 0; j < state.transitions.length; j++) {
        const t = state.transitions[j];
        if (!(t instanceof RuleTransition)) {
          continue;
        }
        let outermostPrecedenceReturn = -1;
        if (atn.ruleToStartState[t.target.ruleIndex].isPrecedenceRule) {
          if (t.precedence === 0) {
            outermostPrecedenceReturn = t.target.ruleIndex;
          }
        }
        trans = new EpsilonTransition(t.followState, outermostPrecedenceReturn);
        atn.ruleToStopState[t.target.ruleIndex].addTransition(trans);
      }
    }
    for (i = 0; i < atn.states.length; i++) {
      state = atn.states[i];
      if (state instanceof BlockStartState) {
        if (state.endState === null) {
          throw new Error("IllegalState");
        }
        if (state.endState.startState) {
          throw new Error("IllegalState");
        }
        state.endState.startState = state;
      }
      if (state instanceof PlusLoopbackState) {
        for (j = 0; j < state.transitions.length; j++) {
          target = state.transitions[j].target;
          if (target instanceof PlusBlockStartState) {
            target.loopBackState = state;
          }
        }
      } else if (state instanceof StarLoopbackState) {
        for (j = 0; j < state.transitions.length; j++) {
          target = state.transitions[j].target;
          if (target instanceof StarLoopEntryState) {
            target.loopBackState = state;
          }
        }
      }
    }
  }
  readDecisions(atn) {
    const decisionCount = this.readInt();
    for (let i = 0; i < decisionCount; i++) {
      const s = this.readInt();
      const decState = atn.states[s];
      atn.decisionToState.push(decState);
      decState.decision = i;
    }
  }
  readLexerActions(atn) {
    if (atn.grammarType === ATNType.LEXER) {
      const count = this.readInt();
      atn.lexerActions = [];
      for (let i = 0; i < count; i++) {
        const actionType = this.readInt();
        const data1 = this.readInt();
        const data2 = this.readInt();
        atn.lexerActions.push(this.lexerActionFactory(actionType, data1, data2));
      }
    }
  }
  generateRuleBypassTransitions(atn) {
    let i;
    const count = atn.ruleToStartState.length;
    for (i = 0; i < count; i++) {
      atn.ruleToTokenType[i] = atn.maxTokenType + i + 1;
    }
    for (i = 0; i < count; i++) {
      this.generateRuleBypassTransition(atn, i);
    }
  }
  generateRuleBypassTransition(atn, idx) {
    let i;
    let state;
    const bypassStart = new BasicBlockStartState();
    bypassStart.ruleIndex = idx;
    atn.addState(bypassStart);
    const bypassStop = new BlockEndState();
    bypassStop.ruleIndex = idx;
    atn.addState(bypassStop);
    bypassStart.endState = bypassStop;
    atn.defineDecisionState(bypassStart);
    bypassStop.startState = bypassStart;
    let excludeTransition = null;
    let endState = null;
    if (atn.ruleToStartState[idx].isPrecedenceRule) {
      endState = null;
      for (i = 0; i < atn.states.length; i++) {
        state = atn.states[i];
        if (this.stateIsEndStateFor(state, idx)) {
          endState = state;
          excludeTransition = state.loopBackState.transitions[0];
          break;
        }
      }
      if (excludeTransition === null) {
        throw new Error("Couldn't identify final state of the precedence rule prefix section.");
      }
    } else {
      endState = atn.ruleToStopState[idx];
    }
    for (i = 0; i < atn.states.length; i++) {
      state = atn.states[i];
      for (const transition of state.transitions) {
        if (transition === excludeTransition) {
          continue;
        }
        if (transition.target === endState) {
          transition.target = bypassStop;
        }
      }
    }
    const ruleToStartState = atn.ruleToStartState[idx];
    const count = ruleToStartState.transitions.length;
    while (count > 0) {
      bypassStart.addTransition(ruleToStartState.transitions[count - 1]);
      ruleToStartState.transitions = ruleToStartState.transitions.slice(-1);
    }
    atn.ruleToStartState[idx].addTransition(new EpsilonTransition(bypassStart));
    if (endState) {
      bypassStop.addTransition(new EpsilonTransition(endState));
    }
    const matchState = new BasicState();
    atn.addState(matchState);
    matchState.addTransition(new AtomTransition(bypassStop, atn.ruleToTokenType[idx]));
    bypassStart.addTransition(new EpsilonTransition(matchState));
  }
  stateIsEndStateFor(state, idx) {
    if (state.ruleIndex !== idx) {
      return null;
    }
    if (!(state instanceof StarLoopEntryState)) {
      return null;
    }
    const maybeLoopEndState = state.transitions[state.transitions.length - 1].target;
    if (!(maybeLoopEndState instanceof LoopEndState)) {
      return null;
    }
    if (maybeLoopEndState.epsilonOnlyTransitions && maybeLoopEndState.transitions[0].target instanceof RuleStopState) {
      return state;
    } else {
      return null;
    }
  }
  /**
   * Analyze the {@link StarLoopEntryState} states in the specified ATN to set
   * the {@link StarLoopEntryState} field to the correct value.
   *
   * @param atn The ATN.
   */
  markPrecedenceDecisions(atn) {
    for (const state of atn.states) {
      if (!(state instanceof StarLoopEntryState)) {
        continue;
      }
      if (atn.ruleToStartState[state.ruleIndex].isPrecedenceRule) {
        const maybeLoopEndState = state.transitions[state.transitions.length - 1].target;
        if (maybeLoopEndState instanceof LoopEndState) {
          if (maybeLoopEndState.epsilonOnlyTransitions && maybeLoopEndState.transitions[0].target instanceof RuleStopState) {
            state.precedenceRuleDecision = true;
          }
        }
      }
    }
  }
  verifyATN(atn) {
    if (!this.deserializationOptions.verifyATN) {
      return;
    }
    for (const state of atn.states) {
      if (state === null) {
        continue;
      }
      this.checkCondition(state.epsilonOnlyTransitions || state.transitions.length <= 1);
      if (state instanceof PlusBlockStartState) {
        this.checkCondition(state.loopBackState !== null);
      } else if (state instanceof StarLoopEntryState) {
        this.checkCondition(state.loopBackState !== null);
        this.checkCondition(state.transitions.length === 2);
        if (state.transitions[0].target instanceof StarBlockStartState) {
          this.checkCondition(state.transitions[1].target instanceof LoopEndState);
          this.checkCondition(!state.nonGreedy);
        } else if (state.transitions[0].target instanceof LoopEndState) {
          this.checkCondition(state.transitions[1].target instanceof StarBlockStartState);
          this.checkCondition(state.nonGreedy);
        } else {
          throw new Error("IllegalState");
        }
      } else if (state instanceof StarLoopbackState) {
        this.checkCondition(state.transitions.length === 1);
        this.checkCondition(state.transitions[0].target instanceof StarLoopEntryState);
      } else if (state instanceof LoopEndState) {
        this.checkCondition(state.loopBackState !== null);
      } else if (state instanceof RuleStartState) {
        this.checkCondition(state.stopState !== null);
      } else if (state instanceof BlockStartState) {
        this.checkCondition(state.endState !== null);
      } else if (state instanceof BlockEndState) {
        this.checkCondition(state.startState !== null);
      } else if (state instanceof DecisionState) {
        this.checkCondition(state.transitions.length <= 1 || state.decision >= 0);
      } else {
        this.checkCondition(state.transitions.length <= 1 || state instanceof RuleStopState);
      }
    }
  }
  checkCondition(condition, message) {
    if (!condition) {
      if (message === void 0 || message === null) {
        message = "IllegalState";
      }
      throw message;
    }
  }
  readInt() {
    return this.data[this.pos++];
  }
  edgeFactory(atn, type, trg, arg1, arg2, arg3, sets) {
    const target = atn.states[trg];
    switch (type) {
      case TransitionType.EPSILON:
        return new EpsilonTransition(target);
      case TransitionType.RANGE:
        return arg3 !== 0 ? new RangeTransition(target, Token.EOF, arg2) : new RangeTransition(target, arg1, arg2);
      case TransitionType.RULE:
        return new RuleTransition(atn.states[arg1], arg2, arg3, target);
      case TransitionType.PREDICATE:
        return new PredicateTransition(target, arg1, arg2, arg3 !== 0);
      case TransitionType.PRECEDENCE:
        return new PrecedencePredicateTransition(target, arg1);
      case TransitionType.ATOM:
        return arg3 !== 0 ? new AtomTransition(target, Token.EOF) : new AtomTransition(target, arg1);
      case TransitionType.ACTION:
        return new ActionTransition(target, arg1, arg2, arg3 !== 0);
      case TransitionType.SET:
        return new SetTransition(target, sets[arg1]);
      case TransitionType.NOT_SET:
        return new NotSetTransition(target, sets[arg1]);
      case TransitionType.WILDCARD:
        return new WildcardTransition(target);
      default:
        throw new Error("The specified transition type: " + type + " is not valid.");
    }
  }
  stateFactory(type, ruleIndex) {
    if (this.stateFactories === null) {
      const sf = [];
      sf[ATNStateType.INVALID_TYPE] = null;
      sf[ATNStateType.BASIC] = () => {
        return new BasicState();
      };
      sf[ATNStateType.RULE_START] = () => {
        return new RuleStartState();
      };
      sf[ATNStateType.BLOCK_START] = () => {
        return new BasicBlockStartState();
      };
      sf[ATNStateType.PLUS_BLOCK_START] = () => {
        return new PlusBlockStartState();
      };
      sf[ATNStateType.STAR_BLOCK_START] = () => {
        return new StarBlockStartState();
      };
      sf[ATNStateType.TOKEN_START] = () => {
        return new TokensStartState();
      };
      sf[ATNStateType.RULE_STOP] = () => {
        return new RuleStopState();
      };
      sf[ATNStateType.BLOCK_END] = () => {
        return new BlockEndState();
      };
      sf[ATNStateType.STAR_LOOP_BACK] = () => {
        return new StarLoopbackState();
      };
      sf[ATNStateType.STAR_LOOP_ENTRY] = () => {
        return new StarLoopEntryState();
      };
      sf[ATNStateType.PLUS_LOOP_BACK] = () => {
        return new PlusLoopbackState();
      };
      sf[ATNStateType.LOOP_END] = () => {
        return new LoopEndState();
      };
      this.stateFactories = sf;
    }
    if (type > this.stateFactories.length || this.stateFactories[type] === null) {
      throw new Error("The specified state type " + type + " is not valid.");
    } else {
      const s = this.stateFactories[type]?.() ?? null;
      if (s !== null) {
        s.ruleIndex = ruleIndex;
        return s;
      }
    }
    return null;
  }
  lexerActionFactory(type, data1, data2) {
    if (this.actionFactories === null) {
      const af = [];
      af[LexerActionType.CHANNEL] = (data12, _data2) => {
        return new LexerChannelAction(data12);
      };
      af[LexerActionType.CUSTOM] = (data12, data22) => {
        return new LexerCustomAction(data12, data22);
      };
      af[LexerActionType.MODE] = (data12, _data2) => {
        return new LexerModeAction(data12);
      };
      af[LexerActionType.MORE] = (_data1, _data2) => {
        return LexerMoreAction.INSTANCE;
      };
      af[LexerActionType.POP_MODE] = (_data1, _data2) => {
        return LexerPopModeAction.INSTANCE;
      };
      af[LexerActionType.PUSH_MODE] = (data12, _data2) => {
        return new LexerPushModeAction(data12);
      };
      af[LexerActionType.SKIP] = (_data1, _data2) => {
        return LexerSkipAction.INSTANCE;
      };
      af[LexerActionType.TYPE] = (data12, _data2) => {
        return new LexerTypeAction(data12);
      };
      this.actionFactories = af;
    }
    if (type > this.actionFactories.length || this.actionFactories[type] === null) {
      throw new Error("The specified lexer action type " + type + " is not valid.");
    } else {
      return this.actionFactories[type](data1, data2);
    }
  }
};

// src/NoViableAltException.ts
var NoViableAltException = class extends RecognitionException {
  static {
    __name(this, "NoViableAltException");
  }
  /** Which configurations did we try at input.index() that couldn't match input.LT(1)? */
  deadEndConfigs = null;
  /**
   * The token object at the start index; the input stream might
   * 	not be buffering tokens so get a reference to it. (At the
   *  time the error occurred, of course the stream needs to keep a
   *  buffer all of the tokens but later we might not have access to those.)
   */
  startToken;
  constructor(recognizer, input = null, startToken = null, offendingToken = null, deadEndConfigs = null, ctx = null) {
    ctx = ctx ?? recognizer.context;
    offendingToken = offendingToken ?? recognizer.getCurrentToken();
    startToken = startToken ?? recognizer.getCurrentToken();
    input = input ?? recognizer.inputStream;
    super({ message: "", recognizer, input, ctx });
    this.deadEndConfigs = deadEndConfigs;
    this.startToken = startToken;
    this.offendingToken = offendingToken;
  }
};

// src/utils/DoubleDict.ts
var DoubleDict = class {
  static {
    __name(this, "DoubleDict");
  }
  cacheMap;
  constructor() {
    this.cacheMap = new HashMap();
  }
  get(a, b) {
    const d = this.cacheMap.get(a) ?? null;
    return d === null ? null : d.get(b) ?? null;
  }
  set(a, b, o) {
    let d = this.cacheMap.get(a) ?? null;
    if (d === null) {
      d = new HashMap();
      this.cacheMap.set(a, d);
    }
    d.set(b, o);
  }
};

// src/atn/PredictionMode.ts
var PredictionMode = class _PredictionMode {
  static {
    __name(this, "PredictionMode");
  }
  /**
   * The SLL(*) prediction mode. This prediction mode ignores the current
   * parser context when making predictions. This is the fastest prediction
   * mode, and provides correct results for many grammars. This prediction
   * mode is more powerful than the prediction mode provided by ANTLR 3, but
   * may result in syntax errors for grammar and input combinations which are
   * not SLL.
   *
   * <p>
   * When using this prediction mode, the parser will either return a correct
   * parse tree (i.e. the same parse tree that would be returned with the
   * {@link LL} prediction mode), or it will report a syntax error. If a
   * syntax error is encountered when using the {@link SLL} prediction mode,
   * it may be due to either an actual syntax error in the input or indicate
   * that the particular combination of grammar and input requires the more
   * powerful {@link LL} prediction abilities to complete successfully.</p>
   *
   * <p>
   * This prediction mode does not provide any guarantees for prediction
   * behavior for syntactically-incorrect inputs.</p>
   */
  static SLL = 0;
  /**
   * The LL(*) prediction mode. This prediction mode allows the current parser
   * context to be used for resolving SLL conflicts that occur during
   * prediction. This is the fastest prediction mode that guarantees correct
   * parse results for all combinations of grammars with syntactically correct
   * inputs.
   *
   * <p>
   * When using this prediction mode, the parser will make correct decisions
   * for all syntactically-correct grammar and input combinations. However, in
   * cases where the grammar is truly ambiguous this prediction mode might not
   * report a precise answer for <em>exactly which</em> alternatives are
   * ambiguous.</p>
   *
   * <p>
   * This prediction mode does not provide any guarantees for prediction
   * behavior for syntactically-incorrect inputs.</p>
   */
  static LL = 1;
  /**
   *
   * The LL(*) prediction mode with exact ambiguity detection. In addition to
   * the correctness guarantees provided by the {@link LL} prediction mode,
   * this prediction mode instructs the prediction algorithm to determine the
   * complete and exact set of ambiguous alternatives for every ambiguous
   * decision encountered while parsing.
   *
   * <p>
   * This prediction mode may be used for diagnosing ambiguities during
   * grammar development. Due to the performance overhead of calculating sets
   * of ambiguous alternatives, this prediction mode should be avoided when
   * the exact results are not necessary.</p>
   *
   * <p>
   * This prediction mode does not provide any guarantees for prediction
   * behavior for syntactically-incorrect inputs.</p>
   */
  static LL_EXACT_AMBIG_DETECTION = 2;
  /**
   *
   *Computes the SLL prediction termination condition.
   *
   *<p>
   *This method computes the SLL prediction termination condition for both of
   *the following cases.</p>
   *
   *<ul>
   *<li>The usual SLL+LL fallback upon SLL conflict</li>
   *<li>Pure SLL without LL fallback</li>
   *</ul>
   *
   *<p><strong>COMBINED SLL+LL PARSING</strong></p>
   *
   *<p>When LL-fallback is enabled upon SLL conflict, correct predictions are
   *ensured regardless of how the termination condition is computed by this
   *method. Due to the substantially higher cost of LL prediction, the
   *prediction should only fall back to LL when the additional lookahead
   *cannot lead to a unique SLL prediction.</p>
   *
   *<p>Assuming combined SLL+LL parsing, an SLL configuration set with only
   *conflicting subsets should fall back to full LL, even if the
   *configuration sets don't resolve to the same alternative (e.g.
   *{@code {1,2}} and {@code {3,4}}. If there is at least one non-conflicting
   *configuration, SLL could continue with the hopes that more lookahead will
   *resolve via one of those non-conflicting configurations.</p>
   *
   *<p>Here's the prediction termination rule them: SLL (for SLL+LL parsing)
   *stops when it sees only conflicting configuration subsets. In contrast,
   *full LL keeps going when there is uncertainty.</p>
   *
   *<p><strong>HEURISTIC</strong></p>
   *
   *<p>As a heuristic, we stop prediction when we see any conflicting subset
   *unless we see a state that only has one alternative associated with it.
   *The single-alt-state thing lets prediction continue upon rules like
   *(otherwise, it would admit defeat too soon):</p>
   *
   *<p>{@code [12|1|[], 6|2|[], 12|2|[]]. s : (ID | ID ID?) ';' ;}</p>
   *
   *<p>When the ATN simulation reaches the state before {@code ';'}, it has a
   *DFA state that looks like: {@code [12|1|[], 6|2|[], 12|2|[]]}. Naturally
   *{@code 12|1|[]} and {@code 12|2|[]} conflict, but we cannot stop
   *processing this node because alternative to has another way to continue,
   *via {@code [6|2|[]]}.</p>
   *
   *<p>It also let's us continue for this rule:</p>
   *
   *<p>{@code [1|1|[], 1|2|[], 8|3|[]] a : A | A | A B ;}</p>
   *
   *<p>After matching input A, we reach the stop state for rule A, state 1.
   *State 8 is the state right before B. Clearly alternatives 1 and 2
   *conflict and no amount of further lookahead will separate the two.
   *However, alternative 3 will be able to continue and so we do not stop
   *working on this state. In the previous example, we're concerned with
   *states associated with the conflicting alternatives. Here alt 3 is not
   *associated with the conflicting configs, but since we can continue
   *looking for input reasonably, don't declare the state done.</p>
   *
   *<p><strong>PURE SLL PARSING</strong></p>
   *
   *<p>To handle pure SLL parsing, all we have to do is make sure that we
   *combine stack contexts for configurations that differ only by semantic
   *predicate. From there, we can do the usual SLL termination heuristic.</p>
   *
   *<p><strong>PREDICATES IN SLL+LL PARSING</strong></p>
   *
   *<p>SLL decisions don't evaluate predicates until after they reach DFA stop
   *states because they need to create the DFA cache that works in all
   *semantic situations. In contrast, full LL evaluates predicates collected
   *during start state computation so it can ignore predicates thereafter.
   *This means that SLL termination detection can totally ignore semantic
   *predicates.</p>
   *
   *<p>Implementation-wise, {@link ATNConfigSet} combines stack contexts but not
   *semantic predicate contexts so we might see two configurations like the
   *following.</p>
   *
   *<p>{@code (s, 1, x, {}), (s, 1, x', {p})}</p>
   *
   *<p>Before testing these configurations against others, we have to merge
   *{@code x} and {@code x'} (without modifying the existing configurations).
   *For example, we test {@code (x+x')==x''} when looking for conflicts in
   *the following configurations.</p>
   *
   *<p>{@code (s, 1, x, {}), (s, 1, x', {p}), (s, 2, x'', {})}</p>
   *
   *<p>If the configuration set has predicates (as indicated by
   *{@link ATNConfigSet//hasSemanticContext}), this algorithm makes a copy of
   *the configurations to strip out all of the predicates so that a standard
   *{@link ATNConfigSet} will merge everything ignoring predicates.</p>
   */
  static hasSLLConflictTerminatingPrediction(mode, configs) {
    if (_PredictionMode.allConfigsInRuleStopStates(configs)) {
      return true;
    }
    if (mode === _PredictionMode.SLL) {
      if (configs.hasSemanticContext) {
        const dup = new ATNConfigSet();
        for (let c of configs.items) {
          c = new ATNConfig({ semanticContext: SemanticContext.NONE }, c);
          dup.add(c);
        }
        configs = dup;
      }
    }
    const altSets = _PredictionMode.getConflictingAltSubsets(configs);
    return _PredictionMode.hasConflictingAltSet(altSets) && !_PredictionMode.hasStateAssociatedWithOneAlt(configs);
  }
  /**
   * Checks if any configuration in {@code configs} is in a
   * {@link RuleStopState}. Configurations meeting this condition have reached
   * the end of the decision rule (local context) or end of start rule (full
   * context).
   *
   * @param configs the configuration set to test
   * @returns `true` if any configuration in {@code configs} is in a
   * {@link RuleStopState}, otherwise {@code false}
   */
  static hasConfigInRuleStopState(configs) {
    for (const c of configs.items) {
      if (c.state instanceof RuleStopState) {
        return true;
      }
    }
    return false;
  }
  /**
   * Checks if all configurations in {@code configs} are in a
   * {@link RuleStopState}. Configurations meeting this condition have reached
   * the end of the decision rule (local context) or end of start rule (full
   * context).
   *
   * @param configs the configuration set to test
   * @returns `true` if all configurations in {@code configs} are in a
   * {@link RuleStopState}, otherwise {@code false}
   */
  static allConfigsInRuleStopStates(configs) {
    for (const c of configs.items) {
      if (!(c.state instanceof RuleStopState)) {
        return false;
      }
    }
    return true;
  }
  /**
   *
   *Full LL prediction termination.
   *
   *<p>Can we stop looking ahead during ATN simulation or is there some
   *uncertainty as to which alternative we will ultimately pick, after
   *consuming more input? Even if there are partial conflicts, we might know
   *that everything is going to resolve to the same minimum alternative. That
   *means we can stop since no more lookahead will change that fact. On the
   *other hand, there might be multiple conflicts that resolve to different
   *minimums. That means we need more look ahead to decide which of those
   *alternatives we should predict.</p>
   *
   *<p>The basic idea is to split the set of configurations {@code C}, into
   *conflicting subsets {@code (s, _, ctx, _)} and singleton subsets with
   *non-conflicting configurations. Two configurations conflict if they have
   *identical {@link ATNConfig//state} and {@link ATNConfig//context} values
   *but different {@link ATNConfig//alt} value, e.g. {@code (s, i, ctx, _)}
   *and {@code (s, j, ctx, _)} for {@code i!=j}.</p>
   *
   *<p>Reduce these configuration subsets to the set of possible alternatives.
   *You can compute the alternative subsets in one pass as follows:</p>
   *
   *<p>{@code A_s,ctx = {i | (s, i, ctx, _)}} for each configuration in
   *{@code C} holding {@code s} and {@code ctx} fixed.</p>
   *
   *<p>Or in pseudo-code, for each configuration {@code c} in {@code C}:</p>
   *
   *<pre>
   *map[c] U= c.{@link ATNConfig//alt alt} // map hash/equals uses s and x, not
   *alt and not pred
   *</pre>
   *
   *<p>The values in {@code map} are the set of {@code A_s,ctx} sets.</p>
   *
   *<p>If {@code |A_s,ctx|=1} then there is no conflict associated with
   *{@code s} and {@code ctx}.</p>
   *
   *<p>Reduce the subsets to singletons by choosing a minimum of each subset. If
   *the union of these alternative subsets is a singleton, then no amount of
   *more lookahead will help us. We will always pick that alternative. If,
   *however, there is more than one alternative, then we are uncertain which
   *alternative to predict and must continue looking for resolution. We may
   *or may not discover an ambiguity in the future, even if there are no
   *conflicting subsets this round.</p>
   *
   *<p>The biggest sin is to terminate early because it means we've made a
   *decision but were uncertain as to the eventual outcome. We haven't used
   *enough lookahead. On the other hand, announcing a conflict too late is no
   *big deal; you will still have the conflict. It's just inefficient. It
   *might even look until the end of file.</p>
   *
   *<p>No special consideration for semantic predicates is required because
   *predicates are evaluated on-the-fly for full LL prediction, ensuring that
   *no configuration contains a semantic context during the termination
   *check.</p>
   *
   *<p><strong>CONFLICTING CONFIGS</strong></p>
   *
   *<p>Two configurations {@code (s, i, x)} and {@code (s, j, x')}, conflict
   *when {@code i!=j} but {@code x=x'}. Because we merge all
   *{@code (s, i, _)} configurations together, that means that there are at
   *most {@code n} configurations associated with state {@code s} for
   *{@code n} possible alternatives in the decision. The merged stacks
   *complicate the comparison of configuration contexts {@code x} and
   *{@code x'}. Sam checks to see if one is a subset of the other by calling
   *merge and checking to see if the merged result is either {@code x} or
   *{@code x'}. If the {@code x} associated with lowest alternative {@code i}
   *is the superset, then {@code i} is the only possible prediction since the
   *others resolve to {@code min(i)} as well. However, if {@code x} is
   *associated with {@code j>i} then at least one stack configuration for
   *{@code j} is not in conflict with alternative {@code i}. The algorithm
   *should keep going, looking for more lookahead due to the uncertainty.</p>
   *
   *<p>For simplicity, I'm doing a equality check between {@code x} and
   *{@code x'} that lets the algorithm continue to consume lookahead longer
   *than necessary. The reason I like the equality is of course the
   *simplicity but also because that is the test you need to detect the
   *alternatives that are actually in conflict.</p>
   *
   *<p><strong>CONTINUE/STOP RULE</strong></p>
   *
   *<p>Continue if union of resolved alternative sets from non-conflicting and
   *conflicting alternative subsets has more than one alternative. We are
   *uncertain about which alternative to predict.</p>
   *
   *<p>The complete set of alternatives, {@code [i for (_,i,_)]}, tells us which
   *alternatives are still in the running for the amount of input we've
   *consumed at this point. The conflicting sets let us to strip away
   *configurations that won't lead to more states because we resolve
   *conflicts to the configuration with a minimum alternate for the
   *conflicting set.</p>
   *
   *<p><strong>CASES</strong></p>
   *
   *<ul>
   *
   *<li>no conflicts and more than 1 alternative in set =&gt; continue</li>
   *
   *<li> {@code (s, 1, x)}, {@code (s, 2, x)}, {@code (s, 3, z)},
   *{@code (s', 1, y)}, {@code (s', 2, y)} yields non-conflicting set
   *{@code {3}} U conflicting sets {@code min({1,2})} U {@code min({1,2})} =
   *{@code {1,3}} =&gt; continue
   *</li>
   *
   *<li>{@code (s, 1, x)}, {@code (s, 2, x)}, {@code (s', 1, y)},
   *{@code (s', 2, y)}, {@code (s'', 1, z)} yields non-conflicting set
   *{@code {1}} U conflicting sets {@code min({1,2})} U {@code min({1,2})} =
   *{@code {1}} =&gt; stop and predict 1</li>
   *
   *<li>{@code (s, 1, x)}, {@code (s, 2, x)}, {@code (s', 1, y)},
   *{@code (s', 2, y)} yields conflicting, reduced sets {@code {1}} U
   *{@code {1}} = {@code {1}} =&gt; stop and predict 1, can announce
   *ambiguity {@code {1,2}}</li>
   *
   *<li>{@code (s, 1, x)}, {@code (s, 2, x)}, {@code (s', 2, y)},
   *{@code (s', 3, y)} yields conflicting, reduced sets {@code {1}} U
   *{@code {2}} = {@code {1,2}} =&gt; continue</li>
   *
   *<li>{@code (s, 1, x)}, {@code (s, 2, x)}, {@code (s', 3, y)},
   *{@code (s', 4, y)} yields conflicting, reduced sets {@code {1}} U
   *{@code {3}} = {@code {1,3}} =&gt; continue</li>
   *
   *</ul>
   *
   *<p><strong>EXACT AMBIGUITY DETECTION</strong></p>
   *
   *<p>If all states report the same conflicting set of alternatives, then we
   *know we have the exact ambiguity set.</p>
   *
   *<p><code>|A_<em>i</em>|&gt;1</code> and
   *<code>A_<em>i</em> = A_<em>j</em></code> for all <em>i</em>, <em>j</em>.</p>
   *
   *<p>In other words, we continue examining lookahead until all {@code A_i}
   *have more than one alternative and all {@code A_i} are the same. If
   *{@code A={{1,2}, {1,3}}}, then regular LL prediction would terminate
   *because the resolved set is {@code {1}}. To determine what the real
   *ambiguity is, we have to know whether the ambiguity is between one and
   *two or one and three so we keep going. We can only stop prediction when
   *we need exact ambiguity detection when the sets look like
   *{@code A={{1,2}}} or {@code {{1,2},{1,2}}}, etc...</p>
   */
  static resolvesToJustOneViableAlt(altSets) {
    return _PredictionMode.getSingleViableAlt(altSets);
  }
  /**
   * Determines if every alternative subset in {@code altSets} contains more
   * than one alternative.
   *
   * @param altSets a collection of alternative subsets
   * @returns `true` if every {@link BitSet} in {@code altSets} has
   * {@link BitSet//cardinality cardinality} &gt; 1, otherwise {@code false}
   */
  static allSubsetsConflict(altSets) {
    return !_PredictionMode.hasNonConflictingAltSet(altSets);
  }
  /**
   * Determines if any single alternative subset in {@code altSets} contains
   * exactly one alternative.
   *
   * @param altSets a collection of alternative subsets
   * @returns `true` if {@code altSets} contains a {@link BitSet} with
   * {@link BitSet//cardinality cardinality} 1, otherwise {@code false}
   */
  static hasNonConflictingAltSet(altSets) {
    for (const alts of altSets) {
      if (alts.length === 1) {
        return true;
      }
    }
    return false;
  }
  /**
   * Determines if any single alternative subset in {@code altSets} contains
   * more than one alternative.
   *
   * @param altSets a collection of alternative subsets
   * @returns `true` if {@code altSets} contains a {@link BitSet} with
   * {@link BitSet//cardinality cardinality} &gt; 1, otherwise {@code false}
   */
  static hasConflictingAltSet(altSets) {
    for (const alts of altSets) {
      if (alts.length > 1) {
        return true;
      }
    }
    return false;
  }
  /**
   * Determines if every alternative subset in {@code altSets} is equivalent.
   *
   * @param altSets a collection of alternative subsets
   * @returns `true` if every member of {@code altSets} is equal to the
   * others, otherwise {@code false}
   */
  static allSubsetsEqual(altSets) {
    let first = null;
    for (const alts of altSets) {
      if (first === null) {
        first = alts;
      } else if (alts !== first) {
        return false;
      }
    }
    return true;
  }
  /**
   * Returns the unique alternative predicted by all alternative subsets in
   * {@code altSets}. If no such alternative exists, this method returns
   * {@link ATN//INVALID_ALT_NUMBER}.
   *
   * @param altSets a collection of alternative subsets
   */
  static getUniqueAlt(altSets) {
    const all = _PredictionMode.getAlts(altSets);
    if (all.length === 1) {
      return all.nextSetBit(0);
    } else {
      return ATN.INVALID_ALT_NUMBER;
    }
  }
  /**
   * Gets the complete set of represented alternatives for a collection of
   * alternative subsets. This method returns the union of each {@link BitSet}
   * in {@code altSets}.
   *
   * @param altSets a collection of alternative subsets
   * @returns the set of represented alternatives in {@code altSets}
   */
  static getAlts(altSets) {
    const all = new BitSet();
    altSets.forEach((alts) => {
      all.or(alts);
    });
    return all;
  }
  /**
   * This function gets the conflicting alt subsets from a configuration set.
   * For each configuration {@code c} in {@code configs}:
   *
   * <pre>
   * map[c] U= c.{@link ATNConfig//alt alt} // map hash/equals uses s and x, not
   * alt and not pred
   * </pre>
   */
  static getConflictingAltSubsets(configs) {
    const configToAlts = new HashMap(
      (cfg) => {
        return HashCode.hashStuff(cfg.state.stateNumber, cfg.context);
      },
      (c12, c22) => {
        return c12.state.stateNumber === c22.state.stateNumber && (c12.context?.equals(c22.context) ?? true);
      }
    );
    configs.items.forEach((cfg) => {
      let alts = configToAlts.get(cfg);
      if (alts === null) {
        alts = new BitSet();
        configToAlts.set(cfg, alts);
      }
      alts.set(cfg.alt);
    });
    return configToAlts.getValues();
  }
  /**
   * Get a map from state to alt subset from a configuration set. For each
   * configuration {@code c} in {@code configs}:
   *
   * <pre>
   * map[c.{@link ATNConfig//state state}] U= c.{@link ATNConfig//alt alt}
   * </pre>
   */
  static getStateToAltMap(configs) {
    const m2 = new HashMap();
    configs.items.forEach((c) => {
      let alts = m2.get(c.state);
      if (!alts) {
        alts = new BitSet();
        m2.set(c.state, alts);
      }
      alts.set(c.alt);
    });
    return m2;
  }
  static hasStateAssociatedWithOneAlt(configs) {
    const counts = {};
    configs.items.forEach((c) => {
      const stateNumber = c.state.stateNumber;
      if (!counts[stateNumber]) {
        counts[stateNumber] = 0;
      }
      counts[stateNumber]++;
    });
    return Object.values(counts).some((count) => {
      return count === 1;
    });
  }
  static getSingleViableAlt(altSets) {
    let result = null;
    for (const alts of altSets) {
      const minAlt = alts.nextSetBit(0);
      if (result === null) {
        result = minAlt;
      } else if (result !== minAlt) {
        return ATN.INVALID_ALT_NUMBER;
      }
    }
    return result ?? 0;
  }
};

// src/atn/ParserATNSimulator.ts
var ParserATNSimulator = class _ParserATNSimulator extends ATNSimulator {
  static {
    __name(this, "ParserATNSimulator");
  }
  static debug;
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static trace_atn_sim = false;
  static debug_add = false;
  static debug_closure = false;
  static dfa_debug = false;
  static retry_debug = false;
  /** SLL, LL, or LL + exact ambig detection? */
  predictionMode;
  decisionToDFA;
  parser;
  /**
   * Each prediction operation uses a cache for merge of prediction contexts.
   *  Don't keep around as it wastes huge amounts of memory. DoubleKeyMap
   *  isn't synchronized but we're ok since two threads shouldn't reuse same
   *  parser/atn sim object because it can only handle one input at a time.
   *  This maps graphs a and b to merged result c. (a,b)->c. We can avoid
   *  the merge if we ever see a and b again.  Note that (b,a)->c should
   *  also be examined during cache lookup.
   */
  mergeCache = null;
  // LAME globals to avoid parameters!!!!! I need these down deep in predTransition
  _input = null;
  _startIndex = 0;
  _outerContext = null;
  _dfa = null;
  constructor(recog, atn, decisionToDFA, sharedContextCache) {
    super(atn, sharedContextCache);
    this.parser = recog;
    this.decisionToDFA = decisionToDFA;
  }
  static getUniqueAlt(configs) {
    let alt = ATN.INVALID_ALT_NUMBER;
    for (const c of configs.items) {
      if (alt === ATN.INVALID_ALT_NUMBER) {
        alt = c.alt;
      } else if (c.alt !== alt) {
        return ATN.INVALID_ALT_NUMBER;
      }
    }
    return alt;
  }
  reset() {
  }
  clearDFA() {
    for (let d = 0; d < this.decisionToDFA.length; d++) {
      this.decisionToDFA[d] = new DFA(this.atn.getDecisionState(d), d);
    }
  }
  adaptivePredict(input, decision, outerContext) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.trace_atn_sim) {
      console.log("adaptivePredict decision " + decision + " exec LA(1)==" + this.getLookaheadName(input) + " line " + input.LT(1).line + ":" + input.LT(1).column);
    }
    this._input = input;
    this._startIndex = input.index;
    this._outerContext = outerContext;
    const dfa = this.decisionToDFA[decision];
    this._dfa = dfa;
    const m2 = input.mark();
    const index = input.index;
    try {
      let s0;
      if (dfa.precedenceDfa) {
        s0 = dfa.getPrecedenceStartState(this.parser.getPrecedence());
      } else {
        s0 = dfa.s0;
      }
      if (s0 === null) {
        if (outerContext === null) {
          outerContext = ParserRuleContext.EMPTY;
        }
        if (_ParserATNSimulator.debug) {
          console.log("predictATN decision " + dfa.decision + " exec LA(1)==" + this.getLookaheadName(input) + ", outerContext=" + outerContext.toString(this.parser.ruleNames));
        }
        const fullCtx = false;
        let s0_closure = this.computeStartState(dfa.atnStartState, ParserRuleContext.EMPTY, fullCtx);
        if (dfa.precedenceDfa) {
          dfa.s0.configs = s0_closure;
          s0_closure = this.applyPrecedenceFilter(s0_closure);
          s0 = this.addDFAState(dfa, new DFAState(s0_closure));
          dfa.setPrecedenceStartState(this.parser.getPrecedence(), s0);
        } else {
          s0 = this.addDFAState(dfa, new DFAState(s0_closure));
          dfa.s0 = s0;
        }
      }
      const alt = this.execATN(dfa, s0, input, index, outerContext);
      if (_ParserATNSimulator.debug) {
        console.log("DFA after predictATN: " + dfa.toString(this.parser.vocabulary));
      }
      return alt;
    } finally {
      this._dfa = null;
      this.mergeCache = null;
      input.seek(index);
      input.release(m2);
    }
  }
  /**
   * Performs ATN simulation to compute a predicted alternative based
   *  upon the remaining input, but also updates the DFA cache to avoid
   *  having to traverse the ATN again for the same input sequence.
   *
   * There are some key conditions we're looking for after computing a new
   * set of ATN configs (proposed DFA state):
   *       if the set is empty, there is no viable alternative for current symbol
   *       does the state uniquely predict an alternative?
   *       does the state have a conflict that would prevent us from
   *         putting it on the work list?
   *
   * We also have some key operations to do:
   *       add an edge from previous DFA state to potentially new DFA state, D,
   *         upon current symbol but only if adding to work list, which means in all
   *         cases except no viable alternative (and possibly non-greedy decisions?)
   *       collecting predicates and adding semantic context to DFA accept states
   *       adding rule context to context-sensitive DFA accept states
   *       consuming an input symbol
   *       reporting a conflict
   *       reporting an ambiguity
   *       reporting a context sensitivity
   *       reporting insufficient predicates
   *
   * cover these cases:
   *    dead end
   *    single alt
   *    single alt + preds
   *    conflict
   *    conflict + preds
   */
  execATN(dfa, s0, input, startIndex, outerContext) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.trace_atn_sim) {
      console.log("execATN decision " + dfa.decision + ", DFA state " + s0 + ", LA(1)==" + this.getLookaheadName(input) + " line " + input.LT(1).line + ":" + input.LT(1).column);
    }
    let alt;
    let previousD = s0;
    if (_ParserATNSimulator.debug) {
      console.log("s0 = " + s0);
    }
    let t = input.LA(1);
    for (; ; ) {
      let D = this.getExistingTargetState(previousD, t);
      if (D === null) {
        D = this.computeTargetState(dfa, previousD, t);
      }
      if (D === ATNSimulator.ERROR) {
        const e = this.noViableAlt(input, outerContext, previousD.configs, startIndex);
        input.seek(startIndex);
        alt = this.getSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(previousD.configs, outerContext);
        if (alt !== ATN.INVALID_ALT_NUMBER) {
          return alt;
        } else {
          throw e;
        }
      }
      if (D.requiresFullContext && this.predictionMode !== PredictionMode.SLL) {
        let conflictingAlts = null;
        if (D.predicates !== null) {
          if (_ParserATNSimulator.debug) {
            console.log("DFA state has preds in DFA sim LL failover");
          }
          const conflictIndex = input.index;
          if (conflictIndex !== startIndex) {
            input.seek(startIndex);
          }
          conflictingAlts = this.evalSemanticContext(D.predicates, outerContext, true);
          if (conflictingAlts.length === 1) {
            if (_ParserATNSimulator.debug) {
              console.log("Full LL avoided");
            }
            return conflictingAlts.nextSetBit(0);
          }
          if (conflictIndex !== startIndex) {
            input.seek(conflictIndex);
          }
        }
        if (_ParserATNSimulator.dfa_debug) {
          console.log("ctx sensitive state " + outerContext + " in " + D);
        }
        const fullCtx = true;
        const s0_closure = this.computeStartState(dfa.atnStartState, outerContext, fullCtx);
        this.reportAttemptingFullContext(dfa, conflictingAlts, D.configs, startIndex, input.index);
        alt = this.execATNWithFullContext(dfa, D, s0_closure, input, startIndex, outerContext);
        return alt;
      }
      if (D.isAcceptState) {
        if (D.predicates === null) {
          return D.prediction;
        }
        const stopIndex = input.index;
        input.seek(startIndex);
        const alts = this.evalSemanticContext(D.predicates, outerContext, true);
        if (alts.length === 0) {
          throw this.noViableAlt(input, outerContext, D.configs, startIndex);
        } else if (alts.length === 1) {
          return alts.nextSetBit(0);
        } else {
          this.reportAmbiguity(dfa, D, startIndex, stopIndex, false, alts, D.configs);
          return alts.nextSetBit(0);
        }
      }
      previousD = D;
      if (t !== Token.EOF) {
        input.consume();
        t = input.LA(1);
      }
    }
  }
  /**
   * Get an existing target state for an edge in the DFA. If the target state
   * for the edge has not yet been computed or is otherwise not available,
   * this method returns {@code null}.
   *
   * @param previousD The current DFA state
   * @param t The next input symbol
   * @returns The existing target DFA state for the given input symbol
   * {@code t}, or {@code null} if the target state for this edge is not
   * already cached
   */
  getExistingTargetState(previousD, t) {
    const edges = previousD.edges;
    if (edges === null) {
      return null;
    } else {
      return edges[t + 1] || null;
    }
  }
  /**
   * Compute a target state for an edge in the DFA, and attempt to add the
   * computed state and corresponding edge to the DFA.
   *
   * @param dfa The DFA
   * @param previousD The current DFA state
   * @param t The next input symbol
   *
   * @returns The computed target DFA state for the given input symbol
   * {@code t}. If {@code t} does not lead to a valid DFA state, this method
   * returns {@link ERROR
   */
  computeTargetState(dfa, previousD, t) {
    const reach = this.computeReachSet(previousD.configs, t, false);
    if (reach === null) {
      this.addDFAEdge(dfa, previousD, t, ATNSimulator.ERROR);
      return ATNSimulator.ERROR;
    }
    let D = new DFAState(reach);
    const predictedAlt = _ParserATNSimulator.getUniqueAlt(reach);
    if (_ParserATNSimulator.debug) {
      const altSubSets = PredictionMode.getConflictingAltSubsets(reach);
      console.log("SLL altSubSets=" + arrayToString(altSubSets) + /*", previous=" + previousD.configs + */
      ", configs=" + reach + ", predict=" + predictedAlt + ", allSubsetsConflict=" + PredictionMode.allSubsetsConflict(altSubSets) + ", conflictingAlts=" + this.getConflictingAlts(reach));
    }
    if (predictedAlt !== ATN.INVALID_ALT_NUMBER) {
      D.isAcceptState = true;
      D.configs.uniqueAlt = predictedAlt;
      D.prediction = predictedAlt;
    } else if (PredictionMode.hasSLLConflictTerminatingPrediction(this.predictionMode, reach)) {
      D.configs.conflictingAlts = this.getConflictingAlts(reach);
      D.requiresFullContext = true;
      D.isAcceptState = true;
      D.prediction = D.configs.conflictingAlts.nextSetBit(0);
    }
    if (D.isAcceptState && D.configs.hasSemanticContext) {
      this.predicateDFAState(D, this.atn.getDecisionState(dfa.decision));
      if (D.predicates !== null) {
        D.prediction = ATN.INVALID_ALT_NUMBER;
      }
    }
    D = this.addDFAEdge(dfa, previousD, t, D);
    return D;
  }
  getRuleName(index) {
    if (this.parser !== null && index >= 0) {
      return this.parser.ruleNames[index];
    } else {
      return "<rule " + index + ">";
    }
  }
  getTokenName(t) {
    if (t === Token.EOF) {
      return "EOF";
    }
    const vocabulary = this.parser != null ? this.parser.vocabulary : Vocabulary.EMPTY_VOCABULARY;
    const displayName = vocabulary.getDisplayName(t);
    if (displayName === t.toString()) {
      return displayName;
    }
    return displayName + "<" + t + ">";
  }
  getLookaheadName(input) {
    return this.getTokenName(input.LA(1));
  }
  /**
   * Used for debugging in adaptivePredict around execATN but I cut
   * it out for clarity now that alg. works well. We can leave this
   * "dead" code for a bit
   */
  dumpDeadEndConfigs(e) {
    console.log("dead end configs: ");
    const decs = e.deadEndConfigs.items;
    for (const c of decs) {
      let trans = "no edges";
      if (c.state.transitions.length > 0) {
        const t = c.state.transitions[0];
        if (t instanceof AtomTransition) {
          trans = "Atom " + this.getTokenName(t.labelValue);
        } else if (t instanceof SetTransition) {
          const neg = t instanceof NotSetTransition;
          trans = (neg ? "~" : "") + "Set " + t.label;
        }
      }
      console.error(c.toString(this.parser, true) + ":" + trans);
    }
  }
  predicateDFAState(dfaState, decisionState) {
    const altCount = decisionState.transitions.length;
    const altsToCollectPredsFrom = this.getConflictingAltsOrUniqueAlt(dfaState.configs);
    const altToPred = this.getPredsForAmbigAlts(altsToCollectPredsFrom, dfaState.configs, altCount);
    if (altToPred !== null) {
      dfaState.predicates = this.getPredicatePredictions(altsToCollectPredsFrom, altToPred);
      dfaState.prediction = ATN.INVALID_ALT_NUMBER;
    } else {
      dfaState.prediction = altsToCollectPredsFrom.nextSetBit(0);
    }
  }
  // comes back with reach.uniqueAlt set to a valid alt
  execATNWithFullContext(dfa, D, s0, input, startIndex, outerContext) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.trace_atn_sim) {
      console.log("execATNWithFullContext " + s0);
    }
    const fullCtx = true;
    let foundExactAmbig = false;
    let reach;
    let previous = s0;
    input.seek(startIndex);
    let t = input.LA(1);
    let predictedAlt = -1;
    for (; ; ) {
      reach = this.computeReachSet(previous, t, fullCtx);
      if (reach === null) {
        const e = this.noViableAlt(input, outerContext, previous, startIndex);
        input.seek(startIndex);
        const alt = this.getSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(previous, outerContext);
        if (alt !== ATN.INVALID_ALT_NUMBER) {
          return alt;
        } else {
          throw e;
        }
      }
      const altSubSets = PredictionMode.getConflictingAltSubsets(reach);
      if (_ParserATNSimulator.debug) {
        console.log("LL altSubSets=" + altSubSets + ", predict=" + PredictionMode.getUniqueAlt(altSubSets) + ", resolvesToJustOneViableAlt=" + PredictionMode.resolvesToJustOneViableAlt(altSubSets));
      }
      reach.uniqueAlt = _ParserATNSimulator.getUniqueAlt(reach);
      if (reach.uniqueAlt !== ATN.INVALID_ALT_NUMBER) {
        predictedAlt = reach.uniqueAlt;
        break;
      } else if (this.predictionMode !== PredictionMode.LL_EXACT_AMBIG_DETECTION) {
        predictedAlt = PredictionMode.resolvesToJustOneViableAlt(altSubSets);
        if (predictedAlt !== ATN.INVALID_ALT_NUMBER) {
          break;
        }
      } else {
        if (PredictionMode.allSubsetsConflict(altSubSets) && PredictionMode.allSubsetsEqual(altSubSets)) {
          foundExactAmbig = true;
          predictedAlt = PredictionMode.getSingleViableAlt(altSubSets);
          break;
        }
      }
      previous = reach;
      if (t !== Token.EOF) {
        input.consume();
        t = input.LA(1);
      }
    }
    if (reach.uniqueAlt !== ATN.INVALID_ALT_NUMBER) {
      this.reportContextSensitivity(dfa, predictedAlt, reach, startIndex, input.index);
      return predictedAlt;
    }
    this.reportAmbiguity(dfa, D, startIndex, input.index, foundExactAmbig, null, reach);
    return predictedAlt;
  }
  computeReachSet(closure, t, fullCtx) {
    if (_ParserATNSimulator.debug) {
      console.log("in computeReachSet, starting closure: " + closure);
    }
    if (this.mergeCache === null) {
      this.mergeCache = new DoubleDict();
    }
    const intermediate = new ATNConfigSet(fullCtx);
    let skippedStopStates = null;
    for (const c of closure.items) {
      if (_ParserATNSimulator.debug) {
        console.log("testing " + this.getTokenName(t) + " at " + c);
      }
      if (c.state instanceof RuleStopState) {
        if (fullCtx || t === Token.EOF) {
          if (skippedStopStates === null) {
            skippedStopStates = [];
          }
          skippedStopStates.push(c);
          if (_ParserATNSimulator.debug_add) {
            console.log("added " + c + " to skippedStopStates");
          }
        }
        continue;
      }
      for (const trans of c.state.transitions) {
        const target = this.getReachableTarget(trans, t);
        if (target !== null) {
          const cfg = new ATNConfig({ state: target }, c);
          intermediate.add(cfg, this.mergeCache);
          if (_ParserATNSimulator.debug_add) {
            console.log("added " + cfg + " to intermediate");
          }
        }
      }
    }
    let reach = null;
    if (skippedStopStates === null && t !== Token.EOF) {
      if (intermediate.items.length === 1) {
        reach = intermediate;
      } else if (_ParserATNSimulator.getUniqueAlt(intermediate) !== ATN.INVALID_ALT_NUMBER) {
        reach = intermediate;
      }
    }
    if (reach === null) {
      reach = new ATNConfigSet(fullCtx);
      const closureBusy = new HashSet();
      const treatEofAsEpsilon = t === Token.EOF;
      for (const config of intermediate.items) {
        this.closure(config, reach, closureBusy, false, fullCtx, treatEofAsEpsilon);
      }
    }
    if (t === Token.EOF) {
      reach = this.removeAllConfigsNotInRuleStopState(reach, reach === intermediate);
    }
    if (skippedStopStates !== null && (!fullCtx || !PredictionMode.hasConfigInRuleStopState(reach))) {
      for (const config of skippedStopStates) {
        reach.add(config, this.mergeCache);
      }
    }
    if (_ParserATNSimulator.trace_atn_sim) {
      console.log("computeReachSet " + closure + " -> " + reach);
    }
    if (reach.items.length === 0) {
      return null;
    } else {
      return reach;
    }
  }
  /**
   * Return a configuration set containing only the configurations from
   * `configs` which are in a {@link RuleStopState}. If all
   * configurations in `configs` are already in a rule stop state, this
   * method simply returns `configs`.
   *
   * <p>When {@code lookToEndOfRule} is true, this method uses
   * {@link ATN//nextTokens} for each configuration in `configs` which is
   * not already in a rule stop state to see if a rule stop state is reachable
   * from the configuration via epsilon-only transitions.</p>
   *
   * @param configs the configuration set to update
   * @param lookToEndOfRule when true, this method checks for rule stop states
   * reachable by epsilon-only transitions from each configuration in
   * `configs`.
   *
   * @returns `configs` if all configurations in `configs` are in a
   * rule stop state, otherwise return a new configuration set containing only
   * the configurations from `configs` which are in a rule stop state
   */
  removeAllConfigsNotInRuleStopState(configs, lookToEndOfRule) {
    if (PredictionMode.allConfigsInRuleStopStates(configs)) {
      return configs;
    }
    const result = new ATNConfigSet(configs.fullCtx);
    for (const config of configs.items) {
      if (config.state instanceof RuleStopState) {
        result.add(config, this.mergeCache);
        continue;
      }
      if (lookToEndOfRule && config.state.epsilonOnlyTransitions) {
        const nextTokens = this.atn.nextTokens(config.state);
        if (nextTokens.contains(Token.EPSILON)) {
          const endOfRuleState = this.atn.ruleToStopState[config.state.ruleIndex];
          result.add(new ATNConfig({ state: endOfRuleState }, config), this.mergeCache);
        }
      }
    }
    return result;
  }
  computeStartState(p, ctx, fullCtx) {
    const initialContext = predictionContextFromRuleContext(this.atn, ctx);
    const configs = new ATNConfigSet(fullCtx);
    if (_ParserATNSimulator.trace_atn_sim) {
      console.log("computeStartState from ATN state " + p + " initialContext=" + initialContext.toString(this.parser));
    }
    for (let i = 0; i < p.transitions.length; i++) {
      const target = p.transitions[i].target;
      const c = new ATNConfig({ state: target, alt: i + 1, context: initialContext }, null);
      const closureBusy = new HashSet();
      this.closure(c, configs, closureBusy, true, fullCtx, false);
    }
    return configs;
  }
  /**
   * This method transforms the start state computed by
   * {@link computeStartState} to the special start state used by a
   * precedence DFA for a particular precedence value. The transformation
   * process applies the following changes to the start state's configuration
   * set.
   *
   * <ol>
   * <li>Evaluate the precedence predicates for each configuration using
   * {@link SemanticContext//evalPrecedence}.</li>
   * <li>Remove all configurations which predict an alternative greater than
   * 1, for which another configuration that predicts alternative 1 is in the
   * same ATN state with the same prediction context. This transformation is
   * valid for the following reasons:
   * <ul>
   * <li>The closure block cannot contain any epsilon transitions which bypass
   * the body of the closure, so all states reachable via alternative 1 are
   * part of the precedence alternatives of the transformed left-recursive
   * rule.</li>
   * <li>The "primary" portion of a left recursive rule cannot contain an
   * epsilon transition, so the only way an alternative other than 1 can exist
   * in a state that is also reachable via alternative 1 is by nesting calls
   * to the left-recursive rule, with the outer calls not being at the
   * preferred precedence level.</li>
   * </ul>
   * </li>
   * </ol>
   *
   * <p>
   * The prediction context must be considered by this filter to address
   * situations like the following.
   * </p>
   * <code>
   * <pre>
   * grammar TA;
   * prog: statement* EOF;
   * statement: letterA | statement letterA 'b' ;
   * letterA: 'a';
   * </pre>
   * </code>
   * <p>
   * If the above grammar, the ATN state immediately before the token
   * reference {@code 'a'} in {@code letterA} is reachable from the left edge
   * of both the primary and closure blocks of the left-recursive rule
   * {@code statement}. The prediction context associated with each of these
   * configurations distinguishes between them, and prevents the alternative
   * which stepped out to {@code prog} (and then back in to {@code statement}
   * from being eliminated by the filter.
   * </p>
   *
   * @param configs The configuration set computed by
   * {@link computeStartState} as the start state for the DFA.
   * @returns The transformed configuration set representing the start state
   * for a precedence DFA at a particular precedence level (determined by
   * calling {@link Parser//getPrecedence})
   */
  applyPrecedenceFilter(configs) {
    const statesFromAlt1 = [];
    const configSet = new ATNConfigSet(configs.fullCtx);
    for (const config of configs.items) {
      if (config.alt !== 1) {
        continue;
      }
      const updatedContext = config.semanticContext.evalPrecedence(this.parser, this._outerContext);
      if (updatedContext === null) {
        continue;
      }
      statesFromAlt1[config.state.stateNumber] = config.context;
      if (updatedContext !== config.semanticContext) {
        configSet.add(new ATNConfig({ semanticContext: updatedContext }, config), this.mergeCache);
      } else {
        configSet.add(config, this.mergeCache);
      }
    }
    for (const config of configs.items) {
      if (config.alt === 1) {
        continue;
      }
      if (!config.precedenceFilterSuppressed) {
        const context = statesFromAlt1[config.state.stateNumber] || null;
        if (context !== null && context.equals(config.context)) {
          continue;
        }
      }
      configSet.add(config, this.mergeCache);
    }
    return configSet;
  }
  getReachableTarget(trans, ttype) {
    if (trans.matches(ttype, 0, this.atn.maxTokenType)) {
      return trans.target;
    } else {
      return null;
    }
  }
  getPredsForAmbigAlts(ambigAlts, configs, altCount) {
    let altToPred = [];
    for (const c of configs.items) {
      if (ambigAlts.get(c.alt)) {
        altToPred[c.alt] = SemanticContext.orContext(altToPred[c.alt] ?? null, c.semanticContext);
      }
    }
    let nPredAlts = 0;
    for (let i = 1; i < altCount + 1; i++) {
      const pred = altToPred[i] ?? null;
      if (pred === null) {
        altToPred[i] = SemanticContext.NONE;
      } else if (pred !== SemanticContext.NONE) {
        nPredAlts += 1;
      }
    }
    if (nPredAlts === 0) {
      altToPred = null;
    }
    if (_ParserATNSimulator.debug) {
      console.log("getPredsForAmbigAlts result " + arrayToString(altToPred));
    }
    return altToPred;
  }
  getPredicatePredictions(ambigAlts, altToPred) {
    const pairs = [];
    let containsPredicate = false;
    for (let i = 1; i < altToPred.length; i++) {
      const pred = altToPred[i];
      if (ambigAlts !== null && ambigAlts.get(i)) {
        pairs.push(new DFAState.PredPrediction(pred, i));
      }
      if (pred !== SemanticContext.NONE) {
        containsPredicate = true;
      }
    }
    if (!containsPredicate) {
      return null;
    }
    return pairs;
  }
  /**
   * This method is used to improve the localization of error messages by
   * choosing an alternative rather than throwing a
   * {@link NoViableAltException} in particular prediction scenarios where the
   * {@link ERROR} state was reached during ATN simulation.
   *
   * <p>
   * The default implementation of this method uses the following
   * algorithm to identify an ATN configuration which successfully parsed the
   * decision entry rule. Choosing such an alternative ensures that the
   * {@link ParserRuleContext} returned by the calling rule will be complete
   * and valid, and the syntax error will be reported later at a more
   * localized location.</p>
   *
   * <ul>
   * <li>If a syntactically valid path or paths reach the end of the decision rule and
   * they are semantically valid if predicated, return the min associated alt.</li>
   * <li>Else, if a semantically invalid but syntactically valid path exist
   * or paths exist, return the minimum associated alt.
   * </li>
   * <li>Otherwise, return {@link ATN//INVALID_ALT_NUMBER}.</li>
   * </ul>
   *
   * <p>
   * In some scenarios, the algorithm described above could predict an
   * alternative which will result in a {@link FailedPredicateException} in
   * the parser. Specifically, this could occur if the <em>only</em> configuration
   * capable of successfully parsing to the end of the decision rule is
   * blocked by a semantic predicate. By choosing this alternative within
   * {@link adaptivePredict} instead of throwing a
   * {@link NoViableAltException}, the resulting
   * {@link FailedPredicateException} in the parser will identify the specific
   * predicate which is preventing the parser from successfully parsing the
   * decision rule, which helps developers identify and correct logic errors
   * in semantic predicates.
   * </p>
   *
   * @param configs The ATN configurations which were valid immediately before
   * the {@link ERROR} state was reached
   * @param outerContext The is the \gamma_0 initial parser context from the paper
   * or the parser stack at the instant before prediction commences.
   *
   * @returns The value to return from {@link adaptivePredict}, or
   * {@link ATN//INVALID_ALT_NUMBER} if a suitable alternative was not
   * identified and {@link adaptivePredict} should report an error instead
   */
  getSynValidOrSemInvalidAltThatFinishedDecisionEntryRule(configs, outerContext) {
    const splitConfigs = this.splitAccordingToSemanticValidity(configs, outerContext);
    const semValidConfigs = splitConfigs[0];
    const semInvalidConfigs = splitConfigs[1];
    let alt = this.getAltThatFinishedDecisionEntryRule(semValidConfigs);
    if (alt !== ATN.INVALID_ALT_NUMBER) {
      return alt;
    }
    if (semInvalidConfigs.items.length > 0) {
      alt = this.getAltThatFinishedDecisionEntryRule(semInvalidConfigs);
      if (alt !== ATN.INVALID_ALT_NUMBER) {
        return alt;
      }
    }
    return ATN.INVALID_ALT_NUMBER;
  }
  getAltThatFinishedDecisionEntryRule(configs) {
    const alts = [];
    for (const c of configs.items) {
      if (c.reachesIntoOuterContext > 0 || c.state instanceof RuleStopState && c.context.hasEmptyPath()) {
        if (alts.indexOf(c.alt) < 0) {
          alts.push(c.alt);
        }
      }
    }
    if (alts.length === 0) {
      return ATN.INVALID_ALT_NUMBER;
    } else {
      return Math.min(...alts);
    }
  }
  /**
   * Walk the list of configurations and split them according to
   * those that have preds evaluating to true/false.  If no pred, assume
   * true pred and include in succeeded set.  Returns Pair of sets.
   *
   * Create a new set so as not to alter the incoming parameter.
   *
   * Assumption: the input stream has been restored to the starting point
   * prediction, which is where predicates need to evaluate.
   */
  splitAccordingToSemanticValidity(configs, outerContext) {
    const succeeded = new ATNConfigSet(configs.fullCtx);
    const failed = new ATNConfigSet(configs.fullCtx);
    for (const c of configs.items) {
      if (c.semanticContext !== SemanticContext.NONE) {
        const predicateEvaluationResult = c.semanticContext.evaluate(this.parser, outerContext);
        if (predicateEvaluationResult) {
          succeeded.add(c);
        } else {
          failed.add(c);
        }
      } else {
        succeeded.add(c);
      }
    }
    return [succeeded, failed];
  }
  /**
   * Look through a list of predicate/alt pairs, returning alts for the
   * pairs that win. A {@code NONE} predicate indicates an alt containing an
   * unpredicated config which behaves as "always true." If !complete
   * then we stop at the first predicate that evaluates to true. This
   * includes pairs with null predicates.
   */
  evalSemanticContext(predPredictions, outerContext, complete) {
    const predictions = new BitSet();
    for (const pair of predPredictions) {
      if (pair.pred === SemanticContext.NONE) {
        predictions.set(pair.alt);
        if (!complete) {
          break;
        }
        continue;
      }
      const predicateEvaluationResult = pair.pred.evaluate(this.parser, outerContext);
      if (_ParserATNSimulator.debug || _ParserATNSimulator.dfa_debug) {
        console.log("eval pred " + pair + "=" + predicateEvaluationResult);
      }
      if (predicateEvaluationResult) {
        if (_ParserATNSimulator.debug || _ParserATNSimulator.dfa_debug) {
          console.log("PREDICT " + pair.alt);
        }
        predictions.set(pair.alt);
        if (!complete) {
          break;
        }
      }
    }
    return predictions;
  }
  // TODO: If we are doing predicates, there is no point in pursuing
  //     closure operations if we reach a DFA state that uniquely predicts
  //     alternative. We will not be caching that DFA state and it is a
  //     waste to pursue the closure. Might have to advance when we do
  //     ambig detection thought :(
  //
  closure(config, configs, closureBusy, collectPredicates, fullCtx, treatEofAsEpsilon) {
    const initialDepth = 0;
    this.closureCheckingStopState(
      config,
      configs,
      closureBusy,
      collectPredicates,
      fullCtx,
      initialDepth,
      treatEofAsEpsilon
    );
  }
  closureCheckingStopState(config, configs, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon) {
    if (_ParserATNSimulator.trace_atn_sim || _ParserATNSimulator.debug_closure) {
      console.log("closure(" + config.toString(this.parser, true) + ")");
    }
    if (config.state instanceof RuleStopState) {
      if (config.context && !config.context.isEmpty()) {
        for (let i = 0; i < config.context.length; i++) {
          if (config.context.getReturnState(i) === PredictionContext.EMPTY_RETURN_STATE) {
            if (fullCtx) {
              configs.add(new ATNConfig(
                { state: config.state, context: PredictionContext.EMPTY },
                config
              ), this.mergeCache);
              continue;
            } else {
              if (_ParserATNSimulator.debug) {
                console.log("FALLING off rule " + this.getRuleName(config.state.ruleIndex));
              }
              this.closure_(
                config,
                configs,
                closureBusy,
                collectPredicates,
                fullCtx,
                depth,
                treatEofAsEpsilon
              );
            }
            continue;
          }
          const returnState = this.atn.states[config.context.getReturnState(i)];
          const newContext = config.context.getParent(i);
          const parameters = {
            state: returnState,
            alt: config.alt,
            context: newContext,
            semanticContext: config.semanticContext
          };
          const c = new ATNConfig(parameters, null);
          c.reachesIntoOuterContext = config.reachesIntoOuterContext;
          this.closureCheckingStopState(
            c,
            configs,
            closureBusy,
            collectPredicates,
            fullCtx,
            depth - 1,
            treatEofAsEpsilon
          );
        }
        return;
      } else if (fullCtx) {
        configs.add(config, this.mergeCache);
        return;
      } else {
        if (_ParserATNSimulator.debug) {
          console.log("FALLING off rule " + this.getRuleName(config.state.ruleIndex));
        }
      }
    }
    this.closure_(config, configs, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon);
  }
  // Do the actual work of walking epsilon edges//
  closure_(config, configs, closureBusy, collectPredicates, fullCtx, depth, treatEofAsEpsilon) {
    const p = config.state;
    if (!p.epsilonOnlyTransitions) {
      configs.add(config, this.mergeCache);
    }
    for (let i = 0; i < p.transitions.length; i++) {
      if (i === 0 && this.canDropLoopEntryEdgeInLeftRecursiveRule(config)) {
        continue;
      }
      const t = p.transitions[i];
      const continueCollecting = collectPredicates && !(t instanceof ActionTransition);
      const c = this.getEpsilonTarget(config, t, continueCollecting, depth === 0, fullCtx, treatEofAsEpsilon);
      if (c !== null) {
        let newDepth = depth;
        if (config.state instanceof RuleStopState) {
          if (this._dfa !== null && this._dfa.precedenceDfa) {
            const outermostPrecedenceReturn = t.outermostPrecedenceReturn;
            if (outermostPrecedenceReturn === this._dfa.atnStartState?.ruleIndex) {
              c.precedenceFilterSuppressed = true;
            }
          }
          c.reachesIntoOuterContext += 1;
          if (closureBusy.add(c) !== c) {
            continue;
          }
          configs.dipsIntoOuterContext = true;
          newDepth -= 1;
          if (_ParserATNSimulator.debug) {
            console.log("dips into outer ctx: " + c);
          }
        } else {
          if (!t.isEpsilon && closureBusy.add(c) !== c) {
            continue;
          }
          if (t instanceof RuleTransition) {
            if (newDepth >= 0) {
              newDepth += 1;
            }
          }
        }
        this.closureCheckingStopState(
          c,
          configs,
          closureBusy,
          continueCollecting,
          fullCtx,
          newDepth,
          treatEofAsEpsilon
        );
      }
    }
  }
  canDropLoopEntryEdgeInLeftRecursiveRule(config) {
    const p = config.state;
    if (p.stateType !== ATNStateType.STAR_LOOP_ENTRY || !config.context) {
      return false;
    }
    if (!p.precedenceRuleDecision || config.context.isEmpty() || config.context.hasEmptyPath()) {
      return false;
    }
    const numCtxs = config.context.length;
    for (let i = 0; i < numCtxs; i++) {
      const returnState = this.atn.states[config.context.getReturnState(i)];
      if (returnState.ruleIndex !== p.ruleIndex) {
        return false;
      }
    }
    const decisionStartState = p.transitions[0].target;
    const blockEndStateNum = decisionStartState.endState.stateNumber;
    const blockEndState = this.atn.states[blockEndStateNum];
    for (let i = 0; i < numCtxs; i++) {
      const returnStateNumber = config.context.getReturnState(i);
      const returnState = this.atn.states[returnStateNumber];
      if (returnState.transitions.length !== 1 || !returnState.transitions[0].isEpsilon) {
        return false;
      }
      const returnStateTarget = returnState.transitions[0].target;
      if (returnState.stateType === ATNStateType.BLOCK_END && returnStateTarget === p) {
        continue;
      }
      if (returnState === blockEndState) {
        continue;
      }
      if (returnStateTarget === blockEndState) {
        continue;
      }
      if (returnStateTarget.stateType === ATNStateType.BLOCK_END && returnStateTarget.transitions.length === 1 && returnStateTarget.transitions[0].isEpsilon && returnStateTarget.transitions[0].target === p) {
        continue;
      }
      return false;
    }
    return true;
  }
  getEpsilonTarget(config, t, collectPredicates, inContext, fullCtx, treatEofAsEpsilon) {
    switch (t.serializationType) {
      case TransitionType.RULE:
        return this.ruleTransition(config, t);
      case TransitionType.PRECEDENCE:
        return this.precedenceTransition(
          config,
          t,
          collectPredicates,
          inContext,
          fullCtx
        );
      case TransitionType.PREDICATE:
        return this.predTransition(config, t, collectPredicates, inContext, fullCtx);
      case TransitionType.ACTION:
        return this.actionTransition(config, t);
      case TransitionType.EPSILON:
        return new ATNConfig({ state: t.target }, config);
      case TransitionType.ATOM:
      case TransitionType.RANGE:
      case TransitionType.SET:
        if (treatEofAsEpsilon) {
          if (t.matches(Token.EOF, 0, 1)) {
            return new ATNConfig({ state: t.target }, config);
          }
        }
        return null;
      default:
        return null;
    }
  }
  actionTransition(config, t) {
    if (_ParserATNSimulator.debug) {
      const index = t.actionIndex === -1 ? 65535 : t.actionIndex;
      console.log("ACTION edge " + t.ruleIndex + ":" + index);
    }
    return new ATNConfig({ state: t.target }, config);
  }
  precedenceTransition(config, pt, collectPredicates, inContext, fullCtx) {
    if (_ParserATNSimulator.debug) {
      console.log("PRED (collectPredicates=" + collectPredicates + ") " + pt.precedence + ">=_p, ctx dependent=true");
      if (this.parser !== null) {
        console.log("context surrounding pred is " + arrayToString(this.parser.getRuleInvocationStack()));
      }
    }
    let c = null;
    if (collectPredicates && inContext) {
      if (fullCtx && this._input) {
        const currentPosition = this._input.index;
        this._input.seek(this._startIndex);
        const predSucceeds = pt.getPredicate().evaluate(this.parser, this._outerContext);
        this._input.seek(currentPosition);
        if (predSucceeds) {
          c = new ATNConfig({ state: pt.target }, config);
        }
      } else {
        const newSemCtx = SemanticContext.andContext(config.semanticContext, pt.getPredicate());
        c = new ATNConfig({ state: pt.target, semanticContext: newSemCtx }, config);
      }
    } else {
      c = new ATNConfig({ state: pt.target }, config);
    }
    if (_ParserATNSimulator.debug) {
      console.log("config from pred transition=" + c);
    }
    return c;
  }
  predTransition(config, pt, collectPredicates, inContext, fullCtx) {
    if (_ParserATNSimulator.debug) {
      console.log("PRED (collectPredicates=" + collectPredicates + ") " + pt.ruleIndex + ":" + pt.predIndex + ", ctx dependent=" + pt.isCtxDependent);
      if (this.parser !== null) {
        console.log("context surrounding pred is " + arrayToString(this.parser.getRuleInvocationStack()));
      }
    }
    let c = null;
    if (collectPredicates && (pt.isCtxDependent && inContext || !pt.isCtxDependent)) {
      if (fullCtx && this._input) {
        const currentPosition = this._input.index;
        this._input.seek(this._startIndex);
        const predSucceeds = pt.getPredicate().evaluate(this.parser, this._outerContext);
        this._input.seek(currentPosition);
        if (predSucceeds) {
          c = new ATNConfig({ state: pt.target }, config);
        }
      } else {
        const newSemCtx = SemanticContext.andContext(config.semanticContext, pt.getPredicate());
        c = new ATNConfig({ state: pt.target, semanticContext: newSemCtx }, config);
      }
    } else {
      c = new ATNConfig({ state: pt.target }, config);
    }
    if (_ParserATNSimulator.debug) {
      console.log("config from pred transition=" + c);
    }
    return c;
  }
  ruleTransition(config, t) {
    if (_ParserATNSimulator.debug) {
      console.log("CALL rule " + this.getRuleName(t.target.ruleIndex) + ", ctx=" + config.context);
    }
    const returnState = t.followState;
    const newContext = SingletonPredictionContext.create(config.context, returnState.stateNumber);
    return new ATNConfig({ state: t.target, context: newContext }, config);
  }
  getConflictingAlts(configs) {
    const altSets = PredictionMode.getConflictingAltSubsets(configs);
    return PredictionMode.getAlts(altSets);
  }
  /**
   * Sam pointed out a problem with the previous definition, v3, of
   * ambiguous states. If we have another state associated with conflicting
   * alternatives, we should keep going. For example, the following grammar
   *
   * s : (ID | ID ID?) ';' ;
   *
   * When the ATN simulation reaches the state before ';', it has a DFA
   * state that looks like: [12|1|[], 6|2|[], 12|2|[]]. Naturally
   * 12|1|[] and 12|2|[] conflict, but we cannot stop processing this node
   * because alternative to has another way to continue, via [6|2|[]].
   * The key is that we have a single state that has config's only associated
   * with a single alternative, 2, and crucially the state transitions
   * among the configurations are all non-epsilon transitions. That means
   * we don't consider any conflicts that include alternative 2. So, we
   * ignore the conflict between alts 1 and 2. We ignore a set of
   * conflicting alts when there is an intersection with an alternative
   * associated with a single alt state in the state -> config-list map.
   *
   * It's also the case that we might have two conflicting configurations but
   * also a 3rd nonconflicting configuration for a different alternative:
   * [1|1|[], 1|2|[], 8|3|[]]. This can come about from grammar:
   *
   * a : A | A | A B ;
   *
   * After matching input A, we reach the stop state for rule A, state 1.
   * State 8 is the state right before B. Clearly alternatives 1 and 2
   * conflict and no amount of further lookahead will separate the two.
   * However, alternative 3 will be able to continue and so we do not
   * stop working on this state. In the previous example, we're concerned
   * with states associated with the conflicting alternatives. Here alt
   * 3 is not associated with the conflicting configs, but since we can continue
   * looking for input reasonably, I don't declare the state done. We
   * ignore a set of conflicting alts when we have an alternative
   * that we still need to pursue
   */
  getConflictingAltsOrUniqueAlt(configs) {
    let conflictingAlts;
    if (configs.uniqueAlt !== ATN.INVALID_ALT_NUMBER) {
      conflictingAlts = new BitSet();
      conflictingAlts.set(configs.uniqueAlt);
    } else {
      conflictingAlts = configs.conflictingAlts;
    }
    return conflictingAlts;
  }
  noViableAlt(input, outerContext, configs, startIndex) {
    return new NoViableAltException(this.parser, input, input.get(startIndex), input.LT(1), configs, outerContext);
  }
  /**
   * Add an edge to the DFA, if possible. This method calls
   * {@link addDFAState} to ensure the {@code to} state is present in the
   * DFA. If {@code from} is {@code null}, or if {@code t} is outside the
   * range of edges that can be represented in the DFA tables, this method
   * returns without adding the edge to the DFA.
   *
   * <p>If {@code to} is {@code null}, this method returns {@code null}.
   * Otherwise, this method returns the {@link DFAState} returned by calling
   * {@link addDFAState} for the {@code to} state.</p>
   *
   * @param dfa The DFA
   * @param from_ The source state for the edge
   * @param t The input symbol
   * @param to The target state for the edge
   *
   * @returns If {@code to} is {@code null}, this method returns {@code null};
   * otherwise this method returns the result of calling {@link addDFAState}
   * on {@code to}
   */
  addDFAEdge(dfa, from_, t, to) {
    if (_ParserATNSimulator.debug) {
      console.log("EDGE " + from_ + " -> " + to + " upon " + this.getTokenName(t));
    }
    if (to === null) {
      return null;
    }
    to = this.addDFAState(dfa, to);
    if (from_ === null || t < -1 || t > this.atn.maxTokenType) {
      return to;
    }
    if (from_.edges === null) {
      from_.edges = new Array(this.atn.maxTokenType + 2);
      from_.edges.fill(null);
    }
    from_.edges[t + 1] = to;
    if (_ParserATNSimulator.debug) {
      console.log("DFA=\n" + dfa.toString(this.parser != null ? this.parser.vocabulary : Vocabulary.EMPTY_VOCABULARY));
    }
    return to;
  }
  /**
   * Add state {@code D} to the DFA if it is not already present, and return
   * the actual instance stored in the DFA. If a state equivalent to {@code D}
   * is already in the DFA, the existing state is returned. Otherwise this
   * method returns {@code D} after adding it to the DFA.
   *
   * <p>If {@code D} is {@link ERROR}, this method returns {@link ERROR} and
   * does not change the DFA.</p>
   *
   * @param dfa The dfa
   * @param D The DFA state to add
   * @returns The state stored in the DFA. This will be either the existing
   * state if {@code D} is already in the DFA, or {@code D} itself if the
   * state was not already present
   */
  addDFAState(dfa, D) {
    if (D === ATNSimulator.ERROR) {
      return D;
    }
    const existing = dfa.states.get(D);
    if (existing !== null) {
      if (_ParserATNSimulator.trace_atn_sim) {
        console.log("addDFAState " + D + " exists");
      }
      return existing;
    }
    D.stateNumber = dfa.states.length;
    if (!D.configs.readOnly) {
      D.configs.optimizeConfigs(this);
      D.configs.setReadonly(true);
    }
    if (_ParserATNSimulator.trace_atn_sim) {
      console.log("addDFAState new " + D);
    }
    dfa.states.add(D);
    if (_ParserATNSimulator.debug) {
      console.log("adding new DFA state: " + D);
    }
    return D;
  }
  reportAttemptingFullContext(dfa, conflictingAlts, configs, startIndex, stopIndex) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.retry_debug) {
      const interval = new Interval(startIndex, stopIndex + 1);
      console.log("reportAttemptingFullContext decision=" + dfa.decision + ":" + configs + ", input=" + this.parser.tokenStream.getText(interval));
    }
    if (this.parser !== null) {
      this.parser.getErrorListenerDispatch().reportAttemptingFullContext(
        this.parser,
        dfa,
        startIndex,
        stopIndex,
        conflictingAlts,
        configs
      );
    }
  }
  reportContextSensitivity(dfa, prediction, configs, startIndex, stopIndex) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.retry_debug) {
      const interval = new Interval(startIndex, stopIndex + 1);
      console.log("reportContextSensitivity decision=" + dfa.decision + ":" + configs + ", input=" + this.parser.tokenStream.getText(interval));
    }
    if (this.parser !== null) {
      this.parser.getErrorListenerDispatch().reportContextSensitivity(
        this.parser,
        dfa,
        startIndex,
        stopIndex,
        prediction,
        configs
      );
    }
  }
  // If context sensitive parsing, we know it's ambiguity not conflict//
  reportAmbiguity(dfa, D, startIndex, stopIndex, exact, ambigAlts, configs) {
    if (_ParserATNSimulator.debug || _ParserATNSimulator.retry_debug) {
      const interval = new Interval(startIndex, stopIndex + 1);
      console.log("reportAmbiguity " + ambigAlts + ":" + configs + ", input=" + this.parser.tokenStream.getText(interval));
    }
    if (this.parser !== null) {
      this.parser.getErrorListenerDispatch().reportAmbiguity(
        this.parser,
        dfa,
        startIndex,
        stopIndex,
        exact,
        ambigAlts,
        configs
      );
    }
  }
};

// src/atn/PredictionContextCache.ts
var PredictionContextCache = class {
  static {
    __name(this, "PredictionContextCache");
  }
  cache = new HashMap();
  /**
   * Add a context to the cache and return it. If the context already exists,
   * return that one instead and do not add a new context to the cache.
   * Protect shared cache from unsafe thread access.
   *
   * @param ctx tbd
   * @returns tbd
   */
  add(ctx) {
    if (ctx === PredictionContext.EMPTY) {
      return PredictionContext.EMPTY;
    }
    const existing = this.cache.get(ctx) || null;
    if (existing !== null) {
      return existing;
    }
    this.cache.set(ctx, ctx);
    return ctx;
  }
  get(ctx) {
    return this.cache.get(ctx) || null;
  }
  get length() {
    return this.cache.length;
  }
};

// src/atn/DecisionEventInfo.ts
var DecisionEventInfo = class {
  static {
    __name(this, "DecisionEventInfo");
  }
  /**
   * The invoked decision number which this event is related to.
   *
   * @see ATN#decisionToState
   */
  decision;
  /**
   * The configuration set containing additional information relevant to the
   * prediction state when the current event occurred, or {@code null} if no
   * additional information is relevant or available.
   */
  configs;
  /**
   * The input token stream which is being parsed.
   */
  input;
  /**
   * The token index in the input stream at which the current prediction was
   * originally invoked.
   */
  startIndex;
  /**
   * The token index in the input stream at which the current event occurred.
   */
  stopIndex;
  /**
   * {@code true} if the current event occurred during LL prediction;
   * otherwise, {@code false} if the input occurred during SLL prediction.
   */
  fullCtx;
  constructor(decision, configs, input, startIndex, stopIndex, fullCtx) {
    this.decision = decision;
    this.configs = configs;
    this.input = input;
    this.startIndex = startIndex;
    this.stopIndex = stopIndex;
    this.fullCtx = fullCtx;
  }
};

// src/atn/AmbiguityInfo.ts
var AmbiguityInfo = class extends DecisionEventInfo {
  static {
    __name(this, "AmbiguityInfo");
  }
  /** The set of alternative numbers for this decision event that lead to a valid parse. */
  ambigAlts;
  /**
   * Constructs a new instance of the {@link AmbiguityInfo} class with the
   * specified detailed ambiguity information.
   *
   * @param decision The decision number
   * @param configs The final configuration set identifying the ambiguous
   * alternatives for the current input
   * @param ambigAlts The set of alternatives in the decision that lead to a valid parse.
   *                  The predicted alt is the min(ambigAlts)
   * @param input The input token stream
   * @param startIndex The start index for the current prediction
   * @param stopIndex The index at which the ambiguity was identified during
   * prediction
   * @param fullCtx {@code true} if the ambiguity was identified during LL
   * prediction; otherwise, {@code false} if the ambiguity was identified
   * during SLL prediction
   */
  constructor(decision, configs, ambigAlts, input, startIndex, stopIndex, fullCtx) {
    super(decision, configs, input, startIndex, stopIndex, fullCtx);
    this.ambigAlts = ambigAlts;
  }
};

// src/atn/ContextSensitivityInfo.ts
var ContextSensitivityInfo = class extends DecisionEventInfo {
  static {
    __name(this, "ContextSensitivityInfo");
  }
  /**
   * Constructs a new instance of the {@link ContextSensitivityInfo} class
   * with the specified detailed context sensitivity information.
   *
   * @param decision The decision number
   * @param configs The final configuration set containing the unique
   * alternative identified by full-context prediction
   * @param input The input token stream
   * @param startIndex The start index for the current prediction
   * @param stopIndex The index at which the context sensitivity was
   * identified during full-context prediction
   */
  constructor(decision, configs, input, startIndex, stopIndex) {
    super(decision, configs, input, startIndex, stopIndex, true);
  }
};

// src/atn/DecisionInfo.ts
var DecisionInfo = class {
  static {
    __name(this, "DecisionInfo");
  }
  /**
   * The decision number, which is an index into {@link ATN#decisionToState}.
   */
  decision = 0;
  /**
   * The total number of times {@link ParserATNSimulator#adaptivePredict} was
   * invoked for this decision.
   */
  invocations = 0;
  /**
   * The total time spent in {@link ParserATNSimulator#adaptivePredict} for
   * this decision, in nanoseconds.
   *
   * The value of this field contains the sum of differential results obtained
   * by {@link process.hrtime()}, and is not adjusted to compensate for JIT
   * and/or garbage collection overhead. For best accuracy, use a modern Node.js
   * version that provides precise results from {@link process.hrtime()}, and
   * perform profiling in a separate process which is warmed up by parsing the
   * input prior to profiling.
   */
  timeInPrediction = 0;
  /**
   * The sum of the lookahead required for SLL prediction for this decision.
   * Note that SLL prediction is used before LL prediction for performance
   * reasons even when {@link PredictionMode#LL} or
   * {@link PredictionMode#LL_EXACT_AMBIG_DETECTION} is used.
   */
  SLL_TotalLook = 0;
  /**
   * Gets the minimum lookahead required for any single SLL prediction to
   * complete for this decision, by reaching a unique prediction, reaching an
   * SLL conflict state, or encountering a syntax error.
   */
  SLL_MinLook = 0;
  /**
   * Gets the maximum lookahead required for any single SLL prediction to
   * complete for this decision, by reaching a unique prediction, reaching an
   * SLL conflict state, or encountering a syntax error.
   */
  SLL_MaxLook = 0;
  /**
   * Gets the {@link LookaheadEventInfo} associated with the event where the
   * {@link SLL_MaxLook} value was set.
   */
  SLL_MaxLookEvent;
  /**
   * The sum of the lookahead required for LL prediction for this decision.
   * Note that LL prediction is only used when SLL prediction reaches a
   * conflict state.
   */
  LL_TotalLook = 0;
  /**
   * Gets the minimum lookahead required for any single LL prediction to
   * complete for this decision. An LL prediction completes when the algorithm
   * reaches a unique prediction, a conflict state (for
   * {@link PredictionMode#LL}, an ambiguity state (for
   * {@link PredictionMode#LL_EXACT_AMBIG_DETECTION}, or a syntax error.
   */
  LL_MinLook = 0;
  /**
   * Gets the maximum lookahead required for any single LL prediction to
   * complete for this decision. An LL prediction completes when the algorithm
   * reaches a unique prediction, a conflict state (for
   * {@link PredictionMode#LL}, an ambiguity state (for
   * {@link PredictionMode#LL_EXACT_AMBIG_DETECTION}, or a syntax error.
   */
  LL_MaxLook = 0;
  /**
   * Gets the {@link LookaheadEventInfo} associated with the event where the
   * {@link LL_MaxLook} value was set.
   */
  LL_MaxLookEvent;
  /**
   * A collection of {@link ContextSensitivityInfo} instances describing the
   * context sensitivities encountered during LL prediction for this decision.
   */
  contextSensitivities;
  /**
   * A collection of {@link ErrorInfo} instances describing the parse errors
   * identified during calls to {@link ParserATNSimulator#adaptivePredict} for
   * this decision.
   */
  errors;
  /**
   * A collection of {@link AmbiguityInfo} instances describing the
   * ambiguities encountered during LL prediction for this decision.
   */
  ambiguities;
  /**
   * A collection of {@link PredicateEvalInfo} instances describing the
   * results of evaluating individual predicates during prediction for this
   * decision.
   */
  predicateEvals;
  /**
   * The total number of ATN transitions required during SLL prediction for
   * this decision. An ATN transition is determined by the number of times the
   * DFA does not contain an edge that is required for prediction, resulting
   * in on-the-fly computation of that edge.
  /**
   * If DFA caching of SLL transitions is employed by the implementation, ATN
   * computation may cache the computed edge for efficient lookup during
   * future parsing of this decision. Otherwise, the SLL parsing algorithm
   * will use ATN transitions exclusively.
   *
   * @see #SLL_DFATransitions
   * @see ParserATNSimulator#computeTargetState
   * @see LexerATNSimulator#computeTargetState
   */
  SLL_ATNTransitions = 0;
  /**
   * The total number of DFA transitions required during SLL prediction for
   * this decision.
   *
   * If the ATN simulator implementation does not use DFA caching for SLL
   * transitions, this value will be 0.
   *
   * @see ParserATNSimulator#getExistingTargetState
   * @see LexerATNSimulator#getExistingTargetState
   */
  SLL_DFATransitions = 0;
  /**
   * Gets the total number of times SLL prediction completed in a conflict
   * state, resulting in fallback to LL prediction.
   *
   * Note that this value is not related to whether or not
   * {@link PredictionMode#SLL} may be used successfully with a particular
   * grammar. If the ambiguity resolution algorithm applied to the SLL
   * conflicts for this decision produce the same result as LL prediction for
   * this decision, {@link PredictionMode#SLL} would produce the same overall
   * parsing result as {@link PredictionMode#LL}.
   */
  LL_Fallback = 0;
  /**
   * The total number of ATN transitions required during LL prediction for
   * this decision. An ATN transition is determined by the number of times the
   * DFA does not contain an edge that is required for prediction, resulting
   * in on-the-fly computation of that edge.
   *
   * If DFA caching of LL transitions is employed by the implementation, ATN
   * computation may cache the computed edge for efficient lookup during
   * future parsing of this decision. Otherwise, the LL parsing algorithm will
   * use ATN transitions exclusively.
   *
   * @see #LL_DFATransitions
   * @see ParserATNSimulator#computeTargetState
   * @see LexerATNSimulator#computeTargetState
   */
  LL_ATNTransitions = 0;
  /**
   * The total number of DFA transitions required during LL prediction for
   * this decision.
   *
   * If the ATN simulator implementation does not use DFA caching for LL
   * transitions, this value will be 0.
   *
   * @see ParserATNSimulator#getExistingTargetState
   * @see LexerATNSimulator#getExistingTargetState
   */
  LL_DFATransitions = 0;
  /**
   * Constructs a new instance of the {@link DecisionInfo} class to contain
   * statistics for a particular decision.
   *
   * @param decision The decision number
   */
  constructor(decision) {
    this.decision = decision;
    this.contextSensitivities = [];
    this.errors = [];
    this.ambiguities = [];
    this.predicateEvals = [];
  }
  toString() {
    return "{decision=" + this.decision + ", contextSensitivities=" + this.contextSensitivities.length + ", errors=" + this.errors.length + ", ambiguities=" + this.ambiguities.length + ", SLL_lookahead=" + this.SLL_TotalLook + ", SLL_ATNTransitions=" + this.SLL_ATNTransitions + ", SLL_DFATransitions=" + this.SLL_DFATransitions + ", LL_Fallback=" + this.LL_Fallback + ", LL_lookahead=" + this.LL_TotalLook + ", LL_ATNTransitions=" + this.LL_ATNTransitions + "}";
  }
};

// src/atn/ErrorInfo.ts
var ErrorInfo = class extends DecisionEventInfo {
  static {
    __name(this, "ErrorInfo");
  }
  /**
   * Constructs a new instance of the {@link ErrorInfo} class with the
   * specified detailed syntax error information.
   *
   * @param decision The decision number
   * @param configs The final configuration set reached during prediction
   * prior to reaching the {@link ATNSimulator#ERROR} state
   * @param input The input token stream
   * @param startIndex The start index for the current prediction
   * @param stopIndex The index at which the syntax error was identified
   * @param fullCtx {@code true} if the syntax error was identified during LL
   * prediction; otherwise, {@code false} if the syntax error was identified
   * during SLL prediction
   */
  constructor(decision, configs, input, startIndex, stopIndex, fullCtx) {
    super(decision, configs, input, startIndex, stopIndex, fullCtx);
  }
};

// src/atn/LookaheadEventInfo.ts
var LookaheadEventInfo = class extends DecisionEventInfo {
  static {
    __name(this, "LookaheadEventInfo");
  }
  /**
   * The alternative chosen by adaptivePredict(), not necessarily
   * the outermost alt shown for a rule; left-recursive rules have
   * user-level alts that differ from the rewritten rule with a (...) block
   * and a (..)* loop.
   */
  predictedAlt;
  /**
   * Constructs a new instance of the {@link LookaheadEventInfo} class with
   * the specified detailed lookahead information.
   *
   * @param decision The decision number
   * @param configs The final configuration set containing the necessary
   * information to determine the result of a prediction, or {@code null} if
   * the final configuration set is not available
   * @param predictedAlt The predicted alternative
   * @param input The input token stream
   * @param startIndex The start index for the current prediction
   * @param stopIndex The index at which the prediction was finally made
   * @param fullCtx {@code true} if the current lookahead is part of an LL
   * prediction; otherwise, {@code false} if the current lookahead is part of
   * an SLL prediction
   */
  constructor(decision, configs, predictedAlt, input, startIndex, stopIndex, fullCtx) {
    super(decision, configs, input, startIndex, stopIndex, fullCtx);
    this.predictedAlt = predictedAlt;
  }
};

// src/atn/ProfilingATNSimulator.ts
var ProfilingATNSimulator = class extends ParserATNSimulator {
  static {
    __name(this, "ProfilingATNSimulator");
  }
  decisions;
  numDecisions = 0;
  currentDecision = 0;
  currentState;
  /**
   * At the point of LL failover, we record how SLL would resolve the conflict so that
   *  we can determine whether or not a decision / input pair is context-sensitive.
   *  If LL gives a different result than SLL's predicted alternative, we have a
   *  context sensitivity for sure. The converse is not necessarily true, however.
   *  It's possible that after conflict resolution chooses minimum alternatives,
   *  SLL could get the same answer as LL. Regardless of whether or not the result indicates
   *  an ambiguity, it is not treated as a context sensitivity because LL prediction
   *  was not required in order to produce a correct prediction for this decision and input sequence.
   *  It may in fact still be a context sensitivity but we don't know by looking at the
   *  minimum alternatives for the current input.
   */
  conflictingAltResolvedBySLL;
  #sllStopIndex = 0;
  #llStopIndex = 0;
  constructor(parser) {
    const sharedContextCache = parser.interpreter.getSharedContextCache();
    super(parser, parser.interpreter.atn, parser.interpreter.decisionToDFA, sharedContextCache);
    if (sharedContextCache) {
      this.numDecisions = this.atn.decisionToState.length;
      this.decisions = new Array(this.numDecisions);
      for (let i = 0; i < this.numDecisions; i++) {
        this.decisions[i] = new DecisionInfo(i);
      }
    }
  }
  adaptivePredict(input, decision, outerContext) {
    try {
      this.#sllStopIndex = -1;
      this.#llStopIndex = -1;
      this.currentDecision = decision;
      const start = performance.now();
      const alt = super.adaptivePredict(input, decision, outerContext);
      const stop = performance.now();
      this.decisions[decision].timeInPrediction += stop - start;
      this.decisions[decision].invocations++;
      const sllLook = this.#sllStopIndex - this._startIndex + 1;
      this.decisions[decision].SLL_TotalLook += sllLook;
      this.decisions[decision].SLL_MinLook = this.decisions[decision].SLL_MinLook === 0 ? sllLook : Math.min(this.decisions[decision].SLL_MinLook, sllLook);
      if (sllLook > this.decisions[decision].SLL_MaxLook) {
        this.decisions[decision].SLL_MaxLook = sllLook;
        this.decisions[decision].SLL_MaxLookEvent = new LookaheadEventInfo(
          decision,
          null,
          alt,
          input,
          this._startIndex,
          this.#sllStopIndex,
          false
        );
      }
      if (this.#llStopIndex >= 0) {
        const llLook = this.#llStopIndex - this._startIndex + 1;
        this.decisions[decision].LL_TotalLook += llLook;
        this.decisions[decision].LL_MinLook = this.decisions[decision].LL_MinLook === 0 ? llLook : Math.min(this.decisions[decision].LL_MinLook, llLook);
        if (llLook > this.decisions[decision].LL_MaxLook) {
          this.decisions[decision].LL_MaxLook = llLook;
          this.decisions[decision].LL_MaxLookEvent = new LookaheadEventInfo(
            decision,
            null,
            alt,
            input,
            this._startIndex,
            this.#llStopIndex,
            true
          );
        }
      }
      return alt;
    } finally {
      this.currentDecision = -1;
    }
  }
  getExistingTargetState(previousD, t) {
    if (this._input) {
      this.#sllStopIndex = this._input.index;
      const existingTargetState = super.getExistingTargetState(previousD, t);
      if (existingTargetState !== null) {
        this.decisions[this.currentDecision].SLL_DFATransitions++;
        if (existingTargetState === ATNSimulator.ERROR) {
          this.decisions[this.currentDecision].errors.push(
            new ErrorInfo(
              this.currentDecision,
              previousD.configs,
              this._input,
              this._startIndex,
              this.#sllStopIndex,
              false
            )
          );
        }
      }
      this.currentState = existingTargetState;
      return existingTargetState;
    }
    return null;
  }
  computeTargetState(dfa, previousD, t) {
    const state = super.computeTargetState(dfa, previousD, t);
    this.currentState = state;
    return state;
  }
  computeReachSet(closure, t, fullCtx) {
    if (fullCtx && this._input) {
      this.#llStopIndex = this._input.index;
    }
    const reachConfigs = super.computeReachSet(closure, t, fullCtx);
    if (this._input) {
      if (fullCtx) {
        this.decisions[this.currentDecision].LL_ATNTransitions++;
        if (reachConfigs === null) {
          this.decisions[this.currentDecision].errors.push(
            new ErrorInfo(
              this.currentDecision,
              closure,
              this._input,
              this._startIndex,
              this.#llStopIndex,
              true
            )
          );
        }
      } else {
        this.decisions[this.currentDecision].SLL_ATNTransitions++;
        if (reachConfigs === null) {
          this.decisions[this.currentDecision].errors.push(
            new ErrorInfo(
              this.currentDecision,
              closure,
              this._input,
              this._startIndex,
              this.#sllStopIndex,
              false
            )
          );
        }
      }
    }
    return reachConfigs;
  }
  reportAttemptingFullContext(dfa, conflictingAlts, configs, startIndex, stopIndex) {
    if (conflictingAlts !== null) {
      this.conflictingAltResolvedBySLL = conflictingAlts.nextSetBit(0);
    } else {
      this.conflictingAltResolvedBySLL = configs.getAlts().nextSetBit(0);
    }
    this.decisions[this.currentDecision].LL_Fallback++;
    if (conflictingAlts) {
      super.reportAttemptingFullContext(dfa, conflictingAlts, configs, startIndex, stopIndex);
    }
  }
  reportContextSensitivity(dfa, prediction, configs, startIndex, stopIndex) {
    if (prediction !== this.conflictingAltResolvedBySLL && this._input) {
      this.decisions[this.currentDecision].contextSensitivities.push(
        new ContextSensitivityInfo(this.currentDecision, configs, this._input, startIndex, stopIndex)
      );
    }
    super.reportContextSensitivity(dfa, prediction, configs, startIndex, stopIndex);
  }
  reportAmbiguity(dfa, state, startIndex, stopIndex, exact, ambigAlts, configs) {
    let prediction;
    if (ambigAlts !== null) {
      prediction = ambigAlts.nextSetBit(0);
    } else {
      prediction = configs.getAlts().nextSetBit(0);
    }
    if (this._input) {
      if (configs.fullCtx && prediction !== this.conflictingAltResolvedBySLL) {
        this.decisions[this.currentDecision].contextSensitivities.push(
          new ContextSensitivityInfo(this.currentDecision, configs, this._input, startIndex, stopIndex)
        );
      }
      this.decisions[this.currentDecision].ambiguities.push(
        new AmbiguityInfo(
          this.currentDecision,
          configs,
          ambigAlts,
          this._input,
          startIndex,
          stopIndex,
          configs.fullCtx
        )
      );
    }
    super.reportAmbiguity(dfa, state, startIndex, stopIndex, exact, ambigAlts, configs);
  }
  getDecisionInfo() {
    return this.decisions;
  }
  getCurrentState() {
    return this.currentState;
  }
};

// src/atn/ParseInfo.ts
var ParseInfo = class {
  static {
    __name(this, "ParseInfo");
  }
  atnSimulator;
  constructor(atnSimulator) {
    this.atnSimulator = atnSimulator;
  }
  getDecisionInfo() {
    return this.atnSimulator.getDecisionInfo();
  }
  getLLDecisions() {
    const decisions = this.atnSimulator.getDecisionInfo();
    const LL = [];
    for (let i = 0; i < decisions.length; i++) {
      const fallBack = decisions[i].LL_Fallback;
      if (fallBack > 0)
        LL.push(i);
    }
    return LL;
  }
  getTotalTimeInPrediction() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let t = 0;
    for (let i = 0; i < decisions.length; i++) {
      t += decisions[i].timeInPrediction;
    }
    return t;
  }
  getTotalSLLLookaheadOps() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let k = 0;
    for (let i = 0; i < decisions.length; i++) {
      k += decisions[i].SLL_TotalLook;
    }
    return k;
  }
  getTotalLLLookaheadOps() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let k = 0;
    for (let i = 0; i < decisions.length; i++) {
      k += decisions[i].LL_TotalLook;
    }
    return k;
  }
  getTotalSLLATNLookaheadOps() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let k = 0;
    for (let i = 0; i < decisions.length; i++) {
      k += decisions[i].SLL_ATNTransitions;
    }
    return k;
  }
  getTotalLLATNLookaheadOps() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let k = 0;
    for (let i = 0; i < decisions.length; i++) {
      k += decisions[i].LL_ATNTransitions;
    }
    return k;
  }
  getTotalATNLookaheadOps() {
    const decisions = this.atnSimulator.getDecisionInfo();
    let k = 0;
    for (let i = 0; i < decisions.length; i++) {
      k += decisions[i].SLL_ATNTransitions;
      k += decisions[i].LL_ATNTransitions;
    }
    return k;
  }
  // getDFASize(): number {
  //   let n = 0;
  //   const decisionToDFA: DFA[] = this.atnSimulator.decisionToDFA;
  //   for (let i = 0; i < decisionToDFA.length; i++) {
  //     n += this.getDFASize(i);
  //   }
  //   return n;
  // }
  getDFASize(decision) {
    const decisionToDFA = this.atnSimulator.decisionToDFA[decision];
    return decisionToDFA.states.length;
  }
};

// src/atn/PredicateEvalInfo.ts
var PredicateEvalInfo = class extends DecisionEventInfo {
  static {
    __name(this, "PredicateEvalInfo");
  }
  /**
   * The semantic context which was evaluated.
   */
  semctx;
  /**
   * The alternative number for the decision which is guarded by the semantic
   * context {@link #semctx}. Note that other ATN
   * configurations may predict the same alternative which are guarded by
   * other semantic contexts and/or {@link SemanticContext#NONE}.
   */
  predictedAlt;
  /**
   * The result of evaluating the semantic context {@link #semctx}.
   */
  evalResult;
  /**
   * Constructs a new instance of the {@link PredicateEvalInfo} class with the
   * specified detailed predicate evaluation information.
   *
   * @param decision The decision number
   * @param input The input token stream
   * @param startIndex The start index for the current prediction
   * @param stopIndex The index at which the predicate evaluation was
   * triggered. Note that the input stream may be reset to other positions for
   * the actual evaluation of individual predicates.
   * @param semctx The semantic context which was evaluated
   * @param evalResult The results of evaluating the semantic context
   * @param predictedAlt The alternative number for the decision which is
   * guarded by the semantic context {@code semctx}. See {@link #predictedAlt}
   * for more information.
   * @param fullCtx {@code true} if the semantic context was
   * evaluated during LL prediction; otherwise, {@code false} if the semantic
   * context was evaluated during SLL prediction
   *
   * @see ParserATNSimulator#evalSemanticContext(SemanticContext, ParserRuleContext, int, boolean)
   * @see SemanticContext#eval(Recognizer, RuleContext)
   */
  constructor(decision, input, startIndex, stopIndex, semctx, evalResult, predictedAlt, fullCtx) {
    super(decision, new ATNConfigSet(), input, startIndex, stopIndex, fullCtx);
    this.semctx = semctx;
    this.evalResult = evalResult;
    this.predictedAlt = predictedAlt;
  }
};

// src/misc/ParseCancellationException.ts
var ParseCancellationException = class _ParseCancellationException extends Error {
  static {
    __name(this, "ParseCancellationException");
  }
  constructor(_e) {
    super();
    Error.captureStackTrace(this, _ParseCancellationException);
  }
};

// src/misc/InterpreterDataReader.ts
var InterpreterDataReader = class {
  static {
    __name(this, "InterpreterDataReader");
  }
  /**
   * The structure of the data file is very simple. Everything is line based with empty lines
   * separating the different parts. For lexers the layout is:
   * token literal names:
   * ...
   *
   * token symbolic names:
   * ...
   *
   * rule names:
   * ...
   *
   * channel names:
   * ...
   *
   * mode names:
   * ...
   *
   * atn:
   * a single line with comma separated int values, enclosed in a pair of squared brackets.
   *
   * Data for a parser does not contain channel and mode names.
   */
  static parseInterpreterData(source) {
    const ruleNames = [];
    const channels = [];
    const modes = [];
    const literalNames = [];
    const symbolicNames = [];
    const lines = source.split("\n");
    let index = 0;
    let line = lines[index++];
    if (line !== "token literal names:") {
      throw new Error("Unexpected data entry");
    }
    do {
      line = lines[index++];
      if (line.length === 0) {
        break;
      }
      literalNames.push(line === "null" ? null : line);
    } while (true);
    line = lines[index++];
    if (line !== "token symbolic names:") {
      throw new Error("Unexpected data entry");
    }
    do {
      line = lines[index++];
      if (line.length === 0) {
        break;
      }
      symbolicNames.push(line === "null" ? null : line);
    } while (true);
    line = lines[index++];
    if (line !== "rule names:") {
      throw new Error("Unexpected data entry");
    }
    do {
      line = lines[index++];
      if (line.length === 0) {
        break;
      }
      ruleNames.push(line);
    } while (true);
    line = lines[index++];
    if (line === "channel names:") {
      do {
        line = lines[index++];
        if (line.length === 0) {
          break;
        }
        channels.push(line);
      } while (true);
      line = lines[index++];
      if (line !== "mode names:") {
        throw new Error("Unexpected data entry");
      }
      do {
        line = lines[index++];
        if (line.length === 0) {
          break;
        }
        modes.push(line);
      } while (true);
    }
    line = lines[index++];
    if (line !== "atn:") {
      throw new Error("Unexpected data entry");
    }
    line = lines[index++];
    const elements = line.split(",");
    let value;
    const serializedATN = [];
    for (let i = 0; i < elements.length; ++i) {
      const element = elements[i];
      if (element.startsWith("[")) {
        value = Number(element.substring(1).trim());
      } else if (element.endsWith("]")) {
        value = Number(element.substring(0, element.length - 1).trim());
      } else {
        value = Number(element.trim());
      }
      serializedATN[i] = value;
    }
    const deserializer = new ATNDeserializer();
    return {
      atn: deserializer.deserialize(serializedATN),
      vocabulary: new Vocabulary(literalNames, symbolicNames, []),
      ruleNames,
      channels: channels.length > 0 ? channels : void 0,
      modes: modes.length > 0 ? modes : void 0
    };
  }
};

// src/tree/AbstractParseTreeVisitor.ts
var AbstractParseTreeVisitor = class {
  static {
    __name(this, "AbstractParseTreeVisitor");
  }
  visit(tree) {
    return tree.accept(this);
  }
  visitChildren(node) {
    let result = this.defaultResult();
    const n2 = node.getChildCount();
    for (let i = 0; i < n2; i++) {
      if (!this.shouldVisitNextChild(node, result)) {
        break;
      }
      const c = node.getChild(i);
      if (c) {
        const childResult = c.accept(this);
        result = this.aggregateResult(result, childResult);
      }
    }
    return result;
  }
  visitTerminal(_node) {
    return this.defaultResult();
  }
  visitErrorNode(_node) {
    return this.defaultResult();
  }
  defaultResult() {
    return null;
  }
  shouldVisitNextChild(_node, _currentResult) {
    return true;
  }
  aggregateResult(aggregate, nextResult) {
    return nextResult;
  }
};

// src/tree/ParseTreeWalker.ts
var ParseTreeWalker = class _ParseTreeWalker {
  static {
    __name(this, "ParseTreeWalker");
  }
  // eslint-disable-next-line @typescript-eslint/naming-convention
  static DEFAULT = new _ParseTreeWalker();
  /**
   * Performs a walk on the given parse tree starting at the root and going down recursively
   * with depth-first search. On each node, {@link ParseTreeWalker//enterRule} is called before
   * recursively walking down into child nodes, then
   * {@link ParseTreeWalker//exitRule} is called after the recursive call to wind up.
   *
   * @param listener The listener used by the walker to process grammar rules
   * @param t The parse tree to be walked on
   */
  walk(listener, t) {
    const errorNode = t instanceof ErrorNode;
    if (errorNode) {
      listener.visitErrorNode(t);
    } else if (t instanceof TerminalNode) {
      listener.visitTerminal(t);
    } else {
      const r = t;
      this.enterRule(listener, r);
      for (let i = 0; i < t.getChildCount(); i++) {
        this.walk(listener, t.getChild(i));
      }
      this.exitRule(listener, r);
    }
  }
  /**
   * Enters a grammar rule by first triggering the generic event {@link ParseTreeListener.enterEveryRule}
   * then by triggering the event specific to the given parse tree node
   *
   * @param listener The listener responding to the trigger events
   * @param r The grammar rule containing the rule context
   */
  enterRule(listener, r) {
    const ctx = r.ruleContext;
    listener.enterEveryRule(ctx);
    ctx.enterRule(listener);
  }
  /**
   * Exits a grammar rule by first triggering the event specific to the given parse tree node
   * then by triggering the generic event {@link ParseTreeListener//exitEveryRule}
   *
   * @param listener The listener responding to the trigger events
   * @param r The grammar rule containing the rule context
   */
  exitRule(listener, r) {
    const ctx = r.ruleContext;
    ctx.exitRule(listener);
    listener.exitEveryRule(ctx);
  }
};

// src/CharStream.ts
var CharStreamImpl = class {
  static {
    __name(this, "CharStreamImpl");
  }
  name = "";
  index = 0;
  data;
  constructor(input) {
    const codePoints = [];
    for (const char of input) {
      codePoints.push(char.codePointAt(0));
    }
    this.data = new Uint32Array(codePoints);
  }
  /**
   * Reset the stream so that it's in the same state it was
   * when the object was created *except* the data array is not
   * touched.
   */
  reset() {
    this.index = 0;
  }
  consume() {
    if (this.index >= this.data.length) {
      throw new Error("cannot consume EOF");
    }
    this.index += 1;
  }
  LA(offset) {
    if (offset === 0) {
      return 0;
    }
    if (offset < 0) {
      offset += 1;
    }
    const pos = this.index + offset - 1;
    if (pos < 0 || pos >= this.data.length) {
      return Token.EOF;
    }
    return this.data[pos];
  }
  // mark/release do nothing; we have entire buffer
  mark() {
    return -1;
  }
  release(_marker) {
  }
  /**
   * consume() ahead until p==_index; can't just set p=_index as we must
   * update line and column. If we seek backwards, just set p
   */
  seek(index) {
    if (index <= this.index) {
      this.index = index;
      return;
    }
    this.index = Math.min(index, this.data.length);
  }
  getText(intervalOrStart, stop) {
    let begin;
    let end;
    if (intervalOrStart instanceof Interval) {
      begin = intervalOrStart.start;
      end = intervalOrStart.stop;
    } else {
      begin = intervalOrStart;
      end = stop ?? this.data.length - 1;
    }
    if (end >= this.data.length) {
      end = this.data.length - 1;
    }
    if (begin >= this.data.length) {
      return "";
    } else {
      return this.#stringFromRange(begin, end + 1);
    }
  }
  toString() {
    return this.#stringFromRange(0);
  }
  get size() {
    return this.data.length;
  }
  getSourceName() {
    if (this.name) {
      return this.name;
    } else {
      return IntStream.UNKNOWN_SOURCE_NAME;
    }
  }
  #stringFromRange(start, stop) {
    const data = this.data.slice(start, stop);
    let result = "";
    data.forEach((value) => {
      result += String.fromCodePoint(value);
    });
    return result;
  }
};

// src/CharStreams.ts
var CharStreams = class {
  static {
    __name(this, "CharStreams");
  }
  // Creates an CharStream from a string.
  static fromString(str) {
    return new CharStreamImpl(str);
  }
};

// src/BufferedTokenStream.ts
var BufferedTokenStream = class {
  static {
    __name(this, "BufferedTokenStream");
  }
  /**
   * The {@link TokenSource} from which tokens for this stream are fetched.
   */
  tokenSource;
  /**
   * A collection of all tokens fetched from the token source. The list is
   * considered a complete view of the input once {@link fetchedEOF} is set
   * to `true`.
   */
  tokens = [];
  /**
   * The index into {@link tokens} of the current token (next token to
   * {@link consume}). {@link tokens}`[p]` should be
   * {@link LT LT(1)}.
   *
   * <p>This field is set to -1 when the stream is first constructed or when
   * {@link setTokenSource} is called, indicating that the first token has
   * not yet been fetched from the token source. For additional information,
   * see the documentation of {@link IntStream} for a description of
   * Initializing Methods.</p>
   */
  p = -1;
  /**
   * Indicates whether the {@link Token.EOF} token has been fetched from
   * {@link tokenSource} and added to {@link tokens}. This field improves
   * performance for the following cases:
   *
   * <ul>
   * <li>{@link consume}: The lookahead check in {@link consume} to prevent
   * consuming the EOF symbol is optimized by checking the values of
   * {@link fetchedEOF} and {@link p} instead of calling {@link LA}.</li>
   * <li>{@link fetch}: The check to prevent adding multiple EOF symbols into
   * {@link tokens} is trivial with this field.</li>
   * <ul>
   */
  fetchedEOF = false;
  constructor(tokenSource) {
    this.tokenSource = tokenSource;
  }
  mark() {
    return 0;
  }
  release(_marker) {
  }
  reset() {
    this.seek(0);
  }
  seek(index) {
    this.lazyInit();
    this.p = this.adjustSeekIndex(index);
  }
  get size() {
    return this.tokens.length;
  }
  get index() {
    return this.p;
  }
  get(index) {
    this.lazyInit();
    return this.tokens[index];
  }
  consume() {
    let skipEofCheck = false;
    if (this.p >= 0) {
      if (this.fetchedEOF) {
        skipEofCheck = this.p < this.tokens.length - 1;
      } else {
        skipEofCheck = this.p < this.tokens.length;
      }
    } else {
      skipEofCheck = false;
    }
    if (!skipEofCheck && this.LA(1) === Token.EOF) {
      throw new Error("cannot consume EOF");
    }
    if (this.sync(this.p + 1)) {
      this.p = this.adjustSeekIndex(this.p + 1);
    }
  }
  /**
   * Make sure index {@code i} in tokens has a token.
   *
   * @returns {boolean} {@code true} if a token is located at index {@code i}, otherwise
   * {@code false}.
   * @see //get(int i)
   */
  sync(i) {
    const n2 = i - this.tokens.length + 1;
    if (n2 > 0) {
      const fetched = this.fetch(n2);
      return fetched >= n2;
    }
    return true;
  }
  /**
   * Add {@code n} elements to buffer.
   *
   * @returns {number} The actual number of elements added to the buffer.
   */
  fetch(n2) {
    if (this.fetchedEOF) {
      return 0;
    }
    for (let i = 0; i < n2; i++) {
      const t = this.tokenSource.nextToken();
      t.tokenIndex = this.tokens.length;
      this.tokens.push(t);
      if (t.type === Token.EOF) {
        this.fetchedEOF = true;
        return i + 1;
      }
    }
    return n2;
  }
  // Get all tokens from start..stop, inclusively.
  getTokens(start, stop, types) {
    this.lazyInit();
    if (start === void 0 && stop === void 0) {
      return this.tokens;
    }
    start ??= 0;
    if (stop === void 0) {
      stop = this.tokens.length - 1;
    }
    if (start < 0 || stop >= this.tokens.length || stop < 0 || start >= this.tokens.length) {
      throw new RangeError("start " + start + " or stop " + stop + " not in 0.." + (this.tokens.length - 1));
    }
    if (start > stop) {
      return [];
    }
    if (types === void 0) {
      return this.tokens.slice(start, stop + 1);
    }
    const subset = [];
    if (stop >= this.tokens.length) {
      stop = this.tokens.length - 1;
    }
    for (let i = start; i < stop; i++) {
      const t = this.tokens[i];
      if (t.type === Token.EOF) {
        subset.push(t);
        break;
      }
      if (types.has(t.type)) {
        subset.push(t);
      }
    }
    return subset;
  }
  LA(i) {
    return this.LT(i).type;
  }
  LB(k) {
    if (this.p - k < 0) {
      return null;
    }
    return this.tokens[this.p - k];
  }
  LT(k) {
    this.lazyInit();
    if (k === 0) {
      return null;
    }
    if (k < 0) {
      return this.LB(-k);
    }
    const i = this.p + k - 1;
    this.sync(i);
    if (i >= this.tokens.length) {
      return this.tokens[this.tokens.length - 1];
    }
    return this.tokens[i];
  }
  /**
   * Allowed derived classes to modify the behavior of operations which change
   * the current stream position by adjusting the target token index of a seek
   * operation. The default implementation simply returns {@code i}. If an
   * exception is thrown in this method, the current stream index should not be
   * changed.
   *
   * <p>For example, {@link CommonTokenStream} overrides this method to ensure
   * that
   * the seek target is always an on-channel token.</p>
   *
   * @param {number} i The target token index.
   * @returns {number} The adjusted target token index.
   */
  adjustSeekIndex(i) {
    return i;
  }
  lazyInit() {
    if (this.p === -1) {
      this.setup();
    }
  }
  setup() {
    this.sync(0);
    this.p = this.adjustSeekIndex(0);
  }
  // Reset this token stream by setting its token source.///
  setTokenSource(tokenSource) {
    this.tokenSource = tokenSource;
    this.tokens = [];
    this.p = -1;
    this.fetchedEOF = false;
  }
  getTokenSource() {
    return this.tokenSource;
  }
  /**
   * Given a starting index, return the index of the next token on channel.
   * Return i if tokens[i] is on channel. Return -1 if there are no tokens
   * on channel between i and EOF.
   */
  nextTokenOnChannel(i, channel) {
    this.sync(i);
    if (i >= this.tokens.length) {
      return -1;
    }
    let token = this.tokens[i];
    while (token.channel !== channel) {
      if (token.type === Token.EOF) {
        return -1;
      }
      i += 1;
      this.sync(i);
      token = this.tokens[i];
    }
    return i;
  }
  /**
   * Given a starting index, return the index of the previous token on channel.
   * Return i if tokens[i] is on channel. Return -1 if there are no tokens
   * on channel between i and 0.
   */
  previousTokenOnChannel(i, channel) {
    while (i >= 0 && this.tokens[i].channel !== channel) {
      i -= 1;
    }
    return i;
  }
  /**
   * Collect all tokens on specified channel to the right of
   * the current token up until we see a token on DEFAULT_TOKEN_CHANNEL or
   * EOF. If channel is -1, find any non default channel token.
   */
  getHiddenTokensToRight(tokenIndex, channel) {
    if (channel === void 0) {
      channel = -1;
    }
    this.lazyInit();
    if (tokenIndex < 0 || tokenIndex >= this.tokens.length) {
      throw new Error(`${tokenIndex} not in 0..${this.tokens.length - 1}`);
    }
    const nextOnChannel = this.nextTokenOnChannel(tokenIndex + 1, Lexer.DEFAULT_TOKEN_CHANNEL);
    const from = tokenIndex + 1;
    const to = nextOnChannel === -1 ? this.tokens.length - 1 : nextOnChannel;
    return this.filterForChannel(from, to, channel);
  }
  /**
   * Collect all tokens on specified channel to the left of
   * the current token up until we see a token on DEFAULT_TOKEN_CHANNEL.
   * If channel is -1, find any non default channel token.
   */
  getHiddenTokensToLeft(tokenIndex, channel) {
    if (channel === void 0) {
      channel = -1;
    }
    this.lazyInit();
    if (tokenIndex < 0 || tokenIndex >= this.tokens.length) {
      throw new Error(`${tokenIndex} not in 0..${this.tokens.length - 1}`);
    }
    const prevOnChannel = this.previousTokenOnChannel(tokenIndex - 1, Lexer.DEFAULT_TOKEN_CHANNEL);
    if (prevOnChannel === tokenIndex - 1) {
      return null;
    }
    const from = prevOnChannel + 1;
    const to = tokenIndex - 1;
    return this.filterForChannel(from, to, channel);
  }
  filterForChannel(left, right, channel) {
    const hidden = [];
    for (let i = left; i < right + 1; i++) {
      const t = this.tokens[i];
      if (channel === -1) {
        if (t.channel !== Lexer.DEFAULT_TOKEN_CHANNEL) {
          hidden.push(t);
        }
      } else if (t.channel === channel) {
        hidden.push(t);
      }
    }
    if (hidden.length === 0) {
      return null;
    }
    return hidden;
  }
  getSourceName() {
    return this.tokenSource.sourceName;
  }
  getText(...args) {
    switch (args.length) {
      case 0: {
        return this.getText(Interval.of(0, this.size - 1));
      }
      case 1: {
        if (args[0] instanceof Interval) {
          const interval = args[0];
          const start = interval.start;
          let stop = interval.stop;
          if (start < 0 || stop < 0) {
            return "";
          }
          this.sync(stop);
          if (stop >= this.tokens.length) {
            stop = this.tokens.length - 1;
          }
          let buf = "";
          for (let i = start; i <= stop; i++) {
            const t = this.tokens[i];
            if (t.type === Token.EOF) {
              break;
            }
            buf += t.text;
          }
          return buf.toString();
        } else {
          const ctx = args[0];
          return this.getText(ctx.getSourceInterval());
        }
      }
      case 2: {
        const start = args[0];
        const stop = args[1];
        if (start !== null && stop !== null) {
          return this.getText(Interval.of(start.tokenIndex, stop.tokenIndex));
        }
        return "";
      }
      default: {
        throw new Error(`Invalid number of arguments`);
      }
    }
  }
  // Get all tokens from lexer until EOF.
  fill() {
    this.lazyInit();
    while (this.fetch(1e3) === 1e3) {
      ;
    }
  }
};

// src/CommonTokenStream.ts
var CommonTokenStream = class extends BufferedTokenStream {
  static {
    __name(this, "CommonTokenStream");
  }
  /**
   * Specifies the channel to use for filtering tokens.
   *
   * <p>
   * The default value is {@link Token#DEFAULT_CHANNEL}, which matches the
   * default channel assigned to tokens created by the lexer.</p>
   */
  channel = Token.DEFAULT_CHANNEL;
  constructor(lexer, channel) {
    super(lexer);
    this.channel = channel ?? Token.DEFAULT_CHANNEL;
  }
  adjustSeekIndex(i) {
    return this.nextTokenOnChannel(i, this.channel);
  }
  LB(k) {
    if (k === 0 || this.index - k < 0) {
      return null;
    }
    let i = this.index;
    let n2 = 1;
    while (n2 <= k) {
      i = this.previousTokenOnChannel(i - 1, this.channel);
      n2 += 1;
    }
    if (i < 0) {
      return null;
    }
    return this.tokens[i];
  }
  LT(k) {
    this.lazyInit();
    if (k === 0) {
      return null;
    }
    if (k < 0) {
      return this.LB(-k);
    }
    let i = this.index;
    let n2 = 1;
    while (n2 < k) {
      if (this.sync(i + 1)) {
        i = this.nextTokenOnChannel(i + 1, this.channel);
      }
      n2 += 1;
    }
    return this.tokens[i];
  }
  // Count EOF just once.
  getNumberOfOnChannelTokens() {
    let n2 = 0;
    this.fill();
    for (const t of this.tokens) {
      if (t.channel === this.channel) {
        n2 += 1;
      }
      if (t.type === Token.EOF) {
        break;
      }
    }
    return n2;
  }
};

// src/tree/xpath/XPathLexer.ts
var XPathLexer = class _XPathLexer extends Lexer {
  static {
    __name(this, "XPathLexer");
  }
  static TOKEN_REF = 1;
  static RULE_REF = 2;
  static ANYWHERE = 3;
  static ROOT = 4;
  static WILDCARD = 5;
  static BANG = 6;
  static ID = 7;
  static STRING = 8;
  static channelNames = [
    "DEFAULT_TOKEN_CHANNEL",
    "HIDDEN"
  ];
  static literalNames = [
    null,
    null,
    null,
    "'//'",
    "'/'",
    "'*'",
    "'!'"
  ];
  static symbolicNames = [
    null,
    "TOKEN_REF",
    "RULE_REF",
    "ANYWHERE",
    "ROOT",
    "WILDCARD",
    "BANG",
    "ID",
    "STRING"
  ];
  static modeNames = [
    "DEFAULT_MODE"
  ];
  static ruleNames = [
    "ANYWHERE",
    "ROOT",
    "WILDCARD",
    "BANG",
    "ID",
    "NameChar",
    "NameStartChar",
    "STRING"
  ];
  constructor(input) {
    super(input);
    this.interpreter = new LexerATNSimulator(this, _XPathLexer._ATN, _XPathLexer.decisionsToDFA, new PredictionContextCache());
  }
  get grammarFileName() {
    return "XPathLexer.g4";
  }
  get literalNames() {
    return _XPathLexer.literalNames;
  }
  get symbolicNames() {
    return _XPathLexer.symbolicNames;
  }
  get ruleNames() {
    return _XPathLexer.ruleNames;
  }
  get serializedATN() {
    return _XPathLexer._serializedATN;
  }
  get channelNames() {
    return _XPathLexer.channelNames;
  }
  get modeNames() {
    return _XPathLexer.modeNames;
  }
  action(localContext, ruleIndex, actionIndex) {
    switch (ruleIndex) {
      case 4:
        this.ID_action(localContext, actionIndex);
        break;
    }
  }
  ID_action(localContext, actionIndex) {
    switch (actionIndex) {
      case 0:
        const text = this.text;
        if (text.charAt(0) === text.charAt(0).toUpperCase()) {
          this.type = _XPathLexer.TOKEN_REF;
        } else {
          this.type = _XPathLexer.RULE_REF;
        }
        break;
    }
  }
  static _serializedATN = [
    4,
    0,
    8,
    48,
    6,
    -1,
    2,
    0,
    7,
    0,
    2,
    1,
    7,
    1,
    2,
    2,
    7,
    2,
    2,
    3,
    7,
    3,
    2,
    4,
    7,
    4,
    2,
    5,
    7,
    5,
    2,
    6,
    7,
    6,
    2,
    7,
    7,
    7,
    1,
    0,
    1,
    0,
    1,
    0,
    1,
    1,
    1,
    1,
    1,
    2,
    1,
    2,
    1,
    3,
    1,
    3,
    1,
    4,
    1,
    4,
    5,
    4,
    29,
    8,
    4,
    10,
    4,
    12,
    4,
    32,
    9,
    4,
    1,
    4,
    1,
    4,
    1,
    5,
    1,
    5,
    1,
    6,
    1,
    6,
    1,
    7,
    1,
    7,
    5,
    7,
    42,
    8,
    7,
    10,
    7,
    12,
    7,
    45,
    9,
    7,
    1,
    7,
    1,
    7,
    1,
    43,
    0,
    8,
    1,
    3,
    3,
    4,
    5,
    5,
    7,
    6,
    9,
    7,
    11,
    0,
    13,
    0,
    15,
    8,
    1,
    0,
    2,
    784,
    0,
    0,
    8,
    14,
    27,
    48,
    57,
    65,
    90,
    95,
    95,
    97,
    122,
    127,
    159,
    170,
    170,
    173,
    173,
    181,
    181,
    186,
    186,
    192,
    214,
    216,
    246,
    248,
    705,
    710,
    721,
    736,
    740,
    748,
    748,
    750,
    750,
    768,
    884,
    886,
    887,
    890,
    893,
    895,
    895,
    902,
    902,
    904,
    906,
    908,
    908,
    910,
    929,
    931,
    1013,
    1015,
    1153,
    1155,
    1159,
    1162,
    1327,
    1329,
    1366,
    1369,
    1369,
    1376,
    1416,
    1425,
    1469,
    1471,
    1471,
    1473,
    1474,
    1476,
    1477,
    1479,
    1479,
    1488,
    1514,
    1519,
    1522,
    1536,
    1541,
    1552,
    1562,
    1564,
    1564,
    1568,
    1641,
    1646,
    1747,
    1749,
    1757,
    1759,
    1768,
    1770,
    1788,
    1791,
    1791,
    1807,
    1866,
    1869,
    1969,
    1984,
    2037,
    2042,
    2042,
    2045,
    2045,
    2048,
    2093,
    2112,
    2139,
    2144,
    2154,
    2160,
    2183,
    2185,
    2190,
    2192,
    2193,
    2200,
    2403,
    2406,
    2415,
    2417,
    2435,
    2437,
    2444,
    2447,
    2448,
    2451,
    2472,
    2474,
    2480,
    2482,
    2482,
    2486,
    2489,
    2492,
    2500,
    2503,
    2504,
    2507,
    2510,
    2519,
    2519,
    2524,
    2525,
    2527,
    2531,
    2534,
    2545,
    2556,
    2556,
    2558,
    2558,
    2561,
    2563,
    2565,
    2570,
    2575,
    2576,
    2579,
    2600,
    2602,
    2608,
    2610,
    2611,
    2613,
    2614,
    2616,
    2617,
    2620,
    2620,
    2622,
    2626,
    2631,
    2632,
    2635,
    2637,
    2641,
    2641,
    2649,
    2652,
    2654,
    2654,
    2662,
    2677,
    2689,
    2691,
    2693,
    2701,
    2703,
    2705,
    2707,
    2728,
    2730,
    2736,
    2738,
    2739,
    2741,
    2745,
    2748,
    2757,
    2759,
    2761,
    2763,
    2765,
    2768,
    2768,
    2784,
    2787,
    2790,
    2799,
    2809,
    2815,
    2817,
    2819,
    2821,
    2828,
    2831,
    2832,
    2835,
    2856,
    2858,
    2864,
    2866,
    2867,
    2869,
    2873,
    2876,
    2884,
    2887,
    2888,
    2891,
    2893,
    2901,
    2903,
    2908,
    2909,
    2911,
    2915,
    2918,
    2927,
    2929,
    2929,
    2946,
    2947,
    2949,
    2954,
    2958,
    2960,
    2962,
    2965,
    2969,
    2970,
    2972,
    2972,
    2974,
    2975,
    2979,
    2980,
    2984,
    2986,
    2990,
    3001,
    3006,
    3010,
    3014,
    3016,
    3018,
    3021,
    3024,
    3024,
    3031,
    3031,
    3046,
    3055,
    3072,
    3084,
    3086,
    3088,
    3090,
    3112,
    3114,
    3129,
    3132,
    3140,
    3142,
    3144,
    3146,
    3149,
    3157,
    3158,
    3160,
    3162,
    3165,
    3165,
    3168,
    3171,
    3174,
    3183,
    3200,
    3203,
    3205,
    3212,
    3214,
    3216,
    3218,
    3240,
    3242,
    3251,
    3253,
    3257,
    3260,
    3268,
    3270,
    3272,
    3274,
    3277,
    3285,
    3286,
    3293,
    3294,
    3296,
    3299,
    3302,
    3311,
    3313,
    3315,
    3328,
    3340,
    3342,
    3344,
    3346,
    3396,
    3398,
    3400,
    3402,
    3406,
    3412,
    3415,
    3423,
    3427,
    3430,
    3439,
    3450,
    3455,
    3457,
    3459,
    3461,
    3478,
    3482,
    3505,
    3507,
    3515,
    3517,
    3517,
    3520,
    3526,
    3530,
    3530,
    3535,
    3540,
    3542,
    3542,
    3544,
    3551,
    3558,
    3567,
    3570,
    3571,
    3585,
    3642,
    3648,
    3662,
    3664,
    3673,
    3713,
    3714,
    3716,
    3716,
    3718,
    3722,
    3724,
    3747,
    3749,
    3749,
    3751,
    3773,
    3776,
    3780,
    3782,
    3782,
    3784,
    3790,
    3792,
    3801,
    3804,
    3807,
    3840,
    3840,
    3864,
    3865,
    3872,
    3881,
    3893,
    3893,
    3895,
    3895,
    3897,
    3897,
    3902,
    3911,
    3913,
    3948,
    3953,
    3972,
    3974,
    3991,
    3993,
    4028,
    4038,
    4038,
    4096,
    4169,
    4176,
    4253,
    4256,
    4293,
    4295,
    4295,
    4301,
    4301,
    4304,
    4346,
    4348,
    4680,
    4682,
    4685,
    4688,
    4694,
    4696,
    4696,
    4698,
    4701,
    4704,
    4744,
    4746,
    4749,
    4752,
    4784,
    4786,
    4789,
    4792,
    4798,
    4800,
    4800,
    4802,
    4805,
    4808,
    4822,
    4824,
    4880,
    4882,
    4885,
    4888,
    4954,
    4957,
    4959,
    4992,
    5007,
    5024,
    5109,
    5112,
    5117,
    5121,
    5740,
    5743,
    5759,
    5761,
    5786,
    5792,
    5866,
    5870,
    5880,
    5888,
    5909,
    5919,
    5940,
    5952,
    5971,
    5984,
    5996,
    5998,
    6e3,
    6002,
    6003,
    6016,
    6099,
    6103,
    6103,
    6108,
    6109,
    6112,
    6121,
    6155,
    6169,
    6176,
    6264,
    6272,
    6314,
    6320,
    6389,
    6400,
    6430,
    6432,
    6443,
    6448,
    6459,
    6470,
    6509,
    6512,
    6516,
    6528,
    6571,
    6576,
    6601,
    6608,
    6617,
    6656,
    6683,
    6688,
    6750,
    6752,
    6780,
    6783,
    6793,
    6800,
    6809,
    6823,
    6823,
    6832,
    6845,
    6847,
    6862,
    6912,
    6988,
    6992,
    7001,
    7019,
    7027,
    7040,
    7155,
    7168,
    7223,
    7232,
    7241,
    7245,
    7293,
    7296,
    7304,
    7312,
    7354,
    7357,
    7359,
    7376,
    7378,
    7380,
    7418,
    7424,
    7957,
    7960,
    7965,
    7968,
    8005,
    8008,
    8013,
    8016,
    8023,
    8025,
    8025,
    8027,
    8027,
    8029,
    8029,
    8031,
    8061,
    8064,
    8116,
    8118,
    8124,
    8126,
    8126,
    8130,
    8132,
    8134,
    8140,
    8144,
    8147,
    8150,
    8155,
    8160,
    8172,
    8178,
    8180,
    8182,
    8188,
    8203,
    8207,
    8234,
    8238,
    8255,
    8256,
    8276,
    8276,
    8288,
    8292,
    8294,
    8303,
    8305,
    8305,
    8319,
    8319,
    8336,
    8348,
    8400,
    8412,
    8417,
    8417,
    8421,
    8432,
    8450,
    8450,
    8455,
    8455,
    8458,
    8467,
    8469,
    8469,
    8473,
    8477,
    8484,
    8484,
    8486,
    8486,
    8488,
    8488,
    8490,
    8493,
    8495,
    8505,
    8508,
    8511,
    8517,
    8521,
    8526,
    8526,
    8544,
    8584,
    11264,
    11492,
    11499,
    11507,
    11520,
    11557,
    11559,
    11559,
    11565,
    11565,
    11568,
    11623,
    11631,
    11631,
    11647,
    11670,
    11680,
    11686,
    11688,
    11694,
    11696,
    11702,
    11704,
    11710,
    11712,
    11718,
    11720,
    11726,
    11728,
    11734,
    11736,
    11742,
    11744,
    11775,
    11823,
    11823,
    12293,
    12295,
    12321,
    12335,
    12337,
    12341,
    12344,
    12348,
    12353,
    12438,
    12441,
    12442,
    12445,
    12447,
    12449,
    12538,
    12540,
    12543,
    12549,
    12591,
    12593,
    12686,
    12704,
    12735,
    12784,
    12799,
    13312,
    19903,
    19968,
    42124,
    42192,
    42237,
    42240,
    42508,
    42512,
    42539,
    42560,
    42607,
    42612,
    42621,
    42623,
    42737,
    42775,
    42783,
    42786,
    42888,
    42891,
    42954,
    42960,
    42961,
    42963,
    42963,
    42965,
    42969,
    42994,
    43047,
    43052,
    43052,
    43072,
    43123,
    43136,
    43205,
    43216,
    43225,
    43232,
    43255,
    43259,
    43259,
    43261,
    43309,
    43312,
    43347,
    43360,
    43388,
    43392,
    43456,
    43471,
    43481,
    43488,
    43518,
    43520,
    43574,
    43584,
    43597,
    43600,
    43609,
    43616,
    43638,
    43642,
    43714,
    43739,
    43741,
    43744,
    43759,
    43762,
    43766,
    43777,
    43782,
    43785,
    43790,
    43793,
    43798,
    43808,
    43814,
    43816,
    43822,
    43824,
    43866,
    43868,
    43881,
    43888,
    44010,
    44012,
    44013,
    44016,
    44025,
    44032,
    55203,
    55216,
    55238,
    55243,
    55291,
    63744,
    64109,
    64112,
    64217,
    64256,
    64262,
    64275,
    64279,
    64285,
    64296,
    64298,
    64310,
    64312,
    64316,
    64318,
    64318,
    64320,
    64321,
    64323,
    64324,
    64326,
    64433,
    64467,
    64829,
    64848,
    64911,
    64914,
    64967,
    65008,
    65019,
    65024,
    65039,
    65056,
    65071,
    65075,
    65076,
    65101,
    65103,
    65136,
    65140,
    65142,
    65276,
    65279,
    65279,
    65296,
    65305,
    65313,
    65338,
    65343,
    65343,
    65345,
    65370,
    65382,
    65470,
    65474,
    65479,
    65482,
    65487,
    65490,
    65495,
    65498,
    65500,
    65529,
    65531,
    65536,
    65547,
    65549,
    65574,
    65576,
    65594,
    65596,
    65597,
    65599,
    65613,
    65616,
    65629,
    65664,
    65786,
    65856,
    65908,
    66045,
    66045,
    66176,
    66204,
    66208,
    66256,
    66272,
    66272,
    66304,
    66335,
    66349,
    66378,
    66384,
    66426,
    66432,
    66461,
    66464,
    66499,
    66504,
    66511,
    66513,
    66517,
    66560,
    66717,
    66720,
    66729,
    66736,
    66771,
    66776,
    66811,
    66816,
    66855,
    66864,
    66915,
    66928,
    66938,
    66940,
    66954,
    66956,
    66962,
    66964,
    66965,
    66967,
    66977,
    66979,
    66993,
    66995,
    67001,
    67003,
    67004,
    67072,
    67382,
    67392,
    67413,
    67424,
    67431,
    67456,
    67461,
    67463,
    67504,
    67506,
    67514,
    67584,
    67589,
    67592,
    67592,
    67594,
    67637,
    67639,
    67640,
    67644,
    67644,
    67647,
    67669,
    67680,
    67702,
    67712,
    67742,
    67808,
    67826,
    67828,
    67829,
    67840,
    67861,
    67872,
    67897,
    67968,
    68023,
    68030,
    68031,
    68096,
    68099,
    68101,
    68102,
    68108,
    68115,
    68117,
    68119,
    68121,
    68149,
    68152,
    68154,
    68159,
    68159,
    68192,
    68220,
    68224,
    68252,
    68288,
    68295,
    68297,
    68326,
    68352,
    68405,
    68416,
    68437,
    68448,
    68466,
    68480,
    68497,
    68608,
    68680,
    68736,
    68786,
    68800,
    68850,
    68864,
    68903,
    68912,
    68921,
    69248,
    69289,
    69291,
    69292,
    69296,
    69297,
    69373,
    69404,
    69415,
    69415,
    69424,
    69456,
    69488,
    69509,
    69552,
    69572,
    69600,
    69622,
    69632,
    69702,
    69734,
    69749,
    69759,
    69818,
    69821,
    69821,
    69826,
    69826,
    69837,
    69837,
    69840,
    69864,
    69872,
    69881,
    69888,
    69940,
    69942,
    69951,
    69956,
    69959,
    69968,
    70003,
    70006,
    70006,
    70016,
    70084,
    70089,
    70092,
    70094,
    70106,
    70108,
    70108,
    70144,
    70161,
    70163,
    70199,
    70206,
    70209,
    70272,
    70278,
    70280,
    70280,
    70282,
    70285,
    70287,
    70301,
    70303,
    70312,
    70320,
    70378,
    70384,
    70393,
    70400,
    70403,
    70405,
    70412,
    70415,
    70416,
    70419,
    70440,
    70442,
    70448,
    70450,
    70451,
    70453,
    70457,
    70459,
    70468,
    70471,
    70472,
    70475,
    70477,
    70480,
    70480,
    70487,
    70487,
    70493,
    70499,
    70502,
    70508,
    70512,
    70516,
    70656,
    70730,
    70736,
    70745,
    70750,
    70753,
    70784,
    70853,
    70855,
    70855,
    70864,
    70873,
    71040,
    71093,
    71096,
    71104,
    71128,
    71133,
    71168,
    71232,
    71236,
    71236,
    71248,
    71257,
    71296,
    71352,
    71360,
    71369,
    71424,
    71450,
    71453,
    71467,
    71472,
    71481,
    71488,
    71494,
    71680,
    71738,
    71840,
    71913,
    71935,
    71942,
    71945,
    71945,
    71948,
    71955,
    71957,
    71958,
    71960,
    71989,
    71991,
    71992,
    71995,
    72003,
    72016,
    72025,
    72096,
    72103,
    72106,
    72151,
    72154,
    72161,
    72163,
    72164,
    72192,
    72254,
    72263,
    72263,
    72272,
    72345,
    72349,
    72349,
    72368,
    72440,
    72704,
    72712,
    72714,
    72758,
    72760,
    72768,
    72784,
    72793,
    72818,
    72847,
    72850,
    72871,
    72873,
    72886,
    72960,
    72966,
    72968,
    72969,
    72971,
    73014,
    73018,
    73018,
    73020,
    73021,
    73023,
    73031,
    73040,
    73049,
    73056,
    73061,
    73063,
    73064,
    73066,
    73102,
    73104,
    73105,
    73107,
    73112,
    73120,
    73129,
    73440,
    73462,
    73472,
    73488,
    73490,
    73530,
    73534,
    73538,
    73552,
    73561,
    73648,
    73648,
    73728,
    74649,
    74752,
    74862,
    74880,
    75075,
    77712,
    77808,
    77824,
    78933,
    82944,
    83526,
    92160,
    92728,
    92736,
    92766,
    92768,
    92777,
    92784,
    92862,
    92864,
    92873,
    92880,
    92909,
    92912,
    92916,
    92928,
    92982,
    92992,
    92995,
    93008,
    93017,
    93027,
    93047,
    93053,
    93071,
    93760,
    93823,
    93952,
    94026,
    94031,
    94087,
    94095,
    94111,
    94176,
    94177,
    94179,
    94180,
    94192,
    94193,
    94208,
    100343,
    100352,
    101589,
    101632,
    101640,
    110576,
    110579,
    110581,
    110587,
    110589,
    110590,
    110592,
    110882,
    110898,
    110898,
    110928,
    110930,
    110933,
    110933,
    110948,
    110951,
    110960,
    111355,
    113664,
    113770,
    113776,
    113788,
    113792,
    113800,
    113808,
    113817,
    113821,
    113822,
    113824,
    113827,
    118528,
    118573,
    118576,
    118598,
    119141,
    119145,
    119149,
    119170,
    119173,
    119179,
    119210,
    119213,
    119362,
    119364,
    119808,
    119892,
    119894,
    119964,
    119966,
    119967,
    119970,
    119970,
    119973,
    119974,
    119977,
    119980,
    119982,
    119993,
    119995,
    119995,
    119997,
    120003,
    120005,
    120069,
    120071,
    120074,
    120077,
    120084,
    120086,
    120092,
    120094,
    120121,
    120123,
    120126,
    120128,
    120132,
    120134,
    120134,
    120138,
    120144,
    120146,
    120485,
    120488,
    120512,
    120514,
    120538,
    120540,
    120570,
    120572,
    120596,
    120598,
    120628,
    120630,
    120654,
    120656,
    120686,
    120688,
    120712,
    120714,
    120744,
    120746,
    120770,
    120772,
    120779,
    120782,
    120831,
    121344,
    121398,
    121403,
    121452,
    121461,
    121461,
    121476,
    121476,
    121499,
    121503,
    121505,
    121519,
    122624,
    122654,
    122661,
    122666,
    122880,
    122886,
    122888,
    122904,
    122907,
    122913,
    122915,
    122916,
    122918,
    122922,
    122928,
    122989,
    123023,
    123023,
    123136,
    123180,
    123184,
    123197,
    123200,
    123209,
    123214,
    123214,
    123536,
    123566,
    123584,
    123641,
    124112,
    124153,
    124896,
    124902,
    124904,
    124907,
    124909,
    124910,
    124912,
    124926,
    124928,
    125124,
    125136,
    125142,
    125184,
    125259,
    125264,
    125273,
    126464,
    126467,
    126469,
    126495,
    126497,
    126498,
    126500,
    126500,
    126503,
    126503,
    126505,
    126514,
    126516,
    126519,
    126521,
    126521,
    126523,
    126523,
    126530,
    126530,
    126535,
    126535,
    126537,
    126537,
    126539,
    126539,
    126541,
    126543,
    126545,
    126546,
    126548,
    126548,
    126551,
    126551,
    126553,
    126553,
    126555,
    126555,
    126557,
    126557,
    126559,
    126559,
    126561,
    126562,
    126564,
    126564,
    126567,
    126570,
    126572,
    126578,
    126580,
    126583,
    126585,
    126588,
    126590,
    126590,
    126592,
    126601,
    126603,
    126619,
    126625,
    126627,
    126629,
    126633,
    126635,
    126651,
    130032,
    130041,
    131072,
    173791,
    173824,
    177977,
    177984,
    178205,
    178208,
    183969,
    183984,
    191456,
    194560,
    195101,
    196608,
    201546,
    201552,
    205743,
    917505,
    917505,
    917536,
    917631,
    917760,
    917999,
    662,
    0,
    65,
    90,
    97,
    122,
    170,
    170,
    181,
    181,
    186,
    186,
    192,
    214,
    216,
    246,
    248,
    705,
    710,
    721,
    736,
    740,
    748,
    748,
    750,
    750,
    880,
    884,
    886,
    887,
    890,
    893,
    895,
    895,
    902,
    902,
    904,
    906,
    908,
    908,
    910,
    929,
    931,
    1013,
    1015,
    1153,
    1162,
    1327,
    1329,
    1366,
    1369,
    1369,
    1376,
    1416,
    1488,
    1514,
    1519,
    1522,
    1568,
    1610,
    1646,
    1647,
    1649,
    1747,
    1749,
    1749,
    1765,
    1766,
    1774,
    1775,
    1786,
    1788,
    1791,
    1791,
    1808,
    1808,
    1810,
    1839,
    1869,
    1957,
    1969,
    1969,
    1994,
    2026,
    2036,
    2037,
    2042,
    2042,
    2048,
    2069,
    2074,
    2074,
    2084,
    2084,
    2088,
    2088,
    2112,
    2136,
    2144,
    2154,
    2160,
    2183,
    2185,
    2190,
    2208,
    2249,
    2308,
    2361,
    2365,
    2365,
    2384,
    2384,
    2392,
    2401,
    2417,
    2432,
    2437,
    2444,
    2447,
    2448,
    2451,
    2472,
    2474,
    2480,
    2482,
    2482,
    2486,
    2489,
    2493,
    2493,
    2510,
    2510,
    2524,
    2525,
    2527,
    2529,
    2544,
    2545,
    2556,
    2556,
    2565,
    2570,
    2575,
    2576,
    2579,
    2600,
    2602,
    2608,
    2610,
    2611,
    2613,
    2614,
    2616,
    2617,
    2649,
    2652,
    2654,
    2654,
    2674,
    2676,
    2693,
    2701,
    2703,
    2705,
    2707,
    2728,
    2730,
    2736,
    2738,
    2739,
    2741,
    2745,
    2749,
    2749,
    2768,
    2768,
    2784,
    2785,
    2809,
    2809,
    2821,
    2828,
    2831,
    2832,
    2835,
    2856,
    2858,
    2864,
    2866,
    2867,
    2869,
    2873,
    2877,
    2877,
    2908,
    2909,
    2911,
    2913,
    2929,
    2929,
    2947,
    2947,
    2949,
    2954,
    2958,
    2960,
    2962,
    2965,
    2969,
    2970,
    2972,
    2972,
    2974,
    2975,
    2979,
    2980,
    2984,
    2986,
    2990,
    3001,
    3024,
    3024,
    3077,
    3084,
    3086,
    3088,
    3090,
    3112,
    3114,
    3129,
    3133,
    3133,
    3160,
    3162,
    3165,
    3165,
    3168,
    3169,
    3200,
    3200,
    3205,
    3212,
    3214,
    3216,
    3218,
    3240,
    3242,
    3251,
    3253,
    3257,
    3261,
    3261,
    3293,
    3294,
    3296,
    3297,
    3313,
    3314,
    3332,
    3340,
    3342,
    3344,
    3346,
    3386,
    3389,
    3389,
    3406,
    3406,
    3412,
    3414,
    3423,
    3425,
    3450,
    3455,
    3461,
    3478,
    3482,
    3505,
    3507,
    3515,
    3517,
    3517,
    3520,
    3526,
    3585,
    3632,
    3634,
    3635,
    3648,
    3654,
    3713,
    3714,
    3716,
    3716,
    3718,
    3722,
    3724,
    3747,
    3749,
    3749,
    3751,
    3760,
    3762,
    3763,
    3773,
    3773,
    3776,
    3780,
    3782,
    3782,
    3804,
    3807,
    3840,
    3840,
    3904,
    3911,
    3913,
    3948,
    3976,
    3980,
    4096,
    4138,
    4159,
    4159,
    4176,
    4181,
    4186,
    4189,
    4193,
    4193,
    4197,
    4198,
    4206,
    4208,
    4213,
    4225,
    4238,
    4238,
    4256,
    4293,
    4295,
    4295,
    4301,
    4301,
    4304,
    4346,
    4348,
    4680,
    4682,
    4685,
    4688,
    4694,
    4696,
    4696,
    4698,
    4701,
    4704,
    4744,
    4746,
    4749,
    4752,
    4784,
    4786,
    4789,
    4792,
    4798,
    4800,
    4800,
    4802,
    4805,
    4808,
    4822,
    4824,
    4880,
    4882,
    4885,
    4888,
    4954,
    4992,
    5007,
    5024,
    5109,
    5112,
    5117,
    5121,
    5740,
    5743,
    5759,
    5761,
    5786,
    5792,
    5866,
    5870,
    5880,
    5888,
    5905,
    5919,
    5937,
    5952,
    5969,
    5984,
    5996,
    5998,
    6e3,
    6016,
    6067,
    6103,
    6103,
    6108,
    6108,
    6176,
    6264,
    6272,
    6276,
    6279,
    6312,
    6314,
    6314,
    6320,
    6389,
    6400,
    6430,
    6480,
    6509,
    6512,
    6516,
    6528,
    6571,
    6576,
    6601,
    6656,
    6678,
    6688,
    6740,
    6823,
    6823,
    6917,
    6963,
    6981,
    6988,
    7043,
    7072,
    7086,
    7087,
    7098,
    7141,
    7168,
    7203,
    7245,
    7247,
    7258,
    7293,
    7296,
    7304,
    7312,
    7354,
    7357,
    7359,
    7401,
    7404,
    7406,
    7411,
    7413,
    7414,
    7418,
    7418,
    7424,
    7615,
    7680,
    7957,
    7960,
    7965,
    7968,
    8005,
    8008,
    8013,
    8016,
    8023,
    8025,
    8025,
    8027,
    8027,
    8029,
    8029,
    8031,
    8061,
    8064,
    8116,
    8118,
    8124,
    8126,
    8126,
    8130,
    8132,
    8134,
    8140,
    8144,
    8147,
    8150,
    8155,
    8160,
    8172,
    8178,
    8180,
    8182,
    8188,
    8305,
    8305,
    8319,
    8319,
    8336,
    8348,
    8450,
    8450,
    8455,
    8455,
    8458,
    8467,
    8469,
    8469,
    8473,
    8477,
    8484,
    8484,
    8486,
    8486,
    8488,
    8488,
    8490,
    8493,
    8495,
    8505,
    8508,
    8511,
    8517,
    8521,
    8526,
    8526,
    8544,
    8584,
    11264,
    11492,
    11499,
    11502,
    11506,
    11507,
    11520,
    11557,
    11559,
    11559,
    11565,
    11565,
    11568,
    11623,
    11631,
    11631,
    11648,
    11670,
    11680,
    11686,
    11688,
    11694,
    11696,
    11702,
    11704,
    11710,
    11712,
    11718,
    11720,
    11726,
    11728,
    11734,
    11736,
    11742,
    11823,
    11823,
    12293,
    12295,
    12321,
    12329,
    12337,
    12341,
    12344,
    12348,
    12353,
    12438,
    12445,
    12447,
    12449,
    12538,
    12540,
    12543,
    12549,
    12591,
    12593,
    12686,
    12704,
    12735,
    12784,
    12799,
    13312,
    19903,
    19968,
    42124,
    42192,
    42237,
    42240,
    42508,
    42512,
    42527,
    42538,
    42539,
    42560,
    42606,
    42623,
    42653,
    42656,
    42735,
    42775,
    42783,
    42786,
    42888,
    42891,
    42954,
    42960,
    42961,
    42963,
    42963,
    42965,
    42969,
    42994,
    43009,
    43011,
    43013,
    43015,
    43018,
    43020,
    43042,
    43072,
    43123,
    43138,
    43187,
    43250,
    43255,
    43259,
    43259,
    43261,
    43262,
    43274,
    43301,
    43312,
    43334,
    43360,
    43388,
    43396,
    43442,
    43471,
    43471,
    43488,
    43492,
    43494,
    43503,
    43514,
    43518,
    43520,
    43560,
    43584,
    43586,
    43588,
    43595,
    43616,
    43638,
    43642,
    43642,
    43646,
    43695,
    43697,
    43697,
    43701,
    43702,
    43705,
    43709,
    43712,
    43712,
    43714,
    43714,
    43739,
    43741,
    43744,
    43754,
    43762,
    43764,
    43777,
    43782,
    43785,
    43790,
    43793,
    43798,
    43808,
    43814,
    43816,
    43822,
    43824,
    43866,
    43868,
    43881,
    43888,
    44002,
    44032,
    55203,
    55216,
    55238,
    55243,
    55291,
    63744,
    64109,
    64112,
    64217,
    64256,
    64262,
    64275,
    64279,
    64285,
    64285,
    64287,
    64296,
    64298,
    64310,
    64312,
    64316,
    64318,
    64318,
    64320,
    64321,
    64323,
    64324,
    64326,
    64433,
    64467,
    64829,
    64848,
    64911,
    64914,
    64967,
    65008,
    65019,
    65136,
    65140,
    65142,
    65276,
    65313,
    65338,
    65345,
    65370,
    65382,
    65470,
    65474,
    65479,
    65482,
    65487,
    65490,
    65495,
    65498,
    65500,
    65536,
    65547,
    65549,
    65574,
    65576,
    65594,
    65596,
    65597,
    65599,
    65613,
    65616,
    65629,
    65664,
    65786,
    65856,
    65908,
    66176,
    66204,
    66208,
    66256,
    66304,
    66335,
    66349,
    66378,
    66384,
    66421,
    66432,
    66461,
    66464,
    66499,
    66504,
    66511,
    66513,
    66517,
    66560,
    66717,
    66736,
    66771,
    66776,
    66811,
    66816,
    66855,
    66864,
    66915,
    66928,
    66938,
    66940,
    66954,
    66956,
    66962,
    66964,
    66965,
    66967,
    66977,
    66979,
    66993,
    66995,
    67001,
    67003,
    67004,
    67072,
    67382,
    67392,
    67413,
    67424,
    67431,
    67456,
    67461,
    67463,
    67504,
    67506,
    67514,
    67584,
    67589,
    67592,
    67592,
    67594,
    67637,
    67639,
    67640,
    67644,
    67644,
    67647,
    67669,
    67680,
    67702,
    67712,
    67742,
    67808,
    67826,
    67828,
    67829,
    67840,
    67861,
    67872,
    67897,
    67968,
    68023,
    68030,
    68031,
    68096,
    68096,
    68112,
    68115,
    68117,
    68119,
    68121,
    68149,
    68192,
    68220,
    68224,
    68252,
    68288,
    68295,
    68297,
    68324,
    68352,
    68405,
    68416,
    68437,
    68448,
    68466,
    68480,
    68497,
    68608,
    68680,
    68736,
    68786,
    68800,
    68850,
    68864,
    68899,
    69248,
    69289,
    69296,
    69297,
    69376,
    69404,
    69415,
    69415,
    69424,
    69445,
    69488,
    69505,
    69552,
    69572,
    69600,
    69622,
    69635,
    69687,
    69745,
    69746,
    69749,
    69749,
    69763,
    69807,
    69840,
    69864,
    69891,
    69926,
    69956,
    69956,
    69959,
    69959,
    69968,
    70002,
    70006,
    70006,
    70019,
    70066,
    70081,
    70084,
    70106,
    70106,
    70108,
    70108,
    70144,
    70161,
    70163,
    70187,
    70207,
    70208,
    70272,
    70278,
    70280,
    70280,
    70282,
    70285,
    70287,
    70301,
    70303,
    70312,
    70320,
    70366,
    70405,
    70412,
    70415,
    70416,
    70419,
    70440,
    70442,
    70448,
    70450,
    70451,
    70453,
    70457,
    70461,
    70461,
    70480,
    70480,
    70493,
    70497,
    70656,
    70708,
    70727,
    70730,
    70751,
    70753,
    70784,
    70831,
    70852,
    70853,
    70855,
    70855,
    71040,
    71086,
    71128,
    71131,
    71168,
    71215,
    71236,
    71236,
    71296,
    71338,
    71352,
    71352,
    71424,
    71450,
    71488,
    71494,
    71680,
    71723,
    71840,
    71903,
    71935,
    71942,
    71945,
    71945,
    71948,
    71955,
    71957,
    71958,
    71960,
    71983,
    71999,
    71999,
    72001,
    72001,
    72096,
    72103,
    72106,
    72144,
    72161,
    72161,
    72163,
    72163,
    72192,
    72192,
    72203,
    72242,
    72250,
    72250,
    72272,
    72272,
    72284,
    72329,
    72349,
    72349,
    72368,
    72440,
    72704,
    72712,
    72714,
    72750,
    72768,
    72768,
    72818,
    72847,
    72960,
    72966,
    72968,
    72969,
    72971,
    73008,
    73030,
    73030,
    73056,
    73061,
    73063,
    73064,
    73066,
    73097,
    73112,
    73112,
    73440,
    73458,
    73474,
    73474,
    73476,
    73488,
    73490,
    73523,
    73648,
    73648,
    73728,
    74649,
    74752,
    74862,
    74880,
    75075,
    77712,
    77808,
    77824,
    78895,
    78913,
    78918,
    82944,
    83526,
    92160,
    92728,
    92736,
    92766,
    92784,
    92862,
    92880,
    92909,
    92928,
    92975,
    92992,
    92995,
    93027,
    93047,
    93053,
    93071,
    93760,
    93823,
    93952,
    94026,
    94032,
    94032,
    94099,
    94111,
    94176,
    94177,
    94179,
    94179,
    94208,
    100343,
    100352,
    101589,
    101632,
    101640,
    110576,
    110579,
    110581,
    110587,
    110589,
    110590,
    110592,
    110882,
    110898,
    110898,
    110928,
    110930,
    110933,
    110933,
    110948,
    110951,
    110960,
    111355,
    113664,
    113770,
    113776,
    113788,
    113792,
    113800,
    113808,
    113817,
    119808,
    119892,
    119894,
    119964,
    119966,
    119967,
    119970,
    119970,
    119973,
    119974,
    119977,
    119980,
    119982,
    119993,
    119995,
    119995,
    119997,
    120003,
    120005,
    120069,
    120071,
    120074,
    120077,
    120084,
    120086,
    120092,
    120094,
    120121,
    120123,
    120126,
    120128,
    120132,
    120134,
    120134,
    120138,
    120144,
    120146,
    120485,
    120488,
    120512,
    120514,
    120538,
    120540,
    120570,
    120572,
    120596,
    120598,
    120628,
    120630,
    120654,
    120656,
    120686,
    120688,
    120712,
    120714,
    120744,
    120746,
    120770,
    120772,
    120779,
    122624,
    122654,
    122661,
    122666,
    122928,
    122989,
    123136,
    123180,
    123191,
    123197,
    123214,
    123214,
    123536,
    123565,
    123584,
    123627,
    124112,
    124139,
    124896,
    124902,
    124904,
    124907,
    124909,
    124910,
    124912,
    124926,
    124928,
    125124,
    125184,
    125251,
    125259,
    125259,
    126464,
    126467,
    126469,
    126495,
    126497,
    126498,
    126500,
    126500,
    126503,
    126503,
    126505,
    126514,
    126516,
    126519,
    126521,
    126521,
    126523,
    126523,
    126530,
    126530,
    126535,
    126535,
    126537,
    126537,
    126539,
    126539,
    126541,
    126543,
    126545,
    126546,
    126548,
    126548,
    126551,
    126551,
    126553,
    126553,
    126555,
    126555,
    126557,
    126557,
    126559,
    126559,
    126561,
    126562,
    126564,
    126564,
    126567,
    126570,
    126572,
    126578,
    126580,
    126583,
    126585,
    126588,
    126590,
    126590,
    126592,
    126601,
    126603,
    126619,
    126625,
    126627,
    126629,
    126633,
    126635,
    126651,
    131072,
    173791,
    173824,
    177977,
    177984,
    178205,
    178208,
    183969,
    183984,
    191456,
    194560,
    195101,
    196608,
    201546,
    201552,
    205743,
    47,
    0,
    1,
    1,
    0,
    0,
    0,
    0,
    3,
    1,
    0,
    0,
    0,
    0,
    5,
    1,
    0,
    0,
    0,
    0,
    7,
    1,
    0,
    0,
    0,
    0,
    9,
    1,
    0,
    0,
    0,
    0,
    15,
    1,
    0,
    0,
    0,
    1,
    17,
    1,
    0,
    0,
    0,
    3,
    20,
    1,
    0,
    0,
    0,
    5,
    22,
    1,
    0,
    0,
    0,
    7,
    24,
    1,
    0,
    0,
    0,
    9,
    26,
    1,
    0,
    0,
    0,
    11,
    35,
    1,
    0,
    0,
    0,
    13,
    37,
    1,
    0,
    0,
    0,
    15,
    39,
    1,
    0,
    0,
    0,
    17,
    18,
    5,
    47,
    0,
    0,
    18,
    19,
    5,
    47,
    0,
    0,
    19,
    2,
    1,
    0,
    0,
    0,
    20,
    21,
    5,
    47,
    0,
    0,
    21,
    4,
    1,
    0,
    0,
    0,
    22,
    23,
    5,
    42,
    0,
    0,
    23,
    6,
    1,
    0,
    0,
    0,
    24,
    25,
    5,
    33,
    0,
    0,
    25,
    8,
    1,
    0,
    0,
    0,
    26,
    30,
    3,
    13,
    6,
    0,
    27,
    29,
    3,
    11,
    5,
    0,
    28,
    27,
    1,
    0,
    0,
    0,
    29,
    32,
    1,
    0,
    0,
    0,
    30,
    28,
    1,
    0,
    0,
    0,
    30,
    31,
    1,
    0,
    0,
    0,
    31,
    33,
    1,
    0,
    0,
    0,
    32,
    30,
    1,
    0,
    0,
    0,
    33,
    34,
    6,
    4,
    0,
    0,
    34,
    10,
    1,
    0,
    0,
    0,
    35,
    36,
    7,
    0,
    0,
    0,
    36,
    12,
    1,
    0,
    0,
    0,
    37,
    38,
    7,
    1,
    0,
    0,
    38,
    14,
    1,
    0,
    0,
    0,
    39,
    43,
    5,
    39,
    0,
    0,
    40,
    42,
    9,
    0,
    0,
    0,
    41,
    40,
    1,
    0,
    0,
    0,
    42,
    45,
    1,
    0,
    0,
    0,
    43,
    44,
    1,
    0,
    0,
    0,
    43,
    41,
    1,
    0,
    0,
    0,
    44,
    46,
    1,
    0,
    0,
    0,
    45,
    43,
    1,
    0,
    0,
    0,
    46,
    47,
    5,
    39,
    0,
    0,
    47,
    16,
    1,
    0,
    0,
    0,
    3,
    0,
    30,
    43,
    1,
    1,
    4,
    0
  ];
  static __ATN;
  static get _ATN() {
    if (!_XPathLexer.__ATN) {
      _XPathLexer.__ATN = new ATNDeserializer().deserialize(_XPathLexer._serializedATN);
    }
    return _XPathLexer.__ATN;
  }
  static vocabulary = new Vocabulary(_XPathLexer.literalNames, _XPathLexer.symbolicNames, []);
  get vocabulary() {
    return _XPathLexer.vocabulary;
  }
  static decisionsToDFA = _XPathLexer._ATN.decisionToState.map((ds, index) => {
    return new DFA(ds, index);
  });
};

// src/tree/xpath/XPathLexerErrorListener.ts
var XPathLexerErrorListener = class extends BaseErrorListener {
  static {
    __name(this, "XPathLexerErrorListener");
  }
  syntaxError(_recognizer, _offendingSymbol, _line, _charPositionInLine, _msg, _e) {
  }
};

// src/tree/xpath/XPathElement.ts
var XPathElement = class {
  static {
    __name(this, "XPathElement");
  }
  invert;
  nodeName;
  /**
   * Construct element like `/ID` or `ID` or `/*` etc...
   * op is null if just node
   */
  constructor(nodeName) {
    this.nodeName = nodeName;
    this.invert = false;
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathRuleAnywhereElement.ts
var XPathRuleAnywhereElement = class extends XPathElement {
  static {
    __name(this, "XPathRuleAnywhereElement");
  }
  ruleIndex;
  constructor(ruleName, ruleIndex) {
    super(ruleName);
    this.ruleIndex = ruleIndex;
  }
  evaluate(t) {
    return Trees.findAllRuleNodes(t, this.ruleIndex);
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathRuleAnywhereElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathRuleElement.ts
var XPathRuleElement = class extends XPathElement {
  static {
    __name(this, "XPathRuleElement");
  }
  ruleIndex;
  constructor(ruleName, ruleIndex) {
    super(ruleName);
    this.ruleIndex = ruleIndex;
  }
  evaluate(t) {
    const nodes = [];
    for (const c of Trees.getChildren(t)) {
      if (c instanceof ParserRuleContext) {
        if (c.ruleIndex === this.ruleIndex && !this.invert || c.ruleIndex !== this.ruleIndex && this.invert) {
          nodes.push(c);
        }
      }
    }
    return nodes;
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathRuleElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathTokenAnywhereElement.ts
var XPathTokenAnywhereElement = class extends XPathElement {
  static {
    __name(this, "XPathTokenAnywhereElement");
  }
  tokenType;
  constructor(tokenName, tokenType) {
    super(tokenName);
    this.tokenType = tokenType;
  }
  evaluate(t) {
    return Trees.findAllTokenNodes(t, this.tokenType);
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathTokenAnywhereElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathTokenElement.ts
var XPathTokenElement = class extends XPathElement {
  static {
    __name(this, "XPathTokenElement");
  }
  tokenType;
  constructor(tokenName, tokenType) {
    super(tokenName);
    this.tokenType = tokenType;
  }
  evaluate(t) {
    const nodes = [];
    for (const c of Trees.getChildren(t)) {
      if (c instanceof TerminalNode && c.symbol) {
        if (c.symbol.type === this.tokenType && !this.invert || c.symbol.type !== this.tokenType && this.invert) {
          nodes.push(c);
        }
      }
    }
    return nodes;
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathTokenElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathWildcardAnywhereElement.ts
var XPathWildcardAnywhereElement = class extends XPathElement {
  static {
    __name(this, "XPathWildcardAnywhereElement");
  }
  constructor() {
    super(XPath.WILDCARD);
  }
  evaluate(t) {
    if (this.invert) {
      return [];
    }
    return Trees.descendants(t);
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathWildcardAnywhereElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPathWildcardElement.ts
var XPathWildcardElement = class extends XPathElement {
  static {
    __name(this, "XPathWildcardElement");
  }
  constructor() {
    super(XPath.WILDCARD);
  }
  evaluate(t) {
    const kids = [];
    if (this.invert) {
      return kids;
    }
    for (const c of Trees.getChildren(t)) {
      kids.push(c);
    }
    return kids;
  }
  toString() {
    const inv = this.invert ? "!" : "";
    return "XPathWildcardElement[" + inv + this.nodeName + "]";
  }
};

// src/tree/xpath/XPath.ts
var XPath = class _XPath {
  static {
    __name(this, "XPath");
  }
  static WILDCARD = "*";
  // word not operator/separator
  static NOT = "!";
  // word for invert operator
  path;
  elements;
  parser;
  constructor(parser, path) {
    this.parser = parser;
    this.path = path;
    this.elements = this.split(path);
  }
  static findAll(tree, xpath, parser) {
    const p = new _XPath(parser, xpath);
    return p.evaluate(tree);
  }
  // TODO: check for invalid token/rule names, bad syntax
  split(path) {
    const lexer = new XPathLexer(CharStreams.fromString(path));
    lexer.recover = (e) => {
      throw e;
    };
    lexer.removeErrorListeners();
    lexer.addErrorListener(new XPathLexerErrorListener());
    const tokenStream = new CommonTokenStream(lexer);
    try {
      tokenStream.fill();
    } catch (e) {
      if (e instanceof LexerNoViableAltException) {
        const pos = lexer.column;
        const msg = "Invalid tokens or characters at index " + pos + " in path '" + path + "' -- " + e.message;
        throw new RangeError(msg);
      }
      throw e;
    }
    const tokens = tokenStream.getTokens();
    const elements = [];
    const n2 = tokens.length;
    let i = 0;
    loop:
      while (i < n2) {
        const el = tokens[i];
        let next;
        switch (el.type) {
          case XPathLexer.ROOT:
          case XPathLexer.ANYWHERE:
            const anywhere = el.type === XPathLexer.ANYWHERE;
            i++;
            next = tokens[i];
            const invert = next.type === XPathLexer.BANG;
            if (invert) {
              i++;
              next = tokens[i];
            }
            const pathElement = this.getXPathElement(next, anywhere);
            pathElement.invert = invert;
            elements.push(pathElement);
            i++;
            break;
          case XPathLexer.TOKEN_REF:
          case XPathLexer.RULE_REF:
          case XPathLexer.WILDCARD:
            elements.push(this.getXPathElement(el, false));
            i++;
            break;
          case Token.EOF:
            break loop;
          default:
            throw new Error("Unknown path element " + el);
        }
      }
    return elements;
  }
  /**
   * Return a list of all nodes starting at `t` as root that satisfy the
   * path. The root `/` is relative to the node passed to {@link evaluate}.
   */
  evaluate(t) {
    const dummyRoot = new ParserRuleContext();
    dummyRoot.addChild(t);
    let work = /* @__PURE__ */ new Set([dummyRoot]);
    let i = 0;
    while (i < this.elements.length) {
      const next = /* @__PURE__ */ new Set();
      for (const node of work) {
        if (node.getChildCount() > 0) {
          const matching = this.elements[i].evaluate(node);
          matching.forEach((tree) => {
            next.add(tree);
          }, next);
        }
      }
      i++;
      work = next;
    }
    return work;
  }
  /**
   * Convert word like `*` or `ID` or `expr` to a path
   * element. `anywhere` is `true` if `//` precedes the
   * word.
   */
  getXPathElement(wordToken, anywhere) {
    if (wordToken.type === Token.EOF) {
      throw new Error("Missing path element at end of path");
    }
    const word = wordToken.text;
    if (word == null) {
      throw new Error("Expected wordToken to have text content.");
    }
    const ttype = this.parser.getTokenType(word);
    const ruleIndex = this.parser.getRuleIndex(word);
    switch (wordToken.type) {
      case XPathLexer.WILDCARD:
        return anywhere ? new XPathWildcardAnywhereElement() : new XPathWildcardElement();
      case XPathLexer.TOKEN_REF:
      case XPathLexer.STRING:
        if (ttype === Token.INVALID_TYPE) {
          throw new Error(word + " at index " + wordToken.start + " isn't a valid token name");
        }
        return anywhere ? new XPathTokenAnywhereElement(word, ttype) : new XPathTokenElement(word, ttype);
      default:
        if (ruleIndex === -1) {
          throw new Error(word + " at index " + wordToken.start + " isn't a valid rule name");
        }
        return anywhere ? new XPathRuleAnywhereElement(word, ruleIndex) : new XPathRuleElement(word, ruleIndex);
    }
  }
};

// src/InputMismatchException.ts
var InputMismatchException = class extends RecognitionException {
  static {
    __name(this, "InputMismatchException");
  }
  constructor(recognizer) {
    super({ message: "", recognizer, input: recognizer.inputStream, ctx: recognizer.context });
    this.offendingToken = recognizer.getCurrentToken();
  }
};

// src/FailedPredicateException.ts
var FailedPredicateException = class extends RecognitionException {
  static {
    __name(this, "FailedPredicateException");
  }
  ruleIndex = 0;
  predicateIndex = 0;
  predicate;
  constructor(recognizer, predicate, message = null) {
    super({
      message: formatMessage(predicate ?? "no predicate", message ?? null),
      recognizer,
      input: recognizer.inputStream,
      ctx: recognizer.context
    });
    const s = recognizer.atn.states[recognizer.state];
    const trans = s.transitions[0];
    if (trans instanceof PredicateTransition) {
      this.ruleIndex = trans.ruleIndex;
      this.predicateIndex = trans.predIndex;
    } else {
      this.ruleIndex = 0;
      this.predicateIndex = 0;
    }
    this.predicate = predicate;
    this.offendingToken = recognizer.getCurrentToken();
  }
};
var formatMessage = /* @__PURE__ */ __name((predicate, message) => {
  if (message !== null) {
    return message;
  } else {
    return "failed predicate: {" + predicate + "}?";
  }
}, "formatMessage");

// src/DefaultErrorStrategy.ts
var DefaultErrorStrategy = class {
  static {
    __name(this, "DefaultErrorStrategy");
  }
  /**
   * Indicates whether the error strategy is currently "recovering from an
   * error". This is used to suppress reporting multiple error messages while
   * attempting to recover from a detected syntax error.
   *
   * @see #inErrorRecoveryMode
   */
  errorRecoveryMode = false;
  /**
   * The index into the input stream where the last error occurred.
   * 	This is used to prevent infinite loops where an error is found
   *  but no token is consumed during recovery...another error is found,
   *  ad nauseam.  This is a failsafe mechanism to guarantee that at least
   *  one token/tree node is consumed for two errors.
   */
  lastErrorIndex = -1;
  lastErrorStates = new IntervalSet();
  /**
   * This field is used to propagate information about the lookahead following
   * the previous match. Since prediction prefers completing the current rule
   * to error recovery efforts, error reporting may occur later than the
   * original point where it was discoverable. The original context is used to
   * compute the true expected sets as though the reporting occurred as early
   * as possible.
   */
  nextTokensContext = null;
  /**
   * @see #nextTokensContext
   */
  nextTokenState = 0;
  /**
   * <p>The default implementation simply calls {@link endErrorCondition} to
   * ensure that the handler is not in error recovery mode.</p>
   */
  reset(recognizer) {
    this.endErrorCondition(recognizer);
  }
  /**
   * This method is called to enter error recovery mode when a recognition
   * exception is reported.
   *
   * @param _recognizer the parser instance
   */
  beginErrorCondition(_recognizer) {
    this.errorRecoveryMode = true;
  }
  inErrorRecoveryMode(_recognizer) {
    return this.errorRecoveryMode;
  }
  /**
   * This method is called to leave error recovery mode after recovering from
   * a recognition exception.
   */
  endErrorCondition(_recognizer) {
    this.errorRecoveryMode = false;
    this.lastErrorStates = new IntervalSet();
    this.lastErrorIndex = -1;
  }
  /**
   * {@inheritDoc}
   * <p>The default implementation simply calls {@link endErrorCondition}.</p>
   */
  reportMatch(recognizer) {
    this.endErrorCondition(recognizer);
  }
  /**
   * {@inheritDoc}
   *
   * <p>The default implementation returns immediately if the handler is already
   * in error recovery mode. Otherwise, it calls {@link beginErrorCondition}
   * and dispatches the reporting task based on the runtime type of {@code e}
   * according to the following table.</p>
   *
   * <ul>
   * <li>{@link NoViableAltException}: Dispatches the call to
   * {@link reportNoViableAlternative}</li>
   * <li>{@link InputMismatchException}: Dispatches the call to
   * {@link reportInputMismatch}</li>
   * <li>{@link FailedPredicateException}: Dispatches the call to
   * {@link reportFailedPredicate}</li>
   * <li>All other types: calls {@link Parser//notifyErrorListeners} to report
   * the exception</li>
   * </ul>
   */
  reportError(recognizer, e) {
    if (this.inErrorRecoveryMode(recognizer)) {
      return;
    }
    this.beginErrorCondition(recognizer);
    if (e instanceof NoViableAltException) {
      this.reportNoViableAlternative(recognizer, e);
    } else if (e instanceof InputMismatchException) {
      this.reportInputMismatch(recognizer, e);
    } else if (e instanceof FailedPredicateException) {
      this.reportFailedPredicate(recognizer, e);
    } else {
      recognizer.notifyErrorListeners(e.message, e.offendingToken, e);
    }
  }
  /**
   *
   * {@inheritDoc}
   *
   * <p>The default implementation resynchronizes the parser by consuming tokens
   * until we find one in the resynchronization set--loosely the set of tokens
   * that can follow the current rule.</p>
   *
   */
  recover(recognizer, _e) {
    if (this.lastErrorIndex === recognizer.inputStream.index && this.lastErrorStates.contains(recognizer.state)) {
      recognizer.consume();
    }
    this.lastErrorIndex = recognizer.inputStream.index;
    this.lastErrorStates.addOne(recognizer.state);
    const followSet = this.getErrorRecoverySet(recognizer);
    this.consumeUntil(recognizer, followSet);
  }
  /**
   * The default implementation of {@link ANTLRErrorStrategy//sync} makes sure
   * that the current lookahead symbol is consistent with what were expecting
   * at this point in the ATN. You can call this anytime but ANTLR only
   * generates code to check before subrules/loops and each iteration.
   *
   * <p>Implements Jim Idle's magic sync mechanism in closures and optional
   * subrules. E.g.,</p>
   *
   * <pre>
   * a : sync ( stuff sync )* ;
   * sync : {consume to what can follow sync} ;
   * </pre>
   *
   * At the start of a sub rule upon error, {@link sync} performs single
   * token deletion, if possible. If it can't do that, it bails on the current
   * rule and uses the default error recovery, which consumes until the
   * resynchronization set of the current rule.
   *
   * <p>If the sub rule is optional ({@code (...)?}, {@code (...)*}, or block
   * with an empty alternative), then the expected set includes what follows
   * the subrule.</p>
   *
   * <p>During loop iteration, it consumes until it sees a token that can start a
   * sub rule or what follows loop. Yes, that is pretty aggressive. We opt to
   * stay in the loop as long as possible.</p>
   *
   * <p><strong>ORIGINS</strong></p>
   *
   * <p>Previous versions of ANTLR did a poor job of their recovery within loops.
   * A single mismatch token or missing token would force the parser to bail
   * out of the entire rules surrounding the loop. So, for rule</p>
   *
   * <pre>
   * classDef : 'class' ID '{' member* '}'
   * </pre>
   *
   * input with an extra token between members would force the parser to
   * consume until it found the next class definition rather than the next
   * member definition of the current class.
   *
   * <p>This functionality cost a little bit of effort because the parser has to
   * compare token set at the start of the loop and at each iteration. If for
   * some reason speed is suffering for you, you can turn off this
   * functionality by simply overriding this method as a blank { }.</p>
   *
   */
  sync(recognizer) {
    if (this.inErrorRecoveryMode(recognizer)) {
      return;
    }
    const s = recognizer.atn.states[recognizer.state];
    const la = recognizer.tokenStream.LA(1);
    const nextTokens = recognizer.atn.nextTokens(s);
    if (nextTokens.contains(la)) {
      this.nextTokensContext = null;
      this.nextTokenState = ATNState.INVALID_STATE_NUMBER;
      return;
    } else if (nextTokens.contains(Token.EPSILON)) {
      if (this.nextTokensContext === null) {
        this.nextTokensContext = recognizer.context;
        this.nextTokenState = recognizer.state;
      }
      return;
    }
    switch (s.stateType) {
      case ATNStateType.BLOCK_START:
      case ATNStateType.STAR_BLOCK_START:
      case ATNStateType.PLUS_BLOCK_START:
      case ATNStateType.STAR_LOOP_ENTRY:
        if (this.singleTokenDeletion(recognizer) !== null) {
          return;
        } else {
          throw new InputMismatchException(recognizer);
        }
      case ATNStateType.PLUS_LOOP_BACK:
      case ATNStateType.STAR_LOOP_BACK:
        {
          this.reportUnwantedToken(recognizer);
          const expecting = new IntervalSet();
          expecting.addSet(recognizer.getExpectedTokens());
          const whatFollowsLoopIterationOrRule = expecting.addSet(this.getErrorRecoverySet(recognizer));
          this.consumeUntil(recognizer, whatFollowsLoopIterationOrRule);
        }
        break;
      default:
    }
  }
  /**
   * This is called by {@link reportError} when the exception is a
   * {@link NoViableAltException}.
   *
   * @see //reportError
   *
   * @param recognizer the parser instance
   * @param e the recognition exception
   */
  reportNoViableAlternative(recognizer, e) {
    if (e.message.length > 0) {
      recognizer.notifyErrorListeners(e.message, e.offendingToken, e);
      return;
    }
    const tokens = recognizer.tokenStream;
    let input;
    if (tokens !== null && e.startToken) {
      if (e.startToken.type === Token.EOF) {
        input = "<EOF>";
      } else {
        input = tokens.getText(new Interval(e.startToken.tokenIndex, e.offendingToken.tokenIndex));
      }
    } else {
      input = "<unknown input>";
    }
    const msg = "no viable alternative at input " + this.escapeWSAndQuote(input);
    recognizer.notifyErrorListeners(msg, e.offendingToken, e);
  }
  /**
   * This is called by {@link reportError} when the exception is an
   * {@link InputMismatchException}.
   *
   * @see //reportError
   *
   * @param recognizer the parser instance
   * @param e the recognition exception
   */
  reportInputMismatch(recognizer, e) {
    if (e.message.length > 0) {
      recognizer.notifyErrorListeners(e.message, e.offendingToken, e);
      return;
    }
    const msg = "mismatched input " + this.getTokenErrorDisplay(e.offendingToken) + " expecting " + e.getExpectedTokens().toString(recognizer.vocabulary);
    recognizer.notifyErrorListeners(msg, e.offendingToken, e);
  }
  /**
   * This is called by {@link reportError} when the exception is a
   * {@link FailedPredicateException}.
   *
   * @see //reportError
   *
   * @param recognizer the parser instance
   * @param e the recognition exception
   */
  reportFailedPredicate(recognizer, e) {
    const ruleName = recognizer.ruleNames[recognizer.context.ruleIndex];
    const msg = "rule " + ruleName + " " + e.message;
    recognizer.notifyErrorListeners(msg, e.offendingToken, e);
  }
  /**
   * This method is called to report a syntax error which requires the removal
   * of a token from the input stream. At the time this method is called, the
   * erroneous symbol is current {@code LT(1)} symbol and has not yet been
   * removed from the input stream. When this method returns,
   * {@code recognizer} is in error recovery mode.
   *
   * <p>This method is called when {@link singleTokenDeletion} identifies
   * single-token deletion as a viable recovery strategy for a mismatched
   * input error.</p>
   *
   * <p>The default implementation simply returns if the handler is already in
   * error recovery mode. Otherwise, it calls {@link beginErrorCondition} to
   * enter error recovery mode, followed by calling
   * {@link Parser//notifyErrorListeners}.</p>
   *
   * @param recognizer the parser instance
   */
  reportUnwantedToken(recognizer) {
    if (this.inErrorRecoveryMode(recognizer)) {
      return;
    }
    this.beginErrorCondition(recognizer);
    const t = recognizer.getCurrentToken();
    const tokenName = this.getTokenErrorDisplay(t);
    const expecting = this.getExpectedTokens(recognizer);
    const msg = "extraneous input " + tokenName + " expecting " + expecting.toString(recognizer.vocabulary);
    recognizer.notifyErrorListeners(msg, t, null);
  }
  /**
   * This method is called to report a syntax error which requires the
   * insertion of a missing token into the input stream. At the time this
   * method is called, the missing token has not yet been inserted. When this
   * method returns, {@code recognizer} is in error recovery mode.
   *
   * <p>This method is called when {@link singleTokenInsertion} identifies
   * single-token insertion as a viable recovery strategy for a mismatched
   * input error.</p>
   *
   * <p>The default implementation simply returns if the handler is already in
   * error recovery mode. Otherwise, it calls {@link beginErrorCondition} to
   * enter error recovery mode, followed by calling
   * {@link Parser//notifyErrorListeners}.</p>
   *
   * @param recognizer the parser instance
   */
  reportMissingToken(recognizer) {
    if (this.inErrorRecoveryMode(recognizer)) {
      return;
    }
    this.beginErrorCondition(recognizer);
    const t = recognizer.getCurrentToken();
    const expecting = this.getExpectedTokens(recognizer);
    const msg = "missing " + expecting.toString(recognizer.vocabulary) + " at " + this.getTokenErrorDisplay(t);
    recognizer.notifyErrorListeners(msg, t, null);
  }
  /**
   * <p>The default implementation attempts to recover from the mismatched input
   * by using single token insertion and deletion as described below. If the
   * recovery attempt fails, this method throws an
   * {@link InputMismatchException}.</p>
   *
   * <p><strong>EXTRA TOKEN</strong> (single token deletion)</p>
   *
   * <p>{@code LA(1)} is not what we are looking for. If {@code LA(2)} has the
   * right token, however, then assume {@code LA(1)} is some extra spurious
   * token and delete it. Then consume and return the next token (which was
   * the {@code LA(2)} token) as the successful result of the match operation.</p>
   *
   * <p>This recovery strategy is implemented by {@link singleTokenDeletion}.</p>
   *
   * <p><strong>MISSING TOKEN</strong> (single token insertion)</p>
   *
   * <p>If current token (at {@code LA(1)}) is consistent with what could come
   * after the expected {@code LA(1)} token, then assume the token is missing
   * and use the parser's {@link TokenFactory} to create it on the fly. The
   * "insertion" is performed by returning the created token as the successful
   * result of the match operation.</p>
   *
   * <p>This recovery strategy is implemented by {@link singleTokenInsertion}.</p>
   *
   * <p><strong>EXAMPLE</strong></p>
   *
   * <p>For example, Input {@code i=(3;} is clearly missing the {@code ')'}. When
   * the parser returns from the nested call to {@code expr}, it will have
   * call chain:</p>
   *
   * <pre>
   * stat -> expr -> atom
   * </pre>
   *
   * and it will be trying to match the {@code ')'} at this point in the
   * derivation:
   *
   * <pre>
   * =&gt; ID '=' '(' INT ')' ('+' atom)* ';'
   * ^
   * </pre>
   *
   * The attempt to match {@code ')'} will fail when it sees {@code ';'} and
   * call {@link recoverInline}. To recover, it sees that {@code LA(1)==';'}
   * is in the set of tokens that can follow the {@code ')'} token reference
   * in rule {@code atom}. It can assume that you forgot the {@code ')'}.
   */
  recoverInline(recognizer) {
    const matchedSymbol = this.singleTokenDeletion(recognizer);
    if (matchedSymbol !== null) {
      recognizer.consume();
      return matchedSymbol;
    }
    if (this.singleTokenInsertion(recognizer)) {
      return this.getMissingSymbol(recognizer);
    }
    throw new InputMismatchException(recognizer);
  }
  /**
   * This method implements the single-token insertion inline error recovery
   * strategy. It is called by {@link recoverInline} if the single-token
   * deletion strategy fails to recover from the mismatched input. If this
   * method returns {@code true}, {@code recognizer} will be in error recovery
   * mode.
   *
   * <p>This method determines whether or not single-token insertion is viable by
   * checking if the {@code LA(1)} input symbol could be successfully matched
   * if it were instead the {@code LA(2)} symbol. If this method returns
   * {@code true}, the caller is responsible for creating and inserting a
   * token with the correct type to produce this behavior.</p>
   *
   * @param recognizer the parser instance
   * @returns `true` if single-token insertion is a viable recovery
   * strategy for the current mismatched input, otherwise {@code false}
   */
  singleTokenInsertion(recognizer) {
    const currentSymbolType = recognizer.tokenStream.LA(1);
    const atn = recognizer.atn;
    const currentState = atn.states[recognizer.state];
    const next = currentState.transitions[0].target;
    const expectingAtLL2 = atn.nextTokens(next, recognizer.context);
    if (expectingAtLL2.contains(currentSymbolType)) {
      this.reportMissingToken(recognizer);
      return true;
    } else {
      return false;
    }
  }
  /**
   * This method implements the single-token deletion inline error recovery
   * strategy. It is called by {@link recoverInline} to attempt to recover
   * from mismatched input. If this method returns null, the parser and error
   * handler state will not have changed. If this method returns non-null,
   * {@code recognizer} will <em>not</em> be in error recovery mode since the
   * returned token was a successful match.
   *
   * <p>If the single-token deletion is successful, this method calls
   * {@link reportUnwantedToken} to report the error, followed by
   * {@link Parser//consume} to actually "delete" the extraneous token. Then,
   * before returning {@link reportMatch} is called to signal a successful
   * match.</p>
   *
   * @param recognizer the parser instance
   * @returns the successfully matched {@link Token} instance if single-token
   * deletion successfully recovers from the mismatched input, otherwise
   * {@code null}
   */
  singleTokenDeletion(recognizer) {
    const nextTokenType = recognizer.tokenStream.LA(2);
    const expecting = this.getExpectedTokens(recognizer);
    if (expecting.contains(nextTokenType)) {
      this.reportUnwantedToken(recognizer);
      recognizer.consume();
      const matchedSymbol = recognizer.getCurrentToken();
      this.reportMatch(recognizer);
      return matchedSymbol;
    } else {
      return null;
    }
  }
  /**
   * Conjure up a missing token during error recovery.
   *
   * The recognizer attempts to recover from single missing
   * symbols. But, actions might refer to that missing symbol.
   * For example, x=ID {f($x);}. The action clearly assumes
   * that there has been an identifier matched previously and that
   * $x points at that token. If that token is missing, but
   * the next token in the stream is what we want we assume that
   * this token is missing and we keep going. Because we
   * have to return some token to replace the missing token,
   * we have to conjure one up. This method gives the user control
   * over the tokens returned for missing tokens. Mostly,
   * you will want to create something special for identifier
   * tokens. For literals such as '{' and ',', the default
   * action in the parser or tree parser works. It simply creates
   * a CommonToken of the appropriate type. The text will be the token.
   * If you change what tokens must be created by the lexer,
   * override this method to create the appropriate tokens.
   *
   */
  getMissingSymbol(recognizer) {
    const currentSymbol = recognizer.getCurrentToken();
    const expecting = this.getExpectedTokens(recognizer);
    let expectedTokenType = Token.INVALID_TYPE;
    if (!expecting.isNil) {
      expectedTokenType = expecting.minElement;
    }
    let tokenText;
    if (expectedTokenType === Token.EOF) {
      tokenText = "<missing EOF>";
    } else {
      tokenText = "<missing " + recognizer.vocabulary.getDisplayName(expectedTokenType) + ">";
    }
    let current = currentSymbol;
    const lookBack = recognizer.tokenStream.LT(-1);
    if (current.type === Token.EOF && lookBack !== null) {
      current = lookBack;
    }
    return recognizer.getTokenFactory().create(
      current.source,
      expectedTokenType,
      tokenText,
      Token.DEFAULT_CHANNEL,
      -1,
      -1,
      current.line,
      current.column
    );
  }
  getExpectedTokens(recognizer) {
    return recognizer.getExpectedTokens();
  }
  /**
   * How should a token be displayed in an error message? The default
   * is to display just the text, but during development you might
   * want to have a lot of information spit out. Override in that case
   * to use t.toString() (which, for CommonToken, dumps everything about
   * the token). This is better than forcing you to override a method in
   * your token objects because you don't have to go modify your lexer
   * so that it creates a new Java type.
   */
  getTokenErrorDisplay(t) {
    if (t === null) {
      return "<no token>";
    }
    let s = t.text;
    if (s === null) {
      if (t.type === Token.EOF) {
        s = "<EOF>";
      } else {
        s = "<" + t.type + ">";
      }
    }
    return this.escapeWSAndQuote(s);
  }
  escapeWSAndQuote(s) {
    s = s.replace(/\n/g, "\\n");
    s = s.replace(/\r/g, "\\r");
    s = s.replace(/\t/g, "\\t");
    return "'" + s + "'";
  }
  /**
   * Compute the error recovery set for the current rule. During
   * rule invocation, the parser pushes the set of tokens that can
   * follow that rule reference on the stack; this amounts to
   * computing FIRST of what follows the rule reference in the
   * enclosing rule. See LinearApproximator.FIRST().
   * This local follow set only includes tokens
   * from within the rule; i.e., the FIRST computation done by
   * ANTLR stops at the end of a rule.
   *
   * EXAMPLE
   *
   * When you find a "no viable alt exception", the input is not
   * consistent with any of the alternatives for rule r. The best
   * thing to do is to consume tokens until you see something that
   * can legally follow a call to r//or* any rule that called r.
   * You don't want the exact set of viable next tokens because the
   * input might just be missing a token--you might consume the
   * rest of the input looking for one of the missing tokens.
   *
   * Consider grammar:
   *
   * a : '[' b ']'
   * | '(' b ')'
   * ;
   * b : c '^' INT ;
   * c : ID
   * | INT
   * ;
   *
   * At each rule invocation, the set of tokens that could follow
   * that rule is pushed on a stack. Here are the various
   * context-sensitive follow sets:
   *
   * FOLLOW(b1_in_a) = FIRST(']') = ']'
   * FOLLOW(b2_in_a) = FIRST(')') = ')'
   * FOLLOW(c_in_b) = FIRST('^') = '^'
   *
   * Upon erroneous input "[]", the call chain is
   *
   * a -> b -> c
   *
   * and, hence, the follow context stack is:
   *
   * depth follow set start of rule execution
   * 0 <EOF> a (from main())
   * 1 ']' b
   * 2 '^' c
   *
   * Notice that ')' is not included, because b would have to have
   * been called from a different context in rule a for ')' to be
   * included.
   *
   * For error recovery, we cannot consider FOLLOW(c)
   * (context-sensitive or otherwise). We need the combined set of
   * all context-sensitive FOLLOW sets--the set of all tokens that
   * could follow any reference in the call chain. We need to
   * resync to one of those tokens. Note that FOLLOW(c)='^' and if
   * we resync'd to that token, we'd consume until EOF. We need to
   * sync to context-sensitive FOLLOWs for a, b, and c: {']','^'}.
   * In this case, for input "[]", LA(1) is ']' and in the set, so we would
   * not consume anything. After printing an error, rule c would
   * return normally. Rule b would not find the required '^' though.
   * At this point, it gets a mismatched token error and throws an
   * exception (since LA(1) is not in the viable following token
   * set). The rule exception handler tries to recover, but finds
   * the same recovery set and doesn't consume anything. Rule b
   * exits normally returning to rule a. Now it finds the ']' (and
   * with the successful match exits errorRecovery mode).
   *
   * So, you can see that the parser walks up the call chain looking
   * for the token that was a member of the recovery set.
   *
   * Errors are not generated in errorRecovery mode.
   *
   * ANTLR's error recovery mechanism is based upon original ideas:
   *
   * "Algorithms + Data Structures = Programs" by Niklaus Wirth
   *
   * and
   *
   * "A note on error recovery in recursive descent parsers":
   * http://portal.acm.org/citation.cfm?id=947902.947905
   *
   * Later, Josef Grosch had some good ideas:
   *
   * "Efficient and Comfortable Error Recovery in Recursive Descent
   * Parsers":
   * ftp://www.cocolab.com/products/cocktail/doca4.ps/ell.ps.zip
   *
   * Like Grosch I implement context-sensitive FOLLOW sets that are combined
   * at run-time upon error to avoid overhead during parsing.
   */
  getErrorRecoverySet(recognizer) {
    const atn = recognizer.atn;
    let ctx = recognizer.context;
    const recoverSet = new IntervalSet();
    while (ctx !== null && ctx.invokingState >= 0) {
      const invokingState = atn.states[ctx.invokingState];
      const rt = invokingState.transitions[0];
      const follow = atn.nextTokens(rt.followState);
      recoverSet.addSet(follow);
      ctx = ctx.parent;
    }
    recoverSet.removeOne(Token.EPSILON);
    return recoverSet;
  }
  // Consume tokens until one matches the given token set.//
  consumeUntil(recognizer, set) {
    let ttype = recognizer.tokenStream.LA(1);
    while (ttype !== Token.EOF && !set.contains(ttype)) {
      recognizer.consume();
      ttype = recognizer.tokenStream.LA(1);
    }
  }
};

// src/BailErrorStrategy.ts
var BailErrorStrategy = class extends DefaultErrorStrategy {
  static {
    __name(this, "BailErrorStrategy");
  }
  /**
   * Instead of recovering from exception {@code e}, re-throw it wrapped
   * in a {@link ParseCancellationException} so it is not caught by the
   * rule function catches. Use {@link Exception//getCause()} to get the
   * original {@link RecognitionException}.
   */
  recover(recognizer, e) {
    let context = recognizer.context;
    while (context !== null) {
      context.exception = e;
      context = context.parent;
    }
    throw new ParseCancellationException(e);
  }
  /**
   * Make sure we don't attempt to recover inline; if the parser
   * successfully recovers, it won't throw an exception.
   */
  recoverInline(recognizer) {
    const exception = new InputMismatchException(recognizer);
    let context = recognizer.context;
    while (context !== null) {
      context.exception = exception;
      context = context.parent;
    }
    throw new ParseCancellationException(exception);
  }
  // Make sure we don't attempt to recover from problems in subrules.//
  sync(_recognizer) {
  }
};

// src/DiagnosticErrorListener.ts
var DiagnosticErrorListener = class extends BaseErrorListener {
  static {
    __name(this, "DiagnosticErrorListener");
  }
  /**
   * When `true`, only exactly known ambiguities are reported.
   */
  exactOnly;
  constructor(exactOnly) {
    super();
    this.exactOnly = exactOnly ?? true;
  }
  reportAmbiguity = (recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs) => {
    if (this.exactOnly && !exact) {
      return;
    }
    const decision = this.getDecisionDescription(recognizer, dfa);
    const conflictingAlts = this.getConflictingAlts(ambigAlts, configs);
    const text = recognizer.tokenStream.getText(Interval.of(startIndex, stopIndex));
    const message = `reportAmbiguity d=${decision}: ambigAlts=${conflictingAlts}, input='${text}'`;
    recognizer.notifyErrorListeners(message, null, null);
  };
  reportAttemptingFullContext = (recognizer, dfa, startIndex, stopIndex, _conflictingAlts, _configs) => {
    const decision = this.getDecisionDescription(recognizer, dfa);
    const text = recognizer.tokenStream.getText(Interval.of(startIndex, stopIndex));
    const message = `reportAttemptingFullContext d=${decision}, input='${text}'`;
    recognizer.notifyErrorListeners(message, null, null);
  };
  reportContextSensitivity = (recognizer, dfa, startIndex, stopIndex, _prediction, _configs) => {
    const decision = this.getDecisionDescription(recognizer, dfa);
    const text = recognizer.tokenStream.getText(Interval.of(startIndex, stopIndex));
    const message = `reportContextSensitivity d=${decision}, input='${text}'`;
    recognizer.notifyErrorListeners(message, null, null);
  };
  getDecisionDescription = (recognizer, dfa) => {
    const decision = dfa.decision;
    const ruleIndex = dfa.atnStartState.ruleIndex;
    const ruleNames = recognizer.ruleNames;
    if (ruleIndex < 0 || ruleIndex >= ruleNames.length) {
      return decision.toString();
    }
    const ruleName = ruleNames[ruleIndex];
    if (ruleName.length === 0) {
      return decision.toString();
    }
    return `${decision} (${ruleName})`;
  };
  /**
   * Computes the set of conflicting or ambiguous alternatives from a
   * configuration set, if that information was not already provided by the
   * parser.
   *
   * @param reportedAlts The set of conflicting or ambiguous alternatives, as
   * reported by the parser.
   * @param configs The conflicting or ambiguous configuration set.
   * @returns Returns {@code reportedAlts} if it is not {@code null}, otherwise
   * returns the set of alternatives represented in {@code configs}.
   */
  getConflictingAlts = (reportedAlts, configs) => {
    if (reportedAlts !== null) {
      return reportedAlts;
    }
    const result = new BitSet();
    for (let i = 0; i < configs.configs.length; i++) {
      result.set(configs.configs[i].alt);
    }
    return result;
  };
};

// src/InterpreterRuleContext.ts
var InterpreterRuleContext = class extends ParserRuleContext {
  static {
    __name(this, "InterpreterRuleContext");
  }
  /** This is the backing field for {@link #getRuleIndex}. */
  #ruleIndex;
  constructor(ruleIndex, parent, invokingStateNumber) {
    if (invokingStateNumber !== void 0) {
      super(parent, invokingStateNumber);
    } else {
      super();
    }
    this.#ruleIndex = ruleIndex;
  }
  get ruleIndex() {
    return this.#ruleIndex;
  }
};

// src/LexerInterpreter.ts
var LexerInterpreter = class extends Lexer {
  static {
    __name(this, "LexerInterpreter");
  }
  #grammarFileName;
  #atn;
  #ruleNames;
  #channelNames;
  #modeNames;
  #vocabulary;
  #decisionToDFA;
  #sharedContextCache = new PredictionContextCache();
  constructor(grammarFileName, vocabulary, ruleNames, channelNames, modeNames, atn, input) {
    super(input);
    if (atn.grammarType !== ATNType.LEXER) {
      throw new Error("IllegalArgumentException: The ATN must be a lexer ATN.");
    }
    this.#grammarFileName = grammarFileName;
    this.#atn = atn;
    this.#ruleNames = ruleNames.slice(0);
    this.#channelNames = channelNames.slice(0);
    this.#modeNames = modeNames.slice(0);
    this.#vocabulary = vocabulary;
    this.#decisionToDFA = atn.decisionToState.map((ds, i) => {
      return new DFA(ds, i);
    });
    this.interpreter = new LexerATNSimulator(this, atn, this.#decisionToDFA, this.#sharedContextCache);
  }
  get atn() {
    return this.#atn;
  }
  get grammarFileName() {
    return this.#grammarFileName;
  }
  get ruleNames() {
    return this.#ruleNames;
  }
  get channelNames() {
    return this.#channelNames;
  }
  get modeNames() {
    return this.#modeNames;
  }
  get vocabulary() {
    return this.#vocabulary;
  }
};

// src/TraceListener.ts
var TraceListener = class {
  static {
    __name(this, "TraceListener");
  }
  parser;
  constructor(parser) {
    this.parser = parser;
  }
  enterEveryRule(ctx) {
    console.log("enter   " + this.parser.ruleNames[ctx.ruleIndex] + ", LT(1)=" + this.parser.inputStream.LT(1).text);
  }
  visitTerminal(node) {
    console.log("consume " + node.getSymbol() + " rule " + this.parser.ruleNames[this.parser.context.ruleIndex]);
  }
  exitEveryRule(ctx) {
    console.log("exit    " + this.parser.ruleNames[ctx.ruleIndex] + ", LT(1)=" + this.parser.inputStream.LT(1).text);
  }
  visitErrorNode(_node) {
  }
};

// src/Parser.ts
var Parser = class extends Recognizer {
  static {
    __name(this, "Parser");
  }
  /** For testing only. */
  printer = null;
  /**
   * Specifies whether or not the parser should construct a parse tree during
   * the parsing process. The default value is {@code true}.
   *
   * @see #getBuildParseTree
   * @see #setBuildParseTree
   */
  buildParseTrees = true;
  /**
   * The error handling strategy for the parser. The default value is a new
   * instance of {@link DefaultErrorStrategy}.
   *
   * @see #getErrorHandler
   * @see #setErrorHandler
   */
  errorHandler = new DefaultErrorStrategy();
  /**
   * The {@link ParserRuleContext} object for the currently executing rule.
   * This is always non-null during the parsing process.
   */
  // TODO: make private
  context = null;
  /**
   * The input stream.
   *
   * @see #getInputStream
   * @see #setInputStream
   */
  _input = null;
  _precedenceStack = [];
  /**
   * The list of {@link ParseTreeListener} listeners registered to receive
   * events during the parse.
   *
   * @see #addParseListener
   */
  _parseListeners = null;
  /**
   * The number of syntax errors reported during parsing. This value is
   * incremented each time {@link #notifyErrorListeners} is called.
   */
  _syntaxErrors = 0;
  /** Indicates parser has matched EOF token. See {@link #exitRule()}. */
  matchedEOF = false;
  /**
   * When {@link #setTrace}{@code (true)} is called, a reference to the
   * {@link TraceListener} is stored here so it can be easily removed in a
   * later call to {@link #setTrace}{@code (false)}. The listener itself is
   * implemented as a parser listener so this field is not directly used by
   * other parser methods.
   */
  _tracer = null;
  /**
   * This field holds the deserialized {@link ATN} with bypass alternatives, created
   * lazily upon first demand. In 4.10 I changed from map<serializedATNstring, ATN>
   * since we only need one per parser object and also it complicates other targets
   * that don't use ATN strings.
   *
   * @see ATNDeserializationOptions#isGenerateRuleBypassTransitions()
   */
  bypassAltsAtnCache = null;
  /**
   * this is all the parsing support code essentially; most of it is error
   * recovery stuff.
   */
  constructor(input) {
    super();
    this._precedenceStack.push(0);
    this._syntaxErrors = 0;
    this.tokenStream = input;
  }
  // reset the parser's state
  reset() {
    if (this._input !== null) {
      this._input.seek(0);
    }
    this.errorHandler.reset(this);
    this.context = null;
    this._syntaxErrors = 0;
    this.setTrace(false);
    this._precedenceStack = [];
    this._precedenceStack.push(0);
    if (this.interpreter) {
      this.interpreter.reset();
    }
  }
  /**
   * Match current input symbol against {@code ttype}. If the symbol type
   * matches, {@link ANTLRErrorStrategy//reportMatch} and {@link consume} are
   * called to complete the match process.
   *
   * <p>If the symbol type does not match,
   * {@link ANTLRErrorStrategy//recoverInline} is called on the current error
   * strategy to attempt recovery. If {@link buildParseTree} is
   * {@code true} and the token index of the symbol returned by
   * {@link ANTLRErrorStrategy//recoverInline} is -1, the symbol is added to
   * the parse tree by calling {@link ParserRuleContext//addErrorNode}.</p>
   *
   * @param ttype the token type to match
   * @returns the matched symbol
   * @throws RecognitionException if the current input symbol did not match
   * {@code ttype} and the error strategy could not recover from the
   * mismatched symbol
   */
  match(ttype) {
    let t = this.getCurrentToken();
    if (t.type === ttype) {
      this.errorHandler.reportMatch(this);
      this.consume();
    } else {
      t = this.errorHandler.recoverInline(this);
      if (this.buildParseTrees && t.tokenIndex === -1) {
        this.context.addErrorNode(this.createErrorNode(this.context, t));
      }
    }
    return t;
  }
  /**
   * Match current input symbol as a wildcard. If the symbol type matches
   * (i.e. has a value greater than 0), {@link ANTLRErrorStrategy//reportMatch}
   * and {@link consume} are called to complete the match process.
   *
   * <p>If the symbol type does not match,
   * {@link ANTLRErrorStrategy//recoverInline} is called on the current error
   * strategy to attempt recovery. If {@link buildParseTree} is
   * {@code true} and the token index of the symbol returned by
   * {@link ANTLRErrorStrategy//recoverInline} is -1, the symbol is added to
   * the parse tree by calling {@link ParserRuleContext//addErrorNode}.</p>
   *
   * @returns the matched symbol
   * @throws RecognitionException if the current input symbol did not match
   * a wildcard and the error strategy could not recover from the mismatched
   * symbol
   */
  matchWildcard() {
    let t = this.getCurrentToken();
    if (t.type > 0) {
      this.errorHandler.reportMatch(this);
      this.consume();
    } else {
      t = this.errorHandler.recoverInline(this);
      if (this.buildParseTrees && t.tokenIndex === -1) {
        this.context.addErrorNode(this.createErrorNode(this.context, t));
      }
    }
    return t;
  }
  getParseListeners() {
    return this._parseListeners ?? [];
  }
  /**
   * Registers {@code listener} to receive events during the parsing process.
   *
   * <p>To support output-preserving grammar transformations (including but not
   * limited to left-recursion removal, automated left-factoring, and
   * optimized code generation), calls to listener methods during the parse
   * may differ substantially from calls made by
   * {@link ParseTreeWalker//DEFAULT} used after the parse is complete. In
   * particular, rule entry and exit events may occur in a different order
   * during the parse than after the parser. In addition, calls to certain
   * rule entry methods may be omitted.</p>
   *
   * <p>With the following specific exceptions, calls to listener events are
   * <em>deterministic</em>, i.e. for identical input the calls to listener
   * methods will be the same.</p>
   *
   * <ul>
   * <li>Alterations to the grammar used to generate code may change the
   * behavior of the listener calls.</li>
   * <li>Alterations to the command line options passed to ANTLR 4 when
   * generating the parser may change the behavior of the listener calls.</li>
   * <li>Changing the version of the ANTLR Tool used to generate the parser
   * may change the behavior of the listener calls.</li>
   * </ul>
   *
   * @param listener the listener to add
   *
   * @throws NullPointerException if {@code} listener is {@code null}
   */
  addParseListener(listener) {
    if (listener === null) {
      throw new Error("listener");
    }
    if (this._parseListeners === null) {
      this._parseListeners = [];
    }
    this._parseListeners.push(listener);
  }
  /**
   * Remove {@code listener} from the list of parse listeners.
   *
   * <p>If {@code listener} is {@code null} or has not been added as a parse
   * listener, this method does nothing.</p>
   *
   * @param listener the listener to remove
   */
  removeParseListener(listener) {
    if (this._parseListeners !== null && listener !== null) {
      const idx = this._parseListeners.indexOf(listener);
      if (idx >= 0) {
        this._parseListeners.splice(idx, 1);
      }
      if (this._parseListeners.length === 0) {
        this._parseListeners = null;
      }
    }
  }
  // Remove all parse listeners.
  removeParseListeners() {
    this._parseListeners = null;
  }
  // Notify any parse listeners of an enter rule event.
  triggerEnterRuleEvent() {
    if (this._parseListeners !== null) {
      const ctx = this.context;
      this._parseListeners.forEach((listener) => {
        listener.enterEveryRule(ctx);
        ctx.enterRule(listener);
      });
    }
  }
  /**
   * Notify any parse listeners of an exit rule event.
   *
   * @see //addParseListener
   */
  triggerExitRuleEvent() {
    if (this._parseListeners !== null) {
      const ctx = this.context;
      this._parseListeners.slice(0).reverse().forEach((listener) => {
        ctx.exitRule(listener);
        listener.exitEveryRule(ctx);
      });
    }
  }
  getTokenFactory() {
    return this._input.getTokenSource().tokenFactory;
  }
  // Tell our token source and error strategy about a new way to create tokens.
  setTokenFactory(factory) {
    this._input.getTokenSource().tokenFactory = factory;
  }
  /**
   * The ATN with bypass alternatives is expensive to create so we create it
   * lazily.
   *
   * @throws UnsupportedOperationException if the current parser does not
   * implement the {@link getSerializedATN()} method.
   */
  getATNWithBypassAlts() {
    const serializedAtn = this.getSerializedATN();
    if (serializedAtn === null) {
      throw new Error("The current parser does not support an ATN with bypass alternatives.");
    }
    if (this.bypassAltsAtnCache !== null) {
      return this.bypassAltsAtnCache;
    }
    const deserializationOptions = new ATNDeserializationOptions();
    deserializationOptions.generateRuleBypassTransitions = true;
    this.bypassAltsAtnCache = new ATNDeserializer(deserializationOptions).deserialize(serializedAtn);
    return this.bypassAltsAtnCache;
  }
  get tokenStream() {
    return this._input;
  }
  // Set the token stream and reset the parser.
  set tokenStream(input) {
    this._input = null;
    this.reset();
    this._input = input;
  }
  get inputStream() {
    return this._input;
  }
  set inputStream(input) {
    this.tokenStream = input;
  }
  /**
   * Gets the number of syntax errors reported during parsing. This value is
   * incremented each time {@link notifyErrorListeners} is called.
   */
  get numberOfSyntaxErrors() {
    return this._syntaxErrors;
  }
  /**
   * Match needs to return the current input symbol, which gets put
   * into the label for the associated token ref; e.g., x=ID.
   */
  getCurrentToken() {
    return this._input.LT(1);
  }
  notifyErrorListeners(msg, offendingToken, err) {
    offendingToken = offendingToken ?? null;
    err = err ?? null;
    if (offendingToken === null) {
      offendingToken = this.getCurrentToken();
    }
    this._syntaxErrors += 1;
    const line = offendingToken.line;
    const column = offendingToken.column;
    const listener = this.getErrorListenerDispatch();
    listener.syntaxError(this, offendingToken, line, column, msg, err);
  }
  /**
   * Consume and return the {@link getCurrentToken current symbol}.
   *
   * <p>E.g., given the following input with {@code A} being the current
   * lookahead symbol, this function moves the cursor to {@code B} and returns
   * {@code A}.</p>
   *
   * <pre>
   * A B
   * ^
   * </pre>
   *
   * If the parser is not in error recovery mode, the consumed symbol is added
   * to the parse tree using {@link ParserRuleContext//addChild(Token)}, and
   * {@link ParseTreeListener//visitTerminal} is called on any parse listeners.
   * If the parser <em>is</em> in error recovery mode, the consumed symbol is
   * added to the parse tree using
   * {@link ParserRuleContext//addErrorNode(Token)}, and
   * {@link ParseTreeListener//visitErrorNode} is called on any parse
   * listeners.
   */
  consume() {
    const o = this.getCurrentToken();
    if (o.type !== Token.EOF) {
      this.tokenStream.consume();
    }
    const hasListener = this._parseListeners !== null && this._parseListeners.length > 0;
    if (this.buildParseTrees || hasListener) {
      let node;
      if (this.errorHandler.inErrorRecoveryMode(this)) {
        node = this.context.addErrorNode(this.createErrorNode(this.context, o));
      } else {
        node = this.context.addTokenNode(o);
      }
      if (hasListener) {
        this._parseListeners.forEach((listener) => {
          if (node instanceof ErrorNode) {
            listener.visitErrorNode(node);
          } else {
            listener.visitTerminal(node);
          }
        });
      }
    }
    return o;
  }
  addContextToParseTree() {
    if (this.context?.parent !== null) {
      this.context.parent.addChild(this.context);
    }
  }
  /**
   * Always called by generated parsers upon entry to a rule. Access field
   * {@link context} get the current context.
   */
  enterRule(localctx, state, _ruleIndex) {
    this.state = state;
    this.context = localctx;
    this.context.start = this._input.LT(1);
    if (this.buildParseTrees) {
      this.addContextToParseTree();
    }
    this.triggerEnterRuleEvent();
  }
  exitRule() {
    this.context.stop = this._input.LT(-1);
    this.triggerExitRuleEvent();
    this.state = this.context.invokingState;
    this.context = this.context.parent;
  }
  enterOuterAlt(localctx, altNum) {
    localctx.setAltNumber(altNum);
    if (this.buildParseTrees && this.context !== localctx) {
      if (this.context.parent !== null) {
        this.context.parent.removeLastChild();
        this.context.parent.addChild(localctx);
      }
    }
    this.context = localctx;
  }
  /**
   * Get the precedence level for the top-most precedence rule.
   *
   * @returns The precedence level for the top-most precedence rule, or -1 if
   * the parser context is not nested within a precedence rule.
   */
  getPrecedence() {
    if (this._precedenceStack.length === 0) {
      return -1;
    } else {
      return this._precedenceStack[this._precedenceStack.length - 1];
    }
  }
  enterRecursionRule(localctx, state, ruleIndex, precedence) {
    this.state = state;
    this._precedenceStack.push(precedence);
    this.context = localctx;
    this.context.start = this._input.LT(1);
    this.triggerEnterRuleEvent();
  }
  // Like {@link enterRule} but for recursive rules.
  pushNewRecursionContext(localctx, state, _ruleIndex) {
    const previous = this.context;
    previous.parent = localctx;
    previous.invokingState = state;
    previous.stop = this._input.LT(-1);
    this.context = localctx;
    this.context.start = previous.start;
    if (this.buildParseTrees) {
      this.context.addChild(previous);
    }
    this.triggerEnterRuleEvent();
  }
  unrollRecursionContexts(parent) {
    this._precedenceStack.pop();
    this.context.stop = this._input.LT(-1);
    const retCtx = this.context;
    const parseListeners = this.getParseListeners();
    if (parseListeners !== null && parseListeners.length > 0) {
      while (this.context !== parent) {
        this.triggerExitRuleEvent();
        this.context = this.context.parent;
      }
    } else {
      this.context = parent;
    }
    retCtx.parent = parent;
    if (this.buildParseTrees && parent !== null) {
      parent.addChild(retCtx);
    }
  }
  getInvokingContext(ruleIndex) {
    let ctx = this.context;
    while (ctx !== null) {
      if (ctx.ruleIndex === ruleIndex) {
        return ctx;
      }
      ctx = ctx.parent;
    }
    return null;
  }
  precpred(_localctx, precedence) {
    return precedence >= this._precedenceStack[this._precedenceStack.length - 1];
  }
  inContext(_context) {
    return false;
  }
  /**
   * Checks whether or not {@code symbol} can follow the current state in the
   * ATN. The behavior of this method is equivalent to the following, but is
   * implemented such that the complete context-sensitive follow set does not
   * need to be explicitly constructed.
   *
   * <pre>
   * return getExpectedTokens().contains(symbol);
   * </pre>
   *
   * @param symbol the symbol type to check
   * @returns `true` if {@code symbol} can follow the current state in
   * the ATN, otherwise {@code false}.
   */
  isExpectedToken(symbol) {
    const atn = this.interpreter.atn;
    let ctx = this.context;
    const s = atn.states[this.state];
    let following = atn.nextTokens(s);
    if (following.contains(symbol)) {
      return true;
    }
    if (!following.contains(Token.EPSILON)) {
      return false;
    }
    while (ctx !== null && ctx.invokingState >= 0 && following.contains(Token.EPSILON)) {
      const invokingState = atn.states[ctx.invokingState];
      const rt = invokingState.transitions[0];
      following = atn.nextTokens(rt.followState);
      if (following.contains(symbol)) {
        return true;
      }
      ctx = ctx.parent;
    }
    if (following.contains(Token.EPSILON) && symbol === Token.EOF) {
      return true;
    } else {
      return false;
    }
  }
  /**
   * Computes the set of input symbols which could follow the current parser
   * state and context, as given by {@link getState} and {@link getContext},
   * respectively.
   *
   * @see ATN.getExpectedTokens(int, RuleContext)
   */
  getExpectedTokens() {
    return this.interpreter.atn.getExpectedTokens(this.state, this.context);
  }
  getExpectedTokensWithinCurrentRule() {
    const atn = this.interpreter.atn;
    const s = atn.states[this.state];
    return atn.nextTokens(s);
  }
  // Get a rule's index (i.e., {@code RULE_ruleName} field) or -1 if not found.
  getRuleIndex(ruleName) {
    const ruleIndex = this.getRuleIndexMap().get(ruleName);
    if (ruleIndex != null) {
      return ruleIndex;
    } else {
      return -1;
    }
  }
  /**
   * Return List&lt;String&gt; of the rule names in your parser instance
   * leading up to a call to the current rule. You could override if
   * you want more details such as the file/line info of where
   * in the ATN a rule is invoked.
   *
   * this is very useful for error messages.
   */
  getRuleInvocationStack(p) {
    p = p ?? null;
    if (p === null) {
      p = this.context;
    }
    const stack = [];
    while (p !== null) {
      const ruleIndex = p.ruleIndex;
      if (ruleIndex < 0) {
        stack.push("n/a");
      } else {
        stack.push(this.ruleNames[ruleIndex]);
      }
      p = p.parent;
    }
    return stack;
  }
  /**
   * For debugging and other purposes.
   *
   * TODO: this differs from the Java version. Change it.
   */
  getDFAStrings() {
    return this.interpreter.decisionToDFA.toString();
  }
  /** For debugging and other purposes. */
  dumpDFA() {
    let seenOne = false;
    for (const dfa of this.interpreter.decisionToDFA) {
      if (dfa.states.length > 0) {
        if (seenOne) {
          console.log();
        }
        if (this.printer) {
          this.printer.println("Decision " + dfa.decision + ":");
          this.printer.print(dfa.toString(this.vocabulary));
        }
        seenOne = true;
      }
    }
  }
  getSourceName() {
    return this._input.getSourceName();
  }
  setProfile(profile) {
    const interp = this.interpreter;
    const saveMode = interp.predictionMode;
    if (profile) {
      if (!(interp instanceof ProfilingATNSimulator)) {
        this.interpreter = new ProfilingATNSimulator(this);
      }
    } else if (interp instanceof ProfilingATNSimulator) {
      const sharedContextCache = interp.getSharedContextCache();
      if (sharedContextCache) {
        const sim = new ParserATNSimulator(this, this.atn, interp.decisionToDFA, sharedContextCache);
        this.interpreter = sim;
      }
    }
    this.interpreter.predictionMode = saveMode;
  }
  /**
   * During a parse is sometimes useful to listen in on the rule entry and exit
   * events as well as token matches. this is for quick and dirty debugging.
   */
  setTrace(trace) {
    if (!trace) {
      this.removeParseListener(this._tracer);
      this._tracer = null;
    } else {
      if (this._tracer !== null) {
        this.removeParseListener(this._tracer);
      }
      this._tracer = new TraceListener(this);
      this.addParseListener(this._tracer);
    }
  }
  createTerminalNode(parent, t) {
    return new TerminalNode(t);
  }
  createErrorNode(parent, t) {
    return new ErrorNode(t);
  }
};

// src/ParserInterpreter.ts
var ParserInterpreter = class extends Parser {
  static {
    __name(this, "ParserInterpreter");
  }
  _rootContext;
  _parentContextStack = [];
  overrideDecision = -1;
  overrideDecisionInputIndex = -1;
  overrideDecisionAlt = -1;
  overrideDecisionReached = false;
  _overrideDecisionRoot = null;
  #grammarFileName;
  #atn;
  #ruleNames;
  #vocabulary;
  #decisionToDFA;
  #sharedContextCache = new PredictionContextCache();
  #pushRecursionContextStates;
  constructor(grammarFileName, vocabulary, ruleNames, atn, input) {
    super(input);
    this.#grammarFileName = grammarFileName;
    this.#atn = atn;
    this.#ruleNames = ruleNames.slice(0);
    this.#vocabulary = vocabulary;
    this.#pushRecursionContextStates = new BitSet();
    for (const state of atn.states) {
      if (state instanceof StarLoopEntryState && state.precedenceRuleDecision) {
        this.#pushRecursionContextStates.set(state.stateNumber);
      }
    }
    this.#decisionToDFA = atn.decisionToState.map((ds, i) => {
      return new DFA(ds, i);
    });
    this.interpreter = new ParserATNSimulator(this, atn, this.#decisionToDFA, this.#sharedContextCache);
  }
  reset() {
    super.reset();
    this.overrideDecisionReached = false;
    this._overrideDecisionRoot = null;
  }
  get atn() {
    return this.#atn;
  }
  get vocabulary() {
    return this.#vocabulary;
  }
  get ruleNames() {
    return this.#ruleNames;
  }
  get grammarFileName() {
    return this.#grammarFileName;
  }
  get atnState() {
    return this.#atn.states[this.state];
  }
  parse(startRuleIndex) {
    const startRuleStartState = this.#atn.ruleToStartState[startRuleIndex];
    this._rootContext = this.createInterpreterRuleContext(null, ATNState.INVALID_STATE_NUMBER, startRuleIndex);
    if (startRuleStartState.isPrecedenceRule) {
      this.enterRecursionRule(this._rootContext, startRuleStartState.stateNumber, startRuleIndex, 0);
    } else {
      this.enterRule(this._rootContext, startRuleStartState.stateNumber, startRuleIndex);
    }
    while (true) {
      const p = this.atnState;
      switch (p.stateType) {
        case ATNStateType.RULE_STOP:
          if (this.context?.isEmpty) {
            if (startRuleStartState.isPrecedenceRule) {
              const result = this.context;
              const parentContext = this._parentContextStack.pop();
              this.unrollRecursionContexts(parentContext[0]);
              return result;
            } else {
              this.exitRule();
              return this._rootContext;
            }
          }
          this.visitRuleStopState(p);
          break;
        default:
          try {
            this.visitState(p);
          } catch (e) {
            if (e instanceof RecognitionException) {
              this.state = this.#atn.ruleToStopState[p.ruleIndex].stateNumber;
              this.context.exception = e;
              this.errorHandler.reportError(this, e);
              this.recover(e);
            } else {
              throw e;
            }
          }
          break;
      }
    }
  }
  addDecisionOverride(decision, tokenIndex, forcedAlt) {
    this.overrideDecision = decision;
    this.overrideDecisionInputIndex = tokenIndex;
    this.overrideDecisionAlt = forcedAlt;
  }
  get overrideDecisionRoot() {
    return this._overrideDecisionRoot;
  }
  get rootContext() {
    return this._rootContext;
  }
  enterRecursionRule(localctx, state, ruleIndex, precedence) {
    this._parentContextStack.push([this.context, localctx.invokingState]);
    super.enterRecursionRule(localctx, state, ruleIndex, precedence);
  }
  visitState(p) {
    let predictedAlt = 1;
    if (p instanceof DecisionState) {
      predictedAlt = this.visitDecisionState(p);
    }
    const transition = p.transitions[predictedAlt - 1];
    switch (transition.serializationType) {
      case TransitionType.EPSILON:
        if (this.#pushRecursionContextStates.get(p.stateNumber) && !(transition.target instanceof LoopEndState)) {
          const parentContext = this._parentContextStack[this._parentContextStack.length - 1];
          const localctx = this.createInterpreterRuleContext(parentContext[0], parentContext[1], this.context.ruleIndex);
          this.pushNewRecursionContext(
            localctx,
            this.#atn.ruleToStartState[p.ruleIndex].stateNumber,
            this.context.ruleIndex
          );
        }
        break;
      case TransitionType.ATOM:
        this.match(transition.label.minElement);
        break;
      case TransitionType.RANGE:
      case TransitionType.SET:
      case TransitionType.NOT_SET:
        if (!transition.matches(this._input.LA(1), Token.MIN_USER_TOKEN_TYPE, 65535)) {
          this.recoverInline();
        }
        this.matchWildcard();
        break;
      case TransitionType.WILDCARD:
        this.matchWildcard();
        break;
      case TransitionType.RULE:
        const ruleStartState = transition.target;
        const ruleIndex = ruleStartState.ruleIndex;
        const newContext = this.createInterpreterRuleContext(this.context, p.stateNumber, ruleIndex);
        if (ruleStartState.isPrecedenceRule) {
          this.enterRecursionRule(
            newContext,
            ruleStartState.stateNumber,
            ruleIndex,
            transition.precedence
          );
        } else {
          this.enterRule(newContext, transition.target.stateNumber, ruleIndex);
        }
        break;
      case TransitionType.PREDICATE:
        const predicateTransition = transition;
        if (!this.sempred(this.context, predicateTransition.ruleIndex, predicateTransition.predIndex)) {
          throw new FailedPredicateException(this);
        }
        break;
      case TransitionType.ACTION:
        const actionTransition = transition;
        this.action(this.context, actionTransition.ruleIndex, actionTransition.actionIndex);
        break;
      case TransitionType.PRECEDENCE:
        if (!this.precpred(this.context, transition.precedence)) {
          const precedence = transition.precedence;
          throw new FailedPredicateException(this, `precpred(_ctx, ${precedence})`);
        }
        break;
      default:
        throw new Error("UnsupportedOperationException: Unrecognized ATN transition type.");
    }
    this.state = transition.target.stateNumber;
  }
  visitDecisionState(p) {
    let predictedAlt = 1;
    if (p.transitions.length > 1) {
      this.errorHandler.sync(this);
      const decision = p.decision;
      if (decision === this.overrideDecision && this._input.index === this.overrideDecisionInputIndex && !this.overrideDecisionReached) {
        predictedAlt = this.overrideDecisionAlt;
        this.overrideDecisionReached = true;
      } else {
        predictedAlt = this.interpreter.adaptivePredict(this._input, decision, this.context);
      }
    }
    return predictedAlt;
  }
  createInterpreterRuleContext(parent, invokingStateNumber, ruleIndex) {
    return new InterpreterRuleContext(ruleIndex, parent, invokingStateNumber);
  }
  visitRuleStopState(p) {
    const ruleStartState = this.#atn.ruleToStartState[p.ruleIndex];
    if (ruleStartState.isPrecedenceRule) {
      const [parentContext, state] = this._parentContextStack.pop();
      this.unrollRecursionContexts(parentContext);
      this.state = state;
    } else {
      this.exitRule();
    }
    const ruleTransition = this.#atn.states[this.state].transitions[0];
    this.state = ruleTransition.followState.stateNumber;
  }
  recover(e) {
    const i = this._input.index;
    this.errorHandler.recover(this, e);
    if (this._input.index === i) {
      const tok = e.offendingToken;
      if (!tok) {
        throw new Error("Expected exception to have an offending token");
      }
      const source = tok.tokenSource;
      const stream = source?.inputStream ?? null;
      const sourcePair = [source, stream];
      if (e instanceof InputMismatchException) {
        const expectedTokens = e.getExpectedTokens();
        if (!expectedTokens) {
          throw new Error("Expected the exception to provide expected tokens");
        }
        let expectedTokenType = Token.INVALID_TYPE;
        if (!expectedTokens.isNil) {
          expectedTokenType = expectedTokens.minElement;
        }
        const errToken = this.getTokenFactory().create(
          sourcePair,
          expectedTokenType,
          tok.text,
          Token.DEFAULT_CHANNEL,
          -1,
          -1,
          tok.line,
          tok.column
        );
        this.context.addErrorNode(this.createErrorNode(this.context, errToken));
      } else {
        const errToken = this.getTokenFactory().create(
          sourcePair,
          Token.INVALID_TYPE,
          tok.text,
          Token.DEFAULT_CHANNEL,
          -1,
          -1,
          tok.line,
          tok.column
        );
        this.context.addErrorNode(this.createErrorNode(this.context, errToken));
      }
    }
  }
  recoverInline() {
    return this.errorHandler.recoverInline(this);
  }
};

// src/TokenStreamRewriter.ts
var TokenStreamRewriter = class _TokenStreamRewriter {
  static {
    __name(this, "TokenStreamRewriter");
  }
  static DEFAULT_PROGRAM_NAME = "default";
  static PROGRAM_INIT_SIZE = 100;
  static MIN_TOKEN_INDEX = 0;
  /** Our source stream */
  tokens;
  /**
   * You may have multiple, named streams of rewrite operations.
   *  I'm calling these things "programs."
   *  Maps String (name) -> rewrite (List)
   */
  programs = /* @__PURE__ */ new Map();
  /** Map String (program name) -> Integer index */
  lastRewriteTokenIndexes;
  /**
   * @param tokens The token stream to modify
   */
  constructor(tokens) {
    this.tokens = tokens;
  }
  getTokenStream() {
    return this.tokens;
  }
  /**
   * Insert the supplied text after the specified token (or token index)
   */
  insertAfter(tokenOrIndex, text, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    let index;
    if (typeof tokenOrIndex === "number") {
      index = tokenOrIndex;
    } else {
      index = tokenOrIndex.tokenIndex;
    }
    const rewrites = this.getProgram(programName);
    const op = new InsertAfterOp(this.tokens, index, rewrites.length, text);
    rewrites.push(op);
  }
  /**
   * Insert the supplied text before the specified token (or token index)
   */
  insertBefore(tokenOrIndex, text, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    let index;
    if (typeof tokenOrIndex === "number") {
      index = tokenOrIndex;
    } else {
      index = tokenOrIndex.tokenIndex;
    }
    const rewrites = this.getProgram(programName);
    const op = new InsertBeforeOp(this.tokens, index, rewrites.length, text);
    rewrites.push(op);
  }
  /**
   * Replace the specified token with the supplied text
   */
  replaceSingle(tokenOrIndex, text, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    this.replace(tokenOrIndex, tokenOrIndex, text, programName);
  }
  /**
   * Replace the specified range of tokens with the supplied text.
   */
  replace(from, to, text, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    if (typeof from !== "number") {
      from = from.tokenIndex;
    }
    if (typeof to !== "number") {
      to = to.tokenIndex;
    }
    if (from > to || from < 0 || to < 0 || to >= this.tokens.size) {
      throw new RangeError(`replace: range invalid: ${from}..${to}(size=${this.tokens.size})`);
    }
    const rewrites = this.getProgram(programName);
    const op = new ReplaceOp(this.tokens, from, to, rewrites.length, text);
    rewrites.push(op);
  }
  /**
   * Delete the specified range of tokens
   */
  delete(from, to, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    if (to == null) {
      to = from;
    }
    this.replace(from, to, null, programName);
  }
  getProgram(name) {
    let is = this.programs.get(name);
    if (is == null) {
      is = this.initializeProgram(name);
    }
    return is;
  }
  initializeProgram(name) {
    const is = [];
    this.programs.set(name, is);
    return is;
  }
  /**
   * @returns the text from the original tokens altered per the instructions given to this rewriter
   */
  getText(intervalOrProgram, programName = _TokenStreamRewriter.DEFAULT_PROGRAM_NAME) {
    let interval;
    if (intervalOrProgram instanceof Interval) {
      interval = intervalOrProgram;
    } else {
      interval = new Interval(0, this.tokens.size - 1);
    }
    if (typeof intervalOrProgram === "string") {
      programName = intervalOrProgram;
    }
    const rewrites = this.programs.get(programName);
    let start = interval.start;
    let stop = interval.stop;
    if (stop > this.tokens.size - 1) {
      stop = this.tokens.size - 1;
    }
    if (start < 0) {
      start = 0;
    }
    if (rewrites == null || rewrites.length === 0) {
      return this.tokens.getText(new Interval(start, stop));
    }
    const buf = [];
    const indexToOp = this.reduceToSingleOperationPerIndex(rewrites);
    let i = start;
    while (i <= stop && i < this.tokens.size) {
      const op = indexToOp.get(i);
      indexToOp.delete(i);
      const t = this.tokens.get(i);
      if (op == null) {
        if (t.type !== Token.EOF) {
          buf.push(String(t.text));
        }
        i++;
      } else {
        i = op.execute(buf);
      }
    }
    if (stop === this.tokens.size - 1) {
      for (const op of indexToOp.values()) {
        if (op && op.index >= this.tokens.size - 1) {
          buf.push(String(op.text));
        }
      }
    }
    return buf.join("");
  }
  /**
   * @returns a map from token index to operation
   */
  reduceToSingleOperationPerIndex(rewrites) {
    for (let i = 0; i < rewrites.length; i++) {
      const op = rewrites[i];
      if (op == null) {
        continue;
      }
      if (!(op instanceof ReplaceOp)) {
        continue;
      }
      const rop = op;
      const inserts = this.getKindOfOps(rewrites, InsertBeforeOp, i);
      for (const iop of inserts) {
        if (iop.index === rop.index) {
          rewrites[iop.instructionIndex] = null;
          rop.text = String(iop.text) + (rop.text != null ? rop.text.toString() : "");
        } else if (iop.index > rop.index && iop.index <= rop.lastIndex) {
          rewrites[iop.instructionIndex] = null;
        }
      }
      const prevReplaces = this.getKindOfOps(rewrites, ReplaceOp, i);
      for (const prevRop of prevReplaces) {
        if (prevRop.index >= rop.index && prevRop.lastIndex <= rop.lastIndex) {
          rewrites[prevRop.instructionIndex] = null;
          continue;
        }
        const disjoint = prevRop.lastIndex < rop.index || prevRop.index > rop.lastIndex;
        if (prevRop.text == null && rop.text == null && !disjoint) {
          rewrites[prevRop.instructionIndex] = null;
          rop.index = Math.min(prevRop.index, rop.index);
          rop.lastIndex = Math.max(prevRop.lastIndex, rop.lastIndex);
        } else if (!disjoint) {
          throw new Error(`replace op boundaries of ${rop} overlap with previous ${prevRop}`);
        }
      }
    }
    for (let i = 0; i < rewrites.length; i++) {
      const op = rewrites[i];
      if (op == null) {
        continue;
      }
      if (!(op instanceof InsertBeforeOp)) {
        continue;
      }
      const iop = op;
      const prevInserts = this.getKindOfOps(rewrites, InsertBeforeOp, i);
      for (const prevIop of prevInserts) {
        if (prevIop.index === iop.index) {
          if (prevIop instanceof InsertAfterOp) {
            iop.text = this.catOpText(prevIop.text, iop.text);
            rewrites[prevIop.instructionIndex] = null;
          } else if (prevIop instanceof InsertBeforeOp) {
            iop.text = this.catOpText(iop.text, prevIop.text);
            rewrites[prevIop.instructionIndex] = null;
          }
        }
      }
      const prevReplaces = this.getKindOfOps(rewrites, ReplaceOp, i);
      for (const rop of prevReplaces) {
        if (iop.index === rop.index) {
          rop.text = this.catOpText(iop.text, rop.text);
          rewrites[i] = null;
          continue;
        }
        if (iop.index >= rop.index && iop.index <= rop.lastIndex) {
          throw new Error(`insert op ${iop} within boundaries of previous ${rop}`);
        }
      }
    }
    const m2 = /* @__PURE__ */ new Map();
    for (const op of rewrites) {
      if (op == null) {
        continue;
      }
      if (m2.get(op.index) != null) {
        throw new Error("should only be one op per index");
      }
      m2.set(op.index, op);
    }
    return m2;
  }
  catOpText(a, b) {
    let x = "";
    let y = "";
    if (a != null) {
      x = a.toString();
    }
    if (b != null) {
      y = b.toString();
    }
    return x + y;
  }
  /**
   * Get all operations before an index of a particular kind
   */
  getKindOfOps(rewrites, kind, before) {
    return rewrites.slice(0, before).filter((op) => {
      return op && op instanceof kind;
    });
  }
};
var RewriteOperation = class {
  static {
    __name(this, "RewriteOperation");
  }
  /** What index into rewrites List are we? */
  instructionIndex;
  /** Token buffer index. */
  index;
  text;
  tokens;
  constructor(tokens, index, instructionIndex, text) {
    this.tokens = tokens;
    this.instructionIndex = instructionIndex;
    this.index = index;
    this.text = text === void 0 ? "" : text;
  }
  execute(_buf) {
    return this.index;
  }
  toString() {
    return "<RewriteOperation@" + this.tokens.get(this.index) + ':"' + this.text + '">';
  }
};
var InsertBeforeOp = class extends RewriteOperation {
  static {
    __name(this, "InsertBeforeOp");
  }
  constructor(tokens, index, instructionIndex, text) {
    super(tokens, index, instructionIndex, text);
  }
  /**
   * @returns the index of the next token to operate on
   */
  execute(buf) {
    if (this.text) {
      buf.push(this.text.toString());
    }
    if (this.tokens.get(this.index).type !== Token.EOF) {
      buf.push(String(this.tokens.get(this.index).text));
    }
    return this.index + 1;
  }
  toString() {
    return "<InsertBeforeOp@" + this.tokens.get(this.index) + ':"' + this.text + '">';
  }
};
var InsertAfterOp = class extends InsertBeforeOp {
  static {
    __name(this, "InsertAfterOp");
  }
  constructor(tokens, index, instructionIndex, text) {
    super(tokens, index + 1, instructionIndex, text);
  }
  toString() {
    return "<InsertAfterOp@" + this.tokens.get(this.index) + ':"' + this.text + '">';
  }
};
var ReplaceOp = class extends RewriteOperation {
  static {
    __name(this, "ReplaceOp");
  }
  lastIndex;
  constructor(tokens, from, to, instructionIndex, text) {
    super(tokens, from, instructionIndex, text);
    this.lastIndex = to;
  }
  /**
   * @returns the index of the next token to operate on
   */
  execute(buf) {
    if (this.text) {
      buf.push(this.text.toString());
    }
    return this.lastIndex + 1;
  }
  toString() {
    if (this.text == null) {
      return "<DeleteOp@" + this.tokens.get(this.index) + ".." + this.tokens.get(this.lastIndex) + ">";
    }
    return "<ReplaceOp@" + this.tokens.get(this.index) + ".." + this.tokens.get(this.lastIndex) + ':"' + this.text + '">';
  }
};

// src/WritableToken.ts
var isWritableToken = /* @__PURE__ */ __name((candidate) => {
  return candidate.setText !== void 0;
}, "isWritableToken");
export {
  ATN,
  ATNConfig,
  ATNConfigSet,
  ATNDeserializationOptions,
  ATNDeserializer,
  ATNSimulator,
  ATNState,
  ATNStateType,
  ATNType,
  AbstractParseTreeVisitor,
  AbstractPredicateTransition,
  ActionTransition,
  AmbiguityInfo,
  ArrayPredictionContext,
  AtomTransition,
  BailErrorStrategy,
  BaseErrorListener,
  BasicBlockStartState,
  BasicState,
  BitSet,
  BlockEndState,
  BlockStartState,
  BufferedTokenStream,
  CharStreamImpl,
  CharStreams,
  CommonToken,
  CommonTokenFactory,
  CommonTokenStream,
  ConsoleErrorListener,
  ContextSensitivityInfo,
  DFA,
  DFASerializer,
  DFAState,
  DecisionEventInfo,
  DecisionInfo,
  DecisionState,
  DefaultErrorStrategy,
  DiagnosticErrorListener,
  EmptyPredictionContext,
  EpsilonTransition,
  ErrorInfo,
  ErrorNode,
  FailedPredicateException,
  HashCode,
  HashMap,
  HashSet,
  InputMismatchException,
  IntStream,
  InterpreterDataReader,
  InterpreterRuleContext,
  Interval,
  IntervalSet,
  LL1Analyzer,
  Lexer,
  LexerATNConfig,
  LexerATNSimulator,
  LexerAction,
  LexerActionExecutor,
  LexerActionType,
  LexerChannelAction,
  LexerCustomAction,
  LexerDFASerializer,
  LexerIndexedCustomAction,
  LexerInterpreter,
  LexerModeAction,
  LexerMoreAction,
  LexerNoViableAltException,
  LexerPopModeAction,
  LexerPushModeAction,
  LexerSkipAction,
  LexerTypeAction,
  LookaheadEventInfo,
  LoopEndState,
  NoViableAltException,
  NotSetTransition,
  OrderedATNConfigSet,
  ParseCancellationException,
  ParseInfo,
  ParseTreeWalker,
  Parser,
  ParserATNSimulator,
  ParserInterpreter,
  ParserRuleContext,
  PlusBlockStartState,
  PlusLoopbackState,
  PrecedencePredicateTransition,
  PredicateEvalInfo,
  PredicateTransition,
  PredictionContext,
  PredictionContextCache,
  PredictionMode,
  ProfilingATNSimulator,
  ProxyErrorListener,
  RangeTransition,
  RecognitionException,
  Recognizer,
  RuleContext,
  RuleStartState,
  RuleStopState,
  RuleTransition,
  SemanticContext,
  SetTransition,
  SingletonPredictionContext,
  StarBlockStartState,
  StarLoopEntryState,
  StarLoopbackState,
  TerminalNode,
  Token,
  TokenStreamRewriter,
  TokensStartState,
  TraceListener,
  Transition,
  TransitionType,
  Trees,
  Vocabulary,
  WildcardTransition,
  XPath,
  XPathElement,
  XPathLexer,
  XPathLexerErrorListener,
  XPathRuleAnywhereElement,
  XPathRuleElement,
  XPathTokenAnywhereElement,
  XPathTokenElement,
  XPathWildcardAnywhereElement,
  XPathWildcardElement,
  arrayToString,
  combineCommonParents,
  equalArrays,
  escapeWhitespace,
  getAllContextNodes,
  getCachedPredictionContext,
  isToken,
  isWritableToken,
  merge,
  mergeRoot,
  mergeSingletons,
  predictionContextFromRuleContext,
  standardEqualsFunction,
  standardHashCodeFunction,
  stringHashCode
};
