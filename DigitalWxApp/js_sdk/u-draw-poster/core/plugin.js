import { globalPlugins } from '../helpers/internal';
import { helperPluginParams } from '../helpers/params';
export class Plugins {
    constructor(dp) {
        var _a, _b;
        this.dp = dp;
        /** 当前示例中挂在的所有 plugins （不包含 global plugins） */
        this.$plugins = [];
        /**
         * 局部插件挂载
         * @param args
         */
        this.use = (...args) => {
            var _a;
            const plugin = helperPluginParams(this.$plugins, ...args);
            if (this.dp['canvas'])
                (_a = plugin === null || plugin === void 0 ? void 0 : plugin.mounted) === null || _a === void 0 ? void 0 : _a.call(plugin, this.dp);
            return this;
        };
        this.run = (lifeCycleName) => {
            this.plugins.forEach((lifeCycle) => { var _a; return (_a = lifeCycle[lifeCycleName]) === null || _a === void 0 ? void 0 : _a.call(lifeCycle, this.dp); });
        };
        if ((_a = dp.$options) === null || _a === void 0 ? void 0 : _a.plugins)
            this.$plugins.push(...(_b = dp.$options) === null || _b === void 0 ? void 0 : _b.plugins);
    }
    /** 当前示例中挂在的所有 plugins（包含 global plugins） */
    get plugins() {
        return [...globalPlugins, ...this.$plugins];
    }
}
/**
 * 全局插件挂载
 * @param args
 */
Plugins.use = (...args) => {
    helperPluginParams(globalPlugins, ...args);
    return Plugins;
};
//# sourceMappingURL=plugin.js.map