import { DrawPosterPlugin } from '../core';
/**
 * 处理 drawPoster 参数
 * @param args
 * @returns options
 */
export declare const helper: (...args: any[]) => {
    selector: string;
    componentThis?: any;
    type?: "2d" | "context" | "webgl" | undefined;
    loading?: boolean | {
        render?: string | undefined;
        create?: string | undefined;
    } | undefined;
    debug?: boolean | undefined;
    gcanvas?: boolean | undefined;
    width?: number | undefined;
    height?: number | undefined;
    plugins?: DrawPosterPlugin[] | undefined;
};
/**
 * 对插件参数进行处理并引入
 * @param plugins 插件列表
 * @param args 参数
 */
export declare const helperPluginParams: (plugins: DrawPosterPlugin[], ...args: any[]) => DrawPosterPlugin | undefined;
