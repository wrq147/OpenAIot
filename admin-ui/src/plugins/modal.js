import Vue from 'vue'
let loadingInstance;
export default {
  // 消息提示
  msg(content) {
    new Vue().$message.info(content)
  },
  // 错误消息
  msgError(content) {
    new Vue().$message.error(content)
  },
  // 成功消息
  msgSuccess(content) {
    new Vue().$message.success(content)
  },
  // 警告消息
  msgWarning(content) {
    new Vue().$message.warning(content)
  },
  // 弹出提示
  alert(content) {
    new Vue().$msgbox.alert(content, "系统提示")
  },
  // 错误提示
  alertError(content) {
    new Vue().$msgbox.alert(content, "系统提示", { type: 'error' })
  },
  // 成功提示
  alertSuccess(content) {
    new Vue().$msgbox.alert(content, "系统提示", { type: 'success' })
  },
  // 警告提示
  alertWarning(content) {
    new Vue().$msgbox.alert(content, "系统提示", { type: 'warning' })
  },
  // 通知提示
  notify(content) {
    new Vue().$notify.info(content)
  },
  // 错误通知
  notifyError(content) {
    new Vue().$notify.error(content);
  },
  // 成功通知
  notifySuccess(content) {
    new Vue().$notify.success(content)
  },
  // 警告通知
  notifyWarning(content) {
    new Vue().$notify.warning(content)
  },
  // 确认窗体
  confirm(content) {
    return new Vue().$msgbox.confirm(content, "系统提示", {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: "warning",
    })
  },
  // 打开遮罩层
  loading(content) {
    loadingInstance = new Vue().$loading({
      lock: true,
      text: content,
      spinner: "el-icon-loading",
      background: "rgba(0, 0, 0, 0.7)",
    })
  },
  // 关闭遮罩层
  closeLoading() {
    loadingInstance.close();
  }
}
