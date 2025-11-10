<template>
	<view>
		<mz-editor :urlType='urlType' :datalist="infoStr" @Save="save"></mz-editor>
	</view>
</template>

<script>
	import {
		uploadFile
	} from '@/api/file.js'
	import {
		editCard
	} from '@/api/userCard.js'
	import {
		setPagesParam
	} from '@/common/util.js'
	export default {
		data() {
			return {
				infoStr: "",
				urlType: "",
			}
		},
		onLoad: function(options) {
			if (this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1].infoStr) {
				this.infoStr = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length -
					1].infoStr;

				// console.log("this.$store.state.submitMod.formArrary",this.$store.state.submitMod.formArrary);
				// console.log(this.infoStr, "3333339");
			}

			if (options) {
				if (options.urlType) {
					this.urlType = options.urlType;
				}
			}
		},
		onUnload: function() {
			// this.$store.state.submitMod.formArrary.pop();
		},
		methods: {
			save(liststr, cb) {
				uni.showLoading({
					title:"保存中"
				})
				// let tid = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1].Id;
				// liststr = JSON.parse(liststr);
				// console.log("8888", liststr, typeof liststr);
				setPagesParam('getIntro', liststr, 1, 1000);//传递内容详情参数并返回
				uni.hideLoading()
			}
		}
	}
</script>

<style lang="scss">

</style>
