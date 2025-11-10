var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
/**
 * 查询 fields 信息
 * @param selector
 * @param componentThis
 * @param options
 * @returns
 */
export const queryFields = (selector, componentThis, options) => {
    const query = componentThis ? uni.createSelectorQuery().in(componentThis) : uni.createSelectorQuery();
    return new Promise((resolve) => {
        query
            .select(selector)
            .fields(options || {}, resolve)
            .exec();
    });
};
/**
 * 当前环境信息
 */
export const UNI_PLATFORM = (() => {
    // @ts-ignore
    if (typeof requirePlugin !== 'undefined')
        return 'mp-weixin';
    return 'other';
})();
/**
 * 将 Uni | wx 转换为异步 Api
 * @param api api
 */
export const promisify = (api) => {
    return (...args) => __awaiter(void 0, void 0, void 0, function* () {
        return new Promise((resolve, reject) => __awaiter(void 0, void 0, void 0, function* () {
            const [options, ...other] = args;
            api(Object.assign(Object.assign({}, options), { success: resolve, fail: reject }), ...other);
        }));
    });
};
//# sourceMappingURL=index.js.map