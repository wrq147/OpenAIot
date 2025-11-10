import { helper } from '../helpers/params';
import { builder } from './builder';
import { Plugins } from './plugin';
function useDrawPoster(...args) {
    return builder(helper(...args)).ready();
}
export { useDrawPoster, Plugins };
//# sourceMappingURL=index.js.map