//工作流专用
export function deepCopy(obj) {
  return JSON.parse(JSON.stringify(obj))
}
//获取表单字段
export function getItems(formItems){
  let templateArry = [];
  if (formItems.length > 0) {
    formItems.forEach((ite) => {
      if (ite.name === "SpanLayout") {
        if (ite.props.items.length > 0) {
          templateArry = [
            ...templateArry,
            ...getItems(ite.props.items)
          ];
        }
      } else {
        let objs = [ite];
        templateArry = [...templateArry, ...objs];
      }
    });
  }
  return templateArry;
}

Array.prototype.remove = function (value) {
  let index = this.indexOf(value)
  if (index > -1) {
    this.splice(index, 1)
  }
  return index
}

//移除对象数组，匹配唯一key
Array.prototype.removeByKey = function (key, val) {
  let index = this.findIndex(value => value[key] === val)
  if (index > -1) {
    this.splice(index, 1)
  }
  return index
}

//对象数组转map
Array.prototype.toMap = function (key) {
	console.log("keykey",key);
  let map = new Map()
  this.forEach(v => map.set(v[key], v))
  return map
}
