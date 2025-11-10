import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLRenderBuffer';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLRenderbuffer {
    constructor(id) {
        this.className = name;
        this.id = id;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLRenderbuffer.uuid = uuid;
//# sourceMappingURL=Renderbuffer.js.map