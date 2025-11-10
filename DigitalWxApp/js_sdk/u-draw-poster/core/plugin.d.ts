import { DrawPosterResult } from './typed';
export interface DrawPosterLifeCycle<I = Required<DrawPosterResult>, O = Record<string, any>> {
    (instance: I, options?: O): void;
}
export interface DrawPosterLifeCycles {
    /** 创建实例前 */
    beforeMount?: DrawPosterLifeCycle<Partial<DrawPosterResult>>;
    /** 创建实例后 */
    mounted?: DrawPosterLifeCycle;
    /** 卸载实例前 */
    beforeUnmount?: DrawPosterLifeCycle;
    /** 卸载实例后 */
    unmounted?: DrawPosterLifeCycle;
    /** 创建绘图前 */
    beforeCreate?: DrawPosterLifeCycle;
    /** 创建绘图后 */
    created?: DrawPosterLifeCycle;
}
export interface DrawPosterPlugin extends DrawPosterLifeCycles {
    /** 扩展名称 */
    name: string;
}
export interface DrawPosterUse {
    (name: string, lifeCycle: DrawPosterLifeCycle): void;
    (name: string, options: DrawPosterLifeCycles): void;
    (options: DrawPosterPlugin): void;
}
export declare class Plugins {
    dp: Partial<DrawPosterResult>;
    /** 当前示例中挂在的所有 plugins （不包含 global plugins） */
    private $plugins;
    /** 当前示例中挂在的所有 plugins（包含 global plugins） */
    get plugins(): DrawPosterPlugin[];
    constructor(dp: Partial<DrawPosterResult>);
    /**
     * 全局插件挂载
     * @param args
     */
    static use: DrawPosterUse;
    /**
     * 局部插件挂载
     * @param args
     */
    use: DrawPosterUse;
    run: (lifeCycleName: keyof DrawPosterLifeCycles) => void;
}
