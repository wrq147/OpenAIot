import { getTransferedObjectUUID } from './classUtils';
const name = 'WebGLProgram';
function uuid(id) {
    return getTransferedObjectUUID(name, id);
}
export default class WebGLProgram {
    constructor(id) {
        this.className = name;
        this.id = id;
    }
    uuid() {
        return uuid(this.id);
    }
}
WebGLProgram.uuid = uuid;
//# sourceMappingURL=Program.js.map