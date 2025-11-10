var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { downloadImgUrl } from './utils';
//缓存
var dircache={};
var imgcache={};
const plugin = {
    name: '__ctx-drawImage__',
    mounted: ({ ctx, $options, canvas }) => {
        ctx.drawImageProto = ctx.drawImage;
        ctx.drawImage = (url, sx, sy, sh, sw, dx, dy, dh, dw) => __awaiter(void 0, void 0, void 0, function* () {
			let path = null;
			if(dircache.hasOwnProperty(url)){
				path=dircache[url];
			}
			else{
				path = yield downloadImgUrl(url);
				dircache[url]=path;
			}
            // // 下载路径
            // const path = yield downloadImgUrl(url);
            // 标记当前绘画存在图片绘制
            let result = false;
            // 基本绘制方法, 如果是 fit 方式, 则传入所有参数, 不然则只传入四个参数
            const baseDrawImage = (imageResource) => {
                const isFit = typeof dx === 'number' && typeof dw === 'number';
                if (isFit) {
                    ctx.drawImageProto(imageResource, sx, sy, sh, sw, dx, dy, dh, dw);
                }
                else {
                    ctx.drawImageProto(imageResource, sx, sy, sh, sw);
                }
            };
            // 如果是 context 绘制方式, 则直接绘制
            if ($options.type === 'context') {
                baseDrawImage(path);
                result = true;
            }
            // 如果是 type2d 绘制方式, 则等待图片绘制完毕
            if ($options.type === '2d') {
                result = yield new Promise((resolve) => {
					if(imgcache.hasOwnProperty(path)){
						const image=imgcache[path];
						baseDrawImage(image);
						resolve(true);
					}
					else{
						const image = canvas.createImage();
						image.src = path;
						image.onload = () => {
								imgcache[path]=image;
						    baseDrawImage(image);
						    resolve(true);
						};
						image.onerror = () => resolve(false);
					}
                });
            }
            return result;
        });
    }
};
export default () => plugin;
//# sourceMappingURL=index.js.map