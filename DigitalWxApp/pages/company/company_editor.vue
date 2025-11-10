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
		orgEditSave
	} from '@/api/company.js'
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
		},
		onUnload: function() {
			this.$store.state.submitMod.formArrary.pop();
		},
		methods: {
			save(liststr, cb) {
				let tid = this.$store.state.submitMod.formArrary[this.$store.state.submitMod.formArrary.length - 1].Id;
				uni.showLoading({
					title:'数据保存中'
				})
				orgEditSave({
					Id: tid,
					Intro: liststr
				}).then(rsp => {
					cb();
					reloadPrePage(1);
					uni.hideLoading();
					uni.navigateBack();
				});
			}
		}
	}
</script>

<style lang="scss">

</style>
