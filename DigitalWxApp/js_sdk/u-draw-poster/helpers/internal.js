var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
export const globalPlugins = [];
export const getCurrentDrawPoster = (selector) => {
    const pages = getCurrentPages();
    const page = pages[pages.length - 1];
    if (page[`__dp_${selector}`])
        return page[`__dp_${selector}`];
};
export const setCurrentDrawPoster = (dp, ps) => {
    const pages = getCurrentPages();
    const page = pages[pages.length - 1];
    page[`__dp_${dp.id}`] = dp;
    const onUnload = page.onUnload;
    page.onUnload = function () {
        ps.run('beforeUnmount');
        dp.stop();
        onUnload();
        ps.run('unmounted');
    };
};
export const onReady = (callback) => __awaiter(void 0, void 0, void 0, function* () {
    const pages = getCurrentPages();
    const page = pages[pages.length - 1];
    const onReady = page.onReady;
    return new Promise((resolve) => {
        page.onReady = function () {
            callback === null || callback === void 0 ? void 0 : callback();
            resolve();
            onReady === null || onReady === void 0 ? void 0 : onReady();
        };
    });
});
//# sourceMappingURL=internal.js.map