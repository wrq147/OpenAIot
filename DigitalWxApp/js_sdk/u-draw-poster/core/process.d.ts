import { Consola } from './consola';
import { DrawPosterResult } from './typed';
declare class DrawProcess {
    dp: Partial<DrawPosterResult>;
    consola: Consola;
    /** 进程堆栈 */
    stacks: Array<() => Promise<any>>;
    /** 是否阻止运行 */
    prevent: boolean;
    constructor(dp: Partial<DrawPosterResult>, consola: Consola);
    push: (callback: Function) => void;
    stop: () => void;
    walk: () => Promise<any[]>;
}
export default DrawProcess;
