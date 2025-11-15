import PreviewItem from "./index.vue";

const Preview = {};

// 注册
Preview.install = function(Vue) {
  const PreviewConstructor = Vue.extend(PreviewItem);
  const instance = new PreviewConstructor();
  instance.$mount(document.createElement("div"));
  document.body.appendChild(instance.$el);

  /**
   * 挂载在vue原型上
   * @param {Array} imgs 需要预览的图片数组
   */
  Vue.prototype.$openPreview = function(imgs = []) {
    instance.showPreview = true;
    instance.previewImages = imgs;
  };
};

export default Preview;