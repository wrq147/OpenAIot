<template>
    <div>
        <el-input style="width:100%;" :disabled="disabled" type="number"
            v-if="Item.type == 'int' || Item.type == 'float'" @change="onChange"
            v-model.number="tmpVal"></el-input>
        <el-date-picker style="width:100%;" value-format="timestamp" type="datetime" :disabled="disabled"
            v-else-if="Item.type == 'date'" @change="onChange" v-model="tmpVal"></el-date-picker>
        <el-switch v-else-if="Item.type == 'boolean'" style="width:100%;" :disabled="disabled" v-model="tmpVal"
            active-text="真" inactive-text="假" @change="onChange">
        </el-switch>
        <el-select style="width:100%;" :disabled="disabled" v-else-if="Item.type == 'enum'" @change="onChange"
            v-model="tmpVal" placeholder="请选择">
            <el-option v-for="(val, kv) in Item.elements" :key="kv" :label="kv" :value="val">
            </el-option>
        </el-select>
        <el-input style="width:100%;" :disabled="disabled" v-else v-model="tmpVal" @change="onChange"></el-input>
    </div>
</template>

<script>
export default {
    name: "paramItem",
    props: {
        Item: {
            type: Object,
            default: () => {
                return {};
            }
        },
        disabled: {
            type: Boolean,
            default: () => {
                return false;
            }
        }
    },
    data() {
        return {
            tmpVal: null
        };
    },
    watch: {
        Item:{
            deep:true,
            handler(newval, oldval) {
                console.log(this.Item.defval,'this.Item.defval');
                this.tmpVal = this.Item.defval;
                this.onChange(this.tmpVal);
            }
        }
    },
    mounted() {
        this.tmpVal = this.Item.defval;
        this.onChange(this.tmpVal);
    },
    methods: {
        reset() {
            this.tmpVal = null;
        },
        getVal() {
            if(this.Item.disabledDef==true){
                return null;
            }
            else{
                if(this.tmpVal==null){
                    if(this.Item.type=="boolean"){
                        return false;
                    }
                    else if(this.Item.type=="int"||this.Item.type=="float"){
                        return 0;
                    }
                    else if(this.Item.type=="enum"){
                        if(this.Item.elements!=null&&this.Item.elements.length>0){
                            return this.Item.elements[0];
                        }
                        else{
                            return null;
                        }
                    }
                    else if(this.Item.type=="date"){
                        return new Date().getTime();
                    }
                    else{
                        return "";
                    }
                }
            }
            return this.tmpVal;
        },
        onChange(val) {
            if (this.Item.type == 'date') {
                this.$emit("change", this.parseTime(val));
            }
            else {
                this.$emit("change", val);
            }
        }
    }
};
</script>

<style lang="less"></style>