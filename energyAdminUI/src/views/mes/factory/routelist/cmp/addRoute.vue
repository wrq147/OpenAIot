<template>
    <el-dialog v-if="dialogFlag" :title="title" :visible.sync="dialogFlag" :close-on-click-modal="false" append-to-body
      width="1100px" top="2vh" @close="cancel">
        <el-form ref="ruleForm" :model="ruleForm" :rules="rules" label-width="120px" class="addPeople">
            <el-row>
                <el-col :span="12">
                    <el-form-item label="工艺路线名称" prop="routeName">
                        <el-input v-model="ruleForm.routeName" placeholder="请输入工艺路线名称" />
                    </el-form-item>
                </el-col>
                <el-col :span="12">
                    <el-form-item label="目标仓库">
                        <el-select v-model="ruleForm.toHouseId" clearable placeholder="请选择目标仓库">
                            <el-option v-for="(item, index) in houseList" :key="index" :label="item.StoreName" :value="item.Id" />
                        </el-select>
                    </el-form-item>
                </el-col>
            </el-row>
        </el-form>
      <div style="margin: 10px 0">
        <el-button type="primary" icon="el-icon-plus" plain @click="addProductList">添加一行</el-button>
      </div>
      <el-table :data="ruleForm.items" border tooltip-effect="dark" style="width: 100%">
        <el-table-column type="index" label="序号" align="center" width="50" />
        <el-table-column align="center">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>工序
            </span>
          </template>
          <template slot-scope="scope">
            <div style="display: flex;">
              <el-select v-model="scope.row.OperId" placeholder="请选择工序" @change="selectOperInfo($event, scope.$index)">
                <el-option v-for="(item, index) in operListData" :key="index" :label="item.OperName" :value="item.Id" />
              </el-select>
            </div>
          </template>
        </el-table-column>
        <el-table-column align="center">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>报工数配比
            </span>
          </template>
          <template slot-scope="scope">
            <div>
              <el-input type="number" v-model="scope.row.PropOf" placeholder="请输入报工数配比" />
            </div>
          </template>
        </el-table-column>
        <el-table-column align="center">
          <template #header>
            <span>
              <span style="color: #f56c6c;">*</span>工时(分钟)
            </span>
          </template>
          <template slot-scope="scope">
            <div>
              <el-input type="number" v-model="scope.row.WorkTime" placeholder="请输入工时(分钟)" />
            </div>
          </template>
        </el-table-column>
        <el-table-column v-for="(item, index) in filedTableList" :key="index" align="center">
          <template #header>
            <span>
              <span v-if="item.is_required" style="color: #f56c6c;">*</span>{{ item.name }}
            </span>
          </template>
          <template slot-scope="scope">
            <div>
              <el-select :disabled="item.is_readonly" :allow-create="item.is_add" :multiple="item.type == '复选框'" :clearable="!item.is_required"
                  v-model="scope.row[item.mapid]" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%"
                  v-if=" (item.type == '单选框' && item.show_way == '下拉') || (item.type == '复选框' && item.show_way == '下拉') ">
                    <el-option v-for="(it, ix) in item.optionals" :label="it" :value="it" :key="ix"></el-option>
                </el-select>
                <el-radio-group :disabled="item.is_readonly" v-model="scope.row[item.mapid]" v-if="item.type == '单选框' && item.show_way == '平铺'">
                    <el-radio v-for="(it, ix) in item.optionals" :label="it" :key="ix">{{ it }}</el-radio>
                </el-radio-group>
                <el-checkbox-group :disabled="item.is_readonly" v-model="scope.row[item.mapid]" v-if="item.type == '复选框' && item.show_way == '平铺'">
                    <el-checkbox v-for="(it, ix) in item.optionals" :label="it" :key="ix">{{ it }}</el-checkbox>
                </el-checkbox-group>
                <el-date-picker :disabled="item.is_readonly" v-if="item.type == '时间'" v-model="scope.row[item.mapid]"
                  type="datetime" :placeholder="item.prompt_text ? item.prompt_text : '请选择'" style="width: 100%" :value-format="item.format" :format="item.format"></el-date-picker>
                <el-input :disabled="item.is_readonly" v-if="item.type == '文本'" :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                  :type="item.is_multiple ? 'textarea' : 'text'" v-model="scope.row[item.mapid]"></el-input>
                <el-input :disabled="item.is_readonly" v-if="item.type == '数字'" :placeholder="item.prompt_text ? item.prompt_text : '请输入'"
                  type="number" v-model="scope.row[item.mapid]" :precision="item.decimals"></el-input>
                <el-link :disabled="item.is_readonly" v-if="item.type == '超链接'" href="#" target="_blank">{{ item.describe_text }}</el-link>
                <image-upload v-model="scope.row[item.mapid]" :limit="1" v-if="item.type == '图片'"></image-upload>
                <el-select @focus="afterValSearch(form[item.mapid],item)" :clearable="true" style="width: 100%" v-model="form[item.mapid]" filterable remote reserve-keyword
                  :placeholder="item.prompt_text ? item.prompt_text : '请选择'" :remote-method="(query)=>associationMethod(query,item)" :loading="Supplierloading" v-if="item.type == '关联对象'">
                  <el-option v-for="ite in associationObject[item.mapid]" :key="ite.Value" :label="ite.Name" :value="ite.Value+','+ite.ValueName">{{ite.Name}}</el-option>
                </el-select>
            </div>
          </template>
        </el-table-column>
        <el-table-column label="操作" align="center" width="100">
          <template slot-scope="scope">
            <div>
              <el-button type="text" icon="el-icon-delete" style="color:red" @click="delProductList(scope.$index)">删除</el-button>
            </div>
          </template>
        </el-table-column>
      </el-table>
      <span slot="footer" class="dialog-footer">
        <el-button @click="cancel">取消</el-button>
        <el-button type="primary" @click="submitForm('ruleForm')">确定</el-button>
      </span>
    </el-dialog>
</template>
<script>
  import { houseList } from "@/api/storage/house";
  import { operList, operInfo } from "@/api/mes/oper";
  import { factorySearchObject } from "@/api/factory/product";
  import { routeAdd, routeEdit } from "@/api/mes/processRoute";
  export default {
    name: 'addRoute',
    props: {
      dialogVisible: {
        type: Boolean
      },
      title: {
        type: String
      },
      routeList: {
        type: Array,
        default: () => []
      },
      filedTableList: {
        type: Array,
        default: () => []
      }
    },
    data() {
      return {
        dialogFlag: false,
        // 表单
        ruleForm: {},
        // 校验
        rules: {
            routeName: [
                { required: true, message: "请输入工艺路线名称", trigger: "blur" },
            ],
        },
        operListData: [],
        houseList: [],
        // 存储每行的错误信息
        rowErrors: [],
        associationObject: {}, //所有关联对象对应的下拉的参数列表
      }
    },
    watch: {
      dialogVisible(newValue) {
        this.dialogFlag = newValue;
        if (newValue) {
          // 加载下拉列表数据
          operList().then(res => {
            this.operListData = res.data.List;
          });
          houseList().then(res => {
            this.houseList = res.data.List;
          });
        }
      }
    },
    methods: {
      // 提交新增/修改按钮
      submitForm(formName) {
        this.$refs[formName].validate((valid) => {
          if (valid) {
            // 验证表格行
            if (!this.validateAllRows()) {
              this.$message.error('请完善表格中的必填项');
              return false;
            }
            this.ruleForm.items.forEach((item, index) => {
              item.Sequence = index + 1;
            });
            if (this.title === '新增工艺路线') {
              this.getAddRpt()
            } else {
              this.setEditRpt()
            }
          } else {
            return false
          }
        })
      },

      // 选择工序时触发的事件
      selectOperInfo(id, index) {
        operInfo( { id: id } ).then(res => {
          if(res.data){
            this.ruleForm.items[index].PropOf = res.data.PropOf;
            this.ruleForm.items[index].WorkTime = res.data.WorkTime;
            this.filedTableList.forEach(item => {
              this.ruleForm.items[index][item.mapid] = res.data[item.mapid]; // 初始化字段值
            });
          }
        })
      },

      getAddRpt() {
        routeAdd(this.ruleForm).then(res => {
          this.$message.success('添加成功!')
          this.$emit('getList')
        })
      },

      setEditRpt() {
        routeEdit(this.ruleForm).then(res => {
          this.$message.success('修改成功!')
          this.$emit('getList')
        })
      },

      // 添加子项
      addProductList() {
        this.ruleForm.items.push({
          id: "",
          OrgId: this.$store.state.user.orgId,
          OperId: '',
          RouteId: '',
          PropOf: "",
          WorkTime: '',
          Sequence: '',
          // 添加所有mapid作为属性，初始值为空字符串
          ...this.filedTableList.reduce((acc, item) => {
              acc[item.mapid] = ''; // 初始化为空值
              return acc;
          }, {}),
        })
      },

      // 删除子项
      delProductList(index){
        if (index >= 0 && index < this.ruleForm.items.length) {
          this.ruleForm.items.splice(index, 1);
        }
      },

      // 验证单行
      validateRow(index) {
        const row = this.ruleForm.items[index];
        
        // 先初始化基础错误对象
        const errors = {
          OperId: !row || !row.OperId,
          PropOf: !row || (!row.PropOf && row.PropOf !== 0),
          WorkTime: !row || (!row.WorkTime && row.WorkTime !== 0)
        };
        
        // 再添加动态字段的验证（根据filedTableList）
        if (this.filedTableList && Array.isArray(this.filedTableList)) {
          this.filedTableList.forEach(item => {
            // 只处理必填字段
            if (item.is_required) {
              // 检查字段是否存在且有值
              errors[item.mapid] = !row || !row[item.mapid];
            } else {
              // 非必填字段默认为false（无错误）
              errors[item.mapid] = false;
            }
          });
        }
        
        this.rowErrors[index] = errors;
        
        // 返回是否所有字段都验证通过
        return Object.values(errors).every(error => !error);
      },

      // 验证所有行
      validateAllRows() {
        let isValid = true;
        
        this.ruleForm.items.forEach((_, index) => {
          if (!this.validateRow(index)) {
            isValid = false;
          }
        });
        
        return isValid;
      },
      cancel() {
        this.$emit('cancelForm')
      },
      //关联对象回显时获取列表
      afterValSearch(val,item){
          if(val&&val.indexOf(',') > -1){
            let keyVal=val.split(',')
            this.associationMethod(keyVal[1], item)
          }else{
            this.associationMethod('', item)
          }
      },
      //关联对象的远程搜索事件
      associationMethod(query, item){
          this.getFactorySearchObject(query, item.object_type, item.mapid);
      },
      //根据不同的关联对象获取对象的列表
      async getFactorySearchObject(key, objtype, mapid){
          let obj = {
              key: key,
              objtype: objtype,
              pageNum: 1,
              pageSize: 10
          };
          let res = await factorySearchObject(obj);
          if(res.data.List) {
              this.associationObject[mapid] = JSON.parse(JSON.stringify(res.data.List))
          }
      },
    }
  }
</script>
  
<style lang="scss" scoped>
  ::v-deep {
    .el-dialog__header {
      border-bottom: 1px solid #ccc;
    }
  
    .el-select,
    .el-cascader {
      width: 100%;
    }
  }
  
  .addPeople>.box {
    display: flex;
    align-items: center;
    justify-content: space-between;
    flex-wrap: wrap;
  }
  
  .addPeople>.btn {
    width: 100%;
    justify-content: flex-end;
    display: flex;
    align-items: center;
  }
  </style>