<template>
	<view style="width: 100%;">
		<view v-if="disabled" style="width: 100%;">
			<view class="view_input">
				<view class="pal_col" v-if="!_value||_value.length==0">
					{{placeholder?placeholder:'Please select department'}}
				</view>
				<view class="view_li_con personnel_li_con" v-if="_value&&_value.length>0">
					<view class="view_li_cot personnel_li_cot">
						<view class="view_li" v-for="(item,inx) in _value" :class="{'disabled_text':disabled}">
							<view class="view_text">
								{{item.name}}
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
		<view v-else style="width: 100%;">
			<view class="view_input" @click="choiceDepartment">
				<view class="pal_col" v-if="!_value||_value.length==0">
					{{placeholder?placeholder:'Please select department'}}
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
		<uni-popup :ref="'deptSelect'+keyId" :mask-click="false" background-color="#161A26" :safe-area="true"
			mask-background-color="rgba(0, 0, 0, 0.5)" type="top"  :zIndex="999">
			<view style="width: 100%;height: 100vh;position: relative;">
				<employeeSelect :ref="'empsel'+keyId" topTitle="Select department" :multiple="multiple" type="dept"
					:selected="_value&&_value.length>0?_value:[]" @closeSelect="closeSelect" @selectDept="selectDept"></employeeSelect>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";
	import employeeSelect from "@/components/employee_select/employee_select.vue"
	export default {
		mixins: [componentMinxins],
		name: "DeptPicker",
		components:{employeeSelect},
		props: {
			value: {
				type: Array,
				default: () => {
					return [];
				},
			},
			placeholder: {
				type: String,
				default: "Please select department",
			},
			multiple: {
				type: Boolean,
				default: false,
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
				// showOrgSelect: false,
			};
		},
		methods: {
			closeSelect(){
				this.$refs['deptSelect'+this.keyId].close()
			},
			choiceDepartment() {
				//选择部门
				if (!this.disabled) {
					
					// this.$emit('setActiveItem', this.keyId)
					let selected = this._value ? this._value : []
					this.$refs['deptSelect'+this.keyId].open()
					this.$nextTick(()=>{
						this.$refs['empsel'+this.keyId].setSelected(selected)
					})
					// if (this.multiple) {
					// 	uni.navigateTo({
					// 		url: '/pages_Inventory/employee_select?selected=' + selected + '&multiple=' + this
					// 			.multiple + '&type=dept'
					// 	})
					// } else {
					// 	uni.navigateTo({
					// 		url: '/pages_Inventory/employee_select?selected=' + selected + '&type=dept'
					// 	})
					// }

				}

			},
			selectDept(val) {
				//选中部门后
				console.log("部门", val);
				this.closeSelect()
				if (val) {
					this._value = val
					this.$forceUpdate()
				}

			},
			delMulPer(inx) {
				this._value.splice(inx, 1)
				this.$forceUpdate()
			},
		},
	}
</script>

<style scoped>
	.placeholder {
		margin-left: 10px;
		color: #adabab;
		font-size: smaller;
	}
</style>