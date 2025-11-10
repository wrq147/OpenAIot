export class SimpleStack {
    constructor() {
        this.stack = [];
    }
    push(item) {
        this.stack.push(item);
    }
    pop() {
        return this.stack.pop();
    }
    peek() {
        return this.stack[this.stack.length - 1];
    }
    clear() {
        this.stack = [];
    }
    size() {
        return this.stack.length;
    }
    isEmpty() {
        return this.stack.length === 0;
    }
}
