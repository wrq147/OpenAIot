import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLBuffer';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLBuffer {
    constructor(id) {
        this.className = name;
        this.id = id;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLBuffer.uuid = uuid;
//# sourceMappingURL=Buffer.js.map