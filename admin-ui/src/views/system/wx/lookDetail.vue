<template>
    <el-dialog v-if="dialogFlag" title="详情" :visible.sync="dialogFlag" :close-on-click-modal="false" width="800px" top="30vh" @close="cancel">
        <el-descriptions class="margin-top" :column="3" border>
            <el-descriptions-item>
                <template slot="label">
                    AgentId
                </template>
                {{ lookData.AgentId }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    AppId
                </template>
                {{ lookData.AppId }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    OrgId
                </template>
                {{ lookData.OrgId }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    名称
                </template>
                {{ lookData.Name }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    存储账号名称
                </template>
                {{ lookData.OrgName }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    账号类型
                </template>
                {{ accountType(lookData.AccType) }}
            </el-descriptions-item>
            <el-descriptions-item>
                <template slot="label">
                    AppSecret
                </template>
               {{ lookData.AppSecret }}
            </el-descriptions-item>
        </el-descriptions>
        <span slot="footer" class="dialog-footer">
            <el-button @click="cancel">取消</el-button>
        </span>
    </el-dialog>
  </template>
  
  <script>
  export default {
    name: 'lookDetail',
    props: {
      dialogVisible: {
        type: Boolean
      },
      lookData: {
        type: Object
      },
    },
    data() {
      return {
        dialogFlag: false
      }
    },
    watch: {
      dialogVisible(newValue) {
        this.dialogFlag = newValue
      }
    },
    methods: {
        accountType(type) {
            switch (type) {
                case 'js':
                    return '公众号';
                case 'applet':
                    return '小程序';
                case 'corp':
                    return '企业微信';
            }
        },
        cancel() {
            this.$emit('cancelForm')
        }
    }
  }
  </script>
  <style lang="scss" scoped>
    ::v-deep {
        .el-dialog__header{
            border-bottom: 1px solid #ccc;
        }
    }
  </style>