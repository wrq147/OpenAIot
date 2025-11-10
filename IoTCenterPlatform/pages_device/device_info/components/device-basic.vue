<template>
	<view>
		<view class="basic_info_list" v-for="(val,key) in deviceBasicInfoObj" v-if="val.value&&val.value.length>0">
			<view class="line_title">
				<view class="left_text">
					<text>{{val.name}}</text>
				</view>
				<view class="line"></view>
			</view>
			<view class="basic_info_li basic_flex" v-for="row in val.value"
				v-if="val.value&&val.value.length>0&&row.DisplayValue">
				<view class="flex_left">
					<view class="li_label">{{row.Name}}</view>
					<view class="li_val">{{row.DisplayValue}}</view>
				</view>
				<view class="flex_right"
					v-if="val.type==2&&row.MapCode&&row.Option.type == 'date' ||val.type==2&&row.MapCode&&row.Option.type == 'float' ||val.type==2&&row.MapCode&&row.Option.type == 'int'"
					@click.stop="toHistoryIot(row)">
					<custom-icons iconsName="icon-lishishuju" iconsSize="36rpx" iconsColor="#FFFFFF"></custom-icons>
				</view>
			</view>
		</view>
		<view class="empty_con" v-if="!deviceBasicInfoObj.basic.name&&!deviceBasicInfoObj.Tag.name">
			<image class="image" src="/static/no_data.png" mode=""></image>
			<view class="text">There is currently no data available!</view>
		</view>
	</view>
</template>

<script>
	export default {
		name: "device-basic",
		props: {
			deviceBasicInfoObj: {
				type: Object,
				default: () => {
					return {}
				}
			},
			deviceBasic: {
				type: Object,
				default: ''
			}
		},
		data() {
			return {

			};
		},
		watch: {},
		mounted() {},
		methods: {
			toHistoryIot(item) {
				//跳转至历史数据
				console.log("item", item);
				let itemObj = {
					Code: item.MapCode,
					Name: item.Name,
					OptionType: item.Option.type,
					Unit: item.Option.unit
				}
				if (itemObj.OptionType == 'date' || itemObj.OptionType == 'float' || itemObj.OptionType == 'int') {
					uni.navigateTo({
						url: '/pages_device/device_info/iot_history?deviceId=' + this.deviceBasic.Id +
							'&deviceNu=' + this.deviceBasic.DeviceId + '&activeAttr=' + JSON.stringify(itemObj)
					})
				}
			},
		}
	}
</script>

<style lang="scss" scoped>
	.basic_info_list {
		.basic_info_li.basic_flex {
			display: flex;
			justify-content: space-between;
			align-items: center;
		}
	}
</style>