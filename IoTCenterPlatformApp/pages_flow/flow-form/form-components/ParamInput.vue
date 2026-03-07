<template>
	<view style="width: 100%;">
		<!-- <el-button type="info" icon="el-icon-link" @click="showRForm" round>关联的{{formType}}</el-button> -->
		<view class="view_input disabled_view">
			<view class="pal_col">
				{{formType?formType:'请选择关联表单'}}
			</view>
			<view class="form_sel_icon">
				<custom-icons iconsName="icon-a-youjiantoubai" iconsSize="20rpx"
					iconsColor="#999999"></custom-icons>
			</view>
		</view>
	</view>
</template>

<script>
	import componentMinxins from "../ComponentMinxins";

	export default {
		mixins: [componentMinxins],
		name: "ParamInput",
		components: {},
		props: {
			value: {
				type: String,
				default: null,
			},
			formType: {
				type: String,
				default: "",
			},
			formId: {
				type: String,
				default: "",
			},
			disabled: {
				default: false,
				type: Boolean,
			},
		},
		data() {
			return {
				targetId: undefined
			};
		},
		beforeMount() {
			if(!this.value||this.value==null){
				this._value='a'
			}
		},
		methods: {
			showRForm() {
				if (this.mode != 'DESIGN') {
					var tmpid = this.valueModel[this.formId];
				}

			}
		},
	};
</script>

<style scoped>
</style>