<template>
  <view style="width: 100%;">
    <view v-if="disabled" style="width: 100%;">
	  <view class="view_input" v-if="!_value||_value.length==0">
	  	<view class="pal_col">
	  		{{placeholder?placeholder:'Please select personnel'}}
	  	</view>
	  	<view class="view_mask"></view>
	  	<view class="form_sel_icon">
	  		<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
	  			iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
	  	</view>
	  </view>
	  <view class="user_info" v-for="item in _value" v-if="_value&&_value.length>0" :class="{'disabled_text':disabled}">
	  	<view class="avatar">
	  		<image class="image" :src="item.avatar" mode="aspectFit"></image>
	  	</view>
	  	<view class="info_right">
	  		<view class="user_name">{{item.name}}</view>
	  	</view>
	  </view>
    </view>
    <view v-else style="width: 100%;">
	  <view class="view_input" @click="choiceManage">
	  	<view class="pal_col" v-if="!_value||_value.length==0">
	  		{{placeholder?placeholder:'Please select personnel'}}
	  	</view>
	  	<view class="view_li_con personnel_li_con" v-if="_value&&_value.length>0">
	  		<view class="view_li_cot personnel_li_cot">
	  			<view class="view_li" v-for="(item,inx) in _value">
	  				<view class="view_text">
	  					{{item.name}}
	  				</view>
	  				<view class="view_icon" @click.stop="delMulPer(inx)">
	  					<custom-icons iconsName="icon-guanbidanchuang" iconsSize="20rpx"
	  						iconsColor="rgba(255, 255, 255, 1)"></custom-icons>
	  				</view>
	  			</view>
	  		</view>
	  	</view>
	  	<view class="view_mask"></view>
	  	<view class="form_sel_icon">
	  		<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
	  			iconsColor="rgba(255, 255, 255, 0.5)"></custom-icons>
	  	</view>
	  </view>
    </view>
	<uni-popup :ref="'userSelect'+keyId" :mask-click="false" background-color="#161A26" :safe-area="true"
		mask-background-color="rgba(0, 0, 0, 0.5)" type="top"  :zIndex="999">
		<view style="width: 100%;height: 100vh;position: relative;">
			<employeeSelect :ref="'empsel'+keyId" topTitle="Select employee" :multiple="multiple" type="user"
				:selected="_value&&_value.length>0?_value:[]" @closeSelect="closeSelect" @selectEmplee="selectEmplee"></employeeSelect>
		</view>
	</uni-popup>
  </view>
</template>

<script>
import componentMinxins from '../ComponentMinxins'
import employeeSelect from "@/components/employee_select/employee_select.vue"
export default {
  mixins: [componentMinxins],
  name: "UserPicker",
  components:{employeeSelect},
  props: {
    value:{
      type: Array,
      default: () => {
        return []
      }
    },
    placeholder: {
      type: String,
      default: 'Please select personnel'
    },
    multiple:{
      type: Boolean,
      default: false
    },
    disabled: {
      default: false,
      type: Boolean,
    },
	keyId: {
		type: [String, Number],
		default: ''
	},
  },
  data() {
    return {
      showOrgSelect: false
	  
    }
  },
  mounted() {
  	this.$nextTick(()=>{
		// console.log("展示的人员",this._value);
	})
  },
  methods: {
	  closeSelect(){
	  	this.$refs['userSelect'+this.keyId].close()
	  },
	  delMulPer(inx) {
	  	this._value.splice(inx, 1)
	  	this.$forceUpdate()
	  },
	  selectEmplee(val) {
	  	//选中负责人后
	  	console.log("负责人", val);
		this.closeSelect()
	  	if (val) {
	  		this._value = val
			// this.$emit('setActiveItem','')
	  		this.$forceUpdate()
	  	}
	  
	  },
	  choiceManage() {
	  	//选择员工
	  	if(!this.disabled){
			let selected = this._value ? this._value : []
			this.$refs['userSelect'+this.keyId].open()
			this.$nextTick(()=>{
				this.$refs['empsel'+this.keyId].setSelected(selected)
			})
			// this.$emit('setActiveItem',this.keyId)
			// let selected = this._value ? JSON.stringify(this._value) : JSON.stringify([])
			// if (this.multiple) {
			// 	uni.navigateTo({
			// 		url: '/pages_Inventory/employee_select?selected=' + selected + '&multiple=' + this
			// 			.multiple
			// 	})
			// } else {
			// 	uni.navigateTo({
			// 		url: '/pages_Inventory/employee_select?selected=' + selected
			// 	})
			// }
		}
	  },
  }
}
</script>

<style scoped>
.placeholder{
  margin-left: 10px;
  color: #adabab;
  font-size: smaller;
}
</style>
