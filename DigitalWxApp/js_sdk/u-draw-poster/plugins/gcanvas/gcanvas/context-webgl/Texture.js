import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLTexture';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLTexture {
    constructor(id, type) {
        this.className = name;
        this.id = id;
        this.type = type;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLTexture.uuid = uuid;
//# sourceMappingURL=Texture.js.map