<template>
  <el-row :gutter="20">
    <el-col :span="16"  :offset="4">
      <el-card shadow="always" class="cdcs">
        <div>
          <span>令牌：{{tokenStr}}</span><i class="el-icon-copy-document" @click="copy"></i>
        </div>
        <div style="margin-top:15px;">
          <el-link  icon="el-icon-search" type="primary" :href='url' target="_blank">打开API接口文档</el-link>
        </div>
      </el-card>
    </el-col>
  </el-row>
</template>
<script>
import { getToken } from '@/utils/auth'
export default {
  name: "ToolToken",
  data() {
    return {
      tokenStr: '',
      url: process.env.VUE_APP_BASE_API + "swagger/index.html"
    };
  },
  created: function () {
    this.tokenStr = getToken();
    this.url=this.url+"?tk="+this.tokenStr;
  },
  methods: {
    copyToClipboard(content){
          //window.clipboardData的作用是在页面上将需要的东西复制到剪贴板上，
          //提供了对于预定义的剪贴板格式的访问，以便在编辑操作中使用。
      if (window.clipboardData) {
          /*
          window.clipboardData有三个方法:
        （1）clearData(sDataFormat) 删除剪贴板中指定格式的数据。sDataFormat:"text","url"
        （2）getData(sDataFormat) 从剪贴板获取指定格式的数据。 sDataFormat:"text","url"
        （3）setData(sDataFormat, sData) 给剪贴板赋予指定格式的数据。返回 true 表示操作成功。
          */
        window.clipboardData.setData('text', content);
      } else {
        (function (content) {
          //oncopy 事件在用户拷贝元素上的内容时触发。
          document.oncopy = function (e) {
            e.clipboardData.setData('text', content);
            e.preventDefault(); //取消事件的默认动作
            document.oncopy = null;
          }
        })(content);
        //execCommand方法是执行一个对当前文档/当前选择/给出范围的命令。
        //'Copy':将当前选中区复制到剪贴板。 
        document.execCommand('Copy');
      }
    },
    copy: function () {
      this.copyToClipboard(this.tokenStr); // 需要复制的文本内容
      this.$message.success('复制成功，注意不要泄露您的令牌！');
    }
  }
};
</script>
<style lang="less" scoped>
.cdcs{
  margin-top:80px;
  span{
    word-break: break-all;
    line-height: 28px;
  }
  i{
    margin-left:50px;
    cursor: pointer;
  }
}
</style>