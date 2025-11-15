// var fs = require('fs')
// var file = fs.readFileSync('../../assets/icons/customIcon/iconfont.css').toString();
// var os = require('os')

// var icons = file.split(os.EOL)
// console.log("图标", icons);
var icons;

const styleSheets = document.styleSheets;
let iconNames = [];
for (let i = 0; i < styleSheets.length; i++) {
    const rules = styleSheets[i].rules || styleSheets[i].cssRules;
    for (let j = 0; j < rules.length; j++) {
        if (rules[j].selectorText) {
            const selectors = rules[j].selectorText.split(',');
            selectors.forEach(selector => {
                if (selector.includes('.zhongtai-icon-')) {
                    iconNames.push(selector.replace('.zhongtai-icon-', '').substring(0, selector.replace('.zhongtai-icon-', '').lastIndexOf(":") - 1)); // 提取类名，去掉点号
                }
            });
        }
    }
}
icons = [...new Set(iconNames)]; // 使用Set去重，然后转回数组
// console.log(icons, 'icons');
export default icons