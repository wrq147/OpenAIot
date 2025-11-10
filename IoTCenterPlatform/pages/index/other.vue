<template>
	<view style="width: 100%;height: 100vh;background-color: rgba(22, 26, 38, 1);">
		
	</view>
</template>

<script>
	export default {
		data() {
			return {
				
			}
		},
		onLoad() {
			this.$nextTick(()=>{
				uni.switchTab({
					url:'/pages/profile/profile'
				})
			})
		},
		methods: {
			
		}
	}
</script>

<style>

</style>
