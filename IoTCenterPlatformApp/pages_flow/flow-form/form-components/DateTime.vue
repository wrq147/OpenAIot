<template>
	<view style="width: 100%;">
		<view style="width: 100%;" v-if="disabled&&value&&value!=null">
			<view class="dis_text">
				<text>{{_value}}</text>
			</view>
		</view>
		<view style="width: 100%;" v-else>
			<view class="int_date">
				<view class="date_con long_date" style="color: #333;">
					<uni-datetime-picker ref="dateChoice1" class="date" :type="type"
						placeholder-style="font-size:32rpx;color:#999999" :clear-icon="false" v-model="_value"
						:placeholder='placeholder' :isCustom="true" :isDark="false" :return-type="format"
						:disabled="disabled" :border="disabled">
						<view class="date_slot" :class="{'has_val':_value,'disabled_text':disabled}">
							<custom-icons iconsName="icon-xuanzeshijian" iconsSize="28rpx"
								:iconsColor="disabled?'rgba(153, 153, 153, 1)':'rgba(193, 193, 193, 1)'"></custom-icons>
							<view class="text">
								{{_value?_value:'选择时间'}}
							</view>
						</view>
					</uni-datetime-picker>
				</view>
			</view>
		</view>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";

	export default {
		mixins: [componentMinxins],
		name: "DateTime",
		components: {},
		props: {
			value: {
				type:String,
				default: null,
			},
			format: {
				type: String,
				default: "yyyy-MM-dd HH:mm",
			},
			placeholder: {
				type: String,
				default: "请选择日期时间",
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		computed: {
			type() {
				switch (this.format) {
					case "yyyy":
						return "year";
					case "yyyy-MM":
						return "month";
					case "yyyy-MM-dd":
						return "date";
					case "yyyy-MM-dd HH:mm":
						return "datetime";
					default:
						return "datetime";
				}
			},
		},
		data() {
			return {};
		},

		methods: {},
	};
</script>

<style scoped>

</style>