var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import { getCurrentDrawPoster, setCurrentDrawPoster } from '../helpers/internal';
import { promisify, queryFields } from '../utils';
import { Consola } from './consola';
import { Plugins } from './plugin';
import DrawProcess from './process';
export const builder = (options, wait) => {
    // 假如当前页面已存在实例, 则直接返回
    const currentDrawPoster = getCurrentDrawPoster(options.selector);
    if (currentDrawPoster)
        return currentDrawPoster;
    // #region 初始化参数定义, 初始化插件系统 / debug 系统 / 进程系统
    const dp = { $options: options };
    const ps = new Plugins(dp);
    const consola = new Consola(dp);
    const pcs = new DrawProcess(dp, consola);
    Object.defineProperty(dp, 'id', { get: () => options.selector });
    Object.defineProperty(dp, 'plugins', { get: () => ps.plugins });
    // #endregion
    // #region private
    const build = () => __awaiter(void 0, void 0, void 0, function* () {
        var _a, _b, _c, _d;
        const { $drawPrototype, $options } = dp;
        const { selector = '', componentThis } = $options || {};
        if ($drawPrototype)
            return $drawPrototype;
        const nodeInfo = yield queryFields(selector, componentThis, { node: true });
        const canvas = ((_a = nodeInfo) === null || _a === void 0 ? void 0 : _a.node) || {};
        const ctx = ((_b = canvas.getContext) === null || _b === void 0 ? void 0 : _b.call(canvas, '2d')) || uni.createCanvasContext(selector, componentThis);
        if (!canvas || !ctx || !selector) {
            throw new Error('DrawPoster Error: useDrawPoster to build drawPoster instance');
        }
        canvas.width = (_c = $options === null || $options === void 0 ? void 0 : $options.width) !== null && _c !== void 0 ? _c : 0;
        canvas.height = (_d = $options === null || $options === void 0 ? void 0 : $options.height) !== null && _d !== void 0 ? _d : 0;
        return { canvas, ctx };
    });
    const mount = () => __awaiter(void 0, void 0, void 0, function* () {
        ps.run('beforeMount');
        Object.defineProperty(dp, 'render', { get: () => render });
        Object.defineProperty(dp, 'create', { get: () => create });
        Object.defineProperty(dp, 'ready', { get: () => ready });
        Object.defineProperty(dp, 'draw', { get: () => pcs.push });
        Object.defineProperty(dp, 'stop', { get: () => pcs.stop });
        Object.defineProperty(dp, 'use', { get: () => ps.use });
        yield (wait === null || wait === void 0 ? void 0 : wait());
        const { canvas, ctx } = yield build();
        Object.defineProperty(dp, 'canvas', { get: () => canvas });
        Object.defineProperty(dp, 'ctx', { get: () => ctx });
        ps.run('mounted');
        consola.success('挂载成功!', dp);
    });
    const ready = () => __awaiter(void 0, void 0, void 0, function* () { return promised.then(() => dp); });
    // #endregion
    // #region public
    const render = () => __awaiter(void 0, void 0, void 0, function* () {
        yield ready();
        const hideLoading = consola.loading('create');
        const tips = yield pcs.walk();
        consola.log('绘制状况: ', undefined, tips);
        if (options.type === 'context') {
            yield new Promise((resolve) => dp.ctx.draw(true, resolve));
        }
        hideLoading();
        return tips;
    });
    const create = (_options_ = {}) => __awaiter(void 0, void 0, void 0, function* () {
        yield ready();
        ps.run('beforeCreate');
        consola.log('beforeCreate');
        if (pcs.stacks.length > 0)
            yield dp.render();
        if (pcs.prevent)
            return Promise.reject();
        const hideLoading = consola.loading('create');
        const toPathOptions = _options_;
        if (options.type === '2d')
            toPathOptions.canvas = dp.canvas;
        if (options.type === 'context')
            toPathOptions.canvasId = dp.id;
        try {
            const { tempFilePath } = yield promisify(uni.canvasToTempFilePath)(toPathOptions);
            consola.success('绘制成功', { tempFilePath });
            return tempFilePath;
        }
        catch (error) {
            consola.error('绘制失败', error);
        }
        finally {
            hideLoading();
        }
    });
    // #endregion
    const promised = mount();
    // 保存实例, 实现单页面同个实例
    setCurrentDrawPoster(dp, ps);
    return dp;
};
//# sourceMappingURL=builder.js.map