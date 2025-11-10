import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLShader';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLShader {
    constructor(id, type) {
        this.className = name;
        this.id = id;
        this.type = type;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLShader.uuid = uuid;
//# sourceMappingURL=Shader.js.map