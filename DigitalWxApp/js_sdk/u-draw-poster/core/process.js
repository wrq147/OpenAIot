var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class DrawProcess {
    constructor(dp, consola) {
        this.dp = dp;
        this.consola = consola;
        /** 进程堆栈 */
        this.stacks = [];
        /** 是否阻止运行 */
        this.prevent = false;
        this.push = (callback) => {
            const length = this.stacks.length;
            this.stacks.push(() => __awaiter(this, void 0, void 0, function* () {
                var _a, _b;
                try {
                    this.dp.ctx.save();
                    yield callback(this.dp.ctx);
                    this.dp.ctx.restore();
                    return true;
                }
                catch (error) {
                    if (!((_b = (_a = error === null || error === void 0 ? void 0 : error.message) === null || _a === void 0 ? void 0 : _a.includes) === null || _b === void 0 ? void 0 : _b.call(_a, `'nodeId' of undefined`)))
                        this.consola.error(`绘画栈(${length})，绘制错误：`, error);
                    return false;
                }
            }));
        };
        this.stop = () => {
            this.prevent = true;
        };
        this.walk = () => __awaiter(this, void 0, void 0, function* () {
            const results = [];
            for (const next of this.stacks) {
                if (this.prevent)
                    return results;
                results.push(yield next());
            }
            this.stacks = [];
            return results;
        });
    }
}
export default DrawProcess;
//# sourceMappingURL=process.js.map