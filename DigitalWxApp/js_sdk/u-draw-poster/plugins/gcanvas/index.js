import { enable, Image, WeexBridge } from './gcanvas';
const plugin = {
    name: '__dp-gcanvas__',
    beforeMount: (dp) => {
        var _a, _b, _c, _d, _e;
        dp.$gcanvas = { enable, Image, WeexBridge };
        const canvas = dp.$gcanvas.enable((_c = (_b = (_a = dp.$options) === null || _a === void 0 ? void 0 : _a.componentThis) === null || _b === void 0 ? void 0 : _b.$refs) === null || _c === void 0 ? void 0 : _c[dp.id.replace('#', '')], { bridge: WeexBridge });
        const ctx = (_d = canvas.getContext) === null || _d === void 0 ? void 0 : _d.call(canvas, (_e = dp.$options) === null || _e === void 0 ? void 0 : _e.type);
        dp.$drawPrototype = { canvas, ctx };
    }
};
export default () => plugin;
//# sourceMappingURL=index.js.map