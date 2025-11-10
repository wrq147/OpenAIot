const fs = require('fs');
let configFilePath = `${process.env.UNI_INPUT_DIR}/res/PlatFormConfig.json`;
let configFile = fs.readFileSync(configFilePath, { encoding: 'utf-8' });
console.warn(configFilePath, 'configFilePath');
/**
 * 获取平台对应mianfest path key
 */
function getFileKey4PlatFormManifest() {
    let _configFile = JSON.parse(configFile.toString());
    // console.log("_configFile: " + JSON.stringify(_configFile));
    // console.log("_configFile.platFormConfig.platFormKey: ",_configFile.platFormConfig.platFormKey);
    return _configFile.platFormConfig.platFormKey;
}
/* 文件key */
let platFormKey = getFileKey4PlatFormManifest();
/* process.env.UNI_INPUT_DIR为项目所在的绝对路径，经测试，相对路径会找不到文件 */
/* 对应平台manifest.json文件路径 */
let platFormFilePath = `${process.env.UNI_INPUT_DIR}/res/platformManifest/manifest.${platFormKey}.json`;
// console.log("platFormFilePath: ",platFormFilePath);
/* 全局mianfest.json文件路径 */
let mainFilePath = `${process.env.UNI_INPUT_DIR}/manifest.json`;

fs.readFile(platFormFilePath, function (err, data) {
    if (err) {
        console.error(`${platFormFilePath}读取失败`, JSON.stringify(err));
    } else {
        var _data = JSON.parse(data.toString());
        _data = JSON.stringify(_data);
        // 写入
        fs.writeFile(
            mainFilePath,
            _data,
            {
                encoding: 'utf-8',
            },
            function (err) {
                if (err) {
                    console.error(
                        `manifest动态修改结果：${platFormFilePath} 写入 ${mainFilePath} 失败`,
                        err
                    );
                } else {
                    // console.warn(`manifest动态修改结果：${platFormFilePath} 写入 ${mainFilePath} 成功`)
                    console.warn(
                        `manifest动态修改结果：manifest.${platFormKey}.json 写入 manifest.json 成功。`
                    );
                    // let platConfig = require(`${process.env.UNI_INPUT_DIR}/res/platformConfig/HC.${platFormKey}.json`);
                    // let modelManifestJson = require(`${process.env.UNI_INPUT_DIR}/res/platformManifest/manifest.${platFormKey}.json`)
                    // console.log("pConfig:", JSON.stringify(platConfig));
                    // console.log("modelManifestJson:", JSON.stringify(modelManifestJson));
                    let modelManifestJson = JSON.parse(_data);
                    // console.warn(process.env.NODE_ENV, 'GGGG');
                    // let _configFile = JSON.parse(configFile.toString());
                    // let manifestJson = JSON
                    // var urlKey = _configFile.netRequestConfig.baseUrlKey;
                    // let shopConfig = platConfig.shopConfig;
                    // if (process.env.NODE_ENV == 'development') {
                    //     console.warn('当前环境: 运行环境(debug)');
                    //     urlKey = _configFile.netRequestConfig.betaUrlKay;
                    //     if (shopConfig != null && shopConfig.devUrl != null) {
                    //         shopConfig = shopConfig.devUrl;
                    //     }
                    // } else {
                    //     urlKey = _configFile.netRequestConfig.baseUrlKey;
                    //     console.warn('当前环境: 发行环境(Release)');
                    //     if (shopConfig != null && shopConfig.prodUrl != null) {
                    //         shopConfig = shopConfig.prodUrl;
                    //     }
                    // }
                    // let url = platConfig[urlKey] || '';
                    // console.log('当前工程平台', modelManifestJson);
                    // console.warn('当前工程平台服务器地址', urlKey, '：', url);
                    console.warn('当前工程版本名称', modelManifestJson.versionName);
                    console.warn('当前工程版本号', modelManifestJson.versionCode);
                }
            }
        );
    }
});
