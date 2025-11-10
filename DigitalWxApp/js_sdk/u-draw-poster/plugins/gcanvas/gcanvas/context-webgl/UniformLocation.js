import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLUniformLocation';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLUniformLocation {
    constructor(id, type) {
        this.className = name;
        this.id = id;
        this.type = type;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLUniformLocation.uuid = uuid;
//# sourceMappingURL=UniformLocation.js.map