import { Plugins, DrawPosterPlugin } from './plugin';
import { DrawPosterOptions, DrawPosterResult } from './typed';
/**
 * 创建 dp 绘制（仅支持 vue3 setup 中使用）
 * @param selector
 * @param options
 */
declare function useDrawPoster(selector: string, options?: Partial<Omit<DrawPosterOptions, 'selector'>>): Promise<DrawPosterResult>;
declare function useDrawPoster(options: DrawPosterOptions): Promise<DrawPosterResult>;
export { useDrawPoster, DrawPosterPlugin, Plugins };
