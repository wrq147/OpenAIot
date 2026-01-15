// 用于标记 luckysheet 是否已经加载完成
let luckysheetLoaded = false
// 用于存储加载中的 Promise，避免重复加载
let loadPromise = null

/**
 * 检查指定 URL 的脚本/样式是否已存在于页面中
 * @param {string} url 资源URL
 * @param {string} type 类型：'script' 或 'link'
 * @returns {boolean} 是否存在
 */
function isResourceExists(url, type) {
  // 提取资源的核心标识（避免CDN版本号等参数干扰）
  const getKey = (url) => {
    const pureUrl = url.split('?')[0] // 去掉URL参数
    return pureUrl.substring(pureUrl.lastIndexOf('/') + 1)
  }

  const targetKey = getKey(url)
  let elements = []

  if (type === 'script') {
    elements = Array.from(document.querySelectorAll('script'))
    return elements.some(script => getKey(script.src) === targetKey)
  } else if (type === 'link') {
    elements = Array.from(document.querySelectorAll('link[rel="stylesheet"]'))
    return elements.some(link => getKey(link.href) === targetKey)
  }
  return false
}

/**
 * 动态加载单个 CSS 资源
 * @param {string} url CSS地址
 * @returns {Promise} 加载Promise
 */
function loadCSS(url) {
  return new Promise((resolve, reject) => {
    // 先检查是否已加载该CSS
    if (isResourceExists(url, 'link')) {
      console.log(`CSS ${url} 已存在，无需重复加载`)
      resolve()
      return
    }

    const link = document.createElement('link')
    link.rel = 'stylesheet'
    link.href = url
    link.onload = () => {
      console.log(`luckysheet CSS 加载完成: ${url}`)
      resolve()
    }
    link.onerror = (err) => reject(new Error(`luckysheet CSS 加载失败 ${url}: ` + err))
    document.head.appendChild(link)
  })
}

/**
 * 动态加载单个 JS 资源
 * @param {string} url JS地址
 * @returns {Promise} 加载Promise
 */
function loadJS(url) {
  return new Promise((resolve, reject) => {
    // 核心检查：判断是否已存在该JS（重点针对 luckysheet.umd.js）
    if (isResourceExists(url, 'script')) {
      console.log(`JS ${url} 已存在，无需重复加载`)
      resolve()
      return
    }

    const script = document.createElement('script')
    script.src = url
    script.async = false // 同步加载，保证依赖顺序
    script.onload = () => {
      console.log(`luckysheet JS 加载完成: ${url}`)
      resolve()
    }
    script.onerror = (err) => reject(new Error(`luckysheet JS 加载失败 ${url}: ` + err))
    document.body.appendChild(script)
  })
}

/**
 * 动态加载 luckysheet 资源
 * @returns {Promise} 加载完成的 Promise
 */
export function loadLuckysheet() {
  // 如果已经加载完成，直接返回成功的 Promise
  if (luckysheetLoaded) {
    return Promise.resolve()
  }

  // 如果正在加载中，返回同一个 Promise
  if (loadPromise) {
    return loadPromise
  }

  // 开始加载资源
  loadPromise = new Promise(async (resolve, reject) => {
    try {
      // 1. 按顺序加载所有 CSS 资源
      const cssUrls = [
        '/luckysheet/plugins/css/pluginsCss.css',
        '/luckysheet/plugins/plugins.css',
        '/luckysheet/css/luckysheet.css',
        '/luckysheet/assets/iconfont/iconfont.css'
      ]
      for (const cssUrl of cssUrls) {
        await loadCSS(cssUrl)
      }

      // 2. 按依赖顺序加载 JS 资源（重点检查 luckysheet.umd.js）
      const jsUrls = [
        '/luckysheet/plugins/js/plugin.js',
        '/luckysheet/luckysheet.umd.js'
      ]
      for (const jsUrl of jsUrls) {
        await loadJS(jsUrl)
      }

      // 标记加载完成
      luckysheetLoaded = true
      resolve()
    } catch (error) {
      // 加载失败时重置 Promise，允许重新加载
      loadPromise = null
      reject(error)
    }
  })

  return loadPromise
}