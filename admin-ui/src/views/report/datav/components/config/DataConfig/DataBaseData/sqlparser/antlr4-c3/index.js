var __defProp = Object.defineProperty;
var __name = (target, value) => __defProp(target, "name", { value, configurable: true });

// src/types.ts
var MemberVisibility = /* @__PURE__ */ ((MemberVisibility2) => {
  MemberVisibility2[MemberVisibility2["Unknown"] = 0] = "Unknown";
  MemberVisibility2[MemberVisibility2["Open"] = 1] = "Open";
  MemberVisibility2[MemberVisibility2["Public"] = 2] = "Public";
  MemberVisibility2[MemberVisibility2["Protected"] = 3] = "Protected";
  MemberVisibility2[MemberVisibility2["Private"] = 4] = "Private";
  MemberVisibility2[MemberVisibility2["FilePrivate"] = 5] = "FilePrivate";
  MemberVisibility2[MemberVisibility2["Library"] = 6] = "Library";
  return MemberVisibility2;
})(MemberVisibility || {});
var Modifier = /* @__PURE__ */ ((Modifier3) => {
  Modifier3[Modifier3["Static"] = 0] = "Static";
  Modifier3[Modifier3["Final"] = 1] = "Final";
  Modifier3[Modifier3["Sealed"] = 2] = "Sealed";
  Modifier3[Modifier3["Abstract"] = 3] = "Abstract";
  Modifier3[Modifier3["Deprecated"] = 4] = "Deprecated";
  Modifier3[Modifier3["Virtual"] = 5] = "Virtual";
  Modifier3[Modifier3["Const"] = 6] = "Const";
  Modifier3[Modifier3["Overwritten"] = 7] = "Overwritten";
  return Modifier3;
})(Modifier || {});
var TypeKind = /* @__PURE__ */ ((TypeKind2) => {
  TypeKind2[TypeKind2["Unknown"] = 0] = "Unknown";
  TypeKind2[TypeKind2["Integer"] = 1] = "Integer";
  TypeKind2[TypeKind2["Float"] = 2] = "Float";
  TypeKind2[TypeKind2["Number"] = 3] = "Number";
  TypeKind2[TypeKind2["String"] = 4] = "String";
  TypeKind2[TypeKind2["Char"] = 5] = "Char";
  TypeKind2[TypeKind2["Boolean"] = 6] = "Boolean";
  TypeKind2[TypeKind2["Class"] = 7] = "Class";
  TypeKind2[TypeKind2["Interface"] = 8] = "Interface";
  TypeKind2[TypeKind2["Array"] = 9] = "Array";
  TypeKind2[TypeKind2["Map"] = 10] = "Map";
  TypeKind2[TypeKind2["Enum"] = 11] = "Enum";
  TypeKind2[TypeKind2["Alias"] = 12] = "Alias";
  return TypeKind2;
})(TypeKind || {});
var ReferenceKind = /* @__PURE__ */ ((ReferenceKind3) => {
  ReferenceKind3[ReferenceKind3["Irrelevant"] = 0] = "Irrelevant";
  ReferenceKind3[ReferenceKind3["Pointer"] = 1] = "Pointer";
  ReferenceKind3[ReferenceKind3["Reference"] = 2] = "Reference";
  ReferenceKind3[ReferenceKind3["Instance"] = 3] = "Instance";
  return ReferenceKind3;
})(ReferenceKind || {});

// src/BaseSymbol.ts
var BaseSymbol = class {
  static {
    __name(this, "BaseSymbol");
  }
  /** The name of the symbol or empty if anonymous. */
  name;
  /** Reference to the parse tree which contains this symbol. */
  context;
  modifiers = /* @__PURE__ */ new Set();
  visibility = 0 /* Unknown */;
  #parent;
  constructor(name = "") {
    this.name = name;
  }
  get parent() {
    return this.#parent;
  }
  get firstSibling() {
    if (!this.#parent) {
      return void 0;
    }
    return this.#parent?.firstChild;
  }
  /**
   * @returns the symbol before this symbol in its scope.
   */
  get previousSibling() {
    if (!this.#parent) {
      return void 0;
    }
    if (!this.#parent) {
      return this;
    }
    return this.#parent.previousSiblingOf(this);
  }
  /**
   * @returns the symbol following this symbol in its scope.
   */
  get nextSibling() {
    return this.#parent?.nextSiblingOf(this);
  }
  get lastSibling() {
    return this.#parent?.lastChild;
  }
  /**
   * @returns the next symbol in definition order, regardless of the scope.
   */
  get next() {
    return this.#parent?.nextOf(this);
  }
  /**
   * @returns the outermost entity (below the symbol table) that holds us.
   */
  get root() {
    let run = this.#parent;
    while (run) {
      if (!run.parent || this.isSymbolTable(run.parent)) {
        return run;
      }
      run = run.parent;
    }
    return run;
  }
  /**
   * @returns the symbol table we belong too or undefined if we are not yet assigned.
   */
  get symbolTable() {
    if (this.isSymbolTable(this)) {
      return this;
    }
    let run = this.#parent;
    while (run) {
      if (this.isSymbolTable(run)) {
        return run;
      }
      run = run.parent;
    }
    return void 0;
  }
  /**
   * @returns the list of symbols from this one up to root.
   */
  get symbolPath() {
    const result = [];
    let run = this;
    while (run) {
      result.push(run);
      if (!run.parent) {
        break;
      }
      run = run.parent;
    }
    return result;
  }
  /**
   * This is rather an internal method and should rarely be used by external code.
   *
   * @param parent The new parent to use.
   */
  setParent(parent) {
    this.#parent = parent;
  }
  /**
   * Remove this symbol from its parent scope.
   */
  removeFromParent() {
    this.#parent?.removeSymbol(this);
    this.#parent = void 0;
  }
  /**
   * Asynchronously looks up a symbol with a given name, in a bottom-up manner.
   *
   * @param name The name of the symbol to find.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns A promise resolving to the first symbol with a given name, in the order of appearance in this scope
   *          or any of the parent scopes (conditionally).
   */
  async resolve(name, localOnly = false) {
    return this.#parent?.resolve(name, localOnly);
  }
  /**
   * Synchronously looks up a symbol with a given name, in a bottom-up manner.
   *
   * @param name The name of the symbol to find.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns the first symbol with a given name, in the order of appearance in this scope
   *          or any of the parent scopes (conditionally).
   */
  resolveSync(name, localOnly = false) {
    return this.#parent?.resolveSync(name, localOnly);
  }
  /**
   * @param t The type of objects to return.
   *
   * @returns the next enclosing parent of the given type.
   */
  getParentOfType(t) {
    let run = this.#parent;
    while (run) {
      if (run instanceof t) {
        return run;
      }
      run = run.parent;
    }
    return void 0;
  }
  /**
   * Creates a qualified identifier from this symbol and its parent.
   * If `full` is true then all parents are traversed in addition to this instance.
   *
   * @param separator The string to be used between the parts.
   * @param full A flag indicating if the full path is to be returned.
   * @param includeAnonymous Use a special string for empty scope names.
   *
   * @returns the constructed qualified identifier.
   */
  qualifiedName(separator = ".", full = false, includeAnonymous = false) {
    if (!includeAnonymous && this.name.length === 0) {
      return "";
    }
    let result = this.name.length === 0 ? "<anonymous>" : this.name;
    let run = this.#parent;
    while (run) {
      if (includeAnonymous || run.name.length > 0) {
        result = (run.name.length === 0 ? "<anonymous>" : run.name) + separator + result;
      }
      if (!full || !run.parent) {
        break;
      }
      run = run.parent;
    }
    return result;
  }
  /**
   * Type guard to check for ISymbolTable.
   *
   * @param candidate The object to check.
   *
   * @returns true if the object is a symbol table.
   */
  isSymbolTable(candidate) {
    return candidate.info !== void 0;
  }
};

// src/ArrayType.ts
var ArrayType = class extends BaseSymbol {
  static {
    __name(this, "ArrayType");
  }
  elementType;
  size;
  // > 0 if fixed length.
  referenceKind;
  constructor(name, referenceKind, elemType, size = 0) {
    super(name);
    this.referenceKind = referenceKind;
    this.elementType = elemType;
    this.size = size;
  }
  get baseTypes() {
    return [];
  }
  get kind() {
    return 9 /* Array */;
  }
  get reference() {
    return this.referenceKind;
  }
};

// src/DuplicateSymbolError.ts
var DuplicateSymbolError = class extends Error {
  static {
    __name(this, "DuplicateSymbolError");
  }
};

// src/ScopedSymbol.ts
var ScopedSymbol = class _ScopedSymbol extends BaseSymbol {
  static {
    __name(this, "ScopedSymbol");
  }
  /** All child symbols in definition order. */
  #children = [];
  // All used child names. Used to detect name collisions.
  #names = /* @__PURE__ */ new Map();
  constructor(name = "") {
    super(name);
  }
  /**
   * @returns A promise resolving to all direct child symbols with a scope (e.g. classes in a module).
   */
  get directScopes() {
    return this.getSymbolsOfType(_ScopedSymbol);
  }
  get children() {
    return this.#children;
  }
  get firstChild() {
    if (this.#children.length > 0) {
      return this.#children[0];
    }
    return void 0;
  }
  get lastChild() {
    if (this.#children.length > 0) {
      return this.#children[this.#children.length - 1];
    }
    return void 0;
  }
  clear() {
    this.#children = [];
    this.#names.clear();
  }
  /**
   * Adds the given symbol to this scope. If it belongs already to a different scope
   * it is removed from that before adding it here.
   *
   * @param symbol The symbol to add as a child.
   */
  addSymbol(symbol) {
    symbol.removeFromParent();
    const symbolTable = this.symbolTable;
    const count = this.#names.get(symbol.name);
    if (!symbolTable || !symbolTable.options.allowDuplicateSymbols) {
      if (count !== void 0) {
        throw new DuplicateSymbolError("Attempt to add duplicate symbol '" + (symbol.name ?? "<anonymous>") + "'");
      } else {
        this.#names.set(symbol.name, 1);
      }
      const index = this.#children.indexOf(symbol);
      if (index > -1) {
        throw new DuplicateSymbolError("Attempt to add duplicate symbol '" + (symbol.name ?? "<anonymous>") + "'");
      }
    } else {
      this.#names.set(symbol.name, count === void 0 ? 1 : count + 1);
    }
    this.#children.push(symbol);
    symbol.setParent(this);
  }
  removeSymbol(symbol) {
    const index = this.#children.indexOf(symbol);
    if (index > -1) {
      this.#children.splice(index, 1);
      symbol.setParent(void 0);
      const count = this.#names.get(symbol.name);
      if (count !== void 0) {
        if (count === 1) {
          this.#names.delete(symbol.name);
        } else {
          this.#names.set(symbol.name, count - 1);
        }
      }
    }
  }
  /**
   * Asynchronously retrieves child symbols of a given type from this symbol.
   *
   * @param t The type of of the objects to return.
   *
   * @returns A promise resolving to all (nested) children of the given type.
   */
  async getNestedSymbolsOfType(t) {
    const result = [];
    const childPromises = [];
    this.#children.forEach((child) => {
      if (child instanceof t) {
        result.push(child);
      }
      if (child instanceof _ScopedSymbol) {
        childPromises.push(child.getNestedSymbolsOfType(t));
      }
    });
    const childSymbols = await Promise.all(childPromises);
    childSymbols.forEach((entry) => {
      result.push(...entry);
    });
    return result;
  }
  /**
   * Synchronously retrieves child symbols of a given type from this symbol.
   *
   * @param t The type of of the objects to return.
   *
   * @returns A list of all (nested) children of the given type.
   */
  getNestedSymbolsOfTypeSync(t) {
    const result = [];
    this.#children.forEach((child) => {
      if (child instanceof t) {
        result.push(child);
      }
      if (child instanceof _ScopedSymbol) {
        result.push(...child.getNestedSymbolsOfTypeSync(t));
      }
    });
    return result;
  }
  /**
   * @param name If given only returns symbols with that name.
   *
   * @returns A promise resolving to symbols from this and all nested scopes in the order they were defined.
   */
  async getAllNestedSymbols(name) {
    const result = [];
    const childPromises = [];
    this.#children.forEach((child) => {
      if (!name || child.name === name) {
        result.push(child);
      }
      if (child instanceof _ScopedSymbol) {
        childPromises.push(child.getAllNestedSymbols(name));
      }
    });
    const childSymbols = await Promise.all(childPromises);
    childSymbols.forEach((entry) => {
      result.push(...entry);
    });
    return result;
  }
  /**
   * @param name If given only returns symbols with that name.
   *
   * @returns A list of all symbols from this and all nested scopes in the order they were defined.
   */
  getAllNestedSymbolsSync(name) {
    const result = [];
    this.#children.forEach((child) => {
      if (!name || child.name === name) {
        result.push(child);
      }
      if (child instanceof _ScopedSymbol) {
        result.push(...child.getAllNestedSymbolsSync(name));
      }
    });
    return result;
  }
  /**
   * @param t The type of of the objects to return.
   *
   * @returns A promise resolving to direct children of a given type.
   */
  getSymbolsOfType(t) {
    return new Promise((resolve) => {
      const result = [];
      this.#children.forEach((child) => {
        if (child instanceof t) {
          result.push(child);
        }
      });
      resolve(result);
    });
  }
  /**
   * TODO: add optional position dependency (only symbols defined before a given caret pos are viable).
   *
   * @param t The type of the objects to return.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns A promise resolving to all symbols of the the given type, accessible from this scope (if localOnly is
   *          false), within the owning symbol table.
   */
  async getAllSymbols(t, localOnly = false) {
    const result = [];
    for (const child of this.#children) {
      if (child instanceof t) {
        result.push(child);
      }
      if (this.isNamespace(child)) {
        const childSymbols = await child.getAllSymbols(t, true);
        result.push(...childSymbols);
      }
    }
    if (!localOnly) {
      if (this.parent) {
        const childSymbols = await this.getAllSymbols(t, true);
        result.push(...childSymbols);
      }
    }
    return result;
  }
  /**
   * TODO: add optional position dependency (only symbols defined before a given caret pos are viable).
   *
   * @param t The type of the objects to return.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns A list with all symbols of the the given type, accessible from this scope (if localOnly is
   *          false), within the owning symbol table.
   */
  getAllSymbolsSync(t, localOnly = false) {
    const result = [];
    for (const child of this.#children) {
      if (child instanceof t) {
        result.push(child);
      }
      if (this.isNamespace(child)) {
        const childSymbols = child.getAllSymbolsSync(t, true);
        result.push(...childSymbols);
      }
    }
    if (!localOnly) {
      if (this.parent) {
        const childSymbols = this.getAllSymbolsSync(t, true);
        result.push(...childSymbols);
      }
    }
    return result;
  }
  /**
   * @param name The name of the symbol to resolve.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns A promise resolving to the first symbol with a given name, in the order of appearance in this scope
   *          or any of the parent scopes (conditionally).
   */
  async resolve(name, localOnly = false) {
    return new Promise((resolve) => {
      for (const child of this.#children) {
        if (child.name === name) {
          resolve(child);
          return;
        }
      }
      if (!localOnly) {
        if (this.parent) {
          resolve(this.parent.resolve(name, false));
          return;
        }
      }
      resolve(void 0);
    });
  }
  /**
   * @param name The name of the symbol to resolve.
   * @param localOnly If true only child symbols are returned, otherwise also symbols from the parent of this symbol
   *                  (recursively).
   *
   * @returns the first symbol with a given name, in the order of appearance in this scope
   *          or any of the parent scopes (conditionally).
   */
  resolveSync(name, localOnly = false) {
    for (const child of this.#children) {
      if (child.name === name) {
        return child;
      }
    }
    if (!localOnly) {
      if (this.parent) {
        return this.parent.resolveSync(name, false);
      }
    }
    return void 0;
  }
  /**
   * @param path The path consisting of symbol names separator by `separator`.
   * @param separator The character to separate path segments.
   *
   * @returns the symbol located at the given path through the symbol hierarchy.
   */
  symbolFromPath(path, separator = ".") {
    const elements = path.split(separator);
    let index = 0;
    if (elements[0] === this.name || elements[0].length === 0) {
      ++index;
    }
    let result = this;
    while (index < elements.length) {
      if (!(result instanceof _ScopedSymbol)) {
        return void 0;
      }
      const child = result.children.find((candidate) => {
        return candidate.name === elements[index];
      });
      if (!child) {
        return void 0;
      }
      result = child;
      ++index;
    }
    return result;
  }
  /**
   * @param child The child to search for.
   *
   * @returns the index of the given child symbol in the child list or -1 if it couldn't be found.
   */
  indexOfChild(child) {
    return this.#children.findIndex((value) => {
      return value === child;
    });
  }
  /**
   * @param child The reference node.
   *
   * @returns the sibling symbol after the given child symbol, if one exists.
   */
  nextSiblingOf(child) {
    const index = this.indexOfChild(child);
    if (index === -1 || index >= this.#children.length - 1) {
      return void 0;
    }
    return this.#children[index + 1];
  }
  /**
   * @param child The reference node.
   *
   * @returns the sibling symbol before the given child symbol, if one exists.
   */
  previousSiblingOf(child) {
    const index = this.indexOfChild(child);
    if (index < 1) {
      return void 0;
    }
    return this.#children[index - 1];
  }
  /**
   * @param child The reference node.
   *
   * @returns the next symbol in definition order, regardless of the scope.
   */
  nextOf(child) {
    if (!child.parent) {
      return void 0;
    }
    if (child.parent !== this) {
      return child.parent.nextOf(child);
    }
    if (child instanceof _ScopedSymbol && child.children.length > 0) {
      return child.children[0];
    }
    const sibling = this.nextSiblingOf(child);
    if (sibling) {
      return sibling;
    }
    return this.parent.nextOf(this);
  }
  isNamespace(candidate) {
    return candidate.inline !== void 0 && candidate.attributes !== void 0;
  }
};

// src/BlockSymbol.ts
var BlockSymbol = class extends ScopedSymbol {
  static {
    __name(this, "BlockSymbol");
  }
};

// src/TypedSymbol.ts
var TypedSymbol = class extends BaseSymbol {
  static {
    __name(this, "TypedSymbol");
  }
  type;
  constructor(name, type) {
    super(name);
    this.type = type;
  }
};

// src/VariableSymbol.ts
var VariableSymbol = class extends TypedSymbol {
  static {
    __name(this, "VariableSymbol");
  }
  value;
  constructor(name, value, type) {
    super(name, type);
    this.value = value;
  }
};

// src/FieldSymbol.ts
var FieldSymbol = class extends VariableSymbol {
  static {
    __name(this, "FieldSymbol");
  }
  setter;
  getter;
};

// src/ParameterSymbol.ts
var ParameterSymbol = class extends VariableSymbol {
  static {
    __name(this, "ParameterSymbol");
  }
};

// src/RoutineSymbol.ts
var RoutineSymbol = class extends ScopedSymbol {
  static {
    __name(this, "RoutineSymbol");
  }
  returnType;
  // Can be null if result is void.
  constructor(name, returnType) {
    super(name);
    this.returnType = returnType;
  }
  getVariables(_localOnly = true) {
    return this.getSymbolsOfType(VariableSymbol);
  }
  getParameters(_localOnly = true) {
    return this.getSymbolsOfType(ParameterSymbol);
  }
};

// src/MethodSymbol.ts
var MethodFlags = /* @__PURE__ */ ((MethodFlags2) => {
  MethodFlags2[MethodFlags2["None"] = 0] = "None";
  MethodFlags2[MethodFlags2["Virtual"] = 1] = "Virtual";
  MethodFlags2[MethodFlags2["Const"] = 2] = "Const";
  MethodFlags2[MethodFlags2["Overwritten"] = 4] = "Overwritten";
  MethodFlags2[MethodFlags2["SetterOrGetter"] = 8] = "SetterOrGetter";
  MethodFlags2[MethodFlags2["Explicit"] = 16] = "Explicit";
  return MethodFlags2;
})(MethodFlags || {});
var MethodSymbol = class extends RoutineSymbol {
  static {
    __name(this, "MethodSymbol");
  }
  methodFlags = 0 /* None */;
};

// src/ClassSymbol.ts
var ClassSymbol = class extends ScopedSymbol {
  static {
    __name(this, "ClassSymbol");
  }
  isStruct = false;
  reference = 0 /* Irrelevant */;
  /** Usually only one member, unless the language supports multiple inheritance (like C++). */
  // eslint-disable-next-line no-use-before-define
  extends;
  /** Typescript allows a class to implement a class, not only interfaces. */
  // eslint-disable-next-line no-use-before-define
  implements;
  constructor(name, ext, impl) {
    super(name);
    this.extends = ext;
    this.implements = impl;
  }
  get baseTypes() {
    return this.extends;
  }
  get kind() {
    return 7 /* Class */;
  }
  /**
   * @param _includeInherited Not used.
   *
   * @returns a list of all methods.
   */
  getMethods(_includeInherited = false) {
    return this.getSymbolsOfType(MethodSymbol);
  }
  /**
   * @param _includeInherited Not used.
   *
   * @returns all fields.
   */
  getFields(_includeInherited = false) {
    return this.getSymbolsOfType(FieldSymbol);
  }
};

// src/FundamentalType.ts
var FundamentalType = class _FundamentalType {
  static {
    __name(this, "FundamentalType");
  }
  static integerType = new _FundamentalType("int", 1 /* Integer */, 3 /* Instance */);
  static floatType = new _FundamentalType("float", 2 /* Float */, 3 /* Instance */);
  static stringType = new _FundamentalType("string", 4 /* String */, 3 /* Instance */);
  static boolType = new _FundamentalType("bool", 6 /* Boolean */, 3 /* Instance */);
  name;
  typeKind;
  referenceKind;
  constructor(name, typeKind = 0 /* Unknown */, referenceKind = 0 /* Irrelevant */) {
    this.name = name;
    this.typeKind = typeKind;
    this.referenceKind = referenceKind;
  }
  get baseTypes() {
    return [];
  }
  get kind() {
    return this.typeKind;
  }
  get reference() {
    return this.referenceKind;
  }
};

// src/InterfaceSymbol.ts
var InterfaceSymbol = class extends ScopedSymbol {
  static {
    __name(this, "InterfaceSymbol");
  }
  reference = 0 /* Irrelevant */;
  /** Typescript allows an interface to extend a class, not only interfaces. */
  // eslint-disable-next-line no-use-before-define
  extends;
  constructor(name, ext) {
    super(name);
    this.extends = ext;
  }
  get baseTypes() {
    return this.extends;
  }
  get kind() {
    return 8 /* Interface */;
  }
  /**
   * @param _includeInherited not used
   *
   * @returns a list of all methods.
   */
  getMethods(_includeInherited = false) {
    return this.getSymbolsOfType(MethodSymbol);
  }
  /**
   * @param _includeInherited Not used.
   *
   * @returns all fields.
   */
  getFields(_includeInherited = false) {
    return this.getSymbolsOfType(FieldSymbol);
  }
};

// src/LiteralSymbol.ts
var LiteralSymbol = class extends TypedSymbol {
  static {
    __name(this, "LiteralSymbol");
  }
  value;
  constructor(name, value, type) {
    super(name, type);
    this.value = value;
  }
};

// src/NamespaceSymbol.ts
var NamespaceSymbol = class extends ScopedSymbol {
  static {
    __name(this, "NamespaceSymbol");
  }
  inline;
  attributes;
  constructor(name, inline = false, attributes = []) {
    super(name);
    this.inline = inline;
    this.attributes = attributes;
  }
};

// src/TypeAlias.ts
var TypeAlias = class extends BaseSymbol {
  static {
    __name(this, "TypeAlias");
  }
  targetType;
  constructor(name, target) {
    super(name);
    this.targetType = target;
  }
  get baseTypes() {
    return [this.targetType];
  }
  get kind() {
    return 12 /* Alias */;
  }
  get reference() {
    return 0 /* Irrelevant */;
  }
};

// src/CodeCompletionCore.ts
import {
  ATNState,
  IntervalSet,
  ParserRuleContext,
  Token,
  TransitionType,
  ATNStateType
} from "../antlr4ng";

// src/utils.ts
var longestCommonPrefix = /* @__PURE__ */ __name((arr1, arr2) => {
  if (!arr1 || !arr2) {
    return [];
  }
  let i;
  for (i = 0; i < Math.min(arr1.length, arr2.length); i++) {
    if (arr1[i] !== arr2[i]) {
      break;
    }
  }
  return arr1.slice(0, i);
}, "longestCommonPrefix");

// src/CodeCompletionCore.ts
var CandidatesCollection = class {
  static {
    __name(this, "CandidatesCollection");
  }
  tokens = /* @__PURE__ */ new Map();
  rules = /* @__PURE__ */ new Map();
};
var FollowSetWithPath = class {
  static {
    __name(this, "FollowSetWithPath");
  }
  intervals;
  path = [];
  following = [];
};
var CodeCompletionCore = class _CodeCompletionCore {
  static {
    __name(this, "CodeCompletionCore");
  }
  static followSetsByATN = /* @__PURE__ */ new Map();
  static atnStateTypeMap = [
    "invalid",
    "basic",
    "rule start",
    "block start",
    "plus block start",
    "star block start",
    "token start",
    "rule stop",
    "block end",
    "star loop back",
    "star loop entry",
    "plus loop back",
    "loop end"
  ];
  // Debugging options. Print human readable ATN state and other info.
  /** Not dependent on showDebugOutput. Prints the collected rules + tokens to terminal. */
  showResult = false;
  /** Enables printing ATN state info to terminal. */
  showDebugOutput = false;
  /** Only relevant when showDebugOutput is true. Enables transition printing for a state. */
  debugOutputWithTransitions = false;
  /** Also depends on showDebugOutput. Enables call stack printing for each rule recursion. */
  showRuleStack = false;
  /**
   * Tailoring of the result:
   * Tokens which should not appear in the candidates set.
   */
  ignoredTokens;
  /**
   * Rules which replace any candidate token they contain.
   * This allows to return descriptive rules (e.g. className, instead of ID/identifier).
   */
  preferredRules;
  /**
   * Specify if preferred rules should translated top-down (higher index rule returns first) or
   * bottom-up (lower index rule returns first).
   */
  translateRulesTopDown = false;
  parser;
  atn;
  vocabulary;
  ruleNames;
  tokens;
  precedenceStack;
  tokenStartIndex = 0;
  statesProcessed = 0;
  /**
   * A mapping of rule index + token stream position to end token positions.
   * A rule which has been visited before with the same input position will always produce the same output positions.
   */
  shortcutMap = /* @__PURE__ */ new Map();
  /** The collected candidates (rules and tokens). */
  candidates = new CandidatesCollection();
  constructor(parser) {
    this.parser = parser;
    this.atn = parser.atn;
    this.vocabulary = parser.vocabulary;
    this.ruleNames = parser.ruleNames;
    this.ignoredTokens = /* @__PURE__ */ new Set();
    this.preferredRules = /* @__PURE__ */ new Set();
  }
  /**
   * This is the main entry point. The caret token index specifies the token stream index for the token which
   * currently covers the caret (or any other position you want to get code completion candidates for).
   * Optionally you can pass in a parser rule context which limits the ATN walk to only that or called rules.
   * This can significantly speed up the retrieval process but might miss some candidates (if they are outside of
   * the given context).
   *
   * @param caretTokenIndex The index of the token at the caret position.
   * @param context An option parser rule context to limit the search space.
   * @returns The collection of completion candidates.
   */
  collectCandidates(caretTokenIndex, context) {
    this.shortcutMap.clear();
    this.candidates.rules.clear();
    this.candidates.tokens.clear();
    this.statesProcessed = 0;
    this.precedenceStack = [];
    this.tokenStartIndex = context?.start ? context.start.tokenIndex : 0;
    const tokenStream = this.parser.tokenStream;
    this.tokens = [];
    let offset = this.tokenStartIndex;
    while (true) {
      const token = tokenStream.get(offset++);
      if (!token) {
        break;
      }
      if (token.channel === Token.DEFAULT_CHANNEL) {
        this.tokens.push(token);
        if (token.tokenIndex >= caretTokenIndex || token.type === Token.EOF) {
          break;
        }
      }
      if (token.type === Token.EOF) {
        break;
      }
    }
    const callStack = [];
    const startRule = context ? context.ruleIndex : 0;
    this.processRule(this.atn.ruleToStartState[startRule], 0, callStack, 0, 0);
    if (this.showResult) {
      console.log(`States processed: ${this.statesProcessed}`);
      console.log("\n\nCollected rules:\n");
      for (const rule of this.candidates.rules) {
        let path = "";
        for (const token of rule[1].ruleList) {
          path += this.ruleNames[token] + " ";
        }
        console.log(this.ruleNames[rule[0]] + ", path: ", path);
      }
      const sortedTokens = /* @__PURE__ */ new Set();
      for (const token of this.candidates.tokens) {
        let value = this.vocabulary.getDisplayName(token[0]) ?? "";
        for (const following of token[1]) {
          value += " " + this.vocabulary.getDisplayName(following);
        }
        sortedTokens.add(value);
      }
      console.log("\n\nCollected tokens:\n");
      for (const symbol of sortedTokens) {
        console.log(symbol);
      }
      console.log("\n\n");
    }
    return this.candidates;
  }
  /**
   * Checks if the predicate associated with the given transition evaluates to true.
   *
   * @param transition The transition to check.
   * @returns the evaluation result of the predicate.
   */
  checkPredicate(transition) {
    return transition.getPredicate().evaluate(this.parser, ParserRuleContext.EMPTY);
  }
  /**
   * Walks the rule chain upwards or downwards (depending on translateRulesTopDown) to see if that matches any of the
   * preferred rules. If found, that rule is added to the collection candidates and true is returned.
   *
   * @param ruleWithStartTokenList The list to convert.
   * @returns true if any of the stack entries was converted.
   */
  translateStackToRuleIndex(ruleWithStartTokenList) {
    if (this.preferredRules.size === 0) {
      return false;
    }
    if (this.translateRulesTopDown) {
      for (let i = ruleWithStartTokenList.length - 1; i >= 0; i--) {
        if (this.translateToRuleIndex(i, ruleWithStartTokenList)) {
          return true;
        }
      }
    } else {
      for (let i = 0; i < ruleWithStartTokenList.length; i++) {
        if (this.translateToRuleIndex(i, ruleWithStartTokenList)) {
          return true;
        }
      }
    }
    return false;
  }
  /**
   * Given the index of a rule from a rule chain, check if that matches any of the preferred rules. If it matches,
   * that rule is added to the collection candidates and true is returned.
   *
   * @param i The rule index.
   * @param ruleWithStartTokenList The list to check.
   * @returns true if the specified rule is in the list of preferred rules.
   */
  translateToRuleIndex(i, ruleWithStartTokenList) {
    const { ruleIndex, startTokenIndex } = ruleWithStartTokenList[i];
    if (this.preferredRules.has(ruleIndex)) {
      const path = ruleWithStartTokenList.slice(0, i).map(({ ruleIndex: candidate }) => {
        return candidate;
      });
      let addNew = true;
      for (const rule of this.candidates.rules) {
        if (rule[0] !== ruleIndex || rule[1].ruleList.length !== path.length) {
          continue;
        }
        if (path.every((v, j) => {
          return v === rule[1].ruleList[j];
        })) {
          addNew = false;
          break;
        }
      }
      if (addNew) {
        this.candidates.rules.set(ruleIndex, {
          startTokenIndex,
          ruleList: path
        });
        if (this.showDebugOutput) {
          console.log("=====> collected: ", this.ruleNames[ruleIndex]);
        }
      }
      return true;
    }
    return false;
  }
  /**
   * This method follows the given transition and collects all symbols within the same rule that directly follow it
   * without intermediate transitions to other rules and only if there is a single symbol for a transition.
   *
   * @param transition The transition from which to start.
   * @returns A list of toke types.
   */
  getFollowingTokens(transition) {
    const result = [];
    const pipeline = [transition.target];
    while (pipeline.length > 0) {
      const state = pipeline.pop();
      if (state) {
        state.transitions.forEach((outgoing) => {
          if (outgoing.serializationType === TransitionType.ATOM) {
            if (!outgoing.isEpsilon) {
              const list = outgoing.label.toArray();
              if (list.length === 1 && !this.ignoredTokens.has(list[0])) {
                result.push(list[0]);
                pipeline.push(outgoing.target);
              }
            } else {
              pipeline.push(outgoing.target);
            }
          }
        });
      }
    }
    return result;
  }
  /**
   * Entry point for the recursive follow set collection function.
   *
   * @param start Start state.
   * @param stop Stop state.
   * @returns Follow sets.
   */
  determineFollowSets(start, stop) {
    const sets = [];
    const stateStack = [];
    const ruleStack = [];
    const isExhaustive = this.collectFollowSets(start, stop, sets, stateStack, ruleStack);
    const combined = new IntervalSet();
    for (const set of sets) {
      combined.addSet(set.intervals);
    }
    return { sets, isExhaustive, combined };
  }
  /**
   * Collects possible tokens which could be matched following the given ATN state. This is essentially the same
   * algorithm as used in the LL1Analyzer class, but here we consider predicates also and use no parser rule context.
   *
   * @param s The state to continue from.
   * @param stopState The state which ends the collection routine.
   * @param followSets A pass through parameter to add found sets to.
   * @param stateStack A stack to avoid endless recursions.
   * @param ruleStack The current rule stack.
   * @returns true if the follow sets is exhaustive, i.e. we terminated before the rule end was reached, so no
   * subsequent rules could add tokens
   */
  collectFollowSets(s, stopState, followSets, stateStack, ruleStack) {
    if (stateStack.find((x) => {
      return x === s;
    })) {
      return true;
    }
    stateStack.push(s);
    if (s === stopState || s.stateType === ATNStateType.RULE_STOP) {
      stateStack.pop();
      return false;
    }
    let isExhaustive = true;
    for (const transition of s.transitions) {
      if (transition.serializationType === TransitionType.RULE) {
        const ruleTransition = transition;
        if (ruleStack.indexOf(ruleTransition.target.ruleIndex) !== -1) {
          continue;
        }
        ruleStack.push(ruleTransition.target.ruleIndex);
        const ruleFollowSetsIsExhaustive = this.collectFollowSets(
          transition.target,
          stopState,
          followSets,
          stateStack,
          ruleStack
        );
        ruleStack.pop();
        if (!ruleFollowSetsIsExhaustive) {
          const nextStateFollowSetsIsExhaustive = this.collectFollowSets(
            ruleTransition.followState,
            stopState,
            followSets,
            stateStack,
            ruleStack
          );
          isExhaustive &&= nextStateFollowSetsIsExhaustive;
        }
      } else if (transition.serializationType === TransitionType.PREDICATE) {
        if (this.checkPredicate(transition)) {
          const nextStateFollowSetsIsExhaustive = this.collectFollowSets(
            transition.target,
            stopState,
            followSets,
            stateStack,
            ruleStack
          );
          isExhaustive &&= nextStateFollowSetsIsExhaustive;
        }
      } else if (transition.isEpsilon) {
        const nextStateFollowSetsIsExhaustive = this.collectFollowSets(
          transition.target,
          stopState,
          followSets,
          stateStack,
          ruleStack
        );
        isExhaustive &&= nextStateFollowSetsIsExhaustive;
      } else if (transition.serializationType === TransitionType.WILDCARD) {
        const set = new FollowSetWithPath();
        set.intervals = IntervalSet.of(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType);
        set.path = ruleStack.slice();
        followSets.push(set);
      } else {
        let label = transition.label;
        if (label && label.length > 0) {
          if (transition.serializationType === TransitionType.NOT_SET) {
            label = label.complement(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType);
          }
          const set = new FollowSetWithPath();
          set.intervals = label ?? new IntervalSet();
          set.path = ruleStack.slice();
          set.following = this.getFollowingTokens(transition);
          followSets.push(set);
        }
      }
    }
    stateStack.pop();
    return isExhaustive;
  }
  /**
   * Walks the ATN for a single rule only. It returns the token stream position for each path that could be matched
   * in this rule.
   * The result can be empty in case we hit only non-epsilon transitions that didn't match the current input or if we
   * hit the caret position.
   *
   * @param startState The start state.
   * @param tokenListIndex The token index we are currently at.
   * @param callStack The stack that indicates where in the ATN we are currently.
   * @param precedence The current precedence level.
   * @param indentation A value to determine the current indentation when doing debug prints.
   * @returns the set of token stream indexes (which depend on the ways that had to be taken).
   */
  processRule(startState, tokenListIndex, callStack, precedence, indentation) {
    let positionMap = this.shortcutMap.get(startState.ruleIndex);
    if (!positionMap) {
      positionMap = /* @__PURE__ */ new Map();
      this.shortcutMap.set(startState.ruleIndex, positionMap);
    } else {
      if (positionMap.has(tokenListIndex)) {
        if (this.showDebugOutput) {
          console.log("=====> shortcut");
        }
        return positionMap.get(tokenListIndex);
      }
    }
    const result = /* @__PURE__ */ new Set();
    let setsPerState = _CodeCompletionCore.followSetsByATN.get(this.parser.constructor.name);
    if (!setsPerState) {
      setsPerState = /* @__PURE__ */ new Map();
      _CodeCompletionCore.followSetsByATN.set(this.parser.constructor.name, setsPerState);
    }
    let followSets = setsPerState.get(startState.stateNumber);
    if (!followSets) {
      const stop = this.atn.ruleToStopState[startState.ruleIndex];
      followSets = this.determineFollowSets(startState, stop);
      setsPerState.set(startState.stateNumber, followSets);
    }
    const startTokenIndex = this.tokens[tokenListIndex].tokenIndex;
    callStack.push({
      startTokenIndex,
      ruleIndex: startState.ruleIndex
    });
    if (tokenListIndex >= this.tokens.length - 1) {
      if (this.preferredRules.has(startState.ruleIndex)) {
        this.translateStackToRuleIndex(callStack);
      } else {
        for (const set of followSets.sets) {
          const fullPath = callStack.slice();
          const followSetPath = set.path.map((path) => {
            return {
              startTokenIndex,
              ruleIndex: path
            };
          });
          fullPath.push(...followSetPath);
          if (!this.translateStackToRuleIndex(fullPath)) {
            for (const symbol of set.intervals.toArray()) {
              if (!this.ignoredTokens.has(symbol)) {
                if (this.showDebugOutput) {
                  console.log("=====> collected: ", this.vocabulary.getDisplayName(symbol));
                }
                if (!this.candidates.tokens.has(symbol)) {
                  this.candidates.tokens.set(symbol, set.following);
                } else {
                  if (this.candidates.tokens.get(symbol) !== set.following) {
                    this.candidates.tokens.set(symbol, []);
                  }
                }
              }
            }
          }
        }
      }
      if (!followSets.isExhaustive) {
        result.add(tokenListIndex);
      }
      callStack.pop();
      return result;
    } else {
      const currentSymbol = this.tokens[tokenListIndex].type;
      if (followSets.isExhaustive && !followSets.combined.contains(currentSymbol)) {
        callStack.pop();
        return result;
      }
    }
    if (startState.isPrecedenceRule) {
      this.precedenceStack.push(precedence);
    }
    const statePipeline = [];
    let currentEntry;
    statePipeline.push({ state: startState, tokenListIndex });
    while (statePipeline.length > 0) {
      currentEntry = statePipeline.pop();
      ++this.statesProcessed;
      const currentSymbol = this.tokens[currentEntry.tokenListIndex].type;
      const atCaret = currentEntry.tokenListIndex >= this.tokens.length - 1;
      if (this.showDebugOutput) {
        this.printDescription(
          indentation,
          currentEntry.state,
          this.generateBaseDescription(currentEntry.state),
          currentEntry.tokenListIndex
        );
        if (this.showRuleStack) {
          this.printRuleState(callStack);
        }
      }
      if (currentEntry.state.stateType === ATNStateType.RULE_STOP) {
        result.add(currentEntry.tokenListIndex);
        continue;
      }
      const transitions = currentEntry.state.transitions;
      for (const transition of transitions) {
        switch (transition.serializationType) {
          case TransitionType.RULE: {
            const ruleTransition = transition;
            const endStatus = this.processRule(
              transition.target,
              currentEntry.tokenListIndex,
              callStack,
              ruleTransition.precedence,
              indentation + 1
            );
            for (const position of endStatus) {
              statePipeline.push({
                state: transition.followState,
                tokenListIndex: position
              });
            }
            break;
          }
          case TransitionType.PREDICATE: {
            if (this.checkPredicate(transition)) {
              statePipeline.push({
                state: transition.target,
                tokenListIndex: currentEntry.tokenListIndex
              });
            }
            break;
          }
          case TransitionType.PRECEDENCE: {
            const predTransition = transition;
            if (predTransition.precedence >= this.precedenceStack[this.precedenceStack.length - 1]) {
              statePipeline.push({
                state: transition.target,
                tokenListIndex: currentEntry.tokenListIndex
              });
            }
            break;
          }
          case TransitionType.WILDCARD: {
            if (atCaret) {
              if (!this.translateStackToRuleIndex(callStack)) {
                for (const token of IntervalSet.of(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType).toArray()) {
                  if (!this.ignoredTokens.has(token)) {
                    this.candidates.tokens.set(token, []);
                  }
                }
              }
            } else {
              statePipeline.push({
                state: transition.target,
                tokenListIndex: currentEntry.tokenListIndex + 1
              });
            }
            break;
          }
          default: {
            if (transition.isEpsilon) {
              statePipeline.push({
                state: transition.target,
                tokenListIndex: currentEntry.tokenListIndex
              });
              continue;
            }
            let set = transition.label;
            if (set && set.length > 0) {
              if (transition.serializationType === TransitionType.NOT_SET) {
                set = set.complement(Token.MIN_USER_TOKEN_TYPE, this.atn.maxTokenType);
              }
              if (atCaret) {
                if (!this.translateStackToRuleIndex(callStack)) {
                  const list = set.toArray();
                  const hasTokenSequence = list.length === 1;
                  for (const symbol of list) {
                    if (!this.ignoredTokens.has(symbol)) {
                      if (this.showDebugOutput) {
                        console.log(
                          "=====> collected: ",
                          this.vocabulary.getDisplayName(symbol)
                        );
                      }
                      const followingTokens = hasTokenSequence ? this.getFollowingTokens(transition) : [];
                      if (!this.candidates.tokens.has(symbol)) {
                        this.candidates.tokens.set(symbol, followingTokens);
                      } else {
                        this.candidates.tokens.set(
                          symbol,
                          longestCommonPrefix(
                            followingTokens,
                            this.candidates.tokens.get(symbol)
                          )
                        );
                      }
                    }
                  }
                }
              } else {
                if (set.contains(currentSymbol)) {
                  if (this.showDebugOutput) {
                    console.log("=====> consumed: ", this.vocabulary.getDisplayName(currentSymbol));
                  }
                  statePipeline.push({
                    state: transition.target,
                    tokenListIndex: currentEntry.tokenListIndex + 1
                  });
                }
              }
            }
          }
        }
      }
    }
    callStack.pop();
    if (startState.isPrecedenceRule) {
      this.precedenceStack.pop();
    }
    positionMap.set(tokenListIndex, result);
    return result;
  }
  generateBaseDescription(state) {
    const stateValue = state.stateNumber === ATNState.INVALID_STATE_NUMBER ? "Invalid" : state.stateNumber;
    return `[${stateValue} ${_CodeCompletionCore.atnStateTypeMap[state.stateType]}] in ${this.ruleNames[state.ruleIndex]}`;
  }
  printDescription(indentation, state, baseDescription, tokenIndex) {
    const indent = "  ".repeat(indentation);
    let output = indent;
    let transitionDescription = "";
    if (this.debugOutputWithTransitions) {
      for (const transition of state.transitions) {
        let labels = "";
        const symbols = transition.label ? transition.label.toArray() : [];
        if (symbols.length > 2) {
          labels = this.vocabulary.getDisplayName(symbols[0]) + " .. " + this.vocabulary.getDisplayName(symbols[symbols.length - 1]);
        } else {
          for (const symbol of symbols) {
            if (labels.length > 0) {
              labels += ", ";
            }
            labels += this.vocabulary.getDisplayName(symbol);
          }
        }
        if (labels.length === 0) {
          labels = "\u03B5";
        }
        transitionDescription += `
${indent}	(${labels}) [${transition.target.stateNumber} ${_CodeCompletionCore.atnStateTypeMap[transition.target.stateType]}] in ${this.ruleNames[transition.target.ruleIndex]}`;
      }
    }
    if (tokenIndex >= this.tokens.length - 1) {
      output += `<<${this.tokenStartIndex + tokenIndex}>> `;
    } else {
      output += `<${this.tokenStartIndex + tokenIndex}> `;
    }
    console.log(output + "Current state: " + baseDescription + transitionDescription);
  }
  printRuleState(stack) {
    if (stack.length === 0) {
      console.log("<empty stack>");
      return;
    }
    for (const rule of stack) {
      console.log(this.ruleNames[rule.ruleIndex]);
    }
  }
};

// src/SymbolTable.ts
var SymbolTable = class extends ScopedSymbol {
  constructor(name, options) {
    super(name);
    this.options = options;
  }
  static {
    __name(this, "SymbolTable");
  }
  /**  Other symbol information available to this instance. */
  dependencies = /* @__PURE__ */ new Set();
  get info() {
    return {
      dependencyCount: this.dependencies.size,
      symbolCount: this.children.length
    };
  }
  clear() {
    super.clear();
    this.dependencies.clear();
  }
  addDependencies(...tables) {
    tables.forEach((value) => {
      this.dependencies.add(value);
    });
  }
  removeDependency(table) {
    if (this.dependencies.has(table)) {
      this.dependencies.delete(table);
    }
  }
  addNewSymbolOfType(t, parent, ...args) {
    const result = new t(...args);
    if (!parent || parent === this) {
      this.addSymbol(result);
    } else {
      parent.addSymbol(result);
    }
    return result;
  }
  async addNewNamespaceFromPath(parent, path, delimiter = ".") {
    const parts = path.split(delimiter);
    let i = 0;
    let currentParent = parent === void 0 ? this : parent;
    while (i < parts.length - 1) {
      let namespace = await currentParent.resolve(parts[i], true);
      if (namespace === void 0) {
        namespace = this.addNewSymbolOfType(NamespaceSymbol, currentParent, parts[i]);
      }
      currentParent = namespace;
      ++i;
    }
    return this.addNewSymbolOfType(NamespaceSymbol, currentParent, parts[parts.length - 1]);
  }
  addNewNamespaceFromPathSync(parent, path, delimiter = ".") {
    const parts = path.split(delimiter);
    let i = 0;
    let currentParent = parent === void 0 ? this : parent;
    while (i < parts.length - 1) {
      let namespace = currentParent.resolveSync(parts[i], true);
      if (namespace === void 0) {
        namespace = this.addNewSymbolOfType(NamespaceSymbol, currentParent, parts[i]);
      }
      currentParent = namespace;
      ++i;
    }
    return this.addNewSymbolOfType(NamespaceSymbol, currentParent, parts[parts.length - 1]);
  }
  async getAllSymbols(t, localOnly = false) {
    const result = await super.getAllSymbols(t, localOnly);
    if (!localOnly) {
      const dependencyResults = await Promise.all([...this.dependencies].map((dependency) => {
        return dependency.getAllSymbols(t, localOnly);
      }));
      dependencyResults.forEach((value) => {
        result.push(...value);
      });
    }
    return result;
  }
  getAllSymbolsSync(t, localOnly = false) {
    const result = super.getAllSymbolsSync(t, localOnly);
    if (!localOnly) {
      this.dependencies.forEach((dependency) => {
        result.push(...dependency.getAllSymbolsSync(t, localOnly));
      });
    }
    return result;
  }
  async symbolWithContext(context) {
    const findRecursive = /* @__PURE__ */ __name((symbol) => {
      if (symbol.context === context) {
        return symbol;
      }
      if (symbol instanceof ScopedSymbol) {
        for (const child of symbol.children) {
          const result = findRecursive(child);
          if (result) {
            return result;
          }
        }
      }
      return void 0;
    }, "findRecursive");
    let symbols = await this.getAllSymbols(BaseSymbol);
    for (const symbol of symbols) {
      const result = findRecursive(symbol);
      if (result) {
        return result;
      }
    }
    for (const dependency of this.dependencies) {
      symbols = await dependency.getAllSymbols(BaseSymbol);
      for (const symbol of symbols) {
        const result = findRecursive(symbol);
        if (result) {
          return result;
        }
      }
    }
    return void 0;
  }
  symbolWithContextSync(context) {
    const findRecursive = /* @__PURE__ */ __name((symbol) => {
      if (symbol.context === context) {
        return symbol;
      }
      if (symbol instanceof ScopedSymbol) {
        for (const child of symbol.children) {
          const result = findRecursive(child);
          if (result) {
            return result;
          }
        }
      }
      return void 0;
    }, "findRecursive");
    let symbols = this.getAllSymbolsSync(BaseSymbol);
    for (const symbol of symbols) {
      const result = findRecursive(symbol);
      if (result) {
        return result;
      }
    }
    for (const dependency of this.dependencies) {
      symbols = dependency.getAllSymbolsSync(BaseSymbol);
      for (const symbol of symbols) {
        const result = findRecursive(symbol);
        if (result) {
          return result;
        }
      }
    }
    return void 0;
  }
  async resolve(name, localOnly = false) {
    let result = await super.resolve(name, localOnly);
    if (!result && !localOnly) {
      for (const dependency of this.dependencies) {
        result = await dependency.resolve(name, false);
        if (result) {
          return result;
        }
      }
    }
    return result;
  }
  resolveSync(name, localOnly = false) {
    let result = super.resolveSync(name, localOnly);
    if (!result && !localOnly) {
      for (const dependency of this.dependencies) {
        result = dependency.resolveSync(name, false);
        if (result) {
          return result;
        }
      }
    }
    return result;
  }
};
export {
  ArrayType,
  BaseSymbol,
  BlockSymbol,
  CandidatesCollection,
  ClassSymbol,
  CodeCompletionCore,
  DuplicateSymbolError,
  FieldSymbol,
  FundamentalType,
  InterfaceSymbol,
  LiteralSymbol,
  MemberVisibility,
  MethodFlags,
  MethodSymbol,
  Modifier,
  NamespaceSymbol,
  ParameterSymbol,
  ReferenceKind,
  RoutineSymbol,
  ScopedSymbol,
  SymbolTable,
  TypeAlias,
  TypeKind,
  TypedSymbol,
  VariableSymbol
};
