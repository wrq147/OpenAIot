import GLenum from './GLenum';
import ActiveInfo from './ActiveInfo';
import Buffer from './Buffer';
import Framebuffer from './Framebuffer';
import Renderbuffer from './Renderbuffer';
import Texture from './Texture';
import Program from './Program';
import Shader from './Shader';
import ShaderPrecisionFormat from './ShaderPrecisionFormat';
import UniformLocation from './UniformLocation';
import GLmethod from './GLmethod';
const processArray = (array, checkArrayType = false) => {
    function joinArray(arr, sep) {
        let res = '';
        for (let i = 0; i < arr.length; i++) {
            if (i !== 0) {
                res += sep;
            }
            res += arr[i];
        }
        return res;
    }
    let type = 'Float32Array';
    if (checkArrayType) {
        if (array instanceof Uint8Array) {
            type = 'Uint8Array';
        }
        else if (array instanceof Uint16Array) {
            type = 'Uint16Array';
        }
        else if (array instanceof Uint32Array) {
            type = 'Uint32Array';
        }
        else if (array instanceof Float32Array) {
            type = 'Float32Array';
        }
        else {
            throw new Error('Check array type failed. Array type is ' + typeof array);
        }
    }
    const ArrayTypes = {
        Uint8Array: 1,
        Uint16Array: 2,
        Uint32Array: 4,
        Float32Array: 14
    };
    return ArrayTypes[type] + ',' + btoa(joinArray(array, ','));
};
export default class WebGLRenderingContext {
    constructor(canvas, type, attrs) {
        // static GBridge = null;
        this.className = 'WebGLRenderingContext';
        this.activeTexture = function (textureUnit) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.activeTexture + ',' + textureUnit, true);
        };
        this.attachShader = function (progarm, shader) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.attachShader + ',' + progarm.id + ',' + shader.id, true);
        };
        this.bindAttribLocation = function (program, index, name) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bindAttribLocation + ',' + program.id + ',' + index + ',' + name, true);
        };
        this.bindBuffer = function (target, buffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bindBuffer + ',' + target + ',' + (buffer ? buffer.id : 0), true);
        };
        this.bindFramebuffer = function (target, framebuffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bindFramebuffer + ',' + target + ',' + (framebuffer ? framebuffer.id : 0), true);
        };
        this.bindRenderbuffer = function (target, renderBuffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bindRenderbuffer + ',' + target + ',' + (renderBuffer ? renderBuffer.id : 0), true);
        };
        this.bindTexture = function (target, texture) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bindTexture + ',' + target + ',' + (texture ? texture.id : 0), true);
        };
        this.blendColor = function (r, g, b, a) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.blendColor + ',' + target + ',' + r + ',' + g + ',' + b + ',' + a, true);
        };
        this.blendEquation = function (mode) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.blendEquation + ',' + mode, true);
        };
        this.blendEquationSeparate = function (modeRGB, modeAlpha) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.blendEquationSeparate + ',' + modeRGB + ',' + modeAlpha, true);
        };
        this.blendFunc = function (sfactor, dfactor) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.blendFunc + ',' + sfactor + ',' + dfactor, true);
        };
        this.blendFuncSeparate = function (srcRGB, dstRGB, srcAlpha, dstAlpha) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.blendFuncSeparate + ',' + srcRGB + ',' + dstRGB + ',' + srcAlpha + ',' + dstAlpha, true);
        };
        this.bufferData = function (target, data, usage) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bufferData + ',' + target + ',' + processArray(data, true) + ',' + usage, true);
        };
        this.bufferSubData = function (target, offset, data) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.bufferSubData + ',' + target + ',' + offset + ',' + processArray(data, true), true);
        };
        this.checkFramebufferStatus = function (target) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.checkFramebufferStatus + ',' + target);
            return Number(result);
        };
        this.clear = function (mask) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.clear + ',' + mask);
            this._canvas._needRender = true;
        };
        this.clearColor = function (r, g, b, a) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.clearColor + ',' + r + ',' + g + ',' + b, true);
        };
        this.clearDepth = function (depth) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.clearDepth + ',' + depth, true);
        };
        this.clearStencil = function (s) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.clearStencil + ',' + s);
        };
        this.colorMask = function (r, g, b, a) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.colorMask + ',' + r + ',' + g + ',' + b + ',' + a);
        };
        this.compileShader = function (shader) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.compileShader + ',' + shader.id, true);
        };
        this.compressedTexImage2D = function (target, level, internalformat, width, height, border, pixels) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.compressedTexImage2D + ',' + target + ',' + level + ',' + internalformat + ',' +
                width + ',' + height + ',' + border + ',' + processArray(pixels), true);
        };
        this.compressedTexSubImage2D = function (target, level, xoffset, yoffset, width, height, format, pixels) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.compressedTexSubImage2D + ',' + target + ',' + level + ',' + xoffset + ',' + yoffset + ',' +
                width + ',' + height + ',' + format + ',' + processArray(pixels), true);
        };
        this.copyTexImage2D = function (target, level, internalformat, x, y, width, height, border) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.copyTexImage2D + ',' + target + ',' + level + ',' + internalformat + ',' + x + ',' + y + ',' +
                width + ',' + height + ',' + border, true);
        };
        this.copyTexSubImage2D = function (target, level, xoffset, yoffset, x, y, width, height) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.copyTexSubImage2D + ',' + target + ',' + level + ',' + xoffset + ',' + yoffset + ',' + x + ',' + y + ',' +
                width + ',' + height);
        };
        this.createBuffer = function () {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createBuffer + '');
            const buffer = new Buffer(result);
            this._map.set(buffer.uuid(), buffer);
            return buffer;
        };
        this.createFramebuffer = function () {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createFramebuffer + '');
            const framebuffer = new Framebuffer(result);
            this._map.set(framebuffer.uuid(), framebuffer);
            return framebuffer;
        };
        this.createProgram = function () {
            const id = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createProgram + '');
            const program = new Program(id);
            this._map.set(program.uuid(), program);
            return program;
        };
        this.createRenderbuffer = function () {
            const id = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createRenderbuffer + '');
            const renderBuffer = new Renderbuffer(id);
            this._map.set(renderBuffer.uuid(), renderBuffer);
            return renderBuffer;
        };
        this.createShader = function (type) {
            const id = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createShader + ',' + type);
            const shader = new Shader(id, type);
            this._map.set(shader.uuid(), shader);
            return shader;
        };
        this.createTexture = function () {
            const id = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.createTexture + '');
            const texture = new Texture(id);
            this._map.set(texture.uuid(), texture);
            return texture;
        };
        this.cullFace = function (mode) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.cullFace + ',' + mode, true);
        };
        this.deleteBuffer = function (buffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteBuffer + ',' + buffer.id, true);
        };
        this.deleteFramebuffer = function (framebuffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteFramebuffer + ',' + framebuffer.id, true);
        };
        this.deleteProgram = function (program) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteProgram + ',' + program.id, true);
        };
        this.deleteRenderbuffer = function (renderbuffer) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteRenderbuffer + ',' + renderbuffer.id, true);
        };
        this.deleteShader = function (shader) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteShader + ',' + shader.id, true);
        };
        this.deleteTexture = function (texture) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.deleteTexture + ',' + texture.id, true);
        };
        this.depthFunc = function (func) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.depthFunc + ',' + func);
        };
        this.depthMask = function (flag) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.depthMask + ',' + Number(flag), true);
        };
        this.depthRange = function (zNear, zFar) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.depthRange + ',' + zNear + ',' + zFar, true);
        };
        this.detachShader = function (program, shader) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.detachShader + ',' + program.id + ',' + shader.id, true);
        };
        this.disable = function (cap) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.disable + ',' + cap, true);
        };
        this.disableVertexAttribArray = function (index) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.disableVertexAttribArray + ',' + index, true);
        };
        this.drawArrays = function (mode, first, count) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.drawArrays + ',' + mode + ',' + first + ',' + count);
            this._canvas._needRender = true;
        };
        this.drawElements = function (mode, count, type, offset) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.drawElements + ',' + mode + ',' + count + ',' + type + ',' + offset + ';');
            this._canvas._needRender = true;
        };
        this.enable = function (cap) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.enable + ',' + cap, true);
        };
        this.enableVertexAttribArray = function (index) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.enableVertexAttribArray + ',' + index, true);
        };
        this.flush = function () {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.flush + '');
        };
        this.framebufferRenderbuffer = function (target, attachment, textarget, texture, level) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.framebufferRenderbuffer + ',' + target + ',' + attachment + ',' + textarget + ',' + (texture ? texture.id : 0) + ',' + level, true);
        };
        this.framebufferTexture2D = function (target, attachment, textarget, texture, level) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.framebufferTexture2D + ',' + target + ',' + attachment + ',' + textarget + ',' + (texture ? texture.id : 0) + ',' + level, true);
        };
        this.frontFace = function (mode) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.frontFace + ',' + mode, true);
        };
        this.generateMipmap = function (target) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.generateMipmap + ',' + target, true);
        };
        this.getActiveAttrib = function (progarm, index) {
            const resultString = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getActiveAttrib + ',' + progarm.id + ',' + index);
            const [type, size, name] = resultString.split(',');
            return new ActiveInfo({
                type: Number(type),
                size: Number(size),
                name
            });
        };
        this.getActiveUniform = function (progarm, index) {
            const resultString = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getActiveUniform + ',' + progarm.id + ',' + index);
            const [type, size, name] = resultString.split(',');
            return new ActiveInfo({
                type: Number(type),
                size: Number(size),
                name
            });
        };
        this.getAttachedShaders = function (progarm) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getAttachedShaders + ',' + progarm.id);
            const [type, ...ids] = result;
            return ids.map(id => this._map.get(Shader.uuid(id)));
        };
        this.getAttribLocation = function (progarm, name) {
            return WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getAttribLocation + ',' + progarm.id + ',' + name);
        };
        this.getBufferParameter = function (target, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getBufferParameter + ',' + target + ',' + pname);
            const [type, res] = getBufferParameter;
            return res;
        };
        this.getError = function () {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getError + '');
            return result;
        };
        this.getExtension = function (name) {
            return null;
        };
        this.getFramebufferAttachmentParameter = function (target, attachment, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getFramebufferAttachmentParameter + ',' + target + ',' + attachment + ',' + pname);
            switch (pname) {
                case GLenum.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME:
                    return this._map.get(Renderbuffer.uuid(result)) || this._map.get(Texture.uuid(result)) || null;
                default:
                    return result;
            }
        };
        this.getParameter = function (pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getParameter + ',' + pname);
            switch (pname) {
                case GLenum.VERSION:
                    return this._version;
                case GLenum.ARRAY_BUFFER_BINDING: // buffer
                case GLenum.ELEMENT_ARRAY_BUFFER_BINDING: // buffer
                    return this._map.get(Buffer.uuid(result)) || null;
                case GLenum.CURRENT_PROGRAM: // program
                    return this._map.get(Program.uuid(result)) || null;
                case GLenum.FRAMEBUFFER_BINDING: // framebuffer
                    return this._map.get(Framebuffer.uuid(result)) || null;
                case GLenum.RENDERBUFFER_BINDING: // renderbuffer
                    return this._map.get(Renderbuffer.uuid(result)) || null;
                case GLenum.TEXTURE_BINDING_2D: // texture
                case GLenum.TEXTURE_BINDING_CUBE_MAP: // texture
                    return this._map.get(Texture.uuid(result)) || null;
                case GLenum.ALIASED_LINE_WIDTH_RANGE: // Float32Array
                case GLenum.ALIASED_POINT_SIZE_RANGE: // Float32Array
                case GLenum.BLEND_COLOR: // Float32Array
                case GLenum.COLOR_CLEAR_VALUE: // Float32Array
                case GLenum.DEPTH_RANGE: // Float32Array
                case GLenum.MAX_VIEWPORT_DIMS: // Int32Array
                case GLenum.SCISSOR_BOX: // Int32Array
                case GLenum.VIEWPORT: // Int32Array            
                case GLenum.COMPRESSED_TEXTURE_FORMATS: // Uint32Array
                default:
                    const [type, ...res] = result.split(',');
                    if (res.length === 1) {
                        return Number(res[0]);
                    }
                    else {
                        return res.map(Number);
                    }
            }
        };
        this.getProgramInfoLog = function (progarm) {
            return WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getProgramInfoLog + ',' + progarm.id);
        };
        this.getProgramParameter = function (program, pname) {
            const res = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getProgramParameter + ',' + program.id + ',' + pname);
            const [type, result] = res.split(',').map(i => parseInt(i));
            if (type === 1) {
                return Boolean(result);
            }
            else if (type === 2) {
                return result;
            }
            else {
                throw new Error('Unrecongized program paramater ' + res + ', type: ' + typeof res);
            }
        };
        this.getRenderbufferParameter = function (target, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getRenderbufferParameter + ',' + target + ',' + pname);
            return result;
        };
        this.getShaderInfoLog = function (shader) {
            return WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getShaderInfoLog + ',' + shader.id);
        };
        this.getShaderParameter = function (shader, pname) {
            return WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getShaderParameter + ',' + shader.id + ',' + pname);
        };
        this.getShaderPrecisionFormat = function (shaderType, precisionType) {
            const [rangeMin, rangeMax, precision] = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getShaderPrecisionFormat + ',' + shaderType + ',' + precisionType);
            const shaderPrecisionFormat = new ShaderPrecisionFormat({
                rangeMin: Number(rangeMin),
                rangeMax: Number(rangeMax),
                precision: Number(precision)
            });
            return shaderPrecisionFormat;
        };
        this.getShaderSource = function (shader) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getShaderSource + ',' + shader.id);
            return result;
        };
        this.getSupportedExtensions = function () {
            return Object.keys({});
        };
        this.getTexParameter = function (target, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getTexParameter + ',' + target + ',' + pname);
            return result;
        };
        this.getUniformLocation = function (program, name) {
            const id = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getUniformLocation + ',' + program.id + ',' + name);
            if (id === -1) {
                return null;
            }
            else {
                return new UniformLocation(Number(id));
            }
        };
        this.getVertexAttrib = function (index, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getVertexAttrib + ',' + index + ',' + pname);
            switch (pname) {
                case GLenum.VERTEX_ATTRIB_ARRAY_BUFFER_BINDING:
                    return this._map.get(Buffer.uuid(result)) || null;
                case GLenum.CURRENT_VERTEX_ATTRIB: // Float32Array
                default:
                    return result;
            }
        };
        this.getVertexAttribOffset = function (index, pname) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.getVertexAttribOffset + ',' + index + ',' + pname);
            return Number(result);
        };
        this.isBuffer = function (buffer) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isBuffer + ',' + buffer.id);
            return Boolean(result);
        };
        this.isContextLost = function () {
            return false;
        };
        this.isEnabled = function (cap) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isEnabled + ',' + cap);
            return Boolean(result);
        };
        this.isFramebuffer = function (framebuffer) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isFramebuffer + ',' + framebuffer.id);
            return Boolean(result);
        };
        this.isProgram = function (program) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isProgram + ',' + program.id);
            return Boolean(result);
        };
        this.isRenderbuffer = function (renderBuffer) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isRenderbuffer + ',' + renderbuffer.id);
            return Boolean(result);
        };
        this.isShader = function (shader) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isShader + ',' + shader.id);
            return Boolean(result);
        };
        this.isTexture = function (texture) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.isTexture + ',' + texture.id);
            return Boolean(result);
        };
        this.lineWidth = function (width) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.lineWidth + ',' + width, true);
        };
        this.linkProgram = function (program) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.linkProgram + ',' + program.id, true);
        };
        this.pixelStorei = function (pname, param) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.pixelStorei + ',' + pname + ',' + Number(param));
        };
        this.polygonOffset = function (factor, units) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.polygonOffset + ',' + factor + ',' + units);
        };
        this.readPixels = function (x, y, width, height, format, type, pixels) {
            const result = WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.readPixels + ',' + x + ',' + y + ',' + width + ',' + height + ',' + format + ',' + type);
            return result;
        };
        this.renderbufferStorage = function (target, internalFormat, width, height) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.renderbufferStorage + ',' + target + ',' + internalFormat + ',' + width + ',' + height, true);
        };
        this.sampleCoverage = function (value, invert) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.sampleCoverage + ',' + value + ',' + Number(invert), true);
        };
        this.scissor = function (x, y, width, height) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.scissor + ',' + x + ',' + y + ',' + width + ',' + height, true);
        };
        this.shaderSource = function (shader, source) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.shaderSource + ',' + shader.id + ',' + source);
        };
        this.stencilFunc = function (func, ref, mask) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilFunc + ',' + func + ',' + ref + ',' + mask, true);
        };
        this.stencilFuncSeparate = function (face, func, ref, mask) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilFuncSeparate + ',' + face + ',' + func + ',' + ref + ',' + mask, true);
        };
        this.stencilMask = function (mask) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilMask + ',' + mask, true);
        };
        this.stencilMaskSeparate = function (face, mask) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilMaskSeparate + ',' + face + ',' + mask, true);
        };
        this.stencilOp = function (fail, zfail, zpass) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilOp + ',' + fail + ',' + zfail + ',' + zpass);
        };
        this.stencilOpSeparate = function (face, fail, zfail, zpass) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.stencilOp + ',' + face + ',' + fail + ',' + zfail + ',' + zpass, true);
        };
        this.texImage2D = function (...args) {
            WebGLRenderingContext.GBridge.texImage2D(this._canvas.id, ...args);
        };
        this.texParameterf = function (target, pname, param) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.texParameterf + ',' + target + ',' + pname + ',' + param, true);
        };
        this.texParameteri = function (target, pname, param) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.texParameteri + ',' + target + ',' + pname + ',' + param);
        };
        this.texSubImage2D = function (...args) {
            WebGLRenderingContext.GBridge.texSubImage2D(this._canvas.id, ...args);
        };
        this.uniform1f = function (location, v0) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform1f + ',' + location.id + ',' + v0);
        };
        this.uniform1fv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform1fv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform1i = function (location, v0) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform1i + ',' + location.id + ',' + v0);
        };
        this.uniform1iv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform1iv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform2f = function (location, v0, v1) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform2f + ',' + location.id + ',' + v0 + ',' + v1, true);
        };
        this.uniform2fv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform2fv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform2i = function (location, v0, v1) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform2i + ',' + location.id + ',' + v0 + ',' + v1, true);
        };
        this.uniform2iv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform2iv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform3f = function (location, v0, v1, v2) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform3f + ',' + location.id + ',' + v0 + ',' + v1 + ',' + v2, true);
        };
        this.uniform3fv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform3fv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform3i = function (location, v0, v1, v2) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform3i + ',' + location.id + ',' + v0 + ',' + v1 + ',' + v2, true);
        };
        this.uniform3iv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform3iv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform4f = function (location, v0, v1, v2, v3) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform4f + ',' + location.id + ',' + v0 + ',' + v1 + ',' + v2 + ',' + v3, true);
        };
        this.uniform4fv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform4fv + ',' + location.id + ',' + processArray(value), true);
        };
        this.uniform4i = function (location, v0, v1, v2, v3) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform4i + ',' + location.id + ',' + v0 + ',' + v1 + ',' + v2 + ',' + v3, true);
        };
        this.uniform4iv = function (location, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniform4iv + ',' + location.id + ',' + processArray(value, true), true);
        };
        this.uniformMatrix2fv = function (location, transpose, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniformMatrix2fv + ',' + location.id + ',' + Number(transpose) + ',' + processArray(value), true);
        };
        this.uniformMatrix3fv = function (location, transpose, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniformMatrix3fv + ',' + location.id + ',' + Number(transpose) + ',' + processArray(value), true);
        };
        this.uniformMatrix4fv = function (location, transpose, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.uniformMatrix4fv + ',' + location.id + ',' + Number(transpose) + ',' + processArray(value), true);
        };
        this.useProgram = function (progarm) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.useProgram + ',' + progarm.id + '', true);
        };
        this.validateProgram = function (program) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.validateProgram + ',' + program.id, true);
        };
        this.vertexAttrib1f = function (index, v0) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib1f + ',' + index + ',' + v0, true);
        };
        this.vertexAttrib2f = function (index, v0, v1) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib2f + ',' + index + ',' + v0 + ',' + v1, true);
        };
        this.vertexAttrib3f = function (index, v0, v1, v2) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib3f + ',' + index + ',' + v0 + ',' + v1 + ',' + v2, true);
        };
        this.vertexAttrib4f = function (index, v0, v1, v2, v3) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib4f + ',' + index + ',' + v0 + ',' + v1 + ',' + v2 + ',' + v3, true);
        };
        this.vertexAttrib1fv = function (index, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib1fv + ',' + index + ',' + processArray(value), true);
        };
        this.vertexAttrib2fv = function (index, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib2fv + ',' + index + ',' + processArray(value), true);
        };
        this.vertexAttrib3fv = function (index, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib3fv + ',' + index + ',' + processArray(value), true);
        };
        this.vertexAttrib4fv = function (index, value) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttrib4fv + ',' + index + ',' + processArray(value), true);
        };
        this.vertexAttribPointer = function (index, size, type, normalized, stride, offset) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.vertexAttribPointer + ',' + index + ',' + size + ',' + type + ',' + Number(normalized) + ',' + stride + ',' + offset, true);
        };
        this.viewport = function (x, y, width, height) {
            WebGLRenderingContext.GBridge.callNative(this._canvas.id, GLmethod.viewport + ',' + x + ',' + y + ',' + width + ',' + height, true);
        };
        this._canvas = canvas;
        this._type = type;
        this._version = 'WebGL 1.0';
        this._attrs = attrs;
        this._map = new Map();
        Object.keys(GLenum)
            .forEach(name => Object.defineProperty(this, name, {
            value: GLenum[name]
        }));
    }
    get canvas() {
        return this._canvas;
    }
}
//# sourceMappingURL=RenderingContext.js.map