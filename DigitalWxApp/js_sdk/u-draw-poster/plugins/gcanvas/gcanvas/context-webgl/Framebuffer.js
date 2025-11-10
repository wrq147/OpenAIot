import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLFrameBuffer';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLFramebuffer {
    constructor(id) {
        this.className = name;
        this.id = id;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLFramebuffer.uuid = uuid;
//# sourceMappingURL=Framebuffer.js.map