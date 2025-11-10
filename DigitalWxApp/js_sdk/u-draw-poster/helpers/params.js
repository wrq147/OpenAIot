import { isFunction, isObject, isString } from 'lodash';
import { UNI_PLATFORM } from '../utils';
import { globalPlugins } from './internal';
/**
 * 处理 drawPoster 参数
 * @param args
 * @returns options
 */
export const helper = (...args) => {
    var _a, _b, _c, _d;
    const _default = {
        selector: '',
        componentThis: undefined,
        type: UNI_PLATFORM === 'mp-weixin' ? '2d' : 'context',
        loading: false,
        debug: false,
        gcanvas: false
    };
    let _overrides;
    if (isObject(args[0])) {
        _overrides = args[0];
    }
    else if (isObject(args[1])) {
        _overrides = args[1];
        _overrides.selector = args[0];
    }
    else {
        _overrides = { selector: args[0] };
    }
    const options = Object.assign(Object.assign({}, _default), _overrides);
    options.selector = options.selector.replace('#', '');
    if (options.type === '2d') {
        options.selector = `#${options.selector}`;
    }
    if (options.loading === true) {
        options.loading = { render: '绘制海报中...', create: '生成图片中...' };
    }
    if (isObject(options.loading)) {
        options.loading.render = (_b = (_a = options.loading) === null || _a === void 0 ? void 0 : _a.render) !== null && _b !== void 0 ? _b : '绘制海报中...';
        options.loading.create = (_d = (_c = options.loading) === null || _c === void 0 ? void 0 : _c.create) !== null && _d !== void 0 ? _d : '生成图片中...';
    }
    return options;
};
/**
 * 对插件参数进行处理并引入
 * @param plugins 插件列表
 * @param args 参数
 */
export const helperPluginParams = (plugins, ...args) => {
    if (!args[0]) {
        throw new Error('DrawPoster Error: plugins arguments required');
    }
    let _options = { name: '' };
    if (isString(args[0]) && isFunction(args[1])) {
        _options.name = args[0];
        _options.mounted = args[1];
    }
    if (isString(args[0]) && isObject(args[1])) {
        _options = Object.assign({ name: args[0] }, args[1]);
    }
    if (isObject(args[0])) {
        _options = args[0];
    }
    if (![...globalPlugins, ...plugins].some((v) => _options.name === v.name)) {
        plugins.push(_options);
        return _options;
    }
    console.warn(`该扩展已存在: ${_options.name}`);
};
//# sourceMappingURL=params.js.map