<template>
	<view>
		<mz-editor :datalist="introStr" @Save="save"></mz-editor>
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
		reloadPrePage
	} from '@/common/util.js'
	export default {
		data() {
			return {
				introStr: ""
			}
		},
		onLoad: function() {
			this.introStr = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1]
				.Intro;
		// console.log("个人介绍",this.introStr);
		},
		onUnload: function() {
			this.$store.state.submitMod.formArrary.pop();
		},
		methods: {
			save(liststr, cb) {
				let tid = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1].Id;
				// console.log("个人介绍2",tid);
				uni.showLoading({
					title:'数据保存中'
				})
				editCard({
					Id: tid,
					Intro: liststr
				}).then(rsp => {
					cb();
					reloadPrePage(1, "intro");
					uni.hideLoading();
					uni.navigateBack();
				});
			}
		}
	}
</script>

<style lang="scss">

</style>
