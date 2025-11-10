var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { concreteRect } from './object-sizing';
import { promisify } from '../../utils';
const plugin = {
    name: '__ctx-drawImageFit__',
    mounted: ({ ctx }) => {
        ctx.drawImageFit = (url, options) => __awaiter(void 0, void 0, void 0, function* () {
            var _a, _b, _c;
            const imageInfo = yield promisify(uni.getImageInfo)({ src: url });
            const style = Object.assign({ radius: 0, objectFit: 'cover', intrinsicSize: { width: (_a = imageInfo === null || imageInfo === void 0 ? void 0 : imageInfo.width) !== null && _a !== void 0 ? _a : 100, height: (_b = imageInfo === null || imageInfo === void 0 ? void 0 : imageInfo.height) !== null && _b !== void 0 ? _b : 100 }, specifiedSize: { width: 100, height: 100 }, intrinsicPosition: ['center', 'center'], specifiedPosition: [0, 0] }, options);
            // 计算图片尺寸
            const drawInfo = concreteRect(style, style.intrinsicSize, style.specifiedSize);
            // 如有圆角, 则进行裁剪
            if (style.radius > 0) {
                ctx.save();
                (_c = ctx.setFillStyle) === null || _c === void 0 ? void 0 : _c.call(ctx, 'transparent');
                ctx.fillStyle = 'transparent';
                ctx.fillRoundRect(style.specifiedPosition[0], style.specifiedPosition[1], style.specifiedSize.width, style.specifiedSize.height, style.radius);
                ctx.clip();
            }
            const result = yield ctx.drawImage(url, ...Object.values(drawInfo));
            if (style.radius > 0)
                ctx.restore();
            return result;
        });
    }
};
export default () => plugin;
//# sourceMappingURL=index.js.map