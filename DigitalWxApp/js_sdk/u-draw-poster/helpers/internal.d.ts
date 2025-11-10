import { DrawPosterPlugin, Plugins } from '../core/plugin';
import { DrawPosterResult } from '../core/typed';
export declare const globalPlugins: DrawPosterPlugin[];
export declare const getCurrentDrawPoster: (selector: string) => DrawPosterResult | undefined;
export declare const setCurrentDrawPoster: (dp: Partial<DrawPosterResult>, ps: Plugins) => void;
export declare const onReady: (callback?: Function | undefined) => Promise<void>;
