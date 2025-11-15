import { getFormDetail } from "@/api/flowable/design";
import { getItems } from "@/views/flowable/common/utlity.js"

export const flowminix = {
    
    data() {
        return {
            formInit: [[],[],[]],
            tbloading:[false,false]
        }
    },
    watch: {
       
    },
    computed:{
        
    },
    destroyed() {
        
    },
    mounted() {
       
    },
    methods: {
        setformInitData(data,inx){
          this.formInit[inx]=JSON.parse(JSON.stringify(data))
        },
        returnformInit(index){
            let subformInit = JSON.parse(JSON.stringify(this.formInit[index]))
            subformInit = subformInit.map(row => {
              let obj = {}
              if (row.eltype != 'TableList') {
                obj = { "id": row.id, "title": row.title, "eltype": row.eltype, "way": row.way, "val": row.val }
              } else {
                obj = { "id": row.id, "title": row.title, "eltype": row.eltype, "way": row.way, "val": row.val }
                if (row.way == 1) {
                  obj.val = JSON.stringify(row.value)
                }
              }
              return obj
            })
            return subformInit
        },
        async onSelected(item, issetVal, setVal) {
            // console.log("选择流程后",item);
            this.tbloading[this.activeIdx]=true
            if (this.activeIdx == 0) {
              this.form.LeaveTemplateId = item.Id;
              this.form.LeaveTemplateName = item.Name;
            }
            else if (this.activeIdx == 1)  {
              this.form.EnterTemplateId = item.Id;
              this.form.EnterTemplateName = item.Name;
            }else if (this.activeIdx == 2)  {
              this.form.LeaveApplyTemplateId = item.Id;
              this.form.LeaveApplyTemplateName = item.Name;
            }
            let rsp = await getFormDetail(item.Id);
            let tformItems = JSON.parse(rsp.data.Form.FormFields);
            let newformItems = getItems(tformItems);
            this.formInit[this.activeIdx].length=0;
            newformItems.map(element => {
                if (element.name == "TextInput" || element.name == "TextareaInput") {
                    let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": "" }
                    if (issetVal && setVal) {
                        let rowval = setVal.find(rw => rw.id == element.id)
                        if (rowval) {
                            obj.val = rowval.val
                            obj.way = rowval.way
                        }
                    }
                    this.formInit[this.activeIdx].push(obj);
                }
                else if (element.name == "DevicPicker") {
                    this.formInit[this.activeIdx].push({ "id": element.id, "title": element.title, "eltype": element.name, "way": 1, "val": "目标设备" });
                }
                else if (element.name == "TableList") {
                    let obj = { "id": element.id, "title": element.title, "eltype": element.name, "way": 0, "val": [], "props": element.props }
                    if (issetVal && setVal) {
                        let rowval = setVal.find(rw => rw.id == element.id)
                        if (rowval) {
                            obj.way = rowval.way
                            obj.val = rowval.val
                        }
                    }
                    this.formInit[this.activeIdx].push(obj);
                }
            });
            this.tbloading[this.activeIdx]=false
            this.formInit=JSON.parse(JSON.stringify(this.formInit))
            this.$forceUpdate()
          },
    }
}