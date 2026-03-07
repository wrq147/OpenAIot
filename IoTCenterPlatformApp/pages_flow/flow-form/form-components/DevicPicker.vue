<template>
	<view style="width:100%">
		<view v-if="disabled" style="width:100%">
			<view class="form_device_list disabled_text">
				<view class="form_device_li" v-for="(item,inx) in _value" :key="item.id" @click="jumpDevDetail(item)">
					<view class="li_left">
						<image class="image" :src="returnDeviceImg(item.photoUrl)+'?wh=500x500'" mode=""></image>
					</view>
					<view class="li_right">
						<view class="name">{{item.name}}</view>
						<view class="number">{{item.deviceNumber}}</view>
					</view>
				</view>
			</view>
		</view>
		<view v-else style="width:100%">
			<view class="form_device_list">
				<view class="form_device_li" v-for="(item,inx) in _value" :key="item.id">
					<view class="li_left">
						<image class="image" :src="returnDeviceImg(item.photoUrl)+'?wh=500x500'" mode="" v-if="item.photoUrl"></image>
						<image class="image" :src="getSerVerUrl()+'/appimg/device_default.png'" mode="aspectFill" v-else></image>
					</view>
					<view class="li_right">
						<view class="name">{{item.name}}</view>
						<view class="number">{{item.deviceNumber}}</view>
					</view>
					<view class="del_icon" @click.stop="delSelDev(inx)">
						<view class="icons_del t-icon-yichu1"></view>
					</view>
				</view>
				<view class="form_device_add" @click="toChoiceDevice">
					<custom-icons iconsName="icon-tianjia" iconsSize="24rpx"
						iconsColor="#999999"></custom-icons>
					<view class="text">
						添加设备
					</view>
				</view>
			</view>
		</view>
		<uni-popup :ref="'devSelect'+keyId" :mask-click="false" background-color="#ffffff" :safe-area="true"
			mask-background-color="rgba(0, 0, 0, 0.5)" type="top" :zIndex="999">
			<view style="width: 100%;height: 100vh;position: relative;">
				<deviceSelect :ref="'devSel'+keyId" topTitle="选择设备" :isMulSelect="limit>1"
					:selected="_value&&_value.length>0?_value:[]" @closeSelect="closeSelect"
					@selectDevice="selectDevice" :ProductList="limit_product" :limit="limit"></deviceSelect>
			</view>
		</uni-popup>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";
	import deviceSelect from "@/components/device-select/device-select.vue"
	import {
		DeviceInfo,
		DeviceLiveInfo
	} from "@/api/device";
	import serverUrl from '@/common/constVar.js'
	export default {
		mixins: [componentMinxins],
		components: {
			deviceSelect
		},
		name: "DevicPicker",
		props: {
			value: {
				type: Array,
				default: () => {
					return [];
				}
			},
			limit_product: {
				type: Array,
				default: () => {
					return [];
				}
			},
			placeholder: {
				type: String,
				default: "请选择设备"
			},
			limit: {
				type: Number,
				default: 2
			},
			synclist: {
				type: Array,
				default: () => {
					return [];
				}
			},
			disabled: {
				default: false,
				type: Boolean
			},
			keyId: {
				type: [String, Number],
				default: ''
			},
		},
		data() {
			return {
				showOrgSelect: false
			};
		},
		mounted() {
			// console.log(this.value,this.limit,this.limit_product, "限制的产品传值");
			console.log(this._value,'_value');
		},
		methods: {
			returnDeviceImg(img){
				if(img&&img.indexOf('http')>-1||serverUrl.getServerUrl()=='/'){
					return img
				}else if(img){
					return serverUrl.getServerUrl()+img
				}
			},
			jumpDevDetail(item){
				uni.navigateTo({
					url: '/pages_device/device_info/device_info?id=' + item.id
				})
			},
			closeSelect(){
				this.$refs['devSelect'+this.keyId].close()
			},
			delSelDev(inx) {
				this._value.splice(inx, 1)
			},
			toChoiceDevice() {
				let selected = this._value ? this._value : []
				this.$refs['devSelect' + this.keyId].open()
				this.$nextTick(() => {
					this.$refs['devSel'+this.keyId].setSelected(selected)
				})
				// this.$emit('setActiveItem', this.keyId)
				// let select = ''
				// if (this._value) {
				// 	select = JSON.stringify(this._value)
				// } else {
				// 	select = JSON.stringify([])
				// }

				// let isMulDevice = this.limit > 1
				// if (isMulDevice) {
				// 	uni.navigateTo({
				// 		url: '/pages_device/select_list?selectDevice=' + select + '&isMulSelect=' + isMulDevice
				// 	})
				// } else {
				// 	uni.navigateTo({
				// 		url: '/pages_device/select_list?selectDevice=' + select
				// 	})
				// }
			},
			selectDevice(val) {
				//选中设备后
				// console.log("设备", val);
				this.closeSelect()
				if (val) {
					let arr = []
					val.map(row => {
						let obj ={}
						if(row.id&&row.type){
							obj = {
								id: row.id,
								name: row.name,
								photoUrl: row.photoUrl,
								selected: true,
								type: 'device',
								deviceNumber: row.deviceNumber
							}
						}else{
							obj = {
								id: row.Id,
								name: row.Name,
								photoUrl: row.PhotoUrl,
								selected: true,
								type: 'device',
								deviceNumber: row.DeviceNumber
							}
						}
						
						arr.push(obj)
					})
					this._value = arr
					// if(value.length>0){
					//   DeviceInfo({id:values[0].id}).then(rsp=>{
					//     if(this.$isNotEmpty(rsp.data.DeviceId)){
					//       return DeviceLiveInfo({ id: rsp.data.DeviceId,needTag:true });
					//     }
					//     else{
					//       return Promise.reject();
					//     }
					//   }).then(rsp=>{
					//     if(this.synclist==null)return;

					//     this.synclist.forEach(x=>{
					//       let tmpval=rsp.data.filter(z=>z.Code==x.code);
					//                 console.info(tmpval)
					//       if(tmpval.length>0){
					//         this.valueModel[x.field_id]=tmpval[0].Value;
					//       }

					//     });

					//   });
					// }
					this.$emit('setActiveItem', '')
					this.$forceUpdate()
				}

			},
		}
	};
</script>

<style lang="less" scoped>
	.placeholder {
		margin-left: 10px;
		color: #adabab;
		font-size: smaller;
	}

	.form_device_list.disabled_text {
		.form_device_li {
			background-color: #F8F8F8;
			border: 1rpx solid #F8F8F8;
			// opacity: 0.5;
		}
	}
</style>