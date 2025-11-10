var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
const plugin = {
    name: '__ctx-drawRoundImage__',
    mounted: ({ ctx }) => {
        ctx.drawRoundImage = (url, x, y, w, h, r = 15) => __awaiter(void 0, void 0, void 0, function* () {
            var _a;
            ctx.save();
            (_a = ctx.setFillStyle) === null || _a === void 0 ? void 0 : _a.call(ctx, 'transparent');
            ctx.fillStyle = 'transparent';
            ctx.fillRoundRect(x, y, w, h, r);
            ctx.clip();
            const result = yield ctx.drawImage(url, x, y, w, h);
            ctx.restore();
            return result;
        });
    }
};
export default () => plugin;
//# sourceMappingURL=index.js.map